using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeBase;
using System.Data;
using System.Text;

namespace OpenDental{
///<summary></summary>
	public partial class FormProvEdit : FormODBase {
		///<summary>Provider Identifiers showing in the list for this provider.</summary>
		private ProviderIdent[] ListProvIdent;
		private List<SchoolClass> _listSchoolClasses;
		private Userod _existingUser;
		public Provider ProvCur;
		private List<ProviderClinic> _listProvClinicsOld;
		private List<ProviderClinic> _listProvClinicsNew;
		private ProviderClinic _provClinicDefault;
		///<summary>The clinics this provider is linked to. May include clinics the user does not have access to.</summary>
		private List<ProviderClinicLink> _listProvClinicLinks=new List<ProviderClinicLink>();
		///<summary>The clinics this user has access to.</summary>
		private List<Clinic> _listClinicsForUser=new List<Clinic>();
		///<summary></summary>
		public bool IsNew;

		protected override string GetHelpOverride() {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return "FormProvEditCanada";
			}
			return "FormProvEdit";
		}

		///<summary></summary>
		public FormProvEdit(){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
			//ProvCur=provCur;
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				labelNPI.Text=Lan.g(this,"CDA Number");
			}
			else{
				labelCanadianOfficeNum.Visible=false;
				textCanadianOfficeNum.Visible=false;
			}
		}

		private void FormProvEdit_Load(object sender, System.EventArgs e) {
			//if(IsNew){
			//	Providers.Cur.SigOnFile=true;
			//	Providers.InsertCur();
				//one field handled from previous form
			//}
			comboEhrMu.Items.Add("Use Global");
			comboEhrMu.Items.Add("Stage 1");
			comboEhrMu.Items.Add("Stage 2");
			comboEhrMu.Items.Add("Modified Stage 2");
			comboEhrMu.SelectedIndex=ProvCur.EhrMuStage;
			if(!PrefC.GetBool(PrefName.ShowFeatureEhr)) {
				comboEhrMu.Visible=false;
				labelEhrMU.Visible=false;
			}
			if(!PrefC.GetBool(PrefName.EasyHideDentalSchools) //Dental Schools is turned on
				&& (ProvCur.SchoolClassNum!=0 || ProvCur.IsInstructor))//Adding/Editing Students or Instructors
			{
				if(!ProvCur.IsNew) {
					labelPassDescription.Visible=true;
					textProvNum.Text=ProvCur.ProvNum.ToString();
					List<Userod> userList=Providers.GetAttachedUsers(ProvCur.ProvNum);
					if(userList.Count>0) {
						textUserName.Text=userList[0].UserName;//Should always happen if they are a student.
						_existingUser=userList[0];
					}
				}
				else {
					textUserName.Text=Providers.GetNextAvailableProvNum().ToString();//User-names are suggested to be the ProvNum of the provider.  This can be changed at will.
				}
				_listSchoolClasses=SchoolClasses.GetDeepCopy();
				for(int i=0;i<_listSchoolClasses.Count;i++) {
					comboSchoolClass.Items.Add(SchoolClasses.GetDescript(_listSchoolClasses[i]));
					comboSchoolClass.SelectedIndex=0;
					if(_listSchoolClasses[i].SchoolClassNum==ProvCur.SchoolClassNum) {
						comboSchoolClass.SelectedIndex=i;
					}
				}
				if(ProvCur.SchoolClassNum!=0) {
					labelSchoolClass.Visible=true;
					comboSchoolClass.Visible=true;
				}
			}
			else {
				tabControlProvider.TabPages.Remove(tabDentalSchools);
			}
			if(Programs.IsEnabled(ProgramName.eClinicalWorks)) {
				textEcwID.Text=ProvCur.EcwID;
			}
			else{
				labelEcwID.Visible=false;
				textEcwID.Visible=false;
			}
			List<EhrProvKey> listProvKey=EhrProvKeys.GetKeysByFLName(ProvCur.LName,ProvCur.FName);
			if(listProvKey.Count>0) {
				textLName.Enabled=false;
				textFName.Enabled=false;
			}
			else{
				textLName.Enabled=true;
				textFName.Enabled=true;
			}
			//We'll just always show the Anesthesia fields since they are part of the standard database.
			if(ProvCur.ProvNum!=0) {
				textProviderID.Text=ProvCur.ProvNum.ToString();
			}
			textAbbr.Text=ProvCur.Abbr;
			textLName.Text=ProvCur.LName;
			textFName.Text=ProvCur.FName;
			textMI.Text=ProvCur.MI;
			textSuffix.Text=ProvCur.Suffix;
			textPreferredName.Text=ProvCur.PreferredName;
			textSSN.Text=ProvCur.SSN;
			dateTerm.SetDateTime(ProvCur.DateTerm);
			if(ProvCur.UsingTIN){
				radioTIN.Checked=true;
			}
			else {
				radioSSN.Checked=true;
			}
			_listProvClinicsOld=ProviderClinics.GetListForProvider(ProvCur.ProvNum,
				Clinics.GetForUserod(Security.CurUser,true).Select(x => x.ClinicNum)
				.Union(new List<long>() { 0 })//Always include 0 clinic, this is the default, NOT a headquarters only value.
				.Distinct()
				.ToList());
			_listProvClinicsNew=_listProvClinicsOld.Select(x => x.Copy()).ToList();
			_provClinicDefault=_listProvClinicsNew.Find(x => x.ClinicNum==0);
			//Doesn't exist in Db, create a new one.
			if(_provClinicDefault==null) {
				_provClinicDefault=new ProviderClinic {
					ProvNum=ProvCur.ProvNum,
					ClinicNum=0,
					DEANum=ProvCur.DEANum,
					StateLicense=ProvCur.StateLicense,
					StateRxID=ProvCur.StateRxID,
					StateWhereLicensed=ProvCur.StateWhereLicensed,
				};
				_listProvClinicsNew.Add(_provClinicDefault);
			}
			textDEANum.Text=_provClinicDefault.DEANum;
			textStateLicense.Text=_provClinicDefault.StateLicense;
			textStateWhereLicensed.Text=_provClinicDefault.StateWhereLicensed;
			textStateRxID.Text=_provClinicDefault.StateRxID;
			//textBlueCrossID.Text=ProvCur.BlueCrossID;
			textMedicaidID.Text=ProvCur.MedicaidID;
			textNationalProvID.Text=ProvCur.NationalProvID;
			textCanadianOfficeNum.Text=ProvCur.CanadianOfficeNum;
			textCustomID.Text=ProvCur.CustomID;
			textSchedRules.Text=ProvCur.SchedNote;
			checkIsSecondary.Checked=ProvCur.IsSecondary;
			checkSigOnFile.Checked=ProvCur.SigOnFile;
			checkIsHidden.Checked=ProvCur.IsHidden;
			checkIsInstructor.Checked=ProvCur.IsInstructor;
			odColorPickerAppt.AllowTransparentColor=true;
			odColorPickerOutline.AllowTransparentColor=true;
			// if porvider color is not set (new prov), set color to transparent, same as pressing "none" button.
			// Prevents issue where appointment background color was not set causing appointment to be transparent instead of white
            if(ProvCur.ProvColor.Name=="0") {
				odColorPickerAppt.BackgroundColor=Color.Transparent;
			}
            else {
				odColorPickerAppt.BackgroundColor=ProvCur.ProvColor;
			}
			odColorPickerOutline.BackgroundColor=ProvCur.OutlineColor;
			checkIsHiddenOnReports.Checked=ProvCur.IsHiddenReport;
			checkUseErx.Checked=(ProvCur.IsErxEnabled!=ErxEnabledStatus.Disabled);
			ErxOption erxOption=Erx.GetErxOption();
			if(erxOption==ErxOption.DoseSpotWithLegacy) {
				checkAllowLegacy.Visible=true;
				checkAllowLegacy.Checked=(ProvCur.IsErxEnabled==ErxEnabledStatus.EnabledWithLegacy);
			}
			textBirthdate.Text="";
			textProdGoalHr.Text=ProvCur.HourlyProdGoalAmt.ToString("f");
			if(ProvCur.Birthdate.Year>=1880) {
				textBirthdate.Text=ProvCur.Birthdate.ToShortDateString();
			}
			listFeeSched.Items.AddList(FeeScheds.GetDeepCopy(true),x => x.Description);
			for(int i=0;i<listFeeSched.Items.Count;i++){
				if(((FeeSched)listFeeSched.Items.GetObjectAt(i)).FeeSchedNum==ProvCur.FeeSched){
					listFeeSched.SelectedIndex=i;
				}
			}
			if(listFeeSched.SelectedIndex==-1){
				listFeeSched.SelectedIndex=0;
			}
			listSpecialty.Items.Clear();
			Def[] specDefs=Defs.GetDefsForCategory(DefCat.ProviderSpecialties,true).ToArray();
			for(int i=0;i<specDefs.Length;i++) {
				listSpecialty.Items.Add(Lan.g("enumDentalSpecialty",specDefs[i].ItemName));
				if(i==0 || ProvCur.Specialty==specDefs[i].DefNum) {//default to the first item in the list
					listSpecialty.SelectedIndex=i;
				}
			}
			textTaxonomyOverride.Text=ProvCur.TaxonomyCodeOverride;
			FillGridProvIdent();
			//These radio buttons are used to properly filter the provider dropdowns on FormAnetheticRecord
			if (ProvCur.AnesthProvType == 0)
				{
					radNone.Checked = true;
				}
			
			if (ProvCur.AnesthProvType == 1)
				{
					radAnesthSurg.Checked = true;
				}

			if (ProvCur.AnesthProvType == 2)
			{
				radAsstCirc.Checked = true;
			}
			checkIsCDAnet.Checked=ProvCur.IsCDAnet;
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				checkIsCDAnet.Visible=true;
			}
			checkIsNotPerson.Checked=ProvCur.IsNotPerson;
			comboProv.Items.AddProvNone();
			comboProv.Items.AddProvsFull(Providers.GetDeepCopy(true));
			comboProv.SetSelectedProvNum(ProvCur.ProvNumBillingOverride);
			textWebSchedDescript.Text=ProvCur.WebSchedDescript;
			FillImage();
			if(ProvCur.ProvStatus==ProviderStatus.Deleted) {
				this.DisableAllExcept(new Control[]{butCancel});
				//Make the cancel button the only thing the user can click on.
			}
			if(PrefC.HasClinicsEnabled) {
				_listProvClinicLinks=ProviderClinicLinks.GetForProvider(ProvCur.ProvNum);
				_listClinicsForUser=Clinics.GetForUserod(Security.CurUser);
				//If there are no ProviderClinicLinks, then the provider is associated to all clinics.
				bool doSelectAll=(_listProvClinicLinks.Count==0 || _listClinicsForUser.All(x => _listProvClinicLinks.Any(y => y.ClinicNum==x.ClinicNum)));
				listBoxClinics.Items.AddList(_listClinicsForUser,x => x.Abbr);
				List<long> listClinicNumsForClinicLinks=_listProvClinicLinks.Select(x => x.ClinicNum).ToList();;
				for(int i=0; i<_listClinicsForUser.Count;i++) {
					if(!doSelectAll && listClinicNumsForClinicLinks.Contains(_listClinicsForUser[i].ClinicNum)) {
						listBoxClinics.SetSelected(i,true);
					}
				}
				checkAllClinics.Checked=doSelectAll;
			}
			else {
				butClinicOverrides.Visible=false;
				groupClinicOverrides.Text="";
				tabControlProvider.TabPages.Remove(tabClinics);
			}
		}

		private void FillImage() {
			if(string.IsNullOrEmpty(ProvCur.WebSchedImageLocation)) {
				return;
			}
			string fullImagePath=FileAtoZ.CombinePaths(ImageStore.GetProviderImagesFolder(),ProvCur.WebSchedImageLocation);
			try {
				pictureWebSched.Image=FileAtoZ.GetImage(fullImagePath);
			}
			catch(Exception ex) {
				Shown+=new EventHandler((o,e) => FriendlyException.Show(Lans.g(this,"Unable to display image."),ex));
			}
		}

		private void radioSSN_Click(object sender, System.EventArgs e) {
			ProvCur.UsingTIN=false;
		}

		private void radioTIN_Click(object sender, System.EventArgs e) {
			ProvCur.UsingTIN=true;
		}

		private void FillGridProvIdent() {
			ProviderIdents.RefreshCache();
			ListProvIdent=ProviderIdents.GetForProv(ProvCur.ProvNum);
			gridProvIdent.BeginUpdate();
			gridProvIdent.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableQueue","Payor ID"),90,HorizontalAlignment.Center);
			gridProvIdent.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableQueue","Type"),110,HorizontalAlignment.Center);
			gridProvIdent.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableQueue","ID Number"),100,HorizontalAlignment.Center);
			gridProvIdent.ListGridColumns.Add(col);
			gridProvIdent.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ListProvIdent.Length;i++)	{
				row=new GridRow();
				row.Cells.Add(ListProvIdent[i].PayorID);
				row.Cells.Add(ListProvIdent[i].SuppIDType.ToString());
				row.Cells.Add(ListProvIdent[i].IDNumber);
				gridProvIdent.ListGridRows.Add(row);
			}
			gridProvIdent.EndUpdate();
		}

		private void gridProvIdent_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormProviderIdentEdit FormP=new FormProviderIdentEdit();
			FormP.ProvIdentCur=ListProvIdent[e.Row];
			FormP.ShowDialog();
			FillGridProvIdent();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormProviderIdentEdit FormP=new FormProviderIdentEdit();
			FormP.ProvIdentCur=new ProviderIdent();
			FormP.ProvIdentCur.ProvNum=ProvCur.ProvNum;
			FormP.IsNew=true;
			FormP.ShowDialog();
			FillGridProvIdent();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			int selectedRow=gridProvIdent.GetSelectedIndex();
			if(selectedRow==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			if(MessageBox.Show(Lan.g(this,"Delete the selected Provider Identifier?"),"",
				MessageBoxButtons.OKCancel)!=DialogResult.OK)
			{
				return;
			}
			ProviderIdents.Delete(ListProvIdent[selectedRow]);
			FillGridProvIdent();
		}

		private void butPickPict_Click(object sender,EventArgs e) {
			using OpenFileDialog dlg=new OpenFileDialog();
			dlg.Multiselect=false;
			if(dlg.ShowDialog()!=DialogResult.OK) {
				return;
			}
			string localFileName=dlg.FileName;
			if(!File.Exists(localFileName)) {
				MsgBox.Show(this,"File does not exist.");
				return;
			}
			if(!ImageHelper.HasImageExtension(localFileName)) {
				MsgBox.Show(this,"Only allowed to import an image.");
				return;
			}
			string atoZFileName=FileAtoZ.CombinePaths(ImageStore.GetProviderImagesFolder(),Path.GetFileName(localFileName));
			if(FileAtoZ.Exists(atoZFileName)) {
				int attempts=1;
				string newAtoZFileName=FileAtoZ.AppendSuffix(atoZFileName,"_"+attempts);
				while(FileAtoZ.Exists(newAtoZFileName)) {
					if(attempts++ > 1000) {
						MsgBox.Show(this,"Unable to upload image.");
						return;
					}
					newAtoZFileName=FileAtoZ.AppendSuffix(atoZFileName,"_"+attempts);
				}
				atoZFileName=newAtoZFileName;
			}
			try {
				FileAtoZ.Upload(localFileName,atoZFileName);
			}
			catch(Exception ex) {
				FriendlyException.Show(Lans.g(this,"Unable to upload image."),ex);
				return;
			}
			ProvCur.WebSchedImageLocation=Path.GetFileName(atoZFileName);
			try {
				pictureWebSched.Image=Image.FromFile(localFileName);
			}
			catch(Exception ex) {
				pictureWebSched.Image=null;
				FriendlyException.Show(Lans.g(this,"Unable to display image."),ex);
			}
		}

		private void butPictureNone_Click(object sender,EventArgs e) {
			ProvCur.WebSchedImageLocation="";
			pictureWebSched.Image=null;
		}

		private void butClinicOverrides_Click(object sender,EventArgs e) {
			//Update current changes in the form before going to look at all values
			_provClinicDefault.DEANum=textDEANum.Text;
			_provClinicDefault.StateLicense=textStateLicense.Text;
			_provClinicDefault.StateRxID=textStateRxID.Text;
			_provClinicDefault.StateWhereLicensed=textStateWhereLicensed.Text;
			using FormProvAdditional FormPA=new FormProvAdditional(_listProvClinicsNew,ProvCur);
			FormPA.ShowDialog();
			if(FormPA.DialogResult==DialogResult.OK) {
				_listProvClinicsNew=FormPA.ListProviderClinicOut;
				_provClinicDefault=_listProvClinicsNew.Find(x => x.ClinicNum==0);
				textDEANum.Text=_provClinicDefault.DEANum;
				textStateLicense.Text=_provClinicDefault.StateLicense;
				textStateRxID.Text=_provClinicDefault.StateRxID;
				textStateWhereLicensed.Text=_provClinicDefault.StateWhereLicensed;
			}
		}

		private void listBoxClinics_SelectedIndexChanged(object sender,EventArgs e) {
			if(checkAllClinics.Checked) {
				checkAllClinics.CheckedChanged-=checkAllClinics_CheckedChanged;
				checkAllClinics.Checked=false;
				checkAllClinics.CheckedChanged+=checkAllClinics_CheckedChanged;
			}
		}

		private void checkAllClinics_CheckedChanged(object sender,EventArgs e) {
			if(checkAllClinics.Checked) {
				listBoxClinics.SelectedIndexChanged-=listBoxClinics_SelectedIndexChanged;
				listBoxClinics.SelectedIndices.Clear();
				listBoxClinics.SelectedIndexChanged+=listBoxClinics_SelectedIndexChanged;
			}
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(!dateTerm.IsValid()) {
				MsgBox.Show(this,"Term Date invalid.");
				return;
			}
			if(textAbbr.Text=="") {
				MessageBox.Show(Lan.g(this,"Abbreviation not allowed to be blank."));
				return;
			}
			if(textSSN.Text.Contains("-")) {
				MsgBox.Show(this,"SSN/TIN not allowed to have dash.");
				return;
			}
			if(checkIsHidden.Checked) {
				if(PrefC.GetLong(PrefName.PracticeDefaultProv)==ProvCur.ProvNum) {
					MsgBox.Show(this,"Not allowed to hide practice default provider.");
					return;
				}
				if(Clinics.IsDefaultClinicProvider(ProvCur.ProvNum)) {
					MsgBox.Show(this,"Not allowed to hide a clinic default provider.");
					return;
				}
				if(PrefC.GetLong(PrefName.InsBillingProv)==ProvCur.ProvNum) {
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"You are about to hide the default ins billing provider. Continue?")) {
						return;
					}
				}
				if(Clinics.IsInsBillingProvider(ProvCur.ProvNum)) {
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"You are about to hide a clinic ins billing provider. Continue?")) {
						return;
					}
				}
				List<ApptViewItem> listApptViewItems=ApptViewItems.GetForProvider(ProvCur.ProvNum);
				if(!listApptViewItems.IsNullOrEmpty()) {	
					#region Provider Attached to View Check
					//Create a list of Provider associated Appointment Views
					List<ApptView> listApptView=listApptViewItems.Select(x => ApptViews.GetApptView(x.ApptViewNum)).Where(x => x!=null).ToList();
					//This list must be distincted before being shown to the user. A single Provider can be associated to the same "view" multiple times.
					if(!listApptView.IsNullOrEmpty()) {
						string listProviderAssociatedViews=string.Join("\r\n",listApptView.Select(x => x.Description).Distinct().OrderBy(x => x));
						string msg=Lans.g(this,"Not allowed to hide a Provider associated to an Appointment View.");
						msg+=("\r\n"+Lans.g(this,"To continue remove them from the following Appointment View(s):"));
						msg+=("\r\n"+listProviderAssociatedViews);
						MsgBox.Show(msg);
						return;
					}
					#endregion
				}
			}
			if(Providers.GetExists(x => x.ProvNum!=ProvCur.ProvNum && x.Abbr==textAbbr.Text && PrefC.GetBool(PrefName.EasyHideDentalSchools))) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This abbreviation is already in use by another provider.  Continue anyway?")) {
					return;
				}
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && checkIsCDAnet.Checked) {
				if(textNationalProvID.Text!=OpenDentBusiness.Eclaims.Canadian.TidyAN(textNationalProvID.Text,9,true)) {
					MsgBox.Show(this,"CDA number must be 9 characters long and composed of numbers and letters only.");
					return;
				}
				if(textCanadianOfficeNum.Text!=OpenDentBusiness.Eclaims.Canadian.TidyAN(textCanadianOfficeNum.Text,4,true)) {
					MsgBox.Show(this,"Office number must be 4 characters long and composed of numbers and letters only.");
					return;
				}
			}
			if(checkIsNotPerson.Checked) {
				if(textFName.Text!="" || textMI.Text!="") {
					MsgBox.Show(this,"When the 'Not a Person' box is checked, the provider may not have a First Name or Middle Initial entered.");
					return;
				}
			}
			if(checkIsHidden.Checked && ProvCur.IsHidden==false) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"If there are any future hours on this provider's schedule, they will be removed.  "
					+"This does not affect scheduled appointments or any other appointments in any way.")) 
				{
					return;
				}
				Providers.RemoveProvFromFutureSchedule(ProvCur.ProvNum);
			}
			if(!PrefC.GetBool(PrefName.EasyHideDentalSchools) && (ProvCur.IsInstructor || ProvCur.SchoolClassNum!=0)) {//Is an Instructor or a Student
				if(textUserName.Text=="") {
					MsgBox.Show(this,"User Name is not allowed to be blank.");
					return;
				}
			}
			long claimBillingOverrideProvNum=comboProv.GetSelectedProvNum();
			if(claimBillingOverrideProvNum!=0) {
				Provider provClaimBillingOverride=comboProv.GetSelected<Provider>();
				if(provClaimBillingOverride==null) {//Hidden provider, need to get them the normal way
					provClaimBillingOverride=Providers.GetProv(claimBillingOverrideProvNum);//Can return null if the provider doesn't exist
				}
				if(provClaimBillingOverride!=null && !provClaimBillingOverride.IsNotPerson) {//Override is a person.
					MsgBox.Show(this,"E-claim Billing Prov Override cannot be a person.");
					return;
				}
			}
			if(ProvCur.IsNew == false && comboProv.GetSelectedProvNum()==ProvCur.ProvNum) {//Override is the same provider.
				MsgBox.Show(this,"E-claim Billing Prov Override cannot be the same provider.");
				return;
			}
			if(ProvCur.IsErxEnabled==ErxEnabledStatus.Disabled && checkUseErx.Checked) {//The user enabled eRx for this provider when it was previously disabled.
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"By clicking Yes, you acknowledge and approve Electronic Rx (eRx) fees for this "
					+"provider. See the website for more details. ERx only works for the United States and its territories. Do you want to continue?"))
				{
					return;
				}
			}
			if(textBirthdate.Text!="" && !textBirthdate.IsValid()) {
				MsgBox.Show(this,"Birthdate invalid.");
				return;
			}
			if(!textProdGoalHr.IsValid()) {
				MsgBox.Show(this,"Hourly production goal invalid.");
				return;
			}
			ProvCur.Abbr=textAbbr.Text;
			ProvCur.LName=textLName.Text;
			ProvCur.FName=textFName.Text;
			ProvCur.MI=textMI.Text;
			ProvCur.Suffix=textSuffix.Text;
			ProvCur.PreferredName=textPreferredName.Text;
			ProvCur.SSN=textSSN.Text;
			ProvCur.StateLicense=textStateLicense.Text;
			_provClinicDefault.StateLicense=textStateLicense.Text;
			ProvCur.StateWhereLicensed=textStateWhereLicensed.Text;
			_provClinicDefault.StateWhereLicensed=textStateWhereLicensed.Text;
			ProvCur.DEANum=textDEANum.Text;
			_provClinicDefault.DEANum=textDEANum.Text;
			ProvCur.StateRxID=textStateRxID.Text;
			_provClinicDefault.StateRxID=textStateRxID.Text;
			//ProvCur.BlueCrossID=textBlueCrossID.Text;
			ProvCur.MedicaidID=textMedicaidID.Text;
			ProvCur.NationalProvID=textNationalProvID.Text;
			ProvCur.CanadianOfficeNum=textCanadianOfficeNum.Text;
			//EhrKey and EhrHasReportAccess set when user uses the ... button
			ProvCur.IsSecondary=checkIsSecondary.Checked;
			ProvCur.SigOnFile=checkSigOnFile.Checked;
			ProvCur.IsHidden=checkIsHidden.Checked;
			ProvCur.IsCDAnet=checkIsCDAnet.Checked;
			ProvCur.ProvColor=odColorPickerAppt.BackgroundColor;
			ProvCur.OutlineColor=odColorPickerOutline.BackgroundColor;
			ProvCur.IsInstructor=checkIsInstructor.Checked;
			ProvCur.EhrMuStage=comboEhrMu.SelectedIndex;
			ProvCur.IsHiddenReport=checkIsHiddenOnReports.Checked;
			ProvCur.IsErxEnabled=checkUseErx.Checked?ErxEnabledStatus.Enabled:ErxEnabledStatus.Disabled;
			ErxOption erxOption=Erx.GetErxOption();
			//If the ErxOption is Legacy, we want to keep the EnabledStatus as EnabledWithLegacy.
			//If the office switches eRx Options we will know to prompt those providers which eRx solution to use until the office disables legacy.
			if(erxOption!=ErxOption.Legacy && checkAllowLegacy.Visible && checkAllowLegacy.Checked) {
				ProvCur.IsErxEnabled=ErxEnabledStatus.EnabledWithLegacy;
			}
			ProvCur.CustomID=textCustomID.Text;
			ProvCur.SchedNote=textSchedRules.Text;
			ProvCur.Birthdate=PIn.Date(textBirthdate.Text);
			ProvCur.WebSchedDescript=textWebSchedDescript.Text;
			ProvCur.HourlyProdGoalAmt=PIn.Double(textProdGoalHr.Text);
			ProvCur.DateTerm=dateTerm.GetDateTime();
			if(!PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
				if(ProvCur.SchoolClassNum!=0) {
					ProvCur.SchoolClassNum=_listSchoolClasses[comboSchoolClass.SelectedIndex].SchoolClassNum;
				}
			}
			if(listFeeSched.SelectedIndex!=-1) {
				ProvCur.FeeSched=listFeeSched.GetSelected<FeeSched>().FeeSchedNum;
			}
			//default to first specialty in the list if it can't find the specialty by exact name
			ProvCur.Specialty=Defs.GetByExactNameNeverZero(DefCat.ProviderSpecialties,listSpecialty.SelectedItem.ToString());//selected index defaults to 0
			ProvCur.TaxonomyCodeOverride=textTaxonomyOverride.Text;
			if(radAnesthSurg.Checked) {
				ProvCur.AnesthProvType=1;
			}
			else if(radAsstCirc.Checked) {
				ProvCur.AnesthProvType=2;
			}
			else {
				ProvCur.AnesthProvType=0;
			}
			ProvCur.IsNotPerson=checkIsNotPerson.Checked;
			ProvCur.ProvNumBillingOverride=comboProv.GetSelectedProvNum();
			if(IsNew) {
				long provNum=Providers.Insert(ProvCur);
				//Set the providerclinics to the new provider's ProvNum that was just retreived from the database.
				_listProvClinicsNew.ForEach(x => x.ProvNum=provNum);
				if(ProvCur.IsInstructor) {
					Userod user=new Userod();
					user.UserName=textUserName.Text;
					user.LoginDetails=Authentication.GenerateLoginDetailsSHA512(textPassword.Text);
					user.ProvNum=provNum;
					try {
						Userods.Insert(user,new List<long> { PrefC.GetLong(PrefName.SecurityGroupForInstructors) });
					}
					catch(Exception ex) {
						Providers.Delete(ProvCur);
						MessageBox.Show(ex.Message);
						return;
					}
				}
			}
			else {
				try {
					if(_existingUser!=null && (ProvCur.IsInstructor || ProvCur.SchoolClassNum!=0)) {
						_existingUser.UserName=textUserName.Text;
						_existingUser.LoginDetails=Authentication.GenerateLoginDetailsSHA512(textPassword.Text);
						Userods.Update(_existingUser);
					}
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					return;
				}
				Providers.Update(ProvCur);
				#region Date Term Check
				if(ProvCur.DateTerm.Year > 1880 && ProvCur.DateTerm < DateTime.Now) {
					List<ClaimPaySplit> listClaimPaySplits=Claims.GetOutstandingClaimsByProvider(ProvCur.ProvNum,ProvCur.DateTerm);
					StringBuilder claimMessage=new StringBuilder(Lan.g(this,"Clinic\tPatNum\tPatient Name\tDate of Service\tClaim Status\tFee\tCarrier")+"\r\n");
					foreach(ClaimPaySplit claimPaySplit in listClaimPaySplits) {
						claimMessage.Append(claimPaySplit.ClinicDesc+"\t"
							+POut.Long(claimPaySplit.PatNum)+"\t"
							+claimPaySplit.PatName+"\t"
							+claimPaySplit.DateClaim.ToShortDateString()+"\t");
						switch(claimPaySplit.ClaimStatus) {
							case "W":
								claimMessage.Append("Waiting in Queue\t");
								break;
							case "H":
								claimMessage.Append("Hold\t");
								break;
							case "U":
								claimMessage.Append("Unsent\t");
								break;
							case "S":
								claimMessage.Append("Sent\t");
								break;
							default:
								break;
						}
						claimMessage.AppendLine(claimPaySplit.FeeBilled+"\t"+claimPaySplit.Carrier);
					}
					using MsgBoxCopyPaste msg=new MsgBoxCopyPaste(claimMessage.ToString());
					msg.Text=Lan.g(this,"Outstanding Claims for the Provider Whose Term Has Expired");
					if(listClaimPaySplits.Count > 0) {
						msg.ShowDialog();
					}
				}
				#endregion Date Term Check
			}
			ProviderClinics.Sync(_listProvClinicsNew,_listProvClinicsOld);
			if(PrefC.HasClinicsEnabled) {
				List<long> listSelectedClinicNumLinks=listBoxClinics.GetListSelected<Clinic>().Select(x => x.ClinicNum).ToList();
				List<Clinic> listClinicsAll=Clinics.GetDeepCopy(true);
				List<long> listClinicNumsForUser=_listClinicsForUser.Select(x => x.ClinicNum).ToList();
				bool canUserAccessAllClinics=(_listClinicsForUser.Count==listClinicsAll.Count);
				if(checkAllClinics.Checked) {
					if(canUserAccessAllClinics) {
						listSelectedClinicNumLinks.Clear();//No clinic links means the provider is associated to all clinics
					}
					else {
						listSelectedClinicNumLinks=_listClinicsForUser.Select(x => x.ClinicNum).ToList();
					}
				}
				else {//'All' is not checked
					if(listSelectedClinicNumLinks.Count==0 
						&& !_listProvClinicLinks.Any(x => x.ClinicNum > -1 && !ListTools.In(x.ClinicNum,listClinicNumsForUser))) 
					{
						//The user wants to assign this provider to no clinics.
						listSelectedClinicNumLinks.Add(-1);//Since no clinic links means the provider is associated to all clinics, we're gonna use -1.
					}
					else if(!canUserAccessAllClinics && _listProvClinicLinks.Count==0) {
						//The provider previously was associated to all clinics. We need to add in the clinics this user does not have access to.
						listSelectedClinicNumLinks.AddRange(listClinicsAll.Where(x => !ListTools.In(x.ClinicNum,listClinicNumsForUser)).Select(x => x.ClinicNum));
					}
				}
				List<ProviderClinicLink> listProvClinicLinksNew=_listProvClinicLinks.Select(x => x.Copy()).ToList();
				listProvClinicLinksNew.RemoveAll(x => ListTools.In(x.ClinicNum,listClinicNumsForUser) || x.ClinicNum==-1);
				listProvClinicLinksNew.AddRange(listSelectedClinicNumLinks.Select(x => new ProviderClinicLink(x,ProvCur.ProvNum)));
				if(ProviderClinicLinks.Sync(listProvClinicLinksNew,_listProvClinicLinks)) {
					DataValid.SetInvalid(InvalidType.ProviderClinicLink);
				}
			}
			DialogResult = DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormProvEdit_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(DialogResult==DialogResult.OK) {
				//There can be a "hasChanged" boolean added in the future.  For now, due to FormProviderSetup, we need to refresh the cache just in case.
				DataValid.SetInvalid(InvalidType.Providers);
				return;
			}
			if(IsNew){
				//UserPermissions.DeleteAllForProv(Providers.Cur.ProvNum);
				ProviderIdents.DeleteAllForProv(ProvCur.ProvNum);
				Providers.Delete(ProvCur);
			}
		}
	}
}




