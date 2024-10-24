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
using CodeBase;
using OpenDentBusiness;
using WpfControls;
using WpfControls.UI;

namespace OpenDental {
	/// <summary></summary>
	public partial class FrmReferralsPatient : FrmODBase {
		public long PatNum;
		private List<RefAttach> _listRefAttaches;
		///<summary>This number is normally zero.  If this number is set externally before opening this form, then this will behave differently.</summary>
		public long ProcNum;
		///<summary>Selection mode is currently only used for transitions of care.  Changes text of butClose to Cancel and shows OK and None buttons.</summary>
		public bool IsSelectionMode;
		///<summary>This number is normally zero.  If in selection mode, this will be the PK of the selected refattach.</summary>
		public long RefAttachNum;
		///<summary>This number is normally zero.  If form is opened by double clicking a summary of care event then this will be filled with the current FKey of that measure event.</summary>
		public long DefaultRefAttachNum;

		///<summary></summary>
		public FrmReferralsPatient()
		{
			InitializeComponent();
			PreviewKeyDown+=FrmReferralsPatient_PreviewKeyDown;
			gridMain.CellDoubleClick+=gridMain_CellDoubleClick;
			Load+=FrmReferralsPatient_Load;
			linkLabel.LinkClicked+=LinkLabel_LinkClicked;
		}

		private void FrmReferralsPatient_Load(object sender,EventArgs e) {
			Lang.F(this);
			if(IsSelectionMode) {
				gridMain.SelectionMode=GridSelectionMode.OneRow;
			}
			else {
				butOK.Visible=false;
			}
			if(ProcNum!=0) {
				Text=Lang.g(this,"Referrals");
				butAddFrom.IsEnabled=false;
				butAddCustom.IsEnabled=false;
			}
			else {//all for patient
				checkShowAll.Visible=false;//we will always show all
			}
			FillGrid();
			if(_listRefAttaches.Count>0 && !IsSelectionMode) {
				gridMain.SetSelected(0,true);
			}
			Plugins.HookAddCode(this,"FormReferralsPatient.Load_end");
		}

		private void LinkLabel_LinkClicked(object sender,EventArgs e) {
			string str="Neither To nor From. Will not show on referral reports. Will still show in Letters dropdown. To change the label in front of it, go to DisplayFields, Patient Information. Change the display text for the field called Referrals. This will show in the Family module, in this grid, and in the referral edit window.";
			MsgBox.Show(str);
		}

		private void checkShowAll_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			_listRefAttaches=RefAttaches.RefreshFiltered(PatNum,true,0);
			string referralDescript=DisplayFields.GetForCategory(DisplayFieldCategory.PatientInformation)
				.FirstOrDefault(x => x.InternalName=="Referrals")?.Description; 
			if(string.IsNullOrWhiteSpace(referralDescript)) {//either not displaying the Referral field or no description entered, default to 'Referral'
				//used to also show (other) here, but that word is already used in automatic import.
				referralDescript=Lang.g(this,"Referral");
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new GridColumn(Lang.g("TableRefList","Referral Type"),85));
			gridMain.Columns.Add(new GridColumn(Lang.g("TableRefList","Name"),120));
			gridMain.Columns.Add(new GridColumn(Lang.g("TableRefList","Date"),65));
			gridMain.Columns.Add(new GridColumn(Lang.g("TableRefList","Status"),70));
			gridMain.Columns.Add(new GridColumn(Lang.g("TableRefList","Proc"),120));
			gridMain.Columns.Add(new GridColumn(Lang.g("TableRefList","Note"),170));
			gridMain.Columns.Add(new GridColumn(Lang.g("TableRefList","Email"),190));
			gridMain.ListGridRows.Clear();
			bool hasInvalidRef=false;
			GridRow row;
			List<string> listRefTypeNames=new List<string>() {Lang.g(this,"To"),Lang.g(this,"From"),referralDescript };
			for(int i=0;i<_listRefAttaches.Count;i++) {
				RefAttach refAttach=_listRefAttaches[i];
				if(ProcNum != 0 && !checkShowAll.Checked==true
					&& ProcNum != refAttach.ProcNum)
				{
					continue;
				}
				row=new GridRow();
				row.Cells.Add(listRefTypeNames[(int)refAttach.RefType]);
				row.Cells.Add(Referrals.GetNameFL(refAttach.ReferralNum));
				if(refAttach.RefDate.Year < 1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(refAttach.RefDate.ToShortDateString());
				}
				row.Cells.Add(Lang.g("enumReferralToStatus",refAttach.RefToStatus.ToString()));
				if(refAttach.ProcNum==0) {
					row.Cells.Add("");
				}
				else {
					Procedure procedure=Procedures.GetOneProc(refAttach.ProcNum,false);
					string str=Procedures.GetDescription(procedure);
					row.Cells.Add(str);
				}
				row.Cells.Add(refAttach.Note);
				Referral referral=ReferralL.GetReferral(refAttach.ReferralNum,false);
				if(referral==null) {
					hasInvalidRef=true;
					continue;
				}
				row.Cells.Add(referral.EMail);
				row.Tag=refAttach;
				gridMain.ListGridRows.Add(row);
			}
			if(hasInvalidRef) {
				MsgBox.Show("Referrals","Could not retrieve referral. Please run Database Maintenance or call support.");
			}
			gridMain.EndUpdate();
			for(int i=0;i<_listRefAttaches.Count;i++) {
				if(_listRefAttaches[i].RefAttachNum==DefaultRefAttachNum) {
					gridMain.SetSelected(i,true);
					break;
				}
			}
		}

		private void gridMain_CellDoubleClick(object sender,GridClickEventArgs e) {
			//This does not automatically select a retattch when in selection mode; it just lets user edit.
			FrmRefAttachEdit frmRefAttachEdit=new FrmRefAttachEdit();
			RefAttach refattach=((RefAttach)gridMain.ListGridRows[e.Row].Tag).Copy();
			frmRefAttachEdit.RefAttachCur=refattach;
			frmRefAttachEdit.ShowDialog();
			FillGrid();
			//reselect
			for(int i=0;i<_listRefAttaches.Count;i++){
				if(_listRefAttaches[i].RefAttachNum==refattach.RefAttachNum) {
					gridMain.SetSelected(i,true);
					break;
				}
			}
		}

		private void butAddFrom_Click(object sender,System.EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.RefAttachAdd)) {
				return;
			}
			FrmReferralSelect frmReferralSelect=new FrmReferralSelect();
			frmReferralSelect.IsSelectionMode=true;
			frmReferralSelect.ShowDialog();
			if(frmReferralSelect.IsDialogCancel) {
				return;
			}
			RefAttach refattach=new RefAttach();
			refattach.ReferralNum=frmReferralSelect.ReferralSelected.ReferralNum;
			refattach.PatNum=PatNum;
			refattach.RefType=ReferralType.RefFrom;
			refattach.RefDate=DateTime.Today;
			refattach.IsTransitionOfCare=frmReferralSelect.ReferralSelected.IsDoctor;
			refattach.ItemOrder=_listRefAttaches.Select(x=>x.ItemOrder+1).OrderByDescending(x=>x).FirstOrDefault();//Max+1 or 0
			RefAttaches.Insert(refattach);
			SecurityLogs.MakeLogEntry(EnumPermType.RefAttachAdd,PatNum,"Referred From "+Referrals.GetNameFL(refattach.ReferralNum));
			FillGrid();
			for(int i=0;i<_listRefAttaches.Count;i++){
				if(_listRefAttaches[i].RefAttachNum==refattach.RefAttachNum) {
					gridMain.SetSelected(i,true);
					break;
				}
			}
		}

		private void sendSummaryOfCare(Referral referral, long refAttachNum) {
			Patient patient=Patients.GetPat(PatNum);
			string strCcdValidationErrors=EhrCCD.ValidateSettings();
			//This is like FormEhrClinicalSummary.butSendToPortal_Click such that the email gets treated like a web mail.
			if(strCcdValidationErrors!="") {
				MsgBox.Show(this,"There was a problem automatically sending a summary of care.  Please go to the EHR dashboard to send a summary of care to meet the summary of care core measure.");
				return;
			}
			strCcdValidationErrors=EhrCCD.ValidatePatient(patient);
			if(strCcdValidationErrors!="") {
				MsgBox.Show(this,"There was a problem automatically sending a summary of care.  Please go to the EHR dashboard to send a summary of care to meet the summary of care core measure.");
				return;
			}
			Provider provider=null;
			if(Security.CurUser.ProvNum!=0) {
				provider=Providers.GetProv(Security.CurUser.ProvNum);
			}
			else {
				provider=Providers.GetProv(patient.PriProv);
			}
			EmailMessage emailMessage=new EmailMessage();//New mail object				
			emailMessage.FromAddress=provider.GetFormalName();//Adding from address
			emailMessage.ToAddress=patient.GetNameFL();//Adding to address
			emailMessage.PatNum=patient.PatNum;//Adding patient number
			emailMessage.SentOrReceived=EmailSentOrReceived.WebMailSent;//Setting to sent
			emailMessage.ProvNumWebMail=provider.ProvNum;//Adding provider number
			emailMessage.Subject="Referral To "+referral.GetNameFL();
			emailMessage.BodyText=
				"You have been referred to another provider.  Your summary of care is attached.\r\n"
				+"You may give a copy of this summary of care to the referred provider if desired.\r\n"
				+"The contact information for the doctor you are being referred to is as follows:\r\n"
				+"\r\n";
			//Here we provide the same information that would go out on a Referral Slip.
			//When the user prints a Referral Slip, the doctor referred to information is included and contains the doctor's name, address, and phone.
			emailMessage.BodyText+="Name: "+referral.GetNameFL()+"\r\n";
			if(referral.Address.Trim()!="") {
				emailMessage.BodyText+="Address: "+referral.Address.Trim()+"\r\n";
				if(referral.Address2.Trim()!="") {
					emailMessage.BodyText+="\t"+referral.Address2.Trim()+"\r\n";
				}
				emailMessage.BodyText+="\t"+referral.City+" "+referral.ST+" "+referral.Zip+"\r\n";
			}
			if(referral.Telephone!="") {
				emailMessage.BodyText+="Phone: "+TelephoneNumbers.ReFormat(referral.Telephone)+"\r\n";
			}
			emailMessage.BodyText+=
				"\r\n"
				+"To view the Summary of Care for the referral to this provider:\r\n"
				+"1) Download all attachments to the same folder.  Do not rename the files.\r\n"
				+"2) Open the ccd.xml file in an internet browser.";
			emailMessage.MsgDateTime=DateTime.Now;//Message time is now
			emailMessage.PatNumSubj=patient.PatNum;//Subject of the message is current patient
			string ccd="";
			Cursor=Cursors.Wait;
			try {
				ccd=EhrCCD.GenerateSummaryOfCare(Patients.GetPat(PatNum),out string warnings);//Create summary of care, can throw exceptions
				if(!string.IsNullOrEmpty(warnings)) {
					this.Cursor=Cursors.Arrow; 
					if(MsgBox.Show(this,MsgBoxButtons.OKCancel,warnings,"Warnings")==IsDialogCancel) {
						return;
					}
				}
				emailMessage.Attachments.Add(EmailAttaches.CreateAttach("ccd.xml",Encoding.UTF8.GetBytes(ccd)));//Create summary of care attachment, can throw exceptions
				emailMessage.Attachments.Add(EmailAttaches.CreateAttach("ccd.xsl",Encoding.UTF8.GetBytes(EhrSummaryCcds.GetEhrResource("CCD"))));//Create xsl attachment, can throw exceptions
			}
			catch {
				//We are just trying to be helpful so it doesn't really matter if something failed above. 
				//They can simply go to the EHR dashboard and send the summary of care manually like they always have.  They will get detailed validation errors there.
				MsgBox.Show(this,"There was a problem automatically sending a summary of care.  Please go to the EHR dashboard to send a summary of care to meet the summary of care core measure.");
				return;
			}
			emailMessage.MsgType=EmailMessageSource.WebMail;
			EmailMessages.Insert(emailMessage);//Insert mail into DB for patient portal
			EhrMeasureEvent ehrMeasureEvent=new EhrMeasureEvent();
			ehrMeasureEvent.DateTEvent=DateTime.Now;
			ehrMeasureEvent.EventType=EhrMeasureEventType.SummaryOfCareProvidedToDr;
			ehrMeasureEvent.PatNum=patient.PatNum;
			ehrMeasureEvent.FKey=refAttachNum;//Can be 0 if user didn't pick a referral for some reason.
			EhrMeasureEvents.Insert(ehrMeasureEvent);
		}

		private void butAddTo_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.RefAttachAdd)) {
				return;
			}
			FrmReferralSelect frmReferralSelect=new FrmReferralSelect();
			frmReferralSelect.IsSelectionMode=true;
			frmReferralSelect.ShowDialog();
			if(frmReferralSelect.IsDialogCancel) {
				return;
			}
			RefAttach refattach=new RefAttach();
			refattach.ReferralNum=frmReferralSelect.ReferralSelected.ReferralNum;
			refattach.PatNum=PatNum;
			refattach.RefType=ReferralType.RefTo;
			refattach.RefDate=DateTime.Today;
			refattach.IsTransitionOfCare=frmReferralSelect.ReferralSelected.IsDoctor;
			refattach.ItemOrder=_listRefAttaches.Select(x=>x.ItemOrder+1).OrderByDescending(x=>x).FirstOrDefault();//Max+1 or 0
			refattach.ProcNum=ProcNum;
			//We want to help EHR users meet their measures.  Therefore, we are going to make an educated guess as to who is making this referral.
			//We are doing this for non-EHR users as well because we think it might be nice automation.
			long provNumLastAppt=Appointments.GetProvNumFromLastApptForPat(PatNum);
			if(Security.CurUser.ProvNum!=0) {
				refattach.ProvNum=Security.CurUser.ProvNum;
			}
			else if(provNumLastAppt!=0) {
				refattach.ProvNum=provNumLastAppt;
			}
			else {
				refattach.ProvNum=Patients.GetProvNum(PatNum);
			}
			RefAttaches.Insert(refattach);
			SecurityLogs.MakeLogEntry(EnumPermType.RefAttachAdd,PatNum,"Referred To "+Referrals.GetNameFL(refattach.ReferralNum));
			if(PrefC.GetBool(PrefName.AutomaticSummaryOfCareWebmail)) {
				FrmRefAttachEdit frmRefAttachEdit=new FrmRefAttachEdit();
				frmRefAttachEdit.RefAttachCur=refattach;
				frmRefAttachEdit.ShowDialog();
				//In order to help offices meet EHR Summary of Care measure 1 of Core Measure 15 of 17, we are going to send a summary of care to the patient portal behind the scenes.
				//We can send the summary of care to the patient instead of to the Dr. because of the following point in the Additional Information section of the Core Measure:
				//"The EP can send an electronic or paper copy of the summary care record directly to the next provider or can provide it to the patient to deliver to the next provider, if the patient can reasonably expected to do so and meet Measure 1."
				//We will only send the summary of care if the ref attach is a TO referral and is a transition of care.
				if(frmRefAttachEdit.IsDialogOK && refattach.RefType==ReferralType.RefTo && refattach.IsTransitionOfCare) {
					sendSummaryOfCare(frmReferralSelect.ReferralSelected, frmRefAttachEdit.RefAttachCur.RefAttachNum);
				}
			}
			Cursor=Cursors.Arrow; //Cursors.Default doesn't exist in WPF
			FillGrid();
			int index=-1;
			for(int i=0;i<_listRefAttaches.Count;i++) {
				if(_listRefAttaches[i].ReferralNum==refattach.ReferralNum) {
					index=i;
				}
			}
			gridMain.SetSelected(index,true);
		}

		private void butAddCustom_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.RefAttachAdd)) {
				return;
			}
			FrmReferralSelect frmReferralSelect=new FrmReferralSelect();
			frmReferralSelect.IsSelectionMode=true;
			frmReferralSelect.ShowDialog();
			if(frmReferralSelect.IsDialogCancel) {
				return;
			}
			RefAttach refattach=new RefAttach();
			refattach.ReferralNum=frmReferralSelect.ReferralSelected.ReferralNum;
			refattach.PatNum=PatNum;
			refattach.RefType=ReferralType.RefCustom;
			refattach.RefDate=DateTime.Today;
			refattach.IsTransitionOfCare=false;
			refattach.ItemOrder=_listRefAttaches.Select(x=>x.ItemOrder+1).OrderByDescending(x=>x).FirstOrDefault();//Max+1 or 0
			RefAttaches.Insert(refattach);
			SecurityLogs.MakeLogEntry(EnumPermType.RefAttachAdd,PatNum,"Referred (custom) "+Referrals.GetNameFL(refattach.ReferralNum));
			FillGrid();
			for(int i=0;i<_listRefAttaches.Count;i++){
				if(_listRefAttaches[i].RefAttachNum==refattach.RefAttachNum) {
					gridMain.SetSelected(i,true);
					break;
				}
			}
		}

		private void butSlip_Click(object sender,EventArgs e) {
			int idx=gridMain.GetSelectedIndex();
			if(idx==-1){
				MsgBox.Show(this,"Please select a referral first");
				return;
			}
			//The selected referral's provNum will be non zero if referral type is "referred to".
			RefAttach refAttach=(RefAttach)gridMain.ListGridRows[idx].Tag;
			Referral referral=ReferralL.GetReferral(refAttach.ReferralNum);
			if(referral==null) {
				return;
			}
			SheetDef sheetDef;
			if(referral.Slip==0){
				sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.ReferralSlip);
			}
			else{
				sheetDef=SheetDefs.GetSheetDef(referral.Slip);
			}
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,PatNum);
			SheetParameter.SetParameter(sheet,"PatNum",PatNum);
			SheetParameter.SetParameter(sheet,"ReferralNum",referral.ReferralNum);
			SheetFiller.FillFields(sheet:sheet,refAttachProvNum:refAttach.ProvNum);//Attempt to pass in the selected referral's provider rather than defaulting to patient's provider.
			SheetUtil.CalculateHeights(sheet);
			FormLauncher formLauncherSheetFillEdit=new FormLauncher(EnumFormName.FormSheetFillEdit);
			formLauncherSheetFillEdit.SetField("SheetCur",sheet);
			formLauncherSheetFillEdit.ShowDialog();
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length!=1) {
				MsgBox.Show(this,"Please select exactly one referral first.");
				return;
			}
			int selectedIndex=gridMain.GetSelectedIndex();
			if(selectedIndex==0) {//already at top
				return;
			}
			RefAttach refAttachSource=((RefAttach)gridMain.ListGridRows[selectedIndex].Tag);
			RefAttach refAttachDest=((RefAttach)gridMain.ListGridRows[selectedIndex-1].Tag);
			int sourceIdx=refAttachSource.ItemOrder;
			refAttachSource.ItemOrder=refAttachDest.ItemOrder;
			RefAttaches.Update(refAttachSource);
			refAttachDest.ItemOrder=sourceIdx;
			RefAttaches.Update(refAttachDest);
			if(!gridMain.SwapRows(selectedIndex,selectedIndex-1)) {
				MsgBox.Show(this,"Unable to change order.");
				return;
			}
			gridMain.SetSelected(selectedIndex-1,true);
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length!=1) {
				MsgBox.Show(this,"Please select exactly one referral first.");
				return;
			}
			int selectedIndex=gridMain.GetSelectedIndex();
			if(selectedIndex==gridMain.ListGridRows.Count-1) {//already at bottom
				return;
			}
			RefAttach refAttachSource=((RefAttach)gridMain.ListGridRows[selectedIndex].Tag);
			RefAttach refAttachDest=((RefAttach)gridMain.ListGridRows[selectedIndex+1].Tag);
			int sourceIdx=refAttachSource.ItemOrder;
			refAttachSource.ItemOrder=refAttachDest.ItemOrder;
			RefAttaches.Update(refAttachSource);
			refAttachDest.ItemOrder=sourceIdx;
			RefAttaches.Update(refAttachDest);
			if(!gridMain.SwapRows(selectedIndex,selectedIndex+1)) {
				MsgBox.Show(this,"Unable to change order.");
				return;
			}			
			gridMain.SetSelected(selectedIndex+1,true);
		}

		private void FrmReferralsPatient_PreviewKeyDown(object sender,KeyEventArgs e){
			if(butUp.IsAltKey(Key.U,e)){
				butUp_Click(this,new EventArgs());
			}
			if(butDown.IsAltKey(Key.D,e)){
				butDown_Click(this,new EventArgs());
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex() < 0) {
				MsgBox.Show(this,"Please select a referral first");
				return;
			}
			if(IsSelectionMode && PrefC.GetBool(PrefName.ShowFeatureEhr)) {
				string warning="";
				if(_listRefAttaches[gridMain.GetSelectedIndex()].ProvNum==0) {
					warning+=Lans.g(this,"Selected patient referral does not have a referring provider set.");
				}
				if(_listRefAttaches[gridMain.GetSelectedIndex()].RefType!=ReferralType.RefTo) {
					if(warning!="") {
						warning+="\r\n";
					}
					warning+=Lans.g(this,"Selected patient referral is not an outgoing referral.");
				}
				if(!_listRefAttaches[gridMain.GetSelectedIndex()].IsTransitionOfCare) {
					if(warning!="") {
						warning+="\r\n";
					}
					warning+=Lans.g(this,"Selected patient referral is not flagged as a transition of care.");
				}
				if(warning!="") {
					warning+="\r\n"+Lans.g(this,"It does not meet the EHR summary of care requirements.")+"  "+Lans.g(this,"Continue anyway?");
					if(MsgBox.Show(this,MsgBoxButtons.OKCancel,warning,Lans.g(this,"EHR Measure Warning"))==IsDialogCancel) {
						return;
					}
				}
			}
			RefAttachNum=_listRefAttaches[gridMain.GetSelectedIndex()].RefAttachNum;
			IsDialogOK=true;
		}

	}
}