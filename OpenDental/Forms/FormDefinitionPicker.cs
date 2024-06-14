using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormDefinitionPicker:FormODBase {
		///<summary>The passed-in list of Defs.</summary>
		private List<Def> _listDefsInitialSelected=new List<Def>();
		///<summary>After clicking OK, this is the list of selected defs.</summary>
		public List<Def> ListDefsSelected;
		///<summary>List of all defs of the passed-in category type.</summary>
		private List<Def> _listDefsShowing;
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
				_listDefsInitialSelected=new List<Def>(listDefs);
			}
			gridMain.Title=defCat.ToString();
			FillListDefs(defCat,defNumExclude);
		}

		private void FormDefinitionPicker_Load(object sender,EventArgs e) {
			if(!HasShowHiddenOption) {
				checkShowHidden.Visible=false;
			}
			if(!IsMultiSelectionMode) {
				gridMain.SelectionMode=GridSelectionMode.OneRow;
			}
			if(_listDefsInitialSelected.Any(x => x.IsHidden==true)){
				checkShowHidden.Checked=true;
			}
			FillGrid();
			List<long> listDefNumsSelected=_listDefsInitialSelected.Select(x => x.DefNum).ToList();
			for(int i=0;i<gridMain.ListGridRows.Count;i++){
				if(listDefNumsSelected.Contains(((Def)gridMain.ListGridRows[i].Tag).DefNum)) {
					gridMain.SetSelected(i,true);
				}
			}
		}

		///<summary>Fills local list from cache and removes any that should not be available to select given the currently selected DefNum.  Used to
		///prevent infinite loop error if a descendant category is made the parent of an ancestor.  The currently selected Def and any direct-line
		///descendants are removed from the list so they cannot be selected as the current def's parent.
		///If defNum is 0 or invalid, returns Defs.GetListLong.</summary>
		private void FillListDefs(DefCat defCat,long defNumExclude) {
			_listDefsShowing=Defs.GetDefsForCategory(defCat);
			Def defExclude=_listDefsShowing.FirstOrDefault(x => x.DefNum==defNumExclude);
			if(defExclude==null) {//either defNum is 0 or it is an invalid DefNum, either way nothing to exclude
				return;
			}
			List<long> listInvalidNums=new List<long>() { defExclude.DefNum };
			for(int i=0;i<listInvalidNums.Count;i++) {
				string strDefNum=listInvalidNums[i].ToString();
				List<long> listDefNums=_listDefsShowing.FindAll(x => x.ItemValue==strDefNum).Select(x => x.DefNum).ToList();//children
				listInvalidNums.AddRange(listDefNums);//adding to list while looping through it.
			}
			_listDefsShowing.RemoveAll(x => listInvalidNums.Contains(x.DefNum));
		}

		private void FillGrid() {
			List<long> listDefNumsSelected=new List<long>();
			listDefNumsSelected=gridMain.SelectedTags<Def>().Select(x => x.DefNum).ToList();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"Definition"),200));
			gridMain.Columns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"ItemValue"),70));
			if(HasShowHiddenOption) {
				gridMain.Columns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"Hidden"),20){ IsWidthDynamic=true });
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listDefsShowing.Count;i++) {
				//even if "Show Hidden" is not checked, show hidden defs if they were passed in in the initial list.
				if(_listDefsShowing[i].IsHidden && !checkShowHidden.Checked){
					continue;
				}
				row=new GridRow();
				row.Cells.Add(_listDefsShowing[i].ItemName);
				//for auto note categories, the ItemValue is stored as the long of the parent DefNum, so we have to get the name from the list.  But always
				//default to the ItemValue if the num cannot be found in the list.
				if(_listDefsShowing[i].Category==DefCat.AutoNoteCats && !string.IsNullOrWhiteSpace(_listDefsShowing[i].ItemValue)) {
					Def defParent=_listDefsShowing.FirstOrDefault(x => x.DefNum.ToString()==_listDefsShowing[i].ItemValue);
					if(defParent==null) {
						row.Cells.Add(_listDefsShowing[i].ItemValue);
					}
					else {
						row.Cells.Add(defParent.ItemName);
					}
				}
				else {
					row.Cells.Add(_listDefsShowing[i].ItemValue);
				}
				if(HasShowHiddenOption) {
					row.Cells.Add(_listDefsShowing[i].IsHidden?"X":"");
				}
				row.Tag=_listDefsShowing[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			for(int i=0;i<gridMain.ListGridRows.Count;i++){
				if(listDefNumsSelected.Contains(((Def)gridMain.ListGridRows[i].Tag).DefNum)){
					gridMain.SetSelected(i);
				}
			}
		}

		private void checkShowHidden_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			ListDefsSelected=ListTools.FromSingle((Def)gridMain.ListGridRows[e.Row].Tag);
			DialogResult=DialogResult.OK;
		}

		private void ButNone_Click(object sender,EventArgs e) {
			gridMain.SetAll(false);
		}

		private void butOK_Click(object sender,EventArgs e) {
			ListDefsSelected=gridMain.SelectedTags<Def>();
			DialogResult=DialogResult.OK;
		}

	}
}