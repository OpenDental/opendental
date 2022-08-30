using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	public partial class JobManagerUserOverview:UserControl {
		private Userod _user;
		//public bool MouseIsOver {
		//	get {
		//		return gridJobs.MouseIsOver;
		//	}
		//}

		public JobManagerUserOverview(Color color) {
			InitializeComponent();
			labelActiveJobs.BackColor=color;
			this.BackColor=color;
		}

		public Userod User {
			get {
				return _user;
			}
			set {
				if(value!=null) {
					_user=value;
					groupOverview.Text=_user.UserName+" Overview";
					labelActiveJobs.Text=_user.UserName+" Active Jobs";
				}
			}
		}

		private void JobManagerUserOverview_Load(object sender,EventArgs e) {

		}

		public void FillControls(List<Job> listJobs) {
			List<Job> listJobsFiltered = listJobs.Where(x => x.OwnerAction==JobAction.WriteCode).ToList();
			FillGrid(listJobsFiltered);
			FillOverview(listJobsFiltered);
			FillCount(listJobs);//Use listJobs here so we can count all jobs where the User is the Owner
		}

		private void FillGrid(List<Job> listJobs) {
			dataJobs.Rows.Clear();
			foreach(Job job in listJobs) {
				if(job.PhaseCur!=JobPhase.Development) {
					continue;
				}
				int idxNewRow=dataJobs.Rows.Add();
				DataGridViewRow row=dataJobs.Rows[idxNewRow];
				row.Tag=job;
				row.Cells["Priority"].Value=job.Priority.ToString();
				row.Cells["DateEntered"].Value=job.DateTimeEntry.ToShortDateString();
				row.Cells["LastPhaseChange"].Value=job.DateTimeEntry.ToShortDateString();
				row.Cells["Title"].Value=job.Title;
				row.Tag=job;
			}
		}

		private void FillOverview(List<Job> listJobs) {
			if(listJobs.Count==0) {
				textDevHours.Text="0";
				textLongestHourEst.Text="0";
				textJobsNoEst.Text="0";
				textReviewRequests.Text="0";
				textHighPrio.Text="0";
				textQuotedTotal.Text="0";
				return;
			}
			//textDevHours.Text=listJobs.Sum(x => x.MinutesEstimate).ToString();
			textLongestHourEst.Text=listJobs.Max(x => x.HoursEstimate).ToString();
			//textJobsNoEst.Text=listJobs.Count(x => x.MinutesEstimate==0).ToString();
			textReviewRequests.Text=listJobs.Sum(x => x.ListJobReviews.Count()).ToString();
			textHighPrio.Text=listJobs.Count(x => x.Priority==Defs.GetDefsForCategory(DefCat.JobPriorities,true).FirstOrDefault(y => y.ItemValue.Contains("High")).DefNum).ToString();
			textQuotedTotal.Text=listJobs.Sum(x => x.ListJobQuotes.Sum(y => PIn.Double(y.Amount))).ToString("C");
		}

		private void FillCount(List<Job> listJobs) {
			textConceptJobs.Text=listJobs.Count(x => x.PhaseCur==JobPhase.Concept).ToString();
			textWriteupJobs.Text=listJobs.Count(x => x.OwnerAction==JobAction.WriteConcept || x.OwnerAction==JobAction.WriteJob).ToString();
			textDevelopmentJobs.Text=listJobs.Count(x => x.PhaseCur==JobPhase.Development).ToString();
			textActiveAdvisorJobs.Text=listJobs.Count(x => x.OwnerAction==JobAction.Advise).ToString();
			textJobsOnHold.Text=listJobs.Count(x => x.Priority==Defs.GetDefsForCategory(DefCat.JobPriorities,true).FirstOrDefault(y => y.ItemValue.Contains("OnHold")).DefNum).ToString();
		}

		private void dataJobs_CellDoubleClick(object sender,DataGridViewCellEventArgs e) {
			FormOpenDental.S_GoToJob(((Job)(dataJobs.Rows[e.RowIndex].Tag)).JobNum);

		}
	}
}
