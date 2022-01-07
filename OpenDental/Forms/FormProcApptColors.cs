using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormProcApptColors:FormODBase {
		private List<ProcApptColor> _listProcApptColors;

		///<summary></summary>
		public FormProcApptColors()	{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormProcApptColors_Load(object sender,System.EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			ProcApptColors.RefreshCache();
			_listProcApptColors=ProcApptColors.GetDeepCopy();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormProcApptColors","Code Range"),20){ IsWidthDynamic=true };
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listProcApptColors.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listProcApptColors[i].CodeRange);
				row.ColorText=_listProcApptColors[i].ColorText;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormProcApptColorEdit FormPACE=new FormProcApptColorEdit();
			FormPACE.ProcApptColorCur=new ProcApptColor();
			FormPACE.ProcApptColorCur.IsNew=true;
			FormPACE.ShowDialog();
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormProcApptColorEdit FormP=new FormProcApptColorEdit();
			FormP.ProcApptColorCur=_listProcApptColors[e.Row];
			FormP.ShowDialog();
			FillGrid();
		}

		private void butClose_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		

		

		



		
	}
}





















