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
		private List<Def> _listDefsInitial=new List<Def>();
		///<summary>The selected defs at any given point. Initially, this is the same as _listDefsInitial.</summary>
		public List<Def> ListDefsSelected=new List<Def>();
		///<summary>List of all defs of the passed-in category type.</summary>
		private List<Def> _listDefs;
		///<summary>Set to true to allow showing hidden.</summary>
		public bool HasShowHiddenOption;
		///<summary>Allows selecting multiple.  If false, ListSelectedDefs will only have one result.</summary>
		public bool IsMultiSelectionMode;

		///<summary>Passing in a list of Defs will make those defs pre-selected and highlighted when this window loads.  For AutoNoteCategories this is
		///used to select a parent category for the current category, but we want to prevent an infinite loop so defNumExclude is used to exclude the
		///currently selected def and any direct-line descendants from being selected as the category's parent.</summary>
		public FormDefinitionPicker(DefCat defCat,List<Def> listDefs=null,long defNumExclude=0) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			if(listDefs!=null) {
				ListDefsSelected=listDefs; //initially, selected defs and list defs are the same. However, ListSelectedDefs changes while _listDefInitial doesn't.
				_listDefsInitial=new List<Def>(listDefs);
			}
			gridMain.Title=defCat.ToString();
			FillListDefs(defCat,defNumExclude);
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
		///If defNum is 0 or invalid, returns Defs.GetListLong.</summary>
		private void FillListDefs(DefCat defCat,long defNumExclude) {
			_listDefs=Defs.GetDefsForCategory(defCat);
			Def defExclude=_listDefs.FirstOrDefault(x => x.DefNum==defNumExclude);
			if(defExclude==null) {//either defNum is 0 or it is an invalid DefNum, either way nothing to exclude
				return;
			}
			List<long> listInvalidNums=new List<long>() { defExclude.DefNum };
			for(int i=0;i<listInvalidNums.Count;i++) {
				string defNumStr=listInvalidNums[i].ToString();
				List<long> listDefNums=_listDefs.FindAll(x => x.ItemValue==defNumStr).Select(x => x.DefNum).ToList();//children
				listInvalidNums.AddRange(listDefNums);//adding to list while looping through it.
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
			for(int i=0;i<_listDefs.Count;i++) {
				//even if "Show Hidden" is not checked, show hidden defs if they were passed in in the initial list.
				if(_listDefs[i].IsHidden && !checkShowHidden.Checked && !_listDefsInitial.Any(x => _listDefs[i].DefNum==x.DefNum)) {
					continue;
				}
				row=new GridRow();
				row.Cells.Add(_listDefs[i].ItemName);
				//for auto note categories, the ItemValue is stored as the long of the parent DefNum, so we have to get the name from the list.  But always
				//default to the ItemValue if the num cannot be found in the list.
				if(_listDefs[i].Category==DefCat.AutoNoteCats && !string.IsNullOrWhiteSpace(_listDefs[i].ItemValue)) {
					Def defParent=_listDefs.FirstOrDefault(x => x.DefNum.ToString()==_listDefs[i].ItemValue);
					if(defParent==null) {
						row.Cells.Add(_listDefs[i].ItemValue);
					}
					else {
						row.Cells.Add(defParent.ItemName);
					}
				}
				else {
					row.Cells.Add(_listDefs[i].ItemValue);
				}
				if(HasShowHiddenOption) {
					row.Cells.Add(_listDefs[i].IsHidden?"X":"");
				}
				row.Tag=_listDefs[i];
				gridMain.ListGridRows.Add(row);
				if(ListDefsSelected.Any(x => _listDefs[i].DefNum==x.DefNum)) {//after adding row, see if it should be reselected after grid update
					listSelectedIndexes.Add(gridMain.ListGridRows.Count-1);//if reselecting this def, add the row count-1 to list of indexes
				}
			}
			gridMain.EndUpdate();
			listSelectedIndexes.ForEach(x => gridMain.SetSelected(x,true));
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			ListDefsSelected.Clear();
			ListDefsSelected.AddRange(gridMain.SelectedIndices.Select(x => (Def)gridMain.ListGridRows[x].Tag));
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
			ListDefsSelected.Clear();
			FillGrid();
		}
	}
}