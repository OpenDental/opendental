using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;

namespace OpenDental {
	public partial class FormAlertCategorySetup:FormODBase {
		private List<AlertCategory> _listAlertCategoriesInternal=new List<AlertCategory>();
		private List<AlertCategory> _listAlertCategoriesCustom=new List<AlertCategory>();

		public FormAlertCategorySetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}
		
		private void FormAlertCategorySetup_Load(object sender,EventArgs e) {
			FillGrids();
		}

		private void FillGrids(long selectedInternalKey=0,long selectedCustomKey=0) {
			_listAlertCategoriesCustom.Clear();
			_listAlertCategoriesInternal.Clear();
			List<AlertCategory> listAlertCategories=AlertCategories.GetDeepCopy();
			for(int i=0;i<listAlertCategories.Count;i++) {
				if(listAlertCategories[i].IsHQCategory) {
					_listAlertCategoriesInternal.Add(listAlertCategories[i]);
				}
				else {
					_listAlertCategoriesCustom.Add(listAlertCategories[i]);
				}
			}
			_listAlertCategoriesInternal.OrderBy(x => x.InternalName);
			_listAlertCategoriesCustom.OrderBy(x => x.InternalName);
			FillInternalGrid(selectedInternalKey);
			FillCustomGrid(selectedCustomKey);
		}

		private void FillInternalGrid(long selectedInternalKey) {
			gridInternal.BeginUpdate();
			gridInternal.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Description"),100);
			gridInternal.Columns.Add(col);
			gridInternal.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listAlertCategoriesInternal.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listAlertCategoriesInternal[i].Description);
				row.Tag=_listAlertCategoriesInternal[i].AlertCategoryNum;
				gridInternal.ListGridRows.Add(row);
				int index=gridInternal.ListGridRows.Count-1;
				if(selectedInternalKey==_listAlertCategoriesInternal[i].AlertCategoryNum) {
					gridCustom.SetSelected(index,true);
				}
			}
			gridInternal.EndUpdate();
		}
		
		private void FillCustomGrid(long selectedCustomKey) {
			gridCustom.BeginUpdate();
			gridCustom.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Description"),100);
			gridCustom.Columns.Add(col);
			gridCustom.ListGridRows.Clear();
			GridRow row;
			int index=0;
			for(int i=0;i<_listAlertCategoriesCustom.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listAlertCategoriesCustom[i].Description);
				row.Tag=_listAlertCategoriesCustom[i].AlertCategoryNum;
				gridCustom.ListGridRows.Add(row);
				index=gridCustom.ListGridRows.Count-1;
				if(selectedCustomKey!=_listAlertCategoriesCustom[i].AlertCategoryNum) {
					index=0;
				}
			}
			if(index!=0) {
				gridCustom.SetSelected(index,true);
			}
			gridCustom.EndUpdate();
		}
		
		private void gridInternal_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormAlertCategoryEdit formAlertCategoryEdit=new FormAlertCategoryEdit(_listAlertCategoriesInternal[e.Row]);
			if(formAlertCategoryEdit.ShowDialog()==DialogResult.OK) {
				FillGrids();
			}
		}

		private void gridCustom_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormAlertCategoryEdit formAlertCategoryEdit=new FormAlertCategoryEdit(_listAlertCategoriesCustom[e.Row]);
			if(formAlertCategoryEdit.ShowDialog()==DialogResult.OK) {
				FillGrids();
			}
		}

		private void butCopy_Click(object sender,EventArgs e) {
			if(gridInternal.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select an internal alert category from the list first.");
				return;
			}
			InsertCopyAlertCategory(_listAlertCategoriesInternal[gridInternal.GetSelectedIndex()].Copy());
		}

		private void butDuplicate_Click(object sender,EventArgs e) {
			if(gridCustom.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select a custom alert category from the list first.");
				return;
			}
			InsertCopyAlertCategory(_listAlertCategoriesCustom[gridCustom.GetSelectedIndex()].Copy());
		}

		private void InsertCopyAlertCategory(AlertCategory alertCategory) {
			alertCategory.IsHQCategory=false;
			alertCategory.Description+=Lan.g(this,"(Copy)");
			//alertCategory.AlertCategoryNum reflects the original pre-copied PK. After Insert this will be a new PK for the new row.
			List<AlertCategoryLink> listAlertCategoryLinks=AlertCategoryLinks.GetForCategory(alertCategory.AlertCategoryNum);
			alertCategory.AlertCategoryNum=AlertCategories.Insert(alertCategory);
			//At this point alertCategory has a new PK, so we need to update and insert our new copied alertCategoryLinks
			listAlertCategoryLinks.ForEach(x => {
				x.AlertCategoryNum=alertCategory.AlertCategoryNum;
				AlertCategoryLinks.Insert(x);
			});
			DataValid.SetInvalid(InvalidType.AlertCategories,InvalidType.AlertCategoryLinks);
			FillGrids();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}