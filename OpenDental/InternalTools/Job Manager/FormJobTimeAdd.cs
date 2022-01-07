using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormJobTimeAdd:FormODBase {
		public JobReview TimeLogCur;

		///<summary></summary>
		public FormJobTimeAdd(JobReview timeLog) {
			TimeLogCur=timeLog;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormJobTimeAdd_Load(object sender,EventArgs e) {
			TimeLogCur.ReviewStatus=JobReviewStatus.TimeLog;
			TimeLogCur.ReviewerNum=Security.CurUser.UserNum;
			TimeLogCur.DateTStamp=DateTime.Now;
			textUser.Text=Security.CurUser.UserName;
			textDate.Text=TimeLogCur.DateTStamp.ToShortDateString();
			List<string> listAvailableIncrements=new List<string>();
			double i=10;
			while(i>-10.5) {
				listAvailableIncrements.Add(i.ToString());
				i=i-(.5);
			}
			textTimeHours.Text="0";
			textTimeMinutes.Text="0";
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textTimeHours.IsValid() && !textTimeMinutes.IsValid()) {
				MsgBox.Show(this,"Please input valid integers into the hours and minutes boxes.");
				return;
			}
			if(textTimeHours.Text=="0" && textTimeMinutes.Text=="0") {
				MsgBox.Show(this,"Please choose an option besides 0 for your time added.");
				return;
			}
			try {
				TimeLogCur.TimeReview=TimeSpan.FromMinutes(int.Parse(textTimeHours.Text)*60+int.Parse(textTimeMinutes.Text));
			}
			catch(Exception ex) {
				MessageBox.Show(this,ex.Message);
				return;
			}
			TimeLogCur.Description=textDescription.Text;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel; //removing new jobs from the DB is taken care of in FormClosing
		}
	}
}