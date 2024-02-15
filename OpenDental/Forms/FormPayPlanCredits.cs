using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using System.Drawing;
using System.Drawing.Printing;
using CodeBase;

namespace OpenDental {
	///<summary>This form should not be made available for insurance payment plans.</summary>
	public partial class FormPayPlanCredits:FormODBase {
		///<summary>Payment plan credits for the current payment plan.  Must be passed-in.</summary>
		public List<PayPlanCharge> ListPayPlanChargesCredit;
		private PaymentEdit.ConstructResults _constructResults;
		private int _headingPrintH;
		private bool _headingPrinted;
		private List<PayPlanEdit.PayPlanEntry> _listPayPlanEntries;
		private int _pagesPrinted;
		private Patient _patient;
		private PayPlan _payPlan;
		///<summary>The provider that was passed into the constructor of this form. Does not always match the provider on the payment plan.</summary>
		private long _provNum;

		public FormPayPlanCredits(Patient patient,PayPlan payPlan,long provNum) {
			_payPlan=payPlan;
			_provNum=provNum;
			_patient=patient;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPayPlanCredits_Load(object sender,EventArgs e) {
			textCode.Text=Lan.g(this,"None");
			FillGrid();
			if(!Security.IsAuthorized(EnumPermType.PayPlanEdit,true)) {
				this.DisableAllExcept(checkHideUnattached,checkShowImplicit,butPrint,gridMain);
			}
			Plugins.HookAddCode(this,"FormPayPlanCredits.Load_end",_payPlan,_patient,_constructResults);
		}

		private void FillGrid() {
			_listPayPlanEntries=CreatePayPlanEntries(checkShowImplicit.Checked);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TablePaymentPlanProcsAndCreds","Date"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePaymentPlanProcsAndCreds","Provider"),65);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePaymentPlanProcsAndCreds","Stat"),30);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePaymentPlanProcsAndCreds","Code"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePaymentPlanProcsAndCreds","Fee"),55,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePaymentPlanProcsAndCreds","Rem Before"),70,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePaymentPlanProcsAndCreds","Credit Date"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePaymentPlanProcsAndCreds","Amount"),55,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePaymentPlanProcsAndCreds","Rem After"),60,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePaymentPlanProcsAndCreds","Note"),70){ IsWidthDynamic=true };
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			double totalAttached=0;
			for(int i=0;i<_listPayPlanEntries.Count;i++) { //for all account charges
				if(checkHideUnattached.Checked && !ListPayPlanChargesCredit.Exists(x => x.ProcNum==_listPayPlanEntries[i].ProcNumOrd)) {
					continue;
				}
				row=new GridRow();
				//we color the relevant cells to make the table easier to read.
				//the colors have been looked-at and approved by colourblind Josh.
				//In the future, we will probably make these customizable definitions.
				GridCell cell=new GridCell(_listPayPlanEntries[i].DateStr);
				//for procedure rows, cell color should be LightYellow for all relevant fields.
				cell.ColorBackG=_listPayPlanEntries[i].IsChargeOrd ? Color.White : Color.LightYellow;
				row.Cells.Add(cell);
				cell=new GridCell(_listPayPlanEntries[i].ProvAbbr);
				cell.ColorBackG=_listPayPlanEntries[i].IsChargeOrd ? Color.White : Color.LightYellow;
				row.Cells.Add(cell);
				cell=new GridCell(_listPayPlanEntries[i].StatStr);
				cell.ColorBackG=_listPayPlanEntries[i].IsChargeOrd ? Color.White : Color.LightYellow;
				row.Cells.Add(cell);
				cell=new GridCell(_listPayPlanEntries[i].ProcStr);
				cell.ColorBackG=_listPayPlanEntries[i].IsChargeOrd ? Color.White : Color.LightYellow;
				row.Cells.Add(cell);
				cell=new GridCell(_listPayPlanEntries[i].FeeStr);
				cell.ColorBackG=_listPayPlanEntries[i].IsChargeOrd ? Color.White : Color.LightYellow;
				row.Cells.Add(cell);
				cell=new GridCell(_listPayPlanEntries[i].RemBefStr);
				cell.ColorBackG=_listPayPlanEntries[i].IsChargeOrd ? Color.White : Color.LightYellow;
				row.Cells.Add(cell);
				cell=new GridCell(_listPayPlanEntries[i].CredDateStr);
				//for charge rows, cell color should be LightCyan for all relevant fields.
				cell.ColorBackG=_listPayPlanEntries[i].IsChargeOrd ? Color.LightCyan : Color.White;
				row.Cells.Add(cell);
				cell=new GridCell(_listPayPlanEntries[i].AmtStr);
				cell.ColorBackG=_listPayPlanEntries[i].IsChargeOrd ? Color.LightCyan : Color.White;
				row.Cells.Add(cell);
				totalAttached+=PIn.Double(_listPayPlanEntries[i].AmtStr);
				cell=new GridCell(_listPayPlanEntries[i].RemAftStr);
				cell.ColorBackG=_listPayPlanEntries[i].IsChargeOrd ? Color.LightCyan : Color.White;
				row.Cells.Add(cell);
				cell=new GridCell(_listPayPlanEntries[i].NoteStr);
				cell.ColorBackG=_listPayPlanEntries[i].IsChargeOrd ? Color.LightCyan : Color.White;
				row.Cells.Add(cell);
				row.Tag=_listPayPlanEntries[i];
				if(!_listPayPlanEntries[i].IsChargeOrd) {
					row.ColorLborder=Color.Black;
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			textTotal.Text=totalAttached.ToString("f");
		}

		///<summary>Returns a list of ordered PayPlanEntries based off of the current production on the patient's account.</summary>
		private List<PayPlanEdit.PayPlanEntry> CreatePayPlanEntries(bool showImplicitlyPaidOffProcs) {
			//Showing implicitly paid off procedures means the user wants to see procedures that have not been explicitly paid off yet.
			_constructResults=PaymentEdit.ConstructAndLinkChargeCredits(_patient.PatNum,isIncomeTxfr:showImplicitlyPaidOffProcs,
				doIncludeTreatmentPlanned:true);
			List<PayPlanEdit.PayPlanEntry> listPayPlanEntries=new List<PayPlanEdit.PayPlanEntry>();
			listPayPlanEntries.AddRange(PayPlanEdit.CreatePayPlanEntriesForAccountCharges(_constructResults.ListAccountEntries,_patient,_payPlan));
			ListPayPlanChargesCredit=ListPayPlanChargesCredit.OrderBy(x => x.ChargeDate).ToList();
			listPayPlanEntries.AddRange(PayPlanEdit.CreatePayPlanEntriesForPayPlanCredits(_constructResults.ListAccountEntries,ListPayPlanChargesCredit));
			if(ListPayPlanChargesCredit.Exists(x => x.ProcNum==0)) {//only add "Unattached" if there is a credit without a procedure. 
				listPayPlanEntries.Add(PayPlanEdit.CreatePayPlanEntryForUnattachedProcs(ListPayPlanChargesCredit,_patient.FName));
			}
			return listPayPlanEntries.OrderByDescending(x => x.ProcStatOrd)
				.ThenByDescending(x => x.ProcNumOrd)
				.ThenBy(x => x.IsChargeOrd)
				.ThenBy(x => x.DateOrd).ToList();
		}

		private void SetTextBoxes() {
			List<PayPlanEdit.PayPlanEntry> listPayPlanEntriesSelected=gridMain.SelectedTags<PayPlanEdit.PayPlanEntry>();
			bool isUpdateButton=false;//keep track of the state of the button, if it is add or update. 
			if(listPayPlanEntriesSelected.Count==0) { //if there are no entries selected
				//button should say Add, textboxes should be editable. No attached procedure.
				butAddOrUpdate.Text=Lan.g(this,"Add");
				textAmt.Text="";
				textDate.Text="";
				textCode.Text=Lan.g(this,"None");
				textNote.Text="";
				textAmt.ReadOnly=false;
				textDate.ReadOnly=false;
				textNote.ReadOnly=false;
			}
			else if(listPayPlanEntriesSelected.Count==1) { //if there is one entry selected
				PayPlanEdit.PayPlanEntry payPlanEntrySelected=listPayPlanEntriesSelected[0]; //all textboxes should be editable
				textAmt.ReadOnly=false;
				textDate.ReadOnly=false;
				textNote.ReadOnly=false;
				if(payPlanEntrySelected.IsChargeOrd) { //if it's a PayPlanCharge
					//button should say Update, text boxes should fill with info from that charge.
					butAddOrUpdate.Text=Lan.g(this,"Update");
					isUpdateButton=true;
					textAmt.Text=payPlanEntrySelected.AmtStr;
					textNote.Text=payPlanEntrySelected.NoteStr;
					if(payPlanEntrySelected.ProcStatOrd==ProcStat.TP && payPlanEntrySelected.ProcNumOrd!=0) {//if tp, grey out the date textbox. it should always be maxvalue.
						//tp and procnum==0 means that it's the unattached row, in which case we don't want to make the text boxes ready-only.
						textDate.ReadOnly=true;
						textDate.Text="";
					}
					else {
						textDate.Text=payPlanEntrySelected.CredDateStr;
					}
					if(payPlanEntrySelected.Proc==null) { //selected charge could be unattached.
						textCode.Text=Lan.g(this,"Unattached");
					}
					else {
						textCode.Text=ProcedureCodes.GetStringProcCode(payPlanEntrySelected.Proc.CodeNum);
					}
				}
				else {// selected line item is a procedure (or the "Unattached" entry)
					//button should say "Add", text boxes should fill with info from that procedure (or "unattached").
					butAddOrUpdate.Text=Lan.g(this,"Add");
					if(payPlanEntrySelected.Proc==null) {
						textCode.Text=Lan.g(this,"Unattached");
						textAmt.Text="0.00";
						textNote.Text="";
						textDate.Text=DateTime.Today.ToShortDateString();
					}
					else { //if it is a procedure (and not the "unattached" row)
						List<PayPlanEdit.PayPlanEntry> listPayPlanEntriesForProc=_listPayPlanEntries
						.Where(x => x.ProcNumOrd==payPlanEntrySelected.ProcNumOrd)
						.Where(x => x.IsChargeOrd==true).ToList();
						if(listPayPlanEntriesForProc.Count==0) { //if there are no other charges attached to the procedure
							textAmt.Text=payPlanEntrySelected.RemBefStr; //set textAmt to the value in RemBefore
						}
						else {//if there are other charges attached, fill the amount textbox with the minimum value in the RemAftr column.
							textAmt.Text=listPayPlanEntriesForProc.Min(x => PIn.Double(x.RemAftStr)).ToString("f");
						}
						textDate.Text=DateTime.Today.ToShortDateString();
						textNote.Text=ProcedureCodes.GetStringProcCode(payPlanEntrySelected.Proc.CodeNum)+": "+Procedures.GetDescription(payPlanEntrySelected.Proc);
						textCode.Text=ProcedureCodes.GetStringProcCode(payPlanEntrySelected.Proc.CodeNum);
					}
				}
			}
			else if(listPayPlanEntriesSelected.Count>1) { //if they selected multiple line items
				//change the button to say "add"
				//blank out and make read-only all text boxes.
				butAddOrUpdate.Text=Lan.g(this,"Add");
				textAmt.Text="";
				textDate.Text="";
				textNote.Text="";
				textCode.Text=Lan.g(this,"Multiple");
				textAmt.ReadOnly=true;
				textDate.ReadOnly=true;
				textNote.ReadOnly=true;
			}
			if(listPayPlanEntriesSelected.Any(x => Security.IsGlobalDateLock(EnumPermType.PayPlanEdit,x.DateOrd,true))) {
				if(isUpdateButton) {
					butAddOrUpdate.Enabled=false;//only disallow them from updating a tx credit, adding a new one is okay. 
				}
				else {
					butAddOrUpdate.Enabled=true;
				}
				butDelete.Enabled=false;
				return;
			}
			butAddOrUpdate.Enabled=true;
			butDelete.Enabled=true;
		}

		private void gridMain_MouseUp(object sender,MouseEventArgs e) {
			SetTextBoxes();
		}

		private void butAddOrUpdate_Click(object sender,EventArgs e) {
			List<PayPlanEdit.PayPlanEntry> listPayPlanEntriesSelected=gridMain.SelectedTags<PayPlanEdit.PayPlanEntry>();
			if(listPayPlanEntriesSelected.Count<=1) { //validation (doesn't matter if multiple are selected)
				if(string.IsNullOrEmpty(textAmt.Text) || !textAmt.IsValid() || PIn.Double(textAmt.Text)==0) {
					MsgBox.Show(this,"Please enter a valid amount.");
					return;
				}
				if(textDate.Text!="" && !textDate.IsValid()) {
					MsgBox.Show(this,"Please enter a valid date.");
					return;
				}
			}
			if(textDate.Text=="") {
				textDate.Text=DateTime.Today.ToShortDateString();
			}
			if(Security.IsGlobalDateLock(EnumPermType.PayPlanEdit,PIn.Date(textDate.Text))) {
				return;
			}
			//There will be no FauxAccountEntries for procedures associated to a dynamic payment plans that do not have any debits.
			//If that is a scenario that requires the user be blocked then we need to get all of the PayPlanNums associated to the patient and then
			//grab all of the Procedure payment plan links for said plans (credits) regardless of the payment plan charges in the database (debits).
			//For now, we can assume that users are not going to have multiple different payment plans (patient and dynamic) associated to the same proc.
			List<long> listProcNumsDynamicPayPlan=_constructResults.ListAccountEntries.FindAll(x => x.GetType()==typeof(FauxAccountEntry)
					&& ((FauxAccountEntry)x).IsDynamic
					&& x.ProcNum > 0)
				.Select(x => x.ProcNum)
				.Distinct()
				.ToList();
			if(listPayPlanEntriesSelected.Count==0 ) {
				if(PrefC.GetInt(PrefName.RigorousAccounting)==(int)RigorousAccounting.EnforceFully) { //if they have none selected
					MsgBox.Show(this,"All treatment credits (excluding adjustments) must have a procedure.");
					return;
				}
				//add an unattached charge only if not on enforce fully
				PayPlanCharge payPlanChargeAdd=PayPlanEdit.CreateUnattachedCredit(textDate.Text,_patient.PatNum,textNote.Text,_payPlan.PayPlanNum,
					PIn.Double(textAmt.Text));
				ListPayPlanChargesCredit.Add(payPlanChargeAdd);
			}
			else if(listPayPlanEntriesSelected.Count==1) { //if they have one selected
				PayPlanEdit.PayPlanEntry payPlanEntrySelected=listPayPlanEntriesSelected[0];
				if(payPlanEntrySelected.Proc!=null && listProcNumsDynamicPayPlan.Contains(payPlanEntrySelected.Proc.ProcNum)) {
					MsgBox.Show(this,"This procedure is already linked to a payment plan.");
					return;
				}
				if(PrefC.GetInt(PrefName.RigorousAccounting)==(int)RigorousAccounting.EnforceFully) {
					if((payPlanEntrySelected.Proc==null || payPlanEntrySelected.Proc.ProcNum==0)
						&& !(payPlanEntrySelected.Charge!=null && payPlanEntrySelected.Charge.IsCreditAdjustment)) 
					{
						MsgBox.Show(this,"All treatment credits (excluding adjustments) must have a procedure.");
						return;
					}
				}
				PayPlanCharge payPlanChargeSelected=new PayPlanCharge();
				if(payPlanEntrySelected.IsChargeOrd) {
					//get the charge from the grid.
					//DO NOT use PayPlanChargeNum. They are not pre-inserted so they will all be 0 if new.
					payPlanChargeSelected=((PayPlanEdit.PayPlanEntry)(gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag)).Charge;
				}
				ListPayPlanChargesCredit=PayPlanEdit.CreateOrUpdateChargeForSelectedEntry(payPlanEntrySelected,ListPayPlanChargesCredit,PIn.Double(textAmt.Text),
					textNote.Text,textDate.Text,_patient.PatNum,_payPlan.PayPlanNum,payPlanChargeSelected);
			}
			else if(listPayPlanEntriesSelected.Count>1) { //if they have more than one entry selected
				//remove everythig that doesn't have a procnum from the list
				List<PayPlanEdit.PayPlanEntry> listPayPlanEntriesProcSelected=listPayPlanEntriesSelected.FindAll(x => !x.IsChargeOrd && x.Proc!=null);
				if(listPayPlanEntriesSelected.Count==0) { //if the list is then empty, there's nothing to do.
					MsgBox.Show(this,"You must have at least one procedure selected.");
					return;
				}
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,
					"Add a payment plan credit for each of the selected procedure's remaining amount?  Selected credits will be ignored."))
				{
					return;
				}
				//Skip any procedures that are associated to a dynamic payment plan in any way.
				int countProcsSkipped=listPayPlanEntriesProcSelected.RemoveAll(x => listProcNumsDynamicPayPlan.Contains(x.Proc.ProcNum));
				ListPayPlanChargesCredit=PayPlanEdit.CreateCreditsForAllSelectedEntries(listPayPlanEntriesProcSelected,_listPayPlanEntries,DateTime.Today,_patient.PatNum,
					_payPlan.PayPlanNum,ListPayPlanChargesCredit);
				if(countProcsSkipped>0) {
					MsgBox.Show(this,"Credits were not made for some procedures because they were linked to one or more payment plans.");
				}
			}
			textAmt.Text="";
			textDate.Text="";
			textNote.Text="";
			FillGrid();
			SetTextBoxes();
		}

		private void butClear_Click(object sender,EventArgs e) {
			gridMain.SetAll(false);
			SetTextBoxes();
			//txtAmt does not need this because validation is done on text changed, textDate validation logic is not linked to text change so we will 
			//manually clear error since there is no text and current control logic would result in the error being cleared when empty.
			textDate.Validate();
		}

		private void checkHideUnattached_CheckedChanged(object sender,EventArgs e) {
			FillGrid();
			SetTextBoxes();
		}

		private void checkShowImplicit_CheckedChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			_headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Outstanding insurance report printed"),PrintoutOrientation.Landscape);
		}

		private void pd_PrintPage(object sender,PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			using Font fontHeading=new Font("Arial",13,FontStyle.Bold);
			using Font fontSubHeading=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			if(!_headingPrinted) {
				text=Lan.g(this,"Payment Plan Credits");
				g.DrawString(text,fontHeading,Brushes.Black,center-g.MeasureString(text,fontHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontHeading).Height;
				text=DateTime.Today.ToShortDateString();
				g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontHeading).Height;
				text=_patient.LName+", "+_patient.FName;
				g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
				yPos+=20;
				_headingPrinted=true;
				_headingPrintH=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,_pagesPrinted,bounds,_headingPrintH);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
				return;
			}
			e.HasMorePages=false;
			text=Lan.g(this,"Total")+": "+PIn.Double(textTotal.Text).ToString("c");
			g.DrawString(text,fontSubHeading,Brushes.Black,center+gridMain.Width/2-g.MeasureString(text,fontSubHeading).Width-10,yPos);
		}

		private void butDelete_Click(object sender,EventArgs e) {
			List<PayPlanEdit.PayPlanEntry> listPayPlanEntriesSelected=new List<PayPlanEdit.PayPlanEntry>();
			for(int i=0;i < gridMain.SelectedIndices.Count();i++) {
				listPayPlanEntriesSelected.Add((PayPlanEdit.PayPlanEntry)(gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag));
			}
			List<PayPlanCharge> listPayPlanChargesSelected=listPayPlanEntriesSelected.Where(x => x.Charge != null).Select(x => x.Charge).ToList();
			//remove all procedures from the list. you cannot delete procedures from here.
			if(listPayPlanChargesSelected.Count<1) {
				MsgBox.Show(this,"You must have at least one payment plan charge selected.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Delete selected payment plan charges?")) {
				return;
			}
			for(int i=0;i<listPayPlanChargesSelected.Count;i++) {
				ListPayPlanChargesCredit.Remove(listPayPlanChargesSelected[i]);
			}
			FillGrid();
			SetTextBoxes();
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(PrefC.GetInt(PrefName.RigorousAccounting)==(int)RigorousAccounting.EnforceFully) {
				//If no procs attached and not an adjustment with a negative amount
				if(ListPayPlanChargesCredit.Any(x => x.ProcNum==0 && !x.IsCreditAdjustment)) {
					MsgBox.Show(this,"All treatment credits (excluding adjustments) must have a procedure.");
					return;
				}
			}
			//find if any charges exist for attached credits that have a provider other than the provdier on the payment plan. Unattached credits will be 0.
			if(ListPayPlanChargesCredit.Count>0 && _listPayPlanEntries.FindAll(x => x.Charge!=null && x.ProvNum!=0).Any(x => x.ProvNum!=_provNum)) {
				//All credits go to the provider on the payment plan. We need to check if the procedure's provider does not match.
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Provider(s) for procedure credits do not match the provider on the payment plan. Continue?")) {
					return;
				}
			}
			DialogResult=DialogResult.OK;
			Plugins.HookAddCode(this,"FormPayPlanCredits.butOK_Click_end",_patient,_payPlan,_listPayPlanEntries);
		}

		private void FormPayPlanCredits_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult!=DialogResult.Cancel) {
				return;
			}
			Plugins.HookAddCode(this,"FormPayPlanCredits.butCancel_Click_end");
		}
	}
}