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
		private Patient _patient;
		///<summary>Will be null if the Orthocase is a transfer or a banding proc has not been selected yet.</summary>
		private Procedure _procedureBanding;
		///<summary>Will be null if a debond procedure has not been completed yet.</summary>
		private Procedure _procedureDebond;
		///<summary>List of all Visit procedure links for completed visit procs.</summary>
		private List<OrthoProcLink> _listOrthoProcLinksVisit=new List<OrthoProcLink>();
		///<summary>List of all Visit procedure completed for the OrthoCase.</summary>
		private List<Procedure> _listProceduresVisit=new List<Procedure>();
		///<summary>Assigned a copy of _orthoCaseCur on load if _isNew is false.</summary>
		private OrthoCase _orthoCaseOld;
		///<summary>Null when OrthoCase is new.</summary>
		private OrthoCase _orthoCase;
		///<summary>Assigned a copy of _orthoScheduleCur on load if _isNew is false.</summary>
		private OrthoSchedule _orthoScheduleOld;
		///<summary>Null when OrthoCase is new.</summary>
		private OrthoSchedule _orthoSchedule;
		///<summary>Assigned a copy of _orthoPlanLinkScheduleCur on load if _isNew is false.</summary>
		private OrthoPlanLink _orthoPlanLinkScheduleOld;
		///<summary>Null when OrthoCase is new.</summary>
		private OrthoPlanLink _orthoPlanLinkSchedule;
		///<summary>Null when OrthoCase is new or is a transfer.</summary>
		private OrthoProcLink _orthoProcLinkBanding;
		///<summary>Null when banding procedure hasn't been completed yet.</summary>
		private OrthoProcLink _orthoProcLinkDebond;
		///<summary>list of proclinks that have been detached from the OrthoCase. Will be deleted if user clicks OK or Closes Case.</summary>
		private List<OrthoProcLink> _listOrthoProcLinksDetached=new List<OrthoProcLink>();
		///<summary>Used to return early from TextChanged events.</summary>
		private bool _doPreventTextChangedEvent=false;
		///<summary>Used to communicate if this case is showing secondary insurance fields</summary>
		private bool _isShowingSecondary;
		///<summary>Dynamic payment plan linked to the ortho case. Can be null.</summary>
		private PayPlan _payPlanPatient;
		///<summary>OrthoPlanLink between dynamic pay plan and ortho case. Can be null.</summary>
		private OrthoPlanLink _orthoPlanLinkPatPayPlan;

		///<summary>List of all completed procedures linked to ortho case. Includes banding, debond, and all visit procs. Nulls are removed.</summary>
		private List<Procedure> ListAllCompletedProcs() {
				List<Procedure> listProceduresLinked=new List<Procedure>(){_procedureBanding,_procedureDebond};
				listProceduresLinked.AddRange(_listProceduresVisit);
				listProceduresLinked.RemoveAll(x => x==null);
				//FUTURE TO DO: Need to change this when treatment planned procs can be added to dynamic pay plans as we will add TP'd banding procs as well.
				listProceduresLinked.RemoveAll(x => x.ProcStatus!=ProcStat.C);
				return listProceduresLinked;
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

		public FormOrthoCase(bool isNew,Patient patient,OrthoCase orthoCase=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_isNew=isNew;
			_patient=patient;
			_orthoCase=orthoCase;
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
			DoShowSecondaryIns(PatPlans.GetPatPlansForPat(_patient.PatNum).Count>1);
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
			_orthoCaseOld=_orthoCase.Copy();
			_orthoPlanLinkSchedule=OrthoPlanLinks.GetOneForOrthoCaseByType(_orthoCase.OrthoCaseNum,OrthoPlanLinkType.OrthoSchedule);
			_orthoPlanLinkScheduleOld=_orthoPlanLinkSchedule.Copy();
			_orthoSchedule=OrthoSchedules.GetOne(_orthoPlanLinkSchedule.FKey);
			_orthoScheduleOld=_orthoSchedule.Copy();
			if(!_orthoCase.IsTransfer) {
				_orthoProcLinkBanding=OrthoProcLinks.GetByType(_orthoCase.OrthoCaseNum,OrthoProcType.Banding);
				//Existing Orthocases that aren't transfers should always have _bandingProcLink, so no need for _bandingProcLink null check.
				_procedureBanding=Procedures.GetOneProc(_orthoProcLinkBanding.ProcNum,false);
				if(_procedureBanding.ProcNum==0) {//Set to null if GetOneProc returned a blank proc.
					_procedureBanding=null;
				}
			}
			_orthoProcLinkDebond=OrthoProcLinks.GetByType(_orthoCase.OrthoCaseNum,OrthoProcType.Debond);
			if(_orthoProcLinkDebond!=null) {
				_procedureDebond=Procedures.GetOneProc(_orthoProcLinkDebond.ProcNum,false);
				if(_procedureDebond.ProcNum==0) {//Set to null if GetOneProc returned a blank proc.
					_procedureDebond=null;
				}
			}
			_listOrthoProcLinksVisit=OrthoProcLinks.GetVisitLinksForOrthoCase(_orthoCase.OrthoCaseNum);
			_listProceduresVisit=Procedures.GetManyProc(_listOrthoProcLinksVisit.Select(x => x.ProcNum).ToList(),false);
			_listProceduresVisit.RemoveAll(x => x.ProcNum==0);//Remove any blank procs that were added to list by GetManyProc(), mostly precautionary
			//Should only be one patient pay plan linked per ortho case.
			_orthoPlanLinkPatPayPlan=OrthoPlanLinks.GetOneForOrthoCaseByType(_orthoCase.OrthoCaseNum,OrthoPlanLinkType.PatPayPlan);
			//Set Fields, buttons, and Grid
			checkIsTransfer.Checked=_orthoCase.IsTransfer;//Have to set this control here so that SetExistingOrthoCaseControls() can use checked state.
			SetExistingOrthoCaseControls();
			RefreshGridOrthoScheduleRows();
		}

		private void FillGridOrthoScheduleColumns() {
			gridOrthoSchedule.BeginUpdate();
			gridOrthoSchedule.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TableOrthoSchedule","Procedure"),80);
			gridOrthoSchedule.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOrthoSchedule","Percent"),60,HorizontalAlignment.Right);
			gridOrthoSchedule.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOrthoSchedule","Amount"),80,HorizontalAlignment.Right);
			gridOrthoSchedule.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOrthoSchedule","Date Completed"),90,HorizontalAlignment.Right){ IsWidthDynamic=true };
			gridOrthoSchedule.Columns.Add(col);
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
				textBandingAmount.Text=_orthoSchedule.BandingAmount.ToString("f");
				textBandingPercent.Text=(_orthoSchedule.BandingAmount/_orthoCase.Fee*100).ToString("f");
			}
			//Fee Details
			//Pat may not have secondary insurance, but we want to show secondary ins fee if it was entered previously when they did have secondary ins.
			if(CompareDecimal.IsGreaterThanZero(_orthoCase.FeeInsSecondary)) {
				DoShowSecondaryIns(true);
			}
			textTotalFee.Text=_orthoCase.Fee.ToString("f");
			textPrimaryInsuranceFee.Text=_orthoCase.FeeInsPrimary.ToString("f");
			textSecondaryInsuranceFee.Text=_orthoCase.FeeInsSecondary.ToString("f");
			textPatientFee.Text=_orthoCase.FeePat.ToString("f");
			double totalCompleted=_listProceduresVisit.Sum(x => x.ProcFeeTotal);
			if(_procedureBanding!=null && _procedureBanding.ProcStatus==ProcStat.C) {
				totalCompleted+=_procedureBanding.ProcFeeTotal;
			}
			if(_procedureDebond!=null && _procedureDebond.ProcStatus==ProcStat.C) {
				totalCompleted+=_procedureDebond.ProcFeeTotal;
			}
			textTotalCompleted.Text=totalCompleted.ToString("f");
			textTotalRemaining.Text=(_orthoCase.Fee-PIn.Double(textTotalCompleted.Text)).ToString("f");
			textPrimaryInsCompleted.Text=(totalCompleted*_orthoCase.FeeInsPrimary/_orthoCase.Fee).ToString("f");
			textPrimaryInsRemaining.Text=(_orthoCase.FeeInsPrimary-PIn.Double(textPrimaryInsCompleted.Text)).ToString("f");
			textSecondaryInsCompleted.Text=(totalCompleted*_orthoCase.FeeInsSecondary/_orthoCase.Fee).ToString("f");
			textSecondaryInsRemaining.Text=(_orthoCase.FeeInsSecondary-PIn.Double(textSecondaryInsCompleted.Text)).ToString("f");
			double insTotalCompleted=PIn.Double(textPrimaryInsCompleted.Text)+PIn.Double(textSecondaryInsCompleted.Text);
			textPatCompleted.Text=(totalCompleted-insTotalCompleted).ToString("f");
			textPatRemaining.Text=(_orthoCase.FeePat-PIn.Double(textPatCompleted.Text)).ToString("f");
			//Procedure Breakdown
			textDebondAmount.Text=_orthoSchedule.DebondAmount.ToString("f");
			textAllVisitsAmount.Text=(_orthoCase.Fee-_orthoSchedule.BandingAmount-_orthoSchedule.DebondAmount).ToString("f");
			textDebondPercent.Text=(_orthoSchedule.DebondAmount/_orthoCase.Fee*100).ToString("f");
			textAllVisitsPercent.Text=(PIn.Double(textAllVisitsAmount.Text)/_orthoCase.Fee*100).ToString("f");
			//Visit Details
			textVisitAmount.Text=_orthoSchedule.VisitAmount.ToString("f");
			textVisitPercent.Text=(_orthoSchedule.VisitAmount/_orthoCase.Fee*100).ToString("f");
			textVisitCountPlanned.Text=OrthoSchedules.CalculatePlannedVisitsCount(_orthoSchedule.BandingAmount,_orthoSchedule.DebondAmount
				,_orthoSchedule.VisitAmount,_orthoCase.Fee).ToString();
			TextVisitCountComplete.Text=_listProceduresVisit.Count.ToString();
			if(_orthoSchedule.VisitAmount==0) {//Set visits planned to zero if we divided by 0 above.
				textVisitCountPlanned.Text="0";
			}
			//Dates
			textBandingDate.Text=_orthoCase.BandingDate.ToShortDateString();
			textExpectedDebondDate.Text=_orthoCase.DebondDateExpected.ToShortDateString();
			labelExpectedDebondDate.Text=Lans.g(this,"Expected Debond Date");
			labelTreatmentLength.Text=Lans.g(this,"Expected days of treatment");
			if(_procedureDebond!=null && _procedureDebond.ProcStatus==ProcStat.C) {
				textExpectedDebondDate.Text=_orthoCase.DebondDate.ToShortDateString();
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
			if(!_orthoCase.IsActive) {
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
			if(_listProceduresVisit.Count>0 || (_procedureDebond!=null && _procedureDebond.ProcStatus==ProcStat.C) 
					|| (_procedureBanding!=null && _procedureBanding.ProcStatus==ProcStat.C)) 
			{
				checkIsTransfer.Enabled=false;
				butAddBandingProcedure.Enabled=false;
				DisableTextFields();
				textBandingDate.Enabled=false;
				if(_procedureDebond!=null && _procedureDebond.ProcStatus==ProcStat.C) {
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
				if(_procedureBanding!=null && _procedureBanding.ProcStatus==ProcStat.C) {
					dateCompleted=_procedureBanding.ProcDate.ToShortDateString();
					row.ColorText=Color.Green;
					row.Tag=_procedureBanding;
				}
				row.Cells.Add(dateCompleted);
				gridOrthoSchedule.ListGridRows.Add(row);
			}
			//Add Visits
			int visitRowCount=Math.Max(PIn.Int(textVisitCountPlanned.Text,false),_listProceduresVisit.Count);
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
				if(i<_listProceduresVisit.Count) {
					dateCompleted=_listProceduresVisit[i].ProcDate.ToShortDateString();
					row.ColorText=Color.Green;
					row.Tag=_listProceduresVisit[i];
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
			if(_procedureDebond!=null && _procedureDebond.ProcStatus==ProcStat.C) {
				dateCompleted=_procedureDebond.ProcDate.ToShortDateString();
				row.ColorText=Color.Green;
				row.Tag=_procedureDebond;
			}
			row.Cells.Add(dateCompleted);
			gridOrthoSchedule.ListGridRows.Add(row);
			gridOrthoSchedule.EndUpdate();
		}

		private void ButAddBandingProcedure_Click(object sender,EventArgs e) {
			using FormProcBandingSelect formProcBandingSelect=new FormProcBandingSelect(_patient.PatNum);
			if(formProcBandingSelect.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_procedureBanding=formProcBandingSelect.ProcedureSelected;
			FillTextBandingProc();
			_doClearAndDisableTextFields=false;//Prevent text fields from being cleared if checkIsTransfer switches from checked to unchecked.
			checkIsTransfer.Checked=false;//Uncheck IsTransfer box as we cannot have a banding proc attached to a transfer.
			_doClearAndDisableTextFields=true;
			EnableTextFields();
		}

		private void FillTextBandingProc() {
			if(_procedureBanding==null) {
				return;
			}
			ProcedureCode procedureCode=ProcedureCodes.GetProcCode(_procedureBanding.CodeNum);
			textBandingProc.Text=procedureCode.ProcCode+" - "+procedureCode.AbbrDesc;
		}

		private void CheckIsTransfer_CheckedChanged(object sender,EventArgs e) {
			_doPreventTextChangedEvent=true;
			if(checkIsTransfer.Checked) {
				_procedureBanding=null;
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

		///<summary>Updates the field linked to the changed field. Sets field color red where there are errors. Clears subordinate fields.  validDoubleSecondLinkedField is specifically for fields of the Fee Details group box. Behavior changes when secondary ins is visible.</summary>
		private void TextFieldChangedHelper(ValidDouble validDoubleChangedField,double changedFieldLimit,ValidDouble validDoubleLinkedField,FieldType fieldTypeLinked
			,ValidDouble validDoubleSecondLinkedField=null)
		{
			if(_doPreventTextChangedEvent) {
				return;
			}
			_doPreventTextChangedEvent=true;
			double totalFee=PIn.Double(textTotalFee.Text);
			double changedFieldNumber=PIn.Double(validDoubleChangedField.Text);
			if(CompareDecimal.IsGreaterThanOrEqualToZero(changedFieldNumber) && changedFieldNumber<=changedFieldLimit) {
				switch(fieldTypeLinked){
					case FieldType.Fee:
						if(!_isShowingSecondary) {//Can only auto-calculate a linked Fee field when we are working with one insurance plan.
							validDoubleLinkedField.Text=(totalFee-changedFieldNumber).ToString("f");
						}
						break;
					case FieldType.Percent:
						validDoubleLinkedField.Text=(changedFieldNumber/totalFee*100).ToString("f");
						break;
					case FieldType.Amount:
						validDoubleLinkedField.Text=(totalFee*changedFieldNumber/100).ToString("f");
						break;
				}
			}
			else {
				validDoubleLinkedField.Text="";
				//We should only clear both linked fields if we are working with both primary and secondary insurance.
				if(_isShowingSecondary && validDoubleSecondLinkedField!=null) {
					validDoubleSecondLinkedField.Text="";
				}
			}
			ClearSubordinateFields(validDoubleChangedField);
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
			gridOrthoSchedule.SelectionMode=GridSelectionMode.OneRow;
		}

		private void ButDetachProc_Click(object sender,EventArgs e) {
			if(gridOrthoSchedule.SelectedGridRows.Where(x => x.Tag!=null).ToList().Count<=0) {//Return if no selected rows have an attached procedure.
				return;
			}
			//If row is bandingProc prompt for deletion of orthocase
			if(_procedureBanding!=null && ((Procedure)gridOrthoSchedule.SelectedGridRows[0].Tag).ProcNum==_procedureBanding.ProcNum) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Detaching the banding procedure requires that the ortho case be deleted. " +
					"Do you want to delete this ortho case?")) 
				{
					return;
				}
				try {
					OrthoCases.Delete(_orthoCase.OrthoCaseNum,_orthoSchedule,_orthoPlanLinkSchedule
						,OrthoProcLinks.GetManyByOrthoCase(_orthoCase.OrthoCaseNum),_orthoPlanLinkPatPayPlan);
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
			for(int i=0;i<gridOrthoSchedule.SelectedGridRows.Count;i++) {
				if(gridOrthoSchedule.SelectedGridRows[i].Tag!=null) {
					DetachProcedure(((Procedure)gridOrthoSchedule.SelectedGridRows[i].Tag).ProcNum);
				}
			}
			SetExistingOrthoCaseControls();
			RefreshGridOrthoScheduleRows();
		}

		private void DetachProcedure(long procNum) {
			if(_procedureDebond!=null && _procedureDebond.ProcNum==procNum) {//detching debond
				_procedureDebond=null;
				_listOrthoProcLinksDetached.Add(_orthoProcLinkDebond);
				_orthoProcLinkDebond=null;
			}
			else {
				_listOrthoProcLinksDetached.AddRange(_listOrthoProcLinksVisit.Where(x => x.ProcNum==procNum).ToList());
				_listOrthoProcLinksVisit.RemoveAll(x => x.ProcNum==procNum);
				_listProceduresVisit.RemoveAll(x => x.ProcNum==procNum);
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
			if(_procedureBanding!=null && checkIsTransfer.Checked) {
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
			if(_payPlanPatient!=null) {
				_payPlanPatient=PayPlans.GetOne(_payPlanPatient.PayPlanNum);//Refresh if we are opening the pay plan again, in case it was deleted.
			}
			else if(_payPlanPatient==null &&_orthoPlanLinkPatPayPlan!=null) {//Get from link if we have not opened pay plan yet.
				_payPlanPatient=PayPlans.GetOne(_orthoPlanLinkPatPayPlan.FKey);
			}
			if(_payPlanPatient==null) {//If pay plan is null it was never created or it was deleted since the window opened.
				_orthoPlanLinkPatPayPlan=null;
				CreateNewPayPlan();
			}
			List<Procedure> listProcedures=ListAllCompletedProcs();
			using FormPayPlanDynamic formPayPlanDynamic=new FormPayPlanDynamic(_payPlanPatient,false,listProcedures);
			formPayPlanDynamic.ShowDialog();
			if(formPayPlanDynamic.DialogResult==DialogResult.Cancel && _payPlanPatient.IsNew) {
				try {
					PayPlans.Delete(_payPlanPatient);
					_payPlanPatient=null;
					_orthoPlanLinkPatPayPlan=null;
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					return;
				}
			}
			else {
				_payPlanPatient=PayPlans.GetOne(_payPlanPatient.PayPlanNum);
				if(_payPlanPatient==null) {//In case pay plan was deleted from form.
					_orthoPlanLinkPatPayPlan=null;
				}
			}
		}

		private void CreateNewPayPlan() {
			_payPlanPatient=new PayPlan();
			_payPlanPatient.IsNew=true;
			_payPlanPatient.PatNum=_patient.PatNum;
			_payPlanPatient.Guarantor=_patient.Guarantor;
			_payPlanPatient.PayPlanDate=DateTime.Today;
			_payPlanPatient.CompletedAmt=0;
			_payPlanPatient.IsDynamic=true;
			_payPlanPatient.ChargeFrequency=PayPlanFrequency.Monthly;
			_payPlanPatient.PayPlanNum=PayPlans.Insert(_payPlanPatient);
		}

		private void ButCloseOrthoCase_Click(object sender,EventArgs e) {
			if(HasErrors()) {
				return;
			}
			_orthoCase.IsActive=false;
			_orthoSchedule.IsActive=false;
			_orthoPlanLinkSchedule.IsActive=false;
			UpdateOrthoCase();
			OrthoProcLinks.DeleteMany(_listOrthoProcLinksDetached.Select(x => x.OrthoProcLinkNum).ToList());
			DialogResult=DialogResult.OK;
		}

		private void ButDeleteOrthoCase_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Do you want to delete this ortho case?")) {
				return;
			}
			try {
				OrthoCases.Delete(_orthoCase.OrthoCaseNum,_orthoSchedule,_orthoPlanLinkSchedule
				,OrthoProcLinks.GetManyByOrthoCase(_orthoCase.OrthoCaseNum),_orthoPlanLinkPatPayPlan);
			}
			catch(ApplicationException ex) {
				FriendlyException.Show(ex.Message,ex);
			}
			DialogResult=DialogResult.OK;
		}

		private void UpdateOrthoCase() {
			//OrthoCase
			_orthoCase.Fee=PIn.Double(textTotalFee.Text);
			_orthoCase.FeeInsPrimary=PIn.Double(textPrimaryInsuranceFee.Text);
			_orthoCase.FeeInsSecondary=PIn.Double(textSecondaryInsuranceFee.Text);
			_orthoCase.FeePat=PIn.Double(textPatientFee.Text);
			_orthoCase.BandingDate=PIn.Date(textBandingDate.Text);
			_orthoCase.IsTransfer=checkIsTransfer.Checked;
			//If we don't have a debond proc, user may have updated the expected debond date and we may need to set debond date back to min value.
			if(_procedureDebond==null) {
				_orthoCase.DebondDateExpected=PIn.Date(textExpectedDebondDate.Text);
				_orthoCase.DebondDate=DateTime.MinValue;
			}
			if(checkIsTransfer.Checked && _orthoProcLinkBanding!=null) {//OrthoCase has been changed to a transfer. Delete banding proc link if it exists.
				OrthoProcLinks.Delete(_orthoProcLinkBanding.OrthoProcLinkNum);
			}
			else if(!checkIsTransfer.Checked) {
				if(_orthoProcLinkBanding==null) {//OrthoCase has been changed to a non-transfer. Link banding proc.
					OrthoProcLinks.Insert(OrthoProcLinks.CreateHelper(_orthoCase.OrthoCaseNum,_procedureBanding.ProcNum,OrthoProcType.Banding));
				}
				else {//OrthoCase is still a non-transfer, but banding proc linked may have changed so update.
					OrthoProcLink orthoProcLinkNewBanding=_orthoProcLinkBanding.Copy();
					orthoProcLinkNewBanding.ProcNum=_procedureBanding.ProcNum;
					OrthoProcLinks.Update(orthoProcLinkNewBanding,_orthoProcLinkBanding);
				}
				if(_procedureBanding!=null && _procedureBanding.AptNum!=0) {//Banding proc may have changed, so check to see if it is scheduled and update bandingDate.
					_orthoCase.BandingDate=_procedureBanding.ProcDate;
				}
			}
			OrthoCases.Update(_orthoCase,_orthoCaseOld);
			//OrthoSchedule
			_orthoSchedule.BandingAmount=PIn.Double(textBandingAmount.Text);
			_orthoSchedule.DebondAmount=PIn.Double(textDebondAmount.Text);
			_orthoSchedule.VisitAmount=PIn.Double(textVisitAmount.Text);
			OrthoSchedules.Update(_orthoSchedule,_orthoScheduleOld);
			OrthoPlanLinks.Update(_orthoPlanLinkSchedule,_orthoPlanLinkScheduleOld);
		}

		private void ButOK_Click(object sender,EventArgs e) {
			if(HasErrors()) {
				return;
			}
			if(_isNew) {
				//Ortho Case
				OrthoCase orthoCaseNew=new OrthoCase();
				orthoCaseNew.PatNum=_patient.PatNum;
				orthoCaseNew.Fee=PIn.Double(textTotalFee.Text);
				orthoCaseNew.FeeInsPrimary=PIn.Double(textPrimaryInsuranceFee.Text);
				orthoCaseNew.FeeInsSecondary=PIn.Double(textSecondaryInsuranceFee.Text);
				orthoCaseNew.FeePat=PIn.Double(textPatientFee.Text);
				orthoCaseNew.BandingDate=PIn.Date(textBandingDate.Text);
				orthoCaseNew.DebondDateExpected=PIn.Date(textExpectedDebondDate.Text);
				orthoCaseNew.IsTransfer=checkIsTransfer.Checked;
				orthoCaseNew.SecUserNumEntry=Security.CurUser.UserNum;
				orthoCaseNew.IsActive=true;//New Ortho Cases can only be added if there are no other active ones. So we automatically set a new ortho case as active.
				if(_procedureBanding!=null && _procedureBanding.AptNum!=0) {//If banding is scheduled save the appointment date instead.
					orthoCaseNew.BandingDate=_procedureBanding.ProcDate;
				}
				long orthoCaseNum=OrthoCases.Insert(orthoCaseNew);
				_orthoCase=orthoCaseNew;
				//Ortho Schedule
				OrthoSchedule orthoScheduleNew=new OrthoSchedule();
				orthoScheduleNew.BandingAmount=PIn.Double(textBandingAmount.Text);
				orthoScheduleNew.DebondAmount=PIn.Double(textDebondAmount.Text);
				orthoScheduleNew.VisitAmount=PIn.Double(textVisitAmount.Text);
				orthoScheduleNew.IsActive=true;
				long orthoScheduleNum=OrthoSchedules.Insert(orthoScheduleNew);
				//Ortho Plan Link
				OrthoPlanLink orthoPlanLinkScheduleNew=new OrthoPlanLink();
				orthoPlanLinkScheduleNew.OrthoCaseNum=orthoCaseNum;
				orthoPlanLinkScheduleNew.LinkType=OrthoPlanLinkType.OrthoSchedule;
				orthoPlanLinkScheduleNew.FKey=orthoScheduleNum;
				orthoPlanLinkScheduleNew.IsActive=true;
				orthoPlanLinkScheduleNew.SecUserNumEntry=Security.CurUser.UserNum;
				OrthoPlanLinks.Insert(orthoPlanLinkScheduleNew);
				//Banding Proc Link
				if(!orthoCaseNew.IsTransfer) {
					OrthoProcLinks.Insert(OrthoProcLinks.CreateHelper(orthoCaseNum,_procedureBanding.ProcNum,OrthoProcType.Banding));
				}
			}
			else {
				UpdateOrthoCase();
			}
			OrthoProcLinks.DeleteMany(_listOrthoProcLinksDetached.Select(x => x.OrthoProcLinkNum).ToList());
			if(_payPlanPatient!=null && _orthoPlanLinkPatPayPlan==null) {
				_orthoPlanLinkPatPayPlan=new OrthoPlanLink();
				_orthoPlanLinkPatPayPlan.LinkType=OrthoPlanLinkType.PatPayPlan;
				_orthoPlanLinkPatPayPlan.FKey=_payPlanPatient.PayPlanNum;
				_orthoPlanLinkPatPayPlan.OrthoCaseNum=_orthoCase.OrthoCaseNum;
				_orthoPlanLinkPatPayPlan.IsActive=true;
				_orthoPlanLinkPatPayPlan.SecUserNumEntry=Security.CurUser.UserNum;
				OrthoPlanLinks.Insert(_orthoPlanLinkPatPayPlan);
			}
			DialogResult=DialogResult.OK;
		}

		private void ButCancel_Click(object sender,EventArgs e) {
			if(_isNew && _payPlanPatient!=null) {
				try {
					if(MsgBox.Show(this,MsgBoxButtons.YesNo,"You are canceling a new ortho case. Do you want to delete the payment plan associated to it?")) {
						PayPlans.Delete(_payPlanPatient);
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