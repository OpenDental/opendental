using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormAutomatedConfirmationStatuses:FormODBase {
		private static Color COLOR_NOT_EDITABLE=Color.LightGray;
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

		public FormAutomatedConfirmationStatuses() {
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
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Change on eClipboard Checkin"), 130, HorizontalAlignment.Center) 
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
			bool isPrefRefreshRequired=false;
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmExcludeESend,POut.String(string.Join(",",_listDontSendConfNums)));
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmExcludeEConfirm,POut.String(string.Join(",",_listDontChangeConfNums)));
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmExcludeERemind,POut.String(string.Join(",",_listDontSendReminderNums)));
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmExcludeEThankYou,POut.String(string.Join(",",_listExcludeThanksNums)));
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmExcludeArrivalSend,POut.String(string.Join(",",_listExcludeArrivalSendNums)));
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmExcludeArrivalResponse,POut.String(string.Join(",",_listExcludeArrivalResponseNums)));
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmByodEnabled,POut.String(string.Join(",",_listByodEnabledNums)));
			isPrefRefreshRequired|=Prefs.UpdateString(PrefName.ApptConfirmExcludeEclipboard,POut.String(string.Join(",",_listEclipboardExcludeNums)));
			if(isPrefRefreshRequired) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}



	}
}
