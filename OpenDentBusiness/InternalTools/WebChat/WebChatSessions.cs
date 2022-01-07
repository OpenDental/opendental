using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OpenDentBusiness {
	public class WebChatSessions {

		///<summary>Also sets primary key and DateTcreated.</summary>
		public static long Insert(WebChatSession webChatSession) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				webChatSession.WebChatSessionNum=Meth.GetLong(MethodBase.GetCurrentMethod(),webChatSession);
				return webChatSession.WebChatSessionNum;
			}
			WebChatMisc.DbAction(delegate() {
				Crud.WebChatSessionCrud.Insert(webChatSession);
			});
			WebChatMisc.DbAction(delegate() {
				Signalods.SetInvalid(InvalidType.WebChatSessions);//Signal OD HQ to refresh sessions.
			},false);
			return webChatSession.WebChatSessionNum;
		}

		public static List <WebChatSession> GetSessions(bool hasEndedSessionsIncluded,DateTime dateCreatedFrom,DateTime dateCreatedTo) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<WebChatSession>>(MethodBase.GetCurrentMethod(),hasEndedSessionsIncluded,dateCreatedFrom,dateCreatedTo);
			}
			List<WebChatSession> listWebChatSessions=null;
			WebChatMisc.DbAction(delegate() {
				string command="SELECT * FROM webchatsession "
					+"WHERE DateTcreated >= "+POut.DateT(dateCreatedFrom)+" AND DateTcreated <= "+POut.DateT(dateCreatedTo)+" ";
				if(!hasEndedSessionsIncluded) {//Do not show ended sessions?
					command+="AND DateTend < "+POut.DateT(new DateTime(1880,1,1))+" ";//Session is ended if DateTend is set.
				}
				command+="ORDER BY DateTend,DateTcreated";//By DateTend first, so that currently active sessions show at the top of the list.
				listWebChatSessions=Crud.WebChatSessionCrud.SelectMany(command);
			});
			return listWebChatSessions;
		}

		public static List<WebChatSession> GetActiveSessions() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<WebChatSession>>(MethodBase.GetCurrentMethod());
			}
			List<WebChatSession> listWebChatSessions=new List<WebChatSession>();
			WebChatMisc.DbAction(() => {
				string command="SELECT * FROM webchatsession WHERE DateTend < "+POut.DateT(new DateTime(1880,1,1))+" ";//Session is ended if DateTend is set.
				command+="ORDER BY DateTcreated";
				listWebChatSessions=Crud.WebChatSessionCrud.SelectMany(command);
			});
			return listWebChatSessions;
		}

		public static WebChatSession GetActiveSessionsForEmployee(string techName) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<WebChatSession>(MethodBase.GetCurrentMethod(),techName);
			}
			WebChatSession webChatSession=null;
			WebChatMisc.DbAction(() => {
				string command="SELECT * FROM webchatsession WHERE DateTend < "+POut.DateT(new DateTime(1880,1,1))
					+" AND webchatsession.TechName = '"+POut.String(techName)+"'"
					+" ORDER BY webchatsession.DateTcreated ";
				webChatSession=Crud.WebChatSessionCrud.SelectOne(command);
			});
			return webChatSession;
		}

		public static WebChatSession GetOne(long webChatSessionNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<WebChatSession>(MethodBase.GetCurrentMethod(),webChatSessionNum);
			}
			WebChatSession session=null;
			WebChatMisc.DbAction(delegate() {																																								 
				session=Crud.WebChatSessionCrud.SelectOne(webChatSessionNum);
			});
			return session;
		}

		public static void Update(WebChatSession webChatSession,WebChatSession oldWebChatSession,bool hasSignal=true) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),webChatSession,oldWebChatSession,hasSignal);
				return;
			}
			WebChatMisc.DbAction(delegate() {
				Crud.WebChatSessionCrud.Update(webChatSession,oldWebChatSession);
			});
			if(hasSignal) {
				WebChatMisc.DbAction(delegate() {
					Signalods.SetInvalid(InvalidType.WebChatSessions);//Signal OD HQ to refresh sessions.
				},false);
			}
		}

		public static void SendWelcomeMessage(long webChatSessionNum) {
			//No need to check RemotingRole; no call to db.
			WebChatMessage welcomeMessage=new WebChatMessage();
			welcomeMessage.WebChatSessionNum=webChatSessionNum;
			welcomeMessage.UserName=WebChatPrefs.GetString(WebChatPrefName.SystemName);
			welcomeMessage.MessageText=WebChatPrefs.GetString(WebChatPrefName.SystemWelcomeMessage);
			welcomeMessage.MessageType=WebChatMessageType.System;
			WebChatMessages.Insert(welcomeMessage);
		}

		///<summary>Sets the DateTend field to now and inserts a message into the chat so all users can see the session has ended.</summary>
		public static void EndSession(long webChatSessionNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),webChatSessionNum);
				return;
			}
			WebChatMisc.DbAction(delegate() {
				string command="UPDATE webchatsession SET DateTend=NOW() WHERE WebChatSessionNum="+POut.Long(webChatSessionNum);
				DataCore.NonQ(command);
				//Last message just after session ended, in case someone types another message into the thread just as the thread is ending.
				//This way the end session message is guaranteed to be last, since the timestamp on it is after the session technically ended.
				WebChatMessage endSessionMessage=new WebChatMessage();
				endSessionMessage.WebChatSessionNum=webChatSessionNum;
				endSessionMessage.UserName=WebChatPrefs.GetString(WebChatPrefName.SystemName);
				endSessionMessage.MessageText=WebChatPrefs.GetString(WebChatPrefName.SystemSessionEndMessage);
				endSessionMessage.MessageType=WebChatMessageType.EndSession;
				WebChatMessages.Insert(endSessionMessage);
			});
		}

		///<summary>Customer ends session. Sets the DateTend field to now and inserts a message into the chat so all users can see the session has ended.</summary>
		public static void EndSessionFromCustomer(long webChatSessionNum,string customerName) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),webChatSessionNum,customerName);
				return;
			}
			if(String.IsNullOrEmpty(customerName)) {
				customerName="Customer";
			}
			WebChatMisc.DbAction(delegate () {
				string command="UPDATE webchatsession SET DateTend=NOW() WHERE WebChatSessionNum="+POut.Long(webChatSessionNum);
				DataCore.NonQ(command);
				//Last message just after session ended, in case someone types another message into the thread just as the thread is ending.
				//This way the end session message is guaranteed to be last, since the timestamp on it is after the session technically ended.
				WebChatMessage endSessionMessage=new WebChatMessage();
				endSessionMessage.WebChatSessionNum=webChatSessionNum;
				endSessionMessage.UserName=customerName;
				endSessionMessage.MessageText=WebChatPrefs.GetString(WebChatPrefName.SystemSessionEndMessage);
				endSessionMessage.MessageType=WebChatMessageType.EndSession;
				WebChatMessages.Insert(endSessionMessage);
			});
		}

		public static void SendCustomerMessage(long webChatSessionNum,string userName,string messageText) {
			//No need to check RemotingRole; no call to db.
			if(String.IsNullOrEmpty(messageText)) {
				return;//No blank messages allowed.
			}
			WebChatMessage customerMessage=new WebChatMessage();
			customerMessage.WebChatSessionNum=webChatSessionNum;
			if(String.IsNullOrEmpty(userName)) {
				customerMessage.UserName="Customer";
			}
			else {
				customerMessage.UserName=userName;
			}
			customerMessage.MessageText=messageText;
			customerMessage.MessageType=WebChatMessageType.Customer;
			WebChatMessages.Insert(customerMessage);
		}

		public static void SendTechMessage(long webChatSessionNum,string messageText) {
			//Refresh the session in case the session ended in less than the last signal interval.
			WebChatSession webChatSession=WebChatSessions.GetOne(webChatSessionNum);
			if(String.IsNullOrEmpty(messageText) || webChatSession.DateTend.Year>1880) {//No blank messages or messages after session end allowed.
				return;
			}
			WebChatMessage techMessage=new WebChatMessage();
			techMessage.WebChatSessionNum=webChatSession.WebChatSessionNum;
			techMessage.UserName=Security.CurUser.UserName;
			techMessage.MessageText=messageText;
			techMessage.MessageType=WebChatMessageType.Technician;
			WebChatMessages.Insert(techMessage);
		}

		///<summary>Takes a messageText string and checks it for existing html links. If it finds one, it adds the necessary
		///html brackets to present the text as a link for users to click. This link will open in a new window.
		///IMPORTANT: This should _only_ be applied to scrubbed text to avoid potential malacious code.</summary>
		public static string ConvertMsgTextHtmlLinks(string msg) {
			//Regex found https://stackoverflow.com/questions/6038061/regular-expression-to-find-urls-within-a-string 
			//Credited to Sillas Camara
			MatchCollection url=Regex.Matches(msg,"((\\w+:\\/\\/\\S+)|(\\w+[\\.:]\\w+\\S+))[^\\s,\\.]",RegexOptions.IgnoreCase);
			foreach(string match in url.OfType<Match>().Select(x => x.Value).Distinct()) {
				string pattern=Regex.Escape(match);//Make a 'literal' regular expression pattern out of the match.
				if(match.Contains("://")) { //expects something like https://www.patientviewer.com or http://opendental.com
					msg=Regex.Replace(msg,pattern,"<a href='"+match+"' target='_blank'>"+match+"</a>");
				}
				else {
					//If a url is passed in without a prepending http:// it will be re-routed to localhost and fail.
					//target=_blank opens the url in a seperate window for the user
					msg=Regex.Replace(msg,pattern,"<a href='http://"+match+"' target='_blank'>"+match+"</a>");
				}
			}
			return msg;
		}

	}
}