using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.WebTypes.WebForms;

namespace OpenDental {
	/// <summary>
	/// This Form is primarily used by the dental office to upload sheetDefs
	/// </summary>
	public partial class FormWebFormSetup:FormODBase {
		
		private string _sheetDefAddress="";
		private string _redirectURL="";
		private List<WebForms_SheetDef> _listWebForms_SheetDefs;
		private long _webSheetDefIDSelected=0;
		private long _dentalOfficeID=0;
		private long _registrationKeyID=0;
		private List<long> _listNextFormIdsSelected=new List<long>();
		private bool _isWebSchedSetup=false;
		private long _clinicNumDefault=0;
		private WebForms_Preference _webForms_PreferenceOld=new WebForms_Preference();
		private WebForms_Preference _webForms_Preference=new WebForms_Preference();
		public string SheetURLs="";
		private bool _isAutoFillNameBirthday=true;
		private bool _isDisableTypeSignature=false;

		public FormWebFormSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		public FormWebFormSetup(long clinicNum,string urlWebForm,bool isWebSchedSetup=true):this() {
			if(!isWebSchedSetup) {
				return;
			}
			_isWebSchedSetup=isWebSchedSetup;
			_clinicNumDefault=clinicNum;
			gridMain.SelectionMode=GridSelectionMode.OneRow;//Allow only one form to be selected
			if(!string.IsNullOrEmpty(urlWebForm)) {				
				string decodeURL=HttpUtility.UrlDecode(urlWebForm);
				Uri uRIUnparsed=new Uri(decodeURL);
				NameValueCollection nameValueCollection=HttpUtility.ParseQueryString(uRIUnparsed.Query);
				if(!string.IsNullOrEmpty(nameValueCollection["WSDID"])) {
					_webSheetDefIDSelected=PIn.Long(nameValueCollection.Get("WSDID"),false);
				}
				if(!string.IsNullOrEmpty(nameValueCollection["NFID"])) {
					_listNextFormIdsSelected=nameValueCollection.Get("NFID").Split(',').Select(x => PIn.Long(x,false)).ToList();
				}
				if(!string.IsNullOrEmpty(nameValueCollection["ReturnURL"])) {
					_redirectURL=nameValueCollection.Get("ReturnURL");
				}
				if(!string.IsNullOrEmpty(nameValueCollection["AFNAB"])) {
					if(nameValueCollection.Get("AFNAB")=="N") {
						_isAutoFillNameBirthday=false;
					}
				}
				if(!string.IsNullOrEmpty(nameValueCollection["DTS"])) {
					if(nameValueCollection.Get("DTS")=="Y") {
						_isDisableTypeSignature=true;
					}
				}
			}
			for(int i = 0;i<Controls.Count;i++) {
				if(Controls[i]==gridMain || Controls[i]==groupConstructURL || Controls[i]==butSave || Controls[i]==checkEnableAutoDownload) {
					continue;
				}
				Controls[i].Visible=false;
			}
			comboClinic.Enabled=false;
		}

	private void FormWebFormSetup_Load(object sender,EventArgs e) {
			if(PrefC.HasClinicsEnabled) {
				if(_isWebSchedSetup){
					comboClinic.ClinicNumSelected=_clinicNumDefault;
				}
				else{
					comboClinic.ClinicNumSelected=Clinics.ClinicNum;
				}
			}
			butSave.Enabled=false;
			checkAutoFillNameAndBirthdate.Checked=PrefC.GetBool(PrefName.WebFormsAutoFillNameAndBirthdate);
			checkEnableAutoDownload.Checked=PrefC.GetBool(PrefName.WebFormsDownloadAutomcatically);
		}

		private void FormWebFormSetup_Shown(object sender,EventArgs e) {
			FetchValuesFromWebServer();
			if(_listNextFormIdsSelected.Count > 0) {//If entering form with Next Form Id values, fill the textNextForms box with the corresponding descriptions.
				textNextForms.Text=string.Join(", ",_listWebForms_SheetDefs.Where(x => _listNextFormIdsSelected
					.Contains(x.WebSheetDefID)).Select(y => y.Description));
			}
			if(_webSheetDefIDSelected!=0) {//If entering the form with WSDID, have corresponding grid item selected and URL updated.
				for(int i=0;i<gridMain.ListGridRows.Count;i++) {
					if(((WebForms_SheetDef)gridMain.ListGridRows[i].Tag).WebSheetDefID==_webSheetDefIDSelected) {
						gridMain.SetSelected(i,true);
					}
				}
			}
			if(!string.IsNullOrEmpty(_redirectURL)) {
				textRedirectURL.Text=_redirectURL;
			}
			checkAutoFillNameAndBirthdate.Checked=_isAutoFillNameBirthday;
			checkDisableTypedSig.Checked=_isDisableTypeSignature;
			ConstructURLs();
		}

		private void FetchValuesFromWebServer() {
			string webHostSynchServerURL=PrefC.GetString(PrefName.WebHostSynchServerURL);
			textboxWebHostAddress.Text=webHostSynchServerURL;
			butSave.Enabled=false;
			if((webHostSynchServerURL==WebFormL.SynchUrlStaging) || (webHostSynchServerURL==WebFormL.SynchUrlDev)) {
				WebFormL.IgnoreCertificateErrors();
			}
			Cursor=Cursors.WaitCursor;
			_dentalOfficeID=WebUtils.GetDentalOfficeID();
			_registrationKeyID=WebUtils.GetRegKeyID();
			if(_registrationKeyID==0 && _dentalOfficeID==0) {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Either the registration key provided by the dental office is incorrect or the Host Server Address cannot be found.");
				return;
			}
			if(WebForms_Preferences.TryGetPreference(out _webForms_Preference)) {
				butWebformBorderColor.BackColor=_webForms_Preference.ColorBorder;
				_sheetDefAddress=WebUtils.GetSheetDefAddress();
				checkDisableWebFormSignatures.Checked=_webForms_Preference.DisableSignatures;
				if(string.IsNullOrEmpty(_webForms_Preference.CultureName)){//Just in case.
					_webForms_Preference.CultureName=System.Globalization.CultureInfo.CurrentCulture.Name;
					WebForms_Preferences.SetPreferences(_webForms_Preference);
				}
				_webForms_PreferenceOld=_webForms_Preference.Copy();
			}
			FillGrid();//Also gets sheet def list from server
			Cursor=Cursors.Default;
		}

		///<summary>This now also gets a new list of sheet defs from the server.  But it's only called after testing that the web service exists.</summary>
		private void FillGrid() {
			if(!WebForms_SheetDefs.TryDownloadSheetDefs(out _listWebForms_SheetDefs)) {
				MsgBox.Show(this,"Failed to download sheet definitions.");
				_listWebForms_SheetDefs=new List<WebForms_SheetDef>();
			}
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Description"),200);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			for(int i=0;i<_listWebForms_SheetDefs.Count;i++) {
				GridRow row=new GridRow();
				row.Tag=_listWebForms_SheetDefs[i];
				row.Cells.Add(_listWebForms_SheetDefs[i].Description);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private string SheetDefBaseURL(WebForms_SheetDef webForms_SheetDef) {
			if(_registrationKeyID==0) {
				return _sheetDefAddress+"?DOID="+_dentalOfficeID+"&WSDID="+webForms_SheetDef.WebSheetDefID;//The "old way" 
			}
			return _sheetDefAddress+"?DOID="+_dentalOfficeID+"&RKID="+_registrationKeyID+"&WSDID="+webForms_SheetDef.WebSheetDefID;
		}

		private void gridMain_MouseUp(object sender,MouseEventArgs e) {
			ConstructURLs();
		}

		private void OpenBrowser() {
			if(gridMain.SelectedIndices.Length<1) {
				return;
			}
			string[] stringArrayLines=textURLs.Text.Split(
				new[]{ "\r\n","\r","\n" },
				StringSplitOptions.None
			);
			for(int i=0;i<stringArrayLines.Length;i++){
				ODException.SwallowAnyException(() => { System.Diagnostics.Process.Start(stringArrayLines[i]); });
			}
		}

		/// <summary>Returns true if any of the preferences/settings that get saved on a Save/OK click have changed from what they were when form was 
		/// opened or last saved. Otherwise, false</summary>
		private void ResetSaveButton() {
			if(_webForms_PreferenceOld.ColorBorder.ToArgb()!=_webForms_Preference.ColorBorder.ToArgb()
				|| _webForms_PreferenceOld.CultureName!=_webForms_Preference.CultureName
				|| _webForms_PreferenceOld.DisableSignatures!=_webForms_Preference.DisableSignatures
				|| textboxWebHostAddress.Text.Trim()!=PrefC.GetString(PrefName.WebHostSynchServerURL) 
				|| checkAutoFillNameAndBirthdate.Checked!=PrefC.GetBool(PrefName.WebFormsAutoFillNameAndBirthdate)
				|| checkEnableAutoDownload.Checked!=PrefC.GetBool(PrefName.WebFormsDownloadAutomcatically))
			{
				butSave.Enabled=true;
				return;
			}
			butSave.Enabled=false;
		}

		private void textboxWebHostAddress_TextChanged(object sender,EventArgs e) {
			ResetSaveButton();
		}

		private void butSavePrefs_Click(object sender,EventArgs e) {
			//disabled unless user changed url
			if(SavePrefs()) {
				butSave.Enabled=false;
			}
		}

		private bool SavePrefs(bool includeAutoFillBirthdatePref=false) {
			Cursor=Cursors.WaitCursor;
			if(!WebForms_Preferences.SetPreferences(_webForms_Preference,urlOverride:textboxWebHostAddress.Text.Trim())) {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Either the registration key provided by the dental office is incorrect or the Host Server Address cannot be found.");
				return false;
			}
			_webForms_PreferenceOld=_webForms_Preference.Copy();
			int freqDownloadAlert=3600000;//Default, which is every 1 hour
			if(checkEnableAutoDownload.Checked) {
				freqDownloadAlert=120000;//Every 2 minutes
			}
			if(Prefs.UpdateString(PrefName.WebHostSynchServerURL,textboxWebHostAddress.Text.Trim())
				| (includeAutoFillBirthdatePref && Prefs.UpdateBool(PrefName.WebFormsAutoFillNameAndBirthdate,checkAutoFillNameAndBirthdate.Checked))
				| Prefs.UpdateBool(PrefName.WebFormsDownloadAutomcatically,checkEnableAutoDownload.Checked)
				| Prefs.UpdateInt(PrefName.WebFormsDownloadAlertFrequency,freqDownloadAlert))
			{
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			Cursor=Cursors.Default;
			return true;
		}

		private void butWebformBorderColor_Click(object sender,EventArgs e) {
			ShowColorDialog();
		}

		private void butChange_Click(object sender,EventArgs e) {
			ShowColorDialog();
		}

		private void ShowColorDialog(){
			colorDialog1.Color=butWebformBorderColor.BackColor;
			if(colorDialog1.ShowDialog()!=DialogResult.OK) {
				colorDialog1.Dispose();
				return;
			}
			butWebformBorderColor.BackColor=colorDialog1.Color;
			_webForms_Preference.ColorBorder=butWebformBorderColor.BackColor;
			ResetSaveButton();
			colorDialog1.Dispose();
		}

		private void textRedirectURL_TextChanged(object sender,EventArgs e) {
			ConstructURLs();
		}

		private void butNextForms_Click(object sender,EventArgs e) {
			if(_listWebForms_SheetDefs==null) {
				MsgBox.Show(this,"No selected Available Web Forms");
				return;
			}
			List<string> listSheetDefDescriptionsSelected=textNextForms.Text.Split(", ",StringSplitOptions.RemoveEmptyEntries).ToList();
			List<WebForms_SheetDef> listWebForms_SheetDefsSelected=new List<WebForms_SheetDef>();
			for(int i=0;i < listSheetDefDescriptionsSelected.Count;i++) {//Loop through each selected description to preserve order.
				listWebForms_SheetDefsSelected.Add(_listWebForms_SheetDefs.Find(x => x.Description==listSheetDefDescriptionsSelected[i]));
			}
			using FormWebFormNextForms formWebFormNextForms=new FormWebFormNextForms(_listWebForms_SheetDefs,listWebForms_SheetDefsSelected);
			if(formWebFormNextForms.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_listNextFormIdsSelected=formWebFormNextForms.ListWebForms_SheetDefsSelected.Select(x => x.WebSheetDefID).ToList();
			textNextForms.Text=string.Join(", ",formWebFormNextForms.ListWebForms_SheetDefsSelected.Select(x => x.Description));
			ConstructURLs();
		}

		private void ComboClinic_SelectionChangeCommitted(object sender, EventArgs e){
			ConstructURLs();
		}

		private void checkAutoFillNameAndBirthdate_CheckedChanged(object sender,EventArgs e) {
			ConstructURLs();
		}

		private void checkDisableTypedSig_CheckChanged(object sender,EventArgs e) {
			ConstructURLs();
		}

		///<summary>Build a URL based on selected forms. Should be run after gridMain is filled.</summary>
		private void ConstructURLs() {
			textURLs.Clear();
			List<string> listURLs=new List<string>();
			for(int i=0;i<gridMain.SelectedTags<WebForms_SheetDef>().Count();i++) {
				string url=SheetDefBaseURL(gridMain.SelectedTags<WebForms_SheetDef>()[i]);
				if(_listNextFormIdsSelected.Count>0) {
					url+="&NFID="+string.Join("&NFID=",_listNextFormIdsSelected.FindAll(x => x!=gridMain.SelectedTags<WebForms_SheetDef>()[i].WebSheetDefID));//Will remove ids that match the id of the sheet def we are currently constructing a url for.
				}
				if(comboClinic.ClinicNumSelected > 0) {//'None' is not selected
					url+="&CID="+comboClinic.ClinicNumSelected.ToString(); 
				}
				if(!checkAutoFillNameAndBirthdate.Checked) {
					url+="&AFNAB=N";
				}
				if(checkDisableTypedSig.Enabled && checkDisableTypedSig.Checked) {
					url+="&DTS=Y";
				}
				if(textRedirectURL.Text != "") {
					url+="&ReturnURL="+HttpUtility.UrlEncode(textRedirectURL.Text);
				}
				listURLs.Add(url);
			}
			textURLs.AppendText(string.Join("\r\n",listURLs));
			ResetSaveButton();
		}
		
		private void checkDisableWebFormSignatures_CheckedChanged(object sender,EventArgs e) {
			//Do not allow user to disable typed signatures if they have already disabled all signatures.
			_webForms_Preference.DisableSignatures=checkDisableWebFormSignatures.Checked;
			checkDisableTypedSig.Enabled=!checkDisableWebFormSignatures.Checked;
			ResetSaveButton();
		}

		private void checkDisableAutoDownload_CheckedChanged(object sender,EventArgs e) {
			ResetSaveButton();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			FrmSheetPicker frmSheetPicker=new FrmSheetPicker(isWebForm:true);
			frmSheetPicker.AllowMultiSelect=true;
			frmSheetPicker.SheetType=SheetTypeEnum.PatientForm;
			frmSheetPicker.HideKioskButton=true;
			frmSheetPicker.ShowDialog();
			if(!frmSheetPicker.IsDialogOK) {
				return;
			}
			//Make sure each selected sheet contains FName, LName, and Birthdate.
			List<SheetDef> listSheetDefs=frmSheetPicker.ListSheetDefsSelected;
			for(int i=0;i<listSheetDefs.Count;i++) {
				if(!WebFormL.VerifyRequiredFieldsPresent(listSheetDefs[i])) {
					return;
				}
			}
			Cursor=Cursors.WaitCursor;
			for(int i=0;i<listSheetDefs.Count;i++) {
				WebFormL.LoadImagesToSheetDef(listSheetDefs[i]);
				WebFormL.TryAddOrUpdateSheetDef(this,listSheetDefs[i],isNew:true);
			}
			FillGrid();
			Cursor=Cursors.Default;
		}

		private void butUpdate_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length < 1) {
				MsgBox.Show(this,"Please select an item from the grid first.");
				return;
			}
			if(gridMain.SelectedIndices.Length > 1) {
				MsgBox.Show(this,"Please select one web form at a time.");
				return;
			}
			WebForms_SheetDef webForms_sheetDef=gridMain.SelectedTag<WebForms_SheetDef>();
			SheetDef sheetDef=SheetDefs.GetFirstOrDefault(x => x.SheetDefNum==webForms_sheetDef.SheetDefNum);
			if(sheetDef==null) {//This web form has never had a SheetDefNum assigned or the sheet has been deleted.
				MsgBox.Show(this,"This Web Form is not linked to a valid Sheet.  Please select the correct Sheet that this Web Form should be linked to.");
				FrmSheetPicker frmSheetPicker=new FrmSheetPicker(isWebForm:true);
				frmSheetPicker.SheetType=SheetTypeEnum.PatientForm;
				frmSheetPicker.HideKioskButton=true;
				frmSheetPicker.ShowDialog();
				if(!frmSheetPicker.IsDialogOK || frmSheetPicker.ListSheetDefsSelected.Count==0) {
					return;
				}
				sheetDef=frmSheetPicker.ListSheetDefsSelected.FirstOrDefault();
			}
			else {//sheetDef not null
				SheetDefs.GetFieldsAndParameters(sheetDef);
			}
			if(!WebFormL.VerifyRequiredFieldsPresent(sheetDef)) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			WebFormL.LoadImagesToSheetDef(sheetDef);
			WebFormL.TryAddOrUpdateSheetDef(this,sheetDef,isNew:false,new List<WebForms_SheetDef> { webForms_sheetDef });
			FillGrid();
			Cursor=Cursors.Default;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select an item from the grid first.");
				return;
			}
			int failures=0;
			List<WebForms_SheetDef> listWebForms_SheetDefs=gridMain.SelectedTags<WebForms_SheetDef>().ToList();//Sheets to be deleted
			List<WebForms_SheetDef> listWebForms_SheetDefsNextForms=listWebForms_SheetDefs.FindAll(x => _listNextFormIdsSelected.Contains(x.WebSheetDefID));//Sheets currently in the 'Next Forms' field.
			if(listWebForms_SheetDefsNextForms.Count>0) {//If a sheet that is to be deleted is currently in the 'Next Forms' field.
				string sheetDescriptions=string.Join(",\n",listWebForms_SheetDefsNextForms.Select(x => x.Description));
				//Prompt user if they want to continue with delete. If no, simply return.
				if(MessageBox.Show(this,Lan.g(this,"The following sheet(s) will also be deleted from the 'Next Forms' field:")+"\n"+sheetDescriptions+"\n"+Lan.g(this,"Do you want to continue?")
					,Lan.g(this,"Warning"),MessageBoxButtons.YesNo)==DialogResult.No)
				{ 
					return;
				}
			}
			Cursor=Cursors.WaitCursor;
			for(int i=0;i<listWebForms_SheetDefs.Count;i++) {
				if(!WebForms_SheetDefs.DeleteSheetDef(listWebForms_SheetDefs[i].WebSheetDefID)) {
					failures++;
				}
			}
			if(failures>0) {
				Cursor=Cursors.Default;
				MessageBox.Show(this,Lan.g(this,"Error deleting")+" "+POut.Int(failures)+" "+Lan.g(this,"web form(s). Either the web service is not available or "
					+"the Host Server Address cannot be found."));
			}
			FillGrid();
			//Remove all the NextForm ids that belong to SheetDefs that were deleted.
			_listNextFormIdsSelected=_listNextFormIdsSelected.FindAll(x => !listWebForms_SheetDefs.Any(y => y.WebSheetDefID==x));
			//Recreate the textNextForms control's text.
			List<string> listNextFormsDescriptions=new List<string>();
			for(int i=0;i<_listNextFormIdsSelected.Count;i++) {
				WebForms_SheetDef webForms_SheetDef=_listWebForms_SheetDefs.Find(x => x.WebSheetDefID==_listNextFormIdsSelected[i]);
				if(webForms_SheetDef!=null) {
					listNextFormsDescriptions.Add(webForms_SheetDef.Description);
				}
			}
			textNextForms.Text=string.Join(", ",listNextFormsDescriptions);
			ConstructURLs();
			Cursor=Cursors.Default;
		}

		private void butNavigateTo_Click(object sender,EventArgs e) {
			OpenBrowser();
		}

		private void butCopyToClipboard_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length<1) {
				return;
			}
			WebForms_SheetDef webSheetDef=gridMain.SelectedTag<WebForms_SheetDef>();
			try {
				ODClipboard.SetClipboard(textURLs.Text);
			}
			catch(Exception ex) {
				MsgBox.Show(this,"Could not copy contents to the clipboard.  Please try again.");
				ex.DoNothing();
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(!string.IsNullOrWhiteSpace(textRedirectURL.Text) && !Uri.IsWellFormedUriString(textRedirectURL.Text,UriKind.Absolute)) {
				string error=label3.Text+Lan.g(this," should use the following format: http://www.patientviewer.com or https://www.patientviewer.com");
				MsgBox.Show(this,error);
				return;
			}
			SheetURLs=textURLs.Text;
			SavePrefs(true);//No check for success, since we are closing just fail silently.
			DialogResult=DialogResult.OK;
		}

	}
}