using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEServicesAutoMsgingAdvanced:FormODBase {
		private static Color COLOR_NOT_EDITABLE=Color.LightGray;
		private long _clinicNumCur;
		///<summary>A list of clinic prefs for ApptThankYouCalendarTitle. Includes clinic 0.</summary>
		private List<ClinicPref> _listClinicPrefsThankYouTitles;
		private List<Def> _listDefs;
		List<long> _listDontSendConfNums;
		List<long> _listDontChangeConfNums;
		List<long> _listDontSendReminderNums;
		List<long> _listExcludeThanksNums;
		List<long> _listExcludeArrivalSendNums;
		List<long> _listExcludeArrivalResponseNums;
		List<long> _listEclipboardExcludeNums;
		List<long> _listByodEnabledNums;
		///<summary>This list holds specific DefNums so that we know which ones can't be edited.</summary>
		List<long> _listSpecialDefNums;
		long _arrivalDefNum;

		public FormEServicesAutoMsgingAdvanced() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void AddToList(long defNum,List<long> listToAdd) {
			if(!listToAdd.Contains(defNum)) {
				listToAdd.Add(defNum);
			}
		}

		private void FormAutomatedConfirmationStatuses_Load(object sender,EventArgs e) {
			#region Confirmation Settings
			//Fill the comboboxes and radiobuttons
			checkEnableNoClinic.Checked=PrefC.GetBool(PrefName.ApptConfirmEnableForClinicZero);
			if(PrefC.HasClinicsEnabled) {//CLINICS
				groupAutomationStatuses.Text=Lan.g(this,"eConfirmation Settings")+" - "+Lan.g(this,"Affects all Clinics");
			}
			else {//NO CLINICS
				checkEnableNoClinic.Visible=false;
				groupAutomationStatuses.Text=Lan.g(this,"eConfirmation Settings");
			}
			comboClinic.SelectedClinicNum=0;//To keep consistent with the previous form, start on clinic 0.
			_clinicNumCur=comboClinic.SelectedClinicNum;
			comboStatusESent.Items.Clear();
			comboStatusEAccepted.Items.Clear();
			comboStatusEDeclined.Items.Clear();
			comboStatusEFailed.Items.Clear();
			comboStatusESent.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ApptConfirmed,true));
			comboStatusEAccepted.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ApptConfirmed,true));
			comboStatusEDeclined.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ApptConfirmed,true));
			comboStatusEFailed.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ApptConfirmed,true));
			long prefApptEConfirmStatusSent=PrefC.GetLong(PrefName.ApptEConfirmStatusSent);
			long prefApptEConfirmStatusAccepted=PrefC.GetLong(PrefName.ApptEConfirmStatusAccepted);
			long prefApptEConfirmStatusDeclined=PrefC.GetLong(PrefName.ApptEConfirmStatusDeclined);
			long prefApptEConfirmStatusSendFailed=PrefC.GetLong(PrefName.ApptEConfirmStatusSendFailed);
			//SENT
			if(prefApptEConfirmStatusSent>0) {
				//Selects combo box option if it exists, if it doesn't it sets the text of the combo box to the hidden one.
				comboStatusESent.SetSelectedDefNum(prefApptEConfirmStatusSent);
			}
			else {
				comboStatusESent.SelectedIndex=0;
			}
			//CONFIRMED
			if(prefApptEConfirmStatusAccepted>0) {
				comboStatusEAccepted.SetSelectedDefNum(prefApptEConfirmStatusAccepted);
			}
			else {
				comboStatusEAccepted.SelectedIndex=0;
			}
			//NOT CONFIRMED
			if(prefApptEConfirmStatusDeclined>0) {
				comboStatusEDeclined.SetSelectedDefNum(prefApptEConfirmStatusDeclined);
			}
			else {
				comboStatusEDeclined.SelectedIndex=0;
			}
			//Failed
			if(prefApptEConfirmStatusSendFailed>0) {
				comboStatusEFailed.SetSelectedDefNum(prefApptEConfirmStatusSendFailed);
			}
			else {
				comboStatusEFailed.SelectedIndex=0;
			}
			if(PrefC.GetBool(PrefName.ApptEConfirm2ClickConfirmation)) {
				radio2ClickConfirm.Checked=true;
			}
			else {
				radio1ClickConfirm.Checked=true;
			}
			#endregion
			#region Thank-You Settings
			bool allowEdit=Security.IsAuthorized(Permissions.EServicesSetup,true);
			groupAutomationStatuses.Enabled=allowEdit;
			//Do we want to hide editing this behind a pref that's on the previous form?
			textThankYouTitle.Enabled=PrefC.GetBool(PrefName.ApptThankYouAutoEnabled) && allowEdit;
			labelThankYouTitle.Enabled=PrefC.GetBool(PrefName.ApptThankYouAutoEnabled) && allowEdit;
			_listClinicPrefsThankYouTitles=ClinicPrefs.GetPrefAllClinics(PrefName.ApptThankYouCalendarTitle,includeDefault:true);
			//Will never be null since we start on clinic 0.
			textThankYouTitle.Text=_listClinicPrefsThankYouTitles.FirstOrDefault(x => x.ClinicNum==_clinicNumCur).ValueString;
			#endregion
			//Fill all of the defNum lists
			FillLists();
			//Add them to these lists just in case they didn't get added when the user switched a trigger in FormModuleSetup
			foreach(long defNum in _listSpecialDefNums) {
				AddToList(defNum,_listDontSendConfNums);
				AddToList(defNum,_listDontChangeConfNums);
				AddToList(defNum,_listDontSendReminderNums);
				AddToList(defNum,_listExcludeThanksNums);
				AddToList(defNum,_listExcludeArrivalSendNums);
				AddToList(defNum,_listExcludeArrivalResponseNums);
				AddToList(defNum,_listEclipboardExcludeNums);
			}
			AddToList(_arrivalDefNum,_listByodEnabledNums);
			FillGrid();
		}

		private void FillLists() {
			_listDefs=Defs.GetDefsForCategory(DefCat.ApptConfirmed,true);
			_listDontSendConfNums=GetDefNumsFromPref(PrefName.ApptConfirmExcludeESend);
			_listDontChangeConfNums=GetDefNumsFromPref(PrefName.ApptConfirmExcludeEConfirm);
			_listDontSendReminderNums=GetDefNumsFromPref(PrefName.ApptConfirmExcludeERemind);
			_listExcludeThanksNums=GetDefNumsFromPref(PrefName.ApptConfirmExcludeEThankYou);
			_listExcludeArrivalSendNums=GetDefNumsFromPref(PrefName.ApptConfirmExcludeArrivalSend);
			_listExcludeArrivalResponseNums=GetDefNumsFromPref(PrefName.ApptConfirmExcludeArrivalResponse);
			_listEclipboardExcludeNums=GetDefNumsFromPref(PrefName.ApptConfirmExcludeEclipboard);
			_listByodEnabledNums=GetDefNumsFromPref(PrefName.ApptConfirmByodEnabled);
			_arrivalDefNum=PrefC.GetLong(PrefName.AppointmentTimeArrivedTrigger);
			_listSpecialDefNums=new List<long>() { _arrivalDefNum, PrefC.GetLong(PrefName.AppointmentTimeDismissedTrigger),
				PrefC.GetLong(PrefName.AppointmentTimeSeatedTrigger) };
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Status"),100));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Color"),40));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Send eConfirmation"),80,HorizontalAlignment.Center) { Tag=_listDontSendConfNums });
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Change eConfirmation Status"),120,HorizontalAlignment.Center) { Tag=_listDontChangeConfNums });
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Send eReminder"),90,HorizontalAlignment.Center) { Tag=_listDontSendReminderNums});
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Send Automated Thank-You"),115,HorizontalAlignment.Center) { Tag=_listExcludeThanksNums});
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Send Arrival SMS"),110,HorizontalAlignment.Center) { Tag=_listExcludeArrivalSendNums});
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Send Arrival Response SMS"),110,HorizontalAlignment.Center) 
				{ Tag=_listExcludeArrivalResponseNums});
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Change on eClipboard Check-in"),130,HorizontalAlignment.Center) 
				{ Tag=_listEclipboardExcludeNums});
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Enable BYOD"),70,HorizontalAlignment.Center) { Tag=_listByodEnabledNums});
			gridMain.ListGridRows.Clear();
			foreach(Def def in _listDefs) {
				GridRow row=new GridRow();
				row.Cells.Add(def.ItemName);
				row.Cells.Add(new GridCell() { ColorBackG=def.ItemColor });
				row.Cells.Add(new GridCell(_listDontSendConfNums.Contains(def.DefNum)?"":"X") { 
					ColorBackG=_listSpecialDefNums.Contains(def.DefNum)?COLOR_NOT_EDITABLE:Color.Empty});
				row.Cells.Add(new GridCell(_listDontChangeConfNums.Contains(def.DefNum)?"":"X") { 
					ColorBackG=_listSpecialDefNums.Contains(def.DefNum)?COLOR_NOT_EDITABLE:Color.Empty});
				row.Cells.Add(new GridCell(_listDontSendReminderNums.Contains(def.DefNum)?"":"X") { 
					ColorBackG=_listSpecialDefNums.Contains(def.DefNum)?COLOR_NOT_EDITABLE:Color.Empty});
				row.Cells.Add(new GridCell(_listExcludeThanksNums.Contains(def.DefNum)?"":"X") { 
					ColorBackG=_listSpecialDefNums.Contains(def.DefNum)?COLOR_NOT_EDITABLE:Color.Empty});
				row.Cells.Add(new GridCell(_listExcludeArrivalSendNums.Contains(def.DefNum)?"":"X") { 
					ColorBackG=_listSpecialDefNums.Contains(def.DefNum)?COLOR_NOT_EDITABLE:Color.Empty});
				row.Cells.Add(new GridCell(_listExcludeArrivalResponseNums.Contains(def.DefNum)?"":"X") { 
					ColorBackG=_listSpecialDefNums.Contains(def.DefNum)?COLOR_NOT_EDITABLE:Color.Empty});
				row.Cells.Add(new GridCell(_listEclipboardExcludeNums.Contains(def.DefNum)?"":"X") { 
					ColorBackG=_listSpecialDefNums.Contains(def.DefNum)?COLOR_NOT_EDITABLE:Color.Empty});
				row.Cells.Add(new GridCell(_listByodEnabledNums.Contains(def.DefNum)?"X":"") { 
					ColorBackG=def.DefNum==_arrivalDefNum?COLOR_NOT_EDITABLE:Color.Empty});
				row.Tag=def;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private List<long> GetDefNumsFromPref(PrefName pref) {
			return PrefC.GetString(pref).Split(',').Select(x => PIn.Long(x)).ToList();
		}

		private void SaveToDb() {
			bool isPrefRefreshRequired=false;
			#region Confirmation Settings
			if(comboStatusESent.SelectedIndex!=-1) {
				isPrefRefreshRequired|=Prefs.UpdateLong(PrefName.ApptEConfirmStatusSent,Defs.GetDefsForCategory(DefCat.ApptConfirmed,true)[comboStatusESent.SelectedIndex].DefNum);
			}
			if(comboStatusEAccepted.SelectedIndex!=-1) {
				isPrefRefreshRequired|=Prefs.UpdateLong(PrefName.ApptEConfirmStatusAccepted,Defs.GetDefsForCategory(DefCat.ApptConfirmed,true)[comboStatusEAccepted.SelectedIndex].DefNum);
			}
			if(comboStatusEDeclined.SelectedIndex!=-1) {
				isPrefRefreshRequired|=Prefs.UpdateLong(PrefName.ApptEConfirmStatusDeclined,Defs.GetDefsForCategory(DefCat.ApptConfirmed,true)[comboStatusEDeclined.SelectedIndex].DefNum);
			}
			if(comboStatusEFailed.SelectedIndex!=-1) {
				isPrefRefreshRequired|=Prefs.UpdateLong(PrefName.ApptEConfirmStatusSendFailed,Defs.GetDefsForCategory(DefCat.ApptConfirmed,true)[comboStatusEFailed.SelectedIndex].DefNum);
			}
			isPrefRefreshRequired|=Prefs.UpdateBool(PrefName.ApptConfirmEnableForClinicZero,checkEnableNoClinic.Checked);
			isPrefRefreshRequired|=Prefs.UpdateBool(PrefName.ApptEConfirm2ClickConfirmation,radio2ClickConfirm.Checked);
			#endregion
			#region Thank-You Settings
			ParseThankYouTitle();
			bool isClinicPrefRefreshRequired=false;
			for(int i = 0;i<_listClinicPrefsThankYouTitles.Count;i++) {
				if(_listClinicPrefsThankYouTitles[i].ClinicNum==0) {
					isPrefRefreshRequired|=Prefs.UpdateString(_listClinicPrefsThankYouTitles[i].PrefName,_listClinicPrefsThankYouTitles[i].ValueString);
				}
				else {
					isClinicPrefRefreshRequired|=ClinicPrefs.Upsert(_listClinicPrefsThankYouTitles[i].PrefName,_listClinicPrefsThankYouTitles[i].ClinicNum,
						_listClinicPrefsThankYouTitles[i].ValueString);
				}
			}
			#endregion
			//Update the grid
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmExcludeESend,POut.String(string.Join(",",_listDontSendConfNums)));
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmExcludeEConfirm,POut.String(string.Join(",",_listDontChangeConfNums)));
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmExcludeERemind,POut.String(string.Join(",",_listDontSendReminderNums)));
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmExcludeEThankYou,POut.String(string.Join(",",_listExcludeThanksNums)));
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmExcludeArrivalSend,POut.String(string.Join(",",_listExcludeArrivalSendNums)));
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmExcludeArrivalResponse,POut.String(string.Join(",",_listExcludeArrivalResponseNums)));
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmByodEnabled,POut.String(string.Join(",",_listByodEnabledNums)));
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmExcludeEclipboard,POut.String(string.Join(",",_listEclipboardExcludeNums)));
			if(isClinicPrefRefreshRequired) {
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
			if(isPrefRefreshRequired) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}

		///<summary>Parses textThankYouTitle textbox into the appropriate ClinicPref in _listThankYouTitles.</summary>
		private void ParseThankYouTitle() {
			ClinicPref clinicPref=_listClinicPrefsThankYouTitles.FirstOrDefault(x => x.ClinicNum==_clinicNumCur);
			clinicPref.ValueString=textThankYouTitle.Text;
		}

		///<summary></summary>
		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			//Save the previous title.
			ParseThankYouTitle();
			_clinicNumCur=comboClinic.SelectedClinicNum;
			ClinicPref clinicPref=_listClinicPrefsThankYouTitles.FirstOrDefault(x => x.ClinicNum==_clinicNumCur);
			if(clinicPref is null) {
				//If the ClinicPref doesn't exist, add it.
				string defaultTitle=PrefC.GetString(PrefName.ApptThankYouCalendarTitle);
				clinicPref=new ClinicPref(_clinicNumCur,PrefName.ApptThankYouCalendarTitle,defaultTitle);
				_listClinicPrefsThankYouTitles.Add(clinicPref);
			}
			textThankYouTitle.Text=_listClinicPrefsThankYouTitles.FirstOrDefault(x => x.ClinicNum==_clinicNumCur).ValueString;
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {			
			if(!Security.IsAuthorized(Permissions.EServicesSetup)){
				return;
			}
			if(e.Col==0){
				return;//Don't change status
			}
			GridRow row=gridMain.ListGridRows[e.Row];
			GridCell cell=row.Cells[e.Col];
			if(cell.ColorBackG==COLOR_NOT_EDITABLE) {
				return;
			}
			Def defCur=(Def)row.Tag;
			if(e.Col==1) { //Color column
				colorDialog.Color=cell.ColorBackG;
				if(colorDialog.ShowDialog()!=DialogResult.OK) {
					return;
				}
				defCur.ItemColor=colorDialog.Color;
				Defs.Update(defCur);
			}
			else {
				List<long> listDefNums=(List<long>)gridMain.ListGridColumns[e.Col].Tag;
				UpdateListDefHelper(listDefNums,defCur,cell.Text);
			}
			FillGrid();
		}

		private void UpdateListDefHelper(List<long> listDefNums,Def defCur,string cellText) {
			if(listDefNums==_listByodEnabledNums) {
				//Literally only for list of byod def nums. The byod pref is saved as a positive. 
				if(cellText=="X") {
					listDefNums.Remove(defCur.DefNum);
				}
				else {
					if(!listDefNums.Contains(defCur.DefNum)) {
						listDefNums.Add(defCur.DefNum);
					}
				}
			}
			else {
				//All other prefs were originally negatives/excludes.
				if(cellText=="X") {
					if(!listDefNums.Contains(defCur.DefNum)) {
						listDefNums.Add(defCur.DefNum);
					}
				}
				else {
					listDefNums.Remove(defCur.DefNum);
				}
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butOK_Click(object sender,EventArgs e) {
			//check for duplicate selections:
			List<int> listConfButtonStatuses=new List<int>();
			listConfButtonStatuses.Add(comboStatusEAccepted.SelectedIndex);
			listConfButtonStatuses.Add(comboStatusESent.SelectedIndex);
			listConfButtonStatuses.Add(comboStatusEDeclined.SelectedIndex);
			listConfButtonStatuses.Add(comboStatusEFailed.SelectedIndex);
			listConfButtonStatuses=listConfButtonStatuses.FindAll(x => x!=-1);
			int countDistinct=listConfButtonStatuses.Distinct().Count();
			int countAll=listConfButtonStatuses.Count();
			if(countDistinct<countAll) { //checks if any are duplicates. If there are, show the message below.		
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"All eConfirmation appointment statuses should be different. Continue anyway?")) {
					return;
				}
			}
			SaveToDb();
			Defs.RefreshCache(); //for any changed colors
			DialogResult=DialogResult.OK;
		}
	}
}
