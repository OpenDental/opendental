using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;
using OpenDentBusiness.WebTypes;
using System.Text;
using OpenDentBusiness.WebTypes.Shared.XWeb;
using PdfSharp.Pdf;

namespace OpenDental {

	///<summary></summary>
	public partial class ControlAccount:UserControl {
		#region Fields - Public
		///<summary>Public so this can be checked from FormOpenDental and the note can be saved.  Necessary because in some cases the leave event doesn't
		///fire, like when a user switches to a non-modal form, like big phones, and switches patients from that form.</summary>
		public bool IsFinNoteChanged;
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		///<summary>Public so this can be checked from FormOpenDental and the note can be saved.  Necessary because in some cases the leave event doesn't
		///fire, like when a user switches to a non-modal form, like big phones, and switches patients from that form.</summary>
		public bool IsUrgFinNoteChanged;
		#endregion Fields - Public	

		#region Fields - Private
		private int _scrollValueWhenDoubleClick=-1;
		private int _scrollValueWhenDoubleClickTpUnearned=-1;
		///<summary>This holds some of the data needed for display.  It is retrieved in one call to the database.</summary>
		private DataSet _dataSetMain;
		private Def[] _arrayDefsAcctProcQuickAdd;
		private Family _family;
		private FormRpServiceDateView _formRpServiceDateView=null;
		private bool _isInitializedOnStartup;
		private List<DisplayField> _listDisplayFieldsForMainGrid;
		///<summary>List of all orthocases for the selected patient.</summary>
		private List<OrthoCase> _listOrthoCases=new List<OrthoCase>();
		private PatField[] _patFieldArray;
		private List<DisplayField> _listDisplayFieldsPatInfo;
		private RepeatCharge[] _repeatChargeArray;
		///<summary>Shows a breakdown of payment split totals by each unearned type.</summary>
		private Label labelUnearnedBreakdown=new Label();
		private List<PaySplit> _listPaySplitsHidden=new List<PaySplit>();
		///<summary>This holds nearly all of the data needed for display.  It is retrieved in one call to the database.</summary>
		private AccountModules.LoadData _loadData;
		///<summary>Partially implemented lock object for an attempted bug fix.</summary>
		private object _lockDataSetMain=new object();
		private GridOD gridUnearnedBreakdown=new GridOD() { TitleVisible=false,VScrollVisible=false,Visible=false };
		///<summary></summary>
		private Patient _patient;
		private PatientNote _patientNote;
		///<summary>Gets updated to PatCur.PatNum that the last security log was made with so that we don't make too many security logs for this patient.  When _patNumLast no longer matches PatCur.PatNum (e.g. switched to a different patient within a module), a security log will be entered.  Gets reset (cleared and the set back to PatCur.PatNum) any time a module button is clicked which will cause another security log to be entered.</summary>
		private long _patNumLast;
		private decimal _patientPortionBalanceTotal;
		private string _famUrgFinNoteOnLoad;
		private string _famFinNoteOnLoad;
		///<summary>Used to track status of split panel visibility to avoid flicker from changing status too often.</summary>
		private bool _showGridPayPlan;
		///<summary>Used to track status of split panel visibility to avoid flicker from changing status too often.</summary>
		private bool _showGridRepeating;
		#endregion Fields - Private	
	
		#region Constructor
		///<summary></summary>
		public ControlAccount() {
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.Start);
			InitializeComponent();// This call is required by the Windows.Forms Form Designer.
			Font=LayoutManagerForms.FontInitial;
			LayoutManager.Add(gridUnearnedBreakdown,this);
			LayoutManager.Add(textQuickProcs,ToolBarMain);
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.End);
		}
		#endregion Constructor

		#region Delegates
		private delegate void ToolBarClick();
		#endregion Delegates

		#region Structs Nested
		private struct AutoOrthoPat {
			public InsPlan InsPlan_;
			public PatPlan PatPlan_;
			public string CarrierName;
			public string SubID;
			public double DefaultFee;
		}
		#endregion Structs Nested

		#region Properties
		public long GetPatNum() {
			return _patient.PatNum;
		} 

		///<summary>True if 'Entire Family' is selected in the Select Patient grid.</summary>
		private bool IsFamilySelected() {
			if(_dataSetMain==null) {
				return false;
			}
			return gridAcctPat.GetSelectedIndex()==gridAcctPat.ListGridRows.Count-1;//last row
		}
		#endregion Properties	

		#region Methods - Event Handlers Buttons
		private void but45days_Click(object sender,EventArgs e) {
			textDateStart.Text=DateTime.Today.AddDays(-45).ToShortDateString();
			textDateEnd.Text="";
			ModuleSelected(_patient.PatNum);
		}

		private void but90days_Click(object sender,EventArgs e) {
			textDateStart.Text=DateTime.Today.AddDays(-90).ToShortDateString();
			textDateEnd.Text="";
			ModuleSelected(_patient.PatNum);
		}

		private void ButAddOrthoCase_Click(object sender,EventArgs e) {
			using FormOrthoCase formOrthoCase=new FormOrthoCase(true,_patient);
			formOrthoCase.ShowDialog();
			ModuleSelected(_patient.PatNum);
		}

		private void butAutoOrthoDefaultMonthsTreat_Click(object sender,EventArgs e) {
			//Setting OrthoMonthsTreatOverride locks this value into place just in case it the pref changes down the road.
			_patientNote.OrthoMonthsTreatOverride=PrefC.GetByte(PrefName.OrthoDefaultMonthsTreat);
			PatientNotes.Update(_patientNote,_patient.Guarantor);
			FillAutoOrtho();
		}

		private void butAutoOrthoDefaultPlacement_Click(object sender,EventArgs e) {
			_patientNote.DateOrthoPlacementOverride=DateTime.MinValue;
			PatientNotes.Update(_patientNote,_patient.Guarantor);
			FillAutoOrtho();
		}

		private void butAutoOrthoEditMonthsTreat_Click(object sender,EventArgs e) {
			int txMonths;
			try {
				txMonths=PIn.Byte(textAutoOrthoMonthsTreat.Text);
			}
			catch {
				MsgBox.Show(this,"Please enter a number between 0 and 255.");
				return;
			}
			_patientNote.OrthoMonthsTreatOverride=txMonths;
			PatientNotes.Update(_patientNote,_patient.Guarantor);
			FillAutoOrtho();
		}

		private void butCreditCard_Click(object sender,EventArgs e) {
			using FormCreditCardManage formCreditCardManage=new FormCreditCardManage(_patient);
			formCreditCardManage.ShowDialog();
		}

		private void butDatesAll_Click(object sender,EventArgs e) {
			textDateStart.Text="";
			textDateEnd.Text="";
			ModuleSelected(_patient.PatNum);
		}

		private void butEditAutoOrthoPlacement_Click(object sender,EventArgs e) {
			DateTime dateOrthoPlacement;
			try {
				dateOrthoPlacement=PIn.Date(textDateAutoOrthoPlacement.Text);
			}
			catch {
				MsgBox.Show(this,"Invalid date.");
				return;
			}
			_patientNote.DateOrthoPlacementOverride=dateOrthoPlacement;
			PatientNotes.Update(_patientNote,_patient.Guarantor);
			FillAutoOrtho();
		}

		private void ButMakeOrthoCaseActive_Click(object sender,EventArgs e) {
			if(gridOrthoCases.SelectedGridRows.Count<1) {
				return;
			}
			OrthoCase orthoCase=(OrthoCase)gridOrthoCases.SelectedGridRows[0].Tag;
			_listOrthoCases=OrthoCases.Activate(orthoCase,_patient.PatNum);
			RefreshOrthoCasesGridRows();
			OrthoProcLink orthoProcLink=OrthoProcLinks.GetByType(orthoCase.OrthoCaseNum,OrthoProcType.Debond);
			if(orthoProcLink!=null) {//If link exists debond proc must be complete
				MsgBox.Show(this,"The activated Ortho Case has a completed debond procedure. This procedure must be detached before others can be added.");
			}
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			if(_patient==null){
				return;
			}
			ModuleSelected(_patient.PatNum);
		}

		private void butServiceDateView_Click(object sender,EventArgs e) {
			//If the window is already open and it's for the same patient, bring the window to front. Otherwise close and/or open it.
			long patNum=_patient.PatNum;
			if(IsFamilySelected()) {
				patNum=_family.Guarantor.PatNum;
			}
			if(_formRpServiceDateView!=null && (_formRpServiceDateView.PatNum!=patNum || _formRpServiceDateView.IsFamily!=IsFamilySelected())) {
				_formRpServiceDateView.Close();
				_formRpServiceDateView=null;
			}
			if(_formRpServiceDateView==null || _formRpServiceDateView.IsDisposed) {
				_formRpServiceDateView=new FormRpServiceDateView(patNum,IsFamilySelected());
				_formRpServiceDateView.FormClosed+=new FormClosedEventHandler((o,e1) => {_formRpServiceDateView=null;});
				_formRpServiceDateView.Show();
			}
			if(_formRpServiceDateView.WindowState==FormWindowState.Minimized) {
				_formRpServiceDateView.WindowState=FormWindowState.Normal;
			}
			_formRpServiceDateView.BringToFront();
		}

		private void butToday_Click(object sender,EventArgs e) {
			textDateStart.Text=DateTime.Today.ToShortDateString();
			textDateEnd.Text=DateTime.Today.ToShortDateString();
			ModuleSelected(_patient.PatNum);
		}
		#endregion Methods - Event Handlers Buttons

		#region Methods - Event Handlers CheckBoxes
		private void CheckHideInactiveOrthoCases_CheckedChanged(object sender,EventArgs e) {
			RefreshOrthoCasesGridRows();
		}

		private void checkShowCompletePayPlans_Click(object sender,EventArgs e) {
			Prefs.UpdateBool(PrefName.AccountShowCompletedPaymentPlans,checkShowCompletePayPlans.Checked);
			FillPaymentPlans();
			RefreshModuleScreen(false); //so the grids get redrawn if the payment plans grid hides/shows itself.
		}

		private void checkShowDetail_Click(object sender,EventArgs e) {
			UserOdPref userOdPref=UserOdPrefs.GetFirstOrNewByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.AcctProcBreakdown);
			userOdPref.ValueString=POut.Bool(checkShowDetail.Checked);
			UserOdPrefs.Upsert(userOdPref);
			DataValid.SetInvalid(InvalidType.UserOdPrefs);
			if(_patient==null){
				return;
			}
			ModuleSelected(_patient.PatNum);
		}

		private void checkShowFamilyComm_Click(object sender,EventArgs e) {
			FillComm();
		}

		///<summary>Uses the UserODPref to store ShowAutomatedCommlog separately from the chart module.</summary>
		private void checkShowAutoComm_Click(object sender, EventArgs e) {
			UserOdPref userOdPref=UserOdPrefs.GetFirstOrNewByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.ShowAutomatedCommlog);
			userOdPref.ValueString=POut.Bool(checkShowCommAuto.Checked);
			UserOdPrefs.Upsert(userOdPref);
			DataValid.SetInvalid(InvalidType.UserOdPrefs);
			if(_patient==null) {
				return;
			}
			ModuleSelected(_patient.PatNum);
		}
		#endregion Methods - Event Handlers CheckBoxes

		#region Methods - Event Handlers ContextMenus
		///<summary>Hides the 'Add Adjustment' and 'Refund' context menus if anything other than a procedure (for adjustment) or one single payment (for refund) is selected.</summary>
		private void contextMenuAcctGrid_Popup(object sender,EventArgs e) {
			DataTable table=_dataSetMain.Tables["account"];
			List<int> listIdxRowsSelected=gridAccount.SelectedIndices.ToList();
			//Add Adjustment---------------------------------------------------------------------------------------------------
			menuItemAddAdj.Enabled=true;
			//Disable the Add Adjustment menu item if any non-procedure rows are selected.
			for(int i=0;i<listIdxRowsSelected.Count;i++) {
				if(table.Rows[listIdxRowsSelected[i]]["ProcNum"].ToString()!="0") {
					continue;
				}
				menuItemAddAdj.Enabled=false;
				break;
			}
			//Refund-----------------------------------------------------------------------------------------------------------
			Payment payment=null;
			menuItemAddRefund.Enabled=true;
			menuItemAddRefundWorkNotPerformed.Enabled=true;
			for(int i=0;i<listIdxRowsSelected.Count;i++) {
				long payNum=PIn.Long(table.Rows[listIdxRowsSelected[i]]["PayNum"].ToString());
				if(payNum==0) {
					continue;//something is selected that's not a payment, move on.
				}
				if(payment==null) {//found the first valid payment
					payment=Payments.GetPayment(payNum);
				}
				if(payment.PayNum!=payNum) {
					//more than one payment was selected and they aren't the same payment.
					menuItemAddRefund.Enabled=false;
					menuItemAddRefundWorkNotPerformed.Enabled=false;
					break;
				}
			}
			//Disable the refund menu item if no payments were selected.
			if(payment==null){
				menuItemAddRefund.Enabled=false;
				menuItemAddRefundWorkNotPerformed.Enabled=false;
			}
			//Delete PayPlan Charge--------------------------------------------------------------------------------------------
			menuItemDeletePayPlanCharge.Visible=false;
			if(!Security.IsAuthorized(EnumPermType.PayPlanEdit,suppressMessage:true)){
				return;
			}
			for(int i=0;i<listIdxRowsSelected.Count;i++) {
				long payPlanChargeNum=PIn.Long(table.Rows[listIdxRowsSelected[i]]["PayPlanChargeNum"].ToString());
				if(payPlanChargeNum==0) {
					continue;
				}
				menuItemDeletePayPlanCharge.Visible=true;
			}
		}

		private void contextMenuPayment_Popup(object sender, EventArgs e){
			if(PrefC.GetBool(PrefName.ShowIncomeTransferManager)){
				menuItemIncomeTransfer.Visible=true;
			}
			else{
				menuItemIncomeTransfer.Visible=false;
			}
		}

		///<summary>This gets run just prior to the contextMenuQuickCharge menu displaying to the user.</summary>
		private void contextMenuQuickProcs_Popup(object sender,EventArgs e) {
			//Dynamically fill contextMenuQuickCharge's menu items because the definitions may have changed since last time it was filled.
			_arrayDefsAcctProcQuickAdd=Defs.GetDefsForCategory(DefCat.AccountQuickCharge,true).ToArray();
			contextMenuQuickProcs.MenuItems.Clear();
			for(int i=0;i<_arrayDefsAcctProcQuickAdd.Length;i++) {
				contextMenuQuickProcs.MenuItems.Add(new MenuItem(_arrayDefsAcctProcQuickAdd[i].ItemName,menuItemQuickProcs_Click));
			}
			if(_arrayDefsAcctProcQuickAdd.Length==0) {
				contextMenuQuickProcs.MenuItems.Add(new MenuItem(Lan.g(this,"No quick charge procedures defined. Go to Setup | Definitions to add."),(x,y) => { }));//"null" event handler.
			}
		}
		#endregion Methods - Event Handlers ContextMenus

		#region Methods - Event Handlers ContrAccount
		private void ContrAccount_Layout(object sender,LayoutEventArgs e) {
			//see LayoutPanels()
		}

		private void ContrAccount_Load(object sender,EventArgs e) {
			this.Parent.MouseWheel+=new MouseEventHandler(Parent_MouseWheel);
			if(!PrefC.IsODHQ) {
				menuPrepayment.Visible=false;
				menuPrepayment.Enabled=false;
			}
		}

		private void ContrAccount_Resize(object sender,EventArgs e) {
			if(PrefC.HListIsNull()){
				return;//helps on startup.
			}
			LayoutPanelsAndRefreshMainGrids();
		}
		#endregion Methods - Event Handlers ContrAccount

		#region Methods - Event Handlers Forms
		/// <summary>Event handler for closing FormSheetFillEdit when it is non-modal.</summary>
		private void FormSheetFillEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(((FormSheetFillEdit)sender).DialogResult==DialogResult.OK || ((FormSheetFillEdit)sender).DidChangeSheet) {
				ModuleSelected(_patient.PatNum);
			}
		}
		#endregion Methods - Event Handlers Forms

		#region Methods - Event Handlers Grids
		private void gridAccount_CellClick(object sender,ODGridClickEventArgs e) {
			DataTable table=_dataSetMain.Tables["account"];
			//this seems to fire after a doubleclick, so this prevents error:
			if(e.Row>=table.Rows.Count){
				return;
			}
			gridPayPlan.SetAll(false);
			for(int i=0;i<gridAccount.SelectedIndices.Count();i++) {
				if(gridAccount.SelectedIndices[i]>table.Rows.Count-1){
					continue;//An office was getting an exception here, but we're not sure how grid and table could be out of sync and can't duplicate.
				}
				if(table.Rows[gridAccount.SelectedIndices[i]]["PayPlanNum"].ToString()=="0") {
					continue;
				}
				for(int j=0;j<gridPayPlan.ListGridRows.Count;j++) {
					if(((DataRow)(gridPayPlan.ListGridRows[j].Tag))["PayPlanNum"].ToString()==table.Rows[gridAccount.SelectedIndices[i]]["PayPlanNum"].ToString()) {
						gridPayPlan.SetSelected(j,true);
					}
				}
				if(table.Rows[gridAccount.SelectedIndices[i]]["procsOnObj"].ToString()=="0") {
					continue;
				}
				for(int j=0;j<table.Rows.Count;j++) {//loop through all rows
					if(j>=gridAccount.ListGridRows.Count) {
						break;
					}
					if(table.Rows[j]["ProcNum"].ToString()==table.Rows[gridAccount.SelectedIndices[i]]["procsOnObj"].ToString()) {
						gridAccount.SetSelected(j,true);//select the pertinent procedure
						break;
					}
					if(table.Rows[j]["AdjNum"].ToString()==table.Rows[gridAccount.SelectedIndices[i]]["adjustsOnObj"].ToString()) {
						gridAccount.SetSelected(j,true);//select the pertinent adjustment
						break;
					}
				}
			}
			for(int i=0;i<gridAccount.SelectedIndices.Count();i++) {
				DataRow dataRow=table.Rows[gridAccount.SelectedIndices[i]];
				if(dataRow["ClaimNum"].ToString()!="0") {//claims and claimpayments
					//Since we removed all selected items above, we need to reselect the claim the user just clicked on at the very least.
					//The "procsOnObj" column is going to be a comma delimited list of ProcNums associated to the corresponding claim.
					List<string> listProcsOnClaim=dataRow["procsOnObj"].ToString().Split(',').ToList();
					//Loop through the entire table and select any rows that are related to this claim (payments) while keeping track of their related ProcNums.
					for(int j=0;j<table.Rows.Count;j++) {//loop through all rows
						if(j>=gridAccount.ListGridRows.Count) {
							break;
						}
						if(table.Rows[j]["ClaimNum"].ToString()==dataRow["ClaimNum"].ToString()) {
							gridAccount.SetSelected(j,true);//for the claim payments
							listProcsOnClaim.AddRange(table.Rows[j]["procsOnObj"].ToString().Split(','));
						}
					}
					//Other software companies allow claims to be created with no procedures attached.
					//This would cause "procsOnObj" to contain a ProcNum of '0' which the following loop would then select seemingly random rows (any w/ ProcNum=0)
					//Therefore, we need to specifically remove any entries of '0' from our procsOnClaim list before looping through it.
					listProcsOnClaim.RemoveAll(x => x=="0");
					//Loop through the table again in order to select any related procedures.
					for(int j=0;j<table.Rows.Count;j++) {
						if(j>=gridAccount.ListGridRows.Count) {
							break;
						}
						if(listProcsOnClaim.Contains(table.Rows[j]["ProcNum"].ToString())) {
							gridAccount.SetSelected(j,true);
						}
					}
				}
				else if(dataRow["PayNum"].ToString()!="0") {
					List<string> listProcsOnPayment=dataRow["procsOnObj"].ToString().Split(',').ToList();
					List<string> listPaymentsOnObj=dataRow["paymentsOnObj"].ToString().Split(',').ToList();
					List<string> listAdjustsOnPayment=dataRow["adjustsOnObj"].ToString().Split(',').ToList();
					for(int j = 0;j<table.Rows.Count;j++) {//loop through all rows
						if(j>=gridAccount.ListGridRows.Count) {
							break;
						}
						if(table.Rows[j]["PayNum"].ToString()==dataRow["PayNum"].ToString()) {
							gridAccount.SetSelected(j,true);//for other splits in family view
							listProcsOnPayment.AddRange(table.Rows[j]["procsOnObj"].ToString().Split(','));
							listPaymentsOnObj.AddRange(table.Rows[j]["paymentsOnObj"].ToString().Split(','));
							listAdjustsOnPayment.AddRange(table.Rows[j]["adjustsOnObj"].ToString().Split(','));
						}
					}
					for(int j=0;j<table.Rows.Count;j++){
						if(j>=gridAccount.ListGridRows.Count) {
							break;
						}
						if(listProcsOnPayment.Contains(table.Rows[j]["ProcNum"].ToString())) {
							gridAccount.SetSelected(j,true);
						}
						if(listPaymentsOnObj.Contains(table.Rows[j]["PayNum"].ToString())) {
							gridAccount.SetSelected(j,true);
						}
						if(listAdjustsOnPayment.Contains(table.Rows[j]["Adjnum"].ToString())) {
							gridAccount.SetSelected(j,true);
						}
					}
				}
				else if(gridAccount.SelectedIndices.Contains(e.Row) && dataRow["AdjNum"].ToString()!="0" && dataRow["procsOnObj"].ToString()!="0") {
					for(int j=0;j<table.Rows.Count;j++) {
						if(j>=gridAccount.ListGridRows.Count) {
							break;
						}
						if(table.Rows[j]["ProcNum"].ToString()==dataRow["procsOnObj"].ToString()) {
							gridAccount.SetSelected(j,true);
							break;
						}
					}
				}
				else if(dataRow["ProcNumLab"].ToString()!="0" && dataRow["ProcNumLab"].ToString()!="") {//Canadian Lab procedure, select parents and other associated labs too.
					for(int j=0;j<table.Rows.Count;j++) {
						if(j>=gridAccount.ListGridRows.Count) {
							break;
						}
						if(table.Rows[j]["ProcNum"].ToString()==dataRow["ProcNumLab"].ToString()) {
							gridAccount.SetSelected(j,true);
							continue;
						}
						if(table.Rows[j]["ProcNumLab"].ToString()==dataRow["ProcNumLab"].ToString()) {
							gridAccount.SetSelected(j,true);
							continue;
						}
					}
				}
				else if(dataRow["ProcNum"].ToString()!="0") {//Not a Canadian lab and is a procedure.
					for(int j=0;j<table.Rows.Count;j++) {
						if(j>=gridAccount.ListGridRows.Count) {
							break;
						}
						if(table.Rows[j]["ProcNumLab"].ToString()==dataRow["ProcNum"].ToString()) {
							gridAccount.SetSelected(j,true);
							continue;
						}
					}
				}
			}
		}

		private void gridAccount_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			_scrollValueWhenDoubleClick=gridAccount.ScrollValue;
			DataTable table=_dataSetMain.Tables["account"];
			if(table.Rows[e.Row]["ProcNum"].ToString()!="0"){
				Procedure procedure=Procedures.GetOneProc(PIn.Long(table.Rows[e.Row]["ProcNum"].ToString()),true);
				Patient patient=_family.GetPatient(procedure.PatNum);
				using FormProcEdit formProcEdit=new FormProcEdit(procedure,patient,_family);
				formProcEdit.ListClaimProcHists=_loadData.HistList;
				formProcEdit.ListClaimProcHistsLoop=new List<ClaimProcHist>();
				formProcEdit.ShowDialog();
			}
			else if(table.Rows[e.Row]["AdjNum"].ToString()!="0"){
				Adjustment adjustment=Adjustments.GetOne(PIn.Long(table.Rows[e.Row]["AdjNum"].ToString()));
				if(adjustment==null) {
					MsgBox.Show(this,"The adjustment has been deleted.");//Don't return. Fall through to the refresh. 
				}
				else { 
					using FormAdjust formAdjust=new FormAdjust(_patient,adjustment);
					formAdjust.ShowDialog();
				}
			}
			else if(table.Rows[e.Row]["PayNum"].ToString()!="0"){
				Payment payment=Payments.GetPayment(PIn.Long(table.Rows[e.Row]["PayNum"].ToString()));
				if(payment==null) {
					MessageBox.Show(Lans.g(this,"No payment exists.  Please run database maintenance method")+" "+nameof(DatabaseMaintenances.PaySplitWithInvalidPayNum));
					return;
				}
				using FormPayment formPayment=new FormPayment(_patient,_family,payment,false);
				formPayment.IsNew=false;
				formPayment.ShowDialog();
			}
			else if(table.Rows[e.Row]["ClaimNum"].ToString()!="0"){//claims and claimpayments
				if(!Security.IsAuthorized(EnumPermType.ClaimView)) {
					return;
				}
				Claim claim=Claims.GetClaim(PIn.Long(table.Rows[e.Row]["ClaimNum"].ToString()));
				if(claim==null) {
					MsgBox.Show(this,"The claim has been deleted.");
				}
				else {
					Patient patient=_family.GetPatient(claim.PatNum);
					using FormClaimEdit formClaimEdit=new FormClaimEdit(claim,patient,_family);
					formClaimEdit.IsNew=false;
					formClaimEdit.ShowDialog();
				}
			}
			else if(table.Rows[e.Row]["StatementNum"].ToString()!="0"){
				Statement statement=Statements.GetStatement(PIn.Long(table.Rows[e.Row]["StatementNum"].ToString()));
				if(statement==null) {
					MsgBox.Show(this,"The statement has been deleted");//Don't return. Fall through to the refresh. 
				}
				else { 
					using FormStatementOptions formStatementOptions=new FormStatementOptions();
					formStatementOptions.StatementCur=statement;
					formStatementOptions.ShowDialog();
				}
			}
			else if(table.Rows[e.Row]["PayPlanNum"].ToString()!="0"){
				PayPlan payplan=PayPlans.GetOne(PIn.Long(table.Rows[e.Row]["PayPlanNum"].ToString()));
				if(payplan==null) {
					MsgBox.Show(this,"This pay plan has been deleted by another user.");
					ModuleSelected(_patient.PatNum,IsFamilySelected());
					return;
				}
				if(payplan.IsDynamic) {
					using FormPayPlanDynamic formPayPlanDynamic=new FormPayPlanDynamic(payplan);
					formPayPlanDynamic.ShowDialog();
					if(formPayPlanDynamic.PatNumGoto!=0) {
						FormOpenDental.S_Contr_PatientSelected(Patients.GetPat(formPayPlanDynamic.PatNumGoto),false);
						ModuleSelected(formPayPlanDynamic.PatNumGoto,false);
						return;
					}
				}
				else {//static payplan
					using FormPayPlan formPayPlan=new FormPayPlan(payplan);
					formPayPlan.ShowDialog();
					if(formPayPlan.PatNumGoto!=0) {
						FormOpenDental.S_Contr_PatientSelected(Patients.GetPat(formPayPlan.PatNumGoto),false);
						ModuleSelected(formPayPlan.PatNumGoto,false);
						return;
					}
				}
			}
			ModuleSelected(_patient.PatNum,IsFamilySelected());
		}

		private void gridAcctPat_CellClick(object sender,ODGridClickEventArgs e) {			
			if(e.Row==gridAcctPat.ListGridRows.Count-1) {//last row
				FormOpenDental.S_Contr_PatientSelected(_family.ListPats[0],false);
				ModuleSelected(_family.ListPats[0].PatNum,true);
				return;
			}
			long patNum=(long)gridAcctPat.ListGridRows[e.Row].Tag;
			Patient patient=_family.ListPats.First(x => x.PatNum==patNum);
			if(patient==null) {
				return;
			}
			FormOpenDental.S_Contr_PatientSelected(patient,false);
			ModuleSelected(patNum);
		}

		private void gridAutoOrtho_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridAutoOrtho.ListGridRows[e.Row].Tag==null || gridAutoOrtho.ListGridRows[e.Row].Tag.GetType()!=typeof(AutoOrthoPat)) {
				return;
			}
			AutoOrthoPat autoOrthoPat=(AutoOrthoPat)gridAutoOrtho.ListGridRows[e.Row].Tag;
			if(autoOrthoPat.InsPlan_.OrthoType!=OrthoClaimType.InitialPlusPeriodic) {
				MsgBox.Show(this,"To view this setup window, the insurance plan must be set to have an Ortho Claim Type of Initial Plus Periodic.");
				return;
			}
			using FormOrthoPat formOrthoPat=new FormOrthoPat(autoOrthoPat.PatPlan_,autoOrthoPat.InsPlan_,autoOrthoPat.CarrierName,autoOrthoPat.SubID,autoOrthoPat.DefaultFee);
			formOrthoPat.ShowDialog();
			if(formOrthoPat.DialogResult==DialogResult.OK) {
				PatPlans.Update(autoOrthoPat.PatPlan_);
				FillAutoOrtho();
			}
		}

		private void gridComm_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			int row=(int)gridComm.ListGridRows[e.Row].Tag;
			if(_dataSetMain.Tables["Commlog"].Rows[row]["CommlogNum"].ToString()!="0") {
				Commlog commlog=Commlogs.GetOne(PIn.Long(_dataSetMain.Tables["Commlog"].Rows[row]["CommlogNum"].ToString()));
				if(commlog==null) {
					MsgBox.Show(this,"This commlog has been deleted by another user.");
					ModuleSelected(_patient.PatNum);
					return;
				}
				using FormCommItem formCommItem=new FormCommItem(commlog);
				if(formCommItem.ShowDialog()==DialogResult.OK) {
					ModuleSelected(_patient.PatNum);
				}
				return;
			}
			if(_dataSetMain.Tables["Commlog"].Rows[row]["WebChatSessionNum"].ToString()!="0") {
				long webChatSessionNum=PIn.Long(_dataSetMain.Tables["Commlog"].Rows[row]["WebChatSessionNum"].ToString());
				WebChatSession webChatSession=WebChatSessions.GetOne(webChatSessionNum);
				using FormWebChatSession formWebChatSession=new FormWebChatSession(webChatSession,() => {ModuleSelected(_patient.PatNum);});
				formWebChatSession.ShowDialog();
				return;
			}
			if(_dataSetMain.Tables["Commlog"].Rows[row]["EmailMessageNum"].ToString()!="0") {
				EmailMessage emailMessage=
					EmailMessages.GetOne(PIn.Long(_dataSetMain.Tables["Commlog"].Rows[row]["EmailMessageNum"].ToString()));
				if(EmailMessages.IsSecureWebMail(emailMessage.SentOrReceived)) {
					//web mail uses special secure messaging portal
					using FormWebMailMessageEdit formWebMailMessageEdit=new FormWebMailMessageEdit(_patient.PatNum,emailMessage);
					if(formWebMailMessageEdit.ShowDialog()==DialogResult.OK) {
						ModuleSelected(_patient.PatNum);
					}
					return;
				}
				using FormEmailMessageEdit formEmailMessageEdit=new FormEmailMessageEdit(emailMessage);
				formEmailMessageEdit.ShowDialog();
				if(formEmailMessageEdit.DialogResult==DialogResult.OK) {
					ModuleSelected(_patient.PatNum);
				}
				return;
			}
			if(_dataSetMain.Tables["Commlog"].Rows[row]["FormPatNum"].ToString()!="0") {
				FormPat formPat=FormPats.GetOne(PIn.Long(_dataSetMain.Tables["Commlog"].Rows[row]["FormPatNum"].ToString()));
				using FormFormPatEdit formFormPatEdit=new FormFormPatEdit();
				formFormPatEdit.FormPatCur=formPat;
				formFormPatEdit.ShowDialog();
				if(formFormPatEdit.DialogResult==DialogResult.OK) {
					ModuleSelected(_patient.PatNum);
				}
				return;
			}
			if(_dataSetMain.Tables["Commlog"].Rows[row]["SheetNum"].ToString()!="0") {
				Sheet sheet=Sheets.GetSheet(PIn.Long(_dataSetMain.Tables["Commlog"].Rows[row]["SheetNum"].ToString()));
				SheetUtilL.ShowSheet(sheet,_patient,FormSheetFillEdit_FormClosing);
			}
		}

		private void GridOrthoCases_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormOrthoCase formOrthoCase=new FormOrthoCase(false,_patient,(OrthoCase)gridOrthoCases.ListGridRows[e.Row].Tag);
			formOrthoCase.ShowDialog();
			ModuleSelected(_patient.PatNum);
		}

		private void gridPatInfo_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(TerminalActives.PatIsInUse(_patient.PatNum)) {
				MsgBox.Show(this,"Patient is currently entering info at a reception terminal.  Please try again later.");
				return;
			}
			if(gridPatInfo.ListGridRows[e.Row].Tag is PatFieldDef) {//patfield for an existing PatFieldDef
				PatFieldDef patFieldDef=(PatFieldDef)gridPatInfo.ListGridRows[e.Row].Tag;
				PatField patField=PatFields.GetByName(patFieldDef.FieldName,_patFieldArray);
				PatFieldL.OpenPatField(patField,patFieldDef,_patient.PatNum);
				ModuleSelected(_patient.PatNum);
				return;
			}
			if(gridPatInfo.ListGridRows[e.Row].Tag is PatField) {//PatField for a PatFieldDef that no longer exists
				PatField patField=(PatField)gridPatInfo.ListGridRows[e.Row].Tag;
				using FormPatFieldEdit formPatFieldEdit=new FormPatFieldEdit(patField);
				formPatFieldEdit.ShowDialog();
				ModuleSelected(_patient.PatNum);
				return;
			}
			if(!Security.IsAuthorized(EnumPermType.PatientEdit)) {
				return;
			}
			using FormPatientEdit formPatientEdit=new FormPatientEdit(_patient,_family);
			formPatientEdit.IsNew=false;
			formPatientEdit.ShowDialog();
			if(formPatientEdit.DialogResult==DialogResult.OK) {
				FormOpenDental.S_Contr_PatientSelected(_patient,false);
			}
			ModuleSelected(_patient.PatNum);
		}

		private void gridPayPlan_CellClick(object sender,ODGridClickEventArgs e) {
			DataRow dataRow=((DataRow)(gridPayPlan.ListGridRows[e.Row].Tag));
			if(dataRow["PayPlanNum"].ToString()=="0") {
				return;
			}
			PayPlan payPlan=PayPlans.GetOne(PIn.Long(dataRow["PayPlanNum"].ToString()));
			if(payPlan==null) {
				MsgBox.Show(this,"This pay plan has been deleted by another user.");
				return;
			}
			if(gridPayPlan.Columns[e.Col].Heading!="eClipboard") {
				return;
			}
			if(payPlan.MobileAppDeviceNum<=0) {
				return;
			}
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to recall this payment plan from the mobile device?")) {
				PushNotificationUtils.CI_RemovePaymentPlan(payPlan.MobileAppDeviceNum,payPlan);
				//Signalods.SetInvalid(InvalidType.AccModule,KeyType.PatNum,payPlan.PatNum);
				return;
			}
		}

		private void gridPayPlan_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			DataRow dataRow=((DataRow)(gridPayPlan.ListGridRows[e.Row].Tag));
			if(dataRow["PayPlanNum"].ToString()=="0") {//Installment Plan
				using FormInstallmentPlanEdit formInstallmentPlanEdit=new FormInstallmentPlanEdit();
				formInstallmentPlanEdit.InstallmentPlanCur=InstallmentPlans.GetOne(PIn.Long(dataRow["InstallmentPlanNum"].ToString()));
				formInstallmentPlanEdit.IsNew=false;
				formInstallmentPlanEdit.ShowDialog();
				ModuleSelected(_patient.PatNum);
				return;
			}
			//Payment plan
			PayPlan payPlan=PayPlans.GetOne(PIn.Long(dataRow["PayPlanNum"].ToString()));
			if(payPlan==null) {
				MsgBox.Show(this,"This pay plan has been deleted by another user.");
				ModuleSelected(_patient.PatNum,IsFamilySelected());
				return;
			}
			if(payPlan.IsDynamic) {
				using FormPayPlanDynamic formPayPlanDynamic=new FormPayPlanDynamic(payPlan);
				formPayPlanDynamic.ShowDialog();
				if(formPayPlanDynamic.PatNumGoto!=0) {
					FormOpenDental.S_Contr_PatientSelected(Patients.GetPat(formPayPlanDynamic.PatNumGoto),false);
					ModuleSelected(formPayPlanDynamic.PatNumGoto,false);
					return;
				}
				ModuleSelected(_patient.PatNum,IsFamilySelected());
				return;
			}
			using FormPayPlan formPayPlan=new FormPayPlan(payPlan);
			formPayPlan.ShowDialog();
			if(formPayPlan.PatNumGoto!=0) {
				FormOpenDental.S_Contr_PatientSelected(Patients.GetPat(formPayPlan.PatNumGoto),false);
				ModuleSelected(formPayPlan.PatNumGoto,false);
				return;
			}
			ModuleSelected(_patient.PatNum,IsFamilySelected());
		}

		private void gridRepeat_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormRepeatChargeEdit formRepeatChargeEdit=new FormRepeatChargeEdit(_repeatChargeArray[e.Row]);
			formRepeatChargeEdit.ShowDialog();
			ModuleSelected(_patient.PatNum);
		}

		private void GridTpSplits_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			_scrollValueWhenDoubleClickTpUnearned=gridTpSplits.ScrollValue;
			PaySplit paySplit=(PaySplit)gridTpSplits.ListGridRows[e.Row].Tag;
			if(paySplit==null) {
				return;
			}
			Payment payment=Payments.GetPayment(paySplit.PayNum);
			if(payment==null) {
				MsgBox.Show(this,"Payment does not exist.");
				return;
			}
			using FormPayment formPayment=new FormPayment(_patient,_family,payment,false);
			formPayment.IsNew=false;
			formPayment.ShowDialog();
			ModuleSelected(_patient.PatNum,IsFamilySelected());
		}
		#endregion Methods - Event Handlers Grids

		#region Methods - Event Handlers Labels
		private void labelInsRem_Click(object sender,EventArgs e) {
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				//Since the bonus information in FormInsRemain is currently only helpful in Canada,
				//we have decided not to show the form for other countries at this time.
				return;
			}
			if(_patient==null) {
				return;
			}
			using FormInsRemain formInsRemain=new FormInsRemain(_patient.PatNum);
			formInsRemain.ShowDialog();
		}

		private void labelInsRem_MouseEnter(object sender,EventArgs e) {
			groupBoxFamilyIns.Visible=true;
			groupBoxIndIns.Visible=true;
		}

		private void labelInsRem_MouseLeave(object sender,EventArgs e) {
			groupBoxFamilyIns.Visible=false;
			groupBoxIndIns.Visible=false;
		}

		private void labelDisRem_MouseEnter(object sender,EventArgs e) {
			groupBoxIndDis.Visible=true;
			groupBoxIndDis.Enabled=true;
		}

		private void labelDisRem_MouseLeave(object sender,EventArgs e) {
			groupBoxIndDis.Visible=false;
			groupBoxIndDis.Enabled=false;
		}

		private void labelUnearnedAmt_MouseEnter(object sender,EventArgs e) {
			if(Math.Abs(PIn.Decimal(labelUnearnedAmt.Text))>0) {
				gridUnearnedBreakdown.Visible=true;
				gridUnearnedBreakdown.Enabled=true;
			}
		}

		private void labelUnearnedAmt_MouseLeave(object sender,EventArgs e) {
			gridUnearnedBreakdown.Visible=false;
			gridUnearnedBreakdown.Enabled=false;
		}
		#endregion Methods - Event Handlers Labels

		#region Methods - Event Handlers Menus
		private void menuInsMedical_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.ClaimView)) {
				return;
			}
			if(!ClaimL.CheckClearinghouseDefaults()) {
				return;
			}
			AccountModules.CreateClaimData claimData=AccountModules.GetCreateClaimData(_patient,_family);
			long medSubNum=0;
			for(int i=0;i<claimData.ListPatPlans.Count;i++){
				InsSub insSub=InsSubs.GetSub(claimData.ListPatPlans[i].InsSubNum,claimData.ListInsSubs);
				if(InsPlans.GetPlan(insSub.PlanNum,claimData.ListInsPlans).IsMedical){
					medSubNum=insSub.InsSubNum;
					break;
				}
			}
			if(medSubNum==0){
				MsgBox.Show(this,"Patient does not have medical insurance.");
				return;
			}
			DataTable table=_dataSetMain.Tables["account"];
			Procedure procedure;
			if(gridAccount.SelectedIndices.Length==0){
				//autoselect procedures
				for(int i=0;i<table.Rows.Count;i++){//loop through every line showing on screen
					if(table.Rows[i]["ProcNum"].ToString()=="0"){
						continue;//ignore non-procedures
					}
					procedure=Procedures.GetProcFromList(claimData.ListProcs,PIn.Long(table.Rows[i]["ProcNum"].ToString()));
					if(procedure.ProcFee==0){
						continue;//ignore zero fee procedures, but user can explicitly select them
					}
					if(procedure.MedicalCode==""){
						continue;//ignore non-medical procedures
					}
					if(Procedures.NeedsSent(procedure.ProcNum,medSubNum,claimData.ListClaimProcs) && i<gridAccount.ListGridRows.Count) {
						gridAccount.SetSelected(i,true);
					}
				}
				if(gridAccount.SelectedIndices.Length==0){//if still none selected
					MsgBox.Show(this,"Please select procedures first.");
					return;
				}
			}
			bool areAllProcedures=true;
			for(int i=0;i<gridAccount.SelectedIndices.Length;i++){
				if(table.Rows[gridAccount.SelectedIndices[i]]["ProcNum"].ToString()=="0"){
					areAllProcedures=false;
				}
			}
			if(!areAllProcedures){
				MsgBox.Show(this,"You can only select procedures.");
				return;
			}
			//Medical claims are slightly different so we'll just manually create the CreateClaimDataWrapper needed for creating the claim.
			CreateClaimDataWrapper createClaimDataWrapper=new CreateClaimDataWrapper() {
				Patient_=_patient,
				Family_=_family,
				ListCreateClaimItems=GetCreateClaimItemsFromUI(),
				CreateClaimData_=claimData,
			};
			//Block users for creating claims where the procedure can be associated with duplicate claim procs
			if(ClaimL.WarnUsersForDuplicateClaimProcs(createClaimDataWrapper))  {
				return;
			}
			Claim claim=new Claim();
			claim.ClaimStatus="W";
			claim.DateSent=DateTime.Today;
			claim.DateSentOrig=DateTime.MinValue;
			//Set ClaimCur to CreateClaim because the reference to ClaimCur gets broken when inserting.
			claim=ClaimL.CreateClaim(claim,"Med",true,createClaimDataWrapper);
			if(claim.ClaimNum==0){
				ModuleSelected(_patient.PatNum);
				return;
			}
			//still have not saved some changes to the claim at this point
			using FormClaimEdit formClaimEdit=new FormClaimEdit(claim,_patient,_family);
			formClaimEdit.IsNew=true;//this causes it to delete the claim if cancelling.
			//If there's unallocated amounts, we want to redistribute the money to other procedures.
			if(formClaimEdit.ShowDialog()==DialogResult.OK) {
				ClaimL.AllocateUnearnedPayment(_patient,_family,PIn.Double(labelUnearnedAmt.Text),claim);
			}
			ModuleSelected(_patient.PatNum);
		}

		private void menuInsOther_Click(object sender,EventArgs e) {
			CreateClaimDataWrapper createClaimDataWrapper=ClaimL.GetCreateClaimDataWrapper(_patient,_family,GetCreateClaimItemsFromUI(),true,true);
			if(createClaimDataWrapper.HasError) {
				return;
			}
			Claim claim=new Claim();
			claim.ClaimStatus="U";
			//Set Claim to CreateClaim because the reference to Claim gets broken when inserting.
			claim=ClaimL.CreateClaim(claim,"Other",true,createClaimDataWrapper);
			if(claim.ClaimNum==0) {
				ModuleSelected(_patient.PatNum);
				return;
			}
			//still have not saved some changes to the claim at this point
			using FormClaimEdit formClaimEdit=new FormClaimEdit(claim,_patient,_family);
			formClaimEdit.IsNew=true;//this causes it to delete the claim if cancelling.
			if(formClaimEdit.ShowDialog()==DialogResult.OK) {
				ClaimL.AllocateUnearnedPayment(_patient,_family,PIn.Double(labelUnearnedAmt.Text),claim);
			}
			ModuleSelected(_patient.PatNum);
		}

		private void menuInsPri_Click(object sender,EventArgs e) {
			CreateClaimDataWrapper createClaimDataWrapper=ClaimL.GetCreateClaimDataWrapper(_patient,_family,GetCreateClaimItemsFromUI(),true,true);
			if(createClaimDataWrapper.HasError) {
				return;
			}
			if(PatPlans.GetOrdinal(PriSecMed.Primary,createClaimDataWrapper.CreateClaimData_.ListPatPlans,createClaimDataWrapper.CreateClaimData_.ListInsPlans
				,createClaimDataWrapper.CreateClaimData_.ListInsSubs)==0)
			{
				MsgBox.Show(this,"The patient does not have any dental insurance plans.");
				return;
			}
			Claim claim=new Claim();
			claim.ClaimStatus="W";
			claim.DateSent=DateTime.Today;
			claim.DateSentOrig=DateTime.MinValue;
			//Set Claim to CreateClaim because the reference to Claim gets broken when inserting.
			claim=ClaimL.CreateClaim(claim,"P",true,createClaimDataWrapper);
			if(claim.ClaimNum==0){
				ModuleSelected(_patient.PatNum);
				return;
			}
			//still have not saved some changes to the claim at this point
			using FormClaimEdit formClaimEdit=new FormClaimEdit(claim,_patient,_family);
			formClaimEdit.IsNew=true;//this causes it to delete the claim if cancelling.
			//If there's unallocated amounts, we want to redistribute the money to other procedures.
			if(formClaimEdit.ShowDialog()==DialogResult.OK) {
				ClaimL.AllocateUnearnedPayment(_patient,_family,PIn.Double(labelUnearnedAmt.Text),claim);
			}
			ModuleSelected(_patient.PatNum);
		}

		private void menuInsSec_Click(object sender,EventArgs e) {
			CreateClaimDataWrapper createClaimDataWrapper=ClaimL.GetCreateClaimDataWrapper(_patient,_family,GetCreateClaimItemsFromUI(),true,true);
			if(createClaimDataWrapper.HasError) {
				return;
			}
			if(createClaimDataWrapper.CreateClaimData_.ListPatPlans.Count<2) {
				MessageBox.Show(Lan.g(this,"Patient does not have secondary insurance."));
				return;
			}
			if(PatPlans.GetOrdinal(PriSecMed.Secondary,createClaimDataWrapper.CreateClaimData_.ListPatPlans,createClaimDataWrapper.CreateClaimData_.ListInsPlans
				,createClaimDataWrapper.CreateClaimData_.ListInsSubs)==0)
			{
				MsgBox.Show(this,"Patient does not have secondary insurance.");
				return;
			}
			Claim claim=new Claim();
			claim.ClaimStatus="W";
			claim.DateSent=DateTime.Today;
			claim.DateSentOrig=DateTime.MinValue;
			//Set Claim to CreateClaim because the reference to Claim gets broken when inserting.
			claim=ClaimL.CreateClaim(claim,"S",true,createClaimDataWrapper);
			if(claim.ClaimNum==0) {
				ModuleSelected(_patient.PatNum);
				return;
			}
			using FormClaimEdit formClaimEdit=new FormClaimEdit(claim,_patient,_family);
			formClaimEdit.IsNew=true;//this causes it to delete the claim if cancelling.
			//If there's unallocated amounts, we want to redistribute the money to other procedures.
			if(formClaimEdit.ShowDialog()==DialogResult.OK) {
				ClaimL.AllocateUnearnedPayment(_patient,_family,PIn.Double(labelUnearnedAmt.Text),claim);
			}
			ModuleSelected(_patient.PatNum);
		}

		private void menuPrepayment_Click(object sender,EventArgs e) {
			if(_patient==null) {
				return;
			}
			using FormPrepaymentTool formPrepaymentTool=new FormPrepaymentTool(_patient);
			if(formPrepaymentTool.ShowDialog()==DialogResult.OK) {
				Family family=Patients.GetFamily(_patient.PatNum);
				using FormPayment formPayment=new FormPayment(_patient,family,formPrepaymentTool.PaymentReturn,false);
				formPayment.IsNew=true;
				Payments.Insert(formPrepaymentTool.PaymentReturn);
				RefreshModuleData(_patient.PatNum,false);
				RefreshModuleScreen(false);
				formPayment.ShowDialog();
			}
			RefreshModuleData(_patient.PatNum,false);
			RefreshModuleScreen(false);
		}
		#endregion Methods - Event Handlers Menus

		#region Methods - Event Handlers MenuItem
		private void menuItemAddAdj_Click(object sender,EventArgs e) {
			AddAdjustmentToSelectedProcsHelper();
		}

		private void menuItemAddRefund_Click(object sender,EventArgs e) {
			MenuItem menuItem=(MenuItem)sender;
			DataTable table=_dataSetMain.Tables["account"];
			List<int> listRowsSelected=gridAccount.SelectedIndices.ToList();
			Payment paymentExisting=null;
			//Figure out what payment was right clicked on.
			for(int i=0;i<listRowsSelected.Count;i++) {
				if(table.Rows[listRowsSelected[i]]["PayNum"].ToString()!="0") {
					long payNum=PIn.Long(table.Rows[listRowsSelected[i]]["PayNum"].ToString());
					paymentExisting=Payments.GetPayment(payNum);
					break;
				}
			}
			if(paymentExisting==null) {
				MsgBox.Show(this,"Payment is invalid.");
				return;
			}
			List<PaySplit> listPaySplitsExisting=PaySplits.GetForPayment(paymentExisting.PayNum);
			//Negative payment
			if(listPaySplitsExisting.Any(x => x.SplitAmt<=0)) {
				MsgBox.Show(this,"Cannot refund payments that have negative splits.");
				return;
			}
			//Attached to payplan
			if(listPaySplitsExisting.Any(x => x.PayPlanNum>0)) {
				MsgBox.Show(this,"Cannot refund payments attached to payment plans.");
				return;
			}
			//Create negative adjustments to offset the production (typically procedures) from the existing payment if the user clicked on 'work not performed'.
			List<Adjustment> listAdjustmentsAdded=new List<Adjustment>();
			if(menuItem==menuItemAddRefundWorkNotPerformed) {
				if(PrefC.GetLong(PrefName.RefundAdjustmentType)==0) {
					MsgBox.Show(this,"Refund adjustment type has not been set. Please go to Setup | Account to fix this.");
					return;
				}
				listAdjustmentsAdded=Adjustments.CreateNegativeAdjustmentsForRefund(paymentExisting);
				//Show FormAdjMulti with with procedures to refund, and generated adjustments prefilled in.
				if(listAdjustmentsAdded.Count>0) {
					using FormAdjMulti formAdjMulti=new FormAdjMulti(_patient,listAdjustments:listAdjustmentsAdded);
					if(formAdjMulti.ShowDialog()!=DialogResult.OK) {
						return;
					}
				}
			}
			//Make a offsetting payment that negates each of the paysplits in the existing payment.
			Payment paymentRefund=Payments.MakeNegativePaymentsRefund(paymentExisting);
			//Show formPayment with the new generated paySplits
			using FormPayment formPayment=new FormPayment(_patient,_family,paymentRefund,false);
			formPayment.IsNew=true;
			if(formPayment.ShowDialog()!=DialogResult.OK){	//if the user clicks cancel or x, undo any changes to the database.
				ODException.SwallowAnyException(() => Payments.Delete(paymentRefund));
				for(int i = 0;i<listAdjustmentsAdded.Count;i++) {
					Adjustments.Delete(listAdjustmentsAdded[i]);
				}
			}
			ModuleSelected(_patient.PatNum, IsFamilySelected());
		}

		private void menuItemAddMultAdj_Click(object sender,EventArgs e) {
			AddAdjustmentToSelectedProcsHelper(true);
		}

		private void menuItemAllocateUnearned_Click(object sender,EventArgs e) {
			toolBarButPay_Click(0,isPrePay:true,isIncomeTransfer:true);
		}

		private void menuItemDeletePayPlanCharge_Click(object sender,EventArgs e) {
			DataTable table=_dataSetMain.Tables["account"];
			List<long> listSelectedPayPlanChargeNums=new List<long>();
			List<int> listIndices=gridAccount.SelectedIndices.ToList();
			for(int i=0;i<listIndices.Count();i++) {
				long payPlanChargeNum=PIn.Long(table.Rows[listIndices[i]]["PayPlanChargeNum"].ToString());
				if(payPlanChargeNum==0) {
					continue;
				}
				listSelectedPayPlanChargeNums.Add(payPlanChargeNum);
			}
			List<PayPlanCharge> listPayPlanCharges=PayPlanCharges.GetMany(listSelectedPayPlanChargeNums);
			List<PayPlanCharge> listPayPlanChargesNotDeleted=PayPlanCharges.DeleteDebitsWithoutPayments(listPayPlanCharges);
			if(listPayPlanChargesNotDeleted.Count > 0) {
				string msgString="Cannot delete";
				if(listPayPlanChargesNotDeleted.Exists(x=>x.Note.ToLower().Contains("down payment"))){
					msgString+=" down payment charges, or";
				}
				msgString+=" charges with payments attached.";
				MsgBox.Show(Lans.g(this,msgString));
			}
			if(listPayPlanCharges.Count!=listPayPlanChargesNotDeleted.Count) {
				//at least one payplan charge was deleted.
				SecurityLogs.MakeLogEntry(EnumPermType.PayPlanChargeEdit,_patient.PatNum,"Deleted.");
			}
			ModuleSelected(_patient.PatNum);
		}

		private void MenuItemDynamicPayPlan_Click(object sender,EventArgs e) {
			PayPlanHelper(PayPlanModes.Dynamic);//when payment plan is dynamic, insurance vs. pat does not matter.
		}

		private void menuItemIncomeTransfer_Click(object sender,EventArgs e) {
			using FormIncomeTransferManage formIncomeTransferManage=new FormIncomeTransferManage(_family,_patient);
			formIncomeTransferManage.ShowDialog();
			ModuleSelected(_patient.PatNum);
		}

		private void menuItemInvoice_Click(object sender,EventArgs e) {
			DataTable table=_dataSetMain.Tables["account"];
			List<DataRow> listDataRowsSuperFam=new List<DataRow>();
			Patient guarantor=Patients.GetPat(_patient.Guarantor);
			Patient superHead=Patients.GetPat(_patient.SuperFamily);
			if(gridAccount.SelectedIndices.Length==0 
				&& (superHead==null || !PrefC.GetBool(PrefName.ShowFeatureSuperfamilies) || !guarantor.HasSuperBilling || !superHead.HasSuperBilling)) 
			{
				//autoselect procedures, adjustments, and some pay plan charges
				for(int i=0;i<table.Rows.Count;i++) {//loop through every line showing on screen
					if(table.Rows[i]["ProcNum"].ToString()=="0" 
						&& table.Rows[i]["AdjNum"].ToString()=="0"
						&& table.Rows[i]["PayPlanChargeNum"].ToString()=="0") 
					{
						continue;//ignore items that aren't procs, adjustments, or pay plan charges
					}
					if(PIn.Date(table.Rows[i]["date"].ToString())!=DateTime.Today) {
						continue;
					}
					if(table.Rows[i]["ProcNum"].ToString()!="0") {//if selected item is a procedure
						Procedure procedure=Procedures.GetOneProc(PIn.Long(table.Rows[i]["ProcNum"].ToString()),false);
						if(procedure.StatementNum!=0) {//already attached so don't autoselect
							continue;
						}
						if(procedure.PatNum!=_patient.PatNum) {
							continue;
						}
					}
					else if(table.Rows[i]["PayPlanChargeNum"].ToString()!="0") {//selected item is pay plan charge
						PayPlanCharge payPlanCharge=PayPlanCharges.GetOne(PIn.Long(table.Rows[i]["PayPlanChargeNum"].ToString()));
						if(payPlanCharge.PatNum!=_patient.PatNum){
							continue;
						}
						if(payPlanCharge.ChargeType!=PayPlanChargeType.Debit) {
							continue;
						}
						if(payPlanCharge.StatementNum!=0) {
							continue;
						}
					}
					else {//item must be adjustment
						Adjustment adjustment=Adjustments.GetOne(PIn.Long(table.Rows[i]["AdjNum"].ToString()));
						if(adjustment.StatementNum!=0) {//already attached so don't autoselect
							continue;
						}
						if(adjustment.PatNum!=_patient.PatNum) {
							continue;
						}
					}
					if(i < gridAccount.ListGridRows.Count) {
						gridAccount.SetSelected(i,true);
					}
				}
				if(gridAccount.SelectedIndices.Length==0) {//if still none selected
					MsgBox.Show(this,"Please select procedures, adjustments or payment plan charges first.");
					return;
				}
			}
			else if(gridAccount.SelectedIndices.Length==0 
				&& (PrefC.GetBool(PrefName.ShowFeatureSuperfamilies) && guarantor.HasSuperBilling && superHead.HasSuperBilling)) 
			{
				//No selections and superbilling is enabled for this family.  Show a window to select and attach procs to this statement for the superfamily.
				using FormInvoiceItemSelect formInvoiceItemSelect=new FormInvoiceItemSelect(_patient.SuperFamily);
				if(formInvoiceItemSelect.ShowDialog()==DialogResult.Cancel) {
					return;
				}
				listDataRowsSuperFam=formInvoiceItemSelect.ListDataRowsSelected;
			}
			for(int i=0;i<gridAccount.SelectedIndices.Length;i++) {
				DataRow dataRow=table.Rows[gridAccount.SelectedIndices[i]];
				if(dataRow["ProcNum"].ToString()=="0" 
					&& dataRow["AdjNum"].ToString()=="0"
					&& dataRow["PayPlanChargeNum"].ToString()=="0") //the selected item is neither a procedure nor an adjustment
				{
					MsgBox.Show(this,"You can only select procedures, payment plan charges or adjustments.");
					gridAccount.SetAll(false);
					return;
				}
				if(dataRow["ProcNum"].ToString()!="0") {//the selected item is a proc
					Procedure procedure=Procedures.GetOneProc(PIn.Long(dataRow["ProcNum"].ToString()),false);
					if(procedure.PatNum!=_patient.PatNum) {
						MsgBox.Show(this,"You can only select procedures, payment plan charges or adjustments for the current patient on an invoice.");
						gridAccount.SetAll(false);
						return;
					}
					if(procedure.StatementNum!=0) {
						MsgBox.Show(this,"Selected procedure(s) are already attached to an invoice.");
						gridAccount.SetAll(false);
						return;
					}
				}
				else if(dataRow["PayPlanChargeNum"].ToString()!="0") {
					PayPlanCharge payPlanCharge=PayPlanCharges.GetOne(PIn.Long(dataRow["PayPlanChargeNum"].ToString()));
					if(payPlanCharge.PatNum!=_patient.PatNum){
						MsgBox.Show(this,"You can only select procedures, payment plan charges or adjustments for a single patient on an invoice.");
						gridAccount.SetAll(false);
						return;
					}
					if(payPlanCharge.ChargeType!=PayPlanChargeType.Debit) {
						MsgBox.Show(this,"You can only select payment plans charges that are debits.");
						gridAccount.SetAll(false);
						return;
					}
					if(payPlanCharge.StatementNum!=0) {
						MsgBox.Show(this,"Selected payment plan charges(s) are already attached to an invoice.");
						gridAccount.SetAll(false);
						return;
					}
				}
				else{//the selected item must be an adjustment
					Adjustment adjustment=Adjustments.GetOne(PIn.Long(dataRow["AdjNum"].ToString()));
					if(adjustment.AdjDate.Date > DateTime.Today.Date && !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
						MsgBox.Show(this,"Adjustments cannot be made for future dates");
						return;
					}
					if(adjustment.PatNum!=_patient.PatNum) {
						MsgBox.Show(this,"You can only select procedures, payment plan charges or adjustments for a single patient on an invoice.");
						gridAccount.SetAll(false);
						return;
					}
					if(adjustment.StatementNum!=0) {
						MsgBox.Show(this,"Selected adjustment(s) are already attached to an invoice.");
						gridAccount.SetAll(false);
						return;
					}
				}
			}
			//At this point, all selected items are procedures or adjustments, and are not already attached, and are for a single patient.
			Statement statement=new Statement();
			statement.PatNum=_patient.PatNum;
			statement.DateSent=DateTime.Today;
			statement.IsSent=false;
			statement.Mode_=StatementMode.InPerson;
			statement.HidePayment=true;
			statement.SinglePatient=true;
			statement.Intermingled=false;
			statement.IsReceipt=false;
			statement.IsInvoice=true;
			statement.StatementType=StmtType.NotSet;
			statement.DateRangeFrom=DateTime.MinValue;
			statement.DateRangeTo=DateTime.Today;
			statement.Note=PrefC.GetString(PrefName.BillingDefaultsInvoiceNote);
			statement.NoteBold="";
			statement.IsBalValid=true;
			statement.BalTotal=guarantor.BalTotal;
			statement.InsEst=guarantor.InsEst;
			if(listDataRowsSuperFam.Count > 0) {
				statement.SuperFamily=_patient.SuperFamily;
			}
			Statements.Insert(statement);
			statement.IsNew=true;
			List<Procedure> listProceduresForPat=Procedures.Refresh(_patient.PatNum);
			for(int i=0;i<gridAccount.SelectedIndices.Length;i++) {
				DataRow dataRow=table.Rows[gridAccount.SelectedIndices[i]];
				if(dataRow["ProcNum"].ToString()!="0") {//if selected item is a procedure
					Procedure procedure=Procedures.GetProcFromList(listProceduresForPat,PIn.Long(dataRow["ProcNum"].ToString()));
					Procedure procedureOld=procedure.Copy();
					procedure.StatementNum=statement.StatementNum;
					if(procedure.ProcStatus==ProcStat.C && procedure.ProcDate.Date > DateTime.Today.Date && !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
						MsgBox.Show(this,"Completed procedures cannot be set for future dates.");
						return;
					}
					Procedures.Update(procedure,procedureOld);
				}
				else if(dataRow["PayPlanChargeNum"].ToString()!="0") {
					PayPlanCharge payPlanCharge=PayPlanCharges.GetOne(PIn.Long(dataRow["PayPlanChargeNum"].ToString()));
					payPlanCharge.StatementNum=statement.StatementNum;
					PayPlanCharges.Update(payPlanCharge);
				}
				else {//selected item must be adjustment
					Adjustment adjustment=Adjustments.GetOne(PIn.Long(dataRow["AdjNum"].ToString()));
					adjustment.StatementNum=statement.StatementNum;
					Adjustments.Update(adjustment);
				}
			}
			for(int i=0;i<listDataRowsSuperFam.Count();i++) {
				if(listDataRowsSuperFam[i]["ChargeType"].ToString()!="") {//payplan
					PayPlanCharge payPlanCharge = PayPlanCharges.GetOne(PIn.Long(listDataRowsSuperFam[i]["PriKey"].ToString()));
					payPlanCharge.StatementNum=statement.StatementNum;
					PayPlanCharges.Update(payPlanCharge);
				}
				else if(listDataRowsSuperFam[i]["AdjType"].ToString()!=""){//adjustment
					Adjustment adjustment=Adjustments.GetOne(PIn.Long(listDataRowsSuperFam[i]["PriKey"].ToString()));
					adjustment.StatementNum=statement.StatementNum;
					Adjustments.Update(adjustment);
				}
				else {
					Procedure procedureNew = Procedures.GetOneProc(PIn.Long(listDataRowsSuperFam[i]["PriKey"].ToString()),false);
					Procedure procedureOld=procedureNew.Copy();
					procedureNew.StatementNum=statement.StatementNum;
					if(procedureNew.ProcStatus==ProcStat.C && procedureNew.ProcDate.Date>DateTime.Today.Date && !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
						MsgBox.Show(this,"Procedures cannot be set for future dates.");
						return;
					}
					Procedures.Update(procedureNew,procedureOld);
				}
			}
			//All printing and emailing will be done from within the form:
			using FormStatementOptions formStatementOptions=new FormStatementOptions();
			formStatementOptions.StatementCur=statement;
			formStatementOptions.ShowDialog();
			if(formStatementOptions.DialogResult!=DialogResult.OK) {
				Statements.DeleteStatements(new List<Statement> { statement });//detached from adjustments, procedurelogs, and paysplits as well
			}
			Signalods.SetInvalid(InvalidType.BillingList);
			ModuleSelected(_patient.PatNum);
		}

		private void menuItemInsPayPlan_Click(object sender,EventArgs e) {
			PayPlanHelper(PayPlanModes.Insurance);
		}

		private void menuItemLimited_Click(object sender,EventArgs e) {
			DataTable table=_dataSetMain.Tables["account"];
			DataRow dataRow;
			#region Autoselect Today's Procedures
			if(gridAccount.SelectedIndices.Length==0) {//autoselect procedures
				for(int i=0;i<table.Rows.Count;i++) {//loop through every line showing on screen
					dataRow=table.Rows[i];
					if(dataRow["ProcNum"].ToString()=="0" //ignore items that aren't procs
						|| PIn.Date(dataRow["date"].ToString())!=DateTime.Today //autoselecting todays procs only
						|| PIn.Long(dataRow["PatNum"].ToString())!=_patient.PatNum) //only procs for the current patient
					{
						continue;
					}
					if(i < gridAccount.ListGridRows.Count) {
						gridAccount.SetSelected(i,true);
					}
				}
				if(gridAccount.SelectedIndices.Length==0) {//if still none selected
					MsgBox.Show(this,"Please select procedures, adjustments, payments, claims, or pay plan charges first.");
					return;
				}
			}
			#endregion Autoselect Today's Procedures
			PayPlanVersions payPlanVersions=(PayPlanVersions)PrefC.GetInt(PrefName.PayPlansVersion);
			//guaranteed to have rows selected from here down, verify they are allowed transactions
			bool areStatementsSelected=gridAccount.SelectedIndices.Any(x => table.Rows[x]["StatementNum"].ToString()!="0");
			bool arePayPlanCreditsSelected=false;//Default for PayPlanVersions.NoCharges.
			if(payPlanVersions==PayPlanVersions.AgeCreditsAndDebits || payPlanVersions==PayPlanVersions.AgeCreditsOnly) {
				arePayPlanCreditsSelected=gridAccount.SelectedIndices.Any(x => table.Rows[x]["PayPlanChargeNum"].ToString()!="0" && table.Rows[x]["charges"].ToString()=="");
			}
			else if(payPlanVersions==PayPlanVersions.DoNotAge) {
				arePayPlanCreditsSelected=gridAccount.SelectedIndices.Any(x => table.Rows[x]["PayPlanNum"].ToString()!="0" && table.Rows[x]["charges"].ToString()=="");
			}
			if(areStatementsSelected || arePayPlanCreditsSelected) {
				MsgBox.Show(this,"You can only select procedures, adjustments, payments, claims, or pay plan charges.");
				gridAccount.SetAll(false);
				return;
			}
			//At this point, all selected items are procedures, adjustments, payments, claims, or pay plan charges.
			//get all ClaimNums from claimprocs for the selected procs
			List<long> listProcClaimNums=ClaimProcs.GetForProcs(gridAccount.SelectedIndices.Where(x => table.Rows[x]["ProcNum"].ToString()!="0")
				.Select(x => PIn.Long(table.Rows[x]["ProcNum"].ToString())).ToList()).FindAll(x => x.ClaimNum!=0).Select(x => x.ClaimNum).ToList();
			//get all ClaimNums for any selected claimpayments
			List<long> listPayClaimNums=gridAccount.SelectedIndices
				.Where(x => table.Rows[x]["ClaimNum"].ToString()!="0" && table.Rows[x]["ClaimPaymentNum"].ToString()=="1")
				.Select(x => PIn.Long(table.Rows[x]["ClaimNum"].ToString())).ToList();
			//prevent user from selecting a claimpayment that is not associated with any of the selected procs
			if(listPayClaimNums.Any(x => !listProcClaimNums.Contains(x))) {
				MsgBox.Show(this,"You can only select claim payments for the selected procedures.");
				gridAccount.SetAll(false);
				return;
			}
			List<long> listPatNums=gridAccount.SelectedIndices
				.Select(x => table.Rows[x]["PatNum"].ToString()).Distinct().Select(x => PIn.Long(x)).ToList();
			List<long> listAdjNums=gridAccount.SelectedIndices
				.Where(x => table.Rows[x]["AdjNum"].ToString()!="0")
				.Select(x => PIn.Long(table.Rows[x]["AdjNum"].ToString())).ToList();
			List<long> listPayNums=gridAccount.SelectedIndices
				.Where(x => table.Rows[x]["PayNum"].ToString()!="0")
				.Select(x => PIn.Long(table.Rows[x]["PayNum"].ToString())).ToList();
			List<long> listProcNums=gridAccount.SelectedIndices
				.Where(x => table.Rows[x]["ProcNum"].ToString()!="0")
				.Select(x => PIn.Long(table.Rows[x]["ProcNum"].ToString())).ToList();
			List<long> listPayPlanChargeNums=gridAccount.SelectedIndices
				.Where(x => table.Rows[x]["PayPlanChargeNum"].ToString()!="0")
				.Select(x => PIn.Long(table.Rows[x]["PayPlanChargeNum"].ToString())).ToList();//Debits attached to insurance payplans do not get shown in the account module.
			Statement statement=Statements.CreateLimitedStatement(listPatNums,_patient.Guarantor,listPayClaimNums,listAdjNums,listPayNums,listProcNums,listPayPlanChargeNums);
			//All printing and emailing will be done from within the form:
			using FormStatementOptions formStatementOptions=new FormStatementOptions();
			formStatementOptions.StatementCur=statement;
			formStatementOptions.ShowDialog();
			if(formStatementOptions.DialogResult!=DialogResult.OK) {
				Statements.DeleteStatements(new List<Statement> { statement });//detached from adjustments, procedurelogs, and paysplits as well
			}
			ModuleSelected(_patient.PatNum);
		}

		private void menuItemLimitedCustom_Click(object sender,EventArgs e) {
			DataRow dataRow;
			DataTable table=_dataSetMain.Tables["account"];
			#region Autoselect Items
			#region Autoselect Today's Procedures
			if(gridAccount.SelectedIndices.Length==0) {//autoselect procedures
				for(int i=0;i<table.Rows.Count;i++) {//loop through every line showing on screen
					dataRow=table.Rows[i];
					if(dataRow["ProcNum"].ToString()=="0" //ignore items that aren't procs
						|| PIn.Date(dataRow["date"].ToString())!=DateTime.Today //autoselecting todays procs only
						|| PIn.Long(dataRow["PatNum"].ToString())!=_patient.PatNum) //only procs for the current patient
					{
						continue;
					}
					if(i < gridAccount.ListGridRows.Count) {
						gridAccount.SetSelected(i,true);
					}
				}
			}
			#endregion Autoselect Today's Procedures
			List<long> listPatNums=new List<long>();
			List<long> listProcClaimNums=new List<long>();
			List<long> listPayClaimNums=new List<long>();
			List<long> listProcNums=new List<long>();
			List<long> listAdjNums=new List<long>();
			List<long> listPayNums=new List<long>();
			List<long> listPayPlanChargeNums=new List<long>();
			if(gridAccount.SelectedIndices.Length>0) {
				PayPlanVersions payPlanVersions=(PayPlanVersions)PrefC.GetInt(PrefName.PayPlansVersion);
				//guaranteed to have rows selected from here down, verify they are allowed transactions
				bool areStatementsSelected=gridAccount.SelectedIndices.Any(x => table.Rows[x]["StatementNum"].ToString()!="0");
				bool arePayPlanCreditsSelected=false;//Default for PayPlanVersions.NoCharges.
				if(payPlanVersions==PayPlanVersions.AgeCreditsAndDebits || payPlanVersions==PayPlanVersions.AgeCreditsOnly) {
					arePayPlanCreditsSelected=gridAccount.SelectedIndices.Any(x => table.Rows[x]["PayPlanChargeNum"].ToString()!="0" && table.Rows[x]["charges"].ToString()=="");
				}
				else if(payPlanVersions==PayPlanVersions.DoNotAge) {
					arePayPlanCreditsSelected=gridAccount.SelectedIndices.Any(x => table.Rows[x]["PayPlanNum"].ToString()!="0" && table.Rows[x]["charges"].ToString()=="");
				}
				if(areStatementsSelected || arePayPlanCreditsSelected) {
					MsgBox.Show(this,"You can only select procedures, adjustments, payments, claims, and pay plan charges.");
					gridAccount.SetAll(false);
					return;
				}
				//get all ClaimNums from claimprocs for the selected procs
				listProcClaimNums=ClaimProcs.GetForProcs(gridAccount.SelectedIndices.Where(x => table.Rows[x]["ProcNum"].ToString()!="0")
					.Select(x => PIn.Long(table.Rows[x]["ProcNum"].ToString())).ToList()).FindAll(x => x.ClaimNum!=0).Select(x => x.ClaimNum).ToList();
				//get all ClaimNums for any selected claimpayments
				listPayClaimNums=gridAccount.SelectedIndices
					.Where(x => table.Rows[x]["ClaimNum"].ToString()!="0" && table.Rows[x]["ClaimPaymentNum"].ToString()=="1")
					.Select(x => PIn.Long(table.Rows[x]["ClaimNum"].ToString())).ToList();
				//prevent user from selecting a claimpayment that is not associatede with any of the selected procs
				if(listPayClaimNums.Any(x => !listProcClaimNums.Contains(x))) {
					MsgBox.Show(this,"You can only select claim payments for the selected procedures.");
					gridAccount.SetAll(false);
					return;
				}
				listPatNums=gridAccount.SelectedIndices
					.Select(x => table.Rows[x]["PatNum"].ToString()).Distinct().Select(x => PIn.Long(x)).ToList();
				listAdjNums=gridAccount.SelectedIndices
					.Where(x => table.Rows[x]["AdjNum"].ToString()!="0")
					.Select(x => PIn.Long(table.Rows[x]["AdjNum"].ToString())).ToList();
				listPayNums=gridAccount.SelectedIndices
					.Where(x => table.Rows[x]["PayNum"].ToString()!="0")
					.Select(x => PIn.Long(table.Rows[x]["PayNum"].ToString())).ToList();
				listProcNums=gridAccount.SelectedIndices
					.Where(x => table.Rows[x]["ProcNum"].ToString()!="0")
					.Select(x => PIn.Long(table.Rows[x]["ProcNum"].ToString())).ToList();
				listPayPlanChargeNums=gridAccount.SelectedIndices
					.Where(x => table.Rows[x]["PayPlanChargeNum"].ToString()!="0")
					.Select(x => PIn.Long(table.Rows[x]["PayPlanChargeNum"].ToString())).ToList();//Debits attached to insurance payplans do not get shown in the account module.
			}
			#endregion
			bool isFamMember=_family.ListPats.Length>1;
			bool isSuperFamMember=(_patient.SuperFamily > 0 && Patients.GetPat(_patient.Guarantor).HasSuperBilling);
			List<Patient> listPatientsSuperFamily = new List<Patient>();
			List<Patient> listPatients=new List<Patient>(_family.ListPats);
			//GetSuperFamAccount takes in a statement which will be used to build the account table, so we must build a temporary one to pass in.
			//Will not be inserted, only used to determine behavior of GetSuperFamAccount.
			Statement statement=new Statement();
			//The point of this fake statement is to get all account information to populate the Limited Statement Select window.
			//Purposefully set the StatementType to NotSet so that GetAccount() acts like we are loading the Account module.
			//Setting it to LimitedStatement would be detrimental because GetAccount() would require lists of account entries to limit the DataSet it returns.
			statement.StatementType=StmtType.NotSet;
			statement.IsNew=true;
			statement.PatNum=_patient.PatNum;
			statement.DateRangeFrom=DateTime.MinValue;
			statement.DateRangeTo=DateTime.Today;
			statement.LimitedCustomFamily=EnumLimitedCustomFamily.Patient;
			if(isSuperFamMember) {
				statement.LimitedCustomFamily=EnumLimitedCustomFamily.SuperFamily;
				statement.SuperFamily=_patient.SuperFamily;
				 listPatientsSuperFamily=Patients.GetBySuperFamily(statement.SuperFamily);
				//Only add the families of the super family where the guarantor allows super billing.
				listPatientsSuperFamily=listPatientsSuperFamily.GroupBy(x => x.Guarantor)
					.ToDictionary(x => x.First(y => y.PatNum==x.Key),x => x.ToList())
					.Where(x => x.Key.HasSuperBilling)
					.SelectMany(x => x.Value).ToList();
				listPatients.AddRange(listPatientsSuperFamily);
			}
			else if(isFamMember) {
				statement.LimitedCustomFamily=EnumLimitedCustomFamily.Family;
			}
			listPatients=listPatients.DistinctBy(x=>x.PatNum).ToList();
			DataSet dataSetSuperFam=AccountModules.GetSuperFamAccount(statement, doIncludePatLName:true, isComputeAging:false,listPatients:listPatients);
			table=dataSetSuperFam.Tables["account"];
			using FormLimitedStatementSelect formLimitedStatementSelect=new FormLimitedStatementSelect();
			formLimitedStatementSelect.TableAccount=table.Copy();
			formLimitedStatementSelect.ListPayClaimNums=listPayClaimNums??new List<long>();
			formLimitedStatementSelect.ListAdjNums=listAdjNums??new List<long>();
			formLimitedStatementSelect.ListPayNums=listPayNums??new List<long>();
			formLimitedStatementSelect.ListProcNums=listProcNums??new List<long>();
			formLimitedStatementSelect.ListPatNums=listPatNums??new List<long>();
			formLimitedStatementSelect.ListPayPlanChargeNums=listPayPlanChargeNums??new List<long>();
			formLimitedStatementSelect.PatCur=_patient;
			if(isSuperFamMember) {
				formLimitedStatementSelect.ListPatNumsSuperFamily=listPatientsSuperFamily.Select(x=>x.PatNum).ToList();
			}
			if(isFamMember) {
				formLimitedStatementSelect.ListPatNumsFamily=_family.GetPatNums();
			}
			if(formLimitedStatementSelect.ShowDialog()!=DialogResult.OK) {
				return;
			}
			listPatNums=formLimitedStatementSelect.ListPatNums;
			listPayClaimNums=formLimitedStatementSelect.ListPayClaimNums;
			listProcNums=formLimitedStatementSelect.ListProcNums;
			listAdjNums=formLimitedStatementSelect.ListAdjNums;
			listPayNums=formLimitedStatementSelect.ListPayNums;
			listPayPlanChargeNums=formLimitedStatementSelect.ListPayPlanChargeNums;
			//At this point, all selected items are procedures, adjustments, payments, or claims.
			Statement statementLimited;
			//Determine if we have selected super fam members.
			bool isSuperFamLimitedStatement=false;
			if(isSuperFamMember) {
				//If any patnums are selected that are not in the family, it must be a super family.
				if(listPatNums.Any(x => !x.In(_family.GetPatNums().ToArray())))	{
					isSuperFamLimitedStatement=true;
				}
			}
			EnumLimitedCustomFamily limitedCustomFamily=EnumLimitedCustomFamily.Family;
			//Figure out which patient deserves to be the PatNum associated to this statement. Start with assumption we will be using the guarantor.
			long patNumStatement=_patient.Guarantor;
			long superFamNum=0;
			if(isSuperFamLimitedStatement) {
				//Only set SuperFamNum if we selected superfam entries.
				limitedCustomFamily=EnumLimitedCustomFamily.SuperFamily;
				patNumStatement=_patient.SuperFamily;
				superFamNum=_patient.SuperFamily;
			}
			else if(listPatNums.Count==1 && listPatNums[0]==_patient.Guarantor) {
				//This is NOT a super family statement. Therefore, if the patient is the guarantor this is a patient statement.
				limitedCustomFamily=EnumLimitedCustomFamily.Patient;
			}
			statementLimited=Statements.CreateLimitedStatement(listPatNums,patNumStatement,listPayClaimNums,listAdjNums,listPayNums,listProcNums,listPayPlanChargeNums,superFamily:superFamNum,limitedCustomFamily:limitedCustomFamily);
			//All printing and emailing will be done from within the form:
			using FormStatementOptions formStatementOptions=new FormStatementOptions();
			formStatementOptions.StatementCur=statementLimited;
			formStatementOptions.ShowDialog();
			if(formStatementOptions.DialogResult!=DialogResult.OK) {
				Statements.DeleteStatements(new List<Statement> { statementLimited });//detached from adjustments, procedurelogs, and paysplits as well
			}
			ModuleSelected(_patient.PatNum);
		}

		private void menuItemQuickProcs_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.AccountProcsQuickAdd)) {
				return;
			}
			//One of the QuickCharge menu items was clicked.
			if(sender.GetType()!=typeof(MenuItem)) {
				return;
			}
			Def defQuickCharge=_arrayDefsAcctProcQuickAdd[contextMenuQuickProcs.MenuItems.IndexOf((MenuItem)sender)];
			string[] stringArrayProcCodes=defQuickCharge.ItemValue.Split(',');
			if(stringArrayProcCodes.Length==0) {
				//No items entered into the definition category.  Notify the user.
				MsgBox.Show(this,"There are no Quick Charge items in Setup | Definitions.  There must be at least one in order to use the Quick Charge drop down menu.");
			}
			List<string> listHiddenProcCodes=ProcedureCodes.GetProcCodesInHiddenCats(stringArrayProcCodes.Select(x => ProcedureCodes.GetCodeNum(x)).ToArray());
			if(listHiddenProcCodes.Count > 0) {
				MessageBox.Show(this,$"{Lan.g(this,"Cannot add the following procedures because they are in a hidden category")}: {string.Join(",",listHiddenProcCodes)}");
				return;
			}
			List<string> listProcCodesAdded=new List<string>();
			Provider provider=Providers.GetProv(_patient.PriProv);
			for(int i=0;i<stringArrayProcCodes.Length;i++) {
				if(AddProcAndValidate(stringArrayProcCodes[i],provider)) {
					listProcCodesAdded.Add(stringArrayProcCodes[i]);
				}
			}
			if(listProcCodesAdded.Count>0) {
				SecurityLogs.MakeLogEntry(EnumPermType.AccountProcsQuickAdd,_patient.PatNum
					,Lan.g(this,"The following procedures were added via the Quick Charge button from the Account module")
						+": "+string.Join(",",listProcCodesAdded));
				ModuleSelected(_patient.PatNum);
			}
		}

		private void menuItemReceipt_Click(object sender,EventArgs e) {
			Statement statement=new Statement();
			statement.PatNum=_patient.PatNum;
			statement.DateSent=DateTime.Today;
			statement.IsSent=true;
			statement.Mode_=StatementMode.InPerson;
			statement.HidePayment=true;
			statement.Intermingled=PrefC.GetBool(PrefName.IntermingleFamilyDefault);
			statement.SinglePatient=!statement.Intermingled;
			statement.IsReceipt=true;
			statement.StatementType=StmtType.NotSet;
			statement.DateRangeFrom=DateTime.Today;
			statement.DateRangeTo=DateTime.Today;
			statement.Note="";
			statement.NoteBold="";
			Patient guarantor=null;
			if(_patient!=null) {
				guarantor = Patients.GetPat(_patient.Guarantor);
			}
			if(guarantor!=null) {
				statement.IsBalValid=true;
				statement.BalTotal=guarantor.BalTotal;
				statement.InsEst=guarantor.InsEst;
			}
			PrintStatement(statement);
			ModuleSelected(_patient.PatNum);
		}

		private void menuItemRepeatCanada_Click(object sender,EventArgs e) {
			if(!ProcedureCodes.GetContainsKey("001")) {
				return;
			}
			UpdatePatientBillingDay(_patient.PatNum);
			RepeatCharge repeatCharge=new RepeatCharge();
			repeatCharge.PatNum=_patient.PatNum;
			repeatCharge.ProcCode="001";
			repeatCharge.ChargeAmt=145;
			repeatCharge.DateStart=DateTime.Today;
			repeatCharge.DateStop=DateTime.Today.AddMonths(11);
			repeatCharge.IsEnabled=true;
			repeatCharge.RepeatChargeNum=RepeatCharges.Insert(repeatCharge);
			RepeatCharges.InsertRepeatChargeChangeSecurityLogEntry(repeatCharge,EnumPermType.RepeatChargeCreate,_patient,isAutomated:false);
			repeatCharge=new RepeatCharge();
			repeatCharge.PatNum=_patient.PatNum;
			repeatCharge.ProcCode="001";
			repeatCharge.ChargeAmt=119;
			repeatCharge.DateStart=DateTime.Today.AddYears(1);
			repeatCharge.IsEnabled=true;
			repeatCharge.RepeatChargeNum=RepeatCharges.Insert(repeatCharge);
			RepeatCharges.InsertRepeatChargeChangeSecurityLogEntry(repeatCharge,EnumPermType.RepeatChargeCreate,_patient,isAutomated:false);
			ModuleSelected(_patient.PatNum);
		}

		private void MenuItemRepeatEmail_Click(object sender,EventArgs e) {
			if(!ProcedureCodes.GetContainsKey("008")) {
				return;
			}
			UpdatePatientBillingDay(_patient.PatNum);
			RepeatCharge repeatCharge=new RepeatCharge();
			repeatCharge.PatNum=_patient.PatNum;
			repeatCharge.ProcCode="008";
			repeatCharge.ChargeAmt=89;
			repeatCharge.DateStart=DateTime.Today;
			repeatCharge.IsEnabled=true;
			repeatCharge.RepeatChargeNum=RepeatCharges.Insert(repeatCharge);
			RepeatCharges.InsertRepeatChargeChangeSecurityLogEntry(repeatCharge,EnumPermType.RepeatChargeCreate,_patient,isAutomated:false);
			ModuleSelected(_patient.PatNum);
		}

		private void menuItemRepeatSignupPortal_Click(object sender,EventArgs e) {
			if(_patient==null) {
				MsgBox.Show(this,"A customer must be selected first.");
				return;
			}
			List<RegistrationKey> listRegistrationKeys=RegistrationKeys.GetForPatient(_patient.PatNum)
				//.Where(x => RegistrationKeys.KeyIsEnabled(x)) //We no longer want to only show enabled keys, sometimes we need to manage disabled.
				.OrderBy(x => x.RegKey)
				.ToList();
			if(listRegistrationKeys.Count<1) {
				MsgBox.Show(this,"No registration keys found for this customer's family.");
				return;
			}
			RegistrationKey registrationKey;
			if(listRegistrationKeys.Count==1) {
				registrationKey=listRegistrationKeys[0];
			}
			else {
				List<string> listKeysDisplayed=new List<string>();
				for(int i=0;i<listRegistrationKeys.Count();i++) {
					string str="PatNum: "+listRegistrationKeys[i].PatNum+"  RegKey: "+listRegistrationKeys[i].RegKey;
					if(listRegistrationKeys[i].DateDisabled<DateTime.Today && listRegistrationKeys[i].DateDisabled.Year>1) {
						str+="  Disabled: "+listRegistrationKeys[i].DateDisabled.ToShortDateString();
					}
					if(listRegistrationKeys[i].DateEnded<DateTime.Today && listRegistrationKeys[i].DateEnded.Year>1) {
						str+="  Ended: "+listRegistrationKeys[i].DateEnded.ToShortDateString();
					}
					listKeysDisplayed.Add(str);
				}
				using InputBox inputBox=new InputBox("Select a registration key to load into the Signup Portal",listKeysDisplayed);
				if(inputBox.ShowDialog()!=DialogResult.OK) {
					return;
				}
				registrationKey=listRegistrationKeys[inputBox.SelectedIndex];
			}
			WebServiceMainHQProxy.EServiceSetup.SignupOut signupOut=null;
			try {
				//Get the URL for the selected registration key.
				signupOut=WebServiceMainHQProxy.GetEServiceSetupLite(SignupPortalPermission.FromHQ,registrationKey.RegKey,"","","");
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			using FormEServicesSignup formEServicesSignup=new FormEServicesSignup(signupOut);
			formEServicesSignup.ShowDialog();
			ModuleSelected(_patient.PatNum);//Refresh the module.
		}

		private void MenuItemRepeatStand_Click(object sender,EventArgs e) {
			if(!ProcedureCodes.GetContainsKey("001")) {
				return;
			}
			UpdatePatientBillingDay(_patient.PatNum);
			RepeatCharge repeatCharge=new RepeatCharge();
			repeatCharge.PatNum=_patient.PatNum;
			repeatCharge.ProcCode="001";
			repeatCharge.ChargeAmt=179;
			repeatCharge.DateStart=DateTime.Today;
			repeatCharge.DateStop=DateTime.Today.AddMonths(11);
			repeatCharge.IsEnabled=true;
			repeatCharge.RepeatChargeNum=RepeatCharges.Insert(repeatCharge);
			RepeatCharges.InsertRepeatChargeChangeSecurityLogEntry(repeatCharge,EnumPermType.RepeatChargeCreate,_patient,isAutomated:false);
			repeatCharge=new RepeatCharge();
			repeatCharge.PatNum=_patient.PatNum;
			repeatCharge.ProcCode="001";
			repeatCharge.ChargeAmt=129;
			repeatCharge.DateStart=DateTime.Today.AddYears(1);
			repeatCharge.IsEnabled=true;
			repeatCharge.RepeatChargeNum=RepeatCharges.Insert(repeatCharge);
			RepeatCharges.InsertRepeatChargeChangeSecurityLogEntry(repeatCharge,EnumPermType.RepeatChargeCreate,_patient,isAutomated:false);
			ModuleSelected(_patient.PatNum);
		}

		private void menuItemSalesTax_Click(object sender,EventArgs e) {
			if(gridAccount.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select at least one procedure.");
				return;
			}
			DataTable table=_dataSetMain.Tables["account"];
			List<long> listProcNumsSelected=new List<long>();
			List<int>listidxs=gridAccount.SelectedIndices.ToList();
			for(int i=0;i<listidxs.Count();i++) {
				if(table.Rows[listidxs[i]]["ProcNum"].ToString()=="0") {
					continue;
				}
				listProcNumsSelected.Add(PIn.Long(table.Rows[listidxs[i]]["ProcNum"].ToString()));
			}
			List<OrthoProcLink> listOrthoProcLinks=OrthoProcLinks.GetManyForProcs(listProcNumsSelected);
			if(listOrthoProcLinks.Count>0) {
				MsgBox.Show(this,"One or more of the selected procedures cannot be adjusted because it is attached to an ortho case." +
					" Please deselect these items and try again.");
				return;
			}
			List<Procedure> listProcedures=Procedures.GetManyProc(listProcNumsSelected,false);
			for(int i=0;i<listProcedures.Count;i++) {
				Adjustments.CreateAdjustmentForSalesTax(listProcedures[i],true);
			}
			Signalods.SetInvalid(InvalidType.BillingList);
			ModuleSelected(_patient.PatNum);
		}

		private void menuItemStatementEmail_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.EmailSend)) {
				Cursor=Cursors.Default;
				return;
			}
			Statement statement=new Statement();
			statement.PatNum=_patient.Guarantor;
			statement.DateSent=DateTime.Today;
			statement.IsSent=true;
			statement.Mode_=StatementMode.Email;
			statement.HidePayment=false;
			statement.SinglePatient=false;
			statement.Intermingled=PrefC.GetBool(PrefName.IntermingleFamilyDefault);
			statement.IsReceipt=false;
			statement.StatementType=StmtType.NotSet;
			statement.DateRangeFrom=DateTime.MinValue;
			if(textDateStart.IsValid()){
				if(textDateStart.Text!=""){
					statement.DateRangeFrom=PIn.Date(textDateStart.Text);
				}
			}
			statement.DateRangeTo=DateTime.Today;//Needed for payplan accuracy.  Used to be setting to new DateTime(2200,1,1);
			if(textDateEnd.IsValid()){
				if(textDateEnd.Text!=""){
					statement.DateRangeTo=PIn.Date(textDateEnd.Text);
				}
			}
			statement.Note="";
			statement.NoteBold="";
			Patient patientGuarantor = null;
			if(_patient!=null) {
				patientGuarantor = Patients.GetPat(_patient.Guarantor);
			}
			if(patientGuarantor!=null) {
				statement.IsBalValid=true;
				statement.BalTotal=patientGuarantor.BalTotal;
				statement.InsEst=patientGuarantor.InsEst;
			}
			//It's pointless to give the user the window to select statement options, because they could just as easily have hit the More Options dropdown, then Email from there.
			PrintStatement(statement);
			ModuleSelected(_patient.PatNum);
		}

		private void menuItemStatementMore_Click(object sender,EventArgs e) {
			Statement statement=new Statement();
			statement.PatNum=_patient.PatNum;
			statement.DateSent=DateTime.Today;
			statement.IsSent=false;
			statement.Mode_=StatementMode.InPerson;
			statement.HidePayment=false;
			statement.SinglePatient=false;
			statement.Intermingled=PrefC.GetBool(PrefName.IntermingleFamilyDefault);
			statement.IsReceipt=false;
			statement.StatementType=StmtType.NotSet;
			statement.DateRangeFrom=DateTime.MinValue;
			statement.DateRangeFrom=DateTime.MinValue;
			if(textDateStart.IsValid()){
				if(textDateStart.Text!=""){
					statement.DateRangeFrom=PIn.Date(textDateStart.Text);
				}
			}
			statement.DateRangeTo=DateTime.Today;//Needed for payplan accuracy.//new DateTime(2200,1,1);
			if(textDateEnd.IsValid()){
				if(textDateEnd.Text!=""){
					statement.DateRangeTo=PIn.Date(textDateEnd.Text);
				}
			}
			statement.Note="";
			statement.NoteBold="";
			Patient guarantor=null;
			if(_patient!=null) {
				guarantor=Patients.GetPat(_patient.Guarantor);
			}
			if(guarantor!=null) {
				statement.IsBalValid=true;
				statement.BalTotal=guarantor.BalTotal;
				statement.InsEst=guarantor.InsEst;
			}
			//All printing and emailing will be done from within the form:
			using FormStatementOptions formStatementOptions=new FormStatementOptions();
			statement.IsNew=true;
			formStatementOptions.StatementCur=statement;
			formStatementOptions.ShowDialog();
			ModuleSelected(_patient.PatNum);
		}

		private void menuItemStatementWalkout_Click(object sender,EventArgs e) {
			Statement statement=new Statement();
			statement.PatNum=_patient.PatNum;
			statement.DateSent=DateTime.Today;
			statement.IsSent=true;
			statement.Mode_=StatementMode.InPerson;
			statement.HidePayment=true;
			statement.Intermingled=PrefC.GetBool(PrefName.IntermingleFamilyDefault);
			statement.SinglePatient=!statement.Intermingled;
			statement.IsReceipt=false;
			statement.StatementType=StmtType.NotSet;
			statement.DateRangeFrom=DateTime.Today;
			statement.DateRangeTo=DateTime.Today;
			statement.Note="";
			statement.NoteBold="";
			Patient guarantor=null;
			if(_patient!=null) {
				guarantor=Patients.GetPat(_patient.Guarantor);
			}
			if(guarantor!=null) {
				statement.IsBalValid=true;
				statement.BalTotal=guarantor.BalTotal;
				statement.InsEst=guarantor.InsEst;
			}
			PrintStatement(statement);
			ModuleSelected(_patient.PatNum);
		}

		private void menuItemSendPaymentToDevice_Click(object sender,EventArgs e) {
			if(_patient==null){
				MsgBox.Show("Please select a patient first.");
				return;
			}
			if(!MobileAppDevices.IsClinicSignedUpForEClipboard(Clinics.ClinicNum)) {
				if(PrefC.HasClinicsEnabled) {
					MsgBox.Show(this,"Please enable eClipboard for the current clinic to use this feature.");
					return;
				}
				MsgBox.Show(this,"Please enable eClipboard to use this feature.");
				return;
			}
			string error="";
			if(!PrefC.HasOnlinePaymentEnabled(out ProgramName progName)) {
				error+="Please enable online payments.\n";
			}
			if(!IsWebPaymentsEnabled()) {
				error+="Please enable payments for eClipboard to use this feature.\n";
			}
			if(!error.IsNullOrEmpty()) {
				MsgBox.Show(error);
				return;
			}
			if(MobileAppDevices.ShouldSendPush(_patient.PatNum, out MobileAppDevice mobileAppDevice)) {
				PushPaymentToEclipboard(mobileAppDevice);
				return;
			}
			OpenUnlockCodeForPayment();
		}
		#endregion Methods - Event Handlers MenuItem

		#region Methods - Event Handlers Parent
		private void Parent_MouseWheel(Object sender,MouseEventArgs e){
			if(Visible){
				this.OnMouseWheel(e);
			}
		}
		#endregion Methods - Event Handlers Parent

		#region Methods - Event Handlers TaskGoToEvent
		public void TaskGoToEvent(object sender,CancelEventArgs e) {
			FormTaskEdit formTaskEdit=(FormTaskEdit)sender;
			TaskObjectType taskObjectType=formTaskEdit.TaskObjectTypeGoTo;
			long keyNum=formTaskEdit.KeyNumGoTo;
			if(taskObjectType==TaskObjectType.None) {
				return;
			}
			if(taskObjectType==TaskObjectType.Patient) {
				if(keyNum!=0) {
					Patient patient=Patients.GetPat(keyNum);
					FormOpenDental.S_Contr_PatientSelected(patient,false);
					ModuleSelected(patient.PatNum);
					return;
				}
			}
			if(taskObjectType==TaskObjectType.Appointment) {
				//There's nothing to do here, since we're not in the appt module.
				return;
			}
		}
		#endregion Methods - Event Handlers TaskGoToEvent 

		#region Methods - Event Handlers Text Fields
		private void textFinNote_Leave(object sender,EventArgs e) {
			UpdateFinNote();
		}

		private void textFinNote_TextChanged(object sender,EventArgs e) {
			IsFinNoteChanged=true;
		}

		private void textQuickCharge_CaptureChange(object sender,EventArgs e) {
			if(textQuickProcs.Visible==true) {
				textQuickProcs.Capture=true;
			}
		}

		private void textQuickCharge_FocusLost(object sender,EventArgs e) {
			textQuickProcs.Text="";
			textQuickProcs.Visible=false;
			textQuickProcs.Capture=false;
		}

		private void textQuickCharge_KeyDown(object sender,KeyEventArgs e) {
			//This is only the KeyDown event, user can still type if we return here.
			if(e.KeyCode!=Keys.Enter) {
				return;
			}
			textQuickProcs.Visible=false;
			textQuickProcs.Capture=false;
			e.Handled=true;//Suppress the "ding" in windows when pressing enter.
			e.SuppressKeyPress=true;//Suppress the "ding" in windows when pressing enter.
			if(textQuickProcs.Text=="") {
				return;
			}
			string quickProcText=textQuickProcs.Text;//because the text seems to disappear from textbox in menu bar when MsgBox comes up.
			if(PrefC.IsODHQ){
				if (_patient.State=="") {
					MessageBox.Show("A valid state is required to process sales tax on procedures. "
						+"Please delete the procedure, enter a valid state, then reenter the procedure.");
				}
				//if this patient is in a taxable state
				if(AvaTax.GetListTaxableStates()!=null && AvaTax.GetListTaxableStates().Any(x => x==_patient.State)){
					if(!Patients.HasValidUSZipCode(_patient)) {
						MessageBox.Show("A valid zip code is required to process sales tax on procedures in this patient's state. "
						+"Please delete the procedure, enter a valid zip, then reenter the procedure.");
					}
				}
			}
			Provider provider=Providers.GetProv(_patient.PriProv);
			if(AddProcAndValidate(quickProcText,provider)) {
				SecurityLogs.MakeLogEntry(EnumPermType.AccountProcsQuickAdd,_patient.PatNum
					,Lan.g(this,"The following procedures were added via the Quick Charge button from the Account module")
						+": "+string.Join(",",quickProcText));
				ModuleSelected(_patient.PatNum);
			}
			textQuickProcs.Text="";
		}

		private void textQuickCharge_MouseClick(object sender,MouseEventArgs e) {
			if(e.X<0 || e.X>textQuickProcs.Width ||e.Y<0 || e.Y>textQuickProcs.Height) {
				textQuickProcs.Text="";
				textQuickProcs.Visible=false;
				textQuickProcs.Capture=false;
			}
		}

		private void textUrgFinNote_Leave(object sender,EventArgs e) {
			//need to skip this if selecting another module. Handled in ModuleUnselected due to click event
			UpdateUrgFinNote();
		}

		private void textUrgFinNote_TextChanged(object sender,EventArgs e) {
			IsUrgFinNoteChanged=true;
		}
		#endregion Methods - Event Handlers Text Fields

		#region Methods - Event Handlers ToolBarMain
		private void ToolBarMain_ButtonClick(object sender,ODToolBarButtonClickEventArgs e) {
			if(e.Button.Tag.GetType()==typeof(string)){
				if(Patients.GetPat(_patient.PatNum).PatStatus==PatientStatus.Deleted) {
					MsgBox.Show(this, "Selected patient has been deleted by another workstation.");
					return;
				}
				//standard predefined button
				switch(e.Button.Tag.ToString()){
					//case "Patient":
					//	OnPat_Click();
					//	break;
					case "Payment":
						if(Plugins.HookMethod(this,"ContrAccount.ToolBarMain_ButtonClick_Payment")) {
							break;
						}
						bool isTsiPayment=(TsiTransLogs.IsTransworldEnabled(_patient.ClinicNum)
							&& Patients.IsGuarCollections(_patient.Guarantor,includeSuspended:false)
							&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"The guarantor of this family has been sent to TSI for a past due balance.  "
								+"Is the payment you are applying directly from the debtor or guarantor?\r\n\r\n"
								+"Yes - this payment is directly from the debtor/guarantor\r\n\r\n"
								+"No - this payment is from TSI"));
						List<InputBoxParam> listInputBoxParams=new List<InputBoxParam>();
						listInputBoxParams.Add(new InputBoxParam(InputBoxType.ValidDouble,Lan.g(this,"Please enter an amount: ")));
						if(_family.ListPats.Length>1){
							listInputBoxParams.Add(new InputBoxParam(InputBoxType.CheckBox,"",Lan.g(this," - Prefer this patient"),new Size(120,20)));
						}
						Func<string, bool> funcOkClick=new Func<string, bool>((text) => {
							if(text=="") {
								MsgBox.Show(this,"Please enter a value.");
								return false;//Should stop user from continuing to payment window.
							}
							return true;//Allow user to the payment window.
							});
						InputBox inputBox=new InputBox(listInputBoxParams,funcOkClick);
						Plugins.HookAddCode(this,"ContrAccount.ToolBarMain_ButtonClick_paymentInputBox",inputBox,_patient);
						if(inputBox.ShowDialog()!=DialogResult.OK) {
							inputBox.Dispose();
							break;
						}
						Plugins.HookAddCode(this,"ControlAccount.ToolBarMain_ButtonClick_afterPaymentInputBox",inputBox,_patient);
						toolBarButPay_Click(PIn.Double(inputBox.textResult.Text),preferCurrentPat:(inputBox.checkBoxResult?.Checked??false),isTsiPayment:isTsiPayment);
						inputBox.Dispose();
						break;
					case "Adjustment":
						toolBarButAdj_Click();
						break;
					case "Insurance":
						CreateClaimDataWrapper createClaimDataWrapper=ClaimL.GetCreateClaimDataWrapper(_patient,_family,GetCreateClaimItemsFromUI(),true);
						if(createClaimDataWrapper.HasError) {
							break;
						}
						createClaimDataWrapper=ClaimL.CreateClaimFromWrapper(true,createClaimDataWrapper);
						if(!createClaimDataWrapper.HasError || createClaimDataWrapper.ShouldRefresh) {
							ModuleSelected(_patient.PatNum);
						}
						break;
					case "PayPlan":
						contextMenuPayPlan.Show(ToolBarMain,new Point(e.Button.Bounds.Location.X,e.Button.Bounds.Height));
						break;
					case "InstallPlan":
						toolBarButInstallPlan_Click();
						break;
					case "RepeatCharge":
						toolBarButRepeatCharge_Click();
						break;
					case "Statement":
						//The reason we are using a delegate and BeginInvoke() is because of a Microsoft bug that causes the Print Dialog window to not be in focus			
						//when it comes from a toolbar click.
						//https://social.msdn.microsoft.com/Forums/windows/en-US/681a50b4-4ae3-407a-a747-87fb3eb427fd/first-mouse-click-after-showdialog-hits-the-parent-form?forum=winforms
						ToolBarClick toolBarClick=toolBarButStatement_Click;
						this.BeginInvoke(toolBarClick);
						break;
					case "QuickProcs":
						toolBarButQuickProcs_Click();
						break;
				}
			}
			else if(e.Button.Tag.GetType()==typeof(Program)) {
				ProgramL.Execute(((Program)e.Button.Tag).ProgramNum,_patient);
			}
			Plugins.HookAddCode(this,"ContrAccount.ToolBarMain_ButtonClick_end",_patient,e);
		}
		#endregion Methods - Event Handlers ToolBarMain

		#region Methods - Public
		///<summary></summary>
		public void InitializeOnStartup() {
			if(_isInitializedOnStartup) {
				return;
			}
			_isInitializedOnStartup=true;
			//can't use Lan.F(this);
			Lan.C(this,
				labelStartDate,
				labelEndDate,
				label2,//label2 is the 'Family Aging' text.
				label7,//label7 is the '0-30' text.
				label6,//label6 is the '31-60' text.
				label5,//label5 is the '61-90' text.
				label3,//label3 is the 'over 90' text.
				labelUrgFinNote,
				labelFamFinancial,
				tabControlAccount,
				gridAccount,
				gridAcctPat,
				gridComm,
				gridPatInfo,
				gridPayPlan,
				gridRepeat,
				labelDisRem,
				label21,//label21 is the 'TOTAL  Owed w/ Plan:' text.
				labelInsEst,
				labelBalance,
				labelPatEstBal,
				labelUnearned,
				labelInsRem,
				labelTotal,
				tabMain,
				tabShow,
				butToday,
				but45days,
				but90days,
				butDatesAll,
				butRefresh,
				checkShowCommAuto,
				checkShowCompletePayPlans,
				checkShowFamilyComm,
				checkShowDetail,
				butServiceDateView,
				butCreditCard,
				groupBoxFamilyIns,
				label4,//label4 is the 'Primary' text.
				label8,//label8 is the 'Annual Max' text.
				label9,//label9 is the 'Secondary' text.
				label17,//label17 is the 'Fam Ded' text.
				groupBoxIndIns,
				label10,//label10 is also 'Primary' text.
				label11,//label11 is also 'Annual Max' text.
				label18,//label18 is the 'Ded Remain' text.
				label12,//label12 is the 'Deductible' text.
				label13,//label13 is the 'Ins Used' text.
				label14,//label14 is the 'Remaining' text.
				label15,//label15 is the 'Pending' text.
				label16//label16 is also 'Secondary' text.
			);
			Lan.C(this,contextMenuIns,contextMenuStatement);
			LayoutToolBar();
			textQuickProcs.AcceptsTab=true;
			textQuickProcs.KeyDown+=textQuickCharge_KeyDown;
			textQuickProcs.MouseDown+=textQuickCharge_MouseClick;
			textQuickProcs.MouseCaptureChanged+=textQuickCharge_CaptureChange;
			textQuickProcs.LostFocus+=textQuickCharge_FocusLost;
			//This just makes the patient information grid show up or not.
			_listDisplayFieldsPatInfo=DisplayFields.GetForCategory(DisplayFieldCategory.AccountPatientInformation);
			LayoutPanels();//Only place that we call this outside of LayoutPanelsAndRefreshMainGrids() since no grid data has been loaded yet
			splitContainerAccountCommLog.SplitterDistance=splitContainerParent.Panel2.Height * 3/5;//Make Account grid slightly bigger than commlog
			checkShowFamilyComm.Checked=PrefC.GetBoolSilent(PrefName.ShowAccountFamilyCommEntries,true);
			checkShowCompletePayPlans.Checked=PrefC.GetBool(PrefName.AccountShowCompletedPaymentPlans);
			Plugins.HookAddCode(this,"ContrAccount.InitializeOnStartup_end");
		}

		///<summary>Causes the toolbar to be laid out again.</summary>
		public void LayoutToolBar() {
			ToolBarMain.Buttons.Clear();
			ODToolBarButton toolBarButton;
			_butPayment=new ODToolBarButton(Lan.g(this,"Payment"),1,"","Payment");
			_butPayment.Style=ODToolBarButtonStyle.DropDownButton;
			_butPayment.DropDownMenu=contextMenuPayment;
			ToolBarMain.Buttons.Add(_butPayment);
			toolBarButton=new ODToolBarButton(Lan.g(this,"Adjustment"),2,"","Adjustment");
			toolBarButton.Style=ODToolBarButtonStyle.DropDownButton;
			toolBarButton.DropDownMenu=contextMenuAdjust;
			ToolBarMain.Buttons.Add(toolBarButton);
			toolBarButton=new ODToolBarButton(Lan.g(this,"New Claim"),3,"","Insurance");
			toolBarButton.Style=ODToolBarButtonStyle.DropDownButton;
			toolBarButton.DropDownMenu=contextMenuIns;
			ToolBarMain.Buttons.Add(toolBarButton);
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			toolBarButton=new ODToolBarButton(Lan.g(this,"Payment Plan"),-1,"","PayPlan");
			toolBarButton.Style=ODToolBarButtonStyle.DropDownButton;
			toolBarButton.DropDownMenu=contextMenuPayPlan;
			ToolBarMain.Buttons.Add(toolBarButton);
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Installment Plan"),-1,"","InstallPlan"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			_butQuickProcs=new ODToolBarButton(Lan.g(this,"Quick Procs"),-1,"","QuickProcs");
			_butQuickProcs.Style=ODToolBarButtonStyle.DropDownButton;
			_butQuickProcs.DropDownMenu=contextMenuQuickProcs;
			contextMenuQuickProcs.Popup+=new EventHandler(contextMenuQuickProcs_Popup);
			ToolBarMain.Buttons.Add(_butQuickProcs);
			if(!PrefC.GetBool(PrefName.EasyHideRepeatCharges)) {
				toolBarButton=new ODToolBarButton(Lan.g(this,"Repeating Charge"),-1,"","RepeatCharge");
				toolBarButton.Style=ODToolBarButtonStyle.NormalButton;
				if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {//contextMenuRepeat items only get initialized when at HQ.
					toolBarButton.Style=ODToolBarButtonStyle.DropDownButton;
					toolBarButton.DropDownMenu=contextMenuRepeat;
				}
				ToolBarMain.Buttons.Add(toolBarButton);
			}
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			toolBarButton=new ODToolBarButton(Lan.g(this,"Statement"),4,"","Statement");
			toolBarButton.Style=ODToolBarButtonStyle.DropDownButton;
			toolBarButton.DropDownMenu=contextMenuStatement;
			ToolBarMain.Buttons.Add(toolBarButton);
			ProgramL.LoadToolbar(ToolBarMain,EnumToolBar.AccountModule);
			ToolBarMain.Invalidate();
			Plugins.HookAddCode(this,"ContrAccount.LayoutToolBar_end",_patient);
		}

		///<summary></summary>
		public void ModuleSelected(long patNum) {
			ModuleSelected(patNum,false);
		}

		///<summary></summary>
		public void ModuleSelected(long patNum,bool isSelectingFamily) {
			UserOdPref userOdPrefProcBreakdown=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.AcctProcBreakdown).FirstOrDefault();
			UserOdPref userOdPrefShowAutoCommlog=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.ShowAutomatedCommlog).FirstOrDefault();
			if(userOdPrefProcBreakdown==null) {
				checkShowDetail.Checked=true;
			}
			else {
				checkShowDetail.Checked=PIn.Bool(userOdPrefProcBreakdown.ValueString);
			}
			if(userOdPrefShowAutoCommlog==null) {
				checkShowCommAuto.Checked=true;
			}
			else {
				checkShowCommAuto.Checked=PIn.Bool(userOdPrefShowAutoCommlog.ValueString);
			}
			Logger.LogAction("RefreshModuleData",LogPath.AccountModule,() => RefreshModuleData(patNum,isSelectingFamily));
			if(_patient!=null && _patient.PatStatus==PatientStatus.Deleted) {
				MsgBox.Show("Selected patient has been deleted by another workstation.");
				PatientL.RemoveFromMenu(_patient.PatNum);
				FormOpenDental.S_Contr_PatientSelected(new Patient(),false);
				RefreshModuleData(0,isSelectingFamily);
			}
			if(_patient!=null && _patient.PatStatus==PatientStatus.Archived && !Security.IsAuthorized(EnumPermType.ArchivedPatientSelect,suppressMessage:true)) {
				FormOpenDental.S_Contr_PatientSelected(new Patient(),false);
				RefreshModuleData(0,isSelectingFamily);
			}
			Logger.LogAction("RefreshModuleScreen",LogPath.AccountModule,() => RefreshModuleScreen(isSelectingFamily));
			PatientDashboardDataEvent.Fire(ODEventType.ModuleSelected,_loadData);
			if(_patient!=null && DatabaseIntegrities.DoShowPopup(_patient.PatNum,EnumModuleType.Account)) {
				List<PayPlan> listPayPlans=PayPlans.GetForPatNum(_patient.PatNum);
				DataTable tableAccount=_dataSetMain.Tables["account"];
				List<long> listPayNums=tableAccount.Select().Select(x => PIn.Long(x["PayNum"].ToString())).ToList();
				listPayNums.RemoveAll(x => x==0); //remove all non payment PKs
				List<PaySplit> listPaySplits=PaySplits.GetForPayments(listPayNums);
				bool areHashesValid=Patients.AreAllHashesValid(_patient,new List<Appointment>(),listPayPlans,listPaySplits);
				if(!areHashesValid) {
					DatabaseIntegrities.AddPatientModuleToCache(_patient.PatNum,EnumModuleType.Account); //Add to cached list for next time
					//show popup
					DatabaseIntegrity databaseIntegrity=DatabaseIntegrities.GetModule();
					using FormDatabaseIntegrity formDatabaseIntegrity=new FormDatabaseIntegrity();
					formDatabaseIntegrity.MessageToShow=databaseIntegrity.Message;
					formDatabaseIntegrity.ShowDialog();
				}
			}
			Plugins.HookAddCode(this,"ContrAccount.ModuleSelected_end",patNum,isSelectingFamily);
		}

		///<summary>Used when jumping to this module and directly to a claim.</summary>
		public void ModuleSelected(long patNum,long claimNum) {
			ModuleSelected(patNum);
			DataTable table=_dataSetMain.Tables["account"];
			for(int i=0;i<table.Rows.Count;i++){
				if(table.Rows[i]["ClaimPaymentNum"].ToString()!="0") {//claimpayment
					continue;
				}
				if(table.Rows[i]["ClaimNum"].ToString()=="0") {//not a claim or claimpayment
					continue;
				}
				long claimNumRow=PIn.Long(table.Rows[i]["ClaimNum"].ToString());
				if(claimNumRow!=claimNum){
					continue;
				}
				if(i<gridAccount.ListGridRows.Count) {
					gridAccount.SetSelected(i,true);
				}
			}
		}

		///<summary></summary>
		public void ModuleUnselected() {
			UpdateUrgFinNote();
			UpdateFinNote();
			_family=null;
			_repeatChargeArray=null;
			_patNumLast=0;//Clear out the last pat num so that a security log gets entered that the module was "visited" or "refreshed".
			Plugins.HookAddCode(this,"ContrAccount.ModuleUnselected_end");
		}

		/// <summary>Only for use in FormOpenDental.cs form deactivate event handler. Used to prevent a bug that would clear out the FamUrgFinNote when closing the program sometimes.
		/// There is a case where a user has a note on load, intentionally clears that note and OD is shutdown via shutdown signal or other process termination before they leave the textbox. This will result in the note that they deleted not being updated and still being present when OD is opened again as if the changes made were not committed.</summary>
		public bool canUpdateFinNote() {
			if(IsFinNoteChanged) {
				if(textFinNote.Text=="" && _famFinNoteOnLoad != textFinNote.Text) {
					return false;
				}
			}
			return true;
		}

		public void UpdateFinNote() {
			if(_family==null)
				return;
			if(IsFinNoteChanged){
				_patientNote.FamFinancial=textFinNote.Text;
				PatientNotes.Update(_patientNote,_patient.Guarantor);
				IsFinNoteChanged=false;
			}
		}

		/// <summary>Only for use in FormOpenDental.cs form deactivate event handler. Used to prevent a bug that would clear out the FamUrgFinNote when closing the program sometimes.
		/// There is a case where a user has a note on load, intentionally clears that note and OD is shutdown via shutdown signal or other process termination before they leave the textbox. This will result in the note that they deleted not being updated and still being present when OD is opened again as if the changes made were not committed.</summary>
		public bool canUpdateUrgFinNote() {
			if(IsUrgFinNoteChanged) {
				if(textUrgFinNote.Text=="" && _famUrgFinNoteOnLoad != textUrgFinNote.Text) {
					return false;
				}
			}
			return true;
		}

		public void UpdateUrgFinNote() {
			if(_family==null)
				return;
			if(IsUrgFinNoteChanged){
				Patient patientOld=_family.ListPats[0].Copy();
				_family.ListPats[0].FamFinUrgNote=textUrgFinNote.Text;
				Patients.Update(_family.ListPats[0],patientOld);
				IsUrgFinNoteChanged=false;
			}
		}
		#endregion Methods - Public

		#region Methods - Private ToolBar
		private void toolBarButAdj_Click() {
			AddAdjustmentToSelectedProcsHelper();
		}

		private void toolBarButInstallPlan_Click() {
			if(InstallmentPlans.GetOneForFam(_patient.Guarantor)!=null) {
				MsgBox.Show(this,"Family already has an installment plan.");
				return;
			}
			InstallmentPlan installmentPlan=new InstallmentPlan();
			installmentPlan.PatNum=_patient.Guarantor;
			installmentPlan.DateAgreement=DateTime.Today;
			installmentPlan.DateFirstPayment=DateTime.Today;
			//InstallmentPlans.Insert(installPlan);
			using FormInstallmentPlanEdit formInstallmentPlanEdit=new FormInstallmentPlanEdit();
			formInstallmentPlanEdit.InstallmentPlanCur=installmentPlan;
			formInstallmentPlanEdit.IsNew=true;
			formInstallmentPlanEdit.ShowDialog();
			ModuleSelected(_patient.PatNum);
		}

		private void toolBarButPay_Click(double payAmt,bool preferCurrentPat=false,bool isPrePay=false,bool isIncomeTransfer=false,bool isTsiPayment=false) {
			Payment payment=new Payment();
			payment.PayDate=DateTime.Today;
			payment.PatNum=_patient.PatNum;
			//Explicitly set ClinicNum=0, since a pat's ClinicNum will remain set if the user enabled clinics, assigned patients to clinics, and then
			//disabled clinics because we use the ClinicNum to determine which PayConnect or XCharge/XWeb credentials to use for payments.
			payment.ClinicNum=0;
			if(PrefC.HasClinicsEnabled) {//if clinics aren't enabled default to 0
				if((PayClinicSetting)PrefC.GetInt(PrefName.PaymentClinicSetting)==PayClinicSetting.PatientDefaultClinic) {
					payment.ClinicNum=_patient.ClinicNum;
				}
				else if((PayClinicSetting)PrefC.GetInt(PrefName.PaymentClinicSetting)==PayClinicSetting.SelectedExceptHQ) {
					payment.ClinicNum=(Clinics.ClinicNum==0)?_patient.ClinicNum:Clinics.ClinicNum;
				}
				else {
					payment.ClinicNum=Clinics.ClinicNum;
				}
			}
			payment.DateEntry=DateTime.Today;//So that it will show properly in the new window.
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			if(listDefs.Count>0) {
				payment.PayType=listDefs[0].DefNum;
			}
			payment.PaymentSource=CreditCardSource.None;
			payment.ProcessStatus=ProcessStat.OfficeProcessed;
			payment.PayAmt=payAmt;
			using FormPayment formPayment=new FormPayment(_patient,_family,payment,preferCurrentPat);
			formPayment.IsNew=true;
			formPayment.IsIncomeTransfer=isIncomeTransfer;
			List<AccountEntry> listAccountEntries=new List<AccountEntry>();
			if(gridAccount.SelectedIndices.Length>0) {
				DataTable table=_dataSetMain.Tables["account"];
				List<int> listIndicesSelected=gridAccount.SelectedIndices.ToList();
				for(int i=0;i<listIndicesSelected.Count();i++) {
					long adjNum=PIn.Long(table.Rows[listIndicesSelected[i]]["AdjNum"].ToString());
					double chargesDouble=PIn.Double(table.Rows[listIndicesSelected[i]]["chargesDouble"].ToString());
					long payPlanChargeNum=PIn.Long(table.Rows[listIndicesSelected[i]]["PayPlanChargeNum"].ToString());
					long procNum=PIn.Long(table.Rows[listIndicesSelected[i]]["ProcNum"].ToString());
					//Add each selected proc to the list
					if(procNum > 0) {
						listAccountEntries.Add(new AccountEntry(Procedures.GetOneProc(procNum,false)));
					}
					//Add selected positive pay plan debit to the list. Important to check for chargesDouble because there can be negative debits.
					if(CompareDecimal.IsGreaterThanZero(chargesDouble) && payPlanChargeNum > 0) {
						listAccountEntries.Add(new AccountEntry(PayPlanCharges.GetOne(payPlanChargeNum)));
					}
					if(adjNum > 0) {
						Adjustment adjustment=Adjustments.GetOne(adjNum);
						//Don't include negative adjustments or ones attached to procs because of the way we pay off procs.
						if(adjustment.AdjAmt>0 && adjustment.ProcNum==0) {
							listAccountEntries.Add(new AccountEntry(adjustment));
						}
					}
				}
			}
			double unearnedAmt=PIn.Double(labelUnearnedAmt.Text);
			//Don't allow the user to allocate negative unearned which is a problem that needs to be handled with a real income transfer.
			if(isPrePay && CompareDecimal.IsGreaterThanZero(unearnedAmt)) {
				if(listAccountEntries.Count<1) {
					using FormProcSelect formProcSelect=new FormProcSelect(_patient.PatNum,false,true,doShowAdjustments:true,doShowTreatmentPlanProcs:false);
					if(formProcSelect.ShowDialog()!=DialogResult.OK) {
						return;
					}
					listAccountEntries=formProcSelect.ListAccountEntries;
				}
				formPayment.UnearnedAmt=unearnedAmt;
			}
			formPayment.ListAccountEntriesPayFirst=listAccountEntries;
			if(payment.PayDate.Date>DateTime.Today.Date && !PrefC.GetBool(PrefName.FutureTransDatesAllowed) && !PrefC.GetBool(PrefName.AccountAllowFutureDebits)) {
				MsgBox.Show(this,"Payments cannot be in the future.");
				return;
			}
			payment.PayAmt=payAmt;
			Payments.Insert(payment);
			formPayment.ShowDialog();
			//If this is a payment received from Transworld, we don't want to send any new update messages to Transworld for any splits on this payment.
			//To prevent new msgs from being sent, we will insert TsiTransLogs linked to all splits with TsiTransType.None.  The ODService will update the
			//log TransAmt for any edits to this paysplit instead of sending a new msg to Transworld.
			if(!isTsiPayment) {
				ModuleSelected(_patient.PatNum);
				return;
			}
			Payment paymentTsi=Payments.GetPayment(payment.PayNum);
			if(paymentTsi!=null) {
				List<PaySplit> listPaySplits=PaySplits.GetForPayment(paymentTsi.PayNum);
				if(listPaySplits.Count>0) {
					PatAging patAging=Patients.GetAgingListFromGuarNums(new List<long>() { _patient.Guarantor }).FirstOrDefault();
					List<TsiTransLog> listTsiTransLogs=new List<TsiTransLog>();
					for(int i=0;i<listPaySplits.Count();i++) {
						double logAmt=patAging.ListTsiLogs.FindAll(x => x.FKeyType==TsiFKeyType.PaySplit && x.FKey==listPaySplits[i].SplitNum).Sum(x => x.TransAmt);
						if(CompareDouble.IsEqual(listPaySplits[i].SplitAmt,logAmt)) {
							continue;//split already linked to logs that sum to the split amount, nothing to do with this one
						}
						listTsiTransLogs.Add(new TsiTransLog() {
							PatNum=patAging.PatNum,//this is the account guarantor, since these are reconciled by guars
							UserNum=Security.CurUser.UserNum,
							TransType=TsiTransType.None,
							//TransDateTime=DateTime.Now,//set on insert, not editable by user
							//DemandType=TsiDemandType.Accelerator,//only valid for placement msgs
							//ServiceCode=TsiServiceCode.Diplomatic,//only valid for placement msgs
							ClientId=patAging.ListTsiLogs.FirstOrDefault()?.ClientId??"",//can be blank, not used since this isn't really sent to Transworld
							TransAmt=-listPaySplits[i].SplitAmt-logAmt,//Ex. already logged -10; split changed to -20; -20-(-10)=-10; -10 this split + -10 already logged = -20 split amt
							AccountBalance=patAging.AmountDue-listPaySplits[i].SplitAmt-logAmt,
							FKeyType=TsiFKeyType.PaySplit,
							FKey=listPaySplits[i].SplitNum,
							RawMsgText="This was not a message sent to Transworld.  This paysplit was entered due to a payment received from Transworld.",
							ClinicNum=(PrefC.HasClinicsEnabled?patAging.ClinicNum:0)
							//,TransJson=""//only valid for placement msgs
						});
					}
					if(listTsiTransLogs.Count>0) {
						TsiTransLogs.InsertMany(listTsiTransLogs);
					}
				}
			}
			ModuleSelected(_patient.PatNum);
		}

		private void toolBarButQuickProcs_Click() {
			if(!Security.IsAuthorized(EnumPermType.AccountProcsQuickAdd)) {
				return;
			}
			//Main QuickCharge button was clicked.  Create a textbox that can be entered so users can insert manually entered proc codes.
			if(!Security.IsAuthorized(EnumPermType.ProcComplCreate,true)) {//Button doesn't show up unless they have AccountQuickCharge permission. 
				//user can still use dropdown, just not type in codes.
				contextMenuQuickProcs.Show(this,new Point(_butQuickProcs.Bounds.X,_butQuickProcs.Bounds.Y+_butQuickProcs.Bounds.Height));
				return; 
			}
			textQuickProcs.SetBounds(_butQuickProcs.Bounds.X+1,_butQuickProcs.Bounds.Y+2,_butQuickProcs.Bounds.Width-17,_butQuickProcs.Bounds.Height-2);
			textQuickProcs.Visible=true;
			textQuickProcs.BringToFront();
			textQuickProcs.Focus();
			textQuickProcs.Capture=true;
		}

		private void toolBarButRepeatCharge_Click() {
			RepeatCharge repeatCharge=new RepeatCharge();
			repeatCharge.PatNum=_patient.PatNum;
			repeatCharge.DateStart=DateTime.Today;
			using FormRepeatChargeEdit formRepeatChargeEdit=new FormRepeatChargeEdit(repeatCharge);
			formRepeatChargeEdit.IsNew=true;
			formRepeatChargeEdit.ShowDialog();
			ModuleSelected(_patient.PatNum);
		}

		private void toolBarButStatement_Click() {
			Statement statement=new Statement();
			statement.PatNum=_patient.Guarantor;
			statement.DateSent=DateTime.Today;
			statement.IsSent=true;
			statement.Mode_=StatementMode.InPerson;
			statement.HidePayment=false;
			statement.SinglePatient=false;
			statement.Intermingled=PrefC.GetBool(PrefName.IntermingleFamilyDefault);
			statement.StatementType=StmtType.NotSet;
			statement.DateRangeTo=DateTime.Today;//This is needed for payment plan accuracy.//new DateTime(2200,1,1);
			if(textDateEnd.IsValid() && textDateEnd.Text!="") {
				statement.DateRangeTo=PIn.Date(textDateEnd.Text);
			}
			statement.DateRangeFrom=DateTime.MinValue;
			if(textDateStart.IsValid() && textDateStart.Text!="") {//textDateStart has ultimate precedence. User may have intentionally set the date range for statement.
				statement.DateRangeFrom=PIn.Date(textDateStart.Text);
			}
			else {//Use preferences to determine the "from" date.
				long billingDefaultsLastDaysPref=PrefC.GetLong(PrefName.BillingDefaultsLastDays);
				if(billingDefaultsLastDaysPref > 0) {//0 days means ignore preference and show everything.
					statement.DateRangeFrom=DateTime.Today.AddDays(-billingDefaultsLastDaysPref);
				}
				if(PrefC.GetBool(PrefName.BillingShowTransSinceBalZero)) {
					Patient patient=Patients.GetPat(statement.PatNum);
					List<PatAging> listPatAgings=Patients.GetAgingListSimple(new List<long> {}, new List<long> { patient.Guarantor },true);
					DataTable tableBals=Ledgers.GetDateBalanceBegan(listPatAgings,isSuperBills:false);//More Options selection has a super family option. We would need new checkbox here.
					if(tableBals.Rows.Count > 0) {
						DateTime dateTimeFrom=PIn.Date(tableBals.Rows[0]["DateZeroBal"].ToString());
						if(dateTimeFrom > statement.DateRangeFrom) {//Zero balance date range has precedence if it's more recent than billing default date range.
							statement.DateRangeFrom=dateTimeFrom;
						}
					}
				}
			}
			statement.Note="";
			statement.NoteBold="";
			Patient patientGuarantor=null;
			if(_patient!=null) {
				patientGuarantor=Patients.GetPat(_patient.Guarantor);
			}
			if(patientGuarantor!=null) {
				statement.IsBalValid=true;
				statement.BalTotal=patientGuarantor.BalTotal;
				statement.InsEst=patientGuarantor.InsEst;
			}
			PrintStatement(statement);
			ModuleSelected(_patient.PatNum);
		}
		#endregion Methods - Private ToolBar

		#region Methods - Private Refresh
		///<summary></summary>
		private void RefreshModuleData(long patNum,bool isSelectingFamily) {
			UpdateUrgFinNote();
			UpdateFinNote();
			if(patNum==0){
				_patient=null;
				_family=null;
				_dataSetMain=null;
				_listPaySplitsHidden.Clear();
				Plugins.HookAddCode(this,"ContrAccount.RefreshModuleData_null");
				return;
			}
			DateTime dateFrom=DateTime.MinValue;
			DateTime dateTo=DateTime.MaxValue;
			if(textDateStart.IsValid() && textDateEnd.IsValid()) {
				if(textDateStart.Text!="") {
					dateFrom=PIn.Date(textDateStart.Text);
				}
				if(textDateEnd.Text!="") {
					dateTo=PIn.Date(textDateEnd.Text);
				}
			}
			bool doMakeSecLog=false;
			if(_patNumLast!=patNum) {
				doMakeSecLog=true;
				_patNumLast=patNum;
			}
			bool doGetAutoOrtho=PrefC.GetBool(PrefName.OrthoEnabled);
			Action action=()=> _loadData=AccountModules.GetAll(patNum,dateFrom,dateTo,isSelectingFamily,checkShowDetail.Checked,true,true,doMakeSecLog,doGetAutoOrtho);
			try {
				Logger.LogAction("Patients.GetFamily",LogPath.AccountModule,action);
			}
			catch(ApplicationException ex) {
				if(ex.Message=="Missing codenum") {
					MsgBox.Show(this,$"Missing codenum. Please run database maintenance method {nameof(DatabaseMaintenances.ProcedurelogCodeNumInvalid)}.");
					_patient=null;
					_dataSetMain=null;
					return;
				}
				throw;
			}
			lock(_lockDataSetMain) {
				_dataSetMain=_loadData.DataSetMain;
			}
			_family=_loadData.Fam;
			_patient=_family.GetPatient(patNum);
			_patientNote=_loadData.PatNote;
			_patFieldArray=_loadData.ArrPatFields;
			_listPaySplitsHidden=_loadData.ListUnearnedSplits
				.FindAll(x => PaySplits.GetHiddenUnearnedDefNums().Contains(x.UnearnedType) && _family.GetPatNums().Contains(x.PatNum));//Don't show out of family pay splits for the payer.
			FillSummary();
			SetDiscountPlanOrInsurancePlanDash();
			Plugins.HookAddCode(this,"ContrAccount.RefreshModuleData_end",_family,_patient,_dataSetMain,_patientPortionBalanceTotal,isSelectingFamily);
		}

		private void RefreshModuleScreen(bool isSelectingFamily) {
			if(_patient==null){
				tabControlAccount.Enabled=false;
				ToolBarMain.Buttons["Payment"].Enabled=false;
				ToolBarMain.Buttons["Adjustment"].Enabled=false;
				ToolBarMain.Buttons["Insurance"].Enabled=false;
				ToolBarMain.Buttons["PayPlan"].Enabled=false;
				ToolBarMain.Buttons["InstallPlan"].Enabled=false;
				if(ToolBarMain.Buttons["QuickProcs"]!=null) {
					ToolBarMain.Buttons["QuickProcs"].Enabled=false;
				}
				if(ToolBarMain.Buttons["RepeatCharge"]!=null) {
					ToolBarMain.Buttons["RepeatCharge"].Enabled=false;
				}
				ToolBarMain.Buttons["Statement"].Enabled=false;
				ToolBarMain.Invalidate();
				textUrgFinNote.Enabled=false;
				textFinNote.Enabled=false;
				//butComm.Enabled=false;
				tabControlShow.Enabled=false;
				Plugins.HookAddCode(this,"ContrAccount.RefreshModuleScreen_null");
			}
			else{
				tabControlAccount.Enabled=true;
				ToolBarMain.Buttons["Payment"].Enabled=true;
				ToolBarMain.Buttons["Adjustment"].Enabled=true;
				ToolBarMain.Buttons["Insurance"].Enabled=true;
				ToolBarMain.Buttons["PayPlan"].Enabled=true;
				ToolBarMain.Buttons["InstallPlan"].Enabled=true;
				if(ToolBarMain.Buttons["QuickProcs"]!=null) {
					ToolBarMain.Buttons["QuickProcs"].Enabled=true;
				}
				if(ToolBarMain.Buttons["RepeatCharge"]!=null) {
					ToolBarMain.Buttons["RepeatCharge"].Enabled=true;
				} 
				ToolBarMain.Buttons["Statement"].Enabled=true;
				ToolBarMain.Invalidate();
				textUrgFinNote.Enabled=true;
				textFinNote.Enabled=true;
				//butComm.Enabled=true;
				tabControlShow.Enabled=true;
			}
			Logger.LogAction("FillPats",LogPath.AccountModule,() => FillPats(isSelectingFamily));
			Logger.LogAction("FillMisc",LogPath.AccountModule,() => FillMisc());
			Logger.LogAction("FillAging",LogPath.AccountModule,() => FillAging(isSelectingFamily));
			//must be in this order.
			Logger.LogAction("FillRepeatCharges",LogPath.AccountModule,() => FillRepeatCharges());//1
			Logger.LogAction("FillPaymentPlans",LogPath.AccountModule,() => FillPaymentPlans());//2
			if(PrefC.GetBool(PrefName.OrthoEnabled)){
				FillAutoOrtho(false);
			}
			if(OrthoCases.HasOrthoCasesEnabled()) {
				FillOrthoCasesGrid();
			}
			Logger.LogAction("FillPatInfo",LogPath.AccountModule,() => FillPatInfo());
			FillTpUnearned();
			LayoutPanelsAndRefreshMainGrids(true);
			Plugins.HookAddCode(this,"ContrAccount.RefreshModuleScreen_end",_family,_patient,_dataSetMain,_patientPortionBalanceTotal,isSelectingFamily);
		}

		///<summary>Sets the visibility of the DPlan Rem or Ins Rem labels</summary>
		private void SetDiscountPlanOrInsurancePlanDash() {
			if(_loadData.DiscountPlanSub==null || _patient==null) {
				labelInsRem.Visible=true;
				labelInsRem.Enabled=true;
				labelDisRem.Visible=false;
				labelDisRem.Enabled=false;
				return;
			} 
			groupBoxIndDis.RefreshDiscountPlan(_patient,_loadData.DiscountPlanSub,_loadData.DiscountPlan);
			labelDisRem.Visible=true;
			labelDisRem.Enabled=true;
			labelInsRem.Visible=false;
			labelInsRem.Enabled=false;
		}

		private void RefreshOrthoCasesGridRows() {
			gridOrthoCases.BeginUpdate();
			gridOrthoCases.ListGridRows.Clear();
			if(IsFamilySelected()) {
				gridOrthoCases.EndUpdate();
				return;
			}
			GridRow row;
			if(_patient!=null) {
				_listOrthoCases=OrthoCases.Refresh(_patient.PatNum);
			}
			List<OrthoProcLink> listOrthoProcLinksForPat=OrthoProcLinks.GetManyByOrthoCases(_listOrthoCases.Select(x => x.OrthoCaseNum).ToList());
			List<Procedure> listProceduresLinkedForPat=Procedures.GetManyProc(listOrthoProcLinksForPat.Select(x => x.ProcNum).ToList(),false);
			butAddOrthoCase.Enabled=true;
			for(int i=0;i<_listOrthoCases.Count();i++) {
				//Skip the orthocase if it is inactive and we are not showing inactive orthocases
				if(checkHideInactiveOrthoCases.Checked && !_listOrthoCases[i].IsActive) {
					continue;
				}
				row=new GridRow();
				if(_listOrthoCases[i].IsActive) {
					row.Cells.Add("X");
					butAddOrthoCase.Enabled=false;//Can only have one active OrthoCase, se we deactivate the button to add a new active OrthoCase.
				}
				else {
					row.Cells.Add("");
				}
				if(_listOrthoCases[i].IsTransfer) {
					row.Cells.Add("X");
					row.Cells.Add(_listOrthoCases[i].BandingDate.ToShortDateString());
				}
				else {
					row.Cells.Add("");
					OrthoProcLink orthoProcLinkBanding=listOrthoProcLinksForPat.Find(x=>x.ProcLinkType==OrthoProcType.Banding && x.OrthoCaseNum==_listOrthoCases[i].OrthoCaseNum);
					if(orthoProcLinkBanding is null) {
						row.Cells.Add(Lans.g("TableOrthoCases","Banding Not Scheduled"));
					}
					else {
						Procedure procedureBanding=listProceduresLinkedForPat.Find(x=>x.ProcNum==orthoProcLinkBanding.ProcNum);
						if(procedureBanding is null) {
							row.Cells.Add(Lans.g("TableOrthoCases","Banding Not Scheduled"));
						}
						else if(procedureBanding.ProcStatus==ProcStat.C) {
							row.Cells.Add(procedureBanding.ProcDate.ToShortDateString());
						}
						else if(procedureBanding.ProcStatus==ProcStat.TP && procedureBanding.AptNum!=0) {
							row.Cells.Add(procedureBanding.ProcDate.ToShortDateString());
						}
						else {
							row.Cells.Add(Lans.g("TableOrthoCases","Banding Not Scheduled"));
						}
					}
				}
				OrthoProcLink orthoProcLinkDebond=listOrthoProcLinksForPat.Find(x=>x.ProcLinkType==OrthoProcType.Debond && x.OrthoCaseNum==_listOrthoCases[i].OrthoCaseNum);
				if(orthoProcLinkDebond is null) {
					row.Cells.Add(Lan.g("TableOrthoCases","Debond Incomplete"));
				}
				else{
					Procedure procedureDebond=listProceduresLinkedForPat.Find(x=>x.ProcNum==orthoProcLinkDebond.ProcNum);
					if(procedureDebond is null) {
						row.Cells.Add(Lan.g("TableOrthoCases","Debond Incomplete"));
					}
					else if(procedureDebond.ProcStatus==ProcStat.C) {
						row.Cells.Add(procedureDebond.ProcDate.ToShortDateString());
					}
					else {
						row.Cells.Add(Lan.g("TableOrthoCases","Debond Incomplete"));
					}
				}
				row.Tag=_listOrthoCases[i];
				gridOrthoCases.ListGridRows.Add(row);
			}
			gridOrthoCases.EndUpdate();
		}
		#endregion Methods - Private Refresh

		#region Methods - Private Fill
		private void FillAging(bool isSelectingFamily) {
			if(Plugins.HookMethod(this,"ContrAccount.FillAging",_family,_patient,_dataSetMain,isSelectingFamily)) {
				return;
			}
			if(_patient==null) {
				textOver90.Text="";
				text61_90.Text="";
				text31_60.Text="";
				text0_30.Text="";
				labelTotalAmt.Text="";
				labelInsEstAmt.Text="";
				labelBalanceAmt.Text="";
				labelPatEstBalAmt.Text="";
				labelUnearnedAmt.Text="";
				//labelInsLeftAmt.Text="";
				return;
			}
			textOver90.Text=_family.ListPats[0].BalOver90.ToString("F");
			text61_90.Text=_family.ListPats[0].Bal_61_90.ToString("F");
			text31_60.Text=_family.ListPats[0].Bal_31_60.ToString("F");
			text0_30.Text=_family.ListPats[0].Bal_0_30.ToString("F");
			decimal total=(decimal)_family.ListPats[0].BalTotal;
			List<long> listDefNumsTpUnearned=Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType)
				.FindAll(x => x.ItemValue!="")
				.Select(x => x.DefNum)
				.ToList();
			labelTotalAmt.Text=total.ToString("F");
			labelInsEstAmt.Text=_family.ListPats[0].InsEst.ToString("F");
			labelBalanceAmt.Text=(total - (decimal)_family.ListPats[0].InsEst).ToString("F");
			labelPatEstBalAmt.Text="";
			DataTable tableMisc=_dataSetMain.Tables["misc"];
			if(!isSelectingFamily) {
				for(int i=0;i<tableMisc.Rows.Count;i++){
					if(tableMisc.Rows[i]["descript"].ToString()=="patInsEst"){
						decimal estBal=(decimal)_patient.EstBalance-PIn.Decimal(tableMisc.Rows[i]["value"].ToString());
						labelPatEstBalAmt.Text=estBal.ToString("F");
					}
				}
			}
			labelUnearnedAmt.Text="";
			for(int i=0;i<tableMisc.Rows.Count;i++){
				if(tableMisc.Rows[i]["descript"].ToString()=="unearnedIncome") {
					//remove TP splits that do not show on account due to def being checked. 
					List<PaySplit> listUnearnedShownOnAccount=_loadData.ListUnearnedSplits.FindAll(x => !listDefNumsTpUnearned.Contains(x.UnearnedType) 
						&& _family.ListPats.Select(y => y.PatNum).Contains(x.PatNum));//We do not want to show unearned balances for paysplits to other families
					labelUnearnedAmt.Text=listUnearnedShownOnAccount.Sum(x => x.SplitAmt).ToString("F");
					if(PIn.Double(labelUnearnedAmt.Text)<=0) {
						labelUnearnedAmt.ForeColor=Color.Black;
						labelUnearnedAmt.Font=new Font(labelUnearnedAmt.Font,FontStyle.Regular);
					}
					else {
						labelUnearnedAmt.ForeColor=Color.Firebrick;
						labelUnearnedAmt.Font=new Font(labelUnearnedAmt.Font,FontStyle.Bold);
					}
					FillUnearnedBreakDown(listUnearnedShownOnAccount);
				}
			}
			//labelInsLeft.Text=Lan.g(this,"Ins Left");
			//labelInsLeftAmt.Text="";//etc. Will be same for everyone
			Font fontBold=new Font(FontFamily.GenericSansSerif,11,FontStyle.Bold);
			//In the new way of doing it, they are all visible and calculated identically,
			//but the emphasis simply changes by slight renaming of labels
			//and by font size changes.
			if(PrefC.GetBool(PrefName.BalancesDontSubtractIns)){
				labelTotal.Text=Lan.g(this,"Balance");
				labelTotalAmt.Font=fontBold;
				labelTotalAmt.ForeColor=Color.Firebrick;
				panelAgeLine.Visible=true;//verical line
				labelInsEst.Text=Lan.g(this,"Ins Pending");
				labelBalance.Text=Lan.g(this,"After Ins");
				labelBalanceAmt.Font=this.Font;
				labelBalanceAmt.ForeColor=Color.Black;
				return;
			}
			//this is more common
			labelTotal.Text=Lan.g(this,"Total");
			labelTotalAmt.Font=this.Font;
			labelTotalAmt.ForeColor = Color.Black;
			panelAgeLine.Visible=false;
			labelInsEst.Text=Lan.g(this,"-InsEst");
			labelBalance.Text=Lan.g(this,"=Est Bal");
			labelBalanceAmt.Font=fontBold;
			labelBalanceAmt.ForeColor=Color.Firebrick;
		}

		private void FillUnearnedBreakDown(List<PaySplit> listPaySplitsOnAccount) {
			List<Def> listUnearnedDefs=Defs.GetUnearnedDefs();
			//Display every unearned type along with the total of payment splits that are within said unearned bucket.
			gridUnearnedBreakdown.BeginUpdate();
			gridUnearnedBreakdown.Columns.Clear();
			gridUnearnedBreakdown.Columns.Add(new GridColumn(Lan.g(this,"Type"),113));
			gridUnearnedBreakdown.Columns.Add(new GridColumn(Lan.g(this,"Amount"),80,HorizontalAlignment.Right));
			gridUnearnedBreakdown.ListGridRows.Clear();
			for(int i=0;i<listUnearnedDefs.Count;i++) {
				if(!listPaySplitsOnAccount.Exists(x=>x.UnearnedType==listUnearnedDefs[i].DefNum)) {
					continue;
				}
				GridRow row=new GridRow();
				row.Cells.Add(listUnearnedDefs[i].ItemName);
				double sumForType=listPaySplitsOnAccount.FindAll(x=>x.UnearnedType==listUnearnedDefs[i].DefNum).Sum(x=>x.SplitAmt);
				row.Cells.Add(sumForType.ToString("N2"));
				gridUnearnedBreakdown.ListGridRows.Add(row);
			}
			gridUnearnedBreakdown.EndUpdate();
			LayoutManager.MoveSize(gridUnearnedBreakdown,new Size(gridUnearnedBreakdown.Columns.Sum(x => x.ColWidth),
				gridUnearnedBreakdown.ListGridRows.Sum(x => x.State.HeightTotal)+18));//+18 for header height 15 plus 3 extra pixels for line spacing
			LayoutManager.MoveLocation(gridUnearnedBreakdown,new Point(groupBoxFamilyIns.Left-1,groupBoxFamilyIns.Top-1));
			gridUnearnedBreakdown.BringToFront();
		}

		private void FillAutoOrtho(bool doCalculateFirstDate=true) {
			if(_patient==null) {
				return;
			}
			gridAutoOrtho.BeginUpdate();
			gridAutoOrtho.Columns.Clear();
			gridAutoOrtho.Columns.Add(new GridColumn("",(gridAutoOrtho.Width/2)-20));//,HorizontalAlignment.Right));
			gridAutoOrtho.Columns.Add(new GridColumn("",(gridAutoOrtho.Width/2)+20));
			gridAutoOrtho.ListGridRows.Clear();
			GridRow row=new GridRow();
			//Insurance Information
			//PriClaimType
			List<PatPlan> listPatPlans=_loadData.ListPatPlans;
			if(listPatPlans.Count==0) {
				row=new GridRow();
				row.Cells.Add("");
				row.Cells.Add(Lan.g(this,"Patient has no insurance."));
				gridAutoOrtho.ListGridRows.Add(row);
			}
			else {
				List<Def> listDefs=Defs.GetDefsForCategory(DefCat.MiscColors);
				for(int i=0;i<listPatPlans.Count;i++) {
					PatPlan patPlan=listPatPlans[i];
					InsSub insSub=InsSubs.GetSub(patPlan.InsSubNum,_loadData.ListInsSubs);
					InsPlan insPlan=InsPlans.GetPlan(insSub.PlanNum,_loadData.ListInsPlans);
					string carrierName=Carriers.GetCarrier(insPlan.CarrierNum).CarrierName;
					string subIDCur=insSub.SubscriberID;
					row=new GridRow();
					AutoOrthoPat autoOrthoPat=new AutoOrthoPat() {
						InsPlan_=insPlan,
						PatPlan_=patPlan,
						CarrierName=carrierName,
						DefaultFee=insPlan.OrthoAutoFeeBilled,
						SubID=subIDCur
					};
					if(i==listPatPlans.Count-1) { //last row in the insurance info section
						row.ColorLborder=Color.Black;
					}
					row.ColorBackG=listDefs[(int)DefCatMiscColors.FamilyModuleCoverage].ItemColor; //same logic as family module insurance colors.
					switch(i) {
						case 0: //primary
							row.Cells.Add(Lan.g(this,"Primary Ins"));
							break;
						case 1: //secondary
							row.Cells.Add(Lan.g(this,"Secondary Ins"));
							break;
						case 2: //tertiary
							row.Cells.Add(Lan.g(this,"Tertiary Ins"));
							break;
						default: //other
							row.Cells.Add(Lan.g(this,"Other Ins"));
							break;
					}
					row.Cells.Add("");
					row.Bold=true;
					row.Tag=autoOrthoPat;
					gridAutoOrtho.ListGridRows.Add(row);
					//claimtype
					row=new GridRow();
					row.Cells.Add(Lan.g(this,"ClaimType"));
					if(insPlan==null) {
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(insPlan.OrthoType.ToString());
					}
					row.Tag=autoOrthoPat;
					gridAutoOrtho.ListGridRows.Add(row);
					//Only show for initialPlusPeriodic claimtype.
					if(insPlan.OrthoType==OrthoClaimType.InitialPlusPeriodic) {
						//Frequency
						row= new GridRow();
						row.Cells.Add(Lan.g(this,"Frequency"));
						row.Cells.Add(insPlan.OrthoAutoProcFreq.ToString());
						row.Tag=autoOrthoPat;
						gridAutoOrtho.ListGridRows.Add(row);
						//Fee
						row= new GridRow();
						row.Cells.Add(Lan.g(this,"FeeBilled"));
						row.Cells.Add(patPlan.OrthoAutoFeeBilledOverride==-1 ? POut.Double(insPlan.OrthoAutoFeeBilled) : POut.Double(patPlan.OrthoAutoFeeBilledOverride));
						row.Tag=autoOrthoPat;
						gridAutoOrtho.ListGridRows.Add(row);
					}
					//Last Claim Date
					row= new GridRow();
					DateTime dateLast;
					if(!_loadData.DictDateLastOrthoClaims.TryGetValue(patPlan.PatPlanNum,out dateLast)) {
						dateLast=Claims.GetDateLastOrthoClaim(patPlan,insPlan.OrthoType);
					}
					row.Cells.Add(Lan.g(this,"LastClaim"));
					row.Cells.Add(dateLast==null || dateLast.Date == DateTime.MinValue.Date ? Lan.g(this,"None Sent") : dateLast.ToShortDateString());
					row.Tag=autoOrthoPat;
					gridAutoOrtho.ListGridRows.Add(row);
					//NextClaimDate - Only show for initialPlusPeriodic claimtype.
					if(insPlan.OrthoType==OrthoClaimType.InitialPlusPeriodic) {
						row=new GridRow();
						row.Cells.Add(Lan.g(this,"NextClaim"));
						row.Cells.Add(patPlan.OrthoAutoNextClaimDate.Date == DateTime.MinValue.Date ? Lan.g(this,"Stopped") : patPlan.OrthoAutoNextClaimDate.ToShortDateString());
						row.Tag=autoOrthoPat;
						gridAutoOrtho.ListGridRows.Add(row);
					}
				}
			}
			//Pat Ortho Info Title
			row= new GridRow();
			row.Cells.Add(Lan.g(this,"Pat Ortho Info"));
			row.Cells.Add("");
			row.ColorBackG=Color.LightCyan;
			row.Bold=true;
			row.ColorLborder=Color.Black;
			gridAutoOrtho.ListGridRows.Add(row);
			//OrthoAutoProc Freq
			if(doCalculateFirstDate) {
				_loadData.FirstOrthoProcDate=Procedures.GetFirstOrthoProcDate(_patientNote);
			}
			DateTime dateFirstOrthoProc=_loadData.FirstOrthoProcDate;
			if(dateFirstOrthoProc==DateTime.MinValue) { //no ortho procedures charted for this patient.
				row=new GridRow();
				row.Cells.Add(""); 
				row.Cells.Add(Lan.g(this,"No ortho procedures charted."));
				gridAutoOrtho.ListGridRows.Add(row);
				gridAutoOrtho.EndUpdate();
				return;
			}
			int txMonthsTotal=(_patientNote.OrthoMonthsTreatOverride==-1?PrefC.GetByte(PrefName.OrthoDefaultMonthsTreat):_patientNote.OrthoMonthsTreatOverride);
			row=new GridRow();
			row.Cells.Add(Lan.g(this,"Total Tx Time")); //Number of Years/Months/Days since the first ortho procedure on this account
			DateSpan dateSpan=new DateSpan(dateFirstOrthoProc,DateTime.Today);
			int txTimeInMonths=(dateSpan.YearsDiff*12)+dateSpan.MonthsDiff+(dateSpan.DaysDiff<15?0:1);
			if(txTimeInMonths>txMonthsTotal && PrefC.GetBool(PrefName.OrthoDebondProcCompletedSetsMonthsTreat)) { //Capping if preference is set
				dateSpan=new DateSpan(dateFirstOrthoProc,dateFirstOrthoProc.AddMonths(txMonthsTotal));
				txTimeInMonths=(dateSpan.YearsDiff*12)+dateSpan.MonthsDiff+(dateSpan.DaysDiff<15?0:1);
			}
			string strDateDiff="";
			if(dateSpan.YearsDiff!=0) {
				strDateDiff+=dateSpan.YearsDiff+" "+Lan.g(this,"year"+(dateSpan.YearsDiff==1 ? "" : "s"));
			}
			if(dateSpan.MonthsDiff!=0) {
				if(strDateDiff!="") {
					strDateDiff+=", ";
				}
				strDateDiff+=dateSpan.MonthsDiff+" "+Lan.g(this,"month"+(dateSpan.MonthsDiff==1 ? "" : "s"));
			}
			if(dateSpan.DaysDiff!=0 || strDateDiff=="") {
				if(strDateDiff!="") {
					strDateDiff+=", ";
				}
				strDateDiff+=dateSpan.DaysDiff+" "+Lan.g(this,"day"+(dateSpan.DaysDiff==1 ? "" : "s"));
			}
			row.Cells.Add(strDateDiff);
			gridAutoOrtho.ListGridRows.Add(row);
			//Date Start
			row=new GridRow();
			row.Cells.Add(Lan.g(this,"Date Start")); //Date of the first ortho procedure on this account
			row.Cells.Add(dateFirstOrthoProc.ToShortDateString());
			gridAutoOrtho.ListGridRows.Add(row);
			//Tx Months Total
			row=new GridRow();
			row.Cells.Add(Lan.g(this,"Tx Months Total")); //this patient's OrthoClaimMonthsTreatment, or the practice default if 0.
			row.Cells.Add(txMonthsTotal.ToString());
			gridAutoOrtho.ListGridRows.Add(row);
			//Months in treatment
			row=new GridRow();
			row.Cells.Add(Lan.g(this,"Months in Treatment"));
			row.Cells.Add(txTimeInMonths.ToString());
			gridAutoOrtho.ListGridRows.Add(row);
			//Months Rem
			row=new GridRow();
			row.Cells.Add(Lan.g(this,"Months Rem")); //Months Total - Total Tx Time
			row.Cells.Add(Math.Max(0,txMonthsTotal-txTimeInMonths).ToString());
			gridAutoOrtho.ListGridRows.Add(row);
			gridAutoOrtho.EndUpdate();
		}

		/// <summary>Fills the commlog grid on this form.  It does not refresh the data from the database.</summary>
		private void FillComm() {
			if(_dataSetMain==null) {
				gridComm.BeginUpdate();
				gridComm.ListGridRows.Clear();
				gridComm.EndUpdate();
				return;
			}
			gridComm.BeginUpdate();
			gridComm.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableCommLogAccount","Date"),70);
			gridComm.Columns.Add(col);
			col=new GridColumn(Lan.g("TableCommLogAccount","Time"),42);//,HorizontalAlignment.Right);
			gridComm.Columns.Add(col);
			col=new GridColumn(Lan.g("TableCommLogAccount","Name"),80);
			gridComm.Columns.Add(col);
			col=new GridColumn(Lan.g("TableCommLogAccount","Type"),80);
			gridComm.Columns.Add(col);
			col=new GridColumn(Lan.g("TableCommLogAccount","Mode"),55);
			gridComm.Columns.Add(col);
			//col=new ODGridColumn(Lan.g("TableCommLogAccount","Sent/Recd"),75);
			//gridComm.Columns.Add(col);
			col=new GridColumn(Lan.g("TableCommLogAccount","Note"),455);
			gridComm.Columns.Add(col);
			gridComm.ListGridRows.Clear();
			GridRow row;
			bool isCommlogAutomated;
			DataTable table=_dataSetMain.Tables["Commlog"];
			for(int i=0;i<table.Rows.Count;i++) {
				isCommlogAutomated=Commlogs.IsAutomated(table.Rows[i]["commType"].ToString(),
					PIn.Enum<CommItemSource>(table.Rows[i]["CommSource"].ToString()));
				//Skip commlog entries which are automated per user option.
				if(!this.checkShowCommAuto.Checked && isCommlogAutomated) {
					continue;
				}
				//Skip commlog entries which belong to other family members per user option.
				if(!this.checkShowFamilyComm.Checked//show family not checked
					&& !IsFamilySelected()//family not selected
					&& table.Rows[i]["PatNum"].ToString()!=_patient.PatNum.ToString()//not this patient
					&& table.Rows[i]["FormPatNum"].ToString()=="0")//not a questionnaire (FormPat)
				{
					continue;
				}
				else if(table.Rows[i]["EmailMessageNum"].ToString()!="0") {//if this is an Email
					if(((HideInFlags)PIn.Int(table.Rows[i]["EmailMessageHideIn"].ToString())).HasFlag(HideInFlags.AccountCommLog)) {
						continue;
					}
				}
				row=new GridRow();
				int argbColorValue=PIn.Int(table.Rows[i]["colorText"].ToString());//Convert to int. If blank or 0, will use default color.
				if(argbColorValue!=Color.Empty.ToArgb()) {//A color was set for this commlog type
					row.ColorText=Color.FromArgb(argbColorValue);
				}
				row.Cells.Add(table.Rows[i]["commDate"].ToString());
				row.Cells.Add(table.Rows[i]["commTime"].ToString());
				if(IsFamilySelected()) {
					row.Cells.Add(table.Rows[i]["patName"].ToString());
				}
				else {//one patient
					if(table.Rows[i]["PatNum"].ToString()==_patient.PatNum.ToString()) {//if this patient
						row.Cells.Add("");
					}
					else {//other patient
						row.Cells.Add(table.Rows[i]["patName"].ToString());
					}
				}
				row.Cells.Add(table.Rows[i]["commName"].ToString());
				row.Cells.Add(table.Rows[i]["mode"].ToString());
				//row.Cells.Add(table.Rows[i]["sentOrReceived"].ToString());
				if(isCommlogAutomated) { //If it's an automated commlog, show only the first line.
					row.Cells.Add(Commlogs.GetNoteFirstLine(table.Rows[i]["Note"].ToString()));
				}
				else {
					row.Cells.Add(table.Rows[i]["Note"].ToString());
				}
				row.Tag=i;
				gridComm.ListGridRows.Add(row);
			}
			gridComm.EndUpdate();
			gridComm.ScrollToEnd();
		}

		private void FillMain() {
			gridAccount.BeginUpdate();
			gridAccount.Columns.Clear();
			GridColumn col;
			_listDisplayFieldsForMainGrid=DisplayFields.GetForCategory(DisplayFieldCategory.AccountModule);
			if(!PrefC.HasClinicsEnabled) {
				//remove clinics from displayfields if clinics are disabled
				_listDisplayFieldsForMainGrid.RemoveAll(x => x.InternalName.ToLower().Contains("clinic"));
			}
			HorizontalAlignment horizontalAlignment;
			for(int i=0;i<_listDisplayFieldsForMainGrid.Count;i++) {
				horizontalAlignment=HorizontalAlignment.Left;
				if(_listDisplayFieldsForMainGrid[i].InternalName=="Charges"
					|| _listDisplayFieldsForMainGrid[i].InternalName=="Credits"
					|| _listDisplayFieldsForMainGrid[i].InternalName=="Balance") 
				{
					horizontalAlignment=HorizontalAlignment.Right;
				}
				if(_listDisplayFieldsForMainGrid[i].InternalName=="Signed") {
					horizontalAlignment=HorizontalAlignment.Center;
				}
				if(_listDisplayFieldsForMainGrid[i].Description=="") {
					col=new GridColumn(_listDisplayFieldsForMainGrid[i].InternalName,_listDisplayFieldsForMainGrid[i].ColumnWidth,horizontalAlignment);
				}
				else {
					col=new GridColumn(_listDisplayFieldsForMainGrid[i].Description,_listDisplayFieldsForMainGrid[i].ColumnWidth,horizontalAlignment);
				}
				gridAccount.Columns.Add(col);
			}
			if(gridAccount.Columns.Sum(x => x.ColWidth)>gridAccount.Width) {
				gridAccount.HScrollVisible=true;
			}
			else {
			}
			gridAccount.ListGridRows.Clear();
			GridRow row;
			DataTable table=null;
			if(_patient==null){
				table=new DataTable();
			}
			else{
				table=_dataSetMain.Tables["account"];
			}
			for(int i=0;i<table.Rows.Count;i++) {
				row=new GridRow();
				for(int f=0;f<_listDisplayFieldsForMainGrid.Count;f++) {
					switch(_listDisplayFieldsForMainGrid[f].InternalName) {
						case "Date":
							row.Cells.Add(table.Rows[i]["date"].ToString());
							break;
						case "Patient":
							row.Cells.Add(table.Rows[i]["patient"].ToString());
							break;
						case "Prov":
							row.Cells.Add(table.Rows[i]["prov"].ToString());
							break;
						case "Clinic":
							row.Cells.Add(Clinics.GetAbbr(PIn.Long(table.Rows[i]["ClinicNum"].ToString())));
							break;
						case "ClinicDesc":
							row.Cells.Add(Clinics.GetDesc(PIn.Long(table.Rows[i]["ClinicNum"].ToString())));
							break;
						case "Code":
							row.Cells.Add(table.Rows[i]["ProcCode"].ToString());
							break;
						case "Tth":
							row.Cells.Add(table.Rows[i]["tth"].ToString());
							break;
						case "Description":
							row.Cells.Add(table.Rows[i]["description"].ToString());
							break;
						case "Charges":
							row.Cells.Add(table.Rows[i]["charges"].ToString());
							break;
						case "Credits":
							row.Cells.Add(table.Rows[i]["credits"].ToString());
							break;
						case "Balance":
							row.Cells.Add(table.Rows[i]["balance"].ToString());
							break;
						case "Signed":
							row.Cells.Add(table.Rows[i]["signed"].ToString());
							break;
						case "Abbr": //procedure abbreviation
							if(!String.IsNullOrEmpty(table.Rows[i]["AbbrDesc"].ToString())) {
								row.Cells.Add(table.Rows[i]["AbbrDesc"].ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						default:
							row.Cells.Add("");
							break;
					}
				}
				row.ColorText=Color.FromArgb(PIn.Int(table.Rows[i]["colorText"].ToString()));
				if(i==table.Rows.Count-1//last row
					|| (DateTime)table.Rows[i]["DateTime"]!=(DateTime)table.Rows[i+1]["DateTime"])
				{
					row.ColorLborder=Color.Black;
				}
				gridAccount.ListGridRows.Add(row);
			}
			gridAccount.EndUpdate();
			if(_scrollValueWhenDoubleClick==-1) {
				gridAccount.ScrollToEnd();
			}
			else {
				gridAccount.ScrollValue=_scrollValueWhenDoubleClick;
				_scrollValueWhenDoubleClick=-1;
			}
		}

		private void FillMisc() {
			//textCC.Text="";
			//textCCexp.Text="";
			if(_patient==null) {
				textUrgFinNote.Text="";
				textFinNote.Text="";
			}
			else{
				textUrgFinNote.Text=_family.ListPats[0].FamFinUrgNote;
				_famUrgFinNoteOnLoad=_family.ListPats[0].FamFinUrgNote;
				textFinNote.Text=_patientNote.FamFinancial;
				_famFinNoteOnLoad=_patientNote.FamFinancial;
				if(!textFinNote.Focused) {
					textFinNote.SelectionStart=textFinNote.Text.Length;
					//This will cause a crash if the richTextBox currently has focus. We don't know why.
					//Only happens if you call this during a Leave event, and only when moving between two ODtextBoxes.
					//Tested with two ordinary richTextBoxes, and the problem does not exist.
					//We may pursue fixing the root problem some day, but this workaround will do for now.
					textFinNote.ScrollToCaret();
				}
				if(!textUrgFinNote.Focused) {
					textUrgFinNote.SelectionStart=0;
					textUrgFinNote.ScrollToCaret();
				}
			}
			IsUrgFinNoteChanged=false;
			IsFinNoteChanged=false;
			//CCChanged=false;
			textUrgFinNote.ReadOnly=false;
			textFinNote.ReadOnly=false;
		}

		private void FillOrthoCasesGrid() {
			gridOrthoCases.BeginUpdate();
			gridOrthoCases.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TableOrthoCases","Is Active"),70,HorizontalAlignment.Center);
			gridOrthoCases.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOrthoCases","Is Transfer"),70,HorizontalAlignment.Center);
			gridOrthoCases.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOrthoCases","Start Date"),130,HorizontalAlignment.Center);
			gridOrthoCases.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOrthoCases","Completion Date"),120,HorizontalAlignment.Center){ IsWidthDynamic=true };
			gridOrthoCases.Columns.Add(col);
			gridOrthoCases.ListGridRows.Clear();
			gridOrthoCases.EndUpdate();
			RefreshOrthoCasesGridRows();
		}

		private void FillPatInfo() {
			if(_patient==null) {
				gridPatInfo.BeginUpdate();
				gridPatInfo.ListGridRows.Clear();
				gridPatInfo.Columns.Clear();
				gridPatInfo.EndUpdate();
				return;
			}
			gridPatInfo.BeginUpdate();
			gridPatInfo.Columns.Clear();
			GridColumn col=new GridColumn("",80);
			gridPatInfo.Columns.Add(col);
			col=new GridColumn("",150);
			gridPatInfo.Columns.Add(col);
			gridPatInfo.ListGridRows.Clear();
			GridRow row;
			_listDisplayFieldsPatInfo=DisplayFields.GetForCategory(DisplayFieldCategory.AccountPatientInformation);
			for(int f=0;f<_listDisplayFieldsPatInfo.Count;f++) {
				row=new GridRow();
				if(_listDisplayFieldsPatInfo[f].Description=="") {
					if(_listDisplayFieldsPatInfo[f].InternalName=="PatFields") {
						//don't add a cell
					}
					else {
						row.Cells.Add(_listDisplayFieldsPatInfo[f].InternalName);
					}
				}
				else {
					if(_listDisplayFieldsPatInfo[f].InternalName=="PatFields") {
						//don't add a cell
					}
					else {
						row.Cells.Add(_listDisplayFieldsPatInfo[f].Description);
					}
				}
				switch(_listDisplayFieldsPatInfo[f].InternalName) {
					case "Billing Type":
						row.Cells.Add(Defs.GetName(DefCat.BillingTypes,_patient.BillingType));
						break;
					case "PatFields":
						PatFieldL.AddPatFieldsToGrid(gridPatInfo,_patFieldArray.ToList(),FieldLocations.Account);
						break;
				}
				if(_listDisplayFieldsPatInfo[f].InternalName=="PatFields") {
					//don't add the row here
				}
				else {
					gridPatInfo.ListGridRows.Add(row);
				}
			}
			gridPatInfo.EndUpdate();
		}

		private void FillPats(bool isSelectingFamily) {
			if(_patient==null) {
				gridAcctPat.BeginUpdate();
				gridAcctPat.ListGridRows.Clear();
				gridAcctPat.EndUpdate();
				return;
			}
			gridAcctPat.BeginUpdate();
			gridAcctPat.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableAccountPat","Patient"),105);
			gridAcctPat.Columns.Add(col);
			col=new GridColumn(Lan.g("TableAccountPat","Bal"),49,HorizontalAlignment.Right);
			gridAcctPat.Columns.Add(col);
			gridAcctPat.ListGridRows.Clear();
			GridRow row;
			DataTable table=_dataSetMain.Tables["patient"];
			decimal balance=0;
			for(int i=0;i<table.Rows.Count;i++) {
				if(i!=table.Rows.Count-1 && PatientLinks.WasPatientMerged(PIn.Long(table.Rows[i]["PatNum"].ToString()),_loadData.ListMergeLinks) 
					&& _family.ListPats[i].PatNum!=_patient.PatNum && ((decimal)table.Rows[i]["balanceDouble"])==0) 
				{
					//Hide merged patients so that new things don't get added to them. If the user really wants to find this patient, they will have to use 
					//the Select Patient window.
					continue;
				}
				balance+=(decimal)table.Rows[i]["balanceDouble"];
				row=new GridRow();
				row.Cells.Add(GetPatNameFromTable(table,i));
				row.Cells.Add(table.Rows[i]["balance"].ToString());
				row.Tag=PIn.Long(table.Rows[i]["PatNum"].ToString());
				if(i==0 || i==table.Rows.Count-1) {
					row.Bold=true;
				}
				gridAcctPat.ListGridRows.Add(row);
			}
			gridAcctPat.EndUpdate();
			if(isSelectingFamily){
				gridAcctPat.SetSelected(gridAcctPat.ListGridRows.Count-1,true);
			}
			else{
				int index=gridAcctPat.ListGridRows.ToList().FindIndex(x => (long)x.Tag==_patient.PatNum);
				if(index>=0) {
					//If the index is greater than the number of rows, it will return and not select anything.
					gridAcctPat.SetSelected(index,true);
				}
			}
			if(isSelectingFamily){
				ToolBarMain.Buttons["Insurance"].Enabled=false;
				return;
			}
				ToolBarMain.Buttons["Insurance"].Enabled=true;
		}

		private void FillPaymentPlans() {
			_patientPortionBalanceTotal=0;
			_showGridPayPlan=false;
			if(_patient==null) {
				return;
			}
			DataTable table=_dataSetMain.Tables["payplan"];
			if(table.Rows.OfType<DataRow>().Count(x => PIn.Long(x["Guarantor"].ToString())==_patient.PatNum 
				|| PIn.Long(x["PatNum"].ToString())==_patient.PatNum)==0 && !IsFamilySelected()) //if we are looking at the entire family, show all the payplans 
			{
				return;
			}
			List<long> listPayPlanNums=table.Select().Select(x => PIn.Long(x["PayPlanNum"].ToString())).ToList();
			List<PayPlan> listPayPlansOvercharged=PayPlans.GetOverChargedPayPlans(listPayPlanNums);
			//do not hide payment plans that still have a balance when not on v2
			if(!checkShowCompletePayPlans.Checked) { //Hide the payment plans grid if there are no payment plans currently visible.
				bool existsOpenPayPlan=false;
				for(int i=0;i<table.Rows.Count;i++) { //for every payment plan
					if(DoShowPayPlan(checkShowCompletePayPlans.Checked,PIn.Bool(table.Rows[i]["IsClosed"].ToString()),
						PIn.Double(table.Rows[i]["balance"].ToString())))
					{						
						existsOpenPayPlan=true;
						break; //break
					}
				}
				if(!existsOpenPayPlan) {
					return;//no need to do anything else.
				}
			}
			_showGridPayPlan=true;
			gridPayPlan.BeginUpdate();
			gridPayPlan.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TablePaymentPlans","Date"),65);
			gridPayPlan.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePaymentPlans","Guarantor"),85);
			gridPayPlan.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePaymentPlans","Patient"),85);
			gridPayPlan.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePaymentPlans","Type"),30,HorizontalAlignment.Center);
			gridPayPlan.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePaymentPlans","Category"),60,HorizontalAlignment.Center);
			gridPayPlan.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePaymentPlans","Principal"),60,HorizontalAlignment.Right);
			gridPayPlan.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePaymentPlans","Total Cost"),60,HorizontalAlignment.Right);
			gridPayPlan.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePaymentPlans","Paid"),40,HorizontalAlignment.Right);
			gridPayPlan.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePaymentPlans","PrincPaid"),60,HorizontalAlignment.Right);
			gridPayPlan.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePaymentPlans","Balance"),60,HorizontalAlignment.Right);
			gridPayPlan.Columns.Add(col);
			if(PrefC.GetBool(PrefName.PayPlanHideDueNow)) {
				col=new GridColumn("Closed",60,HorizontalAlignment.Center);
			}
			else {
				col=new GridColumn(Lan.g("TablePaymentPlans","Due Now"),60,HorizontalAlignment.Right);
			}
			gridPayPlan.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePaymentPlans","eClipboard"),65,HorizontalAlignment.Center);
			gridPayPlan.Columns.Add(col);
			gridPayPlan.ListGridRows.Clear();
			GridRow row;
			GridCell cell;
			for(int i=0;i<table.Rows.Count;i++) {
				if(!DoShowPayPlan(checkShowCompletePayPlans.Checked,PIn.Bool(table.Rows[i]["IsClosed"].ToString()),
					PIn.Double(table.Rows[i]["balance"].ToString())))
				{
					continue;//hide
				}
				row=new GridRow();
				row.Cells.Add(table.Rows[i]["date"].ToString());
				if(table.Rows[i]["InstallmentPlanNum"].ToString()!="0" && table.Rows[i]["PatNum"].ToString()!=_patient.Guarantor.ToString()) {//Installment plan and not on guar
					cell=new GridCell(((string)"Invalid Guarantor"));
					cell.Bold=YN.Yes;
					cell.ColorText=Color.Red;
				}
				else {
					long payPlanNum=PIn.Long(table.Rows[i]["PayPlanNum"].ToString());
					PayPlan payPlan=PayPlans.GetOne(payPlanNum);
					cell=new GridCell("");
					//Installment Plans have no payPlanNum so we must skip this next if-statement if we do not receive one.
					if(payPlan!=null && payPlan.PlanNum==0) {//Only test via PlanNum for Insurance PayPlans as per RPPayPlan.cs' logic.
						cell=new GridCell(table.Rows[i]["guarantor"].ToString());//If not an Insurance PayPlan set guarantor as per usual.
					}
				}
				row.Cells.Add(cell);
				row.Cells.Add(table.Rows[i]["patient"].ToString());
				row.Cells.Add(table.Rows[i]["type"].ToString());
				long planCategory=PIn.Long(table.Rows[i]["PlanCategory"].ToString());
				if(planCategory==0) {
					row.Cells.Add(Lan.g(this,"None"));
				}
				else {
					row.Cells.Add(Defs.GetDef(DefCat.PayPlanCategories,planCategory).ItemName);
				}
				row.Cells.Add(table.Rows[i]["principal"].ToString());
				row.Cells.Add(table.Rows[i]["totalCost"].ToString());
				row.Cells.Add(table.Rows[i]["paid"].ToString());
				row.Cells.Add(table.Rows[i]["princPaid"].ToString());
				row.Cells.Add(table.Rows[i]["balance"].ToString());
				if(table.Rows[i]["IsClosed"].ToString()=="1" && PrefC.GetInt(PrefName.PayPlansVersion)==2) {
					cell=new GridCell(Lan.g(this,"Closed"));
					row.ColorText=Color.Gray;
				}
				else if(PrefC.GetBool(PrefName.PayPlanHideDueNow)) {//pref can only be enabled when PayPlansVersion == 2.
					cell=new GridCell("");
				}
				else { //they aren't hiding the "Due Now" cell text.
					cell=new GridCell(table.Rows[i]["due"].ToString());
					//Only color the due now red and bold in version 1 and 3 of payplans.
					if(PrefC.GetInt(PrefName.PayPlansVersion).In((int)PayPlanVersions.DoNotAge,(int)PayPlanVersions.AgeCreditsOnly,(int)PayPlanVersions.NoCharges)) 
					{
						if(table.Rows[i]["type"].ToString()!="Ins") {
							cell.Bold=YN.Yes;
							cell.ColorText=Color.Red;
						}
					}
				}
				row.Cells.Add(cell);
				if(PIn.Long(table.Rows[i]["MobileAppDeviceNum"].ToString())>0) {
					row.Cells.Add("X");
				}
				else {
					row.Cells.Add("");
				}
				row.Tag=table.Rows[i];
				if(listPayPlansOvercharged.Select(x => x.PayPlanNum).ToList().Contains(PIn.Long(table.Rows[i]["PayPlanNum"].ToString()))) {
					row.ColorBackG=Color.FromArgb(255,255,128);
				}
				gridPayPlan.ListGridRows.Add(row);
				_patientPortionBalanceTotal+=(Convert.ToDecimal(PIn.Double(table.Rows[i]["balance"].ToString())));
			}
			gridPayPlan.EndUpdate();
		}

		///<summary></summary>
		private void FillRepeatCharges() {
			_showGridRepeating=false;
			if(_patient==null) {
				return;
			}
			_repeatChargeArray=_loadData.ArrRepeatCharges;
			if(_repeatChargeArray.Length==0) {
				return;
			}
			if(PrefC.GetBool(PrefName.BillingUseBillingCycleDay)) {
				gridRepeat.Title=Lan.g(gridRepeat,"Repeat Charges")+" - Billing Day "+_patient.BillingCycleDay;
			}
			else {
				gridRepeat.Title=Lan.g(gridRepeat,"Repeat Charges");
			}
			_showGridRepeating=true;
			gridRepeat.BeginUpdate();
			gridRepeat.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableRepeatCharges","Description"),150);
			gridRepeat.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRepeatCharges","Amount"),60,HorizontalAlignment.Right);
			gridRepeat.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRepeatCharges","Start Date"),70,HorizontalAlignment.Center);
			gridRepeat.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRepeatCharges","Stop Date"),70,HorizontalAlignment.Center);
			gridRepeat.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRepeatCharges","Enabled"),55,HorizontalAlignment.Center);
			gridRepeat.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRepeatCharges","Note"),355);
			gridRepeat.Columns.Add(col);
			gridRepeat.ListGridRows.Clear();
			GridRow row;
			ProcedureCode procedureCode;
			for(int i=0;i<_repeatChargeArray.Length;i++) {
				row=new GridRow();
				procedureCode=ProcedureCodes.GetProcCode(_repeatChargeArray[i].ProcCode);
				row.Cells.Add(procedureCode.Descript);
				row.Cells.Add(_repeatChargeArray[i].ChargeAmt.ToString("F"));
				if(_repeatChargeArray[i].DateStart.Year>1880) {
					row.Cells.Add(_repeatChargeArray[i].DateStart.ToShortDateString());
				}
				else {
					row.Cells.Add("");
				}
				if(_repeatChargeArray[i].DateStop.Year>1880) {
					row.Cells.Add(_repeatChargeArray[i].DateStop.ToShortDateString());
				}
				else {
					row.Cells.Add("");
				}
				if(_repeatChargeArray[i].IsEnabled) {
					row.Cells.Add("X");
				}
				else {
					row.Cells.Add("");
				}
				string note="";
				if(!string.IsNullOrEmpty(_repeatChargeArray[i].Npi)) {
					note+="NPI="+_repeatChargeArray[i].Npi+" ";
				}
				if(!string.IsNullOrEmpty(_repeatChargeArray[i].ErxAccountId)) {
					note+="ErxAccountId="+_repeatChargeArray[i].ErxAccountId+" ";
				}
				if(!string.IsNullOrEmpty(_repeatChargeArray[i].ProviderName)) {
					note+=_repeatChargeArray[i].ProviderName+" ";
				}
				note+=_repeatChargeArray[i].Note;
				row.Cells.Add(note);
				gridRepeat.ListGridRows.Add(row);
			}
			gridRepeat.EndUpdate();
		}

		private void FillSummary() {
			textFamPriMax.Text="";
			textFamPriDed.Text="";
			textFamSecMax.Text="";
			textFamSecDed.Text="";
			textPriMax.Text="";
			textPriDed.Text="";
			textPriDedRem.Text="";
			textPriUsed.Text="";
			textPriPend.Text="";
			textPriRem.Text="";
			textSecMax.Text="";
			textSecDed.Text="";
			textSecDedRem.Text="";
			textSecUsed.Text="";
			textSecPend.Text="";
			textSecRem.Text="";
			if(_patient==null) {
				return;
			}
			double maxFam=0;
			double maxInd=0;
			double ded=0;
			double dedFam=0;
			double dedRem=0;
			double remain=0;
			double pend=0;
			double used=0;
			InsPlan insPlan;
			InsSub insSub;
			List<InsSub> listInsSubs=_loadData.ListInsSubs;
			List<InsPlan> listInsPlans=_loadData.ListInsPlans;
			List<PatPlan> listPatPlans=_loadData.ListPatPlans;
			List<Benefit> listBenefits=_loadData.ListBenefits;
			List<Claim> listClaims=_loadData.ListClaims;
			List<ClaimProcHist> listClaimProcHists=_loadData.HistList;
			if(listPatPlans.Count>0) {
				insSub=InsSubs.GetSub(listPatPlans[0].InsSubNum,listInsSubs);
				insPlan=InsPlans.GetPlan(insSub.PlanNum,listInsPlans);
				pend=InsPlans.GetPendingDisplay(listClaimProcHists,DateTime.Today,insPlan,listPatPlans[0].PatPlanNum,-1,_patient.PatNum,listPatPlans[0].InsSubNum,listBenefits);
				used=InsPlans.GetInsUsedDisplay(listClaimProcHists,DateTime.Today,insPlan.PlanNum,listPatPlans[0].PatPlanNum,-1,listInsPlans,listBenefits,_patient.PatNum,listPatPlans[0].InsSubNum);
				textPriPend.Text=pend.ToString("F");
				textPriUsed.Text=used.ToString("F");
				maxFam=Benefits.GetAnnualMaxDisplay(listBenefits,insPlan.PlanNum,listPatPlans[0].PatPlanNum,true);
				maxInd=Benefits.GetAnnualMaxDisplay(listBenefits,insPlan.PlanNum,listPatPlans[0].PatPlanNum,false);
				if(maxFam==-1) {
					textFamPriMax.Text="";
				}
				else {
					textFamPriMax.Text=maxFam.ToString("F");
				}
				if(maxInd==-1) {//if annual max is blank
					textPriMax.Text="";
					textPriRem.Text="";
				}
				else {
					remain=maxInd-used-pend;
					if(remain<0) {
						remain=0;
					}
					//textFamPriMax.Text=max.ToString("F");
					textPriMax.Text=maxInd.ToString("F");
					textPriRem.Text=remain.ToString("F");
				}
				//deductible:
				ded=Benefits.GetDeductGeneralDisplay(listBenefits,insPlan.PlanNum,listPatPlans[0].PatPlanNum,BenefitCoverageLevel.Individual);
				dedFam=Benefits.GetDeductGeneralDisplay(listBenefits,insPlan.PlanNum,listPatPlans[0].PatPlanNum,BenefitCoverageLevel.Family);
				if(ded!=-1) {
					textPriDed.Text=ded.ToString("F");
					dedRem=InsPlans.GetDedRemainDisplay(listClaimProcHists,DateTime.Today,insPlan.PlanNum,listPatPlans[0].PatPlanNum,-1,listInsPlans,_patient.PatNum,ded,dedFam);
					textPriDedRem.Text=dedRem.ToString("F");
				}
				if(dedFam!=-1) {
					textFamPriDed.Text=dedFam.ToString("F");
				}
			}
			if(listPatPlans.Count>1) {
				insSub=InsSubs.GetSub(listPatPlans[1].InsSubNum,listInsSubs);
				insPlan=InsPlans.GetPlan(insSub.PlanNum,listInsPlans);
				pend=InsPlans.GetPendingDisplay(listClaimProcHists,DateTime.Today,insPlan,listPatPlans[1].PatPlanNum,-1,_patient.PatNum,listPatPlans[1].InsSubNum,listBenefits);
				textSecPend.Text=pend.ToString("F");
				used=InsPlans.GetInsUsedDisplay(listClaimProcHists,DateTime.Today,insPlan.PlanNum,listPatPlans[1].PatPlanNum,-1,listInsPlans,listBenefits,_patient.PatNum,listPatPlans[1].InsSubNum);
				textSecUsed.Text=used.ToString("F");
				//max=Benefits.GetAnnualMaxDisplay(BenefitList,PlanCur.PlanNum,PatPlanList[1].PatPlanNum);
				maxFam=Benefits.GetAnnualMaxDisplay(listBenefits,insPlan.PlanNum,listPatPlans[1].PatPlanNum,true);
				maxInd=Benefits.GetAnnualMaxDisplay(listBenefits,insPlan.PlanNum,listPatPlans[1].PatPlanNum,false);
				if(maxFam==-1) {
					textFamSecMax.Text="";
				}
				else {
					textFamSecMax.Text=maxFam.ToString("F");
				}
				if(maxInd==-1) {//if annual max is blank
					textSecMax.Text="";
					textSecRem.Text="";
				}
				else {
					remain=maxInd-used-pend;
					if(remain<0) {
						remain=0;
					}
					//textFamSecMax.Text=max.ToString("F");
					textSecMax.Text=maxInd.ToString("F");
					textSecRem.Text=remain.ToString("F");
				}
				//deductible:
				ded=Benefits.GetDeductGeneralDisplay(listBenefits,insPlan.PlanNum,listPatPlans[1].PatPlanNum,BenefitCoverageLevel.Individual);
				dedFam=Benefits.GetDeductGeneralDisplay(listBenefits,insPlan.PlanNum,listPatPlans[1].PatPlanNum,BenefitCoverageLevel.Family);
				if(ded!=-1) {
					textSecDed.Text=ded.ToString("F");
					dedRem=InsPlans.GetDedRemainDisplay(listClaimProcHists,DateTime.Today,insPlan.PlanNum,listPatPlans[1].PatPlanNum,-1,listInsPlans,_patient.PatNum,ded,dedFam);
					textSecDedRem.Text=dedRem.ToString("F");
				}
				if(dedFam!=-1) {
					textFamSecDed.Text=dedFam.ToString("F");
				}
			}
		}

		///<summary>Show the splits that are flagged as being hidden. </summary>
		private void FillTpUnearned() {
			if(_patient==null) {
				return;
			}
			if(_listPaySplitsHidden.Count==0) {
				return;
			}
			List<Procedure> listProceduresForHiddenSplits=Procedures.GetManyProc(_listPaySplitsHidden.Select(x => x.ProcNum).ToList(),includeNote:false);
			gridTpSplits.BeginUpdate();
			gridTpSplits.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableTpUnearned","Date"),65);
			gridTpSplits.Columns.Add(col);
			col=new GridColumn(Lan.g("TableTpUnearned","Patient"),150);
			gridTpSplits.Columns.Add(col);
			col=new GridColumn(Lan.g("TableTpUnearned","Provider"),70);
			gridTpSplits.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g("TableTpUnearned","Clinic"),60);
				gridTpSplits.Columns.Add(col);
			}
			col=new GridColumn(Lan.g("TableTpUnearned","Code"),80);
			gridTpSplits.Columns.Add(col);
			col=new GridColumn(Lan.g("TableTpUnearned","Description"),180);
			gridTpSplits.Columns.Add(col);
			col=new GridColumn(Lan.g("TableTpUnearned","Amount"),60,HorizontalAlignment.Right);
			gridTpSplits.Columns.Add(col);
			col=new GridColumn(Lan.g("TableTpUnearned","Total"),60,HorizontalAlignment.Right);
			gridTpSplits.Columns.Add(col);
			gridTpSplits.ListGridRows.Clear();
			List<Patient> listPatientsOtherFams=new List<Patient>();
			double tpRunningTotal=0;
			for(int i=0;i<_listPaySplitsHidden.Count();i++) {
				GridRow row=new GridRow();
				row.Cells.Add(_listPaySplitsHidden[i].DatePay.ToShortDateString());//Date
				Patient patientForSplit=_loadData.Fam.ListPats.ToList().Find(x => x.PatNum==_listPaySplitsHidden[i].PatNum);
				if(patientForSplit is null) {//from another family
					patientForSplit=listPatientsOtherFams.Find(x => x.PatNum==_listPaySplitsHidden[i].PatNum);
				}
				if(patientForSplit is null) {//first time we've encountered this patient in this loop
					patientForSplit=Patients.GetPat(_listPaySplitsHidden[i].PatNum);
				}
				row.Cells.Add(patientForSplit.LName+", "+patientForSplit.FName);//Patient
				row.Cells.Add(Providers.GetAbbr(_listPaySplitsHidden[i].ProvNum));//Provider
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(_listPaySplitsHidden[i].ClinicNum));//Clinics
				}
				long codeNum=listProceduresForHiddenSplits.FirstOrDefault(x => x.ProcNum==_listPaySplitsHidden[i].ProcNum)?.CodeNum??0;
				ProcedureCode procedureCode=ProcedureCodes.GetFirstOrDefault(x => x.CodeNum==codeNum);
				string paymentType=Defs.GetName(DefCat.PaymentTypes,Payments.GetPayment(_listPaySplitsHidden[i].PayNum).PayType);
				long dynamicPayPlanPrepaymentUnearnedType=PrefC.GetLong(PrefName.DynamicPayPlanPrepaymentUnearnedType);
				if(_listPaySplitsHidden[i].UnearnedType==dynamicPayPlanPrepaymentUnearnedType && _listPaySplitsHidden[i].PayPlanNum!=0) {
					paymentType+=" (Attached to Payment Plan)";
				}
				if(procedureCode!=null) {
					row.Cells.Add(procedureCode.ProcCode);//Code
					row.Cells.Add(paymentType+", "+procedureCode.Descript);//Description
				}
				else {
					row.Cells.Add("");//Code
					row.Cells.Add(paymentType);//Description
				}
				row.Cells.Add(_listPaySplitsHidden[i].SplitAmt.ToString("F"));//Amount
				row.Cells.Add((tpRunningTotal+=_listPaySplitsHidden[i].SplitAmt).ToString("F"));//Total
				row.Tag=_listPaySplitsHidden[i];
				Color color=Defs.GetDefsForCategory(DefCat.AccountColors)[3].ItemColor;
				row.ColorLborder=color;
				row.ColorText=color;
				gridTpSplits.ListGridRows.Add(row);
			}
			gridTpSplits.EndUpdate();
			if(_scrollValueWhenDoubleClickTpUnearned==-1) {
				gridTpSplits.ScrollToEnd();
				return;
			}
			gridTpSplits.ScrollValue=_scrollValueWhenDoubleClickTpUnearned;
			_scrollValueWhenDoubleClickTpUnearned=-1;
		}
		#endregion Methods - Private Fill

		#region Methods - Private Other
		///<summary>Validated the procedure code using FormProcEdit and prompts user for input if required.</summary>
		private bool AddProcAndValidate(string procString,Provider provider) {
			ProcedureCode procedurecode=ProcedureCodes.GetProcCode(procString);
			if(procedurecode.CodeNum==0) {
				MsgBox.Show(Lan.g(this,"Invalid Procedure Code:")+" "+procString);
				return false; //Invalid ProcCode string manually entered.
			}
			if(ProcedureCodes.AreAnyProcCodesHidden(procedurecode.CodeNum)) {
				MsgBox.Show($"{Lan.g(this,"Cannot add procedure because it is in a hidden category")}: {procedurecode.ProcCode}");
				return false;
			}
			Procedure procedure=new Procedure();
			procedure.ProcStatus=ProcStat.C;
			procedure.ClinicNum=_patient.ClinicNum;
			procedure.CodeNum=procedurecode.CodeNum;
			procedure.DateEntryC=DateTime.Now;
			procedure.DateTP=DateTime.Now;
			procedure.PatNum=_patient.PatNum;
			procedure.ProcDate=DateTime.Now;
			procedure.ToothRange="";
			procedure.PlaceService=Clinics.GetPlaceService(procedure.ClinicNum); 
			if(!PrefC.GetBool(PrefName.EasyHidePublicHealth)) {
				procedure.SiteNum=_patient.SiteNum;
			}
			procedure.ProvNum=procedurecode.ProvNumDefault;//use proc default prov if set
			if(procedure.ProvNum==0) { //if none set, use primary provider.
				procedure.ProvNum=provider.ProvNum;
			}
			List<InsSub> listInsSubs=InsSubs.RefreshForFam(_family);
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
			List<PatPlan> listPatPlans=PatPlans.Refresh(_patient.PatNum);
			procedure.MedicalCode=procedurecode.MedicalCode;
			procedure.ProcFee=Procedures.GetProcFee(_patient,listPatPlans,listInsSubs,listInsPlans,procedure.CodeNum,procedure.ProvNum,procedure.ClinicNum,procedure.MedicalCode);
			procedure.UnitQty=1;
			//Find out if we are going to link the procedure to an ortho case.
			OrthoCaseProcedureLinker orthoCaseProcedureLinker=OrthoCaseProcedureLinker.CreateOneForPatient(procedure.PatNum);
			procedure.ProcStatus=ProcStat.D;
			bool shouldProcedureLinkToOrthoCase=orthoCaseProcedureLinker.ShouldProcedureLinkToOrthoCase(procedure,procedurecode.ProcCode);
			Procedures.Insert(procedure,skipDiscountPlanAdjustment:shouldProcedureLinkToOrthoCase);
			procedure.ProcStatus=ProcStat.C;
			Procedures.SetOrthoProcComplete(procedure,procedurecode);//does nothing if not an ortho proc
			orthoCaseProcedureLinker.LinkProcedureToActiveOrthoCaseIfNeeded(procedure,doUpdateProcedure:true);
			//launch form silently to validate code. If entry errors occur the form will be shown to user, otherwise it will close immediately.
			List<ClaimProcHist> listClaimProcHistsLoop=new List<ClaimProcHist>();
			using FormProcEdit formProcEdit=new FormProcEdit(procedure,_patient,_family,true);
			formProcEdit.ListClaimProcHists=_loadData.HistList;
			formProcEdit.ListClaimProcHistsLoop=listClaimProcHistsLoop;
			formProcEdit.IsNew=true;
			formProcEdit.ShowDialog();
			if(formProcEdit.DialogResult!=DialogResult.OK) {
				Procedures.Delete(procedure.ProcNum);
				return false;
			}
			if(procedure.ProcStatus==ProcStat.C) {
				AutomationL.Trigger(EnumAutomationTrigger.ProcedureComplete,new List<string>() { ProcedureCodes.GetStringProcCode(procedure.CodeNum) },_patient.PatNum);
				Procedures.AfterProcsSetComplete(new List<Procedure>() { procedure });
			}
			return true;
		}

		///<summary>For one patient in HQ db with thousands of big email rows, it was taking 9 seconds to refresh. This reduces it to 1 second.  A better fix would be for the grid to be internally faster for this scenario.</summary>
		public void LayoutPanelsAndRefreshMainGrids(bool doLogFillMain=false) {
			gridAccount.ListGridRows.Clear();
			gridComm.ListGridRows.Clear();
			LayoutPanels();
			if(doLogFillMain) {//Only log this fill on the refresh method call
				Logger.LogAction("FillMain",LogPath.AccountModule,() => FillMain());
			}
			else {
				FillMain();
			}
			FillComm();
		}

		///<summary>This used to be a layout event, but that was making it get called far too frequently.  Now, this must explicitly and intelligently be called.</summary>
		public void LayoutPanels(){
			textUrgFinNote.Font=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleF(8.25f));
			//splitContainerParent
			//  Panel1: splitContainerRepChargesPP
			//    Panel1: gridRepeat
			//    Panel2: gridPayPlan
			//  Panel2: splitContainerAccountCommLog
			//    Panel1: tabControlAccount
			//      tabPagePatAccount: gridAccount
			//			tabPageAutoOrtho: gridAutoOrtho
			//      tabPageOrthoCases: gridOrthoCases
			//      tabPageHiddenSplits: gridTpSplits
			//    Panel2: 
			//      gridComm
			LayoutManager.MoveLocation(splitContainerParent,new Point(0,LayoutManager.Scale(63)));
			LayoutManager.MoveSize(splitContainerParent,new Size(tabControlShow.Left-1,Height-splitContainerParent.Top-1));
			//If the two top grids are not visible, collapse the entire parent panel 1 so it does not show extra white space.
			splitContainerParent.Panel1Collapsed=!_showGridPayPlan && !_showGridRepeating;
			if(!splitContainerParent.Panel1Collapsed) {
				if(_showGridRepeating) {
					splitContainerRepChargesPP.Panel1Collapsed=false;
				}
				else {
					splitContainerRepChargesPP.Panel1Collapsed=true;
					splitContainerParent.Panel1MinSize=LayoutManager.Scale(20);
				}
				if(_showGridPayPlan) {
					splitContainerRepChargesPP.Panel2Collapsed=false;
				}
				else{
					splitContainerRepChargesPP.Panel2Collapsed=true;
					splitContainerParent.Panel1MinSize=LayoutManager.Scale(20);
				}
			}
			//If both visible, make sure the minimum size is set back to orignal value.
			if(_showGridPayPlan && _showGridRepeating) {
				splitContainerParent.Panel1MinSize=LayoutManager.Scale(45);
			}
			if(gridAccount.HScrollVisible){
				splitContainerParent.Panel2MinSize=LayoutManager.Scale(85);//85px is the height needed for the account grid and the commlog grid.
				splitContainerAccountCommLog.Panel1MinSize=LayoutManager.Scale(60);//60px is the height needed for the tabs, the grid title, and the horizontal scrollbar.
			}
			else{
				splitContainerParent.Panel2MinSize=LayoutManager.Scale(85)-gridAccount.HScrollHeight;
				splitContainerAccountCommLog.Panel1MinSize=LayoutManager.Scale(60)-gridAccount.HScrollHeight;
			}
			if(_listDisplayFieldsPatInfo!=null && _listDisplayFieldsPatInfo.Count==0) {
				gridPatInfo.Visible=false;
			}
			else {
				gridPatInfo.Visible=true;
			}
			//only show the auto ortho grid and tab control if they have the show feature enabled.
			//otherwise, hide the tabs and re-size the account grid.
			if(!PrefC.GetBool(PrefName.OrthoEnabled)) {
				tabControlAccount.TabPages.Remove(tabPageAutoOrtho);
			}
			else if(!tabControlAccount.TabPages.Contains(tabPageAutoOrtho)) {
				LayoutManager.Add(tabPageAutoOrtho,tabControlAccount);
			}
			if(!OrthoCases.HasOrthoCasesEnabled()) {
				tabControlAccount.TabPages.Remove(tabPageOrthoCases);
			}
			else if(!tabControlAccount.TabPages.Contains(tabPageOrthoCases)) {
				LayoutManager.Add(tabPageOrthoCases,tabControlAccount);
			}
			if(_listPaySplitsHidden.Count==0) {//might need to get updated more often than from loadData. Not sure how much we care. 
				tabControlAccount.TabPages.Remove(tabPageHiddenSplits);
			}
			else{
				if(!tabControlAccount.TabPages.Contains(tabPageHiddenSplits)) {
					LayoutManager.Add(tabPageHiddenSplits,tabControlAccount);
				}
				List<PaySplit> listPaySplits=gridTpSplits.GetTags<PaySplit>();
				double totPaySplitAmount=listPaySplits.Sum(x => x.SplitAmt);
				if(CompareDouble.IsZero(totPaySplitAmount)) {
					tabPageHiddenSplits.ColorTab=Color.Empty;
				}
				else{
					tabPageHiddenSplits.ColorTab=Color.LightCoral;//make the tab red if hidden splits do not total $0
				}
			}
			if(tabControlAccount.TabPages.Contains(tabPageAutoOrtho) 
				|| tabControlAccount.TabPages.Contains(tabPageHiddenSplits) 
				|| tabControlAccount.TabPages.Contains(tabPageOrthoCases)) 
			{//if any additional tabs present besides main one
				tabControlAccount.TabsAreCollapsed=false;
				return;
			}
			tabControlAccount.TabsAreCollapsed=true;
		}

		///<summary>Returns true if the payment plan should be displayed.</summary>
		private bool DoShowPayPlan(bool doShowCompletedPlans,bool isClosed,double balance) {	
			if(doShowCompletedPlans) {
				return true;
			}		
			//do not hide payment plans that still have a balance when not on v2
			bool doShowClosedPlansWithBalance=(PrefC.GetInt(PrefName.PayPlansVersion)!=(int)PayPlanVersions.AgeCreditsAndDebits);
			return !isClosed
						|| (doShowClosedPlansWithBalance && !CompareDouble.IsEqual(balance,0)); //Or the payment plan has a balance
		}

		///<summary>Returns a list of CreateClaimItems comprised from the selected items within gridAccount.
		///If no rows are currently selected then the list returned will be comprised of all items within the "account" table in the DataSet.</summary>
		private List<CreateClaimItem> GetCreateClaimItemsFromUI() {
			//There have been reports of concurrency issues so make a deep copy of the selected indices and the table first to help alleviate the problem.
			//See task #830623 and task #1266253 for more details.
			int[] intArraySelectedIndices=(int[])gridAccount.SelectedIndices.Clone();
			DataTable table=GetTableFromDataSet("account");
			List<CreateClaimItem> listCreateClaimItems=ClaimL.GetCreateClaimItems(table,intArraySelectedIndices);
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				//We do not want to consider Canadian lab procs to be selected.  If we do, these lab procs will later cause the corresponding lab ClaimProcs to 
				//be included in the Claim's list of ClaimProcs, which will then cause the ClaimProcs for the labs to get a LineNumber, which will in turn cause
				//the EOB Importer to fail because the LineNumbers in the database's list of ClaimProcs no longer match the EOB LineNumbers.
				listCreateClaimItems.RemoveAll(x => x.ProcNumLab!=0);
			}
			return listCreateClaimItems;
		}

		private string GetPatNameFromTable(DataTable table,int index) {
			string name=table.Rows[index]["name"].ToString();
			if(PrefC.GetBool(PrefName.TitleBarShowSpecialty) && string.Compare(name,"Entire Family",true)!=0) {
				long patNum=PIn.Long(table.Rows[index]["PatNum"].ToString());
				string specialty=Patients.GetPatientSpecialtyDef(patNum)?.ItemName??"";
				name+=string.IsNullOrWhiteSpace(specialty)?"":"\r\n"+specialty;
			}
			return name;
		}

		///<summary>Returns a deep copy of the corresponding table from the main data set.
		///Utilizes a lock object that is partially implemented in an attempt to fix an error when invoking DataTable.Clone()</summary>
		private DataTable GetTableFromDataSet(string tableName) {
			DataTable table;
			lock(_lockDataSetMain) {
				table=_dataSetMain.Tables[tableName].Clone();
				for(int i=0;i<_dataSetMain.Tables[tableName].Rows.Count;i++) {
					table.ImportRow(_dataSetMain.Tables[tableName].Rows[i]);
				}
			}
			return table;
		}

		/// <summary>Saves the statement.  Attaches a pdf to it by creating a doc object.  Prints it or emails it.  </summary>
		private void PrintStatement(Statement statement) {
			Cursor=Cursors.WaitCursor;
			Statements.Insert(statement);
			//When the statement gets inserted into the db, check and see if a pat is on a mobile device to refresh payments
			List<MobileAppDevice> listMobileAppDevices=MobileAppDevices.GetAll();
			MobileAppDevice mobileAppDevice=listMobileAppDevices.FirstOrDefault(x => x.PatNum==_patient.PatNum);
			if(mobileAppDevice!=null && mobileAppDevice.LastCheckInActivity>DateTime.Now.AddHours(-1)) {
				PushNotificationUtils.CI_RefreshPayment(mobileAppDevice.MobileAppDeviceNum,_patient.PatNum, out string errorMsg);
			}
			SheetDef sheetDef=SheetUtil.GetStatementSheetDef(statement);
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,statement.PatNum,statement.HidePayment);
			DataSet dataSet=AccountModules.GetAccount(statement.PatNum,statement,doShowHiddenPaySplits:statement.IsReceipt);
			sheet.Parameters.Add(new SheetParameter(true,"Statement") { ParamValue=statement });
			SheetFiller.FillFields(sheet,dataSet,statement);
			SheetUtil.CalculateHeights(sheet,dataSet,statement);
			string tempPath=CodeBase.ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),statement.PatNum.ToString()+".pdf");
			SheetPrinting.CreatePdf(sheet,tempPath,statement,dataSet:dataSet);
			long category=0;
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			bool wasStatementDeleted=false;
			for(int i=0;i<listDefs.Count;i++) {
				if(Regex.IsMatch(listDefs[i].ItemValue,@"S")) {
					category=listDefs[i].DefNum;
					break;
				}
			}
			if(category==0) {
				category=listDefs[0].DefNum;//put it in the first category.
			}
			//create doc--------------------------------------------------------------------------------------
			Document document=null;
			try {
				document=ImageStore.Import(tempPath,category,Patients.GetPat(statement.PatNum));
			}
			catch {
				this.Cursor=Cursors.Default;
				MsgBox.Show(this,"Error saving document.");
				return;
			}
			document.ImgType=ImageType.Document;
			if(statement.IsInvoice) {
				document.Description=Lan.g(this,"Invoice");
			}
			else {
				if(statement.IsReceipt) {
					document.Description=Lan.g(this,"Receipt");
				}
				else {
					document.Description=Lan.g(this,"Statement");
				}
			}
			statement.DateSent=document.DateCreated;
			statement.DocNum=document.DocNum;//this signals the calling class that the pdf was created successfully.
			Statements.AttachDoc(statement.StatementNum,document);
			//if(ImageStore.UpdatePatient == null){
			//	ImageStore.UpdatePatient = new FileStore.UpdatePatientDelegate(Patients.Update);
			//}
			Patient patientGuarantor=Patients.GetPat(statement.PatNum);
			string guarFolder=ImageStore.GetPatientFolder(patientGuarantor,ImageStore.GetPreferredAtoZpath());
			//OpenDental.Imaging.ImageStoreBase imageStore = OpenDental.Imaging.ImageStore.GetImageStore(guar);
			if(statement.Mode_==StatementMode.Email) {
				if(!Security.IsAuthorized(EnumPermType.EmailSend)) {
					Cursor=Cursors.Default;
					return;
				}
				string attachPath=EmailAttaches.GetAttachPath();
				Random random=new Random();
				string fileName=DateTime.Now.ToString("yyyyMMdd")+DateTime.Now.TimeOfDay.Ticks.ToString()+random.Next(1000).ToString()+".pdf";
				string filePathAndName=FileAtoZ.CombinePaths(attachPath,fileName);
				FileAtoZ.Copy(ImageStore.GetFilePath(Documents.GetByNum(statement.DocNum),guarFolder),filePathAndName,FileAtoZSourceDestination.AtoZToAtoZ);
				//Process.Start(filePathAndName);
				EmailMessage emailMessage=Statements.GetEmailMessageForStatement(statement,patientGuarantor);
				EmailAttach emailAttach=new EmailAttach();
				emailAttach.DisplayedFileName="Statement.pdf";
				emailAttach.ActualFileName=fileName;
				emailMessage.Attachments.Add(emailAttach);
				using FormEmailMessageEdit formEmailMessageEdit=new FormEmailMessageEdit(emailMessage,EmailAddresses.GetByClinic(patientGuarantor.ClinicNum));
				formEmailMessageEdit.IsNew=true;
				formEmailMessageEdit.ShowDialog();
				//If user clicked delete or cancel, delete pdf and statement
				if(formEmailMessageEdit.DialogResult==DialogResult.Cancel) {
					Patient patient;
					string patFolder;
					if(statement.DocNum!=0) {
						//delete the pdf
						patient=Patients.GetPat(statement.PatNum);
						patFolder=ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath());
						List<Document> listDocuments=new List<Document>();
						listDocuments.Add(Documents.GetByNum(statement.DocNum));
						try {
							ImageStore.DeleteDocuments(listDocuments,patFolder);
						}
						catch {  //Image could not be deleted, in use.
							//This should never get hit because the file was created by this user within this method.  
							//If the doc cannot be deleted, then we will not stop them, they will have to manually delete it from the images module.
						}
					}
					//delete statement
					Statements.Delete(statement);
					wasStatementDeleted=true;
				}
			}
			else {//not email
				if(ODBuild.IsDebug()) {
					//don't bother to check valid path because it's just debug.
					Document document2=Documents.GetByNum(statement.DocNum);
					string imgPath=ImageStore.GetFilePath(document2,guarFolder);
					DateTime dateTimeNow=DateTime.Now;
					while(DateTime.Now<dateTimeNow.AddSeconds(5) && !FileAtoZ.Exists(imgPath)) {//wait up to 5 seconds.
						Application.DoEvents();
					}
					try {
						FileAtoZ.StartProcess(imgPath);
					}
					catch(Exception ex) {
						FriendlyException.Show($"Unable to open the following file: {document2.FileName}",ex);
					}
				}
				else {
					//Thread thread=new Thread(new ParameterizedThreadStart(SheetPrinting.PrintStatement));
					//thread.Start(new List<object> { sheetDef,stmt,tempPath });
					//NOTE: This is printing a "fresh" GDI+ version of the statment which is ever so slightly different than the PDFSharp statment that was saved to disk.
					sheet=SheetUtil.CreateSheet(sheetDef,statement.PatNum,statement.HidePayment);
					SheetFiller.FillFields(sheet,dataSet,statement);
					SheetUtil.CalculateHeights(sheet,dataSet,statement);
					SheetPrinting.Print(sheet,1,false,statement);//use GDI+ printing, which is slightly different than the pdf.
				}
			}
			if(!wasStatementDeleted) {
				Statements.SyncStatementProdsForStatement(dataSet,statement.StatementNum,statement.DocNum);
			}
			Cursor=Cursors.Default;

		}

		///<summary>Call this before inserting new repeat charge to update patient.BillingCycleDay if no other repeat charges exist.
		///Changes the patient's BillingCycleDay to today if no other active repeat charges are on the patient's account</summary>
		private void UpdatePatientBillingDay(long patNum) {
			if(RepeatCharges.ActiveRepeatChargeExists(patNum)) {
				return;
			}
			Patient patientOld=Patients.GetPat(patNum);
			if(patientOld.BillingCycleDay==DateTime.Today.Day) {
				return;
			}
			Patient patientNew=patientOld.Copy();
			patientNew.BillingCycleDay=DateTime.Today.Day;
			Patients.Update(patientNew,patientOld);
		}
		#endregion Methods - Private Other

		#region Methods - Helpers
		///<summary>If the user selects multiple procedures (validated) then we pass the selected procedures to FormMultiAdj. Otherwise if the user
		///selects one procedure (not validated) we maintain the previous functionality of opening FormAdjust.</summary>
		private void AddAdjustmentToSelectedProcsHelper(bool openMultiAdj=false) {
			Plugins.HookAddCode(this,"ContrAccount.AddAdjustmentToSelectedProcsHelper_beginning",_patient,gridPayPlan);
			DataTable tableAcct=_dataSetMain.Tables["account"];
			List<long> listProcNumsSelected=new List<long>();
			for(int i=0;i<gridAccount.SelectedIndices.Length;i++){
				long procNum=PIn.Long(tableAcct.Rows[gridAccount.SelectedIndices[i]]["ProcNum"].ToString());
				if(procNum==0){
					MsgBox.Show(this,"You can only select procedures.");
					return;
				}
				listProcNumsSelected.Add(procNum);
			}
			//If the user selected multiple procedures or clicked the Add Multi Adj button then open FormMultiAdj.
			if(listProcNumsSelected.Count>1 || openMultiAdj) {
				//Open the form with only the selected procedures
				using FormAdjMulti formAdjMulti=new FormAdjMulti(_patient,listProcNumsSelected);
				formAdjMulti.ShowDialog();
			}
			else {
				Patient patient=_patient;
				Adjustment adjustment=new Adjustment();
				adjustment.DateEntry=DateTime.Today;//cannot be changed. Handled automatically
				adjustment.AdjDate=DateTime.Today;
				adjustment.ProcDate=DateTime.Today;
				adjustment.ProvNum=_patient.PriProv;
				adjustment.PatNum=_patient.PatNum;
				adjustment.ClinicNum=_patient.ClinicNum;
				if(gridAccount.SelectedGridRows.Count==1) {
					OrthoProcLink orthoProcLink=OrthoProcLinks.GetByProcNum(PIn.Long(tableAcct.Rows[gridAccount.SelectedIndices[0]]["ProcNum"].ToString()));
					if(orthoProcLink!=null) {
						MsgBox.Show(this,"Procedures linked to ortho cases cannot be adjusted.");
						return;
					}
					long procNum=PIn.Long(tableAcct.Rows[gridAccount.SelectedIndices[0]]["ProcNum"].ToString());
					Procedure procedure=Procedures.GetOneProc(procNum,false);
					if(!Security.IsAuthorized(EnumPermType.ProcCompleteAddAdj,Procedures.GetDateForPermCheck(procedure))) {
						return;
					}
					adjustment.ProcNum=procNum;
					if(procedure!=null) {
						adjustment.ProvNum=procedure.ProvNum;
						adjustment.ClinicNum=procedure.ClinicNum;
						adjustment.PatNum=procedure.PatNum;
						if(adjustment.PatNum!=_patient.PatNum) {
							patient=_family.GetPatient(adjustment.PatNum)??Patients.GetPat(adjustment.PatNum);
						}
					}
				}
				using FormAdjust formAdjust=new FormAdjust(patient,adjustment);
				formAdjust.IsNew=true;
				formAdjust.ShowDialog();
				//Shared.ComputeBalances();
			}
			ModuleSelected(_patient.PatNum);
		}

		///<summary>Returns true if XCharge or PayConnect payments are allowed to be made for the currently selected clinic.</summary>
		private static bool IsWebPaymentsEnabled() {
			return IsXWebPaymentsEnabled() || IsPayConnectPaymentsEnabled();
		}

		private static bool IsXWebPaymentsEnabled() {
			WebPaymentProperties webPaymentProperties;
			try {
				ProgramProperties.GetXWebCreds(Clinics.ClinicNum,out webPaymentProperties);
			}
			catch {
				return false;
			}
			return webPaymentProperties.IsPaymentsAllowed;
		}

		private static bool IsPayConnectPaymentsEnabled() {
			PayConnect.WebPaymentProperties webPaymentProperties=new PayConnect.WebPaymentProperties();
			try {
				ProgramProperties.GetPayConnectPatPortalCreds(Clinics.ClinicNum,out webPaymentProperties);
			}
			catch {
				return false;
			}
			return webPaymentProperties.IsPaymentsAllowed;
		}

		private void PayPlanHelper(PayPlanModes payPlanMode) {
			if(!Security.IsAuthorized(EnumPermType.PayPlanEdit)) {
				return;
			}
			bool isTsiPayplan=TsiTransLogs.IsTransworldEnabled(_family.Guarantor.ClinicNum) && Patients.IsGuarCollections(_patient.Guarantor,false);
			string msg="";
			if(isTsiPayplan) {
				if(!Security.IsAuthorized(EnumPermType.Billing,true)) {
					msg=Lan.g(this,"The guarantor of this family has been sent to TSI for a past due balance.")+"\r\n"
						+Lan.g(this,"Creating a payment plan for this guarantor would cause the account to be suspended in the TSI system but you are not "
							+"authorized for")+"\r\n"
						+GroupPermissions.GetDesc(EnumPermType.Billing);
					MessageBox.Show(this,msg);
					return;
				}
				string billingType=Defs.GetName(DefCat.BillingTypes,PrefC.GetLong(PrefName.TransworldPaidInFullBillingType));
				msg=Lan.g(this,"The guarantor of this family has been sent to TSI for a past due balance.")+"\r\n"
					+Lan.g(this,"Creating this payment plan will suspend the TSI account for a maximum of 50 days if the account is in the Accelerator or "
						+"Profit Recovery stage.")+"\r\n"
					+Lan.g(this,"Continue creating the payment plan?")+"\r\n\r\n"
					+Lan.g(this,"Yes - Create the payment plan, send a suspend message to TSI, and change the guarantor's billing type to")+" "
						+billingType+".\r\n\r\n"
					+Lan.g(this,"No - Do not create the payment plan and allow TSI to continue managing the account.");
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,msg)) {
					return;
				}
			}
			PayPlan payPlan=new PayPlan();
			payPlan.IsNew=true;
			payPlan.PatNum=_patient.PatNum;
			payPlan.Guarantor=_patient.Guarantor;
			if(payPlanMode.HasFlag(PayPlanModes.Insurance)) {
				payPlan.Guarantor=0;//Insurance PayPlans have no guarantor because they are charged to insurance
			}
			payPlan.PayPlanDate=DateTime.Today;
			payPlan.CompletedAmt=0;
			long patNumGoto=0;
			if(payPlanMode.HasFlag(PayPlanModes.Dynamic)) {
				payPlan.IsDynamic=true;
				payPlan.ChargeFrequency=PayPlanFrequency.Monthly;
				payPlan.PayPlanNum=PayPlans.Insert(payPlan);
				using FormPayPlanDynamic formPayPlanDynamic=new FormPayPlanDynamic(payPlan);
				formPayPlanDynamic.ShowDialog();
				patNumGoto=formPayPlanDynamic.PatNumGoto;
			}
			else {
				payPlan.PayPlanNum=PayPlans.Insert(payPlan);
				using FormPayPlan formPayPlan=new FormPayPlan(payPlan);
				formPayPlan.TotalAmt=_patient.EstBalance;
				formPayPlan.IsNew=true;
				formPayPlan.IsInsPayPlan=true;
				formPayPlan.ShowDialog();
				patNumGoto=formPayPlan.PatNumGoto;
			}
			if(patNumGoto!=0) {
				FormOpenDental.S_Contr_PatientSelected(Patients.GetPat(patNumGoto),false);
				ModuleSelected(patNumGoto);//switches to other patient.
			}
			else{
				ModuleSelected(_patient.PatNum);
			}
			if(isTsiPayplan && PayPlans.GetOne(payPlan.PayPlanNum)!=null) {
				msg=TsiTransLogs.SuspendGuar(_family.Guarantor);
				if(!string.IsNullOrEmpty(msg)) {
					MessageBox.Show(this,msg+"\r\n"+Lan.g(this,"The account will have to be suspended manually using the A/R Manager or the TSI web portal."));
				}
			}
		}

		
		///<summary>If the "Make Payment" action item in eClip isn't present, this will add it.</summary>
		private void PushPaymentToEclipboard(MobileAppDevice mobileAppDevice){
			if(PushNotificationUtils.CI_SendPayment(mobileAppDevice.MobileAppDeviceNum,_patient.PatNum, out string errorMsg)) 
			{
				MsgBox.Show($"Payment option sent to: {mobileAppDevice.DeviceName}");
			}
			else {//Error occurred
				MsgBox.Show($"Error sending payment option: {errorMsg}");
			}
		}

		///<summary>Opens a FormMobileCode window.</summary>
		private void OpenUnlockCodeForPayment(){
			MobileDataByte funcInsertDataForUnlockCode(string unlockCode) {
				MobileDataByte mobileDataByte=new MobileDataByte() {
					PatNum=_patient.PatNum,
					RawBase64Code=(unlockCode.IsNullOrEmpty()?"":Convert.ToBase64String(Encoding.UTF8.GetBytes(unlockCode))),
					ActionType=eActionType.MakePayment,
				};
				if(MobileDataBytes.Insert(mobileDataByte)>0){
					return mobileDataByte;
				}
				return null;
			}
			using(FormMobileCode formMobileCode=new FormMobileCode(funcInsertDataForUnlockCode)) {
				formMobileCode.ShowDialog();
			}
		}
		#endregion Methods - Helpers

		#region Methods - Inactive
		//private void textCC_Leave(object sender,EventArgs e) {
		//  if(FamCur==null)
		//    return;
		//  if(CCChanged) {
		//    CCSave();
		//    CCChanged=false;
		//    ModuleSelected(PatCur.PatNum);
		//  }
		//}

		//private void textCCexp_Leave(object sender,EventArgs e) {
		//  if(FamCur==null)
		//    return;
		//  if(CCChanged){
		//    CCSave();
		//    CCChanged=false;
		//    ModuleSelected(PatCur.PatNum);
		//  }
		//}

		//private void CCSave(){
		//  string cc=textCC.Text;
		//  if(Regex.IsMatch(cc,@"^\d{4}-\d{4}-\d{4}-\d{4}$")){
		//    PatientNoteCur.CCNumber=cc.Substring(0,4)+cc.Substring(5,4)+cc.Substring(10,4)+cc.Substring(15,4);
		//  }
		//  else{
		//    PatientNoteCur.CCNumber=cc;
		//  }
		//  string exp=textCCexp.Text;
		//  if(Regex.IsMatch(exp,@"^\d\d[/\- ]\d\d$")){//08/07 or 08-07 or 08 07
		//    PatientNoteCur.CCExpiration=new DateTime(Convert.ToInt32("20"+exp.Substring(3,2)),Convert.ToInt32(exp.Substring(0,2)),1);
		//  }
		//  else if(Regex.IsMatch(exp,@"^\d{4}$")){//0807
		//    PatientNoteCur.CCExpiration=new DateTime(Convert.ToInt32("20"+exp.Substring(2,2)),Convert.ToInt32(exp.Substring(0,2)),1);
		//  } 
		//  else if(exp=="") {
		//    PatientNoteCur.CCExpiration=new DateTime();//Allow the experation date to be deleted.
		//  } 
		//  else {
		//    MsgBox.Show(this,"Expiration format invalid.");
		//  }
		//  PatientNotes.Update(PatientNoteCur,PatCur.Guarantor);
		//}

		//private void FillPatientButton() {
		//	Patients.AddPatsToMenu(menuPatient,new EventHandler(menuPatient_Click),PatCur,FamCur);
		//}

		//private void textCC_TextChanged(object sender,EventArgs e) {
		//  CCChanged=true;
		//  if(Regex.IsMatch(textCC.Text,@"^\d{4}$")
		//    || Regex.IsMatch(textCC.Text,@"^\d{4}-\d{4}$")
		//    || Regex.IsMatch(textCC.Text,@"^\d{4}-\d{4}-\d{4}$")) 
		//  {
		//    textCC.Text=textCC.Text+"-";
		//    textCC.Select(textCC.Text.Length,0);
		//  }
		//}

		//private void textCCexp_TextChanged(object sender,EventArgs e) {
		//  CCChanged=true;
		//}

		/*private void butTask_Click(object sender, System.EventArgs e) {
			//FormTaskListSelect FormT=new FormTaskListSelect(TaskObjectType.Patient,PatCur.PatNum);
			//FormT.ShowDialog();
		}*/

		//private void gridProg_MouseUp(object sender,MouseEventArgs e) {
		//}
		#endregion Methods - Inactive


	}

}