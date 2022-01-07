using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>A window for displaying and selecting outstanding production that utilizes some of the same logic from the income transfer system.</summary>
	public partial class FormProdSelect:FormODBase {
		public List<AccountEntry> ListAccountEntriesSelected;
		private Patient _patCur;
		private List<AccountEntry> _listAccountEntriesAll;

		public FormProdSelect(Patient patCur,List<AccountEntry> listAccountEntries) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patCur=patCur;
			_listAccountEntriesAll=listAccountEntries;
		}

		private void FormProdSelect_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			string tran=gridProduction.TranslationName;
			gridProduction.BeginUpdate();
			gridProduction.Title=Lan.g(tran,"Production for")+" "+_patCur.GetNameFLnoPref();
			gridProduction.ListGridColumns.Clear();
			gridProduction.ListGridColumns.Add(new GridColumn(Lan.g(tran,"Date"),70,HorizontalAlignment.Center));
			gridProduction.ListGridColumns.Add(new GridColumn(Lan.g(tran,"Prov"),80));
			if(PrefC.HasClinicsEnabled) {
				gridProduction.ListGridColumns.Add(new GridColumn(Lan.g(tran,"Clinic"),80));
			}
			gridProduction.ListGridColumns.Add(new GridColumn(Lan.g(tran,"Status"),80));
			gridProduction.ListGridColumns.Add(new GridColumn(Lan.g(tran,"Type"),0) { IsWidthDynamic=true });
			gridProduction.ListGridColumns.Add(new GridColumn(Lan.g(tran,"Amount\r\nOriginal"),60,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridProduction.ListGridColumns.Add(new GridColumn(Lan.g(tran,"Amount\r\nEnd"),60,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridProduction.ListGridRows.Clear();
			List<Type> listProdTypes=new List<Type>() { typeof(Adjustment),typeof(Procedure) };
			for(int i=0;i<_listAccountEntriesAll.Count;i++) {
				if(_listAccountEntriesAll[i].PatNum!=_patCur.PatNum || _listAccountEntriesAll[i].AmountEnd==0 || !listProdTypes.Contains(_listAccountEntriesAll[i].GetType())) {
					continue;
				}
				GridRow row=new GridRow();
				row.Cells.Add(_listAccountEntriesAll[i].Date.ToShortDateString());
				row.Cells.Add(Providers.GetAbbr(_listAccountEntriesAll[i].ProvNum,includeHidden:true));
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(_listAccountEntriesAll[i].ClinicNum));
				}
				if(_listAccountEntriesAll[i].GetType()==typeof(Procedure)) {
					Procedure proc=(Procedure)_listAccountEntriesAll[i].Tag;
					if(proc.ProcStatus==ProcStat.TP) {
						row.Cells.Add(Lan.g(this,"Treatment Planned"));
					}
					else if(proc.ProcStatus==ProcStat.C) {
						row.Cells.Add(Lan.g(this,"Completed"));
					}
					else {
						row.Cells.Add("");//shouldn't happen, but since we're not sure...
					}
					row.Cells.Add($"{ProcedureCodes.GetStringProcCode(proc.CodeNum)} - {ProcedureCodes.GetLaymanTerm(proc.CodeNum)}");
				}
				else if(this._listAccountEntriesAll[i].GetType()==typeof(Adjustment)) {
					Adjustment adj=(Adjustment)this._listAccountEntriesAll[i].Tag;
					row.Cells.Add(Lan.g(this,"Adjustment"));
					row.Cells.Add($"{Defs.GetName(DefCat.AdjTypes,adj.AdjType)}");
				}
				row.Cells.Add(this._listAccountEntriesAll[i].AmountOriginal.ToString("C"));
				row.Cells.Add(this._listAccountEntriesAll[i].AmountEnd.ToString("C"));
				row.Tag=this._listAccountEntriesAll[i];
				gridProduction.ListGridRows.Add(row);
			}
			gridProduction.EndUpdate();
		}

		private void gridProduction_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			ListAccountEntriesSelected=gridProduction.SelectedTags<AccountEntry>();
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridProduction.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			ListAccountEntriesSelected=gridProduction.SelectedTags<AccountEntry>();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}