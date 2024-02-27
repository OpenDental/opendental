using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Data;
using CodeBase;
using OpenDentBusiness.InternalTools.Job_Manager.HelperClasses;

namespace OpenDental.InternalTools.Job_Manager {
	public partial class UserControlTeamReport:UserControl {
		#region Fields/Properties
		private const string _SAVE_DIALOG_DEFAULT_EXTENSION="csv";
		private List<TeamReportUser> _listTeamReportUsers=new List<TeamReportUser>();
		private string _teamNameLastRun="";
		private DateTime _dateTimeFrom {
			get { return odDateRangePicker.GetDateTimeFrom(); }
		}
		private DateTime _dateTimeTo {
			get { return odDateRangePicker.GetDateTimeTo(); }
		}
		private JobTeam _jobTeamSelected {
			get { return comboTeamFilter.GetSelected<JobTeam>(); }
		}
		private List<JobTeam> _listJobTeamsAll {
			get { return comboTeamFilter.Items.GetAll<JobTeam>(); }
		}
		private string _fileNameExportDefault {
			get {
				if(_teamNameLastRun.IsNullOrEmpty()) {
					return "";
				}
				//example: AlphaTeam_summary_01-01-2023-01-07-2023.csv
				return _teamNameLastRun.Replace(" ","")
					+"_summary_"
					+_dateTimeFrom.ToString("MM-dd-yyyy")
					+"_"
					+_dateTimeTo.ToString("MM-dd-yyyy")
					+"."
					+_SAVE_DIALOG_DEFAULT_EXTENSION;
			}	
		}
		#endregion Fields/Properties

		#region Initialization
		public UserControlTeamReport() {
			InitializeComponent();
			InitializeDateFilter();
			InitializeTeamFilter();
			InitializeGridColumns();
		}

		private void InitializeDateFilter() {
			//Default range is previous full 7 days.
			odDateRangePicker.DefaultDateTimeFrom=DateTime.Today.AddDays(-7);
			odDateRangePicker.DefaultDateTimeTo=DateTime.Today.AddDays(-1);
		}

		private void InitializeTeamFilter() {
			comboTeamFilter.IncludeAll=true;
			comboTeamFilter.Items.AddList(JobTeams.GetDeepCopy(),x => x.TeamName);
			JobTeamUser jobTeamUserForCurUser=JobTeamUsers.GetFirstOrDefault(x => x.UserNumEngineer==Security.CurUser.UserNum);
			//Select the user's team if they are the team's lead.
			if(jobTeamUserForCurUser!=null && jobTeamUserForCurUser.IsTeamLead) {
				comboTeamFilter.SetSelectedKey<JobTeam>(jobTeamUserForCurUser.JobTeamNum,x => x.JobTeamNum);
			}
			//Otherwise, select All
			else {
				comboTeamFilter.IsAllSelected=true;
			}
		}

		private void InitializeGridColumns() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn column=new GridColumn("Job",650);
			gridMain.Columns.Add(column);
			column=new GridColumn("Team",140);
			gridMain.Columns.Add(column);
			column=new GridColumn("Status",210);
			gridMain.Columns.Add(column);
			column=new GridColumn("Priority",100,HorizontalAlignment.Right);
			gridMain.Columns.Add(column);
			column=new GridColumn("% Total Hours",100,HorizontalAlignment.Right);
			gridMain.Columns.Add(column);
			column=new GridColumn("Discussion Prompt",150);
			gridMain.Columns.Add(column);
			gridMain.EndUpdate();
		}
		#endregion Initialization

		#region Run Report/Summary
		private void butRefresh_Click(object sender,EventArgs e) {
			RunReport();
		}

		private void butSummary_Click(object sender,EventArgs e) {
			//We have no data and running the report produced no data.
			if(_listTeamReportUsers.IsNullOrEmpty() && !RunReport()) { 
				return;
			}
            using FormTeamSummary formJobTeamSummary=new FormTeamSummary(
				_teamNameLastRun,
				_dateTimeFrom,
				_dateTimeTo,
				_listTeamReportUsers
			);
			formJobTeamSummary.ShowDialog();
		}

		///<summary>Returns true if the report ran. Shows message and returns false on failure.</summary>
		private bool RunReport() {
			List<JobTeam> listJobTeamsSelected=GetJobTeamsFromSelection();
			if(listJobTeamsSelected.IsNullOrEmpty() || listJobTeamsSelected.FirstOrDefault()==null)
			{
				MsgBox.Show("Select a team first");
				return false;
			}
			_listTeamReportUsers=TeamReportUser.CreateListForTeam(listJobTeamsSelected,_dateTimeFrom,_dateTimeTo,_listJobTeamsAll);
			if( _listTeamReportUsers.IsNullOrEmpty()) {
				MsgBox.Show("The team has no users or something went wrong while running the report.");
				return false;
			}
			FillGridRows();
			_teamNameLastRun=(comboTeamFilter.IsAllSelected) ? "All" : _jobTeamSelected.TeamName;
			return true;
		}

		private List<JobTeam> GetJobTeamsFromSelection() {
			if(comboTeamFilter.IsAllSelected) {
				return _listJobTeamsAll;
			}
			else {
				return new List<JobTeam>(){ _jobTeamSelected };
			}
		}

		private void FillGridRows() {
			gridMain.BeginUpdate();
			gridMain.ListGridRows.Clear();
			gridMain.Title=(comboTeamFilter.IsAllSelected) ? "All Teams" : _jobTeamSelected.TeamName;
			foreach(TeamReportUser user in _listTeamReportUsers) {
				gridMain.ListGridRows.AddRange(user.CreateGridRows(checkNoLogsThirtyDays.Checked,_dateTimeFrom));
			}
			gridMain.EndUpdate();
		}
		#endregion Run Report/Summary

		#region Export
		private void butExport_Click(object sender,EventArgs e) {
			if(_listTeamReportUsers.IsNullOrEmpty()) {
				MsgBox.Show("Report hasn't been run, the team has no users, or something went wrong while running the report.");
				return;
			}
			try {
				ExportReportPrompt();
			}
			catch(Exception ex) {
				MsgBox.Show($"Error creating report file: {ex.Message}");
			}
		}

		///<summary>Throws exceptions, surround with try/catch.</summary>
		private void ExportReportPrompt() {
			SaveFileDialog saveFileDialog=CreateSaveFileDialog();
			if(saveFileDialog.ShowDialog()!=DialogResult.OK) {
				return;
			}
			WriteGridMainToFile(saveFileDialog.FileName);
		}

		private SaveFileDialog CreateSaveFileDialog() {
			using SaveFileDialog saveFileDialog=new SaveFileDialog();
			saveFileDialog.AddExtension=true;
			saveFileDialog.DefaultExt=_SAVE_DIALOG_DEFAULT_EXTENSION;
			saveFileDialog.FileName=_fileNameExportDefault;
			if(!Directory.Exists(PrefC.GetString(PrefName.ExportPath))) {
				try {
					Directory.CreateDirectory(PrefC.GetString(PrefName.ExportPath));
					saveFileDialog.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
				}
				catch {
					//initial directory will be blank
				}
			}
			else {
				saveFileDialog.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
			}
			saveFileDialog.Filter="CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
			saveFileDialog.FilterIndex=1;//1 based index
			return saveFileDialog;
		}

		///<summary>Throws exceptions. Allowed to bubble up to event handlers.</summary>
		private void WriteGridMainToFile(string fileName) {
			using StreamWriter streamWriter=new StreamWriter(fileName,append:false);
			List<string> listHeaders=gridMain.Columns.Select(column => column.Heading).ToList();
			streamWriter.WriteLine(string.Join(",", listHeaders));
			foreach(GridRow row in gridMain.ListGridRows) {
				List<string> listCellTexts=row.Cells.Select(cell => QuoteCsvField(cell.Text)).ToList();
				streamWriter.WriteLine(string.Join(",",listCellTexts));
			}
		}

		///<summary>Rules 5,6, and 7 on this page (https://super-csv.github.io/super-csv/csv_specification.html)
		///explain the use of double quotes around fields that contain certain characters.</summary>
		private string QuoteCsvField(string text) {
			// Check if the text contains commas, double quotes, or line breaks
			if (text.Contains(",") || text.Contains("\"") || text.Contains("\n") || text.Contains("\r")) {
				// Escape existing double quotes by replacing them with two double quotes
				text = text.Replace("\"", "\"\"");
				// Enclose the text in double quotes
				return $"\"{text}\"";
			}
			return text;
		}
		#endregion Export

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Job job=(Job)gridMain.ListGridRows[e.Row]?.Tag;
			if(job==null) {
				return;
			}
			Jobs.FillInMemoryLists(new List<Job>() { job });
			FormJobEdit formJobEdit=new FormJobEdit(job);
			formJobEdit.Show();
		}
	}
}
