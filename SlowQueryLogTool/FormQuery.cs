using System;
using System.Drawing;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using SlowQueryLog.UI;

namespace SlowQueryTool {
	public partial class FormQuery:Form {

		public Query QueryCur;

		public FormQuery(Query queryCur) {
			QueryCur=queryCur;
			InitializeComponent();
		}

		private void FormQuery_Load(object sender,EventArgs e) {
			textQueryNum.Text=QueryCur.QueryNum.ToString();
			textTimeRan.Text=QueryCur.TimeRan.ToString();
			textComputerName.Text=QueryCur.ComputerName.ToString();
			textComputerIP.Text=QueryCur.ComputerIP.ToString();
			textComputerName.Text=QueryCur.ComputerName.ToString();
			checkThirdParty.Checked=QueryCur.IsLikelyThirdParty;
			textExecutionTime.Text=QueryCur.QueryExecutionTime.ToString()+"s";
			textLockTime.Text=QueryCur.LockTime.ToString()+"s";
			textRowsSent.Text=QueryCur.RowsSent.ToString();
			textThirdPartyReason.Text=QueryCur.ThirdPartyReason.GetDescription();
			textRowsExamined.Text=QueryCur.RowsExamined.ToString();
			checkUserQuery.Checked=QueryCur.IsLikelyUserQuery;
			checkVictim.Checked=QueryCur.IsVictim;
			if(checkVictim.Checked) {
				textPerpetrator.Visible=true;
				labelPerpetrator.Visible=true;
				textPerpetrator.Text=QueryCur.Perpetrator.ToString();
			}
			textQuery.Text=QueryCur.FormattedQuery;
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		private void checkFormatted_CheckedChanged(object sender,EventArgs e) {
			if(checkFormatted.Checked) {
				textQuery.Text=QueryCur.FormattedQuery;
			}
			else {
				textQuery.Text=QueryCur.UnformattedQuery;
			}
		}
	}
}
