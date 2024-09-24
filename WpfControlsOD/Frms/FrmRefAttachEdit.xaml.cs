using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	///<summary></summary>
	public partial class FrmRefAttachEdit : FrmODBase {
		///<summary></summary>
    public bool IsNew;
		///<summary></summary>
		public RefAttach RefAttachCur;
		///<summary>List of referral slips for this pat/ref combo.</summary>
		private List<Sheet> _listSheets; 
		///<summary>Select a referring provider for referals to other providers.</summary>
		private RefAttach _refAttachOld;
		private List<Provider> _listProviders;

		///<summary></summary>
		public FrmRefAttachEdit(){
			InitializeComponent();
			Load+=FormRefAttachEdit_Load;
			listSheets.MouseDoubleClick+=listSheets_DoubleClick;
			listRefType.SelectedIndexChanged+=listRefType_SelectedIndexChanged;
			PreviewKeyDown+=FrmRefAttachEdit_PreviewKeyDown;
		}

		private void FormRefAttachEdit_Load(object sender,EventArgs e) {
			Lang.F(this);
			if(Plugins.HookMethod(this,"FormRefAttachEdit.Load",RefAttachCur,IsNew)) {
				return;
			}
			if(IsNew) {
				Text=Lang.g(this,"Add Referral Attachment");
			}
			else {
				_refAttachOld=RefAttachCur.Copy();
				Text=Lang.g(this,"Edit Referral Attachment");
			}
			string referralDescript=DisplayFields.GetForCategory(DisplayFieldCategory.PatientInformation)
				.FirstOrDefault(x => x.InternalName=="Referrals")?.Description;
			if(string.IsNullOrWhiteSpace(referralDescript)) {//either not displaying the Referral field or no description entered, default to 'Referral'
				//used to also show (other) here, but that word is already used in automatic import.
				referralDescript=Lang.g(this,"Referral");
			}
			listRefType.Items.Add(Lang.g(this,"To"));
			listRefType.Items.Add(Lang.g(this,"From"));
			listRefType.Items.Add(referralDescript);
			FillData();
			FillSheets();
			comboProvNum.Items.Clear();
			comboProvNum.Items.AddProvsFull(Providers.GetDeepCopy(isShort:true));
			comboProvNum.SetSelectedProvNum(RefAttachCur.ProvNum);
			if(RefAttachCur.RefType!=ReferralType.RefTo) {//prov is only visible for To
				butNoneProv.Visible=false;
				butPickProv.Visible=false;
				comboProvNum.Visible=false;
				labelProv.Visible=false;
				return;
			}
			butNoneProv.Visible=true;
			butPickProv.Visible=true;
			comboProvNum.Visible=true;
			labelProv.Visible=true;
		}

		private void FillData(){
			Referral referral=Referrals.GetReferral(RefAttachCur.ReferralNum);
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
				comboRefToStatus.Items.Add(Lang.g("enumReferralToStatus",Enum.GetNames(typeof(ReferralToStatus))[i]));
				if((int)RefAttachCur.RefToStatus==i){
					comboRefToStatus.SelectedIndex=i;
				}
			}
			textNote.Text=RefAttachCur.Note;
			checkIsTransitionOfCare.Checked=RefAttachCur.IsTransitionOfCare;
			textProc.Text="";
			if(RefAttachCur.ProcNum!=0) {
				Procedure procedure=Procedures.GetOneProc(RefAttachCur.ProcNum,false);
				textProc.Text=Procedures.GetDescription(procedure);
			}
			if(RefAttachCur.DateProcComplete.Year<1880) {
				textDateProcCompleted.Text="";
				return;
			}
			textDateProcCompleted.Text=RefAttachCur.DateProcComplete.ToShortDateString();
		}

		private void FillSheets(){
			_listSheets=Sheets.GetReferralSlips(RefAttachCur.PatNum,RefAttachCur.ReferralNum);
			listSheets.Items.Clear();
			listSheets.Items.AddList(_listSheets,x => x.DateTimeSheet.ToShortDateString());
		}

		private void FrmRefAttachEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butEditReferral.IsAltKey(Key.E,e)) {
				butEditReferral_Click(this,new EventArgs());
			}
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butEditReferral_Click(object sender,EventArgs e) {
			try{
				DataToCur();
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			Referral referral=Referrals.GetReferral(RefAttachCur.ReferralNum);
			if(referral==null) {
				return;
			}
			FrmReferralEdit frmReferralEdit=new FrmReferralEdit(referral);
			frmReferralEdit.ShowDialog();
			Referrals.RefreshCache();
			FillData();
		}

		private void butChangeReferral_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.RefAttachAdd,false)) {
				return;
			}
			RefAttach refAttachOld=RefAttachCur.Copy();
			FrmReferralSelect frmReferralSelect=new FrmReferralSelect();
			frmReferralSelect.IsSelectionMode=true;
			frmReferralSelect.ShowDialog();
			if(frmReferralSelect.IsDialogCancel) {
				return;
			}
			RefAttachCur.ReferralNum=frmReferralSelect.ReferralSelected.ReferralNum;
			if(refAttachOld.ReferralNum!=RefAttachCur.ReferralNum) {
				SecurityLogs.MakeLogEntry(EnumPermType.RefAttachAdd,RefAttachCur.PatNum,
					"Referral attachment changed from: '"+Referrals.GetNameFL(refAttachOld.ReferralNum)+"' to: '"+Referrals.GetNameFL(RefAttachCur.ReferralNum)+"'");
			}
			FillData();
		}

		private void listSheets_DoubleClick(object sender,EventArgs e) {
			if(listSheets.SelectedIndex==-1){
				return;
			}
			Sheet sheet=_listSheets[listSheets.SelectedIndex];
			SheetFields.GetFieldsAndParameters(sheet);
			FormLauncher formLauncher=new FormLauncher(EnumFormName.FormSheetFillEdit);
			formLauncher.SetField("SheetCur",sheet);
			formLauncher.ShowDialog();
			FillSheets();
		}

		private void butPickProv_Click(object sender,EventArgs e) {
			FrmProviderPick frmProviderPick=new FrmProviderPick();
			if(comboProvNum.SelectedIndex>-1) {//Initial formP selection if selected prov is not hidden.
				frmProviderPick.ProvNumSelected=comboProvNum.GetSelectedProvNum();
			}
			frmProviderPick.ShowDialog();
			if(!frmProviderPick.IsDialogOK) {
				return;
			}
			comboProvNum.SelectedIndex=Providers.GetIndex(frmProviderPick.ProvNumSelected);
		}

		private void butNoneProv_Click(object sender,EventArgs e) {
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
			if(!textRefDate.IsValid()
				|| !textDateProcCompleted.IsValid()) 
			{
				throw new ApplicationException(Lang.g(this,"Please fix data entry errors first."));
			}
			RefAttachCur.RefType=(ReferralType)listRefType.SelectedIndex;
			RefAttachCur.ProvNum=-1;
			if(listRefType.SelectedIndex==0){//If the Referral Type is 'To', use the selected ProvNum
				RefAttachCur.ProvNum=comboProvNum.GetSelectedProvNum();
			}
			//(Optional) Also Set ProvNum on RefType.Other??
			RefAttachCur.RefDate=PIn.Date(textRefDate.Text);
			//RefAttachCur.ItemOrder=PIn.Int(textOrder.Text);//no. It's read only
			RefAttachCur.RefToStatus=(ReferralToStatus)comboRefToStatus.SelectedIndex;
			RefAttachCur.Note=textNote.Text;
			RefAttachCur.IsTransitionOfCare=(bool)checkIsTransitionOfCare.Checked;
			RefAttachCur.DateProcComplete=PIn.Date(textDateProcCompleted.Text);
		}

		private void butDetach_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.RefAttachDelete)) {
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Detach Referral?")) {
				return;
			}
			SecurityLogs.MakeLogEntry(EnumPermType.RefAttachDelete,RefAttachCur.PatNum,"Referral attachment deleted for "+Referrals.GetNameFL(RefAttachCur.ReferralNum));
			RefAttaches.Delete(RefAttachCur);
			IsDialogOK=true;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			//We want to help EHR users meet their summary of care measure.  So all outgoing patient referrals should warn them if they didn't enter data correctly.
			if((ReferralType)listRefType.SelectedIndex==ReferralType.RefTo && PrefC.GetBool(PrefName.ShowFeatureEhr)) {
				string warning="";
				if(comboProvNum.SelectedIndex<0) {
					warning+=Lans.g(this,"Selected patient referral does not have a referring provider set.");
				}
				if(checkIsTransitionOfCare.Checked==false) {
					if(warning!="") {
						warning+="\r\n";
					}
					warning+=Lans.g(this,"Selected patient referral is not flagged as a transition of care.");
				}
				if(warning!="") {
					warning+="\r\n"+Lans.g(this,"It will not meet the EHR summary of care requirements.")+"  "+Lans.g(this,"Continue anyway?");
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,warning,Lans.g(this,"EHR Measure Warning"))){
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
			IsDialogOK=true;
		}

	}
}