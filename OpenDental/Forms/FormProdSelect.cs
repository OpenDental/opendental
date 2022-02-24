using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>A window for displaying and selecting outstanding production that utilizes some of the same logic from the income transfer system.</summary>
	public partial class FormProdSelect:FormODBase {
		public List<AccountEntry> ListSelectedEntries;
		private Patient _patCur;
		private Family _famCur;

		public FormProdSelect(Patient patCur,Family famCur) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patCur=patCur;
			_famCur=famCur;
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
			//Gather account information as if an income transfer is about to be made so that explicit linking is the only type of linking performed.
			PaymentEdit.ConstructResults constructResults=PaymentEdit.ConstructAndLinkChargeCredits(_famCur.GetPatNums(),_patCur.PatNum,
				new List<PaySplit>(),new Payment(),new List<AccountEntry>(),isIncomeTxfr:true,doIncludeTreatmentPlanned:true);
			List<Type> listProdTypes=new List<Type>() { typeof(Adjustment),typeof(Procedure) };
			foreach(AccountEntry accountEntry in constructResults.ListAccountCharges) {
				if(accountEntry.PatNum!=_patCur.PatNum || accountEntry.AmountEnd==0 || !listProdTypes.Contains(accountEntry.GetType())) {
					continue;
				}
				GridRow row=new GridRow();
				row.Cells.Add(accountEntry.Date.ToShortDateString());
				row.Cells.Add(Providers.GetAbbr(accountEntry.ProvNum,includeHidden:true));
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(accountEntry.ClinicNum));
				}
				if(accountEntry.GetType()==typeof(Procedure)) {
					Procedure proc=(Procedure)accountEntry.Tag;
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
				else if(accountEntry.GetType()==typeof(Adjustment)) {
					Adjustment adj=(Adjustment)accountEntry.Tag;
					row.Cells.Add(Lan.g(this,"Adjustment"));
					row.Cells.Add($"{Defs.GetName(DefCat.AdjTypes,adj.AdjType)}");
				}
				row.Cells.Add(accountEntry.AmountOriginal.ToString("C"));
				row.Cells.Add(accountEntry.AmountEnd.ToString("C"));
				row.Tag=accountEntry;
				gridProduction.ListGridRows.Add(row);
			}
			gridProduction.EndUpdate();
		}

		private void gridProduction_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			ListSelectedEntries=gridProduction.SelectedTags<AccountEntry>();
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridProduction.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			ListSelectedEntries=gridProduction.SelectedTags<AccountEntry>();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}