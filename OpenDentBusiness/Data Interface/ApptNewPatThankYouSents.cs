using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ApptNewPatThankYouSents{
		public const string NEW_PAT_WEB_FORM_TAG="[NewPatWebFormURL]";
		public static void InsertMany(List<ApptNewPatThankYouSent> listApptNewPatThankYouSents) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listApptNewPatThankYouSents);
				return;
			}
			Crud.ApptNewPatThankYouSentCrud.InsertMany(listApptNewPatThankYouSents);
		}
	}
}