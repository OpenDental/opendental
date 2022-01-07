using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;

namespace OpenDental.Bridges {
	public partial class FormTrophyNamePick:FormODBase {
		public List<TrophyFolder> ListMatches;
		///<summary>If dialogResult is OK, then this will contain the picked folder name.  If blank, then the calling class will need to generate a new folder name.</summary>
		public string PickedName;

		public FormTrophyNamePick() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormTrophyNamePick_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormTrophyNamePick","FolderName"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormTrophyNamePick","Last Name"),120);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormTrophyNamePick","First Name"),120);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormTrophyNamePick","Birthdate"),80);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ListMatches.Count;i++) {
				row=new GridRow();
				row.Cells.Add(ListMatches[i].FolderName);
				row.Cells.Add(ListMatches[i].LName);
				row.Cells.Add(ListMatches[i].FName);
				row.Cells.Add(ListMatches[i].BirthDate.ToShortDateString());
				row.Tag=ListMatches[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butNew_Click(object sender,EventArgs e) {
			PickedName="";
			DialogResult=DialogResult.OK;
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			PickedName=gridMain.SelectedTag<TrophyFolder>().FolderName;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select an item from the list first.");
				return;
			}
			PickedName=gridMain.SelectedTag<TrophyFolder>().FolderName;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		
	}
}