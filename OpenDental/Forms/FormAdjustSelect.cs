using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental {
	public partial class FormAdjustSelect:FormODBase {
		public Adjustment SelectedAdj;
		private List<PaySplit> _listSplitsCur;
		///<summary>The display the opposite amount of the amtPaySplitCur that was originally passed in.
		///This is because PaySplits are stored in the opposite of how they are applied.
		///E.g. A PaySplit stored as $30 will subtract $30 from something (and visa versa).</summary>
		private double _amtPaySplitCurDisplay;
		///<summary>Account entries made out of all adjustments that are not associated to a procedure for the patient.</summary>
		private List<AccountEntry> _listPatAdjEntries=new List<AccountEntry>();

		///<summary></summary>
		public FormAdjustSelect(double amtPaySplitCur,PaySplit paySplitCur,List<PaySplit> listSplitsCur,List<Adjustment> listPatAdjusts,
			List<PaySplit> listAdjPaySplits)
		{
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			//Flip the sign on the pay split amount passed in so that.
			_amtPaySplitCurDisplay=amtPaySplitCur * -1;
			_listSplitsCur=listSplitsCur;
			//Convert all unattached adjustments to account entries.
			_listPatAdjEntries=listPatAdjusts
				.FindAll(x => x.ProcNum==0)
				.Select(x => new AccountEntry(x))
				.ToList();
			foreach(AccountEntry entry in _listPatAdjEntries) {
				//Figure out how much each adjustment has left, not counting this payment.
				entry.AmountAvailable-=(decimal)Adjustments.GetAmtAllocated(entry.PriKey,paySplitCur.PayNum,
					listAdjPaySplits.FindAll(x => x.AdjNum==entry.PriKey));
				//Reduce adjustments based on current payment's splits as well but not the current split (could be new, could be modified).
				entry.AmountAvailable-=(decimal)Adjustments.GetAmtAllocated(entry.PriKey,0,
					_listSplitsCur.FindAll(x => x.AdjNum==entry.PriKey && x!=paySplitCur));
			}
		}

		private void FormAdjustSelect_Load(object sender,EventArgs e) {
			labelCurSplitAmt.Text=_amtPaySplitCurDisplay.ToString("C");
			FillGrid();
		}

		private void FillGrid() {
			gridAdjusts.BeginUpdate();
			gridAdjusts.ListGridRows.Clear();
			gridAdjusts.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableAdjustSelect","Date"),70,HorizontalAlignment.Center);
			gridAdjusts.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableAdjustSelect","Prov"),60){ IsWidthDynamic=true };
			gridAdjusts.ListGridColumns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g("TableAdjustSelect","Clinic"),60){ IsWidthDynamic=true };
				gridAdjusts.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g("TableAdjustSelect","Amt Orig"),60,HorizontalAlignment.Right);
			gridAdjusts.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableAdjustSelect","Amt Avail"),60,HorizontalAlignment.Right);
			gridAdjusts.ListGridColumns.Add(col);
			GridRow row;
			foreach(AccountEntry entry in _listPatAdjEntries) {
				row=new GridRow();
				row.Cells.Add(((Adjustment)entry.Tag).AdjDate.ToShortDateString());
				row.Cells.Add(Providers.GetAbbr(((Adjustment)entry.Tag).ProvNum));
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(((Adjustment)entry.Tag).ClinicNum));
				}
				row.Cells.Add(entry.AmountOriginal.ToString("F"));
				row.Cells.Add(entry.AmountAvailable.ToString("F"));
				row.Tag=entry;
				gridAdjusts.ListGridRows.Add(row);
			}
			gridAdjusts.EndUpdate();
		}

		private void gridAdjusts_CellClick(object sender,UI.ODGridClickEventArgs e) {
			AccountEntry entry=gridAdjusts.SelectedTag<AccountEntry>();
			labelAmtOriginal.Text=entry.AmountOriginal.ToString("C");
			//Figure out the amount that has already been used (orig - avail) and then flip the sign for display purposes (like _amtPaySplitCurDisplay).
			decimal amtUsedDisplay=(entry.AmountOriginal-entry.AmountAvailable) * -1;
			labelAmtUsed.Text=amtUsedDisplay.ToString("C");
			labelAmtAvail.Text=entry.AmountAvailable.ToString("C");
			labelAmtEnd.Text=((double)entry.AmountAvailable+_amtPaySplitCurDisplay).ToString("C");
		}

		private void gridAdjusts_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			SelectedAdj=(Adjustment)gridAdjusts.SelectedTag<AccountEntry>().Tag;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridAdjusts.SelectedIndices.Length<1) {
				MsgBox.Show(this,"Please select an adjustment first or press Cancel.");
				return;
			}
			SelectedAdj=(Adjustment)gridAdjusts.SelectedTag<AccountEntry>().Tag;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}