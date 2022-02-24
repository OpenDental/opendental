using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
	///<summary></summary>
	public partial class FormDisplayFields : FormODBase {
		private bool changed;
		///<summary>Should not be set to DisplayFieldCategory.OrthoChart.  For ortho chart, use FormDisplayFieldsOrthoChart.</summary>
		public DisplayFieldCategory Category;
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
			labelCategory.Text=Category.ToString();
			DisplayFields.RefreshCache();
			_listDisplayFieldsShowing=DisplayFields.GetForCategory(Category);
			if(Category==DisplayFieldCategory.ChartPatientInformation
				&& !PrefC.GetBool(PrefName.ShowFeatureEhr)
				&& _listDisplayFieldsShowing.Any(x => x.InternalName=="Tobacco Use"))
			{
				//user may have enable EHR features, added the tobacco use display field, and then disabled EHR features, remove the tobacco use display field
				_listDisplayFieldsShowing.RemoveAll(x => x.InternalName=="Tobacco Use");
				changed=true;
			}
			FillGrids();
		}

		private void FillGrids(){
			_listDisplayFieldsAvail=DisplayFields.GetAllAvailableList(Category);//This one needs to be called repeatedly.
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("FormDisplayFields","FieldName"),110);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormDisplayFields","New Descript"),110);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormDisplayFields","Width"),60);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listDisplayFieldsShowing.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listDisplayFieldsShowing[i].InternalName);
				row.Cells.Add(_listDisplayFieldsShowing[i].Description);
				row.Cells.Add(_listDisplayFieldsShowing[i].ColumnWidth.ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			//Remove items from AvailList that are in the ListShowing.
			for(int i=0;i<_listDisplayFieldsShowing.Count;i++){
				for(int j=0;j<_listDisplayFieldsAvail.Count;j++) {
					//Only removing one item from AvailList per iteration of i, so RemoveAt() is safe without going backwards.
					if(_listDisplayFieldsShowing[i].InternalName==_listDisplayFieldsAvail[j].InternalName) {
						_listDisplayFieldsAvail.RemoveAt(j);
						break;
					}
				}
			}
			listAvailable.Items.Clear();
			listAvailable.Items.AddList(_listDisplayFieldsAvail,x=>x.ToString());
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			DisplayField tempField=_listDisplayFieldsShowing[e.Row].Copy();
			using FormDisplayFieldEdit formD=new FormDisplayFieldEdit();
			formD.FieldCur=_listDisplayFieldsShowing[e.Row];
			formD.ShowDialog();
			if(formD.DialogResult!=DialogResult.OK) {
				_listDisplayFieldsShowing[e.Row]=tempField.Copy();
				return;
			}
			FillGrids();
			changed=true;
		}

		private void butDefault_Click(object sender,EventArgs e) {
			_listDisplayFieldsShowing=DisplayFields.GetDefaultList(Category);
			FillGrids();
			changed=true;
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
			changed=true;
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
			changed=true;
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid first.");
				return;
			}
			int[] selected=new int[gridMain.SelectedIndices.Length];
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){
				selected[i]=gridMain.SelectedIndices[i];
			}
			if(selected[0]==0){
				return;
			}
			for(int i=0;i<selected.Length;i++){
				_listDisplayFieldsShowing.Reverse(selected[i]-1,2);
			}
			FillGrids();
			for(int i=0;i<selected.Length;i++){
				gridMain.SetSelected(selected[i]-1,true);
			}
			changed=true;
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid first.");
				return;
			}
			int[] selected=new int[gridMain.SelectedIndices.Length];
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				selected[i]=gridMain.SelectedIndices[i];
			}
			if(selected[selected.Length-1]==_listDisplayFieldsShowing.Count-1) {
				return;
			}
			for(int i=selected.Length-1;i>=0;i--) {//go backwards
				_listDisplayFieldsShowing.Reverse(selected[i],2);
			}
			FillGrids();
			for(int i=0;i<selected.Length;i++) {
				gridMain.SetSelected(selected[i]+1,true);
			}
			changed=true;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!changed) {
				DialogResult=DialogResult.OK;
				return;
			}
			DisplayFields.SaveListForCategory(_listDisplayFieldsShowing,Category);
			DataValid.SetInvalid(InvalidType.DisplayFields);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
