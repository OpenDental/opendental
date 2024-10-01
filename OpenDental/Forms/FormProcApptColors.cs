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
		public FormProcApptColors() {
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
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormProcApptColors","Code Range"),20){ IsWidthDynamic=true };
			gridMain.Columns.Add(col);
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
			using FormProcApptColorEdit formProcApptColorEdit=new FormProcApptColorEdit();
			formProcApptColorEdit.ProcApptColorCur=new ProcApptColor();
			formProcApptColorEdit.ProcApptColorCur.IsNew=true;
			formProcApptColorEdit.ShowDialog();
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormProcApptColorEdit formProcApptColorEdit=new FormProcApptColorEdit();
			formProcApptColorEdit.ProcApptColorCur=_listProcApptColors[e.Row];
			formProcApptColorEdit.ShowDialog();
			FillGrid();
		}

	}
}