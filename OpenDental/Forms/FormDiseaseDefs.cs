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
		public List<DiseaseDef> ListSelectedDiseaseDefs;
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
		private List<long> _listDiseaseDefsNumsNotDeletable;
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
			if(listDiseaseDefNums!=null) {
				_listDiseaseDefNumsColored=listDiseaseDefNums;
			}
			else {
				_listDiseaseDefNumsColored=new List<long>();
			}
		}

		private void FormDiseaseDefs_Load(object sender, System.EventArgs e) {
			SetFilterControlsAndAction(() => FillGrid(),
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
			if(ListSelectedDiseaseDefs==null) {
				ListSelectedDiseaseDefs=new List<DiseaseDef>();
			}
			_listDiseaseDefsOld=_listDiseaseDefs.Select(x => x.Copy()).ToList();
			_listDiseaseDefsShowing=new List<DiseaseDef>();//fillGrid takes care of filling this.
			_listDiseaseDefsNumsNotDeletable=DiseaseDefs.ValidateDeleteList(_listDiseaseDefs.Select(x => x.DiseaseDefNum).ToList());
			FillGrid();
		}

		private void FillGrid() {
			//get the list of disease defs currently showing based on the search terms.
			_listDiseaseDefsShowing=FilterList(_listDiseaseDefs);
			//disable the up/down buttons if not all the problems are showing.
			if(_listDiseaseDefsShowing.Count!=_listDiseaseDefs.Count) {
				butUp.Enabled=false;
				butDown.Enabled=false;
				butAlphabetize.Enabled=false;
			}
			else {
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
			foreach(DiseaseDef defCur in _listDiseaseDefsShowing) {
				row=new GridRow();
				row.Cells.Add(defCur.ICD9Code);
				row.Cells.Add(defCur.Icd10Code);
				row.Cells.Add(defCur.SnomedCode);
				row.Cells.Add(defCur.DiseaseName);
				if(!IsSelectionMode) {
					row.Cells.Add(defCur.IsHidden ? "X" : "");
				}
				row.Tag=defCur;
				if(_listDiseaseDefNumsColored.Contains(defCur.DiseaseDefNum)) {
					row.ColorBackG=Color.LightCyan;
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		///<summary>Starts with the passed-in list of DiseaseDefs and cascades down to filter based on the search criteria without making db calls. 
		///Returns the filtered list. Normally, pass in the full list of DiseaseDefs.</summary>
		private List<DiseaseDef> FilterList(List<DiseaseDef> listCur) {
			string[] listTerms=new string[] {""}; //list of terms to filter on.
			List<DiseaseDef> listICD9=new List<DiseaseDef>();
			//ICD9
			listTerms=textICD9.Text.Split(new char[] { ' ' }); //each space is a new filter item. we could add other punctuation here if we wanted to. 
			if(listTerms.Length!=0) {
				foreach(DiseaseDef defCur in listCur) { //take the starting list, and for each element...
					if(listTerms.All(x => defCur.ICD9Code.ToLower().Contains(x.ToLower()))) { //if every filter item matches something in the disease's ICD9Code field..
						if(!listICD9.Contains(defCur)) {
							listICD9.Add(defCur); //add it to the result list.
						}
					}
				}
			}
			else {
				listICD9=listCur;
			}
			List<DiseaseDef> listICD10=new List<DiseaseDef>();
			//ICD10
			listTerms=textICD10.Text.Split(new char[] { ' ' });
			if(listTerms.Length!=0) {
				foreach(DiseaseDef defCur in listICD9) { //use the result list from above and further filter it based on the text entered into this search box.
					if(listTerms.All(x => defCur.Icd10Code.ToLower().Contains(x.ToLower()))) {
						if(!listICD10.Contains(defCur)) {
							listICD10.Add(defCur);
						}
					}
				}
			}
			else {
				listICD10=listICD9;
			}
			List<DiseaseDef> listSnoMed=new List<DiseaseDef>();
			//SNOMED
			listTerms=textSnoMed.Text.Split(new char[] { ' ' });
			if(listTerms.Length!=0) {
				foreach(DiseaseDef defCur in listICD10) {//use the result list from above and further filter it based on the text entered into this search box.
					if(listTerms.All(x => defCur.SnomedCode.ToLower().Contains(x.ToLower()))) {
						if(!listSnoMed.Contains(defCur)) {
							listSnoMed.Add(defCur);
						}
					}
				}
			}
			else {
				listSnoMed=listICD10;
			}
			List<DiseaseDef> listDesc=new List<DiseaseDef>();
			//DESCRIPTION
			listTerms=textDescript.Text.Split(new char[] { ' ' });
			if(listTerms.Length!=0) {
				foreach(DiseaseDef defCur in listSnoMed) {//use the result list from above and further filter it based on the text entered into this search box.
					if(listTerms.All(x => defCur.DiseaseName.ToLower().Contains(x.ToLower()))) {
						if(!listDesc.Contains(defCur)) {
							listDesc.Add(defCur);
						}
					}
				}
			}
			else {
				listDesc=listSnoMed;
			}
			//HIDDEN
			if(!checkShowHidden.Checked) {//use the result list from above and further filter it based on the whether it's hidden or not.
				listDesc=listDesc.Where(x => !x.IsHidden).ToList();
			}
			return listDesc; //return the completely filtered list.
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
			DiseaseDef selectedDiseaseDef = (DiseaseDef)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			#region Selection Mode. Close the Form
			if(IsSelectionMode) {//selection mode.
				if(!IsMultiSelect && Snomeds.GetByCode(selectedDiseaseDef.SnomedCode)==null) {
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
			if(_listDiseaseDefsNumsNotDeletable.Contains(selectedDiseaseDef.DiseaseDefNum)) {
				hasDelete=false;
			}
			//everything below this point is _not_ selection mode.  User guaranteed to have permission for ProblemEdit.
			using FormDiseaseDefEdit FormD=new FormDiseaseDefEdit(selectedDiseaseDef,hasDelete);
			FormD.ShowDialog();
			//Security log entry made inside that form.
			if(FormD.DialogResult!=DialogResult.OK) {
				return;
			}
			#endregion
			_listSecurityLogMsgs.Add(FormD.SecurityLogMsgText);
			if(FormD.DiseaseDefCur==null) {//User deleted the DiseaseDef.
				_listDiseaseDefs.Remove(selectedDiseaseDef);
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
			DiseaseDef def=new DiseaseDef() {
				ICD9Code="",
				Icd10Code="",
				SnomedCode="",
				ItemOrder=DiseaseDefs.GetCount()
			};
			using FormDiseaseDefEdit FormD=new FormDiseaseDefEdit(def,true);//also sets ItemOrder correctly if using alphabetical during the insert diseaseDef call.
			FormD.IsNew=true;
			FormD.ShowDialog();
			if(FormD.DialogResult!=DialogResult.OK) {
				return;
			}
			_listSecurityLogMsgs.Add(FormD.SecurityLogMsgText);
			//Need to invalidate cache for selection mode so that the new problem shows up.
			if(IsSelectionMode) {
				//In Middle Tier, the Sync in FormClosing() was not updating the PKs for the objects in each row tag for gridMain.  
				//This was the assumption of what would happen to retain selection when going back to the calling form (ex. FormMedical).
				//As a result, any new defs added here did not have a PK when being sent to the calling form via grid.SelectedTags and threw a UE.
				DiseaseDefs.Insert(FormD.DiseaseDefCur);
				DataValid.SetInvalid(InvalidType.Diseases);
				//No need to re-order as the ItemOrder given is already at the end of the list, and you can't change item order in selection mode.
				_listDiseaseDefsOld.Add(FormD.DiseaseDefCur);
				_listDiseaseDefs.Add(FormD.DiseaseDefCur);
				//Sync on FormClosing() will not happen; we don't need it since Adding a new DiseaseDef is the only change user can make in SelectionMode.
			}
			else {
				//Items are already in the right order in the DB, re-order in memory list to match
				_listDiseaseDefs.FindAll(x => x.ItemOrder>=def.ItemOrder).ForEach(x => x.ItemOrder++);
				_listDiseaseDefs.Add(def);
				_listDiseaseDefs.Sort(DiseaseDefs.SortItemOrder);
				_isChanged=true;
			}
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
			List<int> listSelectedIndexes=gridMain.SelectedIndices.ToList();
			if(listSelectedIndexes.First()==0) {
				return;
			}
			listSelectedIndexes.ForEach(x => _listDiseaseDefs.Reverse(x-1,2));
			for(int i=0;i<_listDiseaseDefs.Count;i++) {
				_listDiseaseDefs[i].ItemOrder=i;//change itemOrder to reflect order changes.
			}
			FillGrid();
			listSelectedIndexes.ForEach(x => gridMain.SetSelected(x-1,true));
			_isChanged=true;
		}

		///<summary>Only visible when !IsSelectionMode, and disabled if any filtering has been done via the search boxes. </summary>
		private void butDown_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid first.");
				return;
			}
			List<int> listSelectedIndexes=gridMain.SelectedIndices.ToList();
			if(listSelectedIndexes.Last()==_listDiseaseDefs.Count-1) {
				return;
			}
			listSelectedIndexes.Reverse<int>().ToList().ForEach(x => _listDiseaseDefs.Reverse(x,2));
			for(int i=0;i<_listDiseaseDefs.Count;i++) {
				_listDiseaseDefs[i].ItemOrder=i;//change itemOrder to reflect order changes.
			}
			FillGrid();
			listSelectedIndexes.ForEach(x => gridMain.SetSelected(x+1,true));
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
			if(IsSelectionMode && !IsMultiSelect) {
				if(Snomeds.GetByCode(_listDiseaseDefs[gridMain.GetSelectedIndex()].SnomedCode)==null) {
					MsgBox.Show(this,"You have selected a problem containing an invalid SNOMED CT.");
					return;
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormDiseaseDefs_FormClosing(object sender,FormClosingEventArgs e) {
			if(_isChanged) {
				DiseaseDefs.Sync(_listDiseaseDefs,_listDiseaseDefsOld);//Update if anything has changed, even in selection mode.
				//old securitylog pattern pasted from FormDiseaseDefEdit
				_listSecurityLogMsgs.FindAll(x => !string.IsNullOrEmpty(x)).ForEach(x => SecurityLogs.MakeLogEntry(Permissions.ProblemEdit,0,x));
				DataValid.SetInvalid(InvalidType.Diseases);//refreshes cache
			}
			ListSelectedDiseaseDefs.Clear();
			for(int i=0;i < gridMain.SelectedIndices.Count();i++) {
				ListSelectedDiseaseDefs.Add((DiseaseDef)(gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag));
			}
		}
	}
}



























