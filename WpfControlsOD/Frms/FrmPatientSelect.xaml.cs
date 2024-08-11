using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CodeBase;
using OpenDentBusiness;
using WpfControls;
using WpfControls.UI;

namespace OpenDental {
/*
Conversion Checklist====================================================================================================================
Questions (do not edit)                                              |Answers might include "yes", "ok", "no", "n/a", "done", etc.
-Review this form. Any unsupported controls or properties?           |maybe
   Search for "progress". Any progress bars?                         |
   Anything in the Tray?                                             |Yes, a timer
   Search for "filter". Any use of SetFilterControlsAndAction?       |
   If yes, then STOP here. Talk to Jordan for strategy               |
-Look in the code for any references to other Forms. If those forms  |n/a
   have not been converted, then STOP.  Convert those forms first.   |
-Will we include TabIndexes?  If so, up to what index?  This applies |Yes, 3
   even if one single control is set so that cursor will start there |
-Grids: get familiar with properties in bold and with events.        |done
-Run UnitTests FormWpfConverter, type in Form name, TabI, and convert|done
-Any conversion exceptions? If so, talk to Jordan.                   |no
-In WpfControlsOD/Frms, include the new files in the project.        |done
-Switch to using this checklist in the new Frm. Delete the other one-|done
-Do the red areas and issues at top look fixable? Consider reverting |probably
-Does convert script need any changes instead of fixing manually?    |no
-Fix all the red areas.                                              |done
-Address all the issues at the top. Leave in place for review.       |done
-Verify that all the click events converted automatically.  ...      |done
-Attach all orphaned event handlers to events in constructor.        |Done
-Possibly make some labels or other controls slightly bigger due to  |Done
   font change.                                                      |
-Change all places where the form is called to now call the new Frm. |1 place so far.
-If there are more than about 2 or 3 refs, then review first with J. |
-Test thoroughly                                                     |done
-Are behavior and look absolutely identical? List any variation.     |I believe so. The only thing I can see missing is right clicking on the grid cells and the copy cell text / copy rows buttons not showing up. There is also a small change in behavior when the user hits enter after typing while they are waiting for a refresh delay where it will select the top patient instead of searching again (minor issue).
   Exceptions include taborders only applying to textboxes           |
   and minor control color variations if they are not annoying       |
-Copy original Form.cs into WpfControlsOD/Frms temporarily for review|done
-Review with Jordan                                                  |
-Commit                                                              |
-Delete the old Winform files. That gets reviewed on the next round  |
-Delete this checklist. That also gets reviewed on the next round    |
End of Checklist=========================================================================================================================
*/
///<summary>All this dialog does is set the patnum and it is up to the calling form to do an immediate refresh, or possibly just change the patnum back to what it was.  So the other patient fields must remain intact during all logic in this form, especially if SelectionModeOnly.</summary>
	public partial class FrmPatientSelect : FrmODBase{
		///<summary></summary>
		public bool CanAddPatients=false;
		///<summary>When closing the form, this indicates whether a new patient was added from within this form.</summary>
		public bool IsNewPatientAdded;
		///<summary>Only used when double clicking blank area in Appts. Sets this value to the currently selected pt.  That patient will come up on the screen already selected and user just has to click OK. Or they can select a different pt or add a new pt.  If 0, then no initial patient is selected.</summary>
		public long PatNumInitial;
		private DataTable _tablePats;
		///<summary>When closing the form, this will hold the value of the newly selected PatNum.</summary>
		public long PatNumSelected;
		private List<DisplayField> _listDisplayFields;
		///<summary>Set to true if constructor passed in patient object to prefill text boxes.  Used to make sure fillGrid is not called 
		///before FormSelectPatient_Load.</summary>
		private bool _isPreFillLoad=false;
		///<summary>If set, initial patient list will be set to these patients.</summary>
		public List<long> ListPatNumsExplicit;
		private Process _processOnScreenKeyboard=null;
		private List<Site> _listSites;
		private List<Def> _listDefsBillingType;
		///<summary>Used to adjust gridpat contextmenu for right click, and unmask text</summary>
		private Point _pointLastClicked;
		private MenuItem menuItemUnmaskSSN;
		private MenuItem menuItemUnmaskDOB;
		private System.Windows.Controls.Separator separator;
		private FilterControlsAndAction _filterControlsAndAction;
		private bool _doLimitOnePage=true;
		/// <summary>Used so that if the checkbox for refresh while typing is unchecked, the grid doesn't refresh upon typing.</summary>
		private bool _ignoreRefresh=false;
		///<summary>Used for determining the accept button. False means that the search button will be the accept button, and true means the OK button will be the accept button.</summary>
		private bool _isGridRefreshed=false;

		#region On Screen Keyboard Dll Imports
		[System.Runtime.InteropServices.DllImport("kernel32.dll",SetLastError = true)]
		static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);

		[System.Runtime.InteropServices.DllImport("kernel32.dll",SetLastError = true)]
		static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);
		#endregion

		///<summary>This takes a partially built patient object and uses it to prefill textboxes to assist in searching.  
		///Currently only implements FName, LName, and HmPhone.</summary>
		public FrmPatientSelect(){
			InitializeComponent();//required first
//todo: Add this back if we later merge this with FormPatientSelect.
			/*
			if(patient!=null) {
				PreFillSearchBoxes(patient);
			}*/
			Load+=FrmPatientSelect_Load;
			#region FilterControls
			_filterControlsAndAction=new FilterControlsAndAction();
			_filterControlsAndAction.AddControl(checkShowMerged);
			_filterControlsAndAction.AddControl(comboClinic);
			_filterControlsAndAction.AddControl(comboSite);
			_filterControlsAndAction.AddControl(comboBillingType);
			_filterControlsAndAction.AddControl(checkGuarantors);
			_filterControlsAndAction.AddControl(checkShowInactive);
			_filterControlsAndAction.AddControl(textLName);
			_filterControlsAndAction.AddControl(textInvoiceNumber);
			_filterControlsAndAction.AddControl(textRegKey);
			_filterControlsAndAction.AddControl(textCountry);
			_filterControlsAndAction.AddControl(textEmail);
			_filterControlsAndAction.AddControl(textSubscriberID);
			_filterControlsAndAction.AddControl(textBirthdate);
			_filterControlsAndAction.AddControl(textChartNumber);
			_filterControlsAndAction.AddControl(textSSN);
			_filterControlsAndAction.AddControl(textPatNum);
			_filterControlsAndAction.AddControl(textState);
			_filterControlsAndAction.AddControl(textCity);
			_filterControlsAndAction.AddControl(textAddress);
			_filterControlsAndAction.AddControl(textPhone);
			_filterControlsAndAction.AddControl(textFName);
			_filterControlsAndAction.FuncDb=RefreshFromDb;
			//Using PrefC.GetString on the following prefs so that we can call PIn.Int with hasExceptions=false.
			//We are guaranteed to get a valid number from these prefs.
			_filterControlsAndAction.SetMinChars(PIn.Int(PrefC.GetString(PrefName.PatientSelectSearchMinChars)));
			_filterControlsAndAction.SetInterval(Int32.Parse(PrefC.GetString(PrefName.PatientSelectSearchPauseMs)));
			_filterControlsAndAction.ActionComplete=FillGrid;
			#endregion
			#region textbox event subscriptions
			//keydown
			textLName.KeyDown+=textbox_KeyDown;
			textInvoiceNumber.KeyDown+=textbox_KeyDown;
			textRegKey.KeyDown+=textbox_KeyDown;
			textCountry.KeyDown+=textbox_KeyDown;
			textEmail.KeyDown+=textbox_KeyDown;
			textSubscriberID.KeyDown+=textbox_KeyDown;
			textBirthdate.KeyDown+=textbox_KeyDown;
			textChartNumber.KeyDown+=textbox_KeyDown;
			textSSN.KeyDown+=textbox_KeyDown;
			textPatNum.KeyDown+=textbox_KeyDown;
			textState.KeyDown+=textbox_KeyDown;
			textCity.KeyDown+=textbox_KeyDown;
			textAddress.KeyDown+=textbox_KeyDown;
			textPhone.KeyDown+=textbox_KeyDown;
			textFName.KeyDown+=textbox_KeyDown;
			#endregion
			gridMain.MouseDown+=gridMain_MouseDown;
			gridMain.CellDoubleClick+=gridMain_CellDoubleClick;
			gridMain.CellClick+=gridMain_CellClick;
			FormClosing+=FrmPatientSelect_FormClosing;
			textPhone.TextChanged+=PatientL.ValidPhone_TextChanged;
			KeyDown+=FrmPatientSelect_KeyDown;
			PreviewKeyDown+=FrmPatientSelect_PreviewKeyDown;
		}

		///<summary></summary>
		public void FrmPatientSelect_Load(object sender,System.EventArgs e) {
			Lang.F(this);
			ContextMenu contextMenu=new ContextMenu(gridMain);
			if(!PrefC.GetBool(PrefName.DockPhonePanelShow)) {
				labelCountry.Visible=false;
				textCountry.Visible=false;
			}
			if(!PrefC.GetBool(PrefName.DistributorKey)) {
				labelRegKey.Visible=false;
				textRegKey.Visible=false;
			}
			checkRefresh.IsEnabled=PrefC.GetBool(PrefName.EnterpriseAllowRefreshWhileTyping);
			checkShowInactive.Checked=PrefC.GetBool(PrefName.PatientSelectShowInactive);
			groupAddPatient.Visible=false;
			if(CanAddPatients){
				groupAddPatient.Visible=true;
			}
/*Todo:
			//Cannot add new patients from OD select patient interface.  Patient must be added from HL7 message.
			if(HL7Defs.IsExistingHL7Enabled()) {
				HL7Def hl7Def=HL7Defs.GetOneDeepEnabled();
				if(hl7Def.ShowDemographics!=HL7ShowDemographics.ChangeAndAdd) {
					groupAddPt.Visible=false;
				}
			}
			else {
				if(Programs.UsingEcwTightOrFullMode()) {
					groupAddPt.Visible=false;
				}
			}*/
			comboBillingType.Items.Add(Lang.g(this,"All"));
			comboBillingType.SelectedIndex=0;
			_listDefsBillingType=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			for(int i=0;i<_listDefsBillingType.Count;i++){
				comboBillingType.Items.Add(_listDefsBillingType[i].ItemName);
			}
			if(PrefC.GetBool(PrefName.EasyHidePublicHealth)){
				comboSite.Visible=false;
				labelSite.Visible=false;
			}
			else{
				comboSite.Items.Add(Lang.g(this,"All"));
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
					comboClinic.ClinicNumSelected=Clinics.ClinicNum;
				}
			}
			contextMenu.Opened+=contextMenu_Opened;
			if(PrefC.GetBool(PrefName.PatientSSNMasked)) {
				//Add "View SS#" right click option, MenuItemPopup() will show and hide it as needed.
				menuItemUnmaskSSN=new MenuItem();
				menuItemUnmaskSSN.IsEnabled=false;
				menuItemUnmaskSSN.Visibility=Visibility.Collapsed;
				menuItemUnmaskSSN.Name="ViewSSN";
				menuItemUnmaskSSN.Text="View SS#";
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					menuItemUnmaskSSN.Text="View SIN";
				}
				menuItemUnmaskSSN.Click+= new RoutedEventHandler(MenuItemUnmaskSSN_Click);
				contextMenu.Add(menuItemUnmaskSSN);
			}
			if(PrefC.GetBool(PrefName.PatientDOBMasked)) {
				menuItemUnmaskDOB=new MenuItem();
				menuItemUnmaskDOB.IsEnabled=false;
				menuItemUnmaskDOB.Visibility=Visibility.Collapsed;
				menuItemUnmaskDOB.Name="ViewDOB";
				menuItemUnmaskDOB.Text="View DOB";
				menuItemUnmaskDOB.Click+= new RoutedEventHandler(MenuItemUnmaskDOB_Click);
				contextMenu.Add(menuItemUnmaskDOB);
			}
			if(menuItemUnmaskSSN!=null || menuItemUnmaskDOB!=null){
				separator=contextMenu.AddSeparator();
			}
			gridMain.ContextMenu=contextMenu;
			FillSearchOption();
			SetGridCols();
			if(ODBuild.IsThinfinity()) {
				//Keyboard does not currently work with THINFINITY users. 
				butOnScreenKeyboard.Visible=false;
			}
			checkShowMerged.Visibility=Visibility.Hidden;
			if(ListPatNumsExplicit!=null && ListPatNumsExplicit.Count>0) {
				_ignoreRefresh=true;
				FillGrid(RefreshFromDb());
				return;
			}
			if(PatNumInitial!=0){
				Patient patientI=Patients.GetLim(PatNumInitial);
				textLName.Text=patientI.LName;
				_doLimitOnePage=false;
				_ignoreRefresh=true;
				FillGrid(RefreshFromDb());
				return;
			}
			//Always fillGrid if _isPreFilledLoad.  Since the first name and last name are pre-filled, the results should be minimal.
			//Also FillGrid if checkRefresh is checked and either PatientSelectSearchWithEmptyParams is set or there is a character in at least one textbox
			if(_isPreFillLoad || DoRefreshGrid()) {
				_ignoreRefresh=true;
				FillGrid(RefreshFromDb());
				_isPreFillLoad=false;
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
			gridMain.Columns.Clear();
			GridColumn col;
			_listDisplayFields=DisplayFields.GetForCategory(DisplayFieldCategory.PatientSelect);
			for(int i=0;i<_listDisplayFields.Count;i++){
				if(_listDisplayFields[i].Description==""){
					col=new GridColumn(_listDisplayFields[i].InternalName,_listDisplayFields[i].ColumnWidth);
				}
				else{
					col=new GridColumn(_listDisplayFields[i].Description,_listDisplayFields[i].ColumnWidth);
				}
				gridMain.Columns.Add(col);
			}
			gridMain.EndUpdate();
		}

//todo: Add this back if we later merge this with FormPatientSelect.
		/*
		///<summary>The pat must not be null.  Takes a partially built patient object and uses it to fill the search by textboxes.
		///Currently only implements FName, LName, and HmPhone.</summary>
		public void PreFillSearchBoxes(Patient patient) {
			_isPreFillLoad=true; //Set to true to stop FillGrid from being called as a result of textChanged events
			if(patient.LName != "") {
				textLName.Text=patient.LName;
			}
			if(patient.FName != "") {
				textFName.Text=patient.FName;
			}
			if(patient.HmPhone != "") {
				textPhone.Text=patient.HmPhone;
			}
		}*/

		///<summary>Returns the count of chars in all of the textboxes on the form.  For ValidPhone textboxes only digit chars are counted.</summary>
		private int TextBoxCharCount() {
			//only count digits in ValidPhone textboxes because we auto-format with special chars, ex: (xxx)xxx-xxxx
			int count=0;
			List<TextBox> listTextBoxes=GetAllChildControlsFlat().OfType<TextBox>().ToList();//ValidPhone is a TextBox
			for(int i=0;i<listTextBoxes.Count;i++){
				if(listTextBoxes[i].Name=="textPhone"){
					count+=listTextBoxes[i].Text.Count(x=>char.IsDigit(x));
					continue;
				}
				count+=listTextBoxes[i].Text.Length;
			}
			return count;
		}

		///<summary>Returns false if either checkRefresh is not checked or PatientSelectSearchWithEmptyParams is Yes or Unknown and all of the textboxes 
		///are empty. Otherwise returns true.</summary>
		private bool DoRefreshGrid() {
			return checkRefresh.Checked==true && (PIn.Enum<YN>(PrefC.GetInt(PrefName.PatientSelectSearchWithEmptyParams))!=YN.No || TextBoxCharCount()>0);
		}

		///<summary>Just prior to displaying the context menu, enable or disables the UnmaskSSN option</summary>
		private void contextMenu_Opened(object sender,EventArgs e) {
			//Check if we need to do anything
			if(menuItemUnmaskSSN==null && menuItemUnmaskDOB==null){
				return;
			}
			//Now we need to find out which one to show based on whatever column was clicked
			int idxColClick = gridMain.PointToCol(_pointLastClicked.X);
			int idxRowClick = gridMain.PointToRow(_pointLastClicked.Y);
			int idxSSN=_listDisplayFields.FindIndex(x => x.InternalName == "SSN");
			int idxDOB=_listDisplayFields.FindIndex(x => x.InternalName == "Birthdate");
			if(idxRowClick > -1 && idxColClick > -1 && idxSSN==idxColClick && menuItemUnmaskSSN!=null) {
				menuItemUnmaskSSN.Visibility=Visibility.Visible;
				separator.Visibility=Visibility.Visible;
				//If we can view the SSN and there is data
				if(Security.IsAuthorized(EnumPermType.PatientSSNView,true) && gridMain.ListGridRows[idxRowClick].Cells[idxColClick].Text!="") {
					menuItemUnmaskSSN.IsEnabled=true;
				}
				//Otherwise we don't have perms or there's no data
				else {
					menuItemUnmaskSSN.IsEnabled=false;
				}
				//Hide SSN if it's showing and we're showing DOB
				if(menuItemUnmaskDOB==null){
					return;
				}
				menuItemUnmaskDOB.Visibility=Visibility.Collapsed;
				return;
			}
			if(idxRowClick > -1 && idxColClick > -1 && idxDOB==idxColClick && menuItemUnmaskDOB!=null) {
				menuItemUnmaskDOB.Visibility=Visibility.Visible;
				separator.Visibility=Visibility.Visible;
				//If we can view the DOB and there is data
				if(Security.IsAuthorized(EnumPermType.PatientDOBView,true) && gridMain.ListGridRows[idxRowClick].Cells[idxColClick].Text!="") {
					menuItemUnmaskDOB.IsEnabled=true;
				}
				//Otherwise we don't have perms or there's no data
				else {
					menuItemUnmaskDOB.IsEnabled=false;
				}
				//Hide SSN if it's showing and we're showing DOB
				if(menuItemUnmaskSSN==null){
					return;
				}
				menuItemUnmaskSSN.Visibility=Visibility.Collapsed;
				return;
			}
			//Otherwise we're not in either column, so hide the items
			if(menuItemUnmaskSSN!=null){
				menuItemUnmaskSSN.Visibility=Visibility.Collapsed;
			}
			if(menuItemUnmaskDOB!=null){
				menuItemUnmaskDOB.Visibility=Visibility.Collapsed;
			}
			separator.Visibility=Visibility.Collapsed;
		}

		private void MenuItemUnmaskSSN_Click(object sender,EventArgs e) {
			//Preference and permissions check has already happened by this point.
			//Guaranteed to be clicking on a valid row & column.
			int idxColClick = gridMain.PointToCol(_pointLastClicked.X);
			int idxRowClick = gridMain.PointToRow(_pointLastClicked.Y);
			long patNumClicked=PIn.Long(_tablePats.Rows[idxRowClick]["PatNum"].ToString());
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
			SecurityLogs.MakeLogEntry(EnumPermType.PatientSSNView,patNumClicked,logtext);
		}

		private void MenuItemUnmaskDOB_Click(object sender,EventArgs e) {
			//Preference and permissions check has already happened by this point.
			//Guaranteed to be clicking on a valid row & column.
			int idxColClick = gridMain.PointToCol(_pointLastClicked.X);
			int idxRowClick = gridMain.PointToRow(_pointLastClicked.Y);
			long patNumClicked=PIn.Long(_tablePats.Rows[idxRowClick]["PatNum"].ToString());
			DateTime dateBirth=PIn.Date(_tablePats.Rows[idxRowClick]["Birthdate"].ToString());
			gridMain.BeginUpdate();
			gridMain.ListGridRows[idxRowClick].Cells[idxColClick].Text=Patients.DOBFormatHelper(dateBirth,false);
			gridMain.EndUpdate();
			string logtext="Date of birth unmasked in Patient Select";
			SecurityLogs.MakeLogEntry(EnumPermType.PatientDOBView,patNumClicked,logtext);
		}

		private void textbox_KeyDown(object sender,KeyEventArgs e) {
			if(e.Key==Key.Up || e.Key==Key.Down) {
				e.Handled=true;
			}
		}

		private void FrmPatientSelect_KeyDown(object sender,KeyEventArgs e) {
			if(e.Key==Key.Enter){
				if(_isGridRefreshed){
					butOK_Click(butOK,new EventArgs());
					return;
				}
				butSearch_Click(butSearch, new EventArgs());
			}
		}

		private void checkRefresh_Click(object sender,EventArgs e) {
			if(checkRefresh.Checked==true) {
				ComputerPrefs.LocalComputer.PatSelectSearchMode=SearchMode.RefreshWhileTyping;
				if(DoRefreshGrid()) {//only fill grid if PatientSelectSearchWithEmptyParams is true or there is something in at least one textbox
					_doLimitOnePage=true;
					FillGrid(RefreshFromDb());
				}
			}
			else{
				ComputerPrefs.LocalComputer.PatSelectSearchMode=SearchMode.UseSearchButton;
			}
			ComputerPrefs.Update(ComputerPrefs.LocalComputer);
		}

		private void checkShowArchived_Click(object sender,EventArgs e) {
			//We are only going to give the option to hide merged patients when Show Archived is checked.
			checkShowMerged.Visible=(checkShowArchived.Checked==true);
			_doLimitOnePage=true;
			FillGrid(RefreshFromDb());
		}

		private void butSearch_Click(object sender, EventArgs e) {
			_doLimitOnePage=true;
			_ignoreRefresh=true;
			FillGrid(RefreshFromDb());
		}

		private void butGetAll_Click(object sender,EventArgs e) {
			_doLimitOnePage=false;
			_ignoreRefresh=true;
			FillGrid(RefreshFromDb());
		}

		private object RefreshFromDb() {
			_isGridRefreshed=false;
			if(Dispatcher.Invoke(()=>checkRefresh.Checked)==false && !_ignoreRefresh){
				return _tablePats;
			}
			long billingType=0;
			if(comboBillingType.SelectedIndex!=0) {
				billingType=_listDefsBillingType[comboBillingType.SelectedIndex-1].DefNum;
			}
			long siteNum=0;
			if(!PrefC.GetBool(PrefName.EasyHidePublicHealth) && comboSite.SelectedIndex!=0) {
				siteNum=_listSites[comboSite.SelectedIndex-1].SiteNum;
			}
			string birthdate="";
			Dispatcher.Invoke(()=>birthdate=textBirthdate.Text);
			DateTime dateBirth=PIn.Date(birthdate); //this will frequently be minval.
			string clinicNums="";
			if(PrefC.HasClinicsEnabled) {
				if(comboClinic.IsAllSelected) {
					//When below preference is false, don't hide user restricted clinics from view. Just return clinicNums as an empty string.
					//If this preference is true, we DO hide user restricted clinics from view.
					if(PrefC.GetBool(PrefName.PatientSelectFilterRestrictedClinics) && (Security.CurUser.ClinicIsRestricted || (checkShowArchived.Checked==false))) {
						//only set clinicNums if user is unrestricted and showing hidden clinics, otherwise the search will show patients from all clinics
						clinicNums=string.Join(",",comboClinic.ListClinics
							//.Where(x => !x.IsHidden || checkShowArchived.Checked)//Only show hidden clinics if "Show Archived" is checked
							.Select(x => x.ClinicNum));
					}
				}
				else {
					Dispatcher.Invoke(()=>clinicNums=comboClinic.ClinicNumSelected.ToString());
					if(checkShowArchived.Checked==true) {
						for(int i=0;i<comboClinic.ListClinics.Count;i++) {
							if(comboClinic.ListClinics[i].IsHidden) {
								Dispatcher.Invoke(()=>clinicNums+=","+comboClinic.ListClinics[i].ClinicNum.ToString());
							}
						}
					}
				}
			}
			bool hasSpecialty=_listDisplayFields.Any(x => x.InternalName=="Specialty");
			bool hasNextLastVisit=_listDisplayFields.Any(x => x.InternalName.In("NextVisit","LastVisit"));
			DataTable tablePats=new DataTable();
			//Because hiding merged patients makes the query take longer, we will default to showing merged patients if the user has not had the 
			//opportunity to set this check box.
			bool doShowMerged=true;
			if(checkShowMerged.Visible) {
				//Only allow hiding merged if the Show Archived box is checked (and Show Merged is therefore visible).
				doShowMerged=checkShowMerged.Checked==true;
			}
			string LastName="";
			Dispatcher.Invoke(()=>LastName=PIn.String(textLName.Text));
			string FirstName="";
			Dispatcher.Invoke(()=>FirstName=PIn.String(textFName.Text));
			string Phone="";
			Dispatcher.Invoke(()=>Phone=PIn.String(textPhone.Text));
			string Address="";
			Dispatcher.Invoke(()=>Address=PIn.String(textAddress.Text));
			string City="";
			Dispatcher.Invoke(()=>City=PIn.String(textCity.Text));
			string State="";
			Dispatcher.Invoke(()=>State=PIn.String(textState.Text));
			string SSN="";
			Dispatcher.Invoke(()=>SSN=PIn.String(textSSN.Text));
			string PatNum="";
			Dispatcher.Invoke(()=>PatNum=PIn.String(textPatNum.Text));
			string ChartNum="";
			Dispatcher.Invoke(()=>ChartNum=PIn.String(textChartNumber.Text));
			string SubID="";
			Dispatcher.Invoke(()=>SubID=PIn.String(textSubscriberID.Text));
			string Email="";
			Dispatcher.Invoke(()=>Email=PIn.String(textEmail.Text));
			string Country="";
			Dispatcher.Invoke(()=>Country=PIn.String(textCountry.Text));
			string RegKey="";
			Dispatcher.Invoke(()=>RegKey=PIn.String(textRegKey.Text));
			string InvoiceNum="";
			Dispatcher.Invoke(()=>InvoiceNum=PIn.String(textInvoiceNumber.Text));
			PtTableSearchParams ptTableSearchParams=new PtTableSearchParams(_doLimitOnePage,LastName,FirstName,Phone,Address,
				checkShowInactive.Checked==false,City,State,SSN,PatNum,ChartNum,billingType,
				checkGuarantors.Checked==true,checkShowArchived.Checked==true,dateBirth,siteNum,SubID,Email,
				Country,RegKey,clinicNums,"",InvoiceNum,ListPatNumsExplicit,PatNumInitial,doShowMerged,
				hasSpecialty,hasNextLastVisit);
			tablePats=Patients.GetPtDataTable(ptTableSearchParams);
			return tablePats;
		}

		private void FillGrid(object objectData) {
			DataTable tablePats=(DataTable)objectData;
			//If the tablePats object is the same, don't bother refilling
			if(tablePats==_tablePats){
				return;
			}
			_tablePats=tablePats;
			labelMatchingRecords.Text=$"{_tablePats.Rows.Count} Records Displayed{(_doLimitOnePage? " Out of Many" : "")}";
			if(PatNumInitial!=0 && _doLimitOnePage) {
				//The InitialPatNum will be at the top, so resort the list alphabetically
				DataView dataView=_tablePats.DefaultView;
				dataView.Sort="LName,FName";
				_tablePats=dataView.ToTable();
			}
			_doLimitOnePage=true;
			gridMain.BeginUpdate();
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_tablePats.Rows.Count;i++) {
				row=new GridRow();
				#region New York Mental Health
				if(PrefC.GetBool(PrefName.OmhNy)) {
					string description=_tablePats.Rows[i]["RecallPastDue"].ToString();
					if(description=="ORANGE") {
						row.ColorText=Color.FromRgb(255,165,0);//Orange
					}
					else if(description=="BLACK") {
						row.ColorText=Color.FromRgb(0,0,0);//Black
					}
					else if(description=="PROPHY") {
						row.ColorText=Color.FromRgb(255,0,0);//Red
						row.Bold=true;
					}
					else if(description=="CHILD PROPHY") {
						row.ColorText=Color.FromRgb(255,0,0);//Red
						row.Bold=true;
					}
					else if(description=="ANNUAL EXAM") {
						row.ColorText=Color.FromRgb(0,128,0);//Green
						row.Bold=true;
					}
					else if(description=="6 MONTH EXAM") {
						row.ColorText=Color.FromRgb(0,0,255);//Blue
						row.Bold=true;
					}
					else if(description=="PANO X-RAY") {
						row.ColorText=Color.FromRgb(128,0,128);//Purple
						row.Bold=true;
					}
					else if(description=="PERIO SRP(UR)") {
						row.ColorText=Color.FromRgb(150,75,0);//Brown
						row.Bold=true;
					}
					else if(description=="PERIO SRP(UL)") {
						row.ColorText=Color.FromRgb(150,75,0);//Brown
						row.Bold=true;
					}
					else if(description=="PERIO SRP(LR)") {
						row.ColorText=Color.FromRgb(150,75,0);//Brown
						row.Bold=true;
					}
					else if(description=="PERIO SRP(LL)") {
						row.ColorText=Color.FromRgb(150,75,0);//Brown
						row.Bold=true;
					}
					else {
						row.ColorText=Color.FromRgb(0,0,0);//Black
					}
				}
				#endregion New York Mental Health
				for(int f=0;f<_listDisplayFields.Count;f++) {
					switch(_listDisplayFields[f].InternalName) {
						case "LastName":
							row.Cells.Add(_tablePats.Rows[i]["LName"].ToString());
							break;
						case "First Name":
							row.Cells.Add(_tablePats.Rows[i]["FName"].ToString());
							break;
						case "MI":
							row.Cells.Add(_tablePats.Rows[i]["MiddleI"].ToString());
							break;
						case "Pref Name":
							row.Cells.Add(_tablePats.Rows[i]["Preferred"].ToString());
							break;
						case "Age":
							row.Cells.Add(_tablePats.Rows[i]["age"].ToString());
							break;
						case "SSN":
							row.Cells.Add(Patients.SSNFormatHelper(_tablePats.Rows[i]["SSN"].ToString(),PrefC.GetBool(PrefName.PatientSSNMasked)));
							break;
						case "Hm Phone":
							row.Cells.Add(_tablePats.Rows[i]["HmPhone"].ToString());
							//Todo later for this WPF version:
							//if(Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
								//row.Cells[row.Cells.Count-1].ColorText=Color.FromRgb(0,0,255);//Blue color
								//row.Cells[row.Cells.Count-1].Underline=YN.Yes;
							//}
							break;
						case "Wk Phone":
							row.Cells.Add(_tablePats.Rows[i]["WkPhone"].ToString());
							//Todo later for this WPF version:
							//if(Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
								//row.Cells[row.Cells.Count-1].ColorText=Color.FromRgb(0,0,255);//Blue color
								//row.Cells[row.Cells.Count-1].Underline=YN.Yes;
							//}
							break;
						case "PatNum":
							row.Cells.Add(_tablePats.Rows[i]["PatNum"].ToString());
							break;
						case "ChartNum":
							row.Cells.Add(_tablePats.Rows[i]["ChartNumber"].ToString());
							break;
						case "Address":
							row.Cells.Add(_tablePats.Rows[i]["Address"].ToString());
							break;
						case "Status":
							row.Cells.Add(_tablePats.Rows[i]["PatStatus"].ToString());
							break;
						case "Bill Type":
							row.Cells.Add(_tablePats.Rows[i]["BillingType"].ToString());
							break;
						case "City":
							row.Cells.Add(_tablePats.Rows[i]["City"].ToString());
							break;
						case "State":
							row.Cells.Add(_tablePats.Rows[i]["State"].ToString());
							break;
						case "Pri Prov":
							row.Cells.Add(_tablePats.Rows[i]["PriProv"].ToString());
							break;
						case "Clinic":
							row.Cells.Add(_tablePats.Rows[i]["clinic"].ToString());
							break;
						case "Birthdate":
							row.Cells.Add(Patients.DOBFormatHelper(PIn.Date(_tablePats.Rows[i]["Birthdate"].ToString())
								,(PrefC.GetBool(PrefName.PatientDOBMasked) || !Security.IsAuthorized(EnumPermType.PatientDOBView,true)))
							);
							break;
						case "Site":
							row.Cells.Add(_tablePats.Rows[i]["site"].ToString());
							break;
						case "Email":
							row.Cells.Add(_tablePats.Rows[i]["Email"].ToString());
							break;
						case "Country":
							row.Cells.Add(_tablePats.Rows[i]["Country"].ToString());
							break;
						case "RegKey":
							row.Cells.Add(_tablePats.Rows[i]["RegKey"].ToString());
							break;
						case "OtherPhone": //will only be available if OD HQ
							row.Cells.Add(_tablePats.Rows[i]["OtherPhone"].ToString());
							break;
						case "Wireless Ph":
							row.Cells.Add(_tablePats.Rows[i]["WirelessPhone"].ToString());
							//Todo later for this WPF version:
							//if(Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
							//	row.Cells[row.Cells.Count-1].ColorText=Color.FromRgb(0,0,255);//Blue color
							//	row.Cells[row.Cells.Count-1].Underline=YN.Yes;
							//}
							break;
						case "Sec Prov":
							row.Cells.Add(_tablePats.Rows[i]["SecProv"].ToString());
							break;
						case "LastVisit":
							row.Cells.Add(_tablePats.Rows[i]["lastVisit"].ToString());
							break;
						case "NextVisit":
							row.Cells.Add(_tablePats.Rows[i]["nextVisit"].ToString());
							break;
						case "Invoice Number":
							row.Cells.Add(_tablePats.Rows[i]["StatementNum"].ToString());
							break;
						case "Specialty":
							row.Cells.Add(_tablePats.Rows[i]["Specialty"].ToString());
							break;
						case "Ward":
							row.Cells.Add(_tablePats.Rows[i]["Ward"].ToString());
							break;
						case "AdmitDate":
							row.Cells.Add(_tablePats.Rows[i]["AdmitDate"].ToString());
							break;
						case "DischargeDate":
							row.Cells.Add(_tablePats.Rows[i]["DischargeDate"].ToString());
							break;
					}
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.SetSelected(0,true);
			for(int i=0;i<_tablePats.Rows.Count;i++) {
				if(PIn.Long(_tablePats.Rows[i][0].ToString())==PatNumInitial) {
					gridMain.SetSelected(i,true);
					break;
				}
			}
			_ignoreRefresh=false;
			_isGridRefreshed=true;
		}

		private void gridMain_MouseDown(object sender,MouseEventArgs e) {
			_pointLastClicked=e.GetPosition(this);
		}

		private void gridMain_CellDoubleClick(object sender,GridClickEventArgs e) {
			PatSelected();
		}

		private void gridMain_CellClick(object sender,GridClickEventArgs e) {
			//GridCell gridCell=gridMain.ListGridRows[e.Row].Cells[e.Col];
			//Only grid cells with phone numbers are blue and underlined.
			//Todo later for this WPF version:
			//if(gridCell.ColorText==Color.FromRgb(0,0,255) //Blue color
			//	&& gridCell.Underline==YN.Yes && Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
			//	DentalTek.PlaceCall(gridCell.Text);
			//}
		}

		private void PatSelected(){
			long patNumSelected=PIn.Long(_tablePats.Rows[gridMain.GetSelectedIndex()]["PatNum"].ToString());
			if(PrefC.HasClinicsEnabled){
				long patClinicNum=PIn.Long(_tablePats.Rows[gridMain.GetSelectedIndex()]["ClinicNum"].ToString());
				List<long> listClinicNumsUser=comboClinic.ListClinics.Select(x => x.ClinicNum).ToList();
				if(!Security.CurUser.ClinicIsRestricted) {
					listClinicNumsUser.Add(0);
				}
				//If the user has security permissions to search all patients, or patient is assigned to one of the user's unrestricted clinics,
				//or patient has an appointment in one of the user's unrestricted clincis, 
				//allow them to select the patient
				if(Security.IsAuthorized(EnumPermType.UnrestrictedSearch,true) 
					|| listClinicNumsUser.Contains(patClinicNum)
					|| Appointments.GetAppointmentsForPat(patNumSelected).Select(x => x.ClinicNum).Any(x => listClinicNumsUser.Contains(x))) 
				{
					PatNumSelected=patNumSelected;
					IsDialogOK=true;
				}
				else {//Otherwise, present the error message explainign why they cannot select the patient.
					MsgBox.Show(this,"This patient is assigned to a clinic that you are not authorized for. Contact an Administrator to grant you access or to " +
						"create an appointment in your clinic to avoid patient duplication.");
				}
			}
			else {
				PatNumSelected=patNumSelected;
				IsDialogOK=true;
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
		}

		///<summary>Opens the On Screen Keyboard</summary>
		private void OpenOnScreenKeyBoard() {
			//Load the on screen keyboard process
			//For 64-Bit Systems, running the 32-bit On Screen Keyboard app requires disabling the Windows on Windows (WOW) path redirection, in order to access the System32 folder
			IntPtr intPtrWow64Value = IntPtr.Zero;
			if(Environment.Is64BitOperatingSystem){
				Wow64DisableWow64FsRedirection(ref intPtrWow64Value);
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
				Wow64RevertWow64FsRedirection(intPtrWow64Value);
			}
		}

		private void FrmPatientSelect_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSearch.IsAltKey(Key.S,e)) {
				butSearch_Click(this,new EventArgs());
			}
			if(butAddPatient.IsAltKey(Key.A,e)) {
				butAddPatient_Click(this,new EventArgs());
			}
			if(butOK.IsAltKey(Key.O,e)) {
				butOK_Click(this,new EventArgs());
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

		private void FrmPatientSelect_FormClosing(object sender,EventArgs e) {
			//Try to close the on screen keyboard if one was opened by this form.
			ODException.SwallowAnyException(CloseOnScreenKeyBoard);
		}

		private void butAddPatient_Click(object sender,EventArgs e) {

		}
		private void butAddMany_Click(object sender,EventArgs e) {

		}

	}
}

//2023-12-11-Jordan
//There is a small change in behavior from old FormPatientSelect.
//If _filterControlsAndAction.SetMinChars is set, and user backspaces to get below that value, 
//then they hit enter while they are waiting for a refresh delay,
//then it will select the top patient instead of searching again.
//It's a very minor edge case and doesn't seem to be a bug to me.