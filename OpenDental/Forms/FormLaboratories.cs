using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormLaboratories : FormODBase {
		//private bool changed;
		private List<Laboratory> ListLabs;

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
			ListLabs=Laboratories.Refresh();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableLabs","Description"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableLabs","Phone"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableLabs","Hidden"),50,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableLabs","Notes"),200);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ListLabs.Count;i++){
				row=new GridRow();
				row.Cells.Add(ListLabs[i].Description);
				row.Cells.Add(ListLabs[i].Phone);
				row.Cells.Add(ListLabs[i].IsHidden?"X":"");
				row.Cells.Add(ListLabs[i].Notes);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormLaboratoryEdit FormL=new FormLaboratoryEdit();
			FormL.LabCur=ListLabs[e.Row];
			FormL.ShowDialog();
			//if(FormL.DialogResult==DialogResult.OK){
				//changed=true;
			FillGrid();
			//}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormLaboratoryEdit FormL=new FormLaboratoryEdit();
			FormL.LabCur=new Laboratory();
			FormL.IsNew=true;
			FormL.ShowDialog();
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





















