using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental {
	///<summary>This form allows users to add adjustments to multiple procedures and does line item accounting for those adjustments. If you are modifying this form make sure you have a strong understanding of the private class it utilizes. MultiAdjEntry is crucial to almost all logical operations in this form.</summary>
	public partial class FormAdjMulti:FormODBase {
		#region Fields - Private
		private List<Procedure> _listProceduresSelected;
		private List<Adjustment> _listAdjustmentsToAdd;
		private List<MultiAdjEntry> _listGridEntries=new List<MultiAdjEntry>();
		private Patient _patient;
		private List<Provider> _listProviders;
		private RigorousAdjustments _rigorousAdjustment;
		#endregion
		
		///<summary>Optionally pass in a list of procedures to always display. Pass in a list of new adjustments that are not in the database that will always display. Any procedures associated to the adjustments passed in will be added to the list of procedures to be selected.</summary>
		public FormAdjMulti(Patient patient,List<Procedure> listProceduresSelected=null,List<Adjustment> listAdjustmentsToAdd=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patient=patient;
			_listProceduresSelected=listProceduresSelected??new List<Procedure>();
			_listAdjustmentsToAdd=listAdjustmentsToAdd;
			if(_listAdjustmentsToAdd!=null) {
				List<long> listProcNums=_listAdjustmentsToAdd.Where(x => !ListTools.In(x.ProcNum,_listProceduresSelected.Select(x => x.ProcNum)))
					.Select(x=>x.ProcNum).Distinct().ToList();
				_listProceduresSelected.AddRange(Procedures.GetManyProc(listProcNums,false));
			}
		}
		
		private void FormMultiAdj_Load(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.AdjustmentCreate,true) && !Security.IsAuthorized(Permissions.AdjustmentEditZero,true)) {
				MessageBox.Show(Lans.g("Security","Not authorized for")+"\r\n"+GroupPermissions.GetDesc(Permissions.AdjustmentCreate));
				DialogResult=DialogResult.Cancel;
				return;
			}
			dateAdjustment.Text=DateTime.Today.ToShortDateString();
			_rigorousAdjustment=PrefC.GetEnum<RigorousAdjustments>(PrefName.RigorousAdjustments);
			FillListBoxAdjTypes();
			FillComboProv();
			if(PrefC.HasClinicsEnabled) {
				//Select 'Inherit' by default which is the unassigned option and was carefully approved by Nathan (and reluctantly Allen)
				comboClinic.SelectedClinicNum=0;
			}
			//Must happen after comboboxes are filled
			if(_rigorousAdjustment==RigorousAdjustments.EnforceFully) {
				comboProv.Enabled=false;
				comboClinic.Enabled=false;
				butPickProv.Enabled=false;
			}
			if(_rigorousAdjustment==RigorousAdjustments.DontEnforce) {
				radioIncludeAll.Checked=true;
			}
			else {
				radioAllocatedOnly.Checked=true;
			}
			if(_listAdjustmentsToAdd!=null) {
				SetGridEntriesToAdjustmentsToAdd();
				FillGrid();
			}
			else {
				RefreshCreditFilter();
			}
			for(int i=0;i<_listProceduresSelected.Count;i++) { 
				gridMain.SetSelected(_listGridEntries.FindIndex(x => x.ProcedureCur!=null && x.ProcedureCur.ProcNum==_listProceduresSelected[i].ProcNum),true);
			}
			textAmt.Select();//start cursor here
		}

		#region Methods - Fill
		private void FillComboProv() {
			comboProv.Items.Clear();
			comboProv.Items.Add("Inherit");//Inherit was carefully approved by Nathan (and reluctantly Allen)
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++) { 
				comboProv.Items.Add(_listProviders[i].Abbr,_listProviders[i]);
			}
			comboProv.SelectedIndex=0;
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TableMultiAdjs","Date"),70,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn() { Heading=Lan.g("TableMultiAdjs","Provider"), IsWidthDynamic=true };
			gridMain.ListGridColumns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn() { Heading=Lan.g("TableMultiAdjs","Clinic"), IsWidthDynamic=true };
				gridMain.ListGridColumns.Add(col);
			}		 	 
			col=new GridColumn() { Heading=Lan.g("TableMultiAdjs","Type"), IsWidthDynamic=true };
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableMultiAdjs","Fee"),70,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableMultiAdjs","Rem Before"),70,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableMultiAdjs","Adj Amt"),70,HorizontalAlignment.Left);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableMultiAdjs","Rem After"),70,HorizontalAlignment.Left);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			RecalculateGridEntries();
			for(int i=0;i<_listGridEntries.Count;i++) {
				GridRow row=_listGridEntries[i].ToGridRow();
				row.Tag=_listGridEntries[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			butAdd.Text=Lan.g(this,"Add Adjustments");
		}

		private void FillListBoxAdjTypes() {
			listTypePos.Items.AddList(Defs.GetPositiveAdjTypes(),x => x.ItemName);
			listTypeNeg.Items.AddList(Defs.GetNegativeAdjTypes(),x => x.ItemName);
		}

		///<summary>This method converts the AccountEntry objects we get back from AccountModules.GetListUnpaidAccountCharges() to MultiAdjEntry objects. These are used to fill the grid and do all relevant logic in this form. This method will return a fresh list of procedures and will not show any existing adjustments that may have been made in this form already. When called from checkShowImplicit any existing adjustments made will not be shown.(Ask Andrew about this functionality)</summary>
		private List<MultiAdjEntry> FillListGridEntries(CreditCalcType calc) {
			//Setting isIncomeTxfr to true will only perform explicit linking; False will perform both explicit and implicit.
			//The 'Only allocated credits' and 'Exclude all credits' radio boxes should perform explicit linking only (isIncomeTxfr = true)
			//The 'Include all credits' radio box should perform both explicit and implicit linking (isIncomeTxfr = false)
			PaymentEdit.ConstructResults constructResults=PaymentEdit.ConstructAndLinkChargeCredits(_patient.PatNum,
				new List<long>(){ _patient.PatNum },
				isIncomeTxfr:!radioIncludeAll.Checked);
			List<MultiAdjEntry> retVal=new List<MultiAdjEntry>();
			//We only want AccountEntries that are procedures.
			List<AccountEntry> listAccountEntries=constructResults.ListAccountCharges.FindAll(x => x.GetType()==typeof(Procedure));
			for(int i=0;i<listAccountEntries.Count;i++) { 
				Procedure procedure=(Procedure)listAccountEntries[i].Tag;
				if(procedure.ProcStatus!=ProcStat.C) {
					continue;//Adjustments can only be made towards completed procedures.
				}
				//The procedure's AccountEntry will have an AmountEnd value forcibly set to $0 when there is insurance overpayment.
				//However, the remBefore and remAfter columns should show the negative value that represents just how much it was overpaid.
				//We also want this number to reflect any existing patient payments, especially considering an office can add adjustments for patients who do
				//not have insurance and therefore would not have any insurance payments (over or otherwise).
				decimal amountAfterPayments=AccountEntry.GetExplicitlyLinkedProcAmt(listAccountEntries[i],doConsiderPatPayments:true);
				double remBefore=(double)amountAfterPayments;
				double remAfter=(double)amountAfterPayments;
				MultiAdjEntry multiAdjEntry=new MultiAdjEntry(procedure,remBefore,remAfter);
				bool isProcSelectedInAccount=_listProceduresSelected.Any(x => x.ProcNum==listAccountEntries[i].ProcNum);
				if(calc==CreditCalcType.ExcludeAll) {//show everything. Regardless of procs loading window or amount remaining.
					retVal.Add(multiAdjEntry);
				}
				else if(ListTools.In(calc,CreditCalcType.IncludeAll,CreditCalcType.AllocatedOnly) && (!CompareDecimal.IsZero(listAccountEntries[i].AmountEnd) || isProcSelectedInAccount)) {
					//Unpaid procedures should always show per Nathan. If proc was specifically selected before entering window, show it anyways.
					retVal.Add(multiAdjEntry);
				}
			}
			return retVal;
		}
		#endregion

		#region Methods - Private
		private void AddAdjustmentToGrid(Adjustment adjustment,double adjAmtOrPerc) {
			AddUnattachedRow();
			MultiAdjEntry adjRow=new MultiAdjEntry(adjustment,adjustment.AdjAmt);
			adjRow.AdjAmtType=AdjAmtType.FixedAmt;
			adjRow.AdjAmtOrPerc=adjAmtOrPerc;
			adjRow.RecalculateAdjAmt(0);
			_listGridEntries.Add(adjRow);
		}

		private void AddAdjustmentWithProcedureToGrid(Procedure procedure,Adjustment adjustment,double adjAmtOrPerc,double procFeeAfter,AdjAmtType adjAmtType,bool doUpdateAdjValues) {
			MultiAdjEntry adjRow=new MultiAdjEntry(procedure,adjustment,procFeeAfter);
			if(doUpdateAdjValues) {
				adjRow=UpdateAdjValues(adjRow);//This will set all of the important values from UI selections
			}
			adjRow.AdjAmtType=adjAmtType;
			adjRow.AdjAmtOrPerc=adjAmtOrPerc;
			_listGridEntries.Add(adjRow);
		}

		///<summary>Adds adjustments to _listGridEntries for the selected procedures (if any) or for an "unattached" proc if no selected entries.</summary>
		private void AddAdjustments(List<MultiAdjEntry> listSelectedEntries) {
			//loop through and update all adjustment rows
			List<MultiAdjEntry> listMultiAdjEntryAdjRows=listSelectedEntries.FindAll(x => x.AdjustmentCur!=null);
			for(int i=0;i<listMultiAdjEntryAdjRows.Count;i++) { 
				UpdateAdjValues(listMultiAdjEntryAdjRows[i]);
			}
			//Create new adjustment
			Adjustment adjustment=new Adjustment();
			adjustment.AdjType=GetSelectedAdjDef().DefNum;
			adjustment.AdjDate=PIn.Date(dateAdjustment.Text);
			adjustment.AdjNote=PIn.String(textNote.Text);
			adjustment.PatNum=_patient.PatNum;
			if(listSelectedEntries.Count==0) {//User did not select anything and hit "add", so they must want to add an unattached adjustment.
				adjustment.ProvNum=_patient.PriProv;
				adjustment.ClinicNum=_patient.ClinicNum;
				adjustment.ProcDate=adjustment.AdjDate;
				adjustment.ProcNum=0;
				AddAdjustmentToGrid(adjustment,PIn.Double(textAmt.Text));
				return;
			}
			//One or more procedure rows selected, create adjustments
			List<MultiAdjEntry> listMultiAdjEntryProcRows=listSelectedEntries.FindAll(x => x.IsProcedureRow());
			for(int i=0;i<listMultiAdjEntryProcRows.Count;i++) {//Loop through all selected procedures
				Adjustment adjustmentNew=adjustment.Clone();
				adjustmentNew.ProcDate=listMultiAdjEntryProcRows[i].ProcedureCur.ProcDate;
				adjustmentNew.ProcNum=listMultiAdjEntryProcRows[i].ProcedureCur.ProcNum;
				AddAdjustmentWithProcedureToGrid(listMultiAdjEntryProcRows[i].ProcedureCur,adjustmentNew,PIn.Double(textAmt.Text),listMultiAdjEntryProcRows[i].RemAfter,GetAdjAmtType(),true);
			}
		}

		private void AddUnattachedRow() {
			//Only insert the Unattached row if we do not already have unattached adjustments in the grid
			if(_listGridEntries.Any(x => x.IsUnattachedRowHeader())) {
				return;
			}
			//We are just adding a dummy row header to help the UI look a little cleaner.
			_listGridEntries.Add(new MultiAdjEntry(null,null,0));
		}

		///<summary>Returns the corresponding AdjAmtType based off of the current state of the UI controls.</summary>
		private AdjAmtType GetAdjAmtType() {
			if(radioFixedAmt.Checked) {
				return AdjAmtType.FixedAmt;
			}
			else if(radioPercentRemBal.Checked) {
				return AdjAmtType.PercentOfRemBal;
			}
			else {
				return AdjAmtType.PercentOfFee;
			}
		}

		private Def GetSelectedAdjDef() {
			Def selectedAdjType=listTypePos.GetSelected<Def>();
			if(selectedAdjType==null) {
				//Nothing was selected in listTypePos so there has to be a selection for negative.
				selectedAdjType=listTypeNeg.GetSelected<Def>();
			}
			return selectedAdjType;
		}

		///<summary>Checks to verify that the user does not try to add an adjustment and update an adjustment at the same time. Returns true if the user has at least one adjustment and at least one procedure row selected at the same time; Otherwise, false.</summary>
		private bool HasProcAndAdjSelected(List<MultiAdjEntry> listSelectedRows) {
			bool hasProcRowSelected=listSelectedRows.Any(x => x.IsProcedureRow());
			bool hasAdjRowSelected=listSelectedRows.Any(x => x.AdjustmentCur!=null);
			if(hasProcRowSelected && hasAdjRowSelected) {
				return true;
			}
			return false;
		}

		private bool IsValid(int numberEntriesSelected) {
			//No procedures selected AND (rigorous adjustments are fully enforced OR the Fixed Amount option is not selected).
			if(numberEntriesSelected==0 && (_rigorousAdjustment==RigorousAdjustments.EnforceFully || !radioFixedAmt.Checked)) {
				MsgBox.Show(this,"You must select a procedure to add the adjustment to.");
				return false;
			}
			if(listTypePos.SelectedIndex==-1 && listTypeNeg.SelectedIndex==-1) {
				MsgBox.Show(this,"Please pick an adjustment type.");
				return false;
			}
			if(comboProv.SelectedIndex==-1) {
				MsgBox.Show(this,"Please pick a provider.");
				return false;
			}
			if(PrefC.HasClinicsEnabled && comboClinic.SelectedClinicNum==-1) {
				MsgBox.Show(this,"Please pick a clinic.");
				return false;
			}
			if(string.IsNullOrWhiteSpace(dateAdjustment.Text) || !dateAdjustment.IsValid()) {
				MsgBox.Show(this,"Please enter a valid date.");
				return false;
			}
			if(PIn.Date(dateAdjustment.Text).Date > DateTime.Today.Date && !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
				MsgBox.Show(this,"Adjustments cannot be made for future dates");
				return false;
			}
			if(string.IsNullOrWhiteSpace(textAmt.Text) || !textAmt.IsValid()) {
				MsgBox.Show(this,"Please enter a valid amount.");
				return false;
			}
			List<long> listSelectedProcNums=gridMain.SelectedTags<MultiAdjEntry>().Where(x => x.ProcedureCur!=null).Select(x => x.ProcedureCur.ProcNum).ToList();
			List<OrthoProcLink> listOrthoProcLinks=OrthoProcLinks.GetManyForProcs(listSelectedProcNums);
			if(listOrthoProcLinks.Count>0) {
				MsgBox.Show(this,"One or more of the selected procedures cannot be adjusted because it is attached to an ortho case." +
					" Please deselect these items and try again.");
				return false;
			}
			return true;
		}

		private void OrderListGridEntries() {
			//This ordering assumes that there should only be exactly one row with a null adjustment per procedure. (the head procedure row)
			//And a null procedure means we are dealing with an unattached adjustment (or an unattached adjustment header row).
			_listGridEntries=_listGridEntries.OrderBy(x => x.ProcedureCur==null)
				.ThenBy(x => x.ProcedureCur!=null ? x.ProcedureCur.ProcNum : 0)//If no Proc (unattached adj) then ordering is taken care of later.
				.ThenBy(x => x.AdjustmentCur!=null ? x.AdjustmentCur.AdjDate : DateTime.MinValue)
				.ThenBy(x => x.AdjustmentCur!=null ? x.AdjustmentEntryNum : 0).ToList();//Unattached adjustments will get ordered here.
		}

		private MultiAdjEntry ProcToMultiAdjEntry(Procedure procedure) {
			MultiAdjEntry multiAdjEntryProc= new MultiAdjEntry(procedure,0,-procedure.ProcFee);
			multiAdjEntryProc.ProcedureCur=procedure;
			return multiAdjEntryProc;
		}

		///<summary>This method will recalculate the RemAfter variable for each MultiAdjEntry in the grid. This is required for line item accounting. If there are no unattached adjustments this method will remove the "Unattached" row, as well.</summary>
		private void RecalculateGridEntries() {
			//Remove the "Unattached" row if there are no unattached adjustments in the grid
			if(!_listGridEntries.Any(x => x.IsUnattachedRow())) {//There are no unattached adjustments
				_listGridEntries.RemoveAll(x => x.IsUnattachedRowHeader());//Remove the header row for unattached adjustments
			}
			//Rename the grid when there are no adjustments
			if(_listGridEntries.Count(x => x.AdjustmentCur!=null)==0) {
				gridMain.Title="Available Procedures";
			}
			#region Recalculate RemAfter for each grid entry
			//Manually recalculate all unattached adjs.  They should all be FixedAmt.
			List<MultiAdjEntry> listMultiAdjEntriesUnattached=_listGridEntries.FindAll(x => x.IsUnattachedRow());
			listMultiAdjEntriesUnattached.ForEach(x => x.RecalculateAdjAmt(0));
			//Make a list out of all of the adjustment rows that are attached to procedures and order them very specifically for recalculations.
			List<MultiAdjEntry> ListMultiAdjEntriesAttached=_listGridEntries.FindAll(x => x.IsAttachedRow()).OrderBy(x => x.AdjustmentCur.AdjDate).ThenBy(x => x.AdjustmentEntryNum).ToList();
			//Loop through each procedure row and recalculate all of the attached adjustment rows.
			List<MultiAdjEntry> listMultiAdjEntriesProcs=_listGridEntries.FindAll(x => x.IsProcedureRow());
			for(int i=0;i<listMultiAdjEntriesProcs.Count;i++) { 
				double amountRemaining=listMultiAdjEntriesProcs[i].RemBefore;
				List<MultiAdjEntry> listMultiAdjEntriesForProc=ListMultiAdjEntriesAttached
					.FindAll(x => x.ProcedureCur.ProcNum == listMultiAdjEntriesProcs[i].ProcedureCur.ProcNum);
				for(int j=0;j<listMultiAdjEntriesForProc.Count;j++) {//list could be empty
					//We must recalculate the AdjAmt for each adjustment here because this is the only place that has context of each line item.
					//For instance, if we update multiple procedures at a time, we would have to re-loop through each adjustment like we do here
					//to ensure that the percents are correct.
					listMultiAdjEntriesForProc[j].RecalculateAdjAmt(amountRemaining);
					amountRemaining+=listMultiAdjEntriesForProc[j].AdjustmentCur.AdjAmt;
					listMultiAdjEntriesForProc[j].RemAfter=amountRemaining;
				}
				listMultiAdjEntriesProcs[i].RemAfter=amountRemaining;
			}
			#endregion
			OrderListGridEntries();
		}

		/// <summary>
		/// Takes over the entire list of grid entries and sets it to a new list of grid entries that represent the adjustments passed into the constructor.
		/// </summary>
		public void SetGridEntriesToAdjustmentsToAdd() {
			for(int i = 0;i<_listAdjustmentsToAdd.Count;i++) {
				Procedure procedure=_listProceduresSelected.Find(x=>x.ProcNum==_listAdjustmentsToAdd[i].ProcNum);
				if(procedure==null) {
					AddAdjustmentToGrid(_listAdjustmentsToAdd[i],_listAdjustmentsToAdd[i].AdjAmt);
				}
				else {
					//Add a procedure row that can have adjustments attached to it.
					_listGridEntries.Add(ProcToMultiAdjEntry(procedure));
					//Add an adjustment row that is associated to the procedure.
					double negativeAdjAmt=-_listAdjustmentsToAdd[i].AdjAmt;
					AddAdjustmentWithProcedureToGrid(procedure,_listAdjustmentsToAdd[i],negativeAdjAmt,0,AdjAmtType.FixedAmt,false);
				}
			}
			OrderListGridEntries();
		}

		///<summary>Updates a selected row with the user selected values. Returns the MultiAdjEntry for the row.</summary>
		private MultiAdjEntry UpdateAdjValues(MultiAdjEntry row) {
			//set prov
			if(comboProv.Items.GetTextShowingAt(comboProv.SelectedIndex)=="Inherit") {//Inherit was carefully approved by Nathan (and reluctantly Allen)
				if(row.ProcedureCur!=null) {
					row.AdjustmentCur.ProvNum=row.ProcedureCur.ProvNum;
				}
			}
			else {
				row.AdjustmentCur.ProvNum=comboProv.GetSelected<Provider>().ProvNum;
			}
			//set clinic
			long selectedClinicNum=0;
			if(PrefC.HasClinicsEnabled) {
				if(comboClinic.IsUnassignedSelected) {//Inherit
					if(row.ProcedureCur!=null) {
						selectedClinicNum=row.ProcedureCur.ClinicNum; 
					}
				}
				else {
					selectedClinicNum=comboClinic.SelectedClinicNum;
				}
			}
			row.AdjustmentCur.AdjType=GetSelectedAdjDef().DefNum;
			row.AdjustmentCur.ClinicNum=selectedClinicNum;
			row.AdjustmentCur.AdjDate=PIn.Date(dateAdjustment.Text);
			row.AdjustmentCur.AdjNote=PIn.String(textNote.Text);
			row.AdjustmentCur.PatNum=_patient.PatNum;
			if(row.ProcedureCur==null) {//Unassigned adjustments have to be fixed amounts, or else they will be 0.
				row.AdjAmtType=AdjAmtType.FixedAmt;
			}
			else {
				row.AdjAmtType=GetAdjAmtType();
			}
			row.AdjAmtOrPerc=PIn.Double(textAmt.Text);
			return row;
		}
		#endregion

		#region Events
		private void butAdd_Click(object sender,EventArgs e) {
			List<MultiAdjEntry> listMultiAdjEntries=gridMain.SelectedTags<MultiAdjEntry>();
			if(!IsValid(listMultiAdjEntries.Count)) {
				return;
			}
			AddAdjustments(listMultiAdjEntries);
			gridMain.Title="Available Procedures with Adjustments";
			FillGrid();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			for(int i=0;i<gridMain.SelectedIndices.Count();i++) {
				MultiAdjEntry multiAdjEntrySelected=(MultiAdjEntry)gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag;
				if(multiAdjEntrySelected.AdjustmentCur!=null) {//user selected an adjustment row
					if(multiAdjEntrySelected.ProcedureCur==null) {//unattached adjustment
						_listGridEntries.Remove(multiAdjEntrySelected);
						continue;
					}
					//Should only be one, but will clean up accidental duplicates if a bug occurs.
					_listGridEntries.RemoveAll(x => x.AdjustmentEntryNum==multiAdjEntrySelected.AdjustmentEntryNum);
				}
			}
			butAdd.Enabled=true;
			FillGrid();
		}

		private void butPickProv_Click(object sender,EventArgs e) {
			using FormProviderPick formProviderPick=new FormProviderPick(_listProviders);
			if(formProviderPick.ShowDialog()==DialogResult.OK) {
				comboProv.SelectedIndex=-1;
				comboProv.SetSelectedKey<Provider>(formProviderPick.SelectedProvNum,x => x.ProvNum);
			}
		}
		
		private void gridMain_MouseUp(object sender,MouseEventArgs e) {
			List<MultiAdjEntry> listMultiAdjEntries=gridMain.SelectedTags<MultiAdjEntry>();
			butAdd.Enabled=true;
			if(listMultiAdjEntries.Count==0) {//if there are no entries selected
				butAdd.Text=Lan.g(this,"Add Adjustments");
			}
			else if(listMultiAdjEntries.Count==1) {
				if(listMultiAdjEntries[0].AdjustmentCur!=null) {//user selected an adjustment row
					butAdd.Text=Lan.g(this,"Update");
					textNote.Text=listMultiAdjEntries[0].AdjustmentCur.AdjNote;
				}
				else {//Selected a procedure row
					butAdd.Text=Lan.g(this,"Add Adjustments");
				}
			}
			else {//Multiple entries selected
				if(HasProcAndAdjSelected(listMultiAdjEntries)) {
					butAdd.Enabled=false;
				}
				else if(listMultiAdjEntries.All(x => x.AdjustmentCur!=null)) {//User has selected multiple adjustments
					butAdd.Text=Lan.g(this,"Update");
				}
				else if(listMultiAdjEntries.All(x => x.IsProcedureRow())) {//User has selected multiple proc rows
					butAdd.Text=Lan.g(this,"Add Adjustments");
				}
			}
		}

		private void listTypeNeg_SelectedIndexChanged(object sender,System.EventArgs e) {
			if(listTypeNeg.SelectedIndex>-1) {
				listTypePos.SelectedIndex=-1;
			}
		}

		private void listTypePos_SelectedIndexChanged(object sender,System.EventArgs e) {
			if(listTypePos.SelectedIndex>-1) {
				listTypeNeg.SelectedIndex=-1;
			}
		}

		private void radioCredits_Click(object sender,EventArgs e) {
			RefreshCreditFilter();
		}

		private void radioFixedAmt_CheckedChanged(object sender,EventArgs e) {
			if(radioFixedAmt.Checked) {
				labelAmount.Text="Amount";
			}
			else {
				labelAmount.Text="Percent";
			}
		}

		private void RefreshCreditFilter() {
			CreditCalcType creditCalcType;
			if(radioAllocatedOnly.Checked) {
				creditCalcType=CreditCalcType.AllocatedOnly;
			}
			else if(radioIncludeAll.Checked) {
				creditCalcType=CreditCalcType.IncludeAll;
			}
			else {
				creditCalcType=CreditCalcType.ExcludeAll;
			}
			List<MultiAdjEntry> listMultiAdjEntriesProcs=FillListGridEntries(creditCalcType);
			if(_listGridEntries.Where(x => x.AdjustmentCur!=null)//They have made an adjustment row for this procedure
				.Any(x => x.ProcedureCur!=null && !ListTools.In(x.ProcedureCur.ProcNum,listMultiAdjEntriesProcs.Where(y => y.ProcedureCur!=null).Select(y => y.ProcedureCur.ProcNum)))//The procedure is no longer in the list
				&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"This will clear out any adjustments you have added. Are you sure you want to continue?")) 
			{
				//When going from ExplicitOnly to FIFO we may lose procedures that currently exist in _listGridEntries.
				//This becomes problematic when the user has entered adjustments that have not been entered in the DB yet.
				//We will warn the user here that by unchecking the checkbox they will lose all adjustments they may have entered.
				//The reason for this is that it keeps our logic simple so that we can simply reset
				//the _listGridEntries to the entries returned by FillListGridEntries().
				radioExcludeAll.Checked=true;//This shows all procedures on the account
				return;
			}
			for(int i=0;i<_listGridEntries.Count;i++) { 
				//We only keep adjustments where the procedure is in the list
				if(_listGridEntries[i].AdjustmentCur==null || _listGridEntries[i].AdjustmentCur.ProcNum==0) {
					continue;
				}
				if(ListTools.In(_listGridEntries[i].ProcedureCur.ProcNum,listMultiAdjEntriesProcs.Where(x => x.ProcedureCur!=null).Select(x => x.ProcedureCur.ProcNum))) {
					listMultiAdjEntriesProcs.Add(_listGridEntries[i]);
				}
			}
			_listGridEntries=listMultiAdjEntriesProcs;
			OrderListGridEntries();
			FillGrid();
		}
		#endregion

		private void butOK_Click(object sender,EventArgs e) {
			List<MultiAdjEntry> listMultiAdjEntries=gridMain.GetTags<MultiAdjEntry>().FindAll(x => x.AdjustmentCur!=null);
			if(listMultiAdjEntries.Count==0) {//no adjustments have been added. Attempt to add adjustments with the current info.
				//get the list of selected procedures to add adjustments to (if there are any). Will make one for unattached if none exist.
				List<MultiAdjEntry> listMultiAdjEntrySelectedProcs=gridMain.SelectedTags<MultiAdjEntry>();
				if(!IsValid(listMultiAdjEntrySelectedProcs.Count)) {
					return;
				}
				AddAdjustments(listMultiAdjEntrySelectedProcs);
				FillGrid();//In case they get stopped at the msgBox below, they should see the new adjustments added. 
				//Refresh listAdjRows because some adjustments rows should have been added to the grid.
				listMultiAdjEntries=gridMain.GetTags<MultiAdjEntry>().FindAll(x => x.AdjustmentCur!=null);
			}
			//now we should be guaranteed to have adjustments
			bool hasNegAmt=false;
			//Make a list out of all of the adjustment rows.
			List<MultiAdjEntry> listMultiAdjEntriesAttached=_listGridEntries.FindAll(x => x.IsAttachedRow());
			//Loop through each procedure row and see if the total of all attached adjustments exceed the AmtBefore on the procedure row.
			List<MultiAdjEntry> listMultiAdjEntriesProcs=_listGridEntries.FindAll(x => x.IsProcedureRow());
			for(int i=0;i<listMultiAdjEntriesProcs.Count;i++) { 
				List<MultiAdjEntry> listMultiAdjEntriesForProc=listMultiAdjEntriesAttached
					.FindAll(x => x.ProcedureCur.ProcNum == listMultiAdjEntriesProcs[i].ProcedureCur.ProcNum);
				if(listMultiAdjEntriesForProc.Count==0) { 
					continue;
				}
				double adjustmentsSum=listMultiAdjEntriesForProc.Sum(x => x.AdjustmentCur.AdjAmt);
				if(CompareDouble.IsLessThanZero((listMultiAdjEntriesProcs[i].RemBefore + adjustmentsSum))) {
					hasNegAmt=true;
					break;
				}
			}
			if(hasNegAmt && !MsgBox.Show(this,MsgBoxButtons.OKCancel,"Remaining amount on a procedure is negative. Continue?","Overpaid Procedure Warning")) {
				return;
			}
			if(!Security.IsAuthorized(Permissions.AdjustmentCreate,true)) {//User does not have full edit permission.
				//Therefore the user only has the ability to edit $0 adjustments (see Load()).
				if(listMultiAdjEntries.Any(x => !CompareDouble.IsZero(x.AdjustmentCur.AdjAmt))) {
					MsgBox.Show(this,"Amount has to be 0.00 due to security permission.");
					return;
				}
			}
			List<string> listStringAdjustmentAmounts=new List<string>();
			for(int i=0;i<listMultiAdjEntries.Count;i++) { 
				Adjustments.Insert(listMultiAdjEntries[i].AdjustmentCur);
				TsiTransLogs.CheckAndInsertLogsIfAdjTypeExcluded(listMultiAdjEntries[i].AdjustmentCur);
				listStringAdjustmentAmounts.Add(listMultiAdjEntries[i].AdjustmentCur.AdjAmt.ToString("c"));
			}
			if(listStringAdjustmentAmounts.Count>0) {
				string log=Lan.g(this,"Adjustment(s) created from Multiple Adjustments window:")+" ";
				SecurityLogs.MakeLogEntry(Permissions.AdjustmentCreate,_patient.PatNum,log+string.Join(",",listStringAdjustmentAmounts));
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	
		///<summary>This is a private class used to simplify grid logic in this form. This class represents either a procedure row, an adjustment row, or an unattached adjustment row.</summary>
		private class MultiAdjEntry {

			#region Fields - Public
			///<summary>Stores an adjustment (not in the db yet). Null when this entry is a procedure row or an unattached dummy row header.</summary>
			public Adjustment AdjustmentCur;
			///<summary>The adjustment amount or percentage that was used when this row was created or last updated.</summary>
			public double AdjAmtOrPerc=0;
			///<summary>Used to know how to treat AdjAmtOrPerc, either as an amount or a percentage.</summary>
			public AdjAmtType AdjAmtType=AdjAmtType.FixedAmt;
			///<summary>No matter which constructor is used, the AdjustmentEntryNum will be unique and automatically assigned.</summary>
			public long AdjustmentEntryNum=(_adjustmentEntryAutoIncrementValue++);
			///<summary>The associated procedure. Null if the adjustment is unattached.</summary>
			public Procedure ProcedureCur;
			///<summary>The amount that is left after this row (and the associated ones above it) have been taken into account.</summary>
			public double RemAfter;
			///<summary>The amount that is available before this row is taken into account.</summary>
			public double RemBefore;
			#endregion

			#region Fields - Private Static
			private static long _adjustmentEntryAutoIncrementValue=1;
			#endregion

			///<summary>This constructor should be called for procedure rows.</summary>
			public MultiAdjEntry(Procedure proc,double remBefore,double remAfter) {
				ProcedureCur=proc;
				AdjustmentCur=null;
				RemBefore=remBefore;
				RemAfter=remAfter;
			}

			///<summary>This constructor should be called for attached adjustment rows.</summary>
			public MultiAdjEntry(Procedure proc,Adjustment adj,double procFeeAfter) {
				ProcedureCur=proc;
				AdjustmentCur=adj;
				RemBefore=-1;
				RemAfter=procFeeAfter;
			}

			///<summary>This constructor should only be used for unattached adjustments.</summary>
			public MultiAdjEntry(Adjustment adj,double adjAmt) {
				//We purposely set the associated procedure to null so that we can differentiate between an unattached adjustment and a regular adjustment
				ProcedureCur=null;
				AdjustmentCur=adj;
				RemBefore=-1;
				RemAfter=adjAmt;
			}

			///<summary>Determines if the row is an attached row (adjustment linked to a procedure).</summary>
			public bool IsAttachedRow() {
				return (ProcedureCur!=null && AdjustmentCur!=null);
			}

			///<summary>Determines if the row is a procedure row (no adjustment in the picture).</summary>
			public bool IsProcedureRow() {
				return (ProcedureCur!=null && AdjustmentCur==null);
			}

			///<summary>Determines if the row is an unattached row (adjustment not linked to a procedure).</summary>
			public bool IsUnattachedRow() {
				return (ProcedureCur==null && AdjustmentCur!=null);
			}

			///<summary>Determines if the row is an unattached row header (no procedure or adjustment, should only be one of these).</summary>
			public bool IsUnattachedRowHeader() {
				return (ProcedureCur==null && AdjustmentCur==null);
			}

			///<summary>Manipulates the AdjAmt on the adjustment object.</summary>
			public void RecalculateAdjAmt(double remBefore) {
				if(AdjAmtType==AdjAmtType.FixedAmt) {
					AdjustmentCur.AdjAmt=AdjAmtOrPerc;
				}
				else if(AdjAmtType==AdjAmtType.PercentOfRemBal) {
					AdjustmentCur.AdjAmt=Math.Round((AdjAmtOrPerc/100)*remBefore,2);
				}
				else if(AdjAmtType==AdjAmtType.PercentOfFee) {
					AdjustmentCur.AdjAmt=Math.Round((AdjAmtOrPerc/100)*ProcedureCur.ProcFee,2);
				}
				if(Defs.GetValue(DefCat.AdjTypes,AdjustmentCur.AdjType)=="-") {
					AdjustmentCur.AdjAmt*=-1;
				}
			}

			public GridRow ToGridRow() {
				GridRow row=new GridRow();
				GridCell cell;
				if(ProcedureCur!=null && AdjustmentCur==null) {//A procedure row
					//Date
					cell=new GridCell(ProcedureCur.ProcDate.ToShortDateString());
					row.Cells.Add(cell);
					//Provider
					cell=new GridCell(Providers.GetAbbr(ProcedureCur.ProvNum));
					row.Cells.Add(cell);
					//Clinic
					if(PrefC.HasClinicsEnabled) {
						cell=new GridCell(Clinics.GetAbbr(ProcedureCur.ClinicNum));
						row.Cells.Add(cell);
					}
					//Code
					cell=new GridCell(ProcedureCodes.GetStringProcCode(ProcedureCur.CodeNum));
					row.Cells.Add(cell);
					//Fee;
					cell=new GridCell(ProcedureCur.ProcFeeTotal.ToString("c"));
					cell.ColorBackG=Color.LightYellow;
					row.Cells.Add(cell);
					//Rem Before
					cell=new GridCell(RemBefore.ToString("c"));
					cell.ColorBackG=Color.LightYellow;
					row.Cells.Add(cell);
					//Adj Amt
					cell=new GridCell("");
					cell.ColorBackG=Color.White;
					row.Cells.Add(cell);
					//Rem After
					cell=new GridCell("");
					cell.ColorBackG=Color.White;
					row.Cells.Add(cell);
				}
				else if(IsUnattachedRowHeader()) {//The row header for unattached adjustments
					row.ColorBackG=Color.LightYellow;
					//Date
					cell=new GridCell("Unassigned");
					row.Cells.Add(cell);
					//Provider
					cell=new GridCell("");
					row.Cells.Add(cell);
					//Clinic
					if(PrefC.HasClinicsEnabled) {
						cell=new GridCell("");
						row.Cells.Add(cell);
					}
					//Code
					cell=new GridCell("");
					row.Cells.Add(cell);
					//Fee;
					cell=new GridCell("");
					row.Cells.Add(cell);
					//Rem Before
					cell=new GridCell("");
					row.Cells.Add(cell);
					//Adj Amt
					cell=new GridCell("");
					row.Cells.Add(cell);
					//Rem After
					cell=new GridCell("");
					row.Cells.Add(cell);
				}
				else {//An adjustment row
					//Adj Date
					cell=new GridCell(AdjustmentCur.AdjDate.ToShortDateString());
					row.Cells.Add(cell);
					//Provider
					cell=new GridCell(Providers.GetAbbr(AdjustmentCur.ProvNum));
					row.Cells.Add(cell);
					//Clinic
					if(PrefC.HasClinicsEnabled) {
						cell=new GridCell(Clinics.GetAbbr(AdjustmentCur.ClinicNum));
						row.Cells.Add(cell);
					}
					//Adj Type
					cell=new GridCell(Defs.GetName(DefCat.AdjTypes,AdjustmentCur.AdjType));
					row.Cells.Add(cell);
					//Fee
					cell=new GridCell();
					row.Cells.Add(cell);
					//Rem Before
					cell=new GridCell();
					row.Cells.Add(cell);
					//Adj Amt
					cell=new GridCell(Math.Round(AdjustmentCur.AdjAmt,2).ToString("c"));
					cell.ColorBackG=Color.LightCyan;
					row.Cells.Add(cell);
					//Rem After
					cell=new GridCell(Math.Round(RemAfter,2).ToString("c"));
					if(ProcedureCur==null) {//Unassigned adj
						cell=new GridCell();
					}
					cell.ColorBackG=Color.LightCyan;
					row.Cells.Add(cell);
				}
				return row;
			}
		}

		private enum AdjAmtType {
			FixedAmt,
			PercentOfRemBal,
			PercentOfFee,
		}
	}
}