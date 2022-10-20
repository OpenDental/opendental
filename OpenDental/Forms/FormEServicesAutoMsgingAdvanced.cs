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
		private static Color _color=Color.LightGray;
		private long _clinicNumCur;
		///<summary>A list of clinic prefs for ApptThankYouCalendarTitle. Includes clinic 0.</summary>
		private List<ClinicPref> _listClinicPrefs;
		private List<Def> _listDefs;
		private List<long> _listDefNumsExcludeESend;
		private List<long> _listDefNumsExcludeEConf;
		private List<long> _listDefNumsExcludeERemind;
		private List<long> _listDefNumsExcludeEThanks;
		private List<long> _listDefNumsExcludeArrivalSend;
		private List<long> _listDefNumsExcludeArrivalResponse;
		private List<long> _listDefNumsExcludeEclipboard;
		private List<long> _listDefNumsByodEnabled;
		private List<long> _listDefNumsExcludeGeneralMessage;
		///<summary>This list holds specific DefNums so that we know which ones can't be edited.</summary>
		private List<long> _listDefNumsUneditable;
		private long _defNumTimeArrived;
		private bool _isAutoThankYouEnabled;

		public FormEServicesAutoMsgingAdvanced() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void AddToList(long defNum,List<long> listDefNums) {
			if(!listDefNums.Contains(defNum)) {
				listDefNums.Add(defNum);
			}
		}

		private void FormAutomatedConfirmationStatuses_Load(object sender,EventArgs e) {
			bool allowEdit=Security.IsAuthorized(Permissions.EServicesSetup,suppressMessage:true);
			#region Confirmation Settings
			//Fill the comboboxes and radiobuttons
			checkEnableNoClinic.Checked=PrefC.GetBool(PrefName.ApptConfirmEnableForClinicZero);
			checkEnableNoClinic.Enabled=allowEdit;
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
			long apptEConfirmStatusSentNum=PrefC.GetLong(PrefName.ApptEConfirmStatusSent);
			long apptEConfirmStatusAcceptedNum=PrefC.GetLong(PrefName.ApptEConfirmStatusAccepted);
			long apptEConfirmStatusDeclinedNum=PrefC.GetLong(PrefName.ApptEConfirmStatusDeclined);
			long apptEConfirmStatusSendFailedNum=PrefC.GetLong(PrefName.ApptEConfirmStatusSendFailed);
			//SENT
			if(apptEConfirmStatusSentNum>0) {
				//Selects combo box option if it exists, if it doesn't it sets the text of the combo box to the hidden one.
				comboStatusESent.SetSelectedDefNum(apptEConfirmStatusSentNum);
			}
			else {
				comboStatusESent.SelectedIndex=0;
			}
			//CONFIRMED
			if(apptEConfirmStatusAcceptedNum>0) {
				comboStatusEAccepted.SetSelectedDefNum(apptEConfirmStatusAcceptedNum);
			}
			else {
				comboStatusEAccepted.SelectedIndex=0;
			}
			//NOT CONFIRMED
			if(apptEConfirmStatusDeclinedNum>0) {
				comboStatusEDeclined.SetSelectedDefNum(apptEConfirmStatusDeclinedNum);
			}
			else {
				comboStatusEDeclined.SelectedIndex=0;
			}
			//Failed
			if(apptEConfirmStatusSendFailedNum>0) {
				comboStatusEFailed.SetSelectedDefNum(apptEConfirmStatusSendFailedNum);
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
			groupAutomationStatuses.Enabled=allowEdit;
			//Do we want to hide editing this behind a pref that's on the previous form?
			_isAutoThankYouEnabled=PrefC.GetBool(PrefName.ApptThankYouAutoEnabled) && allowEdit;
			_listClinicPrefs=ClinicPrefs.GetPrefAllClinics(PrefName.ApptThankYouCalendarTitle,includeDefault:true);
			_listClinicPrefs.AddRange(ClinicPrefs.GetPrefAllClinics(PrefName.ThankYouTitleUseDefault));
			FillThankYouTitleAndUseDefault();
			#endregion
			textPremedTemplate.Enabled=allowEdit;
			textPremedTemplate.Text=PrefC.GetString(PrefName.ApptReminderPremedTemplate);
			//Fill all of the defNum lists
			FillLists();
			//Add them to these lists just in case they didn't get added when the user switched a trigger in FormModuleSetup
			for(int i=0;i<_listDefNumsUneditable.Count;i++) {
				AddToList(_listDefNumsUneditable[i],_listDefNumsExcludeESend);
				AddToList(_listDefNumsUneditable[i],_listDefNumsExcludeEConf);
				AddToList(_listDefNumsUneditable[i],_listDefNumsExcludeERemind);
				AddToList(_listDefNumsUneditable[i],_listDefNumsExcludeEThanks);
				AddToList(_listDefNumsUneditable[i],_listDefNumsExcludeArrivalSend);
				AddToList(_listDefNumsUneditable[i],_listDefNumsExcludeArrivalResponse);
				AddToList(_listDefNumsUneditable[i],_listDefNumsExcludeEclipboard);
				if(_listDefNumsUneditable[i]!=PrefC.GetLong(PrefName.AppointmentTimeDismissedTrigger)) {
					AddToList(_listDefNumsUneditable[i],_listDefNumsExcludeGeneralMessage);
				}
			}
			AddToList(_defNumTimeArrived,_listDefNumsByodEnabled);
			FillGrid();
		}

		private void FillLists() {
			_listDefs=Defs.GetDefsForCategory(DefCat.ApptConfirmed,true);
			_listDefNumsExcludeESend=GetDefNumsFromPref(PrefName.ApptConfirmExcludeESend);
			_listDefNumsExcludeEConf=GetDefNumsFromPref(PrefName.ApptConfirmExcludeEConfirm);
			_listDefNumsExcludeERemind=GetDefNumsFromPref(PrefName.ApptConfirmExcludeERemind);
			_listDefNumsExcludeEThanks=GetDefNumsFromPref(PrefName.ApptConfirmExcludeEThankYou);
			_listDefNumsExcludeArrivalSend=GetDefNumsFromPref(PrefName.ApptConfirmExcludeArrivalSend);
			_listDefNumsExcludeArrivalResponse=GetDefNumsFromPref(PrefName.ApptConfirmExcludeArrivalResponse);
			_listDefNumsExcludeEclipboard=GetDefNumsFromPref(PrefName.ApptConfirmExcludeEclipboard);
			_listDefNumsByodEnabled=GetDefNumsFromPref(PrefName.ApptConfirmByodEnabled);
			_listDefNumsExcludeGeneralMessage=GetDefNumsFromPref(PrefName.ApptConfirmExcludeGeneralMessage);
			_defNumTimeArrived=PrefC.GetLong(PrefName.AppointmentTimeArrivedTrigger);
			_listDefNumsUneditable=new List<long>() { _defNumTimeArrived, PrefC.GetLong(PrefName.AppointmentTimeDismissedTrigger),
				PrefC.GetLong(PrefName.AppointmentTimeSeatedTrigger) };
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new GridColumn(Lan.g(this,"Status"),100));
			gridMain.Columns.Add(new GridColumn(Lan.g(this,"Color"),40));
			gridMain.Columns.Add(new GridColumn(Lan.g(this,"Send eConfirmation"),80,HorizontalAlignment.Center) { Tag=_listDefNumsExcludeESend });
			gridMain.Columns.Add(new GridColumn(Lan.g(this,"Change eConfirmation Status"),120,HorizontalAlignment.Center) { Tag=_listDefNumsExcludeEConf });
			gridMain.Columns.Add(new GridColumn(Lan.g(this,"Send eReminder"),90,HorizontalAlignment.Center) { Tag=_listDefNumsExcludeERemind});
			gridMain.Columns.Add(new GridColumn(Lan.g(this,"Send Automated Thank-You"),115,HorizontalAlignment.Center) { Tag=_listDefNumsExcludeEThanks});
			gridMain.Columns.Add(new GridColumn(Lan.g(this,"Send Arrival SMS"),90,HorizontalAlignment.Center) { Tag=_listDefNumsExcludeArrivalSend});
			gridMain.Columns.Add(new GridColumn(Lan.g(this,"Send General Message"),90,HorizontalAlignment.Center) { Tag=_listDefNumsExcludeGeneralMessage });
			gridMain.Columns.Add(new GridColumn(Lan.g(this,"Send Arrival Response SMS"),110,HorizontalAlignment.Center) 
				{ Tag=_listDefNumsExcludeArrivalResponse});
			gridMain.Columns.Add(new GridColumn(Lan.g(this,"Change on eClipboard Check-in"),130,HorizontalAlignment.Center) 
				{ Tag=_listDefNumsExcludeEclipboard});
			gridMain.Columns.Add(new GridColumn(Lan.g(this,"Enable BYOD"),90,HorizontalAlignment.Center) { Tag=_listDefNumsByodEnabled});
			gridMain.ListGridRows.Clear();
			for(int i=0;i<_listDefs.Count;i++) {
				GridRow row=new GridRow();
				GridCell gridCell;
				row.Cells.Add(_listDefs[i].ItemName);
				gridCell=new GridCell();
				gridCell.ColorBackG=_listDefs[i].ItemColor;
				row.Cells.Add(gridCell);
				gridCell=new GridCell(_listDefNumsExcludeESend.Contains(_listDefs[i].DefNum)?"":"X");
				gridCell.ColorBackG=_listDefNumsUneditable.Contains(_listDefs[i].DefNum)?_color:Color.Empty;
				row.Cells.Add(gridCell);
				gridCell=new GridCell(_listDefNumsExcludeEConf.Contains(_listDefs[i].DefNum)?"":"X");
				gridCell.ColorBackG=_listDefNumsUneditable.Contains(_listDefs[i].DefNum)?_color:Color.Empty;
				row.Cells.Add(gridCell);
				gridCell=new GridCell(_listDefNumsExcludeERemind.Contains(_listDefs[i].DefNum)?"":"X");
				gridCell.ColorBackG=_listDefNumsUneditable.Contains(_listDefs[i].DefNum)?_color:Color.Empty;
				row.Cells.Add(gridCell);
				gridCell=new GridCell(_listDefNumsExcludeEThanks.Contains(_listDefs[i].DefNum)?"":"X");
				gridCell.ColorBackG=_listDefNumsUneditable.Contains(_listDefs[i].DefNum)?_color:Color.Empty;
				row.Cells.Add(gridCell);
				gridCell=new GridCell(_listDefNumsExcludeArrivalSend.Contains(_listDefs[i].DefNum)?"":"X");
				gridCell.ColorBackG=_listDefNumsUneditable.Contains(_listDefs[i].DefNum)?_color:Color.Empty;
				row.Cells.Add(gridCell);
				gridCell=new GridCell(_listDefNumsExcludeGeneralMessage.Contains(_listDefs[i].DefNum)?"":"X");
				gridCell.ColorBackG=_listDefNumsUneditable.Except(new List<long> { PrefC.GetLong(PrefName.AppointmentTimeDismissedTrigger) }).Contains(_listDefs[i].DefNum)?_color:Color.Empty;
				row.Cells.Add(gridCell);
				gridCell=new GridCell(_listDefNumsExcludeArrivalResponse.Contains(_listDefs[i].DefNum)?"":"X");
				gridCell.ColorBackG=_listDefNumsUneditable.Contains(_listDefs[i].DefNum)?_color:Color.Empty;
				row.Cells.Add(gridCell);
				gridCell=new GridCell(_listDefNumsExcludeEclipboard.Contains(_listDefs[i].DefNum)?"":"X");
				gridCell.ColorBackG=_listDefNumsUneditable.Contains(_listDefs[i].DefNum)?_color:Color.Empty;
				row.Cells.Add(gridCell);
				gridCell=new GridCell(_listDefNumsByodEnabled.Contains(_listDefs[i].DefNum)?"X":"");
				gridCell.ColorBackG=_listDefs[i].DefNum==_defNumTimeArrived?_color:Color.Empty;
				row.Cells.Add(gridCell);
				row.Tag=_listDefs[i];
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
			if(!checkUseDefault.Checked || !checkUseDefault.Visible) {
				ParseThankYouTitle();
			}
			bool isClinicPrefRefreshRequired=false;
			for(int i = 0;i<_listClinicPrefs.Count;i++) {
				if(_listClinicPrefs[i].ClinicNum==0) {
					isPrefRefreshRequired|=Prefs.UpdateString(_listClinicPrefs[i].PrefName,_listClinicPrefs[i].ValueString);
				}
				else {
					isClinicPrefRefreshRequired|=ClinicPrefs.Upsert(_listClinicPrefs[i].PrefName,_listClinicPrefs[i].ClinicNum,
						_listClinicPrefs[i].ValueString);
				}
			}
			#endregion
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptReminderPremedTemplate,POut.String(textPremedTemplate.Text));
			//Update the grid
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmExcludeESend,POut.String(string.Join(",",_listDefNumsExcludeESend)));
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmExcludeEConfirm,POut.String(string.Join(",",_listDefNumsExcludeEConf)));
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmExcludeERemind,POut.String(string.Join(",",_listDefNumsExcludeERemind)));
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmExcludeEThankYou,POut.String(string.Join(",",_listDefNumsExcludeEThanks)));
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmExcludeArrivalSend,POut.String(string.Join(",",_listDefNumsExcludeArrivalSend)));
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmExcludeArrivalResponse,POut.String(string.Join(",",_listDefNumsExcludeArrivalResponse)));
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmByodEnabled,POut.String(string.Join(",",_listDefNumsByodEnabled)));
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmExcludeEclipboard,POut.String(string.Join(",",_listDefNumsExcludeEclipboard)));
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmExcludeGeneralMessage,POut.String(string.Join(",",_listDefNumsExcludeGeneralMessage)));
			if(isClinicPrefRefreshRequired) {
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
			if(isPrefRefreshRequired) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}

		///<summary>Parses textThankYouTitle textbox into the appropriate ClinicPref in _listThankYouTitles.</summary>
		private void ParseThankYouTitle() {
			ClinicPref clinicPref=GetClinicPrefFromList(PrefName.ApptThankYouCalendarTitle,_clinicNumCur);
			clinicPref.ValueString=textThankYouTitle.Text;
		}

		/// <summary>Tries to get the clinicPref for the clinicNum from the list of clinicPrefs. If the clinicPref doesn't exist, and prefName is either ApptThankYouCalendarTitle or ThankYouTitleUseDefault it is added to the list and returned. Can return null. </summary>
		private ClinicPref GetClinicPrefFromList(PrefName prefName, long clinicNum) {
			ClinicPref clinicPref=_listClinicPrefs.FirstOrDefault(x => x.ClinicNum==clinicNum && x.PrefName==prefName);
			if(clinicPref==null) {
				if(prefName==PrefName.ApptThankYouCalendarTitle) {
					string thankYouTitle=PrefC.GetString(PrefName.ApptThankYouCalendarTitle);
					clinicPref=new ClinicPref(clinicNum,prefName,thankYouTitle);
				}
				else if (prefName==PrefName.ThankYouTitleUseDefault) {
					bool thankYouUseDefault=PrefC.GetBool(PrefName.ThankYouTitleUseDefault);
					clinicPref=new ClinicPref(clinicNum,prefName,thankYouUseDefault);
				}
				_listClinicPrefs.Add(clinicPref);
			}
			return clinicPref;
		}

		/// <summary>Fills textThankYouTitle and checkUseDefault with their respective clinicPrefs from the DB. If no clinicPrefs are loaded, new clinicPrefs are created. Also enables and disables textThankYouTitle and shows and hides checkUseDefault. </summary>
		private void FillThankYouTitleAndUseDefault() {
			if(!_isAutoThankYouEnabled) {
				textThankYouTitle.Enabled=false;
				labelThankYouTitle.Enabled=false;
				return;
			}
			ClinicPref clinicPrefThankYouTitle=GetClinicPrefFromList(PrefName.ApptThankYouCalendarTitle,_clinicNumCur);
			if(_clinicNumCur==0) {
				checkUseDefault.Visible=false;
				textThankYouTitle.Enabled=true;
				textThankYouTitle.Text=clinicPrefThankYouTitle.ValueString;
				return;
			}
			ClinicPref clinicPrefThankYouUseDefault=GetClinicPrefFromList(PrefName.ThankYouTitleUseDefault,_clinicNumCur);
			checkUseDefault.Visible=true;
			bool useDefault=PIn.Bool(clinicPrefThankYouUseDefault.ValueString);
			checkUseDefault.Checked=useDefault;
			if(useDefault) {
				textThankYouTitle.Enabled=false;
				ClinicPref clinicPrefThankYouTitleHeadquarters=GetClinicPrefFromList(PrefName.ApptThankYouCalendarTitle,0);
				textThankYouTitle.Text=clinicPrefThankYouTitleHeadquarters.ValueString;
				return;
			}
			textThankYouTitle.Enabled=true;
			textThankYouTitle.Text=clinicPrefThankYouTitle.ValueString;
		}

		private void checkUseDefault_Click(object sender,EventArgs e) {
			if(checkUseDefault.Checked) {
				ParseThankYouTitle();
			}
			ClinicPref clinicPref=GetClinicPrefFromList(PrefName.ThankYouTitleUseDefault,_clinicNumCur);
			clinicPref.ValueString=POut.Bool(checkUseDefault.Checked);
			FillThankYouTitleAndUseDefault();
		}

		///<summary></summary>
		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			//Save the previous title if it is not default or we're on the default clinic
			if(!checkUseDefault.Checked || !checkUseDefault.Visible) {
				ParseThankYouTitle();
			}
			_clinicNumCur=comboClinic.SelectedClinicNum;
			FillThankYouTitleAndUseDefault();
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
			if(cell.ColorBackG==_color) {
				return;
			}
			if(!Security.IsAuthorized(Permissions.DefEdit)) {
				return;
			}
			Def def=(Def)row.Tag;
			if(e.Col==1) { //Color column
				colorDialog.Color=cell.ColorBackG;
				if(colorDialog.ShowDialog()!=DialogResult.OK) {
					return;
				}
				def.ItemColor=colorDialog.Color;
				DefL.Update(def);
			}
			else {
				List<long> listDefNums=(List<long>)gridMain.Columns[e.Col].Tag;
				UpdateListDefHelper(listDefNums,def,cell.Text);
			}
			FillGrid();
		}

		private void UpdateListDefHelper(List<long> listDefNums,Def def,string text) {
			if(listDefNums==_listDefNumsByodEnabled) {
				//Literally only for list of byod def nums. The byod pref is saved as a positive. 
				if(text=="X") {
					listDefNums.Remove(def.DefNum);
				}
				else {
					if(!listDefNums.Contains(def.DefNum)) {
						listDefNums.Add(def.DefNum);
						listDefNums=listDefNums.OrderBy(x => x).ToList();
					}
				}
			}
			else {
				//All other prefs were originally negatives/excludes.
				if(text=="X") {
					if(!listDefNums.Contains(def.DefNum)) {
						listDefNums.Add(def.DefNum);
						listDefNums=listDefNums.OrderBy(x => x).ToList();
					}
				}
				else {
					listDefNums.Remove(def.DefNum);
				}
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butOK_Click(object sender,EventArgs e) {
			//check for duplicate selections:
			List<int> listSelectedEConfirmationStatuses=new List<int>();
			listSelectedEConfirmationStatuses.Add(comboStatusEAccepted.SelectedIndex);
			listSelectedEConfirmationStatuses.Add(comboStatusESent.SelectedIndex);
			listSelectedEConfirmationStatuses.Add(comboStatusEDeclined.SelectedIndex);
			listSelectedEConfirmationStatuses.Add(comboStatusEFailed.SelectedIndex);
			listSelectedEConfirmationStatuses=listSelectedEConfirmationStatuses.FindAll(x => x!=-1);
			int countDistinct=listSelectedEConfirmationStatuses.Distinct().Count();
			int countAll=listSelectedEConfirmationStatuses.Count();
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
