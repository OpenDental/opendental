using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental {
	public partial class FormAdjustSelect:FormODBase {
		public Adjustment AdjustmentSelected;
		private List<PaySplit> _listPaySplits;
		///<summary>The display the opposite amount of the amtPaySplitCur that was originally passed in.
		///This is because PaySplits are stored in the opposite of how they are applied.
		///E.g. A PaySplit stored as $30 will subtract $30 from something (and visa versa).</summary>
		private double _amtPaySplitDisplay;
		///<summary>Account entries made out of all adjustments that are not associated to a procedure for the patient.</summary>
		private List<AccountEntry> _listAccountEntriesAdjPat=new List<AccountEntry>();

		///<summary></summary>
		public FormAdjustSelect(double amtPaySplit,PaySplit paySplit,List<PaySplit> listPaySplits,List<Adjustment> listAdjustmentsPat,
			List<PaySplit> listPaySplitsAdj)
		{
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			//Flip the sign on the pay split amount passed in so that.
			_amtPaySplitDisplay=amtPaySplit * -1;
			_listPaySplits=listPaySplits;
			//Convert all unattached adjustments to account entries.
			for(int i=0;i<listAdjustmentsPat.Count;i++) {
				if(listAdjustmentsPat[i].ProcNum!=0) {
					continue;
				}
				_listAccountEntriesAdjPat.Add(new AccountEntry(listAdjustmentsPat[i]));
			}
			for(int i=0;i<_listAccountEntriesAdjPat.Count;i++) {
				//Figure out how much each adjustment has left, not counting this payment.
				_listAccountEntriesAdjPat[i].AmountAvailable-=(decimal)Adjustments.GetAmtAllocated(_listAccountEntriesAdjPat[i].PriKey,paySplit.PayNum,
					listPaySplitsAdj.FindAll(x => x.AdjNum==_listAccountEntriesAdjPat[i].PriKey));
				//Reduce adjustments based on current payment's splits as well but not the current split (could be new, could be modified).
				_listAccountEntriesAdjPat[i].AmountAvailable-=(decimal)Adjustments.GetAmtAllocated(_listAccountEntriesAdjPat[i].PriKey,0,
					_listPaySplits.FindAll(x => x.AdjNum==_listAccountEntriesAdjPat[i].PriKey && x!=paySplit));
			}
		}

		private void FormAdjustSelect_Load(object sender,EventArgs e) {
			labelCurSplitAmt.Text=_amtPaySplitDisplay.ToString("C");
			FillGrid();
		}

		private void FillGrid() {
			gridAdjusts.BeginUpdate();
			gridAdjusts.ListGridRows.Clear();
			gridAdjusts.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableAdjustSelect","Date"),70,HorizontalAlignment.Center);
			gridAdjusts.Columns.Add(col);
			col=new GridColumn(Lan.g("TableAdjustSelect","Prov"),60){ IsWidthDynamic=true };
			gridAdjusts.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g("TableAdjustSelect","Clinic"),60){ IsWidthDynamic=true };
				gridAdjusts.Columns.Add(col);
			}
			col=new GridColumn(Lan.g("TableAdjustSelect","Amt Orig"),60,HorizontalAlignment.Right);
			gridAdjusts.Columns.Add(col);
			col=new GridColumn(Lan.g("TableAdjustSelect","Amt Avail"),60,HorizontalAlignment.Right);
			gridAdjusts.Columns.Add(col);
			GridRow row;
			for(int i=0;i<_listAccountEntriesAdjPat.Count;i++) {
				row=new GridRow();
				row.Cells.Add(((Adjustment)_listAccountEntriesAdjPat[i].Tag).AdjDate.ToShortDateString());
				row.Cells.Add(Providers.GetAbbr(((Adjustment)_listAccountEntriesAdjPat[i].Tag).ProvNum));
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(((Adjustment)_listAccountEntriesAdjPat[i].Tag).ClinicNum));
				}
				row.Cells.Add(_listAccountEntriesAdjPat[i].AmountOriginal.ToString("F"));
				row.Cells.Add(_listAccountEntriesAdjPat[i].AmountAvailable.ToString("F"));
				row.Tag=_listAccountEntriesAdjPat[i];
				gridAdjusts.ListGridRows.Add(row);
			}
			gridAdjusts.EndUpdate();
		}

		private void gridAdjusts_CellClick(object sender,UI.ODGridClickEventArgs e) {
			AccountEntry accountEntry=gridAdjusts.SelectedTag<AccountEntry>();
			labelAmtOriginal.Text=accountEntry.AmountOriginal.ToString("C");
			//Figure out the amount that has already been used (orig - avail) and then flip the sign for display purposes (like _amtPaySplitCurDisplay).
			decimal amtUsedDisplay=(accountEntry.AmountOriginal-accountEntry.AmountAvailable) * -1;
			labelAmtUsed.Text=amtUsedDisplay.ToString("C");
			labelAmtAvail.Text=accountEntry.AmountAvailable.ToString("C");
			labelAmtEnd.Text=((double)accountEntry.AmountAvailable+_amtPaySplitDisplay).ToString("C");
		}

		private void gridAdjusts_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			AdjustmentSelected=(Adjustment)gridAdjusts.SelectedTag<AccountEntry>().Tag;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridAdjusts.SelectedIndices.Length<1) {
				MsgBox.Show(this,"Please select an adjustment first or press Cancel.");
				return;
			}
			AdjustmentSelected=(Adjustment)gridAdjusts.SelectedTag<AccountEntry>().Tag;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}