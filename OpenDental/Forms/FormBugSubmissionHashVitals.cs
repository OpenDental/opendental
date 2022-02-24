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
		private List<BugSubmissionHash> _listHashes;
		///<summary>Key => BugSubmissionHashNum & Value => List of BugSubmissions</summary>
		private Dictionary<long,List<BugSubmission>> _dictBugSubsByHashNum;
		///<summary>Key => BugId & Value => Bug</summary>
		private Dictionary<long,Bug> _dictBugs;
		///<summary>Key => RegKey & Value => Patient</summary>
		private Dictionary<string,Patient> _dictPatients;

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
			_listHashes=BugSubmissionHashes.GetMany(datePicker.GetDateTimeFrom(),datePicker.GetDateTimeTo());
			_dictBugSubsByHashNum=BugSubmissions.GetForHashNums(_listHashes.Select(x => x.BugSubmissionHashNum).ToList());
			_dictBugs=Bugs.GetMany(_listHashes.Select(x => x.BugId).Where(x => x!=0).ToList()).ToDictionary(key => key.BugId);
			_dictPatients=RegistrationKeys.GetPatientsByKeys(_dictBugSubsByHashNum.Values.SelectMany(x => x.Select(y => y.RegKey)).ToList());
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
					_listHashes.ForEach(x => {
						if(PassesHashFilter(x)){
							GridRow gridRow=new GridRow();
							gridRow.Cells.Add(x.BugSubmissionHashNum.ToString());
							gridRow.Cells.Add(x.BugId.ToString());
							gridRow.Cells.Add(x.FullHash);
							gridRow.Cells.Add(x.PartialHash);
							gridRow.Tag=x;
							gridHashes.ListGridRows.Add(gridRow);
							if(_dictBugs.TryGetValue(x.BugId,out Bug bug)){
								if(bug.VersionsFixed!=""){
									gridHashes.ListGridRows.Last().ColorBackG=Color.LightGreen;
								}
							}
						}
					});
					#endregion
					break;
				case GroupingOptions.PartialHash:
					#region PartialHash grouping
					_listHashes.GroupBy(x => x.PartialHash).ForEach(x => { 
						if(PassesHashFilter(x.First())){
							GridRow gridRow=new GridRow();
							gridRow.Cells.Add($"#({x.Count()})");
							gridRow.Cells.Add($"({x.DistinctBy(y => y.BugId).Count()})");
							gridRow.Cells.Add($"#({x.DistinctBy(y => y.FullHash).Count()})");
							gridRow.Cells.Add($"{x.First().PartialHash}");
							gridRow.Tag=x.ToList();
							gridHashes.ListGridRows.Add(gridRow);
							if(_dictBugs.TryGetValue(x.First().BugId,out Bug bug)){
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
					_listHashes.GroupBy(x => x.BugId).ForEach(x => { 
						if(PassesHashFilter(x.First())){
							GridRow gridRow=new GridRow();
							gridRow.Cells.Add($"#({x.Count()})");
							gridRow.Cells.Add($"{x.First().BugId}");
							gridRow.Cells.Add($"#({x.DistinctBy(y => y.FullHash).Count()})");
							gridRow.Cells.Add($"{x.DistinctBy(y => y.PartialHash).Count()}");
							gridRow.Tag=x.ToList();
							gridHashes.ListGridRows.Add(gridRow);
							if(_dictBugs.TryGetValue(x.First().BugId,out Bug bug)){
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
			GridColumn col=new GridColumn("Vers. Sub",100) { IsWidthDynamic=true };
			gridHashData.ListGridColumns.Add(col);
			col=new GridColumn("Count Reg",100) { IsWidthDynamic=true };
			gridHashData.ListGridColumns.Add(col);
			col=new GridColumn("Total",100) { IsWidthDynamic=true };
			gridHashData.ListGridColumns.Add(col);
			col=new GridColumn("Vers. Fixed",100) { IsWidthDynamic=true };
			gridHashData.ListGridColumns.Add(col);
			gridHashData.ListGridRows.Clear();
			gridHashData.EndUpdate();
			//gridSubs Grid Update
			gridSubs.BeginUpdate();
			gridSubs.ListGridColumns.Clear();
			col=new GridColumn("DateTime",100) { IsWidthDynamic=true };
			gridSubs.ListGridColumns.Add(col);
			col=new GridColumn("Submitter",100) { IsWidthDynamic=true };
			gridSubs.ListGridColumns.Add(col);
			col=new GridColumn("Msg Text",100) { IsWidthDynamic=true };
			gridSubs.ListGridColumns.Add(col);
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
			Dictionary<DateTime,List<BugSubmissionHash>> dictHashDates=_listHashes.GroupBy(x => x.DateTimeEntry.Date)
				.ToDictionary(x => x.Key,x => x.ToList());
			chartVitals.ChartAreas[0].AxisX.Minimum=datePicker.GetDateTimeFrom().ToOADate();
			chartVitals.ChartAreas[0].AxisX.Maximum=datePicker.GetDateTimeTo().ToOADate();
			DateTime dateCur=datePicker.GetDateTimeFrom();
			while(dateCur<=datePicker.GetDateTimeTo()){
				int yVal=0;
				if(dictHashDates.TryGetValue(dateCur,out List<BugSubmissionHash> listSubs)){
					yVal=listSubs.Count;
				}
				chartVitals.Series[0].Points.AddXY(dateCur,yVal);
				dateCur=dateCur.AddDays(1);
			}
		}

		///<summary>When grid is clicked, show bug submissions grouped by version in Hash Data grid.</summary>
		private void gridHashes_CellClick(object sender,UI.ODGridClickEventArgs e) {
			Dictionary<string,List<BugSubmission>> dictSubStackTraces;	
			object gridTagObj=gridHashes.ListGridRows[e.Row].Tag;
			if(gridTagObj is BugSubmissionHash){//No Grouping
				dictSubStackTraces=_dictBugSubsByHashNum[(gridTagObj as BugSubmissionHash).BugSubmissionHashNum]
					.GroupBy(x => x.ProgramVersion)
					.ToDictionary(x => x.Key,x => x.ToList());
			}
			else if(gridTagObj is List<BugSubmissionHash>){
				dictSubStackTraces=new Dictionary<string, List<BugSubmission>>();
				foreach(BugSubmissionHash hash in (gridTagObj as List<BugSubmissionHash>)) {
					_dictBugSubsByHashNum[hash.BugSubmissionHashNum]
						.GroupBy(x => x.ProgramVersion)
						.ForEach(x => {
							if(dictSubStackTraces.ContainsKey(x.Key)) {
								dictSubStackTraces[x.Key].AddRange(x.ToList());
							}
							else {
								dictSubStackTraces.Add(x.Key,x.ToList());
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
			foreach(string key in dictSubStackTraces.Keys) {
				BugSubmission sub=dictSubStackTraces[key].OrderBy(x => new Version(x.ProgramVersion)).Last();//Most recent version.
				BugSubmissionHashes.ProcessSubmission(sub
					,out long matchedBugId,out string matchedFixedVersions,out long matchedBugSubmissionHashNum
					,useConnectionStore:false
				);
				string versionSubmitted=new Version(sub.ProgramVersion).ToString();
				string countRegKeys=dictSubStackTraces[key].Select(x => x.RegKey).Distinct().Count().ToString();
				string countTotalSubs=dictSubStackTraces[key].Count().ToString();
				GridRow gridRow=new GridRow();
				gridRow.Cells.Add(versionSubmitted);
				gridRow.Cells.Add(countRegKeys);
				gridRow.Cells.Add(countTotalSubs);
				gridRow.Cells.Add(matchedFixedVersions??"");
				gridRow.Tag=dictSubStackTraces[key];
				gridHashData.ListGridRows.Add(gridRow);
			}
			gridHashData.EndUpdate();
			gridSubs.BeginUpdate();
			gridSubs.ListGridRows.Clear();
			gridSubs.EndUpdate();
		}

		///<summary>When grid is clicked, show individual bug submissions in Submissions grid.</summary>
		private void gridHashData_CellClick(object sender,UI.ODGridClickEventArgs e) {
			List<BugSubmission> listSelectedSubmissions=gridHashData.SelectedTag<List<BugSubmission>>();
			gridSubs.BeginUpdate();
			gridSubs.ListGridRows.Clear();
			listSelectedSubmissions.ForEach(x => {
				string submitterName=x.RegKey;
				if(_dictPatients.TryGetValue(x.RegKey,out Patient pat)){
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
			using(FormBugSubmission form=new FormBugSubmission(gridSubs.SelectedTag<BugSubmission>())){
				form.ShowDialog();
			}
		}

		private void butRefreshSearch_Click(object sender,EventArgs e) {
			RefreshAll();
		}
		
		///<summary>When clicked, allows user to past stacktrace and enter version to attempt to find hash and matched info.</summary>
		private void butCheckHash_Click(object sender,EventArgs e) {
			using InputBox input=new InputBox("Please paste a stack trace",true);
			if(input.ShowDialog()!=DialogResult.OK){
				return;
			}
			BugSubmission sub=new BugSubmission(){ 
				ExceptionStackTrace=input.textResult.Text.Replace("\r\n","\n"),
			};
			using InputBox input2=new InputBox("Please enter a version like: 19.2.1.0");
			if(input2.ShowDialog()!=DialogResult.OK || !Version.TryParse(input2.textResult.Text,out Version version)){
				return;
			}
			BugSubmissionHashes.ProcessSubmission(sub
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