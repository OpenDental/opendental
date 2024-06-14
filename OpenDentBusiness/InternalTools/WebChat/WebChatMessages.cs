using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness {
	public class WebChatMessages {

		///<summary>Sets PK as well as DateT.</summary>
		public static long Insert(WebChatMessage webChatMessage) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				webChatMessage.WebChatMessageNum=Meth.GetLong(MethodBase.GetCurrentMethod(),webChatMessage);
				return webChatMessage.WebChatMessageNum;
			}
			WebChatMisc.DbAction(delegate() {
				Crud.WebChatMessageCrud.Insert(webChatMessage);
			});
			WebChatMisc.DbAction(delegate() {
				Signalod signalSession=new Signalod();
				signalSession.IType=InvalidType.WebChatSessions;
				signalSession.FKey=webChatMessage.WebChatSessionNum;
				Signalods.Insert(signalSession);
			},false);
			return webChatMessage.WebChatMessageNum;
		}

		///<summary>Gets from db.</summary>
		public static List<WebChatMessage> GetAllForSessions(params long[] arrayWebChatSessionNums) {
			if(arrayWebChatSessionNums.Length==0) {
				return new List<WebChatMessage>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<WebChatMessage>>(MethodBase.GetCurrentMethod(),arrayWebChatSessionNums);
			}
			List <WebChatMessage> listWebChatMessages=null;
			WebChatMisc.DbAction(delegate() {
				string command="SELECT * FROM webchatmessage WHERE WebChatSessionNum IN ("+String.Join(",",arrayWebChatSessionNums.Select(x => POut.Long(x)))+")";
				listWebChatMessages=Crud.WebChatMessageCrud.SelectMany(command);
			});
			return listWebChatMessages;
		}

		///<summary>Toggles the value of NeedsFollow up for the given webChatMessage, should only be called for messages with a MessageType of Ai.</summary>
		public static bool ToggleNeedsFollowUp(WebChatMessage webChatMessage) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),webChatMessage);
			}
			webChatMessage.NeedsFollowUp=!webChatMessage.NeedsFollowUp;
			WebChatMisc.DbAction(delegate () {
				Crud.WebChatMessageCrud.Update(webChatMessage);
			});
			WebChatMisc.DbAction(delegate () {
				Signalod signalSession=new Signalod();
				signalSession.IType=InvalidType.WebChatSessions;
				signalSession.FKey=webChatMessage.WebChatSessionNum;
				Signalods.Insert(signalSession);
			},isWebChatDb:false);
			return webChatMessage.NeedsFollowUp;
		}
	}
}