using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls;
using WpfControls.UI;

namespace OpenDental {
///<summary></summary>
	public partial class FrmReferralEdit:FrmODBase {
		///<summary></summary>
		public bool IsNew;
		///<summary></summary>
		private bool _isPatient;
		///<summary></summary>
		public Referral ReferralCur;
		private List<SheetDef> _listSheetDefs;
		///<summary>Tracks the list of clinics that the referral is associated with.</summary>
		private List<ReferralClinicLink> _listReferralClinicLinks;
		///<summary>Used to keep an in memory list of referral type commlogs. Only used when the referral IsNew.</summary>
		private List<Commlog> _listCommlogIsNews=new List<Commlog>();
		private bool _cancelSave=false;

		///<summary></summary>
		public FrmReferralEdit(Referral referral){
			InitializeComponent();
			ReferralCur=referral;
			if(referral.PatNum>0) {
				_isPatient=true;
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				groupSSN.Text=Lang.g(this,"CDA Number");
				radioSSN.Visible=false;
				radioTIN.Visible=false;
				labelST.Text=Lang.g(this,"Province");
				labelZip.Text=Lang.g(this,"Postal Code");
			}
			Load+=FrmReferralEdit_Load;
			gridComm.CellDoubleClick+=gridComm_CellDoubleClick;
			textOtherPhone.TextChanged+=PatientL.ValidPhone_TextChanged;
			FormClosing+=FrmReferralEdit_FormClosing;
			PreviewKeyDown+=FrmReferralEdit_PreviewKeyDown;
		}

		private void FrmReferralEdit_Load(object sender, System.EventArgs e) {
			Lang.F(this);
			labelPatient.Visible=false;//Set in load because not allowed in frm designer
			if(Plugins.HookMethod(this,"FormReferralEdit.Load",ReferralCur,IsNew)) {
				return;
			}
			comboSpecialty.Items.Clear();
			comboSpecialty.Items.AddNone<Def>(); //Add "None" onto the list
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ProviderSpecialties,true);
			comboSpecialty.Items.AddList(listDefs,x=>x.ItemName);
			if(_isPatient){
				if(IsNew){
					Text=Lang.g(this,"Add Referral"); 
					Family family=Patients.GetFamily(ReferralCur.PatNum);
					Patient patient=family.GetPatient(ReferralCur.PatNum);
					ReferralCur.Address=patient.Address;
					ReferralCur.Address2=patient.Address2;	
					ReferralCur.City=patient.City;	
					ReferralCur.EMail=patient.Email;	
					ReferralCur.FName=patient.FName;	
					ReferralCur.LName=patient.LName;	
					ReferralCur.MName=patient.MiddleI;	
					//RefCur.PatNum=Patients.Cur.PatNum;//already handled
					ReferralCur.SSN=patient.SSN;
					ReferralCur.Telephone=TelephoneNumbers.FormatNumbersExactTen(patient.HmPhone);
					if(patient.WkPhone==""){
						ReferralCur.Phone2=patient.WirelessPhone;
					}
					else{
						ReferralCur.Phone2=patient.WkPhone;
					}
					ReferralCur.ST=patient.State;	
					ReferralCur.Zip=patient.Zip;
				}
				labelPatient.Visible=true;
				textLName.ReadOnly=true;
				textFName.ReadOnly=true;
				textMName.ReadOnly=true;
				textTitle.ReadOnly=true;
				textAddress.ReadOnly=true;
				textAddress2.ReadOnly=true;
				textCity.ReadOnly=true;         
				textST.ReadOnly=true;
				textZip.ReadOnly=true;
				checkNotPerson.IsEnabled=false;
				textPhone1.ReadOnly=true;
				textPhone2.ReadOnly=true;
				textPhone3.ReadOnly=true;
				textSSN.ReadOnly=true;				
				radioTIN.IsEnabled=false;
				textEmail.ReadOnly=true;
				textBusinessName.ReadOnly=true;
				comboSpecialty.IsEnabled=false;
				comboSpecialty.SelectedIndex=-1;
				checkIsDoctor.IsEnabled=false;
				textNotes.Focus();
			}
			else{//non patient
				if(IsNew){
					this.Text=Lang.g(this,"Add Referral"); 
					ReferralCur=new Referral();
					ReferralCur.Specialty=0; //If new patient, set specialty to 0 or "None".
				}
				comboSpecialty.SetSelectedKey<Def>(ReferralCur.Specialty,x=>x.DefNum); //If specialty is "None", the selected index will default to 0.
			}
			checkIsDoctor.Checked=ReferralCur.IsDoctor;
			checkNotPerson.Checked=ReferralCur.NotPerson;
			checkHidden.Checked=ReferralCur.IsHidden;
			checkEmailTrustDirect.Checked=ReferralCur.IsTrustedDirect;
			textLName.Text=ReferralCur.LName;
			textFName.Text=ReferralCur.FName;
			textMName.Text=ReferralCur.MName;
			textTitle.Text=ReferralCur.Title;
			textAddress.Text=ReferralCur.Address;
			textAddress2.Text=ReferralCur.Address2;
			textCity.Text=ReferralCur.City;         
			textST.Text=ReferralCur.ST;
			textZip.Text=ReferralCur.Zip;
			string phone=ReferralCur.Telephone;
			if(phone!=null && phone.Length==10){
				textPhone1.Text=phone.Substring(0,3);
				textPhone2.Text=phone.Substring(3,3);
				textPhone3.Text=phone.Substring(6);
			}
			textSSN.Text=ReferralCur.SSN;
			if(ReferralCur.UsingTIN){ 
				radioTIN.Checked=true;
				radioSSN.Checked=false;
			} 
			else{
				radioSSN.Checked=true;
				radioTIN.Checked=false;
			}
			textNationalProvID.Text=ReferralCur.NationalProvID;
			textOtherPhone.Text=ReferralCur.Phone2;  
			textEmail.Text=ReferralCur.EMail; 
			textBusinessName.Text=ReferralCur.BusinessName;
			textNotes.Text=ReferralCur.Note;
			textDisplayNote.Text=ReferralCur.DisplayNote;
			_listReferralClinicLinks=ReferralClinicLinks.GetAllForReferral(ReferralCur.ReferralNum);
			if(_listReferralClinicLinks.Count==0) {
				comboClinicPicker.ClinicNumSelected=0;
			}
			else {
				comboClinicPicker.ListClinicNumsSelected=_listReferralClinicLinks.Select(x=>x.ClinicNum).ToList();
			}
			//Patients using:
			List<string> listPatsTo=RefAttaches.GetPats(ReferralCur.ReferralNum,ReferralType.RefTo);
			List<string> listPatsFrom=RefAttaches.GetPats(ReferralCur.ReferralNum,ReferralType.RefFrom);
			comboPatientsTo.Items.Clear();
			comboPatientsFrom.Items.Clear();
			List<string> listNamesUnique=listPatsTo.Distinct().ToList();
			for(int i = 0;i<listNamesUnique.Count;i++) {
				int count=listPatsTo.Count(x => x==listNamesUnique[i]);
				//Add each patient referral once with the count of how many times they've been referred.
				if(count==1) {//if unique
					comboPatientsTo.Items.Add(listNamesUnique[i]);
				}
				else {//multiple
					comboPatientsTo.Items.Add(listNamesUnique[i]+" ("+count.ToString()+")");
				}
			}
			textPatientsNumTo.Text=listNamesUnique.Count.ToString();
			listNamesUnique=listPatsFrom.Distinct().ToList();
			for(int i=0;i<listNamesUnique.Count;i++) {
				int count=listPatsFrom.Count(x => x==listNamesUnique[i]);
				if(count==1) {
					comboPatientsFrom.Items.Add(listNamesUnique[i]);
				}
				else {
					comboPatientsFrom.Items.Add(listNamesUnique[i]+" ("+count.ToString()+")");
				}
			}
			textPatientsNumFrom.Text=listNamesUnique.Count.ToString();
			if(listPatsTo.Count>0){
				comboPatientsTo.SelectedIndex=0;
			}
			if(listPatsFrom.Count>0){
				comboPatientsFrom.SelectedIndex=0;
			}
			comboSlip.Items.Add(Lang.g(this,"Default"));
			comboSlip.SelectedIndex=0;
			_listSheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.ReferralSlip);
			for(int i=0;i<_listSheetDefs.Count;i++){
				comboSlip.Items.Add(_listSheetDefs[i].Description);
				if(ReferralCur.Slip==_listSheetDefs[i].SheetDefNum){
					comboSlip.SelectedIndex=i+1;
				}
			}
			if(!IsNew && !Security.IsAuthorized(EnumPermType.ReferralEdit,true)) {//User may have add permissions but may not have access to edit.
				this.Text+=" - "+Lang.g(this,"Read Only");
			}
			checkIsPreferred.Checked=ReferralCur.IsPreferred;
			FillCommlogGrid();
		}

		private void radioSSN_Click(object sender, System.EventArgs e) {
			ReferralCur.UsingTIN=false;
		}

		private void radioTIN_Click(object sender, System.EventArgs e) {
			ReferralCur.UsingTIN=true;
		}

		private void FillCommlogGrid() {
			List<Commlog> listCommlogs=new List<Commlog>();
			if(IsNew) {
				listCommlogs=new List<Commlog>(_listCommlogIsNews);
			}
			else {
				listCommlogs=Commlogs.GetForReferral(ReferralCur.ReferralNum);
			}
			//Handle unique grid sorting:
			List<Commlog> listCommlogsAnchored=listCommlogs.FindAll(x => x.CommReferralBehavior==EnumCommReferralBehavior.TopAnchored);
			listCommlogs.RemoveAll(x => x.CommReferralBehavior==EnumCommReferralBehavior.TopAnchored);
			gridComm.BeginUpdate();
			gridComm.Columns.Clear();
			GridColumn col=new GridColumn(Lang.g("TableCommLog","Date"),65);
			gridComm.Columns.Add(col);
			col=new GridColumn(Lang.g("TableCommLog","Hidden"),50,HorizontalAlignment.Center);
			gridComm.Columns.Add(col);
			col=new GridColumn(Lang.g("TableCommLog","Description"),335);
			gridComm.Columns.Add(col);
			gridComm.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listCommlogsAnchored.Count;i++){
				if(listCommlogsAnchored[i].CommReferralBehavior==EnumCommReferralBehavior.Hidden && checkHiddenComms.Checked==false) {
					continue;
				}
				row=new GridRow();
				row.Tag=listCommlogsAnchored[i];
				//Sort by created, oldest on bottom.
				row.Cells.Add(listCommlogsAnchored[i].CommDateTime.ToShortDateString());
				if(listCommlogsAnchored[i].CommReferralBehavior==EnumCommReferralBehavior.Hidden) {
					row.Cells.Add("X");
				}
				else{
					row.Cells.Add("");
				}
				row.Cells.Add(listCommlogsAnchored[i].Note);
				//Visually indicate anchored rows.
				row.ColorBackG=Color.FromArgb(255,240,246,251);
				gridComm.ListGridRows.Add(row);
			}
			for(int i=0;i<listCommlogs.Count;i++){
				if(listCommlogs[i].CommReferralBehavior==EnumCommReferralBehavior.Hidden && checkHiddenComms.Checked==false) {
					continue;
				}
				row=new GridRow();
				row.Tag=listCommlogs[i];
				//Sort by created, oldest on bottom.
				row.Cells.Add(listCommlogs[i].CommDateTime.ToShortDateString());
				if(listCommlogs[i].CommReferralBehavior==EnumCommReferralBehavior.Hidden) {
					row.Cells.Add("X");
				}
				else{
					row.Cells.Add("");
				}
				row.Cells.Add(listCommlogs[i].Note);
				gridComm.ListGridRows.Add(row);
			}
			gridComm.EndUpdate();
		}

		private void checkHiddenComms_Click(object sender,EventArgs e) {
			FillCommlogGrid();
		}

		private void gridComm_CellDoubleClick(object sender,GridClickEventArgs e) {
			FrmCommReferral frmCommReferral=new FrmCommReferral();
			frmCommReferral.CommlogCur=(Commlog)gridComm.ListGridRows[gridComm.GetSelectedIndex()].Tag;
			frmCommReferral.CommlogCur.IsNew=false;
			frmCommReferral.ShowDialog();
			if(frmCommReferral.IsDialogOK) {
				FillCommlogGrid();
			}
		}

		private void butAddComm_Click(object sender,EventArgs e) {
			Commlog commlog=new Commlog();
			commlog.IsNew=true;
			commlog.CommDateTime=DateTime.Now;
			commlog.CommType=0;//Referrals are 0.
			commlog.UserNum=Security.CurUser.UserNum;
			commlog.ReferralNum=ReferralCur.ReferralNum;
			FrmCommReferral frmCommReferral=new FrmCommReferral();
			frmCommReferral.CommlogCur=commlog;
			frmCommReferral.ShowDialog();
			if(frmCommReferral.IsDialogOK) {
				if(IsNew) {
					_listCommlogIsNews.Add(frmCommReferral.CommlogCur);
				}
				FillCommlogGrid();
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				//User may have created referral commlogs. If so delete them.
				for(int i=0;i<_listCommlogIsNews.Count;i++) {
					Commlogs.Delete(_listCommlogIsNews[i]);
				}
				IsDialogOK=false;
				return;
			}
			if(!Security.IsAuthorized(EnumPermType.ReferralEdit)) {
				return;
			}	
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
				return;
			}
			try {
				Referrals.Delete(ReferralCur);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			//Referrals.Delete verified there were no attached patients, claims or procedures, so it's safe to delete any attached referral commlogs.
			Commlogs.DeleteForReferral(ReferralCur.ReferralNum);
			string secLogText="Referral '"+ReferralCur.LName+", "+ReferralCur.FName+"' deleted";
			SecurityLogs.MakeLogEntry(EnumPermType.ReferralEdit,ReferralCur.PatNum,secLogText); //ReferralCur.PatNum should be 0 if not a patient.
			IsDialogOK=true;
		}

		private void FrmReferralEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(!IsNew && !Security.IsAuthorized(EnumPermType.ReferralEdit)) {//User may have add permissions but may not have access to edit.
				return;
			}
			string phone=textPhone1.Text+textPhone2.Text+textPhone3.Text;
			if(phone.Length > 0 && phone.Length < 10){
				MessageBox.Show(Lang.g(this,"Invalid phone"));
				return;
			}
			if(textLName.Text=="") {
				MsgBox.Show(this,"Please enter a last name.");
				return;
			}
			if(comboSpecialty.SelectedIndex==-1 && !_isPatient) {
				MsgBox.Show(this,"Please select a specialty.");
				return;
			}
			Cursor=Cursors.Wait;
			if(checkEmailTrustDirect.Checked==true && !EmailMessages.TryAddTrustDirect(textEmail.Text)) {
				string trustEmailErrorMessage=Lang.g("Referral","You elected to trust this email address for Direct messaging.")
					+"  "+Lang.g("Referral","Adding trust for the address failed, because we were unable to locate the public certificate for the address.")
					+"  "+Lang.g("Referral","Check that the email address is correctly typed and is a valid Direct messaging address, then try again.")
					+"  "+Lang.g("Referral","Otherwise, uncheck the E-mail Trust for Direct checkbox before clicking OK or press Cancel if no other changes were made.");
				MessageBox.Show(trustEmailErrorMessage);
				Cursor=Cursors.Arrow;
				return;
			}
			Cursor=Cursors.Arrow;
			ReferralCur.IsHidden=checkHidden.Checked==true;
			ReferralCur.NotPerson=checkNotPerson.Checked==true;
			ReferralCur.IsTrustedDirect=checkEmailTrustDirect.Checked==true;
			ReferralCur.LName=textLName.Text;
			ReferralCur.FName=textFName.Text;
			ReferralCur.MName=textMName.Text;
			ReferralCur.Title=textTitle.Text;
			ReferralCur.Address=textAddress.Text;
			ReferralCur.Address2=textAddress2.Text;
			ReferralCur.City=textCity.Text;
			ReferralCur.ST=textST.Text;
			ReferralCur.Zip=textZip.Text;
			ReferralCur.Telephone=phone;
			ReferralCur.Phone2=textOtherPhone.Text;    
			ReferralCur.SSN=textSSN.Text;
			ReferralCur.NationalProvID=textNationalProvID.Text;
			ReferralCur.EMail=textEmail.Text;
			ReferralCur.BusinessName=textBusinessName.Text;
			ReferralCur.Note=textNotes.Text; 
			ReferralCur.DisplayNote=textDisplayNote.Text;
			ReferralCur.Slip=0;
			ReferralCur.IsPreferred=checkIsPreferred.Checked==true;
			if(comboSlip.SelectedIndex>0){
				ReferralCur.Slip=_listSheetDefs[comboSlip.SelectedIndex-1].SheetDefNum;
			}
			//ReferralCur.UsingTIN already taken care of
			if(!_isPatient){
				ReferralCur.IsDoctor=checkIsDoctor.Checked==true;
				ReferralCur.Specialty=comboSpecialty.GetSelectedKey<Def>(x=>x.DefNum); //Specialty will be set to 0 if "None" or does not exist.
			}
			if(IsNew){
				if(Referrals.GetExists(x => x.LName+x.FName==ReferralCur.LName+ReferralCur.FName)) {
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Referral of same name exists. Add anyway?")) {
						_cancelSave=true;
						IsDialogOK=false;
						return;
					}
				}
				string secLogText="Referral '"+ReferralCur.LName+", "+ReferralCur.FName+"' created";
				SecurityLogs.MakeLogEntry(EnumPermType.ReferralAdd,ReferralCur.PatNum,secLogText);
				Referrals.Insert(ReferralCur);
				Signalods.SetInvalid(InvalidType.Referral);//Make sure other instances have the new referral in their cache.
				ReferralClinicLinks.InsertClinicLinksForReferral(ReferralCur.ReferralNum,comboClinicPicker.ListClinicNumsSelected,false);
				//Referral IsNew and may have commlog entries, so update all the associated commlogs with the correct ReferralNum.
				for(int i=0;i<_listCommlogIsNews.Count;i++) {
					Commlog commlogOld=_listCommlogIsNews[i].Copy();
					_listCommlogIsNews[i].ReferralNum=ReferralCur.ReferralNum;
					Commlogs.Update(_listCommlogIsNews[i],commlogOld);
				}
			}
			else{
				string secLogText="Referral '"+ReferralCur.LName+", "+ReferralCur.FName+"' edited";
				SecurityLogs.MakeLogEntry(EnumPermType.ReferralEdit,ReferralCur.PatNum,secLogText);
				Referrals.Update(ReferralCur);
				ReferralClinicLinks.InsertClinicLinksForReferral(ReferralCur.ReferralNum,comboClinicPicker.ListClinicNumsSelected,true);
			}
			Referrals.RefreshCache();//We might want to enhance this to call DataValid.SetInvalid(InvalidType.Referral); eventually.
			//MessageBox.Show(ReferralCur.ReferralNum.ToString());
			IsDialogOK=true;
		}

		private void FrmReferralEdit_FormClosing(object sender,CancelEventArgs e) {
			if(IsDialogOK) {
				return;
			}
			if(_cancelSave) {
				return;
			}
			//User canceled out of form, but may have created referral commlogs. If so delete them.
			if(IsNew) {
				for(int i=0;i<_listCommlogIsNews.Count;i++) {
					Commlogs.Delete(_listCommlogIsNews[i]);
				}
			}
		}

	}
}