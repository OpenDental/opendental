using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Security.Cryptography;
using CodeBase;

namespace OpenDental {
	public partial class FormEcwDiag:FormODBase {
		private string connString;
		private string username="ecwUser";
		private string password="l69Rr4Rmj4CjiCTLxrIblg==";//encrypted
		private string server;
		private string port;
		private StringBuilder arbitraryStringName=new StringBuilder();

		public FormEcwDiag() {
			InitializeComponent();
			InitializeLayoutManager();
			server=ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.eClinicalWorks),"eCWServer");//this property will not exist if using Oracle, eCW will never use Oracle
			port=ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.eClinicalWorks),"eCWPort");//this property will not exist if using Oracle, eCW will never use Oracle
			buildConnectionString();
			Lan.F(this);
		}

		private void FormEcwDiag_Load(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			Application.DoEvents();
			VerifyECW();
			Cursor=Cursors.Default;
			Application.DoEvents();
		}

		///<summary>Used to construct a default construction string.</summary>
		private void buildConnectionString() {
			connString=
				"Server="+server+";"
				+"Port="+port+";"//although this does seem to cause a bug in Mono.  We will revisit this bug if needed to exclude the port option only for Mono.
				+"Database=mobiledoc;"//ecwMaster;"
				//+"Connect Timeout=20;"
				+"User ID="+username+";"
				+"Password="+CodeBase.MiscUtils.Decrypt(password)+";"
				+"SslMode=none;"
				+"CharSet=utf8;"
				+"Treat Tiny As Boolean=false;"
				+"Allow User Variables=true;"
				+"Default Command Timeout=300;"//default is 30seconds
				+"Pooling=false"
				;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butRunCheck_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			Application.DoEvents();
			VerifyECW();
			Cursor=Cursors.Default;
			Application.DoEvents();
		}

		///<summary>Surround with wait cursor.</summary>
		private void VerifyECW() {
			//buildConnectionString();
			bool verbose=checkShow.Checked;
			StringBuilder strB=new StringBuilder();
			strB.Append('-',90);
			textLog.Text=DateTime.Now.ToString()+strB.ToString()+"\r\n";
			Application.DoEvents();
			//--------eCW Function Tests Below This Line-------
			try {
				MySql.Data.MySqlClient.MySqlHelper.ExecuteDataRow(connString,"SELECT VERSION();");//meaningless query to test connection.
			}
			catch(Exception ex) {
				ex.DoNothing();
				textLog.Text+="Cannot detect eCW server named \""+server+"\".\r\n";
				Cursor=Cursors.Default;
				return;
			}
			HL7Verification(verbose);//composite check
			Application.DoEvents();
			textLog.Text+=checkDentalVisitTypes(verbose);
			Application.DoEvents();

			//textLog.Text+=appointmentTriggersForHl7(verbose);
			//Application.DoEvents();
			//textLog.Text+=Test1(verbose);
			//Application.DoEvents();
			textLog.Text+="\r\nDone.";
		}

		private string appointmentTriggersForHl7(bool verbose) {
			string retVal="";
			DataTable appTriggers = new DataTable();
			try {
				appTriggers=MySqlHelper.ExecuteDataset(connString,"SELECT * FROM pmitemkeys WHERE name LIKE '%Filter_for_%';").Tables[0];
			}
			catch(Exception ex) {
				return ex.Message+"\r\n";
			}
			foreach(DataRow trigger in appTriggers.Rows) {
				if(trigger["value"].ToString()!="no") {
					if(verbose) {
						retVal+=trigger["name"].ToString().Split('_')[3]+" messages are configured to be sent based on "+trigger["name"].ToString().Split('_')[0]+" filter.\r\n";
					}
					continue;
				}
				if(trigger["value"].ToString()=="no"&&verbose) {
					retVal+=trigger["name"].ToString().Split('_')[3]+" messages are sent for any "+trigger["name"].ToString().Split('_')[0]+".\r\n";
					continue;
				}
			}
			if(retVal!="") {
				string header="\r\n";
				header+="   HL7 Message Triggers\r\n";
				header+="".PadRight(90,'*')+"\r\n";
				retVal=header+retVal;
			}
			return retVal;
		}

		private void HL7Verification(bool verbose) {
			List<int> hl7InterfaceIDs=ColumnToListHelper(MySqlHelper.ExecuteDataset(connString,"SELECT DISTINCT InterfaceId FROM hl7segment_details;").Tables[0],"InterfaceId");
			List<int> interfaceErrorCount=new List<int>(hl7InterfaceIDs.Count);
			List<string> interfaceErrorLogs=new List<string>(hl7InterfaceIDs.Count);//Cache error logs for each interface until we determine which interface to report on.
			for(int ifaceIndex=0;ifaceIndex<hl7InterfaceIDs.Count;ifaceIndex++) {//validate one interface at a time.
				int interfaceID=hl7InterfaceIDs[ifaceIndex];
				interfaceErrorLogs.Add("");//start each interface with a blank error log
				interfaceErrorCount.Add(0);//start each interface with 0 error count
				int errorsFromCurMessage=0;
				//Itterate through and validate all messages defined on this interface.
				List<int> hl7MessageIDs=ColumnToListHelper(MySqlHelper.ExecuteDataset(connString,"SELECT DISTINCT Messageid FROM hl7segment_details WHERE InterfaceID="+interfaceID+";").Tables[0],"Messageid");
				foreach(int messageID in hl7MessageIDs) {//2, in our sample
					string messageType=MySqlHelper.ExecuteDataRow(connString,"SELECT DISTINCT MessageType FROM hl7message_types WHERE MessageTypeId="+messageID)["MessageType"].ToString();
					//Validate each message individually if needed.
					errorsFromCurMessage=0;
					if(messageType.Contains("ADT")) {
						interfaceErrorLogs[ifaceIndex]+=verifyAsADTMessage(interfaceID,messageID,out errorsFromCurMessage,verbose);
					}
					else if(messageType.Contains("SIU")) {
						interfaceErrorLogs[ifaceIndex]+=verifyAsSIUMessage(interfaceID,messageID,out errorsFromCurMessage,verbose);
					}
				}
				interfaceErrorCount[ifaceIndex]+=errorsFromCurMessage;
			}//end foreach interface
			int leastErrorIndex=0;
			for(int i=0;i<hl7InterfaceIDs.Count;i++) {
				if(interfaceErrorCount[i]<interfaceErrorCount[leastErrorIndex]) {
					leastErrorIndex=i;
				}
			}
			if(interfaceErrorLogs[leastErrorIndex]!="" || verbose) {
				textLog.Text+="\r\n";
				textLog.Text+="   HL7 Messages\r\n";
				textLog.Text+="".PadRight(90,'*')+"\r\n";
			}
			if(verbose) {
				textLog.Text+="HL7 Interface "+hl7InterfaceIDs[leastErrorIndex]+" had "+(interfaceErrorCount[leastErrorIndex]==0?"no":"the following")+" issues.\r\n";
			}
			textLog.Text+=interfaceErrorLogs[leastErrorIndex];
			Application.DoEvents();
		}

		private string verifyAsADTMessage(int interfaceID,int messageID,out int errors,bool verbose) {
			string retVal="";
			errors=0;
			bool validMessage=true;
			List<string> segmentsContained=new List<string>();
			DataTable hl7Segments=MySqlHelper.ExecuteDataset(connString,"SELECT SegmentData FROM hl7segment_details WHERE InterfaceID="+interfaceID+" AND Messageid="+messageID+";").Tables[0];
			//validate segments based on content
			foreach(DataRow segment in hl7Segments.Rows) {
				string[] segmentValues=segment["SegmentData"].ToString().Split('|');
				segmentsContained.Add(segmentValues[0]);//used later to validate existance of segments.
				switch(segmentValues[0]) {
					case "EVN":
						//We ignore this field
						continue;
					case "PID":
						if(segmentValues[2]!="{PID}") {
							retVal+="ADT HL7 message is not sending eCW's internal patient number in field PID.02\r\n";
							errors++;
							validMessage=false;
						}
						if(segmentValues[4]!="{CONTNO}") {
							retVal+="ADT HL7 message is not sending eCW's account number in field PID.04\r\n";
							errors++;
							validMessage=false;
						}
						continue;
					case "GT1":
						if(segmentValues[2]!="{GRID}") {
							retVal+="ADT HL7 message is not sending guarantor's id number in field GT1.02\r\n";
							errors++;
							validMessage=false;
						}
						if(segmentValues[3]!="{GRLN}^{GRFN}^{GRMN}") {
							retVal+="ADT HL7 message is not sending eCW's guarantor's name in field GT1.03\r\n";
							errors++;
							validMessage=false;
						}
						if(segmentValues[11]!="{GRREL}") {
							retVal+="ADT HL7 message is not sending guarantor's relationship to patient in field GT1.11\r\n";
							errors++;
							validMessage=false;
						}
						continue;
					default:
						continue;
				}
			}
			//Validate existance of segments
			//if(!segmentsContained.Contains("EVN")) { //We ignore this segment
			//  retVal+="No EVN segment found in ADT HL7 message.\r\n";
			//  errors+=3;//No segment +2 sub errors
			//  validMessage=false;
			//}
			if(!segmentsContained.Contains("PID")) {
				retVal+="No PID segment found in ADT HL7 message.\r\n";
				errors+=3;//No segment +2 sub errors
				validMessage=false;
			}
			if(!segmentsContained.Contains("GT1") && verbose) {
				retVal+="No GT1 segment found in ADT HL7 message. Guarantors for new patients will always be set to self.\r\n";
				validMessage=false;
			}
			//If everything above checks out return a success message
			if(validMessage && verbose) {
				retVal+="Found properly formed ADT HL7 message definition.\r\n";
			}
			return retVal;
		}

		private string verifyAsSIUMessage(int interfaceID,int messageID,out int errors,bool verbose) {
			string retVal="";
			errors=0;
			bool validMessage=true;
			List<string> segmentsContained=new List<string>();
			DataTable hl7Segments=MySqlHelper.ExecuteDataset(connString,"SELECT SegmentData FROM hl7segment_details WHERE InterfaceID="+interfaceID+" AND Messageid="+messageID+";").Tables[0];
			//validate segments based on content
			foreach(DataRow segment in hl7Segments.Rows) {
				string[] segmentFields=segment["SegmentData"].ToString().Split('|');
				segmentsContained.Add(segmentFields[0]);//used later to validate existance of segments.
				switch(segmentFields[0]) {
					case "MSH":
						//validation?
						continue;
					case "SCH":
						if(segmentFields[2]!="{ENCID}") {//eCW's documentation is wrong. SCH.01 is not used as appointment num, instead SCH.02 is used for appointment num.
							retVal+="SIU HL7 message is not sending visit number in field SCH.01\r\n";
							errors++;
							validMessage=false;
						}
						if(segmentFields[7]!="{ENCREASON}") {
							retVal+="SIU HL7 message is not sending visit reason in field SCH.07\r\n";
							errors++;
							validMessage=false;
						}
						if(segmentFields[8]!="{VISITTYPE}") {
							retVal+="SIU HL7 message is not sending visit type in field SCH.08\r\n";
							errors++;
							validMessage=false;
						}
						//if(false) {//segmentFields[9]!="{???}") { //Don't know what this should look like when properly configured. TODO
						//	retVal+="SIU HL7 message is not sending appointment duration in minutes in field SCH.09\r\n";
						//	errors++;
						//	validMessage=false;
						//}
						//if(false) {//segmentFields[10]!="{???}") { //Don't know what this should look like when properly configured. TODO
						//	retVal+="SIU HL7 message is not sending appointment duration units in field SCH.10\r\n";
						//	errors++;
						//	validMessage=false;
						//}
						string[] SCH11=segmentFields[11].Split('^');
						//if(false) {//SCH11[2]!="{???}") { //Don't 
						//	retVal+="SIU HL7 message is not sending appointment duration in field SCH.11.02\r\n";
						//	errors++;
						//	validMessage=false;
						//}
						if(SCH11[3]!="{ENCSDATETIME}") {
							retVal+="SIU HL7 message is not sending appointment start time in field SCH.11.03\r\n";
							errors++;
							validMessage=false;
						}
						if(SCH11[4]!="{ENCEDATETIME}") {
							retVal+="SIU HL7 message is not sending appointment end time in field SCH.11.04\r\n";
							errors++;
							validMessage=false;
						}
						//if(segmentFields[25]!="{STATUS}") {//according to documentation, we need this, but actually we never try to reference it.
						//  retVal+="SIU HL7 message is not sending visit status in field SCH.25\r\n";
						//  errors++;
						//  validMessage=false;
						//}
						continue;
					case "PID":
						if(segmentFields[2]!="{PID}") {
							retVal+="SIU HL7 message is not sending eCW's internal patient number in field PID.02\r\n";
							errors++;
							validMessage=false;
						}
						if(segmentFields[4]!="{CONTNO}" && !Programs.UsingEcwTightOrFullMode()) {
							retVal+="SIU HL7 message is not sending eCW's account number in field PID.04\r\n";
							errors++;
							validMessage=false;
						}
						if(segmentFields[5]!="{PLN}^{PFN}^{PMN}") {
							retVal+="SIU HL7 message is not sending patient's name correctly in field PID.05\r\n";
							errors++;
							validMessage=false;
						}
						if(segmentFields[7]!="{PDOB}") {
							retVal+="SIU HL7 message is not sending patient's date of birth in field PID.07\r\n";
							errors++;
							validMessage=false;
						}
						if(segmentFields[8]!="{PSEX}") {
							retVal+="SIU HL7 message is not sending patient's gender in field PID.08\r\n";
							errors++;
							validMessage=false;
						}
						//No checking of optional fields.
						continue;
					case "PV1":
						if(segmentFields[7]!="{ODDRID}^{ODLN}^{ODFN}") {
							retVal+="SIU HL7 message is not sending provider id in field PV1.07\r\n";
							errors++;
							validMessage=false;
						}
						continue;
					case "AIG":
						if(segmentFields[3]!="{RSDRID}^{RSLN}^{RSFN}") {
							retVal+="SIU HL7 message is not sending provider/resource id in field AIG.03\r\n";
							errors++;
							validMessage=false;
						}
						continue;
					default:
						continue;
				}
			}
			//Validate existance of segments
			if(!segmentsContained.Contains("SCH")) {
				retVal+="No SCH segment found in SIU HL7 message.\r\n";
				errors+=7;//no segment plus 6 sub errors.
				validMessage=false;
			}
			if(!segmentsContained.Contains("PID")) {
				retVal+="No PID segment found in SIU HL7 message.\r\n";
				if(!Programs.UsingEcwTightOrFullMode()) {
					errors++;//to account for not sending eCW's account number
				}
				errors+=5;//no segment plus 4 sub errors.
				validMessage=false;
			}
			if(!segmentsContained.Contains("AIG") && !segmentsContained.Contains("PV1")) {
				retVal+="No AIG or PV1 segments found in SIU HL7 message. Appointments will use patient's default primary provider.\r\n";//ecwSIU.cs sets this when in-processing SIU message.
				validMessage=false;
			}
			//If everything above checks out return a success message
			if(validMessage && verbose) {
				retVal+="Found properly formed SIU HL7 message definition.\r\n";
			}
			return retVal;
		}

		/*private string verifyAsDFTMessage(int interfaceID,int messageID,bool verbose) {
			string retVal="";
			bool validMessage=true;
			List<string> segmentsContained=new List<string>();
			DataTable hl7Segments=MySqlHelper.ExecuteDataset(connString,"SELECT SegmentData FROM hl7segment_details WHERE InterfaceID="+interfaceID+" AND Messageid="+messageID+";").Tables[0];
			//validate segments based on content
			foreach(DataRow segment in hl7Segments.Rows) {
				string[] segmentValues=segment["SegmentData"].ToString().Split('|');
				segmentsContained.Add(segmentValues[0]);//used later to validate existance of segments.
				switch(segmentValues[0]) {
					case "PID":
						if(segmentValues[2]!="{CONTNO}") {
							retVal+="DFT HL7 message is not sending eCW's account number in field PID.2\r\n";
							validMessage=false;
						}
						if(segmentValues[3]!="{PID}") {
							retVal+="DFT HL7 message is not sending eCW's account number in field PID.4\r\n";
							validMessage=false;
						}
						continue;
					case "PV1":
						//TODO: Need example of a valid DFT message to validate this segment.
						continue;
					case "ZX1":
						//TODO: Need example of a valid DFT message to validate this segment.
						continue;
					default:
						continue;
				}
			}
			//Validate existance of segments
			if(!segmentsContained.Contains("PID")) {
				retVal+="No PID segment found in DFT HL7 message.\r\n";
				validMessage=false;
			}
			if(!segmentsContained.Contains("PV1")) {
				retVal+="No PV1 segment found in DFT HL7 message.\r\n";
				validMessage=false;
			}
			if(!segmentsContained.Contains("FT1")) {
				retVal+="No FT1 segment found in DFT HL7 message.\r\n";
				validMessage=false;
			}
			if(!segmentsContained.Contains("ZX1")) {
				retVal+="No ZX1 segment found in DFT HL7 message.\r\n";
				validMessage=false;
			}
			//If everything above checks out return a success message
			if(validMessage && verbose) {
				retVal+="Found properly formed DFT HL7 message definition.\r\n";
			}
			return retVal;
		}*/

		private string checkDentalVisitTypes(bool verbose) {
			string retval = "";
			DataTable tableVisitCodesJOINpmCodes = new DataTable();
			try {
				tableVisitCodesJOINpmCodes=MySqlHelper.ExecuteDataset(connString,"SELECT * FROM visitcodes LEFT OUTER JOIN pmcodes ON visitcodes.Name=pmcodes.ecwcode WHERE dentalvisit=1;").Tables[0];
			}
			catch(Exception ex) {
				return ex.Message+"\r\n";
			}
			//left outer join should show null in ecwcode if there is no corresponding pmcode for the visitcode.
			if(verbose || tableVisitCodesJOINpmCodes.Select("ecwcode is null").Length>0 || tableVisitCodesJOINpmCodes.Rows.Count==0) {
				retval+="\r\n";
				retval+="   Dental Visit Codes\r\n";
				retval+="".PadRight(90,'*')+"\r\n";
				foreach(DataRow dRow in tableVisitCodesJOINpmCodes.Rows) {
					if(dRow["ecwcode"].ToString()=="") {
						retval+="Dental visit code named \""+dRow["Description"].ToString()+"\" found but not set up.\r\n";
					}
					else if(verbose) {
						retval+="Dental visit code named \""+dRow["Description"].ToString()+"\" found.\r\n";
					}
				}
				if(tableVisitCodesJOINpmCodes.Rows.Count==0) {
					retval+="No dental visit codes found or set up.\r\n";
				}
			}
			return retval;
		}

		private List<int> ColumnToListHelper(DataTable dataTable,string colName) {
			List<int> retVal = new List<int>();
			foreach(DataRow dRow in dataTable.Rows) {
				retVal.Add((int)dRow[colName]);
			}
			return retVal;
		}

		private string TestTemplate(bool verbose) {
			StringBuilder retVal=new StringBuilder();
			bool failed=true;
			string command="SHOW FULL TABLES WHERE Table_type='BASE TABLE'";//Tables, not views.  Does not work in MySQL 4.1, however we test for MySQL version >= 5.0 in PrefL.
			DataTable qResult=MySql.Data.MySqlClient.MySqlHelper.ExecuteDataset(connString,command).Tables[0];
			//or MySql.Data.MySqlClient.MySqlDataReader mtDataReader;


			//Place check code here. Also, use a reader, table or both as shown above.

			if(verbose||failed) {
				retVal.Clear();
				retVal.Append("HL7 message definitions are not formed properly. //TODO: maybe add some more specific details here.");
			}
			return retVal.ToString();
		}

		private void checkShow_KeyPress(object sender,KeyPressEventArgs e) {
			KeysConverter kc=new KeysConverter();
			try {
				arbitraryStringName.Append(e.KeyChar);
			}
			catch(Exception ex) {
				ex.DoNothing();
				//fail VERY silently. Mwa Ha Ha.
			}
			if(arbitraryStringName.ToString().EndsWith("X")) {//Clear string if (upper case) 'X' is pressed.
				arbitraryStringName.Clear();
			}
			if(arbitraryStringName.ToString()=="open" || arbitraryStringName.ToString()=="There is no cow level") {
				using FormEcwDiagAdv FormECWA=new FormEcwDiagAdv();
				FormECWA.ShowDialog();
			}

		}

		//private string Test1(bool verbose) {
		//  StringBuilder retVal=new StringBuilder();
		//  bool failed=true;
		//  MySql.Data.MySqlClient.MySqlDataReader myDrTables;
		//  MySql.Data.MySqlClient.MySqlDataReader myDrFields;
		//  try {
		//    myDrTables=MySql.Data.MySqlClient.MySqlHelper.ExecuteReader(connString,"SHOW TABLES"+(textTables.Text==""?"":" LIKE '%"+POut.String(textTables.Text)+"%'"));
		//    if(myDrTables.HasRows) {
		//      failed=false;
		//    }
		//    while(myDrTables.Read()) {
		//      if(myDrTables.GetValue(0).ToString().Contains("copy")//copy is a reserved word
		//        ) {
		//        continue;
		//      }
		//      try {
		//        if(textValue.Text!="" && MySql.Data.MySqlClient.MySqlHelper.ExecuteDataset(connString,queryTableByValue(myDrTables.GetValue(0).ToString())).Tables[0].Rows.Count==0) {
		//          continue;//skip tables with no rows that match the match by value.
		//        }
		//      retVal.Append("**********************************************************\r\n"
		//                   +"**             "+myDrTables.GetValue(0).ToString()+" Rows:"+MySql.Data.MySqlClient.MySqlHelper.ExecuteDataset(connString,"SELECT COUNT(*) AS numRows FROM "+myDrTables.GetValue(0).ToString()).Tables[0].Rows[0]["numRows"].ToString()+"\r\n"
		//                   +"**********************************************************\r\n");
		//      myDrFields=MySql.Data.MySqlClient.MySqlHelper.ExecuteReader(connString,queryTableByValue(myDrTables.GetValue(0).ToString()));//"SELECT * FROM `"+POut.String(myDrTables.GetValue(0).ToString())+"` LIMIT 100");
		//        int row=0;
		//        while(myDrFields.Read()) {
		//          retVal.Append("Row "+row+":  ");
		//          row++;
		//          for(int i=0;i<myDrFields.FieldCount;i++) {
		//            retVal.Append(myDrFields.GetValue(i).ToString()+"  ||  ");
		//          }
		//          retVal.Append("\r\n");
		//        }
		//      }
		//      catch(Exception ex2) {
		//        retVal.Append("Error accesing table:"+myDrTables.GetValue(0).ToString()+"\r\nError Message:"+ex2.Message+"\r\n");
		//      }
		//    }
		//  }
		//  catch(Exception ex) {
		//		if(ODBuild.IsDebug()) {
		//			retVal.Append(ex.Message+"\r\n");
		//			return retVal.ToString();
		//		}
		//  }
		//  //  if(myDrTables.HasRows) {
		//  //    failed=false;
		//  //  }
		//  //  while(myDrTables.Read()){
		//  //    for(int i=0;i<myDrTables.FieldCount;i++){
		//  //      retVal+=myDrTables.GetValue(i).ToString()+"  :  ";
		//  //    }
		//  //    retVal+="\r\n";
		//  //  }
		//  //  //DataTable myDT=MySql.Data.MySqlClient.MySqlHelper.ExecuteDataset(connString,"SELECT * FROM userod").Tables[0];
		//  //  //retVal+="UserOD Table Contents (abridged):\r\n";
		//  //  //for(int i=0;i<myDT.Rows.Count;i++) {
		//  //  //  retVal+="Username :"+myDT.Rows[i]["UserName"]+"\r\n     EmployeeNum :"+myDT.Rows[i]["EmployeeNum"]+"\r\n";
		//  //  //}
		//  //if(verbose || failed) {
		//  //  return retVal+"Test1 has "+(failed?"failed. Please do these things or tell your \"eCW Commander\", or whatever they are called, about this.":"passed.")+"\r\n";
		//  //}
		//  return retVal.ToString();
		//}

		/////<summary><para>SELECT * FROM &lt;tablename&gt; WHERE &lt;Any column contains textValue.text&gt;</para>
		/////<para>_</para>
		/////<para>Used for searching all fields of a table for a specified value</para></summary>
		///// <param name="tableName"></param>
		///// <returns>SQL query that selects applicable rows.</returns>
		//private string queryTableByValue(string tableName) {
		//  StringBuilder retVal=new StringBuilder();
		//  retVal.Append("SELECT * FROM "+tableName);
		//  if(textValue.Text=="") {
		//    return retVal.ToString()+" LIMIT 100;";
		//  }
		//  DataTable cols=MySql.Data.MySqlClient.MySqlHelper.ExecuteDataset(connString,"SHOW COLUMNS FROM "+tableName).Tables[0];
		//  retVal.Append(" WHERE ");
		//  for(int i=0;i<cols.Rows.Count;i++) {
		//    if(i!=0) {
		//      retVal.Append("OR");
		//    }
		//    retVal.Append(" `"+cols.Rows[i]["Field"].ToString()+"` LIKE '%"+POut.String(textValue.Text)+"%' ");
		//  }
		//  return retVal.ToString()+" LIMIT 100;";
		//}


	}
}