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
//using System.Windows.Forms;
//using OpenDentBusiness;
using CodeBase;
using DataConnectionBase;

namespace OpenDentBusiness {
	//The other file was simply getting too big.  It was bogging down VS speed.
	///<summary></summary>
	public partial class ConvertDatabases {

		private static void To6_2_9() {
			if(FromVersion<new Version("6.2.9.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.2.9");//No translation in convert script.
				string command="ALTER TABLE fee CHANGE FeeSched FeeSched int NOT NULL";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '6.2.9.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_3_1();
		}

		private static void To6_3_1() {
			if(FromVersion<new Version("6.3.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.3.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName, ValueString,Comments) VALUES ('MobileSyncPath','','')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName, ValueString,Comments) VALUES ('MobileSyncLastFileNumber','0','')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName, ValueString,Comments) VALUES ('MobileSyncDateTimeLastRun','0001-01-01','')";
					Db.NonQ32(command);
					//I had originally deleted these.  But decided instead to just comment them as obsolete because I think it caused a bug in our upgrade.
					command="UPDATE preference SET Comments = 'Obsolete' WHERE PrefName = 'LettersIncludeReturnAddress'";
					Db.NonQ32(command);
					command="UPDATE preference SET Comments = 'Obsolete' WHERE PrefName ='StationaryImage'";
					Db.NonQ32(command);
					command="UPDATE preference SET Comments = 'Obsolete' WHERE PrefName ='StationaryDocument'";
					Db.NonQ32(command);
				}
				else {//oracle

				}
				command="UPDATE preference SET ValueString = '6.3.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_3_3();
		}

		private static void To6_3_3() {
			if(FromVersion<new Version("6.3.3.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.3.3");//No translation in convert script.
				string command="INSERT INTO preference (PrefName, ValueString,Comments) VALUES ('CoPay_FeeSchedule_BlankLikeZero','1','1 to treat blank entries like zero copay.  0 to make patient responsible on blank entries.')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '6.3.3.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_3_4();
		}

		private static void To6_3_4() {
			if(FromVersion<new Version("6.3.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.3.4");//No translation in convert script.
				string command="ALTER TABLE sheetfielddef CHANGE FieldValue FieldValue text NOT NULL";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '6.3.4.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_4_1();
		}

		private static void To6_4_1() {
			if(FromVersion<new Version("6.4.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.4.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="UPDATE preference SET Comments = '-1 indicates min for all dates' WHERE PrefName = 'RecallDaysPast'";
					Db.NonQ32(command);
					command="UPDATE preference SET Comments = '-1 indicates max for all dates' WHERE PrefName = 'RecallDaysFuture'";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName, ValueString,Comments) VALUES ('RecallShowIfDaysFirstReminder','-1','-1 indicates do not show')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName, ValueString,Comments) VALUES ('RecallShowIfDaysSecondReminder','-1','-1 indicates do not show')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('RecallEmailMessage','You are due for your regular dental check-up on ?DueDate  Please call our office today to schedule an appointment.','')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('RecallEmailFamMsg','You are due for your regular dental check-up.  [FamilyList]  Please call our office today to schedule an appointment.','')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('RecallEmailSubject2','','')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('RecallEmailMessage2','','')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('RecallPostcardMessage2','','')";
					Db.NonQ32(command);
					string prefVal;
					DataTable table;
					command="SELECT ValueString FROM preference WHERE PrefName='RecallPostcardMessage'";
					table=Db.GetTable(command);
					prefVal=table.Rows[0][0].ToString().Replace("?DueDate","[DueDate]");
					command="UPDATE preference SET ValueString='"+POut.String(prefVal)+"' WHERE PrefName='RecallPostcardMessage'";
					Db.NonQ32(command);
					command="SELECT ValueString FROM preference WHERE PrefName='RecallPostcardFamMsg'";
					table=Db.GetTable(command);
					prefVal=table.Rows[0][0].ToString().Replace("?FamilyList","[FamilyList]");
					command="UPDATE preference SET ValueString='"+POut.String(prefVal)+"' WHERE PrefName='RecallPostcardFamMsg'";
					Db.NonQ32(command);
					command="SELECT ValueString FROM preference WHERE PrefName='ConfirmPostcardMessage'";
					table=Db.GetTable(command);
					prefVal=table.Rows[0][0].ToString().Replace("?date","[date]");
					prefVal=prefVal.Replace("?time","[time]");
					command="UPDATE preference SET ValueString='"+POut.String(prefVal)+"' WHERE PrefName='ConfirmPostcardMessage'";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('RecallEmailSubject3','','')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('RecallEmailMessage3','','')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('RecallPostcardMessage3','','')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('RecallEmailFamMsg2','','')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('RecallEmailFamMsg3','','')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('RecallPostcardFamMsg2','','')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('RecallPostcardFamMsg3','','')";
					Db.NonQ32(command);
					command="ALTER TABLE autonote CHANGE ControlsToInc MainText text";
					Db.NonQ32(command);
					command="UPDATE autonote SET MainText = ''";
					Db.NonQ32(command);
					command="UPDATE autonotecontrol SET ControlType='Text' WHERE ControlType='MultiLineTextBox'";
					Db.NonQ32(command);
					command="UPDATE autonotecontrol SET ControlType='OneResponse' WHERE ControlType='ComboBox'";
					Db.NonQ32(command);
					command="UPDATE autonotecontrol SET ControlType='Text' WHERE ControlType='TextBox'";
					Db.NonQ32(command);
					command="UPDATE autonotecontrol SET ControlOptions=MultiLineText WHERE MultiLineText != ''";
					Db.NonQ32(command);
					command="ALTER TABLE autonotecontrol DROP PrefaceText";
					Db.NonQ32(command);
					command="ALTER TABLE autonotecontrol DROP MultiLineText";
					Db.NonQ32(command);
				}
				else {//oracle

				}
				command="UPDATE preference SET ValueString = '6.4.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_4_4();
		}

		private static void To6_4_4() {
			if(FromVersion<new Version("6.4.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.4.4");//No translation in convert script.
				string command;
				//Convert comma-delimited autonote controls to carriage-return delimited.
				command="SELECT AutoNoteControlNum,ControlOptions FROM autonotecontrol";
				DataTable table=Db.GetTable(command);
				string newVal;
				for(int i=0;i<table.Rows.Count;i++) {
					newVal=table.Rows[i]["ControlOptions"].ToString();
					newVal=newVal.TrimEnd(',');
					newVal=newVal.Replace(",","\r\n");
					command="UPDATE autonotecontrol SET ControlOptions='"+POut.String(newVal)
						+"' WHERE AutoNoteControlNum="+table.Rows[i]["AutoNoteControlNum"].ToString();
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '6.4.4.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_5_1();
		}

		private static void To6_5_1() {
			if(FromVersion<new Version("6.5.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.5.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('ShowFeatureMedicalInsurance','0','')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('BillingUseElectronic','0','Set to 1 to used e-billing.')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('BillingElectVendorId','','')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('BillingElectVendorPMSCode','','')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('BillingElectCreditCardChoices','V,MC','Choices of V,MC,D,A comma delimited.')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('BillingElectClientAcctNumber','','')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('BillingElectUserName','','')";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('BillingElectPassword','','')";
					Db.NonQ32(command);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) VALUES(12,22,'Status Condition','',-8978432,0)";
					Db.NonQ32(command);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) VALUES(22,16,'Condition','',-5169880,0)";
					Db.NonQ32(command);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) VALUES(22,17,'Condition (light)','',-1678747,0)";
					Db.NonQ32(command);
					command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('BillingIgnoreInPerson','0','Set to 1 to ignore walkout statements.')";
					Db.NonQ32(command);
					//eClinicalWorks Bridge---------------------------------------------------------------------------
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'eClinicalWorks', "
						+"'eClinicalWorks from www.eclinicalworks.com', "
						+"'0', "
						+"'', "
						+"'', "
						+"'')";
					int programNum=Db.NonQ32(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'HL7FolderIn', "
						+"'')";
					Db.NonQ32(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'HL7FolderOut', "
						+"'')";
					Db.NonQ32(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'DefaultUserGroup', "
						+"'')";
					Db.NonQ32(command);
					command = "ALTER TABLE anesthmedsgiven ADD AnesthMedNum int NOT NULL";
					Db.NonQ32(command);
					command = "ALTER TABLE provider ADD AnesthProvType int NOT NULL";
					Db.NonQ32(command);
					command="DROP TABLE IF EXISTS hl7msg";
					Db.NonQ32(command);
					command=@"CREATE TABLE hl7msg (
						HL7MsgNum int NOT NULL auto_increment,
						HL7Status int NOT NULL,
						MsgText text,
						AptNum int NOT NULL,
						PRIMARY KEY (HL7MsgNum),
						INDEX (AptNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ32(command);
				}
				else {//oracle

				}
				command="UPDATE preference SET ValueString = '6.5.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_6_1();
		}

		private static void To6_6_1() {
			if(FromVersion<new Version("6.6.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.6.1");//No translation in convert script.
				string command;
				DataTable table;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					//Change defaults for XDR bridge-------------------------------------------------------------------
					command="SELECT Enabled,ProgramNum FROM program WHERE ProgName='XDR'";
					table=Db.GetTable(command);
					int programNum;
					if(table.Rows.Count>0 && table.Rows[0]["Enabled"].ToString()=="0") {//if XDR not enabled
						//change the defaults
						programNum=PIn.Int(table.Rows[0]["ProgramNum"].ToString());
						command="UPDATE program SET Path='"+POut.String(@"C:\XDRClient\Bin\XDR.exe")+"' WHERE ProgramNum="+POut.Long(programNum);
						Db.NonQ32(command);
						command="UPDATE programproperty SET PropertyValue='"+POut.String(@"C:\XDRClient\Bin\infofile.txt")+"' "
							+"WHERE ProgramNum="+POut.Long(programNum)+" "
							+"AND PropertyDesc='InfoFile path'";
						Db.NonQ32(command);
						command="UPDATE toolbutitem SET ToolBar=7 "//The toolbar at the top that is common to all modules.
							+"WHERE ProgramNum="+POut.Long(programNum);
						Db.NonQ32(command);
					}
					//iCat Bridge---------------------------------------------------------------------------
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'iCat', "
						+"'iCat from www.imagingsciences.com', "
						+"'0', "
						+"'"+POut.String(@"C:\Program Files\ISIP\iCATVision\Vision.exe")+"', "
						+"'', "
						+"'')";
					programNum=Db.NonQ32(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+programNum.ToString()+"', "
						+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
						+"'0')";
					Db.NonQ32(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+programNum.ToString()+"', "
						+"'Acquisition computer name', "
						+"'')";
					Db.NonQ32(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+programNum.ToString()+"', "
						+"'XML output file path', "
						+"'"+POut.String(@"C:\iCat\Out\pm.xml")+"')";
					Db.NonQ32(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+programNum.ToString()+"', "
						+"'Return folder path', "
						+"'"+POut.String(@"C:\iCat\Return")+"')";
					Db.NonQ32(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"'"+POut.Long(programNum)+"', "
						+"'"+POut.Long((int)ToolBarsAvail.ChartModule)+"', "
						+"'iCat')";
					Db.NonQ32(command);
					//end of iCat Bridge
					string[] commands = new string[]{
						"ALTER TABLE anesthvsdata ADD MessageID varchar(50)",
						"ALTER TABLE anesthvsdata ADD HL7Message longtext"
					};
					Db.NonQ32(commands);
					command="ALTER TABLE computer DROP PrinterName";
					Db.NonQ32(command);
					command="ALTER TABLE computer ADD LastHeartBeat datetime NOT NULL default '0001-01-01'";
					Db.NonQ32(command);
					command="ALTER TABLE registrationkey ADD UsesServerVersion tinyint NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE registrationkey ADD IsFreeVersion tinyint NOT NULL";
					Db.NonQ32(command);
					command="ALTER TABLE registrationkey ADD IsOnlyForTesting tinyint NOT NULL";
					Db.NonQ32(command);
				}
				else {//oracle

				}				
				command="UPDATE preference SET ValueString = '6.6.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_6_2();
		}

		private static void To6_6_2() {
			if(FromVersion<new Version("6.6.2.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.6.2");//No translation in convert script.
				string command;
				command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('WebServiceServerName','','Blank if not using web service.')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '6.6.2.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_6_3();
		}

		private static void To6_6_3() {
			if(FromVersion<new Version("6.6.3.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.6.3");//No translation in convert script.
				string command;
				command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('UpdateInProgressOnComputerName','','Will be blank if update is complete.  If in the middle of an update, the named workstation is the only one allowed to startup OD.')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '6.6.3.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_6_8();
		}

		private static void To6_6_8() {
			if(FromVersion<new Version("6.6.8.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.6.8");//No translation in convert script.
				string command;
				command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('UpdateMultipleDatabases','','Comma delimited')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '6.6.8.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_6_16();
		}

		private static void To6_6_16() {
			if(FromVersion<new Version("6.6.16.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.6.16");//No translation in convert script.
				string command;
				command="SELECT ProgramNum FROM program WHERE ProgName='MediaDent'";
				int programNum=PIn.Int(Db.GetScalar(command));
				command="DELETE FROM programproperty WHERE ProgramNum="+POut.Long(programNum)
					+" AND PropertyDesc='Image Folder'";
				Db.NonQ32(command);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+POut.Long(programNum)+"', "
					+"'"+POut.String("Text file path")+"', "
					+"'"+POut.String(@"C:\MediadentInfo.txt")+"')";
				Db.NonQ32(command);
				command="UPDATE program SET Note='Text file path needs to be the same on all computers.' WHERE ProgramNum="+POut.Long(programNum);
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '6.6.16.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_6_19();
		}

		private static void To6_6_19() {
			if(FromVersion<new Version("6.6.19.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.6.19");//No translation in convert script.
				string command;
				command="UPDATE employee SET LName='O' WHERE LName='' AND FName=''";
				Db.NonQ32(command);
				command="UPDATE schedule SET SchedType=1 WHERE ProvNum != 0 AND SchedType != 1";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '6.6.19.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_7_1();
		}

		private static void To6_7_1() {
			if(FromVersion<new Version("6.7.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.7.1");//No translation in convert script.
				string command;
				command="ALTER TABLE document ADD DateTStamp TimeStamp";
				Db.NonQ32(command);
				command="UPDATE document SET DateTStamp=NOW()";
				Db.NonQ32(command);
				command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('StatementShowNotes','0','Payments and adjustments.')";
				Db.NonQ32(command);
				command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('StatementShowProcBreakdown','0','')";
				Db.NonQ32(command);
				command="ALTER TABLE etrans ADD EtransMessageTextNum INT NOT NULL";
				Db.NonQ32(command);
				command="DROP TABLE IF EXISTS etransmessagetext";
				Db.NonQ32(command);
				command=@"CREATE TABLE etransmessagetext (
					EtransMessageTextNum int NOT NULL auto_increment,
					MessageText text NOT NULL,
					PRIMARY KEY (EtransMessageTextNum),
					INDEX(MessageText(255))
					) DEFAULT CHARSET=utf8";
				Db.NonQ32(command);
				command="ALTER TABLE etrans ADD INDEX(MessageText(255))";
				Db.NonQ32(command);
				command="INSERT INTO etransmessagetext (MessageText) "
					+"SELECT DISTINCT MessageText FROM etrans "
					+"WHERE etrans.MessageText != ''";
				Db.NonQ32(command);
				command="UPDATE etrans,etransmessagetext "
					+"SET etrans.EtransMessageTextNum=etransmessagetext.EtransMessageTextNum "
					+"WHERE etrans.MessageText=etransmessagetext.MessageText";
				Db.NonQ32(command);
				command="ALTER TABLE etrans DROP MessageText";
				Db.NonQ32(command);
				command="ALTER TABLE etransmessagetext DROP INDEX MessageText";
				Db.NonQ32(command);
				command="ALTER TABLE etrans ADD AckEtransNum INT NOT NULL";
				Db.NonQ32(command);
				//Fill the AckEtransNum values for existing claims.
				command=@"DROP TABLE IF EXISTS etack;
CREATE TABLE etack (                                             
EtransNum int(11) NOT NULL auto_increment,                      
DateTimeTrans datetime NOT NULL,  
ClearinghouseNum int(11) NOT NULL,                                                               
BatchNumber int(11) NOT NULL,                                                                  
PRIMARY KEY  (`EtransNum`)                               
)  
SELECT * FROM etrans
WHERE Etype=21;
UPDATE etrans etorig, etack
SET etorig.AckEtransNum=etack.EtransNum 
WHERE etorig.EtransNum != etack.EtransNum
AND etorig.BatchNumber=etack.BatchNumber
AND etorig.ClearinghouseNum=etack.ClearinghouseNum
AND etorig.DateTimeTrans > DATE_SUB(etack.DateTimeTrans,INTERVAL 14 DAY)
AND etorig.DateTimeTrans < DATE_ADD(etack.DateTimeTrans,INTERVAL 1 DAY);
DROP TABLE IF EXISTS etAck";
				Db.NonQ32(command);
				command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('UpdateWebProxyAddress','','')";
				Db.NonQ32(command);
				command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('UpdateWebProxyUserName','','')";
				Db.NonQ32(command);
				command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('UpdateWebProxyPassword','','')";
				Db.NonQ32(command);
				command="ALTER TABLE etrans ADD PlanNum INT NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE etrans ADD INDEX (PlanNum)";
				Db.NonQ32(command);
				//Added new enum value of 0=None to CoverageLevel.
				command="UPDATE benefit SET CoverageLevel=CoverageLevel+1 WHERE BenefitType=2 OR BenefitType=5";//Deductible, Limitations
				Db.NonQ32(command);
				command="ALTER TABLE benefit CHANGE Percent Percent tinyint NOT NULL";//So that we can store -1.
				Db.NonQ32(command);
				command="UPDATE benefit SET Percent=-1 WHERE BenefitType != 1";//set Percent empty where not CoInsurance
				Db.NonQ32(command);
				command="ALTER TABLE benefit DROP OldCode";
				Db.NonQ32(command);
				//set MonetaryAmt empty when ActiveCoverage,CoInsurance,Exclusion
				command="UPDATE benefit SET MonetaryAmt=-1 WHERE BenefitType=0 OR BenefitType=1 OR BenefitType=4";
				Db.NonQ32(command);
				//set MonetaryAmt empty when Limitation and a quantity is entered
				command="UPDATE benefit SET MonetaryAmt=-1 WHERE BenefitType=5 AND Quantity != 0";
				Db.NonQ32(command);
				if(CultureInfo.CurrentCulture.Name=="en-US") {
					command="UPDATE covcat SET CovOrder=CovOrder+1 WHERE CovOrder > 1";
					Db.NonQ32(command);
					command="INSERT INTO covcat (Description,DefaultPercent,CovOrder,IsHidden,EbenefitCat) VALUES('X-Ray',100,2,0,13)";
					int covCatNum=Db.NonQ32(command,true);
					command="INSERT INTO covspan (CovCatNum,FromCode,ToCode) VALUES("+POut.Long(covCatNum)+",'D0200','D0399')";
					Db.NonQ32(command);
					command="SELECT MAX(CovOrder) FROM covcat";
					int covOrder=PIn.Int(Db.GetScalar(command));
					command="INSERT INTO covcat (Description,DefaultPercent,CovOrder,IsHidden,EbenefitCat) VALUES('Adjunctive',-1,"
						+POut.Long(covOrder+1)+",0,14)";//adjunctive
					covCatNum=Db.NonQ32(command,true);
					command="INSERT INTO covspan (CovCatNum,FromCode,ToCode) VALUES("+POut.Long(covCatNum)+",'D9000','D9999')";
					Db.NonQ32(command);
					command="SELECT CovCatNum FROM covcat WHERE EbenefitCat=1";//general
					covCatNum=Db.NonQ32(command,true);
					command="DELETE FROM covspan WHERE CovCatNum="+POut.Long(covCatNum);
					Db.NonQ32(command);
					command="INSERT INTO covspan (CovCatNum,FromCode,ToCode) VALUES("+POut.Long(covCatNum)+",'D0000','D7999')";
					Db.NonQ32(command);
					command="INSERT INTO covspan (CovCatNum,FromCode,ToCode) VALUES("+POut.Long(covCatNum)+",'D9000','D9999')";
					Db.NonQ32(command);
				}
				command="ALTER TABLE claimproc ADD DedEst double NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE claimproc ADD DedEstOverride double NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE claimproc ADD InsEstTotal double NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE claimproc ADD InsEstTotalOverride double NOT NULL";
				Db.NonQ32(command);
				command="UPDATE claimproc SET DedApplied=-1 WHERE ClaimNum=0";//if not attached to a claim, clear this value
				Db.NonQ32(command);
				command="UPDATE claimproc SET DedEstOverride=-1";
				Db.NonQ32(command);
				command="UPDATE claimproc SET InsEstTotal=BaseEst";
				Db.NonQ32(command);
				command="UPDATE claimproc SET InsEstTotalOverride=OverrideInsEst";
				Db.NonQ32(command);
				command="ALTER TABLE claimproc DROP OverrideInsEst";
				Db.NonQ32(command);
				command="ALTER TABLE claimproc DROP DedBeforePerc";
				Db.NonQ32(command);
				command="ALTER TABLE claimproc DROP OverAnnualMax";
				Db.NonQ32(command);
				command="ALTER TABLE claimproc ADD PaidOtherInsOverride double NOT NULL";
				Db.NonQ32(command);
				command="UPDATE claimproc SET PaidOtherInsOverride=PaidOtherIns";
				Db.NonQ32(command);
				command="ALTER TABLE claimproc ADD EstimateNote varchar(255) NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE insplan ADD MonthRenew tinyint NOT NULL";
				Db.NonQ32(command);
				command="SELECT insplan.PlanNum,MONTH(DateEffective) "
					+"FROM insplan,benefit "
					+"WHERE insplan.PlanNum=benefit.PlanNum "
					+"AND benefit.TimePeriod=1 "
					+"GROUP BY insplan.PlanNum";//service year
				DataTable table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="UPDATE insplan SET MonthRenew="+table.Rows[i][1].ToString()
						+" WHERE PlanNum="+table.Rows[i][0].ToString();
					Db.NonQ32(command);
				}
				command="ALTER TABLE appointment ADD InsPlan1 INT NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE appointment ADD INDEX (InsPlan1)";
				Db.NonQ32(command);
				command="ALTER TABLE appointment ADD InsPlan2 INT NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE appointment ADD INDEX (InsPlan2)";
				Db.NonQ32(command);
				command="DROP TABLE IF EXISTS insfilingcode";
				Db.NonQ32(command);
				command=@"CREATE TABLE insfilingcode (
					InsFilingCodeNum INT AUTO_INCREMENT,
					Descript VARCHAR(255),
					EclaimCode VARCHAR(100),
					ItemOrder INT,
					PRIMARY KEY(InsFilingCodeNum),
					INDEX(ItemOrder)
					)";
				Db.NonQ32(command);
				command="DROP TABLE IF EXISTS insfilingcodesubtype";
				Db.NonQ32(command);
				command=@"CREATE TABLE insfilingcodesubtype (
					InsFilingCodeSubtypeNum INT AUTO_INCREMENT,
					InsFilingCodeNum INT,
					Descript VARCHAR(255),
					INDEX(InsFilingCodeNum),
					PRIMARY KEY(InsFilingCodeSubtypeNum)
					)";
				Db.NonQ32(command);
				command="ALTER TABLE insplan ADD FilingCodeSubtype INT NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE insplan ADD INDEX (FilingCodeSubtype)";
				Db.NonQ32(command);
				//eCW bridge enhancements
				command="SELECT ProgramNum FROM program WHERE ProgName='eClinicalWorks'";
				int programNum=PIn.Int(Db.GetScalar(command));
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+POut.Long(programNum)+"', "
					+"'ShowImagesModule', "
					+"'0')";
				Db.NonQ32(command);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+POut.Long(programNum)+"', "
					+"'IsStandalone', "
					+"'0')";//starts out as false
				Db.NonQ32(command);
				command="UPDATE insplan SET FilingCode = FilingCode+1";
				Db.NonQ32(command);
				command="INSERT INTO insfilingcode VALUES(1,'Commercial_Insurance','CI',0)";
				Db.NonQ32(command);
				command="INSERT INTO insfilingcode VALUES(2,'SelfPay','09',1)";
				Db.NonQ32(command);
				command="INSERT INTO insfilingcode VALUES(3,'OtherNonFed','11',2)";
				Db.NonQ32(command);
				command="INSERT INTO insfilingcode VALUES(4,'PPO','12',3)";
				Db.NonQ32(command);
				command="INSERT INTO insfilingcode VALUES(5,'POS','13',4)";
				Db.NonQ32(command);
				command="INSERT INTO insfilingcode VALUES(6,'EPO','14',5)";
				Db.NonQ32(command);
				command="INSERT INTO insfilingcode VALUES(7,'Indemnity','15',6)";
				Db.NonQ32(command);
				command="INSERT INTO insfilingcode VALUES(8,'HMO_MedicareRisk','16',7)";
				Db.NonQ32(command);
				command="INSERT INTO insfilingcode VALUES(9,'DMO','17',8)";
				Db.NonQ32(command);
				command="INSERT INTO insfilingcode VALUES(10,'BCBS','BL',9)";
				Db.NonQ32(command);
				command="INSERT INTO insfilingcode VALUES(11,'Champus','CH',10)";
				Db.NonQ32(command);
				command="INSERT INTO insfilingcode VALUES(12,'Disability','DS',11)";
				Db.NonQ32(command);
				command="INSERT INTO insfilingcode VALUES(13,'FEP','FI',12)";
				Db.NonQ32(command);
				command="INSERT INTO insfilingcode VALUES(14,'HMO','HM',13)";
				Db.NonQ32(command);
				command="INSERT INTO insfilingcode VALUES(15,'LiabilityMedical','LM',14)";
				Db.NonQ32(command);
				command="INSERT INTO insfilingcode VALUES(16,'MedicarePartB','MB',15)";
				Db.NonQ32(command);
				command="INSERT INTO insfilingcode VALUES(17,'Medicaid','MC',16)";
				Db.NonQ32(command);
				command="INSERT INTO insfilingcode VALUES(18,'ManagedCare_NonHMO','MH',17)";
				Db.NonQ32(command);
				command="INSERT INTO insfilingcode VALUES(19,'OtherFederalProgram','OF',18)";
				Db.NonQ32(command);
				command="INSERT INTO insfilingcode VALUES(20,'SelfAdministered','SA',19)";
				Db.NonQ32(command);
				command="INSERT INTO insfilingcode VALUES(21,'Veterans','VA',20)";
				Db.NonQ32(command);
				command="INSERT INTO insfilingcode VALUES(22,'WorkersComp','WC',21)";
				Db.NonQ32(command);
				command="INSERT INTO insfilingcode VALUES(23,'MutuallyDefined','ZZ',22)";
				Db.NonQ32(command);
				//Fixes bug here instead of db maint
				//Duplicated in version 6.6
				command="UPDATE employee SET LName='O' WHERE LName='' AND FName=''";
				Db.NonQ32(command);
				command="UPDATE schedule SET SchedType=1 WHERE ProvNum != 0 AND SchedType != 1";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '6.7.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_7_3();
		}

		private static void To6_7_3() {
			if(FromVersion<new Version("6.7.3.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.7.3");//No translation in convert script.
				string command=@"UPDATE claimform,claimformitem SET claimformitem.FieldName='IsGroupHealthPlan'
					WHERE claimformitem.FieldName='IsStandardClaim' AND claimform.ClaimFormNum=claimformitem.ClaimFormNum
					AND claimform.UniqueID='OD9'";//1500
				Db.NonQ32(command);
				command=@"UPDATE claimform,claimformitem SET claimformitem.XPos='97'
					WHERE claimformitem.XPos='30' AND claimform.ClaimFormNum=claimformitem.ClaimFormNum
					AND claimform.UniqueID='OD9'";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '6.7.3.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_7_4();
		}

		private static void To6_7_4() {
			if(FromVersion<new Version("6.7.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.7.4");//No translation in convert script.
				string command="DELETE FROM medicationpat WHERE EXISTS(SELECT * FROM patient WHERE medicationpat.PatNum=patient.PatNum AND patient.PatStatus=4)";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '6.7.4.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_7_5();
		}

		private static void To6_7_5() {
			if(FromVersion<new Version("6.7.5.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.7.5");//No translation in convert script.
				string command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('ClaimsValidateACN','0','If set to 1, then any claim with a groupName containing ADDP will require an ACN number in the claim remarks.')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '6.7.5.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_7_12();
		}

		private static void To6_7_12() {
			if(FromVersion<new Version("6.7.12.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.7.12");//No translation in convert script.
				string command;
				//Camsight Bridge---------------------------------------------------------------------------
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'Camsight', "
					+"'Camsight from www.camsight.com', "
					+"'0', "
					+"'"+POut.String(@"C:\cdm\cdm\cdmx\cdmx.exe")+"', "
					+"'', "
					+"'')";
				int programNum=Db.NonQ32(command,true);
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
					+"'Camsight')";
				Db.NonQ32(command);
				//CliniView Bridge---------------------------------------------------------------------------
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'CliniView', "
					+"'CliniView', "
					+"'0', "
					+"'"+POut.String(@"C:\Program Files\CliniView\CliniView.exe")+"', "
					+"'', "
					+"'')";
				programNum=Db.NonQ32(command,true);
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
					+"'CliniView')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '6.7.12.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_7_15();
		}

		private static void To6_7_15() {
			//duplicated in 6.6.26
			if(FromVersion<new Version("6.7.15.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.7.15");//No translation in convert script.
				string command;
				command="ALTER TABLE insplan CHANGE FeeSched FeeSched int NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE insplan CHANGE CopayFeeSched CopayFeeSched int NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE insplan CHANGE AllowedFeeSched AllowedFeeSched int NOT NULL";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '6.7.15.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_7_22();
		}

		private static void To6_7_22() {
			if(FromVersion<new Version("6.7.22.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.7.22");//No translation in convert script.
				string command;
				command="UPDATE preference SET ValueString ='http://opendentalsoft.com:1942/WebServiceCustomerUpdates/Service1.asmx' WHERE PrefName='UpdateServerAddress' AND ValueString LIKE '%70.90.133.65%'";
				Db.NonQ(command);
				try {
					command="ALTER TABLE document ADD INDEX (PatNum)";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog ADD INDEX (BillingTypeOne)";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog ADD INDEX (BillingTypeTwo)";
					Db.NonQ(command);
					command="ALTER TABLE securitylog ADD INDEX (PatNum)";
					Db.NonQ(command);
					command="ALTER TABLE toothinitial ADD INDEX (PatNum)";
					Db.NonQ(command);
					command="ALTER TABLE patplan ADD INDEX (PlanNum)";
					Db.NonQ(command);
				}
				catch {
					//in case any of the indices arlready exists.
				}
				command="UPDATE preference SET ValueString = '6.7.22.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To6_8_1();
		}

		//To6_7_25()//duplicated further down

		private static void To6_8_1() {
			if(FromVersion<new Version("6.8.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.8.1");//No translation in convert script.
				string command;
				//add TreatPlanEdit,ReportProdInc,TimecardDeleteEntry permissions to all groups------------------------------------------------------
				command="SELECT UserGroupNum FROM usergroup";
				DataTable table=Db.GetTable(command);
				int groupNum;
				for(int i=0;i<table.Rows.Count;i++) {
					groupNum=PIn.Int(table.Rows[i][0].ToString());
					command="INSERT INTO grouppermission (NewerDate,UserGroupNum,PermType) "
						+"VALUES('0001-01-01',"+POut.Long(groupNum)+","+POut.Long((int)Permissions.TreatPlanEdit)+")";
					Db.NonQ32(command);
					command="INSERT INTO grouppermission (NewerDate,UserGroupNum,PermType) "
						+"VALUES('0001-01-01',"+POut.Long(groupNum)+","+POut.Long((int)Permissions.ReportProdInc)+")";
					Db.NonQ32(command);
					command="INSERT INTO grouppermission (NewerDate,UserGroupNum,PermType) "
						+"VALUES('0001-01-01',"+POut.Long(groupNum)+","+POut.Long((int)Permissions.TimecardDeleteEntry)+")";
					Db.NonQ32(command);
				}
				command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('BillingExcludeIfUnsentProcs','0','')";
				Db.NonQ32(command);
				command="SELECT MAX(DefNum) FROM definition";
				int defNum=PIn.Int(Db.GetScalar(command))+1;
				command="SELECT MAX(ItemOrder) FROM definition WHERE Category=18";
				int order=PIn.Int(Db.GetScalar(command))+1;
				command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) VALUES("
					+POut.Long(defNum)+",18,"+POut.Long(order)+",'Tooth Charts','T',0,0)";
				Db.NonQ32(command);
				command="ALTER TABLE apptview ADD OnlyScheduledProvs tinyint unsigned NOT NULL";
				Db.NonQ32(command);
				//Get rid of some old columns
				command="ALTER TABLE appointment DROP Lab";
				Db.NonQ32(command);
				command="ALTER TABLE appointment DROP InstructorNum";
				Db.NonQ32(command);
				command="ALTER TABLE appointment DROP SchoolClassNum";
				Db.NonQ32(command);
				command="ALTER TABLE appointment DROP SchoolCourseNum";
				Db.NonQ32(command);
				command="ALTER TABLE appointment DROP GradePoint";
				Db.NonQ32(command);
				command="DROP TABLE IF EXISTS graphicassembly";
				Db.NonQ32(command);
				command="DROP TABLE IF EXISTS graphicelement";
				Db.NonQ32(command);
				command="DROP TABLE IF EXISTS graphicpoint";
				Db.NonQ32(command);
				command="DROP TABLE IF EXISTS graphicshape";
				Db.NonQ32(command);
				command="DROP TABLE IF EXISTS graphictype";
				Db.NonQ32(command);
				command="DROP TABLE IF EXISTS proclicense";
				Db.NonQ32(command);
				command="DROP TABLE IF EXISTS scheddefault";
				Db.NonQ32(command);
				//Change all primary and foreign keys to 64 bit---------------------------------------------------------------
				command="ALTER TABLE account CHANGE AccountNum AccountNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE accountingautopay CHANGE AccountingAutoPayNum AccountingAutoPayNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE accountingautopay CHANGE PayType PayType bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE adjustment CHANGE AdjNum AdjNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE adjustment CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE adjustment CHANGE AdjType AdjType bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE adjustment CHANGE ProvNum ProvNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE adjustment CHANGE ProcNum ProcNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE appointment CHANGE AptNum AptNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE appointment CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE appointment CHANGE Confirmed Confirmed bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE appointment CHANGE Op Op bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE appointment CHANGE ProvNum ProvNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE appointment CHANGE ProvHyg ProvHyg bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE appointment CHANGE NextAptNum NextAptNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE appointment CHANGE UnschedStatus UnschedStatus bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE appointment CHANGE Assistant Assistant bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE appointment CHANGE ClinicNum ClinicNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE appointment CHANGE InsPlan1 InsPlan1 bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE appointment CHANGE InsPlan2 InsPlan2 bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE appointmentrule CHANGE AppointmentRuleNum AppointmentRuleNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE apptview CHANGE ApptViewNum ApptViewNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE apptviewitem CHANGE ApptViewItemNum ApptViewItemNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE apptviewitem CHANGE ApptViewNum ApptViewNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE apptviewitem CHANGE OpNum OpNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE apptviewitem CHANGE ProvNum ProvNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE autocode CHANGE AutoCodeNum AutoCodeNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE autocodecond CHANGE AutoCodeCondNum AutoCodeCondNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE autocodecond CHANGE AutoCodeItemNum AutoCodeItemNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE autocodeitem CHANGE AutoCodeItemNum AutoCodeItemNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE autocodeitem CHANGE AutoCodeNum AutoCodeNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE autocodeitem CHANGE CodeNum CodeNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE autonote CHANGE AutoNoteNum AutoNoteNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE autonotecontrol CHANGE AutoNoteControlNum AutoNoteControlNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE benefit CHANGE BenefitNum BenefitNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE benefit CHANGE PlanNum PlanNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE benefit CHANGE PatPlanNum PatPlanNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE benefit CHANGE CovCatNum CovCatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE benefit CHANGE CodeNum CodeNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE canadianclaim CHANGE ClaimNum ClaimNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE canadianextract CHANGE CanadianExtractNum CanadianExtractNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE canadianextract CHANGE ClaimNum ClaimNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE canadiannetwork CHANGE CanadianNetworkNum CanadianNetworkNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE carrier CHANGE CarrierNum CarrierNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE carrier CHANGE CanadianNetworkNum CanadianNetworkNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE claim CHANGE ClaimNum ClaimNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE claim CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE claim CHANGE PlanNum PlanNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE claim CHANGE ProvTreat ProvTreat bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE claim CHANGE ProvBill ProvBill bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE claim CHANGE ReferringProv ReferringProv bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE claim CHANGE PlanNum2 PlanNum2 bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE claim CHANGE ClinicNum ClinicNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE claim CHANGE ClaimForm ClaimForm bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE claimattach CHANGE ClaimAttachNum ClaimAttachNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE claimattach CHANGE ClaimNum ClaimNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE claimcondcodelog CHANGE ClaimCondCodeLogNum ClaimCondCodeLogNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE claimcondcodelog CHANGE ClaimNum ClaimNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE claimform CHANGE ClaimFormNum ClaimFormNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE claimformitem CHANGE ClaimFormItemNum ClaimFormItemNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE claimformitem CHANGE ClaimFormNum ClaimFormNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE claimpayment CHANGE ClaimPaymentNum ClaimPaymentNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE claimpayment CHANGE ClinicNum ClinicNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE claimpayment CHANGE DepositNum DepositNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE claimproc CHANGE ClaimProcNum ClaimProcNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE claimproc CHANGE ProcNum ProcNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE claimproc CHANGE ClaimNum ClaimNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE claimproc CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE claimproc CHANGE ProvNum ProvNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE claimproc CHANGE ClaimPaymentNum ClaimPaymentNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE claimproc CHANGE PlanNum PlanNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE claimvalcodelog CHANGE ClaimValCodeLogNum ClaimValCodeLogNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE claimvalcodelog CHANGE ClaimNum ClaimNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE clearinghouse CHANGE ClearinghouseNum ClearinghouseNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE clinic CHANGE ClinicNum ClinicNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE clinic CHANGE InsBillingProv InsBillingProv bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE clockevent CHANGE ClockEventNum ClockEventNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE clockevent CHANGE EmployeeNum EmployeeNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE commlog CHANGE CommlogNum CommlogNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE commlog CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE commlog CHANGE CommType CommType bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE commlog CHANGE UserNum UserNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE computer CHANGE ComputerNum ComputerNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE computerpref CHANGE ComputerPrefNum ComputerPrefNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE contact CHANGE ContactNum ContactNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE contact CHANGE Category Category bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE covcat CHANGE CovCatNum CovCatNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE covspan CHANGE CovSpanNum CovSpanNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE covspan CHANGE CovCatNum CovCatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE definition CHANGE DefNum DefNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE deletedobject CHANGE DeletedObjectNum DeletedObjectNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE deletedobject CHANGE ObjectNum ObjectNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE deposit CHANGE DepositNum DepositNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE disease CHANGE DiseaseNum DiseaseNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE disease CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE disease CHANGE DiseaseDefNum DiseaseDefNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE diseasedef CHANGE DiseaseDefNum DiseaseDefNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE displayfield CHANGE DisplayFieldNum DisplayFieldNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE document CHANGE DocNum DocNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE document CHANGE DocCategory DocCategory bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE document CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE document CHANGE MountItemNum MountItemNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE dunning CHANGE DunningNum DunningNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE dunning CHANGE BillingType BillingType bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE electid CHANGE ElectIDNum ElectIDNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE emailattach CHANGE EmailAttachNum EmailAttachNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE emailattach CHANGE EmailMessageNum EmailMessageNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE emailmessage CHANGE EmailMessageNum EmailMessageNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE emailmessage CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE emailtemplate CHANGE EmailTemplateNum EmailTemplateNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE employee CHANGE EmployeeNum EmployeeNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE employer CHANGE EmployerNum EmployerNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE etrans CHANGE EtransNum EtransNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE etrans CHANGE ClearingHouseNum ClearingHouseNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE etrans CHANGE ClaimNum ClaimNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE etrans CHANGE CarrierNum CarrierNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE etrans CHANGE CarrierNum2 CarrierNum2 bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE etrans CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE etrans CHANGE EtransMessageTextNum EtransMessageTextNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE etrans CHANGE AckEtransNum AckEtransNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE etrans CHANGE PlanNum PlanNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE etransmessagetext CHANGE EtransMessageTextNum EtransMessageTextNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE fee CHANGE FeeNum FeeNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE fee CHANGE FeeSched FeeSched bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE fee CHANGE CodeNum CodeNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE feesched CHANGE FeeSchedNum FeeSchedNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE files CHANGE DocNum DocNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE formpat CHANGE FormPatNum FormPatNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE formpat CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE grouppermission CHANGE GroupPermNum GroupPermNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE grouppermission CHANGE UserGroupNum UserGroupNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE hl7msg CHANGE HL7MsgNum HL7MsgNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE hl7msg CHANGE AptNum AptNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE insfilingcode CHANGE InsFilingCodeNum InsFilingCodeNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE insfilingcodesubtype CHANGE InsFilingCodeSubTypeNum InsFilingCodeSubTypeNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE insfilingcodesubtype CHANGE InsFilingCodeNum InsFilingCodeNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE insplan CHANGE PlanNum PlanNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE insplan CHANGE Subscriber Subscriber bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE insplan CHANGE FeeSched FeeSched bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE insplan CHANGE ClaimFormNum ClaimFormNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE insplan CHANGE CopayFeeSched CopayFeeSched bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE insplan CHANGE EmployerNum EmployerNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE insplan CHANGE CarrierNum CarrierNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE insplan CHANGE AllowedFeeSched AllowedFeeSched bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE insplan CHANGE FilingCode FilingCode bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE insplan CHANGE FilingCodeSubtype FilingCodeSubtype bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE journalentry CHANGE JournalEntryNum JournalEntryNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE journalentry CHANGE TransactionNum TransactionNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE journalentry CHANGE AccountNum AccountNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE journalentry CHANGE ReconcileNum ReconcileNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE labcase CHANGE LabCaseNum LabCaseNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE labcase CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE labcase CHANGE LaboratoryNum LaboratoryNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE labcase CHANGE AptNum AptNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE labcase CHANGE PlannedAptNum PlannedAptNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE labcase CHANGE ProvNum ProvNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE laboratory CHANGE LaboratoryNum LaboratoryNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE labturnaround CHANGE LabTurnaroundNum LabTurnaroundNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE labturnaround CHANGE LaboratoryNum LaboratoryNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE letter CHANGE LetterNum LetterNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE lettermerge CHANGE LetterMergeNum LetterMergeNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE lettermerge CHANGE Category Category bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE lettermergefield CHANGE FieldNum FieldNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE lettermergefield CHANGE LetterMergeNum LetterMergeNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE medication CHANGE MedicationNum MedicationNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE medication CHANGE GenericNum GenericNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE medicationpat CHANGE MedicationPatNum MedicationPatNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE medicationpat CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE medicationpat CHANGE MedicationNum MedicationNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE mount CHANGE MountNum MountNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE mount CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE mount CHANGE DocCategory DocCategory bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE mountdef CHANGE MountDefNum MountDefNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE mountitem CHANGE MountItemNum MountItemNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE mountitem CHANGE MountNum MountNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE mountitemdef CHANGE MountItemDefNum MountItemDefNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE mountitemdef CHANGE MountDefNum MountDefNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE operatory CHANGE OperatoryNum OperatoryNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE operatory CHANGE ProvDentist ProvDentist bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE operatory CHANGE ProvHygienist ProvHygienist bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE operatory CHANGE ClinicNum ClinicNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE patfield CHANGE PatFieldNum PatFieldNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE patfield CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE patfielddef CHANGE PatFieldDefNum PatFieldDefNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE patient CHANGE PatNum PatNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE patient CHANGE Guarantor Guarantor bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE patient CHANGE PriProv PriProv bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE patient CHANGE SecProv SecProv bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE patient CHANGE FeeSched FeeSched bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE patient CHANGE BillingType BillingType bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE patient CHANGE EmployerNum EmployerNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE patient CHANGE ClinicNum ClinicNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE patient CHANGE SiteNum SiteNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE patient CHANGE ResponsParty ResponsParty bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE patientnote CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE patplan CHANGE PatPlanNum PatPlanNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE patplan CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE patplan CHANGE PlanNum PlanNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE payment CHANGE PayNum PayNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE payment CHANGE PayType PayType bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE payment CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE payment CHANGE ClinicNum ClinicNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE payment CHANGE DepositNum DepositNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE payperiod CHANGE PayPeriodNum PayPeriodNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE payplan CHANGE PayPlanNum PayPlanNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE payplan CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE payplan CHANGE Guarantor Guarantor bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE payplan CHANGE PlanNum PlanNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE payplancharge CHANGE PayPlanChargeNum PayPlanChargeNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE payplancharge CHANGE PayPlanNum PayPlanNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE payplancharge CHANGE Guarantor Guarantor bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE payplancharge CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE payplancharge CHANGE ProvNum ProvNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE paysplit CHANGE SplitNum SplitNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE paysplit CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE paysplit CHANGE PayNum PayNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE paysplit CHANGE ProvNum ProvNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE paysplit CHANGE PayPlanNum PayPlanNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE paysplit CHANGE ProcNum ProcNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE perioexam CHANGE PerioExamNum PerioExamNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE perioexam CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE perioexam CHANGE ProvNum ProvNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE periomeasure CHANGE PerioMeasureNum PerioMeasureNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE periomeasure CHANGE PerioExamNum PerioExamNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE pharmacy CHANGE PharmacyNum PharmacyNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE phonenumber CHANGE PhoneNumberNum PhoneNumberNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE phonenumber CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE plannedappt CHANGE PlannedApptNum PlannedApptNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE plannedappt CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE plannedappt CHANGE AptNum AptNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE popup CHANGE PopupNum PopupNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE popup CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE printer CHANGE PrinterNum PrinterNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE printer CHANGE ComputerNum ComputerNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE procbutton CHANGE ProcButtonNum ProcButtonNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE procbutton CHANGE Category Category bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE procbuttonitem CHANGE ProcButtonItemNum ProcButtonItemNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE procbuttonitem CHANGE ProcButtonNum ProcButtonNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE procbuttonitem CHANGE AutoCodeNum AutoCodeNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE procbuttonitem CHANGE CodeNum CodeNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE proccodenote CHANGE ProcCodeNoteNum ProcCodeNoteNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE proccodenote CHANGE CodeNum CodeNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE proccodenote CHANGE ProvNum ProvNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE procedurecode CHANGE CodeNum CodeNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE procedurecode CHANGE ProcCat ProcCat bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE procedurelog CHANGE ProcNum ProcNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE procedurelog CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE procedurelog CHANGE AptNum AptNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE procedurelog CHANGE Priority Priority bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE procedurelog CHANGE ProvNum ProvNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE procedurelog CHANGE Dx Dx bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE procedurelog CHANGE PlannedAptNum PlannedAptNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE procedurelog CHANGE ClinicNum ClinicNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE procedurelog CHANGE ProcNumLab ProcNumLab bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE procedurelog CHANGE BillingTypeOne BillingTypeOne bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE procedurelog CHANGE BillingTypeTwo BillingTypeTwo bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE procedurelog CHANGE CodeNum CodeNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE procedurelog CHANGE SiteNum SiteNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE procnote CHANGE ProcNoteNum ProcNoteNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE procnote CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE procnote CHANGE ProcNum ProcNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE procnote CHANGE UserNum UserNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE proctp CHANGE ProcTPNum ProcTPNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE proctp CHANGE TreatPlanNum TreatPlanNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE proctp CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE proctp CHANGE ProcNumOrig ProcNumOrig bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE proctp CHANGE Priority Priority bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE program CHANGE ProgramNum ProgramNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE programproperty CHANGE ProgramPropertyNum ProgramPropertyNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE programproperty CHANGE ProgramNum ProgramNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE provider CHANGE ProvNum ProvNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE provider CHANGE FeeSched FeeSched bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE provider CHANGE SchoolClassNum SchoolClassNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE provider CHANGE AnesthProvType AnesthProvType bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE providerident CHANGE ProviderIdentNum ProviderIdentNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE providerident CHANGE ProvNum ProvNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE question CHANGE QuestionNum QuestionNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE question CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE question CHANGE FormPatNum FormPatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE questiondef CHANGE QuestionDefNum QuestionDefNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE quickpastecat CHANGE QuickPasteCatNum QuickPasteCatNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE quickpastenote CHANGE QuickPasteNoteNum QuickPasteNoteNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE quickpastenote CHANGE QuickPasteCatNum QuickPasteCatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE recall CHANGE RecallNum RecallNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE recall CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE recall CHANGE RecallStatus RecallStatus bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE recall CHANGE RecallTypeNum RecallTypeNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE recalltrigger CHANGE RecallTriggerNum RecallTriggerNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE recalltrigger CHANGE RecallTypeNum RecallTypeNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE recalltrigger CHANGE CodeNum CodeNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE recalltype CHANGE RecallTypeNum RecallTypeNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE reconcile CHANGE ReconcileNum ReconcileNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE reconcile CHANGE AccountNum AccountNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE refattach CHANGE RefAttachNum RefAttachNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE refattach CHANGE ReferralNum ReferralNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE refattach CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE referral CHANGE ReferralNum ReferralNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE referral CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE referral CHANGE Slip Slip bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE registrationkey CHANGE RegistrationKeyNum RegistrationKeyNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE registrationkey CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE repeatcharge CHANGE RepeatChargeNum RepeatChargeNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE repeatcharge CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE reqneeded CHANGE ReqNeededNum ReqNeededNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE reqneeded CHANGE SchoolCourseNum SchoolCourseNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE reqneeded CHANGE SchoolClassNum SchoolClassNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE reqstudent CHANGE ReqStudentNum ReqStudentNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE reqstudent CHANGE ReqNeededNum ReqNeededNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE reqstudent CHANGE SchoolCourseNum SchoolCourseNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE reqstudent CHANGE ProvNum ProvNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE reqstudent CHANGE AptNum AptNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE reqstudent CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE reqstudent CHANGE InstructorNum InstructorNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE rxalert CHANGE RxAlertNum RxAlertNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE rxalert CHANGE RxDefNum RxDefNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE rxalert CHANGE DiseaseDefNum DiseaseDefNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE rxdef CHANGE RxDefNum RxDefNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE rxpat CHANGE RxNum RxNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE rxpat CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE rxpat CHANGE ProvNum ProvNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE rxpat CHANGE PharmacyNum PharmacyNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE schedule CHANGE ScheduleNum ScheduleNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE schedule CHANGE ProvNum ProvNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE schedule CHANGE BlockoutType BlockoutType bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE schedule CHANGE EmployeeNum EmployeeNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE scheduleop CHANGE ScheduleOpNum ScheduleOpNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE scheduleop CHANGE ScheduleNum ScheduleNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE scheduleop CHANGE OperatoryNum OperatoryNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE schoolclass CHANGE SchoolClassNum SchoolClassNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE schoolcourse CHANGE SchoolCourseNum SchoolCourseNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE screen CHANGE ScreenNum ScreenNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE screen CHANGE ProvNum ProvNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE screen CHANGE ScreenGroupNum ScreenGroupNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE screengroup CHANGE ScreenGroupNum ScreenGroupNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE securitylog CHANGE SecurityLogNum SecurityLogNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE securitylog CHANGE UserNum UserNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE securitylog CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE sheet CHANGE SheetNum SheetNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE sheet CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE sheetdef CHANGE SheetDefNum SheetDefNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE sheetfield CHANGE SheetFieldNum SheetFieldNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE sheetfield CHANGE SheetNum SheetNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE sheetfielddef CHANGE SheetFieldDefNum SheetFieldDefNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE sheetfielddef CHANGE SheetDefNum SheetDefNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE sigbutdef CHANGE SigButDefNum SigButDefNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE sigbutdefelement CHANGE ElementNum ElementNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE sigbutdefelement CHANGE SigButDefNum SigButDefNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE sigbutdefelement CHANGE SigElementDefNum SigElementDefNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE sigelement CHANGE SigElementNum SigElementNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE sigelement CHANGE SigElementDefNum SigElementDefNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE sigelement CHANGE SignalNum SignalNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE sigelementdef CHANGE SigElementDefNum SigElementDefNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE `signal` CHANGE SignalNum SignalNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE `signal` CHANGE TaskNum TaskNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE site CHANGE SiteNum SiteNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE statement CHANGE StatementNum StatementNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE statement CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE statement CHANGE DocNum DocNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE supplier CHANGE SupplierNum SupplierNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE supply CHANGE SupplyNum SupplyNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE supply CHANGE SupplierNum SupplierNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE supply CHANGE Category Category bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE supplyneeded CHANGE SupplyNeededNum SupplyNeededNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE supplyorder CHANGE SupplyOrderNum SupplyOrderNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE supplyorder CHANGE SupplierNum SupplierNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE supplyorderitem CHANGE SupplyOrderItemNum SupplyOrderItemNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE supplyorderitem CHANGE SupplyOrderNum SupplyOrderNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE supplyorderitem CHANGE SupplyNum SupplyNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE task CHANGE TaskNum TaskNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE task CHANGE TaskListNum TaskListNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE task CHANGE KeyNum KeyNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE task CHANGE FromNum FromNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE task CHANGE UserNum UserNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE taskancestor CHANGE TaskAncestorNum TaskAncestorNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE taskancestor CHANGE TaskNum TaskNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE taskancestor CHANGE TaskListNum TaskListNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE tasklist CHANGE TaskListNum TaskListNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE tasklist CHANGE Parent Parent bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE tasklist CHANGE FromNum FromNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE tasksubscription CHANGE TaskSubscriptionNum TaskSubscriptionNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE tasksubscription CHANGE UserNum UserNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE tasksubscription CHANGE TaskListNum TaskListNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE terminalactive CHANGE TerminalActiveNum TerminalActiveNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE terminalactive CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE timeadjust CHANGE TimeAdjustNum TimeAdjustNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE timeadjust CHANGE EmployeeNum EmployeeNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE toolbutitem CHANGE ToolButItemNum ToolButItemNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE toolbutitem CHANGE ProgramNum ProgramNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE toothinitial CHANGE ToothInitialNum ToothInitialNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE toothinitial CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE transaction CHANGE TransactionNum TransactionNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE transaction CHANGE UserNum UserNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE transaction CHANGE DepositNum DepositNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE transaction CHANGE PayNum PayNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE treatplan CHANGE TreatPlanNum TreatPlanNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE treatplan CHANGE PatNum PatNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE treatplan CHANGE ResponsParty ResponsParty bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE usergroup CHANGE UserGroupNum UserGroupNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE userod CHANGE UserNum UserNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE userod CHANGE UserGroupNum UserGroupNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE userod CHANGE EmployeeNum EmployeeNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE userod CHANGE ClinicNum ClinicNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE userod CHANGE ProvNum ProvNum bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE userod CHANGE TaskListInBox TaskListInBox bigint NOT NULL";
				Db.NonQ32(command);
				command="ALTER TABLE userquery CHANGE QueryNum QueryNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="ALTER TABLE zipcode CHANGE ZipCodeNum ZipCodeNum bigint NOT NULL auto_increment";
				Db.NonQ32(command);
				command="DROP TABLE IF EXISTS replicationserver";
				Db.NonQ32(command);
				command=@"CREATE TABLE replicationserver (
					ReplicationServerNum bigint NOT NULL auto_increment,
					Descript TEXT NOT NULL,
					ServerId INT unsigned NOT NULL,
					RangeStart BIGINT NOT NULL,
					RangeEnd BIGINT NOT NULL,
					PRIMARY KEY(ReplicationServerNum)
					)";
				Db.NonQ32(command);
				command="ALTER TABLE claimproc ADD WriteOffEst double NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE claimproc ADD WriteOffEstOverride double NOT NULL";
				Db.NonQ(command);
				command="UPDATE claimproc SET WriteOffEst = -1";
				Db.NonQ(command);
				command="UPDATE claimproc SET WriteOffEstOverride = -1";
				Db.NonQ(command);
				command="ALTER TABLE paysplit ADD UnearnedType bigint NOT NULL";
				Db.NonQ(command);
				command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES ('RecallMaxNumberReminders','-1','')";
				Db.NonQ(command);
				command="ALTER TABLE recall ADD DisableUntilBalance double NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE recall ADD DisableUntilDate date NOT NULL default '0001-01-01'";
				Db.NonQ(command);
				command="ALTER TABLE program ADD PluginDllName varchar(255) NOT NULL";
				Db.NonQ(command);
				command="DELETE FROM preference WHERE PrefName = 'DeductibleBeforePercentAsDefault'";
				Db.NonQ(command);
				//We will not delete this pref just in case it's needed later.  It's not used anywhere right now.
				//command = "DELETE FROM preference WHERE PrefName='EnableAnesthMod'";
				//We will not delete this pref just in case it's needed later.  It's not used anywhere right now.
				//command="DELETE FROM preference WHERE PrefName='ImageStore'";//this option is no longer supported.
				//Db.NonQ(command);
				command="SELECT MAX(ItemOrder) FROM definition WHERE Category=2";
				int itemOrder=PIn.Int(Db.GetScalar(command))+1;//eg 7+1
				//this should end up with an acceptable autoincrement even if using random primary keys.
				command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES (2,"+POut.Int(itemOrder)+",'E-mailed','E-mailed')";
				Db.NonQ(command);
				command="SELECT DefNum FROM definition WHERE Category=2 AND ItemOrder="+POut.Int(itemOrder);
				string defNumStr=Db.GetScalar(command);
				command="INSERT INTO preference (PrefName,ValueString) VALUES ('ConfirmStatusEmailed','"+defNumStr+"')";
				Db.NonQ(command);
				command="INSERT INTO preference (PrefName,ValueString) VALUES ('ConfirmEmailSubject','Appointment Confirmation')";
				Db.NonQ(command);
				command="INSERT INTO preference (PrefName,ValueString) VALUES ('ConfirmEmailMessage','[NameF], We would like to confirm your appointment on [date] at [time]')";
				Db.NonQ(command);
				command="ALTER TABLE replicationserver ADD AtoZpath varchar(255) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE replicationserver ADD UpdateBlocked tinyint NOT NULL";
				Db.NonQ(command);
				try {
					command="ALTER TABLE task ADD INDEX (KeyNum)";
					Db.NonQ(command);
				}
				catch { }
				command="UPDATE preference SET ValueString = '6.8.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To6_8_7();
		}

		private static void To6_8_7() {
			//duplicated in 6.7
			if(FromVersion<new Version("6.8.7.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.8.7");//No translation in convert script.
				string command;
				try {
					command="ALTER TABLE claimpayment ADD INDEX (DepositNum)";
					Db.NonQ(command);
					command="ALTER TABLE payment ADD INDEX (DepositNum)";
					Db.NonQ(command);
				}
				catch {
					//in case any of the indices already exists.
				}
				command="UPDATE preference SET ValueString = '6.8.7.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To6_8_11();
		}

		private static void To6_8_11() {
			//duplicated in 6.6 and 6.7
			if(FromVersion<new Version("6.8.11.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.8.11");//No translation in convert script.
				string command;
				//Mediadent version 4 and 5---------------------------------------
				command="SELECT COUNT(*) FROM programproperty WHERE PropertyDesc='MediaDent Version 4 or 5'";
				if(Db.GetScalar(command)=="0") {
					command="SELECT ProgramNum FROM program WHERE ProgName='MediaDent'";
					long programNum=PIn.Long(Db.GetScalar(command));
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'"+POut.String("MediaDent Version 4 or 5")+"', "
						+"'5')";
					Db.NonQ(command);
					//add back the image folder
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'"+POut.String("Image Folder")+"', "
						+"'"+POut.String(@"C:\Mediadent\patients\")+"')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '6.8.11.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To6_8_12();
		}

		private static void To6_8_12() {
			if(FromVersion<new Version("6.8.12.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.8.12");//No translation in convert script.
				string command;
				//Ewoo_EZDent bridge-------------------------------------------------------------------------
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'EwooEZDent', "
					+"'EwooEZDent from www.ewoousa.com', "
					+"'0', "
					+"'"+POut.String(@"C:\EasyDent4\Edp4\EasyDent4.exe")+"', "
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
					+"'"+POut.Long((int)ToolBarsAvail.ChartModule)+"', "
					+"'EZDent')";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '6.8.12.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To6_8_24();
		}

		private static void To6_8_24() {
			if(FromVersion<new Version("6.8.24.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.8.24");//No translation in convert script.
				string command;
				command="SELECT CodeNum FROM procedurecode WHERE ProcCode='D1204'";
				string codeNum1204=Db.GetScalar(command);
				command="SELECT CodeNum FROM procedurecode WHERE ProcCode='D1203'";
				string codeNum1203=Db.GetScalar(command);
				if(codeNum1203!="" && codeNum1204!="") {
					command="UPDATE benefit SET CodeNum="+codeNum1203+" WHERE CodeNum="+codeNum1204;
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '6.8.24.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To6_9_1();
		}

		private static void To6_9_1() {
			if(FromVersion<new Version("6.9.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.9.1");//No translation in convert script.
				string command;
				//Mountainside Bridge---------------------------------------------------------------------------
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'Mountainside', "
					+"'Mountainside from www.mountainsidesoftware.com', "
					+"'0', "
					+"'', "
					+"'', "
					+"'')";
				Db.NonQ(command);
				//Move the HL7 folders from eCW to the pref table
				command="SELECT PropertyValue FROM programproperty WHERE PropertyDesc='HL7FolderOut'";
				string folder=Db.GetScalar(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('HL7FolderOut','"+POut.String(folder)+"')";
				Db.NonQ(command);
				command="DELETE FROM programproperty WHERE PropertyDesc='HL7FolderOut'";
				Db.NonQ(command);
				command="SELECT PropertyValue FROM programproperty WHERE PropertyDesc='HL7FolderIn'";
				folder=Db.GetScalar(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('HL7FolderIn','"+POut.String(folder)+"')";
				Db.NonQ(command);
				command="DELETE FROM programproperty WHERE PropertyDesc='HL7FolderIn'";
				Db.NonQ(command);
				//Clinic enhancements----------------------------------------------------------------------------------------
				command="ALTER TABLE paysplit ADD ClinicNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE paysplit ADD INDEX (ClinicNum)";
				Db.NonQ(command);
				command="Update payment,paysplit SET paysplit.ClinicNum = payment.ClinicNum WHERE paysplit.PayNum = payment.PayNum";
				Db.NonQ(command);
				command="ALTER TABLE claimproc ADD ClinicNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE claimproc ADD INDEX (ClinicNum)";
				Db.NonQ(command);
				command="Update procedurelog,claimproc SET claimproc.ClinicNum = procedurelog.ClinicNum WHERE claimproc.ProcNum = procedurelog.ProcNum";
				Db.NonQ(command);
				//then, for claimprocs that are total payments and not attached to any proc:
				command="Update claim,claimproc SET claimproc.ClinicNum = claim.ClinicNum WHERE claimproc.ClaimNum = claim.ClaimNum AND claimproc.ProcNum=0";
				Db.NonQ(command);
				command="ALTER TABLE adjustment ADD ClinicNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE adjustment ADD INDEX (ClinicNum)";
				Db.NonQ(command);
				command="ALTER TABLE payplancharge ADD ClinicNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE payplancharge ADD INDEX (ClinicNum)";
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('PerioColorCAL','-16777011')";
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('PerioColorFurcations','-16777216')";
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('PerioColorFurcationsRed','-7667712')";
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('PerioColorGM','-8388480')";
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('PerioColorMGJ','-29696')";
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('PerioColorProbing','-16744448')";
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('PerioColorProbingRed','-65536')";
				Db.NonQ(command);
				command="ALTER TABLE registrationkey ADD VotesAllotted int NOT NULL";
				Db.NonQ(command);
				command="UPDATE registrationkey SET VotesAllotted =100";
				Db.NonQ(command);
				command="ALTER TABLE apptview ADD OnlySchedBeforeTime time NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE apptview ADD OnlySchedAfterTime time NOT NULL";
				Db.NonQ(command);
				command="DROP TABLE IF EXISTS automation";
				Db.NonQ(command);
				command=@"CREATE TABLE automation (
					AutomationNum bigint NOT NULL auto_increment,
					Description text NOT NULL,
					Autotrigger tinyint NOT NULL,
					ProcCodes text NOT NULL,
					AutoAction tinyint NOT NULL,
					SheetNum bigint NOT NULL,
					CommType bigint NOT NULL,
					MessageContent text NOT NULL,
					PRIMARY KEY(AutomationNum)
					)";
				Db.NonQ(command);
				command="ALTER TABLE sheet ADD Description varchar(255) NOT NULL";
				Db.NonQ(command);
				//for each sheettype, set descriptions for all sheets of that type.
				for(int i=0;i<Enum.GetNames(typeof(SheetTypeEnum)).Length;i++) {
					command="UPDATE sheet SET Description= '"+POut.String(Enum.GetNames(typeof(SheetTypeEnum))[i])+"' "
						+"WHERE SheetType="+POut.Int(i);
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '6.9.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To6_9_4();
		}

		private static void To6_9_4() {
			if(FromVersion<new Version("6.9.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.9.4");//No translation in convert script.
				string command;
				command="ALTER TABLE automation CHANGE SheetNum SheetDefNum bigint NOT NULL";
				Db.NonQ(command);
				//Trophy
				command="SELECT ProgramNum FROM program WHERE ProgName='TrophyEnhanced'";
				long programNum=PIn.Long(Db.GetScalar(command));
				if(programNum>0) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'Enter 1 to enable Numbered Mode', "
						+"'0')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '6.9.4.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To6_9_10();
		}

		private static void To6_9_10() {
			if(FromVersion<new Version("6.9.10.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 6.9.10");//No translation in convert script.
				string command;
				command="ALTER TABLE computerpref ADD COLUMN DirectXFormat VARCHAR(255) DEFAULT ''";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '6.9.10.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_0_1();
		}

		private static void To7_0_1() {
			if(FromVersion<new Version("7.0.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.0.1");//No translation in convert script.
				string command;
				command="INSERT INTO preference(PrefName,ValueString) VALUES('InsDefaultShowUCRonClaims','0')";
				Db.NonQ(command);
				command="DROP TABLE IF EXISTS equipment";
				Db.NonQ(command);
				command=@"CREATE TABLE equipment (
					EquipmentNum bigint NOT NULL auto_increment,
					Description text NOT NULL,
					SerialNumber varchar(255),
					ModelYear varchar(2),
					DatePurchased date NOT NULL default '0001-01-01',
					DateSold date NOT NULL default '0001-01-01',
					PurchaseCost double NOT NULL,
					MarketValue double NOT NULL,
					Location text NOT NULL,
					DateEntry date NOT NULL default '0001-01-01',
					PRIMARY KEY(EquipmentNum)
					)";
				Db.NonQ(command);
				command="ALTER TABLE sheet ADD ShowInTerminal tinyint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE sheetfielddef ADD RadioButtonValue varchar(255) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE sheetfield ADD RadioButtonValue varchar(255) NOT NULL";
				Db.NonQ(command);
				//add a bunch of indexes to the benefit table to make it faster when there are many similar plans
				command="ALTER TABLE benefit ADD INDEX(CovCatNum)";
				Db.NonQ(command);
				command="ALTER TABLE benefit ADD INDEX(BenefitType)";
				Db.NonQ(command);
				command="ALTER TABLE benefit ADD INDEX(Percent)";
				Db.NonQ(command);
				command="ALTER TABLE benefit ADD INDEX(MonetaryAmt)";
				Db.NonQ(command);
				command="ALTER TABLE benefit ADD INDEX(TimePeriod)";
				Db.NonQ(command);
				command="ALTER TABLE benefit ADD INDEX(QuantityQualifier)";
				Db.NonQ(command);
				command="ALTER TABLE benefit ADD INDEX(Quantity)";
				Db.NonQ(command);
				command="ALTER TABLE benefit ADD INDEX(CodeNum)";
				Db.NonQ(command);
				command="ALTER TABLE benefit ADD INDEX(CoverageLevel)";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.0.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ32(command);
			}
			To7_1_1();
		}

		private static void To7_1_1() {
			if(FromVersion<new Version("7.1.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.1.1");//No translation in convert script.
				string command;
				try {
					command="ALTER TABLE refattach ADD INDEX (PatNum)";
					Db.NonQ(command);
				}
				catch {}
				command="INSERT INTO preference(PrefName,ValueString) VALUES('UpdateShowMsiButtons','0')";
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ReportsPPOwriteoffDefaultToProcDate','0')";
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ReportsShowPatNum','0')";
				Db.NonQ(command);
				command="ALTER TABLE userod ADD DefaultHidePopups tinyint NOT NULL";
				Db.NonQ(command);
				command="DROP TABLE IF EXISTS taskunread";
				Db.NonQ(command);
				command=@"CREATE TABLE taskunread (
					TaskUnreadNum bigint NOT NULL auto_increment,
					TaskNum bigint NOT NULL,
					UserNum bigint NOT NULL,
					PRIMARY KEY (TaskUnreadNum),
					INDEX(TaskNum),
					INDEX(UserNum)
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
				//MercuryDE clearinghouse.
				command=@"INSERT INTO clearinghouse(Description,ExportPath,IsDefault,Payors,Eformat,
Password,ResponsePath,CommBridge,ClientProgram,ISA05,ISA07,
ISA08,
ISA15,
GS03) 
VALUES('MercuryDE','"+POut.String(@"C:\MercuryDE\Temp\")+@"','0','','1','','','11','','ZZ','ZZ',
'204203105',
'P',
'204203105')";
				Db.NonQ(command);
				command="ALTER TABLE clockevent CHANGE TimeEntered TimeEntered1 datetime NOT NULL default '0001-01-01 00:00:00'";
				Db.NonQ(command);
				command="ALTER TABLE clockevent CHANGE TimeDisplayed TimeDisplayed1 datetime NOT NULL default '0001-01-01 00:00:00'";
				Db.NonQ(command);
				command="ALTER TABLE clockevent ADD TimeEntered2 datetime NOT NULL default '0001-01-01 00:00:00'";
				Db.NonQ(command);
				command="ALTER TABLE clockevent ADD TimeDisplayed2 datetime NOT NULL default '0001-01-01 00:00:00'";
				Db.NonQ(command);
				command="SELECT * FROM clockevent WHERE ClockStatus != 2 ORDER BY EmployeeNum,TimeDisplayed1";
				DataTable table=Db.GetTable(command);
				DateTime timeEntered2;
				DateTime timeDisplayed2;
				string note;
				int clockStatus;
				for(int i=0;i<table.Rows.Count;i++){
					if(table.Rows[i]["ClockIn"].ToString()=="0"){//false
						continue;//only stop on clock-in rows, not clock-out.
					}
					if(i==table.Rows.Count-1){//if this is the last row
						break;//because we always need the next clock-out to actually do anything.
					}
					if(table.Rows[i+1]["ClockIn"].ToString()=="1"){//true
						continue;//if the next row is also a clock-in, then we have two clock-ins in a row.  Can't do anything.
					}
					if(table.Rows[i]["EmployeeNum"].ToString()!=table.Rows[i+1]["EmployeeNum"].ToString()){//employeeNums don't match
						continue;
					}
					timeEntered2=PIn.DateT(table.Rows[i+1]["TimeEntered1"].ToString());//The time of the second row
					timeDisplayed2=PIn.DateT(table.Rows[i+1]["TimeDisplayed1"].ToString());
					clockStatus=PIn.Int(table.Rows[i+1]["ClockStatus"].ToString());
					note=PIn.String(table.Rows[i+1]["Note"].ToString());
					command="UPDATE clockevent SET "
						+"TimeEntered2 = "+POut.DateT(timeEntered2)+", "
						+"TimeDisplayed2 = "+POut.DateT(timeDisplayed2)+", "
						+"ClockStatus = "+POut.Int(clockStatus)+", "
						+"Note = CONCAT(Note,'"+POut.String(note)+"') "
						+"WHERE ClockEventNum = "+table.Rows[i]["ClockEventNum"].ToString();
					Db.NonQ(command);
					command="DELETE FROM clockevent WHERE ClockEventNum = "+table.Rows[i+1]["ClockEventNum"].ToString();
					Db.NonQ(command);
				}
				//now, breaks, which are out/in instead of in/out.
				command="SELECT * FROM clockevent WHERE ClockStatus = 2 ORDER BY EmployeeNum,TimeDisplayed1";
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++){
					if(table.Rows[i]["ClockIn"].ToString()=="1"){//true
						continue;//only stop on clock-out rows, not clock-in.
					}
					if(i==table.Rows.Count-1){//if this is the last row
						break;//because we always need the next clock-in to actually do anything.
					}
					if(table.Rows[i+1]["ClockIn"].ToString()=="0"){//false
						continue;//if the next row is also a clock-out, then we have two clock-outs in a row.  Can't do anything.
					}
					if(table.Rows[i]["EmployeeNum"].ToString()!=table.Rows[i+1]["EmployeeNum"].ToString()){//employeeNums don't match
						continue;
					}
					timeEntered2=PIn.DateT(table.Rows[i+1]["TimeEntered1"].ToString());//The time of the second row
					timeDisplayed2=PIn.DateT(table.Rows[i+1]["TimeDisplayed1"].ToString());
					//clockStatus=PIn.Int(table.Rows[i+1]["ClockStatus"].ToString());
					note=PIn.String(table.Rows[i+1]["Note"].ToString());
					command="UPDATE clockevent SET "
						+"TimeEntered2 = "+POut.DateT(timeEntered2)+", "
						+"TimeDisplayed2 = "+POut.DateT(timeDisplayed2)+", "
						//+"ClockStatus = "+POut.Int(clockStatus)+", "
						+"Note = CONCAT(Note,'"+POut.String(note)+"') "
						+"WHERE ClockEventNum = "+table.Rows[i]["ClockEventNum"].ToString();
					Db.NonQ(command);
					command="DELETE FROM clockevent WHERE ClockEventNum = "+table.Rows[i+1]["ClockEventNum"].ToString();
					Db.NonQ(command);
				}
				command="ALTER TABLE clockevent DROP ClockIn";
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('PasswordsMustBeStrong','0')";
				Db.NonQ(command);
				command="ALTER TABLE userod ADD PasswordIsStrong tinyint NOT NULL";
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('SecurityLockDays','0')";
				Db.NonQ(command);
				command="ALTER TABLE laboratory DROP LabSlip";
				Db.NonQ(command);
				command="ALTER TABLE laboratory ADD Slip bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE laboratory ADD Address varchar(255) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE laboratory ADD City varchar(255) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE laboratory ADD State varchar(255) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE laboratory ADD Zip varchar(255) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE laboratory ADD Email varchar(255) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE laboratory ADD WirelessPhone varchar(255) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE procedurelog ADD HideGraphics tinyint NOT NULL";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.1.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_1_2();
		}

		private static void To7_1_2() {
			if(FromVersion<new Version("7.1.2.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.1.2");//No translation in convert script.
				string command;
				command="ALTER TABLE provider ADD TaxonomyCodeOverride varchar(255) NOT NULL";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.1.2.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_1_16();
		}

		private static void To7_1_16() {
			if(FromVersion<new Version("7.1.16.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.1.16");//No translation in convert script.
				string command;
				command="ALTER TABLE etransmessagetext CHANGE MessageText MessageText mediumtext NOT NULL";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.1.16.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_1_18();
		}

		private static void To7_1_18() {
			if(FromVersion<new Version("7.1.18.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.1.18");//No translation in convert script.
				string command;
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ToothChartMoveMenuToRight','0')";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.1.18.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_1_24();
		}

		private static void To7_1_24() {
			if(FromVersion<new Version("7.1.24.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.1.24");//No translation in convert script.
				string command;
				command="UPDATE patient SET Guarantor=PatNum WHERE Guarantor=0;";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.1.24.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_2_1();
		}

		private static void To7_2_1() {
			if(FromVersion<new Version("7.2.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.2.1");//No translation in convert script.
				string command;
				//this column was a varchar holding currency amounts.
				command="ALTER TABLE claimvalcodelog CHANGE ValAmount ValAmount double not null";
				Db.NonQ(command);
				command="ALTER TABLE carrier ADD CanadianEncryptionMethod tinyint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE carrier ADD CanadianTransactionPrefix varchar(255) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE carrier ADD CanadianSupportedTypes int NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE canadianclaim DROP EligibilityCode";
				Db.NonQ(command);
				command="ALTER TABLE canadianclaim DROP SchoolName";
				Db.NonQ(command);
				command="ALTER TABLE patient ADD CanadianEligibilityCode tinyint NOT NULL";
				Db.NonQ(command);
				command="DROP TABLE canadianextract";
				Db.NonQ(command);
				command="DROP TABLE canadianclaim";
				Db.NonQ(command);
				command="ALTER TABLE claim ADD CanadianMaterialsForwarded varchar(10) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE claim ADD CanadianReferralProviderNum varchar(20) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE claim ADD CanadianReferralReason tinyint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE claim ADD CanadianIsInitialLower varchar(5) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE claim ADD CanadianDateInitialLower date NOT NULL default '0001-01-01'";
				Db.NonQ(command);
				command="ALTER TABLE claim ADD CanadianMandProsthMaterial tinyint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE claim ADD CanadianIsInitialUpper varchar(5) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE claim ADD CanadianDateInitialUpper date NOT NULL default '0001-01-01'";
				Db.NonQ(command);
				command="ALTER TABLE claim ADD CanadianMaxProsthMaterial tinyint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE carrier DROP IsPMP";
				Db.NonQ(command);
				command="ALTER TABLE insplan ADD CanadianPlanFlag varchar(5) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE procedurelog ADD CanadianTypeCodes varchar(20) NOT NULL";
				Db.NonQ(command);
				command="UPDATE clearinghouse SET ResponsePath='"+POut.String(@"C:\MercuryDE\Reports\")+"' WHERE ResponsePath='' AND Description='MercuryDE' LIMIT 1";
				Db.NonQ(command);
				command="DROP TABLE IF EXISTS guardian";
				Db.NonQ(command);
				command=@"CREATE TABLE guardian (
					GuardianNum bigint NOT NULL auto_increment,
					PatNumChild bigint NOT NULL,
					PatNumGuardian bigint NOT NULL,
					Relationship tinyint NOT NULL,
					PRIMARY KEY (GuardianNum),
					INDEX(PatNumChild),
					INDEX(PatNumGuardian)
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
				command="ALTER TABLE apptviewitem ADD ElementAlignment tinyint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE apptview ADD StackBehavUR tinyint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE apptview ADD StackBehavLR tinyint NOT NULL";
				Db.NonQ(command);
				command="DROP TABLE IF EXISTS apptfield";
				Db.NonQ(command);
				command=@"CREATE TABLE apptfield (
					ApptFieldNum bigint NOT NULL auto_increment,
					AptNum bigint NOT NULL,
					FieldName varchar(255) NOT NULL,
					FieldValue text NOT NULL,
					PRIMARY KEY (ApptFieldNum),
					INDEX(AptNum)
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
				command="DROP TABLE IF EXISTS apptfielddef";
				Db.NonQ(command);
				command=@"CREATE TABLE apptfielddef (
					ApptFieldDefNum bigint NOT NULL auto_increment,
					FieldName varchar(255) NOT NULL,
					PRIMARY KEY (ApptFieldDefNum)
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
				try {
					command="ALTER TABLE patfield ADD INDEX (PatNum)";
					Db.NonQ(command);
				}
				catch {
					//in case the index already exists.
				}
				command="ALTER TABLE labcase ADD LabFee double NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE insplan CHANGE PlanNote PlanNote text NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE insplan CHANGE BenefitNotes BenefitNotes text NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE insplan CHANGE SubscNote SubscNote text NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE apptviewitem ADD ApptFieldDefNum bigint NOT NULL";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.2.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_2_3();
		}

		private static void To7_2_3() {
			if(FromVersion<new Version("7.2.3.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.2.3");//No translation in convert script.
				string command;
				command="ALTER TABLE apptviewitem ADD PatFieldDefNum bigint NOT NULL";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.2.3.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_2_4();
		}

		private static void To7_2_4() {
			if(FromVersion<new Version("7.2.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.2.4");//No translation in convert script.
				string command;
				command="UPDATE apptview SET StackBehavUR=1";//all horiz
				Db.NonQ(command);
				command="SELECT ApptViewNum FROM apptview";//all of them.
				DataTable table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					command="SELECT COUNT(*) FROM apptviewitem WHERE ApptViewNum="+table.Rows[i]["ApptViewNum"].ToString()
						+" AND ElementDesc='AssistantAbbr'";
					if(Db.GetCount(command)=="0") {
						command="INSERT INTO apptviewitem (ApptViewNum,ElementDesc,ElementOrder,ElementColor,ElementAlignment) VALUES("
							+table.Rows[i]["ApptViewNum"].ToString()+","
							+"'AssistantAbbr',"
							+"0,"
							+"-16777216,"//black
							+"2)";//LR
						Db.NonQ(command);
					}
					command="SELECT COUNT(*) FROM apptviewitem WHERE ApptViewNum="+table.Rows[i]["ApptViewNum"].ToString()
						+" AND ElementDesc='ConfirmedColor'";
					if(Db.GetCount(command)=="0") {
						command="INSERT INTO apptviewitem (ApptViewNum,ElementDesc,ElementOrder,ElementColor,ElementAlignment) VALUES("
							+table.Rows[i]["ApptViewNum"].ToString()+","
							+"'ConfirmedColor',"
							+"0,"
							+"-16777216,"
							+"1)";//UR
						Db.NonQ(command);
					}
					command="SELECT COUNT(*) FROM apptviewitem WHERE ApptViewNum="+table.Rows[i]["ApptViewNum"].ToString()
						+" AND ElementDesc='HasIns[I]'";
					if(Db.GetCount(command)=="0") {
						command="INSERT INTO apptviewitem (ApptViewNum,ElementDesc,ElementOrder,ElementColor,ElementAlignment) VALUES("
							+table.Rows[i]["ApptViewNum"].ToString()+","
							+"'HasIns[I]',"
							+"1,"
							+"-16777216,"
							+"1)";//UR
						Db.NonQ(command);
					}
					command="SELECT COUNT(*) FROM apptviewitem WHERE ApptViewNum="+table.Rows[i]["ApptViewNum"].ToString()
						+" AND ElementDesc='InsToSend[!]'";
					if(Db.GetCount(command)=="0") {
						command="INSERT INTO apptviewitem (ApptViewNum,ElementDesc,ElementOrder,ElementColor,ElementAlignment) VALUES("
							+table.Rows[i]["ApptViewNum"].ToString()+","
							+"'InsToSend[!]',"
							+"2,"
							+"-65536,"//red
							+"1)";//UR
						Db.NonQ(command);
					}
				}
				command="UPDATE preference SET ValueString = '7.2.4.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_2_7();
		}

		private static void To7_2_7() {
			if(FromVersion<new Version("7.2.7.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.2.7");//No translation in convert script.
				string command;
				command="UPDATE apptviewitem SET ElementColor=-1 WHERE ElementDesc='"+POut.String("MedOrPremed[+]")+"'";//white
				Db.NonQ(command);
				command="UPDATE apptviewitem SET ElementColor=-1 WHERE ElementDesc='"+POut.String("HasIns[I]")+"'";
				Db.NonQ(command);
				command="UPDATE apptviewitem SET ElementColor=-1 WHERE ElementDesc='"+POut.String("InsToSend[!]")+"'";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.2.7.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_2_12();
		}

		private static void To7_2_12() {
			if(FromVersion<new Version("7.2.12.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.2.12");//No translation in convert script.
				string command;
				command="INSERT INTO preference(PrefName,ValueString) VALUES('RecallUseEmailIfHasEmailAddress','0')";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.2.12.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_2_31();
		}

		private static void To7_2_31() {
			if(FromVersion<new Version("7.2.31.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.2.31");//No translation in convert script.
				string command;
				//add Sopro bridge:
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'Sopro', "
					+"'Sopro by Acteon www.acteongroup.com', "
					+"'0', "
					+"'"+POut.String(@"C:\Program Files\Sopro Imaging\SOPRO Imaging.exe")+"', "
					+"'', "
					+"'')";
				Db.NonQ(command);
				command="SELECT ProgramNum FROM program WHERE ProgName='Sopro' LIMIT 1";
				long programNum=PIn.Long(Db.GetScalar(command));
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
					+"'Sopro')";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.2.31.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_2_36();
		}

		private static void To7_2_36() {
			if(FromVersion<new Version("7.2.36.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.2.36");//No translation in convert script.
				string command;
				command="ALTER TABLE hl7msg CHANGE MsgText MsgText MEDIUMTEXT";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.2.36.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_2_38();
		}

		private static void To7_2_38() {
			if(FromVersion<new Version("7.2.38.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.2.38");//No translation in convert script.
				string command;
				//add Progeny bridge:
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'Progeny', "
					+"'Progeny from www.progenydental.com', "
					+"'0', "
					+"'"+POut.String(@"C:\Program Files\Progeny\Progeny Imaging\PIBridge.exe")+"', "
					+"'', "
					+"'')";
				Db.NonQ(command);
				command="SELECT ProgramNum FROM program WHERE ProgName='Progeny' LIMIT 1";
				long programNum=PIn.Long(Db.GetScalar(command));
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+POut.Long(programNum)+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0')";
				Db.NonQ32(command);
				command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					+"VALUES ("
					+"'"+POut.Long(programNum)+"', "
					+"'"+POut.Int(((int)ToolBarsAvail.ChartModule))+"', "
					+"'Progeny')";
				Db.NonQ32(command);
				//ProcDate, split off time component-----------------------------------------------------------------------
				command="ALTER TABLE procedurelog ADD ProcTime time NOT NULL";
				Db.NonQ(command);
				command="UPDATE procedurelog SET ProcTime = time(ProcDate)";
				Db.NonQ(command);
				command="ALTER TABLE procedurelog CHANGE ProcDate ProcDate date NOT NULL default '0001-01-01'";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.2.38.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_3_1();
		}

		private static void To7_3_1() {
			if(FromVersion<new Version("7.3.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.3.1");//No translation in convert script.
				string command;
				command="ALTER TABLE patient CHANGE SchoolName SchoolName varchar(255) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE sheet ADD IsWebForm tinyint NOT NULL";
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('WebHostSynchServerURL','https://opendentalsoft.com/WebHostSynch/WebHostSynch.asmx')";
				Db.NonQ(command);
				command="ALTER TABLE appointment ADD DateTimeAskedToArrive datetime NOT NULL default '0001-01-01 00:00:00'";
				Db.NonQ(command);
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'OrthoPlex', "
					+"'OrthoPlex from Dentsply GAC', "
					+"'0', "
					+"'"+POut.String(@"C:\\Program Files\\GAC\\OrthoPlex v3.20\\OrthoPlex.exe")+"', "
					+"'-E [PatNum]', "
					+"'')";
				Db.NonQ(command);
				command="SELECT ProgramNum FROM program WHERE ProgName='OrthoPlex' LIMIT 1";
				long programNum=PIn.Long(Db.GetScalar(command));
				command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					+"VALUES ("
					+"'"+POut.Long(programNum)+"', "
					+"'"+POut.Int((int)ToolBarsAvail.ChartModule)+"', "
					+"'OrthoPlex')";
				Db.NonQ(command);
				command="ALTER TABLE patient ADD AskToArriveEarly int NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE appointment ADD ProcsColored text NOT NULL";
				Db.NonQ(command);
				command="DROP TABLE IF EXISTS procapptcolor";
				Db.NonQ(command);
				command=@"CREATE TABLE procapptcolor (
					ProcApptColorNum bigint NOT NULL auto_increment,
					CodeRange varchar(255) NOT NULL,
					ColorText int NOT NULL,
					PRIMARY KEY (ProcApptColorNum)
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
				command="ALTER TABLE procapptcolor ADD ShowPreviousDate tinyint NOT NULL";
				Db.NonQ(command);
				command="DROP TABLE IF EXISTS chartview";
				Db.NonQ(command);
				command=@"CREATE TABLE chartview (
					ChartViewNum bigint NOT NULL auto_increment,
					Description varchar(255) NOT NULL,
					ItemOrder int NOT NULL,
					ProcStatuses tinyint NOT NULL,
					ObjectTypes smallint NOT NULL,
					ShowProcNotes tinyint NOT NULL,
					IsAudit tinyint NOT NULL,
					SelectedTeethOnly tinyint NOT NULL,
					PRIMARY KEY (ChartViewNum)
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
				command="ALTER TABLE sheetfield ADD RadioButtonGroup varchar(255) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE sheetfield ADD IsRequired tinyint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE sheetfielddef ADD RadioButtonGroup varchar(255) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE sheetfielddef ADD IsRequired tinyint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE displayfield ADD ChartViewNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE displayfield ADD INDEX (ChartViewNum)";
				Db.NonQ(command);
				command="DELETE FROM displayfield WHERE Category = 0";
				Db.NonQ(command);
				command="DROP TABLE IF EXISTS procgroupitem";
				Db.NonQ(command);
				command=@"CREATE TABLE procgroupitem (
					ProcGroupItemNum bigint NOT NULL auto_increment,
					ProcNum bigint NOT NULL,
					GroupNum bigint NOT NULL,
					PRIMARY KEY (ProcGroupItemNum),
					INDEX(ProcNum),
					INDEX(GroupNum)
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
				command="SELECT DefNum FROM definition WHERE Category=11 ORDER BY ItemOrder DESC LIMIT 1";
				long procCat=PIn.Long(Db.GetScalar(command));
				command=@"INSERT INTO procedurecode (ProcCode,Descript,AbbrDesc,ProcTime,ProcCat,
					DefaultNote) VALUES('~GRP~','Group Note','GrpNote','"
					+POut.String("/X/")+"',"+POut.Long(procCat)+",'')";
				Db.NonQ(command);
				//add Orion bridge:
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'Orion', "
					+"'Orion', "
					+"'0', "
					+"'', "
					+"'', "
					+"'')";
				Db.NonQ(command);
				//add PayConnect bridge:
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'PayConnect', "
					+"'PayConnect from www.dentalxchange.com', "
					+"'0', "
					+"'', "
					+"'', "
					+"'No program path or arguments. Usernames and passwords are supplied by dentalxchange.')";
				Db.NonQ(command);
				command="SELECT ProgramNum FROM program WHERE ProgName='PayConnect' LIMIT 1";
				programNum=PIn.Long(Db.GetScalar(command));
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+POut.Long(programNum)+"', "
					+"'Username', "
					+"'')";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+POut.Long(programNum)+"', "
					+"'Password', "
					+"'')";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+POut.Long(programNum)+"', "
					+"'PaymentType', "
					+"'0')";
				Db.NonQ(command);
				//Delete NewPatientForm bridge
				command="SELECT ProgramNum From program WHERE ProgName='NewPatientForm.com'";
				programNum=PIn.Long(Db.GetScalar(command));
				if(programNum>0) {
					command="DELETE FROM program WHERE ProgramNum="+POut.Long(programNum);
					Db.NonQ(command);
					command="DELETE FROM toolbutitem WHERE ProgramNum="+POut.Long(programNum);
					Db.NonQ(command);
				}
				command="DROP TABLE IF EXISTS orionproc";
				Db.NonQ(command);
				command=@"CREATE TABLE orionproc (
					OrionProcNum bigint NOT NULL auto_increment,
					ProcNum bigint NOT NULL,
					DPC tinyint NOT NULL,
					DateScheduleBy date NOT NULL default '0001-01-01', 
					DateStopClock date NOT NULL default '0001-01-01',  
					Status2 int NOT NULL,
					IsOnCall tinyint NOT NULL,
					IsEffectiveComm tinyint NOT NULL,
					IsRepair tinyint NOT NULL,
					PRIMARY KEY (OrionProcNum),
					INDEX(ProcNum)
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
				command="ALTER TABLE commlog ADD Signature text NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE commlog ADD SigIsTopaz tinyint NOT NULL";
				Db.NonQ(command);
				command="INSERT INTO grouppermission (NewerDays,UserGroupNum,PermType) " //Everyone starts with sheet edit initially.
					+"SELECT 0,UserGroupNum,"+POut.Int((int)Permissions.SheetEdit)+" "
					+"FROM usergroup";
				Db.NonQ(command);
				command="ALTER TABLE procedurelog ADD ProcTimeEnd time NOT NULL";
				Db.NonQ(command);
				command="INSERT INTO grouppermission (NewerDays,UserGroupNum,PermType) " //Everyone starts with commlog edit initially.
					+"SELECT 0,UserGroupNum,"+POut.Int((int)Permissions.CommlogEdit)+" "
					+"FROM usergroup";
				Db.NonQ(command);	
				command="ALTER TABLE patfielddef ADD FieldType tinyint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE patfielddef ADD PickList text NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE commlog ADD DateTStamp timestamp";
				Db.NonQ(command);
				command="UPDATE commlog SET DateTStamp=NOW()";
				Db.NonQ32(command);
				command="ALTER TABLE procedurelog ADD DateTStamp timestamp";
				Db.NonQ(command);
				command="UPDATE procedurelog SET DateTStamp=NOW()";
				Db.NonQ32(command);
				command="ALTER TABLE procedurecode ADD IsMultiVisit tinyint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE chartview ADD OrionStatusFlags int NOT NULL";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.3.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_4_1();
		}

		private static void To7_4_1() {
			if(FromVersion<new Version("7.4.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.4.1");//No translation in convert script.
				string command;
				command="SELECT TimeAdjustNum,RegHours,OTimeHours FROM timeadjust";
				DataTable table=Db.GetTable(command);
				command="UPDATE timeadjust SET RegHours=0, OTimeHours=0";
				Db.NonQ(command);
				command="ALTER TABLE timeadjust CHANGE RegHours RegHours time NOT NULL default '00:00:00'";
				Db.NonQ(command);
				command="ALTER TABLE timeadjust CHANGE OTimeHours OTimeHours time NOT NULL default '00:00:00'";
				Db.NonQ(command);
				long timeAdjustNum;
				double regDouble;
				double oTimeDouble;
				TimeSpan regSpan;
				TimeSpan oTimeSpan;
				for(int i=0;i<table.Rows.Count;i++) {
					timeAdjustNum=PIn.Long(table.Rows[i]["TimeAdjustNum"].ToString());
					regDouble=PIn.Double(table.Rows[i]["RegHours"].ToString());
					oTimeDouble=PIn.Double(table.Rows[i]["OTimeHours"].ToString());
					regSpan=TimeSpan.FromHours(regDouble);
					oTimeSpan=TimeSpan.FromHours(oTimeDouble);
					command="UPDATE timeadjust "
						+"SET RegHours='"+POut.TSpan(regSpan)+"', "
						+"OTimeHours='"+POut.TSpan(oTimeSpan)+"' "
						+"WHERE TimeAdjustNum="+POut.Long(timeAdjustNum);
					Db.NonQ(command);
				}
				command="ALTER TABLE clockevent ADD OTimeHours time NOT NULL";
				Db.NonQ(command);
				command="UPDATE clockevent SET OTimeHours ='-01:00:00'";//default to -1 to indicate no override.
				Db.NonQ(command);
				command="ALTER TABLE clockevent ADD OTimeAuto time NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE clockevent ADD Adjust time NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE clockevent ADD AdjustAuto time NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE clockevent ADD AdjustIsOverridden tinyint NOT NULL";
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('TimeCardsUseDecimalInsteadOfColon','0')";
				Db.NonQ(command);
				command="DROP TABLE IF EXISTS timecardrule";
				Db.NonQ(command);
				command=@"CREATE TABLE timecardrule (
					TimeCardRuleNum bigint NOT NULL auto_increment,
					EmployeeNum bigint NOT NULL,
					OverHoursPerDay time NOT NULL,
					AfterTimeOfDay time NOT NULL,
					PRIMARY KEY (TimeCardRuleNum),
					INDEX(EmployeeNum)
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('SecurityLogOffWithWindows','0')";
				Db.NonQ(command);
				command="DROP TABLE IF EXISTS automationcondition";
				Db.NonQ(command);
				command=@"CREATE TABLE automationcondition (
					AutomationConditionNum bigint NOT NULL auto_increment,
					AutomationNum bigint NOT NULL,
					CompareField tinyint NOT NULL,
					Comparison tinyint NOT NULL,
					CompareString varchar(255) NOT NULL,
					PRIMARY KEY (AutomationConditionNum),
					INDEX(AutomationNum)
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('TimeCardsMakesAdjustmentsForOverBreaks','0')";
				Db.NonQ(command);
				command="ALTER TABLE timeadjust ADD IsAuto tinyint NOT NULL";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.4.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_4_7();
		}
		
		private static void To7_4_7() {
			if(FromVersion<new Version("7.4.7.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.4.7");//No translation in convert script.
				string command;
				try {
					List<long> aptNums=new List<long>();
					Appointment[] aptList=Appointments.GetForPeriod(DateTime.Now.Date,DateTime.MaxValue.AddDays(-10));
					for(int i=0;i<aptList.Length;i++) {
						aptNums.Add(aptList[i].AptNum);
					}
					List<Procedure> procsMultApts=Procedures.GetProcsMultApts(aptNums);
					for(int i=0;i<aptList.Length;i++) {
						Appointment newApt=aptList[i].Copy();
						newApt.ProcDescript="";
						Procedure[] procsForOne=Procedures.GetProcsOneApt(aptList[i].AptNum,procsMultApts);
						string procDescript="";
						for(int j=0;j<procsForOne.Length;j++) {
							ProcedureCode procCode=ProcedureCodes.GetProcCodeFromDb(procsForOne[j].CodeNum);
							if(j>0) {
								procDescript+=", ";
							}
							switch(procCode.TreatArea.ToString()) {
								case "Surf"://TreatmentArea.Surf:
									procDescript+="#"+Tooth.GetToothLabel(procsForOne[j].ToothNum)+"-"
									+procsForOne[j].Surf+"-";//""#12-MOD-"
									break;
								case "Tooth"://TreatmentArea.Tooth:
									procDescript+="#"+Tooth.GetToothLabel(procsForOne[j].ToothNum)+"-";//"#12-"
									break;
								case "Quad"://TreatmentArea.Quad:
									procDescript+=procsForOne[j].Surf+"-";//"UL-"
									break;
								case "Sextant"://TreatmentArea.Sextant:
									procDescript+="S"+procsForOne[j].Surf+"-";//"S2-"
									break;
								case "Arch"://TreatmentArea.Arch:
									procDescript+=procsForOne[j].Surf+"-";//"U-"
									break;
								case "ToothRange"://TreatmentArea.ToothRange:
									break;
								default://area 3 or 0 (mouth)
									break;
							}
							procDescript+=procCode.AbbrDesc;
						}
						newApt.ProcDescript=procDescript;
						Appointments.Update(newApt,aptList[i]);
					}
				}
				catch { }//do nothing.  Should not have used objects.  They are causing failures as the objects change in future versions.
				command="UPDATE preference SET ValueString = '7.4.7.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_5_1();
		}

		/// <summary>Does nothing if this pref is already present</summary>
		public static void Set_7_5_17_AutoMerge(YN InsPlanConverstion_7_5_17_AutoMergeYN) {
			string command="SELECT COUNT(*) FROM preference WHERE PrefName='InsPlanConverstion_7_5_17_AutoMergeYN'";
			if(Db.GetCount(command)=="0") {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('InsPlanConverstion_7_5_17_AutoMergeYN','"+POut.Int((int)InsPlanConverstion_7_5_17_AutoMergeYN)+"')";
				Db.NonQ(command);
			}
			else {
				command="UPDATE preference SET ValueString ='"+POut.Int((int)InsPlanConverstion_7_5_17_AutoMergeYN)+"' WHERE PrefName = 'InsPlanConverstion_7_5_17_AutoMergeYN'";
				Db.NonQ(command);
			}
		}

		private static void To7_5_1() {
			if(FromVersion<new Version("7.5.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.5.1");//No translation in convert script.
				string command;
				command="DROP TABLE IF EXISTS inssub";
				Db.NonQ(command);
				command=@"CREATE TABLE inssub (
					InsSubNum bigint NOT NULL auto_increment,
					PlanNum bigint NOT NULL,
					Subscriber bigint NOT NULL,
					DateEffective date NOT NULL default '0001-01-01',
					DateTerm date NOT NULL default '0001-01-01',
					ReleaseInfo tinyint NOT NULL,
					AssignBen tinyint NOT NULL,
					SubscriberID varchar(255) NOT NULL,
					BenefitNotes text NOT NULL,
					SubscNote text NOT NULL,
					OldPlanNum bigint NOT NULL,
					PRIMARY KEY (InsSubNum),
					INDEX(PlanNum), 
					INDEX(Subscriber),
					INDEX(OldPlanNum)
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
				command="UPDATE insplan SET TrojanID='' WHERE TrojanID IS NULL";//In a previous version of this script, NULL TrojanIDs caused some insplan values to not carry forward.
				Db.NonQ(command);
				command="UPDATE insplan SET GroupNum='' WHERE GroupNum IS NULL";
				Db.NonQ(command);
				command="UPDATE insplan SET GroupName='' WHERE GroupName IS NULL";
				Db.NonQ(command);
				//Master plan for fixing references to plannums throughout the program--------------------------------------
				//But many of these only apply to plansShared.
				//appointment.InsPlan1/2 -- UPDATE InsPlan1/2
				//benefit.PlanNum -- DELETE unused
				//claim.PlanNum/PlanNum2 -- UPDATE PlanNum/2, add claim.InsSubNum/2
				//claimproc.PlanNum -- UPDATE PlanNum, add claimproc.InsSubNum
				//etrans.PlanNum -- UPDATE PlanNum, add etrans.InsSubNum
				//patplan.PlanNum -- UPDATE PlanNum, add patplan.InsSubNum
				//payplan.PlanNum -- UPDATE PlanNum, add payplan.InsSubNum
				command="ALTER TABLE claim ADD InsSubNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE claim ADD INDEX (InsSubNum)";
				Db.NonQ(command);
				command="ALTER TABLE claim ADD InsSubNum2 bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE claim ADD INDEX (InsSubNum2)";
				Db.NonQ(command);
				command="ALTER TABLE claimproc ADD InsSubNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE claimproc ADD INDEX (InsSubNum)";
				Db.NonQ(command);
				command="ALTER TABLE etrans ADD InsSubNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE etrans ADD INDEX (InsSubNum)";
				Db.NonQ(command);
				command="ALTER TABLE patplan ADD InsSubNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE patplan ADD INDEX (InsSubNum)";
				Db.NonQ(command);
				command="ALTER TABLE payplan ADD InsSubNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE payplan ADD INDEX (InsSubNum)";
				Db.NonQ(command);
				command="SELECT ValueString FROM preference WHERE PrefName = 'InsPlanConverstion_7_5_17_AutoMergeYN'";//This line was added in 7.5.17.
				bool autoMerge=(Db.GetScalar(command)=="1");//Yes
				if(autoMerge) {//This option was added in 7.5.17.
					//Create a temporary table that will hold a copy of all the original plans.
					command="DROP TABLE IF EXISTS tempinsplan";
					Db.NonQ(command);
					command="CREATE TABLE tempinsplan SELECT * FROM insplan";
					Db.NonQ(command);
					command="ALTER TABLE tempinsplan ADD NewPlanNum bigint NOT NULL";//This new column is the only thing different about this table.
					Db.NonQ(command);
					command="ALTER TABLE tempinsplan ADD INDEX (NewPlanNum)";
					Db.NonQ(command);
					command="ALTER TABLE tempinsplan ADD INDEX (PlanNum)";
					Db.NonQ(command);
					command="ALTER TABLE tempinsplan ADD INDEX (Subscriber)";
					Db.NonQ(command);
					command="ALTER TABLE tempinsplan ADD INDEX (CarrierNum)";
					Db.NonQ(command);
					//Create a temporary table that will hold a copy of all the unique plans
					command="DROP TABLE IF EXISTS tempunique";
					Db.NonQ(command);
					command="CREATE TABLE tempunique SELECT * FROM insplan GROUP BY EmployerNum,GroupName,GroupNum,DivisionNo,CarrierNum,IsMedical,TrojanID,FeeSched";
					Db.NonQ(command);
					command="ALTER TABLE tempunique ADD INDEX (PlanNum)";
					Db.NonQ(command);
					command="ALTER TABLE tempunique ADD INDEX (Subscriber)";
					Db.NonQ(command);
					command="ALTER TABLE tempunique ADD INDEX (CarrierNum)";
					Db.NonQ(command);
					command="UPDATE tempinsplan,tempunique SET tempinsplan.NewPlanNum=tempunique.PlanNum "
						+"WHERE tempinsplan.EmployerNum=tempunique.EmployerNum "
						+"AND tempinsplan.GroupName=tempunique.GroupName "
						+"AND tempinsplan.GroupNum=tempunique.GroupNum "
						+"AND tempinsplan.DivisionNo=tempunique.DivisionNo "
						+"AND tempinsplan.CarrierNum=tempunique.CarrierNum "
						+"AND tempinsplan.IsMedical=tempunique.IsMedical "
						+"AND tempinsplan.TrojanID=tempunique.TrojanID "
						+"AND tempinsplan.FeeSched=tempunique.FeeSched";
					Db.NonQ(command);
					//Now, create a series of inssub rows, one for each of the original plans.
					//But instead of referencing the original planNum, reference the NewPlanNum
					command="INSERT INTO inssub (PlanNum,Subscriber,DateEffective,DateTerm,ReleaseInfo,AssignBen,SubscriberID,BenefitNotes,SubscNote,OldPlanNum) "
						+"SELECT NewPlanNum,Subscriber,DateEffective,DateTerm,ReleaseInfo,AssignBen,SubscriberID,BenefitNotes,SubscNote,PlanNum "
						+"FROM tempinsplan";
					Db.NonQ(command);
					command="DROP TABLE IF EXISTS tempinsplan";//to emphasize that we will not be using it again.
					Db.NonQ(command);
					//fix references to plannums throughout the program---------------------------------------------------
					//appointment.InsPlan1/2 -- UPDATE InsPlan1/2
					command="ALTER TABLE appointment ADD OldInsPlan1 bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE appointment ADD INDEX (OldInsPlan1)";
					Db.NonQ(command);
					command="UPDATE appointment SET OldInsPlan1 = InsPlan1";
					Db.NonQ(command);
					command="ALTER TABLE appointment ADD OldInsPlan2 bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE appointment ADD INDEX (OldInsPlan2)";
					Db.NonQ(command);
					command="UPDATE appointment SET OldInsPlan2 = InsPlan2";
					Db.NonQ(command);
					command="UPDATE appointment SET InsPlan1=(SELECT PlanNum FROM inssub WHERE inssub.OldPlanNum=appointment.OldInsPlan1) WHERE InsPlan1 != 0";
					Db.NonQ(command);
					command="UPDATE appointment SET InsPlan2=(SELECT PlanNum FROM inssub WHERE inssub.OldPlanNum=appointment.OldInsPlan2) WHERE InsPlan2 != 0";
					Db.NonQ(command);
					//benefit.PlanNum -- DELETE unused
					command="DELETE FROM benefit WHERE PlanNum > 0 AND NOT EXISTS(SELECT * FROM tempunique WHERE tempunique.PlanNum=benefit.PlanNum)";
					Db.NonQ(command);
					//claim.PlanNum/PlanNum2 -- UPDATE PlanNum/2
					command="ALTER TABLE claim ADD OldPlanNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim ADD INDEX (OldPlanNum)";
					Db.NonQ(command);
					command="UPDATE claim SET OldPlanNum = PlanNum";
					Db.NonQ(command);
					command="ALTER TABLE claim ADD OldPlanNum2 bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim ADD INDEX (OldPlanNum2)";
					Db.NonQ(command);
					command="UPDATE claim SET OldPlanNum2 = PlanNum2";
					Db.NonQ(command);
					command="UPDATE claim SET PlanNum=(SELECT PlanNum FROM inssub WHERE inssub.OldPlanNum=claim.OldPlanNum) WHERE PlanNum != 0";
					Db.NonQ(command);
					command="UPDATE claim SET PlanNum2=(SELECT PlanNum FROM inssub WHERE inssub.OldPlanNum=claim.OldPlanNum2) WHERE PlanNum2 != 0";
					Db.NonQ(command);
					//claimproc.PlanNum -- UPDATE PlanNum
					command="ALTER TABLE claimproc ADD OldPlanNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimproc ADD INDEX (OldPlanNum)";
					Db.NonQ(command);
					command="UPDATE claimproc SET OldPlanNum = PlanNum";
					Db.NonQ(command);
					command="UPDATE claimproc SET PlanNum=(SELECT PlanNum FROM inssub WHERE inssub.OldPlanNum=claimproc.OldPlanNum) WHERE PlanNum != 0";
					Db.NonQ(command);
					//etrans.PlanNum -- UPDATE PlanNum
					command="ALTER TABLE etrans ADD OldPlanNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE etrans ADD INDEX (OldPlanNum)";
					Db.NonQ(command);
					command="UPDATE etrans SET OldPlanNum = PlanNum";
					Db.NonQ(command);
					command="UPDATE etrans SET PlanNum=(SELECT PlanNum FROM inssub WHERE inssub.OldPlanNum=etrans.OldPlanNum) WHERE PlanNum != 0";
					Db.NonQ(command);
					//patplan.PlanNum -- UPDATE PlanNum
					command="ALTER TABLE patplan ADD OldPlanNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE patplan ADD INDEX (OldPlanNum)";
					Db.NonQ(command);
					command="UPDATE patplan SET OldPlanNum = PlanNum";
					Db.NonQ(command);
					command="UPDATE patplan SET PlanNum=(SELECT PlanNum FROM inssub WHERE inssub.OldPlanNum=patplan.OldPlanNum) WHERE PlanNum != 0";
					Db.NonQ(command);
					//payplan.PlanNum -- UPDATE PlanNum
					command="ALTER TABLE payplan ADD OldPlanNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE payplan ADD INDEX (OldPlanNum)";
					Db.NonQ(command);
					command="UPDATE payplan SET OldPlanNum = PlanNum";
					Db.NonQ(command);
					command="UPDATE payplan SET PlanNum=(SELECT PlanNum FROM inssub WHERE inssub.OldPlanNum=payplan.OldPlanNum) WHERE PlanNum != 0";
					Db.NonQ(command);
					//Now, drop all the unused plans-------------------------------------------------------------------------------------------
					command="DELETE FROM insplan WHERE NOT EXISTS(SELECT * FROM tempunique WHERE tempunique.PlanNum=insplan.PlanNum)";
					Db.NonQ(command);
					//Set all the InsSubNums.--------------------------------------------------------------------------------------------------
					//claim.PlanNum/PlanNum2 -- claim.InsSubNum/2
					command="UPDATE claim SET InsSubNum = (SELECT InsSubNum FROM inssub WHERE inssub.OldPlanNum=claim.OldPlanNum) WHERE PlanNum > 0";
					Db.NonQ(command);
					command="UPDATE claim SET InsSubNum2 = (SELECT InsSubNum FROM inssub WHERE inssub.OldPlanNum=claim.OldPlanNum2) WHERE PlanNum2 > 0";
					Db.NonQ(command);
					//claimproc.PlanNum -- claimproc.InsSubNum
					command="UPDATE claimproc SET InsSubNum = (SELECT InsSubNum FROM inssub WHERE inssub.OldPlanNum=claimproc.OldPlanNum) WHERE PlanNum > 0";
					Db.NonQ(command);
					//etrans.PlanNum -- etrans.InsSubNum
					command="UPDATE etrans SET InsSubNum = (SELECT InsSubNum FROM inssub WHERE inssub.OldPlanNum=etrans.OldPlanNum) WHERE PlanNum > 0";
					Db.NonQ(command);
					//patplan.PlanNum -- patplan.InsSubNum
					command="UPDATE patplan SET InsSubNum = (SELECT InsSubNum FROM inssub WHERE inssub.OldPlanNum=patplan.OldPlanNum) WHERE PlanNum > 0";
					Db.NonQ(command);
					//payplan.PlanNum -- payplan.InsSubNum
					command="UPDATE payplan SET InsSubNum = (SELECT InsSubNum FROM inssub WHERE inssub.OldPlanNum=payplan.OldPlanNum) WHERE PlanNum > 0";
					Db.NonQ(command);
					//Drop temp columns and tables-------------------------------------------------------------------------------------------------
					command="ALTER TABLE inssub DROP OldPlanNum";
					Db.NonQ(command);
					command="ALTER TABLE appointment DROP OldInsPlan1";
					Db.NonQ(command);
					command="ALTER TABLE appointment DROP OldInsPlan2";
					Db.NonQ(command);
					command="ALTER TABLE claim DROP OldPlanNum";
					Db.NonQ(command);
					command="ALTER TABLE claim DROP OldPlanNum2";
					Db.NonQ(command);
					command="ALTER TABLE claimproc DROP OldPlanNum";
					Db.NonQ(command);
					command="ALTER TABLE etrans DROP OldPlanNum";
					Db.NonQ(command);
					command="ALTER TABLE patplan DROP OldPlanNum";
					Db.NonQ(command);
					command="ALTER TABLE payplan DROP OldPlanNum";
					Db.NonQ(command);
					command="DROP TABLE IF EXISTS tempunique";
					Db.NonQ(command);
				}
				else {//This option was added in 7.5.17. No combining of plans will happen
					command="INSERT INTO inssub (PlanNum,Subscriber,DateEffective,DateTerm,ReleaseInfo,AssignBen,SubscriberID,BenefitNotes,SubscNote,OldPlanNum) "
						+"SELECT PlanNum,Subscriber,DateEffective,DateTerm,ReleaseInfo,AssignBen,SubscriberID,BenefitNotes,SubscNote,PlanNum "
						+"FROM insplan";
					Db.NonQ(command);
					//Set all InsSubNums----------------------------------------------------------------------------------------------------
					//claim.PlanNum/PlanNum2 -- claim.InsSubNum/2
					command="UPDATE claim SET InsSubNum = (SELECT InsSubNum FROM inssub WHERE inssub.PlanNum=claim.PlanNum) WHERE PlanNum > 0";
					Db.NonQ(command);
					command="UPDATE claim SET InsSubNum2 = (SELECT InsSubNum FROM inssub WHERE inssub.PlanNum=claim.PlanNum2) WHERE PlanNum2 > 0";
					Db.NonQ(command);
					//claimproc.PlanNum -- claimproc.InsSubNum
					command="UPDATE claimproc SET InsSubNum = (SELECT InsSubNum FROM inssub WHERE inssub.PlanNum=claimproc.PlanNum) WHERE PlanNum > 0";
					Db.NonQ(command);
					//etrans.PlanNum -- etrans.InsSubNum
					command="UPDATE etrans SET InsSubNum = (SELECT InsSubNum FROM inssub WHERE inssub.PlanNum=etrans.PlanNum) WHERE PlanNum > 0";
					Db.NonQ(command);
					//patplan.PlanNum -- patplan.InsSubNum
					command="UPDATE patplan SET InsSubNum = (SELECT InsSubNum FROM inssub WHERE inssub.PlanNum=patplan.PlanNum) WHERE PlanNum > 0";
					Db.NonQ(command);
					//payplan.PlanNum -- payplan.InsSubNum
					command="UPDATE payplan SET InsSubNum = (SELECT InsSubNum FROM inssub WHERE inssub.PlanNum=payplan.PlanNum) WHERE PlanNum > 0";
					Db.NonQ(command);
				}
				//Final changes to tables-----------------------------------------------------------------------------------------------------
				command="ALTER TABLE insplan DROP Subscriber";
				Db.NonQ(command);
				command="ALTER TABLE insplan DROP DateEffective";
				Db.NonQ(command);
				command="ALTER TABLE insplan DROP DateTerm";
				Db.NonQ(command);
				command="ALTER TABLE insplan DROP ReleaseInfo";
				Db.NonQ(command);
				command="ALTER TABLE insplan DROP AssignBen";
				Db.NonQ(command);
				command="ALTER TABLE insplan DROP SubscriberID";
				Db.NonQ(command);
				command="ALTER TABLE insplan DROP BenefitNotes";
				Db.NonQ(command);
				command="ALTER TABLE insplan DROP SubscNote";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.5.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_5_4();
		}

		private static void To7_5_4() {
			if(FromVersion<new Version("7.5.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.5.4");//No translation in convert script.
				string command;
				command="DELETE FROM toolbutitem WHERE ProgramNum=(SELECT p.ProgramNum FROM program p WHERE p.ProgName='PayConnect' LIMIT 1)";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.5.4.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_5_7();
		}

		private static void To7_5_7() {
			if(FromVersion<new Version("7.5.7.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.5.7");//No translation in convert script.
				string command;
				command="INSERT INTO preference(PrefName,ValueString) VALUES('SubscriberAllowChangeAlways','0')";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.5.7.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_5_12();
		}

		private static void To7_5_12() {
			if(FromVersion<new Version("7.5.12.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.5.12");//No translation in convert script.
				string command;
				command="ALTER TABLE inssub CHANGE BenefitNotes BenefitNotes text NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE inssub CHANGE SubscNote SubscNote text NOT NULL";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.5.12.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_5_16();
		}

		private static void To7_5_16() {
			if(FromVersion<new Version("7.5.16.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.5.16");//No translation in convert script.
				string command;
				command="ALTER TABLE orionproc ADD DPCpost tinyint NOT NULL";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.5.16.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_6_1();
		}

		private static void To7_6_1() {
			if(FromVersion<new Version("7.6.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.6.1");//No translation in convert script.
				string command;
				command="UPDATE program SET ProgDesc='PayConnect from www.dentalxchange.com' WHERE ProgName='PayConnect' LIMIT 1";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = 'https://opendentalsoft.com/WebHostSynch/Sheets.asmx' WHERE PrefName = 'WebHostSynchServerURL'";
				Db.NonQ(command);
				command="ALTER TABLE operatory ADD SetProspective tinyint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE computerpref CHANGE SensorBinned SensorBinned tinyint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE computerpref CHANGE GraphicsDoubleBuffering GraphicsDoubleBuffering tinyint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE computerpref ADD RecentApptView tinyint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE covcat CHANGE DefaultPercent DefaultPercent smallint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE periomeasure CHANGE IntTooth IntTooth smallint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE periomeasure CHANGE ToothValue ToothValue smallint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE periomeasure CHANGE MBvalue MBvalue smallint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE periomeasure CHANGE Bvalue Bvalue smallint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE periomeasure CHANGE DBvalue DBvalue smallint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE periomeasure CHANGE MLvalue MLvalue smallint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE periomeasure CHANGE Lvalue Lvalue smallint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE periomeasure CHANGE DLvalue DLvalue smallint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE sigbutdef CHANGE ButtonIndex ButtonIndex smallint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE preference DROP PRIMARY KEY";
				Db.NonQ(command);
				command="ALTER TABLE preference ADD COLUMN PrefNum bigint NOT NULL auto_increment FIRST, ADD PRIMARY KEY (PrefNum)";
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('MobileSyncIntervalMinutes','0')";
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('MobileSyncServerURL','https://opendentalsoft.com/WebHostSynch/Mobile.asmx')";
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('MobileExcludeApptsBeforeDate','2009-12-20')";
				Db.NonQ(command);
				command="DELETE FROM preference WHERE PrefName = 'MobileSyncLastFileNumber'";
				Db.NonQ(command);
				command="DELETE FROM preference WHERE PrefName = 'MobileSyncPath'";
				Db.NonQ(command);
				command="ALTER TABLE county DROP PRIMARY KEY";
				Db.NonQ(command);
				command="ALTER TABLE county ADD COLUMN CountyNum bigint NOT NULL auto_increment FIRST, ADD PRIMARY KEY (CountyNum)";
				Db.NonQ(command);
				command="ALTER TABLE language DROP PRIMARY KEY";
				try {
					Db.NonQ(command);
				}
				catch { }//because I don't think there is any primary key for that table.
				command="ALTER TABLE language ADD COLUMN LanguageNum bigint NOT NULL auto_increment FIRST, ADD PRIMARY KEY (LanguageNum)";
				Db.NonQ(command);
				command="ALTER TABLE languageforeign DROP PRIMARY KEY";
				try {
					Db.NonQ(command);
				}
				catch { }
				command="ALTER TABLE languageforeign ADD COLUMN LanguageForeignNum bigint NOT NULL auto_increment FIRST, ADD PRIMARY KEY (LanguageForeignNum)";
				Db.NonQ(command);
				command="ALTER TABLE school DROP PRIMARY KEY";
				Db.NonQ(command);
				command="ALTER TABLE school ADD COLUMN SchoolNum bigint NOT NULL auto_increment FIRST, ADD PRIMARY KEY (SchoolNum)";
				Db.NonQ(command);
				//DbSchema.AddColumn("SchoolNum",OdDbType.Long);
				command="ALTER TABLE payment ADD Receipt text NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE sigelementdef CHANGE ItemOrder ItemOrder smallint NOT NULL";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.6.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_6_2();
		}

		private static void To7_6_2() {
			if(FromVersion<new Version("7.6.2.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.6.2");//No translation in convert script.
				string command;
				command="ALTER TABLE preference DROP COLUMN PrefNum";
				Db.NonQ(command);
				command="ALTER TABLE preference ADD COLUMN PrefNum bigint NOT NULL auto_increment AFTER ValueString, ADD PRIMARY KEY (PrefNum)";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.6.2.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_6_4();
		}

		private static void To7_6_4() {
			if(FromVersion<new Version("7.6.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.6.4");//No translation in convert script.
				string command;
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ReportPandIschedProdSubtractsWO','0')";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.6.4.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_6_10();
		}

		private static void To7_6_10() {
			if(FromVersion<new Version("7.6.10.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.6.10");//No translation in convert script.
				string command;
				command="SELECT ProgramNum FROM program WHERE ProgName='Xcharge'";
				long programNum=PIn.Long(Db.GetScalar(command));
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+POut.Long(programNum)+"', "
					+"'Username', "
					+"'')";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+POut.Long(programNum)+"', "
					+"'Password', "
					+"'')";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.6.10.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
				}
			To7_7_1();
		}

		private static void To7_7_1() {
			if(FromVersion<new Version("7.7.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.7.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE document ADD RawBase64 mediumtext NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE document ADD RawBase64 clob";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE document ADD Thumbnail text NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE document ADD Thumbnail clob";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE rxpat ADD DateTStamp timestamp";
					Db.NonQ(command);
					command="UPDATE rxpat SET DateTStamp = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE rxpat ADD DateTStamp timestamp";
					Db.NonQ(command);
					command="UPDATE rxpat SET DateTStamp = SYSDATE";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('MobileSyncWorkstationName','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'MobileSyncWorkstationName','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS tasknote";
					Db.NonQ(command);
					command=@"CREATE TABLE tasknote (
						TaskNoteNum bigint NOT NULL auto_increment PRIMARY KEY,
						TaskNum bigint NOT NULL,
						UserNum bigint NOT NULL,
						DateTimeNote datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						Note Text NOT NULL,
						INDEX(TaskNum),
						INDEX(UserNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE tasknote'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE tasknote (
						TaskNoteNum number(20) NOT NULL,
						TaskNum number(20) NOT NULL,
						UserNum number(20) NOT NULL,
						DateTimeNote date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						Note varchar2(4000),
						CONSTRAINT tasknote_TaskNoteNum PRIMARY KEY (TaskNoteNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX tasknote_TaskNum ON tasknote (TaskNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX tasknote_UserNum ON tasknote (UserNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('TasksNewTrackedByUser','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'TasksNewTrackedByUser','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ImagesModuleTreeIsCollapsed','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ImagesModuleTreeIsCollapsed','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('TasksShowOpenTickets','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'TasksShowOpenTickets','0')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '7.7.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_8_1();
		}

		private static void To7_8_1() {
			if(FromVersion<new Version("7.8.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.8.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS creditcard";
					Db.NonQ(command);
					command=@"CREATE TABLE creditcard (
						CreditCardNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						Address varchar(255),
						Zip varchar(255),
						XChargeToken varchar(255),
						CCNumberMasked varchar(255),
						CCExpiration date NOT NULL DEFAULT '0001-01-01',
						ItemOrder int NOT NULL,
						INDEX(PatNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE creditcard'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE creditcard (
						CreditCardNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						Address varchar2(255),
						Zip varchar2(255),
						XChargeToken varchar2(255),
						CCNumberMasked varchar2(255),
						CCExpiration date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						ItemOrder number(11) NOT NULL,
						CONSTRAINT creditcard_CreditCardNum PRIMARY KEY (CreditCardNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX creditcard_PatNum ON creditcard (PatNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO creditcard (PatNum,CCExpiration,CCNumberMasked,ItemOrder) SELECT PatNum,CCExpiration,CCNumber,0 FROM patientnote WHERE CCNumber!=''";
					Db.NonQ(command);
				}
				else {//oracle
					command=@"INSERT INTO creditcard (CreditCardNum,PatNum,CCExpiration,CCNumberMasked,ItemOrder) SELECT PatNum,PatNum,CCExpiration,CCNumber,0 FROM patientnote WHERE LENGTH(ccnumber)>0";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patientnote DROP COLUMN CCNumber";
					Db.NonQ(command);
					command="ALTER TABLE patientnote DROP COLUMN CCExpiration";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patientnote DROP (CCNumber, CCExpiration)";
					Db.NonQ(command);
				}
				//Add PerioEdit permission to all groups------------------------------------------------------
				command="SELECT UserGroupNum FROM usergroup";
				DataTable table=Db.GetTable(command);
				long groupNum;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i][0].ToString());
						command="INSERT INTO grouppermission (NewerDays,UserGroupNum,PermType) "
							+"VALUES(0,"+POut.Long(groupNum)+","+POut.Int((int)Permissions.PerioEdit)+")";
						Db.NonQ32(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i][0].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+","+POut.Int((int)Permissions.PerioEdit)+")";
						Db.NonQ32(command);
					}
				}
				//add Cerec bridge:
				if(DataConnection.DBtype==DatabaseType.MySql) {//NOTE: this chunk of code was realeased with MySQL version only, and then revised to use mySQL or Oracle. The mySQL code was unchanged.
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'Cerec', "
						+"'Cerec from Sirona', "
						+"'0', "
						+"'"+POut.String(@"C:\Program Files\Cerec\Cerec system\CerPI.exe")+"', "
						+"'', "
						+@"'Cerec v2.6 default install directory is C:\Program Files\Cerec\System\CerPI.exe 
						\r\nCerec v2.8 default install directory is C:\Program Files\Cerec\Cerec system\CerPI.exe')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
						+"'0')";
					Db.NonQ32(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"'"+POut.Long(programNum)+"', "
						+"'"+POut.Int(((int)ToolBarsAvail.ChartModule))+"', "
						+"'Cerec')";
					Db.NonQ32(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'Cerec', "
						+"'Cerec from Sirona', "
						+"'0', "
						+"'"+POut.String(@"C:\Program Files\Cerec\Cerec system\CerPI.exe")+"', "
						+"'', "
						+@"'Cerec v2.6 default install directory is C:\Program Files\Cerec\System\CerPI.exe 
						\r\nCerec v2.8 default install directory is C:\Program Files\Cerec\Cerec system\CerPI.exe')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum)+1 FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
						+"'0')";
					Db.NonQ32(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
						+"'"+POut.Long(programNum)+"', "
						+"'"+POut.Int(((int)ToolBarsAvail.ChartModule))+"', "
						+"'Cerec')";
					Db.NonQ32(command);
				}//End of Cerec Bridge code. This chunk of code works with MySQL, and is unlikely to cause Oracle bugs.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS school";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE school'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE chartview ADD DatesShowing float NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE chartview ADD DatesShowing number(38,8)";
					Db.NonQ(command);
					command="UPDATE chartview SET DatesShowing = 0 WHERE DatesShowing IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE chartview MODIFY DatesShowing NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurelog ADD Prognosis bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog ADD INDEX (Prognosis)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurelog ADD Prognosis number(20)";
					Db.NonQ(command);
					command="UPDATE procedurelog SET Prognosis = 0 WHERE Prognosis IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog MODIFY Prognosis NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX procedurelog_Prognosis ON procedurelog (Prognosis)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE proctp ADD Prognosis varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE proctp ADD Prognosis varchar2(255)";
					Db.NonQ(command);
				}
				//Add ProcEditShowFee permission to all groups------------------------------------------------------
				command="SELECT UserGroupNum FROM usergroup";
				table=Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i][0].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						+"VALUES("+POut.Long(groupNum)+","+POut.Int((int)Permissions.ProcEditShowFee)+")";
						Db.NonQ32(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i][0].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
						+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+","+POut.Int((int)Permissions.ProcEditShowFee)+")";
						Db.NonQ32(command);
					}
				}
				command="UPDATE preference SET ValueString = '7.8.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_8_3();
		}

		private static void To7_8_3() {
			if(FromVersion<new Version("7.8.3.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.8.3");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('MobileUserName','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'MobileUserName','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE proctp ADD Dx varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE proctp ADD Dx varchar2(255)";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '7.8.3.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_8_4();
		}

		private static void To7_8_4() {
			if(FromVersion<new Version("7.8.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.8.4.");//No translation in convert script.
				string command;
				//add Patterson Imaging bridge:
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'Patterson', "
						+"'Patterson Imaging from Patterson Dental Supply Inc.', "
						+"'0', "
						+"'"+POut.String(@"C:\Program Files\PDI\Shared files\Imaging.exe")+"',"
						+"'', "
						+"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'System path to Patterson Imaging ini', "
						+"'"+POut.String(@"C:\Program Files\PDI\Shared files\Imaging.ini")+"')";
					Db.NonQ32(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"'"+POut.Long(programNum)+"', "
						+"'"+POut.Int(((int)ToolBarsAvail.ChartModule))+"', "
						+"'PattersonImg')";
					Db.NonQ32(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'Patterson', "
						+"'Patterson Imaging from Patterson Dental Supply Inc.', "
						+"'0', "
						+"'"+POut.String(@"C:\Program Files\PDI\Shared files\Imaging.exe")+"',"
						+"'', "
						+"'')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum)+1 FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'System path to Patterson Imaging ini', "
						+"'"+POut.String(@"C:\Program Files\PDI\Shared files\Imaging.ini")+"')";
					Db.NonQ32(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
						+"'"+POut.Long(programNum)+"', "
						+"'"+POut.Int(((int)ToolBarsAvail.ChartModule))+"', "
						+"'PattersonImg')";
					Db.NonQ32(command);
				}//end Patterson Imaging bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE chartview MODIFY DatesShowing TINYINT NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					//Does not need to be changed for oracle.
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('MySqlVersion','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'MySqlVersion','')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '7.8.4.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_8_5();
		}

		private static void To7_8_5() {
			if(FromVersion<new Version("7.8.5.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.8.5");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {//signal is a reserved word in mySQL 5.5
					command="DROP TABLE IF EXISTS signalod";
					Db.NonQ(command);
					command="RENAME TABLE `signal` TO signalod";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE signalod'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command="ALTER TABLE signal RENAME TO signalod";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '7.8.5.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_9_1();
		}

		private static void To7_9_1() {
			if(FromVersion<new Version("7.9.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.9.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD CanadaTransRefNum varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD CanadaTransRefNum varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD CanadaEstTreatStartDate date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD CanadaEstTreatStartDate date";
					Db.NonQ(command);
					command="UPDATE claim SET CanadaEstTreatStartDate = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE CanadaEstTreatStartDate IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim MODIFY CanadaEstTreatStartDate NOT NULL";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD CanadaInitialPayment double NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD CanadaInitialPayment number(38,8)";
					Db.NonQ(command);
					command="UPDATE claim SET CanadaInitialPayment = 0 WHERE CanadaInitialPayment IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim MODIFY CanadaInitialPayment NOT NULL";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD CanadaPaymentMode tinyint unsigned NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD CanadaPaymentMode number(3)";
					Db.NonQ(command);
					command="UPDATE claim SET CanadaPaymentMode = 0 WHERE CanadaPaymentMode IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim MODIFY CanadaPaymentMode NOT NULL";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD CanadaTreatDuration tinyint unsigned NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD CanadaTreatDuration number(3)";
					Db.NonQ(command);
					command="UPDATE claim SET CanadaTreatDuration = 0 WHERE CanadaTreatDuration IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim MODIFY CanadaTreatDuration NOT NULL";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD CanadaNumAnticipatedPayments tinyint unsigned NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD CanadaNumAnticipatedPayments number(3)";
					Db.NonQ(command);
					command="UPDATE claim SET CanadaNumAnticipatedPayments = 0 WHERE CanadaNumAnticipatedPayments IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim MODIFY CanadaNumAnticipatedPayments NOT NULL";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD CanadaAnticipatedPayAmount double NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD CanadaAnticipatedPayAmount number(38,8)";
					Db.NonQ(command);
					command="UPDATE claim SET CanadaAnticipatedPayAmount = 0 WHERE CanadaAnticipatedPayAmount IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim MODIFY CanadaAnticipatedPayAmount NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE creditcard ADD ChargeAmt double NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE creditcard ADD ChargeAmt number(38,8)";
					Db.NonQ(command);
					command="UPDATE creditcard SET ChargeAmt = 0 WHERE ChargeAmt IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE creditcard MODIFY ChargeAmt NOT NULL";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE creditcard ADD DateStart date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE creditcard ADD DateStart date";
					Db.NonQ(command);
					command="UPDATE creditcard SET DateStart = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateStart IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE creditcard MODIFY DateStart NOT NULL";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE creditcard ADD DateStop date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE creditcard ADD DateStop date";
					Db.NonQ(command);
					command="UPDATE creditcard SET DateStop = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateStop IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE creditcard MODIFY DateStop NOT NULL";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE creditcard ADD Note varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE creditcard ADD Note varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS vitalsign";
					Db.NonQ(command);
					command=@"CREATE TABLE vitalsign (
						VitalsignNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						Height float NOT NULL,
						Weight float NOT NULL,
						BpSystolic smallint NOT NULL,
						BpDiastolic smallint NOT NULL,
						DateTaken date NOT NULL DEFAULT '0001-01-01',
						INDEX(PatNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE vitalsign'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE vitalsign (
						VitalsignNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						Height number(38,8) NOT NULL,
						Weight number(38,8) NOT NULL,
						BpSystolic number(11) NOT NULL,
						BpDiastolic number(11) NOT NULL,
						DateTaken date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						CONSTRAINT vitalsign_VitalsignNum PRIMARY KEY (VitalsignNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX vitalsign_PatNum ON vitalsign (PatNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patient ADD OnlinePassword varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patient ADD OnlinePassword varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE medicationpat ADD DateTStamp timestamp";
					Db.NonQ(command);
					command="UPDATE medicationpat SET DateTStamp = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE medicationpat ADD DateTStamp timestamp";
					Db.NonQ(command);
					command="UPDATE medicationpat SET DateTStamp = SYSTIMESTAMP";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE medicationpat ADD IsDiscontinued tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE medicationpat ADD IsDiscontinued number(3)";
					Db.NonQ(command);
					command="UPDATE medicationpat SET IsDiscontinued = 0 WHERE IsDiscontinued IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE medicationpat MODIFY IsDiscontinued NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE disease ADD DateTStamp timestamp";
					Db.NonQ(command);
					command="UPDATE disease SET DateTStamp = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE disease ADD DateTStamp timestamp";
					Db.NonQ(command);
					command="UPDATE disease SET DateTStamp = SYSTIMESTAMP";
					Db.NonQ(command);
				}
				//columns for rxalert----------------------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE rxalert ADD AllergyDefNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE rxalert ADD INDEX (AllergyDefNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE rxalert ADD AllergyDefNum number(20)";
					Db.NonQ(command);
					command="UPDATE rxalert SET AllergyDefNum = 0 WHERE AllergyDefNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE rxalert MODIFY AllergyDefNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX rxalert_AllergyDefNum ON rxalert (AllergyDefNum)";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE rxalert ADD MedicationNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE rxalert ADD INDEX (MedicationNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE rxalert ADD MedicationNum number(20)";
					Db.NonQ(command);
					command="UPDATE rxalert SET MedicationNum = 0 WHERE MedicationNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE rxalert MODIFY MedicationNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX rxalert_MedicationNum ON rxalert (MedicationNum)";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE rxalert ADD NotificationMsg varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE rxalert ADD NotificationMsg varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE rxpat ADD IsElectQueue tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE rxpat ADD IsElectQueue number(3)";
					Db.NonQ(command);
					command="UPDATE rxpat SET IsElectQueue = 0 WHERE IsElectQueue IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE rxpat MODIFY IsElectQueue NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('RxSendNewToQueue','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT MAX(PrefNum)+1 FROM preference),'RxSendNewToQueue','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patient ADD SmokeStatus tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patient ADD SmokeStatus number(3)";
					Db.NonQ(command);
					command="UPDATE patient SET SmokeStatus = 0 WHERE SmokeStatus IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE patient MODIFY SmokeStatus NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE canadiannetwork ADD CanadianTransactionPrefix varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE canadiannetwork ADD CanadianTransactionPrefix varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS icd9";
					Db.NonQ(command);
					command=@"CREATE TABLE icd9 (
						ICD9Num bigint NOT NULL auto_increment PRIMARY KEY,
						ICD9Code varchar(255) NOT NULL,
						Description varchar(255) NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE icd9'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE icd9 (
						ICD9Num number(20) NOT NULL,
						ICD9Code varchar2(255),
						Description varchar2(255),
						CONSTRAINT icd9_ICD9Num PRIMARY KEY (ICD9Num)
						)";
					Db.NonQ(command);
				}
				//Removed for 13.3 release, will be using code system importer tool instead.
				//try {
				//	using(StringReader reader=new StringReader(Properties.Resources.icd9)) {
				//		//loop:
				//		command=reader.ReadLine();
				//		while(command!=null) {
				//			Db.NonQ(command);
				//			command=reader.ReadLine();
				//		}
				//	}
				//}
				//catch(Exception) { }//do nothing
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS medicalorder";
					Db.NonQ(command);
					command=@"CREATE TABLE medicalorder (
						MedicalOrderNum bigint NOT NULL auto_increment PRIMARY KEY,
						MedOrderType tinyint NOT NULL,
						PatNum bigint NOT NULL,
						DateTimeOrder datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						INDEX(PatNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE medicalorder'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE medicalorder (
						MedicalOrderNum number(20) NOT NULL,
						MedOrderType number(3) NOT NULL,
						PatNum number(20) NOT NULL,
						DateTimeOrder date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						CONSTRAINT medicalorder_MedicalOrderNum PRIMARY KEY (MedicalOrderNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX medicalorder_PatNum ON medicalorder (PatNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS allergy";
					Db.NonQ(command);
					command=@"CREATE TABLE allergy (
						AllergyNum bigint NOT NULL auto_increment PRIMARY KEY,
						AllergyDefNum bigint NOT NULL,
						PatNum bigint NOT NULL,
						Reaction varchar(255) NOT NULL,
						StatusIsActive tinyint NOT NULL,
						DateTStamp timestamp,
						INDEX(AllergyDefNum),
						INDEX(PatNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE allergy'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE allergy (
						AllergyNum number(20) NOT NULL,
						AllergyDefNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						Reaction varchar2(255),
						StatusIsActive number(3) NOT NULL,
						DateTStamp timestamp,
						CONSTRAINT allergy_AllergyNum PRIMARY KEY (AllergyNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX allergy_AllergyDefNum ON allergy (AllergyDefNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX allergy_PatNum ON allergy (PatNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS allergydef";
					Db.NonQ(command);
					command=@"CREATE TABLE allergydef (
						AllergyDefNum bigint NOT NULL auto_increment PRIMARY KEY,
						Description varchar(255) NOT NULL,
						IsHidden tinyint NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE allergydef'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE allergydef (
						AllergyDefNum number(20) NOT NULL,
						Description varchar2(255),
						IsHidden number(3) NOT NULL,
						CONSTRAINT allergydef_AllergyDefNum PRIMARY KEY (AllergyDefNum)
						)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE disease ADD ICD9Num bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE disease ADD INDEX (ICD9Num)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE disease ADD ICD9Num number(20)";
					Db.NonQ(command);
					command="UPDATE disease SET ICD9Num = 0 WHERE ICD9Num IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE disease MODIFY ICD9Num NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX disease_ICD9Num ON disease (ICD9Num)";
					Db.NonQ(command);
				} if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE disease ADD ProbStatus tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE disease ADD ProbStatus number(3)";
					Db.NonQ(command);
					command="UPDATE disease SET ProbStatus = 0 WHERE ProbStatus IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE disease MODIFY ProbStatus NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ehrmeasure";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrmeasure (
						EhrMeasureNum bigint NOT NULL auto_increment PRIMARY KEY,
						MeasureType tinyint NOT NULL,
						Numerator smallint NOT NULL,
						Denominator smallint NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ehrmeasure'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrmeasure (
						EhrMeasureNum number(20) NOT NULL,
						MeasureType number(3) NOT NULL,
						Numerator number(11) NOT NULL,
						Denominator number(11) NOT NULL,
						CONSTRAINT ehrmeasure_EhrMeasureNum PRIMARY KEY (EhrMeasureNum)
						)";
					Db.NonQ(command);
				}
				//Add EHR Measures to DB
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(0,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(1,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(2,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(3,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(4,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(5,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(6,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(7,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(8,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(9,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(10,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(11,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(12,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(13,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(14,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(15,-1,-1)";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES(1,0,-1,-1)";//No rows in table and Oracle returns null so set the first one.
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),1,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),2,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),3,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),4,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),5,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),6,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),7,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),8,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),9,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),10,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),11,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),12,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),13,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),14,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),15,-1,-1)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE allergydef ADD DateTStamp timestamp";
					Db.NonQ(command);
					command="UPDATE allergydef SET DateTStamp = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE allergydef ADD DateTStamp timestamp";
					Db.NonQ(command);
					command="UPDATE allergydef SET DateTStamp = SYSTIMESTAMP";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE diseasedef ADD DateTStamp timestamp";
					Db.NonQ(command);
					command="UPDATE diseasedef SET DateTStamp = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE diseasedef ADD DateTStamp timestamp";
					Db.NonQ(command);
					command="UPDATE diseasedef SET DateTStamp = SYSTIMESTAMP";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE icd9 ADD DateTStamp timestamp";
					Db.NonQ(command);
					command="UPDATE icd9 SET DateTStamp = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE icd9 ADD DateTStamp timestamp";
					Db.NonQ(command);
					command="UPDATE icd9 SET DateTStamp = SYSTIMESTAMP";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE medication ADD DateTStamp timestamp";
					Db.NonQ(command);
					command="UPDATE medication SET DateTStamp = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE medication ADD DateTStamp timestamp";
					Db.NonQ(command);
					command="UPDATE medication SET DateTStamp = SYSTIMESTAMP";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS formulary";
					Db.NonQ(command);
					command=@"CREATE TABLE formulary (
						FormularyNum bigint NOT NULL auto_increment PRIMARY KEY,
						Description varchar(255) NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE formulary'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE formulary (
						FormularyNum number(20) NOT NULL,
						Description varchar2(255),
						CONSTRAINT formulary_FormularyNum PRIMARY KEY (FormularyNum)
						)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS formularymed";
					Db.NonQ(command);
					command=@"CREATE TABLE formularymed (
						FormularyMedNum bigint NOT NULL auto_increment PRIMARY KEY,
						FormularyNum bigint NOT NULL,
						MedicationNum bigint NOT NULL,
						INDEX(FormularyNum),
						INDEX(MedicationNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE formularymed'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE formularymed (
						FormularyMedNum number(20) NOT NULL,
						FormularyNum number(20) NOT NULL,
						MedicationNum number(20) NOT NULL,
						CONSTRAINT formularymed_FormularyMedNum PRIMARY KEY (FormularyMedNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX formularymed_FormularyNum ON formularymed (FormularyNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX formularymed_MedicationNum ON formularymed (MedicationNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					try {
						command="ALTER TABLE inssub DROP OldPlanNum";
						Db.NonQ(command);
					}
					catch { 
						//Some might not have this already so do nothing.
						//No need to test if column exists for Oracle because column was created in 
						//7.5 which is before Oracle support started so it will never exist.
					}
				}
				//Migrating the diseases to allergies and corresponding Rx alerts.
				command="SELECT * FROM diseasedef WHERE LOWER(DiseaseName) LIKE '%allerg%'";
				DataTable tableDiseaseDef=Db.GetTable(command);
				for(int i=0;i<tableDiseaseDef.Rows.Count;i++) {
					command="INSERT INTO allergydef (AllergyDefNum,Description,IsHidden) VALUES("
						+POut.Long(PIn.Long(tableDiseaseDef.Rows[i]["DiseaseDefNum"].ToString()))+",'"
						+POut.String(PIn.String(tableDiseaseDef.Rows[i]["DiseaseName"].ToString()))+"',"
						+POut.Int(PIn.Int(tableDiseaseDef.Rows[i]["IsHidden"].ToString()))+")";
					Db.NonQ(command);
					command="SELECT * FROM disease WHERE DiseaseDefNum="+POut.Long(PIn.Long(tableDiseaseDef.Rows[i]["DiseaseDefNum"].ToString()));
					DataTable disease=Db.GetTable(command);
					for(int j=0;j<disease.Rows.Count;j++) {
						command="INSERT INTO allergy (AllergyNum,PatNum,AllergyDefNum,Reaction,StatusIsActive) VALUES ("
						+POut.Long(PIn.Long(disease.Rows[j]["DiseaseNum"].ToString()))+","
						+POut.Long(PIn.Long(disease.Rows[j]["PatNum"].ToString()))+","
						+POut.Long(PIn.Long(disease.Rows[j]["DiseaseDefNum"].ToString()))+",'"
						+POut.String(PIn.String(disease.Rows[j]["PatNote"].ToString()))+"',1)";
						Db.NonQ(command);
						command="DELETE FROM disease WHERE DiseaseNum="+POut.Long(PIn.Long(disease.Rows[j]["DiseaseNum"].ToString()));
						Db.NonQ(command);
					}
					command="UPDATE rxalert SET AllergyDefNum="+POut.Long(PIn.Long(tableDiseaseDef.Rows[i]["DiseaseDefNum"].ToString()))+", DiseaseDefNum=0 WHERE DiseaseDefNum="
						+POut.Long(PIn.Long(tableDiseaseDef.Rows[i]["DiseaseDefNum"].ToString()));
					Db.NonQ(command);
					command="DELETE FROM diseasedef WHERE DiseaseDefNum="+POut.Long(PIn.Long(tableDiseaseDef.Rows[i]["DiseaseDefNum"].ToString()));
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.Oracle) {//Set time stamps to NOW().
					command="UPDATE allergy SET DateTStamp=SYSTIMESTAMP";
					Db.NonQ(command);
					command="UPDATE allergydef SET DateTStamp=SYSTIMESTAMP";
					Db.NonQ(command);
				}
				//Canadian claim carrier default values.
				command="UPDATE carrier SET CanadianEncryptionMethod=1 WHERE IsCDA=1 AND CanadianEncryptionMethod=0";
				Db.NonQ(command);
				command="UPDATE carrier SET CanadianSupportedTypes=262143 WHERE IsCDA=1 AND CanadianSupportedTypes=0";//All transaction types are allowed for each carrier by default.
				Db.NonQ(command);

				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS drugmanufacturer";
					Db.NonQ(command);
					command=@"CREATE TABLE drugmanufacturer (
						DrugManufacturerNum bigint NOT NULL auto_increment PRIMARY KEY,
						ManufacturerName varchar(255) NOT NULL,
						ManufacturerCode varchar(20) NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE drugmanufacturer'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE drugmanufacturer (
						DrugManufacturerNum number(20) NOT NULL,
						ManufacturerName varchar2(255),
						ManufacturerCode varchar2(20),
						CONSTRAINT drugmanufacturer_DrugManNum PRIMARY KEY (DrugManufacturerNum) 
						)"; //Changed drugmanufacturer_DrugManufacturerNum to DrugManNum: Max identifier for Oracle is 30 characters.
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS drugunit";
					Db.NonQ(command);
					command=@"CREATE TABLE drugunit (
						DrugUnitNum bigint NOT NULL auto_increment PRIMARY KEY,
						UnitIdentifier varchar(20) NOT NULL,
						UnitText varchar(255) NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE drugunit'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE drugunit (
						DrugUnitNum number(20) NOT NULL,
						UnitIdentifier varchar2(20),
						UnitText varchar2(255),
						CONSTRAINT drugunit_DrugUnitNum PRIMARY KEY (DrugUnitNum)
						)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS vaccinedef";
					Db.NonQ(command);
					command=@"CREATE TABLE vaccinedef (
						VaccineDefNum bigint NOT NULL auto_increment PRIMARY KEY,
						CVXCode varchar(255) NOT NULL,
						VaccineName varchar(255) NOT NULL,
						DrugManufacturerNum bigint NOT NULL,
						INDEX(DrugManufacturerNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE vaccinedef'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE vaccinedef (
						VaccineDefNum number(20) NOT NULL,
						CVXCode varchar2(255),
						VaccineName varchar2(255),
						DrugManufacturerNum number(20) NOT NULL,
						CONSTRAINT vaccinedef_VaccineDefNum PRIMARY KEY (VaccineDefNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX vaccinedef_DrugManufacturerNum ON vaccinedef (DrugManufacturerNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS vaccinepat";
					Db.NonQ(command);
					command=@"CREATE TABLE vaccinepat (
						VaccinePatNum bigint NOT NULL auto_increment PRIMARY KEY,
						VaccineDefNum bigint NOT NULL,
						DateTimeStart date NOT NULL DEFAULT '0001-01-01',
						DateTimeEnd date NOT NULL DEFAULT '0001-01-01',
						AdministeredAmt float NOT NULL,
						DrugUnitNum bigint NOT NULL,
						LotNumber varchar(255) NOT NULL,
						INDEX(VaccineDefNum),
						INDEX(DrugUnitNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE vaccinepat'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE vaccinepat (
						VaccinePatNum number(20) NOT NULL,
						VaccineDefNum number(20) NOT NULL,
						DateTimeStart date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						DateTimeEnd date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						AdministeredAmt number(38,8) NOT NULL,
						DrugUnitNum number(20) NOT NULL,
						LotNumber varchar2(255),
						CONSTRAINT vaccinepat_VaccinePatNum PRIMARY KEY (VaccinePatNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX vaccinepat_VaccineDefNum ON vaccinepat (VaccineDefNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX vaccinepat_DrugUnitNum ON vaccinepat (DrugUnitNum)";
					Db.NonQ(command);
				}
				//eCW bridge enhancements
				command="SELECT ProgramNum FROM program WHERE ProgName='eClinicalWorks'";
				int programNum=PIn.Int(Db.GetScalar(command));
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+POut.Long(programNum)+"', "
					+"'FeeSchedulesSetManually', "
					+"'0')";
					Db.NonQ32(command);
				}
				else {//oracle
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES((SELECT MAX(ProgramPropertyNum)+1 FROM programproperty),"
					+"'"+POut.Long(programNum)+"', "
					+"'FeeSchedulesSetManually', "
					+"'0')";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '7.9.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_9_2();
		}

		private static void To7_9_2() {
			if(FromVersion<new Version("7.9.2.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.9.2");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('MobileSynchNewTables79Done','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'MobileSynchNewTables79Done','0')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '7.9.2.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_9_8();
		}

		///<summary>Oracle compatible: 5/24/2011</summary>
		private static void To7_9_8() {
			if(FromVersion<new Version("7.9.8.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.9.8");//No translation in convert script.
				string command;
				command=@"UPDATE clearinghouse SET ExportPath='C:\\Program Files\\Renaissance\\dotr\\upload\\' WHERE Description='Renaissance'";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.9.8.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To7_9_10();
		}
		
		///<summary>Oracle compatible: 7/7/2011</summary>
		private static void To7_9_10() {
			if(FromVersion<new Version("7.9.10.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 7.9.10");//No translation in convert script.
				string command;
				command="UPDATE payment SET DateEntry=PayDate WHERE DateEntry < "+POut.Date(new DateTime(1880,1,1));
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '7.9.10.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To11_0_1();
		}

		///<summary>Oracle compatible: 7/8/2011</summary>
		private static void To11_0_1() {
			if(FromVersion<new Version("11.0.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 11.0.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS labpanel";
					Db.NonQ(command);
					command=@"CREATE TABLE labpanel (
						LabPanelNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						RawMessage text NOT NULL,
						LabNameAddress varchar(255) NOT NULL,
						DateTStamp timestamp,
						SpecimenCondition varchar(255) NOT NULL,
						SpecimenSource varchar(255) NOT NULL,
						ServiceId varchar(255) NOT NULL,
						ServiceName varchar(255) NOT NULL,
						INDEX(PatNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE labpanel'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE labpanel (
						LabPanelNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						RawMessage clob,
						LabNameAddress varchar2(255),
						DateTStamp timestamp,
						SpecimenCondition varchar2(255),
						SpecimenSource varchar2(255),
						ServiceId varchar2(255),
						ServiceName varchar2(255),
						CONSTRAINT labpanel_LabPanelNum PRIMARY KEY (LabPanelNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX labpanel_PatNum ON labpanel (PatNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS labresult";
					Db.NonQ(command);
					command=@"CREATE TABLE labresult (
						LabResultNum bigint NOT NULL auto_increment PRIMARY KEY,
						LabPanelNum bigint NOT NULL,
						DateTimeTest datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						TestName varchar(255) NOT NULL,
						DateTStamp timestamp,
						TestID varchar(255) NOT NULL,
						ObsValue varchar(255) NOT NULL,
						ObsUnits varchar(255) NOT NULL,
						ObsRange varchar(255) NOT NULL,
						INDEX(LabPanelNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE labresult'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE labresult (
						LabResultNum number(20) NOT NULL,
						LabPanelNum number(20) NOT NULL,
						DateTimeTest date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						TestName varchar2(255),
						DateTStamp timestamp,
						TestID varchar2(255),
						ObsValue varchar2(255),
						ObsUnits varchar2(255) NOT NULL,
						ObsRange varchar2(255) NOT NULL,
						CONSTRAINT labresult_LabResultNum PRIMARY KEY (LabResultNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX labresult_LabPanelNum ON labresult (LabPanelNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vaccinepat ADD PatNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE vaccinepat ADD INDEX (PatNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vaccinepat ADD PatNum number(20)";
					Db.NonQ(command);
					command="UPDATE vaccinepat SET PatNum = 0 WHERE PatNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE vaccinepat MODIFY PatNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX vaccinepat_PatNum ON vaccinepat (PatNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,Comments) VALUES('EHREmailToAddress','Hidden pref: Email for sending EHR email.')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,Comments) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'EHREmailToAddress','Hidden pref: Email for sending EHR email.')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE medicalorder ADD Description varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE medicalorder ADD Description varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vaccinepat MODIFY DateTimeStart DATETIME";
					Db.NonQ(command);
					command="ALTER TABLE vaccinepat MODIFY DateTimeEnd DATETIME";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vaccinepat MODIFY (DateTimeStart DATE);";
					Db.NonQ(command);
					command="ALTER TABLE vaccinepat MODIFY (DateTimeEnd DATE);";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE securitylog ADD CompName varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE securitylog ADD CompName varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName, ValueString,Comments) VALUES ('EhrEmergencyNow','0','Boolean. 0 means false. 1 grants emergency access to the family module.')";
					Db.NonQ(command);
				}
				else{//oracle
					command="INSERT INTO preference (PrefNum,PrefName, ValueString,Comments) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'EhrEmergencyNow','0','Boolean. 0 means false. 1 grants emergency access to the family module.')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString,Comments) VALUES('SecurityLogOffAfterMinutes','0','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName, ValueString,Comments) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'SecurityLogOffAfterMinutes','0','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patient ADD PreferContactConfidential tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patient ADD PreferContactConfidential number(3)";
					Db.NonQ(command);
					command="UPDATE patient SET PreferContactConfidential = 0 WHERE PreferContactConfidential IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE patient MODIFY PreferContactConfidential NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS reminderrule";
					Db.NonQ(command);
					command=@"CREATE TABLE reminderrule (
						ReminderRuleNum bigint NOT NULL auto_increment PRIMARY KEY,
						ReminderCriterion tinyint NOT NULL,
						CriterionFK bigint NOT NULL,
						CriterionValue varchar(255) NOT NULL,
						Message varchar(255) NOT NULL,
						INDEX(CriterionFK)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE reminderrule'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE reminderrule (
						ReminderRuleNum number(20) NOT NULL,
						ReminderCriterion number(3) NOT NULL,
						CriterionFK number(20) NOT NULL,
						CriterionValue varchar2(255),
						Message varchar2(255),
						CONSTRAINT reminderrule_ReminderRuleNum PRIMARY KEY (ReminderRuleNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX reminderrule_CriterionFK ON reminderrule (CriterionFK)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patplan DROP PlanNum";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patplan DROP COLUMN PlanNum";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString,Comments) VALUES('EHREmailFromAddress','','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString,Comments) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'EHREmailFromAddress','','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString,Comments) VALUES('EHREmailPOPserver','','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString,Comments) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'EHREmailPOPserver','','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString,Comments) VALUES('EHREmailPort','','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString,Comments) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'EHREmailPort','','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString,Comments) VALUES('EHREmailPassword','','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString,Comments) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'EHREmailPassword','','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString,Comments) VALUES('ProblemsIndicateNone','','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString,Comments) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ProblemsIndicateNone','','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString,Comments) VALUES('MedicationsIndicateNone','','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString,Comments) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'MedicationsIndicateNone','','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS eduresource";
					Db.NonQ(command);
					command=@"CREATE TABLE eduresource (
						EduResourceNum bigint NOT NULL auto_increment PRIMARY KEY,
						Icd9Num bigint NOT NULL,
						DiseaseDefNum bigint NOT NULL,
						MedicationNum bigint NOT NULL,
						LabResultID varchar(255) NOT NULL,
						LabResultName varchar(255) NOT NULL,
						LabResultCompare varchar(255) NOT NULL,
						ResourceUrl varchar(255) NOT NULL,
						INDEX(Icd9Num),
						INDEX(DiseaseDefNum),
						INDEX(MedicationNum),
						INDEX(LabResultID)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE eduresource'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE eduresource (
						EduResourceNum number(20) NOT NULL,
						Icd9Num number(20) NOT NULL,
						DiseaseDefNum number(20) NOT NULL,
						MedicationNum number(20) NOT NULL,
						LabResultID varchar2(255),
						LabResultName varchar2(255),
						LabResultCompare varchar2(255),
						ResourceUrl varchar2(255),
						CONSTRAINT eduresource_EduResourceNum PRIMARY KEY (EduResourceNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX eduresource_Icd9Num ON eduresource (Icd9Num)";
					Db.NonQ(command);
					command=@"CREATE INDEX eduresource_DiseaseDefNum ON eduresource (DiseaseDefNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX eduresource_MedicationNum ON eduresource (MedicationNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX eduresource_LabResultID ON eduresource (LabResultID)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE medicalorder ADD IsDiscontinued tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE medicalorder ADD IsDiscontinued number(3)";
					Db.NonQ(command);
					command="UPDATE medicalorder SET IsDiscontinued = 0 WHERE IsDiscontinued IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE medicalorder MODIFY IsDiscontinued NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('StoreCCtokens','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'StoreCCtokens','1')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					//Create a temporary table to hold all of the canadian network information that we know of.
					command="DROP TABLE IF EXISTS `tempcanadiannetwork`";
					Db.NonQ(command);
					command="CREATE TABLE `tempcanadiannetwork` ("
						+"`CanadianNetworkNum` bigint(20) NOT NULL auto_increment,"
						+"`Abbrev` varchar(20) default '',"
						+"`Descript` varchar(255) default '',"
						+"`CanadianTransactionPrefix` varchar(255) default '',"
						+"PRIMARY KEY  (`CanadianNetworkNum`)"
						+") ENGINE=MyISAM DEFAULT CHARSET=utf8";
					Db.NonQ(command);
					command="INSERT INTO `tempcanadiannetwork`(`CanadianNetworkNum`,`Abbrev`,`Descript`,`CanadianTransactionPrefix`) VALUES (7,'TELUS B','TELUS Group B','HD*         ')";
					Db.NonQ(command);
					command="INSERT INTO `tempcanadiannetwork`(`CanadianNetworkNum`,`Abbrev`,`Descript`,`CanadianTransactionPrefix`) VALUES (8,'CSI','Continovation Services Inc.','CSI         ')";
					Db.NonQ(command);
					command="INSERT INTO `tempcanadiannetwork`(`CanadianNetworkNum`,`Abbrev`,`Descript`,`CanadianTransactionPrefix`) VALUES (9,'CDCS','CDCS','CDCS        ')";
					Db.NonQ(command);
					command="INSERT INTO `tempcanadiannetwork`(`CanadianNetworkNum`,`Abbrev`,`Descript`,`CanadianTransactionPrefix`) VALUES (10,'TELUS A','TELUS Group A','1111111119  ')";
					Db.NonQ(command);
					command="INSERT INTO `tempcanadiannetwork`(`CanadianNetworkNum`,`Abbrev`,`Descript`,`CanadianTransactionPrefix`) VALUES (11,'MBC','Manitoba Blue Cross','MBC         ')";
					Db.NonQ(command);
					command="INSERT INTO `tempcanadiannetwork`(`CanadianNetworkNum`,`Abbrev`,`Descript`,`CanadianTransactionPrefix`) VALUES (12,'PBC','Pacific Blue Cross','PBC         ')";
					Db.NonQ(command);
					command="INSERT INTO `tempcanadiannetwork`(`CanadianNetworkNum`,`Abbrev`,`Descript`,`CanadianTransactionPrefix`) VALUES (13,'ABC','Alberta Blue Cross','ABC         ')";
					Db.NonQ(command);
					//Create a column to associate already created canadian networks to our temporary canadian network table.
					command="ALTER TABLE tempcanadiannetwork ADD COLUMN CanadianNetworkNumExisting bigint default 0";
					Db.NonQ(command);
					command="UPDATE tempcanadiannetwork t,canadiannetwork n SET t.CanadianNetworkNumExisting=n.CanadianNetworkNum WHERE TRIM(LOWER(t.Abbrev))=TRIM(LOWER(n.Abbrev))";
					Db.NonQ(command);
					//Create a column to associate our temporary canadian networks to their new primary key in the canadian network table.
					command="ALTER TABLE tempcanadiannetwork ADD COLUMN CanadianNetworkNumNew bigint default 0";
					Db.NonQ(command);
					command="UPDATE tempcanadiannetwork t "
						+"SET t.CanadianNetworkNumNew=CASE "
							+"WHEN t.CanadianNetworkNumExisting<>0 THEN t.CanadianNetworkNumExisting "
							+"ELSE t.CanadianNetworkNum+(SELECT MAX(n.CanadianNetworkNum) FROM canadiannetwork n) END";
					Db.NonQ(command);
					//Update the live canadiannetwork table and set the CanadianTransactionPrefix to known values for networks that were already in the database.
					command="UPDATE canadiannetwork n,tempcanadiannetwork t SET n.CanadianTransactionPrefix=t.CanadianTransactionPrefix WHERE n.CanadianNetworkNum=t.CanadianNetworkNumExisting";
					Db.NonQ(command);
					//Add any missing canadian networks from the temporary canadian network table for those networks that are not already present.
					command="INSERT INTO canadiannetwork (CanadianNetworkNum,Abbrev,Descript,CanadianTransactionPrefix) "
						+"SELECT CanadianNetworkNumNew,Abbrev,Descript,CanadianTransactionPrefix "
						+"FROM tempcanadiannetwork "
						+"WHERE CanadianNetworkNumExisting=0";
					Db.NonQ(command);
					//Remove the CanadianTransactionPrefix column from the carrier table, since it will now be part of the canadiannetwork table.
					command="ALTER TABLE carrier DROP COLUMN CanadianTransactionPrefix";
					Db.NonQ(command);
					//Create a temporary carrier table to hold all of the most recent canadian carrier information that we know about.
					command="DROP TABLE IF EXISTS `tempcarriercanada`";
					Db.NonQ(command);
					command="CREATE TABLE `tempcarriercanada` ("
						+"`CarrierNum` bigint(20) NOT NULL auto_increment,"
						+"`CarrierName` varchar(255) default '',"
						+"`Address` varchar(255) default '',"
						+"`Address2` varchar(255) default '',"
						+"`City` varchar(255) default '',"
						+"`State` varchar(255) default '',"
						+"`Zip` varchar(255) default '',"
						+"`Phone` varchar(255) default '',"
						+"`ElectID` varchar(255) default '',"
						+"`NoSendElect` tinyint(1) unsigned NOT NULL default '0',"
						+"`IsCDA` tinyint(3) unsigned NOT NULL,"
						+"`CDAnetVersion` varchar(100) default '',"
						+"`CanadianNetworkNum` bigint(20) NOT NULL,"
						+"`IsHidden` tinyint(4) NOT NULL,"
						+"`CanadianEncryptionMethod` tinyint(4) NOT NULL,"
						+"`CanadianSupportedTypes` int(11) NOT NULL,"
						+"PRIMARY KEY  (`CarrierNum`)"
						+") ENGINE=MyISAM DEFAULT CHARSET=utf8";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (1,'Accerta','P.O. Box 310','Station \\'P\\'','Toronto','ON','M5S 2S8','1-800-505-7430','311140',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (2,'ADSC - AB Social Services QuikCard','200 Quikcard Centre','17010 103 Avenue','Edmonton','AB','T5S 1K7','1-800-232-1997','000105',0,1,'04',8,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (3,'AGA Financial Group - Groupe Cloutier','525 René-Lévesque Blvd E 6th Floor','P.O. Box 17100','Quebec','QC','G1K 9E2','1 800 461-0770','610226',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (4,'Association des policières et policiers (APPQ)','1981, Léonard-De Vinci','','Ste-Julie','QC','J3E 1Y9','(450) 922-5414','628112',0,1,'02',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (5,'Assumption Life','P. O. Box 160','','Moncton','NB','E1C 8L1','1-800-455-7337','610191',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (6,'Autoben','212 King Street West','Suite 203','Toronto','ON','M5H 1K5','1.866.647.1147','628151',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (7,'Benecaid Health Benefit Solutions (ESI)','185 The West Mall','Suite 1700','Toronto','ON','M9C 5L5','1.877.797.7448','610708',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (8,'Benefits Trust (The)','3800 Steeles Ave. West','Suite #102W','Vaughan','ON','L4L 4G9','1-800-487-2993','610146',0,1,'02',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (9,'Beneplan','150 Ferrand Drive','Suite 500','Toronto','ON','M3C 3E5','1-800-387-1670','400008',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (10,'Capitale','525 René-Lévesque Blvd E 6th Floor','P.O. Box 17100','Quebec','QC','G1K 9E2','1 800 461-0770','600502',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (12,'Claimsecure','1 City Centre Drive','Suite 620','Mississauga','ON','L5B 1M2','1-888-479-7587','610099',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (11,'CDCS','P.O. Box 156 Stn. \"B\"','','Sudbury','ON','P3E 4N5','(705) 675-2222','610129',0,1,'04',9,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (13,'Commision de la construction du Quebec (CCQ)','3530, rue Jean-Talon Ouest','','Montréal','QC','H3R 2G3','1 888 842-8282','000036',0,1,'02',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (14,'Co-operators (The)','Service Quality Department','130 Macdonell Street','Guelph','ON','N1H 6P8','1-800-265-2662','606258',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (15,'Coughlin & Associates','466 Tremblay Road','','Ottawa','ON','K1G 3R1','1-888-613-1234','610105',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (16,'Cowan Wright Beauchamps','705 Fountain Street North','PO Box 1510','Cambridge','ON','N1R 5T2','1-866-912-6926','610153',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (17,'Desjardins Financial Security','200 des Commandeurs','','Lévis','QC','G6V 6R2','1-866-838-7553','000051',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (18,'Empire Life Insurance Company (The)','259 King Street East','','Kingston','ON','K7L 3A8','1 800 561-1268','000033',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (19,'Equitable Life','One Westmount Road North','P.O. Box 1603, Stn Waterloo','Waterloo','ON','N2J 4C7','1-800-265-4556 x601','000029',0,1,'02',10,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (20,'Esorse Corporation','234 Eglinton Avenue East','Suite 502','Toronto','ON','M4P 1K5','(416)-483-3265','610650',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (21,'FAS Administrators','9707 - 110 Street','9th Floor','Edmonton','AB','T5K 3T4','1-800-770-2998','610614',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (22,'Great West Life Assurance Company (The)','100 Osborne Street North','','Winnipeg','MB','R3C 3A5','204-946-1190','000011',0,1,'02',10,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (23,'Green Shield Canada','8677 Anchor Drive','P.O Box 1606','Windsor','ON','N9A 6W1','1-800-265-5615','000102',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (24,'Group Medical Services (GMS - ESI)','2055 Albert Street','PO Box 1949','Regina','SK','S4P 0E3','1.800.667.3699','610217',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (25,'Group Medical Services (GMS - ESI - Saskatchewan)','2055 Albert Street','PO Box 1949','Regina','SK','S4P 0E3','1.800.667.3699','610218',0,1,'04',8,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (26,'groupSource','1550 - 5th Street SW','Suite 400','Calgary','AB','T2R 1K3','1-800-661-6195','605064',0,1,'04',8,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (27,'Industrial Alliance','1080 Grande Allée West','PO Box 1907, Station Terminus','Quebec City','QC','G1K 7M3','1-800-463-6236','000060',0,1,'02',10,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (28,'Industrial Alliance Pacific Insurance and Financial','2165 Broadway West','P.O. Box 5900','Vancouver','BC','V6B 5H6','(604) 734-1667','000024',0,1,'02',10,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (29,'Internationale Compagnie D\\'assurance vie','142 Heriot','P.O. Box 696','Drummondville','QC','J2B 6W9','1 888 864-6684','610643',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (30,'Johnson Inc.','','','','','','1-877-221-2127','627265',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (31,'Johnston Group','','','','','','800-990-4476','627223',0,1,'02',10,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (32,'Lee-Power & Associates Inc.','616 Cooper St.','','Ottawa','ON','K1R 5J2','(613) 236-9007','627585',0,1,'02',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (33,'Manion Wilkins','500 - 21 Four Seasons Place','','Etobicoke','ON','M9B 0A5','1-800-263-5621','610158',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (34,'Manitoba Blue Cross','P.O. Box 1046','','Winnipeg','MB','R3C 2X7','1-800-873-2583','000094',0,1,'04',11,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (35,'Manufacturers Life Insurance Company (The)','500 King Street. N.','P.O. Box 1669','Waterloo','ON','N2J 4Z6','1-888-626-8543','000034',0,1,'02',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (36,'Manulife Financial','500 King Street. N.','P.O. Box 1669','Waterloo','ON','N2J 4Z6','1-888-626-8543','610059',0,1,'02',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (37,'Maritime Life Assurance Company','500 King Street. N.','P.O. Box 1669','Waterloo','ON','N2J 4Z6','1-888-626-8543','311113',0,1,'02',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (38,'Maritime Life Assurance Company','500 King Street. N.','P.O. Box 1669','Waterloo','ON','N2J 4Z6','1-888-626-8543','610070',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (39,'McAteer Group of Companies','45 McIntosh Drive','','Markham','ON','L3R 8C7','(800) 263-3564','000112',0,1,'04',8,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (40,'MDM','MD Management Limited','1870, Alta Vista Drive','Ottawa','ON','K1G 6R7','1 800 267-4022','601052',0,1,'02',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (41,'Medavie Blue Cross','644 Main Street','PO Box 220','Moncton','NB','E1C 8L3','1-800-667-4511','610047',0,1,'02',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (42,'NexGenRX','145 The West Mall','P.O. Box 110 U','Toronto','ON','M8Z 5M4','1-866-424-0257','610634',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (43,'NIHB','Health Canada','Address Locator 0900C2','Ottawa','ON','K1A 0K9','1-866-225-0709','610124',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (44,'Nova Scotia Community Services','10 Webster Street','Suite 202','Kentville','NS','B4N 1H7','(902) 679-6715','000109',0,1,'04',8,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (45,'Nova Scotia Medical Services Insurance','1741 Brunswick Street, Suite 110A','PO Box 1535','Halifax','NS','B3J 2Y3','1-877-292-9597','000108',0,1,'04',8,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (46,'Nunatsiavut Government Department of Health','25 Ikajuktauvik Road','P.O. Box 70','Nain','NL','A0P 1L0','(709) 922-2942','610172',0,1,'04',8,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (47,'Pacific Blue Cross','Pacific Blue Cross/ BC Life','PO Box 7000','Vancouver','BC','V6B 4E1','604 419-2300','000064',0,1,'04',12,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (48,'Quikcard','200 Quikcard Centre','17010 103 Avenue','Edmonton','AB','T5S 1K7','(780) 426-7526','000103',0,1,'04',8,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (49,'RWAM Insurance','49 Industrial Drive','','Elmira','ON','N3B 3B1','(519) 669-1632','610616',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (50,'Saskatchewan Blue Cross','516 2nd Avenue N','PO Box 4030','Saskatoon','SK','S7K 3T2','306.244.1192','000096',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (51,'SES Benefits','2800 Skymark Avenue','Suite 307','Mississauga','ON','L4W 5A6','1-888-939-8885','610196',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (52,'SSQ SOCIÉTÉ d\\'assurance-vie inc.','2525 Laurier Boulevard','P.O. Box 10500, Stn Sainte-Foy','Quebec City','QC','G1V 4H6','1-888-900-3457','000079',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (53,'Standard Life Assurance Company (The)','639 - 5th Avenue South West','Suite 1500','Calgary','AB','T2P 0M9','403-296-9477','000020',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (54,'Sun Life of Canada','','','','','','1-877-786-5433','000016',0,1,'02',10,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (55,'Survivance','1555 Girouard Street West','P.O. Box 10,000','Saint-Hyacinthe','QC','J2S 7C8','450 773-6051','000080',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (56,'Syndicat des fonctionnaires municipaux MTL','429, rue de La Gauchetière Est','','Montréal','QC','H2L 2M7','514 842-9463','610677',0,1,'04',7,0,1,262143)";
					Db.NonQ(command);
					command="INSERT INTO `tempcarriercanada`(`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) VALUES (57,'Wawanesa','900-191 Broadway','','Winnipeg','MB','R3C 3P1','(204) 985-3923','311109',0,1,'02',7,0,1,262143)";
					Db.NonQ(command);
					//Update the CanadianNetworkNum values in the temporary carrier table because the networks in the live canadiannetwork table are different.
					command="UPDATE tempcarriercanada tc,tempcanadiannetwork tn SET tc.CanadianNetworkNum=tn.CanadianNetworkNumNew WHERE tc.CanadianNetworkNum=tn.CanadianNetworkNum";
					Db.NonQ(command);
					//Clear all CanadianNetworkNum foreign keys from the carrier table so that for those carriers which we cannot match to a network, the users will be notified that no network is associated with the carrier.
					command="UPDATE carrier c SET c.CanadianNetworkNum=0";
					Db.NonQ(command);
					//Create a column in the temporary carrier table to link up the existing carriers by electronic ID.
					command="ALTER TABLE tempcarriercanada ADD COLUMN CarrierNumExisting bigint default 0";
					Db.NonQ(command);
					command="UPDATE tempcarriercanada t,carrier c SET t.CarrierNumExisting=c.CarrierNum WHERE c.IsCDA=1 AND t.ElectID=c.ElectID";
					Db.NonQ(command);
					//For those carriers that were already in the live data before this conversion that match a known carrier, update their CanadianNetworkNum to match the temporary canadian network data.
					command="UPDATE carrier c,tempcarriercanada tc SET c.CanadianNetworkNum=tc.CanadianNetworkNum WHERE c.CarrierNum=tc.CarrierNumExisting";
					Db.NonQ(command);
					//Create a column to figure out what the new carriernums will need to be for the new carriers that we are going to add to the carrier table.
					command="ALTER TABLE tempcarriercanada ADD COLUMN CarrierNumNew bigint default 0";
					Db.NonQ(command);
					command="UPDATE tempcarriercanada t SET t.CarrierNumNew=CASE WHEN t.CarrierNumExisting<>0 THEN t.CarrierNumExisting ELSE t.CarrierNum+(SELECT MAX(c.CarrierNum) FROM carrier c) END";
					Db.NonQ(command);
					//Add carriers from the temporary carrier table which do not already exist in the live carrier table.
					//We only want to insert these carriers if in Canada because they are of no use elsewhere.
					if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
						command="INSERT INTO carrier (`CarrierNum`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,"
							+"`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes`) "
							+"SELECT `CarrierNumNew`,`CarrierName`,`Address`,`Address2`,`City`,`State`,`Zip`,`Phone`,`ElectID`,`NoSendElect`,`IsCDA`,"
							+"`CDAnetVersion`,`CanadianNetworkNum`,`IsHidden`,`CanadianEncryptionMethod`,`CanadianSupportedTypes` "
							+"FROM tempcarriercanada "
							+"WHERE CarrierNumExisting=0";
						Db.NonQ(command);
					}
					command="DROP TABLE IF EXISTS `tempcanadiannetwork`";
					Db.NonQ(command);
					command="DROP TABLE IF EXISTS `tempcarriercanada`";
					Db.NonQ(command);
				}
				else {//oracle
					//At this point, there should not be anyone in Canada using Oracle, so these statements have been skipped because they would be a bit of work to create.
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString,Comments) VALUES('AllergiesIndicateNone','','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString,Comments) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'AllergiesIndicateNone','','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE provider ADD IsCDAnet tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE provider ADD IsCDAnet number(3)";
					Db.NonQ(command);
					command="UPDATE provider SET IsCDAnet = 0 WHERE IsCDAnet IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE provider MODIFY IsCDAnet NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ehrmeasureevent";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrmeasureevent (
						EhrMeasureEventNum bigint NOT NULL auto_increment PRIMARY KEY,
						DateTEvent datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						EventType tinyint NOT NULL,
						PatNum bigint NOT NULL,
						MoreInfo varchar(255) NOT NULL,
						INDEX(PatNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ehrmeasureevent'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrmeasureevent (
						EhrMeasureEventNum number(20) NOT NULL,
						DateTEvent date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						EventType number(3) NOT NULL,
						PatNum number(20) NOT NULL,
						MoreInfo varchar2(255),
						CONSTRAINT ehrmeasureevent_EhrMeasureNum PRIMARY KEY (EhrMeasureEventNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrmeasureevent_PatNum ON ehrmeasureevent (PatNum)";
					Db.NonQ(command);
				}
				//EvaSoft link-----------------------------------------------------------------------
				//This insert statement is compatible with both MySQL and Oracle.
				command="SELECT MAX(ProgramNum)+1 FROM program";
				long programNum=PIn.Long(Db.GetScalar(command));
				command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'"+POut.Long(programNum)+"',"
						+"'EvaSoft', "
						+"'EvaSoft from www.imageworkscorporation.com', "
						+"'0', "
						+"'', "
						+"'', "
						+"'"+POut.String(@"No command line or path is needed.")+"')";
				Db.NonQ(command);
				//This insert statement is compatible with both MySQL and Oracle.
				command="SELECT MAX(ProgramPropertyNum)+1 FROM programproperty";
				long programPropertyNum=PIn.Long(Db.GetScalar(command));
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+POut.Long(programPropertyNum)+"',"
					+"'"+programNum.ToString()+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0')";
				Db.NonQ(command);
				//This insert statement is compatible with both MySQL and Oracle.
				command="SELECT MAX(ToolButItemNum)+1 FROM toolbutitem";
				long toolButItemNum=PIn.Long(Db.GetScalar(command));
				command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
					+"VALUES ("
					+"'"+POut.Long(toolButItemNum)+"',"
					+"'"+programNum.ToString()+"', "
					+"'"+POut.Int((int)ToolBarsAvail.ChartModule)+"', "
					+"'EvaSoft')";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE refattach ADD IsTransitionOfCare tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE refattach ADD IsTransitionOfCare number(3)";
					Db.NonQ(command);
					command="UPDATE refattach SET IsTransitionOfCare = 0 WHERE IsTransitionOfCare IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE refattach MODIFY IsTransitionOfCare NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE referral ADD IsDoctor tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE referral ADD IsDoctor number(3)";
					Db.NonQ(command);
					command="UPDATE referral SET IsDoctor = 0 WHERE IsDoctor IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE referral MODIFY IsDoctor NOT NULL";
					Db.NonQ(command);
				}
				command="UPDATE referral SET IsDoctor=1 WHERE PatNum = 0";
				Db.NonQ(command);
				if(DataConnection.DBtype == DatabaseType.MySql) {
					command = "ALTER TABLE allergy ADD DateAdverseReaction date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command = "ALTER TABLE allergy ADD DateAdverseReaction date";
					Db.NonQ(command);
					command = "UPDATE allergy SET DateAdverseReaction = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateAdverseReaction IS NULL";
					Db.NonQ(command);
					command = "ALTER TABLE allergy MODIFY DateAdverseReaction NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE pharmacy ADD DateTStamp timestamp";
					Db.NonQ(command);
					command="UPDATE pharmacy SET DateTStamp = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE pharmacy ADD DateTStamp timestamp";
					Db.NonQ(command);
					command="UPDATE pharmacy SET DateTStamp = SYSTIMESTAMP";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE medicationpat ADD DateStart date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE medicationpat ADD DateStart date";
					Db.NonQ(command);
					command="UPDATE medicationpat SET DateStart = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateStart IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE medicationpat MODIFY DateStart NOT NULL";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE medicationpat ADD DateStop date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE medicationpat ADD DateStop date";
					Db.NonQ(command);
					command="UPDATE medicationpat SET DateStop = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateStop IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE medicationpat MODIFY DateStop NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="UPDATE medicationpat SET DateStop = CURDATE() WHERE IsDiscontinued=1";
					Db.NonQ(command);
				}
				else{
					command="UPDATE medicationpat SET DateStop = SYSDATE WHERE IsDiscontinued=1";
					Db.NonQ(command);
				}
				command="ALTER TABLE medicationpat DROP COLUMN IsDiscontinued";//both oracle and mysql
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE allergydef ADD Snomed tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE allergydef ADD Snomed number(3)";
					Db.NonQ(command);
					command="UPDATE allergydef SET Snomed = 0 WHERE Snomed IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE allergydef MODIFY Snomed NOT NULL";
					Db.NonQ(command);
				}
				//this was supposed to have been deleted:
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE allergydef ADD RxCui bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE allergydef ADD INDEX (RxCui)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE allergydef ADD RxCui number(20)";
					Db.NonQ(command);
					command="UPDATE allergydef SET RxCui = 0 WHERE RxCui IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE allergydef MODIFY RxCui NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX allergydef_RxCui ON allergydef (RxCui)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE labresult ADD AbnormalFlag tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE labresult ADD AbnormalFlag number(3)";
					Db.NonQ(command);
					command="UPDATE labresult SET AbnormalFlag = 0 WHERE AbnormalFlag IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE labresult MODIFY AbnormalFlag NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE disease ADD DateStart date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE disease ADD DateStart date";
					Db.NonQ(command);
					command="UPDATE disease SET DateStart = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateStart IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE disease MODIFY DateStart NOT NULL";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE disease ADD DateStop date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE disease ADD DateStop date";
					Db.NonQ(command);
					command="UPDATE disease SET DateStop = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateStop IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE disease MODIFY DateStop NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE rxpat ADD SendStatus tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE rxpat ADD SendStatus number(3)";
					Db.NonQ(command);
					command="UPDATE rxpat SET SendStatus = 0 WHERE SendStatus IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE rxpat MODIFY SendStatus NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE provider ADD EcwID varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE provider ADD EcwID varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE labpanel ADD MedicalOrderNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE labpanel ADD INDEX (MedicalOrderNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE labpanel ADD MedicalOrderNum number(20)";
					Db.NonQ(command);
					command="UPDATE labpanel SET MedicalOrderNum = 0 WHERE MedicalOrderNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE labpanel MODIFY MedicalOrderNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX labpanel_MedicalOrderNum ON labpanel (MedicalOrderNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE allergydef ADD MedicationNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE allergydef ADD INDEX (MedicationNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE allergydef ADD MedicationNum number(20)";
					Db.NonQ(command);
					command="UPDATE allergydef SET MedicationNum = 0 WHERE MedicationNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE allergydef MODIFY MedicationNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX allergydef_MedicationNum ON allergydef (MedicationNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS orthochart";
					Db.NonQ(command);
					command=@"CREATE TABLE orthochart (
						OrthoChartNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						DateService date NOT NULL DEFAULT '0001-01-01',
						FieldName varchar(255) NOT NULL,
						FieldValue text NOT NULL,
						INDEX(PatNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE orthochart'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE orthochart (
						OrthoChartNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						DateService date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						FieldName varchar2(255),
						FieldValue varchar2(4000),
						CONSTRAINT orthochart_OrthoChartNum PRIMARY KEY (OrthoChartNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX orthochart_PatNum ON orthochart (PatNum)";
					Db.NonQ(command);
				}
				command="ALTER TABLE rxpat DROP COLUMN IsElectQueue";//both oracle and mysql
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE medication ADD RxCui bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE medication ADD INDEX (RxCui)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE medication ADD RxCui number(20)";
					Db.NonQ(command);
					command="UPDATE medication SET RxCui = 0 WHERE RxCui IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE medication MODIFY RxCui NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX medication_RxCui ON medication (RxCui)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString,Comments) VALUES('ShowFeatureEhr','','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString,Comments) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ShowFeatureEhr','','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE provider ADD EhrKey varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE provider ADD EhrKey varchar2(255)";
					Db.NonQ(command);
				}
				bool usingECW=true;
				command="SELECT COUNT(*) FROM program WHERE ProgName='eClinicalWorks' AND Enabled=1";
				if(Db.GetCount(command)=="0") {
					usingECW=false;
				}
				if(usingECW) {
					command="UPDATE provider SET EcwID=Abbr";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS rxnorm";
					Db.NonQ(command);
					command=@"CREATE TABLE rxnorm (
						RxNormNum bigint NOT NULL auto_increment PRIMARY KEY,
						RxCui varchar(255) NOT NULL,
						MmslCode varchar(255) NOT NULL,
						Description varchar(255) NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE rxnorm'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE rxnorm (
						RxNormNum number(20) NOT NULL,
						RxCui varchar2(255),
						MmslCode varchar2(255),
						Description varchar2(255),
						CONSTRAINT rxnorm_RxNormNum PRIMARY KEY (RxNormNum)
						)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ehrprovkey";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrprovkey (
						EhrProvKeyNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						LName varchar(255) NOT NULL,
						FName varchar(255) NOT NULL,
						ProvKey varchar(255) NOT NULL,
						FullTimeEquiv float NOT NULL,
						Notes text NOT NULL,
						INDEX(PatNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ehrprovkey'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrprovkey (
						EhrProvKeyNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						LName varchar2(255),
						FName varchar2(255),
						ProvKey varchar2(255),
						FullTimeEquiv number(38,8) NOT NULL,
						Notes varchar2(4000),
						CONSTRAINT ehrprovkey_EhrProvKeyNum PRIMARY KEY (EhrProvKeyNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrprovkey_PatNum ON ehrprovkey (PatNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString,Comments) VALUES('EhrProvKeyGeneratorPath','','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString,Comments) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'EhrProvKeyGeneratorPath','','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString,Comments) VALUES('ApptPrintColumnsPerPage','','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString,Comments) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ApptPrintColumnsPerPage','','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString,Comments) VALUES('ApptPrintFontSize','','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString,Comments) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ApptPrintFontSize','','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString,Comments) VALUES('ApptPrintTimeStart','','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString,Comments) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ApptPrintTimeStart','','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString,Comments) VALUES('ApptPrintTimeStop','','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString,Comments) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ApptPrintTimeStop','','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE provider ADD StateRxID varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE provider ADD StateRxID varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE medicalorder ADD ProvNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE medicalorder ADD INDEX (ProvNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE medicalorder ADD ProvNum number(20)";
					Db.NonQ(command);
					command="UPDATE medicalorder SET ProvNum = 0 WHERE ProvNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE medicalorder MODIFY ProvNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX medicalorder_ProvNum ON medicalorder (ProvNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE medicationpat ADD ProvNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE medicationpat ADD INDEX (ProvNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE medicationpat ADD ProvNum number(20)";
					Db.NonQ(command);
					command="UPDATE medicationpat SET ProvNum = 0 WHERE ProvNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE medicationpat MODIFY ProvNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX medicationpat_ProvNum ON medicationpat (ProvNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vitalsign ADD HasFollowupPlan tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vitalsign ADD HasFollowupPlan number(3)";
					Db.NonQ(command);
					command="UPDATE vitalsign SET HasFollowupPlan = 0 WHERE HasFollowupPlan IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE vitalsign MODIFY HasFollowupPlan NOT NULL";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vitalsign ADD IsIneligible tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vitalsign ADD IsIneligible number(3)";
					Db.NonQ(command);
					command="UPDATE vitalsign SET IsIneligible = 0 WHERE IsIneligible IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE vitalsign MODIFY IsIneligible NOT NULL";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vitalsign ADD Documentation text NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vitalsign ADD Documentation varchar2(4000)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE insplan ADD CanadianDiagnosticCode varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE insplan ADD CanadianDiagnosticCode varchar2(255)";
					Db.NonQ(command);
				} if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE insplan ADD CanadianInstitutionCode varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE insplan ADD CanadianInstitutionCode varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE canadiannetwork ADD CanadianIsRprHandler tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE canadiannetwork ADD CanadianIsRprHandler number(3)";
					Db.NonQ(command);
					command="UPDATE canadiannetwork SET CanadianIsRprHandler = 0 WHERE CanadianIsRprHandler IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE canadiannetwork MODIFY CanadianIsRprHandler NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE rxdef ADD RxCui bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE rxdef ADD INDEX (RxCui)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE rxdef ADD RxCui number(20)";
					Db.NonQ(command);
					command="UPDATE rxdef SET RxCui = 0 WHERE RxCui IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE rxdef MODIFY RxCui NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX rxdef_RxCui ON rxdef (RxCui)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE rxpat ADD RxCui bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE rxpat ADD INDEX (RxCui)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE rxpat ADD RxCui number(20)";
					Db.NonQ(command);
					command="UPDATE rxpat SET RxCui = 0 WHERE RxCui IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE rxpat MODIFY RxCui NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX rxpat_RxCui ON rxpat (RxCui)";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE rxpat ADD DosageCode varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE rxpat ADD DosageCode varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE insplan ADD RxBIN varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE insplan ADD RxBIN varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ehrsummaryccd";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrsummaryccd (
						EhrSummaryCcdNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						DateSummary date NOT NULL DEFAULT '0001-01-01',
						ContentSummary text NOT NULL,
						INDEX(PatNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ehrsummaryccd'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrsummaryccd (
						EhrSummaryCcdNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						DateSummary date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						ContentSummary clob,
						CONSTRAINT ehrsummaryccd_EhrSummaryCcdNum PRIMARY KEY (EhrSummaryCcdNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrsummaryccd_PatNum ON ehrsummaryccd (PatNum)";
					Db.NonQ(command);
				}
				//Add ProcDelete permission to all who had ProcComplEdit------------------------------------------------------
				command="SELECT NewerDate,NewerDays,UserGroupNum FROM grouppermission WHERE PermType=10";
				DataTable table=Db.GetTable(command);
				DateTime newerDate;
				int newerDays;
				long groupNum;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						newerDate=PIn.Date(table.Rows[i][0].ToString());
						newerDays=PIn.Int(table.Rows[i][1].ToString());
						groupNum=PIn.Long(table.Rows[i][2].ToString());
						command="INSERT INTO grouppermission (NewerDate,NewerDays,UserGroupNum,PermType) "
							+"VALUES("+POut.Date(newerDate)+","+POut.Int(newerDays)+","+POut.Long(groupNum)+","+POut.Int((int)Permissions.ProcDelete)+")";
						Db.NonQ32(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						newerDate=PIn.Date(table.Rows[i][0].ToString());
						newerDays=PIn.Int(table.Rows[i][1].ToString());
						groupNum=PIn.Long(table.Rows[i][2].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDate,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),"+POut.Date(newerDate)+","+POut.Int(newerDays)+","+POut.Long(groupNum)+","+POut.Int((int)Permissions.ProcDelete)+")";
						Db.NonQ32(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vaccinepat ADD NotGiven tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vaccinepat ADD NotGiven number(3)";
					Db.NonQ(command);
					command="UPDATE vaccinepat SET NotGiven = 0 WHERE NotGiven IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE vaccinepat MODIFY NotGiven NOT NULL";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vaccinepat ADD Note text NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vaccinepat ADD Note varchar2(4000)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vitalsign ADD ChildGotNutrition tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vitalsign ADD ChildGotNutrition number(3)";
					Db.NonQ(command);
					command="UPDATE vitalsign SET ChildGotNutrition = 0 WHERE ChildGotNutrition IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE vitalsign MODIFY ChildGotNutrition NOT NULL";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vitalsign ADD ChildGotPhysCouns tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vitalsign ADD ChildGotPhysCouns number(3)";
					Db.NonQ(command);
					command="UPDATE vitalsign SET ChildGotPhysCouns = 0 WHERE ChildGotPhysCouns IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE vitalsign MODIFY ChildGotPhysCouns NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="UPDATE icd9 SET ICD9Code=CONCAT(SUBSTR(ICD9Code,1,3),'.',SUBSTR(ICD9Code,4)) WHERE ICD9Code REGEXP '^[VE0-9]{3}[^.]?[0-9]+'";
					//explanation of the regular expression: All codes where the first three characters are V, E, or 0-9.  The fourth character must not be a period , so [^.]? means zero or more characters that are not a period.  And then [0-9]+ indicates 1 or more numbers after that. That's a complicated way of saying that we will not include codes that have already been converted to period format, and that we will not stick a period on codes that are only 3 numbers long.
					Db.NonQ(command);
				}
				else {//oracle
					command="UPDATE icd9 SET ICD9Code=CONCAT(SUBSTR(ICD9Code,1,3),CONCAT('.',SUBSTR(ICD9Code,4))) WHERE REGEXP_LIKE(ICD9Code, '^[VE0-9]{3}[^.]?[0-9]+')";
					//See above for explanation of the regular expression.
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ehrquarterlykey";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrquarterlykey (
						EhrQuarterlyKeyNum bigint NOT NULL auto_increment PRIMARY KEY,
						YearValue int NOT NULL,
						QuarterValue int NOT NULL,
						PracticeName varchar(255) NOT NULL,
						KeyValue varchar(255) NOT NULL,
						PatNum bigint NOT NULL,
						Notes text NOT NULL,
						INDEX(PatNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ehrquarterlykey'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrquarterlykey (
						EhrQuarterlyKeyNum number(20) NOT NULL,
						YearValue number(11) NOT NULL,
						QuarterValue number(11) NOT NULL,
						PracticeName varchar2(255),
						KeyValue varchar2(255),
						PatNum number(20) NOT NULL,
						Notes varchar2(4000),
						CONSTRAINT ehrquarterlykey_EhrQuarterlyKe PRIMARY KEY (EhrQuarterlyKeyNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrquarterlykey_PatNum ON ehrquarterlykey (PatNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD6' LIMIT 1";
				}
				else {//oracle doesn't have LIMIT
					command="SELECT * FROM (SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD6') WHERE RowNum<=1";
				}
				DataTable tableClaimFormNum=Db.GetTable(command);
				if(tableClaimFormNum.Rows.Count>0) {
					long claimFormNum=PIn.Long(tableClaimFormNum.Rows[0][0].ToString());
					command="UPDATE claimformitem SET FieldName='SubscrIDStrict' WHERE FieldName='SubscrID' AND ClaimFormNum="+POut.Long(claimFormNum);
					Db.NonQ(command);
					command="UPDATE claimformitem SET FieldName='PatIDFromPatPlan' WHERE FieldName='PatientID-MedicaidOrSSN' AND ClaimFormNum="+POut.Long(claimFormNum);
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '11.0.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To11_0_9();
		}

		///<summary>Oracle compatible: 10/13/2011</summary>
		private static void To11_0_9() {
			if(FromVersion<new Version("11.0.9.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 11.0.9");//No translation in convert script.
				string command;
				//ClaimX Clearing House
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO clearinghouse(Description,ExportPath,IsDefault,Payors,Eformat,ResponsePath,CommBridge,ClientProgram,ISA05,ISA07,ISA08,ISA15,GS03) ";
					command+="VALUES(";
					command+="'ClaimX'";//Description
					command+=",'"+POut.String(@"C:\ClaimX\Temp\")+"'";//ExportPath that the X12 is placed into
					command+=",'0'";//IsDefault
					command+=",''";//Payors
					command+=",'1'";//Eformat-1=X12
					command+=",''";//ResponsePath
					command+=",'12'";//CommBridge-12=ClaimX
					command+=",'"+POut.String(@"C:\ProgramFiles\ClaimX\claimxclient.exe")+"'";//ClientProgram
					command+=",'30'";//ISA05 
					command+=",'30'";//ISA07 
					command+=",'351962405'";//ISA08 
					command+=",'P'";//ISA15-P=Production, T=Test
					command+=",'351962405'";//GS03
					command+=")";
					Db.NonQ(command);
				}
				else{//oracle
					command="INSERT INTO clearinghouse(ClearinghouseNum,Description,ExportPath,IsDefault,Payors,Eformat,ResponsePath,CommBridge,ClientProgram,ISA05,ISA07,ISA08,ISA15,GS03) ";
					command+="VALUES(";
					command+="(SELECT MAX(ClearinghouseNum)+1 FROM clearinghouse)";
					command+=",'ClaimX'";//Description
					command+=",'"+POut.String(@"C:\ClaimX\Temp\")+"'";//ExportPath that the X12 is placed into
					command+=",'0'";//IsDefault
					command+=",''";//Payors
					command+=",'1'";//Eformat-1=X12
					command+=",''";//ResponsePath
					command+=",'12'";//CommBridge-12=ClaimX
					command+=",'"+POut.String(@"C:\ProgramFiles\ClaimX\claimxclient.exe")+"'";//ClientProgram
					command+=",'30'";//ISA05
					command+=",'30'";//ISA07
					command+=",'351962405'";//ISA08 
					command+=",'P'";//ISA15-P=Production, T=Test
					command+=",'351962405'";//GS03
					command+=")";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '11.0.9.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To11_0_10();
		}
		
		///<summary>Oracle compatible: 10/13/2011</summary>
		private static void To11_0_10() {
			if(FromVersion<new Version("11.0.10.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 11.0.10");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD6' LIMIT 1";
				}
				else {//oracle doesn't have LIMIT
					command="SELECT * FROM (SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD6') WHERE RowNum<=1";
				}
				DataTable tableClaimFormNum=Db.GetTable(command);
				if(tableClaimFormNum.Rows.Count>0) {
					long claimFormNum=PIn.Long(tableClaimFormNum.Rows[0][0].ToString());
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES ("+POut.Long(claimFormNum)+",'PatientPatNum',494,117,112,16)";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormNum,FieldName,XPos,YPos,Width,Height) VALUES ("+POut.Long(claimFormNum)+",'BillingDentistNPI',308,117,103,16)";
					Db.NonQ(command);
				}
				//It is OK to run the following queries for all of our customers, because if they are not Canadian, then only the Canadian columns will be changed and will then not ever be used.
				//We do not want to check the region settings here because sometimes Canadian customers forget to set the region correctly on their computer before installing/upgrading.
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=32,CanadianNetworkNum=7 WHERE ElectID='311140'";//accerta
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=2469,CanadianNetworkNum=8 WHERE ElectID='000105'";//adsc
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=416,CanadianNetworkNum=7 WHERE ElectID='610226'";//aga
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='02',CanadianSupportedTypes=420,CanadianNetworkNum=7 WHERE ElectID='628112'";//appq
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=2468,CanadianNetworkNum=13 WHERE ElectID='000090'";//alberta blue cross
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=0,CanadianNetworkNum=7 WHERE ElectID='610191'";//assumption life
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=416,CanadianNetworkNum=7 WHERE ElectID='628151'";//autoben
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=416,CanadianNetworkNum=8 WHERE ElectID='610708'";//benecaid health benefit solutions
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='02',CanadianSupportedTypes=384,CanadianNetworkNum=7 WHERE ElectID='610146'";//benefits trust
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=416,CanadianNetworkNum=7 WHERE ElectID='400008'";//beneplan
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=0,CanadianNetworkNum=7 WHERE ElectID='600502'";//capitale
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=416,CanadianNetworkNum=9 WHERE ElectID='610129'";//cdcs
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=1,CanadianNetworkNum=7 WHERE ElectID='610099'";//claimsecure
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='02',CanadianSupportedTypes=32,CanadianNetworkNum=7 WHERE ElectID='000036'";//ccq
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=416,CanadianNetworkNum=8 WHERE ElectID='606258'";//co-operators
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=2464,CanadianNetworkNum=7 WHERE ElectID='610105'";//coughlin & associates
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=32,CanadianNetworkNum=8 WHERE ElectID='610153'";//cowan wright beauchamps
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=384,CanadianNetworkNum=8 WHERE ElectID='000051'";//desjardins financial security
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=384,CanadianNetworkNum=7 WHERE ElectID='000033'";//empire life insurance company
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='02',CanadianSupportedTypes=416,CanadianNetworkNum=10 WHERE ElectID='000029'";//equitable life
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=65956,CanadianNetworkNum=7 WHERE ElectID='610650'";//esorse corporation
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=416,CanadianNetworkNum=7 WHERE ElectID='610614'";//fas administrators
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='02',CanadianSupportedTypes=416,CanadianNetworkNum=10 WHERE ElectID='000011'";//great west life assurance company
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=420,CanadianNetworkNum=7 WHERE ElectID='000102'";//green sheild canada
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=416,CanadianNetworkNum=8 WHERE ElectID='610217'";//group medical services
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=416,CanadianNetworkNum=8 WHERE ElectID='610218'";//group medical services saskatchewan
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=33,CanadianNetworkNum=8 WHERE ElectID='605064'";//groupsource
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='02',CanadianSupportedTypes=416,CanadianNetworkNum=10 WHERE ElectID='000060'";//industrial alliance
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='02',CanadianSupportedTypes=416,CanadianNetworkNum=10 WHERE ElectID='000024'";//industrial alliance pacific insuarnce and financial
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=32,CanadianNetworkNum=8 WHERE ElectID='610643'";//internationale campagnie d'assurance vie
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=0,CanadianNetworkNum=7 WHERE ElectID='627265'";//johnson inc.
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='02',CanadianSupportedTypes=416,CanadianNetworkNum=10 WHERE ElectID='627223'";//johnston group
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='02',CanadianSupportedTypes=0,CanadianNetworkNum=7 WHERE ElectID='627585'";//lee-power & associates
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=32,CanadianNetworkNum=7 WHERE ElectID='610158'";//manion wilkins
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=2464,CanadianNetworkNum=11 WHERE ElectID='000094'";//manitoba blue cross
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=2432,CanadianNetworkNum=8 WHERE ElectID='000114'";//manitoba cleft palate program
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=2048,CanadianNetworkNum=8 WHERE ElectID='000113'";//manitoba health
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='02',CanadianSupportedTypes=416,CanadianNetworkNum=7 WHERE ElectID='000034'";//manufacturers life insurance company
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='02',CanadianSupportedTypes=416,CanadianNetworkNum=7 WHERE ElectID='610059'";//manulife financial
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='02',CanadianSupportedTypes=416,CanadianNetworkNum=7 WHERE ElectID='311113'";//maritime life assurance company
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=416,CanadianNetworkNum=7 WHERE ElectID='610070'";//maritime pro
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='02',CanadianSupportedTypes=417,CanadianNetworkNum=7 WHERE ElectID='601052'";//mdm
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='02',CanadianSupportedTypes=384,CanadianNetworkNum=7 WHERE ElectID='610047'";//medavie blue cross
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=416,CanadianNetworkNum=8 WHERE ElectID='610634'";//nexgenrx
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=32,CanadianNetworkNum=8 WHERE ElectID='610124'";//nihb
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=2469,CanadianNetworkNum=8 WHERE ElectID='000109'";//nova scotia community services
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=2469,CanadianNetworkNum=8 WHERE ElectID='000108'";//nova scotia medical services insurance
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=32,CanadianNetworkNum=8 WHERE ElectID='610172'";//nunatsiavut government department of health
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=2432,CanadianNetworkNum=12 WHERE ElectID='000064'";//pacific blue cross
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=2469,CanadianNetworkNum=8 WHERE ElectID='000103'";//quickcard
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=384,CanadianNetworkNum=8 WHERE ElectID='610256'";//pbas
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=32,CanadianNetworkNum=7 WHERE ElectID='610616'";//rwam insurance
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=0,CanadianNetworkNum=7 WHERE ElectID='000096'";//saskatchewan blue cross
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=32,CanadianNetworkNum=7 WHERE ElectID='610196'";//ses benefits
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=0,CanadianNetworkNum=8 WHERE ElectID='000079'";//ssq societe d'assurance-vie inc.
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=416,CanadianNetworkNum=7 WHERE ElectID='000020'";//standard life assurance company
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='02',CanadianSupportedTypes=416,CanadianNetworkNum=10 WHERE ElectID='000016'";//sun life of canada
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=0,CanadianNetworkNum=8 WHERE ElectID='000080'";//survivance
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='04',CanadianSupportedTypes=32,CanadianNetworkNum=8 WHERE ElectID='610677'";//syndicat des fonctionnaires municipaux mtl
				Db.NonQ(command);
				command="UPDATE carrier SET CDAnetVersion='02',CanadianSupportedTypes=416,CanadianNetworkNum=7 WHERE ElectID='311109'";//wawanesa
				Db.NonQ(command);
				//We only want to insert these carriers if in Canada because they are of no use elsewhere.
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					command="SELECT COUNT(*) FROM carrier WHERE ElectID='000090'"; //alberta blue cross
					if(Db.GetCount(command)=="0") {
						command="INSERT INTO carrier (CarrierName,Address,Address2,City,State,Zip,Phone,ElectID,NoSendElect,IsCDA,CDAnetVersion,CanadianNetworkNum,IsHidden,CanadianEncryptionMethod,CanadianSupportedTypes) VALUES "+
								"('Alberta Blue Cross','10009 108th Street NW','','Edmonton','AB','T5J 3C5','1-800-661-6995','000090','0','1','04',13,'0',1,2468)";
						Db.NonQ(command);
					}
					command="SELECT COUNT(*) FROM carrier WHERE ElectID='000114'"; //manitoba cleft palate program
					if(Db.GetCount(command)=="0") {
						command="INSERT INTO carrier (CarrierName,Address,Address2,City,State,Zip,Phone,ElectID,NoSendElect,IsCDA,CDAnetVersion,CanadianNetworkNum,IsHidden,CanadianEncryptionMethod,CanadianSupportedTypes) VALUES "+
								"('Manitoba Cleft Palate Program','300 Carlton Street','','Winnipeg','MB','R3B 3M9','204-787-4882','000114','0','1','04',8,'0',1,2432)";
						Db.NonQ(command);
					}
					command="SELECT COUNT(*) FROM carrier WHERE ElectID='000113'"; //manitoba health
					if(Db.GetCount(command)=="0") {
						command="INSERT INTO carrier (CarrierName,Address,Address2,City,State,Zip,Phone,ElectID,NoSendElect,IsCDA,CDAnetVersion,CanadianNetworkNum,IsHidden,CanadianEncryptionMethod,CanadianSupportedTypes) VALUES "+
								"('Manitoba Health','300 Carlton Street','','Winnipeg','MB','R3B 3M9','204-788-2581','000113','0','1','04',8,'0',1,2048)";
						Db.NonQ(command);
					}
					command="SELECT COUNT(*) FROM carrier WHERE ElectID='610256'"; //pbas
					if(Db.GetCount(command)=="0") {
						command="INSERT INTO carrier (CarrierName,Address,Address2,City,State,Zip,Phone,ElectID,NoSendElect,IsCDA,CDAnetVersion,CanadianNetworkNum,IsHidden,CanadianEncryptionMethod,CanadianSupportedTypes) VALUES "+
								"('PBAS','318-2099 Lougheed Highway','','Port Coquitlam','BC','V3B 1A8','800-952-9932','610256','0','1','04',8,'0',1,384)";
						Db.NonQ(command);
					}
				}
				command="UPDATE preference SET ValueString = '11.0.10.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To11_0_11();
		}
		
		///<summary>Oracle compatible: 10/13/2011</summary>
		private static void To11_0_11() {
			if(FromVersion<new Version("11.0.11.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 11.0.11");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					//Bug fix for disease column (depricated) being in use. This code is identical to the function run again in 11.0 and is safe to run more than once.
					bool diseasefieldused=false;
					bool problemfieldused=false;
					bool allergyfieldused=false;
					command="SELECT ItemOrder FROM displayfield WHERE InternalName='Diseases'";
					string str=Db.GetScalar(command);
					int itemOrder=0;
					if(!String.IsNullOrEmpty(str)) {
						diseasefieldused=true;
						itemOrder=PIn.Int(str);
					}
					command="SELECT * FROM displayfield WHERE InternalName='Problems'";
					if(Db.GetTable(command).Rows.Count>0) {
						problemfieldused=true;
					}
					command="SELECT * FROM displayfield WHERE InternalName='Allergies'";
					if(Db.GetTable(command).Rows.Count>0) {
						allergyfieldused=true;
					}
					if(diseasefieldused && !problemfieldused && !allergyfieldused) {//disease is used, problems and allergies are not used
						command="DELETE FROM displayfield WHERE InternalName='Diseases' AND Category=5";
						Db.NonQ(command);
						command="INSERT INTO displayfield (InternalName,Description,ItemOrder,ColumnWidth,Category) VALUES ('Problems','',"+POut.Int(itemOrder)+",0,5)";
						Db.NonQ(command);
						command="INSERT INTO displayfield (InternalName,Description,ItemOrder,ColumnWidth,Category) VALUES ('Allergies','',"+POut.Int(itemOrder)+",0,5)";
						Db.NonQ(command);
					}
				}
				else {//oracle
					//do nothing
				}
				command="UPDATE preference SET ValueString = '11.0.11.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To11_0_13();
		}
		
		///<summary>Oracle compatible: 10/13/2011</summary>
		private static void To11_0_13() {
			if(FromVersion<new Version("11.0.13.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 11.0.13");//No translation in convert script.
				string command;
				try {//most users will not have this table
					command="ALTER TABLE phoneempdefault ADD PhoneExt int NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE phoneempdefault ADD IsUnavailable tinyint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE phoneempdefault ADD Notes text NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE phoneempdefault ADD IpAddress varchar(255) NOT NULL";
					Db.NonQ(command);
					command="DROP TABLE phoneoverride";
					Db.NonQ(command);
				}
				catch {
					//do nothing
				}
				command="UPDATE preference SET ValueString = '11.0.13.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To11_0_15();
		}
		
		///<summary>Oracle compatible: 10/13/2011</summary>
		private static void To11_0_15() {
			if(FromVersion<new Version("11.0.15.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 11.0.15");//No translation in convert script.
				string command;
				try {//most users will not have this table
					command="ALTER TABLE phone ADD ScreenshotPath varchar(255) NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE phone ADD ScreenshotImage mediumtext NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE phoneempdefault ADD IsPrivateScreen tinyint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE phoneempdefault CHANGE IpAddress ComputerName varchar(255) NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE phoneempdefault CHANGE IsUnavailable StatusOverride tinyint NOT NULL";
					Db.NonQ(command);
				}
				catch {
					//do nothing
				}
				command="UPDATE preference SET ValueString = '11.0.15.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To11_0_24();
		}
		
		///<summary>Oracle compatible: 10/13/2011</summary>
		private static void To11_0_24() {
			if(FromVersion<new Version("11.0.24.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 11.0.24");//No translation in convert script.
				string command;
				command="ALTER TABLE allergydef DROP COLUMN RxCui";//both oracle and mysql
				Db.NonQ(command);
				//Primary key was renamed in 6.8.1 on accident.  Changing back so generating XML documentation works correctly.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE insfilingcodesubtype CHANGE InsFilingCodeSubTypeNum InsFilingCodeSubtypeNum BIGINT(20) NOT NULL AUTO_INCREMENT";
					Db.NonQ(command);
				}
				else {//oracle
					//No need to change column name in Oracle, strictly affects XML documentation generation.
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE ehrprovkey ADD HasReportAccess tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE ehrprovkey ADD HasReportAccess number(3)";
					Db.NonQ(command);
					command="UPDATE ehrprovkey SET HasReportAccess = 0 WHERE HasReportAccess IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE ehrprovkey MODIFY HasReportAccess NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE provider ADD EhrHasReportAccess tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE provider ADD EhrHasReportAccess number(3)";
					Db.NonQ(command);
					command="UPDATE provider SET EhrHasReportAccess = 0 WHERE EhrHasReportAccess IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE provider MODIFY EhrHasReportAccess NOT NULL";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '11.0.24.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To11_0_36();
		}

		private static void To11_0_36() {
			if(FromVersion<new Version("11.0.36.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 11.0.36");//No translation in convert script.
				string command;
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE procedurelog ADD INDEX procedurelog_ProcNumLab (ProcNumLab)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX procedurelog_ProcNumLab ON procedurelog (ProcNumLab)";
						Db.NonQ(command);
					}
				}
				catch {
					//Oh well, it's just an index. Probably failed because it already exists anyway.
				}
				command="UPDATE preference SET ValueString = '11.0.36.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To11_1_1();
		}
		
		///<summary>Oracle compatible: 10/13/2011</summary>
		private static void To11_1_1() {
			if(FromVersion<new Version("11.1.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 11.1.1");//No translation in convert script.
				string command;
				//Set default appt schedule printing preferences.  Was released when not finished so can't trust current values.
				command="UPDATE preference SET ValueString="+POut.DateT(new DateTime(2011,1,1,0,0,0))+" WHERE PrefName='ApptPrintTimeStart'";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString="+POut.DateT(new DateTime(2011,1,1,23,0,0))+" WHERE PrefName='ApptPrintTimeStop'";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString='8' WHERE PrefName='ApptPrintFontSize'";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString='10' WHERE PrefName='ApptPrintColumnsPerPage'";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ScannerSuppressDialog','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ScannerSuppressDialog','1')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ScannerResolution','150')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ScannerResolution','150')";
					Db.NonQ(command);
				}
				command="DELETE FROM preference WHERE PrefName = 'ScannerCompressionRadiographs'";
				Db.NonQ(command);
				command="DELETE FROM preference WHERE PrefName = 'ScannerCompressionPhotos'";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE payment ADD IsRecurringCC tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE payment ADD IsRecurringCC number(3)";
					Db.NonQ(command);
					command="UPDATE payment SET IsRecurringCC = 0 WHERE IsRecurringCC IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE payment MODIFY IsRecurringCC NOT NULL";
					Db.NonQ(command);
				}
				//To keep current functionality, set all payments up to this point as recurring charges.
				command="UPDATE payment SET IsRecurringCC=1";
				Db.NonQ(command);
				try{
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE procedurelog ADD INDEX (ProcDate)";
						Db.NonQ(command);
						command="ALTER TABLE paysplit ADD INDEX (DatePay)";
						Db.NonQ(command);
					}
					else {//oracle
						command="CREATE INDEX procedurelog_ProcDate ON procedurelog (ProcDate)";
						Db.NonQ(command);
						command="CREATE INDEX paysplit_DatePay ON paysplit (DatePay)";
						Db.NonQ(command);
					}				
				}
				catch{ }
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS dashboardar";
					Db.NonQ(command);
					command=@"CREATE TABLE dashboardar (
						DashboardARNum bigint NOT NULL auto_increment PRIMARY KEY,
						DateCalc date NOT NULL DEFAULT '0001-01-01',
						BalTotal double NOT NULL,
						InsEst double NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE dashboardar'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE dashboardar (
						DashboardARNum number(20) NOT NULL,
						DateCalc date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						BalTotal number(38,8) NOT NULL,
						InsEst number(38,8) NOT NULL,
						CONSTRAINT dashboardar_DashboardARNum PRIMARY KEY (DashboardARNum)
						)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ProcCodeListShowHidden','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ProcCodeListShowHidden','1')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claimpayment ADD DateIssued date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claimpayment ADD DateIssued date";
					Db.NonQ(command);
					command="UPDATE claimpayment SET DateIssued = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateIssued IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimpayment MODIFY DateIssued NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE sheetfield ADD TabOrder int NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE sheetfield ADD TabOrder number(11)";
					Db.NonQ(command);
					command="UPDATE sheetfield SET TabOrder = 0 WHERE TabOrder IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE sheetfield MODIFY TabOrder NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE sheetfielddef ADD TabOrder int NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE sheetfielddef ADD TabOrder number(11)";
					Db.NonQ(command);
					command="UPDATE sheetfielddef SET TabOrder = 0 WHERE TabOrder IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE sheetfielddef MODIFY TabOrder NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS aggpath";
					Db.NonQ(command);
					command=@"CREATE TABLE aggpath (
						AggPathNum bigint NOT NULL auto_increment PRIMARY KEY,
						RemoteURI varchar(255) NOT NULL,
						RemoteUserName varchar(255) NOT NULL,
						RemotePassword varchar(255) NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE aggpath'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE aggpath (
						AggPathNum number(20) NOT NULL,
						RemoteURI varchar2(255),
						RemoteUserName varchar2(255),
						RemotePassword varchar2(255),
						CONSTRAINT aggpath_AggPathNum PRIMARY KEY (AggPathNum)
						)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('AppointmentSearchBehavior','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'AppointmentSearchBehavior','0')";
					Db.NonQ(command);
				}
				//Insert Apixia Imaging Bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'Apixia', "
						+"'Apixia Digital Imaging by Apixia Inc.', "
						+"'0', "
						+"'"+POut.String(@"C:\Program Files\Digirex\digirex.exe")+"',"
						+"'', "
						+"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'System path to Apixia Digital Imaging ini file', "
						+"'"+POut.String(@"C:\Program Files\Digirex\Switch.ini")+"')";
					Db.NonQ32(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"'"+POut.Long(programNum)+"', "
						+"'"+POut.Int(((int)ToolBarsAvail.ChartModule))+"', "
						+"'Apixia')";
					Db.NonQ32(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'Apixia', "
						+"'Apixia Digital Imaging by Apixia Inc.', "
						+"'0', "
						+"'"+POut.String(@"C:\Program Files\Digirex\digirex.exe")+"',"
						+"'', "
						+"'')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum)+1 FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'System path to Apixia Digital Imaging ini file', "
						+"'"+POut.String(@"C:\Program Files\Digirex\Switch.ini")+"')";
					Db.NonQ32(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
						+"'"+POut.Long(programNum)+"', "
						+"'"+POut.Int(((int)ToolBarsAvail.ChartModule))+"', "
						+"'Apixia')";
					Db.NonQ32(command);
				}//end Apixia Imaging bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD PriorAuthorizationNumber varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD PriorAuthorizationNumber varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE provider ADD IsNotPerson tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE provider ADD IsNotPerson number(3)";
					Db.NonQ(command);
					command="UPDATE provider SET IsNotPerson = 0 WHERE IsNotPerson IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE provider MODIFY IsNotPerson NOT NULL";
					Db.NonQ(command);
				}
				command="UPDATE provider SET IsNotPerson=1 WHERE FName=''";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD SpecialProgramCode tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD SpecialProgramCode number(3)";
					Db.NonQ(command);
					command="UPDATE claim SET SpecialProgramCode = 0 WHERE SpecialProgramCode IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim MODIFY SpecialProgramCode NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD UniformBillType varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD UniformBillType varchar2(255)";
					Db.NonQ(command);
				}
				//Add Providers permission to groups with existing Setup permission------------------------------------------------------
				command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType="+POut.Int((int)Permissions.Setup);
				DataTable table=Db.GetTable(command);
				long groupNum;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+","+POut.Int((int)Permissions.Providers)+")";
						Db.NonQ32(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+","+POut.Int((int)Permissions.Providers)+")";
						Db.NonQ32(command);
					}
				}
				//Add ProcedureNoteFull permission to everyone------------------------------------------------------
				command="SELECT DISTINCT UserGroupNum FROM grouppermission";
				table=Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+",53)";//ProcedureNoteFull
						Db.NonQ32(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",53)";//ProcedureNoteFull
						Db.NonQ32(command);
					}
				}
				//Add ReferralAdd permission to everyone------------------------------------------------------
				command="SELECT DISTINCT UserGroupNum FROM grouppermission";
				table=Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+","+POut.Int((int)Permissions.ReferralAdd)+")";
						Db.NonQ32(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+","+POut.Int((int)Permissions.ReferralAdd)+")";
						Db.NonQ32(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD MedType tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD MedType number(3)";
					Db.NonQ(command);
					command="UPDATE claim SET MedType = 0 WHERE MedType IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim MODIFY MedType NOT NULL";
					Db.NonQ(command);
				}
				command="ALTER TABLE claim DROP COLUMN EFormat";
				Db.NonQ(command);
				command="ALTER TABLE procedurelog DROP COLUMN UnitCode";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ClaimMedTypeIsInstWhenInsPlanIsMedical','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ClaimMedTypeIsInstWhenInsPlanIsMedical','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurelog ADD DrugUnit tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurelog ADD DrugUnit number(3)";
					Db.NonQ(command);
					command="UPDATE procedurelog SET DrugUnit = 0 WHERE DrugUnit IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog MODIFY DrugUnit NOT NULL";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurelog ADD DrugQty float NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurelog ADD DrugQty number(38,8)";
					Db.NonQ(command);
					command="UPDATE procedurelog SET DrugQty = 0 WHERE DrugQty IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog MODIFY DrugQty NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurecode ADD DrugNDC varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurecode ADD DrugNDC varchar2(255)";
					Db.NonQ(command);
				}			
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurecode ADD RevenueCodeDefault varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurecode ADD RevenueCodeDefault varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD AdmissionTypeCode varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD AdmissionTypeCode varchar2(255)";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD AdmissionSourceCode varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD AdmissionSourceCode varchar2(255)";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD PatientStatusCode varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD PatientStatusCode varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ClearinghouseDefaultDent','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ClearinghouseDefaultDent','0')";
					Db.NonQ(command);
				}
				//set this new pref with the existing default value from the clearinghouse table
				command="SELECT ClearinghouseNum FROM clearinghouse WHERE IsDefault=1";
				long clearinghouseNum=PIn.Long(Db.GetScalar(command));
				command="UPDATE preference SET ValueString="+POut.Long(clearinghouseNum)+" WHERE PrefName='ClearinghouseDefaultDent'";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ClearinghouseDefaultMed','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ClearinghouseDefaultMed','0')";
					Db.NonQ(command);
				}
				command="ALTER TABLE clearinghouse DROP COLUMN IsDefault";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claimpayment ADD IsPartial tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claimpayment ADD IsPartial number(3)";
					Db.NonQ(command);
					command="UPDATE claimpayment SET IsPartial = 0 WHERE IsPartial IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimpayment MODIFY IsPartial NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claimproc ADD PaymentRow int NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claimproc ADD PaymentRow number(11)";
					Db.NonQ(command);
					command="UPDATE claimproc SET PaymentRow = 0 WHERE PaymentRow IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimproc MODIFY PaymentRow NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patient ADD SuperFamily bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE patient ADD INDEX (SuperFamily)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patient ADD SuperFamily number(20)";
					Db.NonQ(command);
					command="UPDATE patient SET SuperFamily = 0 WHERE SuperFamily IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE patient MODIFY SuperFamily NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX patient_SuperFamily ON patient (SuperFamily)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ShowFeatureSuperfamilies','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ShowFeatureSuperfamilies','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE refattach ADD ProcNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE refattach ADD INDEX (ProcNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE refattach ADD ProcNum number(20)";
					Db.NonQ(command);
					command="UPDATE refattach SET ProcNum = 0 WHERE ProcNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE refattach MODIFY ProcNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX refattach_ProcNum ON refattach (ProcNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE refattach ADD DateProcComplete date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE refattach ADD DateProcComplete date";
					Db.NonQ(command);
					command="UPDATE refattach SET DateProcComplete = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateProcComplete IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE refattach MODIFY DateProcComplete NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ProcGroupNoteDoesAggregate','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ProcGroupNoteDoesAggregate','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE statement ADD DateTStamp timestamp";
					Db.NonQ(command);
					command="UPDATE statement SET DateTStamp = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE statement ADD DateTStamp timestamp";
					Db.NonQ(command);
					command="UPDATE statement SET DateTStamp = SYSTIMESTAMP";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '11.1.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To11_1_6();
		}

		private static void To11_1_6() {
			if(FromVersion<new Version("11.1.6.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 11.1.6");//No translation in convert script.
				string command;
				//We added an index in version 11.0.36 for this column, but some of our customers were already on version 11.1, so we had to add it here as well.
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE procedurelog ADD INDEX procedurelog_ProcNumLab (ProcNumLab)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX procedurelog_ProcNumLab ON procedurelog (ProcNumLab)";
						Db.NonQ(command);
					}
				}
				catch {
					//Oh well, it's just an index. Probably failed because it already exists anyway.
				}
				command="UPDATE preference SET ValueString = '11.1.6.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To11_1_7();
		}

		private static void To11_1_7() {
			if(FromVersion<new Version("11.1.7.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 11.1.7");//No translation in convert script.
				string command;
				command="ALTER TABLE insplan DROP COLUMN DedBeforePerc";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.Oracle) {
					command="ALTER TABLE insplan MODIFY CanadianPlanFlag NULL";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '11.1.7.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To11_1_9();
		}

		///<summary>Oracle compatible: 11/17/2011</summary>
		private static void To11_1_9() {
			if(FromVersion<new Version("11.1.9.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 11.1.9");//No translation in convert script.
				//Update VixWin Bridge
				string command="Select ProgramNum FROM program WHERE ProgName='VixWin'";
				long programNum=PIn.Long(Db.GetScalar(command));
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+POut.Long(programNum)+", "
						+"'Optional Image Path', "
						+"'')";
					Db.NonQ32(command);
				}
				else {//oracle
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum)+1 FROM programproperty),"
						+POut.Long(programNum)+", "
						+"'Optional Image Path', "
						+"'')";
					Db.NonQ32(command);
				}
				//Insert VixWinBase41 Imaging Bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'VixWinBase41', "
						+"'VixWin(Base41) from www.gendexxray.com', "
						+"'0', "
						+"'"+POut.String(@"C:\VixWin\VixWin.exe")+"',"
						+"'', "
						+"'This VixWin bridge uses base 41 PatNums.')";
					programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+POut.Long(programNum)+", "
						+"'Image Path', "
						+"'')";//User will be required to set up image path before using bridge. If they try to use it they will get a warning message and the bridge will fail gracefully.
					Db.NonQ32(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+POut.Long(programNum)+", "
						+POut.Int(((int)ToolBarsAvail.ChartModule))+", "
						+"'VixWinBase41')";
					Db.NonQ32(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'VixWinBase41', "
						+"'VixWin(Base41) from www.gendexxray.com', "
						+"'0', "
						+"'"+POut.String(@"C:\VixWin\VixWin.exe")+"',"
						+"'', "
						+"'This VixWin bridge uses base 41 PatNums.')";
					programNum=Db.NonQ(command,true,"ProgramNum","program");
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
						+POut.Int(((int)ToolBarsAvail.ChartModule))+", "
						+"'VixWinBase41')";
					Db.NonQ32(command);
				}
				command="UPDATE preference SET ValueString = '11.1.9.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To12_0_1();
		}
		
		///<summary></summary>
		private static void To12_0_1() {
			if(FromVersion<new Version("12.0.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 12.0.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('BillingEmailSubject','Statement for account [PatNum]')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'BillingEmailSubject','Statement for account [PatNum]')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('BillingEmailBodyText','Statement attached for [nameFL], account number [PatNum]')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'BillingEmailBodyText','Statement attached for [nameFL], account number [PatNum]')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE creditcard ADD PayPlanNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE creditcard ADD INDEX (PayPlanNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE creditcard ADD PayPlanNum number(20)";
					Db.NonQ(command);
					command="UPDATE creditcard SET PayPlanNum = 0 WHERE PayPlanNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE creditcard MODIFY PayPlanNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX creditcard_PayPlanNum ON creditcard (PayPlanNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE popup ADD PopupLevel tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE popup ADD PopupLevel number(3)";
					Db.NonQ(command);
					command="UPDATE popup SET PopupLevel = 0 WHERE PopupLevel IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE popup MODIFY PopupLevel NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ChartAddProcNoRefreshGrid','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ChartAddProcNoRefreshGrid','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS eobattach";
					Db.NonQ(command);
					command=@"CREATE TABLE eobattach (
						EobAttachNum bigint NOT NULL auto_increment PRIMARY KEY,
						ClaimPaymentNum bigint NOT NULL,
						DateTCreated datetime NOT NULL,
						FileName varchar(255) NOT NULL,
						RawBase64 text NOT NULL,
						INDEX(ClaimPaymentNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE eobattach'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE eobattach (
						EobAttachNum number(20) NOT NULL,
						ClaimPaymentNum number(20) NOT NULL,
						DateTCreated date NOT NULL,
						FileName varchar2(255),
						RawBase64 clob,
						CONSTRAINT eobattach_EobAttachNum PRIMARY KEY (EobAttachNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX eobattach_ClaimPaymentNum ON eobattach (ClaimPaymentNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE insplan ADD CobRule tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE insplan ADD CobRule number(3)";
					Db.NonQ(command);
					command="UPDATE insplan SET CobRule = 0 WHERE CobRule IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE insplan MODIFY CobRule NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('InsDefaultCobRule','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'InsDefaultCobRule','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('MobileSynchNewTables112Done','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'MobileSynchNewTables112Done','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PatientFormsShowConsent','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PatientFormsShowConsent','0')";
					Db.NonQ(command);
				}
				//Add InsPlanChangeSubsc permission to all groups that had SecurityAdmin permission---------------------------------------------
				long groupNum;
				command="SELECT DISTINCT UserGroupNum "
					+"FROM grouppermission "
					+"WHERE PermType="+POut.Int((int)Permissions.SecurityAdmin);
				DataTable table=Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+","+POut.Int((int)Permissions.InsPlanChangeSubsc)+")";
						Db.NonQ32(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+","+POut.Int((int)Permissions.InsPlanChangeSubsc)+")";
						Db.NonQ32(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('MedicalFeeUsedForNewProcs','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'MedicalFeeUsedForNewProcs','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PracticePayToAddress','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PracticePayToAddress','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PracticePayToAddress2','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PracticePayToAddress2','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PracticePayToCity','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PracticePayToCity','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PracticePayToST','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PracticePayToST','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PracticePayToZip','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PracticePayToZip','')";
					Db.NonQ(command);
				}
				try{
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE patient ADD INDEX (SiteNum)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX patient_SiteNum ON patient (SiteNum)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
				try{
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE claimproc ADD INDEX (Status)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX claimproc_Status ON claimproc (Status)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex){
					ex.DoNothing();
				}
				try{
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE patient ADD INDEX (PatStatus)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX patient_PatStatus ON patient (PatStatus)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex){
					ex.DoNothing();
				}
				try{
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE patient ADD INDEX (ClinicNum)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX patient_ClinicNum ON patient (ClinicNum)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex){
					ex.DoNothing();
				}
				try{
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE appointment ADD INDEX (DateTimeArrived)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX appointment_DateTimeArrived ON appointment (DateTimeArrived)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex){
					ex.DoNothing();
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE apptview ADD ClinicNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE apptview ADD INDEX (ClinicNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE apptview ADD ClinicNum number(20)";
					Db.NonQ(command);
					command="UPDATE apptview SET ClinicNum = 0 WHERE ClinicNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE apptview MODIFY ClinicNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX apptview_ClinicNum ON apptview (ClinicNum)";
					Db.NonQ(command);
				}
				long claimFormNum=0;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO claimform(Description,IsHidden,FontName,FontSize,UniqueID,PrintImages,OffsetX,OffsetY) "+
						"VALUES ('UB04',0,'Tahoma',9.75,'OD10',0,0,0)";
					claimFormNum=Db.NonQ(command,true);
				}
				else {//oracle
					command="INSERT INTO claimform(ClaimFormNum,Description,IsHidden,FontName,FontSize,UniqueID,PrintImages,OffsetX,OffsetY) "+
						"VALUES ((SELECT COALESCE(MAX(ClaimFormNum),0)+1 ClaimFormNum FROM claimform),'UB04',0,'Tahoma',9.75,'OD10',0,0,0)";
					claimFormNum=Db.NonQ(command,true,"ClaimFormNum","claimform");
				}
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'UB04.jpg','','','4','5','860','1120')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','AccidentDate','MMddyyyy','45','164','68','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','AccidentST','','675','130','30','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentist','','24','14','239','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistAddress','','24','30','239','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistAddress2','','24','46','239','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistCity','','24','63','113','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistNPI','','682','682','151','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistSSNorTIN','','515','64','98','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistST','','145','63','30','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistZip','','183','63','80','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','DateService','MMddyy','685','64','68','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','DateService','MMddyyyy','132','130','66','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','DateService','MMddyy','615','64','68','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','DateService','MMddyyyy','460','666','68','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Diagnosis1','NoDec','26','880','78','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Diagnosis2','NoDec','105','881','78','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Diagnosis3','NoDec','185','881','78','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Diagnosis4','NoDec','265','881','78','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','FixedText','1','116','663','10','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','FixedText','1','172','663','10','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','FixedText','0001','15','664','46','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','FixedText','9','13','897','9','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedAccidentCode','','15','164','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedAdmissionSourceCode','','255','130','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedAdmissionTypeCode','','225','130','27','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedConditionCode18','','345','130','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedConditionCode19','','375','130','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedConditionCode20','','405','130','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedConditionCode21','','435','130','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedConditionCode22','','465','130','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedConditionCode23','','495','130','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedConditionCode24','','525','130','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedConditionCode25','','555','130','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedConditionCode26','','585','130','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedConditionCode27','','615','130','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedConditionCode28','','645','130','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsAAmtDue','NoDec','654','697','110','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsAAssignBen','','425','697','18','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsAAuthCode','','15','831','308','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsAEmployer','','584','831','249','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsAGroupName','','504','764','148','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsAGroupNum','','654','764','179','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsAInsuredID','','303','764','199','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsAInsuredName','','15','764','257','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsAName','','15','697','228','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsAOtherProvID','','682','698','151','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsAPlanID','','244','697','148','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsAPriorPmt','NoDec','544','697','101','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsARelation','','274','764','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsARelInfo','','394','697','18','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsBAmtDue','NoDec','654','714','110','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsBAssignBen','','425','714','18','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsBAuthCode','','15','848','308','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsBEmployer','','584','848','249','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsBGroupName','','504','781','148','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsBGroupNum','','654','781','179','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsBInsuredID','','303','781','199','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsBInsuredName','','15','781','257','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsBName','','15','714','228','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsBOtherProvID','','682','715','151','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsBPlanID','','244','714','148','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsBPriorPmt','NoDec','544','714','101','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsBRelation','','274','781','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsBRelInfo','','394','714','18','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsCAmtDue','NoDec','654','732','110','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsCAssignBen','','425','732','18','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsCAuthCode','','15','864','308','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsCEmployer','','584','864','249','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsCGroupName','','504','798','148','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsCGroupNum','','654','798','179','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsCInsuredID','','303','798','199','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsCInsuredName','','15','798','257','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsCName','','15','732','228','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsCOtherProvID','','682','733','151','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsCPlanID','','244','732','148','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsCPriorPmt','NoDec','544','732','101','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsCRelation','','274','798','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsCRelInfo','','394','732','18','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedInsCrossoverIndicator','','776','53','50','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedPatientStatusCode','','315','130','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedUniformBillType','','785','30','49','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValAmount39a','NoDec','574','216','99','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValAmount39b','NoDec','574','232','99','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValAmount39c','NoDec','574','249','99','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValAmount39d','NoDec','574','265','99','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValAmount40a','NoDec','704','216','99','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValAmount40b','NoDec','704','232','99','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValAmount40c','NoDec','704','249','99','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValAmount40d','NoDec','704','265','99','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValAmount41a','NoDec','833','216','99','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValAmount41b','NoDec','833','232','99','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValAmount41c','NoDec','833','249','99','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValAmount41d','NoDec','833','265','99','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValCode39a','','445','216','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValCode39b','','445','232','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValCode39c','','445','249','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValCode39d','','445','265','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValCode40a','','575','216','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValCode40b','','575','232','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValCode40c','','575','249','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValCode40d','','575','265','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValCode41a','','705','216','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValCode41b','','705','232','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValCode41c','','705','249','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MedValCode41d','','705','265','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P10CodeAndMods','','311','448','148','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P10Date','MMddyyyy','461','448','68','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P10Description','','62','448','247','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P10Fee','NoDec','704','448','93','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P10RevCode','','15','448','45','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P10UnitQty','','531','448','77','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1CodeAndMods','','311','299','148','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1Date','MMddyyyy','461','299','68','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1Description','','62','299','247','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1Fee','NoDec','704','299','93','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1RevCode','','15','299','45','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1UnitQty','','531','299','77','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2CodeAndMods','','311','316','148','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2Date','MMddyyyy','461','316','68','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2Description','','62','316','247','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2Fee','NoDec','704','316','93','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2RevCode','','15','316','45','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2UnitQty','','531','316','77','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3CodeAndMods','','311','332','148','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3Date','MMddyyyy','461','332','68','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3Description','','62','332','247','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3Fee','NoDec','704','332','93','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3RevCode','','15','332','45','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3UnitQty','','531','332','77','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4CodeAndMods','','311','349','148','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4Date','MMddyyyy','461','349','68','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4Description','','62','349','247','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4Fee','NoDec','704','349','93','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4RevCode','','15','349','45','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4UnitQty','','531','349','77','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5CodeAndMods','','311','365','148','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5Date','MMddyyyy','461','365','68','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5Description','','62','365','247','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5Fee','NoDec','704','365','93','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5RevCode','','15','365','45','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5UnitQty','','531','365','77','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6CodeAndMods','','311','382','148','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6Date','MMddyyyy','461','382','68','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6Description','','62','382','247','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6Fee','NoDec','704','382','93','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6RevCode','','15','382','45','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6UnitQty','','531','382','77','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P7CodeAndMods','','311','398','148','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P7Date','MMddyyyy','461','398','68','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P7Description','','62','398','247','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P7Fee','NoDec','704','398','93','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P7RevCode','','15','398','45','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P7UnitQty','','531','398','77','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P8CodeAndMods','','311','415','148','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P8Date','MMddyyyy','461','415','68','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P8Description','','62','415','247','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P8Fee','NoDec','704','415','93','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P8RevCode','','15','415','45','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P8UnitQty','','531','415','77','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P9CodeAndMods','','311','431','148','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P9Date','MMddyyyy','461','431','68','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P9Description','','62','431','247','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P9Fee','NoDec','704','431','93','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P9RevCode','','15','431','45','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P9UnitQty','','531','431','77','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientAddress','','426','80','409','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientChartNum','','545','14','238','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientCity','','325','97','319','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientDOB','MMddyyyy','15','130','88','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientFirstMiddleLast','','26','97','287','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientGenderLetter','','105','130','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientST','','655','97','28','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientZip','','695','97','98','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Remarks','','15','1014','238','50')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TotalFee','NoDec','704','666','94','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingDentistFName','','715','982','118','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingDentistLName','','538','982','155','17')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingDentistNPI','','600','966','100','17')";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clearinghouse ADD ISA02 varchar(10) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clearinghouse ADD ISA02 varchar2(10)";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clearinghouse ADD ISA04 varchar(10) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clearinghouse ADD ISA04 varchar2(10)";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clearinghouse ADD ISA16 varchar(2) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clearinghouse ADD ISA16 varchar2(2)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clearinghouse ADD SeparatorData varchar(2) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clearinghouse ADD SeparatorData varchar2(2)";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clearinghouse ADD SeparatorSegment varchar(2) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clearinghouse ADD SeparatorSegment varchar2(2)";
					Db.NonQ(command);
				}
				//Denti-Cal clearinghouse.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command=@"INSERT INTO clearinghouse(Description,ExportPath,Payors,Eformat,ISA05,SenderTin,ISA07,ISA08,ISA15,Password,ResponsePath,CommBridge,ClientProgram,
						LastBatchNumber,ModemPort,LoginID,SenderName,SenderTelephone,GS03,ISA02,ISA04,ISA16,SeparatorData,SeparatorSegment) 
					VALUES ('Denti-Cal','"+POut.String(@"C:\Denti-Cal\")+"','','5','ZZ','','ZZ','DENTICAL','P','','','13','',0,0,'','','','DENTICAL','DENTICAL','NONE','22','1D','1C')";
					Db.NonQ(command);
				}
				else {//oracle
					command=@"INSERT INTO clearinghouse(ClearinghouseNum,Description,ExportPath,Payors,Eformat,ISA05,SenderTin,ISA07,ISA08,ISA15,Password,ResponsePath,CommBridge,ClientProgram,
						LastBatchNumber,ModemPort,LoginID,SenderName,SenderTelephone,GS03,ISA02,ISA04,ISA16,SeparatorData,SeparatorSegment) 
					VALUES ((SELECT MAX(ClearinghouseNum+1) FROM clearinghouse),'Denti-Cal','"+POut.String(@"C:\Denti-Cal\")+"','','5','ZZ','','ZZ','DENTICAL','P','','','13','',0,0,'','','','DENTICAL','DENTICAL','NONE','22','1D','1C')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS aggpath";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE aggpath'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
				}
				//IAP bridge was using a hardcoded path.  Now we will allow users to use a custom path.
				//Only update the path if the user doesn't have a custom path already entered.
				command="SELECT Path FROM program WHERE ProgName='IAP'";
				if(Db.GetScalar(command)=="") {
					command="UPDATE program SET Path='"+POut.String(@"C:\IAPlus\")+"' WHERE ProgName='IAP'";
					Db.NonQ(command);
				}
				command="UPDATE program SET Note='No buttons are available.' WHERE ProgName='IAP'";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS centralconnection";
					Db.NonQ(command);
					command=@"CREATE TABLE centralconnection (
						CentralConnectionNum bigint NOT NULL auto_increment PRIMARY KEY,
						ServerName varchar(255) NOT NULL,
						DatabaseName varchar(255) NOT NULL,
						MySqlUser varchar(255) NOT NULL,
						MySqlPassword varchar(255) NOT NULL,
						ServiceURI varchar(255) NOT NULL,
						OdUser varchar(255) NOT NULL,
						OdPassword varchar(255) NOT NULL,
						Note text NOT NULL,
						ItemOrder int NOT NULL,
						WebServiceIsEcw tinyint NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE centralconnection'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE centralconnection (
						CentralConnectionNum number(20) NOT NULL,
						ServerName varchar2(255),
						DatabaseName varchar2(255),
						MySqlUser varchar2(255),
						MySqlPassword varchar2(255),
						ServiceURI varchar2(255),
						OdUser varchar2(255),
						OdPassword varchar2(255),
						Note varchar2(255),
						ItemOrder number(11) NOT NULL,
						WebServiceIsEcw number(3) NOT NULL,
						CONSTRAINT centralconnection_CentralConne PRIMARY KEY (CentralConnectionNum)
						)";
					Db.NonQ(command);
				}
				//Add the 1500 claim form fields if the claim form does not already exist. The unique ID is OD9.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD9' LIMIT 1";
				}
				else {//oracle doesn't have LIMIT
					command="SELECT * FROM (SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD9') WHERE RowNum<=1";
				}
				DataTable tableClaimFormNum=Db.GetTable(command);
				if(tableClaimFormNum.Rows.Count==0) {
					//The 1500 claim form does not exist, so safe to add.
					claimFormNum=0;
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO claimform(Description,IsHidden,FontName,FontSize,UniqueID,PrintImages,OffsetX,OffsetY) "+
							"VALUES ('1500',0,'Arial',9,'OD9',1,0,0)";
							claimFormNum=Db.NonQ(command,true);
					}
					else {//oracle
						command="INSERT INTO claimform(ClaimFormNum,Description,IsHidden,FontName,FontSize,UniqueID,PrintImages,OffsetX,OffsetY) "+
							"VALUES ((SELECT COALESCE(MAX(ClaimFormNum),0)+1 ClaimFormNum FROM claimform),'1500',0,'Arial',9,'OD9',1,0,0)";
							claimFormNum=Db.NonQ(command,true,"ClaimFormNum","claimform");
					}
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'1500.gif','','','-54','13','905','1165')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','AccidentST','','467','396','30','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentist','','531','994','250','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentist','','256','995','235','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistAddress','','531','1010','250','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistAddress','','256','1009','235','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistCity','','531','1026','139','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistCity','','256','1023','132','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistNPI','','260','1045','92','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistNPI','','531','1045','92','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistNumIsSSN','','191','961','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistNumIsTIN','','210','961','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistPh123','','680','978','40','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistPh456','','719','978','40','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistPh78910','','759','978','48','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistSSNorTIN','','39','959','131','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistST','','671','1026','30','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistST','','388','1023','28','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistZip','','701','1026','80','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistZip','','416','1023','75','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Diagnosis1','','52','662','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Diagnosis2','','52','695','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Diagnosis3','','324','662','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Diagnosis4','','325','694','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','EmployerName','','528','394','250','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','FixedText','1','615','763','20','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','FixedText','1','615','796','20','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','FixedText','1','615','828','20','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','FixedText','1','615','862','20','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','FixedText','1','615','895','20','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','FixedText','1','615','929','20','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','GroupNum','','530','327','250','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsAutoAccident','','370','396','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsFTStudent','','430','295','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsGroupHealthPlan','','329','161','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsMedicaidClaim','','97','162','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsNotAutoAccident','','430','395','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsNotOccupational','','430','362','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsNotOtherAccident','','431','428','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsOccupational','','370','362','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsOtherAccident','','370','428','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsPTStudent','','491','295','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsCarrierName','','36','460','245','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsExists','','540','462','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsGroupNum','','36','358','245','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsNotExists','','591','462','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsSubscrDOB','MM     dd     yyyy','42','397','100','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsSubscrIsFemale','','261','396','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsSubscrIsMale','','200','396','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsSubscrLastFirst','','36','325','245','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1Code','','273','762','55','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1CodeMod1','','340','762','30','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1CodeMod2','','375','762','30','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1CodeMod3','','405','762','29','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1CodeMod4','','434','762','29','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1Date','MM    dd    yy','32','762','77','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1Date','MM    dd     yy','122','762','78','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1Diagnosis','','470','762','35','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1Fee','','598','762','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1PlaceNumericCode','','206','762','28','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1TreatProvNPI','','698','762','100','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2Code','','273','796','55','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2CodeMod1','','340','796','30','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2CodeMod2','','375','796','30','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2CodeMod3','','405','796','29','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2CodeMod4','','434','796','29','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2Date','MM    dd    yy','32','796','77','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2Date','MM    dd     yy','122','796','78','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2Diagnosis','','470','796','35','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2Fee','','598','796','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2PlaceNumericCode','','205','796','28','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2TreatProvNPI','','698','796','100','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3Code','','273','828','55','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3CodeMod1','','340','827','30','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3CodeMod2','','375','827','30','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3CodeMod3','','405','827','29','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3CodeMod4','','434','827','29','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3Date','MM    dd    yy','32','829','77','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3Date','MM    dd     yy','121','829','78','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3Diagnosis','','470','828','35','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3Fee','','598','828','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3PlaceNumericCode','','206','829','28','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3TreatProvNPI','','698','828','100','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4Code','','273','862','55','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4CodeMod1','','340','862','30','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4CodeMod2','','375','862','30','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4CodeMod3','','405','862','29','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4CodeMod4','','434','862','29','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4Date','MM    dd    yy','32','863','77','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4Date','MM    dd     yy','122','863','78','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4Diagnosis','','470','863','35','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4Fee','','598','862','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4PlaceNumericCode','','205','863','28','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4TreatProvNPI','','699','862','100','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5Code','','273','895','55','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5CodeMod1','','340','895','30','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5CodeMod2','','375','895','30','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5CodeMod3','','405','895','29','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5CodeMod4','','434','895','29','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5Date','MM    dd    yy','32','895','77','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5Date','MM    dd     yy','122','895','78','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5Diagnosis','','470','895','35','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5Fee','','598','895','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5PlaceNumericCode','','205','895','28','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5TreatProvNPI','','699','894','100','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6Code','','273','929','55','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6CodeMod1','','340','929','30','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6CodeMod2','','375','929','30','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6CodeMod3','','405','929','29','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6CodeMod4','','434','929','29','16')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6Date','MM    dd    yy','32','929','77','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6Date','MM    dd     yy','122','929','78','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6Diagnosis','','470','929','35','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6Fee','','598','929','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6PlaceNumericCode','','205','929','28','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6TreatProvNPI','','699','928','100','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientAddress','','37','226','245','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientAssignment','','577','525','210','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientCity','','37','258','200','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientDOB','MM    dd    yyyy','333','195','95','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientIsFemale','','490','194','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientIsMale','','441','195','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientIsMarried','','430','262','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientIsSingle','','370','262','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientLastFirst','','37','194','245','13')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientPhone','','169','296','120','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientRelease','','78','526','240','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientReleaseDate','MM/dd/yyyy','384','525','113','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientST','','281','259','30','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientZip','','37','293','100','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PriInsAddress','','419','96','250','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PriInsAddress2','','419','110','250','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PriInsCarrierName','','419','82','250','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PriInsCity','','419','124','140','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PriInsST','','560','124','30','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PriInsZip','','590','124','79','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','ReferringProvNameFL','','32','597','250','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','ReferringProvNPI','','343','597','150','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','RelatIsChild','','440','229','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','RelatIsOther','','490','228','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','RelatIsSelf','','349','229','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','RelatIsSpouse','','400','229','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','ShowPreauthorizationIfPreauth','','143','69','200','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrAddress','','530','225','250','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrCity','','530','260','200','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrDOB','MM    dd     yyyy','554','363','100','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrID','','529','161','200','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrIsFemale','','771','362','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrIsMale','','701','362','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrLastFirst','','530','192','250','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrPhone','','672','293','120','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrST','','760','260','50','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrZip','','531','294','100','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TotalFee','','620','961','75','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingDentistSigDate','','169','1035','74','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingDentistSignature','','27','1020','142','30')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('CentralManagerPassHash','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'CentralManagerPassHash','')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '12.0.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To12_0_2();
		}

		///<summary>This is a helper method for the 12.0.1 and 12.4.12 conversions.  Without it, there would be an additional 1200 lines of code.</summary>
		private static string GetClaimFormItemNum(){
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return "(SELECT MAX(ClaimFormItemNum)+1 FROM claimformitem)";
			}
			else {
				//for mysql, this seems to be allowed and will automatically increment.
				//Should work fine for both autoincrement and regular.
				return "0";
			}
		}

		///<summary>Oracle compatible: 01/04/2012</summary>
		private static void To12_0_2() {
			if(FromVersion<new Version("12.0.2.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 12.0.2");//No translation in convert script.
				string command;
				//Insert MiPACS Imaging Bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'MiPACS', "
						+"'MiPACS Imaging', "
						+"'0', "
						+"'"+POut.String(@"C:\Program Files\MiDentView\Cmdlink.exe")+"',"
						+"'', "
						+"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"'"+POut.Long(programNum)+"', "
						+"'"+POut.Int(((int)ToolBarsAvail.ChartModule))+"', "
						+"'MiPACS')";
					Db.NonQ32(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'MiPACS', "
						+"'MiPACS Imaging', "
						+"'0', "
						+"'"+POut.String(@"C:\Program Files\MiDentView\Cmdlink.exe")+"',"
						+"'', "
						+"'')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
						+"'"+POut.Long(programNum)+"', "
						+"'"+POut.Int(((int)ToolBarsAvail.ChartModule))+"', "
						+"'MiPACS')";
					Db.NonQ32(command);
				}//end MiPACS Imaging bridge
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE paysplit ADD INDEX (PayPlanNum)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX paysplit_PayPlanNum ON paysplit (PayPlanNum)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
				command="UPDATE preference SET ValueString = '12.0.2.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To12_0_5();
		}

		///<summary>Oracle compatible: 02/02/2012</summary>
		private static void To12_0_5() {
			if(FromVersion<new Version("12.0.5.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 12.0.5");//No translation in convert script.
				string command;
				//Delete duplicate MiPACS Imaging Bridge
				command="SELECT * FROM program WHERE ProgName='MiPACS'";
				DataTable table=Db.GetTable(command);
				if(table.Rows.Count>1) {
					long programNum=PIn.Long(table.Rows[1]["ProgramNum"].ToString());
					command="DELETE FROM program WHERE ProgramNum="+POut.Long(programNum);
					Db.NonQ(command);
					command="DELETE FROM toolbutitem WHERE ProgramNum="+POut.Long(programNum);
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+table.Rows[0]["ProgramNum"].ToString()+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0')";
					Db.NonQ32(command);
				}
				else {//oracle
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
					+"'"+table.Rows[0]["ProgramNum"].ToString()+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0')";
					Db.NonQ32(command);
				}
				command="SELECT * FROM program WHERE ProgName='Apixia'";
				table=Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+table.Rows[0]["ProgramNum"].ToString()+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '12.0.5.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
				}
				else {
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
					+"'"+table.Rows[0]["ProgramNum"].ToString()+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0')";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '12.0.5.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
				}
			}
			To12_0_6();
		}

		///<summary>Oracle compatible: 01/08/2013</summary>
		private static void To12_0_6() {
			if(FromVersion<new Version("12.0.6.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 12.0.6");//No translation in convert script.
				string command;
				//EmdeonMedical clearinghouse.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command=@"INSERT INTO clearinghouse(Description,ExportPath,Payors,Eformat,ISA05,SenderTin,ISA07,ISA08,ISA15,Password,ResponsePath,CommBridge,ClientProgram,
							LastBatchNumber,ModemPort,LoginID,SenderName,SenderTelephone,GS03,ISA02,ISA04,ISA16,SeparatorData,SeparatorSegment) 
						VALUES ('Emdeon Medical','"+POut.String(@"C:\EmdeonMedical\Claims\")+"','','6','ZZ','','ZZ','133052274','P','',"
							+"'"+POut.String(@"C:\EmdeonMedical\Reports\")+"','14','',0,0,'','','','133052274','','','','','')";
					Db.NonQ(command);
				}
				else {//oracle
					command=@"INSERT INTO clearinghouse(ClearinghouseNum,Description,ExportPath,Payors,Eformat,ISA05,SenderTin,ISA07,ISA08,ISA15,Password,ResponsePath,CommBridge,ClientProgram,
							LastBatchNumber,ModemPort,LoginID,SenderName,SenderTelephone,GS03,ISA02,ISA04,ISA16,SeparatorData,SeparatorSegment) 
						VALUES ((SELECT MAX(ClearinghouseNum+1) FROM clearinghouse),'Emdeon Medical','"+POut.String(@"C:\EmdeonMedical\Claims\")+"','','6','ZZ','','ZZ','133052274','P','',"
							+"'"+POut.String(@"C:\EmdeonMedical\Reports\")+"','14','',0,0,'','','','133052274','','','','','')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '12.0.6.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To12_1_0();
		}

		///<summary>Oracle compatible: 01/08/2013</summary>
		private static void To12_1_0() {
			if(FromVersion<new Version("12.1.0.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 12.1.0");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('MobileSynchNewTables121Done','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'MobileSynchNewTables121Done','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('AccountShowPaymentNums','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'AccountShowPaymentNums','0')";
					Db.NonQ(command);
				}
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE insplan ADD INDEX (TrojanID)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX insplan_TrojanID ON insplan (TrojanID)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE statement ADD IsReceipt tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE statement ADD IsReceipt number(3)";
					Db.NonQ(command);
					command="UPDATE statement SET IsReceipt = 0 WHERE IsReceipt IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE statement MODIFY IsReceipt NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE userod ADD ClinicIsRestricted tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE userod ADD ClinicIsRestricted number(3)";
					Db.NonQ(command);
					command="UPDATE userod SET ClinicIsRestricted = 0 WHERE ClinicIsRestricted IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE userod MODIFY ClinicIsRestricted NOT NULL";
					Db.NonQ(command);
				}
				command="UPDATE userod SET ClinicIsRestricted = 1 WHERE ClinicNum != 0";//to preserve old ClinicNum behavior.
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('TimeCardOvertimeFirstDayOfWeek','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'TimeCardOvertimeFirstDayOfWeek','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('AccountingSoftware','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'AccountingSoftware','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('QuickBooksCompanyFile','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'QuickBooksCompanyFile','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('QuickBooksDepositAccounts','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'QuickBooksDepositAccounts','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('QuickBooksIncomeAccount','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'QuickBooksIncomeAccount','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ReplicationFailureAtServer_id','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ReplicationFailureAtServer_id','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE replicationserver ADD SlaveMonitor varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE replicationserver ADD SlaveMonitor varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE commlog ADD DateTimeEnd datetime DEFAULT '0001-01-01' NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE commlog ADD DateTimeEnd date";
					Db.NonQ(command);
					command="UPDATE commlog SET DateTimeEnd = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateTimeEnd IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE commlog MODIFY DateTimeEnd NOT NULL";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '12.1.0.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To12_1_7();
		}

		///<summary>Oracle compatible: 01/08/2013</summary>
		private static void To12_1_7() {
			if(FromVersion<new Version("12.1.7.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 12.1.7");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD CustomTracking bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim ADD INDEX (CustomTracking)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD CustomTracking number(20)";
					Db.NonQ(command);
					command="UPDATE claim SET CustomTracking = 0 WHERE CustomTracking IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim MODIFY CustomTracking NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX claim_CustomTracking ON claim (CustomTracking)";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '12.1.7.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To12_2_1();
		}

		///<summary>Oracle compatible: 01/08/2013</summary>
		private static void To12_2_1() {
			if(FromVersion<new Version("12.2.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 12.2.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS custrefentry";
					Db.NonQ(command);
					command=@"CREATE TABLE custrefentry (
						CustRefEntryNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNumCust bigint NOT NULL,
						PatNumRef bigint NOT NULL,
						DateEntry date NOT NULL DEFAULT '0001-01-01',
						Note varchar(255) NOT NULL,
						INDEX(PatNumCust),
						INDEX(PatNumRef)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE custrefentry'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE custrefentry (
						CustRefEntryNum number(20) NOT NULL,
						PatNumCust number(20) NOT NULL,
						PatNumRef number(20) NOT NULL,
						DateEntry date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						Note varchar2(255),
						CONSTRAINT custrefentry_CustRefEntryNum PRIMARY KEY (CustRefEntryNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX custrefentry_PatNumCust ON custrefentry (PatNumCust)";
					Db.NonQ(command);
					command=@"CREATE INDEX custrefentry_PatNumRef ON custrefentry (PatNumRef)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS custreference";
					Db.NonQ(command);
					command=@"CREATE TABLE custreference (
						CustReferenceNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						DateMostRecent date NOT NULL DEFAULT '0001-01-01',
						Note varchar(255) NOT NULL,
						IsBadRef tinyint NOT NULL,
						INDEX(PatNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE custreference'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE custreference (
						CustReferenceNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						DateMostRecent date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						Note varchar2(255),
						IsBadRef number(3) NOT NULL,
						CONSTRAINT custreference_CustReferenceNum PRIMARY KEY (CustReferenceNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX custreference_PatNum ON custreference (PatNum)";
					Db.NonQ(command);
				}
				//Create a customer reference object for every customer in the db.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO custreference (PatNum) SELECT PatNum FROM patient";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE appointment ADD ColorOverride int NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE appointment ADD ColorOverride number(11)";
					Db.NonQ(command);
					command="UPDATE appointment SET ColorOverride = 0 WHERE ColorOverride IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE appointment MODIFY ColorOverride NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD DateResent date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD DateResent date";
					Db.NonQ(command);
					command="UPDATE claim SET DateResent = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateResent IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim MODIFY DateResent NOT NULL";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD CorrectionType tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD CorrectionType number(3)";
					Db.NonQ(command);
					command="UPDATE claim SET CorrectionType = 0 WHERE CorrectionType IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim MODIFY CorrectionType NOT NULL";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD ClaimIdentifier varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD ClaimIdentifier varchar2(255)";
					Db.NonQ(command);
				}
				command="UPDATE claim SET ClaimIdentifier="+DbHelper.Concat(new string[] {"PatNum","'/'","ClaimNum"});
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD OrigRefNum varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD OrigRefNum varchar2(255)";
					Db.NonQ(command);
				}
				//Add RefAttachDelete permission to everyone------------------------------------------------------
				command="SELECT DISTINCT UserGroupNum FROM grouppermission";
				DataTable table=Db.GetTable(command);
				long groupNum;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+","+POut.Int((int)Permissions.RefAttachDelete)+")";
						Db.NonQ32(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+","+POut.Int((int)Permissions.RefAttachDelete)+")";
						Db.NonQ32(command);
					}
				}
				//Add RefAttachAdd permission to everyone------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+","+POut.Int((int)Permissions.RefAttachAdd)+")";
						Db.NonQ32(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+","+POut.Int((int)Permissions.RefAttachAdd)+")";
						Db.NonQ32(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE apptfielddef ADD FieldType tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE apptfielddef ADD FieldType number(3)";
					Db.NonQ(command);
					command="UPDATE apptfielddef SET FieldType = 0 WHERE FieldType IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE apptfielddef MODIFY FieldType NOT NULL";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE apptfielddef ADD PickList varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE apptfielddef ADD PickList varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurecode ADD ProvNumDefault bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurecode ADD INDEX (ProvNumDefault)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurecode ADD ProvNumDefault number(20)";
					Db.NonQ(command);
					command="UPDATE procedurecode SET ProvNumDefault = 0 WHERE ProvNumDefault IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurecode MODIFY ProvNumDefault NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX procedurecode_ProvNumDefault ON procedurecode (ProvNumDefault)";
					Db.NonQ(command);
				}
				//Getting rid of AutoItem from CommLogItem enum.  Set all commlogs using AutoItem to None.
				command="UPDATE commlog SET Mode_=0 WHERE Mode_=5";
				Db.NonQ(command);
				//Getting rid of IsStatementSent from commlog table.
				command="ALTER TABLE commlog DROP COLUMN IsStatementSent";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE computerpref ADD ScanDocSelectSource tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE computerpref ADD ScanDocSelectSource number(3)";
					Db.NonQ(command);
					command="UPDATE computerpref SET ScanDocSelectSource = 0 WHERE ScanDocSelectSource IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE computerpref MODIFY ScanDocSelectSource NOT NULL";
					Db.NonQ(command);
				}			
				command="UPDATE computerpref SET ScanDocSelectSource = 1";//Default to show select scanner popup.
				Db.NonQ(command);	
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE computerpref ADD ScanDocShowOptions tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE computerpref ADD ScanDocShowOptions number(3)";
					Db.NonQ(command);
					command="UPDATE computerpref SET ScanDocShowOptions = 0 WHERE ScanDocShowOptions IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE computerpref MODIFY ScanDocShowOptions NOT NULL";
					Db.NonQ(command);
				}
				command="UPDATE computerpref SET ScanDocShowOptions = 1";//Default to show scanner options when scanning.
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE computerpref ADD ScanDocDuplex tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE computerpref ADD ScanDocDuplex number(3)";
					Db.NonQ(command);
					command="UPDATE computerpref SET ScanDocDuplex = 0 WHERE ScanDocDuplex IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE computerpref MODIFY ScanDocDuplex NOT NULL";
					Db.NonQ(command);
				}			
				command="UPDATE computerpref SET ScanDocDuplex = 1";//Default to always attempt to scan duplex
				Db.NonQ(command);	
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE computerpref ADD ScanDocGrayscale tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE computerpref ADD ScanDocGrayscale number(3)";
					Db.NonQ(command);
					command="UPDATE computerpref SET ScanDocGrayscale = 0 WHERE ScanDocGrayscale IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE computerpref MODIFY ScanDocGrayscale NOT NULL";
					Db.NonQ(command);
				}
				command="UPDATE computerpref SET ScanDocGrayscale = 1";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE computerpref ADD ScanDocResolution int NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE computerpref ADD ScanDocResolution number(11)";
					Db.NonQ(command);
					command="UPDATE computerpref SET ScanDocResolution = 0 WHERE ScanDocResolution IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE computerpref MODIFY ScanDocResolution NOT NULL";
					Db.NonQ(command);
				} 
				command="SELECT ValueString FROM preference WHERE PrefName='ScannerResolution'";
				int scannerRes=PIn.Int(Db.GetScalar(command));
				command="UPDATE computerpref SET ScanDocResolution = "+POut.Int(scannerRes);
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE computerpref ADD ScanDocQuality tinyint unsigned NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE computerpref ADD ScanDocQuality number(3)";
					Db.NonQ(command);
					command="UPDATE computerpref SET ScanDocQuality = 0 WHERE ScanDocQuality IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE computerpref MODIFY ScanDocQuality NOT NULL";
					Db.NonQ(command);
				}
				command="SELECT ValueString FROM preference WHERE PrefName='ScannerCompression'";
				int scannerComp=PIn.Int(Db.GetScalar(command));
				command="UPDATE computerpref SET ScanDocQuality = "+POut.Int(scannerComp);
				Db.NonQ(command);
				command="DELETE FROM preference WHERE PrefName = 'ScannerCompression'";
				Db.NonQ(command);
				command="DELETE FROM preference WHERE PrefName = 'ScannerResolution'";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE timecardrule ADD BeforeTimeOfDay time NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE timecardrule ADD BeforeTimeOfDay date";
					Db.NonQ(command);
					command="UPDATE timecardrule SET BeforeTimeOfDay = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE BeforeTimeOfDay IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE timecardrule MODIFY BeforeTimeOfDay NOT NULL";
					Db.NonQ(command);
				}
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					command="SELECT CanadianNetworkNum FROM canadiannetwork WHERE Abbrev='CSI' LIMIT 1";
					long canadianNetworkNumCSI=PIn.Long(Db.GetScalar(command));
					command="SELECT COUNT(*) FROM carrier WHERE ElectID='000116'"; //boilermakers' national benefit plan
					if(Db.GetCount(command)=="0") {
						command="INSERT INTO carrier (CarrierName,Address,Address2,City,State,Zip,Phone,ElectID,NoSendElect,IsCDA,CDAnetVersion,CanadianNetworkNum,IsHidden,CanadianEncryptionMethod,CanadianSupportedTypes) VALUES "+
								"('Boilermakers\\' National Benefit Plan','45 McIntosh Drive','','Markham','ON','L3R 8C7','1-800-668-7547','000116','0','1','04',"+POut.Long(canadianNetworkNumCSI)+",'0',1,384)";
						Db.NonQ(command);
					}
					command="SELECT COUNT(*) FROM carrier WHERE ElectID='000115'"; //u.a. local 46 dental plan
					if(Db.GetCount(command)=="0") {
						command="INSERT INTO carrier (CarrierName,Address,Address2,City,State,Zip,Phone,ElectID,NoSendElect,IsCDA,CDAnetVersion,CanadianNetworkNum,IsHidden,CanadianEncryptionMethod,CanadianSupportedTypes) VALUES "+
								"('U.A. Local 46 Dental Plan','936 Warden Avenue','','Scarborough','ON','M1L 4C9','1-800-263-3564','000115','0','1','04',"+POut.Long(canadianNetworkNumCSI)+",'0',1,384)";
						Db.NonQ(command);
					}
					command="SELECT COUNT(*) FROM carrier WHERE ElectID='000110'"; //u.a. local 787 Health Trust Fund Dental Plan
					if(Db.GetCount(command)=="0") {
						command="INSERT INTO carrier (CarrierName,Address,Address2,City,State,Zip,Phone,ElectID,NoSendElect,IsCDA,CDAnetVersion,CanadianNetworkNum,IsHidden,CanadianEncryptionMethod,CanadianSupportedTypes) VALUES "+
								"('U.A. Local 787 Health Trust Fund Dental Plan','419 Deerhurst Drive','','Brampton','ON','L6T 5K3','1-204-985-3940','000110','0','1','04',"+POut.Long(canadianNetworkNumCSI)+",'0',1,384)";
						Db.NonQ(command);
					}
				}
				//Insert RayMage Imaging Bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'RayMage', "
						+"'RayMage from www.cefla.com', "
						+"'0', "
						+"'"+POut.String(@"C:\Program Files\MyRay\rayMage\rayMage.exe")+"',"
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
						+"'RayMage')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'RayMage', "
						+"'RayMage from www.cefla.com', "
						+"'0', "
						+"'"+POut.String(@"C:\Program Files\MyRay\rayMage\rayMage.exe")+"',"
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
						+"'RayMage')";
					Db.NonQ(command);
				}//end RayMage Imaging bridge
				//Insert BioPAK Imaging Bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'BioPAK', "
						+"'BioPAK from www.bioresearchinc.com', "
						+"'0', "
						+"'"+POut.String(@"BioPAK.exe")+"',"
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
						+"'BioPAK')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'BioPAK', "
						+"'BioPAK from www.bioresearchinc.com', "
						+"'0', "
						+"'"+POut.String(@"BioPAK.exe")+"',"
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
						+"'BioPAK')";
					Db.NonQ(command);
				}//end BioPAK Imaging bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE securitylog ADD FKey bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE securitylog ADD INDEX (FKey)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE securitylog ADD FKey number(20)";
					Db.NonQ(command);
					command="UPDATE securitylog SET FKey = 0 WHERE FKey IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE securitylog MODIFY FKey NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX securitylog_FKey ON securitylog (FKey)";
					Db.NonQ(command);
				}
				//This will be specific to eCW because we don't use IsStandalone anywhere else.
				command=@"UPDATE programproperty SET PropertyDesc='eClinicalWorksMode' WHERE PropertyDesc='IsStandalone'";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '12.2.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To12_2_17();
		}

		///<summary>Oracle compatible: 01/08/2013</summary>
		private static void To12_2_17() {
			if(FromVersion<new Version("12.2.17.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 12.2.17");//No translation in convert script.
				string command="";
				//Add the 1500 claim form field for prior authorization if it does not already exist. The unique ID is OD9.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD9' LIMIT 1";
				}
				else {//oracle doesn't have LIMIT
					command="SELECT * FROM (SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD9') WHERE RowNum<=1";
				}
				long claimFormNum=PIn.Long(Db.GetScalar(command));
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES (0,"+POut.Long(claimFormNum)+",'','PriorAuthString','','528','695','282','14')";
				}
				else {
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ((SELECT MAX(ClaimFormItemNum)+1 FROM claimformitem),"+POut.Long(claimFormNum)+",'','PriorAuthString','','528','695','282','14')";
				}
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '12.2.17.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To12_2_28();
		}

		///<summary>Oracle compatible: 01/08/2013</summary>
		private static void To12_2_28() {
			if(FromVersion<new Version("12.2.28.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 12.2.28");//No translation in convert script.
				string command="";
				//Fix medical claim form 1500, P*Date fields printing issue. Fields were too narrow. The unique ID is OD9.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD9' LIMIT 1";
				}
				else {//oracle doesn't have LIMIT
					command="SELECT * FROM (SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD9') WHERE RowNum<=1";
				}
				long claimFormNum=PIn.Long(Db.GetScalar(command));
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="UPDATE claimformitem SET width = 80 WHERE FieldName LIKE 'P%Date' AND ClaimFormNum = "+claimFormNum;
					Db.NonQ(command);
					command="UPDATE claimformitem SET XPos = 206 WHERE FieldName LIKE 'P%PlaceNumericCode' AND ClaimFormNum = "+claimFormNum;
					Db.NonQ(command);
				}
				else {
					command="UPDATE claimformitem SET width = 80 WHERE FieldName LIKE 'P%Date' AND ClaimFormNum = "+claimFormNum;
					Db.NonQ(command);
					command="UPDATE claimformitem SET XPos = 206 WHERE FieldName LIKE 'P%PlaceNumericCode' AND ClaimFormNum = "+claimFormNum;
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '12.2.28.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To12_3_1();
		}

		//In version 12.2.34, there is an eCW section here.

		///<summary>Oracle compatible: 01/09/2013</summary>
		private static void To12_3_1() {
			if(FromVersion<new Version("12.3.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 12.3.1");//No translation in convert script.
				string command;
				//Removing Appointment Complete-Time bar, Patient Note Text and Completed Pt. Note Text.
				command="DELETE FROM definition WHERE Category=17 AND ItemName='Appointment Complete-Time bar'";//Cat 17=AppointmentColors
				Db.NonQ(command);
				command="DELETE FROM definition WHERE Category=17 AND ItemName='Patient Note Text'";
				Db.NonQ(command);
				command="DELETE FROM definition WHERE Category=17 AND ItemName='Completed Pt. Note Text'";
				Db.NonQ(command);
				command="DELETE FROM definition WHERE Category=17 AND ItemName='Patient Note - Pt Name'";
				Db.NonQ(command);
				//Fix the ItemOrder of the AppointmentColors category
				command="UPDATE definition SET ItemOrder=2 WHERE Category=17 AND ItemName='Appointment Complete-Background'";
				Db.NonQ(command);
				command="UPDATE definition SET ItemOrder=3 WHERE Category=17 AND ItemName='Holiday'";
				Db.NonQ(command);
				command="UPDATE definition SET ItemOrder=4 WHERE Category=17 AND ItemName='Blockout Text'";
				Db.NonQ(command);
				command="UPDATE definition SET ItemOrder=5 WHERE Category=17 AND ItemName='Patient Note Background'";
				Db.NonQ(command);
				command="UPDATE definition SET ItemOrder=6 WHERE Category=17 AND ItemName='Completed Pt. Note Background'";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('AppointmentTimeIsLocked','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'AppointmentTimeIsLocked','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('RecallAgeAdult','12')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'RecallAgeAdult','12')";
					Db.NonQ(command);
				}
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE hl7msg ADD INDEX (HL7Status)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX hl7msg_HL7Status ON hl7msg (HL7Status)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//ex is needed, or exception won't get caught.
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE hl7msg ADD DateTStamp timestamp";
					Db.NonQ(command);
					command="UPDATE hl7msg SET DateTStamp = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE hl7msg ADD DateTStamp timestamp";
					Db.NonQ(command);
					command="UPDATE hl7msg SET DateTStamp = SYSTIMESTAMP";
					Db.NonQ(command);
				}
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE hl7msg ADD INDEX (DateTStamp)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX hl7msg_DateTStamp ON hl7msg (DateTStamp)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//ex is needed, or exception won't get caught.
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('TextMsgOkStatusTreatAsNo','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'TextMsgOkStatusTreatAsNo','0')";
					Db.NonQ(command);
				}
				command="DELETE FROM preference WHERE PrefName='MedicalEclaimsEnabled'";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patient ADD TxtMsgOk tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patient ADD TxtMsgOk number(3)";
					Db.NonQ(command);
					command="UPDATE patient SET TxtMsgOk = 0 WHERE TxtMsgOk IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE patient MODIFY TxtMsgOk NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE programproperty ADD ComputerName varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE programproperty ADD ComputerName varchar2(255)";
					Db.NonQ(command);
				}
				//Insert CallFire Bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'CallFire', "
						+"'CallFire from www.callfire.com', "
						+"'0', "
						+"'',"
						+"'', "
						+"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'Key From CallFire', "
						+"'')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'CallFire', "
						+"'CallFire from www.callfire.com', "
						+"'0', "
						+"'',"
						+"'', "
						+"'')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'Key From CallFire', "
						+"'')";
					Db.NonQ(command);
				}//end CallFire bridge
				//ImageDelete was removed from global lock date.  Set the permission dates to the current lock dates.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command=@"UPDATE grouppermission gp,preference pdate,preference pdays 
						SET gp.NewerDate=pdate.ValueString,gp.NewerDays=pdays.ValueString
						WHERE pdate.PrefName='SecurityLockDate'
						AND pdays.PrefName='SecurityLockDays'
						AND gp.PermType=44";//ImageDelete
					Db.NonQ(command);
				}
				else {//oracle
					command=@"UPDATE grouppermission
						SET NewerDate=TO_DATE((SELECT ValueString FROM preference WHERE prefname='SecurityLockDate'),'yyyy-mm-dd')
						,NewerDays=(SELECT ValueString FROM preference WHERE PrefName='SecurityLockDays')
						WHERE PermType=44";//ImageDelete
					Db.NonQ(command);
				}
				//Add CarrierAdd permission to everyone------------------------------------------------------
				command="SELECT DISTINCT UserGroupNum FROM grouppermission";
				DataTable table=Db.GetTable(command);
				long groupNum;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+",58)";//CarrierCreate
						Db.NonQ32(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",58)";//CarrierCreate
						Db.NonQ32(command);
					}
				}
				//Text Message Confirmations--------------------------------------------------------------------------------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ConfirmTextMessage','[NameF], we would like to confirm your dental appointment on [date] at [time].')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ConfirmTextMessage','[NameF], we would like to confirm your dental appointment on [date] at [time].')";
					Db.NonQ(command);
				}
				command="SELECT MAX(ItemOrder) FROM definition WHERE Category="+POut.Int((int)DefCat.ApptConfirmed);
				int itemOrder=PIn.Int(Db.GetScalar(command))+1;//eg 7+1
				long defNumTextMessaged;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO definition(Category,ItemOrder,ItemName,ItemValue) "
						+"VALUES("+POut.Int((int)DefCat.ApptConfirmed)+","+POut.Int(itemOrder)+",'Texted','Texted')";
					defNumTextMessaged=Db.NonQ(command,true);
				}
				else {//oracle
					command="INSERT INTO definition(DefNum,Category,ItemOrder,ItemName,ItemValue) "
						+"VALUES((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),"+POut.Int((int)DefCat.ApptConfirmed)+","+POut.Int(itemOrder)+",'Texted','Texted')";
					defNumTextMessaged=Db.NonQ(command,true,"DefNum","definition");
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ConfirmStatusTextMessaged','"+POut.Long(defNumTextMessaged)+"')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ConfirmStatusTextMessaged','"+POut.Long(defNumTextMessaged)+"')";
					Db.NonQ(command);
				}
				//End Text Message Confirmations----------------------------------------------------------------------------------------------------------------------------------
				//add ReportDashbaord permissions to all groups------------------------------------------------------
				command="SELECT UserGroupNum FROM usergroup";
				table=Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+","+POut.Long((int)Permissions.GraphicalReports)+")";
						Db.NonQ(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+","+POut.Long((int)Permissions.GraphicalReports)+")";
						Db.NonQ(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ICD9DefaultForNewProcs','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ICD9DefaultForNewProcs','')";
					Db.NonQ(command);
				}
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE patient ADD INDEX (Email)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX patient_Email ON patient (Email)";
						Db.NonQ(command);
					}
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE appointment ADD INDEX (AptStatus)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX appointment_AptStatus ON appointment (AptStatus)";
						Db.NonQ(command);
					}
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE commlog ADD INDEX (CommDateTime)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX commlog_CommDateTime ON commlog (CommDateTime)";
						Db.NonQ(command);
					}
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE commlog ADD INDEX (CommType)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX commlog_CommType ON commlog (CommType)";
						Db.NonQ(command);
					}
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE recall ADD INDEX (DatePrevious)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX recall_DatePrevious ON recall (DatePrevious)";
						Db.NonQ(command);
					}
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE recall ADD INDEX (IsDisabled)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX recall_IsDisabled ON recall (IsDisabled)";
						Db.NonQ(command);
					}
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE recall ADD INDEX (RecallTypeNum)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX recall_RecallTypeNum ON recall (RecallTypeNum)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//ex is needed, or exception won't get caught.
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS installmentplan";
					Db.NonQ(command);
					command=@"CREATE TABLE installmentplan (
						InstallmentPlanNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						DateAgreement date NOT NULL DEFAULT '0001-01-01',
						DateFirstPayment date NOT NULL DEFAULT '0001-01-01',
						MonthlyPayment double NOT NULL,
						APR float NOT NULL,
						Note varchar(255) NOT NULL,
						INDEX(PatNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE installmentplan'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE installmentplan (
						InstallmentPlanNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						DateAgreement date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						DateFirstPayment date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						MonthlyPayment number(38,8) NOT NULL,
						APR number(38,8) NOT NULL,
						Note varchar2(255) NOT NULL,
						CONSTRAINT installmentplan_InstallmentPla PRIMARY KEY (InstallmentPlanNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX installmentplan_PatNum ON installmentplan (PatNum)";
					Db.NonQ(command);
				}
				//Insert ClioSoft Bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
				  command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
				    +"'ClioSoft', "
				    +"'ClioSoft from www.sotaimaging.com', "
				    +"'0', "
				    +"'"+POut.String(@"C:\Program Files (x86)\ClioSoft\ClioSoft.exe")+"',"
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
				    +"'ClioSoft')";
				  Db.NonQ(command);
				}
				else {//oracle
				  command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'ClioSoft', "
				    +"'ClioSoft from www.sotaimaging.com', "
				    +"'0', "
				    +"'"+POut.String(@"C:\Program Files (x86)\ClioSoft\ClioSoft.exe")+"',"
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
				    +"'ClioSoft')";
				  Db.NonQ(command);
				}//end ClioSoft bridge				
				//Add AutoNoteQuickNoteEdit permission to everyone------------------------------------------------------
				command="SELECT DISTINCT UserGroupNum FROM grouppermission";
				table=Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+","+POut.Int((int)Permissions.AutoNoteQuickNoteEdit)+")";
						Db.NonQ32(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+","+POut.Int((int)Permissions.AutoNoteQuickNoteEdit)+")";
						Db.NonQ32(command);
					}
				}
				//Insert Tscan Bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
				    +"'Tscan', "
				    +"'Tscan from www.tekscan.com', "
				    +"'0', "
				    +"'"+POut.String(@"tscan.exe")+"',"
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
				    +"'Tscan')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'Tscan', "
				    +"'Tscan from www.tekscan.com', "
				    +"'0', "
				    +"'"+POut.String(@"tscan.exe")+"',"
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
				    +"'Tscan')";
					Db.NonQ(command);
				}//end Tscan bridge
				command="UPDATE preference SET ValueString = '12.3.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To12_3_3();
		}

		///<summary>Oracle compatible: 01/09/2013</summary>
		private static void To12_3_3() {
			if(FromVersion<new Version("12.3.3.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 12.3.3");//No translation in convert script.
				string command="";
				command="UPDATE claimformitem SET FieldName='PayToDentistAddress' WHERE FieldName='BillingDentistAddress'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET FieldName='PayToDentistAddress2' WHERE FieldName='BillingDentistAddress2'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET FieldName='PayToDentistCity' WHERE FieldName='BillingDentistCity'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET FieldName='PayToDentistST' WHERE FieldName='BillingDentistST'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET FieldName='PayToDentistZip' WHERE FieldName='BillingDentistZip'";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '12.3.3.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To12_3_6();
		}

		///<summary>Also in 12.2.34</summary>
		///<summary>Oracle compatible: 01/09/2013</summary>
		private static void To12_3_6() {
			if(FromVersion<new Version("12.3.6.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 12.3.6");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT ProgramNum FROM program WHERE ProgName='eClinicalWorks'";
					int programNum=PIn.Int(Db.GetScalar(command));
					command="SELECT COUNT(*) FROM programproperty WHERE ProgramNum="+programNum+" AND PropertyDesc='eCWServer'";
					if(Db.GetCount(command)=="0") {//Also added in 12.2 so we need to check to see if it exists
						command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
							+") VALUES("
							+"'"+POut.Long(programNum)+"', "
							+"'eCWServer', "
							+"'')";
						Db.NonQ(command);
					}
					command="SELECT COUNT(*) FROM programproperty WHERE ProgramNum="+programNum+" AND PropertyDesc='eCWPort'";
					if(Db.GetCount(command)=="0") {//Also added in 12.2 so we need to check to see if it exists
						command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
							+") VALUES("
							+"'"+POut.Long(programNum)+"', "
							+"'eCWPort', "
							+"'4928')";
						Db.NonQ(command);
					}
				}
				else {//oracle
					//eCW will never use Oracle.
				}
				command="UPDATE preference SET ValueString = '12.3.6.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To12_3_12();
		}

		///<summary>Oracle compatible: 01/09/2013</summary>
		private static void To12_3_12() {
			if(FromVersion<new Version("12.3.12.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 12.3.12");//No translation in convert script.
				string command="";
				//Sheets created in 12.3 with misc check boxes would have a trailing semicolon.  So we will remove the colon for the sheet field def.
				command="UPDATE sheetfielddef set FieldName='misc' WHERE FieldType=8 AND FieldName='misc:'";//FieldType 8 = CheckBox.
				Db.NonQ(command);
				//And remove the colon from all patient forms.
				command="UPDATE sheetfield SET FieldName='misc' WHERE FieldType=8 AND FieldName='misc:'";//FieldType 8 = CheckBox.
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '12.3.12.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To12_3_20();
		}

		///<summary>Oracle compatible: 01/09/2013</summary>
		private static void To12_3_20() {
			if(FromVersion<new Version("12.3.20.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 12.3.20");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('RecallExcludeIfAnyFutureAppt','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'RecallExcludeIfAnyFutureAppt','0')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '12.3.20.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To12_4_1();
		}

		///<summary>Oracle compatible: 01/09/2013</summary>
		private static void To12_4_1() {
			if(FromVersion<new Version("12.4.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 12.4.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS hl7def";
					Db.NonQ(command);
					command=@"CREATE TABLE hl7def (
						HL7DefNum bigint NOT NULL auto_increment PRIMARY KEY,
						Description varchar(255) NOT NULL,
						ModeTx tinyint NOT NULL,
						IncomingFolder varchar(255) NOT NULL,
						OutgoingFolder varchar(255) NOT NULL,
						IncomingPort varchar(255) NOT NULL,
						OutgoingIpPort varchar(255) NOT NULL,
						FieldSeparator varchar(5) NOT NULL,
						ComponentSeparator varchar(5) NOT NULL,
						SubcomponentSeparator varchar(5) NOT NULL,
						RepetitionSeparator varchar(5) NOT NULL,
						EscapeCharacter varchar(5) NOT NULL,
						IsInternal tinyint NOT NULL,
						InternalType varchar(255) NOT NULL,
						InternalTypeVersion varchar(50) NOT NULL,
						IsEnabled tinyint NOT NULL,
						Note text NOT NULL,
						HL7Server varchar(255) NOT NULL,
						HL7ServiceName varchar(255) NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE hl7def'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE hl7def (
						HL7DefNum number(20) NOT NULL,
						Description varchar2(255),
						ModeTx number(3) NOT NULL,
						IncomingFolder varchar2(255),
						OutgoingFolder varchar2(255),
						IncomingPort varchar2(255),
						OutgoingIpPort varchar2(255),
						FieldSeparator varchar2(5),
						ComponentSeparator varchar2(5),
						SubcomponentSeparator varchar2(5),
						RepetitionSeparator varchar2(5),
						EscapeCharacter varchar2(5),
						IsInternal number(3) NOT NULL,
						InternalType varchar2(255),
						InternalTypeVersion varchar2(50),
						IsEnabled number(3) NOT NULL,
						Note clob,
						HL7Server varchar2(255),
						HL7ServiceName varchar2(255),
						CONSTRAINT hl7def_HL7DefNum PRIMARY KEY (HL7DefNum)
						)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS hl7deffield";
					Db.NonQ(command);
					command=@"CREATE TABLE hl7deffield (
						HL7DefFieldNum bigint NOT NULL auto_increment PRIMARY KEY,
						HL7DefSegmentNum bigint NOT NULL,
						OrdinalPos int NOT NULL,
						TableId varchar(255) NOT NULL,
						DataType varchar(255) NOT NULL,
						FieldName varchar(255) NOT NULL,
						FixedText text NOT NULL,
						INDEX(HL7DefSegmentNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE hl7deffield'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE hl7deffield (
						HL7DefFieldNum number(20) NOT NULL,
						HL7DefSegmentNum number(20) NOT NULL,
						OrdinalPos number(11) NOT NULL,
						TableId varchar2(255),
						DataType varchar2(255),
						FieldName varchar2(255),
						FixedText varchar2(2000),
						CONSTRAINT hl7deffield_HL7DefFieldNum PRIMARY KEY (HL7DefFieldNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX hl7deffield_HL7DefSegmentNum ON hl7deffield (HL7DefSegmentNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS hl7defmessage";
					Db.NonQ(command);
					command=@"CREATE TABLE hl7defmessage (
						HL7DefMessageNum bigint NOT NULL auto_increment PRIMARY KEY,
						HL7DefNum bigint NOT NULL,
						MessageType varchar(255) NOT NULL,
						EventType varchar(255) NOT NULL,
						InOrOut tinyint NOT NULL,
						ItemOrder int NOT NULL,
						Note text NOT NULL,
						INDEX(HL7DefNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE hl7defmessage'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE hl7defmessage (
						HL7DefMessageNum number(20) NOT NULL,
						HL7DefNum number(20) NOT NULL,
						MessageType varchar2(255),
						EventType varchar2(255),
						InOrOut number(3) NOT NULL,
						ItemOrder number(11) NOT NULL,
						Note clob,
						CONSTRAINT hl7defmessage_HL7DefMessageNum PRIMARY KEY (HL7DefMessageNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX hl7defmessage_HL7DefNum ON hl7defmessage (HL7DefNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS hl7defsegment";
					Db.NonQ(command);
					command=@"CREATE TABLE hl7defsegment (
						HL7DefSegmentNum bigint NOT NULL auto_increment PRIMARY KEY,
						HL7DefMessageNum bigint NOT NULL,
						ItemOrder int NOT NULL,
						CanRepeat tinyint NOT NULL,
						IsOptional tinyint NOT NULL,
						SegmentName varchar(255) NOT NULL,
						Note text NOT NULL,
						INDEX(HL7DefMessageNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE hl7defsegment'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE hl7defsegment (
						HL7DefSegmentNum number(20) NOT NULL,
						HL7DefMessageNum number(20) NOT NULL,
						ItemOrder number(11) NOT NULL,
						CanRepeat number(3) NOT NULL,
						IsOptional number(3) NOT NULL,
						SegmentName varchar2(255),
						Note clob,
						CONSTRAINT hl7defsegment_HL7DefSegmentNum PRIMARY KEY (HL7DefSegmentNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX hl7defsegment_HL7DefMessageNum ON hl7defsegment (HL7DefMessageNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE recall ADD DateScheduled date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE recall ADD DateScheduled date";
					Db.NonQ(command);
					command="UPDATE recall SET DateScheduled = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateScheduled IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE recall MODIFY DateScheduled NOT NULL";
					Db.NonQ(command);
				}
				try{
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE recall ADD INDEX (DateScheduled)";
						Db.NonQ(command);
					}
					else {//oracle
						command="CREATE INDEX recall_DateScheduled ON recall (DateScheduled)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//ex is needed, or exception won't get caught.
				}
				DataTable table;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command=@"SELECT recalltrigger.RecallTypeNum,MIN(DATE(appointment.AptDateTime)) AS AptDateTime,recall.PatNum
						FROM appointment,procedurelog,recalltrigger,recall
						WHERE appointment.AptNum=procedurelog.AptNum  
						AND procedurelog.CodeNum=recalltrigger.CodeNum 
						AND recall.PatNum=procedurelog.PatNum 
						AND recalltrigger.RecallTypeNum=recall.RecallTypeNum 
						AND (appointment.AptStatus=1 "//Scheduled
						+"OR appointment.AptStatus=4) "//ASAP
						+"AND appointment.AptDateTime > CURDATE() " //early this morning
						+"GROUP BY recalltrigger.RecallTypeNum,recall.PatNum ";
						table=Db.GetTable(command);
						for(int i=0;i<table.Rows.Count;i++) {
							if(table.Rows[i]["RecallTypeNum"].ToString()=="") {
								continue;//this might happen if there are zero results, but the MIN in the query causes one result row with NULLs.
							}
							command=@"UPDATE recall	SET recall.DateScheduled="+POut.Date(PIn.Date(table.Rows[i]["AptDateTime"].ToString()))+" " 
								+"WHERE recall.RecallTypeNum="+POut.Long(PIn.Long(table.Rows[i]["RecallTypeNum"].ToString()))+" "
								+"AND recall.PatNum="+POut.Long(PIn.Long(table.Rows[i]["PatNum"].ToString()));
							Db.NonQ(command);
						}
				}
				else {//oracle
					command=@"SELECT recalltrigger.RecallTypeNum,MIN(TO_DATE(appointment.AptDateTime)) AS AptDateTime,recall.PatNum
						FROM appointment,procedurelog,recalltrigger,recall
						WHERE appointment.AptNum=procedurelog.AptNum  
						AND procedurelog.CodeNum=recalltrigger.CodeNum 
						AND recall.PatNum=procedurelog.PatNum 
						AND recalltrigger.RecallTypeNum=recall.RecallTypeNum 
						AND (appointment.AptStatus=1 "//Scheduled
						+"OR appointment.AptStatus=4) "//ASAP
						+"AND appointment.AptDateTime > SYSDATE " //early this morning
						+"GROUP BY recalltrigger.RecallTypeNum,recall.PatNum ";
					table=Db.GetTable(command);
					for(int i=0;i<table.Rows.Count;i++) {
						if(table.Rows[i]["RecallTypeNum"].ToString()=="") {
							continue;
						}
						command=@"UPDATE recall	SET recall.DateScheduled="+POut.Date(PIn.Date(table.Rows[i]["AptDateTime"].ToString()))+" " 
							+"WHERE recall.RecallTypeNum="+POut.Long(PIn.Long(table.Rows[i]["RecallTypeNum"].ToString()))+" "
							+"AND recall.PatNum="+POut.Long(PIn.Long(table.Rows[i]["PatNum"].ToString()));
						Db.NonQ(command);
					}
				}
				//Insert CaptureLink Bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
				  command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
				    +"'CaptureLink', "
				    +"'CaptureLink from www.henryschein.ca', "
				    +"'0', "
				    +"'"+POut.String(@"C:\Program Files\imaginIT\ImaginIT.exe")+"',"
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
				    +"'CaptureLink')";
				  Db.NonQ(command);
				}
				else {//oracle
				  command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'CaptureLink', "
				    +"'CaptureLink from www.henryschein.ca', "
				    +"'0', "
				    +"'"+POut.String(@"C:\Program Files\imaginIT\ImaginIT.exe")+"',"
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
				    +"'CaptureLink')";
				  Db.NonQ(command);
				}//end CaptureLink bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE deposit ADD Memo varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE deposit ADD Memo varchar2(255)";
					Db.NonQ(command);
				}
				//Insert Divvy Systems/eCards bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
				    +"'Divvy', "
				    +"'Divvy from www.divvysystems.com', "
				    +"'0', "
				    +"'',"
				    +"'', "
				    +"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
					  +"'"+POut.Long(programNum)+"', "
					  +"'Username', "
					  +"'')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
					  +"'"+POut.Long(programNum)+"', "
					  +"'Password', "
					  +"'')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
					  +"'"+POut.Long(programNum)+"', "
					  +"'API Key', "
					  +"'')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
					  +"'"+POut.Long(programNum)+"', "
					  +"'DesignID for Recall Cards', "
					  +"'')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'Divvy', "
				    +"'Divvy from www.divvysystems.com', "
				    +"'0', "
				    +"'',"
				    +"'', "
				    +"'')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
					  +") VALUES("
					  +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
					  +"'"+POut.Long(programNum)+"', "
					  +"'Username', "
					  +"'')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
					  +") VALUES("
					  +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
					  +"'"+POut.Long(programNum)+"', "
					  +"'Password', "
					  +"'')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
					  +") VALUES("
					  +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
					  +"'"+POut.Long(programNum)+"', "
					  +"'API Key', "
					  +"'')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
					  +") VALUES("
					  +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
					  +"'"+POut.Long(programNum)+"', "
					  +"'DesignID for Recall Cards', "
					  +"'')";
					Db.NonQ(command);
				}//end Divvy Systems/eCards bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('InsDefaultAssignBen','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'InsDefaultAssignBen','1')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurelog ADD UnitQtyType tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurelog ADD UnitQtyType number(3)";
					Db.NonQ(command);
					command="UPDATE procedurelog SET UnitQtyType = 0 WHERE UnitQtyType IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog MODIFY UnitQtyType NOT NULL";
					Db.NonQ(command);
				}
				//Fix medical claim form 1500, unit quantity fields.  The unique ID is OD9.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD9' LIMIT 1";
				}
				else {//oracle doesn't have LIMIT
					command="SELECT * FROM (SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD9') WHERE RowNum<=1";
				}
				long claimFormNum=PIn.Long(Db.GetScalar(command));
				command="UPDATE claimformitem SET FieldName='P1UnitQty',FormatString='' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND FormatString='1' AND Xpos=615 AND YPos=763";
				Db.NonQ(command);
				command="UPDATE claimformitem SET FieldName='P2UnitQty',FormatString='' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND FormatString='1' AND Xpos=615 AND YPos=796";
				Db.NonQ(command);
				command="UPDATE claimformitem SET FieldName='P3UnitQty',FormatString='' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND FormatString='1' AND Xpos=615 AND YPos=828";
				Db.NonQ(command);
				command="UPDATE claimformitem SET FieldName='P4UnitQty',FormatString='' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND FormatString='1' AND Xpos=615 AND YPos=862";
				Db.NonQ(command);
				command="UPDATE claimformitem SET FieldName='P5UnitQty',FormatString='' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND FormatString='1' AND Xpos=615 AND YPos=895";
				Db.NonQ(command);
				command="UPDATE claimformitem SET FieldName='P6UnitQty',FormatString='' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND FormatString='1' AND Xpos=615 AND YPos=929";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE hl7msg ADD PatNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE hl7msg ADD INDEX (PatNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE hl7msg ADD PatNum number(20)";
					Db.NonQ(command);
					command="UPDATE hl7msg SET PatNum = 0 WHERE PatNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE hl7msg MODIFY PatNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX hl7msg_PatNum ON hl7msg (PatNum)";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE hl7msg ADD Note text NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE hl7msg ADD Note varchar2(2000)";
					Db.NonQ(command);
				} 
				//Permission for billing
				command="SELECT UserGroupNum FROM usergroup";
				table=Db.GetTable(command);
				long groupNum;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+","+POut.Long((int)Permissions.Billing)+")";
						Db.NonQ(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+","+POut.Long((int)Permissions.Billing)+")";
						Db.NonQ(command);
					}
				}
				//Add EquipmentSetup permission to all groups that had Setup permission---------------------------------------------
				command="SELECT DISTINCT UserGroupNum "
					+"FROM grouppermission "
					+"WHERE PermType="+POut.Int((int)Permissions.Setup);
				table=Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+","+POut.Int((int)Permissions.EquipmentSetup)+")";
						Db.NonQ(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+","+POut.Int((int)Permissions.EquipmentSetup)+")";
						Db.NonQ(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT ProgramNum FROM program WHERE ProgName='eClinicalWorks'";
					int programNum=PIn.Int(Db.GetScalar(command));
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'HL7Server', "
						+"'')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'HL7ServiceName', "
						+"'')";
					Db.NonQ(command);
				}
				else {//oracle
					//eCW will never use Oracle.
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clockevent ADD AmountBonus double NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clockevent ADD AmountBonus number(38,8)";
					Db.NonQ(command);
					command="UPDATE clockevent SET AmountBonus = 0 WHERE AmountBonus IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE clockevent MODIFY AmountBonus NOT NULL";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clockevent ADD AmountBonusAuto double NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clockevent ADD AmountBonusAuto number(38,8)";
					Db.NonQ(command);
					command="UPDATE clockevent SET AmountBonusAuto = 0 WHERE AmountBonusAuto IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE clockevent MODIFY AmountBonusAuto NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE timecardrule ADD AmtDiff double NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE timecardrule ADD AmtDiff number(38,8)";
					Db.NonQ(command);
					command="UPDATE timecardrule SET AmtDiff = 0 WHERE AmtDiff IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE timecardrule MODIFY AmtDiff NOT NULL";
					Db.NonQ(command);
				}
				//Negatives should not be part of prefs.  Too confusing.  But moving to a new pref name is not easy.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('AtoZfolderUsed','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'AtoZfolderUsed','')";
					Db.NonQ(command);
				}
				command="SELECT ValueString FROM preference WHERE PrefName='AtoZfolderNotRequired'";
				string atozFolderNotRequired=Db.GetScalar(command);
				if(atozFolderNotRequired=="1") {
					command="UPDATE preference SET ValueString='0' WHERE PrefName='AtoZfolderUsed'";
					Db.NonQ(command);
				}
				else {//blank or 0
					command="UPDATE preference SET ValueString='1' WHERE PrefName='AtoZfolderUsed'";
					Db.NonQ(command);
				}
				//command="UPDATE preference SET ValueString='' WHERE PrefName='AtoZfolderNotRequired'";//we can't do this because we need the old value in place during update.
				command="UPDATE preference SET Comments='Deprecated.  Use AtoZfolderUsed instead' WHERE PrefName='AtoZfolderNotRequired'";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS documentmisc";
					Db.NonQ(command);
					command=@"CREATE TABLE documentmisc (
						DocMiscNum bigint NOT NULL auto_increment PRIMARY KEY,
						DateCreated date NOT NULL DEFAULT '0001-01-01',
						FileName varchar(255) NOT NULL,
						DocMiscType tinyint NOT NULL,
						RawBase64 longtext NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE documentmisc'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE documentmisc (
						DocMiscNum number(20) NOT NULL,
						DateCreated date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						FileName varchar2(255),
						DocMiscType number(3) NOT NULL,
						RawBase64 clob,
						CONSTRAINT documentmisc_DocMiscNum PRIMARY KEY (DocMiscNum)
						)";
					Db.NonQ(command);
				}
				//Add ProblemEdit permission to all groups that had Setup permission---------------------------------------------
				command="SELECT DISTINCT UserGroupNum "
				  +"FROM grouppermission "
				  +"WHERE PermType="+POut.Int((int)Permissions.Setup);
				table=Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				      +"VALUES("+POut.Long(groupNum)+","+POut.Int((int)Permissions.ProblemEdit)+")";
						Db.NonQ(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
				      +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+","+POut.Int((int)Permissions.ProblemEdit)+")";
						Db.NonQ(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS toothgridcell";
					Db.NonQ(command);
					command=@"CREATE TABLE toothgridcell (
						ToothGridCellNum bigint NOT NULL auto_increment PRIMARY KEY,
						SheetFieldNum bigint NOT NULL,
						ToothGridColNum bigint NOT NULL,
						ValueEntered varchar(255) NOT NULL,
						ToothNum varchar(10) NOT NULL,
						INDEX(SheetFieldNum),
						INDEX(ToothGridColNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE toothgridcell'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE toothgridcell (
						ToothGridCellNum number(20) NOT NULL,
						SheetFieldNum number(20) NOT NULL,
						ToothGridColNum number(20) NOT NULL,
						ValueEntered varchar2(255),
						ToothNum varchar2(10),
						CONSTRAINT toothgridcell_ToothGridCellNum PRIMARY KEY (ToothGridCellNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX toothgridcell_SheetFieldNum ON toothgridcell (SheetFieldNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX toothgridcell_ToothGridColNum ON toothgridcell (ToothGridColNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS toothgridcol";
					Db.NonQ(command);
					command=@"CREATE TABLE toothgridcol (
						ToothGridColNum bigint NOT NULL auto_increment PRIMARY KEY,
						SheetFieldNum bigint NOT NULL,
						NameItem varchar(255) NOT NULL,
						CellType tinyint NOT NULL,
						ItemOrder smallint NOT NULL,
						ColumnWidth smallint NOT NULL,
						CodeNum bigint NOT NULL,
						ProcStatus tinyint NOT NULL,
						INDEX(SheetFieldNum),
						INDEX(CodeNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE toothgridcol'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE toothgridcol (
						ToothGridColNum number(20) NOT NULL,
						SheetFieldNum number(20) NOT NULL,
						NameItem varchar2(255),
						CellType number(3) NOT NULL,
						ItemOrder number(11) NOT NULL,
						ColumnWidth number(11) NOT NULL,
						CodeNum number(20) NOT NULL,
						ProcStatus number(3) NOT NULL,
						CONSTRAINT toothgridcol_ToothGridColNum PRIMARY KEY (ToothGridColNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX toothgridcol_SheetFieldNum ON toothgridcol (SheetFieldNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX toothgridcol_CodeNum ON toothgridcol (CodeNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS toothgriddef";
					Db.NonQ(command);
					command=@"CREATE TABLE toothgriddef (
						ToothGridDefNum bigint NOT NULL auto_increment PRIMARY KEY,
						NameInternal varchar(255),
						NameShowing varchar(255),
						CellType tinyint NOT NULL,
						ItemOrder smallint NOT NULL,
						ColumnWidth smallint NOT NULL,
						CodeNum bigint NOT NULL,
						ProcStatus tinyint NOT NULL,
						INDEX(CodeNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE toothgriddef'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE toothgriddef (
						ToothGridDefNum number(20) NOT NULL,
						NameInternal varchar2(255),
						NameShowing varchar2(255),
						CellType number(3) NOT NULL,
						ItemOrder number(11) NOT NULL,
						ColumnWidth number(11) NOT NULL,
						CodeNum number(20) NOT NULL,
						ProcStatus number(3) NOT NULL,
						CONSTRAINT toothgriddef_ToothGridDefNum PRIMARY KEY (ToothGridDefNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX toothgriddef_CodeNum ON toothgriddef (CodeNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE sheetfield ADD ReportableName varchar(255)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE sheetfield ADD ReportableName varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE sheetfielddef ADD ReportableName varchar(255)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE sheetfielddef ADD ReportableName varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE provider ADD StateWhereLicensed varchar(50) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE provider ADD StateWhereLicensed varchar2(50)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PracticeFax','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PracticeFax','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PublicHealthScreeningUsePat','0')";
					Db.NonQ(command);
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PublicHealthScreeningSheet','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PublicHealthScreeningUsePat','0')";
					Db.NonQ(command);
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PublicHealthScreeningSheet','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD Fax varchar(50) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD Fax varchar2(50)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS screenpat";
					Db.NonQ(command);
					command=@"CREATE TABLE screenpat (
						ScreenPatNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						ScreenGroupNum bigint NOT NULL,
						SheetNum bigint NOT NULL,
						INDEX(PatNum),
						INDEX(ScreenGroupNum),
						INDEX(SheetNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE screenpat'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE screenpat (
						ScreenPatNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						ScreenGroupNum number(20) NOT NULL,
						SheetNum number(20) NOT NULL,
						CONSTRAINT screenpat_ScreenPatNum PRIMARY KEY (ScreenPatNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX screenpat_PatNum ON screenpat (PatNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX screenpat_ScreenGroupNum ON screenpat (ScreenGroupNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX screenpat_SheetNum ON screenpat (SheetNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE adjustment ADD StatementNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE adjustment ADD INDEX (StatementNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE adjustment ADD StatementNum number(20)";
					Db.NonQ(command);
					command="UPDATE adjustment SET StatementNum = 0 WHERE StatementNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE adjustment MODIFY StatementNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX adjustment_StatementNum ON adjustment (StatementNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurelog ADD StatementNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog ADD INDEX (StatementNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurelog ADD StatementNum number(20)";
					Db.NonQ(command);
					command="UPDATE procedurelog SET StatementNum = 0 WHERE StatementNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog MODIFY StatementNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX procedurelog_StatementNum ON procedurelog (StatementNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE statement ADD IsInvoice tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE statement ADD IsInvoice number(3)";
					Db.NonQ(command);
					command="UPDATE statement SET IsInvoice = 0 WHERE IsInvoice IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE statement MODIFY IsInvoice NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE statement ADD IsInvoiceCopy tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE statement ADD IsInvoiceCopy number(3)";
					Db.NonQ(command);
					command="UPDATE statement SET IsInvoiceCopy = 0 WHERE IsInvoiceCopy IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE statement MODIFY IsInvoiceCopy NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('BillingDefaultsInvoiceNote','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'BillingDefaultsInvoiceNote','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('NewCropAccountId','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'NewCropAccountId','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('NewCropName','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'NewCropName','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('NewCropPassword','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'NewCropPassword','')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '12.4.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To12_4_12();
		}

		///<summary>Oracle compatible: 01/09/2013</summary>
		private static void To12_4_12() {
			if(FromVersion<new Version("12.4.12.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 12.4.12");//No translation in convert script.
				string command;
				//Add the ADA2012 claim form. The unique ID is OD11.
				long claimFormNum=0;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO claimform(Description,IsHidden,FontName,FontSize,UniqueID,PrintImages,OffsetX,OffsetY) "+
						"VALUES ('ADA 2012',0,'Arial',9,'OD11',1,0,0)";
					claimFormNum=Db.NonQ(command,true);
				}
				else {//oracle
					command="INSERT INTO claimform(ClaimFormNum,Description,IsHidden,FontName,FontSize,UniqueID,PrintImages,OffsetX,OffsetY) "+
						"VALUES ((SELECT COALESCE(MAX(ClaimFormNum),0)+1 ClaimFormNum FROM claimform),'ADA 2012',0,'Arial',9,'OD11',1,0,0)";
					claimFormNum=Db.NonQ(command,true,"ClaimFormNum","claimform");
				}
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P9UnitQty','',490,617,30,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P10UnitQty','',490,633,30,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P8UnitQty','',490,601,30,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P7UnitQty','',490,584,30,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6UnitQty','',490,567,30,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4UnitQty','',490,533,30,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5UnitQty','',490,550,30,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3UnitQty','',490,516,30,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2UnitQty','',490,499,30,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1UnitQty','',490,483,30,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P9Diagnosis','',440,617,50,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P10Diagnosis','',440,633,50,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P8Diagnosis','',440,601,50,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P7Diagnosis','',440,584,50,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6Diagnosis','',440,567,50,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4Diagnosis','',440,533,50,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5Diagnosis','',440,550,50,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3Diagnosis','',440,516,50,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2Diagnosis','',440,499,50,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1Diagnosis','',440,483,50,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Diagnosis4','',610,683,80,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Diagnosis2','',502,683,80,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Diagnosis3','',610,666,80,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Diagnosis1','',502,666,80,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PlaceNumericCode','',510,750,27,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingDentistNPI','',451,981,160,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistNPI','',42,1029,100,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss17','',333,683,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingProviderSpecialty','',699,999,130,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss16','',333,667,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss15','',314,667,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss13','',274,667,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss14','',294,667,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss12','',253,667,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss11','',234,667,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss9','',193,667,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss10','',214,667,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss8','',174,667,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss7','',155,667,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss6','',134,667,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss5','',113,667,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss3','',73,667,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss2','',54,667,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss1','',34,667,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss4','',94,667,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TotalFee','',827,683,59,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P10Fee','',827,633,59,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P10Description','',521,633,240,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P10Code','',382,633,54,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P10Surface','',322,633,54,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P10ToothNumber','',204,633,110,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P10System','',172,633,27,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P10Area','',142,633,27,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P10Date','MM/dd/yyyy',42,633,96,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P9Fee','',827,617,59,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P9Description','',521,617,240,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P9Code','',382,617,54,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P9Surface','',322,617,54,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P9ToothNumber','',204,617,110,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P9System','',172,617,27,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P9Area','',142,617,27,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P9Date','MM/dd/yyyy',42,617,96,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P8Fee','',827,601,59,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P8Description','',521,601,240,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P8Code','',382,601,54,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P8Surface','',322,601,54,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P8ToothNumber','',204,601,110,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P8System','',172,601,27,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P8Date','MM/dd/yyyy',42,601,96,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P8Area','',142,601,27,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P7Description','',521,584,240,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P7Fee','',827,584,59,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P7Code','',382,584,54,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P7Surface','',322,584,54,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P7ToothNumber','',204,584,110,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P7System','',172,584,27,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P7Area','',142,584,27,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P7Date','MM/dd/yyyy',42,584,96,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6Fee','',827,567,59,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6Code','',382,567,54,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6Description','',521,567,240,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6Surface','',322,567,54,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6System','',172,567,27,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6ToothNumber','',204,567,110,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6Date','MM/dd/yyyy',42,567,96,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6Area','',142,567,27,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5Fee','',827,550,59,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5Description','',521,550,240,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5Code','',382,550,54,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5Surface','',322,550,54,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5ToothNumber','',204,550,110,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5System','',172,550,27,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5Area','',142,550,27,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5Date','MM/dd/yyyy',42,550,96,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4Fee','',827,533,59,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4Description','',521,533,240,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4Code','',382,533,54,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4Surface','',322,533,54,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4ToothNumber','',204,533,110,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4System','',172,533,27,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4Area','',142,533,27,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4Date','MM/dd/yyyy',42,533,96,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3Fee','',827,516,59,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3Description','',521,516,240,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3Code','',382,516,54,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3Surface','',322,516,54,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3ToothNumber','',204,516,110,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3System','',172,516,27,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3Area','',142,516,27,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3Date','MM/dd/yyyy',42,516,96,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2Description','',521,499,240,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2Fee','',827,499,59,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2Code','',382,499,54,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2ToothNumber','',204,499,110,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2Surface','',322,499,54,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2System','',172,499,27,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2Area','',142,499,27,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2Date','MM/dd/yyyy',42,499,96,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1Fee','',827,483,59,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1Description','',521,483,240,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1Code','',382,483,54,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1Surface','',322,483,54,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1ToothNumber','',204,483,110,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1System','',172,483,27,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1Area','',142,483,27,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1Date','MM/dd/yyyy',42,483,96,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientSSN','',665,416,100,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientIsFemale','',614,416,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientIsMale','',584,416,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientDOB','MM/dd/yyyy',448,416,90,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientZip','',698,382,100,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientST','',645,382,40,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientCity','',448,382,185,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientAddress2','',448,366,350,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientAddress','',448,350,350,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientLastFirst','',448,334,350,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','RelatIsOther','',645,301,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','RelatIsChild','',555,301,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','RelatIsSpouse','',495,301,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','RelatIsSelf','',445,301,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','EmployerName','',581,250,230,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrIsFemale','',614,217,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrID','',665,215,120,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','GroupNum','',448,250,95,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrIsMale','',585,217,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrDOB','MM/dd/yyyy',448,215,100,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrZip','',698,181,100,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrST','',645,181,40,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrCity','',448,181,185,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrAddress2','',448,165,350,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrAddress','',448,149,350,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsZip','',292,416,100,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsST','',239,416,40,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrLastFirst','',448,133,350,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsCity','',42,416,185,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsAddress','',42,400,350,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsCarrierName','',42,384,350,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsRelatIsChild','',284,351,0,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsRelatIsOther','',354,351,0,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsRelatIsSelf','',174,351,0,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsRelatIsSpouse','',225,351,0,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsGroupNum','',42,349,111,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsSubscrID','',258,316,130,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsSubscrIsFemale','',205,318,0,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PriInsZip','',300,215,100,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsSubscrIsMale','',174,318,0,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsSubscrDOB','MM/dd/yyyy',42,316,100,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsExistsMed','',155,251,0,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsExistsDent','',75,251,0,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PriInsST','',248,215,40,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsSubscrLastFirst','',43,283,340,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PriInsAddress2','',85,199,315,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PriInsCity','',85,215,150,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PriInsCarrierName','',85,167,315,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PriInsAddress','',85,183,315,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PreAuthString','',42,116,200,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsEnclosuresAttached','',723,767,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsPreAuth','',195,68,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsStandardClaim','',34,68,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingDentistProviderID','',685,1048,130,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistProviderID','',294,1048,120,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss19','',294,683,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss18','',314,683,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss20','',274,683,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss22','',234,683,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss21','',253,683,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss24','',193,683,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss23','',214,683,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss25','',174,683,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss27','',134,683,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss26','',155,683,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss28','',113,683,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss32','',34,683,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss31','',54,683,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss30','',73,683,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Miss29','',94,683,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientRelease','',42,801,240,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','Remarks','',77,701,708,29)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientReleaseDate','MM/dd/yyyy',298,801,110,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientAssignment','',42,867,240,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientAssignmentDate','MM/dd/yyyy',298,867,110,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','FixedText','B',504,650,14,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsNotOrtho','',444,798,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsOrtho','',534,798,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','DateOrthoPlaced','MM/dd/yyyy',676,797,110,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','MonthsOrthoRemaining','',484,830,30,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsReplacementProsth','',564,833,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsNotReplacementProsth','',534,833,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','DatePriorProsthPlaced','MM/dd/yyyy',676,830,110,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsOtherAccident','',694,867,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsAutoAccident','',594,866,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsOccupational','',444,866,0,0)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','AccidentST','',792,882,35,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','AccidentDate','MM/dd/yyyy',573,882,110,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentist','',42,947,350,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PayToDentistAddress2','',42,979,350,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PayToDentistAddress','',42,963,350,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PayToDentistST','',239,995,40,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PayToDentistCity','',42,995,185,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PayToDentistZip','',292,995,100,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistLicenseNum','',170,1029,100,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistSSNorTIN','',300,1029,100,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistPh123','',84,1048,30,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistPh78910','',164,1048,50,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistPh456','',126,1048,30,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingDentistSignature','',438,949,245,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingDentistSigDate','MM/dd/yyyy',709,949,100,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingDentistLicense','',699,981,130,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingDentistAddress','',434,1013,350,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingDentistCity','',434,1029,165,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingDentistST','',606,1029,40,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingDentistZip','',658,1029,110,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingDentistPh456','',524,1048,30,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingDentistPh123','',485,1048,30,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingDentistPh78910','',563,1048,40,14)";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'ADA2012.gif','','',17,15,815,1063)";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '12.4.12.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To12_4_14();
		}

		///<summary>Oracle compatible: 01/09/2013</summary>
		private static void To12_4_14() {
			if(FromVersion<new Version("12.4.14.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 12.4.14");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS erxlog";
					Db.NonQ(command);
					command=@"CREATE TABLE erxlog (
						ErxLogNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						MsgText mediumtext NOT NULL,
						DateTStamp timestamp,
						INDEX(PatNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE erxlog'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE erxlog (
						ErxLogNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						MsgText clob,
						DateTStamp timestamp,
						CONSTRAINT erxlog_ErxLogNum PRIMARY KEY (ErxLogNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX erxlog_PatNum ON erxlog (PatNum)";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '12.4.14.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To12_4_22();
		}

		///<summary>Oracle compatible: 01/09/2013</summary>
		private static void To12_4_22() {
			if(FromVersion<new Version("12.4.22.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 12.4.22");//No translation in convert script.
				string command;
				//Claimstream clearinghouse.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command=@"INSERT INTO clearinghouse(Description,ExportPath,Payors,Eformat,ISA05,SenderTin,ISA07,ISA08,ISA15,Password,ResponsePath,CommBridge,ClientProgram,
						LastBatchNumber,ModemPort,LoginID,SenderName,SenderTelephone,GS03,ISA02,ISA04,ISA16,SeparatorData,SeparatorSegment) 
						VALUES ('Claimstream','"+POut.String(@"C:\ccd\abc\")+"','000090','3','','','','','','','','15','"+POut.String(@"C:\ccd\abc\ccdws.exe")+"',0,0,'','','','','','','','','')";
					Db.NonQ(command);
				}
				else {//oracle
					command=@"INSERT INTO clearinghouse(ClearinghouseNum,Description,ExportPath,Payors,Eformat,ISA05,SenderTin,ISA07,ISA08,ISA15,Password,ResponsePath,CommBridge,ClientProgram,
						LastBatchNumber,ModemPort,LoginID,SenderName,SenderTelephone,GS03,ISA02,ISA04,ISA16,SeparatorData,SeparatorSegment) 
						VALUES ((SELECT MAX(ClearinghouseNum+1) FROM clearinghouse),'Claimstream','"+POut.String(@"C:\ccd\abc\")+"','000090','3','','','','','','','','15','"+POut.String(@"C:\ccd\abc\ccdws.exe")+"',0,0,'','','','','','','','','')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '12.4.22.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To12_4_28();
		}

		///<summary>Oracle compatible: 01/09/2013</summary>
		private static void To12_4_28() {
			if(FromVersion<new Version("12.4.28.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 12.4.28");//No translation in convert script.
				string command;
				if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
					long codeNum=0;
					string procCode="D1208";//This code is needed for important automation.
					command="SELECT CodeNum FROM procedurecode WHERE ProcCode='"+POut.String(procCode)+"'";
					DataTable dtProcCode=Db.GetTable(command);
					if(dtProcCode.Rows.Count==0) {//The procedure code does not exist
						//Make sure the procedure code category exists before inserting the procedure code
						string procCatDescript="Cleanings";
						long defNum=0;
						command="SELECT DefNum FROM definition WHERE Category=11 AND ItemName='"+POut.String(procCatDescript)+"'";//11 is DefCat.ProcCodeCats
						DataTable dtDef=Db.GetTable(command);
						if(dtDef.Rows.Count==0) { //The procedure code category does not exist, add it
							command="SELECT COUNT(*) FROM definition WHERE Category=11";//11 is DefCat.ProcCodeCats
							string itemOrder=Db.GetCount(command);
							if(DataConnection.DBtype==DatabaseType.MySql) {
								command="INSERT INTO definition (Category,ItemName,ItemOrder) "
									+"VALUES (11,'"+POut.String(procCatDescript)+"',"+itemOrder+")";//11 is DefCat.ProcCodeCats
							}
							else {//oracle
								command="INSERT INTO definition (DefNum,Category,ItemName,ItemOrder) "
									+"VALUES ((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),11,'"+POut.String(procCatDescript)+"',"+itemOrder+")";//11 is DefCat.ProcCodeCats
							}
							defNum=Db.NonQ(command,true,"DefNum","definition");
						}
						else { //The procedure code category already exists, get the existing defnum
							defNum=PIn.Long(dtDef.Rows[0][0].ToString());
						}
						//The following variables might be useful if we need to copy this pattern later for adding more codes.
						string procDescript="topical application of fluoride";
						string procAbbrDesc="Flo";
						string procTime="/";
						long procTreatArea=3;
						int procNoBillIns=0;
						int procIsProsth=0;
						int procIsHygiene=1;
						int procPaintType=0;//none
						if(DataConnection.DBtype==DatabaseType.MySql) {
							command=@"INSERT INTO procedurecode(ProcCode,Descript,AbbrDesc,
									ProcTime,ProcCat,TreatArea,NoBillIns,IsProsth,IsHygiene,PaintType,DefaultNote,SubstitutionCode) 
								VALUES ('"+POut.String(procCode)+"','"+POut.String(procDescript)+"','"+POut.String(procAbbrDesc)+"',"
									+"'"+POut.String(procTime)+"','"+POut.Long(defNum)+"',"+POut.Long(procTreatArea)+","+POut.Int(procNoBillIns)+","+POut.Int(procIsProsth)+","
									+POut.Int(procIsHygiene)+","+POut.Int(procPaintType)+",'','')";
							codeNum=Db.NonQ(command,true);
						}
						else {//oracle
							command=@"INSERT INTO procedurecode(CodeNum,ProcCode,Descript,AbbrDesc,
									ProcTime,ProcCat,TreatArea,NoBillIns,IsProsth,IsHygiene,PaintType,IsCanadianLab,BaseUnits,SubstOnlyIf,IsMultiVisit,ProvNumDefault,GraphicColor,DefaultNote,SubstitutionCode) 
								VALUES ((SELECT COALESCE(MAX(CodeNum),0)+1 CodeNum FROM procedurecode),'"+POut.String(procCode)+"','"+POut.String(procDescript)+"','"+POut.String(procAbbrDesc)+"',"
									+"'"+POut.String(procTime)+"','"+POut.Long(defNum)+"',"+POut.Long(procTreatArea)+","+POut.Int(procNoBillIns)+","+POut.Int(procIsProsth)+","
									+POut.Int(procIsHygiene)+","+POut.Int(procPaintType)+",0,0,0,0,0,0,'','')";
							codeNum=Db.NonQ(command,true,"CodeNum","procedurecode");
						}
					}//end D1208 insert
					else {//D1208 already exists.
						codeNum=PIn.Long(dtProcCode.Rows[0][0].ToString());
					}
					//Various references to the old D1203 and D1204 need to be changed to the new D1208.
					long codeNumD1203=0;
					command="SELECT CodeNum FROM procedurecode WHERE ProcCode='D1203'";
					dtProcCode=Db.GetTable(command);
					if(dtProcCode.Rows.Count>0) {
						codeNumD1203=PIn.Long(dtProcCode.Rows[0][0].ToString());
					}
					long codeNumD1204=0;
					command="SELECT CodeNum FROM procedurecode WHERE ProcCode='D1204'";
					dtProcCode=Db.GetTable(command);
					if(dtProcCode.Rows.Count>0) {
						codeNumD1204=PIn.Long(dtProcCode.Rows[0][0].ToString());
					}
					//Update benefits to remove the old codes and replace with the new code. We ignore CodeNum=0 in case D1203 or D1204 is not in the database.
					command="UPDATE benefit SET CodeNum="+POut.Long(codeNum)+" WHERE CodeNum<>0 AND (CodeNum="+POut.Long(codeNumD1203)+" OR CodeNum="+POut.Long(codeNumD1204)+")";
					Db.NonQ(command);
					//Clean up insurance plans with obvious duplicate fluoride benefits (plans that used to have benefits for both D1203 and D1204).
					command="SELECT DISTINCT PlanNum,CovCatNum,TimePeriod,Quantity,CoverageLevel "
						+"FROM benefit "
						+"WHERE BenefitType="+POut.Long((long)InsBenefitType.Limitations)+" AND MonetaryAmt=-1 AND PatPlanNum=0 AND Percent=-1 "
						+"AND QuantityQualifier="+POut.Long((long)BenefitQuantity.AgeLimit)+" AND CodeNum="+POut.Long(codeNum);
					DataTable dtBenFlo=Db.GetTable(command);
					command="DELETE FROM benefit "
						+"WHERE BenefitType="+POut.Long((long)InsBenefitType.Limitations)+" AND MonetaryAmt=-1 AND PatPlanNum=0 AND Percent=-1 "
						+"AND QuantityQualifier="+POut.Long((long)BenefitQuantity.AgeLimit)+" AND CodeNum="+POut.Long(codeNum);
					Db.NonQ(command);
					for(int i=0;i<dtBenFlo.Rows.Count;i++) {
						long planNum=PIn.Long(dtBenFlo.Rows[i]["PlanNum"].ToString());
						long covCatNum=PIn.Long(dtBenFlo.Rows[i]["CovCatNum"].ToString());
						long timePeriod=PIn.Long(dtBenFlo.Rows[i]["TimePeriod"].ToString());
						long quantity=PIn.Long(dtBenFlo.Rows[i]["Quantity"].ToString());
						long coverageLevel=PIn.Long(dtBenFlo.Rows[i]["CoverageLevel"].ToString());
						if(DataConnection.DBtype==DatabaseType.MySql) {
							command="INSERT INTO benefit (PlanNum,PatPlanNum,CovCatNum,BenefitType,Percent,MonetaryAmt,TimePeriod,"
								+"QuantityQualifier,Quantity,CodeNum,CoverageLevel) "
								+"VALUES ("+POut.Long(planNum)+",0,"+POut.Long(covCatNum)+","+POut.Long((long)InsBenefitType.Limitations)+",-1,-1,"+POut.Long(timePeriod)
								+","+POut.Long((long)BenefitQuantity.AgeLimit)+","+POut.Long(quantity)+","+POut.Long(codeNum)+","+POut.Long(coverageLevel)+")";
							Db.NonQ(command);
						}
						else {//oracle
							command="INSERT INTO benefit (BenefitNum,PlanNum,PatPlanNum,CovCatNum,BenefitType,Percent,MonetaryAmt,TimePeriod,"
								+"QuantityQualifier,Quantity,CodeNum,CoverageLevel) "
								+"VALUES ((SELECT MAX(BenefitNum)+1 FROM benefit),"+POut.Long(planNum)+",0,"+POut.Long(covCatNum)+","+POut.Long((long)InsBenefitType.Limitations)+",-1,-1,"+POut.Long(timePeriod)
								+","+POut.Long((long)BenefitQuantity.AgeLimit)+","+POut.Long(quantity)+","+POut.Long(codeNum)+","+POut.Long(coverageLevel)+")";
							Db.NonQ(command);
						}
					}//end benefit inserts
					//Update recall triggers with new fluoride code.
					command="UPDATE recalltrigger SET CodeNum="+POut.Long(codeNum)+" WHERE CodeNum<>0 AND (CodeNum="+POut.Long(codeNumD1203)+" OR CodeNum="+POut.Long(codeNumD1204)+")";
					Db.NonQ(command);
					//Update the fluoride procedures on the recall triggers.
					command="SELECT * FROM recalltype";//Should only be a handful of results.
					DataTable dtRecallType=Db.GetTable(command);
					for(int i=0;i<dtRecallType.Rows.Count;i++) {
						string procs=PIn.String(dtRecallType.Rows[i]["Procedures"].ToString());
						string procsNew=procs.Replace("D1203","D1208").Replace("D1204","D1208");
						if(procs!=procsNew) {
							long recallTypeNum=PIn.Long(dtRecallType.Rows[i]["RecallTypeNum"].ToString());
							command="UPDATE recalltype SET Procedures='"+POut.String(procsNew)+"' WHERE RecallTypeNum="+POut.Long(recallTypeNum);
							Db.NonQ(command);
						}
					}
				}//end United States update
				command="UPDATE preference SET ValueString = '12.4.28.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To12_4_30();
		}

		///<summary>Oracle compatible: 01/09/2013</summary>
		private static void To12_4_30() {
			if(FromVersion<new Version("12.4.30.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 12.4.30");//No translation in convert script.
				string command;
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
									+"VALUES (11,'"+POut.String(procCatDescript)+"',"+itemOrder+")";//11 is DefCat.ProcCodeCats
						}
						else {//oracle
							command="INSERT INTO definition (DefNum,Category,ItemName,ItemOrder) "
									+"VALUES ((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),11,'"+POut.String(procCatDescript)+"',"+itemOrder+")";//11 is DefCat.ProcCodeCats
						}
						defNum=Db.NonQ(command,true,"DefNum","definition");
					}
					else { //The procedure code category already exists, get the existing defnum
						defNum=PIn.Long(dtDef.Rows[0][0].ToString());
					}
					string[] cdtCodesDeleted=new string[] {
						"D0360","D0362","D1203","D1204",
						"D4271","D6254","D6795","D6970",
						"D6972","D6973","D6976","D6977"
					};
					for(int i=0;i<cdtCodesDeleted.Length;i++) {
						string procCode=cdtCodesDeleted[i];
						command="SELECT CodeNum FROM procedurecode WHERE ProcCode='"+POut.String(procCode)+"'";
						DataTable dtProcCode=Db.GetTable(command);
						if(dtProcCode.Rows.Count==0) { //The procedure code does not exist in this database.
							continue;//Do not try to move it.
						}
						long codeNum=PIn.Long(dtProcCode.Rows[0][0].ToString());
						command="UPDATE procedurecode SET ProcCat="+POut.Long(defNum)+" WHERE CodeNum="+POut.Long(codeNum);
						Db.NonQ(command);
					}
				}//end United States update
				command="UPDATE preference SET ValueString = '12.4.30.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To12_4_32();
		}

		///<summary>Oracle compatible: 01/09/2013</summary>
		private static void To12_4_32() {
			if(FromVersion<new Version("12.4.32.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 12.4.32");//No translation in convert script.
				string command;
				if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
					long codeNumD1203=0;
					command="SELECT CodeNum FROM procedurecode WHERE ProcCode='D1203'";
					DataTable dtProcCode=Db.GetTable(command);
					if(dtProcCode.Rows.Count>0) {
						codeNumD1203=PIn.Long(dtProcCode.Rows[0][0].ToString());
					}
					long codeNumD1204=0;
					command="SELECT CodeNum FROM procedurecode WHERE ProcCode='D1204'";
					dtProcCode=Db.GetTable(command);
					if(dtProcCode.Rows.Count>0) {
						codeNumD1204=PIn.Long(dtProcCode.Rows[0][0].ToString());
					}
					long codeNumD1208=0;
					command="SELECT CodeNum FROM procedurecode WHERE ProcCode='D1208'";
					dtProcCode=Db.GetTable(command);
					if(dtProcCode.Rows.Count>0) {
						codeNumD1208=PIn.Long(dtProcCode.Rows[0][0].ToString());
					}
					//Change the procedures on currently scheduled appointments to show D1208 instead of D1203/4.
					command="UPDATE procedurelog "
						+"SET CodeNum="+POut.Long(codeNumD1208)+" "
						+"WHERE ProcStatus="+POut.Long((long)ProcStat.TP)+" AND CodeNum<>0 AND (CodeNum="+POut.Long(codeNumD1203)+" OR CodeNum="+POut.Long(codeNumD1204)+")";
					Db.NonQ(command);
					//Appt procs quick add, change the flouride code to D1208 instead of D1203/4.
					command="SELECT DefNum,ItemValue FROM definition WHERE Category="+POut.Long((long)DefCat.ApptProcsQuickAdd);//A short list, so just get them all.
					DataTable dtApptProcsQuickAdd=Db.GetTable(command);
					for(int i=0;i<dtApptProcsQuickAdd.Rows.Count;i++) {
						string procs=PIn.String(dtApptProcsQuickAdd.Rows[i]["ItemValue"].ToString());
						string procs1208=procs.Replace("D1203","D1208").Replace("D1204","D1208");
						if(procs==procs1208) {
							continue; //There were no D1203 and no D1204.
						}
						long defNum=PIn.Long(dtApptProcsQuickAdd.Rows[i]["DefNum"].ToString());
						command="UPDATE definition SET ItemValue='"+POut.String(procs1208)+"' WHERE DefNum="+POut.Long(defNum);
						Db.NonQ(command);
					}
					//proc buttons, change flouride  to D1208 instead of D1203/4. We don't care about possible duplicates because they are highly unlikely.
					command="UPDATE procbuttonitem SET CodeNum="+POut.Long(codeNumD1208)+" WHERE CodeNum<>0 AND (CodeNum="+POut.Long(codeNumD1203)+" OR CodeNum="+POut.Long(codeNumD1204)+")";
					Db.NonQ(command);
				}//end United States update
				command="UPDATE preference SET ValueString = '12.4.32.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To12_4_38();
		}

		///<summary>Oracle compatible: 02/23/2013</summary>
		private static void To12_4_38() {
			if(FromVersion<new Version("12.4.38.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 12.4.38");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.Oracle) {
					//skip.  Too hard for too little benefit.
				}
				else {
					//The code below synchronizes recall.DateScheduled for all patients.  There was a bug, that necessitated doing this.
					//This is essentially a copy of the code from Recalls.SynchScheduledApptFull().
					//Clear out DateScheduled column for all pats before changing
					command="UPDATE recall SET recall.DateScheduled="+POut.Date(DateTime.MinValue);
					Db.NonQ(command);
					//get all active patients with future scheduled appointments that have a procedure attached which is a recall trigger procedure
					command="SELECT DISTINCT patient.PatNum "
						+"FROM patient "
						+"INNER JOIN appointment ON appointment.PatNum=patient.PatNum AND AptDateTime>CURDATE() AND (AptStatus=1 OR AptStatus=4) "
						+"INNER JOIN procedurelog ON procedurelog.AptNum=appointment.AptNum "
						+"INNER JOIN recalltrigger ON recalltrigger.CodeNum=procedurelog.CodeNum "
						+"WHERE PatStatus=0";
					DataTable tablePats=Db.GetTable(command);
					for(int p=0;p<tablePats.Rows.Count;p++) {
						//Get table of future appointment dates with recall type for this patient, where a procedure is attached that is a recall trigger procedure
						command=@"SELECT recalltrigger.RecallTypeNum,MIN(DATE(appointment.AptDateTime)) AS AptDateTime
							FROM appointment,procedurelog,recalltrigger,recall
							WHERE appointment.AptNum=procedurelog.AptNum 
							AND appointment.PatNum="+POut.Long(PIn.Long(tablePats.Rows[p][0].ToString()))+@" 
							AND procedurelog.CodeNum=recalltrigger.CodeNum 
							AND recall.PatNum=appointment.PatNum 
							AND recalltrigger.RecallTypeNum=recall.RecallTypeNum 
							AND (appointment.AptStatus=1 "//Scheduled
							+"OR appointment.AptStatus=4) "//ASAP
							+"AND appointment.AptDateTime>CURDATE() "//early this morning
							+"GROUP BY recalltrigger.RecallTypeNum";
						DataTable tableSchedDate=Db.GetTable(command);
						//Update the recalls for this patient with DATE(AptDateTime) where there is a future appointment with recall proc on it
						for(int i=0;i<tableSchedDate.Rows.Count;i++) {
							if(tableSchedDate.Rows[i]["RecallTypeNum"].ToString()=="") {
								continue;
							}
							command=@"UPDATE recall	SET recall.DateScheduled="+POut.Date(PIn.Date(tableSchedDate.Rows[i]["AptDateTime"].ToString()))+" " 
								+"WHERE recall.RecallTypeNum="+POut.Long(PIn.Long(tableSchedDate.Rows[i]["RecallTypeNum"].ToString()))+" "
								+"AND recall.PatNum="+POut.Long(PIn.Long(tablePats.Rows[p][0].ToString()))+" ";
							Db.NonQ(command);
						}
					}
				}
				command="UPDATE preference SET ValueString = '12.4.38.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To13_1_1();
		}

		///<summary>Oracle compatible: 02/23/2013</summary>
		private static void To13_1_1() {
			if(FromVersion<new Version("13.1.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 13.1.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clockevent CHANGE AmountBonus AmountBonus double NOT NULL default -1";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clockevent MODIFY (AmountBonus NUMBER(38,8) DEFAULT '-1')";//Column is already NOT NULL.
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clockevent CHANGE AmountBonusAuto AmountBonusAuto double NOT NULL default -1";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clockevent MODIFY (AmountBonusAuto NUMBER(38,8) DEFAULT '-1')";//Column is already NOT NULL.
					Db.NonQ(command);
				}
				command="SELECT ValueString FROM preference WHERE PrefName='StatementShowNotes'";
				string prefStatementShowNotes=Db.GetScalar(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('StatementShowAdjNotes','"+prefStatementShowNotes+"')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'StatementShowAdjNotes','"+prefStatementShowNotes+"')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ProcLockingIsAllowed','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ProcLockingIsAllowed','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurelog ADD IsLocked tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurelog ADD IsLocked number(3)";
					Db.NonQ(command);
					command="UPDATE procedurelog SET IsLocked = 0 WHERE IsLocked IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog MODIFY IsLocked NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE hl7def ADD ShowDemographics tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE hl7def ADD ShowDemographics number(3)";
					Db.NonQ(command);
					command="UPDATE hl7def SET ShowDemographics = 0 WHERE ShowDemographics IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE hl7def MODIFY ShowDemographics NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE hl7def ADD ShowAppts tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE hl7def ADD ShowAppts number(3)";
					Db.NonQ(command);
					command="UPDATE hl7def SET ShowAppts = 0 WHERE ShowAppts IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE hl7def MODIFY ShowAppts NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE hl7def ADD ShowAccount tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE hl7def ADD ShowAccount number(3)";
					Db.NonQ(command);
					command="UPDATE hl7def SET ShowAccount = 0 WHERE ShowAccount IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE hl7def MODIFY ShowAccount NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS dictcustom";
					Db.NonQ(command);
					command=@"CREATE TABLE dictcustom (
						DictCustomNum bigint NOT NULL auto_increment PRIMARY KEY,
						WordText varchar(255) NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE dictcustom'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE dictcustom (
						DictCustomNum number(20) NOT NULL,
						WordText varchar2(255),
						CONSTRAINT dictcustom_DictCustomNum PRIMARY KEY (DictCustomNum)
						)";
					Db.NonQ(command);
				}
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					command="SELECT CanadianNetworkNum FROM canadiannetwork WHERE Abbrev='CSI' LIMIT 1";
					long canadianNetworkNumCSI=PIn.Long(Db.GetScalar(command));
					command="SELECT COUNT(*) FROM carrier WHERE ElectID='000118'"; //local 1030 health benefit plan
					if(Db.GetCount(command)=="0") {
						command="INSERT INTO carrier (CarrierName,Address,Address2,City,State,Zip,Phone,ElectID,NoSendElect,IsCDA,CDAnetVersion,CanadianNetworkNum,IsHidden,CanadianEncryptionMethod,CanadianSupportedTypes) VALUES "+
								"('Local 1030 Health Benefit Plan','45 McIntosh Drive','','Markham','ON','L3R 8C7','1-800-263-3564','000118','0','1','04',"+POut.Long(canadianNetworkNumCSI)+",'0',1,1944)";
						Db.NonQ(command);
					}
					command="SELECT COUNT(*) FROM carrier WHERE ElectID='000119'"; //sheet metal workers local 30 benefit plan
					if(Db.GetCount(command)=="0") {
						command="INSERT INTO carrier (CarrierName,Address,Address2,City,State,Zip,Phone,ElectID,NoSendElect,IsCDA,CDAnetVersion,CanadianNetworkNum,IsHidden,CanadianEncryptionMethod,CanadianSupportedTypes) VALUES "+
								"('Sheet Metal Workers Local 30 Benefit Plan','45 McIntosh Drive','','Markham','ON','L3R 8C7','1-800-263-3564','000119','0','1','04',"+POut.Long(canadianNetworkNumCSI)+",'0',1,1944)";
						Db.NonQ(command);
					}
					command="SELECT COUNT(*) FROM carrier WHERE ElectID='000120'"; //the building union of canada health benefit
					if(Db.GetCount(command)=="0") {
						command="INSERT INTO carrier (CarrierName,Address,Address2,City,State,Zip,Phone,ElectID,NoSendElect,IsCDA,CDAnetVersion,CanadianNetworkNum,IsHidden,CanadianEncryptionMethod,CanadianSupportedTypes) VALUES "+
								"('The Building Union of Canada Health Benefit','45 McIntosh Drive','','Markham','ON','L3R 8C7','1-800-263-3564','000120','0','1','04',"+POut.Long(canadianNetworkNumCSI)+",'0',1,1944)";
						Db.NonQ(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS wikipage";
					Db.NonQ(command);
					command=@"CREATE TABLE wikipage (
						WikiPageNum bigint NOT NULL auto_increment PRIMARY KEY,
						UserNum bigint NOT NULL,
						PageTitle varchar(255) NOT NULL,
						KeyWords varchar(255) NOT NULL,
						PageContent MEDIUMTEXT NOT NULL,
						DateTimeSaved datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						INDEX(UserNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE wikipage'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE wikipage (
						WikiPageNum number(20) NOT NULL,
						UserNum number(20) NOT NULL,
						PageTitle varchar2(255),
						KeyWords varchar2(255),
						PageContent clob,
						DateTimeSaved date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						CONSTRAINT wikipage_WikiPageNum PRIMARY KEY (WikiPageNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX wikipage_UserNum ON wikipage (UserNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS wikipagehist";
					Db.NonQ(command);
					command=@"CREATE TABLE wikipagehist (
						WikiPageNum bigint NOT NULL auto_increment PRIMARY KEY,
						UserNum bigint NOT NULL,
						PageTitle varchar(255) NOT NULL,
						PageContent MEDIUMTEXT NOT NULL,
						DateTimeSaved datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						IsDeleted tinyint NOT NULL,
						INDEX(UserNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE wikipagehist'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE wikipagehist (
						WikiPageNum number(20) NOT NULL,
						UserNum number(20) NOT NULL,
						PageTitle varchar2(255),
						PageContent clob,
						DateTimeSaved date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						IsDeleted number(3) NOT NULL,
						CONSTRAINT wikipagehist_WikiPageNum PRIMARY KEY (WikiPageNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX wikipagehist_UserNum ON wikipagehist (UserNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS emailaddress";
					Db.NonQ(command);
					command=@"CREATE TABLE emailaddress (
						EmailAddressNum bigint NOT NULL auto_increment PRIMARY KEY,
						SMTPserver varchar(255) NOT NULL,
						EmailUsername varchar(255) NOT NULL,
						EmailPassword varchar(255) NOT NULL,
						ServerPort int NOT NULL,
						UseSSL tinyint NOT NULL,
						SenderAddress varchar(255) NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE emailaddress'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE emailaddress (
						EmailAddressNum number(20) NOT NULL,
						SMTPserver varchar2(255),
						EmailUsername varchar2(255),
						EmailPassword varchar2(255),
						ServerPort number(11) NOT NULL,
						UseSSL number(3) NOT NULL,
						SenderAddress varchar2(255),
						CONSTRAINT emailaddress_EmailAddressNum PRIMARY KEY (EmailAddressNum)
						)";
					Db.NonQ(command);
				}
				//Jason: Talked with Ryan and these are going to be the only two pages in this table.  Oracle requires a PK, so manually setting PK's of 1 and 2.
				//todo: edit these 2 starting wikipages
				command="INSERT INTO wikipage (WikiPageNum,UserNum,PageTitle,KeyWords,PageContent,DateTimeSaved) VALUES("
					+"1,"//Oracle requires PK to be not null and does not have auto incrementing.  Simply hard code the PK here.
					+"0,"//no usernum set for the first 2 pages
					+"'_Master',"
					+"'',"
					+"'"+POut.String(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">
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
</html>")+"',";
				if(DataConnection.DBtype==DatabaseType.Oracle) {
					command+="SYSDATE";
				}
				else{
					command+="NOW()";
				}
				command+=")";
				Db.NonQ(command);
				//blank home page
				command="INSERT INTO wikipage (WikiPageNum,UserNum,PageTitle,KeyWords,PageContent,DateTimeSaved) VALUES("
					+"2,"//Oracle requires PK to be not null and does not have auto incrementing.  Simply hard code the PK here.
					+"0,"
					+"'Home',"	
					+"'',"
					+"'Home',";
				if(DataConnection.DBtype==DatabaseType.Oracle) {
					command+="SYSDATE";
				}
				else {
					command+="NOW()";
				}
				command+=")";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailDefaultAddressNum','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'EmailDefaultAddressNum','')";
					Db.NonQ(command);
				}
				string emailUsername=Db.GetScalar("SELECT ValueString FROM preference WHERE PrefName='EmailUsername'");
				if(emailUsername!="") {
					string emailPassword=Db.GetScalar("SELECT ValueString FROM preference WHERE PrefName='EmailPassword'");
					string senderAddress=Db.GetScalar("SELECT ValueString FROM preference WHERE PrefName='EmailSenderAddress'");
					int serverPort=PIn.Int(Db.GetScalar("SELECT ValueString FROM preference WHERE PrefName='EmailPort'").ToString());
					string smtpServer=Db.GetScalar("SELECT ValueString FROM preference WHERE PrefName='EmailSMTPServer'");
					bool useSSL=PIn.Bool(Db.GetScalar("SELECT ValueString FROM preference WHERE PrefName='EmailUseSSL'"));
					command="INSERT INTO emailaddress(EmailPassword,EmailUsername,SenderAddress,ServerPort,SMTPServer,UseSSL) "
						+"VALUES('"+POut.String(emailPassword)+"','"+POut.String(emailUsername)+"','"+POut.String(senderAddress)+"',"
						+POut.Int(serverPort)+",'"+POut.String(smtpServer)+"',"+POut.Bool(useSSL)+")";
					long defaultEmailAddressNum=Db.NonQ(command,true);
					command="UPDATE preference SET ValueString = "+POut.Long(defaultEmailAddressNum)+" WHERE PrefName = 'EmailDefaultAddressNum'";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD EmailAddressNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE clinic ADD INDEX (EmailAddressNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD EmailAddressNum number(20)";
					Db.NonQ(command);
					command="UPDATE clinic SET EmailAddressNum = 0 WHERE EmailAddressNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE clinic MODIFY EmailAddressNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX clinic_EmailAddressNum ON clinic (EmailAddressNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE toothgriddef ADD SheetFieldDefNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE toothgriddef ADD INDEX (SheetFieldDefNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE toothgriddef ADD SheetFieldDefNum number(20)";
					Db.NonQ(command);
					command="UPDATE toothgriddef SET SheetFieldDefNum = 0 WHERE SheetFieldDefNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE toothgriddef MODIFY SheetFieldDefNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX toothgriddef_SheetFieldDefNum ON toothgriddef (SheetFieldDefNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ElectronicRxDateStartedUsing131',"+DbHelper.Now()+")";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ElectronicRxDateStartedUsing131',"+DbHelper.Now()+")";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE rxpat ADD NewCropGuid varchar(40) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE rxpat ADD NewCropGuid varchar2(40)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE payment CHANGE PayNote PayNote TEXT NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE payment MODIFY (PayNote varchar2(4000) NOT NULL)";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '13.1.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To13_1_3();
		}

		///<summary>Oracle compatible: 02/23/2013</summary>
		private static void To13_1_3() {
			if(FromVersion<new Version("13.1.3.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 13.1.3");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('SpellCheckIsEnabled','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SpellCheckIsEnabled','1')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '13.1.3.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To13_1_14();
		}

		///<summary>Oracle compatible: 05/16/2013</summary>
		private static void To13_1_14() {
			if(FromVersion<new Version("13.1.14.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 13.1.14");//No translation in convert script.
				string command;
				command="UPDATE preference SET ValueString = "+DbHelper.Now()+" WHERE PrefName = 'ElectronicRxDateStartedUsing131'";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '13.1.14.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To13_1_19();
		}

		///<summary>Oracle compatible: 05/16/2013</summary>
		private static void To13_1_19() {
			if(FromVersion<new Version("13.1.19.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 13.1.19");//No translation in convert script.
				string command;
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE adjustment ADD INDEX (ProcNum)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX adjustment_ProcNum ON adjustment (ProcNum)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//ex is needed, or exception won't get caught.
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS wikilistheaderwidth";
					Db.NonQ(command);
					command=@"CREATE TABLE wikilistheaderwidth (
						WikiListHeaderWidthNum bigint NOT NULL auto_increment PRIMARY KEY,
						ListName varchar(255) NOT NULL,
						ColName varchar(255) NOT NULL,
						ColWidth int NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					//WikiLists Not Supported in Oracle. but we're still adding this table here for consistency of the schema.  Also, we might turn on this feature for Oracle some day.
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE wikilistheaderwidth'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE wikilistheaderwidth (
						WikiListHeaderWidthNum number(20) NOT NULL,
						ListName varchar2(255),
						ColName varchar2(255),
						ColWidth number(11) NOT NULL,
						CONSTRAINT wikilistheaderwidth_WikiListHe PRIMARY KEY (WikiListHeaderWidthNum)
						)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT ProgramNum FROM program WHERE ProgName='eClinicalWorks'";
					int programNum=PIn.Int(Db.GetScalar(command));
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
							+") VALUES("
							+"'"+POut.Long(programNum)+"', "
							+"'MedicalPanelUrl', "
							+"'')";
					Db.NonQ(command);
				}
				else {//oracle
					//eCW will never use Oracle.
				}
				command="UPDATE preference SET ValueString = '13.1.19.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To13_1_32();
		}

		private static void To13_1_32() {
			if(FromVersion<new Version("13.1.32.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 13.1.32");//No translation in convert script.
				string command;
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE medicationpat ADD INDEX (PatNum)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX medicationpat_PatNum ON medicationpat (PatNum)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//ex is needed, or exception won't get caught.
				}
				command="UPDATE preference SET ValueString = '13.1.32.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To13_1_33();
		}

		private static void To13_1_33() {
			if(FromVersion<new Version("13.1.33.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 13.1.33");//No translation in convert script.
				string command;
				bool isNewCropEnabled=false;
				command="SELECT ValueString FROM preference WHERE PrefName='NewCropAccountId'";
				if(Db.GetScalar(command)!="") {
					isNewCropEnabled=true;
				}
				//Insert NewCrop Bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
		          +") VALUES("
		          +"'NewCrop',"
		          +"'NewCrop electronic Rx',"
		          +"'"+POut.Bool(isNewCropEnabled)+"',"//The enabled status of the NewCrop bridge is synchronous with the NewCropAccountId preference. If disabled later, then the NewCropAccountId will also be cleared.
		          +"'',"//No program path.
		          +"'',"//No command line, because not program path.
		          +"'NewCrop only works for the United States and its territories, including Puerto Rico.  Disable if you are located elsewhere.')";
					Db.NonQ(command);//No programproperties, because we only need the Enabled checkbox.
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
		          +") VALUES("
		          +"(SELECT MAX(ProgramNum)+1 FROM program),"
		          +"'NewCrop',"
		          +"'NewCrop electronic Rx',"
		          +"'"+POut.Bool(isNewCropEnabled)+"',"//The enabled status of the NewCrop bridge is synchronous with the NewCropAccountId preference. If disabled later, then the NewCropAccountId will also be cleared.
		          +"'',"//No program path.
		          +"'',"//No command line, because not program path.
		          +"'NewCrop only works for the United States and its territories, including Puerto Rico.  Disable if you are located elsewhere.')";
					Db.NonQ(command);//No programproperties, because we only need the Enabled checkbox.
				}//end NewCrop bridge
				command="UPDATE preference SET ValueString = '13.1.33.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To13_1_38();
		}

		private static void To13_1_38() {
			if(FromVersion<new Version("13.1.38.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 13.1.38");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('BillingElectSaveHistory','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'BillingElectSaveHistory','0')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '13.1.38.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To13_2_1();
		}


	}
}








				
