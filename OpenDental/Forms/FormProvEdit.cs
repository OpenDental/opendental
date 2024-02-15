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
		private ProviderIdent[] ProviderIdentArray;
		private List<SchoolClass> _listSchoolClasses;
		private Userod _userodExisting;
		public Provider ProviderCur;
		private List<ProviderClinic> _listProviderClinicsOld;
		private List<ProviderClinic> _listProviderClinicsNew;
		private ProviderClinic _providerClinicDefault;
		///<summary>The clinics this provider is linked to. May include clinics the user does not have access to.</summary>
		private List<ProviderClinicLink> _listProviderClinicLinks=new List<ProviderClinicLink>();
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
			comboEhrMu.SelectedIndex=ProviderCur.EhrMuStage;
			if(!PrefC.GetBool(PrefName.ShowFeatureEhr)) {
				comboEhrMu.Visible=false;
				labelEhrMU.Visible=false;
			}
			if(!PrefC.GetBool(PrefName.EasyHideDentalSchools) //Dental Schools is turned on
				&& (ProviderCur.SchoolClassNum!=0 || ProviderCur.IsInstructor))//Adding/Editing Students or Instructors
			{
				if(!ProviderCur.IsNew) {
					labelPassDescription.Visible=true;
					textProvNum.Text=ProviderCur.ProvNum.ToString();
					List<Userod> listUserods=Providers.GetAttachedUsers(ProviderCur.ProvNum);
					if(listUserods.Count>0) {
						textUserName.Text=listUserods[0].UserName;//Should always happen if they are a student.
						_userodExisting=listUserods[0];
					}
				}
				else {
					textUserName.Text=Providers.GetNextAvailableProvNum().ToString();//User-names are suggested to be the ProvNum of the provider.  This can be changed at will.
				}
				_listSchoolClasses=SchoolClasses.GetDeepCopy();
				for(int i=0;i<_listSchoolClasses.Count;i++) {
					comboSchoolClass.Items.Add(SchoolClasses.GetDescript(_listSchoolClasses[i]));
					comboSchoolClass.SelectedIndex=0;
					if(_listSchoolClasses[i].SchoolClassNum==ProviderCur.SchoolClassNum) {
						comboSchoolClass.SelectedIndex=i;
					}
				}
				if(ProviderCur.SchoolClassNum!=0) {
					labelSchoolClass.Visible=true;
					comboSchoolClass.Visible=true;
				}
			}
			else {
				tabControlProvider.TabPages.Remove(tabDentalSchools);
			}
			if(Programs.IsEnabled(ProgramName.eClinicalWorks)) {
				textEcwID.Text=ProviderCur.EcwID;
			}
			else{
				labelEcwID.Visible=false;
				textEcwID.Visible=false;
			}
			List<EhrProvKey> listEhrProvKeys=EhrProvKeys.GetKeysByFLName(ProviderCur.LName,ProviderCur.FName);
			if(listEhrProvKeys.Count>0) {
				textLName.Enabled=false;
				textFName.Enabled=false;
			}
			else{
				textLName.Enabled=true;
				textFName.Enabled=true;
			}
			//We'll just always show the Anesthesia fields since they are part of the standard database.
			if(ProviderCur.ProvNum!=0) {
				textProviderID.Text=ProviderCur.ProvNum.ToString();
			}
			textAbbr.Text=ProviderCur.Abbr;
			textLName.Text=ProviderCur.LName;
			textFName.Text=ProviderCur.FName;
			textMI.Text=ProviderCur.MI;
			textSuffix.Text=ProviderCur.Suffix;
			textPreferredName.Text=ProviderCur.PreferredName;
			textSSN.Text=ProviderCur.SSN;
			dateTerm.SetDateTime(ProviderCur.DateTerm);
			if(ProviderCur.UsingTIN){
				radioTIN.Checked=true;
			}
			else {
				radioSSN.Checked=true;
			}
			_listProviderClinicsOld=ProviderClinics.GetListForProvider(ProviderCur.ProvNum,
				Clinics.GetForUserod(Security.CurUser,true).Select(x => x.ClinicNum)
				.Union(new List<long>() { 0 })//Always include 0 clinic, this is the default, NOT a headquarters only value.
				.Distinct()
				.ToList());
			_listProviderClinicsNew=_listProviderClinicsOld.Select(x => x.Copy()).ToList();
			_providerClinicDefault=_listProviderClinicsNew.Find(x => x.ClinicNum==0);
			//Doesn't exist in Db, create a new one.
			if(_providerClinicDefault==null) {
				_providerClinicDefault=new ProviderClinic {
					ProvNum=ProviderCur.ProvNum,
					ClinicNum=0,
					DEANum=ProviderCur.DEANum,
					StateLicense=ProviderCur.StateLicense,
					StateRxID=ProviderCur.StateRxID,
					StateWhereLicensed=ProviderCur.StateWhereLicensed,
				};
				_listProviderClinicsNew.Add(_providerClinicDefault);
			}
			textDEANum.Text=_providerClinicDefault.DEANum;
			textStateLicense.Text=_providerClinicDefault.StateLicense;
			textStateWhereLicensed.Text=_providerClinicDefault.StateWhereLicensed;
			textStateRxID.Text=_providerClinicDefault.StateRxID;
			//textBlueCrossID.Text=ProvCur.BlueCrossID;
			textMedicaidID.Text=ProviderCur.MedicaidID;
			textNationalProvID.Text=ProviderCur.NationalProvID;
			textCanadianOfficeNum.Text=ProviderCur.CanadianOfficeNum;
			textCustomID.Text=ProviderCur.CustomID;
			textSchedRules.Text=ProviderCur.SchedNote;
			checkIsSecondary.Checked=ProviderCur.IsSecondary;
			checkSigOnFile.Checked=ProviderCur.SigOnFile;
			checkIsHidden.Checked=ProviderCur.IsHidden;
			checkIsInstructor.Checked=ProviderCur.IsInstructor;
			odColorPickerAppt.AllowTransparentColor=true;
			odColorPickerOutline.AllowTransparentColor=true;
			// if porvider color is not set (new prov), set color to transparent, same as pressing "none" button.
			// Prevents issue where appointment background color was not set causing appointment to be transparent instead of white
			if(ProviderCur.ProvColor.Name=="0") {
				odColorPickerAppt.BackgroundColor=Color.Transparent;
			}
			else {
				odColorPickerAppt.BackgroundColor=ProviderCur.ProvColor;
			}
			odColorPickerOutline.BackgroundColor=ProviderCur.OutlineColor;
			checkIsHiddenOnReports.Checked=ProviderCur.IsHiddenReport;
			checkUseErx.Checked=(ProviderCur.IsErxEnabled!=ErxEnabledStatus.Disabled);
			ErxOption erxOption=Erx.GetErxOption();
			if(erxOption==ErxOption.DoseSpotWithNewCrop) {
				checkAllowLegacy.Visible=true;
				checkAllowLegacy.Checked=(ProviderCur.IsErxEnabled==ErxEnabledStatus.EnabledWithLegacy);
			}
			textBirthdate.Text="";
			textProdGoalHr.Text=ProviderCur.HourlyProdGoalAmt.ToString("f");
			if(ProviderCur.Birthdate.Year>=1880) {
				textBirthdate.Text=ProviderCur.Birthdate.ToShortDateString();
			}
			listFeeSched.Items.AddList(FeeScheds.GetDeepCopy(true),x => x.Description);
			for(int i=0;i<listFeeSched.Items.Count;i++){
				if(((FeeSched)listFeeSched.Items.GetObjectAt(i)).FeeSchedNum==ProviderCur.FeeSched){
					listFeeSched.SelectedIndex=i;
				}
			}
			if(listFeeSched.SelectedIndex==-1){
				listFeeSched.SelectedIndex=0;
			}
			listSpecialty.Items.Clear();
			Def[] defArray=Defs.GetDefsForCategory(DefCat.ProviderSpecialties,true).ToArray();
			for(int i=0;i<defArray.Length;i++) {
				listSpecialty.Items.Add(Lan.g("enumDentalSpecialty",defArray[i].ItemName));
				if(i==0 || ProviderCur.Specialty==defArray[i].DefNum) {//default to the first item in the list
					listSpecialty.SelectedIndex=i;
				}
			}
			textTaxonomyOverride.Text=ProviderCur.TaxonomyCodeOverride;
			FillGridProvIdent();
			//These radio buttons are used to properly filter the provider dropdowns on FormAnetheticRecord
			if (ProviderCur.AnesthProvType == 0)
				{
					radNone.Checked = true;
				}
			
			if (ProviderCur.AnesthProvType == 1)
				{
					radAnesthSurg.Checked = true;
				}

			if (ProviderCur.AnesthProvType == 2)
			{
				radAsstCirc.Checked = true;
			}
			checkIsCDAnet.Checked=ProviderCur.IsCDAnet;
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				checkIsCDAnet.Visible=true;
			}
			checkIsNotPerson.Checked=ProviderCur.IsNotPerson;
			comboProv.Items.AddProvNone();
			comboProv.Items.AddProvsFull(Providers.GetDeepCopy(true));
			comboProv.SetSelectedProvNum(ProviderCur.ProvNumBillingOverride);
			textWebSchedDescript.Text=ProviderCur.WebSchedDescript;
			FillImage();
			if(ProviderCur.ProvStatus==ProviderStatus.Deleted) {
				this.DisableAllExcept();
				//Make the cancel button the only thing the user can click on.
			}
			if(PrefC.HasClinicsEnabled) {
				_listProviderClinicLinks=ProviderClinicLinks.GetForProvider(ProviderCur.ProvNum);
				_listClinicsForUser=Clinics.GetForUserod(Security.CurUser);
				//If there are no ProviderClinicLinks, then the provider is associated to all clinics.
				bool doSelectAll=(_listProviderClinicLinks.Count==0 || _listClinicsForUser.All(x => _listProviderClinicLinks.Any(y => y.ClinicNum==x.ClinicNum)));
				listBoxClinics.Items.AddList(_listClinicsForUser,x => x.Abbr);
				List<long> listClinicNumsForClinicLinks=_listProviderClinicLinks.Select(x => x.ClinicNum).ToList();;
				for(int i=0; i<_listClinicsForUser.Count;i++) {
					if(!doSelectAll && listClinicNumsForClinicLinks.Contains(_listClinicsForUser[i].ClinicNum)) {
						listBoxClinics.SetSelected(i,true);
					}
				}
				checkAllClinics.Checked=doSelectAll;
				return;
			}
			butClinicOverrides.Visible=false;
			groupClinicOverrides.Text="";
			tabControlProvider.TabPages.Remove(tabClinics);
		}

		private void FillImage() {
			if(string.IsNullOrEmpty(ProviderCur.WebSchedImageLocation)) {
				return;
			}
			string fullImagePath=FileAtoZ.CombinePaths(ImageStore.GetProviderImagesFolder(),ProviderCur.WebSchedImageLocation);
			try {
				pictureWebSched.Image=FileAtoZ.GetImage(fullImagePath);
			}
			catch(Exception ex) {
				Shown+=new EventHandler((o,e) => FriendlyException.Show(Lans.g(this,"Unable to display image."),ex));
			}
		}

		private void radioSSN_Click(object sender, System.EventArgs e) {
			ProviderCur.UsingTIN=false;
		}

		private void radioTIN_Click(object sender, System.EventArgs e) {
			ProviderCur.UsingTIN=true;
		}

		private void FillGridProvIdent() {
			ProviderIdents.RefreshCache();
			ProviderIdentArray=ProviderIdents.GetForProv(ProviderCur.ProvNum);
			gridProvIdent.BeginUpdate();
			gridProvIdent.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableQueue","Payor ID"),90,HorizontalAlignment.Center);
			gridProvIdent.Columns.Add(col);
			col=new GridColumn(Lan.g("TableQueue","Type"),110,HorizontalAlignment.Center);
			gridProvIdent.Columns.Add(col);
			col=new GridColumn(Lan.g("TableQueue","ID Number"),100,HorizontalAlignment.Center);
			gridProvIdent.Columns.Add(col);
			gridProvIdent.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ProviderIdentArray.Length;i++)	{
				row=new GridRow();
				row.Cells.Add(ProviderIdentArray[i].PayorID);
				row.Cells.Add(ProviderIdentArray[i].SuppIDType.ToString());
				row.Cells.Add(ProviderIdentArray[i].IDNumber);
				gridProvIdent.ListGridRows.Add(row);
			}
			gridProvIdent.EndUpdate();
		}

		private void gridProvIdent_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			FrmProviderIdentEdit frmProviderIdentEdit=new FrmProviderIdentEdit();
			frmProviderIdentEdit.ProviderIdentCur=ProviderIdentArray[e.Row];
			frmProviderIdentEdit.ShowDialog();
			FillGridProvIdent();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			FrmProviderIdentEdit frmProviderIdentEdit=new FrmProviderIdentEdit();
			frmProviderIdentEdit.ProviderIdentCur=new ProviderIdent();
			frmProviderIdentEdit.ProviderIdentCur.ProvNum=ProviderCur.ProvNum;
			frmProviderIdentEdit.IsNew=true;
			frmProviderIdentEdit.ShowDialog();
			FillGridProvIdent();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			int rowSelected=gridProvIdent.GetSelectedIndex();
			if(rowSelected==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			if(MessageBox.Show(Lan.g(this,"Delete the selected Provider Identifier?"),"",
				MessageBoxButtons.OKCancel)!=DialogResult.OK)
			{
				return;
			}
			ProviderIdents.Delete(ProviderIdentArray[rowSelected]);
			FillGridProvIdent();
		}

		private void butPickPict_Click(object sender,EventArgs e) {
			using OpenFileDialog openFileDialog=new OpenFileDialog();
			openFileDialog.Multiselect=false;
			if(openFileDialog.ShowDialog()!=DialogResult.OK) {
				return;
			}
			string localFileName=openFileDialog.FileName;
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
			ProviderCur.WebSchedImageLocation=Path.GetFileName(atoZFileName);
			try {
				pictureWebSched.Image=Image.FromFile(localFileName);
			}
			catch(Exception ex) {
				pictureWebSched.Image=null;
				FriendlyException.Show(Lans.g(this,"Unable to display image."),ex);
			}
		}

		private void butPictureNone_Click(object sender,EventArgs e) {
			ProviderCur.WebSchedImageLocation="";
			pictureWebSched.Image=null;
		}

		private void butClinicOverrides_Click(object sender,EventArgs e) {
			//Update current changes in the form before going to look at all values
			_providerClinicDefault.DEANum=textDEANum.Text;
			_providerClinicDefault.StateLicense=textStateLicense.Text;
			_providerClinicDefault.StateRxID=textStateRxID.Text;
			_providerClinicDefault.StateWhereLicensed=textStateWhereLicensed.Text;
			using FormProvAdditional FormProvAdditional=new FormProvAdditional(_listProviderClinicsNew,ProviderCur);
			FormProvAdditional.ShowDialog();
			if(FormProvAdditional.DialogResult==DialogResult.OK) {
				_listProviderClinicsNew=FormProvAdditional.ListProviderClinicsOut;
				_providerClinicDefault=_listProviderClinicsNew.Find(x => x.ClinicNum==0);
				textDEANum.Text=_providerClinicDefault.DEANum;
				textStateLicense.Text=_providerClinicDefault.StateLicense;
				textStateRxID.Text=_providerClinicDefault.StateRxID;
				textStateWhereLicensed.Text=_providerClinicDefault.StateWhereLicensed;
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

		private void butSave_Click(object sender,System.EventArgs e) {
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
				if(PrefC.GetLong(PrefName.PracticeDefaultProv)==ProviderCur.ProvNum) {
					MsgBox.Show(this,"Not allowed to hide practice default provider.");
					return;
				}
				if(Clinics.IsDefaultClinicProvider(ProviderCur.ProvNum)) {
					MsgBox.Show(this,"Not allowed to hide a clinic default provider.");
					return;
				}
				if(PrefC.GetLong(PrefName.InsBillingProv)==ProviderCur.ProvNum) {
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"You are about to hide the default ins billing provider. Continue?")) {
						return;
					}
				}
				if(Clinics.IsInsBillingProvider(ProviderCur.ProvNum)) {
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"You are about to hide a clinic ins billing provider. Continue?")) {
						return;
					}
				}
				List<ApptViewItem> listApptViewItems=ApptViewItems.GetForProvider(ProviderCur.ProvNum);
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
			if(Providers.GetExists(x => x.ProvNum!=ProviderCur.ProvNum && x.Abbr==textAbbr.Text && PrefC.GetBool(PrefName.EasyHideDentalSchools))) {
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
			if(checkIsHidden.Checked && ProviderCur.IsHidden==false) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"If there are any future hours on this provider's schedule, they will be removed.  "
					+"This does not affect scheduled appointments or any other appointments in any way.")) 
				{
					return;
				}
				Providers.RemoveProvFromFutureSchedule(ProviderCur.ProvNum);
			}
			if(!PrefC.GetBool(PrefName.EasyHideDentalSchools) && (ProviderCur.IsInstructor || ProviderCur.SchoolClassNum!=0)) {//Is an Instructor or a Student
				if(textUserName.Text=="") {
					MsgBox.Show(this,"User Name is not allowed to be blank.");
					return;
				}
				if(ProviderCur.IsInstructor && textUserName.Text!=textUserName.Text.TrimEnd()) {
					MsgBox.Show(this,"User Name cannot end with white space.");
					return;
				}
			}
			long provNumClaimBillingOverride=comboProv.GetSelectedProvNum();
			if(provNumClaimBillingOverride!=0) {
				Provider providerClaimBillingOverride=comboProv.GetSelected<Provider>();
				if(providerClaimBillingOverride==null) {//Hidden provider, need to get them the normal way
					providerClaimBillingOverride=Providers.GetProv(provNumClaimBillingOverride);//Can return null if the provider doesn't exist
				}
				if(providerClaimBillingOverride!=null && !providerClaimBillingOverride.IsNotPerson) {//Override is a person.
					MsgBox.Show(this,"E-claim Billing Prov Override cannot be a person.");
					return;
				}
			}
			if(ProviderCur.IsNew == false && comboProv.GetSelectedProvNum()==ProviderCur.ProvNum) {//Override is the same provider.
				MsgBox.Show(this,"E-claim Billing Prov Override cannot be the same provider.");
				return;
			}
			if(ProviderCur.IsErxEnabled==ErxEnabledStatus.Disabled && checkUseErx.Checked) {//The user enabled eRx for this provider when it was previously disabled.
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
			ProviderCur.Abbr=textAbbr.Text;
			ProviderCur.LName=textLName.Text;
			ProviderCur.FName=textFName.Text;
			ProviderCur.MI=textMI.Text;
			ProviderCur.Suffix=textSuffix.Text;
			ProviderCur.PreferredName=textPreferredName.Text;
			ProviderCur.SSN=textSSN.Text;
			ProviderCur.StateLicense=textStateLicense.Text;
			_providerClinicDefault.StateLicense=textStateLicense.Text;
			ProviderCur.StateWhereLicensed=textStateWhereLicensed.Text;
			_providerClinicDefault.StateWhereLicensed=textStateWhereLicensed.Text;
			ProviderCur.DEANum=textDEANum.Text;
			_providerClinicDefault.DEANum=textDEANum.Text;
			ProviderCur.StateRxID=textStateRxID.Text;
			_providerClinicDefault.StateRxID=textStateRxID.Text;
			//ProvCur.BlueCrossID=textBlueCrossID.Text;
			ProviderCur.MedicaidID=textMedicaidID.Text;
			ProviderCur.NationalProvID=textNationalProvID.Text;
			ProviderCur.CanadianOfficeNum=textCanadianOfficeNum.Text;
			//EhrKey and EhrHasReportAccess set when user uses the ... button
			ProviderCur.IsSecondary=checkIsSecondary.Checked;
			ProviderCur.SigOnFile=checkSigOnFile.Checked;
			ProviderCur.IsHidden=checkIsHidden.Checked;
			ProviderCur.IsCDAnet=checkIsCDAnet.Checked;
			ProviderCur.ProvColor=odColorPickerAppt.BackgroundColor;
			ProviderCur.OutlineColor=odColorPickerOutline.BackgroundColor;
			ProviderCur.IsInstructor=checkIsInstructor.Checked;
			ProviderCur.EhrMuStage=comboEhrMu.SelectedIndex;
			ProviderCur.IsHiddenReport=checkIsHiddenOnReports.Checked;
			ProviderCur.IsErxEnabled=checkUseErx.Checked?ErxEnabledStatus.Enabled:ErxEnabledStatus.Disabled;
			ErxOption erxOption=Erx.GetErxOption();
			//If the ErxOption is Legacy, we want to keep the EnabledStatus as EnabledWithLegacy.
			//If the office switches eRx Options we will know to prompt those providers which eRx solution to use until the office disables NewCrop.
			if(erxOption!=ErxOption.NewCrop && checkAllowLegacy.Visible && checkAllowLegacy.Checked) {
				ProviderCur.IsErxEnabled=ErxEnabledStatus.EnabledWithLegacy;
			}
			ProviderCur.CustomID=textCustomID.Text;
			ProviderCur.SchedNote=textSchedRules.Text;
			ProviderCur.Birthdate=PIn.Date(textBirthdate.Text);
			ProviderCur.WebSchedDescript=textWebSchedDescript.Text;
			ProviderCur.HourlyProdGoalAmt=PIn.Double(textProdGoalHr.Text);
			ProviderCur.DateTerm=dateTerm.GetDateTime();
			if(!PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
				if(ProviderCur.SchoolClassNum!=0) {
					ProviderCur.SchoolClassNum=_listSchoolClasses[comboSchoolClass.SelectedIndex].SchoolClassNum;
				}
			}
			if(listFeeSched.SelectedIndex!=-1) {
				ProviderCur.FeeSched=listFeeSched.GetSelected<FeeSched>().FeeSchedNum;
			}
			//default to first specialty in the list if it can't find the specialty by exact name
			ProviderCur.Specialty=Defs.GetByExactNameNeverZero(DefCat.ProviderSpecialties,listSpecialty.SelectedItem.ToString());//selected index defaults to 0
			ProviderCur.TaxonomyCodeOverride=textTaxonomyOverride.Text;
			if(radAnesthSurg.Checked) {
				ProviderCur.AnesthProvType=1;
			}
			else if(radAsstCirc.Checked) {
				ProviderCur.AnesthProvType=2;
			}
			else {
				ProviderCur.AnesthProvType=0;
			}
			ProviderCur.IsNotPerson=checkIsNotPerson.Checked;
			ProviderCur.ProvNumBillingOverride=comboProv.GetSelectedProvNum();
			if(IsNew) {
				long provNum=Providers.Insert(ProviderCur);
				//Set the providerclinics to the new provider's ProvNum that was just retreived from the database.
				_listProviderClinicsNew.ForEach(x => x.ProvNum=provNum);
				if(ProviderCur.IsInstructor) {
					Userod userod=new Userod();
					userod.UserName=textUserName.Text;
					userod.LoginDetails=Authentication.GenerateLoginDetailsSHA512(textPassword.Text);
					userod.ProvNum=provNum;
					try {
						Userods.Insert(userod,new List<long> { PrefC.GetLong(PrefName.SecurityGroupForInstructors) });
					}
					catch(Exception ex) {
						Providers.Delete(ProviderCur);
						MessageBox.Show(ex.Message);
						return;
					}
				}
			}
			else {
				try {
					if(_userodExisting!=null && (ProviderCur.IsInstructor || ProviderCur.SchoolClassNum!=0)) {
						_userodExisting.UserName=textUserName.Text;
						_userodExisting.LoginDetails=Authentication.GenerateLoginDetailsSHA512(textPassword.Text);
						Userods.Update(_userodExisting);
					}
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					return;
				}
				Providers.Update(ProviderCur);
				#region Date Term Check
				if(ProviderCur.DateTerm.Year > 1880 && ProviderCur.DateTerm < DateTime.Now) {
					List<ClaimPaySplit> listClaimPaySplits=Claims.GetOutstandingClaimsByProvider(ProviderCur.ProvNum,ProviderCur.DateTerm);
					StringBuilder stringBuilderClaimMessage=new StringBuilder(Lan.g(this,"Clinic\tPatNum\tPatient Name\tDate of Service\tClaim Status\tFee\tCarrier")+"\r\n");
					for(int i=0; i<listClaimPaySplits.Count;i++) {
						stringBuilderClaimMessage.Append(listClaimPaySplits[i].ClinicDesc+"\t"
							+POut.Long(listClaimPaySplits[i].PatNum)+"\t"
							+listClaimPaySplits[i].PatName+"\t"
							+listClaimPaySplits[i].DateClaim.ToShortDateString()+"\t");
						switch(listClaimPaySplits[i].ClaimStatus) {
							case "W":
								stringBuilderClaimMessage.Append("Waiting in Queue\t");
								break;
							case "H":
								stringBuilderClaimMessage.Append("Hold\t");
								break;
							case "U":
								stringBuilderClaimMessage.Append("Unsent\t");
								break;
							case "S":
								stringBuilderClaimMessage.Append("Sent\t");
								break;
							default:
								break;
						}
						stringBuilderClaimMessage.AppendLine(listClaimPaySplits[i].FeeBilled+"\t"+listClaimPaySplits[i].Carrier);
					}
					using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(stringBuilderClaimMessage.ToString());
					msgBoxCopyPaste.Text=Lan.g(this,"Outstanding Claims for the Provider Whose Term Has Expired");
					if(listClaimPaySplits.Count > 0) {
						msgBoxCopyPaste.ShowDialog();
					}
				}
				#endregion Date Term Check
			}
			ProviderClinics.Sync(_listProviderClinicsNew,_listProviderClinicsOld);
			if(PrefC.HasClinicsEnabled) {
				List<long> listClinicNumsSelectedLinks=listBoxClinics.GetListSelected<Clinic>().Select(x => x.ClinicNum).ToList();
				List<Clinic> listClinicsAll=Clinics.GetDeepCopy(true);
				List<long> listClinicNumsForUser=_listClinicsForUser.Select(x => x.ClinicNum).ToList();
				bool canUserAccessAllClinics=(_listClinicsForUser.Count==listClinicsAll.Count);
				if(checkAllClinics.Checked) {
					if(canUserAccessAllClinics) {
						listClinicNumsSelectedLinks.Clear();//No clinic links means the provider is associated to all clinics
					}
					else {
						listClinicNumsSelectedLinks=_listClinicsForUser.Select(x => x.ClinicNum).ToList();
					}
				}
				else {//'All' is not checked
					if(listClinicNumsSelectedLinks.Count==0 
						&& !_listProviderClinicLinks.Any(x => x.ClinicNum > -1 && !listClinicNumsForUser.Contains(x.ClinicNum))) 
					{
						//The user wants to assign this provider to no clinics.
						listClinicNumsSelectedLinks.Add(-1);//Since no clinic links means the provider is associated to all clinics, we're gonna use -1.
					}
					else if(!canUserAccessAllClinics && _listProviderClinicLinks.Count==0) {
						//The provider previously was associated to all clinics. We need to add in the clinics this user does not have access to.
						listClinicNumsSelectedLinks.AddRange(listClinicsAll.Where(x => !listClinicNumsForUser.Contains(x.ClinicNum)).Select(x => x.ClinicNum));
					}
				}
				List<ProviderClinicLink> listProviderClinicLinksNew=_listProviderClinicLinks.Select(x => x.Copy()).ToList();
				listProviderClinicLinksNew.RemoveAll(x => listClinicNumsForUser.Contains(x.ClinicNum) || x.ClinicNum==-1);
				listProviderClinicLinksNew.AddRange(listClinicNumsSelectedLinks.Select(x => new ProviderClinicLink(x,ProviderCur.ProvNum)));
				if(ProviderClinicLinks.Sync(listProviderClinicLinksNew,_listProviderClinicLinks)) {
					DataValid.SetInvalid(InvalidType.ProviderClinicLink);
				}
			}
			DialogResult = DialogResult.OK;
		}

		private void FormProvEdit_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(DialogResult==DialogResult.OK) {
				//There can be a "hasChanged" boolean added in the future.  For now, due to FormProviderSetup, we need to refresh the cache just in case.
				DataValid.SetInvalid(InvalidType.Providers);
				return;
			}
			if(IsNew){
				//UserPermissions.DeleteAllForProv(Providers.Cur.ProvNum);
				ProviderIdents.DeleteAllForProv(ProviderCur.ProvNum);
				Providers.Delete(ProviderCur);
			}
		}

	}
}