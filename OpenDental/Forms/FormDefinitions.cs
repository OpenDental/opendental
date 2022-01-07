using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using OpenDental.UI;
using System.Globalization;

namespace OpenDental{
///<summary></summary>
	public partial class FormDefinitions : FormODBase {
		private DefCat _defCatInitial;
		private bool _isDefChanged;
		///<summary>All defs for the selected category, sorted.</summary>
		private List<Def> _listDefsAll;

		protected override string GetHelpOverride() {
			DefCatOptions defCatOption=(DefCatOptions)listCategory.SelectedItem;
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && defCatOption.DefCat==DefCat.ApptProcsQuickAdd) {
				return "FormDefinitionsCanada";
			}
			return "FormDefinitions";
		}

		///<summary>Must check security before allowing this window to open.</summary>
		public FormDefinitions(DefCat defCatInitial){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			_defCatInitial=defCatInitial;
			Lan.F(this);
		}

		private void FormDefinitions_Load(object sender, System.EventArgs e) {
			List<DefCatOptions> listDefCatOptions=new List<DefCatOptions>();
			listDefCatOptions=DefL.GetOptionsForDefCats(Enum.GetValues(typeof(DefCat)));
			listDefCatOptions=listDefCatOptions.OrderBy(x => x.DefCat.GetDescription()).ToList(); //orders alphabetically.
			for(int i=0;i<listDefCatOptions.Count;i++) {
				listCategory.Items.Add(Lan.g(this,listDefCatOptions[i].DefCat.GetDescription()),listDefCatOptions[i]);
				if(_defCatInitial==listDefCatOptions[i].DefCat) {
					listCategory.SetSelected(i,true);
				}
			}
		}

		private void listCategory_SelectedIndexChanged(object sender,System.EventArgs e) {
			FillGridDefs();
		}

		private void RefreshDefs() {
			Defs.RefreshCache();
			_listDefsAll=Defs.GetDeepCopy().SelectMany(x => x.Value).ToList();
		}

		private void FillGridDefs(){
			if(listCategory.SelectedIndex==-1){
				return;//this will happen when dragging to a high dpi window
			}
			if(_listDefsAll==null || _listDefsAll.Count==0) {
				RefreshDefs();
			}
			DefCatOptions defCatOptionsSelected=listCategory.GetSelected<DefCatOptions>();
			List<Def> listDefsSorted=_listDefsAll.Where(x => x.Category==defCatOptionsSelected.DefCat).OrderBy(x => x.ItemOrder).ToList();
			DefL.FillGridDefs(gridDefs,defCatOptionsSelected,listDefsSorted);
			//the following do not require a refresh of the table:
			if(defCatOptionsSelected.CanHide) {
				butHide.Visible=true;
			}
			else {
				butHide.Visible=false;
			}
			if(defCatOptionsSelected.CanEditName) {
				groupEdit.Enabled=true;
				groupEdit.Text=Lans.g(this,"Edit Items");
			}
			else {
				groupEdit.Enabled=false;
				groupEdit.Text=Lans.g(this,"Not allowed");
			}
			textGuide.Text=defCatOptionsSelected.HelpText;
		}

		private void gridDefs_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Def defSelected=(Def)gridDefs.ListGridRows[e.Row].Tag;
			DefCatOptions defCatOptionsSelected=listCategory.GetSelected<DefCatOptions>();
			List<Def> listDefsSorted=_listDefsAll.Where(x => x.Category==defCatOptionsSelected.DefCat).OrderBy(x => x.ItemOrder).ToList();
			_isDefChanged=DefL.GridDefsDoubleClick(defSelected,gridDefs,defCatOptionsSelected,listDefsSorted,_listDefsAll,_isDefChanged);
			if(_isDefChanged) {
				RefreshDefs();
				FillGridDefs();
			}
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			DefCatOptions defCatOptionsSelected=listCategory.GetSelected<DefCatOptions>();
			if(DefL.AddDef(gridDefs,defCatOptionsSelected)) {
				RefreshDefs();
				FillGridDefs();
				_isDefChanged=true;
			}
		}

		private void butHide_Click(object sender, System.EventArgs e) {
			DefCatOptions defCatOptionsSelected=listCategory.GetSelected<DefCatOptions>();
			if(DefL.TryHideDefSelectedInGrid(gridDefs,defCatOptionsSelected)) {
				RefreshDefs();
				FillGridDefs();
				_isDefChanged=true;
			}
		}

		private void butUp_Click(object sender, System.EventArgs e) {
			if(DefL.UpClick(gridDefs)) {
				_isDefChanged=true;
				FillGridDefs();
			}
		}

		private void butDown_Click(object sender, System.EventArgs e) {
			if(DefL.DownClick(gridDefs)){
				_isDefChanged=true;
				FillGridDefs();
			}
		}

		private void butAlphabetize_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Alphabetizing does not have an 'undo' button.  Continue?")) {
				return;
			}
			DefCatOptions defCatOptionsSelected=listCategory.GetSelected<DefCatOptions>();
			List<Def> listDefsSorting=_listDefsAll.Where(x => x.Category==defCatOptionsSelected.DefCat).OrderBy(x => x.ItemName).ToList(); 
			for(int i=0;i<listDefsSorting.Count;i++) {
				listDefsSorting[i].ItemOrder=i;
				Defs.Update(listDefsSorting[i]);
			}
			_isDefChanged=true;
			RefreshDefs();
			FillGridDefs();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormDefinitions_Closing(object sender,System.ComponentModel.CancelEventArgs e) {
			//Correct the item orders of all definition categories.
			List<Def> listDefsUpdates=new List<Def>();
			foreach(KeyValuePair<DefCat,List<Def>> kvp in Defs.GetDeepCopy()) {
				for(int i=0;i<kvp.Value.Count;i++) {
					if(kvp.Value[i].ItemOrder!=i) {
						kvp.Value[i].ItemOrder=i;
						listDefsUpdates.Add(kvp.Value[i]);
					}
				}
			}
			listDefsUpdates.ForEach(x => Defs.Update(x));
			if(_isDefChanged || listDefsUpdates.Count>0) {
				//A specialty could have been renamed, invalidate the specialty associated to the currently selected patient just in case.
				PatientL.InvalidateSelectedPatSpecialty();
				DataValid.SetInvalid(InvalidType.Defs);
			}
		}

	}
}
