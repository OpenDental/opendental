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

		private InsPlan _insPlanInto;
		private InsPlan _insPlanFrom;

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
			using FormInsPlans formInsPlans=new FormInsPlans();
			formInsPlans.IsSelectMode=true;
			if(formInsPlans.ShowDialog()==DialogResult.OK) {
				_insPlanInto=formInsPlans.InsPlanSelected;
				textCarrierNameInto.Text=Carriers.GetName(_insPlanInto.CarrierNum);
			}
		}

		private void butChangePatientFrom_Click(object sender,EventArgs e) {
			using FormInsPlans formInsPlans=new FormInsPlans();
			formInsPlans.IsSelectMode=true;
			if(formInsPlans.ShowDialog()==DialogResult.OK) {
				_insPlanFrom=formInsPlans.InsPlanSelected;
				textCarrierNameFrom.Text=Carriers.GetName(_insPlanFrom.CarrierNum);
			}
		}

		private void butViewInsPlanInto_Click(object sender,EventArgs e) {
			if(_insPlanInto==null || InsPlans.GetPlan(_insPlanInto.PlanNum,new List<InsPlan>())==null) {
				MsgBox.Show(this,"Valid insurance plan not selected.\r\nPlease select a valid insurance plan using the picker button.");
				return;
			}
			using FormInsPlan formInsPlan=new FormInsPlan(_insPlanInto,null,null);
			formInsPlan.ShowDialog();
		}

		private void butViewInsPlanFrom_Click(object sender,EventArgs e) {
			if(_insPlanFrom==null || InsPlans.GetPlan(_insPlanFrom.PlanNum,new List<InsPlan>())==null) {
				MsgBox.Show(this,"Valid insurance plan not selected.\r\nPlease select a valid insurance plan using the picker button.");
				return;
			}
			using FormInsPlan formInsPlan=new FormInsPlan(_insPlanFrom,null,null);
			formInsPlan.ShowDialog();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(_insPlanFrom==null || InsPlans.GetPlan(_insPlanFrom.PlanNum,new List<InsPlan>())==null) {
				MsgBox.Show(this,"Please pick a valid plan to move subscribers from.");
				return;
			}
			if(_insPlanInto==null || InsPlans.GetPlan(_insPlanInto.PlanNum,new List<InsPlan>())==null) {
				MsgBox.Show(this,"Please pick a valid plan to move subscribers to.");
				return;
			}
			if(_insPlanFrom.PlanNum==_insPlanInto.PlanNum) {
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
			Cursor=Cursors.WaitCursor;
			long countInsSubModified;
			try {
				countInsSubModified=InsSubs.MoveSubscribers(_insPlanFrom.PlanNum,_insPlanInto.PlanNum);
			}
			catch(ApplicationException ex) {//The tool was blocked due to validation failure.
				Cursor=Cursors.Default;
				using MsgBoxCopyPaste msgBox=new MsgBoxCopyPaste(ex.Message);//No translaion here, because translation was done in the business layer.
				msgBox.ShowDialog();
				return;//Since this exception is due to validation failure, do not close the form.  Let the user manually click Cancel so they know what happened.
			}
			Cursor=Cursors.Default;
			MessageBox.Show(Lan.g(this,"Count of Subscribers Moved")+": "+countInsSubModified);
			SecurityLogs.MakeLogEntry(Permissions.InsPlanChangeSubsc,0,Lan.g(this,"Subscribers Moved from")+" "+_insPlanFrom.PlanNum+" "+Lan.g(this,"to")+" "+_insPlanInto.PlanNum);
			DialogResult=DialogResult.OK;//Closes the form.
		}

		private void butCancel_Click(object sender,EventArgs e) {
			//probably don't need this log entry, but here to maintain old behavior
			SecurityLogs.MakeLogEntry(Permissions.InsPlanChangeSubsc,0,Lan.g(this,"Subscriber Move Cancel"));
			DialogResult=DialogResult.Cancel;//Closes the form.
		}




	}
}