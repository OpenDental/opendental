using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormPopupsForFam:FormODBase {
		public Patient PatientCur;
		private List<PopupEvent> _listPopupEvents;
		private List<Popup> _listPopups;
		private List<long> _listPatNumsFamily;

		public FormPopupsForFam(List<PopupEvent> listPopupEvents) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listPopupEvents=listPopupEvents;
		}

		private void FormPopupsForFam_Load(object sender,EventArgs e) {
			gridMain.AllowSortingByColumn=true;
			_listPatNumsFamily=Patients.GetAllFamilyPatNums(new List<long>() { PatientCur.PatNum });
			FillGrid();
		}

		private void FillGrid() {
			if(checkDeleted.Checked) {
				_listPopups=Popups.GetDeletedForFamily(PatientCur);
			}
			else {
				_listPopups=Popups.GetForFamily(PatientCur);
			}
			#region Automation PopUpThenDisable10Min
			//Joe - We are mimicing popups through a new AutomationAction PopUpThenDisable10Min.
			//We want to show it in this list to avoid confusion and phone calls.
			List<long> listAutomationNums=FormOpenDental.DicBlockedAutomations.Keys.ToList();
			Dictionary<long,DateTime> dictionaryPatDate;
			//First level dictionary key is an AutomationNum.
			//Value is Dictionary<long,DateTime>.
			Popup popup;
			Automation automation;
			List<Automation> listAutomations=Automations.GetDeepCopy();
			for(int i=0;i<listAutomationNums.Count();i++){
				dictionaryPatDate=FormOpenDental.DicBlockedAutomations[listAutomationNums[i]];//Gets all patnums for current automation.
				//Second level dictionary key is a patNum for current AutomationNum key.
				//Value is DateTime representing block until time.
				foreach(KeyValuePair<long,DateTime> keyValuePair in dictionaryPatDate) {
					if(!Patients.GetAllFamilyPatNums(new List<long>() { keyValuePair.Key }).Contains(PatientCur.PatNum)) {
						continue;//Not in the same family.
					}
					automation=listAutomations.FirstOrDefault(x => x.AutomationNum==listAutomationNums[i]);
					if(automation==null) {
						continue;
					}
					#region Dummy PopUp
					//Create dummy Popup.
					//Since we never insert these into the DB we just put them in our list that we fill the grid with.
					popup=new Popup() {
						PopupNum=listAutomationNums[i],
						PatNum=keyValuePair.Key,
						PopupLevel=EnumPopupLevel.Automation,
						DateTimeEntry=keyValuePair.Value.AddMinutes(-10),
						Description=automation.MessageContent
					};
					_listPopups.Add(popup);
					#endregion Dummy PopUp
				}
			}
			#endregion Automation PopUpThenDisable10Min
			#region Popup sorting
			//Superfamily Popups(These always popup for everyone)
			//Family Popups(For the family of the current patient)
			//Patient Popups(For the current patient)
			//Family Popups(For the other superfamily members)
			//Patient Popups(For the other superfamily members)
			//Automation popups
			//Disabled popups with same sorting as above.
			_listPopups=_listPopups
				.OrderByDescending(x => x.PopupLevel)
				.ThenByDescending(x => x.PatNum==PatientCur.PatNum)
				.ThenBy(x => x.PatNum).ToList();
			List<Popup> listPopupsPat=new List<Popup>();
			List<Popup> listPopupsHiddenPat=new List<Popup>();
			List<Popup> listPopupsAutomated=new List<Popup>();
			List<Popup> listPopupsDisabledPat=new List<Popup>();
			List<Popup> listPopupsDisabledHiddenPat=new List<Popup>();
			List<Popup> listPopupsDisabledAutomated=new List<Popup>();
			void addToList(Popup popup,List<Popup> listPopupsEnabled,List<Popup> listPopupsDisabled) {
				if(popup.DateTimeDisabled!=DateTime.MinValue && popup.DateTimeDisabled < DateTime.Now) {
					listPopupsDisabled.Add(popup);
				}
				else {
					listPopupsEnabled.Add(popup);
				}
			}
			for(int i = 0;i<_listPopups.Count();i++) {
				//Superfamily, selected patient popups, or family members family popups of selected patient.
				if(_listPopups[i].PopupLevel==EnumPopupLevel.Automation) {
					addToList(_listPopups[i],listPopupsAutomated,listPopupsDisabledAutomated);
				}
				else if(_listPopups[i].PatNum==PatientCur.PatNum || _listPopups[i].PopupLevel==EnumPopupLevel.SuperFamily 
					|| (_listPatNumsFamily.Contains(_listPopups[i].PatNum) && _listPopups[i].PopupLevel==EnumPopupLevel.Family))
				{
					addToList(_listPopups[i],listPopupsPat,listPopupsDisabledPat);
				}
				else { //Other Family/Patient popups for patients not in PatCur's family but still in Superfamily.
					addToList(_listPopups[i],listPopupsHiddenPat,listPopupsDisabledHiddenPat);
				}
			}
			_listPopups.Clear();
			_listPopups.AddRange(listPopupsPat);
			_listPopups.AddRange(listPopupsHiddenPat);
			_listPopups.AddRange(listPopupsAutomated);
			_listPopups.AddRange(listPopupsDisabledPat);
			_listPopups.AddRange(listPopupsDisabledHiddenPat);
			_listPopups.AddRange(listPopupsDisabledAutomated);
			#endregion Popup Sorting
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TablePopupsForFamily","Patient"),120);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePopupsForFamily","Level"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePopupsForFamily","Disabled"),60,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePopupsForFamily","Last Viewed"),80,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			if(checkDeleted.Checked) {
				col=new GridColumn(Lan.g("TablePopupsForFamily","Deleted"),60,HorizontalAlignment.Center);
				gridMain.Columns.Add(col);
			}
			col=new GridColumn(Lan.g("TablePopupsForFamily","Popup Message"),120);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			_listPopups.RemoveAll(x => x.PopupLevel==EnumPopupLevel.Automation && !listAutomations.Any(y => y.AutomationNum==x.PopupNum));
			for(int i=0;i<_listPopups.Count;i++) {
				row=new GridRow();
				row.Cells.Add(Patients.GetPat(_listPopups[i].PatNum).GetNameLF());
				if(_listPopups[i].PopupLevel==EnumPopupLevel.Automation) {
					automation=Automations.GetFirstOrDefault(x => x.AutomationNum==_listPopups[i].PopupNum);//Get by PK.
					//this should never happen because we remove any where the AutomationNum is not in Automations.Listt
					//if(autoCur==null) {
					//	continue;
					//}
					row.Cells.Add(Lan.g("enumEnumPopupLevel","Auto")+": "+Lan.g("enumEnumPopupLevel",automation.Autotrigger.ToString()));
					row.Cells.Add("");//Disabled column. Will never happen for automations.
					row.Cells.Add(_listPopups[i].DateTimeEntry.ToShortTimeString());
				}
				else {
					row.Cells.Add(Lan.g("enumEnumPopupLevel",_listPopups[i].PopupLevel.ToString()));
					row.Cells.Add((_listPopups[i].DateTimeDisabled!=DateTime.MinValue && _listPopups[i].DateTimeDisabled < DateTime.Now)?"X":"");
					PopupEvent popupEvent=_listPopupEvents.FirstOrDefault(x => x.PopupNum==_listPopups[i].PopupNum);
					if(popupEvent!=null && popupEvent.DateTimeLastViewed.Year>1880) {
						row.Cells.Add(popupEvent.DateTimeLastViewed.ToShortTimeString());
					}
					else {
						row.Cells.Add("");
					}
				}
				if(checkDeleted.Checked) {
					row.Cells.Add(_listPopups[i].IsArchived?"X":"");
				}
				row.Cells.Add(_listPopups[i].Description);
				row.Tag=_listPopups[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(((Popup)gridMain.ListGridRows[e.Row].Tag).PopupLevel==EnumPopupLevel.Automation){
				MsgBox.Show(this,"To edit automations go to Setup | Automation");
				return;
			}
			using FormPopupEdit formPopupEdit=new FormPopupEdit();
			formPopupEdit.PopupCur=(Popup)gridMain.ListGridRows[e.Row].Tag;
			formPopupEdit.ShowDialog();
			if(formPopupEdit.DialogResult==DialogResult.OK) {
				FillGrid();
			}
		}
		
		private void checkDeleted_CheckedChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormPopupEdit formPopupEdit=new FormPopupEdit();
			Popup popup=new Popup();
			popup.PatNum=PatientCur.PatNum;
			popup.PopupLevel=EnumPopupLevel.Patient;
			popup.IsNew=true;
			formPopupEdit.PopupCur=popup;
			formPopupEdit.ShowDialog();
			if(formPopupEdit.DialogResult==DialogResult.OK) {
				FillGrid();
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}
	}
}