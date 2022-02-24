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
		private List<WebForms_SheetDef> _listWebFormSheetDefs;
		private long _selectedWebFormSheetID=0;
		private long _dentalOfficeID=0;
		private List<long> _listSelectedNextFormIds=new List<long>();
		private bool _isWebSchedSetup=false;
		private long _defaultClinicNum=0;
		private WebForms_Preference _webFormPrefOld=new WebForms_Preference();
		private WebForms_Preference _webFormPref=new WebForms_Preference();
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
			_defaultClinicNum=clinicNum;
			gridMain.SelectionMode=GridSelectionMode.One;//Allow only one form to be selected
			if(!string.IsNullOrEmpty(urlWebForm)) {				
				string decodeURL=HttpUtility.UrlDecode(urlWebForm);
				Uri unparsedURL=new Uri(decodeURL);
				NameValueCollection query=HttpUtility.ParseQueryString(unparsedURL.Query);
				if(!string.IsNullOrEmpty(query["WSDID"])) {
					_selectedWebFormSheetID=PIn.Long(query.Get("WSDID"),false);
				}
				if(!string.IsNullOrEmpty(query["NFID"])) {
					_listSelectedNextFormIds=query.Get("NFID").Split(',').Select(x => PIn.Long(x,false)).ToList();
				}
				if(!string.IsNullOrEmpty(query["ReturnURL"])) {
					_redirectURL=query.Get("ReturnURL");
				}
				if(!string.IsNullOrEmpty(query["AFNAB"])) {
					if(query.Get("AFNAB")=="N") {
						_isAutoFillNameBirthday=false;
					}
				}
				if(!string.IsNullOrEmpty(query["DTS"])) {
					if(query.Get("DTS")=="Y") {
						_isDisableTypeSignature=true;
					}
				}
			}
			foreach(Control ctr in this.Controls) {
				if(ctr==gridMain || ctr==groupConstructURL || ctr==butOk || ctr==butCancel) {
					continue;
				}
				ctr.Visible=false;
			}
			comboClinic.Enabled=false;
		}

		private void FormWebFormSetup_Load(object sender,EventArgs e) {
			if(PrefC.HasClinicsEnabled) {
				if(_isWebSchedSetup){
					comboClinic.SelectedClinicNum=_defaultClinicNum;
				}
				else{
					comboClinic.SelectedClinicNum=Clinics.ClinicNum;
				}
			}
			butSave.Enabled=false;
			checkAutoFillNameAndBirthdate.Checked=PrefC.GetBool(PrefName.WebFormsAutoFillNameAndBirthdate);
		}

		private void FormWebFormSetup_Shown(object sender,EventArgs e) {
			FetchValuesFromWebServer();
			if(_listSelectedNextFormIds.Count > 0) {//If entering form with Next Form Id values, fill the textNextForms box with the corresponding descriptions.
				textNextForms.Text=string.Join(", ",_listWebFormSheetDefs.Where(x => _listSelectedNextFormIds
					.Contains(x.WebSheetDefID)).Select(y => y.Description));
			}
			if(_selectedWebFormSheetID!=0) {//If entering the form with WSDID, have corresponding grid item selected and URL updated.
				for(int i=0;i<gridMain.ListGridRows.Count;i++) {
					if(((WebForms_SheetDef)gridMain.ListGridRows[i].Tag).WebSheetDefID==_selectedWebFormSheetID) {
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
			try {
				String WebHostSynchServerURL=PrefC.GetString(PrefName.WebHostSynchServerURL);
				textboxWebHostAddress.Text=WebHostSynchServerURL;
				butSave.Enabled=false;
				if((WebHostSynchServerURL==WebFormL.SynchUrlStaging) || (WebHostSynchServerURL==WebFormL.SynchUrlDev)) {
					WebFormL.IgnoreCertificateErrors();
				}
				Cursor=Cursors.WaitCursor;
				_dentalOfficeID=WebUtils.GetDentalOfficeID();
				if(_dentalOfficeID==0) {
					Cursor=Cursors.Default;
					MsgBox.Show(this,"Either the registration key provided by the dental office is incorrect or the Host Server Address cannot be found.");
					return;
				}
				if(WebForms_Preferences.TryGetPreference(out _webFormPref)) {
					butWebformBorderColor.BackColor=_webFormPref.ColorBorder;
					_sheetDefAddress=WebUtils.GetSheetDefAddress();
					checkDisableWebFormSignatures.Checked=_webFormPref.DisableSignatures;
					if(string.IsNullOrEmpty(_webFormPref.CultureName)){//Just in case.
						_webFormPref.CultureName=System.Globalization.CultureInfo.CurrentCulture.Name;
						WebForms_Preferences.SetPreferences(_webFormPref);
					}
					_webFormPrefOld=_webFormPref.Copy();
				}
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(ex.Message);
			}
			FillGrid();//Also gets sheet def list from server
			Cursor=Cursors.Default;
		}

		///<summary>This now also gets a new list of sheet defs from the server.  But it's only called after testing that the web service exists.</summary>
		private void FillGrid() {
			try{
				if(!WebForms_SheetDefs.TryDownloadSheetDefs(out _listWebFormSheetDefs)) {
					MsgBox.Show(this,"Failed to download sheet definitions.");
					_listWebFormSheetDefs=new List<WebForms_SheetDef>();
				}
				gridMain.ListGridColumns.Clear();
				GridColumn col=new GridColumn(Lan.g(this,"Description"),200);
				gridMain.ListGridColumns.Add(col);
				gridMain.ListGridRows.Clear();
				foreach(WebForms_SheetDef sheetDef in _listWebFormSheetDefs) {
					GridRow row=new GridRow();
					row.Tag=sheetDef;
					row.Cells.Add(sheetDef.Description);
					gridMain.ListGridRows.Add(row);
				}
				gridMain.EndUpdate();
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(ex.Message);
			}
		}

		private string SheetDefBaseURL(WebForms_SheetDef sheetDef) {
			return _sheetDefAddress+"?DOID="+_dentalOfficeID+"&WSDID="+sheetDef.WebSheetDefID;
		}

		private void gridMain_MouseUp(object sender,MouseEventArgs e) {
			ConstructURLs();
		}

		private void OpenBrowser() {
			if(gridMain.SelectedIndices.Length<1) {
				return;
			}
			string[] lines=textURLs.Text.Split(
				new[]{ "\r\n","\r","\n" },
				StringSplitOptions.None
			);
			foreach(string URL in lines) {
				ODException.SwallowAnyException(() => { System.Diagnostics.Process.Start(URL); });
			}
		}

		/// <summary>Returns true if any of the preferences/settings that get saved on a Save/OK click have changed from what they were when form was 
		/// opened or last saved. Otherwise, false</summary>
		private void ResetSaveButton() {
			if(_webFormPrefOld.ColorBorder.ToArgb()!=_webFormPref.ColorBorder.ToArgb()
				|| _webFormPrefOld.CultureName!=_webFormPref.CultureName
				|| _webFormPrefOld.DisableSignatures!=_webFormPref.DisableSignatures
				|| textboxWebHostAddress.Text.Trim()!=PrefC.GetString(PrefName.WebHostSynchServerURL) 
				|| checkAutoFillNameAndBirthdate.Checked!=PrefC.GetBool(PrefName.WebFormsAutoFillNameAndBirthdate))
			{
				butSave.Enabled=true;
			}
			else {
				butSave.Enabled=false;
			}
		}

		private void textboxWebHostAddress_TextChanged(object sender,EventArgs e) {
			ResetSaveButton();
		}

		private void butSave_Click(object sender,EventArgs e) {
			//disabled unless user changed url
			if(SavePrefs()) {
				butSave.Enabled=false;
			}
		}

		private bool SavePrefs(bool includeAutoFillBirthdatePref=false) {
			Cursor=Cursors.WaitCursor;
			if(!WebForms_Preferences.SetPreferences(_webFormPref,urlOverride:textboxWebHostAddress.Text.Trim())) {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Either the registration key provided by the dental office is incorrect or the Host Server Address cannot be found.");
				return false;
			}
			_webFormPrefOld=_webFormPref.Copy();
			if(Prefs.UpdateString(PrefName.WebHostSynchServerURL,textboxWebHostAddress.Text.Trim())
				|| (includeAutoFillBirthdatePref && Prefs.UpdateBool(PrefName.WebFormsAutoFillNameAndBirthdate,checkAutoFillNameAndBirthdate.Checked)))
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
			_webFormPref.ColorBorder=butWebformBorderColor.BackColor;
			ResetSaveButton();
			colorDialog1.Dispose();
		}

		private void textRedirectURL_TextChanged(object sender,EventArgs e) {
			ConstructURLs();
		}

		private void butNextForms_Click(object sender,EventArgs e) {
			if(_listWebFormSheetDefs==null) {
				MsgBox.Show(this,"No selected Available Web Forms");
				return;
			}
			using InputBox input=new InputBox("Select next forms",_listWebFormSheetDefs.Select(x => x.Description).ToList(),true);
			input.Text=Lan.g(this,"Select Sheet Defs");
			if(input.ShowDialog()==DialogResult.OK) {
				_listSelectedNextFormIds=input.SelectedIndices.Select(x => _listWebFormSheetDefs[x].WebSheetDefID).ToList();
				textNextForms.Text=string.Join(", ",input.SelectedIndices.Select(x => _listWebFormSheetDefs[x].Description));
				ConstructURLs();
			}
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
			foreach(WebForms_SheetDef sheetDef in gridMain.SelectedTags<WebForms_SheetDef>()) {
				string url=SheetDefBaseURL(sheetDef);
				if(_listSelectedNextFormIds.Count>0) {
					url+="&NFID="+string.Join("&NFID=",_listSelectedNextFormIds);
				}
				if(comboClinic.SelectedClinicNum > 0) {//'None' is not selected
					url+="&CID="+comboClinic.SelectedClinicNum.ToString(); 
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
			_webFormPref.DisableSignatures=checkDisableWebFormSignatures.Checked;
			checkDisableTypedSig.Enabled=!checkDisableWebFormSignatures.Checked;
			ResetSaveButton();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormSheetPicker FormS=new FormSheetPicker();
			FormS.SheetType=SheetTypeEnum.PatientForm;
			FormS.HideKioskButton=true;
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK) {
				return;
			}
			//Make sure each selected sheet contains FName, LName, and Birthdate.
			foreach(SheetDef selectedSheetDef in FormS.SelectedSheetDefs) {//There will always only be one
				if(!WebFormL.VerifyRequiredFieldsPresent(selectedSheetDef)) {
					return;
				}
			}
			Cursor=Cursors.WaitCursor;
			foreach(SheetDef selectedSheetDef in FormS.SelectedSheetDefs) {
				WebFormL.LoadImagesToSheetDef(selectedSheetDef);
				WebFormL.TryAddOrUpdateSheetDef(this,selectedSheetDef,true);
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
			WebForms_SheetDef wf_sheetDef=gridMain.SelectedTag<WebForms_SheetDef>();
			SheetDef sheetDef=SheetDefs.GetFirstOrDefault(x => x.SheetDefNum==wf_sheetDef.SheetDefNum);
			if(sheetDef==null) {//This web form has never had a SheetDefNum assigned or the sheet has been deleted.
				MsgBox.Show(this,"This Web Form is not linked to a valid Sheet.  Please select the correct Sheet that this Web Form should be linked to.");
				using FormSheetPicker FormS=new FormSheetPicker();
				FormS.SheetType=SheetTypeEnum.PatientForm;
				FormS.HideKioskButton=true;
				FormS.ShowDialog();
				if(FormS.DialogResult != DialogResult.OK || FormS.SelectedSheetDefs.Count==0) {
					return;
				}
				sheetDef=FormS.SelectedSheetDefs.FirstOrDefault();
			}
			else {//sheetDef not null
				SheetDefs.GetFieldsAndParameters(sheetDef);
			}
			if(!WebFormL.VerifyRequiredFieldsPresent(sheetDef)) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			WebFormL.LoadImagesToSheetDef(sheetDef);
			WebFormL.TryAddOrUpdateSheetDef(this,sheetDef,false,new List<WebForms_SheetDef> { wf_sheetDef });
			FillGrid();
			Cursor=Cursors.Default;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select an item from the grid first.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			int failures=0;
			foreach(WebForms_SheetDef wf_sheetDef in gridMain.SelectedTags<WebForms_SheetDef>()) {
				if(!WebForms_SheetDefs.DeleteSheetDef(wf_sheetDef.WebSheetDefID)) {
					failures++;
				}
			}
			if(failures>0) {
				Cursor=Cursors.Default;
				MessageBox.Show(this,Lan.g(this,"Error deleting ")+POut.Int(failures)+Lan.g(this," web form(s). Either the web service is not available or "
					+"the Host Server Address cannot be found."));
			}
			FillGrid();
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

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butOk_Click(object sender,EventArgs e) {
			if(!string.IsNullOrWhiteSpace(textRedirectURL.Text) && !Uri.IsWellFormedUriString(textRedirectURL.Text,UriKind.Absolute)) {
				string err=label3.Text+Lan.g(this," should use the following format: http://www.patientviewer.com or https://www.patientviewer.com");
				MsgBox.Show(this,err);
				return;
			}
			SheetURLs=textURLs.Text;
			SavePrefs(true);//No check for success, since we are closing just fail silently.
			DialogResult=DialogResult.OK;
		}

		
	}
}