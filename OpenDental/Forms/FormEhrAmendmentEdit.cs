using OpenDentBusiness;
using System;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormEhrAmendmentEdit:FormODBase {
		private EhrAmendment EhrAmendmentCur;

		public FormEhrAmendmentEdit(EhrAmendment ehrAmdCur) {
			InitializeComponent();
			InitializeLayoutManager();
			EhrAmendmentCur=ehrAmdCur;
		}

		private void FormEhrAmendmentEdit_Load(object sender,EventArgs e) {
			for(int i=0;i<Enum.GetNames(typeof(AmendmentSource)).Length;i++) {
				comboSource.Items.Add(Enum.GetNames(typeof(AmendmentSource))[i]);
			}
			listAmdStatus.Items.Add(Lan.g(this,"Requested"));
			listAmdStatus.Items.Add(Lan.g(this,"Accepted"));
			listAmdStatus.Items.Add(Lan.g(this,"Denied"));
			if(EhrAmendmentCur.IsNew) {
				return;
			}
			if(EhrAmendmentCur.DateTAppend.Year>1880) {
				labelScan.Visible=true;
				butScan.Text="View";
			}
			if(EhrAmendmentCur.IsAccepted==YN.Yes) {
				listAmdStatus.SelectedIndex=1;
			}
			else if(EhrAmendmentCur.IsAccepted==YN.No) {
				listAmdStatus.SelectedIndex=2;
			}
			else {
				listAmdStatus.SelectedIndex=0;
			}
			comboSource.SelectedIndex=(int)EhrAmendmentCur.Source;
			textSourceName.Text=EhrAmendmentCur.SourceName;
			textDescription.Text=EhrAmendmentCur.Description;
			if(EhrAmendmentCur.DateTRequest.Year>1880) {
				textDateReq.Text=EhrAmendmentCur.DateTRequest.ToShortDateString()+" "+EhrAmendmentCur.DateTRequest.ToShortTimeString();
			}
			if(EhrAmendmentCur.DateTAcceptDeny.Year>1880) {
				textDateAcc.Text=EhrAmendmentCur.DateTAcceptDeny.ToShortDateString()+" "+EhrAmendmentCur.DateTAcceptDeny.ToShortTimeString();
			}
			if(EhrAmendmentCur.DateTAppend.Year>1880) {
				textDateApp.Text=EhrAmendmentCur.DateTAppend.ToShortDateString()+" "+EhrAmendmentCur.DateTAppend.ToShortTimeString();
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
			using FormImages FormI=new FormImages();
			EhrAmendment amendmentOld=EhrAmendmentCur;
			FormI.EhrAmendmentCur=EhrAmendmentCur;
			FormI.ShowDialog();
			EhrAmendmentCur=EhrAmendments.GetOne(EhrAmendmentCur.EhrAmendmentNum);
			if(EhrAmendmentCur.FileName!="") {
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
			if(EhrAmendmentCur.IsNew) {
				//no need to ask them
			}
			else {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Amendment?")) {
					return;
				}
			}
			//in both cases, delete:
			ImageStore.CleanAmdAttach(EhrAmendmentCur.FileName);
			EhrAmendments.Delete(EhrAmendmentCur.EhrAmendmentNum);
			DialogResult=DialogResult.OK;//Causes grid to refresh in case this amendment is not new.
		}

		private void butOK_Click(object sender,EventArgs e) {
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
			YN status;
			if(listAmdStatus.SelectedIndex==1) {
				status=YN.Yes;
			}
			else if(listAmdStatus.SelectedIndex==2) {
				status=YN.No;
			}
			else {
				status=YN.Unknown;
			}
			if(EhrAmendmentCur.IsNew && textDateReq.Text=="") {
				EhrAmendmentCur.DateTRequest=DateTime.Now;
			}
			if((status==YN.Yes || status==YN.No)//Accepted or Denied (not just U/requested)
				&& textDateAcc.Text=="") 
			{
				if(EhrAmendmentCur.IsNew || EhrAmendmentCur.IsAccepted!=status) {//if status has changed
					EhrAmendmentCur.DateTAcceptDeny=DateTime.Now;//automatically fill date
				}
			}
			if(textDateReq.Text=="") {
				EhrAmendmentCur.DateTRequest=DateTime.MinValue;
			}
			else {
				EhrAmendmentCur.DateTRequest=DateTime.Parse(textDateReq.Text);
			}
			if(textDateAcc.Text=="") {
				EhrAmendmentCur.DateTAcceptDeny=DateTime.MinValue;
			}
			else {
				EhrAmendmentCur.DateTAcceptDeny=DateTime.Parse(textDateAcc.Text);
			}
			if(textDateApp.Text=="") {
				EhrAmendmentCur.DateTAppend=DateTime.MinValue;
			}
			else {
				EhrAmendmentCur.DateTAppend=DateTime.Parse(textDateApp.Text);
			}
			EhrAmendmentCur.IsAccepted=status;
			EhrAmendmentCur.Source=(AmendmentSource)comboSource.SelectedIndex;
			EhrAmendmentCur.SourceName=textSourceName.Text;
			EhrAmendmentCur.Description=textDescription.Text;
			EhrAmendments.Update(EhrAmendmentCur);//always saved to db before entering this form
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormEhrAmendmentEdit_Closing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.OK) {
				return;
			}
			if(EhrAmendmentCur.IsNew) {
				EhrAmendments.Delete(EhrAmendmentCur.EhrAmendmentNum);
				ImageStore.CleanAmdAttach(EhrAmendmentCur.FileName);
			}
		}


	}
}
