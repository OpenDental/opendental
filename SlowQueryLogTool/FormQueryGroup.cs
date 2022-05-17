using System;
using System.Drawing;
using System.Windows.Forms;
using CodeBase;
using SlowQueryLog.UI;

namespace SlowQueryTool {
	public partial class FormQueryGroup:Form {

		public QueryGroup QueryGroupCur;

		public FormQueryGroup(QueryGroup queryGroupCur) {
			QueryGroupCur=queryGroupCur;
			InitializeComponent();
		}

		private void FormQuery_Load(object sender,EventArgs e) {
			textGroupNum.Text=QueryGroupCur.QueryGroupNum.ToString();
			textCount.Text=QueryGroupCur.ListQueriesInGroup.Count.ToString();
			textExecutionMedian.Text=QueryGroupCur.ExecutionTimeMedian.ToString()+"s";
			textExecutionAvg.Text=QueryGroupCur.ExecutionTimeMean.ToString()+"s";
			textExecutionMax.Text=QueryGroupCur.ExecutionTimeMax.ToString()+"s";
			textExecutionMin.Text=QueryGroupCur.ExecutionTimeMin.ToString()+"s";
			textExecutionTotal.Text=QueryGroupCur.ExecutionTimeTotalTime.ToString()+"s";
			textLockMedian.Text=QueryGroupCur.LockTimeMedian.ToString()+"s";
			textLockAvg.Text=QueryGroupCur.LockTimeMean.ToString()+"s";
			textLockMax.Text=QueryGroupCur.LockTimeMax.ToString()+"s";
			textLockMin.Text=QueryGroupCur.LockTimeMin.ToString()+"s";
			textLockTotal.Text=QueryGroupCur.LockTimeTotalTime.ToString()+"s";
			textRowsMedian.Text=QueryGroupCur.RowsExaminedMedian.ToString();
			textRowsAvg.Text=QueryGroupCur.RowsExaminedMean.ToString();
			textRowsMax.Text=QueryGroupCur.RowsExaminedMax.ToString();
			textRowsMin.Text=QueryGroupCur.RowsExaminedMin.ToString();
			textAvgTimeBetween.Text=QueryGroupCur.AverageTimeBetweenQueries.ToString();
			textExampleQuery.Text=QueryGroupCur.ListQueriesInGroup[0].FormattedQuery;
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}
