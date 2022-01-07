using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental{
	/// <summary>
	/// </summary>
	public partial class FormDiseaseDefs:FormODBase {
		///<summary>Set to true when user is using this to select a disease def. Currently used when adding Alerts to Rx.</summary>
		public bool IsSelectionMode;
		///<summary>Set to true when user is using FormMedical to allow multiple problems to be selected at once.</summary>
		public bool IsMultiSelect;
		///<summary>On FormClosing, if IsSelectionMode, this will contain the selected DiseaseDefs.  Unless IsMultiSelect is true, it will only contain one.</summary>
		public List<DiseaseDef> ListDiseaseDefsSelected;
		private bool _isChanged;
		///<summary>A complete list of disease defs including hidden.  Only used when not in selection mode (item orders can change).  
		///It's main purpose is to keep track of the item order for the life of the window 
		///so that we do not have to make unnecessary update calls to the database every time the up and down buttons are clicked.</summary>
		private List<DiseaseDef> _listDiseaseDefs;
		///<summary>Stale deep copy of _listDiseaseDefs to use with sync.</summary>
		private List<DiseaseDef> _listDiseaseDefsOld;
		///<summary>List of diseaseDefs currently available in the grid after filtering.</summary>
		private List<DiseaseDef> _listDiseaseDefsShowing;
		///<summary>List of all the DiseaseDefNums that cannot be deleted because they could be in use by other tables.</summary>
		private List<long> _listDiseaseDefNumsNotDeletable;
		///<summary>List of messages returned by FormDiseaseDefEdit for creating log messages after syncing.  All messages in this list use ProblemEdit
		///permission.</summary>
		private List<string> _listSecurityLogMsgs;
		///<summary>A copy of ListSelectedDefs taken when the form loads. This is the list of diseases that are colored in the grid,
		///and should not change once the form is loaded.</summary>
		private List<long> _listDiseaseDefNumsColored;

		///<summary> On initialization, you may pass in a list of diseaseDefNums that you want colored in this list. 
		///Currently, this is used from FormMedical to show the user which active problems are assigned to the current patient.</summary>
		public FormDiseaseDefs(List<long> listDiseaseDefNums=null)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listDiseaseDefNumsColored=listDiseaseDefNums;
			if(listDiseaseDefNums==null) {
				_listDiseaseDefNumsColored=new List<long>();
			}
		}

		private void FormDiseaseDefs_Load(object sender,EventArgs e) {
			Action actionFillGrid=() => FillGrid();
			SetFilterControlsAndAction(actionFillGrid,
				(int)TimeSpan.FromSeconds(0.5).TotalMilliseconds,
				textICD9,textICD10,textDescript,textSnoMed);
			if(DiseaseDefs.FixItemOrders()) {
				DataValid.SetInvalid(InvalidType.Diseases);
			}
			_listSecurityLogMsgs=new List<string>();
			if(IsSelectionMode){
				//Hide and change UI buttons
				butClose.Text=Lan.g(this,"Cancel");
				butDown.Visible=false;
				butUp.Visible=false;
				labelAlphabetize.Visible=false;
				butAlphabetize.Visible=false;
				checkShowHidden.Visible=false;
				if(IsMultiSelect) {
					gridMain.SelectionMode=GridSelectionMode.MultiExtended;
				}
				//show only non-hidden items.
				_listDiseaseDefs=DiseaseDefs.GetDeepCopy(true);
			}
			else{
				//change UI
				butOK.Visible=false;
				//show all items, including hidden.
				_listDiseaseDefs=DiseaseDefs.GetDeepCopy();
			}
			//If the user has passed in DiseaseDefs, those should be highlighted. Otherwise, initialize a new List<DiseaseDef>.
			if(ListDiseaseDefsSelected==null) {
				ListDiseaseDefsSelected=new List<DiseaseDef>();
			}
			_listDiseaseDefsOld=_listDiseaseDefs.Select(x => x.Copy()).ToList();
			_listDiseaseDefsShowing=new List<DiseaseDef>();//fillGrid takes care of filling this.
			_listDiseaseDefNumsNotDeletable=DiseaseDefs.ValidateDeleteList(_listDiseaseDefs.Select(x => x.DiseaseDefNum).ToList());
			FillGrid();
		}

		private void FillGrid() {
			//get the list of disease defs currently showing based on the search terms.
			_listDiseaseDefsShowing=FilterList();
			butUp.Enabled=false;
			butDown.Enabled=false;
			butAlphabetize.Enabled=false;
			//enable the up/down buttons if all the problems are showing.
			if(_listDiseaseDefsShowing.Count==_listDiseaseDefs.Count) {
				butUp.Enabled=true;
				butDown.Enabled=true;
				butAlphabetize.Enabled=true;
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"ICD-9"),50);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"ICD-10"),50);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"SNOMED CT"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),250);
			gridMain.ListGridColumns.Add(col);
			if(!IsSelectionMode) {
				col=new GridColumn(Lan.g(this,"Hidden"),50,HorizontalAlignment.Center);
				gridMain.ListGridColumns.Add(col);
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listDiseaseDefsShowing.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listDiseaseDefsShowing[i].ICD9Code);
				row.Cells.Add(_listDiseaseDefsShowing[i].Icd10Code);
				row.Cells.Add(_listDiseaseDefsShowing[i].SnomedCode);
				row.Cells.Add(_listDiseaseDefsShowing[i].DiseaseName);
				if(!IsSelectionMode) {
					row.Cells.Add(_listDiseaseDefsShowing[i].IsHidden ? "X" : "");
				}
				row.Tag=_listDiseaseDefsShowing[i];
				if(_listDiseaseDefNumsColored.Contains(_listDiseaseDefsShowing[i].DiseaseDefNum)) {
					row.ColorBackG=Color.LightCyan;
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		///<summary>Returns a list of DiseaseDefs from the ones passed in that match the all criteria from filterText based off of the search type.
		///Passing in an empty string as filterText will return the list of DiseaseDefs that was passed in.</summary>
		private List<DiseaseDef> FilterListByTerms(List<DiseaseDef> listDiseaseDefs,string filterText,SearchTypes searchType) {
			List<DiseaseDef> listDiseaseDefsFiltered=new List<DiseaseDef>();
			if(string.IsNullOrEmpty(filterText)) {
				listDiseaseDefsFiltered=listDiseaseDefs;//Nothing to filter on, return the current list.
			}
			else {
				string[] stringArrayFilterTerms=filterText.Split(new char[] { ' ' });//Each space is a new filter item. we could add other punctuation here if we wanted to. 
				for(int i=0;i<listDiseaseDefs.Count;i++) {
					//Get the correct value to filter against.
					string diseaseDefValue=listDiseaseDefs[i].DiseaseName;
					if(searchType==SearchTypes.ICD9) {
						diseaseDefValue=listDiseaseDefs[i].ICD9Code;
					}
					else if(searchType==SearchTypes.ICD10) {
						diseaseDefValue=listDiseaseDefs[i].Icd10Code;
					}
					else if(searchType==SearchTypes.SNOMED) {
						diseaseDefValue=listDiseaseDefs[i].SnomedCode;
					}
					bool doFiltersMatch=stringArrayFilterTerms.All(x => diseaseDefValue.ToLower().Contains(x.ToLower()));
					if(doFiltersMatch && !listDiseaseDefsFiltered.Contains(listDiseaseDefs[i])) {//Every filter item matches something in the disease's ICD9Code field.
						listDiseaseDefsFiltered.Add(listDiseaseDefs[i]);
					}
				}
			}
			return listDiseaseDefsFiltered;
		}

		///<summary>Returns a list of DiseaseDefs that match all of the search filters in the UI.</summary>
		private List<DiseaseDef> FilterList() {
			List<DiseaseDef> listDiseaseDefsFiltered=new List<DiseaseDef>();
			listDiseaseDefsFiltered=FilterListByTerms(_listDiseaseDefs,textICD9.Text,SearchTypes.ICD9);
			listDiseaseDefsFiltered=FilterListByTerms(listDiseaseDefsFiltered,textICD10.Text,SearchTypes.ICD10);
			listDiseaseDefsFiltered=FilterListByTerms(listDiseaseDefsFiltered,textSnoMed.Text,SearchTypes.SNOMED);
			listDiseaseDefsFiltered=FilterListByTerms(listDiseaseDefsFiltered,textDescript.Text,SearchTypes.DiseaseName);
			//HIDDEN
			if(!checkShowHidden.Checked) {//use the result list from above and further filter it based on the whether it's hidden or not.
				listDiseaseDefsFiltered=listDiseaseDefsFiltered.FindAll(x => !x.IsHidden);
			}
			return listDiseaseDefsFiltered; //return the completely filtered list.
		}

		///<summary>If IsSelectionMode, doubleclicking closes the form and returns the selected diseasedef.
		///If !IsSelectionMode, doubleclicking opens FormDiseaseDefEdit and allows the user to edit or delete the selected diseasedef.
		///Either way, validation always occurs first.</summary>
		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			#region Validation
			if(!IsSelectionMode && !Security.IsAuthorized(Permissions.ProblemEdit)) {//trying to double click to edit, but no permission.
				return;
			}
			if(gridMain.SelectedIndices.Length==0) {
				return;
			}
			#endregion
			DiseaseDef diseaseDef = (DiseaseDef)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			#region Selection Mode. Close the Form
			if(IsSelectionMode) {//selection mode.
				if(!IsMultiSelect && Snomeds.GetByCode(diseaseDef.SnomedCode)==null) {
					MsgBox.Show(this,"You have selected a problem with an unofficial SNOMED CT code.  Please correct the problem definition by going to "
						+"Lists | Problems and choosing an official code from the SNOMED CT list.");
					return;
				}
				DialogResult=DialogResult.OK;//FormClosing takes care of filling ListSelectedDiseaseDefs.
				return;
			}
			#endregion
			#region Not Selection Mode. Open FormDiseaseDefEdit
			//not selection mode. double-click to edit.
			bool hasDelete=true;
			if(_listDiseaseDefNumsNotDeletable.Contains(diseaseDef.DiseaseDefNum)) {
				hasDelete=false;
			}
			//everything below this point is _not_ selection mode.  User guaranteed to have permission for ProblemEdit.
			using FormDiseaseDefEdit formDiseaseDefEdit=new FormDiseaseDefEdit(diseaseDef,hasDelete);
			formDiseaseDefEdit.ShowDialog();
			//Security log entry made inside that form.
			if(formDiseaseDefEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			#endregion
			_listSecurityLogMsgs.Add(formDiseaseDefEdit.SecurityLogMsgText);
			if(formDiseaseDefEdit.DiseaseDefCur==null) {//User deleted the DiseaseDef.
				_listDiseaseDefs.Remove(diseaseDef);
			}
			_isChanged=true;
			FillGrid();
		}

		///<summary>Adds a new disease. New diseases get blank (not null) fields for ICD9, ICD10, and SnoMedCodes 
		///if they are not specified from FormDiseaseDefEdit so that we can do string searches on them.</summary>
		private void butAdd_Click(object sender,System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.ProblemEdit)) {
				return;
			}
			//initialise the new DiseaseDef with blank fields instead of null so we can filter on them.
			DiseaseDef diseaseDef=new DiseaseDef() {
				ICD9Code="",
				Icd10Code="",
				SnomedCode="",
				ItemOrder=DiseaseDefs.GetCount()
			};
			using FormDiseaseDefEdit formDiseaseDefEdit=new FormDiseaseDefEdit(diseaseDef,true);//also sets ItemOrder correctly if using alphabetical during the insert diseaseDef call.
			formDiseaseDefEdit.IsNew=true;
			formDiseaseDefEdit.ShowDialog();
			if(formDiseaseDefEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			_listSecurityLogMsgs.Add(formDiseaseDefEdit.SecurityLogMsgText);
			if(!IsSelectionMode) {
				//Items are already in the right order in the DB, re-order in memory list to match
				_listDiseaseDefs.FindAll(x => x.ItemOrder>=diseaseDef.ItemOrder).ForEach(x => x.ItemOrder++);
				_listDiseaseDefs.Add(diseaseDef);
				_listDiseaseDefs.Sort(DiseaseDefs.SortItemOrder);
				_isChanged=true;
				FillGrid();
				return;
			}
			//Need to invalidate cache for selection mode so that the new problem shows up.
			//In Middle Tier, the Sync in FormClosing() was not updating the PKs for the objects in each row tag for gridMain.  
			//This was the assumption of what would happen to retain selection when going back to the calling form (ex. FormMedical).
			//As a result, any new defs added here did not have a PK when being sent to the calling form via grid.SelectedTags and threw a UE.
			DiseaseDefs.Insert(formDiseaseDefEdit.DiseaseDefCur);
			DataValid.SetInvalid(InvalidType.Diseases);
			//No need to re-order as the ItemOrder given is already at the end of the list, and you can't change item order in selection mode.
			_listDiseaseDefsOld.Add(formDiseaseDefEdit.DiseaseDefCur);
			_listDiseaseDefs.Add(formDiseaseDefEdit.DiseaseDefCur);
			//Sync on FormClosing() will not happen; we don't need it since Adding a new DiseaseDef is the only change user can make in SelectionMode.
			FillGrid();
		}

		///<summary>Only visible when !IsSelectionMode, and disabled if any filtering has been done via the search boxes. 
		///Resets ALL the DiseaseDefs' ItemOrders to be in alphabetical order. Not reversible once done.</summary>
		private void butAlphabetize_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Problems will be ordered alphabetically by description.  This cannot be undone.  Continue?")) {
				return;
			}
			_listDiseaseDefs.Sort(DiseaseDefs.SortAlphabetically);
			for(int i=0;i<_listDiseaseDefs.Count;i++) {
				_listDiseaseDefs[i].ItemOrder=i;
			}
			_isChanged=true;
			FillGrid();
		}

		///<summary>Only visible when !IsSelectionMode, and disabled if any filtering has been done via the search boxes. </summary>
		private void butUp_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid first.");
				return;
			}
			List<int> listSelectedIndices=gridMain.SelectedIndices.ToList();
			if(listSelectedIndices.First()==0) {
				return;
			}
			listSelectedIndices.ForEach(x => _listDiseaseDefs.Reverse(x-1,2));
			for(int i=0;i<_listDiseaseDefs.Count;i++) {
				_listDiseaseDefs[i].ItemOrder=i;//change itemOrder to reflect order changes.
			}
			FillGrid();
			listSelectedIndices.ForEach(x => gridMain.SetSelected(x-1,true));
			_isChanged=true;
		}

		///<summary>Only visible when !IsSelectionMode, and disabled if any filtering has been done via the search boxes. </summary>
		private void butDown_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid first.");
				return;
			}
			List<int> listSelectedIndices=gridMain.SelectedIndices.ToList();
			if(listSelectedIndices.Last()==_listDiseaseDefs.Count-1) {
				return;
			}
			listSelectedIndices.Reverse<int>().ToList().ForEach(x => _listDiseaseDefs.Reverse(x,2));
			for(int i=0;i<_listDiseaseDefs.Count;i++) {
				_listDiseaseDefs[i].ItemOrder=i;//change itemOrder to reflect order changes.
			}
			FillGrid();
			listSelectedIndices.ForEach(x => gridMain.SetSelected(x+1,true));
			_isChanged=true;
		}

		private void checkShowHidden_CheckedChanged(object sender,EventArgs e) {
			FillGrid();
		}

		///<summary>Only visible when using Selection Mode. Most of the actual logic is done on FormClosing.</summary>
		private void butOK_Click(object sender,EventArgs e) {
			//not even visible unless IsSelectionMode
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			bool isInvalidSnomedCode=Snomeds.GetByCode(_listDiseaseDefs[gridMain.GetSelectedIndex()].SnomedCode)==null;
			if(IsSelectionMode && !IsMultiSelect && isInvalidSnomedCode) {
				MsgBox.Show(this,"You have selected a problem containing an invalid SNOMED CT.");
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormDiseaseDefs_FormClosing(object sender,FormClosingEventArgs e) {
			if(_isChanged) {
				DiseaseDefs.Sync(_listDiseaseDefs,_listDiseaseDefsOld);//Update if anything has changed, even in selection mode.
				//old securitylog pattern pasted from FormDiseaseDefEdit
				_listSecurityLogMsgs.FindAll(x => !string.IsNullOrEmpty(x)).ForEach(x => SecurityLogs.MakeLogEntry(Permissions.ProblemEdit,0,x));
				DataValid.SetInvalid(InvalidType.Diseases);//refreshes cache
			}
			ListDiseaseDefsSelected.Clear();
			for(int i=0;i<gridMain.SelectedIndices.Count();i++) {
				ListDiseaseDefsSelected.Add((DiseaseDef)(gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag));
			}
		}

		///<summary>Each search text box in the UI has a corresponding value in this enumeration.</summary>
		private enum SearchTypes {
			DiseaseName,
			ICD9,
			ICD10,
			SNOMED,
		}
	}
}



























