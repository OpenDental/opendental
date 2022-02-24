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
		private DefCat _initialCat;
		private bool _isDefChanged;
		private List<Def> _listDefsAll;

		///<summary>Gets the currently selected DefCat along with its options.</summary>
		private DefCatOptions _selectedDefCatOpt {
			get { return listCategory.GetSelected<DefCatOptions>(); }
		}

		///<summary>All definitions for the current category, hidden and non-hidden.</summary>
		private List<Def> _listDefsCur {
			get { return _listDefsAll.Where(x => x.Category == _selectedDefCatOpt.DefCat).OrderBy(x => x.ItemOrder).ToList(); }
		}

		protected override string GetHelpOverride() {
			DefCatOptions defCatOption=(DefCatOptions)listCategory.SelectedItem;
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && defCatOption.DefCat==DefCat.ApptProcsQuickAdd) {
				return "FormDefinitionsCanada";
			}
			return "FormDefinitions";
		}

		///<summary>Must check security before allowing this window to open.</summary>
		public FormDefinitions(DefCat initialCat){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			_initialCat=initialCat;
			Lan.F(this);
		}

		private void FormDefinitions_Load(object sender, System.EventArgs e) {
			LoadListDefCats();
		}

		private void LoadListDefCats() {
			List<DefCatOptions> listDefCatOptions = new List<DefCatOptions>();
			listDefCatOptions=DefL.GetOptionsForDefCats(Enum.GetValues(typeof(DefCat)));
			listDefCatOptions = listDefCatOptions.OrderBy(x => x.DefCat.GetDescription()).ToList(); //orders alphabetically.
			for(int i=0; i<listDefCatOptions.Count; i++) {
				listCategory.Items.Add(Lan.g(this,listDefCatOptions[i].DefCat.GetDescription()),listDefCatOptions[i]);
				if(_initialCat == listDefCatOptions[i].DefCat) {
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
			if(_listDefsAll == null || _listDefsAll.Count == 0) {
				RefreshDefs();
			}
			DefL.FillGridDefs(gridDefs,_selectedDefCatOpt,_listDefsCur);
			//the following do not require a refresh of the table:
			if(_selectedDefCatOpt.CanHide) {
				butHide.Visible=true;
			}
			else {
				butHide.Visible=false;
			}
			if(_selectedDefCatOpt.CanEditName) {
				groupEdit.Enabled=true;
				groupEdit.Text=Lans.g(this,"Edit Items");
			}
			else {
				groupEdit.Enabled=false;
				groupEdit.Text=Lans.g(this,"Not allowed");
			}
			textGuide.Text=_selectedDefCatOpt.HelpText;
		}

		private void gridDefs_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Def selectedDef = (Def)gridDefs.ListGridRows[e.Row].Tag;
			_isDefChanged=DefL.GridDefsDoubleClick(selectedDef,gridDefs,_selectedDefCatOpt,_listDefsCur,_listDefsAll,_isDefChanged);
			if(_isDefChanged) {
				RefreshDefs();
				FillGridDefs();
			}
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			if(DefL.AddDef(gridDefs,_selectedDefCatOpt)) {
				RefreshDefs();
				FillGridDefs();
				_isDefChanged=true;
			}
		}

		private void butHide_Click(object sender, System.EventArgs e) {
			if(DefL.TryHideDefSelectedInGrid(gridDefs,_selectedDefCatOpt)) {
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
			List<Def> listDefsSorting=_listDefsCur.OrderBy(x => x.ItemName).ToList(); 
			for(int i=0;i < listDefsSorting.Count;i++) {
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
			List<Def> listDefUpdates=new List<Def>();
			foreach(KeyValuePair<DefCat,List<Def>> kvp in Defs.GetDeepCopy()) {
				for(int i=0;i<kvp.Value.Count;i++) {
					if(kvp.Value[i].ItemOrder!=i) {
						kvp.Value[i].ItemOrder=i;
						listDefUpdates.Add(kvp.Value[i]);
					}
				}
			}
			listDefUpdates.ForEach(x => Defs.Update(x));
			if(_isDefChanged || listDefUpdates.Count>0) {
				//A specialty could have been renamed, invalidate the specialty associated to the currently selected patient just in case.
				PatientL.InvalidateSelectedPatSpecialty();
				DataValid.SetInvalid(InvalidType.Defs);
			}
		}

	}
}
