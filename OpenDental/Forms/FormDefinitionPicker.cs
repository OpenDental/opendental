using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormDefinitionPicker:FormODBase {
		///<summary>The passed-in list of Defs.</summary>
		private List<Def> _listDefInitial=new List<Def>();
		///<summary>The selected defs at any given point. Initially, this is the same as _listDefInitial.</summary>
		public List<Def> ListSelectedDefs=new List<Def>();
		///<summary>List of all defs of the passed-in category type.</summary>
		private List<Def> _listDefs;
		///<summary>Set to true to allow showing hidden.</summary>
		public bool HasShowHiddenOption;
		///<summary>Allows selecting multiple.  If false, ListSelectedDefs will only have one result.</summary>
		public bool IsMultiSelectionMode;

		///<summary>Passing in a list of Defs will make those defs pre-selected and highlighted when this window loads.  For AutoNoteCategories this is
		///used to select a parent category for the current category, but we want to prevent an infinite loop so defNumCur is used to exclude the
		///currently selected def and any direct-line descendants from being selected as the category's parent.</summary>
		public FormDefinitionPicker(DefCat cat,List<Def> listDefs=null,long defNumCur=0) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			if(listDefs!=null) {
				ListSelectedDefs=listDefs; //initially, selected defs and list defs are the same. However, ListSelectedDefs changes while _listDefInitial doesn't.
				_listDefInitial=new List<Def>(listDefs);
			}
			gridMain.Title=cat.ToString();
			FillListDefs(cat,defNumCur);
		}

		private void FormDefinitionPicker_Load(object sender,EventArgs e) {
			if(!HasShowHiddenOption) {
				checkShowHidden.Visible=false;
			}
			if(!IsMultiSelectionMode) {
				gridMain.SelectionMode=GridSelectionMode.One;
			}
			FillGrid();
		}

		///<summary>Fills local list from cache and removes any that should not be available to select given the currently selected DefNum.  Used to
		///prevent infinite loop error if a descendant category is made the parent of an ancestor.  The currently selected Def and any direct-line
		///descendants are removed from the list so they cannot be selected as the current def's parent.
		///If defNumCur is 0 or invalid, returns Defs.GetListLong.</summary>
		private void FillListDefs(DefCat cat,long defNumCur) {
			_listDefs=Defs.GetDefsForCategory(cat);
			Def defCur=_listDefs.FirstOrDefault(x => x.DefNum==defNumCur);
			if(defCur==null) {//either defNumCur is 0 or it is an invalid DefNum, either way nothing to exclude
				return;
			}
			List<long> listInvalidNums=new List<long>() { defCur.DefNum };
			for(int i=0;i<listInvalidNums.Count;i++) {
				string defNumStr=listInvalidNums[i].ToString();
				listInvalidNums.AddRange(_listDefs.FindAll(x => x.ItemValue==defNumStr).Select(x => x.DefNum));
			}
			_listDefs.RemoveAll(x => listInvalidNums.Contains(x.DefNum));
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"Definition"),200));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"ItemValue"),70));
			if(HasShowHiddenOption) {
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"Hidden"),20){ IsWidthDynamic=true });
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			List<int> listSelectedIndexes=new List<int>();
			Dictionary<string,string> dictDefNumStrItemName=new Dictionary<string,string>();
			if(_listDefs.Count>0 && _listDefs[0].Category==DefCat.AutoNoteCats) {
				dictDefNumStrItemName=_listDefs.ToDictionary(x => x.DefNum.ToString(),x => x.ItemName);
			}
			foreach(Def defCur in _listDefs) {
				//even if "Show Hidden" is not checked, show hidden defs if they were passed in in the initial list.
				if(defCur.IsHidden && !checkShowHidden.Checked && !_listDefInitial.Any(x => defCur.DefNum==x.DefNum)) {
					continue;
				}
				row=new GridRow();
				row.Cells.Add(defCur.ItemName);
				//for auto note categories, the ItemValue is stored as the long of the parent DefNum, so we have to get the name from the list.  But always
				//default to the ItemValue if the num cannot be found in the list.
				if(defCur.Category==DefCat.AutoNoteCats && !string.IsNullOrWhiteSpace(defCur.ItemValue)) {
					string iValue;
					row.Cells.Add(dictDefNumStrItemName.TryGetValue(defCur.ItemValue,out iValue)?iValue:defCur.ItemValue);
				}
				else {
					row.Cells.Add(defCur.ItemValue);
				}
				if(HasShowHiddenOption) {
					row.Cells.Add(defCur.IsHidden?"X":"");
				}
				row.Tag=defCur;
				gridMain.ListGridRows.Add(row);
				if(ListSelectedDefs.Any(x => defCur.DefNum==x.DefNum)) {//after adding row, see if it should be reselected after grid update
					listSelectedIndexes.Add(gridMain.ListGridRows.Count-1);//if reselecting this def, add the row count-1 to list of indexes
				}
			}
			gridMain.EndUpdate();
			listSelectedIndexes.ForEach(x => gridMain.SetSelected(x,true));
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			ListSelectedDefs.Clear();
			ListSelectedDefs.AddRange(gridMain.SelectedIndices.Select(x => (Def)gridMain.ListGridRows[x].Tag));
		}

		private void checkShowHidden_CheckedChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void ButNone_Click(object sender,EventArgs e) {
			ListSelectedDefs.Clear();
			FillGrid();
		}
	}
}