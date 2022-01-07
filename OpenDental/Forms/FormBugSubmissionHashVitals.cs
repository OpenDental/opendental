using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormBugSubmissionHashVitals:FormODBase {
	
		///<summary>List of all BugSubmissionHashes in date range.</summary>
		private List<BugSubmissionHash> _listBugSubmissionHashes;
		///<summary>Key => BugSubmissionHashNum & Value => List of BugSubmissions</summary>
		private Dictionary<long,List<BugSubmission>> _dictionaryBugSubmissionsByHashNum;
		///<summary>Key => BugId & Value => Bug</summary>
		private Dictionary<long,Bug> _dictionaryBugs;
		///<summary>Key => RegKey & Value => Patient</summary>
		private Dictionary<string,Patient> _dictionaryPatients;

		public FormBugSubmissionHashVitals() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}
		
		private void FormBugSubmissionHashVitals_Load(object sender,EventArgs e) {
			base.SetFilterControlsAndAction(() => RefreshGrids(),textHashNum,textFullHash,textPartHash,textBugIds,datePicker,comboGrouping);
			datePicker.SetDateTimeFrom(DateTime.Today.AddMonths(-3));
			datePicker.SetDateTimeTo(DateTime.Today);
			Enum.GetValues(typeof(GroupingOptions)).OfType<GroupingOptions>().ForEach(x => comboGrouping.Items.Add(x));
			comboGrouping.SelectedIndex=0;//None
			RefreshAll();
		}

		///<summary>Refreshes the form data, then grids, then chart.</summary>
		private void RefreshAll(){
			RefreshData();
			RefreshGrids();
			RefreshChart();
		}

		///<summary>Reloads all data for form.</summary>
		private void RefreshData() {
			_listBugSubmissionHashes=BugSubmissionHashes.GetMany(datePicker.GetDateTimeFrom(),datePicker.GetDateTimeTo());
			_dictionaryBugSubmissionsByHashNum=BugSubmissions.GetForHashNums(_listBugSubmissionHashes.Select(x => x.BugSubmissionHashNum).ToList());
			_dictionaryBugs=Bugs.GetMany(_listBugSubmissionHashes.Select(x => x.BugId).Where(x => x!=0).ToList()).ToDictionary(key => key.BugId);
			_dictionaryPatients=RegistrationKeys.GetPatientsByKeys(_dictionaryBugSubmissionsByHashNum.Values.SelectMany(x => x.Select(y => y.RegKey)).ToList());
		}

		///<summary>Clears all rows and columns from every grid. Only fills gridHashes with _listHashes based on grouping option.</summary>
		private void RefreshGrids(){
			//gridHashes Update
			gridHashes.BeginUpdate();
			gridHashes.ListGridColumns.Clear();
			gridHashes.ListGridColumns.Add(new GridColumn("HashNum",100));
			gridHashes.ListGridColumns.Add(new GridColumn("BugId",100));
			gridHashes.ListGridColumns.Add(new GridColumn("FullHash",100){IsWidthDynamic=true });
			gridHashes.ListGridColumns.Add(new GridColumn("PartialHash",100){IsWidthDynamic=true });
			gridHashes.ListGridRows.Clear();
			switch(comboGrouping.SelectedItem){
				default:
				case GroupingOptions.None:
					#region No grouping
					_listBugSubmissionHashes.ForEach(x => {
						if(PassesHashFilter(x)){
							GridRow row=new GridRow();
							row.Cells.Add(x.BugSubmissionHashNum.ToString());
							row.Cells.Add(x.BugId.ToString());
							row.Cells.Add(x.FullHash);
							row.Cells.Add(x.PartialHash);
							row.Tag=x;
							gridHashes.ListGridRows.Add(row);
							if(_dictionaryBugs.TryGetValue(x.BugId,out Bug bug)){
								if(bug.VersionsFixed!=""){
									gridHashes.ListGridRows.Last().ColorBackG=Color.LightGreen;
								}
							}
						}
					});
					#endregion No grouping
					break;
				case GroupingOptions.PartialHash:
					#region PartialHash grouping
					_listBugSubmissionHashes.GroupBy(x => x.PartialHash).ForEach(x => { 
						if(PassesHashFilter(x.First())){
							GridRow row=new GridRow();
							row.Cells.Add($"#({x.Count()})");
							row.Cells.Add($"({x.DistinctBy(y => y.BugId).Count()})");
							row.Cells.Add($"#({x.DistinctBy(y => y.FullHash).Count()})");
							row.Cells.Add($"{x.First().PartialHash}");
							row.Tag=x.ToList();
							gridHashes.ListGridRows.Add(row);
							if(_dictionaryBugs.TryGetValue(x.First().BugId,out Bug bug)){
								if(bug.VersionsFixed!=""){
									gridHashes.ListGridRows.Last().ColorBackG=Color.LightGreen;
								}
							}
						}
					});
					#endregion
					break;
				case GroupingOptions.BugId:
					#region BugId grouping
					_listBugSubmissionHashes.GroupBy(x => x.BugId).ForEach(x => { 
						if(PassesHashFilter(x.First())){
							GridRow row=new GridRow();
							row.Cells.Add($"#({x.Count()})");
							row.Cells.Add($"{x.First().BugId}");
							row.Cells.Add($"#({x.DistinctBy(y => y.FullHash).Count()})");
							row.Cells.Add($"{x.DistinctBy(y => y.PartialHash).Count()}");
							row.Tag=x.ToList();
							gridHashes.ListGridRows.Add(row);
							if(_dictionaryBugs.TryGetValue(x.First().BugId,out Bug bug)){
								if(bug.VersionsFixed!=""){
									gridHashes.ListGridRows.Last().ColorBackG=Color.LightGreen;
								}
							}
						}
					});
					#endregion
					break;
			}
			gridHashes.EndUpdate();
			//gridHashData Grid Update
			gridHashData.BeginUpdate();
			gridHashData.ListGridColumns.Clear();
			GridColumn column=new GridColumn("Vers. Sub",100) { IsWidthDynamic=true };
			gridHashData.ListGridColumns.Add(column);
			column=new GridColumn("Count Reg",100) { IsWidthDynamic=true };
			gridHashData.ListGridColumns.Add(column);
			column=new GridColumn("Total",100) { IsWidthDynamic=true };
			gridHashData.ListGridColumns.Add(column);
			column=new GridColumn("Vers. Fixed",100) { IsWidthDynamic=true };
			gridHashData.ListGridColumns.Add(column);
			gridHashData.ListGridRows.Clear();
			gridHashData.EndUpdate();
			//gridSubs Grid Update
			gridSubs.BeginUpdate();
			gridSubs.ListGridColumns.Clear();
			column=new GridColumn("DateTime",100) { IsWidthDynamic=true };
			gridSubs.ListGridColumns.Add(column);
			column=new GridColumn("Submitter",100) { IsWidthDynamic=true };
			gridSubs.ListGridColumns.Add(column);
			column=new GridColumn("Msg Text",100) { IsWidthDynamic=true };
			gridSubs.ListGridColumns.Add(column);
			gridSubs.ListGridRows.Clear();
			gridSubs.EndUpdate();
		}

		///<summary>Returns true if given hash passes filter validation.</summary>
		private bool PassesHashFilter(BugSubmissionHash hash){
			bool isHashNumFilterPassed=(textHashNum.Text.IsNullOrEmpty() || hash.BugSubmissionHashNum.ToString().Contains(textHashNum.Text));
			bool isFullHashFilterPassed=(textFullHash.Text.IsNullOrEmpty() || hash.FullHash.ToLower().Contains(textFullHash.Text.ToLower()));
			bool isPartHashFilterPassed=(textPartHash.Text.IsNullOrEmpty() || hash.FullHash.ToLower().Contains(textPartHash.Text.ToLower()));
			bool isBugIdFilterPassed=(textBugIds.Text.IsNullOrEmpty() || hash.BugId.ToString().Contains(textBugIds.Text));
			return (isHashNumFilterPassed && isFullHashFilterPassed && isPartHashFilterPassed && isBugIdFilterPassed);
		}

		///<summary>Clears and loads line chart into view based on hashes in date range.</summary>
		private void RefreshChart() {
			chartVitals.ChartAreas[0].AxisX.LabelStyle.Format = "yyyy-MM-dd";
			chartVitals.Series[0].Points.Clear();
			Dictionary<DateTime,List<BugSubmissionHash>> dictionaryHashDatesListsBugSubmissions=_listBugSubmissionHashes.GroupBy(x => x.DateTimeEntry.Date)
				.ToDictionary(x => x.Key,x => x.ToList());
			chartVitals.ChartAreas[0].AxisX.Minimum=datePicker.GetDateTimeFrom().ToOADate();
			chartVitals.ChartAreas[0].AxisX.Maximum=datePicker.GetDateTimeTo().ToOADate();
			DateTime dateTime=datePicker.GetDateTimeFrom();
			while(dateTime<=datePicker.GetDateTimeTo()){
				int yValue=0;
				if(dictionaryHashDatesListsBugSubmissions.TryGetValue(dateTime,out List<BugSubmissionHash> listSubs)){
					yValue=listSubs.Count;
				}
				chartVitals.Series[0].Points.AddXY(dateTime,yValue);
				dateTime=dateTime.AddDays(1);
			}
		}

		///<summary>When grid is clicked, show bug submissions grouped by version in Hash Data grid.</summary>
		private void gridHashes_CellClick(object sender,UI.ODGridClickEventArgs e) {
			Dictionary<string,List<BugSubmission>> dictionarySubStackTraces;	
			object gridTagObj=gridHashes.ListGridRows[e.Row].Tag;
			if(gridTagObj is BugSubmissionHash){//No Grouping
				dictionarySubStackTraces=_dictionaryBugSubmissionsByHashNum[(gridTagObj as BugSubmissionHash).BugSubmissionHashNum]
					.GroupBy(x => x.ProgramVersion)
					.ToDictionary(x => x.Key,x => x.ToList());
			}
			else if(gridTagObj is List<BugSubmissionHash>){
				dictionarySubStackTraces=new Dictionary<string, List<BugSubmission>>();
				foreach(BugSubmissionHash bugSubmissionHash in (gridTagObj as List<BugSubmissionHash>)) {
					_dictionaryBugSubmissionsByHashNum[bugSubmissionHash.BugSubmissionHashNum]
						.GroupBy(x => x.ProgramVersion)
						.ForEach(x => {
							if(dictionarySubStackTraces.ContainsKey(x.Key)) {
								dictionarySubStackTraces[x.Key].AddRange(x.ToList());
							}
							else {
								dictionarySubStackTraces.Add(x.Key,x.ToList());
							}
					});
				}
			}
			else {
				MsgBox.Show("Error loading Hash Details: Unknown Tag");
				return;
			}
			gridHashData.BeginUpdate();
			gridHashData.ListGridRows.Clear();
			foreach(string key in dictionarySubStackTraces.Keys) {
				BugSubmission bugSubmission=dictionarySubStackTraces[key].OrderBy(x => new Version(x.ProgramVersion)).Last();//Most recent version.
				BugSubmissionHashes.ProcessSubmission(bugSubmission
					,out long matchedBugId,out string matchedFixedVersions,out long matchedBugSubmissionHashNum
					,useConnectionStore:false
				);
				string versionSubmitted=new Version(bugSubmission.ProgramVersion).ToString();
				string countRegKeys=dictionarySubStackTraces[key].Select(x => x.RegKey).Distinct().Count().ToString();
				string countTotalSubs=dictionarySubStackTraces[key].Count().ToString();
				GridRow gridRow=new GridRow();
				gridRow.Cells.Add(versionSubmitted);
				gridRow.Cells.Add(countRegKeys);
				gridRow.Cells.Add(countTotalSubs);
				gridRow.Cells.Add(matchedFixedVersions??"");
				gridRow.Tag=dictionarySubStackTraces[key];
				gridHashData.ListGridRows.Add(gridRow);
			}
			gridHashData.EndUpdate();
			gridSubs.BeginUpdate();
			gridSubs.ListGridRows.Clear();
			gridSubs.EndUpdate();
		}

		///<summary>When grid is clicked, show individual bug submissions in Submissions grid.</summary>
		private void gridHashData_CellClick(object sender,UI.ODGridClickEventArgs e) {
			List<BugSubmission> listBugSubmissionsSelected=gridHashData.SelectedTag<List<BugSubmission>>();
			gridSubs.BeginUpdate();
			gridSubs.ListGridRows.Clear();
			listBugSubmissionsSelected.ForEach(x => {
				string submitterName=x.RegKey;
				if(_dictionaryPatients.TryGetValue(x.RegKey,out Patient pat)){
					submitterName=pat.GetNameLF();
				}
				GridRow gridRow=new GridRow();
				gridRow.Cells.Add(x.SubmissionDateTime.ToString());
				gridRow.Cells.Add(submitterName);
				gridRow.Cells.Add(x.ExceptionMessageText);
				gridRow.Tag=x;
				gridSubs.ListGridRows.Add(gridRow);
			});
			gridSubs.EndUpdate();
		}

		///<summary>When grid is double clicked, open bug submission form.</summary>
		private void gridSubs_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			using(FormBugSubmission formBugSubmission=new FormBugSubmission(gridSubs.SelectedTag<BugSubmission>())){
				formBugSubmission.ShowDialog();
			}
		}

		private void butRefreshSearch_Click(object sender,EventArgs e) {
			RefreshAll();
		}
		
		///<summary>When clicked, allows user to past stacktrace and enter version to attempt to find hash and matched info.</summary>
		private void butCheckHash_Click(object sender,EventArgs e) {
			using InputBox inputBox=new InputBox("Please paste a stack trace",true);
			if(inputBox.ShowDialog()!=DialogResult.OK){
				return;
			}
			BugSubmission bugSubmission=new BugSubmission(){ 
				ExceptionStackTrace=inputBox.textResult.Text.Replace("\r\n","\n"),
			};
			using InputBox inputBox2=new InputBox("Please enter a version like: 19.2.1.0");
			if(inputBox2.ShowDialog()!=DialogResult.OK || !Version.TryParse(inputBox2.textResult.Text,out Version version)){
				return;
			}
			BugSubmissionHashes.ProcessSubmission(bugSubmission
				,out long matchedBugId,out string matchedFixedVersions,out long matchedBugSubmissionHashNum
				,version,false
			);
			MsgBox.Show($"MatchedBugId: {matchedBugId}\r\nMatchedFixedVersions: {matchedFixedVersions}\r\nMatchedBugSubmissionHashNum: {matchedBugSubmissionHashNum}");
		}

		///<summary>Enum defining what is shown in comboGrouping.</summary>
		private enum GroupingOptions {
			None,
			PartialHash,
			BugId
		}

	}
}