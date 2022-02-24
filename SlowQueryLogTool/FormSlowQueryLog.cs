using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using OpenDentBusiness;
using CodeBase;
using OpenDental.UI;

namespace SlowQueryTool {
	public partial class FormSlowQueryLog:Form {

		private QueryLog _queryLog;
		private ODThread _thread=null;

		public FormSlowQueryLog() {
			InitializeComponent();
		}

		private void FormSlowQueryLog_Load(object sender,EventArgs e) {
			FillGridColumnsAndClear();
			SetDefaults();
			this.WindowState=FormWindowState.Maximized;
		}

		private void SetDefaults() {
			comboOpen.SelectedIndex=6;
			comboClose.SelectedIndex=20;
			datePicker.SetDateTimeFrom(DateTime.Today.AddDays(-30));
			datePicker.SetDateTimeTo(DateTime.Today);
		}

		private void FillGridColumnsAndClear() {
			//gridQueries
			gridQueries.BeginUpdate();
			gridQueries.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("Query Num",70,GridSortingStrategy.AmountParse);
			gridQueries.ListGridColumns.Add(col);
			col=new GridColumn("Time Ran",125,GridSortingStrategy.DateParse);
			gridQueries.ListGridColumns.Add(col);
			col=new GridColumn("Execution Time",90,GridSortingStrategy.AmountParse);
			gridQueries.ListGridColumns.Add(col);
			col=new GridColumn("Query Lock Time",100,GridSortingStrategy.AmountParse);
			gridQueries.ListGridColumns.Add(col);
			col=new GridColumn("Rows Examined",100,GridSortingStrategy.AmountParse);
			gridQueries.ListGridColumns.Add(col);
			col=new GridColumn("Third Party",75,GridSortingStrategy.StringCompare);
			gridQueries.ListGridColumns.Add(col);
			col=new GridColumn("User Query",75,GridSortingStrategy.StringCompare);
			gridQueries.ListGridColumns.Add(col);
			col=new GridColumn("Victim",50,GridSortingStrategy.StringCompare);
			gridQueries.ListGridColumns.Add(col);
			col=new GridColumn("Query",75,GridSortingStrategy.StringCompare);
			gridQueries.ListGridColumns.Add(col);
			gridQueries.ListGridRows.Clear();
			gridQueries.EndUpdate();
			//gridQueryGroups
			gridQueryGroups.BeginUpdate();
			gridQueryGroups.ListGridColumns.Clear();
			col=new GridColumn("Group Num",75,GridSortingStrategy.AmountParse);
			gridQueryGroups.ListGridColumns.Add(col);
			col=new GridColumn("Worst",50,GridSortingStrategy.AmountParse);
			gridQueryGroups.ListGridColumns.Add(col);
			col=new GridColumn("Count",50,GridSortingStrategy.AmountParse);
			gridQueryGroups.ListGridColumns.Add(col);
			col=new GridColumn("Avg Time Between Queries",115,GridSortingStrategy.DateParse);
			gridQueryGroups.ListGridColumns.Add(col);
			col=new GridColumn("Execution Time Median",115,GridSortingStrategy.AmountParse);
			gridQueryGroups.ListGridColumns.Add(col);
			col=new GridColumn("Execution Time Mean",115,GridSortingStrategy.AmountParse);
			gridQueryGroups.ListGridColumns.Add(col);
			col=new GridColumn("Rows Examined Median",115,GridSortingStrategy.AmountParse);
			gridQueryGroups.ListGridColumns.Add(col);
			col=new GridColumn("Example Query",100,GridSortingStrategy.StringCompare);
			gridQueryGroups.ListGridColumns.Add(col);
			gridQueryGroups.ListGridRows.Clear();
			gridQueryGroups.EndUpdate();
			labelQueryCount.Text="Rows: ";
			labelQueryGroupCount.Text="Rows: ";
			labelVersion.Text="MySQL Version: ";
			labelFirstQuery.Text="Begin Date: ";
			labelLastQuery.Text="End Date: ";
		}

		private void butAnalyze_Click(object sender,EventArgs e) {
			if(_thread!=null) {
				_queryLog?.Stop();
				return;
			}
			if(!datePicker.IsValid) {
				MessageBox.Show("Please select a valid from and to date.");
				return;
			}
			if(comboOpen.SelectedIndex >= comboClose.SelectedIndex) {
				MessageBox.Show("The start hour for the practice cannot be after the end hour.");
				return;
			}
			textFilterValue.Text="";
			int hourOpen=comboOpen.SelectedIndex;
			int hourClose=comboClose.SelectedIndex;
			_thread=new ODThread(o => { GetData(hourOpen,hourClose); });
			_thread.Name="GetData";
			_thread.AddExitHandler((eh) => {
				//Null the thread out as a flag for next time.
				_thread=null;
				this.BeginInvoke(() => { butAnalyze.Text="Analyze"; });
			});
			_thread.AddExceptionHandler((eh) => {
				this.BeginInvoke(() => { textResults.Text=eh.Message; });
			});
			butAnalyze.Text="Stop";
			_thread.Start();
			textResults.Clear();
			textResults.SelectionFont=new Font(textResults.Font.FontFamily,12,FontStyle.Bold);
			textResults.AppendText("Parsing Queries...");
			FillGridColumnsAndClear();
		}

		private void GetData(int hourOpen,int hourClose) {
			_queryLog=new QueryLog {
				HourOpen=new TimeSpan(hourOpen,0,0),
				HourClose=new TimeSpan(hourClose,0,0),
				DateFrom=datePicker.GetDateTimeFrom(),
				DateTo=datePicker.GetDateTimeTo(),
				FilePath=textLogURL.Text,
				OnProgress=(percent) => {
					this.BeginInvoke(() => {
						textResults.Text=$"Parsing Queries...\r\nProgress: {percent.ToString("0")}%";
					});
				},
			};
			_queryLog.FillQueries();
			this.Invoke(() => {
				labelVersion.Text="MySQL Version: "+_queryLog.MySqlVersion;
				labelFirstQuery.Text="Begin Date: "+_queryLog.FirstQueryDate.ToString();
				labelLastQuery.Text="End Date: "+_queryLog.LastQueryDate.ToString();
				if(datePicker.HasEmptyDateTimeFrom() && datePicker.HasEmptyDateTimeTo()) {
					datePicker.SetDateTimeFrom(_queryLog.FirstQueryDate.Date < _queryLog.LastQueryDate.Date.AddDays(-14) ?
					_queryLog.LastQueryDate.Date.AddDays(-14) : _queryLog.FirstQueryDate.Date);
					datePicker.SetDateTimeTo(_queryLog.LastQueryDate.Date);
				}
				FillGridQueries();
				textResults.Clear();
				textResults.SelectionFont=new Font(textResults.Font.FontFamily,12,FontStyle.Bold);
				textResults.AppendText("Analyzing...");
			});
			if(_queryLog.ListQueries.Count > 0) {
				_queryLog.AnalyzeGroups();
			}
			this.Invoke(() => {
				RefreshGroupsAndSummary();
			});
		}

		private void RefreshGroupsAndSummary() {
			FillGridQueryGroups();
			//Fill Results
			textResults.Clear();
			if(_queryLog.ListQueries.Count > 0) {
				//Grade
				textResults.SelectionFont=new Font(textResults.Font.FontFamily,14,FontStyle.Bold);
				textResults.AppendText("Grade: ");
				textResults.SelectionColor=_queryLog.Grade.PenColor;
				textResults.AppendText(Enum.GetName(_queryLog.Grade.Result.GetType(),_queryLog.Grade.Result));
				textResults.SelectionFont=new Font(textResults.Font.FontFamily,9,FontStyle.Regular);
				textResults.SelectionColor=Color.Black;
				textResults.AppendText("\r\n"+"This grade is calculated based on the weights of the query groups taking into account execution "
					+"time, the date range, and slow query time span.\r\n\r\n");
				if(_queryLog.Grade.Result==TestResults.A || _queryLog.Grade.Result==TestResults.B) {
					textResults.AppendText("The slowness is likely not related to the queries. Check the worst queries regardless."+"\r\n\r\n");
				}
				else if(_queryLog.Grade.Result==TestResults.C) {
					textResults.AppendText("The slowness may be related to the queries. Check the worst queries or escalate if unsure."+"\r\n\r\n");
				}
				else {//D and F
					textResults.AppendText("The slowness is likely related to the queries. Check the worst queries, third party queries, and "
						+"the tests below."+"\r\n\r\n");
				}
				//Test 1
				PrintTestResults(_queryLog.AnalyzeRowsExamined(),1,"Rows Examined");
				//Test 2
				PrintTestResults(_queryLog.AnalyzeThirdPartyQueries(),2,"Non-Open Dental Queries");
				//Test 3
				PrintTestResults(_queryLog.AnalyzeVictimQueries(),3,"Victim Queries");
				//Test 4
				PrintTestResults(_queryLog.AnalyzeUserQueries(),4,"User Queries");
				//Worst Performing Queries
				textResults.SelectionFont=new Font(textResults.Font.FontFamily,12,FontStyle.Bold);
				textResults.AppendText("Worst Performing Queries");
				textResults.SelectionFont=new Font(textResults.Font.FontFamily,9,FontStyle.Regular);
				textResults.AppendText("\r\n\r\n");
				List<QueryGroup> listWorstGroups=_queryLog.AnalyzeWorstQueries();
				for(int i=0;i<listWorstGroups.Count;i++) {
					textResults.SelectionFont=new Font(textResults.Font.FontFamily,10,FontStyle.Bold);
					textResults.SelectionColor=Color.DarkGreen;
					textResults.AppendText("Worst Query #"+(i+1).ToString()+"\r\n\r\n");
					textResults.SelectionFont=new Font(textResults.Font.FontFamily,9,FontStyle.Regular);
					textResults.SelectionColor=Color.Black;
					textResults.AppendText("Query Group Number: "+listWorstGroups[i].QueryGroupNum.ToString()+"\r\n");
					textResults.AppendText("Rows: "+listWorstGroups[i].ListQueriesInGroup.Count.ToString()+"\r\n");
					textResults.AppendText("Average Time Between Queries: "+listWorstGroups[i].AverageTimeBetweenQueries.ToString()+"\r\n");
					textResults.AppendText("Median Execution Time: "+listWorstGroups[i].ExecutionTimeMedian.ToString()+"\r\n");
					textResults.AppendText("Median Lock Time: "+listWorstGroups[i].LockTimeMedian.ToString()+"\r\n");
					textResults.AppendText("Median Rows Examined: "+listWorstGroups[i].RowsExaminedMedian.ToString()+"\r\n\r\n");
				}
			}
		}

		private void PrintTestResults(Test result,int testNum,string testName) {
			textResults.SelectionFont=new Font(textResults.Font.FontFamily,12,FontStyle.Bold);
			textResults.AppendText("Test "+testNum+": "+testName);
			textResults.SelectionFont=new Font(textResults.Font.FontFamily,11,FontStyle.Regular);
			textResults.SelectionColor=result.PenColor;
			textResults.AppendText("\r\n\r\n"+Enum.GetName(result.Result.GetType(),result.Result)+"\r\n");
			textResults.SelectionFont=new Font(textResults.Font.FontFamily,9,FontStyle.Regular);
			textResults.SelectionColor=Color.Black;
			textResults.AppendText(result.ResultText);
		}

		private void FillGridQueries(long groupNum=0,bool GreaterThan=false,bool isExecutionTime=false,int filterNum=0) {
			gridQueries.BeginUpdate();
			gridQueries.ListGridRows.Clear();
			List<Query> listQueriesSorted=_queryLog.ListQueries.OrderByDescending(x => x.QueryExecutionTime).ToList();
			GridRow row;
			foreach(Query query in listQueriesSorted) {
				if(groupNum > 0 && query.QueryGroupNum!=groupNum) {//skip queries not in the group
					continue;
				}
				if(filterNum!=0) {//if there is a filter
					if(GreaterThan) {
						if(isExecutionTime) {
							if(query.QueryExecutionTime < filterNum) {
								continue;
							}
						}
						else {//Rows Examined
							if(query.RowsExamined < filterNum) {
								continue;
							}
						}
					}
					else {//less than
						if(isExecutionTime) {
							if(query.QueryExecutionTime > filterNum) {
								continue;
							}
						}
						else {//Rows Examined
							if(query.RowsExamined > filterNum) {
								continue;
							}
						}
					}
				}
				row=new GridRow();
				row.Cells.Add(query.QueryNum.ToString());
				row.Cells.Add(query.TimeRan.ToString());
				row.Cells.Add(query.QueryExecutionTime.ToString());
				row.Cells.Add(query.LockTime.ToString());
				row.Cells.Add(query.RowsExamined.ToString());
				row.Cells.Add(query.IsLikelyThirdParty ? "X" : "");
				row.Cells.Add(query.IsLikelyUserQuery ? "X" : "");
				row.Cells.Add(query.IsVictim ? "X" : "");
				row.Cells.Add(query.UnformattedQuery.Substring(0,(query.UnformattedQuery.Length > 75 ? 75 : query.UnformattedQuery.Length)));
				row.Tag=query;//Tag used to open changes in TortoiseSVN
				gridQueries.ListGridRows.Add(row);
			}
			gridQueries.EndUpdate();
			labelQueryCount.Text="Rows: "+_queryLog.ListQueries.Count;
		}

		private void FillGridQueryGroups() {
			gridQueryGroups.BeginUpdate();
			gridQueryGroups.ListGridRows.Clear();
			GridRow row;
			foreach(QueryGroup queryGroup in _queryLog.ListQueryGroups) {
				row=new GridRow();
				row.Cells.Add(queryGroup.QueryGroupNum.ToString());
				if(ODBuild.IsDebug()) {
					row.Cells.Add(queryGroup.GroupWeightRaw.ToString());
				}
				else {
					row.Cells.Add(queryGroup.GroupWeightRank.ToString());
				}
				row.Cells.Add(queryGroup.ListQueriesInGroup.Count.ToString());
				row.Cells.Add(queryGroup.AverageTimeBetweenQueries.ToString());
				row.Cells.Add(queryGroup.ExecutionTimeMedian.ToString());
				row.Cells.Add(queryGroup.ExecutionTimeMean.ToString());
				row.Cells.Add(queryGroup.RowsExaminedMedian.ToString());
				row.Cells.Add(queryGroup.ListQueriesInGroup[0].FormattedQuery.Substring(0,
					(queryGroup.ListQueriesInGroup[0].FormattedQuery.Length > 75 ? 75 : queryGroup.ListQueriesInGroup[0].FormattedQuery.Length)));
				row.Tag=queryGroup;//Tag used to open changes in TortoiseSVN
				gridQueryGroups.ListGridRows.Add(row);
			}
			gridQueryGroups.EndUpdate();
			labelQueryGroupCount.Text="Rows: "+_queryLog.ListQueryGroups.Count;
		}

		private void gridQueries_CellClick(object sender,ODGridClickEventArgs e) {
			if(gridQueries.ListGridRows.Count==0 || gridQueryGroups.ListGridRows.Count==0) {
				return;
			}
			Query query=(Query)gridQueries.ListGridRows[e.Row].Tag;
			QueryGroup queryGroup=_queryLog.ListQueryGroups.First(y => y.QueryGroupNum==query.QueryGroupNum);
			gridQueryGroups.SetAll(false);
			for(int i=0;i<gridQueryGroups.ListGridRows.Count;i++) {
				if(((QueryGroup)gridQueryGroups.ListGridRows[i].Tag).QueryGroupNum==queryGroup.QueryGroupNum) {
					gridQueryGroups.SetSelected(i,true);
					gridQueryGroups.ScrollToIndex(i);
					break;
				}
			}
		}

		private void gridQueries_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Query query=(Query)gridQueries.ListGridRows[e.Row].Tag;
			FormQuery formQuery=new FormQuery(query);
			formQuery.Show();
		}

		private void gridQueryGroups_CellClick(object sender,ODGridClickEventArgs e) {
			textFilterValue.Text="";
			QueryGroup queryGroup=(QueryGroup)gridQueryGroups.ListGridRows[e.Row].Tag;
			FillGridQueries(queryGroup.QueryGroupNum);
			gridQueries.ScrollToIndex(0);
		}

		private void gridQueryGroups_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			QueryGroup queryGroup=(QueryGroup)gridQueryGroups.ListGridRows[e.Row].Tag;
			FormQueryGroup formQueryGroup=new FormQueryGroup(queryGroup);
			formQueryGroup.Show();
		}

		private void butNone_Click(object sender,EventArgs e) {
			gridQueryGroups.SetAll(false);
			textFilterValue.Text="";
			FillGridQueries();
		}

		private void menuItemPerpetrator_Click(object sender,EventArgs e) {
			Query query=gridQueries.SelectedTag<Query>();
			FormQuery formQuery=new FormQuery(_queryLog.ListQueries.Find(x => x.QueryNum==query.Perpetrator));
			formQuery.Show();
		}

		private void contextMenuQuery_Opening(object sender,System.ComponentModel.CancelEventArgs e) {
			if(gridQueries.ListGridRows.Count==0) {
				menuItemPerpetrator.Visible=false;
				return;
			}
			Query query=gridQueries.SelectedTag<Query>();
			if(query==null) {
				menuItemPerpetrator.Visible=false;
				return;
			}
			menuItemPerpetrator.Visible=query.IsVictim;
		}

		private void listBoxFilter_SelectedIndexChanged(object sender,EventArgs e) {
			FilterQueries();
		}

		private void listBoxOptions_SelectedIndexChanged(object sender,EventArgs e) {
			FilterQueries();
		}

		private void textFilterValue_TextChanged(object sender,EventArgs e) {
			FilterQueries(true);
		}

		private void FilterQueries(bool textChanged = false) {
			//Set label
			if(listBoxFilter.SelectedIndex==0) {
				labelSeconds.Visible=true;
			}
			else if(listBoxFilter.SelectedIndex==1) {
				labelSeconds.Visible=false;
			}
			//Check to make sure data has been taken in
			if(_queryLog==null || _queryLog.ListQueries.Count==0) {
				return;
			}
			//See if it should be filtered
			if(listBoxFilter.SelectedIndex==-1||listBoxOptions.SelectedIndex==-1) {
				return;
			}
			QueryGroup queryGroup=gridQueryGroups.SelectedTag<QueryGroup>();
			if(textFilterValue.Text=="" && textChanged) {//refill grid if text was erased
				FillGridQueries(groupNum:(queryGroup==null ? 0 : queryGroup.QueryGroupNum));
			}
			//Otherwise filter if they have a valid number
			int num=0;
			try {
				num=PIn.Int(textFilterValue.Text);
			}
			catch {
				return;
			}
			FillGridQueries(groupNum: (queryGroup==null ? 0 : queryGroup.QueryGroupNum),GreaterThan: (listBoxOptions.SelectedIndex==0 ? true : false),
						isExecutionTime: (listBoxFilter.SelectedIndex==0 ? true : false),filterNum: num);
		}
	}
}
