using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormOrthoCase:FormODBase {
		///<summary>True only when the form is opened by adding a new OrthoCase from the Account Module.</summary>
		private bool _isNew;
		///<summary>Flag for allowing or preventing the clearing of most text boxes.</summary>
		private bool _doClearAndDisableTextFields=true;
		///<summary>Currently selected patient.</summary>
		private Patient _patCur;
		///<summary>Will be null if the Orthocase is a transfer or a banding proc has not been selected yet.</summary>
		private Procedure _bandingProc;
		///<summary>Will be null if a debond procedure has not been completed yet.</summary>
		private Procedure _debondProc;
		///<summary>List of all Visit procedure links for completed visit procs.</summary>
		private List<OrthoProcLink> _listVisitProcLinks=new List<OrthoProcLink>();
		///<summary>List of all Visit procedure completed for the OrthoCase.</summary>
		private List<Procedure> _listVisitProcs=new List<Procedure>();
		///<summary>Assigned a copy of _orthoCaseCur on load if _isNew is false.</summary>
		private OrthoCase _orthoCaseOld;
		///<summary>Null when OrthoCase is new.</summary>
		private OrthoCase _orthoCaseCur;
		///<summary>Assigned a copy of _orthoScheduleCur on load if _isNew is false.</summary>
		private OrthoSchedule _orthoScheduleOld;
		///<summary>Null when OrthoCase is new.</summary>
		private OrthoSchedule _orthoScheduleCur;
		///<summary>Assigned a copy of _schedulePlanLinkCur on load if _isNew is false.</summary>
		private OrthoPlanLink _schedulePlanLinkOld;
		///<summary>Null when OrthoCase is new.</summary>
		private OrthoPlanLink _schedulePlanLinkCur;
		///<summary>Null when OrthoCase is new or is a transfer.</summary>
		private OrthoProcLink _bandingProcLink;
		///<summary>Null when banding procedure hasn't been completed yet.</summary>
		private OrthoProcLink _debondProcLink;
		///<summary>list of proclinks that have been detached from the OrthoCase. Will be deleted if user clicks OK or Closes Case.</summary>
		private List<OrthoProcLink> _listProcLinksDetached=new List<OrthoProcLink>();
		///<summary>Used to return early from TextChanged events.</summary>
		private bool _doPreventTextChangedEvent=false;
		///<summary>Used to communicate if this case is showing secondary insurance fields</summary>
		private bool _isShowingSecondary;
		///<summary>Dynamic payment plan linked to the ortho case. Can be null.</summary>
		private PayPlan _patPayPlan;
		///<summary>OrthoPlanLink between dynamic pay plan and ortho case. Can be null.</summary>
		private OrthoPlanLink _orthoPlanLinkPatPayPlan;
		///<summary>List of all completed procedures linked to ortho case. Includes banding, debond, and all visit procs. Nulls are removed.</summary>
		private List<Procedure> _listAllCompletedProcs {
			get {
				List<Procedure> listAllLinkedProcs=new List<Procedure>(){_bandingProc,_debondProc};
				listAllLinkedProcs.AddRange(_listVisitProcs);
				listAllLinkedProcs.RemoveAll(x => x==null);
				//FUTURE TO DO: Need to change this when treatment planned procs can be added to dynamic pay plans as we will add TP'd banding procs as well.
				listAllLinkedProcs.RemoveAll(x => x.ProcStatus!=ProcStat.C);
				return listAllLinkedProcs;
			}
		}

		///<summary>Used to determine what calculation we should make for a field linked field after a textChanged event.</summary>
		private enum FieldType {
			///<summary> 0 - Either the insurance portion or the patient portion</summary>
			Fee,
			///<summary> 1- A banding, debond, or all visit percent</summary>
			Percent,
			///<summary> 2- A banding, debond, or all visit amount</summary>
			Amount,
		}

		public FormOrthoCase(bool isNew,Patient patCur,OrthoCase orthoCase=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_isNew=isNew;
			_patCur=patCur;
			_orthoCaseCur=orthoCase;
		}

		private void FormOrthoCase_Load(object sender,EventArgs e) {
			#region This label and group of text fields will be used by a future job regarding payment plans.
			labelAR.Visible=false;
			textTotalAR.Visible=false;
			textPrimaryInsAR.Visible=false;
			textSecondaryInsAR.Visible=false;
			textPatAR.Visible=false;
			#endregion
			FillGridOrthoScheduleColumns();
			//Show secondary insurance fee fields if patient has more than one insurance plan.
			DoShowSecondaryIns(PatPlans.GetPatPlansForPat(_patCur.PatNum).Count>1);
			if(_isNew) {
				_doPreventTextChangedEvent=true;
				if(!_isShowingSecondary) {
					textSecondaryInsuranceFee.Text="0.00";//Need this to be 0.00 for later calculations when not showing secondary insurance.
				}
				_doPreventTextChangedEvent=false;
				butDeleteOrthoCase.Visible=false;
				butCloseOrthoCase.Visible=false;
				butDetachProcs.Visible=false;
				return;	
			}
			//Else, existing ortho case, get data
			_orthoCaseOld=_orthoCaseCur.Copy();
			_schedulePlanLinkCur=OrthoPlanLinks.GetOneForOrthoCaseByType(_orthoCaseCur.OrthoCaseNum,OrthoPlanLinkType.OrthoSchedule);
			_schedulePlanLinkOld=_schedulePlanLinkCur.Copy();
			_orthoScheduleCur=OrthoSchedules.GetOne(_schedulePlanLinkCur.FKey);
			_orthoScheduleOld=_orthoScheduleCur.Copy();
			if(!_orthoCaseCur.IsTransfer) {
				_bandingProcLink=OrthoProcLinks.GetByType(_orthoCaseCur.OrthoCaseNum,OrthoProcType.Banding);
				//Existing Orthocases that aren't transfers should always have _bandingProcLink, so no need for _bandingProcLink null check.
				_bandingProc=Procedures.GetOneProc(_bandingProcLink.ProcNum,false);
				if(_bandingProc.ProcNum==0) {//Set to null if GetOneProc returned a blank proc.
					_bandingProc=null;
				}
			}
			_debondProcLink=OrthoProcLinks.GetByType(_orthoCaseCur.OrthoCaseNum,OrthoProcType.Debond);
			if(_debondProcLink!=null) {
				_debondProc=Procedures.GetOneProc(_debondProcLink.ProcNum,false);
				if(_debondProc.ProcNum==0) {//Set to null if GetOneProc returned a blank proc.
					_debondProc=null;
				}
			}
			_listVisitProcLinks=OrthoProcLinks.GetVisitLinksForOrthoCase(_orthoCaseCur.OrthoCaseNum);
			_listVisitProcs=Procedures.GetManyProc(_listVisitProcLinks.Select(x => x.ProcNum).ToList(),false);
			_listVisitProcs.RemoveAll(x => x.ProcNum==0);//Remove any blank procs that were added to list by GetManyProc(), mostly precautionary
			//Should only be one patient pay plan linked per ortho case.
			_orthoPlanLinkPatPayPlan=OrthoPlanLinks.GetOneForOrthoCaseByType(_orthoCaseCur.OrthoCaseNum,OrthoPlanLinkType.PatPayPlan);
			//Set Fields, buttons, and Grid
			checkIsTransfer.Checked=_orthoCaseCur.IsTransfer;//Have to set this control here so that SetExistingOrthoCaseControls() can use checked state.
			SetExistingOrthoCaseControls();
			RefreshGridOrthoScheduleRows();
		}

		private void FillGridOrthoScheduleColumns() {
			gridOrthoSchedule.BeginUpdate();
			gridOrthoSchedule.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TableOrthoSchedule","Procedure"),80);
			gridOrthoSchedule.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableOrthoSchedule","Percent"),60,HorizontalAlignment.Right);
			gridOrthoSchedule.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableOrthoSchedule","Amount"),80,HorizontalAlignment.Right);
			gridOrthoSchedule.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableOrthoSchedule","Date Completed"),90,HorizontalAlignment.Right){ IsWidthDynamic=true };
			gridOrthoSchedule.ListGridColumns.Add(col);
			gridOrthoSchedule.EndUpdate();
		}

		private void SetExistingOrthoCaseControls() {
			_doPreventTextChangedEvent=true;
			//Is Transfer
			if(checkIsTransfer.Checked) {
				labelBandingDate.Text=Lans.g(this,"Transfer Date");
			}
			else {
				labelBandingDate.Text=Lans.g(this,"Banding Date");
				FillTextBandingProc();
				textBandingAmount.Text=_orthoScheduleCur.BandingAmount.ToString("f");
				textBandingPercent.Text=(_orthoScheduleCur.BandingAmount/_orthoCaseCur.Fee*100).ToString("f");
			}
			//Fee Details
			//Pat may not have secondary insurance, but we want to show secondary ins fee if it was entered previously when they did have secondary ins.
			if(CompareDecimal.IsGreaterThanZero(_orthoCaseCur.FeeInsSecondary)) {
				DoShowSecondaryIns(true);
			}
			textTotalFee.Text=_orthoCaseCur.Fee.ToString("f");
			textPrimaryInsuranceFee.Text=_orthoCaseCur.FeeInsPrimary.ToString("f");
			textSecondaryInsuranceFee.Text=_orthoCaseCur.FeeInsSecondary.ToString("f");
			textPatientFee.Text=_orthoCaseCur.FeePat.ToString("f");
			double totalCompleted=_listVisitProcs.Sum(x => x.ProcFeeTotal);
			if(_bandingProc!=null && _bandingProc.ProcStatus==ProcStat.C) {
				totalCompleted+=_bandingProc.ProcFeeTotal;
			}
			if(_debondProc!=null && _debondProc.ProcStatus==ProcStat.C) {
				totalCompleted+=_debondProc.ProcFeeTotal;
			}
			textTotalCompleted.Text=totalCompleted.ToString("f");
			textTotalRemaining.Text=(_orthoCaseCur.Fee-PIn.Double(textTotalCompleted.Text)).ToString("f");
			textPrimaryInsCompleted.Text=(totalCompleted*_orthoCaseCur.FeeInsPrimary/_orthoCaseCur.Fee).ToString("f");
			textPrimaryInsRemaining.Text=(_orthoCaseCur.FeeInsPrimary-PIn.Double(textPrimaryInsCompleted.Text)).ToString("f");
			textSecondaryInsCompleted.Text=(totalCompleted*_orthoCaseCur.FeeInsSecondary/_orthoCaseCur.Fee).ToString("f");
			textSecondaryInsRemaining.Text=(_orthoCaseCur.FeeInsSecondary-PIn.Double(textSecondaryInsCompleted.Text)).ToString("f");
			double insTotalCompleted=PIn.Double(textPrimaryInsCompleted.Text)+PIn.Double(textSecondaryInsCompleted.Text);
			textPatCompleted.Text=(totalCompleted-insTotalCompleted).ToString("f");
			textPatRemaining.Text=(_orthoCaseCur.FeePat-PIn.Double(textPatCompleted.Text)).ToString("f");
			//Procedure Breakdown
			textDebondAmount.Text=_orthoScheduleCur.DebondAmount.ToString("f");
			textAllVisitsAmount.Text=(_orthoCaseCur.Fee-_orthoScheduleCur.BandingAmount-_orthoScheduleCur.DebondAmount).ToString("f");
			textDebondPercent.Text=(_orthoScheduleCur.DebondAmount/_orthoCaseCur.Fee*100).ToString("f");
			textAllVisitsPercent.Text=(PIn.Double(textAllVisitsAmount.Text)/_orthoCaseCur.Fee*100).ToString("f");
			//Visit Details
			textVisitAmount.Text=_orthoScheduleCur.VisitAmount.ToString("f");
			textVisitPercent.Text=(_orthoScheduleCur.VisitAmount/_orthoCaseCur.Fee*100).ToString("f");
			textVisitCountPlanned.Text=OrthoSchedules.CalculatePlannedVisitsCount(_orthoScheduleCur.BandingAmount,_orthoScheduleCur.DebondAmount
				,_orthoScheduleCur.VisitAmount,_orthoCaseCur.Fee).ToString();
			TextVisitCountComplete.Text=_listVisitProcs.Count.ToString();
			if(_orthoScheduleCur.VisitAmount==0) {//Set visits planned to zero if we divided by 0 above.
				textVisitCountPlanned.Text="0";
			}
			//Dates
			textBandingDate.Text=_orthoCaseCur.BandingDate.ToShortDateString();
			textExpectedDebondDate.Text=_orthoCaseCur.DebondDateExpected.ToShortDateString();
			labelExpectedDebondDate.Text=Lans.g(this,"Expected Debond Date");
			labelTreatmentLength.Text=Lans.g(this,"Expected days of treatment");
			if(_debondProc!=null && _debondProc.ProcStatus==ProcStat.C) {
				textExpectedDebondDate.Text=_orthoCaseCur.DebondDate.ToShortDateString();
				labelExpectedDebondDate.Text=Lans.g(this,"Debond Date");
				labelTreatmentLength.Text=Lans.g(this,"Days of treatment");
			}
			textTreatmentLength.Text=(PIn.Date(textExpectedDebondDate.Text)-PIn.Date(textBandingDate.Text)).Days.ToString();
			EnableControls();
			_doPreventTextChangedEvent=false;
		}

		private void EnableControls() {
			checkIsTransfer.Enabled=true;
			butAddBandingProcedure.Enabled=true;
			EnableTextFields();
			textBandingDate.Enabled=true;
			textExpectedDebondDate.Enabled=true;
			butDeleteOrthoCase.Visible=true;
			butDetachProcs.Visible=true;
			butCloseOrthoCase.Visible=true;
			if(!_orthoCaseCur.IsActive) {
				checkIsTransfer.Enabled=false;
				butAddBandingProcedure.Enabled=false;
				DisableTextFields();
				textBandingDate.Enabled=false;
				textExpectedDebondDate.Enabled=false;
				butDeleteOrthoCase.Visible=false;
				butDetachProcs.Visible=false;
				butCloseOrthoCase.Visible=false;
				return;
			}
			if(_listVisitProcs.Count>0 || (_debondProc!=null && _debondProc.ProcStatus==ProcStat.C) 
					|| (_bandingProc!=null && _bandingProc.ProcStatus==ProcStat.C)) 
			{
				checkIsTransfer.Enabled=false;
				butAddBandingProcedure.Enabled=false;
				DisableTextFields();
				textBandingDate.Enabled=false;
				if(_debondProc!=null && _debondProc.ProcStatus==ProcStat.C) {
					textExpectedDebondDate.Enabled=false;
				}
			}
			else {
				butCloseOrthoCase.Visible=false;
			}
		}

		private void RefreshGridOrthoScheduleRows() {
			gridOrthoSchedule.BeginUpdate();
			gridOrthoSchedule.ListGridRows.Clear();
			if((textBandingAmount.Enabled && textBandingAmount.Text=="") || (textBandingPercent.Enabled && textBandingPercent.Text=="")
				|| textTotalFee.Text=="" || textAllVisitsAmount.Text=="" || textDebondAmount.Text=="" || textAllVisitsPercent.Text==""
				|| textDebondPercent.Text=="" || textVisitPercent.Text=="" || textVisitAmount.Text=="" || textVisitCountPlanned.Text=="") {
				gridOrthoSchedule.EndUpdate();
				return;
			}
			//Add Banding
			GridRow row;
			string notCompleteText=Lans.g("TableOrthoSchedule","Not Complete");
			string dateCompleted=notCompleteText;
			if(!checkIsTransfer.Checked) {
				row=new GridRow();
				row.Cells.Add(Lans.g("TableOrthoSchedule","Banding"));
				row.Cells.Add(textBandingPercent.Text);
				row.Cells.Add(textBandingAmount.Text);
				if(_bandingProc!=null && _bandingProc.ProcStatus==ProcStat.C) {
					dateCompleted=_bandingProc.ProcDate.ToShortDateString();
					row.ColorText=Color.Green;
					row.Tag=_bandingProc;
				}
				row.Cells.Add(dateCompleted);
				gridOrthoSchedule.ListGridRows.Add(row);
			}
			//Add Visits
			int visitRowCount=Math.Max(PIn.Int(textVisitCountPlanned.Text,false),_listVisitProcs.Count);
			for(int i=0;i<visitRowCount;i++) {
				row=new GridRow();
				row.Cells.Add(Lans.g("TableOrthoSchedule","Visit"));
				if(i+1<PIn.Int(textVisitCountPlanned.Text,false)) {
					row.Cells.Add(PIn.Double(textVisitPercent.Text).ToString("f"));
					row.Cells.Add(PIn.Double(textVisitAmount.Text).ToString("f"));
				}
				else if(i+1==PIn.Int(textVisitCountPlanned.Text,false)) {
					double lastVisitAmount=PIn.Double(textAllVisitsAmount.Text)-i*PIn.Double(textVisitAmount.Text);
					double lastVisitPercent=lastVisitAmount/PIn.Double(textTotalFee.Text)*100;
					row.Cells.Add(lastVisitPercent.ToString("f"));
					row.Cells.Add(lastVisitAmount.ToString("f"));
				}
				else {
					row.Cells.Add(0.ToString());
					row.Cells.Add(0.ToString("f"));
				}
				dateCompleted=notCompleteText;
				if(i<_listVisitProcs.Count) {
					dateCompleted=_listVisitProcs[i].ProcDate.ToShortDateString();
					row.ColorText=Color.Green;
					row.Tag=_listVisitProcs[i];
				}
				row.Cells.Add(dateCompleted);
				gridOrthoSchedule.ListGridRows.Add(row);
			}
			//Add Debond
			row=new GridRow();
			row.Cells.Add(Lans.g("TableOrthoSchedule","Debond"));
			row.Cells.Add(textDebondPercent.Text);
			row.Cells.Add(textDebondAmount.Text);
			dateCompleted=notCompleteText;
			if(_debondProc!=null && _debondProc.ProcStatus==ProcStat.C) {
				dateCompleted=_debondProc.ProcDate.ToShortDateString();
				row.ColorText=Color.Green;
				row.Tag=_debondProc;
			}
			row.Cells.Add(dateCompleted);
			gridOrthoSchedule.ListGridRows.Add(row);
			gridOrthoSchedule.EndUpdate();
		}

		private void ButAddBandingProcedure_Click(object sender,EventArgs e) {
			using FormProcBandingSelect formProcBandingSelect=new FormProcBandingSelect(_patCur.PatNum);
			if(formProcBandingSelect.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_bandingProc=formProcBandingSelect.SelectedProcedure;
			FillTextBandingProc();
			_doClearAndDisableTextFields=false;//Prevent text fields from being cleared if checkIsTransfer switches from checked to unchecked.
			checkIsTransfer.Checked=false;//Uncheck IsTransfer box as we cannot have a banding proc attached to a transfer.
			_doClearAndDisableTextFields=true;
			EnableTextFields();
		}

		private void FillTextBandingProc() {
			if(_bandingProc==null) {
				return;
			}
			ProcedureCode procCode=ProcedureCodes.GetProcCode(_bandingProc.CodeNum);
			textBandingProc.Text=procCode.ProcCode+" - "+procCode.AbbrDesc;
		}

		private void CheckIsTransfer_CheckedChanged(object sender,EventArgs e) {
			_doPreventTextChangedEvent=true;
			if(checkIsTransfer.Checked) {
				_bandingProc=null;
				textBandingProc.Text="";
				textBandingAmount.Text="";
				textBandingPercent.Text="";
				EnableTextFields();
				textBandingAmount.Enabled=false;
				textBandingPercent.Enabled=false;
				labelBandingDate.Text=Lans.g(this,"Transfer Date");
			}
			else {
				labelBandingDate.Text=Lans.g(this,"Banding Date");
				if(_doClearAndDisableTextFields) {//Only want to clear text fields and disable them if we have unchecked this without adding a banding proc.
					ClearTextFields();
					DisableTextFields();
				}
			}
			RefreshGridOrthoScheduleRows();
			SetAmountAndFeeFieldColors();
			_doPreventTextChangedEvent=false;
		}

		#region Leave Methods
		private void TextTotalFee_Leave(object sender,EventArgs e) {
			textTotalFee.Text=PIn.Double(textTotalFee.Text).ToString("f");
		}

		private void TextPrimaryInsuranceFee_Leave(object sender,EventArgs e) {
			textPrimaryInsuranceFee.Text=PIn.Double(textPrimaryInsuranceFee.Text).ToString("f");
		}

		private void TextSecondaryInsuranceFee_Leave(object sender,EventArgs e) {
			textSecondaryInsuranceFee.Text=PIn.Double(textSecondaryInsuranceFee.Text).ToString("f");
		}

		private void TextPatientFee_Leave(object sender,EventArgs e) {
			textPatientFee.Text=PIn.Double(textPatientFee.Text).ToString("f");
		}

		private void TextBandingAmount_Leave(object sender,EventArgs e) {
			textBandingAmount.Text=PIn.Double(textBandingAmount.Text).ToString("f");
		}

		private void TextDebondAmount_Leave(object sender,EventArgs e) {
			textDebondAmount.Text=PIn.Double(textDebondAmount.Text).ToString("f");
		}

		private void TextAllVisitsAmount_Leave(object sender,EventArgs e) {
			textAllVisitsAmount.Text=PIn.Double(textAllVisitsAmount.Text).ToString("f");
		}

		private void TextBandingPercent_Leave(object sender,EventArgs e) {
			textBandingPercent.Text=PIn.Double(textBandingPercent.Text).ToString("f");
		}

		private void TextDebondPercent_Leave(object sender,EventArgs e) {
			textDebondPercent.Text=PIn.Double(textDebondPercent.Text).ToString("f");
		}

		private void TextAllVisitsPercent_Leave(object sender,EventArgs e) {
			textAllVisitsPercent.Text=PIn.Double(textAllVisitsPercent.Text).ToString("f");
		}

		private void TextVisitCountPlanned_Leave(object sender,EventArgs e) {
			textVisitCountPlanned.Text=PIn.Int(textVisitCountPlanned.Text,false).ToString();
		}

		private void TextVisitPercent_Leave(object sender,EventArgs e) {
			textVisitPercent.Text=PIn.Double(textVisitPercent.Text).ToString("f");
		}

		private void TextVisitAmount_Leave(object sender,EventArgs e) {
			textVisitAmount.Text=PIn.Double(textVisitAmount.Text).ToString("f");
		}
		#endregion Leave Methods

		#region TextChanged Methods
		private void TextTotalFee_TextChanged(object sender,EventArgs e) {
			if(_doPreventTextChangedEvent) {
				return;
			}
			_doPreventTextChangedEvent=true;
			ClearSubordinateFields(textTotalFee);
			_doPreventTextChangedEvent=false;
		}

		private void TextPrimaryInsuranceFee_TextChanged(object sender,EventArgs e) {
			TextFieldChangedHelper(textPrimaryInsuranceFee,PIn.Double(textTotalFee.Text),textPatientFee,FieldType.Fee,textSecondaryInsuranceFee);
		}

		private void TextSecondaryInsuranceFee_TextChanged(object sender,EventArgs e) {
			TextFieldChangedHelper(textSecondaryInsuranceFee,PIn.Double(textTotalFee.Text),textPatientFee,FieldType.Fee,textPrimaryInsuranceFee);
		}

		private void TextPatientFee_TextChanged(object sender,EventArgs e) {
			TextFieldChangedHelper(textPatientFee,PIn.Double(textTotalFee.Text),textPrimaryInsuranceFee,FieldType.Fee,textSecondaryInsuranceFee);
		}

		private void TextBandingPercent_TextChanged(object sender,EventArgs e) {
			TextFieldChangedHelper(textBandingPercent,100,textBandingAmount,FieldType.Amount);
		}

		private void TextDebondPercent_TextChanged(object sender,EventArgs e) {
			TextFieldChangedHelper(textDebondPercent,100,textDebondAmount,FieldType.Amount);
		}

		private void TextAllVisitsPercent_TextChanged(object sender,EventArgs e) {
			TextFieldChangedHelper(textAllVisitsPercent,100,textAllVisitsAmount,FieldType.Amount);
		}

		private void TextBandingAmount_TextChanged(object sender,EventArgs e) {
			TextFieldChangedHelper(textBandingAmount,PIn.Double(textTotalFee.Text),textBandingPercent,FieldType.Percent);
		}

		private void TextDebondAmount_TextChanged(object sender,EventArgs e) {
			TextFieldChangedHelper(textDebondAmount,PIn.Double(textTotalFee.Text),textDebondPercent,FieldType.Percent);
		}

		private void TextAllVisitsAmount_TextChanged(object sender,EventArgs e) {
			TextFieldChangedHelper(textAllVisitsAmount,PIn.Double(textTotalFee.Text),textAllVisitsPercent,FieldType.Percent);
		}

		private void TextVisitCountPlanned_TextChanged(object sender,EventArgs e) {
			if(_doPreventTextChangedEvent) {
				return;
			}
			_doPreventTextChangedEvent=true;
			int visitCount=PIn.Int(textVisitCountPlanned.Text,false);
			if(visitCount<0) {
				textVisitPercent.Text="";
				textVisitAmount.Text="";
				textVisitCountPlanned.ForeColor=Color.Red;
			}
			else if(visitCount==0) {
				textVisitPercent.Text=0.ToString();
				textVisitAmount.Text=0.ToString("f");
				SetVisitFieldColors(Color.Black);
				if(PIn.Double(textAllVisitsAmount.Text)!=0) {
					textVisitCountPlanned.ForeColor=Color.Red;
				}
			}
			else {
				double allVisitsAmount=PIn.Double(textAllVisitsAmount.Text);
				textVisitAmount.Text=(allVisitsAmount/visitCount).ToString("f");
				textVisitPercent.Text=(PIn.Double(textVisitAmount.Text)/PIn.Double(textTotalFee.Text)*100).ToString("f");
				SetVisitFieldColors(Color.Black);
				if(CompareDouble.IsZero(PIn.Double(textVisitAmount.Text))) {
					textVisitPercent.Text="";
					textVisitAmount.Text="";
					textVisitCountPlanned.ForeColor=Color.Red;
				}
			}
			RefreshGridOrthoScheduleRows();
			_doPreventTextChangedEvent=false;
		}

		private void TextVisitPercent_TextChanged(object sender,EventArgs e) {
			if(_doPreventTextChangedEvent) {
				return;
			}
			_doPreventTextChangedEvent=true;
			double visitPercent=PIn.Double(textVisitPercent.Text);
			double allVisitsPercent=PIn.Double(textAllVisitsPercent.Text);
			if(CompareDouble.IsLessThan(visitPercent,0) || CompareDouble.IsGreaterThan(visitPercent,allVisitsPercent)) {
				textVisitCountPlanned.Text="";
				textVisitAmount.Text="";
				textVisitPercent.ForeColor=Color.Red;
			}
			else if(visitPercent==0) {
				textVisitCountPlanned.Text="0";
				textVisitAmount.Text=0.ToString("f");
				SetVisitFieldColors(Color.Black);
				if(PIn.Double(textAllVisitsAmount.Text)!=0) {
					textVisitPercent.ForeColor=Color.Red;
				}
			}
			else {
				double totalFee=PIn.Double(textTotalFee.Text);
				textVisitAmount.Text=(totalFee*visitPercent/100).ToString("f");
				textVisitCountPlanned.Text=OrthoSchedules.CalculatePlannedVisitsCount(PIn.Double(textBandingAmount.Text),PIn.Double(textDebondAmount.Text)
					,PIn.Double(textVisitAmount.Text),PIn.Double(textTotalFee.Text)).ToString();
				SetVisitFieldColors(Color.Black);
			}
			RefreshGridOrthoScheduleRows();
			_doPreventTextChangedEvent=false;
		}

		private void TextVisitAmount_TextChanged(object sender,EventArgs e) {
			if(_doPreventTextChangedEvent) {
				return;
			}
			_doPreventTextChangedEvent=true;
			double visitAmount=PIn.Double(textVisitAmount.Text);
			double allVisitsAmount=PIn.Double(textAllVisitsAmount.Text);
			if(CompareDouble.IsLessThan(visitAmount,0) || CompareDouble.IsGreaterThan(visitAmount,allVisitsAmount)) {
				textVisitCountPlanned.Text="";
				textVisitPercent.Text="";
				textVisitAmount.ForeColor=Color.Red;
			}
			else if(visitAmount==0) {
				textVisitCountPlanned.Text="0";
				textVisitPercent.Text=0.ToString();
				SetVisitFieldColors(Color.Black);
				if(PIn.Double(textAllVisitsAmount.Text)!=0) {
					textVisitAmount.ForeColor=Color.Red;
				}
			}
			else {
				double totalFee=PIn.Double(textTotalFee.Text);
				textVisitPercent.Text=(visitAmount/totalFee*100).ToString("f");
				textVisitCountPlanned.Text=OrthoSchedules.CalculatePlannedVisitsCount(PIn.Double(textBandingAmount.Text),PIn.Double(textDebondAmount.Text)
					,PIn.Double(textVisitAmount.Text),PIn.Double(textTotalFee.Text)).ToString();
				SetVisitFieldColors(Color.Black);
			}
			RefreshGridOrthoScheduleRows();
			_doPreventTextChangedEvent=false;
		}

		private void TextBandingDate_TextChanged(object sender,EventArgs e) {
			SetDateFieldColors();
		}

		private void TextExpectedDebondDate_TextChanged(object sender,EventArgs e) {
			SetDateFieldColors();
		}
		#endregion TextChanged Methods

		///<summary>Updates the field linked to the changed field. Sets field color red where there are errors. Clears subordinate fields.</summary>
		///<param name="secondLinkedField">Specifically for fields of the Fee Details group box. Behavior changes when secondary ins is visible.</param>
		private void TextFieldChangedHelper(ValidDouble changedField,double changedFieldLimit,ValidDouble linkedField,FieldType linkedFieldType
			,ValidDouble secondLinkedField=null)
		{
			if(_doPreventTextChangedEvent) {
				return;
			}
			_doPreventTextChangedEvent=true;
			double totalFee=PIn.Double(textTotalFee.Text);
			double changedFieldNumber=PIn.Double(changedField.Text);
			if(CompareDecimal.IsGreaterThanOrEqualToZero(changedFieldNumber) && changedFieldNumber<=changedFieldLimit) {
				switch(linkedFieldType){
					case FieldType.Fee:
						if(!_isShowingSecondary) {//Can only auto-calculate a linked Fee field when we are working with one insurance plan.
							linkedField.Text=(totalFee-changedFieldNumber).ToString("f");
						}
						break;
					case FieldType.Percent:
						linkedField.Text=(changedFieldNumber/totalFee*100).ToString("f");
						break;
					case FieldType.Amount:
						linkedField.Text=(totalFee*changedFieldNumber/100).ToString("f");
						break;
				}
			}
			else {
				linkedField.Text="";
				//We should only clear both linked fields if we are working with both primary and secondary insurance.
				if(_isShowingSecondary && secondLinkedField!=null) {
					secondLinkedField.Text="";
				}
			}
			ClearSubordinateFields(changedField);
			SetAmountAndFeeFieldColors();
			_doPreventTextChangedEvent=false;
		}

		private void EnableTextFields() {
			textTotalFee.Enabled=true;
			textPrimaryInsuranceFee.Enabled=true;
			textSecondaryInsuranceFee.Enabled=true;
			textPatientFee.Enabled=true;
			if(!checkIsTransfer.Checked) {//If this isn't a transfer, enable banding fields.
				textBandingPercent.Enabled=true;
				textBandingAmount.Enabled=true;
			}
			textDebondPercent.Enabled=true;
			textDebondAmount.Enabled=true;
			textAllVisitsPercent.Enabled=true;
			textAllVisitsAmount.Enabled=true;
			textVisitCountPlanned.Enabled=true;
			textVisitPercent.Enabled=true;
			textVisitAmount.Enabled=true;
		}

		private void DisableTextFields() {
			textTotalFee.Enabled=false;
			textPrimaryInsuranceFee.Enabled=false;
			textSecondaryInsuranceFee.Enabled=false;
			textPatientFee.Enabled=false;
			textBandingPercent.Enabled=false;
			textBandingAmount.Enabled=false;
			textDebondPercent.Enabled=false;
			textDebondAmount.Enabled=false;
			textAllVisitsPercent.Enabled=false;
			textAllVisitsAmount.Enabled=false;
			textVisitCountPlanned.Enabled=false;
			textVisitPercent.Enabled=false;
			textVisitAmount.Enabled=false;
		}

		private void ClearTextFields() {
			textTotalFee.Text="";
			textPrimaryInsuranceFee.Text="";
			if(_isShowingSecondary) {
				textSecondaryInsuranceFee.Text="";//Keep the 0.00 for future calculations
			}
			textPatientFee.Text="";
			textBandingPercent.Text="";
			textBandingAmount.Text="";
			textDebondPercent.Text="";
			textDebondAmount.Text="";
			textAllVisitsPercent.Text="";
			textAllVisitsAmount.Text="";
			textVisitCountPlanned.Text="";
			textVisitPercent.Text="";
			textVisitAmount.Text="";
		}

		private void DoShowSecondaryIns(bool doShowSecondaryIns) {
			labelSecondaryInsuranceFee.Visible=doShowSecondaryIns;
			textSecondaryInsuranceFee.Visible=doShowSecondaryIns;
			textSecondaryInsCompleted.Visible=doShowSecondaryIns;
			textSecondaryInsRemaining.Visible=doShowSecondaryIns;
			//textSecondaryInsAR.Visible=doShowSecondaryIns; uncomment when doing job to integrate payment plans.
			_isShowingSecondary=doShowSecondaryIns;
		}

		///<summary>This form is intended to be filled from top to bottom. To encourage this and to prevent some amounts from being invalid,
		///we clear certain fields when a field they depend on has changed.</summary>
		private void ClearSubordinateFields(object sender) {
			if(sender==textTotalFee || sender==textBandingAmount || sender==textBandingPercent || sender==textDebondAmount || sender==textDebondPercent
				|| sender==textAllVisitsAmount || sender==textAllVisitsPercent) 
			{
				textVisitCountPlanned.Text="";
				textVisitPercent.Text="";
				textVisitAmount.Text="";
			}
			if(sender==textTotalFee) {
				textPrimaryInsuranceFee.Text="";
				if(_isShowingSecondary) {//Keep the 0.00 for future calculations
					textSecondaryInsuranceFee.Text="";
				}
				textPatientFee.Text="";
				textBandingPercent.Text="";
				textBandingAmount.Text="";
				textDebondPercent.Text="";
				textDebondAmount.Text="";
				textAllVisitsPercent.Text="";
				textAllVisitsAmount.Text="";
			}
			gridOrthoSchedule.BeginUpdate();
			gridOrthoSchedule.ListGridRows.Clear();
			gridOrthoSchedule.EndUpdate();
		}

		private void SetDateFieldColors() {
			if(PIn.Date(textBandingDate.Text)>PIn.Date(textExpectedDebondDate.Text)) {
				textBandingDate.ForeColor=Color.Red;
				textExpectedDebondDate.ForeColor=Color.Red;
				return;
			}
			textBandingDate.ForeColor=Color.Black;
			textExpectedDebondDate.ForeColor=Color.Black;
			textTreatmentLength.Text=(PIn.Date(textExpectedDebondDate.Text)-PIn.Date(textBandingDate.Text)).Days.ToString();
		}

		private void SetAmountAndFeeFieldColors() {
			double sumAmountFields=PIn.Double(textBandingAmount.Text)+PIn.Double(textDebondAmount.Text)+PIn.Double(textAllVisitsAmount.Text);
			if(CompareDouble.IsEqual(sumAmountFields,PIn.Double(textTotalFee.Text))) {
				textBandingAmount.ForeColor=Color.Black;
				textDebondAmount.ForeColor=Color.Black;
				textAllVisitsAmount.ForeColor=Color.Black;
			}
			else {
				textBandingAmount.ForeColor=Color.Red;
				textDebondAmount.ForeColor=Color.Red;
				textAllVisitsAmount.ForeColor=Color.Red;
			}
			double sumFeeFields=PIn.Double(textPrimaryInsuranceFee.Text)+PIn.Double(textSecondaryInsuranceFee.Text)+PIn.Double(textPatientFee.Text);
			if(CompareDouble.IsEqual(sumFeeFields,PIn.Double(textTotalFee.Text))) {
				textPrimaryInsuranceFee.ForeColor=Color.Black;
				textSecondaryInsuranceFee.ForeColor=Color.Black;
				textPatientFee.ForeColor=Color.Black;
			}
			else {
				textPrimaryInsuranceFee.ForeColor=Color.Red;
				textSecondaryInsuranceFee.ForeColor=Color.Red;
				textPatientFee.ForeColor=Color.Red;
			}
		}

		private void SetVisitFieldColors(Color color) {
			textVisitCountPlanned.ForeColor=color;
			textVisitPercent.ForeColor=color;
			textVisitAmount.ForeColor=color;
		}

		///<summary>Selects all rows at and below the selected row.</summary>
		private void GridOrthoSchedule_CellClick(object sender,ODGridClickEventArgs e) {
			SelectOrthoGridRowsHelper();
		}

		private void gridOrthoSchedule_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			SelectOrthoGridRowsHelper();
		}

		///<summary>Ortho grid rows should never have only 1 grid row selected. This is because when detaching procedures, all procedures
		///under the one selected need to be removed as well.</summary>
		private void SelectOrthoGridRowsHelper() {
			if(gridOrthoSchedule.SelectedGridRows.Count<=0) {
				return;
			}
			int indexSelectedRow=gridOrthoSchedule.ListGridRows.IndexOf(gridOrthoSchedule.SelectedGridRows[0]);
			gridOrthoSchedule.SelectionMode=GridSelectionMode.MultiExtended;
			for(int i = indexSelectedRow+1;i<gridOrthoSchedule.ListGridRows.Count;i++) {
				gridOrthoSchedule.SelectedGridRows.Add(gridOrthoSchedule.ListGridRows[i]);
				gridOrthoSchedule.SetSelected(i,true);
			}
			gridOrthoSchedule.SelectionMode=GridSelectionMode.One;
		}

		private void ButDetachProc_Click(object sender,EventArgs e) {
			if(gridOrthoSchedule.SelectedGridRows.Where(x => x.Tag!=null).ToList().Count<=0) {//Return if no selected rows have an attached procedure.
				return;
			}
			//If row is bandingProc prompt for deletion of orthocase
			if(_bandingProc!=null && ((Procedure)gridOrthoSchedule.SelectedGridRows[0].Tag).ProcNum==_bandingProc.ProcNum) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Detaching the banding procedure requires that the ortho case be deleted. " +
					"Do you want to delete this ortho case?")) 
				{
					return;
				}
				try {
					OrthoCases.Delete(_orthoCaseCur.OrthoCaseNum,_orthoScheduleCur,_schedulePlanLinkCur
						,OrthoProcLinks.GetManyByOrthoCase(_orthoCaseCur.OrthoCaseNum),_orthoPlanLinkPatPayPlan);
					DialogResult=DialogResult.OK;
					return;
				}
				catch(ApplicationException ex) {
					FriendlyException.Show(ex.Message,ex);
					return;
				}
			}
			//Else detach all procedures for selected rows.
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,$"Detach all procedures for the selected rows?")) 
			{
				return;
			}
			foreach(GridRow row in gridOrthoSchedule.SelectedGridRows) {
				if(row.Tag!=null) {
					DetachProcedure(((Procedure)row.Tag).ProcNum);
				}
			}
			SetExistingOrthoCaseControls();
			RefreshGridOrthoScheduleRows();
		}

		private void DetachProcedure(long procNum) {
			if(_debondProc!=null && _debondProc.ProcNum==procNum) {//detching debond
				_debondProc=null;
				_listProcLinksDetached.Add(_debondProcLink);
				_debondProcLink=null;
			}
			else {
				_listProcLinksDetached.AddRange(_listVisitProcLinks.Where(x => x.ProcNum==procNum).ToList());
				_listVisitProcLinks.RemoveAll(x => x.ProcNum==procNum);
				_listVisitProcs.RemoveAll(x => x.ProcNum==procNum);
			}
		}

		private bool HasErrors() {
			if(PIn.Date(textBandingDate.Text) > PIn.Date(textExpectedDebondDate.Text) && (textBandingDate.Enabled || textExpectedDebondDate.Enabled)) {
				MsgBox.Show(this,"Expected Debond Date cannot be earlier than Banding Date.");
				return true;
			}
			double sumFeeFields=PIn.Double(textPrimaryInsuranceFee.Text)+PIn.Double(textSecondaryInsuranceFee.Text)+PIn.Double(textPatientFee.Text);
			if(!CompareDouble.IsEqual(sumFeeFields,PIn.Double(textTotalFee.Text))) {
				MsgBox.Show(this,"Sum of Patient Portion and insurance fees must equal Total Fee.");
				return true;
			}
			double amountFieldsTotal=PIn.Double(textBandingAmount.Text)+PIn.Double(textDebondAmount.Text)+PIn.Double(textAllVisitsAmount.Text);
			if(!(CompareDouble.IsEqual(amountFieldsTotal,PIn.Double(textTotalFee.Text)))) {
				MsgBox.Show(this,"Sum of Banding, Debond, and All Visits amounts must equal the Total Fee.");
				return true;
			}
			if(CompareDouble.IsGreaterThan(PIn.Double(textVisitAmount.Text),PIn.Double(textAllVisitsAmount.Text)) 
				|| PIn.Double(textVisitPercent.Text)>PIn.Double(textAllVisitsPercent.Text)) 
			{
				MsgBox.Show(this,"Percent or amount per visit cannot exceed percent or amount for all visits.");
				return true;
			}
			if(_bandingProc!=null && checkIsTransfer.Checked) {
				MsgBox.Show(this,"Transfers cannot have a banding procedure.");
				return true;
			}
			if(!textTotalFee.IsValid() 
				|| !textPrimaryInsuranceFee.IsValid()
				|| !textSecondaryInsuranceFee.IsValid()
				|| !textPatientFee.IsValid() 
				|| !textBandingAmount.IsValid()
				|| !textBandingPercent.IsValid()
				|| !textDebondAmount.IsValid()
				|| !textDebondPercent.IsValid()
				|| !textAllVisitsAmount.IsValid()
				|| !textAllVisitsPercent.IsValid() 
				|| !textBandingDate.IsValid()
				|| !textExpectedDebondDate.IsValid() 
				|| !textVisitCountPlanned.IsValid()
				|| !textVisitPercent.IsValid() 
				|| !textVisitAmount.IsValid()) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return true;
			}
			if(PIn.Int(textVisitCountPlanned.Text,false)<1 && PIn.Double(textAllVisitsAmount.Text)!=0) {
				MsgBox.Show(this,"Number of Visits cannot be zero if an amount is entered for All Visits.");
				return true;
			}
			if(textTotalFee.Text=="" 
				|| textPrimaryInsuranceFee.Text==""
				|| textSecondaryInsuranceFee.Text==""
				|| textPatientFee.Text=="" 
				|| textBandingDate.Text=="" 
				|| textExpectedDebondDate.Text==""
				|| (textBandingAmount.Enabled && textBandingAmount.Text=="") 
				|| (textBandingPercent.Enabled && textBandingPercent.Text=="")
				|| textDebondPercent.Text=="" 
				|| textDebondAmount.Text=="" 
				|| textAllVisitsPercent.Text=="" 
				|| textAllVisitsAmount.Text==""
				|| textVisitCountPlanned.Text=="" 
				|| textVisitPercent.Text=="" 
				|| textVisitAmount.Text=="") 
			{
				MsgBox.Show(this,"All fields must be completed.");
				return true;
			}
			return false;
		}

		private void ButPatPayPlan_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.PayPlanEdit)) {
				return;
			}
			if(_patPayPlan!=null) {
				_patPayPlan=PayPlans.GetOne(_patPayPlan.PayPlanNum);//Refresh if we are opening the pay plan again, in case it was deleted.
			}
			else if(_patPayPlan==null &&_orthoPlanLinkPatPayPlan!=null) {//Get from link if we have not opened pay plan yet.
				_patPayPlan=PayPlans.GetOne(_orthoPlanLinkPatPayPlan.FKey);
			}
			if(_patPayPlan==null) {//If pay plan is null it was never created or it was deleted since the window opened.
				_orthoPlanLinkPatPayPlan=null;
				CreateNewPayPlan();
			}
			using FormPayPlanDynamic formPayPlanDynamic=new FormPayPlanDynamic(_patPayPlan,false,_listAllCompletedProcs);
			formPayPlanDynamic.ShowDialog();
			if(formPayPlanDynamic.DialogResult==DialogResult.Cancel && _patPayPlan.IsNew) {
				try {
					PayPlans.Delete(_patPayPlan);
					_patPayPlan=null;
					_orthoPlanLinkPatPayPlan=null;
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					return;
				}
			}
			else {
				_patPayPlan=PayPlans.GetOne(_patPayPlan.PayPlanNum);
				if(_patPayPlan==null) {//In case pay plan was deleted from form.
					_orthoPlanLinkPatPayPlan=null;
				}
			}
		}

		private void CreateNewPayPlan() {
			_patPayPlan=new PayPlan();
			_patPayPlan.IsNew=true;
			_patPayPlan.PatNum=_patCur.PatNum;
			_patPayPlan.Guarantor=_patCur.Guarantor;
			_patPayPlan.PayPlanDate=DateTime.Today;
			_patPayPlan.CompletedAmt=0;
			_patPayPlan.IsDynamic=true;
			_patPayPlan.ChargeFrequency=PayPlanFrequency.Monthly;
			_patPayPlan.PayPlanNum=PayPlans.Insert(_patPayPlan);
		}

		private void ButCloseOrthoCase_Click(object sender,EventArgs e) {
			if(HasErrors()) {
				return;
			}
			_orthoCaseCur.IsActive=false;
			_orthoScheduleCur.IsActive=false;
			_schedulePlanLinkCur.IsActive=false;
			UpdateOrthoCase();
			OrthoProcLinks.DeleteMany(_listProcLinksDetached.Select(x => x.OrthoProcLinkNum).ToList());
			DialogResult=DialogResult.OK;
		}

		private void ButDeleteOrthoCase_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Do you want to delete this ortho case?")) {
				return;
			}
			try {
				OrthoCases.Delete(_orthoCaseCur.OrthoCaseNum,_orthoScheduleCur,_schedulePlanLinkCur
				,OrthoProcLinks.GetManyByOrthoCase(_orthoCaseCur.OrthoCaseNum),_orthoPlanLinkPatPayPlan);
			}
			catch(ApplicationException ex) {
				FriendlyException.Show(ex.Message,ex);
			}
			DialogResult=DialogResult.OK;
		}

		private void UpdateOrthoCase() {
			//OrthoCase
			_orthoCaseCur.Fee=PIn.Double(textTotalFee.Text);
			_orthoCaseCur.FeeInsPrimary=PIn.Double(textPrimaryInsuranceFee.Text);
			_orthoCaseCur.FeeInsSecondary=PIn.Double(textSecondaryInsuranceFee.Text);
			_orthoCaseCur.FeePat=PIn.Double(textPatientFee.Text);
			_orthoCaseCur.BandingDate=PIn.Date(textBandingDate.Text);
			_orthoCaseCur.IsTransfer=checkIsTransfer.Checked;
			//If we don't have a debond proc, user may have updated the expected debond date and we may need to set debond date back to min value.
			if(_debondProc==null) {
				_orthoCaseCur.DebondDateExpected=PIn.Date(textExpectedDebondDate.Text);
				_orthoCaseCur.DebondDate=DateTime.MinValue;
			}
			if(checkIsTransfer.Checked && _bandingProcLink!=null) {//OrthoCase has been changed to a transfer. Delete banding proc link if it exists.
				OrthoProcLinks.Delete(_bandingProcLink.OrthoProcLinkNum);
			}
			else if(!checkIsTransfer.Checked) {
				if(_bandingProcLink==null) {//OrthoCase has been changed to a non-transfer. Link banding proc.
					OrthoProcLinks.Insert(OrthoProcLinks.CreateHelper(_orthoCaseCur.OrthoCaseNum,_bandingProc.ProcNum,OrthoProcType.Banding));
				}
				else {//OrthoCase is still a non-transfer, but banding proc linked may have changed so update.
					OrthoProcLink newBandingProcLink=_bandingProcLink.Copy();
					newBandingProcLink.ProcNum=_bandingProc.ProcNum;
					OrthoProcLinks.Update(newBandingProcLink,_bandingProcLink);
				}
				if(_bandingProc!=null && _bandingProc.AptNum!=0) {//Banding proc may have changed, so check to see if it is scheduled and update bandingDate.
					_orthoCaseCur.BandingDate=_bandingProc.ProcDate;
				}
			}
			OrthoCases.Update(_orthoCaseCur,_orthoCaseOld);
			//OrthoSchedule
			_orthoScheduleCur.BandingAmount=PIn.Double(textBandingAmount.Text);
			_orthoScheduleCur.DebondAmount=PIn.Double(textDebondAmount.Text);
			_orthoScheduleCur.VisitAmount=PIn.Double(textVisitAmount.Text);
			OrthoSchedules.Update(_orthoScheduleCur,_orthoScheduleOld);
			OrthoPlanLinks.Update(_schedulePlanLinkCur,_schedulePlanLinkOld);
		}

		private void ButOK_Click(object sender,EventArgs e) {
			if(HasErrors()) {
				return;
			}
			if(_isNew) {
				//Ortho Case
				OrthoCase newOrthoCase=new OrthoCase();
				newOrthoCase.PatNum=_patCur.PatNum;
				newOrthoCase.Fee=PIn.Double(textTotalFee.Text);
				newOrthoCase.FeeInsPrimary=PIn.Double(textPrimaryInsuranceFee.Text);
				newOrthoCase.FeeInsSecondary=PIn.Double(textSecondaryInsuranceFee.Text);
				newOrthoCase.FeePat=PIn.Double(textPatientFee.Text);
				newOrthoCase.BandingDate=PIn.Date(textBandingDate.Text);
				newOrthoCase.DebondDateExpected=PIn.Date(textExpectedDebondDate.Text);
				newOrthoCase.IsTransfer=checkIsTransfer.Checked;
				newOrthoCase.SecUserNumEntry=Security.CurUser.UserNum;
				newOrthoCase.IsActive=true;//New Ortho Cases can only be added if there are no other active ones. So we automatically set a new ortho case as active.
				if(_bandingProc!=null && _bandingProc.AptNum!=0) {//If banding is scheduled save the appointment date instead.
					newOrthoCase.BandingDate=_bandingProc.ProcDate;
				}
				long orthoCaseNum=OrthoCases.Insert(newOrthoCase);
				_orthoCaseCur=newOrthoCase;
				//Ortho Schedule
				OrthoSchedule newOrthoSchedule=new OrthoSchedule();
				newOrthoSchedule.BandingAmount=PIn.Double(textBandingAmount.Text);
				newOrthoSchedule.DebondAmount=PIn.Double(textDebondAmount.Text);
				newOrthoSchedule.VisitAmount=PIn.Double(textVisitAmount.Text);
				newOrthoSchedule.IsActive=true;
				long orthoScheduleNum=OrthoSchedules.Insert(newOrthoSchedule);
				//Ortho Plan Link
				OrthoPlanLink newOrthoSchedulePlanLink=new OrthoPlanLink();
				newOrthoSchedulePlanLink.OrthoCaseNum=orthoCaseNum;
				newOrthoSchedulePlanLink.LinkType=OrthoPlanLinkType.OrthoSchedule;
				newOrthoSchedulePlanLink.FKey=orthoScheduleNum;
				newOrthoSchedulePlanLink.IsActive=true;
				newOrthoSchedulePlanLink.SecUserNumEntry=Security.CurUser.UserNum;
				OrthoPlanLinks.Insert(newOrthoSchedulePlanLink);
				//Banding Proc Link
				if(!newOrthoCase.IsTransfer) {
					OrthoProcLinks.Insert(OrthoProcLinks.CreateHelper(orthoCaseNum,_bandingProc.ProcNum,OrthoProcType.Banding));
				}
			}
			else {
				UpdateOrthoCase();
			}
			OrthoProcLinks.DeleteMany(_listProcLinksDetached.Select(x => x.OrthoProcLinkNum).ToList());
			if(_patPayPlan!=null && _orthoPlanLinkPatPayPlan==null) {
				_orthoPlanLinkPatPayPlan=new OrthoPlanLink();
				_orthoPlanLinkPatPayPlan.LinkType=OrthoPlanLinkType.PatPayPlan;
				_orthoPlanLinkPatPayPlan.FKey=_patPayPlan.PayPlanNum;
				_orthoPlanLinkPatPayPlan.OrthoCaseNum=_orthoCaseCur.OrthoCaseNum;
				_orthoPlanLinkPatPayPlan.IsActive=true;
				_orthoPlanLinkPatPayPlan.SecUserNumEntry=Security.CurUser.UserNum;
				OrthoPlanLinks.Insert(_orthoPlanLinkPatPayPlan);
			}
			DialogResult=DialogResult.OK;
		}

		private void ButCancel_Click(object sender,EventArgs e) {
			if(_isNew && _patPayPlan!=null) {
				try {
					if(MsgBox.Show(this,MsgBoxButtons.YesNo,"You are canceling a new ortho case. Do you want to delete the payment plan associated to it?")) {
						PayPlans.Delete(_patPayPlan);
					}
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					return;
				}
			}
			DialogResult=DialogResult.Cancel;
		}

	}
}