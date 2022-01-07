//#define TRIALONLY //Do not set here because ContrChart.ProcButtonClicked and FormOpenDental also need to test this value.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.Bridges;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
///<summary>All this dialog does is set the patnum and it is up to the calling form to do an immediate refresh, or possibly just change the patnum back to what it was.  So the other patient fields must remain intact during all logic in this form, especially if SelectionModeOnly.</summary>
	public partial class FormPatientSelect : FormODBase{
		private Patients Patients;
		/// <summary>Use when you want to specify a patient without changing the current patient.  If true, then the Add Patient button will not be visible.</summary>
		public bool SelectionModeOnly;
		///<summary>When closing the form, this indicates whether a new patient was added from within this form.</summary>
		public bool NewPatientAdded;
		///<summary>Only used when double clicking blank area in Appts. Sets this value to the currently selected pt.  That patient will come up on the screen already selected and user just has to click OK. Or they can select a different pt or add a new pt.  If 0, then no initial patient is selected.</summary>
		public long InitialPatNum;
		private DataTable _DataTablePats;
		///<summary>When closing the form, this will hold the value of the newly selected PatNum.</summary>
		public long SelectedPatNum;
		private List<DisplayField> _ListDisplayFields;
		///<summary>Set to true if constructor passed in patient object to prefill text boxes.  Used to make sure fillGrid is not called 
		///before FormSelectPatient_Load.</summary>
		private bool _isPreFillLoad=false;
		///<summary>If set, initial patient list will be set to these patients.</summary>
		public List<long> ExplicitPatNums;
		private ODThread _fillGridThread=null;
		private DateTime _dateTimeLastSearch;
		private DateTime _dateTimeLastRequest;
		private Process _processOnScreenKeyboard=null;
		private PtTableSearchParams _ptTableSearchParams;
		private List<Site> _listSites;
		private List<Def> _listBillingTypeDefs;
		///<summary>Local cache of the pref PatientSelectSearchMinChars, since this will be used in every textbox.TextChanged event and we don't want to
		///parse the pref and convert to an int with every character entered.</summary>
		private int _patSearchMinChars=1;
		///<summary>Used to adjust gridpat contextmenu for right click, and unmask text</summary>
		private Point _lastClickedPoint;
		private TextBox selectedTxtBox;

		#region On Screen Keyboard Dll Imports
		[System.Runtime.InteropServices.DllImport("kernel32.dll",SetLastError = true)]
		static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);

		[System.Runtime.InteropServices.DllImport("kernel32.dll",SetLastError = true)]
		static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);
		#endregion

		///<summary></summary>
		public FormPatientSelect():this(null) {
		}

		///<summary>This takes a partially built patient object and uses it to prefill textboxes to assist in searching.  
		///Currently only implements FName,LName.</summary>
		public FormPatientSelect(Patient pat){
			InitializeComponent();//required first
			InitializeLayoutManager();
			Patients=new Patients();
			Lan.F(this);
			if(pat!=null) {
				PreFillSearchBoxes(pat);
			}
		}

		///<summary></summary>
		public void FormSelectPatient_Load(object sender,System.EventArgs e) {
			if(!PrefC.GetBool(PrefName.DockPhonePanelShow)) {
				labelCountry.Visible=false;
				textCountry.Visible=false;
			}
			if(!PrefC.GetBool(PrefName.DistributorKey)) {
				labelRegKey.Visible=false;
				textRegKey.Visible=false;
			}
			checkRefresh.Enabled=PrefC.GetBool(PrefName.EnterpriseAllowRefreshWhileTyping);
			checkShowInactive.Checked=PrefC.GetBool(PrefName.PatientSelectShowInactive);
			if(SelectionModeOnly){
				groupAddPt.Visible=false;
			}
			//Cannot add new patients from OD select patient interface.  Patient must be added from HL7 message.
			if(HL7Defs.IsExistingHL7Enabled()) {
				HL7Def def=HL7Defs.GetOneDeepEnabled();
				if(def.ShowDemographics!=HL7ShowDemographics.ChangeAndAdd) {
					groupAddPt.Visible=false;
				}
			}
			else {
				if(Programs.UsingEcwTightOrFullMode()) {
					groupAddPt.Visible=false;
				}
			}
			comboBillingType.Items.Add(Lan.g(this,"All"));
			comboBillingType.SelectedIndex=0;
			_listBillingTypeDefs=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			for(int i=0;i<_listBillingTypeDefs.Count;i++){
				comboBillingType.Items.Add(_listBillingTypeDefs[i].ItemName);
			}
			if(PrefC.GetBool(PrefName.EasyHidePublicHealth)){
				comboSite.Visible=false;
				labelSite.Visible=false;
			}
			else{
				comboSite.Items.Add(Lan.g(this,"All"));
				comboSite.SelectedIndex=0;
				_listSites=Sites.GetDeepCopy();
				for(int i=0;i<_listSites.Count;i++) {
					comboSite.Items.Add(_listSites[i].Description);
				}
			}
			if(PrefC.HasClinicsEnabled) {
				if(Clinics.ClinicNum==0) {
					comboClinic.IsAllSelected=true;
				}
				else {
					comboClinic.SelectedClinicNum=Clinics.ClinicNum;
				}
			}
			if(PrefC.GetBool(PrefName.PatientSSNMasked)) {
				//Add "View SS#" right click option, MenuItemPopup() will show and hide it as needed.
				if(gridMain.ContextMenu==null) {
					gridMain.ContextMenu=new ContextMenu();//ODGrid will automatically attach the default Popups
				}
				ContextMenu menu = gridMain.ContextMenu;
				MenuItem menuItemUnmaskSSN=new MenuItem();
				menuItemUnmaskSSN.Enabled=false;
				menuItemUnmaskSSN.Visible=false;
				menuItemUnmaskSSN.Name="ViewSS#";
				menuItemUnmaskSSN.Text="View SS#";
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					menuItemUnmaskSSN.Text="View SIN";
				}
				menuItemUnmaskSSN.Click+= new System.EventHandler(this.MenuItemUnmaskSSN_Click);
				menu.MenuItems.Add(menuItemUnmaskSSN);
				menu.Popup+=MenuItemPopupUnmaskSSN;
			}
			if(PrefC.GetBool(PrefName.PatientDOBMasked)) {
				//Add "View DOB" right click option, MenuItemPopup() will show and hide it as needed.
				if(gridMain.ContextMenu==null) {
					gridMain.ContextMenu=new ContextMenu();//ODGrid will automatically attach the default Popups
				}
				ContextMenu menu = gridMain.ContextMenu;
				MenuItem menuItemUnmaskDOB=new MenuItem();
				menuItemUnmaskDOB.Enabled=false;
				menuItemUnmaskDOB.Visible=false;
				menuItemUnmaskDOB.Name="ViewDOB";
				menuItemUnmaskDOB.Text="View DOB";
				menuItemUnmaskDOB.Click+= new System.EventHandler(this.MenuItemUnmaskDOB_Click);
				menu.MenuItems.Add(menuItemUnmaskDOB);
				menu.Popup+=MenuItemPopupUnmaskDOB;
			}
			FillSearchOption();
			SetGridCols();
			//Using PrefC.GetString on the following two prefs so that we can call PIn.Int with hasExceptions=false and using the Math.Max and Math.Min we
			//are guaranteed to get a valid number from these prefs.
			timerFillGrid.Interval=PIn.Int(PrefC.GetString(PrefName.PatientSelectSearchPauseMs));
			_patSearchMinChars=PIn.Int(PrefC.GetString(PrefName.PatientSelectSearchMinChars));
			if(ExplicitPatNums!=null && ExplicitPatNums.Count>0) {
				FillGrid(false,ExplicitPatNums);
				return;
			}
			if(InitialPatNum!=0){
				Patient iPatient=Patients.GetLim(InitialPatNum);
				textLName.Text=iPatient.LName;
				FillGrid(false);
				return;
			}
			//Always fillGrid if _isPreFilledLoad.  Since the first name and last name are pre-filled, the results should be minimal.
			//Also FillGrid if checkRefresh is checked and either PatientSelectSearchWithEmptyParams is set or there is a character in at least one textbox
			if(_isPreFillLoad || DoRefreshGrid()) {
				FillGrid(true);
				_isPreFillLoad=false;
			}
			//Set the Textbox Enter Event Handler to keep track of which TextBox had focus last.  
			//This helps dictate the desired text box for input after opening up the On Screen Keyboard.
			SetAllTextBoxEnterEventListeners();
      if(ODBuild.IsWeb()) {
        //Keyboard does not currently work with WEB users. 
        butOnScreenKeyboard.Visible=false;
      }
		}

		///<summary>This used to be called all the time, now only needs to be called on load.</summary>
		private void FillSearchOption() {
			switch(ComputerPrefs.LocalComputer.PatSelectSearchMode) {
				case SearchMode.Default:
					checkRefresh.Checked=!PrefC.GetBool(PrefName.PatientSelectUsesSearchButton) && PrefC.GetBool(PrefName.EnterpriseAllowRefreshWhileTyping);//Use global preference
					break;
				case SearchMode.RefreshWhileTyping:
					checkRefresh.Checked=PrefC.GetBool(PrefName.EnterpriseAllowRefreshWhileTyping);
					break;
				case SearchMode.UseSearchButton:
				default:
					checkRefresh.Checked=false;
					break;
			}
		}

		private void SetGridCols(){
			//This pattern is wrong.
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			_ListDisplayFields=DisplayFields.GetForCategory(DisplayFieldCategory.PatientSelect);
			for(int i=0;i<_ListDisplayFields.Count;i++){
				if(_ListDisplayFields[i].Description==""){
					col=new GridColumn(_ListDisplayFields[i].InternalName,_ListDisplayFields[i].ColumnWidth);
				}
				else{
					col=new GridColumn(_ListDisplayFields[i].Description,_ListDisplayFields[i].ColumnWidth);
				}
				gridMain.ListGridColumns.Add(col);
			}
			gridMain.EndUpdate();
		}

		///<summary>The pat must not be null.  Takes a partially built patient object and uses it to fill the search by textboxes.
		///Currently only implements FName, LName, and HmPhone.</summary>
		public void PreFillSearchBoxes(Patient pat) {
			_isPreFillLoad=true; //Set to true to stop FillGrid from being called as a result of textChanged events
			if(pat.LName != "") {
				textLName.Text=pat.LName;
			}
			if(pat.FName != "") {
				textFName.Text=pat.FName;
			}
			if(pat.HmPhone != "") {
				textPhone.Text=pat.HmPhone;
			}
		}

		///<summary>Returns the count of chars in all of the textboxes on the form.  For ValidPhone textboxes only digit chars are counted.</summary>
		private int TextBoxCharCount() {
			//only count digits in ValidPhone textboxes because we auto-format with special chars, ex: (xxx)xxx-xxxx
			return UIHelper.GetAllControls(this)
				.OfType<TextBox>()//ValidPhone is a TextBox
				.Sum(x => x is ValidPhone?x.Text.Count(y => char.IsDigit(y)):x.TextLength);
		}

		///<summary>Returns false if either checkRefresh is not checked or PatientSelectSearchWithEmptyParams is Yes or Unknown and all of the textboxes 
		///are empty. Otherwise returns true.</summary>
		private bool DoRefreshGrid() {
			return checkRefresh.Checked && (PIn.Enum<YN>(PrefC.GetInt(PrefName.PatientSelectSearchWithEmptyParams))!=YN.No || TextBoxCharCount()>0);
		}

		///<summary>Just prior to displaying the context menu, enable or disables the UnmaskSSN option</summary>
		private void MenuItemPopupUnmaskSSN(object sender,EventArgs e) {
			MenuItem menuItemSSN=gridMain.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Name == "ViewSS#");
			if(menuItemSSN==null) { return; }//Should not happen
			MenuItem menuItemSeperator=gridMain.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Text == "-");
			if(menuItemSeperator==null) { return; }//Should not happen
			int idxGridPatSSNCol=_ListDisplayFields.FindIndex(x => x.InternalName == "SSN");
			int idxColClick = gridMain.PointToCol(_lastClickedPoint.X);
			int idxRowClick = gridMain.PointToRow(_lastClickedPoint.Y);
			if(idxRowClick > -1 && idxColClick > -1 && idxGridPatSSNCol==idxColClick) {
				if(Security.IsAuthorized(Permissions.PatientSSNView,true) && gridMain.ListGridRows[idxRowClick].Cells[idxColClick].Text!="") {
					menuItemSSN.Visible=true;
					menuItemSSN.Enabled=true;
				}
				else {
					menuItemSSN.Visible=true;
					menuItemSSN.Enabled=false;
				}
				menuItemSeperator.Visible=true;
				menuItemSeperator.Enabled=true;
			}
			else {
				menuItemSSN.Visible=false;
				menuItemSSN.Enabled=false;
				if(gridMain.ContextMenu.MenuItems.OfType<MenuItem>().Count(x => x.Visible==true && x.Text != "-") > 1) {
					//There is more than one item showing, we want the seperator.
					menuItemSeperator.Visible=true;
					menuItemSeperator.Enabled=true;
				}
				else {
					//We dont want the seperator to be there with only one option.
					menuItemSeperator.Visible=false;
					menuItemSeperator.Enabled=false;
				}
			}
		}

		private void MenuItemUnmaskSSN_Click(object sender,EventArgs e) {
			if(_fillGridThread!=null) {//still filtering results (rarely happens). 
				//Slightly annoying to be unresponsive to the click, but the grid is going to overwrite what we fill so don't bother.
				return;
			}
			//Preference and permissions check has already happened by this point.
			//Guaranteed to be clicking on a valid row & column.
			int idxColClick = gridMain.PointToCol(_lastClickedPoint.X);
			int idxRowClick = gridMain.PointToRow(_lastClickedPoint.Y);
			long patNumClicked=PIn.Long(_DataTablePats.Rows[idxRowClick]["PatNum"].ToString());
			gridMain.BeginUpdate();
			gridMain.ListGridRows[idxRowClick].Cells[idxColClick].Text=Patients.SSNFormatHelper(Patients.GetPat(patNumClicked).SSN,false);
			gridMain.EndUpdate();
			string logtext="";
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				logtext="Social Insurance Number";
			}
			else {
				logtext="Social Security Number";
			}
			logtext+=" unmasked in Patient Select";
			SecurityLogs.MakeLogEntry(Permissions.PatientSSNView,patNumClicked,logtext);
		}

		///<summary>Just prior to displaying the context menu, enable or disables the UnmaskDOB option</summary>
		private void MenuItemPopupUnmaskDOB(object sender,EventArgs e) {
			MenuItem menuItemDOB=gridMain.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Name == "ViewDOB");
			if(menuItemDOB==null) { return; }//Should not happen
			MenuItem menuItemSeperator=gridMain.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Text == "-");
			if(menuItemSeperator==null) { return; }//Should not happen
			int idxGridPatDOBCol=_ListDisplayFields.FindIndex(x => x.InternalName == "Birthdate");
			int idxColClick = gridMain.PointToCol(_lastClickedPoint.X);
			int idxRowClick = gridMain.PointToRow(_lastClickedPoint.Y);
			if(idxRowClick > -1 && idxColClick > -1 && idxGridPatDOBCol==idxColClick) {
				if(Security.IsAuthorized(Permissions.PatientDOBView,true) && gridMain.ListGridRows[idxRowClick].Cells[idxColClick].Text!="") {
					menuItemDOB.Visible=true;
					menuItemDOB.Enabled=true;
				}
				else {
					menuItemDOB.Visible=true;
					menuItemDOB.Enabled=false;
				}
				menuItemSeperator.Visible=true;
				menuItemSeperator.Enabled=true;
			}
			else {
				menuItemDOB.Visible=false;
				menuItemDOB.Enabled=false;
				if(gridMain.ContextMenu.MenuItems.OfType<MenuItem>().Count(x => x.Visible==true && x.Text != "-") > 1) {
					//There is more than one item showing, we want the seperator.
					menuItemSeperator.Visible=true;
					menuItemSeperator.Enabled=true;
				}
				else {
					//We dont want the seperator to be there with only one option.
					menuItemSeperator.Visible=false;
					menuItemSeperator.Enabled=false;
				}
			}
		}

		private void MenuItemUnmaskDOB_Click(object sender,EventArgs e) {
			if(_fillGridThread!=null) {//still filtering results (rarely happens). 
				//Slightly annoying to be unresponsive to the click, but the grid is going to overwrite what we fill so don't bother.
				return;
			}
			//Preference and permissions check has already happened by this point.
			//Guaranteed to be clicking on a valid row & column.
			int idxColClick = gridMain.PointToCol(_lastClickedPoint.X);
			int idxRowClick = gridMain.PointToRow(_lastClickedPoint.Y);
			long patNumClicked=PIn.Long(_DataTablePats.Rows[idxRowClick]["PatNum"].ToString());
			DateTime birthdate=PIn.Date(_DataTablePats.Rows[idxRowClick]["Birthdate"].ToString());
			gridMain.BeginUpdate();
			gridMain.ListGridRows[idxRowClick].Cells[idxColClick].Text=Patients.DOBFormatHelper(birthdate,false);
			gridMain.EndUpdate();
			string logtext="Date of birth unmasked in Patient Select";
			SecurityLogs.MakeLogEntry(Permissions.PatientDOBView,patNumClicked,logtext);
		}

		private void textbox_TextChanged(object sender,EventArgs e) {
		timerFillGrid.Stop();
		this.AcceptButton=butSearch;
			if(TextBoxCharCount()<_patSearchMinChars) {
				timerFillGrid.Start();//count of characters entered into all textboxes is < _patSearchMinChars, restart the timer
				return;
			}
			OnDataEntered();//count of characters entered into all textboxes is >= _patSearchMinChars, fill the grid
		}

		private void textbox_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.Up || e.KeyCode==Keys.Down) {
				gridMain.Invalidate();
				e.Handled=true;
			}
		}

		private void checkRefresh_Click(object sender,EventArgs e) {
			timerFillGrid.Stop();
			if(checkRefresh.Checked) {
				ComputerPrefs.LocalComputer.PatSelectSearchMode=SearchMode.RefreshWhileTyping;
				if(DoRefreshGrid()) {//only fill grid if PatientSelectSearchWithEmptyParams is true or there is something in at least one textbox
					FillGrid(true);
				}
			}
			else{
				ComputerPrefs.LocalComputer.PatSelectSearchMode=SearchMode.UseSearchButton;
			}
			ComputerPrefs.Update(ComputerPrefs.LocalComputer);
		}

		private void checkShowArchived_CheckedChanged(object sender,EventArgs e) {
			//We are only going to give the option to hide merged patients when Show Archived is checked.
			checkShowMerged.Visible=checkShowArchived.Checked;
			OnDataEntered();
		}

		private void butSearch_Click(object sender, System.EventArgs e) {
			timerFillGrid.Stop();
			_ptTableSearchParams=null;//this will force a grid refresh
			FillGrid(true);
		}

		private void butGetAll_Click(object sender,EventArgs e) {
			timerFillGrid.Stop();
			_ptTableSearchParams=null;//this will force a grid refresh
			FillGrid(false);
		}

		private void OnDataEntered(object sender=null,EventArgs e=null) {
			timerFillGrid.Stop();//stop the timer, otherwise the timer tick will just fire this again
			//Do not call FillGrid unless _isPreFillLoad=false.  Since the first name and last name are pre-filled, the results should be minimal.
			//DoRefreshGrid will return true if checkRefresh is checked and either PatientSelectSearchWithEmptyParams is true (or unset) or there is some
			//text in at least one of the textboxes
			if(!_isPreFillLoad && DoRefreshGrid()) {
				FillGrid(true);
			}
		}

		private void FillGrid(bool doLimitOnePage,List<long> listtExplicitPatNums=null) {
			timerFillGrid.Stop();//stop the timer, we're filling the grid now
			_dateTimeLastRequest=DateTime.Now;
			if(_fillGridThread!=null) {
				return;
			}
			_dateTimeLastSearch=_dateTimeLastRequest;
			long billingType=0;
			if(comboBillingType.SelectedIndex!=0) {
				billingType=_listBillingTypeDefs[comboBillingType.SelectedIndex-1].DefNum;
			}
			long siteNum=0;
			if(!PrefC.GetBool(PrefName.EasyHidePublicHealth) && comboSite.SelectedIndex!=0) {
				siteNum=_listSites[comboSite.SelectedIndex-1].SiteNum;
			}
			DateTime birthdate=PIn.Date(textBirthdate.Text); //this will frequently be minval.
			string clinicNums="";
			if(PrefC.HasClinicsEnabled) {
				if(comboClinic.IsAllSelected) {
					//When below preference is false, don't hide user restricted clinics from view. Just return clinicNums as an empty string.
					//If this preference is true, we DO hide user restricted clinics from view.
					if(PrefC.GetBool(PrefName.PatientSelectFilterRestrictedClinics) && (Security.CurUser.ClinicIsRestricted || !checkShowArchived.Checked)) {
						//only set clinicNums if user is unrestricted and showing hidden clinics, otherwise the search will show patients from all clinics
						clinicNums=string.Join(",",comboClinic.ListClinics
							//.Where(x => !x.IsHidden || checkShowArchived.Checked)//Only show hidden clinics if "Show Archived" is checked
							.Select(x => x.ClinicNum));
					}
				}
				else {
					clinicNums=comboClinic.SelectedClinicNum.ToString();
					if(checkShowArchived.Checked) {
						foreach(Clinic clinic in comboClinic.ListClinics) {
							if(clinic.IsHidden) {
								clinicNums+=","+clinic.ClinicNum.ToString();
							}
						}
					}
				}
			}
			bool hasSpecialty=_ListDisplayFields.Any(x => x.InternalName=="Specialty");
			bool hasNextLastVisit=_ListDisplayFields.Any(x => ListTools.In(x.InternalName,"NextVisit","LastVisit"));
			DataTable dataTablePats=new DataTable();
			//Because hiding merged patients makes the query take longer, we will default to showing merged patients if the user has not had the 
			//opportunity to set this check box.
			bool doShowMerged=true;
			if(checkShowMerged.Visible) {
				//Only allow hiding merged if the Show Archived box is checked (and Show Merged is therefore visible).
				doShowMerged=checkShowMerged.Checked;
			}
			PtTableSearchParams ptTableSearchParamsCur=new PtTableSearchParams(doLimitOnePage,textLName.Text,textFName.Text,textPhone.Text,textAddress.Text,
				!checkShowInactive.Checked,textCity.Text,textState.Text,textSSN.Text,textPatNum.Text,textChartNumber.Text,billingType,checkGuarantors.Checked,
				checkShowArchived.Checked,birthdate,siteNum,textSubscriberID.Text,textEmail.Text,textCountry.Text,textRegKey.Text,clinicNums,"",
				textInvoiceNumber.Text,listtExplicitPatNums,InitialPatNum,doShowMerged,hasSpecialty,hasNextLastVisit);
			if(_ptTableSearchParams!=null && _ptTableSearchParams.Equals(ptTableSearchParamsCur)) {//fill grid search params haven't changed, just return
				return;
			}
			_ptTableSearchParams=ptTableSearchParamsCur.Copy();
			_fillGridThread=new ODThread(new ODThread.WorkerDelegate((ODThread o) => {
				dataTablePats=Patients.GetPtDataTable(ptTableSearchParamsCur);
			}));
			_fillGridThread.AddExitHandler(new ODThread.WorkerDelegate((ODThread o) => {
				_fillGridThread=null;
				try {
					this.BeginInvoke((Action)(() => {
						_DataTablePats=dataTablePats;
						FillGridFinal(doLimitOnePage);
					}));
				}catch(Exception) { } //do nothing. Usually just a race condition trying to invoke from a disposed form.
			}));
			_fillGridThread.AddExceptionHandler(new ODThread.ExceptionDelegate((e) => {
				try {
					this.BeginInvoke((Action)(() => {
						MessageBox.Show(e.Message);
					}));
				}catch(Exception) { } //do nothing. Usually just a race condition trying to invoke from a disposed form.
			}));
			_fillGridThread.Start(true);
		}

		private void FillGridFinal(bool doLimitOnePage) {
			labelMatchingRecords.Text=$"{_DataTablePats.Rows.Count} Records Displayed{(doLimitOnePage? " Out of Many" : "")}";
			if(InitialPatNum!=0 && doLimitOnePage) {
				//The InitialPatNum will be at the top, so resort the list alphabetically
				DataView dataView=_DataTablePats.DefaultView;
				dataView.Sort="LName,FName";
				_DataTablePats=dataView.ToTable();
			}
			gridMain.BeginUpdate();
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_DataTablePats.Rows.Count;i++) {
				row=new GridRow();
				for(int f=0;f<_ListDisplayFields.Count;f++) {
					switch(_ListDisplayFields[f].InternalName) {
						case "LastName":
							row.Cells.Add(_DataTablePats.Rows[i]["LName"].ToString());
							break;
						case "First Name":
							row.Cells.Add(_DataTablePats.Rows[i]["FName"].ToString());
							break;
						case "MI":
							row.Cells.Add(_DataTablePats.Rows[i]["MiddleI"].ToString());
							break;
						case "Pref Name":
							row.Cells.Add(_DataTablePats.Rows[i]["Preferred"].ToString());
							break;
						case "Age":
							row.Cells.Add(_DataTablePats.Rows[i]["age"].ToString());
							break;
						case "SSN":
							row.Cells.Add(Patients.SSNFormatHelper(_DataTablePats.Rows[i]["SSN"].ToString(),PrefC.GetBool(PrefName.PatientSSNMasked)));
							break;
						case "Hm Phone":
							row.Cells.Add(_DataTablePats.Rows[i]["HmPhone"].ToString());
							if(Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
								row.Cells[row.Cells.Count-1].ColorText=Color.Blue;
								row.Cells[row.Cells.Count-1].Underline=YN.Yes;
							}
							break;
						case "Wk Phone":
							row.Cells.Add(_DataTablePats.Rows[i]["WkPhone"].ToString());
							if(Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
								row.Cells[row.Cells.Count-1].ColorText=Color.Blue;
								row.Cells[row.Cells.Count-1].Underline=YN.Yes;
							}
							break;
						case "PatNum":
							row.Cells.Add(_DataTablePats.Rows[i]["PatNum"].ToString());
							break;
						case "ChartNum":
							row.Cells.Add(_DataTablePats.Rows[i]["ChartNumber"].ToString());
							break;
						case "Address":
							row.Cells.Add(_DataTablePats.Rows[i]["Address"].ToString());
							break;
						case "Status":
							row.Cells.Add(_DataTablePats.Rows[i]["PatStatus"].ToString());
							break;
						case "Bill Type":
							row.Cells.Add(_DataTablePats.Rows[i]["BillingType"].ToString());
							break;
						case "City":
							row.Cells.Add(_DataTablePats.Rows[i]["City"].ToString());
							break;
						case "State":
							row.Cells.Add(_DataTablePats.Rows[i]["State"].ToString());
							break;
						case "Pri Prov":
							row.Cells.Add(_DataTablePats.Rows[i]["PriProv"].ToString());
							break;
						case "Clinic":
							row.Cells.Add(_DataTablePats.Rows[i]["clinic"].ToString());
							break;
						case "Birthdate":
							row.Cells.Add(Patients.DOBFormatHelper(PIn.Date(_DataTablePats.Rows[i]["Birthdate"].ToString()),PrefC.GetBool(PrefName.PatientDOBMasked)));
							break;
						case "Site":
							row.Cells.Add(_DataTablePats.Rows[i]["site"].ToString());
							break;
						case "Email":
							row.Cells.Add(_DataTablePats.Rows[i]["Email"].ToString());
							break;
						case "Country":
							row.Cells.Add(_DataTablePats.Rows[i]["Country"].ToString());
							break;
						case "RegKey":
							row.Cells.Add(_DataTablePats.Rows[i]["RegKey"].ToString());
							break;
						case "OtherPhone": //will only be available if OD HQ
							row.Cells.Add(_DataTablePats.Rows[i]["OtherPhone"].ToString());
							break;
						case "Wireless Ph":
							row.Cells.Add(_DataTablePats.Rows[i]["WirelessPhone"].ToString());
							if(Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
								row.Cells[row.Cells.Count-1].ColorText=Color.Blue;
								row.Cells[row.Cells.Count-1].Underline=YN.Yes;
							}
							break;
						case "Sec Prov":
							row.Cells.Add(_DataTablePats.Rows[i]["SecProv"].ToString());
							break;
						case "LastVisit":
							row.Cells.Add(_DataTablePats.Rows[i]["lastVisit"].ToString());
							break;
						case "NextVisit":
							row.Cells.Add(_DataTablePats.Rows[i]["nextVisit"].ToString());
							break;
						case "Invoice Number":
							row.Cells.Add(_DataTablePats.Rows[i]["StatementNum"].ToString());
							break;
						case "Specialty":
							row.Cells.Add(_DataTablePats.Rows[i]["Specialty"].ToString());
							break;
					}
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			if(_dateTimeLastSearch!=_dateTimeLastRequest) {
				FillGrid(doLimitOnePage);//in case data was entered while thread was running.
			}
			gridMain.SetSelected(0,true);
			for(int i=0;i<_DataTablePats.Rows.Count;i++) {
				if(PIn.Long(_DataTablePats.Rows[i][0].ToString())==InitialPatNum) {
					gridMain.SetSelected(i,true);
					break;
				}
			}
			this.AcceptButton=butOK;
		}

		private void gridMain_MouseDown(object sender,MouseEventArgs e) {
			_lastClickedPoint=e.Location;
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			PatSelected();
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			GridCell gridCellCur=gridMain.ListGridRows[e.Row].Cells[e.Col];
			//Only grid cells with phone numbers are blue and underlined.
			if(gridCellCur.ColorText==Color.Blue && gridCellCur.Underline==YN.Yes && Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
				DentalTek.PlaceCall(gridCellCur.Text);
			}
		}

		///<summary>Remember, this button is not even visible if SelectionModeOnly.</summary>
		private void butAddPt_Click(object sender, System.EventArgs e){
			if(ODBuild.IsTrial()) { 
				MsgBox.Show(this,"Trial version.  Maximum 30 patients");
				if(Patients.GetNumberPatients()>30){
					MsgBox.Show(this,"Maximum reached");
					return;
				}
			}
			if(textLName.Text=="" && textFName.Text=="" && textChartNumber.Text==""){
				MessageBox.Show(Lan.g(this,"Not allowed to add a new patient until you have done a search to see if that patient already exists. "
					+"Hint: just type a few letters into the Last Name box above.")); 
				return;
			}
			long priProv=0;
			if(!PrefC.GetBool(PrefName.PriProvDefaultToSelectProv)) {
				//Explicitly use the combo clinic instead of FormOpenDental.ClinicNum because the combo box should default to that clinic unless manually changed by the user.
				if(PrefC.HasClinicsEnabled && !comboClinic.IsAllSelected) {//clinics enabled and all isn't selected
					//Set the patients primary provider to the clinic default provider.
					Provider prov=Providers.GetDefaultProvider(comboClinic.SelectedClinicNum);
					if(prov!=null) {
						priProv=prov.ProvNum;
					}
				}
				else {
					//Set the patients primary provider to the practice default provider.
					Provider prov=Providers.GetDefaultProvider();
					if(prov!=null) {
						priProv=prov.ProvNum;
					}
				}
			}
			Patient PatCur=Patients.CreateNewPatient(textLName.Text,textFName.Text,PIn.Date(textBirthdate.Text),priProv,Clinics.ClinicNum
				,Lan.g(this,"Created from Select Patient window."));
			Family FamCur=Patients.GetFamily(PatCur.PatNum);
			if(Plugins.HookMethod(this,"FormPatientSelect.butAddPt_Click_showForm",PatCur,FamCur)) {
				return;
			}
			using FormPatientEdit FormPE=new FormPatientEdit(PatCur,FamCur);
			FormPE.IsNew=true;
			FormPE.ShowDialog();
			if(FormPE.DialogResult==DialogResult.OK){
				NewPatientAdded=true;
				SelectedPatNum=PatCur.PatNum;
				ImageStore.GetPatientFolder(PatCur,ImageStore.GetPreferredAtoZpath());
				DialogResult=DialogResult.OK;
			}
		}

		private void butAddAll_Click(object sender,EventArgs e) {
			if(ODBuild.IsTrial()) { 
				MsgBox.Show(this,"Trial version.  Maximum 30 patients");
				if(Patients.GetNumberPatients()>30){
					MsgBox.Show(this,"Maximum reached");
					return;
				}
			}
			if(textLName.Text=="" && textFName.Text=="" && textChartNumber.Text==""){
				MessageBox.Show(Lan.g(this,"Not allowed to add a new patient until you have done a search to see if that patient already exists. Hint: just type a few letters into the Last Name box above.")); 
				return;
			}
			using FormPatientAddAll FormP=new FormPatientAddAll();
			if(textLName.Text.Length>1){//eg Sp
				FormP.LName=textLName.Text.Substring(0,1).ToUpper()+textLName.Text.Substring(1);
			}
			if(textFName.Text.Length>1){
				FormP.FName=textFName.Text.Substring(0,1).ToUpper()+textFName.Text.Substring(1);
			}
			if(textBirthdate.Text.Length>1) {
				FormP.Birthdate=PIn.Date(textBirthdate.Text);
			}
			FormP.ShowDialog();
			if(FormP.DialogResult!=DialogResult.OK){
				return;
			}
			NewPatientAdded=true;
			SelectedPatNum=FormP.SelectedPatNum;
			DialogResult=DialogResult.OK;
		}

		private void PatSelected(){
			if(_fillGridThread!=null) {
				return;//still filtering results (rarely happens)
			}
			long patNumSelected=PIn.Long(_DataTablePats.Rows[gridMain.GetSelectedIndex()]["PatNum"].ToString());
			if(PrefC.HasClinicsEnabled){
				long patClinicNum=PIn.Long(_DataTablePats.Rows[gridMain.GetSelectedIndex()]["ClinicNum"].ToString());
				List<long> listUserClinicNums=comboClinic.ListClinics.Select(x => x.ClinicNum).ToList();
				if(!Security.CurUser.ClinicIsRestricted) {
					listUserClinicNums.Add(0);
				}
				//If the user has security permissions to search all patients, or patient is assigned to one of the user's unrestricted clinics,
				//or patient has an appointment in one of the user's unrestricted clincis, 
				//allow them to select the patient
				if(Security.IsAuthorized(Permissions.UnrestrictedSearch,true) 
					|| ListTools.In(patClinicNum,listUserClinicNums)
					|| Appointments.GetAppointmentsForPat(patNumSelected).Select(x => x.ClinicNum).Any(x => ListTools.In(x,listUserClinicNums))) 
				{
					SelectedPatNum=patNumSelected;
					DialogResult=DialogResult.OK;
				}
				else {//Otherwise, present the error message explainign why they cannot select the patient.
					MsgBox.Show(this,"This patient is assigned to a clinic that you are not authorized for. Contact an Administrator to grant you access or to " +
						"create an appointment in your clinic to avoid patient duplication.");
				}
			}
			else {
				SelectedPatNum=patNumSelected;
				DialogResult=DialogResult.OK;
			}
		}

		#region On Screen Keyboard

		private void butOnScreenKeyboard_Click(object sender, EventArgs e){
			//Toggle the On Screen Keyboard
			if(_processOnScreenKeyboard==null) {
				OpenOnScreenKeyBoard();
			}
			else {
				CloseOnScreenKeyBoard();
			}
		}

		///<summary>This event handler fires when the user closes the On Screen Keyboard by pressing its "X" button.</summary>
		public void OnScreenKeyboardClosedEventHandler(object sender, EventArgs e){
			ODException.SwallowAnyException(CloseOnScreenKeyBoard);
		}

		///<summary>Closes the On Screen Keyboard</summary>
		private void CloseOnScreenKeyBoard() {
			//Remove the on screen keyboard process if it exists
			if(_processOnScreenKeyboard==null) {
				return;
			}
			ODException.SwallowAnyException(() => {
				//Remove the event handler (prevents it from firing when the On Screen Keys are closed by killing a process)
				_processOnScreenKeyboard.Exited-=OnScreenKeyboardClosedEventHandler;
				//If the on screen keyboard process is still running, kill it
				if(!_processOnScreenKeyboard.HasExited) {
					//Focus on the Form Patient Select before killing the On Screen Keyboard Process to avoid a threading error
					//textLName.Select();
					_processOnScreenKeyboard.Refresh();
					_processOnScreenKeyboard.CloseMainWindow();
				}
				_processOnScreenKeyboard=null;
			});
			if(selectedTxtBox!=null) {
				SelectTextBox();
			}
		}

		///<summary>Opens the On Screen Keyboard</summary>
		private void OpenOnScreenKeyBoard() {
			//Load the on screen keyboard process
			//For 64-Bit Systems, running the 32-bit On Screen Keyboard app requires disabling the Windows on Windows (WOW) path redirection, in order to access the System32 folder
			IntPtr wow64Value = IntPtr.Zero;
			if(Environment.Is64BitOperatingSystem){
				Wow64DisableWow64FsRedirection(ref wow64Value);
			}
			_processOnScreenKeyboard=new Process();
			ProcessStartInfo processStartInfo=new ProcessStartInfo();
			string strPath=System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows),Environment.SystemDirectory,"osk.exe");
			processStartInfo.FileName=strPath;
			_processOnScreenKeyboard.StartInfo=processStartInfo;
			//add the event handle for when the on screen keyboard's close button is pressed
			_processOnScreenKeyboard.EnableRaisingEvents=true;
			_processOnScreenKeyboard.Exited+=OnScreenKeyboardClosedEventHandler;
			_processOnScreenKeyboard.Start();
			//Re-enable the WOW path redirection after starting the 32-bit process
			if(Environment.Is64BitOperatingSystem){
				Wow64RevertWow64FsRedirection(wow64Value);
			}
			SelectTextBox();
		}

		///<summary>Focus on the Form Patient Select at the last selected textbox.</summary>
		private void SelectTextBox() {
			if(selectedTxtBox==null) {
				selectedTxtBox=textLName;//Default to the first TextBox in the search criteria if none selected.
			}
			selectedTxtBox.Focus();
			selectedTxtBox.Select(selectedTxtBox.Text.Length,0);
		}

		/// <summary>Keeps track of the latest Selected TextBox (used to maintain cursor location when opening the On-Screen Keyboard)</summary>
		private void textBox_Enter(object sender,EventArgs e) {
			selectedTxtBox=(TextBox)sender;
		}

		/// <summary>Sets the handler for all of the Form's TextBox Enter Events</summary>
		private void SetAllTextBoxEnterEventListeners() {
			foreach(TextBox textBox in UIHelper.GetAllControls(this).OfType<TextBox>()) {
				textBox.Enter+=new System.EventHandler(this.textBox_Enter);
			}
		}
		#endregion

		private void butOK_Click(object sender, System.EventArgs e){
			if(gridMain.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select a patient first.");
				return;
			}
			PatSelected();
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormPatientSelect_FormClosing(object sender,FormClosingEventArgs e) {
			//Try to close the on screen keyboard if one was opened by this form.
			ODException.SwallowAnyException(CloseOnScreenKeyBoard);
			timerFillGrid?.Dispose();//dispose of the timer if it is not null
		}

	}
}
