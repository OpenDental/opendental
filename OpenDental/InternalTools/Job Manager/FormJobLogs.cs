using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	public partial class FormJobLogs:FormODBase {
		private Job _jobCur;
		///<summary>A list of bugs, features, and tasks related to this job.</summary>
		private List<JobLog> _jobLogs;

		///<summary>Opens with links to the passed in JobNum.</summary>
		public FormJobLogs(Job jobCur) {
			_jobCur=jobCur;
			InitializeComponent();
			InitializeLayoutManager(isLayoutMS:true);
			Lan.F(this);
		}

		private void FormJobLogs_Load(object sender,EventArgs e) {
			this.Text="Job Logs For: "+_jobCur.ToString();
			_jobLogs=JobLogs.GetJobLogsForJobNum(_jobCur.JobNum);
			FillGridLog();
		}

		private void FillGridLog() {
			gridLog.BeginUpdate();
			gridLog.ListGridColumns.Clear();
			gridLog.ListGridColumns.Add(new GridColumn("Date",80));
			gridLog.ListGridColumns.Add(new GridColumn("User",60) { TextAlign=HorizontalAlignment.Center });
			gridLog.ListGridColumns.Add(new GridColumn("Expert",60) { TextAlign=HorizontalAlignment.Center });
			gridLog.ListGridColumns.Add(new GridColumn("Engineer",60) { TextAlign=HorizontalAlignment.Center });
			gridLog.ListGridColumns.Add(new GridColumn("Conc",35) { TextAlign=HorizontalAlignment.Center });//shows X if this row has a copy of job description text.
			gridLog.ListGridColumns.Add(new GridColumn("W/U",35) { TextAlign=HorizontalAlignment.Center });//shows X if this row has a copy of job description text.
			gridLog.ListGridColumns.Add(new GridColumn("HrEst",35) { TextAlign=HorizontalAlignment.Center });
			gridLog.ListGridColumns.Add(new GridColumn("Title",60){ IsWidthDynamic=true });
			gridLog.ListGridColumns.Add(new GridColumn("Description",60){ IsWidthDynamic=true });
			gridLog.ListGridRows.Clear();
			gridLog.NoteSpanStart=1;
			gridLog.NoteSpanStop=7;
			GridRow row;
			RichTextBox textRequirements = new RichTextBox();
			RichTextBox textImplementation = new RichTextBox();
			foreach(JobLog jobLog in _jobLogs.OrderBy(x=>x.DateTimeEntry)) {
				row=new GridRow();
				row.Cells.Add(jobLog.DateTimeEntry.ToShortDateString()+" "+jobLog.DateTimeEntry.ToShortTimeString());
				row.Cells.Add(Userods.GetName(jobLog.UserNumChanged));
				row.Cells.Add(Userods.GetName(jobLog.UserNumExpert));
				row.Cells.Add(Userods.GetName(jobLog.UserNumEngineer));
				textRequirements.Clear();
				textImplementation.Clear();
				try {
					textRequirements.Rtf=jobLog.RequirementsRTF;
				}
				catch {
					//fail silently
				}
				try {
					textImplementation.Rtf=jobLog.MainRTF;
				}
				catch {
					//fail silently
				}
				if(checkShowHistoryText.Checked && (!string.IsNullOrWhiteSpace(textImplementation.Text) || !string.IsNullOrWhiteSpace(textRequirements.Text))) {
					row.Note=textRequirements.Text+"\r\n------------------------------\r\n"+textImplementation.Text;//This is ok because a requirements entry should always precede an implementation entry... from now on
				}
				row.Cells.Add(string.IsNullOrWhiteSpace(textRequirements.Text) ? "" : "X");
				row.Cells.Add(string.IsNullOrWhiteSpace(textImplementation.Text) ? "" : "X");
				row.Cells.Add(jobLog.HoursEstimate.ToString());
				row.Cells.Add(String.IsNullOrEmpty(jobLog.Title)?"No Title Stored":jobLog.Title);
				row.Cells.Add(jobLog.Description);
				if(checkShowHistoryText.Checked && gridLog.ListGridRows.Count%2==1) {
					row.ColorBackG=Color.FromArgb(245,251,255);//light blue every other row.
				}
				row.Tag=jobLog;
				gridLog.ListGridRows.Add(row);
			}
			textRequirements.Dispose();
			textImplementation.Dispose();
			gridLog.EndUpdate();
		}

		private void checkShowHistoryText_CheckedChanged(object sender,EventArgs e) {
			FillGridLog();
		}
		
		private void gridLog_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if((string.IsNullOrWhiteSpace(gridLog.ListGridRows[e.Row].Cells[4].Text) && string.IsNullOrWhiteSpace(gridLog.ListGridRows[e.Row].Cells[5].Text)) //because JobLog.MainRTF is not an empty string when it is blank.
				|| !(gridLog.ListGridRows[e.Row].Tag is JobLog)) 
			{
				return;
			}
			JobLog jobLog = (JobLog)gridLog.ListGridRows[e.Row].Tag;
			RichTextBox rtfBox=new RichTextBox();
			rtfBox.Rtf=jobLog.RequirementsRTF;
			rtfBox.AppendText("\r\n-----------------------\r\n");
			rtfBox.Select(rtfBox.TextLength,0);
			rtfBox.SelectedRtf = jobLog.MainRTF;
			using FormSpellChecker FormSC = new FormSpellChecker();
			FormSC.SetText(rtfBox.Rtf);
			FormSC.ShowDialog();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
			Close();
		}

	}
}