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

		private List<AlertCategory> _listInternalAlertCategory=new List<AlertCategory>();
		private List<AlertCategory> _listCustomAlertCategory=new List<AlertCategory>();

		public FormAlertCategorySetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}
		
		private void FormAlertCategorySetup_Load(object sender,EventArgs e) {
			FillGrids();
		}

		private void FillGrids(long selectedIneranlKey=0,long selectedCustomKey=0) {
			_listCustomAlertCategory.Clear();
			_listInternalAlertCategory.Clear();
			AlertCategories.GetDeepCopy().ForEach(x =>
			{
				if(x.IsHQCategory) {
					_listInternalAlertCategory.Add(x);
				}
				else {
					_listCustomAlertCategory.Add(x);
				}
			});
			_listInternalAlertCategory.OrderBy(x => x.InternalName);
			_listCustomAlertCategory.OrderBy(x => x.InternalName);
			FillInternalGrid(selectedIneranlKey);
			FillCustomGrid(selectedCustomKey);
		}

		private void FillInternalGrid(long selectedIneranlKey) {
			gridInternal.BeginUpdate();
			gridInternal.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Description"),100);
			gridInternal.ListGridColumns.Add(col);
			gridInternal.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listInternalAlertCategory.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listInternalAlertCategory[i].Description);
				row.Tag=_listInternalAlertCategory[i].AlertCategoryNum;
				gridInternal.ListGridRows.Add(row);
				int index=gridInternal.ListGridRows.Count-1;
				if(selectedIneranlKey==_listInternalAlertCategory[i].AlertCategoryNum) {
					gridCustom.SetSelected(index,true);
				}
			}
			gridInternal.EndUpdate();
		}
		
		private void FillCustomGrid(long selectedCustomKey) {
			gridCustom.BeginUpdate();
			gridCustom.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Description"),100);
			gridCustom.ListGridColumns.Add(col);
			gridCustom.ListGridRows.Clear();
			GridRow row;
			int index=0;
			for(int i=0;i<_listCustomAlertCategory.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listCustomAlertCategory[i].Description);
				row.Tag=_listCustomAlertCategory[i].AlertCategoryNum;
				gridCustom.ListGridRows.Add(row);
				index=gridCustom.ListGridRows.Count-1;
				if(selectedCustomKey!=_listCustomAlertCategory[i].AlertCategoryNum) {
					index=0;
				}
			}
			if(index!=0) {
				gridCustom.SetSelected(index,true);
			}
			gridCustom.EndUpdate();
		}
		
		private void gridInternal_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormAlertCategoryEdit formACE=new FormAlertCategoryEdit(_listInternalAlertCategory[e.Row]);
			if(formACE.ShowDialog()==DialogResult.OK) {
				FillGrids();
			}
		}

		private void gridCustom_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormAlertCategoryEdit formACE=new FormAlertCategoryEdit(_listCustomAlertCategory[e.Row]);
			if(formACE.ShowDialog()==DialogResult.OK) {
				FillGrids();
			}
		}

		private void butCopy_Click(object sender,EventArgs e) {
			if(gridInternal.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select an internal alert category from the list first.");
				return;
			}
			InsertCopyAlertCategory(_listInternalAlertCategory[gridInternal.GetSelectedIndex()].Copy());
		}

		private void butDuplicate_Click(object sender,EventArgs e) {
			if(gridCustom.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select a custom alert category from the list first.");
				return;
			}
			InsertCopyAlertCategory(_listCustomAlertCategory[gridCustom.GetSelectedIndex()].Copy());
		}

		private void InsertCopyAlertCategory(AlertCategory alertCat) {
			alertCat.IsHQCategory=false;
			alertCat.Description+=Lan.g(this,"(Copy)");
			//alertCat.AlertCategoryNum reflects the original pre-copied PK. After Insert this will be a new PK for the new row.
			List<AlertCategoryLink> listAlertCategoryType=AlertCategoryLinks.GetForCategory(alertCat.AlertCategoryNum);
			alertCat.AlertCategoryNum=AlertCategories.Insert(alertCat);
			//At this point alertCat has a new PK, so we need to update and insert our new copied alertCategoryLinks
			listAlertCategoryType.ForEach(x => {
				x.AlertCategoryNum=alertCat.AlertCategoryNum;
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