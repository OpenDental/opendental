using System.Text;

namespace OpenDentBusiness.AutoComm {
	public class MsgToPayTagReplacer : TagReplacer {
		///<summary>Since we replace tags in OD proper before actually queueing or generating a statement, replacing tags there will either result in 0 or an inaccurate amount. This tag should only be replaced by the AutoCommProcessor.</summary>
		private bool _replaceStatementBalance=true;

		public MsgToPayTagReplacer(bool replaceStatmentBalance=true) {
			_replaceStatementBalance=replaceStatmentBalance;
		}

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
			if(_replaceStatementBalance) {
				double balance=Statements.GetFamilyBalance(autoCommObj.PatNum);
				ReplaceOneTag(sbTemplate,MsgToPaySents.STATEMENT_BALANCE_TAG,balance.ToString(),isEmail);
			}
		}
	}
}
