using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using CodeBase;
using OpenDentBusiness;
using OpenDentBusiness.HL7;

namespace OpenDentHL7 {
	///<summary>This is the still the main Service class.  I just put it in a different file to keep it separate.</summary>
	public partial class ServiceHL7:ServiceBase {
		private static bool ecwOldIsReceiving;
		private System.Threading.Timer ecwOldTimerSend;
		private System.Threading.Timer ecwOldTimerReceive;
		private static string ecwOldHl7FolderIn;
		private static string ecwOldHl7FolderOut;
		///<summary>Indicates the standalone mode for eCW, or the use of Mountainside.  In both cases, chartNumber will be used instead of PatNum.</summary>
		private static bool ecwOldIsStandalone;
		private static bool _ecwOldIsSending;
		private static DateTime _ecwOldDateTimeOldMsgsDeleted;

		private void EcwOldSendAndReceive(){
			ecwOldIsStandalone=true;//and for Mountainside
			if(Programs.UsingEcwTightOrFullMode()) {
				ecwOldIsStandalone=false;
			}
			//if(ODBuild.IsDebug()) {//just so I don't forget to remove it later.
				//IsStandalone=false;
			//}
			ecwOldHl7FolderOut=PrefC.GetString(PrefName.HL7FolderOut);
			if(!Directory.Exists(ecwOldHl7FolderOut)) {
				throw new ApplicationException(ecwOldHl7FolderOut+" does not exist.");
			}
			//start polling the folder for waiting messages to import.  Every 5 seconds.
			TimerCallback timercallbackReceive=new TimerCallback(EcwOldTimerCallbackReceiveFunction);
			ecwOldTimerReceive=new System.Threading.Timer(timercallbackReceive,null,5000,5000);
			if(ecwOldIsStandalone) {
				return;//do not continue with the HL7 sending code below
			}
			//start polling the db for new HL7 messages to send. Every 1.8 seconds.
			ecwOldHl7FolderIn=PrefC.GetString(PrefName.HL7FolderIn);
			if(!Directory.Exists(ecwOldHl7FolderIn)) {
				throw new ApplicationException(ecwOldHl7FolderIn+" does not exist.");
			}
			_ecwOldDateTimeOldMsgsDeleted=DateTime.MinValue;
			_ecwOldIsSending=false;
			TimerCallback timercallbackSend=new TimerCallback(EcwOldTimerCallbackSendFunction);
			ecwOldTimerSend=new System.Threading.Timer(timercallbackSend,null,1800,1800);
		}

		private void EcwOldTimerCallbackReceiveFunction(Object stateInfo) {
			//process all waiting messages
			if(ecwOldIsReceiving) {
				return;//already in the middle of processing files
			}
			ecwOldIsReceiving=true;
			string[] existingFiles=Directory.GetFiles(ecwOldHl7FolderOut);
			for(int i=0;i<existingFiles.Length;i++) {
				EcwOldProcessMessage(existingFiles[i]);
			}
			ecwOldIsReceiving=false;
		}
		
		private void EcwOldProcessMessage(string fullPath) {
			string msgtext="";
			int i=0;
			while(i<5) {
				try {
					msgtext=File.ReadAllText(fullPath);
					break;
				}
				catch {
				}
				Thread.Sleep(200);
				i++;
				if(i==5) {
					EventLog.WriteEntry("Could not read text from file due to file locking issues.",EventLogEntryType.Error);
					return;
				}
			}
			try {
				MessageHL7 msg=new MessageHL7(msgtext);//this creates an entire heirarchy of objects.
				if(msg.MsgType==MessageTypeHL7.ADT) {
					if(IsVerboseLogging) {
						EventLog.WriteEntry("OpenDentHL7","Processed ADT message",EventLogEntryType.Information);
					}
					EcwADT.ProcessMessage(msg,ecwOldIsStandalone,IsVerboseLogging);
				}
				else if(msg.MsgType==MessageTypeHL7.SIU && !ecwOldIsStandalone) {//appointments don't get imported if standalone mode.
					if(IsVerboseLogging) {
						EventLog.WriteEntry("OpenDentHL7","Processed SUI message",EventLogEntryType.Information);
					}
					EcwSIU.ProcessMessage(msg,IsVerboseLogging);
				}
			}
			catch(Exception ex) {
				EventLog.WriteEntry(ex.Message+"\r\n"+ex.StackTrace,EventLogEntryType.Error);
				return;
			}
			//we won't be processing DFT messages.
			//else if(msg.MsgType==MessageType.DFT) {
				//ADT.ProcessMessage(msg);
			//}
			try {
				File.Delete(fullPath);
			}
			catch(Exception ex) {
				EventLog.WriteEntry("Delete failed for "+fullPath+"\r\n"+ex.Message,EventLogEntryType.Error);
			}
		}

		private void EcwOldTimerCallbackSendFunction(Object stateInfo) {
			//does not happen for standalone
			if(_ecwOldIsSending) {//if there is a thread that is still sending, return
				return;
			}
			try {
				_ecwOldIsSending=true;
				if(IsVerboseLogging) {
					EventLog.WriteEntry("GetOnePending Start");
				}
				List<HL7Msg> list=HL7Msgs.GetOnePending();
				if(IsVerboseLogging) {
					EventLog.WriteEntry("GetOnePending Finished");
				}
				string filename;
				for(int i=0;i<list.Count;i++) {//Right now, there will only be 0 or 1 item in the list.
					if(list[i].AptNum==0) {
						filename=ODFileUtils.CreateRandomFile(ecwOldHl7FolderIn,".txt");
					}
					else {
						filename=Path.Combine(ecwOldHl7FolderIn,list[i].AptNum.ToString()+".txt");
					}
					//EventLog.WriteEntry("Attempting to create file: "+filename);
					File.WriteAllText(filename,list[i].MsgText);
					list[i].HL7Status=HL7MessageStatus.OutSent;
					HL7Msgs.Update(list[i]);//set the status to sent.
				}
				if(_ecwOldDateTimeOldMsgsDeleted.Date<DateTime.Now.Date) {
					if(IsVerboseLogging) {
						EventLog.WriteEntry("DeleteOldMsgText Starting");
					}
					_ecwOldDateTimeOldMsgsDeleted=DateTime.Now;
					HL7Msgs.DeleteOldMsgText();//this function deletes if DateTStamp is less than CURDATE-INTERVAL 4 MONTH.  That means it will delete message text only once a day, not time based.
					if(IsVerboseLogging) {
						EventLog.WriteEntry("DeleteOldMsgText Finished");
					}
				}
			}
			catch(Exception ex) {
				EventLog.WriteEntry("OpenDentHL7 error when sending HL7 message: "+ex.Message,EventLogEntryType.Warning);//Warning because the service will spawn a new thread in 1.8 seconds
			}
			finally {
				_ecwOldIsSending=false;
			}
		}

		private void EcwOldStop(){
			if(ecwOldTimerSend!=null) {
				ecwOldTimerSend.Dispose();
			}
		}

	}
}
