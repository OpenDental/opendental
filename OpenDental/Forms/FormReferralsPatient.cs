using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormReferralsPatient : FormODBase {
		public long PatNum;
		private List<RefAttach> RefAttachList;
		///<summary>This number is normally zero.  If this number is set externally before opening this form, then this will behave differently.</summary>
		public long ProcNum;
		///<summary>Selection mode is currently only used for transitions of care.  Changes text of butClose to Cancel and shows OK and None buttons.</summary>
		public bool IsSelectionMode;
		///<summary>This number is normally zero.  If in selection mode, this will be the PK of the selected refattach.</summary>
		public long RefAttachNum;
		///<summary>This number is normally zero.  If form is opened by double clicking a summary of care event then this will be filled with the current FKey of that measure event.</summary>
		public long DefaultRefAttachNum;

		///<summary></summary>
		public FormReferralsPatient()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormReferralsPatient_Load(object sender,EventArgs e) {
			if(IsSelectionMode) {
				gridMain.SelectionMode=GridSelectionMode.One;
				butClose.Text="Cancel";
				butOK.Visible=true;
			}
			if(ProcNum!=0) {
				Text=Lan.g(this,"Referrals");
				butAddFrom.Visible=false;
				butAddCustom.Visible=false;
			}
			else {//all for patient
				checkShowAll.Visible=false;//we will always show all
			}
			FillGrid();
			if(RefAttachList.Count>0 && !IsSelectionMode) {
				gridMain.SetSelected(0,true);
			}
			Plugins.HookAddCode(this,"FormReferralsPatient.Load_end");
		}

		private void checkShowAll_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			RefAttachList=RefAttaches.RefreshFiltered(PatNum,true,0);
			string referralDescript=DisplayFields.GetForCategory(DisplayFieldCategory.PatientInformation)
				.FirstOrDefault(x => x.InternalName=="Referrals")?.Description;
			if(string.IsNullOrWhiteSpace(referralDescript)) {//either not displaying the Referral field or no description entered, default to 'Referral (other)'
				referralDescript=Lan.g(this,"Referral (other)");
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableRefList","Referral Type"),85));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableRefList","Name"),120));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableRefList","Date"),65));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableRefList","Status"),70));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableRefList","Proc"),120));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableRefList","Note"),170));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableRefList","Email"),190));
			gridMain.ListGridRows.Clear();
			bool hasInvalidRef=false;
			GridRow row;
			List<string> listRefTypeNames=new List<string>() {Lan.g(this,"To"),Lan.g(this,"From"),referralDescript };
			for(int i=0;i<RefAttachList.Count;i++) {
				RefAttach refAttachCur=RefAttachList[i];
				if(ProcNum != 0 && !checkShowAll.Checked
					&& ProcNum != refAttachCur.ProcNum)
				{
					continue;
				}
				row=new GridRow();
				row.Cells.Add(listRefTypeNames[(int)refAttachCur.RefType]);
				row.Cells.Add(Referrals.GetNameFL(refAttachCur.ReferralNum));
				if(refAttachCur.RefDate.Year < 1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(refAttachCur.RefDate.ToShortDateString());
				}
				row.Cells.Add(Lan.g("enumReferralToStatus",refAttachCur.RefToStatus.ToString()));
				if(refAttachCur.ProcNum==0) {
					row.Cells.Add("");
				}
				else {
					Procedure proc=Procedures.GetOneProc(refAttachCur.ProcNum,false);
					string str=Procedures.GetDescription(proc);
					row.Cells.Add(str);
				}
				row.Cells.Add(refAttachCur.Note);
				Referral referral=ReferralL.GetReferral(refAttachCur.ReferralNum,false);
				if(referral==null) {
					hasInvalidRef=true;
					continue;
				}
				row.Cells.Add(referral.EMail);
				row.Tag=refAttachCur;
				gridMain.ListGridRows.Add(row);
			}
			if(hasInvalidRef) {
				ReferralL.ShowReferralErrorMsg();
			}
			gridMain.EndUpdate();
			for(int i=0;i<RefAttachList.Count;i++) {
				if(RefAttachList[i].RefAttachNum==DefaultRefAttachNum) {
					gridMain.SetSelected(i,true);
					break;
				}
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//This does not automatically select a retattch when in selection mode; it just lets user edit.
			using FormRefAttachEdit FormRAE2=new FormRefAttachEdit();
			RefAttach refattach=((RefAttach)gridMain.ListGridRows[e.Row].Tag).Copy();
			FormRAE2.RefAttachCur=refattach;
			FormRAE2.ShowDialog();
			FillGrid();
			//reselect
			for(int i=0;i<RefAttachList.Count;i++){
				if(RefAttachList[i].RefAttachNum==refattach.RefAttachNum) {
					gridMain.SetSelected(i,true);
					break;
				}
			}
		}

		private void butAddFrom_Click(object sender,System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.RefAttachAdd)) {
				return;
			}
			using FormReferralSelect FormRS=new FormReferralSelect();
			FormRS.IsSelectionMode=true;
			FormRS.ShowDialog();
			if(FormRS.DialogResult!=DialogResult.OK) {
				return;
			}
			RefAttach refattach=new RefAttach();
			refattach.ReferralNum=FormRS.SelectedReferral.ReferralNum;
			refattach.PatNum=PatNum;
			refattach.RefType=ReferralType.RefFrom;
			refattach.RefDate=DateTime.Today;
			refattach.IsTransitionOfCare=FormRS.SelectedReferral.IsDoctor;
			refattach.ItemOrder=RefAttachList.Select(x=>x.ItemOrder+1).OrderByDescending(x=>x).FirstOrDefault();//Max+1 or 0
			RefAttaches.Insert(refattach);
			SecurityLogs.MakeLogEntry(Permissions.RefAttachAdd,PatNum,"Referred From "+Referrals.GetNameFL(refattach.ReferralNum));
			FillGrid();
			for(int i=0;i<RefAttachList.Count;i++){
				if(RefAttachList[i].RefAttachNum==refattach.RefAttachNum) {
					gridMain.SetSelected(i,true);
					break;
				}
			}
		}

		private void butAddTo_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.RefAttachAdd)) {
				return;
			}
			using FormReferralSelect FormRS=new FormReferralSelect();
			FormRS.IsSelectionMode=true;
			FormRS.ShowDialog();
			if(FormRS.DialogResult!=DialogResult.OK) {
				return;
			}
			RefAttach refattach=new RefAttach();
			refattach.ReferralNum=FormRS.SelectedReferral.ReferralNum;
			refattach.PatNum=PatNum;
			refattach.RefType=ReferralType.RefTo;
			refattach.RefDate=DateTime.Today;
			refattach.IsTransitionOfCare=FormRS.SelectedReferral.IsDoctor;
			refattach.ItemOrder=RefAttachList.Select(x=>x.ItemOrder+1).OrderByDescending(x=>x).FirstOrDefault();//Max+1 or 0
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
			SecurityLogs.MakeLogEntry(Permissions.RefAttachAdd,PatNum,"Referred To "+Referrals.GetNameFL(refattach.ReferralNum));
			if(PrefC.GetBool(PrefName.AutomaticSummaryOfCareWebmail)) {
				using FormRefAttachEdit FormRAE=new FormRefAttachEdit();
				FormRAE.RefAttachCur=refattach;
				FormRAE.ShowDialog();
				//In order to help offices meet EHR Summary of Care measure 1 of Core Measure 15 of 17, we are going to send a summary of care to the patient portal behind the scenes.
				//We can send the summary of care to the patient instead of to the Dr. because of the following point in the Additional Information section of the Core Measure:
				//"The EP can send an electronic or paper copy of the summary care record directly to the next provider or can provide it to the patient to deliver to the next provider, if the patient can reasonably expected to do so and meet Measure 1."
				//We will only send the summary of care if the ref attach is a TO referral and is a transition of care.
				if(FormRAE.DialogResult==DialogResult.OK && refattach.RefType==ReferralType.RefTo && refattach.IsTransitionOfCare) {
					try {
						//This is like FormEhrClinicalSummary.butSendToPortal_Click such that the email gets treated like a web mail.
						Patient PatCur=Patients.GetPat(PatNum);
						string strCcdValidationErrors=EhrCCD.ValidateSettings();
						if(strCcdValidationErrors!="") {
							throw new Exception();
						}
						strCcdValidationErrors=EhrCCD.ValidatePatient(PatCur);
						if(strCcdValidationErrors!="") {
							throw new Exception();
						}
						Provider prov=null;
						if(Security.CurUser.ProvNum!=0) {
							prov=Providers.GetProv(Security.CurUser.ProvNum);
						}
						else {
							prov=Providers.GetProv(PatCur.PriProv);
						}
						EmailMessage msgWebMail=new EmailMessage();//New mail object				
						msgWebMail.FromAddress=prov.GetFormalName();//Adding from address
						msgWebMail.ToAddress=PatCur.GetNameFL();//Adding to address
						msgWebMail.PatNum=PatCur.PatNum;//Adding patient number
						msgWebMail.SentOrReceived=EmailSentOrReceived.WebMailSent;//Setting to sent
						msgWebMail.ProvNumWebMail=prov.ProvNum;//Adding provider number
						msgWebMail.Subject="Referral To "+FormRS.SelectedReferral.GetNameFL();
						msgWebMail.BodyText=
							"You have been referred to another provider.  Your summary of care is attached.\r\n"
							+"You may give a copy of this summary of care to the referred provider if desired.\r\n"
							+"The contact information for the doctor you are being referred to is as follows:\r\n"
							+"\r\n";
						//Here we provide the same information that would go out on a Referral Slip.
						//When the user prints a Referral Slip, the doctor referred to information is included and contains the doctor's name, address, and phone.
						msgWebMail.BodyText+="Name: "+FormRS.SelectedReferral.GetNameFL()+"\r\n";
						if(FormRS.SelectedReferral.Address.Trim()!="") {
							msgWebMail.BodyText+="Address: "+FormRS.SelectedReferral.Address.Trim()+"\r\n";
							if(FormRS.SelectedReferral.Address2.Trim()!="") {
								msgWebMail.BodyText+="\t"+FormRS.SelectedReferral.Address2.Trim()+"\r\n";
							}
							msgWebMail.BodyText+="\t"+FormRS.SelectedReferral.City+" "+FormRS.SelectedReferral.ST+" "+FormRS.SelectedReferral.Zip+"\r\n";
						}
						if(FormRS.SelectedReferral.Telephone!="") {
							msgWebMail.BodyText+="Phone: ("+FormRS.SelectedReferral.Telephone.Substring(0,3)+")"+FormRS.SelectedReferral.Telephone.Substring(3,3)+"-"+FormRS.SelectedReferral.Telephone.Substring(6)+"\r\n";
						}
						msgWebMail.BodyText+=
							"\r\n"
							+"To view the Summary of Care for the referral to this provider:\r\n"
							+"1) Download all attachments to the same folder.  Do not rename the files.\r\n"
							+"2) Open the ccd.xml file in an internet browser.";
						msgWebMail.MsgDateTime=DateTime.Now;//Message time is now
						msgWebMail.PatNumSubj=PatCur.PatNum;//Subject of the message is current patient
						string ccd="";
						Cursor=Cursors.WaitCursor;
						ccd=EhrCCD.GenerateSummaryOfCare(Patients.GetPat(PatNum));//Create summary of care, can throw exceptions but they're caught below
						msgWebMail.Attachments.Add(EmailAttaches.CreateAttach("ccd.xml",Encoding.UTF8.GetBytes(ccd)));//Create summary of care attachment, can throw exceptions but caught below
						msgWebMail.Attachments.Add(EmailAttaches.CreateAttach("ccd.xsl",Encoding.UTF8.GetBytes(FormEHR.GetEhrResource("CCD"))));//Create xsl attachment, can throw exceptions
						msgWebMail.MsgType=EmailMessageSource.WebMail;
						EmailMessages.Insert(msgWebMail);//Insert mail into DB for patient portal
						EhrMeasureEvent newMeasureEvent=new EhrMeasureEvent();
						newMeasureEvent.DateTEvent=DateTime.Now;
						newMeasureEvent.EventType=EhrMeasureEventType.SummaryOfCareProvidedToDr;
						newMeasureEvent.PatNum=PatCur.PatNum;
						newMeasureEvent.FKey=FormRAE.RefAttachCur.RefAttachNum;//Can be 0 if user didn't pick a referral for some reason.
						EhrMeasureEvents.Insert(newMeasureEvent);
					}
					catch {
						//We are just trying to be helpful so it doesn't really matter if something failed above. 
						//They can simply go to the EHR dashboard and send the summary of care manually like they always have.  They will get detailed validation errors there.
						MsgBox.Show(this,"There was a problem automatically sending a summary of care.  Please go to the EHR dashboard to send a summary of care to meet the summary of care core measure.");
					}
				}
			}
			Cursor=Cursors.Default;
			FillGrid();
			for(int i=0;i<RefAttachList.Count;i++) {
				if(RefAttachList[i].ReferralNum==refattach.ReferralNum) {
					gridMain.SetSelected(i,true);
				}
			}
		}

		private void butAddCustom_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.RefAttachAdd)) {
				return;
			}
			using FormReferralSelect FormRS=new FormReferralSelect();
			FormRS.IsSelectionMode=true;
			FormRS.ShowDialog();
			if(FormRS.DialogResult!=DialogResult.OK) {
				return;
			}
			RefAttach refattach=new RefAttach();
			refattach.ReferralNum=FormRS.SelectedReferral.ReferralNum;
			refattach.PatNum=PatNum;
			refattach.RefType=ReferralType.RefCustom;
			refattach.RefDate=DateTime.Today;
			refattach.IsTransitionOfCare=false;
			refattach.ItemOrder=RefAttachList.Select(x=>x.ItemOrder+1).OrderByDescending(x=>x).FirstOrDefault();//Max+1 or 0
			RefAttaches.Insert(refattach);
			SecurityLogs.MakeLogEntry(Permissions.RefAttachAdd,PatNum,"Referred (custom) "+Referrals.GetNameFL(refattach.ReferralNum));
			FillGrid();
			for(int i=0;i<RefAttachList.Count;i++){
				if(RefAttachList[i].RefAttachNum==refattach.RefAttachNum) {
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
			Referral referral=ReferralL.GetReferral(((RefAttach)gridMain.ListGridRows[idx].Tag).ReferralNum);
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
			SheetFiller.FillFields(sheet);
			SheetUtil.CalculateHeights(sheet);
			FormSheetFillEdit.ShowForm(sheet);
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length!=1) {
				MsgBox.Show(this,"Please select exactly one referral first.");
				return;
			}
			int selectedIdx=gridMain.GetSelectedIndex();
			if(selectedIdx==0) {//already at top
				return;
			}
			RefAttach sourceAttach=((RefAttach)gridMain.ListGridRows[selectedIdx].Tag);
			RefAttach destAttach=((RefAttach)gridMain.ListGridRows[selectedIdx-1].Tag);
			int sourceIdx=sourceAttach.ItemOrder;
			sourceAttach.ItemOrder=destAttach.ItemOrder;
			RefAttaches.Update(sourceAttach);
			destAttach.ItemOrder=sourceIdx;
			RefAttaches.Update(destAttach);
			if(!gridMain.SwapRows(selectedIdx,selectedIdx-1)) {
				MsgBox.Show(this,"Unable to change order.");
				return;
			}
			gridMain.SetSelected(selectedIdx-1,true);
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length!=1) {
				MsgBox.Show(this,"Please select exactly one referral first.");
				return;
			}
			int selectedIdx=gridMain.GetSelectedIndex();
			if(selectedIdx==gridMain.ListGridRows.Count-1) {//already at bottom
				return;
			}
			RefAttach sourceAttach=((RefAttach)gridMain.ListGridRows[selectedIdx].Tag);
			RefAttach destAttach=((RefAttach)gridMain.ListGridRows[selectedIdx+1].Tag);
			int sourceIdx=sourceAttach.ItemOrder;
			sourceAttach.ItemOrder=destAttach.ItemOrder;
			RefAttaches.Update(sourceAttach);
			destAttach.ItemOrder=sourceIdx;
			RefAttaches.Update(destAttach);
			if(!gridMain.SwapRows(selectedIdx,selectedIdx+1)) {
				MsgBox.Show(this,"Unable to change order.");
				return;
			}			
			gridMain.SetSelected(selectedIdx+1,true);
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex() < 0) {
				MsgBox.Show(this,"Please select a referral first");
				return;
			}
			if(IsSelectionMode && PrefC.GetBool(PrefName.ShowFeatureEhr)) {
				string warning="";
				if(RefAttachList[gridMain.GetSelectedIndex()].ProvNum==0) {
					warning+=Lans.g(this,"Selected patient referral does not have a referring provider set.");
				}
				if(RefAttachList[gridMain.GetSelectedIndex()].RefType!=ReferralType.RefTo) {
					if(warning!="") {
						warning+="\r\n";
					}
					warning+=Lans.g(this,"Selected patient referral is not an outgoing referral.");
				}
				if(!RefAttachList[gridMain.GetSelectedIndex()].IsTransitionOfCare) {
					if(warning!="") {
						warning+="\r\n";
					}
					warning+=Lans.g(this,"Selected patient referral is not flagged as a transition of care.");
				}
				if(warning!="") {
					warning+="\r\n"+Lans.g(this,"It does not meet the EHR summary of care requirements.")+"  "+Lans.g(this,"Continue anyway?");
					if(MessageBox.Show(warning,Lans.g(this,"EHR Measure Warning"),MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
						return;
					}
				}
			}
			RefAttachNum=RefAttachList[gridMain.GetSelectedIndex()].RefAttachNum;
			DialogResult=DialogResult.OK;
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			if(IsSelectionMode) {//Allows us to know that the user wants to cancel out of picking a refattach.  They should click None if no refattach is needed.
				DialogResult=DialogResult.Cancel;
			}
			Close();
		}

	

		

		

		

		


	}
}





















