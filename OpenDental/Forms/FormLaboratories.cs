using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormLaboratories : FormODBase {
		//private bool changed;
		private List<Laboratory> _listLaboratories;

		///<summary></summary>
		public FormLaboratories()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormLaboratories_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			_listLaboratories=Laboratories.Refresh();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableLabs","Description"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableLabs","Phone"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableLabs","Hidden"),50,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableLabs","Notes"),200);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listLaboratories.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listLaboratories[i].Description);
				row.Cells.Add(_listLaboratories[i].Phone);
				row.Cells.Add(_listLaboratories[i].IsHidden?"X":"");
				row.Cells.Add(_listLaboratories[i].Notes);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormLaboratoryEdit formLaboratoryEdit=new FormLaboratoryEdit();
			formLaboratoryEdit.LaboratoryCur=_listLaboratories[e.Row];
			formLaboratoryEdit.ShowDialog();
			//if(FormL.DialogResult==DialogResult.OK){
				//changed=true;
			FillGrid();
			//}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormLaboratoryEdit formLaboratoryEdit=new FormLaboratoryEdit();
			formLaboratoryEdit.LaboratoryCur=new Laboratory();
			formLaboratoryEdit.IsNew=true;
			formLaboratoryEdit.ShowDialog();
			FillGrid();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormLaboratories_FormClosing(object sender,FormClosingEventArgs e) {
			//if(changed){
				//Labs are not global.
				//DataValid.SetInvalid(InvalidTypes.Providers);
			//}
		}

		

		

		


	}
}





















