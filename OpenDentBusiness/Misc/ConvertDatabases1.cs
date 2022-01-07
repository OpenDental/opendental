using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Design;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Resources;
using System.Text; 
using CodeBase;
using DataConnectionBase;

namespace OpenDentBusiness{

	///<summary></summary>
	public partial class ConvertDatabases{
		public static System.Version FromVersion;
		//private System.Version ToVersion;

		/// <summary>Takes a text file with a series of SQL commands, and sends them as queries to the database.  Used in version upgrades until version 4.4.  But the ConversionFiles folder was a constant source of frustration for new programmers, so it was decided to deprecate the use of the "ConversionFiles" folder.  Since then, all SQL commands needed for upgrades have been included in the source code instead.  The filename is always relative to the application directory.  Throws an exception if it fails.  Due to spotty support for batch commands in the MySQL connector, this function is no longer used.</summary>
		public static void ExecuteFile(string fileName) {
			string path = Path.Combine("ConversionFiles", fileName);
			string content = File.ReadAllText(path).Trim();
			Db.NonQ32(content);
		}

		public static void To2_8_2() {
			if(FromVersion < new Version("2.8.2.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 2.8.2");//No translation in convert script.
				string[] commands=new string[]
				{
					"ALTER TABLE insplan DROP TemplateNum"
					,"DROP TABLE instemplate"
					,"UPDATE preference SET ValueString = '2.8.2.0' WHERE PrefName = 'DataBaseVersion'"
				};
				Db.NonQ32(commands);
			}
			To2_8_3();
		}

		private static void To2_8_3() {
			if(FromVersion < new Version("2.8.3.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 2.8.3");//No translation in convert script.
				string[] commands=new string[]
				{
					"INSERT INTO preference VALUES ('RenaissanceLastBatchNumber','0')"
					,"INSERT INTO preference VALUES ('PatientSelectUsesSearchButton','0')"
					,"UPDATE preference SET ValueString = '2.8.3.0' WHERE PrefName = 'DataBaseVersion'"
				};
				Db.NonQ32(commands);
			}
			To2_8_6();
		}

		private static void To2_8_6() {
			if(FromVersion < new Version("2.8.6.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 2.8.6");//No translation in convert script.
				string[] commands=new string[]
				{
					"ALTER TABLE patient CHANGE City City VARCHAR(100) NOT NULL"
					,"ALTER TABLE patient CHANGE State State VARCHAR(100) NOT NULL"
					,"ALTER TABLE patient CHANGE Zip Zip VARCHAR(100) NOT NULL"
					,"ALTER TABLE patient CHANGE SSN SSN VARCHAR(100) NOT NULL"
					,"UPDATE preference SET ValueString = '2.8.6.0' WHERE PrefName = 'DataBaseVersion'"
				};
				Db.NonQ32(commands);
			}
			To2_8_10();
		}

		private static void To2_8_10() {
			if(FromVersion < new Version("2.8.10.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 2.8.10");//No translation in convert script.
				string[] commands=new string[]
				{
					"ALTER TABLE employer ADD Address varchar(255) NOT NULL"
					,"ALTER TABLE employer ADD Address2 varchar(255) NOT NULL"
					,"ALTER TABLE employer ADD City varchar(255) NOT NULL"
					,"ALTER TABLE employer ADD State varchar(255) NOT NULL"
					,"ALTER TABLE employer ADD Zip varchar(255) NOT NULL"
					,"ALTER TABLE employer ADD Phone varchar(255) NOT NULL"
					,"INSERT INTO preference VALUES ('CustomizedForPracticeWeb','0')"
					,"UPDATE preference SET ValueString = '2.8.10.0' WHERE PrefName = 'DataBaseVersion'"
				};
				Db.NonQ32(commands);
			}
			To2_8_14();
		}

		private static void To2_8_14() {
			if(FromVersion < new Version("2.8.14.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 2.8.14");//No translation in convert script.
				string[] commands=new string[]
				{
					"ALTER TABLE adjustment CHANGE AdjType AdjType smallint unsigned NOT NULL"
					,"ALTER TABLE appointment CHANGE Confirmed Confirmed smallint unsigned NOT NULL"
					,"ALTER TABLE payment CHANGE PayType PayType smallint unsigned NOT NULL"
					,"ALTER TABLE procedurecode CHANGE ProcCat ProcCat smallint unsigned NOT NULL"
					,"ALTER TABLE procedurelog CHANGE Priority Priority smallint unsigned NOT NULL"
					,"ALTER TABLE procedurelog CHANGE Dx Dx smallint unsigned NOT NULL"
					,"UPDATE preference SET ValueString = '2.8.14.0' WHERE PrefName = 'DataBaseVersion'"
				};
				Db.NonQ32(commands);
			}
			To2_9_1();
		}

		private static void To2_9_1() {
			if(FromVersion < new Version("2.9.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 2.9.1");//No translation in convert script.
				ExecuteFile("convert_2_9_1.txt");//might throw an exception which we handle.
				string[] commands=new string[]
				{
					"UPDATE preference SET ValueString = '2.9.1.0' WHERE PrefName = 'DataBaseVersion'"
				};
				Db.NonQ32(commands);
			}
			To2_9_2();
		}

		private static void To2_9_2() {
			if(FromVersion < new Version("2.9.2.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 2.9.2");//No translation in convert script.
				string[] commands=new string[]
				{
					"ALTER TABLE patient ADD PriPending tinyint(1) unsigned NOT NULL"
					,"ALTER TABLE patient ADD SecPending tinyint(1) unsigned NOT NULL"
					,"ALTER TABLE appointment ADD Assistant smallint unsigned NOT NULL"
					,"UPDATE preference SET ValueString = '2.9.2.0' WHERE PrefName = 'DataBaseVersion'"
				};
				Db.NonQ32(commands);
			}
			To2_9_5();
		}

		private static void To2_9_5() {
			if(FromVersion < new Version("2.9.5.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 2.9.5");//No translation in convert script.
				string[] commands=new string[]
				{
					"ALTER TABLE autocode ADD LessIntrusive tinyint(1) unsigned NOT NULL"
					,"UPDATE preference SET ValueString = '2.9.5.0' WHERE PrefName = 'DataBaseVersion'"
				};
				Db.NonQ32(commands);
			}
			To2_9_8();
		}

		private static void To2_9_8() {
			if(FromVersion < new Version("2.9.8.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 2.9.8");//No translation in convert script.
				string claimFormNum;
				//Change the PlaceNumericCode field for both HCFA forms
				string command="SELECT ClaimFormNum FROM claimform WHERE UniqueID = '4'";
				DataTable table=Db.GetTable(command);
				string[] commands;
				if(table.Rows.Count>0) {
					claimFormNum=table.Rows[0][0].ToString();
					commands=new string[]
					{
						"UPDATE claimformitem SET FieldName='P1PlaceNumericCode' "
						+"WHERE FieldName='PlaceNumericCode' && ClaimFormNum='"+claimFormNum+"' "
						+"&& YPos='751'"
						,"UPDATE claimformitem SET FieldName='P2PlaceNumericCode' "
						+"WHERE FieldName='PlaceNumericCode' && ClaimFormNum='"+claimFormNum+"' "
						+"&& YPos='784'"
						,"UPDATE claimformitem SET FieldName='P3PlaceNumericCode' "
						+"WHERE FieldName='PlaceNumericCode' && ClaimFormNum='"+claimFormNum+"' "
						+"&& YPos='817'"
						,"UPDATE claimformitem SET FieldName='P4PlaceNumericCode' "
						+"WHERE FieldName='PlaceNumericCode' && ClaimFormNum='"+claimFormNum+"' "
						+"&& YPos='850'"
						,"UPDATE claimformitem SET FieldName='P5PlaceNumericCode' "
						+"WHERE FieldName='PlaceNumericCode' && ClaimFormNum='"+claimFormNum+"' "
						+"&& YPos='884'"
						,"UPDATE claimformitem SET FieldName='P6PlaceNumericCode' "
						+"WHERE FieldName='PlaceNumericCode' && ClaimFormNum='"+claimFormNum+"' "
						+"&& YPos='917'"
					};
					Db.NonQ32(commands);
				}
				command="SELECT ClaimFormNum FROM claimform WHERE UniqueID = '5'";
				table=Db.GetTable(command);
				if(table.Rows.Count>0) {
					claimFormNum=table.Rows[0][0].ToString();
					commands=new string[]
					{
						"UPDATE claimformitem SET FieldName='P1PlaceNumericCode' "
						+"WHERE FieldName='PlaceNumericCode' && ClaimFormNum='"+claimFormNum+"' "
						+"&& YPos='751'"
						,"UPDATE claimformitem SET FieldName='P2PlaceNumericCode' "
						+"WHERE FieldName='PlaceNumericCode' && ClaimFormNum='"+claimFormNum+"' "
						+"&& YPos='784'"
						,"UPDATE claimformitem SET FieldName='P3PlaceNumericCode' "
						+"WHERE FieldName='PlaceNumericCode' && ClaimFormNum='"+claimFormNum+"' "
						+"&& YPos='817'"
						,"UPDATE claimformitem SET FieldName='P4PlaceNumericCode' "
						+"WHERE FieldName='PlaceNumericCode' && ClaimFormNum='"+claimFormNum+"' "
						+"&& YPos='850'"
						,"UPDATE claimformitem SET FieldName='P5PlaceNumericCode' "
						+"WHERE FieldName='PlaceNumericCode' && ClaimFormNum='"+claimFormNum+"' "
						+"&& YPos='884'"
						,"UPDATE claimformitem SET FieldName='P6PlaceNumericCode' "
						+"WHERE FieldName='PlaceNumericCode' && ClaimFormNum='"+claimFormNum+"' "
						+"&& YPos='917'"
					};
					Db.NonQ32(commands);
				}
				//ADA2002 medicaid id's
				command="SELECT ClaimFormNum FROM claimform WHERE UniqueID = '1'";
				table=Db.GetTable(command);
				if(table.Rows.Count>0) {
					claimFormNum=table.Rows[0][0].ToString();
					commands=new string[]
					{
						"INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+"'"+claimFormNum+"','TreatingDentistMedicaidID','492','946','117','14')"
						,"INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+"'"+claimFormNum+"','BillingDentistMedicaidID','39','990','120','14')"
					};
					Db.NonQ32(commands);
				}
				//ADA2000 employer and 3 radiograph fields.
				command="SELECT ClaimFormNum FROM claimform WHERE UniqueID = '3'";
				table=Db.GetTable(command);
				if(table.Rows.Count>0) {
					claimFormNum=table.Rows[0][0].ToString();
					commands=new string[]
					{
						"INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+"'"+claimFormNum+"','EmployerName','482','391','140','14')"
						,"INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+"'"+claimFormNum+"','IsRadiographsAttached','388','548','0','0')"
						,"INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+"'"+claimFormNum+"','RadiographsNotAttached','495','547','0','0')"
						,"INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+"'"+claimFormNum+"','RadiographsNumAttached','460','545','35','14')"
					};
					Db.NonQ32(commands);
				}
				commands=new string[]
				{
					"UPDATE preference SET ValueString = '2.9.8.0' WHERE PrefName = 'DataBaseVersion'"
				};
				Db.NonQ32(commands);
			}
			To3_0_1();
		}

		///<summary>Used by To3_0_1. IMPORTANT: remember that this method alters TableQ.</summary>
		private static int GetPercent(int patNum,int priPlanNum,int secPlanNum,string procCode,string priORsec) {
			//command="SELECT 
			//get the covCatNum for this Code
			string command="SELECT CovCatNum FROM covspan "
				+"WHERE '"+POut.String(procCode)+"' > FromCode "
				+"AND '"+POut.String(procCode)+"' < ToCode";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0) {
				return 0;//this code is not in any category, so coverage=0
			}
			int covCatNum=PIn.Int(table.Rows[0][0].ToString());
			command="SELECT PlanNum,PriPatNum,SecPatNum,Percent FROM covpat WHERE "
				+"CovCatNum = '"+covCatNum.ToString()+"' "
				+"AND (PlanNum = '"+priPlanNum.ToString()+"' "
				+"OR PlanNum = '"+secPlanNum.ToString()+"' "
				+"OR PriPatNum = '"+patNum.ToString()+"' "
				+"OR SecPatNum = '"+patNum.ToString()+"')";
			table=Db.GetTable(command);
			if(table.Rows.Count==0) {
				return 0;//no percentages have been entered for this patient or plan
			}
			for(int i=0;i<table.Rows.Count;i++) {
				//first handle the patient overrides
				if(priORsec=="pri" && PIn.Long(table.Rows[i][1].ToString())==patNum) {
					return PIn.Int(table.Rows[i][3].ToString());
				}
				if(priORsec=="sec" && PIn.Long(table.Rows[i][2].ToString())==patNum) {
					return PIn.Int(table.Rows[i][3].ToString());
				}
				//then handle the percentages attached to plans(much more common)
				if(priORsec=="pri" && PIn.Long(table.Rows[i][0].ToString())==priPlanNum) {
					return PIn.Int(table.Rows[i][3].ToString());
				}
				if(priORsec=="sec" && PIn.Long(table.Rows[i][0].ToString())==secPlanNum) {
					return PIn.Int(table.Rows[i][3].ToString());
				}
			}
			return 0;
		}

		private static void To3_0_1() {
			if(FromVersion < new Version("3.0.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.0.1");//No translation in convert script.
				ExecuteFile("convert_3_0_1.txt");//might throw an exception which we handle.
				//convert appointment patterns from ten minute to five minute intervals---------------------
				string command="SELECT AptNum,Pattern FROM appointment";
				DataTable table=Db.GetTable(command);
				StringBuilder sb;
				string pattern;
				for(int i=0;i<table.Rows.Count;i++) {
					pattern=PIn.String(table.Rows[i][1].ToString());
					sb=new StringBuilder();
					for(int j=0;j<pattern.Length;j++) {
						sb.Append(pattern.Substring(j,1));
						sb.Append(pattern.Substring(j,1));
					}
					command="UPDATE appointment SET "
						+"Pattern='"+POut.String(sb.ToString())+"' "
						+"WHERE AptNum='"+table.Rows[i][0].ToString()+"'";
					Db.NonQ32(command);
				}
				//add the default 5 Elements to each ApptView-----------------------------------------------
				command="SELECT ApptViewNum FROM apptview";
				table=Db.GetTable(command);
				string[] commands;
				for(int i=0;i<table.Rows.Count;i++) {
					commands=new string[]
					{
						"INSERT INTO apptviewitem(ApptViewNum,ElementDesc,ElementOrder,ElementColor) "
							+"VALUES('"+table.Rows[i][0].ToString()+"','PatientName','0','-16777216')"
						,"INSERT INTO apptviewitem(ApptViewNum,ElementDesc,ElementOrder,ElementColor) "
							+"VALUES('"+table.Rows[i][0].ToString()+"','Lab','1','-65536')"
						,"INSERT INTO apptviewitem(ApptViewNum,ElementDesc,ElementOrder,ElementColor) "
							+"VALUES('"+table.Rows[i][0].ToString()+"','Procs','2','-16777216')"
						,"INSERT INTO apptviewitem(ApptViewNum,ElementDesc,ElementOrder,ElementColor) "
							+"VALUES('"+table.Rows[i][0].ToString()+"','Note','3','-16777216')"
						,"INSERT INTO apptviewitem(ApptViewNum,ElementDesc,ElementOrder,ElementColor) "
							+"VALUES('"+table.Rows[i][0].ToString()+"','Production','4','-16777216')"
					};
					Db.NonQ32(commands);
				}
				//MessageBox.Show("Appointments converted.");
				//Any claimprocs attached to claims with ins being Cap, should be CapClaim, even if paid
				command="SELECT claimproc.ClaimProcNum FROM claimproc,insplan "
					+"WHERE claimproc.PlanNum=insplan.PlanNum "
					+"AND claimproc.ClaimNum != '0' "
					+"AND insplan.PlanType='c'";
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="UPDATE claimproc SET Status='5' "//CapClaim
						+"WHERE ClaimProcNum='"+table.Rows[i][0].ToString()+"'";
					Db.NonQ32(command);
				}
				//edit any existing claimprocs-------------------------------------------------------------
				//These are all associated with claims, but we are not changing the claim values,
				//just some of the estimates.  None of these changes affect any claim or balance.
				//Ignore any status=CapClaim since these are all duplicates just for sending the claim
				//Add percentages etc from procedure
				int percentage=0;
				int planNum=0;
				double baseEst=0;
				double overrideAmt=0;
				DataTable procTable;
				command="SELECT claimproc.ClaimProcNum,patient.PriPlanNum,"//0,1
					+"patient.SecPlanNum,patient.PatNum,claimproc.PlanNum,procedurelog.ADACode,"//2,3,4,5
					+"procedurelog.OverridePri,procedurelog.OverrideSec,procedurelog.ProcFee "//6,7,8
					+"FROM claimproc,procedurelog,patient "
					//this next line ignores any claimprocs not attached to a proc. so skips adjustments.
					+"WHERE claimproc.ProcNum=procedurelog.ProcNum "
					+"AND patient.PatNum=procedurelog.PatNum "
					+"AND claimproc.Status != 2 "//skips preauths
					+"AND claimproc.Status != 4 "//skips supplemental
					+"AND claimproc.Status != 5 ";//skips capClaim
				table=Db.GetTable(command);
				procTable=table.Copy();//so that we can perform other queries
				for(int i=0;i<procTable.Rows.Count;i++) {
					planNum=PIn.Int(procTable.Rows[i][4].ToString());//claimproc.PlanNum
					//if primary
					if(planNum==PIn.Long(procTable.Rows[i][1].ToString())) {//priPlanNum
						percentage=GetPercent(PIn.Int(procTable.Rows[i][3].ToString()),//patNum
							PIn.Int(procTable.Rows[i][1].ToString()),//priPlanNum
							PIn.Int(procTable.Rows[i][2].ToString()),//secPlanNum
							PIn.String(procTable.Rows[i][5].ToString()),//ADACode
							"pri");
						overrideAmt=PIn.Double(procTable.Rows[i][6].ToString());
					}
					//else if secondary
					else if(planNum==PIn.Long(procTable.Rows[i][2].ToString())) {//priPlanNum
						percentage=GetPercent(PIn.Int(procTable.Rows[i][3].ToString()),//patNum
							PIn.Int(procTable.Rows[i][1].ToString()),//priPlanNum
							PIn.Int(procTable.Rows[i][2].ToString()),//secPlanNum
							PIn.String(procTable.Rows[i][5].ToString()),//ADACode
							"sec");
						overrideAmt=PIn.Double(procTable.Rows[i][7].ToString());
					}
					else {
						//plan is neither pri or sec, so disregard
						continue;
					}
					//fee x percentage:
					baseEst=PIn.Double(procTable.Rows[i][8].ToString())*(double)percentage/100;
					command="UPDATE claimproc SET "
						//+"Allowed Amt='-1',"
						+"Percentage='"+percentage.ToString()+"',"
						//+"PercentOverride='-1',"
						//+"CopayAmt='-1',"
						+"OverrideInsEst='"+overrideAmt.ToString()+"',"
						//+"OverAnnualMax='-1',"
						//+"PaidOtherIns='-1',"
						+"BaseEst='"+baseEst.ToString()+"'"
						//+"CopayOverride='-1'"
						+" WHERE ClaimProcNum='"+procTable.Rows[i][0].ToString()+"'";
					//MessageBox.Show(command);
					Db.NonQ32(command);
				}
				//convert all estimates into claimprocs-------------------------------------------------
				command="SELECT procedurelog.ProcNum,procedurelog.PatNum,"//0,1
					+"procedurelog.ProvNum,patient.PriPlanNum,patient.SecPlanNum,"//2,3,4
					//+"claimproc.ClaimProcNum,
					+"procedurelog.ADACode,procedurelog.ProcDate,"//5,6
					+"procedurelog.OverridePri,procedurelog.OverrideSec,procedurelog.NoBillIns,"//7,8,9
					+"procedurelog.CapCoPay,procedurelog.ProcStatus,procedurelog.ProcFee,"//10,11,12
					+"insplan.PlanType, '' as Dummy "//13
					+"FROM procedurelog,patient,insplan "
					//+"LEFT JOIN claimproc ON claimproc.ProcNum=procedurelog.ProcNum "//only interested in NULL
					+"WHERE procedurelog.PatNum=patient.PatNum "
					//this is to test for capitation. It also limits results to patients with insurance.
					+"AND patient.PriPlanNum=insplan.PlanNum "
					//+"AND patient.PriPlanNum > 0 "//only patients with insurance
					+"AND (procedurelog.ProcStatus=1 "//status TP
					+"OR procedurelog.ProcStatus=2) "//status C
					+"AND NOT EXISTS (SELECT * FROM claimproc WHERE claimproc.ProcNum=procedurelog.ProcNum)";
				//claimproc.ClaimProcNum IS NULL "//only if not already attached to a claim
				//+"OR claimproc.Status='5')";//or CapClaim
				table=Db.GetTable(command);
				procTable=table.Copy();//so that we can perform other queries
				int status=0;
				double copay=0;
				double writeoff=0;
				for(int i=0;i<procTable.Rows.Count;i++) {//loop procedures
					//1. noBillIns
					if(PIn.Bool(procTable.Rows[i][10].ToString())//if noBillIns
						&& PIn.Double(procTable.Rows[i][11].ToString()) ==-1) {//and not a cap procedure
						//primary
						if(PIn.Long(procTable.Rows[i][3].ToString())!=0) {//if has pri ins
							command="INSERT INTO claimproc(ProcNum,PatNum,ProvNum,Status,PlanNum,"
								+"DateCP,AllowedAmt,Percentage,PercentOverride,CopayAmt,OverrideInsEst,"
								+"NoBillIns,OverAnnualMax,PaidOtherIns) "
								+"VALUES ("
								+"'"+procTable.Rows[i][0].ToString()+"',"//procnum
								+"'"+procTable.Rows[i][1].ToString()+"',"//patnum
								+"'"+procTable.Rows[i][2].ToString()+"',"//provnum
								+"'6',"//status:Estimate
								+"'"+procTable.Rows[i][3].ToString()+"',"//priPlanNum
								+POut.Date(PIn.Date(procTable.Rows[i][7].ToString()))+","//dateCP
								//these -1's are unnecessary, but I already added them, so they are here.
								+"'-1',"//allowed amt
								+"'-1',"//percentage
								+"'-1',"//percentoverride
								+"'-1',"//copayamt
								+"'-1',"//overrideInsEst
								+"'1',"//NoBillIns,
								+"'-1',"//OverAnnualMax
								+"'-1'"//PaidOtherIns
								+")";
							Db.NonQ32(command);
						}
						//secondary
						if(PIn.Long(procTable.Rows[i][4].ToString())!=0) {//if has sec ins
							command="INSERT INTO claimproc(ProcNum,PatNum,ProvNum,Status,PlanNum,"
								+"DateCP,AllowedAmt,Percentage,PercentOverride,CopayAmt,OverrideInsEst,"
								+"NoBillIns,OverAnnualMax,PaidOtherIns) "
								+"VALUES ("
								+"'"+procTable.Rows[i][0].ToString()+"',"//procnum
								+"'"+procTable.Rows[i][1].ToString()+"',"//patnum
								+"'"+procTable.Rows[i][2].ToString()+"',"//provnum
								+"'6',"//status:Estimate
								+"'"+procTable.Rows[i][4].ToString()+"',"//secPlanNum
								+POut.Date(PIn.Date(procTable.Rows[i][7].ToString()))+","//dateCP
								+"'-1',"//allowed amt
								+"'-1',"//percentage
								+"'-1',"//percentoverride
								+"'-1',"//copayamt
								+"'-1',"//overrideInsEst
								+"'1',"//NoBillIns,
								+"'-1',"//OverAnnualMax
								+"'-1'"//PaidOtherIns
								+")";
							Db.NonQ32(command);
						}
						continue;
					}//1. noBillIns
					//2. capitation. Always primary. If C, then affects aging via CapComplete.
					//Never attached to claim.
					copay=PIn.Double(procTable.Rows[i][11].ToString());
					//if CapCoPay not -1, and priIns is cap, then this is a cap proc
					if(copay!=-1 && PIn.String(procTable.Rows[i][14].ToString())=="c") {
						if(PIn.Long(procTable.Rows[i][12].ToString())==1) {//proc status =tp
							status=8;//claimProc status=CapEstimate
						}
						if(PIn.Long(procTable.Rows[i][12].ToString())==2) {//proc status =c
							status=7;//claimProc status=CapComplete
						}
						writeoff=PIn.Double(procTable.Rows[i][13].ToString())//procFee
							-copay;
						command="INSERT INTO claimproc(ProcNum,PatNum,ProvNum,"
							+"Status,PlanNum,DateCP,WriteOff,AllowedAmt,Percentage,PercentOverride,"
							+"CopayAmt,OverrideInsEst,OverAnnualMax,PaidOtherIns,NoBillIns) "
							+"VALUES ("
							+"'"+procTable.Rows[i][0].ToString()+"',"//procnum
							+"'"+procTable.Rows[i][1].ToString()+"',"//patnum
							+"'"+procTable.Rows[i][2].ToString()+"',"//provnum
							+"'"+status.ToString()+"',"//status
							+"'"+procTable.Rows[i][3].ToString()+"',"//priPlanNum
							+POut.Date(PIn.Date(procTable.Rows[i][7].ToString()))+","//dateCP
							+"'"+writeoff.ToString()+"',"//writeoff
							+"'-1',"//allowed amt
							+"'-1',"//percentage
							+"'-1',"//percentoverride
							+"'"+copay.ToString()+"',"//copayamt
							+"'-1',"//overrideInsEst
							+"'-1',"//OverAnnualMax
							+"'-1',"//PaidOtherIns
							+"'"+procTable.Rows[i][10].ToString()+"'"//noBillIns is allowed for cap
							+")";
						Db.NonQ32(command);
						continue;
					}
					//3. standard primary estimate:
					//always a primary estimate because original query excluded patients with no ins.
					planNum=PIn.Int(procTable.Rows[i][3].ToString());//priPlanNum
					percentage=GetPercent(PIn.Int(procTable.Rows[i][1].ToString()),//patNum
						PIn.Int(procTable.Rows[i][3].ToString()),//priPlanNum
						PIn.Int(procTable.Rows[i][4].ToString()),//secPlanNum
						PIn.String(procTable.Rows[i][6].ToString()),//ADACode
						"pri");
					baseEst=PIn.Double(procTable.Rows[i][13].ToString())*(double)percentage/100;
					command="INSERT INTO claimproc(ProcNum,PatNum,ProvNum,"
						+"Status,PlanNum,DateCP,WriteOff,AllowedAmt,Percentage,PercentOverride,"
						+"CopayAmt,OverrideInsEst,NoBillIns,OverAnnualMax,PaidOtherIns,BaseEst) "
						+"VALUES ("
						+"'"+procTable.Rows[i][0].ToString()+"',"//procnum
						+"'"+procTable.Rows[i][1].ToString()+"',"//patnum
						+"'"+procTable.Rows[i][2].ToString()+"',"//provnum
						+"'6',"//status:Estimate
						+"'"+planNum.ToString()+"',"//plannum
						+POut.Date(PIn.Date(procTable.Rows[i][7].ToString()))+","//dateCP
						+"'0',"//writeoff
						+"'-1',"//allowed amt
						+"'"+percentage.ToString()+"',"//percentage
						+"'-1',"//percentoverride
						+"'-1',"//copayamt
						+"'"+procTable.Rows[i][8].ToString()+"',"//overrideInsEst-pri
						+"'0',"//NoBillIns,
						+"'-1',"//OverAnnualMax
						+"'-1',"//PaidOtherIns
						+"'"+baseEst.ToString()+"'"//BaseEst
						+")";
					Db.NonQ32(command);
					//4. standard secondary estimate
					//secondary can be in addition to primary, or not at all
					planNum=PIn.Int(procTable.Rows[i][4].ToString());//secPlanNum
					if(planNum==0) {
						continue;
					}
					percentage=GetPercent(PIn.Int(procTable.Rows[i][1].ToString()),//patNum
						PIn.Int(procTable.Rows[i][3].ToString()),//priPlanNum
						PIn.Int(procTable.Rows[i][4].ToString()),//secPlanNum
						PIn.String(procTable.Rows[i][6].ToString()),//ADACode
						"sec");
					baseEst=PIn.Double(procTable.Rows[i][13].ToString())*(double)percentage/100;
					command="INSERT INTO claimproc(ProcNum,PatNum,ProvNum,"
						+"Status,PlanNum,DateCP,WriteOff,AllowedAmt,Percentage,PercentOverride,"
						+"CopayAmt,OverrideInsEst,NoBillIns,OverAnnualMax,PaidOtherIns,BaseEst) "
						+"VALUES ("
						+"'"+procTable.Rows[i][0].ToString()+"',"//procnum
						+"'"+procTable.Rows[i][1].ToString()+"',"//patnum
						+"'"+procTable.Rows[i][2].ToString()+"',"//provnum
						+"'6',"//status:Estimate
						+"'"+planNum.ToString()+"',"//plannum
						+POut.Date(PIn.Date(procTable.Rows[i][7].ToString()))+","//dateCP
						+"'0',"//writeoff
						+"'-1',"//allowed amt
						+"'"+percentage.ToString()+"',"//percentage
						+"'-1',"//percentoverride
						+"'-1',"//copayamt
						+"'"+procTable.Rows[i][9].ToString()+"',"//overrideInsEst-pri
						+"'0',"//NoBillIns,
						+"'-1',"//OverAnnualMax
						+"'-1',"//PaidOtherIns
						+"'"+baseEst.ToString()+"'"//BaseEst
						+")";
					Db.NonQ32(command);
				}//loop procedures
				command="UPDATE claimproc SET ProcDate=DateCP";//affects ALL patients
				Db.NonQ32(command);
				//MessageBox.Show("Procedure percentages converted to claimprocs.");
				commands=new string[]
				{
					"UPDATE procedurelog SET OverridePri='0',OverrideSec='0',NoBillIns='0',"
						+"IsCovIns='0',CapCoPay='0'"
				};
				Db.NonQ32(commands);
				//convert medical/service notes from defs table to quickpaste notes----------------------
				commands=new string[]
				{
					"INSERT INTO quickpastecat "
						+"VALUES ('1','Medical Urgent','0','22')"
					,"INSERT INTO quickpastecat "
						+"VALUES ('2','Medical Summary','1','9')"
					,"INSERT INTO quickpastecat "
						+"VALUES ('3','Service Notes','2','10')"
					,"INSERT INTO quickpastecat "
						+"VALUES ('4','Medical History','3','11')"
				};
				Db.NonQ32(commands);
				command="SELECT * FROM definition WHERE Category='8'";//Medical Notes
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="INSERT INTO quickpastenote (QuickPasteCatNum,ItemOrder,Note) "
						+"VALUES ('1','"+i.ToString()+"','"
						+POut.String(table.Rows[i][3].ToString())+"')";
					Db.NonQ32(command);
					command="INSERT INTO quickpastenote (QuickPasteCatNum,ItemOrder,Note) "
						+"VALUES ('2','"+i.ToString()+"','"
						+POut.String(table.Rows[i][3].ToString())+"')";
					Db.NonQ32(command);
					command="INSERT INTO quickpastenote (QuickPasteCatNum,ItemOrder,Note) "
						+"VALUES ('4','"+i.ToString()+"','"
						+POut.String(table.Rows[i][3].ToString())+"')";
					Db.NonQ32(command);
				}
				command="SELECT * FROM definition WHERE Category='14'";//Service Notes
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="INSERT INTO quickpastenote (QuickPasteCatNum,ItemOrder,Note) "
						+"VALUES ('3','"+i.ToString()+"','"
						+POut.String(table.Rows[i][3].ToString())+"')";
					Db.NonQ32(command);
				}
				//add image categories to the chart module-----------------------------------------------
				command="SELECT MAX(ItemOrder) FROM definition WHERE Category=18";
				table=Db.GetTable(command);
				int lastI=PIn.Int(table.Rows[0][0].ToString());
				commands=new string[]
				{
					"INSERT INTO definition(Category,ItemOrder,ItemName,ItemValue) "
						+"VALUES(18,"+POut.Long(lastI+1)+",'BWs','X')"
					,"INSERT INTO definition(Category,ItemOrder,ItemName,ItemValue) "
						+"VALUES(18,"+POut.Long(lastI+2)+",'FMXs','X')"
					,"INSERT INTO definition(Category,ItemOrder,ItemName,ItemValue) "
						+"VALUES(18,"+POut.Long(lastI+3)+",'Panos','X')"
					,"INSERT INTO definition(Category,ItemOrder,ItemName,ItemValue) "
						+"VALUES(18,"+POut.Long(lastI+4)+",'Photos','X')"
					,"UPDATE preference SET ValueString = '3.0.1.0' WHERE PrefName = 'DataBaseVersion'"
				};
				Db.NonQ32(commands);
			}
			To3_0_2();
		}

		private static void To3_0_2() {
			if(FromVersion < new Version("3.0.2.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.0.2");//No translation in convert script.
				string[] commands=new string[]
				{
					"INSERT INTO preference VALUES('TreatPlanShowGraphics','1')"
					,"INSERT INTO preference VALUES('TreatPlanShowCompleted','1')"
					,"INSERT INTO preference VALUES('TreatPlanShowIns','1')"
					,"UPDATE preference SET ValueString = '3.0.2.0' WHERE PrefName = 'DataBaseVersion'"
				};
				Db.NonQ32(commands);
			}
			To3_0_3();
		}

		private static void To3_0_3() {
			if(FromVersion < new Version("3.0.3.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.0.3");//No translation in convert script.
				string command="SELECT CONCAT(CONCAT(LName,', '),FName) FROM payplan,patient "
					+"WHERE patient.PatNum=payplan.PatNum";
				DataTable table=Db.GetTable(command);
				if(table.Rows.Count>0) {
					string planPats="";
					for(int i=0;i<table.Rows.Count;i++) {
						if(i>0) {
							planPats+=",";
						}
						planPats+=PIn.String(table.Rows[i][0].ToString());
					}
					/*
					MessageBox.Show("You have payment plans for the following patients: "
						+planPats+".  "
						+"There was a bug in the way the amount due was being calculated, so you will "
						+"want to follow these steps to correct the amounts due.  For each payment plan, "
						+"simply open the plan from the patient account and then click OK.  This will "
						+"reset the amount due.");*/
				}
				string[] commands=new string[]
				{
					"ALTER TABLE payplan ADD TotalCost double NOT NULL"
					,"UPDATE payplan SET TotalCost = TotalAmount"
					,"UPDATE preference SET ValueString = '3.0.3.0' WHERE PrefName = 'DataBaseVersion'"
				};
				Db.NonQ32(commands);
			}
			To3_0_4();
		}

		private static void To3_0_4() {
			if(FromVersion < new Version("3.0.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.0.4");//No translation in convert script.
				string[] commands=new string[]
				{
					"ALTER TABLE procedurelog ADD HideGraphical tinyint unsigned NOT NULL"
					,"ALTER TABLE adjustment CHANGE AdjNote AdjNote text NOT NULL"
					,"UPDATE preference SET ValueString = '3.0.4.0' WHERE PrefName = 'DataBaseVersion'"
				};
				Db.NonQ32(commands);
			}
			To3_0_5();
		}

		private static void To3_0_5() {
			if(FromVersion < new Version("3.0.5.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.0.5");//No translation in convert script.
				//Delete procedures for patients that have been deleted:
				string command="SELECT patient.PatNum FROM patient,procedurelog "
					+"WHERE patient.PatNum=procedurelog.PatNum "
					+"AND patient.PatStatus=4";
				DataTable table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="DELETE FROM procedurelog "
						+"WHERE PatNum="+table.Rows[i][0].ToString();
					Db.NonQ32(command);
				}
				//Delete extra est entries caused when patient switched plans before conversion:
				command=@"SELECT 
					cp1.ClaimProcNum,patient.PatNum,patient.LName,patient.FName
					FROM claimproc cp1,claimproc cp2,patient
					WHERE patient.PatNum=cp1.PatNum
					AND patient.PatNum=cp2.PatNum
					AND patient.PriPlanNum=cp1.PlanNum
					AND patient.SecPlanNum=0
					AND cp1.ProcNum=cp2.ProcNum
					AND cp1.ClaimProcNum!=cp2.ClaimProcNum
					AND cp1.Status=6";//estimate
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="DELETE FROM claimproc "
						+"WHERE ClaimProcNum="+table.Rows[i][0].ToString();
					Db.NonQ32(command);
				}
				string[] commands=new string[]
				{
					"ALTER TABLE claimform CHANGE UniqueID UniqueID varchar(255) NOT NULL"
					,"UPDATE claimform SET UniqueID=concat('OD',UniqueID)"
					,"UPDATE claimform SET UniqueID='' WHERE UniqueID='OD0'"
					,"UPDATE preference SET ValueString = '3.0.5.0' WHERE PrefName = 'DataBaseVersion'"
				};
				Db.NonQ32(commands);
			}
			To3_1_0();
		}

		private static void To3_1_0() {
			if(FromVersion < new Version("3.1.0.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.1.0");//No translation in convert script.
				ExecuteFile("convert_3_1_0.txt");//Might throw an exception which we handle
				//add Sirona Sidexis:
				string command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'Sirona', "
					+"'Sirona Sidexis from www.sirona.com', "
					+"'0', "
					+"'"+POut.String(@"C:\sidexis\sidexis.exe")+"', "
					+"'', "
					+"'')";
				int programNum=Db.NonQ32(command,true);//we now have a ProgramNum to work with
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+programNum.ToString()+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0')";
				Db.NonQ32(command);
				command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					+"VALUES ("
					+"'"+programNum.ToString()+"', "
					+"'"+((int)ToolBarsAvail.ChartModule).ToString()+"', "
					+"'Sirona')";
				Db.NonQ32(command);
				//convert recall
				//For inactive patients, assume no meaningful info if patients inactive,
				//so no need to create a recall. Only convert active patients.
				command="SELECT PatNum,RecallStatus,RecallInterval "
					+"FROM patient WHERE PatStatus=0";
				DataTable patTable=Db.GetTable(command);
				DataTable table;
				DateTime previousDate;
				DateTime dueDate;
				int patNum;
				int status;
				int interval;
				Interval newInterval;
				for(int i=0;i<patTable.Rows.Count;i++) {
					patNum=PIn.Int(patTable.Rows[i][0].ToString());
					status=PIn.Int(patTable.Rows[i][1].ToString());
					interval=PIn.Int(patTable.Rows[i][2].ToString());
					//get previous date
					command="SELECT MAX(procedurelog.procdate) "
						+"FROM procedurelog,procedurecode "
						+"WHERE procedurelog.PatNum="+patNum.ToString()
						+" AND procedurecode.ADACode = procedurelog.ADACode "
						+"AND procedurecode.SetRecall = 1 "
						+"AND (procedurelog.ProcStatus = 2 "
						+"OR procedurelog.ProcStatus = 3 "
						+"OR procedurelog.ProcStatus = 4) "
						+"GROUP BY procedurelog.PatNum";
					table=Db.GetTable(command);
					if(table.Rows.Count==0) {
						previousDate=DateTime.MinValue;
					}
					else {
						previousDate=PIn.Date(table.Rows[0][0].ToString());
					}
					//If no useful info and no trigger. No recall created
					if(status==0 && (interval==0 || interval==6)
						&& previousDate==DateTime.MinValue)//and no trigger
					{
						continue;
					}
					if(interval==0) {
						newInterval=new Interval(0,0,6,0);
					}
					else {
						newInterval=new Interval(0,0,interval,0);
					}
					if(previousDate==DateTime.MinValue) {
						dueDate=DateTime.MinValue;
					}
					else {
						dueDate=previousDate+newInterval;
					}
					command="INSERT INTO recall (PatNum,DateDueCalc,DateDue,DatePrevious,"
						+"RecallInterval,RecallStatus"
						+") VALUES ("
						+"'"+POut.Long(patNum)+"', "
						+POut.Date(dueDate)+", "
						+POut.Date(dueDate)+", "
						+POut.Date(previousDate)+", "
						+"'"+POut.Long(newInterval.ToInt())+"', "
						+"'"+POut.Long(status)+"')";
					Db.NonQ32(command);
				}//for int i<patTable
				command="UPDATE preference SET ValueString = '3.1.0.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_1_3();
		}

		private static void To3_1_3() {
			if(FromVersion < new Version("3.1.3.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.1.3");//No translation in convert script.
				//0 values in date fields are causing a lot of program slowdown
				string[] commands=new string[]
				{
					"UPDATE adjustment SET AdjDate='0001-01-01' WHERE AdjDate='0000-00-00'"
					,"UPDATE appointment SET AptDateTime='0001-01-01 00:00:00' "
						+"WHERE AptDateTime LIKE '0000-00-00%'"
					,"UPDATE claim SET DateService='0001-01-01' WHERE DateService='0000-00-00'"
					,"UPDATE claim SET DateSent='0001-01-01' WHERE DateSent='0000-00-00'"
					,"UPDATE claim SET DateReceived='0001-01-01' WHERE DateReceived='0000-00-00'"
					,"UPDATE claim SET PriorDate='0001-01-01' WHERE PriorDate='0000-00-00'"
					,"UPDATE claim SET AccidentDate='0001-01-01' WHERE AccidentDate='0000-00-00'"
					,"UPDATE claim SET OrthoDate='0001-01-01' WHERE OrthoDate='0000-00-00'"
					,"UPDATE claimpayment SET CheckDate='0001-01-01' WHERE CheckDate='0000-00-00'"
					,"UPDATE claimproc SET DateCP='0001-01-01' WHERE DateCP='0000-00-00'"
					,"UPDATE claimproc SET ProcDate='0001-01-01' WHERE ProcDate='0000-00-00'"
					,"UPDATE insplan SET DateEffective='0001-01-01' WHERE DateEffective='0000-00-00'"
					,"UPDATE insplan SET DateTerm='0001-01-01' WHERE DateTerm='0000-00-00'"
					,"UPDATE insplan SET RenewMonth='1' WHERE RenewMonth='0'"
					,"UPDATE patient SET Birthdate='0001-01-01' WHERE Birthdate='0000-00-00'"
					,"UPDATE patient SET DateFirstVisit='0001-01-01' WHERE DateFirstVisit='0000-00-00'"
					,"UPDATE procedurelog SET ProcDate='0001-01-01' WHERE ProcDate='0000-00-00'"
					,"UPDATE procedurelog SET DateOriginalProsth='0001-01-01' "
						+"WHERE DateOriginalProsth='0000-00-00'"
					,"UPDATE procedurelog SET DateLocked='0001-01-01' WHERE DateLocked='0000-00-00'"
					,"UPDATE recall SET DateDueCalc='0001-01-01' WHERE DateDueCalc='0000-00-00'"
					,"UPDATE recall SET DateDue='0001-01-01' WHERE DateDue='0000-00-00'"
					,"UPDATE recall SET DatePrevious='0001-01-01' WHERE DatePrevious='0000-00-00'"
					,"ALTER table adjustment CHANGE AdjDate AdjDate date NOT NULL default '0001-01-01'"
					,"ALTER table appointment CHANGE AptDateTime AptDateTime datetime NOT NULL "
						+"default '0001-01-01 00:00:00'"
					,"ALTER table claim CHANGE DateService DateService date NOT NULL default '0001-01-01'"
					,"ALTER table claim CHANGE DateSent DateSent date NOT NULL default '0001-01-01'"
					,"ALTER table claim CHANGE DateReceived DateReceived date NOT NULL default '0001-01-01'"
					,"ALTER table claim CHANGE PriorDate PriorDate date NOT NULL default '0001-01-01'"
					,"ALTER table claim CHANGE AccidentDate AccidentDate date NOT NULL default '0001-01-01'"
					,"ALTER table claim CHANGE OrthoDate OrthoDate date NOT NULL default '0001-01-01'"
					,"ALTER table claimpayment CHANGE CheckDate CheckDate date NOT NULL default '0001-01-01'"
					,"ALTER table claimproc CHANGE DateCP DateCP date NOT NULL default '0001-01-01'"
					,"ALTER table claimproc CHANGE ProcDate ProcDate date NOT NULL default '0001-01-01'"
					,"ALTER table insplan CHANGE DateEffective DateEffective date NOT NULL default '0001-01-01'"
					,"ALTER table insplan CHANGE DateTerm DateTerm date NOT NULL default '0001-01-01'"
					,"ALTER table insplan CHANGE RenewMonth RenewMonth tinyint unsigned NOT NULL default '1'"
					,"ALTER table patient CHANGE Birthdate Birthdate date NOT NULL default '0001-01-01'"
					,"ALTER table patient CHANGE DateFirstVisit DateFirstVisit date NOT NULL default '0001-01-01'"
					,"ALTER table procedurelog CHANGE ProcDate ProcDate date NOT NULL default '0001-01-01'"
					,"ALTER table procedurelog CHANGE DateOriginalProsth DateOriginalProsth "
						+"date NOT NULL default '0001-01-01'"
					,"ALTER table procedurelog CHANGE DateLocked DateLocked date NOT NULL default '0001-01-01'"
					,"ALTER table recall CHANGE DateDueCalc DateDueCalc date NOT NULL default '0001-01-01'"
					,"ALTER table recall CHANGE DateDue DateDue date NOT NULL default '0001-01-01'"
					,"ALTER table recall CHANGE DatePrevious DatePrevious date NOT NULL default '0001-01-01'"
					//Set prosth codes
					,"UPDATE procedurecode SET IsProsth=1 WHERE ADACode='D2740' || ADACode='D2750' "
						+"|| ADACode='D2751' || ADACode='D2752' || ADACode='D2790' || ADACode='D2791' "
						+"|| ADACode='D2792' || ADACode='D5110' || ADACode='D5120' || ADACode='D5130' "
						+"|| ADACode='D5140' || ADACode='D5211' || ADACode='D5212' || ADACode='D5213' "
						+"|| ADACode='D5214' || ADACode='D5225' || ADACode='D5226' || ADACode='D5281'"
						+"|| ADACode='D5810' || ADACode='D5811' || ADACode='D5820' || ADACode='D5821'"
						+"|| ADACode LIKE 'D62%' || ADACode LIKE 'D65%' || ADACode LIKE 'D66%' "
						+"|| ADACode LIKE 'D67%'"
					//add new ada codes
					//,"INSERT INTO procedurecode "
					,"INSERT INTO definition(Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) "
						+"VALUES ('21','7','Commlog Appt Related','','-886','0')"
					,"UPDATE preference SET ValueString = '3.1.3.0' WHERE PrefName = 'DataBaseVersion'"
				};
				Db.NonQ32(commands);
			}
			To3_1_4();
		}

		private static void To3_1_4() {
			if(FromVersion < new Version("3.1.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.1.4");//No translation in convert script.
				string[] commands=new string[]
				{
					"ALTER table clearinghouse ADD LoginID varchar(255) NOT NULL"
					,"UPDATE clearinghouse SET ReceiverID='0135WCH00' WHERE ReceiverID='WebMD'"
					,"ALTER table provider ADD OutlineColor int NOT NULL"
					,"UPDATE provider SET OutlineColor ='-11711155'"
					,"UPDATE preference SET ValueString = '3.1.4.0' WHERE PrefName = 'DataBaseVersion'"
				};
				Db.NonQ32(commands);
			}
			To3_1_13();
		}

		private static void To3_1_13() {
			if(FromVersion < new Version("3.1.13.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.1.13");//No translation in convert script.
				//get rid of any medication pats where medication no longer exists.
				string command="SELECT medicationpat.MedicationPatNum FROM medicationpat "
					+"LEFT JOIN medication ON medicationpat.MedicationNum=medication.MedicationNum "
					+"WHERE medication.MedicationNum IS NULL";
				DataTable table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="DELETE FROM medicationpat WHERE MedicationPatNum="
						+table.Rows[i][0].ToString();
					Db.NonQ32(command);
				}
				string[] commands=new string[]
				{
					"INSERT INTO clearinghouse(Description,ExportPath,IsDefault,Payors,Eformat,ReceiverID,"
						+"SenderID,Password,ResponsePath,CommBridge,ClientProgram) "
						+@"VALUES('RECS','C:\\Recscom\\','0','','1','RECS','','',"
						+@"'','5','C:\\Recscom\\Recscom.exe')"
					,"UPDATE preference SET ValueString = '3.1.13.0' WHERE PrefName = 'DataBaseVersion'"
				};

				Db.NonQ32(commands);
			}
			To3_1_16();
		}

		private static void To3_1_16() {
			if(FromVersion < new Version("3.1.16.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.1.16");//No translation in convert script.
				//this functionality is all copied directly from the Check Database tool.
				string command=@"SELECT PatNum FROM patient
					LEFT JOIN insplan on patient.PriPlanNum=insplan.PlanNum
					WHERE patient.PriPlanNum != 0
					AND insplan.PlanNum IS NULL";
				DataTable table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="UPDATE patient set PriPlanNum=0 "
						+"WHERE PatNum="+table.Rows[i][0].ToString();
					Db.NonQ32(command);
				}
				command=@"SELECT ClaimProcNum FROM claimproc
					LEFT JOIN insplan ON claimproc.PlanNum=insplan.PlanNum
					WHERE insplan.PlanNum IS NULL";
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="DELETE FROM claimproc "
						+"WHERE ClaimProcNum="+table.Rows[i][0].ToString();
					Db.NonQ32(command);
				}
				command=@"SELECT ClaimNum FROM claim
					LEFT JOIN insplan ON claim.PlanNum=insplan.PlanNum
					WHERE insplan.PlanNum IS NULL";
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="DELETE FROM claim "
						+"WHERE ClaimNum="+table.Rows[i][0].ToString();
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '3.1.16.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_4_0();
		}

		private static void To3_4_0() {
			if(FromVersion < new Version("3.4.0.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.4.0");//No translation in convert script.
				ExecuteFile("convert_3_4_0.txt");//Might throw an exception which we handle.
				//----------------Clone payment dates into paysplits--------------------------------------
				string command="SELECT paysplit.SplitNum,payment.PayDate FROM payment,paysplit "
					+"WHERE payment.PayNum=paysplit.PayNum";
				DataTable table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="UPDATE paysplit SET "
						+"DatePay="+POut.Date(PIn.Date(table.Rows[i][1].ToString()))+" "
						+"WHERE SplitNum="+table.Rows[i][0].ToString();
					Db.NonQ32(command);
				}
				//----------------Convert all discounts to adjustments-----------------------------------
				//add adjustment categories.
				command="SELECT Max(ItemOrder) FROM definition WHERE Category=1";
				table=Db.GetTable(command);
				int firstItemOrder=PIn.Int(table.Rows[0][0].ToString())+1;
				command="SELECT * FROM definition WHERE Category=15 ORDER BY ItemOrder";//cat=DiscountTypes
				table=Db.GetTable(command);
				Hashtable HDiscToAdj=new Hashtable();//key=original defNum(discountType. value=new defNum(AdjType)
				int numAdj=0;
				for(int i=0;i<table.Rows.Count;i++) {
					command="INSERT INTO definition (category,itemorder,itemname,itemvalue,ishidden) VALUES("
					+"1, "//category=AdjTypes
					+"'"+POut.Long(firstItemOrder+i)+"', "//itemOrder
					+"'"+POut.String(PIn.String(table.Rows[i][3].ToString()))+"', "//item name
					+"'-', "//itemValue. All discounts are negative
					+"'"+table.Rows[i][6].ToString()+"')";//is hidden
					numAdj=Db.NonQ32(command,true);
					HDiscToAdj.Add(PIn.Long(table.Rows[i][0].ToString()),//defNum of disc
						numAdj);//defNum of adj
				}
				//handle 0:
				HDiscToAdj.Add(0,numAdj);
				//create new adjustments from existing discounts
				command="SELECT * FROM paysplit WHERE IsDiscount=1";//0=SplitNum,1=SplitAmt,2=PatNum,3=ProcDate,
				//4=PayNum,5=IsDiscount,6=DiscountType,7=ProvNum,8=PayPlanNum,9=DatePay
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="INSERT INTO adjustment (AdjDate,AdjAmt,PatNum, "
					+"AdjType,ProvNum,ProcDate) "//AdjNote
					+"VALUES("
					+POut.Date(PIn.Date(table.Rows[i][9].ToString()))+", "//entryDate
					+"'"+POut.Double(-PIn.Double(table.Rows[i][1].ToString()))+"', "//amt
					+"'"+POut.Long(PIn.Long(table.Rows[i][2].ToString()))+"', "//patNum
					+"'"+POut.Long((int)HDiscToAdj[PIn.Long(table.Rows[i][6].ToString())])+"', "//type
					+"'"+POut.Long(PIn.Long(table.Rows[i][7].ToString()))+"', "//provNum
					+POut.Date(PIn.Date(table.Rows[i][3].ToString()))+")";//procDate
					//note
					Db.NonQ32(command);
				}
				command="DELETE FROM paysplit WHERE IsDiscount=1";
				Db.NonQ32(command);
				//--------------------Printers----------------------------------------------------------
				command="SELECT * FROM computer WHERE PrinterName != ''";
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="INSERT INTO printer (ComputerNum,PrintSit,PrinterName,"
					+"DisplayPrompt) "
					+"VALUES("
					+"'"+POut.Long(PIn.Long(table.Rows[i][0].ToString()))+"', "
					+"'"+POut.Long((int)PrintSituation.Default)+"', "
					+"'"+POut.String(PIn.String(table.Rows[i][2].ToString()))+"', "
					+"'1')";
					Db.NonQ32(command);
				}
				command="UPDATE computer SET PrinterName = ''";
				Db.NonQ32(command);
				//HouseCalls link-----------------------------------------------------------------------
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'HouseCalls', "
					+"'HouseCalls from www.housecallsweb.com', "
					+"'0', "
					+"'', "
					+"'', "
					+"'"+POut.String(@"Typical Export Path is C:\HouseCalls\")+"')";
				int programNum=Db.NonQ32(command,true);//we now have a ProgramNum to work with
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+programNum.ToString()+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0')";
				Db.NonQ32(command);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+programNum.ToString()+"', "
					+"'Export Path', "
					+"'"+POut.String(@"C:\HouseCalls\")+"')";
				Db.NonQ32(command);
				command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					+"VALUES ("
					+"'"+programNum.ToString()+"', "
					+"'"+((int)ToolBarsAvail.ChartModule).ToString()+"', "
					+"'HouseCalls')";
				Db.NonQ32(command);
				//Delete program links for WebClaim and Renaissance--------------------------------------


				//Final cleanup-------------------------------------------------------------------------
				command="UPDATE preference SET ValueString = '3.4.0.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_4_7();
		}

		private static void To3_4_7() {
			if(FromVersion < new Version("3.4.7.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.4.7");//No translation in convert script.
				string[] commands=new string[]
				{
					"INSERT INTO clearinghouse(Description,ExportPath,IsDefault,Payors,Eformat,ReceiverID,"
						+"SenderID,Password,ResponsePath,CommBridge,ClientProgram) "
						+@"VALUES('WebClaim','C:\\WebClaim\\Upload\\','0','','1','330989922','','',"
						+@"'','4','')"
					,"UPDATE preference SET ValueString = '3.4.7.0' WHERE PrefName = 'DataBaseVersion'"
				};
				Db.NonQ32(commands);
			}
			To3_4_10();
		}

		private static void To3_4_10() {
			//the only purpose of this is to check the bug fix in conversions
			if(FromVersion < new Version("3.4.10.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.4.10");//No translation in convert script.
				string[] commands=new string[]
				{
					"UPDATE preference SET ValueString = '3.4.10.0' WHERE PrefName = 'DataBaseVersion'"
				};
				Db.NonQ32(commands);
			}
			To3_4_11();
		}

		private static void To3_4_11() {
			if(FromVersion < new Version("3.4.11.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.4.11");//No translation in convert script.
				//Planmeca link-----------------------------------------------------------------------
				string command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'Planmeca', "
					+"'Dimaxis from Planmeca', "
					+"'0', "
					+"'DxStart.exe', "
					+"'', "
					+"'"+POut.String(@"Typical file path is DxStart.exe which is available from Planmeca and should be placed in the same folder as this program.")+"')";
				int programNum=Db.NonQ32(command,true);//we now have a ProgramNum to work with
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+programNum.ToString()+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0')";
				Db.NonQ32(command);
				command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					+"VALUES ("
					+"'"+programNum.ToString()+"', "
					+"'"+((int)ToolBarsAvail.ChartModule).ToString()+"', "
					+"'Planmeca')";
				Db.NonQ32(command);
				command=
					"UPDATE preference SET ValueString = '3.4.11.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_4_16();
		}

		private static void To3_4_16() {
			if(FromVersion < new Version("3.4.16.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.4.16");//No translation in convert script.
				string[] commands=new string[]
				{
					@"UPDATE clearinghouse SET Description='ClaimConnect',ExportPath='C:\\ClaimConnect\\Upload\\' WHERE Description='WebClaim'"
					,"UPDATE preference SET ValueString = '3.4.16.0' WHERE PrefName = 'DataBaseVersion'"
				};
				Db.NonQ32(commands);
			}
			To3_4_17();
		}

		private static void To3_4_17() {
			if(FromVersion < new Version("3.4.17.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.4.17");//No translation in convert script.
				string[] commands=new string[]
				{
					"UPDATE patient SET DateFirstVisit='0001-01-01' WHERE DateFirstVisit='0000-00-00'"
					,"UPDATE preference SET ValueString = '3.4.17.0' WHERE PrefName = 'DataBaseVersion'"
				};
				Db.NonQ32(commands);
			}
			To3_4_24();
		}

		private static void To3_4_24() {
			if(FromVersion < new Version("3.4.24.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.4.24");//No translation in convert script.
				//Delete program links for WebClaim and Renaissance--------------------------------------
				string command="SELECT ProgramNum FROM program WHERE ProgName='WebClaim' OR ProgName='Renaissance'";
				DataTable table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="DELETE FROM program WHERE ProgramNum="+table.Rows[i][0].ToString();
					Db.NonQ32(command);
					command="DELETE FROM toolbutitem WHERE ProgramNum="+table.Rows[i][0].ToString();
					Db.NonQ32(command);
				}
				//Fix utf8 binary collations for ADACode columns-------------------------------------------
				command="SELECT @@version";
				table=Db.GetTable(command);
				string thisVersion=PIn.String(table.Rows[0][0].ToString());
				string[] commands;
				if(thisVersion.Substring(0,3)=="4.1" || thisVersion.Substring(0,3)=="5.0") {
					commands=new string[]
					{
						"ALTER TABLE procedurecode CHANGE ADACode ADACode varchar(15) character set utf8 collate utf8_bin NOT NULL"
						,"ALTER TABLE procedurecode DEFAULT character set utf8"
						,"ALTER TABLE procedurecode MODIFY Descript varchar(255) character set utf8 NOT NULL"
						,"ALTER TABLE procedurecode MODIFY AbbrDesc varchar(50) character set utf8 NOT NULL"
						,"ALTER TABLE procedurecode MODIFY ProcTime varchar(24) character set utf8 NOT NULL"
						,"ALTER TABLE procedurecode MODIFY DefaultNote text character set utf8 NOT NULL"
						,"ALTER TABLE procedurecode MODIFY AlternateCode1 varchar(15) character set utf8 NOT NULL"
						,"ALTER TABLE procedurelog MODIFY ADACode varchar(15) character set utf8 collate utf8_bin NOT NULL"
						,"ALTER TABLE autocodeitem MODIFY ADACode varchar(15) character set utf8 collate utf8_bin NOT NULL"
						,"ALTER TABLE procbuttonitem MODIFY ADACode varchar(15) character set utf8 collate utf8_bin NOT NULL"
						,"ALTER TABLE covspan MODIFY FromCode varchar(15) character set utf8 collate utf8_bin NOT NULL"
						,"ALTER TABLE covspan MODIFY ToCode varchar(15) character set utf8 collate utf8_bin NOT NULL"
					};
					Db.NonQ32(commands);
				}
				commands=new string[]
				{
				//Inmediata clearinghouse--------------------------------------------------------------
					"INSERT INTO clearinghouse(Description,ExportPath,IsDefault,Payors,Eformat,ReceiverID,"
						+"SenderID,Password,ResponsePath,CommBridge,ClientProgram) "
						+@"VALUES('Inmediata Health Group Corp','C:\\Inmediata\\Claims\\','0','','1','660610220','','',"
						+@"'C:\\Inmediata\\Reports\\','6','C:\\Program Files\\Inmediata\\IMPlug.exe')"
					,"UPDATE preference SET ValueString = '3.4.24.0' WHERE PrefName = 'DataBaseVersion'"
				};
				Db.NonQ32(commands);
			}
			To3_5_0();
		}

		private static void To3_5_0() {
			if(FromVersion < new Version("3.5.0.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.5.0");//No translation in convert script.
				ExecuteFile("convert_3_5_0.txt");//Might throw an exception which we handle.
				//Add patient picture category to images
				string command="SELECT MAX(ItemOrder) FROM definition WHERE Category=18";
				DataTable table=Db.GetTable(command);
				int lastI=PIn.Int(table.Rows[0][0].ToString());
				command="INSERT INTO definition(Category,ItemOrder,ItemName,ItemValue) "
					+"VALUES(18,"+POut.Long(lastI+1)+",'Patient Pictures','P')";
				Db.NonQ32(command);
				//ImageFX link-----------------------------------------------------------------------
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'ImageFX', "
					+"'ImageFX from scican.com', "
					+"'0', "
					+"'"+POut.String(@"C:\ImageFX\ImageFX.exe")+"', "
					+"'', "
					+"'"+POut.String(@"Typical file path is C:\ImageFX\ImageFX.exe")+"')";
				int programNum=Db.NonQ32(command,true);//we now have a ProgramNum to work with
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+programNum.ToString()+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0')";
				Db.NonQ32(command);
				command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					+"VALUES ("
					+"'"+programNum.ToString()+"', "
					+"'"+((int)ToolBarsAvail.ChartModule).ToString()+"', "
					+"'ImageFX')";
				Db.NonQ32(command);
				//fix the provider ID field----------------------------------------------------------------
				command="SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD1'";
				table=Db.GetTable(command);
				if(table.Rows.Count>0) {
					command="UPDATE claimformitem SET FieldName='BillingDentistProviderID' WHERE FieldName='BillingDentistMedicaidID' "
						+"AND ClaimFormNum="+table.Rows[0][0].ToString();
					Db.NonQ32(command);
				}
				command=
					"UPDATE preference SET ValueString = '3.5.0.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_5_1();
		}

		private static void To3_5_1() {
			if(FromVersion < new Version("3.5.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.5.1");//No translation in convert script.
				string[] commands=new string[]
				{
					"ALTER TABLE schedule CHANGE Note Note TEXT NOT NULL"
					,"UPDATE preference SET ValueString = '3.5.1.0' WHERE PrefName = 'DataBaseVersion'"
				};
				Db.NonQ32(commands);
			}
			To3_5_3();
		}

		private static void To3_5_3() {
			if(FromVersion < new Version("3.5.3.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.5.3");//No translation in convert script.
				//DentForms link-----------------------------------------------------------------------
				string command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'DentForms', "
					+"'DentForms from medictalk.com', "
					+"'0', "
					+"'"+POut.String(@"C:\MedicTalk\reports\mtconnector.exe")+"', "
					+"'', "
					+"'"+POut.String(@"No command line is needed.  Typical path is C:\MedicTalk\reports\mtconnector.exe")+"')";
				int programNum=Db.NonQ32(command,true);//we now have a ProgramNum to work with
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+programNum.ToString()+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0')";
				Db.NonQ32(command);
				command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					+"VALUES ("
					+"'"+programNum.ToString()+"', "
					+"'"+((int)ToolBarsAvail.ChartModule).ToString()+"', "
					+"'DentForms')";
				Db.NonQ32(command);
				command="UPDATE tasklist SET DateType=0 WHERE Parent !=0";
				Db.NonQ32(command);
				command="UPDATE task SET DateType=0, TaskStatus=0 WHERE TaskListNum !=0";
				Db.NonQ32(command);
				command=
					"UPDATE preference SET ValueString = '3.5.3.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_6_0();
		}

		private static void To3_6_0() {
			if(FromVersion < new Version("3.6.0.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.6.0");//No translation in convert script.
				ExecuteFile("convert_3_6_0.txt");//Might throw an exception which we handle.
				string command;
				command=
					"UPDATE preference SET ValueString = '3.6.0.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_6_1();
		}

		private static void To3_6_1() {
			if(FromVersion < new Version("3.6.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.6.1");//No translation in convert script.
				string command;
				//Not sure how some of the dates got out of synch:
				command="UPDATE payment,paysplit SET paysplit.DatePay=payment.PayDate WHERE paysplit.PayNum=payment.PayNum";
				Db.NonQ32(command);
				//or how procedures can accidently get attached to appointments for different patients:
				command="UPDATE procedurelog,appointment SET procedurelog.AptNum=0 "
					+"WHERE procedurelog.AptNum=appointment.AptNum AND appointment.PatNum!=procedurelog.PatNum";
				Db.NonQ32(command);
				command=
					"UPDATE preference SET ValueString = '3.6.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_6_4();
		}

		private static void To3_6_4() {
			//duplicate of To3_5_6 because we needed to fix for users who had already upgraded to 3.6
			if(FromVersion < new Version("3.6.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.6.4");//No translation in convert script.
				string[] commands=new string[]
					{
						"ALTER TABLE procedurecode CHANGE ADACode ADACode varchar(15) character set utf8 collate utf8_bin NOT NULL"
						,"ALTER TABLE procedurecode DEFAULT character set utf8"
						,"ALTER TABLE procedurecode MODIFY Descript varchar(255) character set utf8 NOT NULL"
						,"ALTER TABLE procedurecode MODIFY AbbrDesc varchar(50) character set utf8 NOT NULL"
						,"ALTER TABLE procedurecode MODIFY ProcTime varchar(24) character set utf8 NOT NULL"
						,"ALTER TABLE procedurecode MODIFY DefaultNote text character set utf8 NOT NULL"
						,"ALTER TABLE procedurecode MODIFY AlternateCode1 varchar(15) character set utf8 NOT NULL"
						,"ALTER TABLE procedurelog MODIFY ADACode varchar(15) character set utf8 collate utf8_bin NOT NULL"
						,"ALTER TABLE autocodeitem MODIFY ADACode varchar(15) character set utf8 collate utf8_bin NOT NULL"
						,"ALTER TABLE procbuttonitem MODIFY ADACode varchar(15) character set utf8 collate utf8_bin NOT NULL"
						,"ALTER TABLE covspan MODIFY FromCode varchar(15) character set utf8 collate utf8_bin NOT NULL"
						,"ALTER TABLE covspan MODIFY ToCode varchar(15) character set utf8 collate utf8_bin NOT NULL"
						,"ALTER TABLE fee MODIFY ADACode varchar(15) character set utf8 collate utf8_bin NOT NULL"
					};
				Db.NonQ32(commands);
				commands=new string[]
				{
					//AOS DATA clearinghouse----------------------------------------ADDED by SPK 7/13/05----------------------
					"INSERT INTO clearinghouse(Description,ExportPath,IsDefault,Payors,Eformat,ReceiverID,"
					+"SenderID,Password,ResponsePath,CommBridge,ClientProgram) "
					+@"VALUES('AOS Data systems','C:\\Program Files\\AOS\\','0','','1','AOS','','',"
					+@"'C:\\Program Files\\AOS\\','7','C:\\Program Files\\AOS\\AOSCommunicator\\AOSCommunicator.exe')"
 				};
				Db.NonQ32(commands);
				string command="UPDATE preference SET ValueString = '3.6.4.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_6_5();
		}

		private static void To3_6_5() {
			if(FromVersion < new Version("3.6.5.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.6.5");//No translation in convert script.
				//delete any unattached adjustments
				string command="SELECT adjustment.AdjNum,procedurelog.ProcNum FROM adjustment "
					+"LEFT JOIN procedurelog ON procedurelog.ProcNum=adjustment.ProcNum "
					+"WHERE adjustment.ProcNum !=0 "
					+"AND procedurelog.ProcNum IS NULL";
				DataTable table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="DELETE FROM adjustment WHERE AdjNum="+table.Rows[i][0].ToString();
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '3.6.5.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_7_0();
		}

		private static void To3_7_0() {
			if(FromVersion < new Version("3.7.0.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.7.0");//No translation in convert script.
				ExecuteFile("convert_3_7_0.txt");//Might throw an exception which we handle.
				string command;
				//Convert pay plans-----------------------------------------------------------------------------
				command="SELECT PayPlanNum,PatNum,Guarantor,PayPlanDate,TotalAmount,APR,"//0-5
					+"PeriodPayment,Term,AccumulatedDue,DateFirstPay,DownPayment,"//6-10
					+"Note,TotalCost,LastPayment "//11-13
					+"FROM payplan";
				DataTable table=Db.GetTable(command);
				int payPlanNum;//0
				int patNum;//1
				int guarantor;//2
				DateTime payPlanDate;// 3
				double totalAmount;//4 aka principal. This gets reduced to 0 in loop
				double APR;// 5
				double monthlyPayment;//6
				int term;//7
				//CurrentDue 8
				DateTime dateFirstPay;//9
				double downPayment;//10
				//Note 11
				double totalCost;// 12. Princ+Int. This gets reduced to 0 in loop
				double lastPayment;//13
				//variables used for the individual charges:
				DateTime chargeDate;
				double principal;
				double interest;
				double monthlyRate;
				for(int i=0;i<table.Rows.Count;i++) {
					payPlanNum=    PIn.Int(table.Rows[i][0].ToString());
					patNum=        PIn.Int(table.Rows[i][1].ToString());
					guarantor=     PIn.Int(table.Rows[i][2].ToString());
					payPlanDate=   PIn.Date(table.Rows[i][3].ToString());
					totalAmount=   PIn.Double(table.Rows[i][4].ToString());
					APR=           PIn.Double(table.Rows[i][5].ToString());
					monthlyPayment=PIn.Double(table.Rows[i][6].ToString());
					term=          PIn.Int(table.Rows[i][7].ToString());
					dateFirstPay=  PIn.Date(table.Rows[i][9].ToString());
					downPayment=   PIn.Double(table.Rows[i][10].ToString());
					totalCost=     PIn.Double(table.Rows[i][12].ToString());
					lastPayment=   PIn.Double(table.Rows[i][13].ToString());
					//down payment
					if(downPayment>0) {
						chargeDate=payPlanDate;
						principal=downPayment;
						totalCost-=downPayment;
						totalAmount-=downPayment;
						interest=0;
						command="INSERT INTO payplancharge (PayPlanNum,Guarantor,PatNum,ChargeDate,Principal,Interest,Note) VALUES("
							+"'"+POut.Long(payPlanNum)+"', "
							+"'"+POut.Long(guarantor)+"', "
							+"'"+POut.Long(patNum)+"', "
							+POut.Date(chargeDate)+", "
							+"'"+POut.Double(principal)+"', "
							+"'"+POut.Double(interest)+"', "
							+"'Downpayment')";
						Db.NonQ32(command);
					}
					if(APR==0) {
						monthlyRate=0;
					}
					else {
						monthlyRate=APR/100/12;
					}
					for(int j=0;j<term;j++) {
						chargeDate=dateFirstPay.AddMonths(j);
						if(j==term-1 && lastPayment==0) {//if this is the very last payment
							//all remaining principal gets applied
							principal=totalAmount;
							totalCost-=totalAmount;
							totalAmount=0;
							//all remaining interest gets applied
							interest=totalCost;
							totalCost=0;
						}
						else {
							interest=Math.Round((totalAmount*monthlyRate),2);//2 decimals
							principal=monthlyPayment-interest;
							totalAmount-=principal;
							totalCost-=monthlyPayment;
						}
						if(principal<0) {
							principal=0;
						}
						if(interest<0) {
							interest=0;
						}
						command="INSERT INTO payplancharge (PayPlanNum,Guarantor,PatNum,ChargeDate,Principal,Interest) VALUES("
							+"'"+POut.Long(payPlanNum)+"', "
							+"'"+POut.Long(guarantor)+"', "
							+"'"+POut.Long(patNum)+"', "
							+POut.Date(chargeDate)+", "
							+"'"+POut.Double(principal)+"', "
							+"'"+POut.Double(interest)+"')";
						Db.NonQ32(command);
					}//loop term
					//last payment
					if(lastPayment!=0) {
						chargeDate=dateFirstPay.AddMonths(term);
						//all remaining principal gets applied
						principal=totalAmount;
						totalCost-=totalAmount;
						totalAmount=0;
						//all remaining interest gets applied
						interest=totalCost;
						totalCost=0;
						command="INSERT INTO payplancharge (PayPlanNum,Guarantor,PatNum,ChargeDate,Principal,Interest) VALUES("
							+"'"+POut.Long(payPlanNum)+"', "
							+"'"+POut.Long(guarantor)+"', "
							+"'"+POut.Long(patNum)+"', "
							+POut.Date(chargeDate)+", "
							+"'"+POut.Double(principal)+"', "
							+"'"+POut.Double(interest)+"')";
						Db.NonQ32(command);
					}
				}
				//get rid of unwanted columns in pay plans
				string[] commands=new string[]
					{
						"ALTER TABLE payplan DROP TotalAmount"
						,"ALTER TABLE payplan DROP PeriodPayment"
						,"ALTER TABLE payplan DROP Term"
						,"ALTER TABLE payplan DROP AccumulatedDue"
						,"ALTER TABLE payplan DROP DateFirstPay"
						,"ALTER TABLE payplan DROP DownPayment"
						,"ALTER TABLE payplan DROP TotalCost"
						,"ALTER TABLE payplan DROP LastPayment"
					};
				Db.NonQ32(commands);
				//Operatories----------------------------------------------------------------------------------------------
				command="SELECT DefNum,ItemOrder,ItemName,ItemValue,IsHidden FROM definition WHERE Category=9 ORDER BY ItemOrder";
				table=Db.GetTable(command);
				//Hashtable hashOps=new Hashtable();//key=defNum,value=OperatoryNum
				int defNum;//represents the old opNum as it was in the database
				int opNum;//the newly assigned key
				string itemName;
				string itemValue;
				for(int i=0;i<table.Rows.Count;i++) {
					defNum=PIn.Int(table.Rows[i][0].ToString());
					itemName=PIn.String(table.Rows[i][2].ToString());
					itemValue=PIn.String(table.Rows[i][3].ToString());
					command="INSERT INTO operatory (OpName,Abbrev,ItemOrder,IsHidden) VALUES("
						+"'"+POut.String(itemName)+"', "
						+"'"+POut.String(itemValue)+"', "
						+"'"+table.Rows[i][1].ToString()+"', "
						+"'"+table.Rows[i][4].ToString()+"')";
					opNum=Db.NonQ32(command,true);
					command="UPDATE appointment SET Op="+POut.Long(opNum)+" WHERE Op="+POut.Long(defNum);
					Db.NonQ32(command);
					command="UPDATE scheddefault SET Op="+POut.Long(opNum)+" WHERE Op="+POut.Long(defNum);
					Db.NonQ32(command);
					command="UPDATE apptviewitem SET OpNum="+POut.Long(opNum)+" WHERE OpNum="+POut.Long(defNum);
					Db.NonQ32(command);
				}
				command="DELETE FROM definition WHERE Category=9";
				Db.NonQ32(command);
				//final cleanup-----------------------------------------------------------------------------------------
				command=
					"UPDATE preference SET ValueString = '3.7.0.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_7_2();
		}

		private static void To3_7_2() {
			if(FromVersion < new Version("3.7.2.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.7.2");//No translation in convert script.
				//add the new permission types to each group
				string command="SELECT UserGroupNum FROM usergroup";
				DataTable table=Db.GetTable(command);
				int groupNum;
				for(int i=0;i<table.Rows.Count;i++) {
					groupNum=PIn.Int(table.Rows[i][0].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) VALUES("+POut.Long(groupNum)+",25)";
					Db.NonQ32(command);
					command="INSERT INTO grouppermission (UserGroupNum,PermType) VALUES("+POut.Long(groupNum)+",26)";
					Db.NonQ32(command);
					command="INSERT INTO grouppermission (UserGroupNum,PermType) VALUES("+POut.Long(groupNum)+",27)";
					Db.NonQ32(command);
					//by default, nobody will have permission to backup
					//command="INSERT INTO grouppermission (UserGroupNum,PermType) VALUES("+POut.PInt(groupNum)+",28)";
					//Db.NonQ32(command);
					//also by default, nobody will have permission to TimcardsEditAll
				}
				command="ALTER TABLE user ADD EmployeeNum smallint NOT NULL";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '3.7.2.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_7_3();
		}

		private static void To3_7_3() {
			if(FromVersion < new Version("3.7.3.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.7.3");//No translation in convert script.
				string command="ALTER TABLE securitylog ADD PatNum mediumint unsigned NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE tasklist ADD DateTimeEntry datetime NOT NULL default '0001-01-01'";
				Db.NonQ32(command);
				command="ALTER TABLE task ADD DateTimeEntry datetime NOT NULL default '0001-01-01'";
				Db.NonQ32(command);
				command="INSERT INTO preference VALUES ('BalancesDontSubtractIns','0')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '3.7.3.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_7_4();
		}

		private static void To3_7_4() {
			if(FromVersion < new Version("3.7.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.7.4");//No translation in convert script.
				//Easy Notes Pro link-----------------------------------------------------------------------
				string command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'EasyNotesPro', "
					+"'Easy Notes Pro from easynotespro.com', "
					+"'0', "
					+"'"+POut.String(@"C:\Program Files\EasyNotesPro\AppBarProcess.exe")+"', "
					+"'"+POut.String("\""+@"C:\Program Files\EasyNotesPro\DefaultDentalToolbar.etb"+"\""+" OpenDental false")+"', "
					+"'"+POut.String(@"Do not try to add buttons to your toolbars because that won't work.  Typical path is C:\Program Files\EasyNotesPro\AppBarProcess.exe")+"')";
				Db.NonQ32(command,true);
				command=
					"UPDATE preference SET ValueString = '3.7.4.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_7_5();
		}

		private static void To3_7_5() {
			if(FromVersion < new Version("3.7.5.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.7.5");//No translation in convert script.
				string command="INSERT INTO preference VALUES ('TimecardSecurityEnabled','0')";
				Db.NonQ32(command);
				command="INSERT INTO preference VALUES ('RecallCardsShowReturnAdd','1')";
				Db.NonQ32(command);
				command="ALTER TABLE insplan ADD BenefitNotes text NOT NULL";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '3.7.5.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_7_6();
		}

		private static void To3_7_6() {
			if(FromVersion < new Version("3.7.6.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.7.6");//No translation in convert script.
				string command="ALTER TABLE clinic ADD DefaultPlaceService tinyint unsigned NOT NULL";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '3.7.6.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_8_0();
		}

		private static void To3_8_0() {
			if(FromVersion < new Version("3.8.0.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.8.0");//No translation in convert script.
				ExecuteFile("convert_3_8_0.txt");//Might throw an exception which we handle.
				//add deposit slip permission to each group
				string command="SELECT UserGroupNum FROM usergroup";
				DataTable table=Db.GetTable(command);
				int groupNum;
				for(int i=0;i<table.Rows.Count;i++) {
					groupNum=PIn.Int(table.Rows[i][0].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) VALUES("+POut.Long(groupNum)+",30)";
					Db.NonQ32(command);
				}
				//Populate the new column: claimpayment.CarrierName
				command="SELECT claimpayment.ClaimPaymentNum,carrier.CarrierName "
					+"FROM claimpayment,claimproc,insplan,carrier "
					+"WHERE claimproc.ClaimPaymentNum = claimpayment.ClaimPaymentNum "
					+"AND claimproc.PlanNum = insplan.PlanNum "
					+"AND insplan.CarrierNum = carrier.CarrierNum "
					+"GROUP BY claimpayment.ClaimPaymentNum";
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="UPDATE claimpayment SET CarrierName='"+POut.String(PIn.String(table.Rows[i][1].ToString()))+"' "
						+"WHERE ClaimPaymentNum="+table.Rows[i][0].ToString();
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '3.8.0.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_8_5();
		}

		private static void To3_8_5() {
			if(FromVersion < new Version("3.8.5.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.8.5");//No translation in convert script.
				//Make a few changes to the paths in the ENP bridge
				string command;//="SELECT ProgramNum FROM program WHERE ProgName='EasyNotesPro'";
				//DataTable table=Db.GetTable(command);
				//if(table.Rows.Count>0){//otherwise user might have deleted the bridge
				//int programNum=PIn.PInt(table.Rows[0][0].ToString());
				command="UPDATE program SET "
					+"CommandLine='"+POut.String("\""+@"C:\Program Files\EasyNotesPro\DefaultDentalToolbar.etb"+"\""+" standalone true")+"' "
					+"WHERE ProgName='EasyNotesPro'";//+POut.PInt(programNum);
				Db.NonQ32(command);
				//}
				command="UPDATE preference SET ValueString = '3.8.5.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_9_0();
		}

		private static void To3_9_0() {
			if(FromVersion < new Version("3.9.0.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.9.0");//No translation in convert script.
				ExecuteFile(Path.Combine("Version 3 9 0", "convert_3_9_0.txt"));//Might throw an exception which we handle.
				//convert two letter languages to 5 char specific culture names-------------------------------------------------
				string command="";
				DataTable table;
				if(CultureInfo.CurrentCulture.Name=="en-US") {
					command="DELETE FROM languageforeign";
					Db.NonQ32(command);
				}
				else {
					/*
					command="SELECT DISTINCT Culture FROM languageforeign";
					table=Db.GetTable(command);
					CultureInfo ci;
					for(int i=0;i<table.Rows.Count;i++) {
						try {
							ci=new CultureInfo(table.Rows[i][0].ToString());
						}
						catch {
							//MessageBox.Show("Invalid culture: "+table.Rows[i][0].ToString());
							continue;
						}
						FormConvertLang39 FormC=new FormConvertLang39();
						FormC.OldCulture=ci;
						FormC.ShowDialog();
						if(FormC.DialogResult!=DialogResult.OK) {
							continue;
						}
						command="UPDATE languageforeign SET Culture='"+FormC.NewName+"' "
							+"WHERE Culture='"+table.Rows[i][0].ToString()+"'";
						Db.NonQ32(command);
					}*/
				}
				//------------------------------------------------------------------------------------------------------------
				//move all patient.PriPlanNum,PriRelationship,SecPlanNum,SecRelationship,
				//PriPending,SecPending,PriPatID,SecPatID to PatPlan objects
				command="SELECT PatNum,PriPlanNum,PriRelationship,PriPending,PriPatID FROM patient WHERE PriPlanNum>0";
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="INSERT INTO patplan (PatNum,PlanNum,Ordinal,IsPending,Relationship,PatID) VALUES ("
						+table.Rows[i][0].ToString()+","//patnum
						+table.Rows[i][1].ToString()+","//planNum
						+"1,"//Ordinal
						+table.Rows[i][3].ToString()+","//IsPending
						+table.Rows[i][2].ToString()+","//Relationship
						+"'"+POut.String(PIn.String(table.Rows[i][4].ToString()))+"'"//PatID
						+")";
					Db.NonQ32(command);
				}
				command="SELECT PatNum,SecPlanNum,SecRelationship,SecPending,SecPatID FROM patient WHERE SecPlanNum>0";
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="INSERT INTO patplan (PatNum,PlanNum,Ordinal,IsPending,Relationship,PatID) VALUES ("
						+table.Rows[i][0].ToString()+","//patnum
						+table.Rows[i][1].ToString()+","//planNum
						+"2,"//Ordinal
						+table.Rows[i][3].ToString()+","//IsPending
						+table.Rows[i][2].ToString()+","//Relationship
						+"'"+POut.String(PIn.String(table.Rows[i][4].ToString()))+"'"//PatID
						+")";
					Db.NonQ32(command);
				}
				//convert all covpat.PriPatNum and SecPatNum to PatPlanNum-----------------------------------------------------
				//primary
				command="SELECT covpat.CovPatNum,patplan.PatPlanNum FROM covpat,patplan "
					+"WHERE covpat.PriPatNum=patplan.PatNum "
					+"AND patplan.Ordinal=1";
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="UPDATE covpat SET PatPlanNum="+table.Rows[i][1].ToString()
						+" WHERE CovPatNum="+table.Rows[i][0].ToString();
					Db.NonQ32(command);
				}
				//secondary
				command="SELECT covpat.CovPatNum,patplan.PatPlanNum FROM covpat,patplan "
					+"WHERE covpat.PriPatNum=patplan.PatNum "
					+"AND patplan.Ordinal=2";
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="UPDATE covpat SET PatPlanNum="+table.Rows[i][1].ToString()
						+" WHERE CovPatNum="+table.Rows[i][0].ToString();
					Db.NonQ32(command);
				}
				//set patient.HasInsurance for everyone-----------------------------------------------------------------------
				command="SELECT DISTINCT PatNum FROM patplan";
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="UPDATE patient SET HasIns='I'"
						+" WHERE PatNum="+table.Rows[i][0].ToString();
					Db.NonQ32(command);
				}
				//delete unwanted columns-------------------------------------------------------------------------------------
				string[] commands=new string[] {
					"ALTER TABLE covpat DROP PriPatNum"
					,"ALTER TABLE covpat DROP SecPatNum"
					,"ALTER TABLE patient DROP PriPlanNum"
					,"ALTER TABLE patient DROP SecPlanNum"
					,"ALTER TABLE patient DROP PriRelationship"
					,"ALTER TABLE patient DROP SecRelationship"
					,"ALTER TABLE patient DROP RecallInterval"
					,"ALTER TABLE patient DROP RecallStatus"
					,"ALTER TABLE patient DROP PriPending"
					,"ALTER TABLE patient DROP SecPending"
					,"ALTER TABLE patient DROP PriPatID"
					,"ALTER TABLE patient DROP SecPatID"
					,"ALTER TABLE insplan DROP Carrier"
					,"ALTER TABLE insplan DROP Phone"
					,"ALTER TABLE insplan DROP Address"
					,"ALTER TABLE insplan DROP Address2"
					,"ALTER TABLE insplan DROP City"
					,"ALTER TABLE insplan DROP State"
					,"ALTER TABLE insplan DROP Zip"
					,"ALTER TABLE insplan DROP NoSendElect"
					,"ALTER TABLE insplan DROP ElectID"
					,"ALTER TABLE insplan DROP Employer"
				};
				Db.NonQ32(commands);
				//final cleanup----------------------------------------------------------------------------------------------
				command="UPDATE preference SET ValueString = '3.9.0.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_9_1();
		}

		private static void To3_9_1() {
			if(FromVersion < new Version("3.9.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.9.1");//No translation in convert script.
				string command="UPDATE preference SET PrefName = 'BackupToPath' WHERE PrefName = 'BackupPath'";
				Db.NonQ32(command);
				command="INSERT INTO preference VALUES ('BackupFromPath', '"+POut.String(@"C:\mysql\data\")+"')";
				Db.NonQ32(command);
				command="INSERT INTO preference VALUES ('BackupRestoreFromPath', '"+POut.String(@"D:\")+"')";
				Db.NonQ32(command);
				command="INSERT INTO preference VALUES ('BackupRestoreToPath', '"+POut.String(@"C:\mysql\data\")+"')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '3.9.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_9_2();
		}

		private static void To3_9_2() {
			if(FromVersion < new Version("3.9.2.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.9.2");//No translation in convert script.
				//DBSWin link-----------------------------------------------------------------------
				string command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'DBSWin', "
					+"'DBSWin from www.duerruk.com', "
					+"'0', "
					+"'', "
					+"'', "
					+"'"+POut.String(@"No command line or path is needed.")+"')";
				int programNum=Db.NonQ32(command,true);//we now have a ProgramNum to work with
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+programNum.ToString()+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0')";
				Db.NonQ32(command);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+programNum.ToString()+"', "
					+"'Text file path', "
					+"'"+POut.String(@"C:\patdata.txt")+"')";
				Db.NonQ32(command);
				command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					+"VALUES ("
					+"'"+programNum.ToString()+"', "
					+"'"+((int)ToolBarsAvail.ChartModule).ToString()+"', "
					+"'DBSWin')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '3.9.2.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_9_3();
		}

		private static void To3_9_3() {
			if(FromVersion < new Version("3.9.3.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.9.3");//No translation in convert script.
				string command="UPDATE preference SET ValueString = '-1' WHERE PrefName = 'InsBillingProv' AND ValueString='1'";
				Db.NonQ32(command);
				//Add diagnosis fields to HCFA-1500
				int claimFormNum;
				command="SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD4'";
				DataTable table=Db.GetTable(command);
				if(table.Rows.Count>0) {
					claimFormNum=PIn.Int(table.Rows[0][0].ToString());
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P1Diagnosis',446,749,75,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P2Diagnosis',446,781,75,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P3Diagnosis',446,816,75,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P4Diagnosis',446,849,75,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P5Diagnosis',446,882,75,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P6Diagnosis',446,915,75,16)";
					Db.NonQ32(command);
				}
				command="SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD5'";
				table=Db.GetTable(command);
				if(table.Rows.Count>0) {
					claimFormNum=PIn.Int(table.Rows[0][0].ToString());
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P1Diagnosis',446,749,75,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P2Diagnosis',446,781,75,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P3Diagnosis',446,816,75,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P4Diagnosis',446,849,75,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P5Diagnosis',446,882,75,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P6Diagnosis',446,915,75,16)";
					Db.NonQ32(command);
				}
				command="ALTER TABLE procedurelog ADD IsPrincDiag tinyint unsigned NOT NULL";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '3.9.3.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_9_4();
		}

		private static void To3_9_4() {
			if(FromVersion < new Version("3.9.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.9.4");//No translation in convert script.
				string command="INSERT INTO preference VALUES ('BillingIncludeChanged', '1')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '3.9.4.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_9_5();
		}

		private static void To3_9_5() {
			if(FromVersion < new Version("3.9.5.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.9.5");//No translation in convert script.
				string command="INSERT INTO preference VALUES ('BackupRestoreAtoZToPath', '"+POut.String(@"C:\OpenDentalData\")+"')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '3.9.5.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_9_6();
		}

		private static void To3_9_6() {
			if(FromVersion < new Version("3.9.6.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.9.6");//No translation in convert script.
				string command="ALTER TABLE referral CHANGE PatNum PatNum int NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE refattach CHANGE PatNum PatNum int NOT NULL";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '3.9.6.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_9_8();
		}

		private static void To3_9_8() {
			if(FromVersion < new Version("3.9.8.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.9.8");//No translation in convert script.
				//DentX link-----------------------------------------------------------------------
				string command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'DentX', "
					+"'ProImage from www.dent-x.com', "
					+"'0', "
					+"'', "
					+"'', "
					+"'"+POut.String(@"No command line or path is needed.")+"')";
				int programNum=Db.NonQ32(command,true);//we now have a ProgramNum to work with
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+programNum.ToString()+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0')";
				Db.NonQ32(command);
				command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					+"VALUES ("
					+"'"+programNum.ToString()+"', "
					+"'"+((int)ToolBarsAvail.ChartModule).ToString()+"', "
					+"'DentX')";
				Db.NonQ32(command);
				//Lightyear bridge--------------------------------------------------------------------------
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'Lightyear', "
					+"'SpeedVision from www.lightyeardirect.com', "
					+"'0', "
					+"'"+POut.String(@"C:\Program Files\Speedvision\speedvision.exe")+"', "
					+"'', "
					+"'"+POut.String(@"Path is usually C:\Program Files\Speedvision\speedvision.exe.  No command line is needed.")+"')";
				programNum=Db.NonQ32(command,true);//we now have a ProgramNum to work with
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+programNum.ToString()+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0')";
				Db.NonQ32(command);
				command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					+"VALUES ("
					+"'"+programNum.ToString()+"', "
					+"'"+((int)ToolBarsAvail.ChartModule).ToString()+"', "
					+"'Lightyear')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '3.9.8.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_9_9();
		}

		private static void To3_9_9() {
			if(FromVersion < new Version("3.9.9.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.9.9");//No translation in convert script.
				//TrackNPost clearinghouse
				string command="INSERT INTO clearinghouse(Description,ExportPath,IsDefault,Payors,Eformat,ReceiverID,"
					+"SenderID,Password,ResponsePath,CommBridge,ClientProgram) "
					+@"VALUES('Post-n-Track','C:\\PostnTrack\\Exports\\','0','','1','PostnTrack','','',"
					+@"'C:\\PostnTrack\\Reports\\','8','')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '3.9.9.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_9_17();
		}

		private static void To3_9_17() {
			if(FromVersion < new Version("3.9.17.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.9.17");//No translation in convert script.
				//Rename VixWin to VixWinOld-----------------------------------------------------------------------
				string command="UPDATE program SET ProgName='VixWinOld' WHERE ProgName='VixWin'";
				Db.NonQ32(command);
				//Add new VixWin bridge---------------------------------------------------------------------------
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'VixWin', "
					+"'VixWin(new) from www.gendexxray.com', "
					+"'0', "
					+"'"+POut.String(@"C:\VixWin\VixWin.exe")+"', "
					+"'', "
					+"'')";
				int programNum=Db.NonQ32(command,true);//we now have a ProgramNum to work with
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+programNum.ToString()+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0')";
				Db.NonQ32(command);
				command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					+"VALUES ("
					+"'"+programNum.ToString()+"', "
					+"'"+((int)ToolBarsAvail.ChartModule).ToString()+"', "
					+"'VixWin')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '3.9.17.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To3_9_18();
		}

		private static void To3_9_18() {
			if(FromVersion < new Version("3.9.18.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 3.9.18");//No translation in convert script.
				//fixes random keys problems:
				string command="ALTER TABLE referral CHANGE ReferralNum ReferralNum mediumint unsigned NOT NULL auto_increment";
				Db.NonQ32(command);
				//these two lines were previously in place and must be accounted for.
				//command="ALTER TABLE patient CHANGE NextAptNum NextAptNum mediumint unsigned NOT NULL";
				//Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '3.9.18.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_0_0();
		}

		private static void To4_0_0() {
			if(FromVersion < new Version("4.0.0.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.0.0");//No translation in convert script.
				ExecuteFile(Path.Combine("Version 4 0 0", "convert_4_0_0.txt"));//Might throw an exception which we handle.
				//first, get rid of a slight database inconsistency------------------------------------------------------------  
				//In my database, I found 65 duplicate covpat entries for certain plans. Users would not notice.
				//Running this loop adds a few minutes to the process, but is unavoidable.
				//Add some indexes to make this query go faster
				string command="ALTER TABLE covpat ADD INDEX indexPlanNum (PlanNum)";
				Db.NonQ32(command);
				command="ALTER TABLE covpat ADD INDEX indexCovCatNum (CovCatNum)";
				Db.NonQ32(command);
				command="ALTER TABLE covpat ADD INDEX indexPatPlanNum (PatPlanNum)";
				Db.NonQ32(command);
				command="ALTER TABLE covpat ADD INDEX indexCovPatNum (CovPatNum)";
				Db.NonQ32(command);
				command=@"SELECT * FROM covpat c1
					WHERE EXISTS(SELECT * FROM covpat c2 
					WHERE c1.PlanNum=c2.PlanNum
					AND c1.CovCatNum=c2.CovCatNum
					AND c1.PatPlanNum=c2.PatPlanNum
					AND c1.CovPatNum<c2.CovPatNum)";
				DataTable table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="DELETE FROM covpat WHERE CovPatNum="+table.Rows[i][0].ToString();
					Db.NonQ32(command);
				}
				//Add a CovCat for Db------------------------------------------------------------------------------
				command="UPDATE covcat SET CovOrder=CovOrder+1";//Move all other covcats down one in order
				Db.NonQ32(command);
				command="INSERT INTO covcat (Description,DefaultPercent,IsPreventive,"
					+"CovOrder,IsHidden) VALUES('General',-1,0,0,0)";
				int covCatNumGeneral=Db.NonQ32(command,true);
				command="INSERT INTO covspan (CovCatNum,FromCode,ToCode) VALUES("+POut.Long(covCatNumGeneral)+",'D0000','D9999')";
				Db.NonQ32(command);
				//Add a note to all InsPlans that do not renew in Jan----------------------------------------------------------
				command="SELECT PlanNum,RenewMonth FROM insplan WHERE RenewMonth != '1'";
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="UPDATE insplan SET "
						+"PlanNote = CONCAT('BENEFIT YEAR BEGINS IN MONTH "+table.Rows[i][1].ToString()
						+". SET EFFECTIVE DATE TO MATCH.',PlanNote) "
						+"WHERE PlanNum='"+table.Rows[i][0].ToString()+"'";
					Db.NonQ32(command);
				}
				//Convert CovPats to Benefits---------------------------------------------------------------------------------
				command="SELECT DISTINCT covpat.CovCatNum,covpat.PlanNum,covpat.Percent,covpat.PatPlanNum,"//0-3
					+"IFNULL(insplan.DeductWaivPrev,1),covcat.IsPreventive,insplan.Deductible,"//4-6
					+"IFNULL(insplan.RenewMonth,1) "//7
					+"FROM covpat "
					+"LEFT JOIN insplan ON covpat.PlanNum=insplan.PlanNum "
					+"LEFT JOIN covcat ON covpat.CovCatNum=covcat.CovCatNum";
				Debug.WriteLine(command);
				//+"ORDER BY covpat.PatPlanNum DESC";
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					//percentages
					command="INSERT INTO benefit (PlanNum,PatPlanNum,CovCatNum,BenefitType,Percent,MonetaryAmt,TimePeriod"
						+") VALUES("
						+"'"+table.Rows[i][1].ToString()+"', "//planNum=1
						+"'"+table.Rows[i][3].ToString()+"', "//patPlanNum=3
						+"'"+table.Rows[i][0].ToString()+"', "//CovCatNum=0
						+"'1', "//benefitType=Percentage
						+"'"+table.Rows[i][2].ToString()+"', "//Percent=2
						+"'0', ";//MonetaryAmt
					if(table.Rows[i][7].ToString()=="1") {//RenewMonth=Jan
						command+="'2')";//TimePeriod=CalendarYear
					}
					else {
						command+="'1')";//TimePeriod=ServiceYear
					}
					Db.NonQ32(command);
					//deductibles waived on preventive
					if(table.Rows[i][6].ToString()=="-1"//deductible=-1(unknown)
						|| table.Rows[i][6].ToString()=="0"//deductible=0
						|| table.Rows[i][5].ToString()=="0"//not preventive
						|| table.Rows[i][4].ToString()=="-1"//deductWaivPrev=-1 (not known if waived)
						|| table.Rows[i][4].ToString()=="0")//deductWaivPrev=0 (not waived)
					{
						continue;
					}
					command="INSERT INTO benefit (PlanNum,PatPlanNum,CovCatNum,BenefitType,Percent,MonetaryAmt,TimePeriod"
						+") VALUES("
						+"'"+table.Rows[i][1].ToString()+"', "//planNum=1
						+"'"+table.Rows[i][3].ToString()+"', "//patPlanNum=3
						+"'"+table.Rows[i][0].ToString()+"', "//CovCatNum=0
						+"'2', "//benefitType=Deductible
						+"'0', "//Percent=3
						+"'0', ";//MonetaryAmt=0 since waived
					if(table.Rows[i][7].ToString()=="1") {//RenewMonth=Jan
						command+="'2')";//TimePeriod=CalendarYear
					}
					else {
						command+="'1')";//TimePeriod=ServiceYear
					}
					Db.NonQ32(command);
				}
				//Convert remaining InsPlan fields to benefits-------------------------------------------------------------
				command="SELECT PlanNum,AnnualMax,Deductible,FloToAge,MissToothExcl,MajorWait,OrthoMax,RenewMonth "
					+"FROM insplan";
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					//AnnualMax
					if(PIn.Double(table.Rows[i][1].ToString())>0) {//if there is an annual max
						command="INSERT INTO benefit (PlanNum,PatPlanNum,CovCatNum,BenefitType,Percent,MonetaryAmt,TimePeriod"
							+") VALUES("
							+"'"+table.Rows[i][0].ToString()+"', "//planNum
							+"'0',"//patPlanNum
							+"'"+POut.Long(covCatNumGeneral)+"',"//CovCatNum
							+"'"+POut.Long((int)InsBenefitType.Limitations)+"', "
							+"'0',"//percent
							+"'"+table.Rows[i][1].ToString()+"', ";//max
						if(table.Rows[i][7].ToString()=="1") {//RenewMonth=Jan
							command+="'"+POut.Long((int)BenefitTimePeriod.CalendarYear)+"')";
						}
						else {
							command+="'"+POut.Long((int)BenefitTimePeriod.ServiceYear)+"')";
						}
						Db.NonQ32(command);
					}
					//Deductible
					if(PIn.Double(table.Rows[i][2].ToString())>-1) {//if there is a deductible
						command="INSERT INTO benefit (PlanNum,PatPlanNum,CovCatNum,BenefitType,Percent,MonetaryAmt,TimePeriod"
							+") VALUES("
							+"'"+table.Rows[i][0].ToString()+"', "//planNum
							+"'0',"//patPlanNum
							+"'"+POut.Long(covCatNumGeneral)+"',"//CovCatNum
							+"'"+POut.Long((int)InsBenefitType.Deductible)+"', "
							+"'0',"//percent
							+"'"+table.Rows[i][2].ToString()+"', ";//deductible amt
						if(table.Rows[i][7].ToString()=="1") {//RenewMonth=Jan
							command+="'"+POut.Long((int)BenefitTimePeriod.CalendarYear)+"')";
						}
						else {
							command+="'"+POut.Long((int)BenefitTimePeriod.ServiceYear)+"')";
						}
						Db.NonQ32(command);
					}
					//FloToAge
					if(CultureInfo.CurrentCulture.Name=="en-US" && table.Rows[i][3].ToString() != "-1") {
						command="INSERT INTO benefit (PlanNum,PatPlanNum,CovCatNum,ADACode,BenefitType,Percent,MonetaryAmt,"
							+"TimePeriod,QuantityQualifier,Quantity) VALUES("
							+"'"+table.Rows[i][0].ToString()+"', "//planNum
							+"'0',"//patPlanNum
							+"'"+POut.Long(covCatNumGeneral)+"',"//CovCatNum=general. But ignored because of ADACode
							+"'D1204',"//ADACode for Adult Flo
							+"'"+POut.Long((int)InsBenefitType.Limitations)+"', "
							+"'0',"//percent
							+"'0', ";//amt
						if(table.Rows[i][7].ToString()=="1") {//RenewMonth=Jan
							command+="'"+POut.Long((int)BenefitTimePeriod.CalendarYear)+"', ";
						}
						else {
							command+="'"+POut.Long((int)BenefitTimePeriod.ServiceYear)+"', ";
						}
						command+="'"+POut.Long((int)BenefitQuantity.AgeLimit)+"', "
							+"'"+table.Rows[i][3].ToString()+"')";//this should work for 0,18, and 99
						Db.NonQ32(command);
					}
					//MissToothExcl
					if(table.Rows[i][4].ToString()!="0") {//if it's not unknown
						command="UPDATE insplan SET "
							+"PlanNote = CONCAT('Missing tooth exclusion: "+((YN)PIn.Long(table.Rows[i][4].ToString())).ToString()
							+". ',PlanNote) "
							+"WHERE PlanNum='"+table.Rows[i][0].ToString()+"'";
						Db.NonQ32(command);
					}
					//MajorWait
					if(table.Rows[i][5].ToString()!="0") {//if it's not unknown
						command="UPDATE insplan SET "
							+"PlanNote = CONCAT('Wait on major: "+((YN)PIn.Long(table.Rows[i][5].ToString())).ToString()
							+". ',PlanNote) "
							+"WHERE PlanNum='"+table.Rows[i][0].ToString()+"'";
						Db.NonQ32(command);
					}
					//OrthoMax
					if(PIn.Long(table.Rows[i][6].ToString())>0) {//not -1 or 0
						command="UPDATE insplan SET "
							+"PlanNote = CONCAT('Ortho Max: "+table.Rows[i][6].ToString()
							+". ',PlanNote) "
							+"WHERE PlanNum='"+table.Rows[i][0].ToString()+"'";
						Db.NonQ32(command);
					}
				}
				string[] commands=new string[]
				{
					 "ALTER TABLE insplan DROP AnnualMax"
					,"ALTER TABLE insplan DROP RenewMonth"
					,"ALTER TABLE insplan DROP Deductible"
					,"ALTER TABLE insplan DROP DeductWaivPrev"
					,"ALTER TABLE insplan DROP OrthoMax"
					,"ALTER TABLE insplan DROP FloToAge"
					,"ALTER TABLE insplan DROP MissToothExcl"
					,"ALTER TABLE insplan DROP MajorWait"
					,"ALTER TABLE insplan DROP IsWrittenOff"
					,"DROP TABLE covpat"
					,"ALTER TABLE covcat DROP IsPreventive"
					,"ALTER TABLE covcat ADD EbenefitCat tinyint unsigned NOT NULL"
					,"UPDATE insplan SET SubscNote=PlanNote"
					,"UPDATE insplan SET PlanNote=''"
				};
				Db.NonQ32(commands);
				//Add enhanced Trophy bridge---------------------------------------------------------------------------
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'TrophyEnhanced', "
					+"'Trophy(enhanced) from www.trophy-imaging.com', "
					+"'0', "
					+"'"+POut.String(@"TW.exe")+"', "
					+"'', "
					+"'"+POut.String(@"The storage path is where all images are stored.  For instance \\SERVER\TrophyImages (no trailing \).  Each patient must also have a folder specified in the patient edit window.  For instance S\SmithJohn or whatever the current folder structure is.")+"')";
				int programNum=Db.NonQ32(command,true);//we now have a ProgramNum to work with
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+programNum.ToString()+"', "
					+"'Storage Path', "
					+"'')";
				Db.NonQ32(command);
				command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					+"VALUES ("
					+"'"+programNum.ToString()+"', "
					+"'"+((int)ToolBarsAvail.ChartModule).ToString()+"', "
					+"'Trophy')";
				Db.NonQ32(command);
				//Add DentalEye bridge----------------------------------------------------------------------------------------
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'DentalEye', "
					+"'DentalEye from www.dentaleye.com', "
					+"'0', "
					+"'"+POut.String(@"C:\DentalEye\DentalEye.exe")+"', "
					+"'', "
					+"'')";
				programNum=Db.NonQ32(command,true);//we now have a ProgramNum to work with
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+programNum.ToString()+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0')";
				Db.NonQ32(command);
				command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					+"VALUES ("
					+"'"+programNum.ToString()+"', "
					+"'"+((int)ToolBarsAvail.ChartModule).ToString()+"', "
					+"'DentalEye')";
				Db.NonQ32(command);
				//Add lab fee fields to Canadian claim form
				int claimFormNum;
				command="SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD6'";
				table=Db.GetTable(command);
				if(table.Rows.Count>0) {
					claimFormNum=PIn.Int(table.Rows[0][0].ToString());
					//get rid of the existing dentist fee column.
					command="DELETE FROM claimformitem WHERE ClaimFormNum='"+POut.Long(claimFormNum)
						+"' AND XPos=342 AND FieldName LIKE '%Fee'";
					Db.NonQ32(command);
					//add the lab fee column
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P1Lab',440,394,66,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P2Lab',440,411,66,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P3Lab',440,428,66,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P4Lab',440,445,66,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P5Lab',440,462,66,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P6Lab',440,479,66,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P7Lab',440,496,66,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P8Lab',440,513,66,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P9Lab',440,530,66,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P10Lab',440,547,66,16)";
					Db.NonQ32(command);
					//add the dentist fee column (fee-lab)
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P1FeeMinusLab',342,394,62,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P2FeeMinusLab',342,411,62,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P3FeeMinusLab',342,428,62,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P4FeeMinusLab',342,445,62,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P5FeeMinusLab',342,462,62,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P6FeeMinusLab',342,479,62,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P7FeeMinusLab',342,496,62,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P8FeeMinusLab',342,513,62,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P9FeeMinusLab',342,530,62,16)";
					Db.NonQ32(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES("
						+POut.Long(claimFormNum)+",'P10FeeMinusLab',342,547,62,16)";
					Db.NonQ32(command);
					//make dates wider so the year doesn't get cut off
					command="UPDATE claimformitem SET Width=85,XPos=28 "
						+"WHERE FieldName LIKE 'P%Date' AND XPos=38 "
						+"AND ClaimFormNum="+POut.Long(claimFormNum);
					Db.NonQ32(command);
				}
				//add chart of accounts-----------------------------------------------------------------
				commands=new string[]
				{
					 "INSERT INTO account (Description,AcctType) VALUES('Checking Account',0)"
					,"INSERT INTO account (Description,AcctType) VALUES('Cash Box',0)"
					,"INSERT INTO account (Description,AcctType) VALUES('Employee Advances',0)"
					,"INSERT INTO account (Description,AcctType) VALUES('Equipment',0)"
					,"INSERT INTO account (Description,AcctType) VALUES('Accumulated Depreciation, Equipment',0)"
					,"INSERT INTO account (Description,AcctType) VALUES('Bank Loans Payable',1)"
					,"INSERT INTO account (Description,AcctType) VALUES('Stated Capital',2)"
					,"INSERT INTO account (Description,AcctType) VALUES('Retained Earnings',2)"
					,"INSERT INTO account (Description,AcctType) VALUES('Patient Fee Income',3)"
					,"INSERT INTO account (Description,AcctType) VALUES('Employee Benefits',4)"
					,"INSERT INTO account (Description,AcctType) VALUES('Supplies',4)"
					,"INSERT INTO account (Description,AcctType) VALUES('Services',4)"
					,"INSERT INTO account (Description,AcctType) VALUES('Wages',4)"
				};
				Db.NonQ32(commands);
				command="UPDATE preference SET ValueString = '4.0.0.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_0_2();
		}

		private static void To4_0_2() {
			if(FromVersion < new Version("4.0.2.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.0.2");//No translation in convert script.
				string command="ALTER TABLE account ADD Inactive tinyint unsigned NOT NULL";
				Db.NonQ32(command);
				//add accounting permission to each admin group------------------------------------------------------
				command="SELECT UserGroupNum FROM grouppermission "
					+"WHERE PermType="+POut.Long((int)Permissions.SecurityAdmin);
				DataTable table=Db.GetTable(command);
				int groupNum;
				for(int i=0;i<table.Rows.Count;i++) {
					groupNum=PIn.Int(table.Rows[i][0].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						+"VALUES("+POut.Long(groupNum)+","+POut.Long((int)Permissions.AccountingCreate)+")";
					Db.NonQ32(command);
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						+"VALUES("+POut.Long(groupNum)+","+POut.Long((int)Permissions.AccountingEdit)+")";
					Db.NonQ32(command);
				}
				//fix the planned appointment 'done' feature--------------------------------------------------------------
				command="ALTER TABLE patient ADD PlannedIsDone tinyint unsigned NOT NULL";
				Db.NonQ32(command);
				command="UPDATE patient SET PlannedIsDone=1 WHERE NextAptNum = -1";
				Db.NonQ32(command);
				//these two lines were previously in place in version 3.9.18.  Calling them again doesn't hurt
				command="ALTER TABLE patient CHANGE NextAptNum NextAptNum mediumint unsigned NOT NULL";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.0.2.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_0_3();
		}

		private static void To4_0_3() {
			if(FromVersion < new Version("4.0.3.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.0.3");//No translation in convert script.
				string command="ALTER TABLE account ADD AccountColor int NOT NULL";
				Db.NonQ32(command);
				command="UPDATE account SET AccountColor = -1";//white
				Db.NonQ32(command);
				command="INSERT INTO preference VALUES ('AccountingDepositAccounts','')";
				Db.NonQ32(command);
				command="INSERT INTO preference VALUES ('AccountingIncomeAccount','')";
				Db.NonQ32(command);
				//two of these were simply deleted in the very next upgrade.
				//command="INSERT INTO preference VALUES ('AccountingCashDepAccounts','')";
				//Db.NonQ32(command);
				command="INSERT INTO preference VALUES ('AccountingCashIncomeAccount','')";
				Db.NonQ32(command);
				//command="INSERT INTO preference VALUES ('AccountingCashPaymentType','')";
				//Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.0.3.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_0_4();
		}

		private static void To4_0_4() {
			if(FromVersion < new Version("4.0.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.0.4");//No translation in convert script.
				string command=@"CREATE TABLE accountingautopay(
					AccountingAutoPayNum mediumint unsigned NOT NULL auto_increment,
					PayType smallint unsigned NOT NULL,
					PickList varchar(255) NOT NULL,
					PRIMARY KEY (AccountingAutoPayNum)
					) DEFAULT CHARSET=utf8;";
				Db.NonQ32(command);
				command="DELETE FROM preference WHERE PrefName='AccountingCashDepAccounts'";
				Db.NonQ32(command);
				command="DELETE FROM preference WHERE PrefName='AccountingCashPaymentType'";
				Db.NonQ32(command);
				command="ALTER TABLE transaction ADD PayNum mediumint unsigned NOT NULL";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.0.4.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_0_5();
		}

		private static void To4_0_5() {
			if(FromVersion < new Version("4.0.5.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.0.5");//No translation in convert script.
				string command=@"CREATE TABLE reconcile(
					ReconcileNum mediumint unsigned NOT NULL auto_increment,
					AccountNum mediumint unsigned NOT NULL,
					StartingBal double NOT NULL,
					EndingBal double NOT NULL,
					DateReconcile date NOT NULL default '0001-01-01',
					IsLocked tinyint unsigned NOT NULL,
					PRIMARY KEY (ReconcileNum)
					) DEFAULT CHARSET=utf8;";
				Db.NonQ32(command);
				command="ALTER TABLE journalentry ADD ReconcileNum mediumint unsigned NOT NULL";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.0.5.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_0_9();
		}

		private static void To4_0_9() {
			if(FromVersion < new Version("4.0.9.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.0.9");//No translation in convert script.
				string command="INSERT INTO preference VALUES ('SkipComputeAgingInAccount','0')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.0.9.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_0_10();
		}

		private static void To4_0_10() {
			if(FromVersion < new Version("4.0.10.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.0.10");//No translation in convert script.
				//Add Trojan bridge----------------------------------------------------------------------------------------
				string command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'Trojan', "
					+"'Trojan from www.trojanonline.com', "
					+"'0', "
					+"'', "
					+"'', "
					+"'No path is needed.  No buttons are available.  Uses the standalone Trojan program.')";
				Db.NonQ32(command);
				//Add IAP bridge----------------------------------------------------------------------------------------
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'IAP', "
					+"'Insurance Answers Plus from www.iaplus.com', "
					+"'0', "
					+"'', "
					+"'', "
					+"'No path is needed.  No buttons are available.')";
				Db.NonQ32(command);
				//fix referrals
				command="ALTER TABLE refattach CHANGE ReferralNum ReferralNum mediumint unsigned NOT NULL";
				Db.NonQ32(command);
				//disable medical claims
				command="INSERT INTO preference VALUES ('MedicalEclaimsEnabled','0')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.0.10.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_0_11();
		}

		private static void To4_0_11() {
			if(FromVersion < new Version("4.0.11.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.0.11");//No translation in convert script.
				//delete all percentages for medicaid and capitation plans
				string command=@"SELECT BenefitNum
					FROM insplan,benefit 
					WHERE benefit.PlanNum=insplan.PlanNum
					AND (PlanType='f' OR PlanType='c')
					AND BenefitType=1";
				DataTable table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++){
					command="DELETE FROM benefit WHERE BenefitNum="+table.Rows[i][0].ToString();
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '4.0.11.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_0_13();
		}

		private static void To4_0_13() {
			if(FromVersion < new Version("4.0.13.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.0.13");//No translation in convert script.
				//Add sales tax fields. Even though we will not use them yet, some customers might make user of them. 
				string command="INSERT INTO preference VALUES ('SalesTaxPercentage','0')";
				Db.NonQ32(command);
				command="ALTER TABLE procedurecode ADD IsTaxed tinyint unsigned NOT NULL";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.0.13.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_1_0();
		}

		private static void To4_1_0() {
			if(FromVersion < new Version("4.1.0.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.1.0");//No translation in convert script.
				string command;
				if(CultureInfo.CurrentCulture.Name=="en-US"){
					//Convert CovCats to new names and ranges----------------------------------------------------------------------
					//Db
					command="UPDATE covcat SET EbenefitCat=1 WHERE Description='General'";
					Db.NonQ32(command);
					//Hide all previous covcats.
					command="UPDATE covcat SET IsHidden=1 WHERE Description != 'General'";
					Db.NonQ32(command);
					//Create all the new cateories from scratch
					int covCatNum;
					//Diagnostic
					command="INSERT INTO covcat (Description,DefaultPercent,EbenefitCat) VALUES('Diagnostic','100','"
						+POut.Long((int)EbenefitCategory.Diagnostic)+"')";
					covCatNum=Db.NonQ32(command,true);
					command="INSERT INTO covspan (CovCatNum,FromCode,ToCode) VALUES("+POut.Long(covCatNum)+",'D0000','D0999')";
					Db.NonQ32(command);
					//RoutinePreventive
					command="INSERT INTO covcat (Description,DefaultPercent,EbenefitCat) VALUES('Preventive','100','"
						+POut.Long((int)EbenefitCategory.RoutinePreventive)+"')";
					covCatNum=Db.NonQ32(command,true);
					command="INSERT INTO covspan (CovCatNum,FromCode,ToCode) VALUES("+POut.Long(covCatNum)+",'D1000','D1999')";
					Db.NonQ32(command);
					//Restorative
					command="INSERT INTO covcat (Description,DefaultPercent,EbenefitCat) VALUES('Restorative','80','"
						+POut.Long((int)EbenefitCategory.Restorative)+"')";
					covCatNum=Db.NonQ32(command,true);
					command="INSERT INTO covspan (CovCatNum,FromCode,ToCode) VALUES("+POut.Long(covCatNum)+",'D2000','D2999')";
					Db.NonQ32(command);
					//Endo
					command="INSERT INTO covcat (Description,DefaultPercent,EbenefitCat) VALUES('Endo','80','"
						+POut.Long((int)EbenefitCategory.Endodontics)+"')";
					covCatNum=Db.NonQ32(command,true);
					command="INSERT INTO covspan (CovCatNum,FromCode,ToCode) VALUES("+POut.Long(covCatNum)+",'D3000','D3999')";
					Db.NonQ32(command);
					//Perio
					command="INSERT INTO covcat (Description,DefaultPercent,EbenefitCat) VALUES('Perio','80','"
						+POut.Long((int)EbenefitCategory.Periodontics)+"')";
					covCatNum=Db.NonQ32(command,true);
					command="INSERT INTO covspan (CovCatNum,FromCode,ToCode) VALUES("+POut.Long(covCatNum)+",'D4000','D4999')";
					Db.NonQ32(command);
					//OralSurgery
					command="INSERT INTO covcat (Description,DefaultPercent,EbenefitCat) VALUES('Oral Surgery','80','"
						+POut.Long((int)EbenefitCategory.OralSurgery)+"')";
					covCatNum=Db.NonQ32(command,true);
					command="INSERT INTO covspan (CovCatNum,FromCode,ToCode) VALUES("+POut.Long(covCatNum)+",'D7000','D7999')";
					Db.NonQ32(command);
					//Crowns
					command="INSERT INTO covcat (Description,DefaultPercent,EbenefitCat) VALUES('Crowns','50','"
						+POut.Long((int)EbenefitCategory.Crowns)+"')";
					covCatNum=Db.NonQ32(command,true);
					command="INSERT INTO covspan (CovCatNum,FromCode,ToCode) VALUES("+POut.Long(covCatNum)+",'D2700','D2799')";
					Db.NonQ32(command);
					//Prosth
					command="INSERT INTO covcat (Description,DefaultPercent,EbenefitCat) VALUES('Prosth','50','"
						+POut.Long((int)EbenefitCategory.Prosthodontics)+"')";
					covCatNum=Db.NonQ32(command,true);
					command="INSERT INTO covspan (CovCatNum,FromCode,ToCode) VALUES("+POut.Long(covCatNum)+",'D5000','D5899')";
					Db.NonQ32(command);
					command="INSERT INTO covspan (CovCatNum,FromCode,ToCode) VALUES("+POut.Long(covCatNum)+",'D6200','D6899')";
					Db.NonQ32(command);
					//MaxProsth
					command="INSERT INTO covcat (Description,DefaultPercent,EbenefitCat) VALUES('Maxillofacial Prosth','-1','"
						+POut.Long((int)EbenefitCategory.MaxillofacialProsth)+"')";
					covCatNum=Db.NonQ32(command,true);
					command="INSERT INTO covspan (CovCatNum,FromCode,ToCode) VALUES("+POut.Long(covCatNum)+",'D5900','D5999')";
					Db.NonQ32(command);
					//Accident
					command="INSERT INTO covcat (Description,DefaultPercent,EbenefitCat) VALUES('Accident','-1','"
						+POut.Long((int)EbenefitCategory.Accident)+"')";
					covCatNum=Db.NonQ32(command,true);
					//Ortho
					command="INSERT INTO covcat (Description,DefaultPercent,EbenefitCat) VALUES('Ortho','-1','"
						+POut.Long((int)EbenefitCategory.Orthodontics)+"')";
					covCatNum=Db.NonQ32(command,true);
					command="INSERT INTO covspan (CovCatNum,FromCode,ToCode) VALUES("+POut.Long(covCatNum)+",'D8000','D8999')";
					Db.NonQ32(command);
					//Then, order everything
					command="SELECT * FROM covcat ORDER BY "
						+"EbenefitCat != "+POut.Long((int)EbenefitCategory.General)
						+",EbenefitCat != "+POut.Long((int)EbenefitCategory.Diagnostic)
						+",EbenefitCat != "+POut.Long((int)EbenefitCategory.RoutinePreventive)
						+",EbenefitCat != "+POut.Long((int)EbenefitCategory.Restorative)
						+",EbenefitCat != "+POut.Long((int)EbenefitCategory.Endodontics)
						+",EbenefitCat != "+POut.Long((int)EbenefitCategory.Periodontics)
						+",EbenefitCat != "+POut.Long((int)EbenefitCategory.OralSurgery)
						+",EbenefitCat != "+POut.Long((int)EbenefitCategory.Crowns)//subcategory of Restorative
						+",EbenefitCat != "+POut.Long((int)EbenefitCategory.Prosthodontics)
						+",EbenefitCat != "+POut.Long((int)EbenefitCategory.MaxillofacialProsth)
						+",EbenefitCat != "+POut.Long((int)EbenefitCategory.Accident)
						+",EbenefitCat != "+POut.Long((int)EbenefitCategory.Orthodontics);
					DataTable table=Db.GetTable(command);
					for(int i=0;i<table.Rows.Count;i++){
						command="UPDATE covcat SET CovOrder="+POut.Long(i)+" WHERE CovCatNum="+table.Rows[i][0].ToString();
						Db.NonQ32(command);
					}
				}
				command="ALTER TABLE fee ADD INDEX indexADACode (ADACode)";
				Db.NonQ32(command);
				command="ALTER TABLE fee ADD INDEX indexFeeSched (FeeSched)";
				Db.NonQ32(command);
				//ProcButton categories---------------------------------------------------------------------------------
				command="ALTER TABLE procbutton ADD Category smallint unsigned NOT NULL";
				Db.NonQ32(command);
				command="INSERT INTO definition (category,itemorder,itemname) VALUES(26,0,'All')";
				int defNum=Db.NonQ32(command,true);
				command="UPDATE procbutton SET Category="+POut.Long(defNum);
				Db.NonQ32(command);
				command="ALTER TABLE procbutton ADD ButtonImage text NOT NULL";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.1.0.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_1_2();
		}

		private static void To4_1_2() {
			if(FromVersion < new Version("4.1.2.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.1.2");//No translation in convert script.
				string command;
				command="DELETE FROM preference WHERE PrefName= 'SkipComputeAgingInAccount'";
				Db.NonQ32(command);
				command="INSERT INTO preference VALUES ('StatementShowReturnAddress','1')";
				Db.NonQ32(command);
				command="INSERT INTO preference VALUES ('ShowIDinTitleBar','0')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.1.2.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_2_0();
		}

		private static void To4_2_0() {
			if(FromVersion < new Version("4.2.0.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.2.0");//No translation in convert script.
				string command;
				//string[] commands;//=new string[] {
				command="ALTER TABLE procedurecode ADD PaintType tinyint NOT NULL";
				Db.NonQ32(command);
				command="SELECT * FROM definition WHERE Category=22 ORDER BY ItemOrder";
				DataTable table=Db.GetTable(command);
				Color cDark;
				Color cLight;
				for(int i=0;i<table.Rows.Count;i++){
					cDark=Color.FromArgb(PIn.Int(table.Rows[i][5].ToString()));
					cLight=Color.FromArgb((cDark.R+255)/2,(cDark.G+255)/2,(cDark.B+255)/2);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor) VALUES(22,"
						+POut.Long(i+5)+",'"+POut.String(PIn.String(table.Rows[i][3].ToString())+" (light)")
						+"','" +POut.Long(cLight.ToArgb())+"')";
					Db.NonQ32(command);
				}
				//Conversions to painting type are listed in order previously displayed.
				command="UPDATE procedurecode SET PaintType="+POut.Long((int)ToothPaintingType.Extraction)+" WHERE GTypeNum=1";
				Db.NonQ32(command);
				command="UPDATE procedurecode SET PaintType="+POut.Long((int)ToothPaintingType.Implant)+" WHERE GTypeNum=10";
				Db.NonQ32(command);
				command="UPDATE procedurecode SET PaintType="+POut.Long((int)ToothPaintingType.RCT)+" WHERE GTypeNum=4";
				Db.NonQ32(command);
				command="UPDATE procedurecode SET PaintType="+POut.Long((int)ToothPaintingType.PostBU)+" WHERE GTypeNum=5";
				Db.NonQ32(command);
				command="UPDATE procedurecode SET PaintType="+POut.Long((int)ToothPaintingType.FillingDark)+" WHERE GTypeNum=2";
				Db.NonQ32(command);
				command="UPDATE procedurecode SET PaintType="+POut.Long((int)ToothPaintingType.FillingLight)+" WHERE GTypeNum=3";
				Db.NonQ32(command);
				command="UPDATE procedurecode SET PaintType="+POut.Long((int)ToothPaintingType.FillingLight)+" WHERE GTypeNum=19";
				Db.NonQ32(command);
				command="UPDATE procedurecode SET PaintType="+POut.Long((int)ToothPaintingType.FillingLight)+" WHERE GTypeNum=11";
				Db.NonQ32(command);
				command="UPDATE procedurecode SET PaintType="+POut.Long((int)ToothPaintingType.CrownDark)+" WHERE GTypeNum=6";
				Db.NonQ32(command);
				command="UPDATE procedurecode SET PaintType="+POut.Long((int)ToothPaintingType.CrownLight)+" WHERE GTypeNum=7";
				Db.NonQ32(command);
				command="UPDATE procedurecode SET PaintType="+POut.Long((int)ToothPaintingType.CrownLight)+" WHERE GTypeNum=20";
				Db.NonQ32(command);
				command="UPDATE procedurecode SET PaintType="+POut.Long((int)ToothPaintingType.CrownLight)+" WHERE GTypeNum=9";
				Db.NonQ32(command);
				command="UPDATE procedurecode SET PaintType="+POut.Long((int)ToothPaintingType.BridgeDark)+" WHERE GTypeNum=12";
				Db.NonQ32(command);
				command="UPDATE procedurecode SET PaintType="+POut.Long((int)ToothPaintingType.BridgeLight)+" WHERE GTypeNum=13";
				Db.NonQ32(command);
				command="UPDATE procedurecode SET PaintType="+POut.Long((int)ToothPaintingType.BridgeLight)+" WHERE GTypeNum=21";
				Db.NonQ32(command);
				command="UPDATE procedurecode SET PaintType="+POut.Long((int)ToothPaintingType.BridgeLight)+" WHERE GTypeNum=14";
				Db.NonQ32(command);
				//veneer not painted
				command="UPDATE procedurecode SET PaintType="+POut.Long((int)ToothPaintingType.DentureDark)+" WHERE GTypeNum=24";
				Db.NonQ32(command);
				command="UPDATE procedurecode SET PaintType="+POut.Long((int)ToothPaintingType.DentureLight)+" WHERE GTypeNum=25";
				Db.NonQ32(command);
				command="UPDATE procedurecode SET PaintType="+POut.Long((int)ToothPaintingType.DentureLight)+" WHERE GTypeNum=26";
				Db.NonQ32(command);
				command="UPDATE procedurecode SET PaintType="+POut.Long((int)ToothPaintingType.DentureLight)+" WHERE GTypeNum=27";
				Db.NonQ32(command);
				command=@"CREATE TABLE toothinitial(
					ToothInitialNum mediumint unsigned NOT NULL auto_increment,
					PatNum mediumint unsigned NOT NULL,
					ToothNum varchar(2) NOT NULL,
					InitialType tinyint unsigned NOT NULL,
          Movement float NOT NULL,
					PRIMARY KEY (ToothInitialNum)
					) DEFAULT CHARSET=utf8";
				Db.NonQ32(command);
				//convert all previous extractions to missing teeth.
				command="SELECT PatNum,ToothNum FROM procedurelog,procedurecode "
					+"WHERE procedurelog.ADACode=procedurecode.ADACode "
					+"AND procedurecode.RemoveTooth=1 "
					+"AND (procedurelog.ProcStatus=2 OR procedurelog.ProcStatus=3 OR procedurelog.ProcStatus=4)";
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="INSERT INTO toothinitial(PatNum,ToothNum,InitialType) VALUES("
						+table.Rows[i][0].ToString()+",'"
						+POut.String(PIn.String(table.Rows[i][1].ToString()))
						+"',0)";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '4.2.0.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_2_1();
		}

		private static void To4_2_1() {
			if(FromVersion < new Version("4.2.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.2.1");//No translation in convert script.
				string command="SELECT PatNum,PrimaryTeeth FROM patient WHERE PrimaryTeeth != ''";
				DataTable table=Db.GetTable(command);
				string[] priTeeth;
				for(int i=0;i<table.Rows.Count;i++){
					priTeeth=(PIn.String(table.Rows[i][1].ToString())).Split(new char[] {','});
					for(int t=0;t<priTeeth.Length;t++){
						if(priTeeth[t]==""){
							continue;
						}
						command="INSERT INTO toothinitial (PatNum,ToothNum,InitialType) VALUES("
							+table.Rows[i][0].ToString()
							+",'"+POut.String(priTeeth[t])+"',2)";
						Db.NonQ32(command);
					}
				}
				command="UPDATE preference SET ValueString = '4.2.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_2_8();
		}

		private static void To4_2_8() {
			if(FromVersion < new Version("4.2.8.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.2.8");//No translation in convert script.
				string command="UPDATE procedurecode SET PaintType=13, TreatArea=2 WHERE ADACode='D1351'";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.2.8.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_2_9();
		}

		private static void To4_2_9() {
			if(FromVersion < new Version("4.2.9.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.2.9");//No translation in convert script.
				string command;
				//Add Florida probe bridge----------------------------------------------------------------------------------------
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'FloridaProbe', "
					+"'Florida Probe from www.floridaprobe.com', "
					+"'0', "
					+"'"+POut.String(@"fp32")+"', "
					+"'', "
					+"'No command line is needed.')";
				int programNum=Db.NonQ32(command,true);//we now have a ProgramNum to work with
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+programNum.ToString()+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0')";
				Db.NonQ32(command);
				command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					+"VALUES ("
					+"'"+programNum.ToString()+"', "
					+"'"+((int)ToolBarsAvail.ChartModule).ToString()+"', "
					+"'Florida Probe')";
				Db.NonQ32(command);
				//Add Dr Ceph bridge----------------------------------------------------------------------------------------
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'DrCeph', "
					+"'Dr. Ceph from www.fyitek.com', "
					+"'0', "
					+"'', "
					+"'', "
					+"'No path or command line is needed.')";
				programNum=Db.NonQ32(command,true);//we now have a ProgramNum to work with
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+programNum.ToString()+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0')";
				Db.NonQ32(command);
				command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					+"VALUES ("
					+"'"+programNum.ToString()+"', "
					+"'"+((int)ToolBarsAvail.ChartModule).ToString()+"', "
					+"'Dr Ceph')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.2.9.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_2_10();
		}

		private static void To4_2_10() {
			if(FromVersion < new Version("4.2.10.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.2.10");//No translation in convert script.
				string command="ALTER TABLE procedurecode ADD GraphicColor int NOT NULL";
				Db.NonQ32(command);
				command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor) VALUES(12,6,"
						+"'CommLog',-65536)";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.2.10.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_3_0();
		}

		private static void To4_3_0(){
			if(FromVersion < new Version("4.3.0.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.3.0");//No translation in convert script.
				ExecuteFile(Path.Combine("Version 4 3 0","convert_4_3_0.txt"));//Might throw an exception which we handle.
				string command;
				//Add NewPatientForm bridge-----------------------------------------------------------------------------------
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'NewPatientForm.com', "
					+"'NewPatientForm.com - Online Registration', "
					+"'0', "
					+"'"+POut.String(@"https://secure.newpatientform.com/ODXNewForms.aspx?un=[username]&pw=[password]")+"', "
					+"'', "
					+"'This function automatically downloads and imports new patient forms that have been completed online.  The button only works from the Images module.')";
				int programNum=Db.NonQ32(command,true);//we now have a ProgramNum to work with
				command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					+"VALUES ("
					+"'"+programNum.ToString()+"', "
					+"'"+((int)ToolBarsAvail.ImagesModule).ToString()+"', "
					+"'NewPatientForm')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.3.0.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_3_3();
		}

		private static void To4_3_3() {
			if(FromVersion < new Version("4.3.3.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.3.3");//No translation in convert script.
				string command="INSERT INTO preference VALUES ('ReportFolderName','Reports')";
				Db.NonQ32(command);
				string imagePath=PrefC.GetString(PrefName.DocPath);
				string reportDir=ODFileUtils.CombinePaths(imagePath,"Reports");
				if(!Directory.Exists(reportDir)) {
					if(Directory.Exists(imagePath)) {
						Directory.CreateDirectory(reportDir);
					}
				}
				command="UPDATE preference SET ValueString = '4.3.3.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_3_4();
		}

		private static void To4_3_4() {
			if(FromVersion < new Version("4.3.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.3.4");//No translation in convert script.
				//get rid of any leading ? in quickpastenote
				string command="SELECT QuickPasteNoteNum,Abbreviation FROM quickpastenote";
				DataTable table=Db.GetTable(command);
				string note;
				for(int i=0;i<table.Rows.Count;i++) {
					note=PIn.String(table.Rows[i][1].ToString());
					if(note.Contains("?")){
						note=note.Replace("?","");
						command="UPDATE quickpastenote SET Abbreviation='"+POut.String(note)+"' "
							+"WHERE QuickPasteNoteNum="+table.Rows[i][0].ToString();
						Db.NonQ32(command);
					}
				}
				command="UPDATE preference SET ValueString = '4.3.4.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_4_0();
		}

		private static void To4_4_0() {
			if(FromVersion < new Version("4.4.0.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.4.0");//No translation in convert script.
				ExecuteFile(Path.Combine("Version 4 4 0", "convert_4_4_0.txt"));//Might throw an exception which we handle.
				string command;
				//add PerioPal bridge
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'PerioPal', "
					+"'PerioPal from www.periopal.com', "
					+"'0', "
					+"'"+POut.String(@"C:\Program Files\PerioPal\PerioPal.exe")+"', "
					+"'', "
					+"'')";
				int programNum=Db.NonQ32(command,true);//we now have a ProgramNum to work with
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+programNum.ToString()+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0')";
				Db.NonQ32(command);
				command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					+"VALUES ("
					+"'"+programNum.ToString()+"', "
					+"'"+((int)ToolBarsAvail.ChartModule).ToString()+"', "
					+"'PerioPal')";
				Db.NonQ32(command);
				//add MediaDent bridge
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'MediaDent', "
					+"'MediaDent from www.mediadentusa.com', "
					+"'0', "
					+"'mediadent.exe', "
					+"'', "
					+"'"+POut.String(@"Example of image folder: C:\Mediadent\patients\")+"')";
				programNum=Db.NonQ32(command,true);//we now have a ProgramNum to work with
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+programNum.ToString()+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0')";
				Db.NonQ32(command);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+programNum.ToString()+"', "
					+"'Image Folder', "
					+"'"+POut.String(@"C:\Mediadent\patients\")+"')";
				Db.NonQ32(command);
				command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					+"VALUES ("
					+"'"+programNum.ToString()+"', "
					+"'"+((int)ToolBarsAvail.ChartModule).ToString()+"', "
					+"'MediaDent')";
				Db.NonQ32(command);

				command="UPDATE preference SET ValueString = '4.4.0.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_4_9();
		}

		private static void To4_4_9() {
			if(FromVersion < new Version("4.4.9.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.4.9");//No translation in convert script.
				string command="INSERT INTO preference VALUES ('EasyHideHospitals','1')";
				Db.NonQ32(command);
				command="ALTER TABLE patient ADD Ward varchar(255) NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE schedule CHANGE ScheduleNum ScheduleNum mediumint unsigned NOT NULL auto_increment";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.4.9.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_4_10();
		}

		private static void To4_4_10() {
			if(FromVersion < new Version("4.4.10.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.4.10");//No translation in convert script.
				string command;
				//EMS clearinghouse------------------------------------
				command="INSERT INTO clearinghouse(Description,ExportPath,IsDefault,Payors,Eformat,ReceiverID,"
					+"SenderID,Password,ResponsePath,CommBridge,ClientProgram) "
					+@"VALUES('EMS','C:\\EMS\\Exports\\','0','','1','EMS','','',"
					+@"'','0','')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.4.10.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_5_0();
		}

		private static void To4_5_0() {
			if(FromVersion < new Version("4.5.0.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.5.0");//No translation in convert script.
				string command;
				string[] commands=new string[]
				{
					"ALTER TABLE procedurelog DROP OverridePri",
					"ALTER TABLE procedurelog DROP OverrideSec",
					"ALTER TABLE procedurelog DROP NoBillIns",
					"ALTER TABLE procedurelog DROP IsCovIns",
					"ALTER TABLE procedurelog DROP CapCoPay",
					"ALTER TABLE procedurelog DROP HideGraphical",
					"ALTER TABLE procedurelog CHANGE NextAptNum PlannedAptNum  mediumint unsigned NOT NULL"
				};
				Db.NonQ32(commands);
				command="DROP TABLE IF EXISTS procnote";
				Db.NonQ32(command);
				command=@"CREATE TABLE procnote(
					ProcNoteNum mediumint unsigned NOT NULL auto_increment,
					PatNum mediumint unsigned NOT NULL,
					ProcNum mediumint unsigned NOT NULL,
					EntryDateTime datetime NOT NULL default '0001-01-01 00:00:00',
					UserNum mediumint unsigned NOT NULL,
					Note text NOT NULL,
					PRIMARY KEY (ProcNoteNum),
					INDEX (PatNum),
					INDEX (ProcNum),
					INDEX (UserNum)
					) DEFAULT CHARSET=utf8";
				Db.NonQ32(command);
				//All previous notes will not have a user assigned.
				command=@"INSERT INTO procnote (PatNum,ProcNum,EntryDateTime,Note)
					SELECT PatNum,ProcNum,DateLocked,ProcNote
					FROM procedurelog
					WHERE ProcNote != ''";
				Db.NonQ32(command);
				commands=new string[]
				{
					"ALTER TABLE procedurelog DROP DateLocked",
					"ALTER TABLE procedurelog DROP ProcNote"
				};
				Db.NonQ32(commands);
				commands=new string[]
				{
					"ALTER TABLE procnote ADD SigIsTopaz tinyint unsigned NOT NULL",
					"ALTER TABLE procnote ADD Signature text NOT NULL"
				};
				Db.NonQ32(commands);
				commands=new string[]
				{
					"INSERT INTO preference VALUES ('EmailUsername','')",
					"INSERT INTO preference VALUES ('EmailPassword','')",
					"INSERT INTO preference VALUES ('EmailPort','587')"
				};
				Db.NonQ32(commands);
				command="DROP TABLE IF EXISTS emailattach";
				Db.NonQ32(command);
				command=@"CREATE TABLE emailattach(
					EmailAttachNum mediumint unsigned NOT NULL auto_increment,
					EmailMessageNum mediumint unsigned NOT NULL,
					DisplayedFileName varchar(255) NOT NULL,
					ActualFileName varchar(255) NOT NULL,
					PRIMARY KEY (EmailAttachNum),
					INDEX (EmailMessageNum)
					) DEFAULT CHARSET=utf8";
				Db.NonQ32(command);
				//We never change previous conversion scripts to support new features.  This code gets skipped on Linux.
				if(!Directory.Exists(PrefC.GetString(PrefName.DocPath)+"EmailAttachments")) {
					if(Directory.Exists(PrefC.GetString(PrefName.DocPath))) {
						Directory.CreateDirectory(PrefC.GetString(PrefName.DocPath)+"EmailAttachments");
					}
				}
				if(!Directory.Exists(PrefC.GetString(PrefName.DocPath)+"Forms")) {
					if(Directory.Exists(PrefC.GetString(PrefName.DocPath))) {
						Directory.CreateDirectory(PrefC.GetString(PrefName.DocPath)+"Forms");
					}
				}
				commands=new string[]
				{
					"ALTER TABLE document ADD Note text NOT NULL",
					"ALTER TABLE document ADD SigIsTopaz tinyint unsigned NOT NULL",
					"ALTER TABLE document ADD Signature text NOT NULL",
					"INSERT INTO preference VALUES ('BankRouting','')",
					"INSERT INTO preference VALUES ('BankAddress','')",
					"ALTER TABLE procedurelog CHANGE ProcDate ProcDate datetime NOT NULL default '0001-01-01 00:00:00'"
				};
				Db.NonQ32(commands);
				command="UPDATE preference SET ValueString = '4.5.0.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_5_1();
		}

		private static void To4_5_1() {
			if(FromVersion<new Version("4.5.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.5.1");//No translation in convert script.
				string command="UPDATE procedurelog SET AptNum=0 WHERE ProcStatus=6";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString='' WHERE PrefName='BillingSelectBillingTypes'";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.5.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_5_14();
		}

		private static void To4_5_14() {
			if(FromVersion<new Version("4.5.14.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.5.14");//No translation in convert script.
				//add XDR bridge:
				string command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'XDR', "
					+"'XDR from www.XDRradiology.com', "
					+"'0', "
					+"'"+POut.String(@"C:\Program Files\DxS\bin\XDR.exe")+"', "
					+"'', "
					+"'')";
				int programNum=Db.NonQ32(command,true);//we now have a ProgramNum to work with
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+programNum.ToString()+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0')";
				Db.NonQ32(command);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+programNum.ToString()+"', "
					+"'InfoFile path', "
					+"'"+POut.String(@"C:\Program Files\Dxs\bin\infofile.txt")+"')";
				Db.NonQ32(command);
				command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					+"VALUES ("
					+"'"+programNum.ToString()+"', "
					+"'"+((int)ToolBarsAvail.ChartModule).ToString()+"', "
					+"'XDR')";
				Db.NonQ32(command);
				command="ALTER TABLE insplan ADD FilingCode tinyint unsigned NOT NULL";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.5.14.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_5_21();
		}

		private static void To4_5_21() {
			if(FromVersion<new Version("4.5.21.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.5.21");//No translation in convert script.
				//All 4 of these fields added on 12/21/06:
				string command="ALTER TABLE patient ADD PreferConfirmMethod tinyint unsigned NOT NULL AFTER Ward";
				Db.NonQ32(command);
				command="ALTER TABLE patient ADD SchedBeforeTime time AFTER PreferConfirmMethod";
				Db.NonQ32(command);
				command="ALTER TABLE patient ADD SchedAfterTime time AFTER SchedBeforeTime";
				Db.NonQ32(command);
				command="ALTER TABLE patient ADD SchedDayOfWeek tinyint unsigned NOT NULL AFTER SchedAfterTime";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.5.21.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_6_0();
		}

		private static void To4_6_0() {
			if(FromVersion<new Version("4.6.0.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.6.0");//No translation in convert script.
				string command;
				command="DROP TABLE IF EXISTS formpat";
				Db.NonQ32(command);
				command=@"CREATE TABLE formpat(
					FormPatNum mediumint unsigned NOT NULL auto_increment,
					PatNum mediumint unsigned NOT NULL,
					FormDateTime datetime NOT NULL default '0001-01-01',
					PRIMARY KEY (FormPatNum),
					INDEX (PatNum)
					) DEFAULT CHARSET=utf8";
				Db.NonQ32(command);
				command="ALTER TABLE emailmessage ADD SentOrReceived tinyint unsigned NOT NULL";
				Db.NonQ32(command);
				command="UPDATE emailmessage SET SentOrReceived = 1 WHERE YEAR(MsgDateTime) > '1900'";
				Db.NonQ32(command);
				command="UPDATE emailmessage SET MsgDateTime = NOW() WHERE YEAR(MsgDateTime) < '1900'";
				Db.NonQ32(command);
				command="DELETE FROM commlog WHERE EmailMessageNum > 0";
				Db.NonQ32(command);
				command="ALTER TABLE commlog DROP EmailMessageNum";
				Db.NonQ32(command);
				command="ALTER TABLE question ADD FormPatNum mediumint unsigned NOT NULL";
				Db.NonQ32(command);
				command="SELECT DISTINCT PatNum FROM question";
				DataTable table=Db.GetTable(command);
				int formPatNum;
				for(int i=0;i<table.Rows.Count;i++){
					command="INSERT INTO formpat (PatNum,FormDateTime) VALUES("
						+"'"+table.Rows[i][0].ToString()+"', NOW())";
					formPatNum=Db.NonQ32(command,true);
					command="UPDATE question SET FormPatNum="+POut.Long(formPatNum)+" WHERE PatNum="+table.Rows[i][0].ToString();
					Db.NonQ32(command);
				}
				command="DROP TABLE IF EXISTS etrans";
				Db.NonQ32(command);
				command=@"CREATE TABLE etrans(
					EtransNum mediumint unsigned NOT NULL auto_increment,
					DateTimeTrans datetime NOT NULL default '0001-01-01',
					ClearinghouseNum mediumint unsigned NOT NULL,
					Etype tinyint unsigned NOT NULL,
					ClaimNum mediumint unsigned NOT NULL,
					OfficeSequenceNumber mediumint unsigned NOT NULL,
					CarrierTransCounter mediumint unsigned NOT NULL,
					CarrierTransCounter2 mediumint unsigned NOT NULL,
					CarrierNum mediumint unsigned NOT NULL,
					CarrierNum2 mediumint unsigned NOT NULL,
					PatNum mediumint unsigned NOT NULL,
					PRIMARY KEY (EtransNum),
					INDEX (ClaimNum),
					INDEX (CarrierNum),
					INDEX (CarrierNum2)
					) DEFAULT CHARSET=utf8";
				Db.NonQ32(command);
				command="DROP TABLE IF EXISTS canadianclaim";
				Db.NonQ32(command);
				command=@"CREATE TABLE canadianclaim(
					ClaimNum mediumint unsigned NOT NULL,
					MaterialsForwarded char(5) NOT NULL,
					ReferralProviderNum char(10) NOT NULL,
					ReferralReason tinyint unsigned NOT NULL,
					SecondaryCoverage char(1) NOT NULL,
					IsInitialLower char(1) NOT NULL,
					DateInitialLower date NOT NULL default '0001-01-01',
					MandProsthMaterial tinyint unsigned NOT NULL,
					IsInitialUpper char(1) NOT NULL,
					DateInitialUpper date NOT NULL default '0001-01-01',
					MaxProsthMaterial tinyint unsigned NOT NULL,
					EligibilityCode tinyint unsigned NOT NULL,
					SchoolName varchar(25) NOT NULL,
					PayeeCode tinyint unsigned NOT NULL,
					PRIMARY KEY (ClaimNum)
					) DEFAULT CHARSET=utf8";
				Db.NonQ32(command);
				command="ALTER TABLE carrier ADD IsCDA tinyint unsigned NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE carrier ADD IsPMP tinyint unsigned NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE provider ADD CanadianOfficeNum varchar(100) NOT NULL";
				Db.NonQ32(command);
				command="INSERT INTO preference VALUES ('LanguagesUsedByPatients','')";
				Db.NonQ32(command);
				command="ALTER TABLE patient ADD Language varchar(100) NOT NULL";
				Db.NonQ32(command);
				command="DROP TABLE IF EXISTS canadianextract";
				Db.NonQ32(command);
				command=@"CREATE TABLE canadianextract(
					CanadianExtractNum mediumint unsigned NOT NULL auto_increment,
					ClaimNum mediumint unsigned NOT NULL,
					ToothNum varchar(10) NOT NULL,
					DateExtraction date NOT NULL default '0001-01-01',
					PRIMARY KEY (CanadianExtractNum),
					INDEX (ClaimNum)
					) DEFAULT CHARSET=utf8";
				Db.NonQ32(command);
				command="ALTER TABLE insplan ADD DentaideCardSequence tinyint unsigned NOT NULL";
				Db.NonQ32(command);
				//added 11/30/06 after r42.
				command="ALTER TABLE etrans ADD MessageText text NOT NULL";
				Db.NonQ32(command);
				//added 12/2/06 after r46:
				command="ALTER TABLE procedurelog DROP LabFee";
				Db.NonQ32(command);
				command="ALTER TABLE procedurelog ADD ProcNumLab mediumint unsigned NOT NULL";
				Db.NonQ32(command);
				//added 12/8/06 after r57:
				command="ALTER TABLE procedurecode ADD IsCanadianLab tinyint unsigned NOT NULL";
				Db.NonQ32(command);
				//added 12/21/06 after r71:
				//Also, see the previous method at line 3450, where 4 more fields were added.
				command="ALTER TABLE claimproc ADD LineNumber tinyint unsigned NOT NULL";
				Db.NonQ32(command);
				//added 12/22/06 after r72:
				command="DROP TABLE IF EXISTS canadiannetwork";
				Db.NonQ32(command);
				command=@"CREATE TABLE canadiannetwork(
					CanadianNetworkNum mediumint unsigned NOT NULL auto_increment,
					Abbrev varchar(20) NOT NULL,
					Descript varchar(255) NOT NULL,
					PRIMARY KEY (CanadianNetworkNum)
					) DEFAULT CHARSET=utf8";
				Db.NonQ32(command);
				string[] commands=new string[]
				{
					"INSERT INTO canadiannetwork VALUES (1,'AHI','BC Emergis (formerly Assure Health Inc)')",
					"INSERT INTO canadiannetwork VALUES (2,'NDC','National Data Corporation')",
					"INSERT INTO canadiannetwork VALUES (3,'CD','Centre Dentaide')",
					"INSERT INTO canadiannetwork VALUES (4,'ABC','Alberta Blue Cross')",
					"INSERT INTO canadiannetwork VALUES (5,'MBC','Manitoba Blue Cross')",
					"INSERT INTO canadiannetwork VALUES (6,'PBC','Pacific Blue Cross')"					
				};
				Db.NonQ32(commands);
				command="UPDATE preference SET ValueString = '4.6.0.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_6_2();
		}

		private static void To4_6_2() {
			if(FromVersion<new Version("4.6.2.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.6.2");//No translation in convert script.
				string command;
				command="ALTER TABLE carrier ADD CDAnetVersion varchar(100) NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE carrier ADD CanadianNetworkNum mediumint unsigned NOT NULL";
				Db.NonQ32(command);
				//CDAnet clearinghouse------------------------------------------------------------------------------------
				command="INSERT INTO clearinghouse(Description,ExportPath,IsDefault,Payors,Eformat,ReceiverID,"
					+"SenderID,Password,ResponsePath,CommBridge,ClientProgram) "
					+@"VALUES('CDAnet','C:\\CCD\\','0','','3','','','',"
					+@"'','9','C:\\CCD\\CCD32.exe')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.6.2.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_6_6();
		}

		private static void To4_6_6() {
			if(FromVersion<new Version("4.6.6.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.6.6");//No translation in convert script.
				string command="DROP TABLE IF EXISTS userod";
				Db.NonQ32(command);
				command="ALTER TABLE user RENAME TO userod";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.6.6.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_6_9();
		}

		///<summary> The following string is used to upgrade MySQL databases into a compatible format for conversion to Oracle. A similar table will also used for the database maintinence tool in order to ensure that the columns below do not contain null data. The array below is composed of groups of 3 strings, following the format: tablename columnname type.</summary>
		private static string[] removeNotNullFieldCommands=new string[] {
			"account","Description","varchar(255)",
			"account","BankNumber","varchar(255)",
			"accountingautopay","PickList","varchar(255)",
			"adjustment","AdjNote","text",
			"appointment","Pattern","varchar(255)",
			"appointment","Note","text",
			"appointment","ProcDescript","varchar(255)",
			"appointmentrule","RuleDesc","varchar(255)",
			"appointmentrule","ADACodeStart","varchar(15) character set utf8 collate utf8_bin",
			"appointmentrule","ADACodeEnd","varchar(15) character set utf8 collate utf8_bin",
			"apptview","Description","varchar(255)",
			"apptviewitem","ElementDesc","varchar(255)",
			"autocode","Description","varchar(255)",
			"autocodeitem","ADACode","varchar(15) character set utf8 collate utf8_bin",
			"benefit","ADACode","varchar(15) character set utf8 collate utf8_bin",
			"canadianclaim","MaterialsForwarded","char(5)",
			"canadianclaim","ReferralProviderNum","char(10)",
			"canadianclaim","SecondaryCoverage","char(1)",
			"canadianclaim","IsInitialLower","char(1)",
			"canadianclaim","IsInitialUpper","char(1)",
			"canadianclaim","SchoolName","varchar(25)",
			"canadianextract","ToothNum","varchar(10)",
			"canadiannetwork","Abbrev","varchar(20)",
			"canadiannetwork","Descript","varchar(255)",
			"carrier","CarrierName","varchar(255)",
			"carrier","Address","varchar(255)",
			"carrier","Address2","varchar(255)",
			"carrier","City","varchar(255)",
			"carrier","State","varchar(255)",
			"carrier","Zip","varchar(255)",
			"carrier","Phone","varchar(255)",
			"carrier","ElectID","varchar(255)",
			"carrier","CDAnetVersion","varchar(100)",
			"claim","ClaimStatus","char(1)",
			"claim","PreAuthString","varchar(40)",
			"claim","IsProsthesis","char(1)",
			"claim","ReasonUnderPaid","varchar(255)",
			"claim","ClaimNote","varchar(255)",
			"claim","ClaimType","varchar(255)",
			"claim","RefNumString","varchar(40)",
			"claim","AccidentRelated","char(1)",
			"claim","AccidentST","varchar(2)",
			"claimform","Description","varchar(50)",
			"claimform","FontName","varchar(255)",
			"claimform","UniqueID","varchar(255)",
			"claimformitem","ImageFileName","varchar(255)",
			"claimformitem","FieldName","varchar(255)",
			"claimformitem","FormatString","varchar(255)",
			"claimpayment","CheckNum","varchar(25)",
			"claimpayment","BankBranch","varchar(25)",
			"claimpayment","Note","varchar(255)",
			"claimpayment","CarrierName","varchar(255)",
			"claimproc","Remarks","varchar(255)",
			"claimproc","CodeSent","varchar(15)",
			"clearinghouse","Description","varchar(255)",
			"clearinghouse","ExportPath","text",
			"clearinghouse","Payors","text",
			"clearinghouse","ReceiverID","varchar(255)",
			"clearinghouse","SenderID","varchar(255)",
			"clearinghouse","Password","varchar(255)",
			"clearinghouse","ResponsePath","varchar(255)",
			"clearinghouse","ClientProgram","varchar(255)",
			"clearinghouse","LoginID","varchar(255)",
			"clinic","Description","varchar(255)",
			"clinic","Address","varchar(255)",
			"clinic","Address2","varchar(255)",
			"clinic","City","varchar(255)",
			"clinic","State","varchar(255)",
			"clinic","Zip","varchar(255)",
			"clinic","Phone","varchar(255)",
			"clinic","BankNumber","varchar(255)",
			"clockevent","Note","text",
			"commlog","Note","text",
			"computer","CompName","varchar(100)",
			"computer","PrinterName","varchar(255)",
			"contact","LName","varchar(255)",
			"contact","FName","varchar(255)",
			"contact","WkPhone","varchar(255)",
			"contact","Fax","varchar(255)",
			"contact","Notes","text",
			"county","CountyName","varchar(255)",
			"county","CountyCode","varchar(255)",
			"covcat","Description","varchar(50)",
			"covspan","FromCode","varchar(15) character set utf8 collate utf8_bin",
			"covspan","ToCode","varchar(15) character set utf8 collate utf8_bin",
			"definition","ItemName","varchar(255)",
			"definition","ItemValue","varchar(255)",
			"deposit","BankAccountInfo","text",
			"disease","PatNote","text",
			"diseasedef","DiseaseName","varchar(255)",
			"document","Description","varchar(255)",
			"document","FileName","varchar(255)",
			"document","ToothNumbers","varchar(255)",
			"document","Note","text",
			"document","Signature","text",
			"dunning","DunMessage","text",
			"electid","PayorID","varchar(255)",
			"electid","CarrierName","varchar(255)",
			"electid","ProviderTypes","varchar(255)",
			"electid","Comments","text",
			"emailattach","DisplayedFileName","varchar(255)",
			"emailattach","ActualFileName","varchar(255)",
			"emailmessage","ToAddress","text",
			"emailmessage","FromAddress","text",
			"emailmessage","Subject","text",
			"emailmessage","BodyText","text",
			"emailtemplate","Subject","text",
			"emailtemplate","BodyText","text",
			"employee","LName","varchar(255)",
			"employee","FName","varchar(255)",
			"employee","MiddleI","varchar(255)",
			"employee","ClockStatus","varchar(255)",
			"employer","EmpName","varchar(255)",
			"employer","Address","varchar(255)",
			"employer","Address2","varchar(255)",
			"employer","City","varchar(255)",
			"employer","State","varchar(255)",
			"employer","Zip","varchar(255)",
			"employer","Phone","varchar(255)",
			"etrans","MessageText","text",
			"fee","ADACode","varchar(15) character set utf8 collate utf8_bin",
			"graphicelement","ToothNum","varchar(2)",
			"graphicelement","Description","varchar(100)",
			"graphicelement","Surface","varchar(5)",
			"graphicshape","ShapeType","char(1)",
			"graphicshape","Description","varchar(100)",
			"graphictype","Description","varchar(100)",
			"graphictype","BrushType","varchar(100)",
			"graphictype","SpecialType","varchar(100)",
			"insplan","GroupName","varchar(50)",
			"insplan","GroupNum","varchar(20)",
			"insplan","PlanNote","text",
			"insplan","PlanType","char(1)",
			"insplan","SubscriberID","varchar(40)",
			"insplan","TrojanID","varchar(100)",
			"insplan","DivisionNo","varchar(255)",
			"insplan","BenefitNotes","text",
			"insplan","SubscNote","text",
			"instructor","LName","varchar(255)",
			"instructor","FName","varchar(255)",
			"instructor","Suffix","varchar(100)",
			"journalentry","Memo","text",
			"journalentry","Splits","text",
			"journalentry","CheckNumber","varchar(255)",
			"language","EnglishComments","text",
			"language","ClassType","text",
			"language","English","text",
			"languageforeign","ClassType","text",
			"languageforeign","English","text",
			"languageforeign","Culture","varchar(255)",
			"languageforeign","Translation","text",
			"languageforeign","Comments","text",
			"letter","Description","varchar(255)",
			"letter","BodyText","text",
			"lettermerge","Description","varchar(255)",
			"lettermerge","TemplateName","varchar(255)",
			"lettermerge","DataFileName","varchar(255)",
			"lettermergefield","FieldName","varchar(255)",
			"medication","MedName","varchar(255)",
			"medication","Notes","text",
			"medicationpat","PatNote","text",
			"operatory","OpName","varchar(255)",
			"operatory","Abbrev","varchar(255)",
			"patfield","FieldName","varchar(255)",
			"patfield","FieldValue","text",
			"patfielddef","FieldName","varchar(255)",
			"patient","LName","varchar(100)",
			"patient","FName","varchar(100)",
			"patient","MiddleI","varchar(100)",
			"patient","Preferred","varchar(100)",
			"patient","SSN","varchar(100)",
			"patient","Address","varchar(100)",
			"patient","Address2","varchar(100)",
			"patient","City","varchar(100)",
			"patient","State","varchar(100)",
			"patient","Zip","varchar(100)",
			"patient","HmPhone","varchar(30)",
			"patient","WkPhone","varchar(30)",
			"patient","WirelessPhone","varchar(30)",
			"patient","CreditType","char(1)",
			"patient","Email","varchar(100)",
			"patient","Salutation","varchar(100)",
			"patient","ImageFolder","varchar(100)",
			"patient","AddrNote","text",
			"patient","FamFinUrgNote","text",
			"patient","MedUrgNote","varchar(255)",
			"patient","ApptModNote","varchar(255)",
			"patient","StudentStatus","char(1)",
			"patient","SchoolName","varchar(30)",
			"patient","ChartNumber","varchar(20)",
			"patient","MedicaidID","varchar(20)",
			"patient","PrimaryTeeth","varchar(255)",
			"patient","EmploymentNote","varchar(255)",
			"patient","County","varchar(255)",
			"patient","GradeSchool","varchar(255)",
			"patient","HasIns","varchar(255)",
			"patient","TrophyFolder","varchar(255)",
			"patient","Ward","varchar(255)",
			"patient","Language","varchar(100)",
			"patientnote","FamFinancial","text",
			"patientnote","ApptPhone","text",
			"patientnote","Medical","text",
			"patientnote","Service","text",
			"patientnote","MedicalComp","text",
			"patientnote","Treatment","text",
			"patplan","PatID","varchar(100)",
			"payment","CheckNum","varchar(25)",
			"payment","BankBranch","varchar(25)",
			"payment","PayNote","varchar(255)",
			"payplan","Note","text",
			"payplancharge","Note","text",
			"preference","PrefName","varchar(255)",
			"preference","ValueString","text",
			"printer","PrinterName","varchar(255)",
			"procbutton","Description","varchar(255)",
			"procbutton","ButtonImage","text",
			"procbuttonitem","ADACode","varchar(15) character set utf8 collate utf8_bin",
			"procedurecode","ADACode","varchar(15) character set utf8 collate utf8_bin",
			"procedurecode","Descript","varchar(255)",
			"procedurecode","AbbrDesc","varchar(50)",
			"procedurecode","ProcTime","varchar(24)",
			"procedurecode","DefaultNote","text",
			"procedurecode","AlternateCode1","varchar(15)",
			"procedurecode","MedicalCode","varchar(15) character set utf8 collate utf8_bin",
			"procedurecode","LaymanTerm","varchar(255)",
			"procedurelog","ADACode","varchar(15) character set utf8 collate utf8_bin",
			"procedurelog","Surf","varchar(10)",
			"procedurelog","ToothNum","varchar(2)",
			"procedurelog","ToothRange","varchar(100)",
			"procedurelog","Prosthesis","char(1)",
			"procedurelog","ClaimNote","varchar(80)",
			"procedurelog","MedicalCode","varchar(15) character set utf8 collate utf8_bin",
			"procedurelog","DiagnosticCode","varchar(255)",
			"procnote","Note","text",
			"procnote","Signature","text",
			"proctp","ToothNumTP","varchar(255)",
			"proctp","Surf","varchar(255)",
			"proctp","ADACode","varchar(255)",
			"proctp","Descript","varchar(255)",
			"program","ProgName","varchar(100)",
			"program","ProgDesc","varchar(100)",
			"program","Path","varchar(255)",
			"program","CommandLine","varchar(255)",
			"program","Note","text",
			"programproperty","PropertyDesc","varchar(255)",
			"programproperty","PropertyValue","varchar(255)",
			"provider","Abbr","varchar(5)",
			"provider","LName","varchar(100)",
			"provider","FName","varchar(100)",
			"provider","MI","varchar(100)",
			"provider","Suffix","varchar(100)",
			"provider","SSN","varchar(12)",
			"provider","StateLicense","varchar(15)",
			"provider","DEANum","varchar(15)",
			"provider","BlueCrossID","varchar(25)",
			"provider","MedicaidID","varchar(20)",
			"provider","NationalProvID","varchar(255)",
			"provider","CanadianOfficeNum","varchar(100)",
			"providerident","PayorID","varchar(255)",
			"providerident","IDNumber","varchar(255)",
			"question","Description","text",
			"question","Answer","text",
			"questiondef","Description","text",
			"quickpastecat","Description","varchar(255)",
			"quickpastecat","DefaultForTypes","text",
			"quickpastenote","Note","text",
			"quickpastenote","Abbreviation","varchar(255)",
			"recall","Note","text",
			"referral","LName","varchar(100)",
			"referral","FName","varchar(100)",
			"referral","MName","varchar(100)",
			"referral","SSN","varchar(9)",
			"referral","ST","varchar(2)",
			"referral","Telephone","varchar(10)",
			"referral","Address","varchar(100)",
			"referral","Address2","varchar(100)",
			"referral","City","varchar(100)",
			"referral","Zip","varchar(10)",
			"referral","Note","text",
			"referral","Phone2","varchar(30)",
			"referral","Title","varchar(255)",
			"referral","EMail","varchar(255)",
			"repeatcharge","ADACode","varchar(15) character set utf8 collate utf8_bin",
			"repeatcharge","Note","text",
			"rxdef","Drug","varchar(255)",
			"rxdef","Sig","varchar(255)",
			"rxdef","Disp","varchar(255)",
			"rxdef","Refills","varchar(30)",
			"rxdef","Notes","varchar(255)",
			"rxpat","Drug","varchar(255)",
			"rxpat","Sig","varchar(255)",
			"rxpat","Disp","varchar(255)",
			"rxpat","Refills","varchar(30)",
			"rxpat","Notes","varchar(255)",
			"schedule","Note","text",
			"school","SchoolName","varchar(255)",
			"school","SchoolCode","varchar(255)",
			"schoolclass","Descript","varchar(255)",
			"schoolcourse","CourseID","varchar(255)",
			"schoolcourse","Descript","varchar(255)",
			"screen","GradeSchool","varchar(255)",
			"screen","County","varchar(255)",
			"screen","ProvName","varchar(255)",
			"screengroup","Description","varchar(255)",
			"securitylog","LogText","text",
			"sigbutdef","ButtonText","varchar(255)",
			"sigbutdef","ComputerName","varchar(255)",
			"sigelementdef","SigText","varchar(255)",
			"sigelementdef","Sound","text",
			"signal","FromUser","varchar(255)",
			"signal","SigText","text",
			"signal","ToUser","varchar(255)",
			"task","Descript","text",
			"tasklist","Descript","varchar(255)",
			"terminalactive","ComputerName","varchar(255)",
			"timeadjust","Note","text",
			"toolbutitem","ButtonText","varchar(255)",
			"toothinitial","ToothNum","varchar(2)",
			"treatplan","Heading","varchar(255)",
			"treatplan","Note","text",
			"usergroup","Description","varchar(255)",
			"userod","UserName","varchar(255)",
			"userod","Password","varchar(255)",
			"userquery","Description","varchar(255)",
			"userquery","FileName","varchar(255)",
			"userquery","QueryText","text",
			"zipcode","ZipCodeDigits","varchar(20)",
			"zipcode","City","varchar(100)",
			"zipcode","State","varchar(20)",
		};

		private static void To4_6_9() {
			if(FromVersion<new Version("4.6.9.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.6.9");//No translation in convert script.
				string command="ALTER TABLE commlog CHANGE Mode Mode_ tinyint(3) unsigned NOT NULL default '0'";
				Db.NonQ32(command);
				command="ALTER TABLE patient ADD PreferContactMethod tinyint unsigned NOT NULL AFTER PreferConfirmMethod";
				Db.NonQ32(command);
				command="ALTER TABLE patient ADD PreferRecallMethod tinyint unsigned NOT NULL AFTER PreferContactMethod";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.6.9.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_6_11();
		}

		private static void To4_6_11() {
			if(FromVersion<new Version("4.6.11.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.6.11");//No translation in convert script.
				string command="INSERT INTO preference VALUES ('OracleInsertId','')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.6.11.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_6_12();
		}

		///<summary>The following changes are to remove the NOT NULL from string fields for all tables in the database in order to become compatible with Oracle, since in Oracle null is the same as the empty string.</summary>
		private static void To4_6_12() {
			if(FromVersion<new Version("4.6.12.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.6.12");//No translation in convert script.
				string command="";
				for(int i=0;i<removeNotNullFieldCommands.Length;i+=3) {
					command="ALTER TABLE "+removeNotNullFieldCommands[i]+" MODIFY "+//table name
							removeNotNullFieldCommands[i+1]+" "+removeNotNullFieldCommands[i+2];	//column name then type
					if(removeNotNullFieldCommands[i+2].ToUpper()!="TEXT") {//For all fields which are not of text type, define default.
						command+=" default ''";
					}
					Db.NonQ32(command);
				}
				//added after r49
				command="INSERT INTO preference VALUES ('DefaultClaimForm','1')";
				Db.NonQ32(command);
				command="ALTER TABLE patient ADD AdmitDate date NOT NULL default '0001-01-01'";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.6.12.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_6_13();
		}

		///<summary>Support for Oracle began with this version.</summary>
		private static void To4_6_13() {
			if(FromVersion<new Version("4.6.13.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.6.13");//No translation in convert script.
				string command="";
				command="INSERT INTO preference VALUES ('RegistrationNumberClaim','')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.6.13.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_6_19();
		}

		///<summary></summary>
		private static void To4_6_19() {
			if(FromVersion<new Version("4.6.19.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.6.19");//No translation in convert script.
				//Owandy X-ray Bridge created by SPK 10/06, added 2/22/07-----------------------------------------------------------
				string command = "INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+ ") VALUES("
					+ "'Owandy', "
					+ "'QuickVision from owandy.com', "
					+ "'0', "
					+ "'" + POut.String(@"\Juliew\mj32.exe") + "', "
					+ "' C /ALINK', "
					+ "'" + POut.String(@"Typical file path with parameters is C:\Juliew\mj32.exe C /ALINK.  Use C /LINK for QV version < 3.15. Letter C refers to drive.") + "')";
				int programNum =Db.NonQ32(command,true);//we now have a ProgramNum to work with
				command = "INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					+ "VALUES ("
					+ "'" + POut.Long(programNum) + "', "
					+ "'" + POut.Long((int)ToolBarsAvail.ChartModule) + "', "
					+ "'Owandy')";
				Db.NonQ32(command);
				//Vipersoft bridge:
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'Vipersoft', "
					+"'Vipersoft aka Clarity', "
					+"'0', "
					+"'"+POut.String(@"C:\Program Files\Vipersoft\Vipersoft.exe")+"', "
					+"'', "
					+"'')";
				programNum=Db.NonQ32(command,true);//we now have a ProgramNum to work with
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+programNum.ToString()+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0')";
				Db.NonQ32(command);
				command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					+"VALUES ("
					+"'"+programNum.ToString()+"', "
					+"'"+((int)ToolBarsAvail.ChartModule).ToString()+"', "
					+"'Vipersoft')";
				Db.NonQ32(command);
				if(DataConnection.DBtype==DatabaseType.MySql){
					command="ALTER TABLE userod ADD ClinicNum mediumint NOT NULL";
					Db.NonQ32(command);
				}
				else{
					command="ALTER TABLE userod ADD ClinicNum int";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '4.6.19.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_6_22();
		}

		///<summary></summary>
		private static void To4_6_22() {
			if(FromVersion<new Version("4.6.22.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.6.22");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql){
					command="ALTER TABLE sigelementdef CHANGE Sound Sound mediumtext";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '4.6.22.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_7_1();
		}

		///<summary></summary>
		private static void To4_7_1() {
			if(FromVersion<new Version("4.7.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.7.1");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql){
					command="ALTER TABLE document ADD CropX mediumint NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE document ADD CropY mediumint NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE document ADD CropW mediumint NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE document ADD CropH mediumint NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE document ADD WindowingMin mediumint NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE document ADD WindowingMax mediumint NOT NULL";
					Db.NonQ32(command);
				}
				else{
					command="ALTER TABLE document ADD CropX int";
					Db.NonQ32(command);
					command="ALTER TABLE document ADD CropY int";
					Db.NonQ32(command);
					command="ALTER TABLE document ADD CropW int";
					Db.NonQ32(command);
					command="ALTER TABLE document ADD CropH int";
					Db.NonQ32(command);
					command="ALTER TABLE document ADD WindowingMin int";
					Db.NonQ32(command);
					command="ALTER TABLE document ADD WindowingMax int";
					Db.NonQ32(command);
				}
				command="INSERT INTO preference VALUES ('ImageWindowingMin','64')";
				Db.NonQ32(command);
				command="INSERT INTO preference VALUES ('ImageWindowingMax','192')";
				Db.NonQ32(command);
				if(DataConnection.DBtype==DatabaseType.MySql){
					command="DROP TABLE IF EXISTS mountdef";
					Db.NonQ32(command);
					command=@"CREATE TABLE mountdef(
						MountDefNum int NOT NULL auto_increment,
						Description varchar(255),
						ItemOrder mediumint NOT NULL,
						IsRadiograph tinyint unsigned NOT NULL,
						Width mediumint NOT NULL,
						Height mediumint NOT NULL,
						PRIMARY KEY (MountDefNum)
						) DEFAULT CHARSET=utf8";
				}
				else{
					command=@"CREATE TABLE mountdef(
						MountDefNum int NOT NULL,
						Description varchar(255),
						ItemOrder int NOT NULL,
						IsRadiograph int NOT NULL,
						Width int NOT NULL,
						Height int NOT NULL,
						PRIMARY KEY (MountDefNum)
						)";
				}
				Db.NonQ32(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS mountitemdef";
					Db.NonQ32(command);
					command=@"CREATE TABLE mountitemdef(
						MountItemDefNum int NOT NULL auto_increment,
						MountDefNum int NOT NULL,
						Xpos mediumint NOT NULL,
						Ypos mediumint NOT NULL,
						Width mediumint NOT NULL,
						Height mediumint NOT NULL,
						PRIMARY KEY (MountItemDefNum)
						) DEFAULT CHARSET=utf8";
				}
				else {
					command=@"CREATE TABLE mountitemdef(
						MountItemDefNum int NOT NULL,
						MountDefNum int NOT NULL,
						Xpos int NOT NULL,
						Ypos int NOT NULL,
						Width int NOT NULL,
						Height int NOT NULL,
						PRIMARY KEY (MountItemDefNum)
						)";
				}
				Db.NonQ32(command);
				command="INSERT INTO preference VALUES ('XRayExposureLevel','1')";
				Db.NonQ32(command);
				//Dxis Bridge---------------------------------------------------------------------------
				command = "INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+ ") VALUES("
					+ "'Dxis', "
					+ "'DXIS from dxis.com', "
					+ "'0', "
					+ "'" + POut.String(@"C:\Dxis\Dxis.exe") + "', "
					+ "'', "
					+ "'" + POut.String(@"") + "')";
				int programNum =Db.NonQ32(command,true);//we now have a ProgramNum to work with
				command = "INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					+ "VALUES ("
					+ "'" + POut.Long(programNum) + "', "
					+ "'" + POut.Long((int)ToolBarsAvail.ChartModule) + "', "
					+ "'DXIS')";
				Db.NonQ32(command);
				//Added after r25:
				command="INSERT INTO preference VALUES ('ToothChartLowerQuality','0')";
				Db.NonQ32(command);
				//Added after r61
				command="INSERT INTO preference VALUES ('AtoZfolderNotRequired','0')";
				Db.NonQ32(command);
				//Added after r113
				command = "INSERT INTO preference VALUES('SolidBlockouts','0')";
				Db.NonQ32(command);
				//Added after r129
				command="ALTER TABLE procedurelog ADD CPTModifier varchar(255)";//valid for Oracle, too.
				Db.NonQ32(command);
				command="ALTER TABLE procedurelog ADD RevenueCode varchar(255)";//valid for Oracle, too.
				Db.NonQ32(command);
				//Added after r130
				command="INSERT INTO preference VALUES ('AgingCalculatedMonthlyInsteadOfDaily','0')";
				Db.NonQ32(command);
				//Added after r141:
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurelog ADD BillingTypeOne smallint NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD BillingTypeTwo smallint NOT NULL";
					Db.NonQ32(command);
				}
				else{//Oracle
					command="ALTER TABLE procedurelog ADD BillingTypeOne int NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD BillingTypeTwo int NOT NULL";
					Db.NonQ32(command);
				}
				//Added after r146:
				//Tesia clearinghouse------------------------------------
				command=@"INSERT INTO clearinghouse(Description,ExportPath,IsDefault,Payors,Eformat,ReceiverID,
					SenderID,Password,ResponsePath,CommBridge,ClientProgram,LastBatchNumber,ModemPort) 
					VALUES('Tesia','C:\\Tesia\\Exports\\','0','','1','Tesia','','',
					'','0','','0','0')";//Valid for Oracle too.
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.7.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_7_4();
		}

		///<summary></summary>
		private static void To4_7_4() {
			if(FromVersion<new Version("4.7.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.7.4");//No translation in convert script.
				string command="";
				command="UPDATE clearinghouse SET ReceiverID='113504607' WHERE ReceiverID='Tesia'";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.7.4.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_8_1();
		}

		///<summary></summary>
		private static void To4_8_1() {
			if(FromVersion<new Version("4.8.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.8.1");//No translation in convert script.
				string command="";
				int practiceDefaultProv=PrefC.GetInt(PrefName.PracticeDefaultProv);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					//Turn all hardcoded clearinghouse fields into dynamic fields------------------------------------------------------
					command="ALTER TABLE clearinghouse ADD ISA05 varchar(255) AFTER Eformat";
					Db.NonQ32(command);
					command="UPDATE clearinghouse SET ISA05='30' WHERE ReceiverID='660610220' OR ReceiverID='AOS'";
					Db.NonQ32(command);
					command="UPDATE clearinghouse SET ISA05='ZZ' WHERE ReceiverID!='660610220' AND ReceiverID!='AOS'";
					Db.NonQ32(command);
					command="ALTER TABLE clearinghouse ADD SenderTIN varchar(255) AFTER ISA05";
					Db.NonQ32(command);
					command="SELECT SSN FROM provider WHERE ProvNum="+POut.Long(practiceDefaultProv);
					string defProvSSN=Db.GetTable(command).Rows[0][0].ToString().Replace("-","");
					command="UPDATE clearinghouse SET SenderTIN='"+POut.String(defProvSSN)+"' "
						+"WHERE ReceiverID='660610220' OR ReceiverID='AOS' OR ReceiverID='113504607'";//Inmediata,AOS, or Tesia
					Db.NonQ32(command);
					command="ALTER TABLE clearinghouse ADD ISA07 varchar(255) AFTER SenderTIN";
					Db.NonQ32(command);
					command="UPDATE clearinghouse SET ISA07='30' WHERE ReceiverID='330989922' OR ReceiverID='660610220' OR ReceiverID='AOS'";
					Db.NonQ32(command);
					command="UPDATE clearinghouse SET ISA07='ZZ' WHERE ReceiverID!='330989922' AND ReceiverID!='660610220' AND ReceiverID!='AOS'";
					Db.NonQ32(command);
					command="ALTER TABLE clearinghouse CHANGE ReceiverID ISA08 varchar(255)";
					Db.NonQ32(command);
					command="ALTER TABLE clearinghouse ADD ISA15 varchar(255) AFTER ISA08";
					Db.NonQ32(command);
					command="UPDATE clearinghouse SET ISA15='P'";
					Db.NonQ32(command);
					command="ALTER TABLE clearinghouse DROP SenderID";
					Db.NonQ32(command);
					command="ALTER TABLE clearinghouse ADD SenderName varchar(255)";
					Db.NonQ32(command);
					command="ALTER TABLE clearinghouse ADD SenderTelephone varchar(255)";
					Db.NonQ32(command);
					command="ALTER TABLE clearinghouse ADD GS03 varchar(255)";
					Db.NonQ32(command);
					command="SELECT Abbr FROM provider WHERE ProvNum="+POut.Long(practiceDefaultProv);
					string AOSnumber=Db.GetTable(command).Rows[0][0].ToString();
					command="UPDATE clearinghouse SET SenderName='"+POut.String(AOSnumber)+"' WHERE ISA08='AOS'";
					Db.NonQ32(command);
					command="UPDATE clearinghouse SET SenderName='"+POut.String(PrefC.GetString(PrefName.PracticeTitle))+"' "
						+"WHERE ISA08='660610220' OR ISA08='113504607'";//Inmediata or Tesia
					Db.NonQ32(command);
					command="UPDATE clearinghouse SET SenderTelephone='"+POut.String(PrefC.GetString(PrefName.PracticePhone))+"' "
						+"WHERE ISA08='660610220' OR ISA08='113504607' OR ISA08='AOS'";//Inmediata or Tesia or AOS
					Db.NonQ32(command);
					command="UPDATE clearinghouse SET GS03=ISA08";
					Db.NonQ32(command);
				}else{//Oracle
					//Recreate clearinghouse table from scratch. The data in this table is not likely to be important, and 
					//can always be added again through the program. Recreating the table is easier than trying to mimic the
					//above dozen or so mysql statements.
					command="DROP TABLE clearinghouse PURGE";
					Db.NonQ32(command);
					command="CREATE TABLE clearinghouse("
						+"ClearinghouseNum number(8,0) NOT NULL,"
						+"Description varchar(255) default '',"
						+"ExportPath varchar2(4000),"
						+"IsDefault number(1,0) default '0' NOT NULL,"
						+"Payors varchar2(4000),"
						+"Eformat number(3,0) default '0' NOT NULL,"
						+"ISA05 varchar(255) default NULL,"
						+"SenderTIN varchar(255) default NULL,"
						+"ISA07 varchar(255) default NULL,"
						+"ISA08 varchar(255) default NULL,"
						+"ISA15 varchar(255) default NULL,"
						+"Password varchar(255) default '',"
						+"ResponsePath varchar(255) default '',"
						+"CommBridge number(3,0) default '0' NOT NULL,"
						+"ClientProgram varchar(255) default '',"
						+"LastBatchNumber number(5,0) default '0' NOT NULL,"
						+"ModemPort number(3,0) default '0' NOT NULL,"
						+"LoginID varchar(255) default '',"
						+"SenderName varchar(255) default NULL,"
						+"SenderTelephone varchar(255) default NULL,"
						+"GS03 varchar(255) default NULL"
						+",PRIMARY KEY (ClearinghouseNum)"
						+");";
					Db.NonQ32(command);
				}
				//added after r167:
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS laboratory";
					Db.NonQ32(command);
					command=@"CREATE TABLE laboratory(
						LaboratoryNum mediumint NOT NULL auto_increment,
						Description varchar(255),
						Phone varchar(255),
						Notes text,
						LabSlip mediumtext,
						PRIMARY KEY (LaboratoryNum)
						) DEFAULT CHARSET=utf8";
				}else {//Oracle.
					command=@"CREATE TABLE laboratory(
						LaboratoryNum number(8,0) NOT NULL,
						Description varchar(255),
						Phone varchar(255),
						Notes varchar2(4000),
						LabSlip varchar2(4000),
						PRIMARY KEY(LaboratoryNum)
						)";
				}
				Db.NonQ32(command);
				//added after r168:
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS labcase";
					Db.NonQ32(command);
					command=@"CREATE TABLE labcase(
						LabCaseNum mediumint NOT NULL auto_increment,
						PatNum mediumint NOT NULL,
						LaboratoryNum mediumint NOT NULL,
						AptNum mediumint NOT NULL,
						PlannedAptNum mediumint NOT NULL,
						DateTimeDue datetime NOT NULL default '0001-01-01',
						DateTimeCreated datetime NOT NULL default '0001-01-01',
						DateTimeSent datetime NOT NULL default '0001-01-01',
						DateTimeRecd datetime NOT NULL default '0001-01-01',
						DateTimeChecked datetime NOT NULL default '0001-01-01',
						PRIMARY KEY (LabCaseNum)
						) DEFAULT CHARSET=utf8";
				}else {//Oracle.
					command=@"CREATE TABLE labcase(
						LabCaseNum number(8,0) NOT NULL,
						PatNum number(8,0) NOT NULL,
						LaboratoryNum number(8,0) NOT NULL,
						AptNum number(8,0) NOT NULL,
						PlannedAptNum number(8,0) NOT NULL,
						DateTimeDue date default '0001-01-01' NOT NULL,
						DateTimeCreated date default '0001-01-01' NOT NULL,
						DateTimeSent date default '0001-01-01' NOT NULL,
						DateTimeRecd date default '0001-01-01' NOT NULL,
						DateTimeChecked date default '0001-01-01' NOT NULL,
						PRIMARY KEY(LabCaseNum)
						)";
				}
				Db.NonQ32(command);
				//Added after r180
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE document CHANGE WithPat PatNum mediumint(8) unsigned NOT NULL default '0'";
					Db.NonQ32(command);
					command="ALTER TABLE document ADD MountItemNum int NOT NULL";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS mount";
					Db.NonQ32(command);
					command=@"CREATE TABLE mount(
						MountNum int NOT NULL auto_increment,
						PatNum mediumint NOT NULL,
						DocCategory smallint(5) NOT NULL,
						DateCreated date NOT NULL default '0001-01-01',
						Description varchar(255) default '',
						ImgType tinyint(3) unsigned NOT NULL default '0',
						PRIMARY KEY (MountNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS mountitem";
					Db.NonQ32(command);
					command=@"CREATE TABLE mountitem(
						MountItemNum int NOT NULL auto_increment,
						MountNum int NOT NULL,
						Xpos mediumint(9) NOT NULL,
						Ypos mediumint(9) NOT NULL,
						PRIMARY KEY (MountItemNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
				}else {//Oracle
					command="ALTER TABLE document RENAME COLUMN WithPat TO PatNum";//Oracle fails here.
					Db.NonQ32(command);
					command="ALTER TABLE document ADD (MountItemNum int NOT NULL)";
					Db.NonQ32(command);
					command=@"CREATE TABLE mount(
						MountNum int NOT NULL,
						PatNum number(5,0) NOT NULL,
						DocCategory number(5,0) NOT NULL,
						DateCreated date default '0001-01-01' NOT NULL,
						Description varchar(255) default '',
						ImgType number(3,0) default '0' NOT NULL,
						PRIMARY KEY (MountNum)
						)";
					Db.NonQ32(command);
					command=@"CREATE TABLE mountitem(
						MountItemNum int NOT NULL,
						MountNum int NOT NULL,
						Xpos number(9,0) NOT NULL,
						Ypos number(9,0) NOT NULL,
						PRIMARY KEY (MountItemNum)
						)";
					Db.NonQ32(command);
				}
				//Added after r186
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurecode ADD PreExisting tinyint(1) NOT NULL default '0'";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS proclicense";
					Db.NonQ32(command);
					command=@"CREATE TABLE proclicense(
						ProcLicenseNum mediumint(5) NOT NULL auto_increment,
						ADACode varchar(15) default '',
						Descript varchar(255) default '',
						PRIMARY KEY (ProcLicenseNum)
						)";
					Db.NonQ32(command);
				}else {//Oracle
					command="ALTER TABLE procedurecode ADD (PreExisting number(1,0) default '0' NOT NULL)";
					Db.NonQ32(command);
					command=@"CREATE TABLE proclicense(
						ProcLicenseNum number(5,0) NOT NULL,
						ADACode varchar(15) default '',
						Descript varchar(255) default '',
						PRIMARY KEY (ProcLicenseNum)
						)";
					Db.NonQ32(command);
				}
				command="UPDATE procedurecode SET PreExisting =1";//to indicate which procedurecodes existed before this version.
				Db.NonQ32(command);
				command="INSERT INTO preference VALUES ('ADAComplianceDateTime','')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.8.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_8_3();
		}

		///<summary>First version where individual computer preferences were introduced.</summary>
		private static void To4_8_3() {
			if(FromVersion<new Version("4.8.3.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.8.3");//No translation in convert script.
				//preferences should not be deleted since it causes bugs when upgrading.
				//string command="DELETE FROM preference WHERE PrefName='ToothChartLowerQuality'";
				//Db.NonQ32(command);
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS computerpref";
					Db.NonQ32(command);
					command=@"CREATE TABLE computerpref(
						ComputerPrefNum int NOT NULL auto_increment,
						ComputerName varchar(64) NOT NULL,
						GraphicsUseHardware tinyint(1) NOT NULL default '0',
						GraphicsSimple tinyint(1) NOT NULL default '0',
						PRIMARY KEY (ComputerPrefNum)
						) DEFAULT CHARSET=utf8";
				}else {//Assume Oracle
					command=@"CREATE TABLE computerpref(
						ComputerPrefNum int NOT NULL,
						ComputerName varchar(64) NOT NULL,
						GraphicsUseHardware number(1,0) default '0' NOT NULL,
						GraphicsSimple number(1,0) default '0' NOT NULL,
						PRIMARY KEY (ComputerPrefNum)
						)";
				}
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '4.8.3.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_8_8();
		}

		///<summary></summary>
		private static void To4_8_8() {
			if(FromVersion<new Version("4.8.8.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.8.8");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE quickpastenote CHANGE QuickPasteCatNum QuickPasteCatNum mediumint NOT NULL";
					Db.NonQ32(command);
				}
				else {//Oracle
					//Cannot specify 'NOT NULL' when already not null. Additionally, not specifying 'NOT NULL' leaves
					//the column as not null, so the following Oracle statement is equivalent the the above MySQL.
					command="ALTER TABLE quickpastenote MODIFY (QuickPasteCatNum number(5,0))";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '4.8.8.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_8_9();
		}

		///<summary>This conversion converts all medium ints in the database to simple signed ints, because the medium int size restriction is causing some key duplication in random primary key generations, and because we want the integers in the database to be complatible with c# ints. Also, using the syntax with regular ints works for both MySQL and Oracle, whereas the mediumint keyword is invalid in Oracle.</summary>
		private static void To4_8_9() {
			if(FromVersion<new Version("4.8.9.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.8.9");//No translation in convert script.
				//The table and column names to change, plus a bool to decide weather to specify the NOT NULL condition and a bool to specify if the column is autoincrement. Table format is in groups (table name, column name, auto increment (a or yes, space for no))
				string[] columns=new string[] {
					//TABLE NAME											COLUMN NAME															AUTO-INCREMENT
					@"account",												@"AccountNum",													@"a",
					@"accountingautopay",							@"AccountingAutoPayNum",								@"a",
					@"adjustment",										@"AdjNum",															@"a",
					@"adjustment",										@"PatNum",															@" ",
					@"adjustment",										@"ProvNum",															@" ",
					@"adjustment",										@"ProcNum",															@" ",
					@"appointment",										@"AptNum",															@"a",
					@"appointment",										@"PatNum",															@" ",
					@"appointment",										@"NextAptNum",													@" ",
					@"appointment",										@"InstructorNum",												@" ",
					@"appointment",										@"SchoolClassNum",											@" ",
					@"appointment",										@"SchoolCourseNum",											@" ",
					@"appointmentrule",								@"AppointmentRuleNum",									@"a",
					@"apptviewitem",									@"ApptViewItemNum",											@"a",
					@"benefit",												@"BenefitNum",													@"a",
					@"benefit",												@"PlanNum",															@" ",
					@"benefit",												@"PatPlanNum",													@" ",
					@"benefit",												@"CovCatNum",														@" ",
					@"canadianclaim",									@"ClaimNum",														@" ",
					@"canadianextract",								@"CanadianExtractNum",									@"a",
					@"canadianextract",								@"ClaimNum",														@" ",
					@"canadiannetwork",								@"CanadianNetworkNum",									@"a",
					@"carrier",												@"CarrierNum",													@"a",
					@"carrier",												@"CanadianNetworkNum",									@" ",
					@"claim",													@"ClaimNum",														@"a",
					@"claim",													@"PatNum",															@" ",
					@"claim",													@"PlanNum",															@" ",
					@"claim",													@"PlanNum2",														@" ",
					@"claimform",											@"ClaimFormNum",												@"a",
					@"claimformitem",									@"ClaimFormItemNum",										@"a",
					@"claimformitem",									@"ClaimFormNum",												@" ",
					@"claimpayment",									@"ClaimPaymentNum",											@"a",
					@"claimpayment",									@"DepositNum",													@" ",
					@"claimproc",											@"ClaimProcNum",												@"a",
					@"claimproc",											@"ProcNum",															@" ",
					@"claimproc",											@"ClaimNum",														@" ",
					@"claimproc",											@"PatNum",															@" ",
					@"claimproc",											@"ClaimPaymentNum",											@" ",
					@"claimproc",											@"PlanNum",															@" ",
					@"clearinghouse",									@"ClearinghouseNum",										@"a",
					@"clockevent",										@"ClockEventNum",												@"a",
					@"commlog",												@"CommlogNum",													@"a",
					@"commlog",												@"PatNum",															@" ",
					@"computer",											@"ComputerNum",													@"a",
					@"contact",												@"ContactNum",													@"a",
					@"covcat",												@"CovCatNum",														@"a",
					@"covspan",												@"CovSpanNum",													@"a",
					@"covspan",												@"CovCatNum",														@" ",
					@"deposit",												@"DepositNum",													@"a",
					@"disease",												@"DiseaseNum",													@"a",
					@"disease",												@"PatNum",															@" ",
					@"disease",												@"DiseaseDefNum",												@" ",
					@"diseasedef",										@"DiseaseDefNum",												@"a",
					@"document",											@"DocNum",															@"a",
					@"document",											@"PatNum",															@" ",
					@"document",											@"CropX",																@" ",
					@"document",											@"CropY",																@" ",
					@"document",											@"CropW",																@" ",
					@"document",											@"CropH",																@" ",
					@"document",											@"WindowingMin",												@" ",
					@"document",											@"WindowingMax",												@" ",
					@"electid",												@"ElectIDNum",													@"a",
					@"emailattach",										@"EmailAttachNum",											@"a",
					@"emailattach",										@"EmailMessageNum",											@" ",
					@"emailmessage",									@"EmailMessageNum",											@"a",
					@"emailmessage",									@"PatNum",															@" ",
					@"emailtemplate",									@"EmailTemplateNum",										@"a",
					@"employer",											@"EmployerNum",													@"a",
					@"etrans",												@"EtransNum",														@"a",
					@"etrans",												@"ClearinghouseNum",										@" ",
					@"etrans",												@"ClaimNum",														@" ",
					@"etrans",												@"OfficeSequenceNumber",								@" ",
					@"etrans",												@"CarrierTransCounter",									@" ",
					@"etrans",												@"CarrierTransCounter2",								@" ",
					@"etrans",												@"CarrierNum",													@" ",
					@"etrans",												@"CarrierNum2 ",												@" ",
					@"etrans",												@"PatNum",															@" ",
					@"fee",														@"FeeNum",															@"a",
					@"formpat",												@"FormPatNum",													@"a",
					@"formpat",												@"PatNum",															@" ",
					@"graphicpoint",									@"GPointNum",														@"a",
					@"grouppermission",								@"GroupPermNum",												@"a",
					@"grouppermission",								@"NewerDays",														@" ",
					@"grouppermission",								@"UserGroupNum",												@" ",
					@"insplan",												@"PlanNum",															@"a",
					@"insplan",												@"Subscriber",													@" ",
					@"insplan",												@"EmployerNum",													@" ",
					@"insplan",												@"CarrierNum",													@" ",
					@"instructor",										@"InstructorNum",												@"a",
					@"journalentry",									@"JournalEntryNum",											@"a",
					@"journalentry",									@"TransactionNum",											@" ",
					@"journalentry",									@"AccountNum",													@" ",
					@"journalentry",									@"ReconcileNum",												@" ",
					@"labcase",												@"LabCaseNum",													@"a",
					@"labcase",												@"PatNum",															@" ",
					@"labcase",												@"LaboratoryNum",												@" ",
					@"labcase",												@"AptNum",															@" ",
					@"labcase",												@"PlannedAptNum",												@" ",
					@"laboratory",										@"LaboratoryNum",												@"a",
					@"letter",												@"LetterNum",														@"a",
					@"lettermerge",										@"LetterMergeNum",											@"a",
					@"lettermergefield",							@"FieldNum",														@"a",
					@"lettermergefield",							@"LetterMergeNum",											@" ",
					@"medication",										@"MedicationNum",												@"a",
					@"medication",										@"GenericNum",													@" ",
					@"medicationpat",									@"MedicationPatNum",										@"a",
					@"medicationpat",									@"PatNum",															@" ",
					@"medicationpat",									@"MedicationNum",												@" ",
					@"mount",													@"PatNum",															@" ",
					@"mountdef",											@"ItemOrder",														@" ",
					@"mountdef",											@"Width",																@" ",
					@"mountdef",											@"Height",															@" ",
					@"mountitem",											@"Xpos",																@" ",
					@"mountitem",											@"Ypos",																@" ",
					@"mountitemdef",									@"Xpos",																@" ",
					@"mountitemdef",									@"Ypos",																@" ",
					@"mountitemdef",									@"Width",																@" ",
					@"mountitemdef",									@"Height",															@" ",
					@"patfield",											@"PatFieldNum",													@"a",
					@"patfield",											@"PatNum",															@" ",
					@"patfielddef",										@"PatFieldDefNum",											@"a",
					@"patient",												@"PatNum",															@"a",
					@"patient",												@"Guarantor",														@" ",
					@"patient",												@"NextAptNum",													@" ",
					@"patient",												@"EmployerNum",													@" ",
					@"patientnote",										@"PatNum",															@" ",
					@"patplan",												@"PatPlanNum",													@"a",
					@"patplan",												@"PatNum",															@" ",
					@"patplan",												@"PlanNum",															@" ",
					@"payment",												@"PayNum",															@"a",
					@"payment",												@"PatNum",															@" ",
					@"payment",												@"DepositNum",													@" ",
					@"payperiod",											@"PayPeriodNum",												@"a",
					@"payplan",												@"PayPlanNum",													@"a",
					@"payplan",												@"PatNum",															@" ",
					@"payplan",												@"Guarantor",														@" ",
					@"payplan",												@"PlanNum",															@" ",
					@"payplancharge",									@"PayPlanChargeNum",										@"a",
					@"payplancharge",									@"PayPlanNum",													@" ",
					@"payplancharge",									@"Guarantor",														@" ",
					@"payplancharge",									@"PatNum",															@" ",
					@"paysplit",											@"SplitNum",														@"a",
					@"paysplit",											@"PatNum",															@" ",
					@"paysplit",											@"PayNum",															@" ",
					@"paysplit",											@"PayPlanNum",													@" ",
					@"paysplit",											@"ProcNum",															@" ",
					@"perioexam",											@"PerioExamNum",												@"a",
					@"perioexam",											@"PatNum",															@" ",
					@"periomeasure",									@"PerioMeasureNum",											@"a",
					@"periomeasure",									@"PerioExamNum",												@" ",
					@"printer",												@"PrinterNum",													@"a",
					@"printer",												@"ComputerNum",													@" ",
					@"procedurelog",									@"ProcNum",															@"a",	
					@"procedurelog",									@"PatNum",															@" ",
					@"procedurelog",									@"AptNum",															@" ",
					@"procedurelog",									@"PlannedAptNum",												@" ",
					@"procedurelog",									@"ProcNumLab",													@" ",
					@"proclicense",										@"ProcLicenseNum",											@"a",
					@"procnote",											@"ProcNoteNum",													@"a",
					@"procnote",											@"PatNum",															@" ",
					@"procnote",											@"ProcNum",															@" ",
					@"procnote",											@"UserNum",															@" ",
					@"proctp",												@"ProcTPNum",														@"a",
					@"proctp",												@"TreatPlanNum",												@" ",
					@"proctp",												@"PatNum",															@" ",
					@"proctp",												@"ProcNumOrig",													@" ",
					@"program",												@"ProgramNum",													@"a",
					@"programproperty",								@"ProgramPropertyNum",									@"a",
					@"programproperty",								@"ProgramNum",													@" ",
					@"provider",											@"SchoolClassNum",											@" ",
					@"providerident",									@"ProviderIdentNum",										@"a",
					@"question",											@"QuestionNum",													@"a",
					@"question",											@"PatNum",															@" ",
					@"question",											@"FormPatNum",													@" ",
					@"questiondef",										@"QuestionDefNum",											@"a",
					@"quickpastecat",									@"QuickPasteCatNum",										@"a",
					@"quickpastenote",								@"QuickPasteNoteNum",										@"a",
					@"quickpastenote",								@"QuickPasteCatNum",										@" ",
					@"recall",												@"RecallNum",														@"a",
					@"recall",												@"PatNum",															@" ",			
					@"reconcile",											@"ReconcileNum",												@"a",
					@"reconcile",											@"AccountNum",													@" ",
					@"refattach",											@"ReferralNum",													@" ",
					@"refattach",											@"PatNum",															@" ",
					@"referral",											@"ReferralNum",													@"a",
					@"referral",											@"PatNum",															@" ",
					@"repeatcharge",									@"RepeatChargeNum",											@"a",
					@"repeatcharge",									@"PatNum",															@" ",
					@"rxalert",												@"RxAlertNum",													@"a",
					@"rxalert",												@"RxDefNum",														@" ",
					@"rxalert",												@"DiseaseDefNum",												@" ",
					@"rxpat",													@"RxNum",																@"a",
					@"rxpat",													@"PatNum",															@" ",
					@"schedule",											@"ScheduleNum",													@"a",
					@"schoolclass",										@"SchoolClassNum",											@"a",
					@"schoolclass",										@"GradYear",														@" ",
					@"schoolcourse",									@"SchoolCourseNum",											@"a",
					@"screen",												@"ScreenNum",														@"a",
					@"screen",												@"ScreenGroupNum",											@" ",
					@"screengroup",										@"ScreenGroupNum",											@"a",
					@"securitylog",										@"SecurityLogNum",											@"a",
					@"securitylog",										@"UserNum",															@" ",
					@"securitylog",										@"PatNum",															@" ",
					@"sigelement",										@"SigElementNum",												@"a",
					@"sigelement",										@"SignalNum",														@" ",
					@"signal",												@"SignalNum",														@"a",
					@"task",													@"TaskNum",															@"a",
					@"task",													@"TaskListNum",													@" ",
					@"task",													@"KeyNum",															@" ",
					@"task",													@"FromNum",															@" ",
					@"tasklist",											@"TaskListNum",													@"a",
					@"tasklist",											@"Parent",															@" ",
					@"tasklist",											@"FromNum",															@" ",
					@"terminalactive",								@"TerminalActiveNum",										@"a",
					@"terminalactive",								@"PatNum",															@" ",
					@"timeadjust",										@"TimeAdjustNum",												@"a",
					@"timeadjust",										@"EmployeeNum",													@" ",
					@"toolbutitem",										@"ToolButItemNum",											@"a",
					@"toolbutitem",										@"ProgramNum",													@" ",
					@"toothinitial",									@"ToothInitialNum",											@"a",
					@"toothinitial",									@"PatNum",															@" ",
					@"transaction",										@"TransactionNum",											@"a",
					@"transaction",										@"UserNum",															@" ",
					@"transaction",										@"DepositNum",													@" ",
					@"transaction",										@"PayNum",															@" ",
					@"treatplan",											@"TreatPlanNum",												@"a",
					@"treatplan",											@"PatNum",															@" ",
					@"usergroup",											@"UserGroupNum",												@"a",
					@"userod",												@"UserNum",															@"a",
					@"userod",												@"UserGroupNum",												@" ",
					@"userod",												@"ClinicNum",														@" ",
					@"zipcode",												@"ZipCodeNum",													@"a",
				};
				string command="";
				for(int i=0;i<columns.Length;i+=3) {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command=@"ALTER TABLE "+columns[i]//table name
						+@" CHANGE "+columns[i+1]+@" "+columns[i+1]+@" int NOT NULL ";//column name (all columns will be NOT NULL)
						if(columns[i+2]!=" ") {
							command+=@"auto_increment ";
						}
					}else {//Oracle.
						command=@"ALTER TABLE "+columns[i]//table name
							+@" MODIFY ("+columns[i+1]+" int)";
						//Cannot specify 'NOT NULL' when already not null column. Additionally, not specifying 'NOT NULL' leaves
						//the column as 'NOT NULL' if it was defined that way. If a column allows null, then we should not specify
						//anything here. Finally, since all of the above columns already exist in the database and have 
						//sequence/trigger pairs for those which are auto_incement, we do not need to worry about the auto_increment
						//flag here (columns[i+2]).
					}
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '4.8.9.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_9_1();
		}

		///<summary></summary>
		private static void To4_9_1() {
			if(FromVersion<new Version("4.9.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.9.1");//No translation in convert script.
				string command;
				DataTable table;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurecode ADD CodeNum int NOT NULL FIRST";//this column will be the new primary key
					Db.NonQ32(command);
					command="ALTER TABLE procedurecode DROP PRIMARY KEY";
					Db.NonQ32(command);
					command="ALTER TABLE procedurecode ADD PRIMARY KEY (CodeNum), CHANGE CodeNum CodeNum int NOT NULL auto_increment";
					Db.NonQ32(command);
					command="ALTER TABLE procedurecode CHANGE ADACode ProcCode varchar(15) character set utf8 collate utf8_bin NOT NULL default ''";
					Db.NonQ32(command);
					command="ALTER TABLE procedurecode ADD INDEX (ProcCode)";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD CodeNum int NOT NULL";
					Db.NonQ32(command);
					//this is written in such a way as to be compatible with Oracle.
					command="UPDATE procedurelog SET procedurelog.CodeNum= (SELECT procedurecode.CodeNum FROM procedurecode WHERE procedurecode.ProcCode=procedurelog.ADACode)";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog CHANGE ADACode OldCode varchar(15) character set utf8 collate utf8_bin NOT NULL default ''";
					Db.NonQ32(command);
					//added after r215
					command="UPDATE procedurelog SET OldCode=''";
					Db.NonQ32(command);
					command="ALTER TABLE fee CHANGE ADACode OldCode varchar(15) character set utf8 collate utf8_bin NOT NULL default ''";
					Db.NonQ32(command);
					command="ALTER TABLE fee ADD CodeNum int NOT NULL";
					Db.NonQ32(command);
					command="UPDATE fee SET fee.CodeNum= (SELECT procedurecode.CodeNum FROM procedurecode WHERE procedurecode.ProcCode=fee.OldCode)";
					Db.NonQ32(command);
					command="UPDATE fee SET OldCode=''";
					Db.NonQ32(command);
					//added after r216
					command="ALTER TABLE appointmentrule CHANGE ADACodeStart CodeStart varchar(15)";
					Db.NonQ32(command);
					command="ALTER TABLE appointmentrule CHANGE ADACodeEnd CodeEnd varchar(15)";
					Db.NonQ32(command);
					//added after r217
					command="ALTER TABLE autocodeitem CHANGE ADACode OldCode varchar(15) character set utf8 collate utf8_bin NOT NULL default ''";
					Db.NonQ32(command);
					command="ALTER TABLE autocodeitem ADD CodeNum int NOT NULL";
					Db.NonQ32(command);
					command="UPDATE autocodeitem SET autocodeitem.CodeNum= (SELECT procedurecode.CodeNum FROM procedurecode WHERE procedurecode.ProcCode=autocodeitem.OldCode)";
					Db.NonQ32(command);
					command="UPDATE autocodeitem SET OldCode=''";
					Db.NonQ32(command);
					//added after r218
					command="ALTER TABLE benefit CHANGE ADACode OldCode varchar(15) character set utf8 collate utf8_bin NOT NULL default ''";
					Db.NonQ32(command);
					command="ALTER TABLE benefit ADD CodeNum int NOT NULL";
					Db.NonQ32(command);
					command="UPDATE benefit SET benefit.CodeNum= (SELECT procedurecode.CodeNum FROM procedurecode WHERE procedurecode.ProcCode=benefit.OldCode)";
					Db.NonQ32(command);
					command="UPDATE benefit SET OldCode=''";
					Db.NonQ32(command);
					command="DELETE FROM procedurecode WHERE ProcCode=''";
					Db.NonQ32(command);
					command="UPDATE benefit SET CodeNum=0 WHERE NOT EXISTS(SELECT * FROM procedurecode WHERE "
						+"benefit.CodeNum=procedurecode.CodeNum)";
					Db.NonQ32(command);
					command="DELETE FROM procedurelog WHERE NOT EXISTS(SELECT * FROM procedurecode WHERE "
						+"procedurelog.CodeNum=procedurecode.CodeNum)";
					Db.NonQ32(command);
					//added after r219
					command="ALTER TABLE procbuttonitem CHANGE ADACode OldCode varchar(15) character set utf8 collate utf8_bin NOT NULL default ''";
					Db.NonQ32(command);
					command="ALTER TABLE procbuttonitem ADD CodeNum int NOT NULL";
					Db.NonQ32(command);
					command="UPDATE procbuttonitem SET procbuttonitem.CodeNum= (SELECT procedurecode.CodeNum FROM procedurecode WHERE procedurecode.ProcCode=procbuttonitem.OldCode)";
					Db.NonQ32(command);
					command="UPDATE procbuttonitem SET OldCode=''";
					Db.NonQ32(command);
					//added after r220
					command="ALTER TABLE proclicense CHANGE ADACode ProcCode varchar(15)";
					Db.NonQ32(command);
					command="ALTER TABLE proctp CHANGE ADACode ProcCode varchar(15)";
					Db.NonQ32(command);
					command="ALTER TABLE repeatcharge CHANGE ADACode ProcCode varchar(15)";
					Db.NonQ32(command);
					//added after r238
					command="ALTER TABLE labcase ADD ProvNum int NOT NULL";
					Db.NonQ32(command);
					//added after r240
					command="ALTER TABLE labcase ADD Instructions text";
					Db.NonQ32(command);
					//added after r243
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor) VALUES(12,7,"
						+"'LabCase',-65536)";
					Db.NonQ32(command);
					//commented this "Appointment" option out after r294 and added it back with other related custom colors after r293
					//command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor) VALUES(12,8,"
					//	+"'Appointment',-8388480)";
					//Db.NonQ32(command);
					//Added after r244
					command="DROP TABLE IF EXISTS labturnaround";
					Db.NonQ32(command);
					command=@"CREATE TABLE labturnaround(
					LabTurnaroundNum int NOT NULL auto_increment,
					LaboratoryNum int NOT NULL,
					Description varchar(255),
					DaysPublished smallint NOT NULL,
					DaysActual smallint NOT NULL,
					PRIMARY KEY (LabTurnaroundNum)
					) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					//Added after r250
					int laboratoryNum=0;
					//Determine the first available primary key number in the database. This is necessary because a sequence/trigger
					//pair may not yet exist in the database from being run in the database maintenence tool. Also, this table is small,
					//so incrementing by 1 will not waste much time.
					do {
						laboratoryNum++;
						command="SELECT LaboratoryNum FROM laboratory WHERE LaboratoryNum='"+laboratoryNum+"'";
						table=Db.GetTable(command);
					} while(table.Rows.Count>0);
					command="INSERT INTO laboratory (LaboratoryNum,Description,Phone,Notes,LabSlip) VALUES('"
						+laboratoryNum+"','Default Lab','','','')";
					Db.NonQ32(command);
					command="SELECT * FROM appointment WHERE Lab != 0";
					table=Db.GetTable(command);
					for(int i=0;i<table.Rows.Count;i++) {
						command="INSERT INTO labcase (LabCaseNum,PatNum,LaboratoryNum,AptNum,PlannedAptNum,DateTimeDue,DateTimeCreated,"
							+"DateTimeSent,DateTimeRecd,DateTimeChecked,ProvNum,Instructions) VALUES("
							+"'"+(i+1)+"', "
							+table.Rows[i]["PatNum"].ToString()+", "
							+POut.Long(laboratoryNum)+", ";
						if(table.Rows[i]["AptStatus"].ToString()=="6") {//if planned apt
							command+="0, "//AptNum
								+table.Rows[i]["AptNum"].ToString()+", ";//PlannedAptNum
						} else {
							command+=table.Rows[i]["AptNum"].ToString()+", "//AptNum
								+"0, ";//PlannedAptNum
						}
						command+=POut.DateT(PIn.DateT(table.Rows[i]["AptNum"].ToString()))+", "//DateTimeDue
							+POut.Date(DateTime.MinValue)+", ";//DateTimeCreated
						if(table.Rows[i]["Lab"].ToString()=="1") {//sent
							command+=POut.DateT(new DateTime(2000,1,1))+", "//DateTimeSent
								+POut.Date(DateTime.MinValue)+", "//DateTimeRecd
								+POut.Date(DateTime.MinValue)+", ";//DateTimeChecked
						} else if(table.Rows[i]["Lab"].ToString()=="2") {//received
							command+=POut.DateT(new DateTime(2000,1,1))+", "//DateTimeSent
								+POut.Date(new DateTime(2000,1,1))+", "//DateTimeRecd
								+POut.Date(DateTime.MinValue)+", ";//DateTimeChecked
						} else if(table.Rows[i]["Lab"].ToString()=="3") {//checked
							command+=POut.DateT(new DateTime(2000,1,1))+", "//DateTimeSent
								+POut.Date(new DateTime(2000,1,1))+", "//DateTimeRecd
								+POut.Date(new DateTime(2000,1,1))+", ";//DateTimeChecked
						}
						command+=table.Rows[i]["ProvNum"].ToString()+", "
							+"'')";
						Db.NonQ32(command);
					}
					//Added after r269
					command="INSERT INTO preference VALUES('PrintSimpleStatements','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('BrokenApptCommLogNotAdjustment','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('ShowNotesInAccount','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('PlannedApptTreatedAsRegularAppt','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('BoldFamilyAccountBalanceView','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('ShowProgressNotesInsteadofCommLog','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('ShowUrgFinNoteInProgressNotes','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('StationaryImage','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('StationaryDocument','0')";
					Db.NonQ32(command);
					//Added after r271
					command="ALTER TABLE schedule ADD EmployeeNum int NOT NULL";
					Db.NonQ32(command);
					//Added after r278
					command="ALTER TABLE procedurelog DROP CPTModifier";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog DROP RevenueCode";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD CodeMod1 char(2)";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD CodeMod2 char(2)";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD CodeMod3 char(2)";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD CodeMod4 char(2)";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD RevCode varchar(45)";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD UnitCode char(2)";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD UnitQty char(15)";
					Db.NonQ32(command);
					//added after r283
					command="INSERT INTO preference VALUES('ScheduleProvUnassigned','1')";
					Db.NonQ32(command);
					//this next one is hard to run manually and can be skipped by developers:
					command="UPDATE preference Set ValueString= '"+PrefC.GetLong(PrefName.PracticeDefaultProv).ToString()
						+"' WHERE PrefName='ScheduleProvUnassigned'";
					Db.NonQ32(command);
					//added after r292
					command="INSERT INTO preference VALUES('AccountingLockDate','')";
					Db.NonQ32(command);
					//added after r294
					//this first line replaces the one commented out earlier in v4.9
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor,IsHidden) VALUES(12,8,"
					+"'Appointment Text - Today',-8388480,0)";
					Db.NonQ32(command);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor,IsHidden) VALUES(12,9,'Appointment Background - Today',-886,0)";//yellow
					Db.NonQ32(command);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor,IsHidden) VALUES(12,10,'Past Appointment Text',-8388480,0)";//purple
					Db.NonQ32(command);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor,IsHidden) VALUES(12,11,'Past Appointment Background','-1',0)";//white
					Db.NonQ32(command);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor,IsHidden) VALUES(12,12,'Future Appointment Text',-8388480,0)";//purple
					Db.NonQ32(command);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor,IsHidden) VALUES(12,13,'Future Appointment Background','-7278960',0)";//green
					Db.NonQ32(command);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor,IsHidden) VALUES(12,14,'Broken/Unschd Appt Text','-8388480',0)";
					Db.NonQ32(command);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor,IsHidden) VALUES(12,15,'Broken/Unschd Appt Background','-1',0)";//white
					Db.NonQ32(command);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor,IsHidden) VALUES(12,16,'Planned Appointment Text',-8388480,0)";//purple
					Db.NonQ32(command);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor,IsHidden) VALUES(12,17,'Planned Appointment Background',-1,0)";
					Db.NonQ32(command);
					//After r299
					command="INSERT INTO preference VALUES('FuchsListSelectionColor','')";
					Db.NonQ32(command);
					//added after r301
					//Load ADA2006 claimform (without background)---------------------------------------------------------------------
					try {
						/*
						int claimFormNum=FormClaimForms.ImportForm("",true,Properties.Resources.ClaimForm2006);
						if(CultureInfo.CurrentCulture.Name=="en-US") {
							command="UPDATE preference SET ValueString="+POut.PInt(claimFormNum)+" WHERE PrefName='DefaultClaimForm'";
							Db.NonQ32(command);
						}
						command="UPDATE insplan SET ClaimFormNum="+POut.PInt(claimFormNum)
							+" WHERE insplan.ClaimFormNum= (SELECT claimform.ClaimFormNum FROM claimform WHERE claimform.UniqueID='OD1')";
						Db.NonQ32(command);
						command="UPDATE insplan SET ClaimFormNum="+POut.PInt(claimFormNum)+" WHERE insplan.ClaimFormNum=0";
						Db.NonQ32(command);*/
					} catch {
						//user will have to do it manually
					}
					//added after r303
					command="INSERT INTO preference VALUES('RegistrationKey','')";
					Db.NonQ32(command);
					//After r304
					command="INSERT INTO preference VALUES('DistributorKey','')";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS registrationkey";
					Db.NonQ32(command);
					command=@"CREATE TABLE registrationkey(
						RegistrationKeyNum int NOT NULL auto_increment,
						PatNum int NOT NULL,
						RegKey varchar(4000),
						Note varchar(4000),
						PRIMARY KEY (RegistrationKeyNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					//After r306
					command="ALTER TABLE preference CHANGE ValueString ValueString text NOT NULL default ''";
					Db.NonQ32(command);
				} else {//oracle
					//Here we want to add a new column CodeNum and make it the primary key in the procedurecode table.
					//However, it appears to be difficult to change a table primary key, so here we create a backup of
					//the old procedurecode table and recreate the procedure code table, copying in the old data. Also,
					//we desire the primary key to be the first column, which also appears to be difficult to specify in Oracle.
					command="ALTER TABLE procedurecode RENAME COLUMN ADACode TO ProcCode";
					Db.NonQ32(command);
					command="ALTER TABLE procedurecode RENAME TO procedurecodeold";
					Db.NonQ32(command);
					command=@"CREATE TABLE procedurecode (
					CodeNum int default '0' NOT NULL,
					ProcCode varchar2(15) NOT NULL,
					Descript varchar2(255) default '',
					AbbrDesc varchar2(50) default '',
					ProcTime varchar2(24) default '',
					ProcCat int default '0' NOT NULL,
					TreatArea int default '0' NOT NULL,
					RemoveTooth int default '0' NOT NULL,
					SetRecall int default '0' NOT NULL,
					NoBillIns int default '0' NOT NULL,
					IsProsth int default '0' NOT NULL,
					DefaultNote varchar2(4000),
					IsHygiene int default '0' NOT NULL,
					GTypeNum int default '0' NOT NULL,
					AlternateCode1 varchar2(15) default '',
					MedicalCode varchar2(15) default '',
					IsTaxed int default '0' NOT NULL,
					PaintType int default '0' NOT NULL,
					GraphicColor int default '0' NOT NULL,
					LaymanTerm varchar2(255) default '',
					IsCanadianLab int default '0' NOT NULL,
					PreExisting int default '0' NOT NULL,
					PRIMARY KEY(CodeNum)
					)";
					Db.NonQ32(command);
					command="CREATE INDEX ind_procedurecode_ProcCode ON procedurecode (ProcCode)";
					Db.NonQ32(command);
					command="SELECT * FROM procedurecodeold";
					table=Db.GetTable(command);
					for(int i=0;i<table.Rows.Count;i++) {
						//Must specify CodeNum here, because auto-incrementation does not take place until an appropriate trigger
						//and sequence are created in the database maintenence tool after this database upgrade is completed. Since
						//the column is new, we can just assign consecutive values to keep things simple.
						command="INSERT INTO procedurecode (CodeNum,ProcCode,Descript,AbbrDesc,ProcTime,"
							+"ProcCat,TreatArea,RemoveTooth,SetRecall,NoBillIns,IsProsth,DefaultNote,"
							+"IsHygiene,GTypeNum,AlternateCode1,MedicalCode,IsTaxed,PaintType,"
							+"GraphicColor,LaymanTerm,IsCanadianLab,PreExisting) VALUES ("
							+"'"+POut.Long(i+1)+"',"
							+"'"+POut.String(PIn.String(table.Rows[i]["ProcCode"].ToString()))+"',"
							+"'"+POut.String(PIn.String(table.Rows[i]["Descript"].ToString()))+"',"
							+"'"+POut.String(PIn.String(table.Rows[i]["AbbrDesc"].ToString()))+"',"
							+"'"+POut.String(PIn.String(table.Rows[i]["ProcTime"].ToString()))+"',"
							+"'"+POut.Long(PIn.Long(table.Rows[i]["ProcCat"].ToString()))+"',"
							+"'"+POut.Long(PIn.Long(table.Rows[i]["TreatArea"].ToString()))+"',"
							+"'"+POut.Long(PIn.Long(table.Rows[i]["RemoveTooth"].ToString()))+"',"
							+"'"+POut.Long(PIn.Long(table.Rows[i]["SetRecall"].ToString()))+"',"
							+"'"+POut.Long(PIn.Long(table.Rows[i]["NoBillIns"].ToString()))+"',"
							+"'"+POut.String(PIn.String(table.Rows[i]["IsProsth"].ToString()))+"',"
							+"'"+POut.Long(PIn.Long(table.Rows[i]["DefaultNote"].ToString()))+"',"
							+"'"+POut.Long(PIn.Long(table.Rows[i]["IsHygiene"].ToString()))+"',"
							+"'"+POut.String(PIn.String(table.Rows[i]["GTypeNum"].ToString()))+"',"
							+"'"+POut.String(PIn.String(table.Rows[i]["AlternateCode1"].ToString()))+"',"
							+"'"+POut.String(PIn.String(table.Rows[i]["MedicalCode"].ToString()))+"',"
							+"'"+POut.Long(PIn.Long(table.Rows[i]["IsTaxed"].ToString()))+"',"
							+"'"+POut.Long(PIn.Long(table.Rows[i]["PaintType"].ToString()))+"',"
							+"'"+POut.Long(PIn.Long(table.Rows[i]["GraphicColor"].ToString()))+"',"
							+"'"+POut.String(PIn.String(table.Rows[i]["LaymanTerm"].ToString()))+"',"
							+"'"+POut.Long(PIn.Long(table.Rows[i]["IsCanadianLab"].ToString()))+"',"
							+"'"+POut.Long(PIn.Long(table.Rows[i]["PreExisting"].ToString()))+"')";
						Db.NonQ32(command);
					}
					command="ALTER TABLE procedurelog ADD CodeNum int default '0' NOT NULL";
					Db.NonQ32(command);
					//this is written in such a way as to be compatible with Oracle.
					command="UPDATE procedurelog SET procedurelog.CodeNum= (SELECT procedurecode.CodeNum FROM procedurecode WHERE procedurecode.ProcCode=procedurelog.ADACode)";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog RENAME COLUMN ADACode TO OldCode";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog MODIFY (OldCode varchar2(15))";
					Db.NonQ32(command);
					//added after r215
					command="UPDATE procedurelog SET OldCode=''";
					Db.NonQ32(command);
					command="ALTER TABLE fee RENAME COLUMN ADACode TO OldCode";
					Db.NonQ32(command);
					command="ALTER TABLE fee MODIFY (OldCode varchar2(15))";
					Db.NonQ32(command);
					command="ALTER TABLE fee ADD CodeNum int default '0' NOT NULL";
					Db.NonQ32(command);
					command="UPDATE fee SET fee.CodeNum=(SELECT procedurecode.CodeNum FROM procedurecode WHERE procedurecode.ProcCode=fee.OldCode)";
					Db.NonQ32(command);
					command="UPDATE fee SET OldCode=''";
					Db.NonQ32(command);
					//added after r216
					command="ALTER TABLE appointmentrule RENAME COLUMN ADACodeStart TO CodeStart";
					Db.NonQ32(command);
					command="ALTER TABLE appointmentrule MODIFY (CodeStart varchar2(15))";
					Db.NonQ32(command);
					command="ALTER TABLE appointmentrule RENAME COLUMN ADACodeEnd TO CodeEnd";
					Db.NonQ32(command);
					command="ALTER TABLE appointmentrule MODIFY (CodeEnd varchar2(15))";
					Db.NonQ32(command);
					//added after r217
					command="ALTER TABLE autocodeitem RENAME COLUMN ADACode TO OldCode";
					Db.NonQ32(command);
					command="ALTER TABLE autocodeitem MODIFY (OldCode varchar2(15))";
					Db.NonQ32(command);
					command="ALTER TABLE autocodeitem ADD CodeNum int default '0' NOT NULL";
					Db.NonQ32(command);
					command="UPDATE autocodeitem SET autocodeitem.CodeNum=(SELECT procedurecode.CodeNum FROM procedurecode WHERE procedurecode.ProcCode=autocodeitem.OldCode)";
					Db.NonQ32(command);
					command="UPDATE autocodeitem SET OldCode=''";
					Db.NonQ32(command);
					//added after r218
					command="ALTER TABLE benefit RENAME COLUMN ADACode TO OldCode";
					Db.NonQ32(command);
					command="ALTER TABLE benefit MODIFY (OldCode varchar2(15))";
					Db.NonQ32(command);
					command="ALTER TABLE benefit ADD CodeNum int default '0' NOT NULL";
					Db.NonQ32(command);
					command="UPDATE benefit SET benefit.CodeNum=(SELECT procedurecode.CodeNum FROM procedurecode WHERE procedurecode.ProcCode=benefit.OldCode)";
					Db.NonQ32(command);
					command="UPDATE benefit SET OldCode=''";
					Db.NonQ32(command);
					command="DELETE FROM procedurecode WHERE ProcCode=''";
					Db.NonQ32(command);
					command="UPDATE benefit SET CodeNum=0 WHERE NOT EXISTS(SELECT * FROM procedurecode WHERE "
						+"benefit.CodeNum=procedurecode.CodeNum)";
					Db.NonQ32(command);
					command="DELETE FROM procedurelog WHERE NOT EXISTS(SELECT * FROM procedurecode WHERE "
						+"procedurelog.CodeNum=procedurecode.CodeNum)";
					Db.NonQ32(command);
					//added after r219
					command="ALTER TABLE procbuttonitem RENAME COLUMN ADACode TO OldCode";
					Db.NonQ32(command);
					command="ALTER TABLE procbuttonitem MODIFY (OldCode varchar2(15))";
					Db.NonQ32(command);
					command="DELETE FROM procbuttonitem WHERE EXISTS(SELECT procedurecode.CodeNum FROM procedurecode,procbuttonitem WHERE procedurecode.ProcCode=procbuttonitem.OldCode)";
					Db.NonQ32(command);
					command="ALTER TABLE procbuttonitem ADD CodeNum int default '0' NOT NULL";
					Db.NonQ32(command);
					command="UPDATE procbuttonitem SET procbuttonitem.CodeNum=(SELECT procedurecode.CodeNum FROM procedurecode WHERE procedurecode.ProcCode=procbuttonitem.OldCode)";
					Db.NonQ32(command);
					command="UPDATE procbuttonitem SET OldCode=''";
					Db.NonQ32(command);
					//added after r220
					command="ALTER TABLE proclicense RENAME COLUMN ADACode TO ProcCode";
					Db.NonQ32(command);
					command="ALTER TABLE proclicense MODIFY (ProcCode varchar2(15))";
					Db.NonQ32(command);
					command="ALTER TABLE proctp RENAME COLUMN ADACOde TO ProcCode";
					Db.NonQ32(command);
					command="ALTER TABLE proctp MODIFY (ProcCode varchar2(15))";
					Db.NonQ32(command);
					command="ALTER TABLE repeatcharge RENAME COLUMN ADACode TO ProcCode";
					Db.NonQ32(command);
					command="ALTER TABLE repeatcharge MODIFY (ProcCode varchar2(15))";
					Db.NonQ32(command);
					//added after r238
					command="ALTER TABLE labcase ADD ProvNum int default '0' NOT NULL";
					Db.NonQ32(command);
					//added after r240
					command="ALTER TABLE labcase ADD Instructions varchar2(4000)";
					Db.NonQ32(command);
					//added after r243
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemColor,IsHidden) "
						+"VALUES((SELECT MAX(DefNum)+1 FROM definition),12,7,'LabCase',-65536,0)";
					Db.NonQ32(command);
					//commented this "Appointment" option out after r294 and added it back with other related custom colors after r293
					//command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor,IsHidden) VALUES(12,8,"
					//	+"'Appointment',-8388480,0)";
					//Db.NonQ32(command);
					//Added after r244
					command=@"CREATE TABLE labturnaround(
					LabTurnaroundNum int default '0' NOT NULL,
					LaboratoryNum int default '0' NOT NULL,
					Description varchar2(255),
					DaysPublished int default '0' NOT NULL,
					DaysActual int default '0' NOT NULL,
					PRIMARY KEY (LabTurnaroundNum)
					)";
					Db.NonQ32(command);
					//Added after r250
					command="INSERT INTO laboratory (LaboratoryNum,Description,Phone,Notes,LabSlip) "
						+"VALUES((SELECT CASE WHEN MAX(LaboratoryNum) IS NULL THEN 1 ELSE MAX(LaboratoryNum)+1 END FROM laboratory),'Default Lab','','','')";
					Db.NonQ32(command);
					command="SELECT * FROM appointment WHERE Lab != 0";
					table=Db.GetTable(command);
					for(int i=0;i<table.Rows.Count;i++) {
						command="INSERT INTO labcase (LabCaseNum,PatNum,LaboratoryNum,AptNum,PlannedAptNum,DateTimeDue,DateTimeCreated,"
							+"DateTimeSent,DateTimeRecd,DateTimeChecked,ProvNum,Instructions) VALUES("
							+"'"+(i+1)+"', "
							+table.Rows[i]["PatNum"].ToString()+", "
							+"(SELECT MAX(LaboratoryNum) FROM laboratory), ";
						if(table.Rows[i]["AptStatus"].ToString()=="6") {//if planned apt
							command+="0, "//AptNum
								+table.Rows[i]["AptNum"].ToString()+", ";//PlannedAptNum
						} else {
							command+=table.Rows[i]["AptNum"].ToString()+", "//AptNum
								+"0, ";//PlannedAptNum
						}
						command+=POut.DateT(PIn.DateT(table.Rows[i]["AptNum"].ToString()))+", "//DateTimeDue
							+POut.Date(DateTime.MinValue)+", ";//DateTimeCreated
						if(table.Rows[i]["Lab"].ToString()=="1") {//sent
							command+=POut.DateT(new DateTime(2000,1,1))+", "//DateTimeSent
								+POut.Date(DateTime.MinValue)+", "//DateTimeRecd
								+POut.Date(DateTime.MinValue)+", ";//DateTimeChecked
						} else if(table.Rows[i]["Lab"].ToString()=="2") {//received
							command+=POut.DateT(new DateTime(2000,1,1))+", "//DateTimeSent
								+POut.Date(new DateTime(2000,1,1))+", "//DateTimeRecd
								+POut.Date(DateTime.MinValue)+", ";//DateTimeChecked
						} else if(table.Rows[i]["Lab"].ToString()=="3") {//checked
							command+=POut.DateT(new DateTime(2000,1,1))+", "//DateTimeSent
								+POut.Date(new DateTime(2000,1,1))+", "//DateTimeRecd
								+POut.Date(new DateTime(2000,1,1))+", ";//DateTimeChecked
						}
						command+=table.Rows[i]["ProvNum"].ToString()+", "
							+"'')";
						Db.NonQ32(command);
					}
					//Added after r269
					command="INSERT INTO preference VALUES('PrintSimpleStatements','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('BrokenApptCommLogNotAdjustment','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('ShowNotesInAccount','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('PlannedApptTreatedAsRegularAppt','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('BoldFamilyAccountBalanceView','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('ShowProgressNotesInsteadofCommLog','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('ShowUrgFinNoteInProgressNotes','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('StationaryImage','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('StationaryDocument','0')";
					Db.NonQ32(command);
					//Added after r271
					command="ALTER TABLE schedule ADD EmployeeNum int";
					Db.NonQ32(command);
					//Added after r278
					command="ALTER TABLE procedurelog DROP COLUMN CPTModifier";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog DROP COLUMN RevenueCode";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD CodeMod1 varchar2(2)";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD CodeMod2 varchar2(2)";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD CodeMod3 varchar2(2)";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD CodeMod4 varchar2(2)";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD RevCode varchar2(45)";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD UnitCode varchar2(2)";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD UnitQty varchar2(15)";
					Db.NonQ32(command);
					//added after r283
					command="INSERT INTO preference VALUES('ScheduleProvUnassigned','1')";
					Db.NonQ32(command);
					//this next one is hard to run manually and can be skipped by developers:
					command="UPDATE preference Set ValueString= '"+PrefC.GetLong(PrefName.PracticeDefaultProv).ToString()
						+"' WHERE PrefName='ScheduleProvUnassigned'";
					Db.NonQ32(command);
					//added after r292
					command="INSERT INTO preference VALUES('AccountingLockDate','')";
					Db.NonQ32(command);
					//added after r294
					//this first line replaces the one commented out earlier in v4.9
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemColor,IsHidden) "
						+"VALUES((SELECT MAX(DefNum)+1 FROM definition),12,8,'Appointment Text - Today',-8388480,0)";
					Db.NonQ32(command);
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemColor,IsHidden) "
						+"VALUES((SELECT MAX(DefNum)+1 FROM definition),12,9,'Appointment Background - Today',-886,0)";//yellow
					Db.NonQ32(command);
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemColor,IsHidden) "
						+"VALUES((SELECT MAX(DefNum)+1 FROM definition),12,10,'Past Appointment Text',-8388480,0)";//purple
					Db.NonQ32(command);
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemColor,IsHidden) "
						+"VALUES((SELECT MAX(DefNum)+1 FROM definition),12,11,'Past Appointment Background','-1',0)";//white
					Db.NonQ32(command);
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemColor,IsHidden) "
						+"VALUES((SELECT MAX(DefNum)+1 FROM definition),12,12,'Future Appointment Text',-8388480,0)";//purple
					Db.NonQ32(command);
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemColor,IsHidden) "
						+"VALUES((SELECT MAX(DefNum)+1 FROM definition),12,13,'Future Appointment Background','-7278960',0)";//green
					Db.NonQ32(command);
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemColor,IsHidden) "
						+"VALUES((SELECT MAX(DefNum)+1 FROM definition),12,14,'Broken/Unschd Appt Text','-8388480',0)";
					Db.NonQ32(command);
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemColor,IsHidden) "
						+"VALUES((SELECT MAX(DefNum)+1 FROM definition),12,15,'Broken/Unschd Appt Background','-1',0)";//white
					Db.NonQ32(command);
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemColor,IsHidden) "
						+"VALUES((SELECT MAX(DefNum)+1 FROM definition),12,16,'Planned Appointment Text',-8388480,0)";//purple
					Db.NonQ32(command);
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemColor,IsHidden) "
						+"VALUES((SELECT MAX(DefNum)+1 FROM definition),12,17,'Planned Appointment Background',-1,0)";
					Db.NonQ32(command);
					//After r299
					command="INSERT INTO preference VALUES('FuchsListSelectionColor','')";
					Db.NonQ32(command);
					//added after r301
					//Load ADA2006 claimform (without background)---------------------------------------------------------------------
					try {
						/*
						int claimFormNum=FormClaimForms.ImportForm("",true,Properties.Resources.ClaimForm2006);
						if(CultureInfo.CurrentCulture.Name=="en-US") {
							command="UPDATE preference SET ValueString="+POut.PInt(claimFormNum)+" WHERE PrefName='DefaultClaimForm'";
							Db.NonQ32(command);
						}
						command="UPDATE insplan SET ClaimFormNum="+POut.PInt(claimFormNum)
							+" WHERE insplan.ClaimFormNum= (SELECT claimform.ClaimFormNum FROM claimform WHERE claimform.UniqueID='OD1')";
						Db.NonQ32(command);
						command="UPDATE insplan SET ClaimFormNum="+POut.PInt(claimFormNum)+" WHERE insplan.ClaimFormNum=0";
						Db.NonQ32(command);*/
					} catch {
						//user will have to do it manually
					}
					//added after r303
					command="INSERT INTO preference VALUES('RegistrationKey','')";
					Db.NonQ32(command);
					//After r304
					command="INSERT INTO preference VALUES('DistributorKey','')";
					Db.NonQ32(command);
					command=@"CREATE TABLE registrationkey(
						RegistrationKeyNum int default '0' NOT NULL,
						PatNum int default '0' NOT NULL,
						RegKey varchar2(4000),
						Note varchar2(4000),
						PRIMARY KEY (RegistrationKeyNum)
						)";
					Db.NonQ32(command);
					//After r306
					command="ALTER TABLE preference MODIFY (ValueString varchar2(4000) default '')";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '4.9.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_9_2();
		}

		///<summary></summary>
		private static void To4_9_2() {
			if(FromVersion<new Version("4.9.2.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.9.2");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurelog DROP INDEX indexADACode";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD INDEX (CodeNum)";
					Db.NonQ32(command);
				} else {//Oracle
					command="CREATE INDEX ind_procedurelog_CodeNum ON procedurelog (CodeNum)";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '4.9.2.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_9_5();
		}

		///<summary></summary>
		private static void To4_9_5() {
			if(FromVersion<new Version("4.9.5.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.9.5");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					//fix labcase.LaboratoryNum orphaned keys.
					//get target labnum
					command="SELECT LaboratoryNum FROM laboratory";
					DataTable table=Db.GetTable(command);
					string labnum=table.Rows[0][0].ToString();//just use the first lab we can find.
					command="SELECT LaboratoryNum FROM labcase WHERE NOT EXISTS (SELECT * FROM laboratory WHERE laboratory.LaboratoryNum=labcase.LaboratoryNum) GROUP BY LaboratoryNum";
					table=Db.GetTable(command);
					for(int i=0;i<table.Rows.Count;i++) {
						command="UPDATE labcase SET LaboratoryNum="+labnum+" WHERE LaboratoryNum="+table.Rows[i][0].ToString();
						Db.NonQ32(command);
					}
				} else {//oracle
					//fix labcase.LaboratoryNum orphaned keys.
					command="UPDATE labcase SET LaboratoryNum=0 WHERE LaboratoryNum NOT IN (SELECT DISTINCT LaboratoryNum FROM laboratory)";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '4.9.5.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_9_7();
		}

		private static void To4_9_7() {
			if(FromVersion<new Version("4.9.7.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.9.7");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					//This step was performed in an earlier conversion, but is required here again, for
					//customers who's data has been converted to using a varchar 4000 here. After this command is
					//run, every MySQL user will be using text in the preference value column.
					command="ALTER TABLE preference CHANGE ValueString ValueString text NOT NULL default ''";
					Db.NonQ32(command);
				} else {
					//Already converted to varchar2 4000 for Oracle. No conversion necessary here.
				}
				command="UPDATE preference SET ValueString = '4.9.7.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To4_9_11();
		}

		private static void To4_9_11() {
			if(FromVersion<new Version("4.9.11.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 4.9.11");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DELETE FROM appointment WHERE AptStatus=6 AND NOT EXISTS(SELECT * FROM patient WHERE patient.NextAptNum=appointment.AptNum)";
					Db.NonQ32(command);
				} else {//oracle
					command="DELETE FROM appointment WHERE AptStatus=6 AND NOT EXISTS(SELECT * FROM patient WHERE patient.NextAptNum=appointment.AptNum)";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '4.9.11.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_0_0();
		}

		///<summary></summary>
		private static void To5_0_0() {
			if(FromVersion<new Version("5.0.0.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.0.0");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					//after r318
					command="DROP TABLE IF EXISTS reqneeded";
					Db.NonQ32(command);
					command=@"CREATE TABLE reqneeded(
						ReqNeededNum int NOT NULL auto_increment,
						Descript varchar(255),
						SchoolCourseNum int NOT NULL,
						SchoolClassNum int NOT NULL,
						PRIMARY KEY (ReqNeededNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					//after r320
					command="ALTER TABLE userod ADD ProvNum int NOT NULL";
					Db.NonQ32(command);
					//after r337,r362,r378
					command="ALTER TABLE mountitem ADD OrdinalPos int NOT NULL default '0'";
					Db.NonQ32(command);
					command="ALTER TABLE mountitem ADD Width int default '0'";
					Db.NonQ32(command);
					command="ALTER TABLE mountitem ADD Height int default '0'";
					Db.NonQ32(command);
					command="ALTER TABLE mount ADD Note text default ''";
					Db.NonQ32(command);
					command="ALTER TABLE mount ADD Width int default '0'";
					Db.NonQ32(command);
					command="ALTER TABLE mount ADD Height int default '0'";
					Db.NonQ32(command);
					//After 342:
					command="UPDATE apptviewitem SET ElementDesc='Procs' WHERE ElementDesc='ProcDescript'";
					Db.NonQ32(command);
					//After 365: These are for new auto recall appointment functions and film/SRP indicators
					command="INSERT INTO preference VALUES('RecallPatternChild','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('RecallProceduresChild','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('RecallPatternPerio','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('RecallProceduresPerio','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('RecallPerioTriggerProcs','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('RecallFMXPanoProc','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('RecallFMXPanoYrInterval','5')";
					Db.NonQ32(command);
					if(PrefC.GetRaw("RecallBW")=="") {
						command="INSERT INTO preference VALUES('RecallDisableAutoFilms','1')";
					} else {
						command="INSERT INTO preference VALUES('RecallDisableAutoFilms','0')";
					}
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('RecallDisablePerioAlt','1')";
					Db.NonQ32(command);
					//After r366
					command="DROP TABLE IF EXISTS reqstudent";
					Db.NonQ32(command);
					command=@"CREATE TABLE reqstudent(
						ReqStudentNum int NOT NULL auto_increment,
						ReqNeededNum int NOT NULL,
						Descript varchar(255),
						SchoolCourseNum int NOT NULL,
						ProvNum int NOT NULL,
						AptNum int NOT NULL,
						PatNum int NOT NULL,
						InstructorNum int NOT NULL,
						DateCompleted date NOT NULL default '0001-01-01',
						PRIMARY KEY (ReqStudentNum),
						INDEX (ReqNeededNum),
						INDEX (ProvNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					//after r381
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor,IsHidden) VALUES(12,18,'Patient Note Text',-8388480,0)";//purple
					Db.NonQ32(command);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor,IsHidden) VALUES(12,19,'Patient Note Background',-1,0)";//white
					Db.NonQ32(command);
					command="ALTER TABLE claim ADD ClaimForm int NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE claim ADD EFormat int NOT NULL";
					Db.NonQ32(command);
					//after r393
					//command="DROP TABLE proclicense";//js-let's keep this around for a while.
					//Db.NonQ32(command);
					//after 401
					//-8355712 drk gray, -16777056 drkblue, -16777216 black, -1051718 lt yellow
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor,IsHidden) VALUES(17,6,"
					+"'Patient Note Text',-16777216,0)";//black
					Db.NonQ32(command);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor,IsHidden) VALUES(17,7,"
					+"'Patient Note Background',-1051718,0)";//post-it note yellow
					Db.NonQ32(command);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor,IsHidden) VALUES(17,8,"
					+"'Patient Note - Pt Name',-16777056,0)";//drk blue
					Db.NonQ32(command);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor,IsHidden) VALUES(17,9,"
					+"'Completed Pt. Note Text',-16777216,0)";//black
					Db.NonQ32(command);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor,IsHidden) VALUES(17,10,"
					+"'Completed Pt. Note Background',-8355712,0)";//drk gray
					Db.NonQ32(command);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor,IsHidden) VALUES(12,20,'Completed Pt Note Text',-8388480,0)";//purple
					Db.NonQ32(command);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor,IsHidden) VALUES(12,21,'Completed Pt Note Background',-1,0)";//white
					Db.NonQ32(command);
					//After r438
					//X-Charge Bridge---------------------------------------------------------------------------
					command=@"INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note
					) VALUES(
					'Xcharge',
					'X-Charge from x-charge.com',
					'0','"
						+POut.String(@"C:\Program Files\X-Charge\XCharge.exe")+"', "
						+"'', "
						+"'"+POut.String(@"This setup is typically performed by right clicking on the X-Charge icon in the payment window.")+"')";
					int programNum=Db.NonQ32(command,true);//we now have a ProgramNum to work with
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+programNum.ToString()+"', "
						+"'PaymentType', "
						+"'0')";
					Db.NonQ32(command);
					//After r442
					command="INSERT INTO preference VALUES('FuchsOptionsOn','0')";
					Db.NonQ32(command);
					//change these from 0 to nothing
					command="UPDATE preference SET ValueString = '' WHERE PrefName = 'StationaryImage'";
					Db.NonQ32(command);
					command=command="UPDATE preference SET ValueString = '' WHERE PrefName = 'StationaryDocument'";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('WordProcessorPath','')";
					Db.NonQ32(command);
					//After r457
					command="ALTER TABLE refattach CHANGE RefAttachNum RefAttachNum int unsigned NOT NULL auto_increment";
					Db.NonQ32(command);
				} else {//oracle
					//after r318
					command=@"CREATE TABLE reqneeded(
						ReqNeededNum int default '0' NOT NULL,
						Descript varchar2(255),
						SchoolCourseNum int default '0' NOT NULL,
						SchoolClassNum int default '0' NOT NULL,
						PRIMARY KEY (ReqNeededNum)
						)";
					Db.NonQ32(command);
					//after r320
					command="ALTER TABLE userod ADD ProvNum int";
					Db.NonQ32(command);
					//after r337,r362,r378
					command="ALTER TABLE mountitem ADD OrdinalPos int default '0'";
					Db.NonQ32(command);
					command="ALTER TABLE mountitem ADD Width int default '0'";
					Db.NonQ32(command);
					command="ALTER TABLE mountitem ADD Height int default '0'";
					Db.NonQ32(command);
					command="ALTER TABLE mount ADD Note varchar2(4000) default ''";
					Db.NonQ32(command);
					command="ALTER TABLE mount ADD Width int default '0'";
					Db.NonQ32(command);
					command="ALTER TABLE mount ADD Height int default '0'";
					Db.NonQ32(command);
					//After 342:
					command="UPDATE apptviewitem SET ElementDesc='Procs' WHERE ElementDesc='ProcDescript'";
					Db.NonQ32(command);
					//After 365: These are for new auto recall appointment functions and film/SRP indicators
					command="INSERT INTO preference VALUES('RecallPatternChild','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('RecallProceduresChild','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('RecallPatternPerio','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('RecallProceduresPerio','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('RecallPerioTriggerProcs','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('RecallFMXPanoProc','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('RecallFMXPanoYrInterval','5')";
					Db.NonQ32(command);
					if(PrefC.GetRaw("RecallBW")=="") {
						command="INSERT INTO preference VALUES('RecallDisableAutoFilms','1')";
					} else {
						command="INSERT INTO preference VALUES('RecallDisableAutoFilms','0')";
					}
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('RecallDisablePerioAlt','1')";
					Db.NonQ32(command);
					//After r366
					command=@"CREATE TABLE reqstudent(
						ReqStudentNum int default '0' NOT NULL,
						ReqNeededNum int default '0' NOT NULL,
						Descript varchar2(255),
						SchoolCourseNum int default '0' NOT NULL,
						ProvNum int default '0' NOT NULL,
						AptNum int default '0' NOT NULL,
						PatNum int default '0' NOT NULL,
						InstructorNum int default '0' NOT NULL,
						DateCompleted date default TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						PRIMARY KEY (ReqStudentNum)
						)";
					Db.NonQ32(command);
					command="CREATE INDEX ind_reqstudent_ReqNeededNum ON reqstudent (ReqNeededNum)";
					Db.NonQ32(command);
					command="CREATE INDEX ind_reqstudent_ProvNum ON reqstudent (ProvNum)";
					Db.NonQ32(command);
					//after r381
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemColor,IsHidden) "
						+"VALUES((SELECT MAX(DefNum)+1 FROM definition),12,18,'Patient Note Text',-8388480,0)";//purple
					Db.NonQ32(command);
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemColor,IsHidden) "
						+"VALUES((SELECT MAX(DefNum)+1 FROM definition),12,19,'Patient Note Background',-1,0)";//white
					Db.NonQ32(command);
					command="ALTER TABLE claim ADD ClaimForm int default '0' NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE claim ADD EFormat int default '0' NOT NULL";
					Db.NonQ32(command);
					//after r393
					//command="DROP TABLE proclicense";//js-let's keep this around for a while.
					//Db.NonQ32(command);
					//after 401
					//-8355712 drk gray, -16777056 drkblue, -16777216 black, -1051718 lt yellow
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemColor,IsHidden) "
						+"VALUES((SELECT MAX(DefNum)+1 FROM definition),17,6,'Patient Note Text',-16777216,0)";//black
					Db.NonQ32(command);
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemColor,IsHidden) "
						+"VALUES((SELECT MAX(DefNum)+1 FROM definition),17,7,'Patient Note Background',-1051718,0)";//post-it note yellow
					Db.NonQ32(command);
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemColor,IsHidden) "
						+"VALUES((SELECT MAX(DefNum)+1 FROM definition),17,8,'Patient Note - Pt Name',-16777056,0)";//drk blue
					Db.NonQ32(command);
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemColor,IsHidden) "
						+"VALUES((SELECT MAX(DefNum)+1 FROM definition),17,9,'Completed Pt. Note Text',-16777216,0)";//black
					Db.NonQ32(command);
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemColor,IsHidden) "
						+"VALUES((SELECT MAX(DefNum)+1 FROM definition),17,10,'Completed Pt. Note Background',-8355712,0)";//drk gray
					Db.NonQ32(command);
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemColor,IsHidden) "
						+"VALUES((SELECT MAX(DefNum)+1 FROM definition),12,20,'Completed Pt Note Text',-8388480,0)";//purple
					Db.NonQ32(command);
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemColor,IsHidden) "
						+"VALUES((SELECT MAX(DefNum)+1 FROM definition),12,21,'Completed Pt Note Background',-1,0)";//white
					Db.NonQ32(command);
					//After r438
					//X-Charge Bridge---------------------------------------------------------------------------
					command=@"INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note
						) VALUES(
						(SELECT MAX(ProgramNum)+1 FROM program),
						'Xcharge',
						'X-Charge from x-charge.com',
						'0',"
						+"'"+POut.String(@"C:\Program Files\X-Charge\XCharge.exe")+"', "
						+"'', "
						+"'This setup is typically performed by right clicking on the X-Charge icon in the payment window.')";
					Db.NonQ32(command);//we now have a ProgramNum to work with
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum)+1 FROM programproperty),"
						+"(SELECT MAX(ProgramNum) FROM program), "
						+"'PaymentType', "
						+"'0')";
					Db.NonQ32(command);
					//After r442
					command="INSERT INTO preference VALUES('FuchsOptionsOn','0')";
					Db.NonQ32(command);
					//change these from 0 to nothing
					command="UPDATE preference SET ValueString = '' WHERE PrefName = 'StationaryImage'";
					Db.NonQ32(command);
					command=command="UPDATE preference SET ValueString = '' WHERE PrefName = 'StationaryDocument'";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('WordProcessorPath','')";
					Db.NonQ32(command);
					//After r457
					command="ALTER TABLE refattach MODIFY (RefAttachNum int)";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.0.0.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_0_2();
		}

		///<summary></summary>
		private static void To5_0_2() {
			if(FromVersion<new Version("5.0.2.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.0.2");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DELETE FROM appointment WHERE AptStatus=6 AND NOT EXISTS(SELECT * FROM patient WHERE patient.NextAptNum=appointment.AptNum)";
					Db.NonQ32(command);
				} else {//oracle
					command="DELETE FROM appointment WHERE AptStatus=6 AND NOT EXISTS(SELECT * FROM patient WHERE patient.NextAptNum=appointment.AptNum)";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.0.2.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_0_4();
		}

		///<summary></summary>
		private static void To5_0_4() {
			if(FromVersion<new Version("5.0.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.0.4");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE computerpref ADD SensorType varchar(255) default 'D'";
					Db.NonQ32(command);
					command="ALTER TABLE computerpref ADD SensorBinned varchar(1) default '0'";
					Db.NonQ32(command);
					command="ALTER TABLE computerpref ADD SensorPort int default '0'";
					Db.NonQ32(command);
					command="ALTER TABLE computerpref ADD SensorExposure int default '1'";
					Db.NonQ32(command);
				} else {//oracle.
					command="ALTER TABLE computerpref ADD SensorType varchar2(255)";
					Db.NonQ32(command);
					command="ALTER TABLE computerpref ADD SensorBinned varchar2(1) default '0'";
					Db.NonQ32(command);
					command="ALTER TABLE computerpref ADD SensorPort int default '0'";
					Db.NonQ32(command);
					command="ALTER TABLE computerpref ADD SensorExposure int default '1'";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.0.4.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_0_5();
		}

		///<summary>Just ensures that the SensorType column in the computerpref table has length of 255 (since older code used a length of 256, which is invalid in MySQL version 4.1).</summary>
		private static void To5_0_5() {
			if(FromVersion<new Version("5.0.5.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.0.5");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE computerpref CHANGE SensorType SensorType varchar(255) default 'D'";
					Db.NonQ32(command);
				} else {//oracle.
					command="ALTER TABLE computerpref MODIFY (SensorType varchar2(255) default 'D')";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.0.5.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_0_6();
		}

		private static void To5_0_6() {
			if(FromVersion<new Version("5.0.6.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.0.6");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					//Adjust the ada 2006 total fee location to fit be in box #33.
					command="UPDATE claimformitem SET  YPos='694' WHERE  FieldName='TotalFee' AND ClaimFormNum=(SELECT ClaimFormNum FROM claimform WHERE Description LIKE '%ADA 2006%')";
					Db.NonQ32(command);
				} else {//oracle
					//Adjust the ada 2006 total fee location to fit be in box #33.
					command="UPDATE claimformitem SET  YPos='694' WHERE  FieldName='TotalFee' AND ClaimFormNum=(SELECT ClaimFormNum FROM claimform WHERE Description LIKE '%ADA 2006%')";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.0.6.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_0_7();
		}

		private static void To5_0_7() {
			if(FromVersion<new Version("5.0.7.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.0.7");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="UPDATE clearinghouse SET ExportPath = '"+POut.String(@"C:\TesiaLink\OUT\")+"' WHERE ISA08='113504607'";
					Db.NonQ32(command);
					command="UPDATE clearinghouse SET ResponsePath = '"+POut.String(@"C:\TesiaLink\IN\")+"' WHERE ISA08='113504607'";
					Db.NonQ32(command);
					command="UPDATE clearinghouse SET ClientProgram = '"+POut.String(@"C:\Program Files\TesiaLink\TesiaLink.exe")+"' WHERE ISA08='113504607'";
					Db.NonQ32(command);
				} else {//oracle
					command="UPDATE clearinghouse SET ExportPath = '"+POut.String(@"C:\TesiaLink\OUT\")+"' WHERE ISA08='113504607'";
					Db.NonQ32(command);
					command="UPDATE clearinghouse SET ResponsePath = '"+POut.String(@"C:\TesiaLink\IN\")+"' WHERE ISA08='113504607'";
					Db.NonQ32(command);
					command="UPDATE clearinghouse SET ClientProgram = '"+POut.String(@"C:\Program Files\TesiaLink\TesiaLink.exe")+"' WHERE ISA08='113504607'";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.0.7.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_0_10();
		}

		private static void To5_0_10() {
			if(FromVersion<new Version("5.0.10.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.0.10");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="UPDATE preference SET ValueString = '0' WHERE PrefName = 'ShowProgressNotesInsteadofCommLog'";
					Db.NonQ32(command);
				} else {//oracle
					command="UPDATE preference SET ValueString = '0' WHERE PrefName = 'ShowProgressNotesInsteadofCommLog'";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.0.10.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_0_11();
		}

		private static void To5_0_11() {
			if(FromVersion<new Version("5.0.11.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.0.11");//No translation in convert script.
				string command;
				//We can't actually run the reset tool here for two reasons:
				//1. We can never use objects in conversions due to differences in object versions.
				//2. We don't know at which version number users will switch from trial version.
				//So the reset tool will always be run on startup.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT preference VALUES('ADAdescriptionsReset','0')";
					Db.NonQ32(command);
				} else {//oracle
					command="INSERT INTO preference VALUES('ADAdescriptionsReset','0')";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.0.11.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_1_1();
		}

		private static void To5_1_1() {
			if(FromVersion<new Version("5.1.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.1.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS claimvalcodelog";
					Db.NonQ32(command);
					command=@"CREATE TABLE claimvalcodelog( 
					ClaimValCodeLogNum int unsigned NOT NULL auto_increment, 
					ClaimNum int unsigned NOT NULL, 
					ClaimField varchar(5) NOT NULL, 
					ValCode char(2) NOT NULL, 
          ValAmount varchar(10) NOT NULL, 
					Ordinal int unsigned NOT NULL, 
					PRIMARY KEY (ClaimValCodeLogNum) 
					) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command="ALTER TABLE referral ADD NationalProvID varchar(255)";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS claimcondcodelog";
					Db.NonQ32(command);
					command=@"CREATE TABLE claimcondcodelog (
					ClaimCondCodeLogNum int unsigned NOT NULL auto_increment,
					ClaimNum int unsigned NOT NULL,
					Code0 varchar(2),
					Code1 varchar(2),
					Code2 varchar(2),
					Code3 varchar(2),
					Code4 varchar(2),
					Code5 varchar(2),
					Code6 varchar(2),
					Code7 varchar(2),
					Code8 varchar(2),
					Code9 varchar(2),
					Code10 varchar(2),
					PRIMARY KEY (ClaimCondCodeLogNum)
					) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					//after r566
					command="INSERT INTO preference VALUES('ApptBubbleDelay','0')";
					Db.NonQ32(command);
					//after r567
					//Change Tesia clearinghouse settings to use internal functions instead of external program for uploading.
					//do this later:
					//command="UPDATE clearinghouse SET ClientProgram = '' WHERE ISA08='113504607'";
					//Db.NonQ32(command);
					//command="UPDATE clearinghouse SET CommBridge = '10' WHERE ISA08='113504607'";
					//Db.NonQ32(command);
					//after r627  No longer use TesiaLink folder.
					//do this later:
					//command="UPDATE clearinghouse SET ExportPath = '"+POut.PString(@"C:\Tesia\OUT\")+"' WHERE ISA08='113504607'";
					//Db.NonQ32(command);
					//command="UPDATE clearinghouse SET ResponsePath = '"+POut.PString(@"C:\Tesia\IN\")+"' WHERE ISA08='113504607'";
					//Db.NonQ32(command);
					//after r630. Altered again after r659
					command="DROP TABLE IF EXISTS autonote";
					Db.NonQ32(command);
					command=@"CREATE TABLE autonote (
					AutoNoteNum int unsigned NOT NULL auto_increment,
					AutoNoteName varchar(50),
					ControlsToInc text,
					PRIMARY KEY (AutoNoteNum)
					) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS autonotecontrol";
					Db.NonQ32(command);
					command=@"CREATE TABLE autonotecontrol (
					AutoNoteControlNum int unsigned NOT NULL auto_increment,
					Descript varchar(50),
					ControlType varchar(50),
					ControlLabel varchar(50),
					PrefaceText text,
					MultiLineText text,
					ControlOptions text,
					PRIMARY KEY (AutoNoteControlNum)
					) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					// After r665
					command=@"ALTER TABLE mount MODIFY DocCategory INTEGER NOT NULL";
					Db.NonQ32(command);
					// After r672
					command=@"ALTER TABLE document MODIFY DocCategory INTEGER NOT NULL";
					Db.NonQ32(command);
					// After r673
					command=@"ALTER TABLE document MODIFY DegreesRotated SMALLINT NOT NULL";
					Db.NonQ32(command);
					//after r678
					command="ALTER TABLE etrans ADD BatchNumber INT NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE etrans ADD AckCode varchar(255)";
					Db.NonQ32(command);
				} else {//oracle
					try {
						command="SELECT COUNT(*) FROM claimvalcodelog";
						Db.GetTable(command);
						//The table exists at this point.
						command="DROP TABLE claimvalcodelog PURGE";
						Db.NonQ32(command);
					} catch {//The table does not exist
					}
					command=@"CREATE TABLE claimvalcodelog( 
						ClaimValCodeLogNum int default '0' NOT NULL, 
						ClaimNum int default '0' NOT NULL, 
						ClaimField varchar2(5) default '0' NOT NULL, 
						ValCode varchar2(2) default '0' NOT NULL, 
						ValAmount varchar2(10) default '0' NOT NULL, 
						Ordinal int default '0' NOT NULL, 
						PRIMARY KEY (ClaimValCodeLogNum) 
					)";
					Db.NonQ32(command);
					command="ALTER TABLE referral ADD NationalProvID varchar2(255)";
					Db.NonQ32(command);
					try {
						command="SELECT COUNT(*) FROM claimcondcodelog";
						Db.GetTable(command);
						//The table exists at this point.
						command="DROP TABLE claimcondcodelog PURGE";
						Db.NonQ32(command);
					} catch {//The table does not exist.
					}
					command=@"CREATE TABLE claimcondcodelog (
						ClaimCondCodeLogNum int default '0' NOT NULL,
						ClaimNum int default '0' NOT NULL,
						Code0 varchar2(2),
						Code1 varchar2(2),
						Code2 varchar2(2),
						Code3 varchar2(2),
						Code4 varchar2(2),
						Code5 varchar2(2),
						Code6 varchar2(2),
						Code7 varchar2(2),
						Code8 varchar2(2),
						Code9 varchar2(2),
						Code10 varchar2(2),
						PRIMARY KEY (ClaimCondCodeLogNum)
					)";
					Db.NonQ32(command);
					//after r566
					command="INSERT INTO preference VALUES('ApptBubbleDelay','0')";
					Db.NonQ32(command);
					//after r630. Altered again after r659
					try {
						command="SELECT COUNT(*) FROM autonote";
						Db.GetTable(command);
						//The table exists at this point
						command="DROP TABLE autonote PURGE";
						Db.NonQ32(command);
					} catch {//The table does not exist.
					}
					command=@"CREATE TABLE autonote (
						AutoNoteNum int default '0' NOT NULL,
						AutoNoteName varchar2(50),
						ControlsToInc varchar2(4000),
						PRIMARY KEY (AutoNoteNum)
					)";
					Db.NonQ32(command);
					try {
						command="SELECT COUNT(*) FROM autonotecontrol";
						Db.GetTable(command);
						//The table exists at this point.
						command="DROP TABLE autonotecontrol PURGE";
						Db.NonQ32(command);
					} catch {//The table does not exist.
					}
					command=@"CREATE TABLE autonotecontrol (
						AutoNoteControlNum int default '0' NOT NULL,
						Descript varchar2(50),
						ControlType varchar2(50),
						ControlLabel varchar2(50),
						PrefaceText varchar2(4000),
						MultiLineText varchar2(4000),
						ControlOptions varchar2(4000),
						PRIMARY KEY (AutoNoteControlNum)
					)";
					Db.NonQ32(command);
					// After r665
					command=@"ALTER TABLE mount MODIFY DocCategory INTEGER";//already not null
					Db.NonQ32(command);
					// After r672
					command=@"ALTER TABLE document MODIFY DocCategory INTEGER";//already not null
					Db.NonQ32(command);
					// After r673
					command=@"ALTER TABLE document MODIFY DegreesRotated INT";//already not null
					Db.NonQ32(command);
					//after r678
					command="ALTER TABLE etrans ADD BatchNumber INT default '0' NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE etrans ADD AckCode varchar2(255)";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.1.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_1_3();
		}

		private static void To5_1_3() {
			if(FromVersion<new Version("5.1.3.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.1.3");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE computerpref ADD GraphicsDoubleBuffering varchar(1) default '0'";
					Db.NonQ32(command);
					command="ALTER TABLE computerpref ADD PreferredPixelFormatNum int default '0'";
					Db.NonQ32(command);
				} else {//oracle.
					command="ALTER TABLE computerpref ADD GraphicsDoubleBuffering varchar2(1) default '0'";
					Db.NonQ32(command);
					command="ALTER TABLE computerpref ADD PreferredPixelFormatNum int default '0'";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.1.3.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_1_4();
		}

		private static void To5_1_4() {
			if(FromVersion<new Version("5.1.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.1.4");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE etrans ADD TransSetNum INT NOT NULL";
					Db.NonQ32(command);
				} else {//oracle
					command="ALTER TABLE etrans ADD TransSetNum INT default '0' NOT NULL";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.1.4.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_1_5();
		}

		private static void To5_1_5() {
			if(FromVersion<new Version("5.1.5.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.1.5");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE etrans ADD Note text";
					Db.NonQ32(command);
				} else {//oracle
					command="ALTER TABLE etrans ADD Note varchar(4000)";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.1.5.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_1_11();
		}

		private static void To5_1_11() {
			if(FromVersion<new Version("5.1.11.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.1.11");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="UPDATE computerpref SET PreferredPixelFormatNum='0'";
					Db.NonQ32(command);
				} else {//oracle
					command="UPDATE computerpref SET PreferredPixelFormatNum='0'";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.1.11.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_1_12();
		}

		private static void To5_1_12() {
			if(FromVersion<new Version("5.1.12.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.1.12");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="UPDATE computerpref SET PreferredPixelFormatNum='0'";//again
					Db.NonQ32(command);
					command="UPDATE computerpref SET GraphicsUseHardware='1'";
					Db.NonQ32(command);
					command="UPDATE computerpref SET GraphicsDoubleBuffering='1'";
					Db.NonQ32(command);
					command="UPDATE computerpref SET GraphicsSimple='0'";
					Db.NonQ32(command);
				} else {//oracle
					command="UPDATE computerpref SET PreferredPixelFormatNum='0'";//again
					Db.NonQ32(command);
					command="UPDATE computerpref SET GraphicsUseHardware='1'";
					Db.NonQ32(command);
					command="UPDATE computerpref SET GraphicsDoubleBuffering='1'";
					Db.NonQ32(command);
					command="UPDATE computerpref SET GraphicsSimple='0'";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.1.12.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_1_18();
		}

		private static void To5_1_18() {
			if(FromVersion<new Version("5.1.18.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.1.18");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="UPDATE computerpref SET GraphicsDoubleBuffering=0, PreferredPixelFormatNum=0";//again
					Db.NonQ32(command);
				} else {//oracle
					command="UPDATE computerpref SET GraphicsDoubleBuffering=0, PreferredPixelFormatNum=0";//again
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.1.18.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_1_19();
		}

		private static void To5_1_19() {
			if(FromVersion<new Version("5.1.19.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.1.19");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="UPDATE computerpref SET GraphicsDoubleBuffering=0, PreferredPixelFormatNum=0";//again (last time, hopefully)
					Db.NonQ32(command);
				} else {//oracle
					command="UPDATE computerpref SET GraphicsDoubleBuffering=0, PreferredPixelFormatNum=0";//again (last time, hopefully)
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.1.19.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_2_0();
		}

		private static void To5_2_0() {
			if(FromVersion<new Version("5.2.0.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.2.0");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference VALUES('UseBillingAddressOnClaims','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('PracticeBillingAddress','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('PracticeBillingAddress2','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('PracticeBillingCity','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('PracticeBillingST','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('PracticeBillingZip','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('TrojanExpressCollectPath','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('TrojanExpressCollectPassword','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('TrojanExpressCollectBillingType','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('TrojanExpressCollectPreviousFileNumber','0')";
					Db.NonQ32(command);
					command="ALTER TABLE userod ADD IsHidden BOOL NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE patientnote ADD CCNumber VARCHAR(255)";
					Db.NonQ32(command);
					command="ALTER TABLE patientnote ADD CCExpiration date NOT NULL default '0001-01-01'";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('StoreCCnumbers','0')";
					Db.NonQ32(command);
					command="ALTER TABLE payplancharge ADD ProvNum int NOT NULL";
					Db.NonQ32(command);
					command="UPDATE payplancharge SET ProvNum=(SELECT PriProv FROM patient WHERE patient.PatNum=payplancharge.PatNum)";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('AppointmentBubblesDisabled','0')";
					Db.NonQ32(command);
				} else {//oracle
					command="INSERT INTO preference VALUES('UseBillingAddressOnClaims','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('PracticeBillingAddress','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('PracticeBillingAddress2','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('PracticeBillingCity','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('PracticeBillingST','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('PracticeBillingZip','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('TrojanExpressCollectPath','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('TrojanExpressCollectPassword','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('TrojanExpressCollectBillingType','')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('TrojanExpressCollectPreviousFileNumber','0')";
					Db.NonQ32(command);
					command="ALTER TABLE userod ADD IsHidden varchar(2) default '0' NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE patientnote ADD CCNumber VARCHAR2(255)";
					Db.NonQ32(command);
					command="ALTER TABLE patientnote ADD CCExpiration date default TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('StoreCCnumbers','0')";
					Db.NonQ32(command);
					command="ALTER TABLE payplancharge ADD ProvNum int default '0' NOT NULL";
					Db.NonQ32(command);
					command="UPDATE payplancharge SET ProvNum=(SELECT PriProv FROM patient WHERE patient.PatNum=payplancharge.PatNum)";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('AppointmentBubblesDisabled','0')";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.2.0.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_3_0();
		}

		private static void To5_3_0() {
			if(FromVersion<new Version("5.3.0.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.3.0");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurecode ADD BaseUnits int NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE insplan ADD ShowBaseUnits TINYINT(1) NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE proctp ADD Discount double NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE claimproc CHANGE AllowedAmt AllowedOverride double NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE refattach ADD RefToStatus TINYINT unsigned NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE refattach ADD Note text";
					Db.NonQ32(command);
					command="ALTER TABLE procedurecode ADD SubstitutionCode VARCHAR(25)";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS proccodenote";
					Db.NonQ32(command);
					command=@"CREATE TABLE proccodenote (
						ProcCodeNoteNum int NOT NULL auto_increment,
						CodeNum int NOT NULL,
						ProvNum int NOT NULL,
						Note text,
						ProcTime varchar(255),				
						PRIMARY KEY (ProcCodeNoteNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command="ALTER TABLE insplan ADD DedBeforePerc TINYINT(1) NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD BaseUnits int NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog MODIFY UnitQty int NOT NULL";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('DeductibleBeforePercentAsDefault','0')";
					Db.NonQ32(command);
					command="ALTER TABLE benefit ADD CoverageLevel int NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE procedurecode ADD SubstOnlyIf int NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD StartTime int NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD StopTime int NOT NULL";
					Db.NonQ32(command);
					command="SELECT COUNT(*) FROM procedurecode WHERE ProcCode='D2391'";
					if(Db.GetCount(command)=="1") {
						command="UPDATE procedurecode SET SubstitutionCode='D2140',SubstOnlyIf=1 WHERE ProcCode='D2391'";//1 surf
						Db.NonQ32(command);
					}
					command="SELECT COUNT(*) FROM procedurecode WHERE ProcCode='D2392'";
					if(Db.GetCount(command)=="1") {
						command="UPDATE procedurecode SET SubstitutionCode='D2150',SubstOnlyIf=1 WHERE ProcCode='D2392'";//2 surf
						Db.NonQ32(command);
					}
					command="SELECT COUNT(*) FROM procedurecode WHERE ProcCode='D2393'";
					if(Db.GetCount(command)=="1") {
						command="UPDATE procedurecode SET SubstitutionCode='D2160',SubstOnlyIf=1 WHERE ProcCode='D2393'";//3 surf
						Db.NonQ32(command);
					}
					command="SELECT COUNT(*) FROM procedurecode WHERE ProcCode='D2394'";
					if(Db.GetCount(command)=="1") {
						command="UPDATE procedurecode SET SubstitutionCode='D2161',SubstOnlyIf=1 WHERE ProcCode='D2394'";//4+ surf
						Db.NonQ32(command);
					}
					command="INSERT INTO preference VALUES('TaskListAlwaysShowsAtBottom','0')";
					Db.NonQ32(command);
					command="ALTER TABLE commlog ADD IsStatementSent tinyint(1) NOT NULL";
					Db.NonQ32(command);
					command="UPDATE commlog SET IsStatementSent=1 WHERE CommType=1";
					Db.NonQ32(command);
					command="ALTER TABLE commlog CHANGE CommType CommType int NOT NULL";
					Db.NonQ32(command);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES(27,0,'ApptRelated','APPT')";
					int defNum=Db.NonQ32(command,true);
					command="UPDATE commlog SET CommType="+POut.Long(defNum)+" WHERE CommType=2";
					Db.NonQ32(command);
					//-----------------
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES(27,1,'Insurance','')";
					defNum=Db.NonQ32(command,true);
					command="UPDATE commlog SET CommType="+POut.Long(defNum)+" WHERE CommType=3";
					Db.NonQ32(command);
					//-----------------
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES(27,2,'Financial','FIN')";
					defNum=Db.NonQ32(command,true);
					command="UPDATE commlog SET CommType="+POut.Long(defNum)+" WHERE CommType=4";
					Db.NonQ32(command);
					//-----------------
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES(27,3,'Recall','RECALL')";
					defNum=Db.NonQ32(command,true);
					command="UPDATE commlog SET CommType="+POut.Long(defNum)+" WHERE CommType=5";
					Db.NonQ32(command);
					//-----------------
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES(27,4,'Misc','MISC')";
					defNum=Db.NonQ32(command,true);
					command="UPDATE commlog SET CommType="+POut.Long(defNum)+" WHERE CommType=6";
					Db.NonQ32(command);
					command="UPDATE commlog SET CommType=0 WHERE CommType=1";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS displayfield";
					Db.NonQ32(command);
					command=@"CREATE TABLE displayfield (
						DisplayFieldNum int NOT NULL auto_increment,
						InternalName varchar(255),
						ItemOrder int NOT NULL,
						Description varchar(255),
						ColumnWidth int NOT NULL,		
						PRIMARY KEY (DisplayFieldNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
				} else {//oracle
					command="ALTER TABLE procedurecode ADD BaseUnits int default '0' NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE insplan ADD ShowBaseUnits INT default '0' NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE proctp ADD Discount BINARY_DOUBLE default '0' NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE claimproc MODIFY (AllowedAmt BINARY_DOUBLE default '0')";//already not null
					Db.NonQ32(command);
					command="ALTER TABLE claimproc RENAME COLUMN AllowedAmt TO AllowedOverride";
					Db.NonQ32(command);
					command="ALTER TABLE refattach ADD RefToStatus INT default '0' NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE refattach ADD Note varchar2(4000)";
					Db.NonQ32(command);
					command="ALTER TABLE procedurecode ADD SubstitutionCode VARCHAR2(25)";
					Db.NonQ32(command);
					try {
						command="SELECT COUNT(*) FROM proccodenote";
						Db.GetTable(command);
						//The table exists at this point.
						command="DROP TABLE proccodenote PURGE";
						Db.NonQ32(command);
					} catch {//Table doesn't exist
					}
					command=@"CREATE TABLE proccodenote (
							ProcCodeNoteNum int default '0' NOT NULL,
							CodeNum int default '0' NOT NULL,
							ProvNum int default '0' NOT NULL,
							Note varchar(4000),
							ProcTime varchar(255),				
							PRIMARY KEY (ProcCodeNoteNum)
					)";
					Db.NonQ32(command);
					command="ALTER TABLE insplan ADD DedBeforePerc INT default '0' NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD BaseUnits int default '0' NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog MODIFY UnitQty int default '0' NOT NULL";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('DeductibleBeforePercentAsDefault','0')";
					Db.NonQ32(command);
					command="ALTER TABLE benefit ADD CoverageLevel int default '0' NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE procedurecode ADD SubstOnlyIf int default '0' NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD StartTime int default '0' NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD StopTime int default '0' NOT NULL";
					Db.NonQ32(command);
					//Not necessary to first check if this code exists. This command will always work, even if the code isn't there.
					command="UPDATE procedurecode SET SubstitutionCode='D2140',SubstOnlyIf=1 WHERE ProcCode='D2391'";//1 surf
					Db.NonQ32(command);
					//Not necessary to first check if this code exists. This command will always work, even if the code isn't there.
					command="UPDATE procedurecode SET SubstitutionCode='D2150',SubstOnlyIf=1 WHERE ProcCode='D2392'";//2 surf
					Db.NonQ32(command);
					//Not necessary to first check if this code exists. This command will always work, even if the code isn't there.
					command="UPDATE procedurecode SET SubstitutionCode='D2160',SubstOnlyIf=1 WHERE ProcCode='D2393'";//3 surf
					Db.NonQ32(command);
					//Not necessary to first check if this code exists. This command will always work, even if the code isn't there.
					command="UPDATE procedurecode SET SubstitutionCode='D2161',SubstOnlyIf=1 WHERE ProcCode='D2394'";//4+ surf
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('TaskListAlwaysShowsAtBottom','0')";
					Db.NonQ32(command);
					command="ALTER TABLE commlog ADD IsStatementSent int default '0' NOT NULL";
					Db.NonQ32(command);
					command="UPDATE commlog SET IsStatementSent=1 WHERE CommType=1";
					Db.NonQ32(command);
					command="ALTER TABLE commlog MODIFY(CommType int default '0')";//already not null
					Db.NonQ32(command);
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue) "+
						"VALUES((SELECT MAX(DefNum)+1 FROM definition),27,0,'ApptRelated','APPT')";
					Db.NonQ32(command);
					command="UPDATE commlog SET CommType=(SELECT MAX(DefNum) FROM definition) WHERE CommType=2";
					Db.NonQ32(command);
					//-----------------
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue) "+
						"VALUES((SELECT MAX(DefNum)+1 FROM definition),27,1,'Insurance','')";
					Db.NonQ32(command);
					command="UPDATE commlog SET CommType=(SELECT MAX(DefNum) FROM definition) WHERE CommType=3";
					Db.NonQ32(command);
					//-----------------
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue) "+
						"VALUES((SELECT MAX(DefNum)+1 FROM definition),27,2,'Financial','FIN')";
					Db.NonQ32(command);
					command="UPDATE commlog SET CommType=(SELECT MAX(DefNum) FROM definition) WHERE CommType=4";
					Db.NonQ32(command);
					//-----------------
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue) "+
						"VALUES((SELECT MAX(DefNum)+1 FROM definition),27,3,'Recall','RECALL')";
					Db.NonQ32(command);
					command="UPDATE commlog SET CommType=(SELECT MAX(DefNum) FROM definition) WHERE CommType=5";
					Db.NonQ32(command);
					//-----------------
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue) "+
						"VALUES((SELECT MAX(DefNum)+1 FROM definition),27,4,'Misc','MISC')";
					Db.NonQ32(command);
					command="UPDATE commlog SET CommType=(SELECT MAX(DefNum) FROM definition) WHERE CommType=6";
					Db.NonQ32(command);
					command="UPDATE commlog SET CommType=0 WHERE CommType=1";
					Db.NonQ32(command);
					try {
						command="SELECT COUNT(*) FROM displayfield";
						Db.GetTable(command);
						//Table exists at this point.
						command="DROP TABLE displayfield PURGE";
						Db.NonQ32(command);
					} catch {//Table doesn't exist.
					}
					command=@"CREATE TABLE displayfield (
							DisplayFieldNum int default '0' NOT NULL,
							InternalName varchar2(255),
							ItemOrder int default '0' NOT NULL,
							Description varchar2(255),
							ColumnWidth int default '0' NOT NULL,		
							PRIMARY KEY (DisplayFieldNum)
						)";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.3.0.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_3_1();
		}

		private static void To5_3_1() {
			if(FromVersion<new Version("5.3.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.3.1");//No translation in convert script.
				string command;
				command="UPDATE preference SET ValueString = '5.3.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_3_9();
		}

		private static void To5_3_9() {
			if(FromVersion<new Version("5.3.9.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.3.9");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command=@"SELECT AptNum FROM appointment WHERE Pattern=''";
					DataTable table=Db.GetTable(command);
					for(int i=0;i<table.Rows.Count;i++) {
						//detach all procedures
						command="UPDATE procedurelog SET AptNum=0 WHERE AptNum="+table.Rows[i][0].ToString();
						Db.NonQ32(command);
						command="UPDATE procedurelog SET PlannedAptNum=0 WHERE PlannedAptNum="+table.Rows[i][0].ToString();
						Db.NonQ32(command);
						command="DELETE FROM appointment WHERE AptNum="+table.Rows[i][0].ToString();
						Db.NonQ32(command);
					}
				} else {//oracle
					command="UPDATE procedurelog SET AptNum=0 WHERE AptNum IN (SELECT AptNum FROM appointment WHERE Pattern='')";
					Db.NonQ32(command);
					command="UPDATE procedurelog SET PlannedAptNum=0 WHERE PlannedAptNum IN "+
						"(SELECT PlannedAptNum FROM appointment WHERE Pattern='')";
					Db.NonQ32(command);
					command="DELETE FROM appointment WHERE Pattern=''";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.3.9.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_3_11();
		}

		private static void To5_3_11() {
			if(FromVersion<new Version("5.3.11.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.3.11");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE appointment ADD INDEX indexProvNum (ProvNum)";
					try {
						Db.NonQ32(command);
					} catch {
					}
					command="ALTER TABLE appointment ADD INDEX indexProvHyg (ProvHyg)";
					try {
						Db.NonQ32(command);
					} catch {
					}
					command="ALTER TABLE labcase ADD INDEX indexAptNum (AptNum)";
					try {
						Db.NonQ32(command);
					} catch {
					}
					command="ALTER TABLE appointment ADD INDEX indexAptDateTime (AptDateTime)";
					try {
						Db.NonQ32(command);
					} catch {
					}
				} else {//oracle
					command="CREATE INDEX APPOINTMENT_PROVNUM ON appointment (ProvNum)";
					try {
						Db.NonQ32(command);
					} catch {
					}
					command="CREATE INDEX APPOINTMENT_PROVHYG ON appointment (ProvHyg)";
					try {
						Db.NonQ32(command);
					} catch {
					}
					command="CREATE INDEX LABCASE_APTNUM ON labcase (AptNum)";
					try {
						Db.NonQ32(command);
					} catch {
					}
					command="CREATE INDEX APPOINTMENT_APTDATETIME ON appointment (AptDateTime)";
					try {
						Db.NonQ32(command);
					} catch {
					}
				}
				command="UPDATE preference SET ValueString = '5.3.11.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_4_0();
		}

		private static void To5_4_0() {
			if(FromVersion<new Version("5.4.0.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.4.0");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS files";
					Db.NonQ32(command);
					command=@"CREATE TABLE files
						(
							DocNum    int        NOT NULL,
							Data      longblob   NOT NULL,
							Thumbnail longblob   NULL,
							PRIMARY KEY(DocNum)
						)";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('ImageStore', 'OpenDental.Imaging.FileStore')";
					Db.NonQ32(command);
					//PT Dental Bridge---------------------------------------------------------------------------
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'PT', "
						+"'PT Dental from gopaperlessnow.com', "
						+"'0', "
						+"'', "
						+"'', "
						+"'"+POut.String(@"No path is needed.  It is hard coded.")+"')";
					int programNum=Db.NonQ32(command,true);//we now have a ProgramNum to work with
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"'"+POut.Long(programNum)+"', "
						+"'"+POut.Long((int)ToolBarsAvail.FamilyModule)+"', "
						+"'PT Dental')";
					Db.NonQ32(command);
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'PTupdate', "
						+"'PT Dental update (2nd button)', "
						+"'0', "
						+"'', "
						+"'', "
						+"'"+POut.String(@"This is the second part of the PT Dental bridge.  It allows the 'update' button to be customized separately.")+"')";
					programNum=Db.NonQ32(command,true);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"'"+POut.Long(programNum)+"', "
						+"'"+POut.Long((int)ToolBarsAvail.FamilyModule)+"', "
						+"'PT Update')";
					Db.NonQ32(command);
					command="ALTER TABLE patient ADD Title VARCHAR(15)";
					Db.NonQ32(command);
					command="DELETE FROM reqstudent WHERE DateCompleted < '1900-01-01' AND AptNum=0";
					Db.NonQ32(command);
					command="ALTER TABLE provider CHANGE Abbr Abbr VARCHAR(255)";
					Db.NonQ32(command);
					command="ALTER TABLE commlog ADD UserNum INT NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE computerpref ADD AtoZpath VARCHAR(255)";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('BrokenAppointmentAdjustmentType','0')";
					Db.NonQ32(command);
					command=@"ALTER TABLE patient MODIFY PriProv INTEGER NOT NULL";
					Db.NonQ32(command);
					command=@"ALTER TABLE patient MODIFY SecProv INTEGER NOT NULL";
					Db.NonQ32(command);
					command=@"ALTER TABLE patient MODIFY FeeSched INTEGER NOT NULL";
					Db.NonQ32(command);
					command=@"ALTER TABLE patient MODIFY BillingType INTEGER NOT NULL";
					Db.NonQ32(command);
					command=@"ALTER TABLE patient MODIFY ClinicNum INTEGER NOT NULL";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS popup";
					Db.NonQ32(command);
					command=@"CREATE TABLE popup(
						PopupNum int NOT NULL auto_increment,
						PatNum int NOT NULL,
						Description text,
						IsDisabled tinyint(1) NOT NULL,		
						PRIMARY KEY (PopupNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command="ALTER TABLE registrationkey ADD DateStarted DATE NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE registrationkey ADD DateDisabled DATE NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE registrationkey ADD DateEnded DATE NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE registrationkey ADD IsForeign BOOL NOT NULL";
					Db.NonQ32(command);
					command="UPDATE registrationkey SET DateStarted=NOW()";
					Db.NonQ32(command);
					command="UPDATE registrationkey SET DateEnded='0001-01-01'";
					Db.NonQ32(command);
					command="UPDATE registrationkey SET DateDisabled='0001-01-01'";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('UpdateWindowShowsClassicView','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('BillingExcludeInsPending','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('UpdateServerAddress','')";
					Db.NonQ32(command);
					command="UPDATE preference SET PrefName='UpdateCode' WHERE PrefName='RegistrationNumber'";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD DateTP DATE NOT NULL";
					Db.NonQ32(command);
					command="UPDATE procedurelog SET DateTP=ProcDate";
					Db.NonQ32(command);
				} else {//oracle
					try {
						command="SELECT COUNT(*) FROM files";
						Db.GetTable(command);
						//Table exists at this point.
						command="DROP TABLE files PURGE";
						Db.NonQ32(command);
					} catch {//The table does not exist.
					}
					command=@"CREATE TABLE files(
							DocNum int default '0' NOT NULL,
							Data blob NOT NULL,
							Thumbnail blob NULL,
							PRIMARY KEY(DocNum)
						)";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('ImageStore', 'OpenDental.Imaging.FileStore')";
					Db.NonQ32(command);
					//PT Dental Bridge---------------------------------------------------------------------------
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"(SELECT MAX(ProgramNum)+1 FROM program),"
						+"'PT', "
						+"'PT Dental from gopaperlessnow.com', "
						+"'0', "
						+"'', "
						+"'', "
						+"'No path is needed.  It is hard coded.')";
					Db.NonQ32(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
						+"(SELECT MAX(ProgramNum) FROM program), "
						+"'"+POut.Long((int)ToolBarsAvail.FamilyModule)+"', "
						+"'PT Dental')";
					Db.NonQ32(command);
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"(SELECT MAX(ProgramNum)+1 FROM program),"
						+"'PTupdate', "
						+"'PT Dental update (2nd button)', "
						+"'0', "
						+"'', "
						+"'', "
						+"'This is the second part of the PT Dental bridge.  It allows the ''update'' button to be customized separately.')";
					Db.NonQ32(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
						+"(SELECT MAX(ProgramNum) FROM program), "
						+"'"+POut.Long((int)ToolBarsAvail.FamilyModule)+"', "
						+"'PT Update')";
					Db.NonQ32(command);
					command="ALTER TABLE patient ADD Title VARCHAR2(15)";
					Db.NonQ32(command);
					command="DELETE FROM reqstudent WHERE DateCompleted < '1900-01-01' AND AptNum=0";
					Db.NonQ32(command);
					command="ALTER TABLE provider MODIFY Abbr VARCHAR2(255)";
					Db.NonQ32(command);
					command="ALTER TABLE commlog ADD UserNum INT default '0' NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE computerpref ADD AtoZpath VARCHAR2(255)";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('BrokenAppointmentAdjustmentType','0')";
					Db.NonQ32(command);
					command="ALTER TABLE patient MODIFY PriProv INTEGER default '0'";//already not null
					Db.NonQ32(command);
					command="ALTER TABLE patient MODIFY SecProv INTEGER default '0'";//already not null
					Db.NonQ32(command);
					command="ALTER TABLE patient MODIFY FeeSched INTEGER default '0'";//already not null
					Db.NonQ32(command);
					command="ALTER TABLE patient MODIFY BillingType INTEGER default '0'";//already not null
					Db.NonQ32(command);
					command="ALTER TABLE patient MODIFY ClinicNum INTEGER default '0'";//already not null
					Db.NonQ32(command);
					try {
						command="SELECT COUNT(*) FROM popup";
						Db.GetTable(command);
						//Table exists at this point.
						command="DROP TABLE popup PURGE";
						Db.NonQ32(command);
					} catch {//The table does not exist.
					}
					command=@"CREATE TABLE popup(
						PopupNum int default '0' NOT NULL,
						PatNum int default '0' NOT NULL,
						Description varchar2(4000),
						IsDisabled int default '0' NOT NULL,		
						PRIMARY KEY (PopupNum)
						)";
					Db.NonQ32(command);
					command="ALTER TABLE registrationkey ADD DateStarted DATE default TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE registrationkey ADD DateDisabled DATE default TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE registrationkey ADD DateEnded DATE default TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE registrationkey ADD IsForeign varchar2(1) default '0' NOT NULL";
					Db.NonQ32(command);
					command="UPDATE registrationkey SET DateStarted=(SELECT CURRENT_DATE FROM dual)";
					Db.NonQ32(command);
					command="UPDATE registrationkey SET DateEnded=TO_DATE('0001-01-01','YYYY-MM-DD')";
					Db.NonQ32(command);
					command="UPDATE registrationkey SET DateDisabled=TO_DATE('0001-01-01','YYYY-MM-DD')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('UpdateWindowShowsClassicView','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('BillingExcludeInsPending','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('UpdateServerAddress','')";
					Db.NonQ32(command);
					command="UPDATE preference SET PrefName='UpdateCode' WHERE PrefName='RegistrationNumber'";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD DateTP DATE default TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL";
					Db.NonQ32(command);
					command="UPDATE procedurelog SET DateTP=ProcDate";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.4.0.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_4_1();
		}

		private static void To5_4_1() {
			if(FromVersion<new Version("5.4.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.4.1");//No translation in convert script.
				string command;
				command="UPDATE preference SET ValueString = '5.4.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_4_7();
		}

		private static void To5_4_7() {
			if(FromVersion<new Version("5.4.7.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.4.7");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference VALUES('RegistrationKeyIsDisabled','0')";
					Db.NonQ32(command);
				} else {//oracle
					command="INSERT INTO preference VALUES('RegistrationKeyIsDisabled','0')";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.4.7.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_4_14();
		}

		private static void To5_4_14() {
			if(FromVersion<new Version("5.4.14.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.4.14");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command=@"UPDATE task SET ObjectType=2 WHERE ObjectType=1 AND task.KeyNum!=0
						AND NOT EXISTS(SELECT * FROM patient WHERE PatNum=KeyNum)";
					Db.NonQ32(command);
				} else {//oracle
					command=@"UPDATE task SET ObjectType=2 WHERE ObjectType=1 AND task.KeyNum!=0
						AND NOT EXISTS(SELECT * FROM patient WHERE PatNum=KeyNum)";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.4.14.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_5_1();
		}

		private static void To5_5_1() {
			if(FromVersion<new Version("5.5.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.5.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS claimattach";
					Db.NonQ32(command);
					command=@"CREATE TABLE claimattach (
						ClaimAttachNum int NOT NULL auto_increment,
						ClaimNum int NOT NULL,
						DisplayedFileName varchar(255),
						ActualFileName varchar(255),
						PRIMARY KEY (ClaimAttachNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					//after r1050
					command="ALTER TABLE treatplan ADD Signature text";
					Db.NonQ32(command);
					command="ALTER TABLE treatplan ADD SigIsTopaz tinyint(1) not null";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('TreatPlanPriorityForDeclined','0')";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS supplyneeded";
					Db.NonQ32(command);
					command=@"CREATE TABLE supplyneeded (
						SupplyNeededNum int NOT NULL auto_increment,
						Description text,
						DateAdded DATE NOT NULL,
						PRIMARY KEY (SupplyNeededNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS supplier";
					Db.NonQ32(command);
					command=@"CREATE TABLE supplier (
						SupplierNum int NOT NULL auto_increment,
						Name varchar(255),
						Phone varchar(255),
						CustomerId varchar(255),
						Website text,
						UserName varchar(255),
						Password varchar(255),
						Note text,
						PRIMARY KEY (SupplierNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS supply";
					Db.NonQ32(command);
					command=@"CREATE TABLE supply (
						SupplyNum int NOT NULL auto_increment,
						SupplierNum int NOT NULL,
						CatalogNumber varchar(255),
						Descript varchar(255),
						Category int NOT NULL,
						ItemOrder int NOT NULL,
						LevelDesired float NOT NULL,
						IsHidden tinyint(1) NOT NULL,
						Price double NOT NULL,
						PRIMARY KEY (SupplyNum),
						INDEX (SupplierNum)
						) DEFAULT CHARSET=utf8";
					//the above definition has changed repeatedly.  You might need to drop the table and do it again:
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS supplyorder";
					Db.NonQ32(command);
					command=@"CREATE TABLE supplyorder (
						SupplyOrderNum int NOT NULL auto_increment,
						SupplierNum int NOT NULL,
						DatePlaced date NOT NULL,
						Note text,
						AmountTotal double NOT NULL,
						PRIMARY KEY (SupplyOrderNum),
						INDEX (SupplierNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS supplyorderitem";
					Db.NonQ32(command);
					command=@"CREATE TABLE supplyorderitem (
						SupplyOrderItemNum int NOT NULL auto_increment,
						SupplyOrderNum int NOT NULL,
						SupplyNum int NOT NULL,
						Qty int NOT NULL,
						Price double NOT NULL,
						PRIMARY KEY (SupplyOrderItemNum),
						INDEX (SupplyOrderNum),
						INDEX (SupplyNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					//supplyorder was altered later.
					command="DROP TABLE IF EXISTS tasksubscription";
					Db.NonQ32(command);
					command=@"CREATE TABLE tasksubscription (
						TaskSubscriptionNum int NOT NULL auto_increment,
						UserNum int NOT NULL,
						TaskListNum int NOT NULL,
						PRIMARY KEY (TaskSubscriptionNum),
						INDEX (UserNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('TasksCheckOnStartup','0')";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS taskancestor";
					Db.NonQ32(command);
					command=@"CREATE TABLE taskancestor (
						TaskAncestorNum int NOT NULL auto_increment,
						TaskNum int NOT NULL,
						TaskListNum int NOT NULL,
						PRIMARY KEY (TaskAncestorNum),
						INDEX (TaskNum),
						INDEX (TaskListNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('TaskAncestorsAllSetInVersion55','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('ApptExclamationShowForUnsentIns','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('ProviderIncomeTransferShows','0')";
					Db.NonQ32(command);
				} else {//oracle
					try {
						command="SELECT COUNT(*) FROM claimattach";
						Db.GetTable(command);
						//Table exists at this point.
						command="DROP TABLE claimattach PURGE";
						Db.NonQ32(command);
					} catch {//The table does not exist.
					}
					command=@"CREATE TABLE claimattach (
						ClaimAttachNum int default '0' NOT NULL,
						ClaimNum int default '0' NOT NULL,
						DisplayedFileName varchar2(255),
						ActualFileName varchar2(255),
						PRIMARY KEY (ClaimAttachNum)
						)";
					Db.NonQ32(command);
					//after r1050
					command="ALTER TABLE treatplan ADD Signature varchar(4000)";
					Db.NonQ32(command);
					command="ALTER TABLE treatplan ADD SigIsTopaz int default '0' not null";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('TreatPlanPriorityForDeclined','0')";
					Db.NonQ32(command);
					try {
						command="SELECT COUNT(*) FROM supplyneeded";
						Db.GetTable(command);
						//The table exists at this point.
						command="DROP TABLE supplyneeded PURGE";
						Db.NonQ32(command);
					} catch {//The table does not exist
					}
					command=@"CREATE TABLE supplyneeded (
						SupplyNeededNum int default '0' NOT NULL,
						Description varchar2(4000),
						DateAdded DATE default TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						PRIMARY KEY (SupplyNeededNum)
						)";
					Db.NonQ32(command);
					try {
						command="SELECT COUNT(*) FROM supplier";
						Db.GetTable(command);
						//The table exists at this point.
						command="DROP TABLE supplier PURGE";
						Db.NonQ32(command);
					} catch {//The table does not exist.
					}
					command=@"CREATE TABLE supplier (
						SupplierNum int default '0' NOT NULL,
						Name varchar2(255),
						Phone varchar2(255),
						CustomerId varchar2(255),
						Website varchar2(4000),
						UserName varchar2(255),
						Password varchar2(255),
						Note varchar2(4000),
						PRIMARY KEY (SupplierNum)
						)";
					Db.NonQ32(command);
					try {
						command="SELECT COUNT(*) FROM supply";
						Db.GetTable(command);
						//The table exists at this point.
						command="DROP TABLE supply PURGE";
						Db.NonQ32(command);
					} catch {//The table does not exist.
					}
					command=@"CREATE TABLE supply (
						SupplyNum int default '0' NOT NULL,
						SupplierNum int default '0' NOT NULL,
						CatalogNumber varchar2(255),
						Descript varchar2(255),
						Category int default '0' NOT NULL,
						ItemOrder int default '0' NOT NULL,
						LevelDesired BINARY_FLOAT default '0' NOT NULL,
						IsHidden int default '0' NOT NULL,
						Price BINARY_DOUBLE default '0' NOT NULL,
						PRIMARY KEY (SupplyNum)
						)";
					//the above definition has changed repeatedly.  You might need to drop the table and do it again:
					Db.NonQ32(command);
					command="CREATE INDEX SUPPLY_SUPPLIERNUM ON supply (SupplierNum)";
					Db.NonQ32(command);
					try {
						command="SELECT COUNT(*) FROM supplyorder";
						Db.GetTable(command);
						//The table exists at this point.
						command="DROP TABLE supplyorder PURGE";
						Db.NonQ32(command);
					} catch {//The table does not exist.
					}
					command=@"CREATE TABLE supplyorder (
						SupplyOrderNum int default '0' NOT NULL,
						SupplierNum int default '0' NOT NULL,
						DatePlaced date default TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						Note varchar2(4000),
						AmountTotal BINARY_DOUBLE default '0' NOT NULL,
						PRIMARY KEY (SupplyOrderNum)
						)";
					Db.NonQ32(command);
					command="CREATE INDEX SUPPLYORDER_SUPPLIERNUM ON supplyorder (SupplierNum)";
					Db.NonQ32(command);
					try {
						command="SELECT COUNT(*) FROM supplyorderitem";
						Db.GetTable(command);
						//The table exists at this point.
						command="DROP TABLE supplyorderitem PURGE";
						Db.NonQ32(command);
					} catch {//The table does not exist.
					}
					command=@"CREATE TABLE supplyorderitem (
						SupplyOrderItemNum int default '0' NOT NULL,
						SupplyOrderNum int default '0' NOT NULL,
						SupplyNum int default '0' NOT NULL,
						Qty int default '0' NOT NULL,
						Price BINARY_DOUBLE NOT NULL,
						PRIMARY KEY (SupplyOrderItemNum)
						)";
					Db.NonQ32(command);
					command="CREATE INDEX SUPPLYORDERITEM_SUPPLYORDERNUM ON supplyorderitem (SupplyOrderNum)";
					Db.NonQ32(command);
					command="CREATE INDEX SUPPLYORDERITEM_SUPPLYNUM ON supplyorderitem (SupplyNum)";
					Db.NonQ32(command);
					//supplyorder was altered later.
					try {
						command="SELECT COUNT(*) FROM tasksubscription";
						Db.GetTable(command);
						//The table exists at this point.
						command="DROP TABLE tasksubscription PURGE";
						Db.NonQ32(command);
					} catch {//The table does not exist.
					}
					command=@"CREATE TABLE tasksubscription (
						TaskSubscriptionNum int default '0' NOT NULL,
						UserNum int default '0' NOT NULL,
						TaskListNum int default '0' NOT NULL,
						PRIMARY KEY (TaskSubscriptionNum)
						)";
					Db.NonQ32(command);
					command="CREATE INDEX TASKSUBSCRIPTION_USERNUM ON tasksubscription (UserNum)";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('TasksCheckOnStartup','0')";
					Db.NonQ32(command);
					try {
						command="SELECT COUNT(*) FROM taskancestor";
						Db.GetTable(command);
						//The table exists at this point.
						command="DROP TABLE taskancestor PURGE";
						Db.NonQ32(command);
					} catch {//The table does not exist.
					}
					command=@"CREATE TABLE taskancestor (
						TaskAncestorNum int default '0' NOT NULL,
						TaskNum int default '0' NOT NULL,
						TaskListNum int default '0' NOT NULL,
						PRIMARY KEY (TaskAncestorNum)
						)";
					Db.NonQ32(command);
					command="CREATE INDEX TASKANCESTOR_TASKNUM ON taskancestor (TaskNum)";
					Db.NonQ32(command);
					command="CREATE INDEX TASKANCESTOR_TASKLISTNUM ON taskancestor (TaskListNum)";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('TaskAncestorsAllSetInVersion55','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('ApptExclamationShowForUnsentIns','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference VALUES('ProviderIncomeTransferShows','0')";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.5.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_6_1();
		}

		private static void To5_6_1() {
			if(FromVersion<new Version("5.6.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.6.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE computerpref ADD TaskKeepListHidden tinyint(1) NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE computerpref ADD TaskDock int NOT NULL default '0'";
					Db.NonQ32(command);
					command="ALTER TABLE computerpref ADD TaskX int NOT NULL default '900'";
					Db.NonQ32(command);
					command="ALTER TABLE computerpref ADD TaskY int NOT NULL default '625'";
					Db.NonQ32(command);
					//added after r1182
					command="UPDATE preference SET ValueString = 'http://70.90.133.65:1942/WebServiceCustomerUpdates/Service1.asmx' WHERE PrefName = 'UpdateServerAddress'";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('ClaimAttachExportPath','C:\\\\TempImages\\\\')";
					Db.NonQ32(command);
					//added after r1239
					command="DROP TABLE IF EXISTS statement";
					Db.NonQ32(command);
					command=@"CREATE TABLE statement (
							StatementNum int NOT NULL auto_increment,
							PatNum int NOT NULL,
							DateSent date NOT NULL,
							DateRangeFrom date NOT NULL,
							DateRangeTo date NOT NULL,
							Note text,
							NoteBold text,
							Mode_ tinyint unsigned NOT NULL,
							HidePayment tinyint(1) NOT NULL,
							SinglePatient tinyint(1) NOT NULL,
							Intermingled tinyint(1) NOT NULL,
							IsSent tinyint(1) NOT NULL,
							DocNum int NOT NULL,
							PRIMARY KEY (StatementNum),
							INDEX (PatNum),
							INDEX (DocNum)
							) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS docattach";
					Db.NonQ32(command);
					command="SELECT MAX(ItemOrder) FROM definition WHERE Category=18";
					int defnum=PIn.Int(Db.GetCount(command));
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) "
						+"VALUES(18,"+POut.Long(defnum+1)+",'Statements','S',0,0)";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('AutoResetTPEntryStatus','1')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('ShowAccountFamilyCommEntries','1')";
					Db.NonQ32(command);
					//convert all existing statement commlog entries to statement objects
					command="SELECT * FROM commlog WHERE IsStatementSent= 1";
					DataTable table=Db.GetTable(command);
					for(int i=0;i<table.Rows.Count;i++) {
						command="INSERT INTO statement (PatNum,DateSent,DateRangeFrom,DateRangeTo,Note,NoteBold,Mode_,"
							+"IsSent) VALUES ("
							+table.Rows[i]["PatNum"].ToString()+","
							+POut.Date(PIn.Date(table.Rows[i]["CommDateTime"].ToString()))+","
							+POut.Date(new DateTime(1,1,1))+","
							+POut.Date(new DateTime(2200,1,1))+","
							+"'"+POut.String(PIn.String(table.Rows[i]["Note"].ToString()))+"',"
							+"'',";
						if(table.Rows[i]["Mode_"].ToString()=="4") {//InPerson
							command+="1,";
						} else if(table.Rows[i]["Mode_"].ToString()=="1") {//Email
							command+="2,";
						} else {//mail
							command+="0,";
						}
						command+="1)";
						Db.NonQ32(command);
					}
					command="DELETE FROM commlog WHERE IsStatementSent=1";
					Db.NonQ32(command);
					command="ALTER TABLE dunning ADD MessageBold text";
					Db.NonQ32(command);
					command="UPDATE dunning SET MessageBold=''";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('BillingDefaultsLastDays','45')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('BillingDefaultsIntermingle','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('BillingDefaultsNote','')";
					Db.NonQ32(command);
					command="ALTER TABLE insplan ADD CodeSubstNone tinyint(1) NOT NULL";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('BackupExcludeImageFolder','0')";
					Db.NonQ32(command);
					command="ALTER TABLE task ADD UserNum int NOT NULL";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('InsDefaultPPOpercent','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('TimecardUsersDontEditOwnCard','0')";
					Db.NonQ32(command);
					command="ALTER TABLE task ADD DateTimeFinished datetime NOT NULL";
					Db.NonQ32(command);
					command="UPDATE task SET DateTimeFinished ="+POut.Date(DateTime.MinValue);
					//equiv to:UPDATE task SET DateTimeFinished ='0001-01-01'
					Db.NonQ32(command);
					command="ALTER TABLE signal ADD TaskNum int NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE signal ADD INDEX indexAckTime (AckTime)";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('EmailUseSSL','0')";
					Db.NonQ32(command);
					command="DELETE FROM preference WHERE PrefName='PrintSimpleStatements'";
					Db.NonQ32(command);
					command="DELETE FROM preference WHERE PrefName='BoldFamilyAccountBalanceView'";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('PayPlansBillInAdvanceDays','10')";
					Db.NonQ32(command);
					command="ALTER TABLE patient ADD PayPlanDue double NOT NULL";
					Db.NonQ32(command);
				} else {//oracle
					command="ALTER TABLE computerpref ADD TaskKeepListHidden int default '0' NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE computerpref ADD TaskDock int default '0' NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE computerpref ADD TaskX int default '900' NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE computerpref ADD TaskY int default '625' NOT NULL";
					Db.NonQ32(command);
					//added after r1182
					command="UPDATE preference SET ValueString = 'http://70.90.133.65:1942/WebServiceCustomerUpdates/Service1.asmx' WHERE PrefName = 'UpdateServerAddress'";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('ClaimAttachExportPath','C:\\\\TempImages\\\\')";
					Db.NonQ32(command);
					//added after r1239
					try {
						command="SELECT COUNT(*) FROM statement";
						Db.GetTable(command);
						//The table exists at this point.
						command="DROP TABLE statement PURGE";
						Db.NonQ32(command);
					} catch {//The table does not exist.
					}
					command=@"CREATE TABLE statement (
							StatementNum int default '0' NOT NULL,
							PatNum int default '0' NOT NULL,
							DateSent date default TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
							DateRangeFrom date default TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
							DateRangeTo date default TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
							Note varchar2(4000),
							NoteBold varchar2(4000),
							Mode_ int default '0' NOT NULL,
							HidePayment int default '0' NOT NULL,
							SinglePatient int default '0' NOT NULL,
							Intermingled int default '0' NOT NULL,
							IsSent int default '0' NOT NULL,
							DocNum int default '0' NOT NULL,
							PRIMARY KEY (StatementNum)
							)";
					Db.NonQ32(command);
					command="CREATE INDEX STATEMENT_PATNUM ON statement (PatNum)";
					Db.NonQ32(command);
					command="CREATE INDEX STATEMENT_DOCNUM ON statement (DocNum)";
					Db.NonQ32(command);
					try {
						command="SELECT COUNT(*) FROM docattach";
						Db.GetTable(command);
						//The table exists at this point.
						command="DROP TABLE docattach PURGE";
						Db.NonQ32(command);
					} catch {//The table does not exist.
					}
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) "
						+"VALUES((SELECT MAX(DefNum)+1 FROM definition),18,"
						+"(SELECT MAX(ItemOrder)+1 FROM definition WHERE Category=18),'Statements','S',0,0)";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('AutoResetTPEntryStatus','1')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('ShowAccountFamilyCommEntries','1')";
					Db.NonQ32(command);
					//convert all existing statement commlog entries to statement objects
					command="INSERT INTO statement (PatNum,DateSent,DateRangeFrom,DateRangeTo,Note,NoteBold,Mode_,IsSent) "+
						"SELECT PatNum,CommDateTime,TO_DATE('0001-01-01','YYYY-MM-DD'),TO_DATE('2200-01-01','YYYY-MM-DD'),Note,'',"+
							"CASE Mode_ WHEN 4 THEN 1 WHEN 1 THEN 2 ELSE 0 END,1 "+
						"FROM commlog "+
						"WHERE IsStatementSent=1";
					Db.NonQ32(command);
					command="DELETE FROM commlog WHERE IsStatementSent=1";
					Db.NonQ32(command);
					command="ALTER TABLE dunning ADD MessageBold varchar2(4000)";
					Db.NonQ32(command);
					command="UPDATE dunning SET MessageBold=''";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('BillingDefaultsLastDays','45')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('BillingDefaultsIntermingle','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('BillingDefaultsNote','')";
					Db.NonQ32(command);
					command="ALTER TABLE insplan ADD CodeSubstNone int default '0' NOT NULL";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('BackupExcludeImageFolder','0')";
					Db.NonQ32(command);
					command="ALTER TABLE task ADD UserNum int default '0' NOT NULL";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('InsDefaultPPOpercent','0')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('TimecardUsersDontEditOwnCard','0')";
					Db.NonQ32(command);
					command=@"ALTER TABLE task ADD DateTimeFinished timestamp
						default TO_TIMESTAMP('0001-01-01 00:00:00','YYYY-MM-DD HH24:MI:SS')  NOT NULL";
					Db.NonQ32(command);
					command="UPDATE task SET DateTimeFinished=TO_DATE('0001-01-01','YYYY-MM-DD')";
					Db.NonQ32(command);
					command="ALTER TABLE signal ADD TaskNum int default '0' NOT NULL";
					Db.NonQ32(command);
					command="CREATE INDEX SIGNAL_ACKTIME ON signal (AckTime)";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('EmailUseSSL','0')";
					Db.NonQ32(command);
					command="DELETE FROM preference WHERE PrefName='PrintSimpleStatements'";
					Db.NonQ32(command);
					command="DELETE FROM preference WHERE PrefName='BoldFamilyAccountBalanceView'";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('PayPlansBillInAdvanceDays','10')";
					Db.NonQ32(command);
					command="ALTER TABLE patient ADD PayPlanDue BINARY_DOUBLE default '0' NOT NULL";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.6.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_6_18();
		}

		private static void To5_6_18() {
			if(FromVersion<new Version("5.6.18.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.6.18");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor,IsHidden) VALUES(0,7,'Insurance Payment',-16744448,0)";//Green
					Db.NonQ32(command);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor,IsHidden) VALUES(0,8,'Received Ins Claim',-5220352,0)";//drkOrange
				} else {//oracle
					command=@"INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemColor,IsHidden) 
						VALUES((SELECT MAX(DefNum)+1 FROM definition),0,7,'Insurance Payment',-16744448,0)";//Green
					Db.NonQ32(command);
					command=@"INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemColor,IsHidden) 
						VALUES((SELECT MAX(DefNum)+1 FROM definition),0,8,'Received Ins Claim',-5220352,0)";//drkOrange
				}
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '5.6.18.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_6_22();
		}

		private static void To5_6_22() {
			if(FromVersion<new Version("5.6.22.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.6.22");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE paysplit ADD INDEX (ProcNum)";
					try {
						Db.NonQ32(command);
					} catch {
					}
					command="ALTER TABLE paysplit ADD INDEX (PayNum)";
					try {
						Db.NonQ32(command);
					} catch {
					}
				} else {//oracle
					command="CREATE INDEX PAYSPLIT_PROCNUM ON paysplit (ProcNum)";
					Db.NonQ32(command);
					command="CREATE INDEX PAYSPLIT_PAYNUM ON paysplit (PayNum)";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.6.22.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_6_28();
		}

		private static void To5_6_28() {
			if(FromVersion<new Version("5.6.28.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.6.28");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					//Digora Bridge---------------------------------------------------------------------------
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'Digora', "
						+"'Digora from www.soredex.com', "
						+"'0', "
						+"'', "
						+"'', "
						+"'"+POut.String(@"No path is needed.")+"')";
					int programNum=Db.NonQ32(command,true);//we now have a ProgramNum to work with
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+programNum.ToString()+"', "
						+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
						+"'0')";
					Db.NonQ32(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"'"+POut.Long(programNum)+"', "
						+"'"+POut.Long((int)ToolBarsAvail.ChartModule)+"', "
						+"'Digora')";
					Db.NonQ32(command);
				} else {//oracle
					//Digora Bridge---------------------------------------------------------------------------
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"(SELECT MAX(ProgramNum)+1 FROM program),"
						+"'Digora', "
						+"'Digora from www.soredex.com', "
						+"'0', "
						+"'', "
						+"'', "
						+"'No path is needed.')";
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum)+1 FROM programproperty),"
						+"(SELECT MAX(ProgramNum) FROM program),"
						+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
						+"'0')";
					Db.NonQ32(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
						+"(SELECT MAX(ProgramNum) FROM program), "
						+"'"+POut.Long((int)ToolBarsAvail.ChartModule)+"', "
						+"'Digora')";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.6.28.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_6_31();
		}

		private static void To5_6_31() {
			if(FromVersion<new Version("5.6.31.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.6.31");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('ClaimFormTreatDentSaysSigOnFile','0')";
					Db.NonQ32(command);
				} else {//oracle
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('ClaimFormTreatDentSaysSigOnFile','0')";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.6.31.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_7_1();
		}

		private static void To5_7_1() {
			if(FromVersion<new Version("5.7.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.7.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('StatementSummaryShowInsInfo','1')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('IntermingleFamilyDefault','0')";
					Db.NonQ32(command);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemColor,IsHidden) VALUES(0,9,'Received Pre-Auth',-8388480,0)";//Purple
					Db.NonQ32(command);
					command="ALTER TABLE commlog ADD INDEX (PatNum)";
					try {
						Db.NonQ32(command);
					} catch {
					}
					command="ALTER TABLE claim ADD AttachedImages int NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE claim ADD AttachedModels int NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE claim ADD AttachedFlags varchar(255)";
					Db.NonQ32(command);
					command="ALTER TABLE claim ADD AttachmentID varchar(255)";
					Db.NonQ32(command);
					//new claim form fields for attachments---------------------------------
					command="SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD8'";
					DataTable table=Db.GetTable(command);
					if(table.Rows.Count>0) {
						string claimFormNum=table.Rows[0][0].ToString();
						command="INSERT INTO claimformitem (ClaimFormNum,ImageFileName,FieldName,FormatString"
							+",XPos,YPos,Width,Height) VALUES("+claimFormNum+",'','AttachedImagesNum','',746,779,27,14)";
						Db.NonQ32(command);
						command="INSERT INTO claimformitem (ClaimFormNum,ImageFileName,FieldName,FormatString"
							+",XPos,YPos,Width,Height) VALUES("+claimFormNum+",'','AttachedModelsNum','',796,779,27,14)";
						Db.NonQ32(command);
					}
					//Appt time override----------------------------------------------------
					//This query uses nested CONCATs for compatibility with Oracle database:
					//Also, an assumption is made that the user is on 10min increments.  This is a somewhat safe assumption, since 15 minute offices will not track their time as closely.  The inconvenience will be minor in any case.
					command="UPDATE appointment SET Note=CONCAT('AddTime: ',CONCAT(AddTime,CONCAT('0',CONCAT(' ',Note)))) WHERE AddTime !=0";
					Db.NonQ32(command);
					command="UPDATE appointment SET AddTime=0";
					Db.NonQ32(command);
					command="ALTER TABLE appointment CHANGE AddTime TimeLocked tinyint(1) NOT NULL";//bool
					Db.NonQ32(command);
					command="UPDATE procedurelog SET UnitQty=1 WHERE UnitQty=0";
					Db.NonQ32(command);
					command="DELETE FROM signal";
					Db.NonQ32(command);
					//currently, the max length will end up being about 100, but that wouldn't leave much room for expansion with only 256.
					command="ALTER TABLE signal CHANGE ITypes ITypes text";
					Db.NonQ32(command);
					command="ALTER TABLE userod ADD TaskListInBox int NOT NULL";
					Db.NonQ32(command);
				} else {//oracle
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('StatementSummaryShowInsInfo','1')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('IntermingleFamilyDefault','0')";
					Db.NonQ32(command);
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemColor,IsHidden) "+
						"VALUES((SELECT MAX(DefNum)+1 FROM definition),0,9,'Received Pre-Auth',-8388480,0)";//Purple
					Db.NonQ32(command);
					command="CREATE INDEX COMMLOG_PATNUM ON commlog (PatNum)";
					Db.NonQ32(command);
					command="ALTER TABLE claim ADD AttachedImages int default '0' NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE claim ADD AttachedModels int default '0' NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE claim ADD AttachedFlags varchar2(255)";
					Db.NonQ32(command);
					command="ALTER TABLE claim ADD AttachmentID varchar2(255)";
					Db.NonQ32(command);
					//new claim form fields for attachments---------------------------------
					command="SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD8'";
					DataTable table=Db.GetTable(command);
					if(table.Rows.Count>0) {
						string claimFormNum=table.Rows[0][0].ToString();
						command="INSERT INTO claimformitem (ClaimFormNum,ImageFileName,FieldName,FormatString"
							+",XPos,YPos,Width,Height) VALUES("+claimFormNum+",'','AttachedImagesNum','',746,779,27,14)";
						Db.NonQ32(command);
						command="INSERT INTO claimformitem (ClaimFormNum,ImageFileName,FieldName,FormatString"
							+",XPos,YPos,Width,Height) VALUES("+claimFormNum+",'','AttachedModelsNum','',796,779,27,14)";
						Db.NonQ32(command);
					}
					//Appt time override----------------------------------------------------
					//Also, an assumption is made that the user is on 10min increments.  This is a somewhat safe assumption, since 15 minute offices will not track their time as closely.  The inconvenience will be minor in any case.
					command="UPDATE appointment SET Note=CONCAT('AddTime: ',CONCAT(AddTime,CONCAT('0',CONCAT(' ',Note)))) WHERE AddTime !=0";
					Db.NonQ32(command);
					command="UPDATE appointment SET AddTime=0";
					Db.NonQ32(command);
					command="ALTER TABLE appointment RENAME COLUMN AddTime TO TimeLocked";
					Db.NonQ32(command);
					command="ALTER TABLE appointment MODIFY (TimeLocked int default '0')";//already not null
					Db.NonQ32(command);
					command="UPDATE procedurelog SET UnitQty=1 WHERE UnitQty=0";
					Db.NonQ32(command);
					command="DELETE FROM signal";
					Db.NonQ32(command);
					//currently, the max length will end up being about 100, but that wouldn't leave much room for expansion with only 256.
					command="ALTER TABLE signal MODIFY (ITypes varchar2(4000))";
					Db.NonQ32(command);
					command="ALTER TABLE userod ADD TaskListInBox int default '0' NOT NULL";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.7.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_7_3();
		}

		private static void To5_7_3() {
			if(FromVersion<new Version("5.7.3.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.7.3");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('DockPhonePanelShow','0')";
					Db.NonQ32(command);
				} else {//oracle
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('DockPhonePanelShow','0')";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.7.3.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_7_4();
		}

		private static void To5_7_4() {
			if(FromVersion<new Version("5.7.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.7.4");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('ClaimFormTreatDentSaysSigOnFile','0')";
					try {
						Db.NonQ32(command);
					} catch {
						//might already exist
					}
					command="ALTER TABLE employee ADD PhoneExt int NOT NULL";
					Db.NonQ32(command);
				} else {//oracle
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('ClaimFormTreatDentSaysSigOnFile','0')";
					try {
						Db.NonQ32(command);
					} catch {
						//might already exist
					}
					command="ALTER TABLE employee ADD PhoneExt int default '0' NOT NULL";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.7.4.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_7_15();
		}

		private static void To5_7_15() {
			if(FromVersion<new Version("5.7.15.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.7.15");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql){
					command="ALTER TABLE procedurelog ADD INDEX (PlannedAptNum)";
					try {
						Db.NonQ32(command);
					}
					catch {
					}
				}else{//oracle
					command="CREATE INDEX PROCEDURELOG_PLANNEDAPTNUM ON procedurelog (PlannedAptNum)";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.7.15.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_8_1();
		}

		private static void To5_8_1() {
			if(FromVersion<new Version("5.8.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.8.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql){
					command="DROP TABLE IF EXISTS site";
					Db.NonQ32(command);
					command=@"CREATE TABLE site (
						SiteNum int NOT NULL auto_increment,
						Description varchar(255),
						Note text,
						PRIMARY KEY (SiteNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command="INSERT INTO site (Description,Note) SELECT SchoolName,SchoolCode FROM school"; 
					Db.NonQ32(command);
					command="INSERT INTO site (Description,Note) SELECT distinct GradeSchool,'' FROM patient WHERE GradeSchool !='' AND NOT EXISTS(SELECT * FROM site WHERE site.Description=patient.GradeSchool)";
					Db.NonQ32(command);
					command="ALTER TABLE patient ADD SiteNum int default '0' NOT NULL";
					Db.NonQ32(command);
					command="UPDATE patient SET patient.SiteNum= (SELECT site.SiteNum FROM site WHERE site.Description=patient.GradeSchool)";
					Db.NonQ32(command);
					command="ALTER TABLE patient DROP COLUMN GradeSchool";
					Db.NonQ32(command);
					command="ALTER TABLE patient DROP COLUMN PrimaryTeeth";
					Db.NonQ32(command);
					command="ALTER TABLE procedurelog ADD SiteNum int default '0' NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE preference ADD Comments text default ''";
					Db.NonQ32(command);
					command="UPDATE preference SET Comments=''";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('TitleBarShowSite','0','Shows the site.Description of the patient.Site in the main title bar.')";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS pharmacy";
					Db.NonQ32(command);
					command=@"CREATE TABLE pharmacy (
						PharmacyNum int NOT NULL auto_increment,
						PharmID varchar(255),
						StoreName varchar(255),
						Phone varchar(255),
						Fax varchar(255),
						Address varchar(255),
						Address2 varchar(255),
						City varchar(255),
						State varchar(255),
						Zip varchar(255),
						Note text,
						PRIMARY KEY (PharmacyNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command="ALTER TABLE rxpat ADD PharmacyNum int default '0' NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE clinic ADD InsBillingProv int default '0' NOT NULL";
					Db.NonQ32(command);
					command="SELECT ValueString FROM preference WHERE PrefName= 'InsBillingProv'";
					DataTable table=Db.GetTable(command);
					int practiceBillingProv=PIn.Int(table.Rows[0][0].ToString());
					command="UPDATE clinic SET InsBillingProv='"+practiceBillingProv.ToString()+"'";
					Db.NonQ32(command);
					command="ALTER TABLE displayfield ADD Category int default '0' NOT NULL";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS sheet";
					Db.NonQ32(command);
					command=@"CREATE TABLE sheet (
						SheetNum int NOT NULL auto_increment,
						SheetType int NOT NULL,
						PatNum int NOT NULL,
						DateTimeSheet datetime default '0001-01-01' NOT NULL,
						FontSize float NOT NULL,
						FontName varchar(255),
						Width int NOT NULL,
						Height int NOT NULL,
						IsLandscape tinyint NOT NULL,
						InternalNote text,
						PRIMARY KEY (SheetNum),
						INDEX (PatNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS sheetfield";
					Db.NonQ32(command);
					command=@"CREATE TABLE sheetfield (
						SheetFieldNum int NOT NULL auto_increment,
						SheetNum int NOT NULL,
						FieldType int NOT NULL,
						FieldName varchar(255),
						FieldValue text,
						FontSize float NOT NULL,
						FontName varchar(255),
						FontIsBold tinyint NOT NULL,
						XPos int NOT NULL,
						YPos int NOT NULL,
						Width int NOT NULL,
						Height int NOT NULL,
						GrowthBehavior int NOT NULL,
						PRIMARY KEY (SheetFieldNum),
						INDEX (SheetNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS sheetdef";
					Db.NonQ32(command);
					command=@"CREATE TABLE sheetdef (
						SheetDefNum int NOT NULL auto_increment,
						Description varchar(255),
						SheetType int NOT NULL,
						FontSize float NOT NULL,
						FontName varchar(255),
						Width int NOT NULL,
						Height int NOT NULL,
						IsLandscape tinyint NOT NULL,	
						PRIMARY KEY (SheetDefNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS sheetfielddef";
					Db.NonQ32(command);
					command=@"CREATE TABLE sheetfielddef (
						SheetFieldDefNum int NOT NULL auto_increment,
						SheetDefNum int NOT NULL,
						FieldType int NOT NULL,
						FieldName varchar(255),
						FieldValue varchar(255),
						FontSize float NOT NULL,
						FontName varchar(255),
						FontIsBold tinyint NOT NULL,
						XPos int NOT NULL,
						YPos int NOT NULL,
						Width int NOT NULL,
						Height int NOT NULL,
						GrowthBehavior int NOT NULL,
						PRIMARY KEY (SheetFieldDefNum),
						INDEX (SheetDefNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command="ALTER TABLE referral ADD Slip int default '0' NOT NULL";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('LabelPatientDefaultSheetDefNum','0','0 indicates default.  Otherwise, foreign key to SheetDefNum.')";
					Db.NonQ32(command);
				}
				else{//oracle

				}
				command="UPDATE preference SET ValueString = '5.8.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_8_2();
		}

		private static void To5_8_2() {
			if(FromVersion<new Version("5.8.2.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.8.2");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql){
					command="ALTER TABLE fee ADD INDEX (CodeNum)";
					try {
						Db.NonQ32(command);
					}
					catch {
					}
					command="UPDATE claim SET AttachedFlags='Mail' WHERE AttachedFlags IS NULL";
					Db.NonQ32(command);
				}else{//oracle
					command="CREATE INDEX FEE_CODENUM ON fee (CodeNum)";
					Db.NonQ32(command);
					command="UPDATE claim SET AttachedFlags='Mail' WHERE AttachedFlags IS NULL";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '5.8.2.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To5_9_1();
		}

		private static void To5_9_1() {
			if(FromVersion<new Version("5.9.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 5.9.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DELETE FROM preference WHERE PrefName='RxOrientVert'";
					Db.NonQ32(command);
					command="DELETE FROM preference WHERE PrefName='RxAdjustRight'";
					Db.NonQ32(command);
					command="DELETE FROM preference WHERE PrefName='RxAdjustDown'";
					Db.NonQ32(command);
					command="DELETE FROM preference WHERE PrefName='RxGeneric'";
					Db.NonQ32(command);
					command="ALTER TABLE rxdef ADD IsControlled tinyint NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE rxpat ADD IsControlled tinyint NOT NULL";
					Db.NonQ32(command);
					command="UPDATE rxdef SET IsControlled = 1";
					Db.NonQ32(command);
					//UAppoint Bridge---------------------------------------------------------------------------
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'UAppoint', "
						+"'UAppoint from www.uappoint.com', "
						+"'0', "
						+"'"+POut.String(@"https://s0.uappoint.com/Sync")+"', "
						+"'', "
						+"'')";
					int programNum=Db.NonQ32(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'Username', "
						+"'')";
					Db.NonQ32(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'Password', "
						+"'')";
					Db.NonQ32(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'WorkstationName', "
						+"'')";
					Db.NonQ32(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'IntervalSeconds', "
						+"'15')";
					Db.NonQ32(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'DateTimeLastUploaded', "
						+"'')";
					Db.NonQ32(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'SynchStatus', "
						+"'')";
					Db.NonQ32(command);
					command="ALTER TABLE patient ADD DateTStamp TimeStamp";
					Db.NonQ32(command);
					command="UPDATE patient SET DateTStamp=NOW()";
					Db.NonQ32(command);
					command="ALTER TABLE provider ADD DateTStamp TimeStamp";
					Db.NonQ32(command);
					command="UPDATE provider SET DateTStamp=NOW()";
					Db.NonQ32(command);
					command="ALTER TABLE appointment ADD DateTStamp TimeStamp";
					Db.NonQ32(command);
					command="UPDATE appointment SET DateTStamp=NOW()";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS deletedobject";
					Db.NonQ32(command);
					command=@"CREATE TABLE deletedobject (
						DeletedObjectNum int NOT NULL auto_increment,
						ObjectNum int NOT NULL,
						ObjectType int NOT NULL,
						DateTStamp TimeStamp,
						PRIMARY KEY (DeletedObjectNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command="ALTER TABLE schedule ADD DateTStamp TimeStamp";
					Db.NonQ32(command);
					command="UPDATE schedule SET DateTStamp=NOW()";
					Db.NonQ32(command);
					command="ALTER TABLE operatory ADD DateTStamp TimeStamp";
					Db.NonQ32(command);
					command="UPDATE operatory SET DateTStamp=NOW()";
					Db.NonQ32(command);
					command="ALTER TABLE recall ADD DateTStamp TimeStamp";
					Db.NonQ32(command);
					command="UPDATE recall SET DateTStamp=NOW()";
					Db.NonQ32(command);
					command="ALTER TABLE procedurecode ADD DateTStamp TimeStamp";
					Db.NonQ32(command);
					command="UPDATE procedurecode SET DateTStamp=NOW()";
					Db.NonQ32(command);
					command="ALTER TABLE insplan ADD IsHidden tinyint NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE carrier ADD IsHidden tinyint NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE insplan ADD INDEX (CarrierNum)";
					try {
						Db.NonQ32(command);
					}
					catch {
					}
					try {
						//this functionality is also duplicated in SheetUtil.GetImagePath, but we try very hard not to use external routines during conversions.
						/*
						if(!PrefC.UsingAtoZfolder) {
							throw new ApplicationException("Must be using AtoZ folders.");
						}
						string imagePath=ODFileUtils.CombinePaths(FormPath.GetPreferredImagePath(),"SheetImages");
						if(!Directory.Exists(imagePath)) {
							Directory.CreateDirectory(imagePath);
						}
						Properties.Resources.Med_History.Save(ODFileUtils.CombinePaths(imagePath,"Med History.gif"));
						Properties.Resources.Patient_Info.Save(ODFileUtils.CombinePaths(imagePath,"Patient Info.gif"));*/
					}
					catch{
					}
					command="DELETE FROM preference WHERE PrefName='ShowNotesInAccount'";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('AllowSettingProcsComplete','0','')";
					Db.NonQ32(command);
				} 
				else {//oracle
					
				}
				command="UPDATE preference SET ValueString = '5.9.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_0_1();
		}

		private static void To6_0_1() {
			if(FromVersion<new Version("6.0.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.0.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('RecallEmailSubject','Continuing Care Reminder','')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('RecallStatusMailed','0','FK to definition.DefNum')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('RecallStatusEmailed','0','FK to definition.DefNum')";
					Db.NonQ32(command);
					command="ALTER TABLE toothinitial ADD DrawingSegment text";
					Db.NonQ32(command);
					command="ALTER TABLE toothinitial ADD ColorDraw int NOT NULL";
					Db.NonQ32(command);
					//Dolphin bridge------------------------------------------------------------------------------------
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'Dolphin', "
						+"'Dolphin from dolphinimaging.com', "
						+"'0', "
						+"'"+POut.String(@"C:\Dolphin\")+"', "
						+"'', "
						+"'The path is to a folder rather than to a specific file.  Filename property refers to the input filename used to transer data.')";
					int programNum=Db.NonQ32(command,true);//we now have a ProgramNum to work with
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+programNum.ToString()+"', "
						+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
						+"'0')";
					Db.NonQ32(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+programNum.ToString()+"', "
						+"'Filename', "
						+"'"+POut.String(@"C:\Dolphin\Import\Import.txt")+"')";
					Db.NonQ32(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"'"+POut.Long(programNum)+"', "
						+"'"+POut.Long((int)ToolBarsAvail.ChartModule)+"', "
						+"'Dolphin')";
					Db.NonQ32(command);
					command="ALTER TABLE appointment ADD DateTimeArrived DateTime NOT NULL";
					Db.NonQ32(command);
					command="UPDATE appointment SET DateTimeArrived = '0001-01-01'";
					Db.NonQ32(command);
					command="ALTER TABLE appointment ADD DateTimeSeated DateTime NOT NULL";
					Db.NonQ32(command);
					command="UPDATE appointment SET DateTimeSeated = '0001-01-01'";
					Db.NonQ32(command);
					command="ALTER TABLE appointment ADD DateTimeDismissed DateTime NOT NULL";
					Db.NonQ32(command);
					command="UPDATE appointment SET DateTimeDismissed = '0001-01-01'";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('AppointmentTimeArrivedTrigger','0','FK to definition.DefNum, Category ApptConfirmed.  0 indicates no trigger.')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('AppointmentTimeSeatedTrigger','0','FK to definition.DefNum, Category ApptConfirmed.  0 indicates no trigger.')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('AppointmentTimeDismissedTrigger','0','FK to definition.DefNum, Category ApptConfirmed.  0 indicates no trigger.')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('ApptModuleRefreshesEveryMinute','1','Keeps the waiting room indicator times current.')";
					Db.NonQ32(command);
					//RECALL---------------------------------------------------------------------------------------
					command="ALTER TABLE recall ADD RecallTypeNum int NOT NULL";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS recalltype";
					Db.NonQ32(command);
					command=@"CREATE TABLE recalltype (
						RecallTypeNum int NOT NULL auto_increment,
						Description varchar(255),
						DefaultInterval int NOT NULL,
						TimePattern varchar(255),
						Procedures varchar(255),
						PRIMARY KEY (RecallTypeNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS recalltrigger";
					Db.NonQ32(command);
					command=@"CREATE TABLE recalltrigger (
						RecallTriggerNum int NOT NULL auto_increment,
						RecallTypeNum int NOT NULL,
						CodeNum int NOT NULL,
						PRIMARY KEY (RecallTriggerNum),
						INDEX (CodeNum),
						INDEX (RecallTypeNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					//Basic recall-----------------------------------------------------------------------------
					command="SELECT ValueString FROM preference WHERE PrefName='RecallPattern'";
					string timepattern=Db.GetCount(command);
					command="SELECT ValueString FROM preference WHERE PrefName='RecallProcedures'";
					string procs=Db.GetCount(command);
					command="INSERT INTO recalltype(RecallTypeNum,Description,DefaultInterval,TimePattern,Procedures) "
						+"VALUES(1,'Prophy',"
						+"393216,"//six months
						+"'"+timepattern+"',"//always / and X, so no need to parameterize
						+"'"+POut.String(procs)+"')";
					Db.NonQ32(command);
					command="SELECT CodeNum FROM procedurecode WHERE SetRecall=1";
					DataTable table=Db.GetTable(command);
					for(int i=0;i<table.Rows.Count;i++){
						command="INSERT INTO recalltrigger(RecallTypeNum,CodeNum) VALUES(1,"+table.Rows[i][0].ToString()+")";
						Db.NonQ32(command);
					}
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('RecallTypeSpecialProphy','1','FK to recalltype.RecallTypeNum.')";
					Db.NonQ32(command);
					//Child recall-----------------------------------------------------------------------------
					command="SELECT ValueString FROM preference WHERE PrefName='RecallPatternChild'";
					timepattern=Db.GetCount(command);
					command="SELECT ValueString FROM preference WHERE PrefName='RecallProceduresChild'";
					procs=Db.GetCount(command);
					command="INSERT INTO recalltype(RecallTypeNum,Description,DefaultInterval,TimePattern,Procedures) "
						+"VALUES(2,'Child Prophy',"
						+"0,"
						+"'"+timepattern+"',"//always / and X, so no need to parameterize
						+"'"+POut.String(procs)+"')";
					Db.NonQ32(command);
					//no triggers
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('RecallTypeSpecialChildProphy','2','FK to recalltype.RecallTypeNum.')";
					Db.NonQ32(command);
					//Perio recall-----------------------------------------------------------------------------
					command="SELECT ValueString FROM preference WHERE PrefName='RecallPatternPerio'";
					timepattern=Db.GetCount(command);
					command="SELECT ValueString FROM preference WHERE PrefName='RecallProceduresPerio'";
					procs=Db.GetCount(command);
					command="INSERT INTO recalltype(RecallTypeNum,Description,DefaultInterval,TimePattern,Procedures) "
						+"VALUES(3,'Perio',"
						+"262144,"//4 months.
						+"'"+timepattern+"',"//always / and X, so no need to parameterize
						+"'"+POut.String(procs)+"')";
					Db.NonQ32(command);
					command="SELECT ValueString FROM preference WHERE PrefName='RecallPerioTriggerProcs'";
					string triggerStr=Db.GetCount(command);
					List<string> perioCodeNums=new List<string>();
					string codeNum;
					if(triggerStr!=""){
						string[] triggerArray=triggerStr.Split(',');
						for(int i=0;i<triggerArray.Length;i++){
							command="SELECT CodeNum FROM procedurecode WHERE ProcCode='"+POut.String(triggerArray[i])+"'";
							table=Db.GetTable(command);
							if(table.Rows.Count==0){
								continue;
							}
							codeNum=table.Rows[0][0].ToString();
							perioCodeNums.Add(codeNum);
							command="INSERT INTO recalltrigger(RecallTypeNum,CodeNum) VALUES(3,"+codeNum+")";
							Db.NonQ32(command);
						}
					}
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('RecallTypeSpecialPerio','3','FK to recalltype.RecallTypeNum.')";
					Db.NonQ32(command);
					if(CultureInfo.CurrentCulture.Name=="en-US"){
						//4BWX-----------------------------------------------------------------------------
						timepattern="";
						procs="D0274";
						command="INSERT INTO recalltype(RecallTypeNum,Description,DefaultInterval,TimePattern,Procedures) "
							+"VALUES(4,'4BW',"
							+"16777216,"//one year.
							+"'"+timepattern+"',"
							+"'"+POut.String(procs)+"')";
						Db.NonQ32(command);
						command="SELECT CodeNum FROM procedurecode WHERE ProcCode='D0274'";
						table=Db.GetTable(command);
						if(table.Rows.Count>0){
							codeNum=table.Rows[0][0].ToString();
							command="INSERT INTO recalltrigger(RecallTypeNum,CodeNum) VALUES(4,"+codeNum+")";
							Db.NonQ32(command);
						}
						//Pano-----------------------------------------------------------------------------
						timepattern="";
						procs="D0330";
						command="INSERT INTO recalltype(RecallTypeNum,Description,DefaultInterval,TimePattern,Procedures) "
							+"VALUES(5,'Pano',"
							+"83886080,"//5 years.
							+"'"+timepattern+"',"
							+"'"+POut.String(procs)+"')";
						Db.NonQ32(command);
						command="SELECT CodeNum FROM procedurecode WHERE ProcCode='D0330'";
						table=Db.GetTable(command);
						if(table.Rows.Count>0){
							codeNum=table.Rows[0][0].ToString();
							command="INSERT INTO recalltrigger(RecallTypeNum,CodeNum) VALUES(5,"+codeNum+")";
							Db.NonQ32(command);
						}
					}
					//Set existing recall objects to new types--------------------------------------------------
					command="UPDATE recall SET RecallTypeNum=1 WHERE RecallTypeNum=0";
					Db.NonQ32(command);
					for(int i=0;i<perioCodeNums.Count;i++){
						command="UPDATE recall SET RecallTypeNum=3 WHERE EXISTS("
							+"SELECT * FROM procedurelog WHERE procedurelog.PatNum=recall.PatNum "
							+"AND procedurelog.CodeNum="+perioCodeNums[i]+" "
							+"AND procedurelog.ProcStatus=2)";//complete
						Db.NonQ32(command);
					}
					//an automatic synch would violate the rule of not calling external methods.
					//Recalls.SynchAllPatients();
					using MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste("When this conversion is done, you will also need to resynchronize all patient recalls from inside the Setup | Recall Types window.");
					msgbox.TopMost=true;
					msgbox.ShowDialog();
					//Get rid of unused prefs-----------------------------------------------------------------
					command="DELETE FROM preference WHERE PrefName='RecallPattern'";
					Db.NonQ32(command);
					command="DELETE FROM preference WHERE PrefName='RecallProcedures'";
					Db.NonQ32(command);
					command="DELETE FROM preference WHERE PrefName='RecallPatternChild'";
					Db.NonQ32(command);
					command="DELETE FROM preference WHERE PrefName='RecallProceduresChild'";
					Db.NonQ32(command);
					command="ALTER TABLE procedurecode DROP SetRecall";
					Db.NonQ32(command);
					command="ALTER TABLE procedurecode DROP RemoveTooth";
					Db.NonQ32(command);
					command="DELETE FROM preference WHERE PrefName='RecallBW'";
					Db.NonQ32(command);
					command="DELETE FROM preference WHERE PrefName='RecallFMXPanoProc'";
					Db.NonQ32(command);
					command="DELETE FROM preference WHERE PrefName='RecallDisableAutoFilms'";
					Db.NonQ32(command);
					command="DELETE FROM preference WHERE PrefName='RecallFMXPanoYrInterval'";
					Db.NonQ32(command);
					command="DELETE FROM preference WHERE PrefName='RecallDisablePerioAlt'";
					Db.NonQ32(command);
					command="DELETE FROM preference WHERE PrefName='RecallPatternPerio'";
					Db.NonQ32(command);
					command="DELETE FROM preference WHERE PrefName='RecallProceduresPerio'";
					Db.NonQ32(command);
					command="DELETE FROM preference WHERE PrefName='RecallPerioTriggerProcs'";
					Db.NonQ32(command);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) VALUES (21,8,'Family Module Referral','',-2823993,0)";
					Db.NonQ32(command);
					command="ALTER TABLE payplan ADD CompletedAmt double NOT NULL";
					Db.NonQ32(command);
					command="UPDATE payplan SET CompletedAmt=(SELECT SUM(Principal) FROM payplancharge WHERE payplan.PayPlanNum=payplancharge.PayPlanNum)";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('ChartQuickAddHideAmalgam','0','')";
					Db.NonQ32(command);
				} 
				else {//oracle
					
				}
				command="UPDATE preference SET ValueString = '6.0.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_0_2();
		}

		private static void To6_0_2() {
			if(FromVersion<new Version("6.0.2.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.0.2");//No translation in convert script.
				string command;
				command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('RecallTypesShowingInList','1,3','Comma-delimited list. FK to recalltype.RecallTypeNum.')";//1=prophy,3=perio
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '6.0.2.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_1_1();
		}

		private static void To6_1_1() {
			if(FromVersion<new Version("6.1.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.1.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS plannedappt";
					Db.NonQ32(command);
					command=@"CREATE TABLE plannedappt (
						PlannedApptNum int NOT NULL auto_increment,
						PatNum int NOT NULL,
						AptNum int NOT NULL,
						ItemOrder int NOT NULL,
						PRIMARY KEY (PlannedApptNum),
						INDEX (PatNum),
						INDEX (AptNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command="SELECT PatNum,NextAptNum FROM patient WHERE NextAptNum != 0";
					DataTable table=Db.GetTable(command);
					for(int i=0;i<table.Rows.Count;i++) {
						command="INSERT INTO plannedappt (PatNum,AptNum,ItemOrder) VALUES("
							+table.Rows[i]["PatNum"].ToString()+","
							+table.Rows[i]["NextAptNum"].ToString()+",0)";
						Db.NonQ32(command);
					}
					command="ALTER TABLE patient DROP NextAptNum";
					Db.NonQ32(command);
					//Billing charges------------------------------------------------------------------------------------------
					command="INSERT INTO preference (PrefName, ValueString,Comments) VALUES ('BillingChargeOrFinanceIsDefault', 'Finance','Value is a string, either Billing or Finance.')";
					Db.NonQ32(command);
					command="SELECT Max(ItemOrder) FROM definition WHERE Category=1";
					table=Db.GetTable(command);
					int billingchargeItemOrder=PIn.Int(table.Rows[0][0].ToString())+1;
					command="INSERT INTO definition (category,itemorder,itemname,itemvalue) VALUES("
						+"1, "//category=AdjTypes
						+"'"+POut.Long(billingchargeItemOrder)+"', "//itemOrder
						+"'Billing Charge', "//itemname
						+"'+')";//itemValue
					int numAdj=Db.NonQ32(command,true);
					command="INSERT INTO preference (PrefName, ValueString,Comments) VALUES ('BillingChargeAdjustmentType', "
						+"'"+POut.Long(numAdj)+"','')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName, ValueString,Comments) VALUES ('BillingChargeLastRun', '0001-01-01','')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName, ValueString,Comments) VALUES ('BillingChargeAmount', '2','')";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS scheduleop";
					Db.NonQ32(command);
					command=@"CREATE TABLE scheduleop (
						ScheduleOpNum int NOT NULL auto_increment,
						ScheduleNum int NOT NULL,
						OperatoryNum int NOT NULL,
						PRIMARY KEY (ScheduleOpNum),
						INDEX (ScheduleNum),
						INDEX (OperatoryNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					//conversion to new operatory paradigm for blockouts-------------------------------------------------------------------
					//get all visible ops
					command="SELECT OperatoryNum FROM operatory WHERE IsHidden=0";
					table=Db.GetTable(command);
					List<int> visibleOps=new List<int>();
					for(int i=0;i<table.Rows.Count;i++) {
						visibleOps.Add(PIn.Int(table.Rows[i]["OperatoryNum"].ToString()));
					}
					//convert blockouts with op=0
					command="SELECT ScheduleNum FROM schedule WHERE SchedType=2 "//blockout
						+"AND Op=0";//indicates all ops
					table=Db.GetTable(command);
					for(int i=0;i<table.Rows.Count;i++) {
						//for each schedule, we need to insert a separate scheduleop for each visible op
						for(int o=0;o<visibleOps.Count;o++) {
							command="INSERT INTO scheduleop(ScheduleNum,OperatoryNum) VALUES("
								+table.Rows[i]["ScheduleNum"].ToString()+","
								+POut.Long(visibleOps[o])+")";
							Db.NonQ32(command);
						}
					}
					//convert blockouts with op>0
					command="SELECT ScheduleNum,Op FROM schedule WHERE SchedType=2 "//blockout
						+"AND Op>0";//indicates one assigned op
					table=Db.GetTable(command);
					for(int i=0;i<table.Rows.Count;i++) {
						command="INSERT INTO scheduleop(ScheduleNum,OperatoryNum) VALUES("
							+table.Rows[i]["ScheduleNum"].ToString()+","
							+table.Rows[i]["Op"].ToString()+")";
						Db.NonQ32(command);
					}
					command="ALTER TABLE schedule DROP Op";
					Db.NonQ32(command);
					//Fee schedule name conversion-------------------------------------------------------------------------------
					command="DROP TABLE IF EXISTS feesched";
					Db.NonQ32(command);
					command=@"CREATE TABLE feesched (
						FeeSchedNum int NOT NULL auto_increment,
						Description varchar(255),
						FeeSchedType int NOT NULL,
						ItemOrder int NOT NULL,
						IsHidden tinyint(1) NOT NULL,
						PRIMARY KEY (FeeSchedNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command="SELECT DefNum,ItemName,ItemValue,ItemOrder,IsHidden FROM definition WHERE Category=7";//fee schedule names
					table=Db.GetTable(command);
					for(int i=0;i<table.Rows.Count;i++) {
						command="INSERT INTO feesched(FeeSchedNum,Description,FeeSchedType,ItemOrder,IsHidden) VALUES("
							+table.Rows[i]["DefNum"].ToString()+","
							+"'"+POut.String(table.Rows[i]["ItemName"].ToString())+"',";
						if(table.Rows[i]["ItemValue"].ToString()=="A") {
							command+=POut.Long((int)FeeScheduleType.OutNetwork)+",";
						}
						else if(table.Rows[i]["ItemValue"].ToString()=="C") {
							command+=POut.Long((int)FeeScheduleType.CoPay)+",";
						}
						else {
							command+=POut.Long((int)FeeScheduleType.Normal)+",";
						}
						command+=table.Rows[i]["ItemOrder"].ToString()+","//although this will be reset in the UI
							+table.Rows[i]["IsHidden"].ToString()+")";
						Db.NonQ32(command);
					}
					command="DELETE FROM definition WHERE Category=7";
					Db.NonQ32(command);
					//end of fee schedule
					command="INSERT INTO preference (PrefName, ValueString,Comments) VALUES ('AllowedFeeSchedsAutomate','0','0 or 1')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName, ValueString,Comments) VALUES ('BackupReminderLastDateRun','0001-01-01','')";
					Db.NonQ32(command);
					//Anesthesia Module Conversions-----------------------------------------------------
					//field to toggle Anesthesia Module on or off. Turned 'off' by default
					command = "INSERT INTO preference (PrefName, ValueString,Comments) VALUES ('EnableAnesthMod', '0','0 or 1, Toggles Anesthesia Module Off and On. Disabled (0) by default')";
					Db.NonQ32(command);
					//individual unique records of delivered anesthetics
					command="DROP TABLE IF EXISTS anestheticrecord";
					Db.NonQ32(command);
					command = @"CREATE TABLE anestheticrecord (
						AnestheticRecordNum int(11) NOT NULL auto_increment,
						PatNum int(11) NOT NULL,
						AnestheticDate datetime NOT NULL,
						ProvNum smallint(5) NOT NULL,
						PRIMARY KEY (AnestheticRecordNum),
						INDEX (PatNum),
						INDEX (ProvNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					//data recorded for an individual anesthetic on a given date and time
					command="DROP TABLE IF EXISTS anestheticdata";
					Db.NonQ32(command);
					command = @"CREATE TABLE anestheticdata (
						AnestheticDataNum int(11) NOT NULL auto_increment,
						AnestheticRecordNum int(11) NOT NULL,
						AnesthOpen char(32),
						AnesthClose char(32),
						SurgOpen char(32),
						SurgClose char(32),
						Anesthetist char(32) NOT NULL,
						Surgeon char(32) NOT NULL,						
						Asst char(32) NOT NULL,
						Circulator char(32) NOT NULL,						
						VSMName char(20) NOT NULL,
						VSMSerNum char(20) NOT NULL,					
						ASA char(3) NOT NULL,
						ASA_EModifier char(1) NOT NULL,					
						InhO2 tinyint(1) NOT NULL,
						InhN2O tinyint(1) NOT NULL,						
						O2LMin smallint(1) NOT NULL,
						N2OLMin smallint(1) NOT NULL,						
						RteNasCan tinyint(1) NOT NULL,
						RteNasHood tinyint(1) NOT NULL,
						RteETT tinyint(1) NOT NULL,
						MedRouteIVCath tinyint(1) NOT NULL,
						MedRouteIVButtFly tinyint(1) NOT NULL,
						MedRouteIM tinyint(1) NOT NULL,
						MedRoutePO tinyint(1) NOT NULL,
						MedRouteNasal tinyint(1) NOT NULL,
						MedRouteRectal tinyint(1) NOT NULL,
						IVSite char(16) NOT NULL,
						IVGauge smallint(2) NOT NULL,
						IVSideR smallint(2) NOT NULL,
						IVSideL smallint(2) NOT NULL,
						IVAtt smallint(1) NOT NULL,
						IVF char(8) NOT NULL,
						IVFVol int(5) NOT NULL,
						MonBP tinyint(1) NOT NULL,
						MonSpO2 tinyint(1)NOT NULL,
						MonEtCO2 tinyint(1) NOT NULL,
						MonTemp tinyint(1) NOT NULL,
						MonPrecordial tinyint(1) NOT NULL,
						MonEKG tinyint(1) NOT NULL,
						Notes text NOT NULL,
						PatWgt smallint(3) NOT NULL,
						WgtUnitsLbs tinyint(1) NOT NULL,
						WgtUnitsKgs tinyint(1) NOT NULL,
						PatHgt char(10) NOT NULL,
						EscortName char(32) NOT NULL,
						EscortCellNum char(13) NOT NULL,
						EscortRel char(16) NOT NULL,
						NPOTime char(5) NOT NULL,
						SigIsTopaz tinyint(3) NOT NULL,
						Signature text NOT NULL,
						PRIMARY KEY (AnestheticDataNum),
						INDEX (AnestheticRecordNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					//a list of anesthetic medications to be delivered to a patient
					command="DROP TABLE IF EXISTS anesthmedsgiven";
					Db.NonQ32(command);
					command = @"CREATE TABLE anesthmedsgiven(
						AnestheticMedNum int(3) NOT NULL auto_increment,
						AnestheticRecordNum int(11) NOT NULL,
						AnesthMed char (20) NOT NULL,
						QtyGiven int(4) NOT NULL,
						QtyWasted int(4) NOT NULL,
						DoseTimeStamp datetime NOT NULL,
						PRIMARY KEY (AnestheticMedNum),
						INDEX (AnestheticMedNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					//a list of DEA scheduled anesthetic medications taken into inventory from a Supplier. Qtys are always in milliLiters so inventory count works properly.
					command="DROP TABLE IF EXISTS anesthmedsintake";
					Db.NonQ32(command);
					command = @"CREATE TABLE anesthmedsintake(
						AnestheticMedNum int(3) NOT NULL auto_increment,
						IntakeDate datetime NOT NULL,
						AnestheticMed char (20) NOT NULL,
						DEASchedule char(2)NOT NULL,
						Qty int(6) NOT NULL, 
						SupplierIDNum char(11) NOT NULL,
						InvoiceNum char(20) NOT NULL,
						PRIMARY KEY (AnestheticMedNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					//fields required to create inventory of anesthetic medications
					command="DROP TABLE IF EXISTS anesthmedsinventory";
					Db.NonQ32(command);
					command = @"CREATE TABLE anesthmedsinventory (
						AnestheticMedNum int(3) NOT NULL auto_increment,
						AnestheticMed char(20) NOT NULL,
						AnesthHowSupplied char(20) NOT NULL,
						QtyOnHand int(5) NOT NULL,
						PRIMARY KEY (AnestheticMedNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					//fields required to adjust inventory of anesthetic medications
					command="DROP TABLE IF EXISTS anesthmedsinventoryadj";
					Db.NonQ32(command);
					command = @"CREATE TABLE anesthmedsinventoryadj (
						AnestheticMedNum int(3) NOT NULL auto_increment,
						AdjPos int(4) NOT NULL,
						AdjNeg int(4)NOT NULL,
						Provider char(4) NOT NULL,
						Notes text NOT NULL,
						TimeStamp datetime NOT NULL,
						PRIMARY KEY (AnestheticMedNum),
						INDEX (AnestheticMedNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					//a list of suppliers of anesthetic medications
					command="DROP TABLE IF EXISTS anesthmedsuppliers";
					Db.NonQ32(command);
					command = @"CREATE TABLE anesthmedsuppliers (
						SupplierIDNum int(3) NOT NULL auto_increment,
						SupplierName char(32) NOT NULL,
						Addr1 char(32)NOT NULL,
						Addr2 char(32) NOT NULL,
						City char(32) NOT NULL,
						State char(10) NOT NULL,
						Country char(32) NOT NULL,
						Phone char(12) NOT NULL,
						Fax char(12) NOT NULL,
						PhoneExt int(5) NOT NULL,
						Contact char(32) NOT NULL,
						Notes text NOT NULL,
						PRIMARY KEY (SupplierIDNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					//keeps the post-anesthesia score and discharge data
					command="DROP TABLE IF EXISTS anesthscore";
					Db.NonQ32(command);
					command = @"CREATE TABLE anesthscore (
						AnestheticRecordNum int(7) NOT NULL auto_increment,
						QActivity smallint(1) NOT NULL,
						QResp smallint(1) NOT NULL,
						QCirc smallint(1) NOT NULL,
						QConc smallint(1) NOT NULL,
						QColor smallint(1) NOT NULL,
						AnesthScore smallint(2) NOT NULL,
						DischAmb tinyint(1) NOT NULL,
						DischWheelChr tinyint(1) NOT NULL,
						DischAmbulance tinyint(1) NOT NULL,
						DischCondStable tinyint(1) NOT NULL,
						PRIMARY KEY (AnestheticRecordNum),
						INDEX (AnestheticRecordNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					//keeps data auto-imported from networkable vital sign monitors
					command="DROP TABLE IF EXISTS anesthvsdata";
					Db.NonQ32(command);
					command = @"CREATE TABLE anesthvsdata (
						AnestheticRecordNum int(7) NOT NULL auto_increment,
						VSMName char(20) NOT NULL,
						VSMSerNum char(20) NOT NULL,
						BPSys int(3) NOT NULL,
						BPDias int(3) NOT NULL,
						BPMAP int(3) NOT NULL,
						HR int(3) NOT NULL,
						SpO2 int(3) NOT NULL,
						EtCo2 int(3) NOT NULL,
						Temp int(3) NOT NULL,
						PRIMARY KEY (AnestheticRecordNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
				}
				else {//oracle

				}
				command="UPDATE preference SET ValueString = '6.1.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_1_8();
		}

		private static void To6_1_8() {
			if(FromVersion<new Version("6.1.8.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.1.8");//No translation in convert script.
				string command="UPDATE userod SET IsHidden=0 WHERE IsHidden=1 "
					+"AND EXISTS(SELECT * FROM grouppermission "
					+"WHERE PermType='"+POut.Long((int)Permissions.SecurityAdmin)+"' "//24
					+"AND grouppermission.UserGroupNum=userod.UserGroupNum)";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '6.1.8.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_2_1();
		}

		private static void To6_2_1() {
			if(FromVersion<new Version("6.2.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.2.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command = "DROP TABLE IF EXISTS anesthmedsintake";
					Db.NonQ32(command);
					command = @"CREATE TABLE anesthmedsintake(
						AnestheticMedNum int(3) NOT NULL auto_increment,
						IntakeDate datetime NOT NULL,
						AnestheticMed char (20) NOT NULL,
						DEASchedule char(2),
						Qty int(6) NOT NULL, 
						SupplierIDNum char(11) NOT NULL,
						InvoiceNum char(20) NOT NULL,
						PRIMARY KEY (AnestheticMedNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command = "DROP TABLE IF EXISTS anesthmedsinventoryadj";
					Db.NonQ32(command);
					command = @"CREATE TABLE anesthmedsinventoryadj (
						AdjustNum int(11) NOT NULL auto_increment,
						AnestheticMedNum int(11) NOT NULL,
						QtyAdj double,
						UserNum int(11) NOT NULL,
						Notes varchar(255),
						TimeStamp datetime NOT NULL,
						PRIMARY KEY (AdjustNum),
						INDEX (AnestheticMedNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS anestheticdata";
					Db.NonQ32(command);
					command = @"CREATE TABLE anestheticdata (
						AnestheticDataNum int(11) NOT NULL auto_increment,
						AnestheticRecordNum int(11) NOT NULL,
						AnesthOpen char(32),
						AnesthClose char(32),
						SurgOpen char(32),
						SurgClose char(32),
						Anesthetist char(32),
						Surgeon char(32),						
						Asst char(32),
						Circulator char(32),						
						VSMName char(20),
						VSMSerNum char(20),					
						ASA char(3),
						ASA_EModifier char(1),										
						O2LMin smallint(1),
						N2OLMin smallint(1),						
						RteNasCan tinyint(1),
						RteNasHood tinyint(1),
						RteETT tinyint(1),
						MedRouteIVCath tinyint(1),
						MedRouteIVButtFly tinyint(1),
						MedRouteIM tinyint(1),
						MedRoutePO tinyint(1),
						MedRouteNasal tinyint(1),
						MedRouteRectal tinyint(1),
						IVSite char(20),
						IVGauge smallint(2),
						IVSideR smallint(2),
						IVSideL smallint(2),
						IVAtt smallint(1),
						IVF char(20),
						IVFVol float(5),
						MonBP tinyint(1),
						MonSpO2 tinyint(1),
						MonEtCO2 tinyint(1),
						MonTemp tinyint(1),
						MonPrecordial tinyint(1),
						MonEKG tinyint(1),
						Notes text,
						PatWgt smallint(3),
						WgtUnitsLbs tinyint(1),
						WgtUnitsKgs tinyint(1),
						PatHgt smallint(3),
						EscortName char(32),
						EscortCellNum char(13),
						EscortRel char(16),
						NPOTime char(5),
						HgtUnitsIn tinyint (1),
						HgtUnitsCm tinyint (1),
						Signature text,
						SigIsTopaz tinyint unsigned default '0',
						PRIMARY KEY (AnestheticDataNum),
						INDEX (AnestheticRecordNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command = "DROP TABLE IF EXISTS anesthmedsuppliers";
					Db.NonQ32(command);
					command = @"CREATE TABLE anesthmedsuppliers (
						SupplierIDNum int(3) NOT NULL auto_increment,
						SupplierName varchar(255) NOT NULL,
						Phone char(13),
						PhoneExt char(6),
						Fax char(13),
						Addr1 varchar(48),
						Addr2 char(32),
						City varchar(48),
						State char(20),
						Zip char(10),
						Contact char(32),
						WebSite varchar(48),
						Notes text,
						PRIMARY KEY (SupplierIDNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					string[] commands = new string[]{
						"ALTER table userod ADD AnesthProvType int(2) default '3' NOT NULL"
						,"ALTER table anesthmedsinventory CHANGE AnestheticMedNum AnestheticMedNum int NOT NULL auto_increment"
						,"ALTER table anesthmedsinventory CHANGE AnestheticMed AnesthMedName char(30)"
						,"ALTER table anesthmedsinventory CHANGE QtyOnHand QtyOnHand double default '0'"
						,"ALTER table anesthmedsinventory ADD DEASchedule char(3)"
						,"ALTER table anesthmedsintake DROP DEASchedule"
						,"ALTER table anesthmedsintake CHANGE AnestheticMed AnesthMedName char(32)"
						,"ALTER table anesthmedsgiven CHANGE QtyGiven QtyGiven double"
						,"ALTER table anesthmedsgiven CHANGE QtyWasted QtyWasted double"
						,"ALTER table anesthmedsgiven CHANGE AnesthMed AnesthMedName char(32)"
						,"ALTER table anesthmedsgiven CHANGE DoseTimeStamp DoseTimeStamp char(32)"
						,"ALTER table anesthmedsgiven ADD QtyOnHandOld double"
					};
					Db.NonQ32(commands);
					command = "DROP TABLE IF EXISTS anesthscore";
					Db.NonQ32(command);
					command = @"CREATE TABLE anesthscore (
						AnesthScoreNum int(11) NOT NULL auto_increment,
						AnestheticRecordNum int(11),
						QActivity smallint(1),
						QResp smallint(1),
						QCirc smallint(1),
						QConc smallint(1),
						QColor smallint(1),
						AnesthesiaScore smallint(2),
						DischAmb tinyint(1),
						DischWheelChr tinyint(1),
						DischAmbulance tinyint(1),
						DischCondStable tinyint(1),
						DischCondUnStable tinyint(1),
						PRIMARY KEY (AnesthScoreNum),
						INDEX (AnestheticRecordNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					//keeps data auto-imported from networkable vital sign monitors
					command = "DROP TABLE IF EXISTS anesthvsdata";
					Db.NonQ32(command);
					command = @"CREATE TABLE anesthvsdata (
						AnesthVSDataNum int(11) NOT NULL auto_increment,
						AnestheticRecordNum int(11) NOT NULL,
						PatNum int(11),
						VSMName char(20),
						VSMSerNum char(32),
						BPSys smallint(3),
						BPDias smallint(3),
						BPMAP smallint(3),
						HR smallint(3),
						SpO2 smallint(3),
						EtCo2 smallint(3),
						Temp smallint(3),
						VSTimeStamp char(32),
						PRIMARY KEY (AnesthVSDataNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
					command="ALTER TABLE schedule ADD INDEX (EmployeeNum)";
					Db.NonQ32(command);
					command="ALTER TABLE schedule ADD INDEX (ProvNum)";
					Db.NonQ32(command);
					command="ALTER TABLE schedule ADD INDEX (SchedDate)";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName, ValueString,Comments) VALUES ('SecurityLockDate','0001-01-01','If present, global lock on procedures, payments, insurance payments, and adjustments.  Prevents editing old entries and backdating entries.')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName, ValueString,Comments) VALUES ('SecurityLockIncludesAdmin','0','0 or 1.  If 1, administrators are also locked out by date.')";
					Db.NonQ32(command);
					command="ALTER TABLE patient ADD ResponsParty int NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE patient ADD INDEX (ResponsParty)";
					Db.NonQ32(command);
					command="ALTER TABLE treatplan ADD ResponsParty int NOT NULL";
					Db.NonQ32(command);
				}
				else {//oracle

				}
				command="UPDATE preference SET ValueString = '6.2.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_2_2();
		}

		private static void To6_2_2() {
			if(FromVersion<new Version("6.2.2.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.2.2");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS phonenumber";
					Db.NonQ32(command);
					command=@"CREATE TABLE phonenumber (
						PhoneNumberNum int NOT NULL auto_increment,
						PatNum int NOT NULL,
						PhoneNumberVal varchar(255),
						PRIMARY KEY (PhoneNumberNum),
						INDEX (PatNum),
						INDEX (PhoneNumberVal)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
				}
				else {

				}
				command="UPDATE preference SET ValueString = '6.2.2.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_2_9();
		}
		


	}

}