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
		private long _patNum;
		///<summary>A list of completed procedures that are associated to this patient or their payment plans.</summary>
		private List<Procedure> _listProcedures;
		private List<PaySplit> _listPaySplits;
		private List<Adjustment> _listAdjustments;
		private List<PayPlanCharge> _listPayPlanCharges;
		private List<ClaimProc> _listClaimProcsInsPayAsTotals;
		private List<ClaimProc> _listClaimProcs;
		private List<AccountEntry> _listAccountEntries;
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
		public List<Procedure> ListProceduresSelected=new List<Procedure>();
		///<summary>List of paysplits for the current payment.</summary>
		public List<PaySplit> ListPaySplits=new List<PaySplit>();
		public bool IsPrepayAllowedForTpProcs=PrefC.GetYN(PrefName.PrePayAllowedForTpProcs);
		public List<AccountEntry> ListAccountEntries=new List<AccountEntry>();
		#endregion

		///<summary>Displays completed procedures for the passed-in pat. 
		///Pass in true for isSimpleView to show all completed procedures, 
		///otherwise the user will be able to pick between credit allocation strategies (FIFO, Explicit, All).</summary>
		public FormProcSelect(long patNum,bool isSimpleView,bool isMultiSelect=false,bool doShowUnallocatedLabel=false,bool doShowAdjustments=false,bool doShowTreatmentPlanProcs=true) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patNum=patNum;
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
			_listProcedures=Procedures.GetCompleteForPats(new List<long> { _patNum });
			if(IsPrepayAllowedForTpProcs && _doShowTreatmentPlanProcs) {
				_listProcedures.AddRange(Procedures.GetTpForPats(new List<long> {_patNum}));
			}
			_listProcedures.RemoveAll(x => x.PatNum!=_patNum);
			_listAdjustments=Adjustments.GetAdjustForPats(new List<long> { _patNum });
			_listPayPlanCharges=PayPlanCharges.GetDueForPayPlans(PayPlans.GetForPats(null,_patNum),_patNum).ToList();//Does not get charges for the future.
			_listPaySplits=PaySplits.GetForPats(new List<long> { _patNum });//Might contain payplan payments.
			for(int i=0;i<ListPaySplits.Count;i++) {
				//If this is a new payment, its paysplits will not be in the database yet, so we need to add them manually. We might also need to set the
				//ProcNum on the pay split if it has changed and has not been saved to the database.
				PaySplit paySplit=_listPaySplits.FirstOrDefault(x => x.IsSame(ListPaySplits[i]));
				if(paySplit==null) {
					_listPaySplits.Add(ListPaySplits[i]);
					continue;
				}
				paySplit.ProcNum=ListPaySplits[i].ProcNum;
			}
			_listClaimProcsInsPayAsTotals=ClaimProcs.GetByTotForPats(new List<long> { _patNum });
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
			CreditCalcType creditCalcType;
			if(_isSimpleView) {
				creditCalcType = CreditCalcType.ExcludeAll;
				groupBreakdown.Visible=false;
				groupCreditLogic.Visible=false;
			}
			else if(radioIncludeAllCredits.Checked) {
				creditCalcType = CreditCalcType.IncludeAll;
			}
			else if(radioOnlyAllocatedCredits.Checked) {
				creditCalcType = CreditCalcType.AllocatedOnly;
			}
			else {
				creditCalcType= CreditCalcType.ExcludeAll;
			}
			_listAccountEntries=AccountModules.GetListUnpaidAccountCharges(_listProcedures, _listAdjustments,
				_listPaySplits, _listClaimProcs, _listPayPlanCharges, _listClaimProcsInsPayAsTotals, creditCalcType, ListPaySplits);
			List<Def> listDefsPosAdj=Defs.GetPositiveAdjTypes();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableProcSelect","Date"),70,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcSelect","Prov"),55);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcSelect","Code"),55);
			gridMain.Columns.Add(col);
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				col=new GridColumn(Lan.g("TableProcSelect","Description"),290);
				gridMain.Columns.Add(col);
			}
			else {
				col=new GridColumn(Lan.g("TableProcSelect","Tooth"),40);
				gridMain.Columns.Add(col);
				col=new GridColumn(Lan.g("TableProcSelect","Description"),250);
				gridMain.Columns.Add(col);
			}
			if(creditCalcType == CreditCalcType.ExcludeAll) {
				col=new GridColumn(Lan.g("TableProcSelect","Amt"),40,HorizontalAlignment.Right){ IsWidthDynamic=true };
				gridMain.Columns.Add(col);
			}
			else {
				col=new GridColumn(Lan.g("TableProcSelect","Amt Orig"),90,HorizontalAlignment.Right);
				gridMain.Columns.Add(col);
				col=new GridColumn(Lan.g("TableProcSelect","Amt End"),90,HorizontalAlignment.Right);
				gridMain.Columns.Add(col);
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listAccountEntries.Count;i++) {
				if((!_listAccountEntries[i].GetType().In(typeof(ProcExtended),typeof(Adjustment)) || Math.Round(_listAccountEntries[i].AmountEnd,3) == 0) && creditCalcType!=CreditCalcType.ExcludeAll) {
					continue;
				}
				string dateStr;
				string procCodeStr="";
				string toothNumStr="";
				string descriptionText="";
				if(_listAccountEntries[i].GetType()==typeof(Adjustment)) {
					if(!_doShowAdjustments || _listAccountEntries[i].AmountEnd<=0 || _listAccountEntries[i].ProcNum>0) {
						//Do not show adjustments or do show adjustments- must not be attached to a procedure or the amount end must be positive
						continue;
					}
					dateStr=_listAccountEntries[i].Date.ToShortDateString();
					Def defAdjType=listDefsPosAdj.FirstOrDefault(x => x.DefNum==((Adjustment)_listAccountEntries[i].Tag).AdjType);
					descriptionText=defAdjType?.ItemName??Lan.g(this,"Adjustment");
				}
				else {//ProcExtended
					Procedure procedure=((ProcExtended)_listAccountEntries[i].Tag).Proc;
					ProcedureCode procedureCode = ProcedureCodes.GetProcCode(procedure.CodeNum);
					dateStr=procedure.ProcDate.ToShortDateString();
					procCodeStr=procedureCode.ProcCode;
					toothNumStr=procedure.ToothNum=="" ? Tooth.SurfTidyFromDbToDisplay(procedure.Surf,procedure.ToothNum) : Tooth.Display(procedure.ToothNum);
					if(procedure.ProcStatus==ProcStat.TP) {
						descriptionText="(TP) ";
					}
					descriptionText+=procedureCode.Descript;
				}
				row=new GridRow();
				row.Cells.Add(dateStr);
				row.Cells.Add(Providers.GetAbbr(_listAccountEntries[i].ProvNum));
				row.Cells.Add(procCodeStr);
				if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
					row.Cells.Add(toothNumStr);
				}
				row.Cells.Add(descriptionText);
				row.Cells.Add(_listAccountEntries[i].AmountOriginal.ToString("f"));
				if(creditCalcType != CreditCalcType.ExcludeAll) {
					row.Cells.Add(_listAccountEntries[i].AmountEnd.ToString("f"));
				}
				row.Tag=_listAccountEntries[i];
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
			List<AccountEntry> listAccountEntries=gridMain.SelectedTags<AccountEntry>();
			List<ProcExtended> listProcExtendeds=listAccountEntries.Where(x => x.Tag.GetType()==typeof(ProcExtended)).Select(x => (ProcExtended)x.Tag).ToList();
			List<AccountEntry> listSelectedPosAdjs=listAccountEntries.FindAll(x => x.Tag.GetType()==typeof(Adjustment));
			decimal decimalPosAdjAmt=listProcExtendeds.Sum(x => (decimal)x.PositiveAdjTotal)+listSelectedPosAdjs.Sum(x => x.AmountEnd);
			labelAmtOriginal.Text=    listProcExtendeds.Sum(x => x.AmountOriginal).ToString("c");
			labelPositiveAdjs.Text=   decimalPosAdjAmt.ToString("c");
			labelNegativeAdjs.Text=   listProcExtendeds.Sum(x => x.NegativeAdjTotals).ToString("c");
			labelPayPlanCredits.Text= (-listProcExtendeds.Sum(x => x.PayPlanCreditTotal)).ToString("c");
			labelPaySplits.Text=      (-listProcExtendeds.Sum(x => x.PaySplitTotal)).ToString("c");
			labelInsEst.Text=         (-listProcExtendeds.Sum(x => x.InsEstTotal)).ToString("c");
			labelInsPay.Text=         (-listProcExtendeds.Sum(x => x.InsPayTotal)).ToString("c");
			labelWriteOff.Text=       (-listProcExtendeds.Sum(x => x.WriteOffTotal)).ToString("c");
			labelWriteOffEst.Text=    (-listProcExtendeds.Sum(x => x.WriteOffEstTotal)).ToString("c");
			labelCurrentSplits.Text=  (-listProcExtendeds.Sum(x => x.SplitsCurTotal)).ToString("c");
			labelAmtEnd.Text=         listAccountEntries.Sum(y => y.AmountEnd).ToString("c");
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
			List<AccountEntry> listAccountEntries=gridMain.SelectedTags<AccountEntry>();
			for(int i=0;i<listAccountEntries.Count;i++) {
				if(listAccountEntries[i].GetType()==typeof(ProcExtended)) {
					Procedure procedure=((ProcExtended)listAccountEntries[i].Tag).Proc;
					ListProceduresSelected.Add(procedure);
					ListAccountEntries.Add(new AccountEntry(procedure));
					continue;
				}
				//Adjustment selected. Don't add to ListSelectedProcs
				ListAccountEntries.Add(listAccountEntries[i]);
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

	}
}