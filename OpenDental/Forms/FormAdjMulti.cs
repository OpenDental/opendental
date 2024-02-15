using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media.TextFormatting;

namespace OpenDental {
	///<summary></summary>
	public partial class FormAdjMulti:FormODBase {
		#region Fields - Private
		///<summary>The list of new adjustments that the user is trying to add. Procedures associated to these procedures will always display regardless of filters.</summary>
		private List<Adjustment> _listAdjustments;
		///<summary>The helper objects that were used to comprise the main grid. Referenced when adding new adjustments with a percentage radio button checked.</summary>
		private List<ProcAdjs> _listProcAdjs=new List<ProcAdjs>();
		///<summary>The list of ProcNums that was passed into the constructor. These procedures should always display in the grid regardless of filters.</summary>
		private List<long> _listProcNums;
		///<summary>All of the account module data for the entire family which is filled once. Used to save calls to the database when changing filters. Do not manipulate this field.</summary>
		private readonly PaymentEdit.LoadData _loadData;
		private Patient _patient;
		private RigorousAdjustments _rigorousAdjustment;
		private Program _program;
		private Patient _patientGuar;
		private List<Def> _listDefsAdjPosCats;
		private List<Def> _listDefsAdjNegCats;
		private List<ProgramProperty> _listProgramPropertiesExcludedAdjTypes;
		private List<ProgramProperty> _listProgramPropertiesForClinicExcludedAdjTypes;
		private List<long> _listExcludedAdjTypeNums;
		#endregion

		///<summary>Optionally pass in a list of ProcNums to always display. Pass in a list of new adjustments that are not in the database which will always display. Any procedures associated to the adjustments passed in will always display as well.</summary>
		public FormAdjMulti(Patient patient,List<long> listProcNums=null,List<Adjustment> listAdjustments=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patient=patient;
			_listProcNums=listProcNums??new List<long>();
			_listAdjustments=listAdjustments??new List<Adjustment>();
			_loadData=PaymentEdit.GetLoadData(patient,new Payment(),true,false);
			_program=Programs.GetCur(ProgramName.Transworld);
			_patientGuar=Patients.GetGuarForPat(_patient.PatNum);
		}

		private void FormMultiAdj_Load(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.AdjustmentCreate,true) && !Security.IsAuthorized(EnumPermType.AdjustmentEditZero,true)) {
				MessageBox.Show(Lans.g("Security","Not authorized for")
					+"\r\n"+GroupPermissions.GetDesc(EnumPermType.AdjustmentCreate)+" "+Lan.g(this,"and")+" "+GroupPermissions.GetDesc(EnumPermType.AdjustmentEditZero));
				DialogResult=DialogResult.Cancel;
				return;
			}
			dateAdjustment.Text=DateTime.Today.ToShortDateString();
			_rigorousAdjustment=PrefC.GetEnum<RigorousAdjustments>(PrefName.RigorousAdjustments);
			checkOnlyTsiExcludedAdjTypes.CheckedChanged-=checkOnlyTsiExcludedAdjTypes_Checked;	
			if(_program.Enabled && Patients.IsGuarCollections(_patientGuar.PatNum)) { //Transworld program link is enabled and the patient is part of a family where the guarantor has been sent to TSI
				checkOnlyTsiExcludedAdjTypes.Checked=true;
			}
			else {
				checkOnlyTsiExcludedAdjTypes.Visible=false;
				checkOnlyTsiExcludedAdjTypes.Checked=false;
			}
			FillListBoxAdjTypes();
			checkOnlyTsiExcludedAdjTypes.CheckedChanged+=checkOnlyTsiExcludedAdjTypes_Checked;
			FillComboProv();
			if(PrefC.HasClinicsEnabled) {
				//Select 'Inherit' by default which is the unassigned option and was carefully approved by Nathan and Allen.
				comboClinic.ClinicNumSelected=0;
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
			FillGrid();
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				if(gridMain.ListGridRows[i].Tag.GetType()!=typeof(Procedure)) {
					continue;
				}
				if(_listProcNums.Contains(((Procedure)gridMain.ListGridRows[i].Tag).ProcNum)) {
					gridMain.SetSelected(i);
				}
			}
			textAmt.Select();//start cursor here
		}

		#region Methods - Fill
		private void FillComboProv() {
			comboProv.Items.Clear();
			//Create a fake provider that will be the first (and default) option within the provider combo box.
			List<Provider> listProviders=new List<Provider>() {
				new Provider() {
					ProvNum=0,
					Abbr=Lan.g(this,"Inherit"),//Inherit was carefully approved by Nathan and Allen.
					IsHidden=false,
				},
			};
			//Add all of the real providers to the list of providers for the provider combo box.
			listProviders.AddRange(Providers.GetDeepCopy(true));
			comboProv.Items.AddProvsAbbr(listProviders);
			comboProv.SelectedIndex=0;//Default to the 'Inherit' provider.
		}

		private void FillGrid() {
			_listProcAdjs=GetProcAdjs();
			List<ProcAdjs> listAssignedProcAdjs=_listProcAdjs.FindAll(x => x.ProcedureCur!=null);
			List<ProcAdjs> listUnassignedProcAdjs=_listProcAdjs.FindAll(x => x.ProcedureCur==null);
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
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TableMultiAdjs","Date"),70,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn() { Heading=Lan.g("TableMultiAdjs","Provider"), IsWidthDynamic=true };
			gridMain.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn() { Heading=Lan.g("TableMultiAdjs","Clinic"), IsWidthDynamic=true };
				gridMain.Columns.Add(col);
			}		 	 
			col=new GridColumn() { Heading=Lan.g("TableMultiAdjs","Type"), IsWidthDynamic=true };
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableMultiAdjs","Fee"),70,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableMultiAdjs","Rem Before"),70,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableMultiAdjs","Adj Amt"),70,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableMultiAdjs","Rem After"),70,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			for(int i=0;i<listAssignedProcAdjs.Count;i++) {
				//Show everything when ExcludeAll is selected. Conditionally filter out procedures if they meet certain criteria.
				//Unpaid procedures should always show per Nathan. If a procedure was explicitly selected before entering this window, always show it.
				//Also, always show the procedure if there are custom adjustments associated to it.
				if(creditCalcType.In(CreditCalcType.IncludeAll,CreditCalcType.AllocatedOnly)
					&& !_listProcNums.Contains(listAssignedProcAdjs[i].ProcNum)
					&& listAssignedProcAdjs[i].ListAccountEntryAdjustments.IsNullOrEmpty()
					&& CompareDecimal.IsZero(listAssignedProcAdjs[i].AccountEntryProc.AmountEnd))
				{
					continue;
				}
				gridMain.ListGridRows.AddRange(listAssignedProcAdjs[i].GetGridRows());
			}
			//Loop through all unallocated adjustments and add them after a custom 'Unassigned' header row.
			for(int i=0;i<listUnassignedProcAdjs.Count;i++) {
				if(i==0) {//Always add the 'Unassigned' header row first. This is because procedures typically act as the 'header' but these adjustments don't have procedures.
					GridRow row=new GridRow();
					row.Tag=null;
					row.ColorBackG=Color.LightYellow;
					row.Cells.Add(Lan.g(this,"Unassigned"));//Date
					row.Cells.Add("");//Provider
					if(PrefC.HasClinicsEnabled) {
						row.Cells.Add("");//Clinic
					}
					row.Cells.Add("");//Code
					row.Cells.Add("");//Fee
					row.Cells.Add("");//Rem Before
					row.Cells.Add("");//Adj Amt
					row.Cells.Add("");//Rem After
					gridMain.ListGridRows.Add(row);
				}
				gridMain.ListGridRows.AddRange(listUnassignedProcAdjs[i].GetGridRows());
			}
			gridMain.EndUpdate();
		}

		private void FillListBoxAdjTypes() {
			listTypePos.Items.Clear();
			listTypeNeg.Items.Clear();
			_listDefsAdjPosCats=Defs.GetPositiveAdjTypes(considerPermission:true);
			_listDefsAdjNegCats=Defs.GetNegativeAdjTypes(considerPermission:true);
			_listProgramPropertiesExcludedAdjTypes=ProgramProperties
				.GetWhere(x => x.ProgramNum==_program.ProgramNum
					&& _program.Enabled
					&& (x.PropertyDesc==ProgramProperties.PropertyDescs.TransWorld.SyncExcludePosAdjType
						|| x.PropertyDesc==ProgramProperties.PropertyDescs.TransWorld.SyncExcludeNegAdjType));
			//use guar's clinic if clinics are enabled and props for that clinic exist, otherwise use ClinicNum 0
			_listProgramPropertiesForClinicExcludedAdjTypes=_listProgramPropertiesExcludedAdjTypes.FindAll(x=>x.ClinicNum==_patientGuar.ClinicNum);
			if(!PrefC.HasClinicsEnabled || _listProgramPropertiesForClinicExcludedAdjTypes.Count==0){
				_listProgramPropertiesForClinicExcludedAdjTypes=_listProgramPropertiesExcludedAdjTypes.FindAll(x=>x.ClinicNum==0);
			}
			_listExcludedAdjTypeNums=_listProgramPropertiesForClinicExcludedAdjTypes.Select(x=>PIn.Long(x.PropertyValue,false)).ToList();
			if(checkOnlyTsiExcludedAdjTypes.Checked) {
				_listDefsAdjPosCats=_listDefsAdjPosCats.FindAll(x => _listExcludedAdjTypeNums.Contains(x.DefNum));
				_listDefsAdjNegCats=_listDefsAdjNegCats.FindAll(x => _listExcludedAdjTypeNums.Contains(x.DefNum));
				listTypePos.Items.AddList(_listDefsAdjPosCats,x=>x.ItemName);
				listTypeNeg.Items.AddList(_listDefsAdjNegCats,x=>x.ItemName);
				return;
			}
			_listDefsAdjPosCats=_listDefsAdjPosCats.FindAll(x => !_listExcludedAdjTypeNums.Contains(x.DefNum));
			_listDefsAdjNegCats=_listDefsAdjNegCats.FindAll(x => !_listExcludedAdjTypeNums.Contains(x.DefNum));
			listTypePos.Items.AddList(_listDefsAdjPosCats,x=>x.ItemName);
			listTypeNeg.Items.AddList(_listDefsAdjNegCats,x=>x.ItemName);
		}
		#endregion

		#region Methods - Private
		///<summary>Adds a singular unassigned adjustment if no procedure is currently selected otherwise adds one adjustment per procedure selected. Every adjustment created utilizes the values set in the current UI. Returns false if the UI is in an invalid state, otherwise returns true.</summary>
		private bool AddAdjustments() {
			if(!IsValid()) {
				return false;
			}
			List<Procedure> listProcedures=gridMain.SelectedTags<Procedure>();
			if(listProcedures.IsNullOrEmpty()) {//Add a singular adjustment if no procedures are selected.
				_listAdjustments.Add(GetAdjFromUI());
			}
			else {//Add an adjustment for every procedure selected.
				List<Procedure> listCompleteProcs=listProcedures.FindAll(x=>x.ProcStatus==ProcStat.C);
				//If the user does not have permission to add adjustments to any of the selected procedures, show an error message and return false.
				if(listCompleteProcs.Count()>0) {
					if(!listCompleteProcs.All(x => Security.IsAuthorized(EnumPermType.ProcCompleteAddAdj,Procedures.GetDateForPermCheck(x), suppressMessage:true))) 
					{ 
						MsgBox.Show("Not allowed to add adjustments to completed procedures exceeding date limitation.");
						return false;
					}
				}
				List<Adjustment> listAdjustmentsWarnOrBlock=new List<Adjustment>();
				EnumAdjustmentBlockOrWarn enumAdjustmentBlockOrWarn=PrefC.GetEnum<EnumAdjustmentBlockOrWarn>(PrefName.AdjustmentBlockNegativeExceedingPatPortion);
				for(int i=0;i<listProcedures.Count;i++) {
					ProcAdjs procAdjs=_listProcAdjs.First(x => x.ProcedureCur==listProcedures[i]);
					Adjustment adjustment=GetAdjFromUI(procedureSelected: listProcedures[i],
						listAdjustmentsRelated: procAdjs.ListAccountEntryAdjustments.Select(x => (Adjustment)x.Tag).ToList());//Get the new adjustment to be added from the UI.
					if(((double)procAdjs.AccountEntryProc.AmountEnd<=0 && adjustment.AdjAmt>0 && listTypeNeg.SelectedIndices.Count>0 && PIn.Decimal(textAmt.Text)>0)
						|| adjustment.AdjNum!=0)//only enforce for brand new adjustments
					{
						continue;
					}
					double adjustmentAmtTotal=_listAdjustments.FindAll(x => x.ProcNum==listProcedures[i].ProcNum).Sum(x => x.AdjAmt);//Get the sum of all new adjustments.
					//If user is adding a negative adjustment to a procedure that is already overpaid, place the new adjustment in a separate list for now if the
					//AdjustmentBlockNegativeExceedingPatPortion pref is set to someting other than allow.
					if(enumAdjustmentBlockOrWarn!=EnumAdjustmentBlockOrWarn.Allow
						&& (double)procAdjs.AccountEntryProc.AmountEnd+adjustmentAmtTotal+adjustment.AdjAmt < 0 
						&& adjustment.AdjAmt<0) {
						listAdjustmentsWarnOrBlock.Add(adjustment);
						continue;
					}
					_listAdjustments.Add(adjustment);
				}
				if(listAdjustmentsWarnOrBlock.Count>0) {
					//If pref is set to block then show error message but do not add negative adjustments to overpaid procs.
					if(enumAdjustmentBlockOrWarn==EnumAdjustmentBlockOrWarn.Block) {//block preference set
						MsgBox.Show(Lan.g(this,"Could not create a negative adjustment exceeding the remaining amount on ")+listAdjustmentsWarnOrBlock.Count+Lan.g(this," procedure(s)."));
					}
					//if pref is set to warn, then warn and only add list of negative adjustments to an overpaid proc if the user allows it.
					else if(enumAdjustmentBlockOrWarn==EnumAdjustmentBlockOrWarn.Warn) {//warning preference set
						if(MsgBox.Show(MsgBoxButtons.YesNo,Lan.g(this,"Remaining amount on ")+listAdjustmentsWarnOrBlock.Count+Lan.g(this," procedure(s) is negative. Continue?"),"Overpaid Procedure Warning")) {
							_listAdjustments.AddRange(listAdjustmentsWarnOrBlock);
						}
					}
				}
			}
			FillGrid();
			return true;
		}

		///<summary>Returns an adjustment that reflects the settings within the UI. Set adjustment in order to use that object as the adjustment that will get manipulated. Leave adjustment null for a new adjustment to be created. Set procedureSelected to the procedure that is currently selected or to the procedure that is associated to the selected adjustment. Set listAdjustmentsRelated to a list of adjustments that should be considered for calculating 'Percentage of Remaining Balance' (when there are multiple adjustments associated to the same procedure).</summary>
		private Adjustment GetAdjFromUI(Adjustment adjustment=null,Procedure procedureSelected=null,List<Adjustment> listAdjustmentsRelated=null) {
			if(listAdjustmentsRelated==null) {
				listAdjustmentsRelated=new List<Adjustment>();
			}
			Def defAdjType=GetSelectedAdjDef();
			if(adjustment==null) {
				adjustment=new Adjustment();
			}
			adjustment.AdjType=defAdjType.DefNum;
			adjustment.AdjDate=PIn.Date(dateAdjustment.Text);
			adjustment.AdjNote=PIn.String(textNote.Text);
			long clinicNumSelected=0;
			if(PrefC.HasClinicsEnabled) {
				if(comboClinic.IsUnassignedSelected) {//Inherit
					if(procedureSelected==null) {
						clinicNumSelected=_patient.ClinicNum;
					}
					else {
						clinicNumSelected=procedureSelected.ClinicNum;
					}
				}
				else {
					clinicNumSelected=comboClinic.ClinicNumSelected;
				}
			}
			adjustment.ClinicNum=clinicNumSelected;
			adjustment.PatNum=_patient.PatNum;
			adjustment.ProcDate=procedureSelected?.ProcDate??DateTime.MinValue;
			adjustment.ProcNum=procedureSelected?.ProcNum??0;
			long provNum=comboProv.GetSelectedProvNum();
			if(provNum==0) {//Inherit
				if(procedureSelected==null) {
					provNum=_patient.PriProv;
				}
				else {
					provNum=procedureSelected.ProvNum;
				}
			}
			adjustment.ProvNum=provNum;
			double adjAmtOrPerc=PIn.Double(textAmt.Text);
			ProcAdjs procAdjs=null;
			if(procedureSelected!=null) {
				procAdjs=_listProcAdjs.First(x => x.ProcedureCur==procedureSelected);
			}
			switch(GetAdjAmtType()) {
				case AdjAmtType.FixedAmt:
					adjustment.AdjAmt=adjAmtOrPerc;
					break;
				case AdjAmtType.PercentOfFee:
					//The procedure row always shows the AmountOriginal value from the AccountEntryProc object in the Fee column.
					adjustment.AdjAmt=Math.Round((adjAmtOrPerc/100)*(double)procAdjs.AccountEntryProc.AmountOriginal,2);
					break;
				case AdjAmtType.PercentOfRemBal:
					//Calculate the remaining balance based off of the adjustments that have been added to the selected procedure, excluding the one passed in.
					double adjSum=listAdjustmentsRelated.Sum(x => x.AdjAmt);
					double amtRem=(double)procAdjs.AccountEntryProc.AmountEnd + adjSum;
					adjustment.AdjAmt=Math.Round((adjAmtOrPerc/100)*amtRem,2);
					break;
			}
			if(defAdjType.ItemValue=="-") {
				adjustment.AdjAmt*=-1;
			}
			return adjustment;
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

		///<summary>Returns a list of helper objects that should be displayed to the user.</summary>
		private List<ProcAdjs> GetProcAdjs() {
			//Invoke the 'construct and link charge credits' method in order to apply the value of the new adjustments to other account entries.
			//Setting isIncomeTxfr to True will only perform explicit linking; False will perform both explicit and implicit.
			//The 'Only allocated credits' and 'Exclude all credits' radio boxes should perform explicit linking only (isIncomeTxfr = true)
			//The 'Include all credits' radio box should perform both explicit and implicit linking (isIncomeTxfr = false)
			PaymentEdit.ConstructResults constructResults=PaymentEdit.ConstructAndLinkChargeCredits(_patient.PatNum,
				listPatNums:ListTools.FromSingle(_patient.PatNum),
				isIncomeTxfr:!radioIncludeAll.Checked,
				loadData:_loadData,
				hasInsOverpay:true);
			//Manually add the adjustments that were added within this window to the list of account entries.
			constructResults.ListAccountEntries.AddRange(_listAdjustments.Select(x => new AccountEntry(x)));
			//Project the list of account entries into helper objects after grouping them up by procedures. Unassigned adjustments should show last.
			return constructResults.ListAccountEntries
				.FindAll(x => x.PatNum==_patient.PatNum
					&& (x.GetType()==typeof(Adjustment) && _listAdjustments.Contains(x.Tag) || (x.GetType()==typeof(Procedure) && ((Procedure)x.Tag).ProcStatus==ProcStat.C)))
				.GroupBy(x => x.ProcNum)
				.ToDictionary(x => x.Key,x => x.ToList())
				.Select(x => new ProcAdjs(x.Value))
				.OrderBy(x => x.ProcedureCur==null)
				.ToList();
		}

		private Def GetSelectedAdjDef() {
			Def selectedAdjType=listTypePos.GetSelected<Def>();
			if(selectedAdjType==null) {
				//Nothing was selected in listTypePos so there has to be a selection for negative.
				selectedAdjType=listTypeNeg.GetSelected<Def>();
			}
			return selectedAdjType;
		}

		private bool IsValid() {
			List<Procedure> listProceduresSelected=gridMain.SelectedTags<Procedure>();
			//No procedures selected AND (rigorous adjustments are fully enforced OR the Fixed Amount option is not selected).
			//This is because rigorous accounting requires procedures and the two percentage radio buttons only work when a procedure is selected.
			if(listProceduresSelected.IsNullOrEmpty() 
				&& (_rigorousAdjustment==RigorousAdjustments.EnforceFully || GetAdjAmtType()!=AdjAmtType.FixedAmt))
			{
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
			if(PrefC.HasClinicsEnabled && comboClinic.ClinicNumSelected==-1) {
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
			List<OrthoProcLink> listOrthoProcLinks=OrthoProcLinks.GetManyForProcs(listProceduresSelected.Select(x => x.ProcNum).ToList());
			if(listOrthoProcLinks.Count>0) {
				MsgBox.Show(this,"One or more of the selected procedures cannot be adjusted because it is attached to an ortho case." +
					" Please deselect these items and try again.");
				return false;
			}
			return true;
		}
		#endregion

		#region Events
		private void butAdd_Click(object sender,EventArgs e) {
			AddAdjustments();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			List<Adjustment> listAdjustmentsSelected=gridMain.SelectedTags<Adjustment>();
			if(listAdjustmentsSelected.IsNullOrEmpty()) {
				return;//Nothing to delete.
			}
			//Removing the adjustments from the class wide list will cause the FillGrid method to remove them from the grid.
			for(int i=0;i<listAdjustmentsSelected.Count;i++) {
				_listAdjustments.Remove(listAdjustmentsSelected[i]);
			}
			FillGrid();
		}

		private void butPickProv_Click(object sender,EventArgs e) {
			//Don't send the fake "Inherit" provider into the Providers picker window (even though it probably wouldn't hurt anything).
			List<Provider> listProviders=comboProv.Items.GetAll<Provider>().FindAll(x => x.ProvNum > 0);
			FrmProviderPick frmProviderPick=new FrmProviderPick(listProviders);
			frmProviderPick.ShowDialog();
			if(frmProviderPick.IsDialogOK) {
				comboProv.SelectedIndex=-1;
				comboProv.SetSelectedKey<Provider>(frmProviderPick.ProvNumSelected,x => x.ProvNum);
			}
		}

		private void butUpdate_Click(object sender,EventArgs e) {
			List<Adjustment> listAdjustments=gridMain.SelectedTags<Adjustment>();
			if(listAdjustments.IsNullOrEmpty()) {
				return;//No adjustments to update.
			}
			bool hasSkipped=false;
			for(int i=0;i<listAdjustments.Count;i++) {
				Procedure procedure=null;
				List<Adjustment> listAdjustmentsRelated=new List<Adjustment>();
				ProcAdjs procAdjs=_listProcAdjs.Where(x => x.ProcNum > 0).FirstOrDefault(x => x.ProcNum==listAdjustments[i].ProcNum);
				//Skip this adjustment if the user has a percentage radio button selected but select an adjustment that is not associated to a procedure.
				if(procAdjs==null) {
					if(GetAdjAmtType().In(AdjAmtType.PercentOfFee,AdjAmtType.PercentOfRemBal)) {
						continue;//There is no such thing as updating an unassigned adjustment with one of the percentage amount options.
					}
				}
				else {//There is a procedure associated to the selected adjustment.
					procedure=procAdjs.ProcedureCur;
					//Grab all of the adjustments that fall before this adjustment in the list of adjustments that are displaying (for percentage purposes).
					int indexAdjEntry=procAdjs.ListAccountEntryAdjustments.FindIndex(x => x.Tag==listAdjustments[i]);
					for(int j=indexAdjEntry-1;j>=0;j--) {
						listAdjustmentsRelated.Add((Adjustment)procAdjs.ListAccountEntryAdjustments[j].Tag);
					}
				}
				//zero adjustments are checked in the save click
				if(!Security.IsAuthorized(EnumPermType.AdjustmentCreate,dateAdjustment.Value,true) && listAdjustments[i].AdjAmt!=0) {
					hasSkipped=true;
					continue;
				}
				//Pass in a shallow copy of the adjustment which will get directly manipulated / updated.
				GetAdjFromUI(adjustment:listAdjustments[i],procedureSelected:procedure,listAdjustmentsRelated:listAdjustmentsRelated);
			}
			if(hasSkipped) {
				MsgBox.Show(this,"Adjustment amount has to be 0.00 due to security permission.");
			}
			FillGrid();
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			List<Adjustment> listAdjustmentsSelected=gridMain.SelectedTags<Adjustment>();
			if(listAdjustmentsSelected.IsNullOrEmpty()) {
				return;//No adjustments were selected.
			}
			textNote.Text=listAdjustmentsSelected.First().AdjNote;
		}

		private void listTypeNeg_SelectedIndexChanged(object sender,EventArgs e) {
			if(listTypeNeg.SelectedIndex>-1) {
				listTypePos.SelectedIndex=-1;
			}
		}

		private void listTypePos_SelectedIndexChanged(object sender,EventArgs e) {
			if(listTypePos.SelectedIndex>-1) {
				listTypeNeg.SelectedIndex=-1;
			}
		}

		private void radioCredits_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void radioFixedAmt_CheckedChanged(object sender,EventArgs e) {
			if(radioFixedAmt.Checked) {
				labelAmount.Text=Lan.g(this,"Amount");
			}
			else {
				labelAmount.Text=Lan.g(this,"Percent");
			}
		}
		#endregion

		private void butSave_Click(object sender,EventArgs e) {
			if(_listAdjustments.IsNullOrEmpty()) {//No adjustments have been added. Attempt to add adjustments with the current info.
				if(!AddAdjustments()) {
					return;//The UI is in an invalid state so no new adjustments can be created. This preserves old behavior
				}
			}
			EnumAdjustmentBlockOrWarn enumAdjustmentBlockOrWarn=PrefC.GetEnum<EnumAdjustmentBlockOrWarn>(PrefName.AdjustmentBlockNegativeExceedingPatPortion);
			if(enumAdjustmentBlockOrWarn!=EnumAdjustmentBlockOrWarn.Allow){
				int count=0;
				List<ProcAdjs> listProcAdjs=GetProcAdjs().Where(x => x.AccountEntryProc!=null).ToList();
				for(int i=0;i<listProcAdjs.Count;i++) {
					decimal sum=0;
					if(listProcAdjs[i].ListAccountEntryAdjustments.All(x => x.AdjNum!=0)) {//No new adjusments were created, do not enfore preference
						continue;
					}
					for(int k=0;k<listProcAdjs[i].ListAccountEntryAdjustments.Count;k++) {
						sum+=listProcAdjs[i].ListAccountEntryAdjustments[k].AmountEnd;
					}
					if((listProcAdjs[i].AccountEntryProc.AmountEnd+sum) < 0) {
						count++;
					}
				}
				if(count>0) {
					if(enumAdjustmentBlockOrWarn==EnumAdjustmentBlockOrWarn.Warn) {
						if(!MsgBox.Show(MsgBoxButtons.YesNo,Lan.g(this,"There are")+" "+count+" "+Lan.g(this,"procedure(s) with negative amounts.")+" "+Lan.g(this,"Proceed?"))) {
							return;
						}
					}
					if(enumAdjustmentBlockOrWarn==EnumAdjustmentBlockOrWarn.Block) {
						MsgBox.Show(count+ " " +Lan.g(this,"procedure(s) cannot have their adjustments updated due to negative remaining values.")+" "+Lan.g(this,"Please fix to save"));
						return;
					}
				}
			}
			if(_program.Enabled && Patients.IsGuarCollections(_patientGuar.PatNum)) {
				_listExcludedAdjTypeNums=_listProgramPropertiesForClinicExcludedAdjTypes.Select(x=>PIn.Long(x.PropertyValue,false)).ToList();
				List<Adjustment> listAdjustmentsPosExcluded=_listAdjustments.FindAll(x=>_listExcludedAdjTypeNums.Contains(x.AdjType));
				List<Adjustment> listAdjustmentsNegExcluded=_listAdjustments.FindAll(x=>_listExcludedAdjTypeNums.Contains(x.AdjType));
				string msgTxt="The guarantor of this family has been sent to TSI for a past due balance and you have selected an adjustment type that will be synched with TSI."
					+" This balance adjustment could result in a TSI charge for collection. Continue?";
				if(listAdjustmentsPosExcluded.Count()>0 || listAdjustmentsNegExcluded.Count()>0) {
					msgTxt="The guarantor of this family has been sent to TSI for a past due balance and you have selected an adjustment type that is excluded from being synched with TSI."
						+" This will not reduce the balance sent for collection by TSI. Continue?";
				}
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,msgTxt)) {
					return;
				}
			}
			//Now we should be guaranteed to have adjustments.
			//Make a deep copy of _loadData since we're going to be faking it like the adjustments have already been inserted into the database.
			PaymentEdit.LoadData loadData=_loadData.Copy();
			//Act like the new adjustments have been inserted into the database which should manipulate any corresponding procedure AmountEnd values.
			loadData.ConstructChargesData.ListAdjustments.AddRange(_listAdjustments);
			PaymentEdit.ConstructResults constructResults=PaymentEdit.ConstructAndLinkChargeCredits(_patient.PatNum,
				listPatNums:ListTools.FromSingle(_patient.PatNum),
				isIncomeTxfr:!radioIncludeAll.Checked,
				loadData:loadData,hasInsOverpay:true);
			if(!Security.IsAuthorized(EnumPermType.AdjustmentCreate,PIn.Date(dateAdjustment.Text),true) 
				||_listAdjustments.Any(x =>!Security.IsAuthorized(EnumPermType.AdjustmentCreate,x.AdjDate,true))) {//User does not have full edit permission.
				//Therefore the user only has the ability to edit $0 adjustments (see Load()).
				if(_listAdjustments.Any(x => !CompareDouble.IsZero(x.AdjAmt))) {
					MsgBox.Show(this,"Amount has to be 0.00 due to security permission.");
					return;
				}
			}
			List<string> listStringAdjustmentAmounts=new List<string>();
			for(int i=0;i<_listAdjustments.Count;i++) { 
				Adjustments.Insert(_listAdjustments[i]);
				TsiTransLogs.CheckAndInsertLogsIfAdjTypeExcluded(_listAdjustments[i]);
				listStringAdjustmentAmounts.Add(_listAdjustments[i].AdjAmt.ToString("c"));
			}
			if(listStringAdjustmentAmounts.Count>0) {
				string log=Lan.g(this,"Adjustment(s) created from Add Multiple Adjustments window:")+" ";
				SecurityLogs.MakeLogEntry(EnumPermType.AdjustmentCreate,_patient.PatNum,log+string.Join(",",listStringAdjustmentAmounts));
			}
			Signalods.SetInvalid(InvalidType.BillingList);
			DialogResult=DialogResult.OK;
		}


		///<summary>Helper class that ties a procedure and its assigned adjustments together. AccountEntryProc will be null for unassigned adjustments.</summary>
		private class ProcAdjs {
			public AccountEntry AccountEntryProc;
			public List<AccountEntry> ListAccountEntryAdjustments;

			public long ProcNum {
				get {
					long procNum=0;
					if(AccountEntryProc!=null) {
						procNum=((Procedure)AccountEntryProc.Tag).ProcNum;
					}
					return procNum;
				}
			}

			public Procedure ProcedureCur {
				get {
					Procedure procedure=null;
					if(AccountEntryProc!=null) {
						procedure=(Procedure)AccountEntryProc.Tag;
					}
					return procedure;
				}
			}

			public ProcAdjs(List<AccountEntry> listAccountEntries) {
				AccountEntryProc=listAccountEntries.FirstOrDefault(x => x.GetType()==typeof(Procedure));
				ListAccountEntryAdjustments=listAccountEntries.FindAll(x => x.GetType()==typeof(Adjustment));
			}

			///<summary>Returns a list of grid row objects that represent this entire helper class. Grid rows that represent procedures will have the Procedure object set as the tag; Adjustment rows will have the corresponding Adjustment object as the tag.</summary>
			public List<GridRow> GetGridRows() {
				double amtRemBefore=0;
				List<GridRow> listGridRows=new List<GridRow>();
				//Procedure row---------------------------------------------------------------------------------
				if(AccountEntryProc!=null) {
					GridRow row=new GridRow();
					row.Tag=ProcedureCur;
					row.Cells.Add(ProcedureCur.ProcDate.ToShortDateString());//Date
					row.Cells.Add(Providers.GetAbbr(ProcedureCur.ProvNum));//Provider
					if(PrefC.HasClinicsEnabled) {
						row.Cells.Add(Clinics.GetAbbr(ProcedureCur.ClinicNum));//Clinic
					}
					row.Cells.Add(ProcedureCodes.GetStringProcCode(ProcedureCur.CodeNum));//Type
					GridCell gridCellFee=new GridCell(AccountEntryProc.AmountOriginal.ToString("c")) { ColorBackG=Color.LightYellow };
					row.Cells.Add(gridCellFee);//Fee
					amtRemBefore=(double)AccountEntryProc.AmountEnd;
					GridCell gridCellRemBefore=new GridCell(amtRemBefore.ToString("c")) { ColorBackG=Color.LightYellow };
					row.Cells.Add(gridCellRemBefore);//Rem Before
					row.Cells.Add("");//Adj Amt
					row.Cells.Add("");//Rem After
					listGridRows.Add(row);
				}
				//Adjustment rows-------------------------------------------------------------------------------
				for(int i=0;i<ListAccountEntryAdjustments.Count;i++) {
					GridRow row=new GridRow();
					Adjustment adjustment=(Adjustment)ListAccountEntryAdjustments[i].Tag;
					row.Tag=adjustment;
					row.Cells.Add(adjustment.AdjDate.ToShortDateString());//Adj Date
					row.Cells.Add(Providers.GetAbbr(adjustment.ProvNum));//Provider
					if(PrefC.HasClinicsEnabled) {
						row.Cells.Add(Clinics.GetAbbr(adjustment.ClinicNum));//Clinic
					}
					row.Cells.Add(Defs.GetName(DefCat.AdjTypes,adjustment.AdjType));//Adj Type
					row.Cells.Add("");//Fee
					row.Cells.Add("");//Rem Before
					double adjAmt=Math.Round(adjustment.AdjAmt,2);
					GridCell gridCellAdjAmt=new GridCell(adjAmt.ToString("c")) { ColorBackG=Color.LightCyan };
					row.Cells.Add(gridCellAdjAmt);//Adj Amt
					GridCell gridCellRemAfter=new GridCell() { ColorBackG=Color.LightCyan };
					if(AccountEntryProc!=null) {
						amtRemBefore+=adjAmt;
						gridCellRemAfter.Text=amtRemBefore.ToString("c");
					}
					row.Cells.Add(gridCellRemAfter);//Rem After
					listGridRows.Add(row);
				}
				return listGridRows;
			}
		}

		private enum AdjAmtType {
			FixedAmt,
			PercentOfRemBal,
			PercentOfFee,
		}

		private void checkOnlyTsiExcludedAdjTypes_Checked(object sender,EventArgs e) {
			FillListBoxAdjTypes();
		}
	}
}