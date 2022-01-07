using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;
using OpenDentBusiness.HL7;

namespace OpenDentBusiness{
	///<summary></summary>
	public class HL7Msgs{
		public static List<HL7Msg> GetOnePending(){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<HL7Msg>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM hl7msg WHERE HL7Status="+POut.Long((int)HL7MessageStatus.OutPending)+" "+DbHelper.LimitAnd(1);
			return Crud.HL7MsgCrud.SelectMany(command);//Just 0 or 1 item in list for now.
		}

		///<summary>This will retrieve the hl7msg object from the database using the primary key Hl7MsgNum.  Used primarily for getting the MsgText of the referenced message, since we do not want to get that potentially large data unless we specifically need it.</summary>
		public static HL7Msg GetOne(long hl7MsgNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<HL7Msg>(MethodBase.GetCurrentMethod(),hl7MsgNum);
			}
			string command="SELECT * FROM hl7msg WHERE HL7MsgNum="+POut.Long(hl7MsgNum);
			return Crud.HL7MsgCrud.SelectOne(command);
		}

		///<summary>When called we will make sure to send a startDate and endDate.  Status parameter 0:All, 1:OutPending, 2:OutSent, 3:OutFailed, 4:InProcessed, 5:InFailed.  This will not return hl7msg.MsgText due to large size of text of many messages.  To see the message text of one of the returned rows, use GetOne(long hl7MsgNum) above.</summary>
		public static List<HL7Msg> GetHL7Msgs(DateTime startDate,DateTime endDate,long patNum,int status) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<HL7Msg>>(MethodBase.GetCurrentMethod(),startDate,endDate,patNum,status);
			}
			//join with the patient table so we can display patient name instead of PatNum
			string command=@"SELECT HL7MsgNum,HL7Status,'' AS MsgText,AptNum,DateTStamp,PatNum,Note	"
				+"FROM hl7msg	WHERE "+DbHelper.DtimeToDate("hl7msg.DateTStamp")+" BETWEEN "+POut.Date(startDate)+" AND "+POut.Date(endDate)+" ";
			if(patNum>0) {
				command+="AND hl7msg.PatNum="+POut.Long(patNum)+" ";
			}
			if(status>0) {
				command+="AND hl7msg.HL7Status="+POut.Long(status-1)+" ";//minus 1 because 0=All but our enum starts at 0
			}
			command+="ORDER BY hl7msg.DateTStamp";
			return Crud.HL7MsgCrud.SelectMany(command);
		}

		///<summary>Gets the message control ID of the message we are attempting to send, for TCP/IP acknowledgment.</summary>
		public static string GetControlId(HL7Msg msg) {
			string retval="";
			if(msg==null) {
				return retval;
			}
			int controlIdOrder=0;
			MessageHL7 msgHl7=new MessageHL7(msg.MsgText);//creates the segments
			HL7Def def=HL7Defs.GetOneDeepEnabled();
			if(def==null) {
				return retval;
			}
			HL7DefMessage hl7defmsg=null;
			for(int i=0;i<def.hl7DefMessages.Count;i++) {
				if(def.hl7DefMessages[i].MessageType==msgHl7.MsgType) {
					hl7defmsg=def.hl7DefMessages[i];
					break;
				}
			}
			if(hl7defmsg==null) {//No message definition for this type of message in the enabled def
				return retval;
			}
			for(int s=0;s<hl7defmsg.hl7DefSegments.Count;s++) {//get MSH segment
				if(hl7defmsg.hl7DefSegments[s].SegmentName==SegmentNameHL7.MSH) {
					for(int f=0;f<hl7defmsg.hl7DefSegments[s].hl7DefFields.Count;f++) {//find messageControlId field in MSH segment def
						if(hl7defmsg.hl7DefSegments[s].hl7DefFields[f].FieldName=="messageControlId") {
							controlIdOrder=hl7defmsg.hl7DefSegments[s].hl7DefFields[f].OrdinalPos;
							break;
						}
					}
					break;
				}
			}
			if(controlIdOrder==0) {//No messageControlId defined for this MSH segment
				return retval;
			}
			for(int i=0;i<msgHl7.Segments.Count;i++) {//get control ID from message located in MSH segment with field determined above
				if(msgHl7.Segments[i].Name==SegmentNameHL7.MSH) {
					retval=msgHl7.Segments[i].Fields[controlIdOrder].ToString();
					break;
				}
			}
			return retval;
		}

		///<summary></summary>
		public static long Insert(HL7Msg hL7Msg) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				hL7Msg.HL7MsgNum=Meth.GetLong(MethodBase.GetCurrentMethod(),hL7Msg);
				return hL7Msg.HL7MsgNum;
			}
			return Crud.HL7MsgCrud.Insert(hL7Msg);
		}

		///<summary></summary>
		public static void Update(HL7Msg hL7Msg) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),hL7Msg);
				return;
			}
			Crud.HL7MsgCrud.Update(hL7Msg);
		}

		///<summary>This is only used when using eCW tight or full to determine whether the Finish&amp;Send button should say Revise instead in FormApptEdit.  Finds hl7msg entries with matching AptNum and HL7Status of OutSent or OutPending.  If any exist, this returns true, otherwise false.</summary>
		public static bool MessageWasSent(long aptNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),aptNum);
			}
			//Any outbound messages in eCW tight and full are DFT messages
			//so if there is an OutSent or OutPending messages with matching AptNum, the button should say Revise so we will return true if count>0
			string command="SELECT COUNT(*) FROM hl7msg WHERE AptNum="+POut.Long(aptNum)+" "
				+"AND (HL7Status="+POut.Int((int)HL7MessageStatus.OutSent)+" OR HL7Status="+POut.Int((int)HL7MessageStatus.OutPending)+")";
			if(Db.GetCount(command)=="0") {
				return false;
			}
			return true;
		}

		///<summary>Doesn't delete the old messages, but just the text of the message.  This avoids breaking MessageWasSent().  Only affects messages that are at least four months old, regardless of status.  The hl7msg rows should not be deleted because we do not want the "complete" button to show up again for old appointments.</summary>
		public static void DeleteOldMsgText() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			string command="UPDATE hl7msg SET MsgText='' "
				+"WHERE DateTStamp < ADDDATE(CURDATE(),INTERVAL -4 MONTH)";
			Db.NonQ(command);
		}

		public static List<HL7Msg> GetOneExisting(HL7Msg hl7Msg) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<HL7Msg>>(MethodBase.GetCurrentMethod(),hl7Msg);
			}
			//Might want to change the following query to:
			//string command="SELECT * FROM hl7msg WHERE HL7Status IN("+POut.Long((int)HL7MessageStatus.InProcessed)+","+POut.Long((int)HL7MessageStatus.OutSent)
			//+") AND MsgText='"+POut.String(hl7Msg.MsgText)+"' "+DbHelper.LimitAnd(1);
			string command="SELECT * FROM hl7msg WHERE MsgText='"+POut.String(hl7Msg.MsgText)+"' "+DbHelper.LimitAnd(1);
			return Crud.HL7MsgCrud.SelectMany(command);//Just 0 or 1 item in list for now.
		}

		public static void UpdateDateTStamp(HL7Msg hl7Msg) {
			if(string.IsNullOrWhiteSpace(hl7Msg.MsgText)) {//don't update DateTStamp if MsgText is blank, that would be all messages more than 4 months old
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),hl7Msg);
				return;
			}
			string command="UPDATE hl7msg SET DateTStamp=CURRENT_TIMESTAMP WHERE MsgText='"+POut.String(hl7Msg.MsgText)+"' ";
			Db.NonQ(command);
		}
	}
}