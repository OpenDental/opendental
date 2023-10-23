using OpenDentBusiness;
using System;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormEhrAmendmentEdit:FormODBase {
		private EhrAmendment _ehrAmendment;

		public FormEhrAmendmentEdit(EhrAmendment ehrAmendment) {
			InitializeComponent();
			InitializeLayoutManager();
			_ehrAmendment=ehrAmendment;
		}

		private void FormEhrAmendmentEdit_Load(object sender,EventArgs e) {
			for(int i=0;i<Enum.GetNames(typeof(AmendmentSource)).Length;i++) {
				comboSource.Items.Add(Enum.GetNames(typeof(AmendmentSource))[i]);
			}
			listAmdStatus.Items.Add(Lan.g(this,"Requested"));
			listAmdStatus.Items.Add(Lan.g(this,"Accepted"));
			listAmdStatus.Items.Add(Lan.g(this,"Denied"));
			if(_ehrAmendment.IsNew) {
				return;
			}
			if(_ehrAmendment.DateTAppend.Year>1880) {
				labelScan.Visible=true;
				butScan.Text="View";
			}
			if(_ehrAmendment.IsAccepted==YN.Yes) {
				listAmdStatus.SelectedIndex=1;
			}
			else if(_ehrAmendment.IsAccepted==YN.No) {
				listAmdStatus.SelectedIndex=2;
			}
			else {
				listAmdStatus.SelectedIndex=0;
			}
			comboSource.SelectedIndex=(int)_ehrAmendment.Source;
			textSourceName.Text=_ehrAmendment.SourceName;
			textDescription.Text=_ehrAmendment.Description;
			if(_ehrAmendment.DateTRequest.Year>1880) {
				textDateReq.Text=_ehrAmendment.DateTRequest.ToShortDateString()+" "+_ehrAmendment.DateTRequest.ToShortTimeString();
			}
			if(_ehrAmendment.DateTAcceptDeny.Year>1880) {
				textDateAcc.Text=_ehrAmendment.DateTAcceptDeny.ToShortDateString()+" "+_ehrAmendment.DateTAcceptDeny.ToShortTimeString();
			}
			if(_ehrAmendment.DateTAppend.Year>1880) {
				textDateApp.Text=_ehrAmendment.DateTAppend.ToShortDateString()+" "+_ehrAmendment.DateTAppend.ToShortTimeString();
			}
		}

		private void butNowReq_Click(object sender,EventArgs e) {
			textDateReq.Text=DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString();
		}

		private void butNowAcc_Click(object sender,EventArgs e) {
			textDateAcc.Text=DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString();
		}

		private void butNowApp_Click(object sender,EventArgs e) {
			textDateApp.Text=DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString();
		}

		private void butScan_Click(object sender,EventArgs e) {
			using FormImages formImages=new FormImages();
			EhrAmendment ehrAmendment=_ehrAmendment;
			formImages.EhrAmendmentCur=_ehrAmendment;
			formImages.ShowDialog();
			_ehrAmendment=EhrAmendments.GetOne(_ehrAmendment.EhrAmendmentNum);
			if(_ehrAmendment.FileName!="") {
				labelScan.Visible=true;
				butScan.Text="View";
				textDateApp.Text=DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString();
			}
			else {
				labelScan.Visible=false;
				butScan.Text="Scan";
				textDateApp.Text="";
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_ehrAmendment.IsNew) {
				//no need to ask them
			}
			else {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Amendment?")) {
					return;
				}
			}
			//in both cases, delete:
			ImageStore.CleanAmdAttach(_ehrAmendment.FileName);
			EhrAmendments.Delete(_ehrAmendment.EhrAmendmentNum);
			DialogResult=DialogResult.OK;//Causes grid to refresh in case this amendment is not new.
		}

		private void butSave_Click(object sender,EventArgs e) {
			try {
				if(textDateReq.Text!="") {
					DateTime.Parse(textDateReq.Text);
				}
				if(textDateAcc.Text!="") {
					DateTime.Parse(textDateAcc.Text);
				}
				if(textDateApp.Text!="") {
					DateTime.Parse(textDateApp.Text);
				}
			}
			catch {
				MsgBox.Show(this,"The date entered does not match the required format.");
				return;
			}
			if(comboSource.SelectedIndex==-1) {
				MessageBox.Show("Please select an amendment source.");
				return;
			}
			if(textSourceName.Text=="") {
				MessageBox.Show("Please input a source name.");
				return;
			}
			if(textDescription.Text=="") {
				MessageBox.Show("Please input an amendment description.");
				return;
			}
			YN YNstatus;
			if(listAmdStatus.SelectedIndex==1) {
				YNstatus=YN.Yes;
			}
			else if(listAmdStatus.SelectedIndex==2) {
				YNstatus=YN.No;
			}
			else {
				YNstatus=YN.Unknown;
			}
			if(_ehrAmendment.IsNew && textDateReq.Text=="") {
				_ehrAmendment.DateTRequest=DateTime.Now;
			}
			if((YNstatus==YN.Yes || YNstatus==YN.No)//Accepted or Denied (not just U/requested)
				&& textDateAcc.Text=="") 
			{
				if(_ehrAmendment.IsNew || _ehrAmendment.IsAccepted!=YNstatus) {//if status has changed
					_ehrAmendment.DateTAcceptDeny=DateTime.Now;//automatically fill date
				}
			}
			if(textDateReq.Text=="") {
				_ehrAmendment.DateTRequest=DateTime.MinValue;
			}
			else {
				_ehrAmendment.DateTRequest=DateTime.Parse(textDateReq.Text);
			}
			if(textDateAcc.Text=="") {
				_ehrAmendment.DateTAcceptDeny=DateTime.MinValue;
			}
			else {
				_ehrAmendment.DateTAcceptDeny=DateTime.Parse(textDateAcc.Text);
			}
			if(textDateApp.Text=="") {
				_ehrAmendment.DateTAppend=DateTime.MinValue;
			}
			else {
				_ehrAmendment.DateTAppend=DateTime.Parse(textDateApp.Text);
			}
			_ehrAmendment.IsAccepted=YNstatus;
			_ehrAmendment.Source=(AmendmentSource)comboSource.SelectedIndex;
			_ehrAmendment.SourceName=textSourceName.Text;
			_ehrAmendment.Description=textDescription.Text;
			EhrAmendments.Update(_ehrAmendment);//always saved to db before entering this form
			DialogResult=DialogResult.OK;
		}

		private void FormEhrAmendmentEdit_Closing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.OK) {
				return;
			}
			if(_ehrAmendment.IsNew) {
				EhrAmendments.Delete(_ehrAmendment.EhrAmendmentNum);
				ImageStore.CleanAmdAttach(_ehrAmendment.FileName);
			}
		}

	}
}