using OpenDentBusiness.AutoComm;
using System.Text;

namespace OpenDentBusiness.AutoComm {
	public class MsgToPayTagReplacer : TagReplacer {
		public MsgToPayTagReplacer() { }

		public static string GetMsgToPayTagRegEx() {
			return MsgToPaySents.MSG_TO_PAY_TAG.Replace("[","\\[")
				.Replace("(","\\(")
				.Replace(")","\\)");
		}

		public static string GetStatementUrlTagRegEx() {
			return MsgToPaySents.STATEMENT_URL_TAG.Replace("[","\\[")
				.Replace("(","\\(")
				.Replace(")","\\)");
		}

		protected override void ReplaceTagsChild(StringBuilder sbTemplate,AutoCommObj autoCommObj,bool isEmail) {
			base.ReplaceTagsChild(sbTemplate,autoCommObj,isEmail);
			double balance=OpenDentBusiness.Statements.GetFamilyBalance(autoCommObj.PatNum);
			ReplaceOneTag(sbTemplate,MsgToPaySents.STATEMENT_BALANCE_TAG,balance.ToString(),isEmail);
		}
	}
}
