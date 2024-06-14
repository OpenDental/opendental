using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
	///<summary></summary>
	public partial class FormDisplayFields : FormODBase {
		private bool _changed;
		///<summary>Should not be set to DisplayFieldCategory.OrthoChart.  For ortho chart, use FormDisplayFieldsOrthoChart.</summary>
		public DisplayFieldCategory DisplayFieldCategoryCur;
		///<summary>When this form opens, this is the list of display fields that the user has already explicitly set to be showing.  If the user did not
		///set any to be showing yet, then this will start out as the default list.  This is a subset of AvailList.  As this window is used, items are
		///added to this list but not saved until window closes with OK.</summary>
		private List<DisplayField> _listDisplayFieldsShowing;
		///<summary>This is the list of all possible display fields.  Once the grids on the form are filled, this AvailList is only the items showing in
		///the list at the right.</summary>
		private List<DisplayField> _listDisplayFieldsAvail;

		public FormDisplayFields() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDisplayFields_Load(object sender,EventArgs e) {
			labelCategory.Text=DisplayFieldCategoryCur.ToString();
			DisplayFields.RefreshCache();
			_listDisplayFieldsShowing=DisplayFields.GetForCategory(DisplayFieldCategoryCur);
			if(DisplayFieldCategoryCur==DisplayFieldCategory.ChartPatientInformation
				&& !PrefC.GetBool(PrefName.ShowFeatureEhr)
				&& _listDisplayFieldsShowing.Any(x => x.InternalName=="Tobacco Use"))
			{
				//user may have enable EHR features, added the tobacco use display field, and then disabled EHR features, remove the tobacco use display field
				_listDisplayFieldsShowing.RemoveAll(x => x.InternalName=="Tobacco Use");
				_changed=true;
			}
			if(DisplayFieldCategoryCur==DisplayFieldCategory.ChartPatientInformation
				&& PrefC.GetBool(PrefName.EasyHideHospitals) //true is hidden
				&& (_listDisplayFieldsShowing.Any(x => x.InternalName=="Admit Date" || x.InternalName=="Discharge Date")))
			{
				//user may have enable Hospitals features, added the Admit Date or Discharge Date display field, and then disabled Hospital features,
				//remove the Admit Date or Discharge Date display field
				_listDisplayFieldsShowing.RemoveAll(x => x.InternalName=="Admit Date");
				_listDisplayFieldsShowing.RemoveAll(x => x.InternalName=="Discharge Date");
				_changed=true;
			}
			FillGrids();
		}

		private void FillGrids(){
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("FormDisplayFields","FieldName"),110);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormDisplayFields","New Descript"),110);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("FormDisplayFields","Width"),60);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listDisplayFieldsShowing.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listDisplayFieldsShowing[i].InternalName);
				string displayText=_listDisplayFieldsShowing[i].DescriptionOverride;
				if(string.IsNullOrWhiteSpace(displayText)) {
					displayText=_listDisplayFieldsShowing[i].Description;
				}
				row.Cells.Add(displayText);
				row.Cells.Add(_listDisplayFieldsShowing[i].ColumnWidth.ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			//Remove items from AvailList that are in the ListShowing.
			_listDisplayFieldsAvail=DisplayFields.GetAllAvailableList(DisplayFieldCategoryCur).Except(_listDisplayFieldsShowing, new DisplayFieldComparer()).ToList();
			listAvailable.Items.Clear();
			if(DisplayFieldCategoryCur==DisplayFieldCategory.SuperFamilyGridCols) {
				listAvailable.Items.AddList(_listDisplayFieldsAvail,x=>x.ToDescriptionString());
				return;
			}
			listAvailable.Items.AddList(_listDisplayFieldsAvail,x=>x.ToString());
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			DisplayField displayFieldTemp=_listDisplayFieldsShowing[e.Row].Copy();
			using FormDisplayFieldEdit formDisplayFieldEdit=new FormDisplayFieldEdit();
			formDisplayFieldEdit.DisplayFieldCur=_listDisplayFieldsShowing[e.Row];
			formDisplayFieldEdit.ShowDialog();
			if(formDisplayFieldEdit.DialogResult!=DialogResult.OK) {
				_listDisplayFieldsShowing[e.Row]=displayFieldTemp.Copy();
				return;
			}
			FillGrids();
			_changed=true;
		}

		private void butDefault_Click(object sender,EventArgs e) {
			_listDisplayFieldsShowing=DisplayFields.GetDefaultList(DisplayFieldCategoryCur);
			FillGrids();
			_changed=true;
		}

		private void butLeft_Click(object sender,EventArgs e) {
			if(listAvailable.SelectedIndices.Count==0) {
				MsgBox.Show(this,"Please select an item in the list on the right first.");
				return;
			}
			List<DisplayField> listDisplayFieldsSelected=listAvailable.GetListSelected<DisplayField>();
			for(int i=0;i<listDisplayFieldsSelected.Count;i++) {
				_listDisplayFieldsShowing.Add(listDisplayFieldsSelected[i]);
			}
			_changed=true;
			FillGrids();
		}

		private void butRight_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid on the left first.");
				return;
			}
			for(int i=gridMain.SelectedIndices.Length-1;i>=0;i--){//go backwards
				_listDisplayFieldsShowing.RemoveAt(gridMain.SelectedIndices[i]);
			}
			FillGrids();
			_changed=true;
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid first.");
				return;
			}
			int[] intArraySelected=new int[gridMain.SelectedIndices.Length];
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){
				intArraySelected[i]=gridMain.SelectedIndices[i];
			}
			if(intArraySelected[0]==0){
				return;
			}
			for(int i=0;i<intArraySelected.Length;i++){
				_listDisplayFieldsShowing.Reverse(intArraySelected[i]-1,2);
			}
			FillGrids();
			for(int i=0;i<intArraySelected.Length;i++){
				gridMain.SetSelected(intArraySelected[i]-1,setValue:true);
			}
			_changed=true;
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid first.");
				return;
			}
			int[] intArraySelected=new int[gridMain.SelectedIndices.Length];
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				intArraySelected[i]=gridMain.SelectedIndices[i];
			}
			if(intArraySelected[intArraySelected.Length-1]==_listDisplayFieldsShowing.Count-1) {
				return;
			}
			for(int i=intArraySelected.Length-1;i>=0;i--) {//go backwards
				_listDisplayFieldsShowing.Reverse(intArraySelected[i],2);
			}
			FillGrids();
			for(int i=0;i<intArraySelected.Length;i++) {
				gridMain.SetSelected(intArraySelected[i]+1,setValue:true);
			}
			_changed=true;
		}

		private bool ContainsDuplicateDescriptions() {
			List<string> listDescriptions=new List<string>();
			for(int i=0;i<_listDisplayFieldsShowing.Count;i++) {
				string description=_listDisplayFieldsShowing[i].Description;
				if(string.IsNullOrEmpty(description)) {
					continue;
				}
				if(_listDisplayFieldsShowing.Any(x => x.InternalName==description) || listDescriptions.Contains(description)) {
					return true;
				}
				listDescriptions.Add(description);
			}
			return false;
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(ContainsDuplicateDescriptions()) {
				MsgBox.Show("Display Fields cannot have duplicate descriptions or descriptions matching FieldName. Fix all entries before saving.");
				return;
			}
			if(!_changed) {
				DialogResult=DialogResult.OK;
				return;
			}
			DisplayFields.SaveListForCategory(_listDisplayFieldsShowing,DisplayFieldCategoryCur);
			DataValid.SetInvalid(InvalidType.DisplayFields);
			DialogResult=DialogResult.OK;
		}

		public class DisplayFieldComparer : IEqualityComparer<DisplayField> {
			public int GetHashCode(DisplayField displayField) {
				string hash=displayField.InternalName;
				return hash.GetHashCode();
			}

			public bool Equals(DisplayField displayFieldA, DisplayField displayFieldB) {
				bool retVal=displayFieldA.InternalName==displayFieldB.InternalName;
				if(displayFieldA.InternalName=="" || displayFieldB.InternalName=="") {
					retVal=retVal && displayFieldA.Description==displayFieldB.Description;
				}
				return retVal;
			}
		}
	}
}