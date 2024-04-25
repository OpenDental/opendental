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
		private Patient _patient;
		private List<AccountEntry> _listAccountEntries;

		public FormProdSelect(Patient patient,List<AccountEntry> listAccountEntries) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patient=patient;
			_listAccountEntries=listAccountEntries;
		}

		private void FormProdSelect_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			string tran=gridProduction.TranslationName;
			gridProduction.BeginUpdate();
			gridProduction.Title=Lan.g(tran,"Production for")+" "+_patient.GetNameFLnoPref();
			gridProduction.Columns.Clear();
			gridProduction.Columns.Add(new GridColumn(Lan.g(tran,"Date"),70,HorizontalAlignment.Center));
			gridProduction.Columns.Add(new GridColumn(Lan.g(tran,"Prov"),80));
			if(PrefC.HasClinicsEnabled) {
				gridProduction.Columns.Add(new GridColumn(Lan.g(tran,"Clinic"),80));
			}
			gridProduction.Columns.Add(new GridColumn(Lan.g(tran,"Status"),80));
			gridProduction.Columns.Add(new GridColumn(Lan.g(tran,"Type"),0) { IsWidthDynamic=true });
			gridProduction.Columns.Add(new GridColumn(Lan.g(tran,"Amount\r\nOriginal"),60,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridProduction.Columns.Add(new GridColumn(Lan.g(tran,"Amount\r\nEnd"),60,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridProduction.ListGridRows.Clear();
			List<Type> listProdTypes=new List<Type>() { typeof(Adjustment),typeof(Procedure) };
			for(int i=0;i<_listAccountEntries.Count;i++) {
				if(_listAccountEntries[i].PatNum!=_patient.PatNum || _listAccountEntries[i].AmountEnd==0 || !listProdTypes.Contains(_listAccountEntries[i].GetType())) {
					continue;
				}
				GridRow row=new GridRow();
				row.Cells.Add(_listAccountEntries[i].Date.ToShortDateString());
				row.Cells.Add(Providers.GetAbbr(_listAccountEntries[i].ProvNum,includeHidden:true));
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(_listAccountEntries[i].ClinicNum));
				}
				if(_listAccountEntries[i].GetType()==typeof(Procedure)) {
					Procedure procedure=(Procedure)_listAccountEntries[i].Tag;
					if(procedure.ProcStatus==ProcStat.TP) {
						row.Cells.Add(Lan.g(this,"Treatment Planned"));
					}
					else if(procedure.ProcStatus==ProcStat.C) {
						row.Cells.Add(Lan.g(this,"Completed"));
					}
					else {
						row.Cells.Add("");//shouldn't happen, but since we're not sure...
					}
					row.Cells.Add($"{ProcedureCodes.GetStringProcCode(procedure.CodeNum)}: {Procedures.GetDescription(procedure)}");
				}
				else if(_listAccountEntries[i].GetType()==typeof(Adjustment)) {
					Adjustment adjustment=(Adjustment)_listAccountEntries[i].Tag;
					row.Cells.Add(Lan.g(this,"Adjustment"));
					row.Cells.Add($"{Defs.GetName(DefCat.AdjTypes,adjustment.AdjType)}");
				}
				row.Cells.Add(_listAccountEntries[i].AmountOriginal.ToString("C"));
				row.Cells.Add(_listAccountEntries[i].AmountEnd.ToString("C"));
				row.Tag=_listAccountEntries[i];
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

	}
}