using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormJobEstimate:FormODBase {
		public Job JobCur;

		///<summary></summary>
		public FormJobEstimate(Job jobCur) {
			JobCur=jobCur;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormJobEstimate_Load(object sender,EventArgs e) {
			textConcept.Text=JobCur.HoursEstimateConcept.ToString();
			textWriteup.Text=JobCur.HoursEstimateWriteup.ToString();
			textDevelopment.Text=JobCur.HoursEstimateDevelopment.ToString();
			textReview.Text=JobCur.HoursEstimateReview.ToString();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textConcept.IsValid()
				|| !textWriteup.IsValid()
				|| !textDevelopment.IsValid()
				|| !textReview.IsValid())
			{
				MsgBox.Show(this,"Please enter valid hour estimates before attempting to save.");
				return;
			}
			Job jobOld=JobCur.Copy();
			JobCur.HoursEstimateConcept=PIn.Double(textConcept.Text);
			JobCur.HoursEstimateWriteup=PIn.Double(textWriteup.Text);
			JobCur.HoursEstimateDevelopment=PIn.Double(textDevelopment.Text);
			JobCur.HoursEstimateReview=PIn.Double(textReview.Text);
			if(JobLogs.MakeLogEntryForEstimateChange(JobCur,jobOld,textNote.Text)) {
				DialogResult=DialogResult.OK;
			}
			else {
				DialogResult=DialogResult.Cancel;
			}
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel; //removing new jobs from the DB is taken care of in FormClosing
		}
	}
}