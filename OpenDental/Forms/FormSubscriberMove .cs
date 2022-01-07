using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormSubscriberMove:FormODBase {

		private InsPlan _intoInsPlan;
		private InsPlan _fromInsPlan;

		public FormSubscriberMove() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSubscriberMove_Load(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPlanChangeSubsc)) {
				DialogResult=DialogResult.Cancel;
				return;
			}
		}

		private void butChangePatientInto_Click(object sender,EventArgs e) {
			using FormInsPlans formIP=new FormInsPlans();
			formIP.IsSelectMode=true;
			if(formIP.ShowDialog()==DialogResult.OK) {
				_intoInsPlan=formIP.SelectedPlan;
				textCarrierNameInto.Text=Carriers.GetName(_intoInsPlan.CarrierNum);
			}
		}

		private void butChangePatientFrom_Click(object sender,EventArgs e) {
			using FormInsPlans formIP=new FormInsPlans();
			formIP.IsSelectMode=true;
			if(formIP.ShowDialog()==DialogResult.OK) {
				_fromInsPlan=formIP.SelectedPlan;
				textCarrierNameFrom.Text=Carriers.GetName(_fromInsPlan.CarrierNum);
			}
		}

		private void butViewInsPlanInto_Click(object sender,EventArgs e) {
			if(_intoInsPlan==null || InsPlans.GetPlan(_intoInsPlan.PlanNum,new List<InsPlan>())==null) {
				MsgBox.Show(this,"Valid insurance plan not selected.\r\nPlease select a valid insurance plan using the picker button.");
				return;
			}
			using FormInsPlan formIP=new FormInsPlan(_intoInsPlan,null,null);
			formIP.ShowDialog();
		}

		private void butViewInsPlanFrom_Click(object sender,EventArgs e) {
			if(_fromInsPlan==null || InsPlans.GetPlan(_fromInsPlan.PlanNum,new List<InsPlan>())==null) {
				MsgBox.Show(this,"Valid insurance plan not selected.\r\nPlease select a valid insurance plan using the picker button.");
				return;
			}
			using FormInsPlan formIP=new FormInsPlan(_fromInsPlan,null,null);
			formIP.ShowDialog();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(_fromInsPlan==null || InsPlans.GetPlan(_fromInsPlan.PlanNum,new List<InsPlan>())==null) {
				MsgBox.Show(this,"Please pick a valid plan to move subscribers from.");
				return;
			}
			if(_intoInsPlan==null || InsPlans.GetPlan(_intoInsPlan.PlanNum,new List<InsPlan>())==null) {
				MsgBox.Show(this,"Please pick a valid plan to move subscribers to.");
				return;
			}
			if(_fromInsPlan.PlanNum==_intoInsPlan.PlanNum) {
				MsgBox.Show(this,"Can not move a plan into itself.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Moving subscribers is irreversible.  Always make a full backup before moving subscribers.  "
				+"Patient specific benefits, subscriber notes, benefit notes, and effective dates will not be copied to the other plan."
				+"\r\n\r\nRunning this tool can take several minutes to run.  We recommend running it after business hours or when network usage is low."
				+"\r\n\r\nClick OK to continue, or click Cancel to abort.")) 
			{
				return;
			}
			try {
				Cursor=Cursors.WaitCursor;
				long insSubModifiedCount=InsSubs.MoveSubscribers(_fromInsPlan.PlanNum,_intoInsPlan.PlanNum);
				Cursor=Cursors.Default;
				MessageBox.Show(Lan.g(this,"Count of Subscribers Moved")+": "+insSubModifiedCount);
			}
			catch(ApplicationException ex) {//The tool was blocked due to validation failure.
				Cursor=Cursors.Default;
				using MsgBoxCopyPaste msgBox=new MsgBoxCopyPaste(ex.Message);//No translaion here, because translation was done in the business layer.
				msgBox.ShowDialog();
				return;//Since this exception is due to validation failure, do not close the form.  Let the user manually click Cancel so they know what happened.
			}
			SecurityLogs.MakeLogEntry(Permissions.InsPlanChangeSubsc,0,Lan.g(this,"Subscribers Moved from")+" "+_fromInsPlan.PlanNum+" "+Lan.g(this,"to")+" "+_intoInsPlan.PlanNum);
			DialogResult=DialogResult.OK;//Closes the form.
		}

		private void butCancel_Click(object sender,EventArgs e) {
			//probably don't need this log entry, but here to maintain old behavior
			SecurityLogs.MakeLogEntry(Permissions.InsPlanChangeSubsc,0,Lan.g(this,"Subscriber Move Cancel"));
			DialogResult=DialogResult.Cancel;//Closes the form.
		}

	}
}