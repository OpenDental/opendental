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
		private string _connectionString;
		private string _username="ecwUser";
		private string _password="l69Rr4Rmj4CjiCTLxrIblg==";//encrypted
		private string _server;
		private string _port;
		private StringBuilder _stringBuilder=new StringBuilder();
		private int _countErrors;

		public FormEcwDiag() {
			InitializeComponent();
			InitializeLayoutManager();
			_server=ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.eClinicalWorks),"eCWServer");//this property will not exist if using Oracle, eCW will never use Oracle
			_port=ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.eClinicalWorks),"eCWPort");//this property will not exist if using Oracle, eCW will never use Oracle
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
			_connectionString=
				"Server="+_server+";"
				+"Port="+_port+";"//although this does seem to cause a bug in Mono.  We will revisit this bug if needed to exclude the port option only for Mono.
				+"Database=mobiledoc;"//ecwMaster;"
				//+"Connect Timeout=20;"
				+"User ID="+_username+";"
				+"Password="+CodeBase.MiscUtils.Decrypt(_password)+";"
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
			StringBuilder stringBuilder=new StringBuilder();
			stringBuilder.Append('-',90);
			textLog.Text=DateTime.Now.ToString()+stringBuilder.ToString()+"\r\n";
			Application.DoEvents();
			//--------eCW Function Tests Below This Line-------
			try {
				MySql.Data.MySqlClient.MySqlHelper.ExecuteDataRow(_connectionString,"SELECT VERSION();");//meaningless query to test connection.
			}
			catch(Exception ex) {
				ex.DoNothing();
				textLog.Text+="Cannot detect eCW server named \""+_server+"\".\r\n";
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
			string msg="";
			DataTable table = new DataTable();
			try {
				table=MySqlHelper.ExecuteDataset(_connectionString,"SELECT * FROM pmitemkeys WHERE name LIKE '%Filter_for_%';").Tables[0];
			}
			catch(Exception ex) {
				return ex.Message+"\r\n";
			}
			for(int r=0;r<table.Rows.Count;r++) {
				if(table.Rows[r]["value"].ToString()!="no") {
					if(verbose) {
						msg+=table.Rows[r]["name"].ToString().Split('_')[3]+" messages are configured to be sent based on "+table.Rows[r]["name"].ToString().Split('_')[0]+" filter.\r\n";
					}
					continue;
				}
				if(table.Rows[r]["value"].ToString()=="no"&&verbose) {
					msg+=table.Rows[r]["name"].ToString().Split('_')[3]+" messages are sent for any "+table.Rows[r]["name"].ToString().Split('_')[0]+".\r\n";
					continue;
				}
			}
			if(msg!="") {
				string header="\r\n";
				header+="   HL7 Message Triggers\r\n";
				header+="".PadRight(90,'*')+"\r\n";
				msg=header+msg;
			}
			return msg;
		}

		private void HL7Verification(bool verbose) {
			List<int> listHL7InterfaceIDs=ColumnToListHelper(MySqlHelper.ExecuteDataset(_connectionString,"SELECT DISTINCT InterfaceId FROM hl7segment_details;").Tables[0],"InterfaceId");
			List<int> listInterfaceErrorCounts=new List<int>(listHL7InterfaceIDs.Count);
			List<string> listInterfaceErrorLogs=new List<string>(listHL7InterfaceIDs.Count);//Cache error logs for each interface until we determine which interface to report on.
			for(int ifaceIndex=0;ifaceIndex<listHL7InterfaceIDs.Count;ifaceIndex++) {//validate one interface at a time.
				int interfaceID=listHL7InterfaceIDs[ifaceIndex];
				listInterfaceErrorLogs.Add("");//start each interface with a blank error log
				listInterfaceErrorCounts.Add(0);//start each interface with 0 error count
				_countErrors=0;
				//Iterate through and validate all messages defined on this interface.
				List<int> listHL7MessageIDs=ColumnToListHelper(MySqlHelper.ExecuteDataset(_connectionString,"SELECT DISTINCT Messageid FROM hl7segment_details WHERE InterfaceID="+interfaceID+";").Tables[0],"Messageid");
				for(int i=0;i<listHL7MessageIDs.Count;i++) {  //2, in our sample
					string messageType=MySqlHelper.ExecuteDataRow(_connectionString,"SELECT DISTINCT MessageType FROM hl7message_types WHERE MessageTypeId="+listHL7MessageIDs[i])["MessageType"].ToString();
					//Validate each message individually if needed.
					_countErrors=0;
					if(messageType.Contains("ADT")) {
						listInterfaceErrorLogs[ifaceIndex]+=verifyAsADTMessage(interfaceID,listHL7MessageIDs[i],verbose);
					}
					else if(messageType.Contains("SIU")) {
						listInterfaceErrorLogs[ifaceIndex]+=verifyAsSIUMessage(interfaceID,listHL7MessageIDs[i],verbose);
					}
				}
				listInterfaceErrorCounts[ifaceIndex]+=_countErrors;
			}//end for
			int leastErrorIndex=0;
			for(int i=0;i<listHL7InterfaceIDs.Count;i++) {
				if(listInterfaceErrorCounts[i]<listInterfaceErrorCounts[leastErrorIndex]) {
					leastErrorIndex=i;
				}
			}
			if(listInterfaceErrorLogs[leastErrorIndex]!="" || verbose) {
				textLog.Text+="\r\n";
				textLog.Text+="   HL7 Messages\r\n";
				textLog.Text+="".PadRight(90,'*')+"\r\n";
			}
			if(verbose) {
				textLog.Text+="HL7 Interface "+listHL7InterfaceIDs[leastErrorIndex]+" had "+(listInterfaceErrorCounts[leastErrorIndex]==0?"no":"the following")+" issues.\r\n";
			}
			textLog.Text+=listInterfaceErrorLogs[leastErrorIndex];
			Application.DoEvents();
		}

		private string verifyAsADTMessage(int interfaceID,int messageID,bool verbose) {
			string msg="";
			_countErrors=0;
			bool isValidMsg=true;
			List<string> listSegmentsContained=new List<string>();
			DataTable tableHL7Segments=MySqlHelper.ExecuteDataset(_connectionString,"SELECT SegmentData FROM hl7segment_details WHERE InterfaceID="+interfaceID+" AND Messageid="+messageID+";").Tables[0];
			//validate segments based on content
			for(int r=0;r<tableHL7Segments.Rows.Count;r++) {
				string[] stringArraySegmentVals=tableHL7Segments.Rows[r]["SegmentData"].ToString().Split('|');
				listSegmentsContained.Add(stringArraySegmentVals[0]);//used later to validate existance of segments.
				switch(stringArraySegmentVals[0]) {
					case "EVN":
						//We ignore this field
						continue;
					case "PID":
						if(stringArraySegmentVals[2]!="{PID}") {
							msg+="ADT HL7 message is not sending eCW's internal patient number in field PID.02\r\n";
							_countErrors++;
							isValidMsg=false;
						}
						if(stringArraySegmentVals[4]!="{CONTNO}") {
							msg+="ADT HL7 message is not sending eCW's account number in field PID.04\r\n";
							_countErrors++;
							isValidMsg=false;
						}
						continue;
					case "GT1":
						if(stringArraySegmentVals[2]!="{GRID}") {
							msg+="ADT HL7 message is not sending guarantor's id number in field GT1.02\r\n";
							_countErrors++;
							isValidMsg=false;
						}
						if(stringArraySegmentVals[3]!="{GRLN}^{GRFN}^{GRMN}") {
							msg+="ADT HL7 message is not sending eCW's guarantor's name in field GT1.03\r\n";
							_countErrors++;
							isValidMsg=false;
						}
						if(stringArraySegmentVals[11]!="{GRREL}") {
							msg+="ADT HL7 message is not sending guarantor's relationship to patient in field GT1.11\r\n";
							_countErrors++;
							isValidMsg=false;
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
			if(!listSegmentsContained.Contains("PID")) {
				msg+="No PID segment found in ADT HL7 message.\r\n";
				_countErrors+=3;//No segment +2 sub errors
				isValidMsg=false;
			}
			if(!listSegmentsContained.Contains("GT1") && verbose) {
				msg+="No GT1 segment found in ADT HL7 message. Guarantors for new patients will always be set to self.\r\n";
				isValidMsg=false;
			}
			//If everything above checks out return a success message
			if(isValidMsg && verbose) {
				msg+="Found properly formed ADT HL7 message definition.\r\n";
			}
			return msg;
		}

		private string verifyAsSIUMessage(int interfaceID,int messageID,bool verbose) {
			string msg="";
			_countErrors=0;
			bool isValidMsg=true;
			List<string> listSegmentsContained=new List<string>();
			DataTable tableHL7Segments=MySqlHelper.ExecuteDataset(_connectionString,"SELECT SegmentData FROM hl7segment_details WHERE InterfaceID="+interfaceID+" AND Messageid="+messageID+";").Tables[0];
			//validate segments based on content
			for(int r=0;r<tableHL7Segments.Rows.Count;r++) {
				string[] stringArraySegmentFields=tableHL7Segments.Rows[r]["SegmentData"].ToString().Split('|');
				listSegmentsContained.Add(stringArraySegmentFields[0]);//used later to validate existance of segments.
				switch(stringArraySegmentFields[0]) {
					case "MSH":
						//validation?
						continue;
					case "SCH":
						if(stringArraySegmentFields[2]!="{ENCID}") {//eCW's documentation is wrong. SCH.01 is not used as appointment num, instead SCH.02 is used for appointment num.
							msg+="SIU HL7 message is not sending visit number in field SCH.01\r\n";
							_countErrors++;
							isValidMsg=false;
						}
						if(stringArraySegmentFields[7]!="{ENCREASON}") {
							msg+="SIU HL7 message is not sending visit reason in field SCH.07\r\n";
							_countErrors++;
							isValidMsg=false;
						}
						if(stringArraySegmentFields[8]!="{VISITTYPE}") {
							msg+="SIU HL7 message is not sending visit type in field SCH.08\r\n";
							_countErrors++;
							isValidMsg=false;
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
						string[] stringArraySCH11=stringArraySegmentFields[11].Split('^');
						//if(false) {//SCH11[2]!="{???}") { //Don't 
						//	retVal+="SIU HL7 message is not sending appointment duration in field SCH.11.02\r\n";
						//	errors++;
						//	validMessage=false;
						//}
						if(stringArraySCH11[3]!="{ENCSDATETIME}") {
							msg+="SIU HL7 message is not sending appointment start time in field SCH.11.03\r\n";
							_countErrors++;
							isValidMsg=false;
						}
						if(stringArraySCH11[4]!="{ENCEDATETIME}") {
							msg+="SIU HL7 message is not sending appointment end time in field SCH.11.04\r\n";
							_countErrors++;
							isValidMsg=false;
						}
						//if(segmentFields[25]!="{STATUS}") {//according to documentation, we need this, but actually we never try to reference it.
						//  retVal+="SIU HL7 message is not sending visit status in field SCH.25\r\n";
						//  errors++;
						//  validMessage=false;
						//}
						continue;
					case "PID":
						if(stringArraySegmentFields[2]!="{PID}") {
							msg+="SIU HL7 message is not sending eCW's internal patient number in field PID.02\r\n";
							_countErrors++;
							isValidMsg=false;
						}
						if(stringArraySegmentFields[4]!="{CONTNO}" && !Programs.UsingEcwTightOrFullMode()) {
							msg+="SIU HL7 message is not sending eCW's account number in field PID.04\r\n";
							_countErrors++;
							isValidMsg=false;
						}
						if(stringArraySegmentFields[5]!="{PLN}^{PFN}^{PMN}") {
							msg+="SIU HL7 message is not sending patient's name correctly in field PID.05\r\n";
							_countErrors++;
							isValidMsg=false;
						}
						if(stringArraySegmentFields[7]!="{PDOB}") {
							msg+="SIU HL7 message is not sending patient's date of birth in field PID.07\r\n";
							_countErrors++;
							isValidMsg=false;
						}
						if(stringArraySegmentFields[8]!="{PSEX}") {
							msg+="SIU HL7 message is not sending patient's gender in field PID.08\r\n";
							_countErrors++;
							isValidMsg=false;
						}
						//No checking of optional fields.
						continue;
					case "PV1":
						if(stringArraySegmentFields[7]!="{ODDRID}^{ODLN}^{ODFN}") {
							msg+="SIU HL7 message is not sending provider id in field PV1.07\r\n";
							_countErrors++;
							isValidMsg=false;
						}
						continue;
					case "AIG":
						if(stringArraySegmentFields[3]!="{RSDRID}^{RSLN}^{RSFN}") {
							msg+="SIU HL7 message is not sending provider/resource id in field AIG.03\r\n";
							_countErrors++;
							isValidMsg=false;
						}
						continue;
					default:
						continue;
				}
			}
			//Validate existance of segments
			if(!listSegmentsContained.Contains("SCH")) {
				msg+="No SCH segment found in SIU HL7 message.\r\n";
				_countErrors+=7;//no segment plus 6 sub errors.
				isValidMsg=false;
			}
			if(!listSegmentsContained.Contains("PID")) {
				msg+="No PID segment found in SIU HL7 message.\r\n";
				if(!Programs.UsingEcwTightOrFullMode()) {
					_countErrors++;//to account for not sending eCW's account number
				}
				_countErrors+=5;//no segment plus 4 sub errors.
				isValidMsg=false;
			}
			if(!listSegmentsContained.Contains("AIG") && !listSegmentsContained.Contains("PV1")) {
				msg+="No AIG or PV1 segments found in SIU HL7 message. Appointments will use patient's default primary provider.\r\n";//ecwSIU.cs sets this when in-processing SIU message.
				isValidMsg=false;
			}
			//If everything above checks out return a success message
			if(isValidMsg && verbose) {
				msg+="Found properly formed SIU HL7 message definition.\r\n";
			}
			return msg;
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
			string msg = "";
			DataTable table = new DataTable();
			try {
				table=MySqlHelper.ExecuteDataset(_connectionString,"SELECT * FROM visitcodes LEFT OUTER JOIN pmcodes ON visitcodes.Name=pmcodes.ecwcode WHERE dentalvisit=1;").Tables[0];
			}
			catch(Exception ex) {
				return ex.Message+"\r\n";
			}
			//left outer join should show null in ecwcode if there is no corresponding pmcode for the visitcode.
			if(verbose || table.Select("ecwcode is null").Length>0 || table.Rows.Count==0) {
				msg+="\r\n";
				msg+="   Dental Visit Codes\r\n";
				msg+="".PadRight(90,'*')+"\r\n";
				for(int r=0;r<table.Rows.Count;r++) {
					if(table.Rows[r]["ecwcode"].ToString()=="") {
						msg+="Dental visit code named \""+table.Rows[r]["Description"].ToString()+"\" found but not set up.\r\n";
					}
					else if(verbose) {
						msg+="Dental visit code named \""+table.Rows[r]["Description"].ToString()+"\" found.\r\n";
					}
				}
				if(table.Rows.Count==0) {
					msg+="No dental visit codes found or set up.\r\n";
				}
			}
			return msg;
		}

		private List<int> ColumnToListHelper(DataTable table,string colName) {
			List<int> listRetVals = new List<int>();
			for(int r=0;r<table.Rows.Count;r++) {
				listRetVals.Add((int)table.Rows[r][colName]);
			}
			return listRetVals;
		}

		private string TestTemplate(bool verbose) {
			StringBuilder stringBuilder=new StringBuilder();
			bool failed=true;
			string command="SHOW FULL TABLES WHERE Table_type='BASE TABLE'";//Tables, not views.  Does not work in MySQL 4.1, however we test for MySQL version >= 5.0 in PrefL.
			DataTable table=MySql.Data.MySqlClient.MySqlHelper.ExecuteDataset(_connectionString,command).Tables[0];
			//or MySql.Data.MySqlClient.MySqlDataReader mtDataReader;
			//Place check code here. Also, use a reader, table or both as shown above.
			if(verbose||failed) {
				stringBuilder.Clear();
				stringBuilder.Append("HL7 message definitions are not formed properly. //TODO: maybe add some more specific details here.");
			}
			return stringBuilder.ToString();
		}

		private void checkShow_KeyPress(object sender,KeyPressEventArgs e) {
			KeysConverter keysConverter=new KeysConverter();
			try {
				_stringBuilder.Append(e.KeyChar);
			}
			catch(Exception ex) {
				ex.DoNothing();
				//fail VERY silently. Mwa Ha Ha.
			}
			if(_stringBuilder.ToString().EndsWith("X")) {//Clear string if (upper case) 'X' is pressed.
				_stringBuilder.Clear();
			}
			if(_stringBuilder.ToString()=="open" || _stringBuilder.ToString()=="There is no cow level") {
				using FormEcwDiagAdv formEcwDiagAdv=new FormEcwDiagAdv();
				formEcwDiagAdv.ShowDialog();
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