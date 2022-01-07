using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using CodeBase;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormProcSelect : FormODBase {
		#region Private Variables
		private long _patNumCur;
		///<summary>A list of completed procedures that are associated to this patient or their payment plans.</summary>
		private List<Procedure> _listProcedures;
		private List<PaySplit> _listPaySplits;
		private List<Adjustment> _listAdjustments;
		private List<PayPlanCharge> _listPayPlanCharges;
		private List<ClaimProc> _listInsPayAsTotal;
		private List<ClaimProc> _listClaimProcs;
		private List<AccountEntry> _listAccountCharges;
		///<summary>Does not perform FIFO logic.</summary>
		private bool _isSimpleView;
		///<summary>Set to true to enable multiple procedure selection mode.</summary>
		private bool _isMultiSelect;
		private Label labelUnallocated;
		private bool _doShowUnallocatedLabel;
		private bool _doShowAdjustments;
		private bool _doShowTreatmentPlanProcs;
		#endregion

		#region Public Variables
		///<summary>If form closes with OK, this contains selected proc num.</summary>
		public List<Procedure> ListSelectedProcs=new List<Procedure>();
		///<summary>List of paysplits for the current payment.</summary>
		public List<PaySplit> ListSplitsCur=new List<PaySplit>();
		public bool isPrepayAllowedForTpProcs=PrefC.GetYN(PrefName.PrePayAllowedForTpProcs);
		public List<AccountEntry> ListAccountEntries=new List<AccountEntry>();
		#endregion

		///<summary>Displays completed procedures for the passed-in pat. 
		///Pass in true for isSimpleView to show all completed procedures, 
		///otherwise the user will be able to pick between credit allocation strategies (FIFO, Explicit, All).</summary>
		public FormProcSelect(long patNum,bool isSimpleView,bool isMultiSelect=false,bool doShowUnallocatedLabel=false,bool doShowAdjustments=false,bool doShowTreatmentPlanProcs=true) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patNumCur=patNum;
			_isSimpleView=isSimpleView;
			_isMultiSelect=isMultiSelect;
			_doShowUnallocatedLabel=doShowUnallocatedLabel;
			_doShowAdjustments=doShowAdjustments;
			if(_doShowAdjustments) {
				gridMain.Title=Lan.g(gridMain.TranslationName,"Account Entries");
				this.Text=Lan.g(this,"Select Account Entries");
			}
			_doShowTreatmentPlanProcs=doShowTreatmentPlanProcs;
		}

		private void FormProcSelect_Load(object sender,System.EventArgs e) {
			if(_isMultiSelect) {
				gridMain.SelectionMode=OpenDental.UI.GridSelectionMode.MultiExtended;
			}
			_listProcedures=Procedures.GetCompleteForPats(new List<long> { _patNumCur });
			if(isPrepayAllowedForTpProcs && _doShowTreatmentPlanProcs) {
				_listProcedures.AddRange(Procedures.GetTpForPats(new List<long> {_patNumCur}));
			}
			_listProcedures.RemoveAll(x => x.PatNum!=_patNumCur);
			_listAdjustments=Adjustments.GetAdjustForPats(new List<long> { _patNumCur });
			_listPayPlanCharges=PayPlanCharges.GetDueForPayPlans(PayPlans.GetForPats(null,_patNumCur),_patNumCur).ToList();//Does not get charges for the future.
			_listPaySplits=PaySplits.GetForPats(new List<long> { _patNumCur });//Might contain payplan payments.
			foreach(PaySplit split in ListSplitsCur) {
				//If this is a new payment, its paysplits will not be in the database yet, so we need to add them manually. We might also need to set the
				//ProcNum on the pay split if it has changed and has not been saved to the database.
				PaySplit splitDb=_listPaySplits.FirstOrDefault(x => x.IsSame(split));
				if(splitDb==null) {
					_listPaySplits.Add(split);
				}
				else {
					splitDb.ProcNum=split.ProcNum;
				}
			}
			_listInsPayAsTotal=ClaimProcs.GetByTotForPats(new List<long> { _patNumCur });
			_listClaimProcs=ClaimProcs.GetForProcs(_listProcedures.Select(x => x.ProcNum).ToList());
			labelUnallocated.Visible=_doShowUnallocatedLabel;
			if(PrefC.GetInt(PrefName.RigorousAdjustments)==(int)RigorousAdjustments.DontEnforce) {
				radioIncludeAllCredits.Checked=true;
			}
			else {
				radioOnlyAllocatedCredits.Checked=true;
			}
			FillGrid();
		}

		private void FillGrid(){
			CreditCalcType credCalc;
			if(_isSimpleView) {
				credCalc = CreditCalcType.ExcludeAll;
				groupBreakdown.Visible=false;
				groupCreditLogic.Visible=false;
			}
			else if(radioIncludeAllCredits.Checked) {
				credCalc = CreditCalcType.IncludeAll;
			}
			else if(radioOnlyAllocatedCredits.Checked) {
				credCalc = CreditCalcType.AllocatedOnly;
			}
			else {
				credCalc= CreditCalcType.ExcludeAll;
			}
			_listAccountCharges=AccountModules.GetListUnpaidAccountCharges(_listProcedures, _listAdjustments,
				_listPaySplits, _listClaimProcs, _listPayPlanCharges, _listInsPayAsTotal, credCalc, ListSplitsCur);
			List<Def> listPosAdjDefs=Defs.GetPositiveAdjTypes();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableProcSelect","Date"),70,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcSelect","Prov"),55);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcSelect","Code"),55);
			gridMain.ListGridColumns.Add(col);
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				col=new GridColumn(Lan.g("TableProcSelect","Description"),290);
				gridMain.ListGridColumns.Add(col);
			}
			else {
				col=new GridColumn(Lan.g("TableProcSelect","Tooth"),40);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g("TableProcSelect","Description"),250);
				gridMain.ListGridColumns.Add(col);
			}
			if(credCalc == CreditCalcType.ExcludeAll) {
				col=new GridColumn(Lan.g("TableProcSelect","Amt"),40,HorizontalAlignment.Right){ IsWidthDynamic=true };
				gridMain.ListGridColumns.Add(col);
			}
			else {
				col=new GridColumn(Lan.g("TableProcSelect","Amt Orig"),90,HorizontalAlignment.Right);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g("TableProcSelect","Amt End"),90,HorizontalAlignment.Right);
				gridMain.ListGridColumns.Add(col);
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			foreach(AccountEntry entry in _listAccountCharges) {
				if((!ListTools.In(entry.GetType(),typeof(ProcExtended),typeof(Adjustment)) || Math.Round(entry.AmountEnd,3) == 0) && credCalc!=CreditCalcType.ExcludeAll) {
					continue;
				}
				string dateStr;
				string procCodeStr="";
				string toothNumStr="";
				string descriptionText="";
				if(entry.GetType()==typeof(Adjustment)) {
					if(!_doShowAdjustments || entry.AmountEnd<=0 || entry.ProcNum>0) {
						//Do not show adjustments or do show adjustments- must not be attached to a procedure or the amount end must be positive
						continue;
					}
					dateStr=entry.Date.ToShortDateString();
					Def adjType=listPosAdjDefs.FirstOrDefault(x => x.DefNum==((Adjustment)entry.Tag).AdjType);
					descriptionText=adjType?.ItemName??Lan.g(this,"Adjustment");
				}
				else {//ProcExtended
					Procedure procCur=((ProcExtended)entry.Tag).Proc;
					ProcedureCode procCodeCur = ProcedureCodes.GetProcCode(procCur.CodeNum);
					dateStr=procCur.ProcDate.ToShortDateString();
					procCodeStr=procCodeCur.ProcCode;
					toothNumStr=procCur.ToothNum=="" ? Tooth.SurfTidyFromDbToDisplay(procCur.Surf,procCur.ToothNum) : Tooth.ToInternat(procCur.ToothNum);
					if(procCur.ProcStatus==ProcStat.TP) {
						descriptionText="(TP) ";
					}
					descriptionText+=procCodeCur.Descript;
				}
				row=new GridRow();
				row.Cells.Add(dateStr);
				row.Cells.Add(Providers.GetAbbr(entry.ProvNum));
				row.Cells.Add(procCodeStr);
				if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
					row.Cells.Add(toothNumStr);
				}
				row.Cells.Add(descriptionText);
				row.Cells.Add(entry.AmountOriginal.ToString("f"));
				if(credCalc != CreditCalcType.ExcludeAll) {
					row.Cells.Add(entry.AmountEnd.ToString("f"));
				}
				row.Tag=entry;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			if(!_isSimpleView) {
				RefreshBreakdown();
			}
		}

		private void RefreshBreakdown() {
			if(gridMain.GetSelectedIndex()==-1) {
				labelAmtOriginal.Text=(0).ToString("c");
				labelPositiveAdjs.Text=(0).ToString("c");
				labelNegativeAdjs.Text=(0).ToString("c");
				labelPayPlanCredits.Text=(0).ToString("c");
				labelPaySplits.Text=(0).ToString("c");
				labelInsEst.Text=(0).ToString("c");
				labelInsPay.Text=(0).ToString("c");
				labelWriteOff.Text=(0).ToString("c");
				labelWriteOffEst.Text=(0).ToString("c");
				labelCurrentSplits.Text=(0).ToString("c");
				labelAmtEnd.Text=(0).ToString("c");
				return;
			}
			//there could be more than one proc selected if IsMultiSelect = true.
			List<AccountEntry> listSelectedEntries=gridMain.SelectedTags<AccountEntry>();
			List<ProcExtended> listSelectedProcExts=listSelectedEntries.Where(x => x.Tag.GetType()==typeof(ProcExtended)).Select(x => (ProcExtended)x.Tag).ToList();
			List<AccountEntry> listSelectedPosAdjs=listSelectedEntries.FindAll(x => x.Tag.GetType()==typeof(Adjustment));
			decimal posAdjAmt=listSelectedProcExts.Sum(x => (decimal)x.PositiveAdjTotal)+listSelectedPosAdjs.Sum(x => x.AmountEnd);
			labelAmtOriginal.Text=    listSelectedProcExts.Sum(x => x.AmountOriginal).ToString("c");
			labelPositiveAdjs.Text=   posAdjAmt.ToString("c");
			labelNegativeAdjs.Text=   listSelectedProcExts.Sum(x => x.NegativeAdjTotals).ToString("c");
			labelPayPlanCredits.Text= (-listSelectedProcExts.Sum(x => x.PayPlanCreditTotal)).ToString("c");
			labelPaySplits.Text=      (-listSelectedProcExts.Sum(x => x.PaySplitTotal)).ToString("c");
			labelInsEst.Text=         (-listSelectedProcExts.Sum(x => x.InsEstTotal)).ToString("c");
			labelInsPay.Text=         (-listSelectedProcExts.Sum(x => x.InsPayTotal)).ToString("c");
			labelWriteOff.Text=       (-listSelectedProcExts.Sum(x => x.WriteOffTotal)).ToString("c");
			labelWriteOffEst.Text=    (-listSelectedProcExts.Sum(x => x.WriteOffEstTotal)).ToString("c");
			labelCurrentSplits.Text=  (-listSelectedProcExts.Sum(x => x.SplitsCurTotal)).ToString("c");
			labelAmtEnd.Text=         listSelectedEntries.Sum(y => y.AmountEnd).ToString("c");
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			RefreshBreakdown();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			SetSelectedAccountEntries();
			DialogResult=DialogResult.OK;
		}

		///<summary>Sets the public field lists with the selected grid items. ListSelectedProcs and ListAccountEntries.</summary>
		private void SetSelectedAccountEntries() {
			ListAccountEntries=new List<AccountEntry>();
			List<AccountEntry> listSelectedAccountEntries=gridMain.SelectedTags<AccountEntry>();
			for(int i=0;i<listSelectedAccountEntries.Count;i++) {
				if(listSelectedAccountEntries[i].GetType()==typeof(ProcExtended)) {
					Procedure procedureSelected=((ProcExtended)listSelectedAccountEntries[i].Tag).Proc;
					ListSelectedProcs.Add(procedureSelected);
					ListAccountEntries.Add(new AccountEntry(procedureSelected));
				}
				else {
					//Adjustment selected. Don't add to ListSelectedProcs
					ListAccountEntries.Add(listSelectedAccountEntries[i]);
				}
			}
		}

		private void radioCreditCalc_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			SetSelectedAccountEntries();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
    }
	}
}





















