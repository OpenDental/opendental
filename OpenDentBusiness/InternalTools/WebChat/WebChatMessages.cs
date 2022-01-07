using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness {
	public class WebChatMessages {

		///<summary>Sets PK as well as DateT.</summary>
		public static long Insert(WebChatMessage webChatMessage) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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

		public static List<WebChatMessage> GetAllForSessions(params long[] arrayWebChatSessionNums) {
			if(arrayWebChatSessionNums.Length==0) {
				return new List<WebChatMessage>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<WebChatMessage>>(MethodBase.GetCurrentMethod(),arrayWebChatSessionNums);
			}
			List <WebChatMessage> listWebChatMessages=null;
			WebChatMisc.DbAction(delegate() {
				string command="SELECT * FROM webchatmessage WHERE WebChatSessionNum IN ("+String.Join(",",arrayWebChatSessionNums.Select(x => POut.Long(x)))+")";
				listWebChatMessages=Crud.WebChatMessageCrud.SelectMany(command);
			});
			return listWebChatMessages;
		}

	}
}