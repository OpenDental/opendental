using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormCodeGroups:FormODBase {
		private List<CodeGroup> _listCodeGroupsOld;
		private List<CodeGroup> _listCodeGroups;

		public FormCodeGroups() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormCodeGroups_Load(object sender,EventArgs e) {
			_listCodeGroupsOld=CodeGroups.GetDeepCopy();
			_listCodeGroups=_listCodeGroupsOld.Select(x => x.Copy()).ToList();
			FillGrid();
		}

		private void FillGrid() {
			string trans=gridMain.TranslationName;
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new GridColumn(Lan.g(trans,"Group Name"),0) { IsWidthDynamic=true,DynamicWeight=1 });
			gridMain.Columns.Add(new GridColumn(Lan.g(trans,"Fixed Group"),110));
			gridMain.Columns.Add(new GridColumn(Lan.g(trans,"Proc Codes"),0) { IsWidthDynamic=true,DynamicWeight=2 });
			if(checkShowHidden.Checked) {
				gridMain.Columns.Add(new GridColumn(Lan.g(trans,"Hidden"),50,HorizontalAlignment.Center));
			}
			CodeGroup codeGroupSelected=gridMain.SelectedTag<CodeGroup>();
			gridMain.ListGridRows.Clear();
			int index=-1;
			for(int i=0;i<_listCodeGroups.Count;i++) {
				if(!checkShowHidden.Checked && _listCodeGroups[i].IsHidden) {
					continue;
				}
				GridRow gridRow=new GridRow();
				gridRow.Cells.Add(_listCodeGroups[i].GroupName);
				string strCodeGroupFixed="";
				if(_listCodeGroups[i].CodeGroupFixed!=EnumCodeGroupFixed.None) {
					strCodeGroupFixed=_listCodeGroups[i].CodeGroupFixed.GetDescription();
				}
				gridRow.Cells.Add(strCodeGroupFixed);
				gridRow.Cells.Add(_listCodeGroups[i].ProcCodes);
				if(checkShowHidden.Checked) {
					gridRow.Cells.Add(_listCodeGroups[i].IsHidden?"X":"");
				}
				gridRow.Tag=_listCodeGroups[i];
				if(_listCodeGroups[i].Equals(codeGroupSelected)) {
					//Use the count before adding this row to the list because index is 0 based.
					index=gridMain.ListGridRows.Count;
				}
				gridMain.ListGridRows.Add(gridRow);
			}
			gridMain.EndUpdate();
			gridMain.SetSelected(index);
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormCodeGroupEdit formCodeGroupEdit=new FormCodeGroupEdit();
			formCodeGroupEdit.CodeGroup=new CodeGroup();
			formCodeGroupEdit.CodeGroup.IsNew=true;
			formCodeGroupEdit.ListEnumCodeGroupFixedsInUse=_listCodeGroups.Select(x => x.CodeGroupFixed).ToList();
			if(formCodeGroupEdit.ShowDialog()!=DialogResult.OK) {
				return;
			}
			if(!_listCodeGroups.IsNullOrEmpty()) {
				formCodeGroupEdit.CodeGroup.ItemOrder=_listCodeGroups.Max(x => x.ItemOrder) + 1;
			}
			_listCodeGroups.Add(formCodeGroupEdit.CodeGroup);
			FillGrid();
		}

		private void butUp_Click(object sender,EventArgs e) {
			CodeGroup codeGroupSelected=gridMain.SelectedTag<CodeGroup>();
			if(codeGroupSelected==null) {
				return;
			}
			//Only manipulate the ItemOrder values of the code groups that are visible to the user.
			List<CodeGroup> listCodeGroups=gridMain.GetTags<CodeGroup>();
			//Figure out where the selected code group falls within the grid.
			int index=listCodeGroups.IndexOf(codeGroupSelected);
			if(index<=0) {
				return;//Not found or first in the grid.
			}
			//Swap the ItemOrder with the code group directly above the selected code group.
			CodeGroup codeGroupSwap=listCodeGroups[index - 1];
			//Swap places with the swappable code group found.
			int itemOrder=codeGroupSelected.ItemOrder;
			codeGroupSelected.ItemOrder=codeGroupSwap.ItemOrder;
			codeGroupSwap.ItemOrder=itemOrder;
			//Physically reorder the list of code groups now that the ItemOrder values have been manipulated.
			_listCodeGroups=_listCodeGroups.OrderBy(x => x.ItemOrder).ToList();
			FillGrid();
		}

		private void butDown_Click(object sender,EventArgs e) {
			CodeGroup codeGroupSelected=gridMain.SelectedTag<CodeGroup>();
			if(codeGroupSelected==null) {
				return;
			}
			//Only manipulate the ItemOrder values of the code groups that are visible to the user.
			List<CodeGroup> listCodeGroups=gridMain.GetTags<CodeGroup>();
			//Figure out where the selected code group falls within the grid.
			int index=listCodeGroups.IndexOf(codeGroupSelected);
			if(index==-1 || index>=(listCodeGroups.Count - 1)) {
				return;//Not found or last in the grid.
			}
			//Swap the ItemOrder with the code group directly below the selected code group.
			CodeGroup codeGroupSwap=listCodeGroups[index + 1];
			//Swap places with the swappable code group found.
			int itemOrder=codeGroupSelected.ItemOrder;
			codeGroupSelected.ItemOrder=codeGroupSwap.ItemOrder;
			codeGroupSwap.ItemOrder=itemOrder;
			//Physically reorder the list of code groups now that the ItemOrder values have been manipulated.
			_listCodeGroups=_listCodeGroups.OrderBy(x => x.ItemOrder).ToList();
			FillGrid();
		}

		private void checkShowHidden_CheckedChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			using FormCodeGroupEdit formCodeGroupEdit=new FormCodeGroupEdit();
			formCodeGroupEdit.CodeGroup=gridMain.SelectedTag<CodeGroup>();
			formCodeGroupEdit.ListEnumCodeGroupFixedsInUse=_listCodeGroups.Select(x => x.CodeGroupFixed).ToList();
			if(formCodeGroupEdit.ShowDialog()!=DialogResult.OK) {
				return;
			}
			FillGrid();
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(CodeGroups.Sync(_listCodeGroups,_listCodeGroupsOld)) {
				DataValid.SetInvalid(InvalidType.CodeGroups);
			}
			DialogResult=DialogResult.OK;
		}
	}
}