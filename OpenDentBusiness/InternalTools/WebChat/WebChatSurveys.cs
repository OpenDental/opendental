using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness {
	public class WebChatSurveys {

		///<summary>Also sets primary key</summary>
		public static long Insert(WebChatSurvey webChatSurvey) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				webChatSurvey.WebChatSurveyNum=Meth.GetLong(MethodBase.GetCurrentMethod(),webChatSurvey);
				return webChatSurvey.WebChatSessionNum;
			}
			WebChatMisc.DbAction(delegate () {
				Crud.WebChatSurveyCrud.Insert(webChatSurvey);
			});
			return webChatSurvey.WebChatSessionNum;
		}

		public static List <WebChatSurvey> GetSurveysForSessions(List<long> listWebChatSessionNums) {
			if(listWebChatSessionNums.Count==0) {
				return new List<WebChatSurvey>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<WebChatSurvey>>(MethodBase.GetCurrentMethod(),listWebChatSessionNums);
			}
			List<WebChatSurvey> listWebChatSurveys=null;
			WebChatMisc.DbAction(delegate() {
				string command="SELECT * FROM webchatsurvey WHERE WebChatSessionNum IN ("+String.Join(",",listWebChatSessionNums)+")";
				listWebChatSurveys=Crud.WebChatSurveyCrud.SelectMany(command);
			});
			return listWebChatSurveys;
		}

	}
}