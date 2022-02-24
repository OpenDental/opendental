using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	///<summary></summary>
	public partial class FormRefAttachEdit : FormODBase {
		///<summary></summary>
    public bool IsNew;
		///<summary></summary>
		public RefAttach RefAttachCur;
		///<summary>List of referral slips for this pat/ref combo.</summary>
		private List<Sheet> SheetList; 
		///<summary>Select a referring provider for referals to other providers.</summary>
		private long _provNumSelected;
		private RefAttach _refAttachOld;
		private List<Provider> _listProviders;

		///<summary></summary>
		public FormRefAttachEdit(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRefAttachEdit_Load(object sender,EventArgs e) {
			if(Plugins.HookMethod(this,"FormRefAttachEdit.Load",RefAttachCur,IsNew)) {
				return;
			}
			if(IsNew) {
				Text=Lan.g(this,"Add Referral Attachment");
			}
			else {
				_refAttachOld=RefAttachCur.Copy();
				Text=Lan.g(this,"Edit Referral Attachment");
			}
			string referralDescript=DisplayFields.GetForCategory(DisplayFieldCategory.PatientInformation)
				.FirstOrDefault(x => x.InternalName=="Referrals")?.Description;
			if(string.IsNullOrWhiteSpace(referralDescript)) {//either not displaying the Referral field or no description entered, default to 'Referral (other)'
				referralDescript=Lan.g(this,"Referral (other)");
			}
			listRefType.Items.Add(Lan.g(this,"To"));
			listRefType.Items.Add(Lan.g(this,"From"));
			listRefType.Items.Add(referralDescript);
			FillData();
			FillSheets();
			_provNumSelected=RefAttachCur.ProvNum;
			comboProvNum.Items.Clear();
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++) {
				comboProvNum.Items.Add(_listProviders[i].GetLongDesc());//Only visible provs added to combobox.
				if(_listProviders[i].ProvNum==RefAttachCur.ProvNum) {
					comboProvNum.SelectedIndex=i;//Sets combo text too.
				}
			}
			if(comboProvNum.SelectedIndex==-1) {//The provider exists but is hidden
				comboProvNum.Text=Providers.GetLongDesc(_provNumSelected);//Appends "(hidden)" to the end of the long description.
			}
			if(RefAttachCur.RefType==ReferralType.RefFrom) {
				butNoneProv.Visible=false;
				butPickProv.Visible=false;
				comboProvNum.Visible=false;
				labelProv.Visible=false;
			}
			else {
				butNoneProv.Visible=true;
				butPickProv.Visible=true;
				comboProvNum.Visible=true;
				labelProv.Visible=true;
			}
		}

		private void FillData(){
			Referral referral=ReferralL.GetReferral(RefAttachCur.ReferralNum);
			if(referral==null) {
				return;
			}
			textName.Text=referral.GetNameFL();
			labelPatient.Visible=referral.PatNum>0;
			textReferralNotes.Text=referral.Note;
			listRefType.SelectedIndex=(int)RefAttachCur.RefType;
			if(RefAttachCur.RefDate.Year<1880) {
				textRefDate.Text="";
			}
			else{
				textRefDate.Text=RefAttachCur.RefDate.ToShortDateString();
			}
			textOrder.Text=RefAttachCur.ItemOrder.ToString();
			textOrder.ReadOnly=true;//It can be reordered by the Up/Down buttons on FormReferralsPatient.
			comboRefToStatus.Items.Clear();
			for(int i=0;i<Enum.GetNames(typeof(ReferralToStatus)).Length;i++){
				comboRefToStatus.Items.Add(Lan.g("enumReferralToStatus",Enum.GetNames(typeof(ReferralToStatus))[i]));
				if((int)RefAttachCur.RefToStatus==i){
					comboRefToStatus.SelectedIndex=i;
				}
			}
			textNote.Text=RefAttachCur.Note;
			checkIsTransitionOfCare.Checked=RefAttachCur.IsTransitionOfCare;
			textProc.Text="";
			if(RefAttachCur.ProcNum!=0) {
				Procedure proc=Procedures.GetOneProc(RefAttachCur.ProcNum,false);
				textProc.Text=Procedures.GetDescription(proc);
			}
			if(RefAttachCur.DateProcComplete.Year<1880) {
				textDateProcCompleted.Text="";
			}
			else {
				textDateProcCompleted.Text=RefAttachCur.DateProcComplete.ToShortDateString();
			}
		}

		private void FillSheets(){
			SheetList=Sheets.GetReferralSlips(RefAttachCur.PatNum,RefAttachCur.ReferralNum);
			listSheets.Items.Clear();
			listSheets.Items.AddList(SheetList,x => x.DateTimeSheet.ToShortDateString());
		}

		private void butEditReferral_Click(object sender,EventArgs e) {
			try{
				DataToCur();
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			Referral referral=ReferralL.GetReferral(RefAttachCur.ReferralNum);
			if(referral==null) {
				return;
			}
			using FormReferralEdit FormRE=new FormReferralEdit(referral);
			FormRE.ShowDialog();
			Referrals.RefreshCache();
			FillData();
		}

		private void butChangeReferral_Click(object sender,EventArgs e) {
            if(!Security.IsAuthorized(Permissions.RefAttachAdd,false)) {
				return;
            }
			RefAttach oldRefAttach=RefAttachCur.Copy();
			using FormReferralSelect FormRS=new FormReferralSelect();
			FormRS.IsSelectionMode=true;
			FormRS.ShowDialog();
			if(FormRS.DialogResult!=DialogResult.OK) {
				return;
			}
			RefAttachCur.ReferralNum=FormRS.SelectedReferral.ReferralNum;
			if(oldRefAttach.ReferralNum!=RefAttachCur.ReferralNum) {
				SecurityLogs.MakeLogEntry(Permissions.RefAttachAdd,RefAttachCur.PatNum,
					"Referral attachment changed from: '"+Referrals.GetNameFL(oldRefAttach.ReferralNum)+"' to: '"+Referrals.GetNameFL(RefAttachCur.ReferralNum)+"'");
			}
			FillData();
		}

		private void listSheets_DoubleClick(object sender,EventArgs e) {
			if(listSheets.SelectedIndex==-1){
				return;
			}
			Sheet sheet=SheetList[listSheets.SelectedIndex];
			SheetFields.GetFieldsAndParameters(sheet);
			FormSheetFillEdit.ShowForm(sheet,delegate { FillSheets(); });
		}

		private void comboProvNum_SelectionChangeCommitted(object sender,EventArgs e) {
			_provNumSelected=_listProviders[comboProvNum.SelectedIndex].ProvNum;
		}

		private void butPickProv_Click(object sender,EventArgs e) {
			using FormProviderPick formP=new FormProviderPick();
			if(comboProvNum.SelectedIndex > -1) {//Initial formP selection if selected prov is not hidden.
				formP.SelectedProvNum=_provNumSelected;
			}
			formP.ShowDialog();
			if(formP.DialogResult!=DialogResult.OK) {
				return;
			}
			comboProvNum.SelectedIndex=Providers.GetIndex(formP.SelectedProvNum);
			_provNumSelected=formP.SelectedProvNum;
		}

		private void butNoneProv_Click(object sender,EventArgs e) {
			_provNumSelected=0;
			comboProvNum.SelectedIndex=-1;
		}

		private void listRefType_SelectedIndexChanged(object sender,EventArgs e) {
			//show referring provider only if referring to
			butNoneProv.Visible=((ReferralType)listRefType.SelectedIndex==ReferralType.RefTo);
			butPickProv.Visible=((ReferralType)listRefType.SelectedIndex==ReferralType.RefTo);
			comboProvNum.Visible=((ReferralType)listRefType.SelectedIndex==ReferralType.RefTo);
			labelProv.Visible=((ReferralType)listRefType.SelectedIndex==ReferralType.RefTo);
		}

		///<summary>Surround with try-catch.  Attempts to take the data on the form and set the values of RefAttachCur.</summary>
		private void DataToCur(){
			if(!textOrder.IsValid()
				|| !textRefDate.IsValid()
				|| !textDateProcCompleted.IsValid()) 
			{
				throw new ApplicationException(Lan.g(this,"Please fix data entry errors first."));
			}
			RefAttachCur.RefType=(ReferralType)listRefType.SelectedIndex;
			RefAttachCur.ProvNum=(listRefType.SelectedIndex==0?_provNumSelected:0);//If the Referral Type is 'To', use the selected ProvNum.
			//(Optional) Also Set ProvNum on RefType.Other??
			RefAttachCur.RefDate=PIn.Date(textRefDate.Text);
			RefAttachCur.ItemOrder=PIn.Int(textOrder.Text);
			RefAttachCur.RefToStatus=(ReferralToStatus)comboRefToStatus.SelectedIndex;
			RefAttachCur.Note=textNote.Text;
			RefAttachCur.IsTransitionOfCare=checkIsTransitionOfCare.Checked;
			RefAttachCur.DateProcComplete=PIn.Date(textDateProcCompleted.Text);
		}

		private void butDetach_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.RefAttachDelete)) {
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Detach Referral?")) {
				return;
			}
			SecurityLogs.MakeLogEntry(Permissions.RefAttachDelete,RefAttachCur.PatNum,"Referral attachment deleted for "+Referrals.GetNameFL(RefAttachCur.ReferralNum));
			RefAttaches.Delete(RefAttachCur);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			//We want to help EHR users meet their summary of care measure.  So all outgoing patient referrals should warn them if they didn't enter data correctly.
			if((ReferralType)listRefType.SelectedIndex==ReferralType.RefTo && PrefC.GetBool(PrefName.ShowFeatureEhr)) {
				string warning="";
				if(comboProvNum.SelectedIndex<0) {
					warning+=Lans.g(this,"Selected patient referral does not have a referring provider set.");
				}
				if(!checkIsTransitionOfCare.Checked) {
					if(warning!="") {
						warning+="\r\n";
					}
					warning+=Lans.g(this,"Selected patient referral is not flagged as a transition of care.");
				}
				if(warning!="") {
					warning+="\r\n"+Lans.g(this,"It will not meet the EHR summary of care requirements.")+"  "+Lans.g(this,"Continue anyway?");
					if(MessageBox.Show(warning,Lans.g(this,"EHR Measure Warning"),MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
						return;
					}
				}
			}
			//this is an old pattern
			try{
				DataToCur();
				if(IsNew){
					RefAttaches.Insert(RefAttachCur);
				}
				else{
					RefAttaches.Update(RefAttachCur,_refAttachOld);
				}
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}








