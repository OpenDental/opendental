using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.UI;
using CodeBase;
using System.Linq;

namespace OpenDental{
///<summary></summary>
	public partial class FormProcCodeEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		private bool _isMouseDown;
		private Point _pointMouseOrigin;
		private Point _pointSliderOrigin;
		private StringBuilder _stringBuilderTime;
		private ProcedureCode _procedureCode;
		private ProcedureCode _procedureCodeOld;
		private List<ProcCodeNote> _listProcCodeNotes;
		private List<FeeSched> _listFeeScheds;
		private List<Def> _listDefsProcCodeCat;
		public bool ShowHiddenCategories;
		private List<Fee> _listFees;

		///<summary>The procedure code must have already been insterted into the database.</summary>
		public FormProcCodeEdit(ProcedureCode procedureCode){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			tbTime.CellClicked += new OpenDental.ContrTable.CellEventHandler(tbTime_CellClicked);
			Lan.F(this);
			_procedureCode=procedureCode;
			_procedureCodeOld=procedureCode.Copy();
		}

		private void FormProcCodeEdit_Load(object sender, System.EventArgs e) {
			List<ProcedureCode> listProcedureCodes=CDT.Class1.GetADAcodes();
			if(listProcedureCodes.Count>0 && _procedureCode.ProcCode.Length==5 && _procedureCode.ProcCode.Substring(0,1)=="D") {
				for(int i=0;i<listProcedureCodes.Count;i++) {
					if(listProcedureCodes[i].ProcCode==_procedureCode.ProcCode) {
						textDescription.ReadOnly=true;
					}
				}
			}
			textProcCode.Text=_procedureCode.ProcCode;
			textAlternateCode1.Text=_procedureCode.AlternateCode1;
			textMedicalCode.Text=_procedureCode.MedicalCode;
			textSubstitutionCode.Text=_procedureCode.SubstitutionCode;
			for(int i=0;i<Enum.GetNames(typeof(SubstitutionCondition)).Length;i++) {
				comboSubstOnlyIf.Items.Add(Lan.g("enumSubstitutionCondition",Enum.GetNames(typeof(SubstitutionCondition))[i]));
			}
			comboSubstOnlyIf.SelectedIndex=(int)_procedureCode.SubstOnlyIf;
			textDescription.Text=_procedureCode.Descript;
			textAbbrev.Text=_procedureCode.AbbrDesc;
			textLaymanTerm.Text=_procedureCode.LaymanTerm;
			_stringBuilderTime=new StringBuilder(_procedureCode.ProcTime);
			butColor.BackColor=_procedureCode.GraphicColor;
			checkMultiVisit.Checked=_procedureCode.IsMultiVisit;
			checkMultiVisit.Visible=Programs.UsingOrion;
			checkNoBillIns.Checked=_procedureCode.NoBillIns;
			checkIsHygiene.Checked=_procedureCode.IsHygiene;
			checkIsProsth.Checked=_procedureCode.IsProsth;
			textBaseUnits.Text=_procedureCode.BaseUnits.ToString();
			textDrugNDC.Text=_procedureCode.DrugNDC;
			textRevenueCode.Text=_procedureCode.RevenueCodeDefault;
			checkIsRadiology.Checked=_procedureCode.IsRadiology;
			checkIsAutoTaxed.Checked=_procedureCode.IsTaxed;
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Not Canadian. en-CA or fr-CA
				checkIsCanadianLab.Visible=false;
			}
			//else {//always enabled
				//checkIsCanadianLab.Enabled=IsNew || !Procedures.IsUsingCode(ProcCode.CodeNum);
			//}
			checkIsCanadianLab.Checked=_procedureCode.IsCanadianLab;
			textNote.Text=_procedureCode.DefaultNote;
			textTpNote.Text=_procedureCode.DefaultTPNote;
			textDefaultClaimNote.Text=_procedureCode.DefaultClaimNote;
			listTreatArea.Items.Clear();
			listTreatArea.Items.AddEnums<TreatmentArea>();
			listTreatArea.SetSelectedEnum(_procedureCode.TreatArea);	
			checkToothRange.Checked=_procedureCode.AreaAlsoToothRange;
			listPaintType.Items.AddEnums<ToothPaintingType>();
			listPaintType.SetSelectedEnum(_procedureCode.PaintType);
			textPaintText.Text=_procedureCode.PaintText;
			_listDefsProcCodeCat=Defs.GetDefsForCategory(DefCat.ProcCodeCats,!ShowHiddenCategories);
			for(int i=0;i<_listDefsProcCodeCat.Count;i++){
				string isHidden=(_listDefsProcCodeCat[i].IsHidden) ? " (hidden)" : "";
				listCategory.Items.Add(_listDefsProcCodeCat[i].ItemName+isHidden);
				if(_listDefsProcCodeCat[i].DefNum==_procedureCode.ProcCat) {
					listCategory.SelectedIndex=i;
				}
			}
			if(listCategory.SelectedIndex==-1) {
				listCategory.SelectedIndex=0;
			}
			comboProvNumDefault.Items.AddProvNone();
			comboProvNumDefault.Items.AddProvsAbbr(Providers.GetDeepCopy(true));
			comboProvNumDefault.SetSelectedProvNum(_procedureCode.ProvNumDefault);
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				labelTreatArea.Visible=false;
				listTreatArea.Visible=false;
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				//Since Time Units are currently only helpful in Canada,
				//we have decided not to show this textbox in other countries for now.
				labelTimeUnits.Visible=true;
				textTimeUnits.Visible=true;
				textTimeUnits.Text=_procedureCode.CanadaTimeUnits.ToString();
			}
			checkBypassLockDate.Checked=(_procedureCode.BypassGlobalLock==BypassLockStatus.BypassIfZero);
			_listFeeScheds=FeeScheds.GetDeepCopy(true);
			FillTime();
			FillFees();
			FillNotes();
		}

		private void FillTime(){
			for (int i=0;i<_stringBuilderTime.Length;i++){
				tbTime.Cell[0,i]=_stringBuilderTime.ToString(i,1);
				tbTime.BackGColor[0,i]=Color.White;
			}
			for (int i=_stringBuilderTime.Length;i<tbTime.MaxRows;i++){
				tbTime.Cell[0,i]="";
				tbTime.BackGColor[0,i]=Color.FromName("Control");
			}
			tbTime.Refresh();
			LayoutManager.MoveLocation(butSlider,new Point(tbTime.Location.X+2
				,(tbTime.Location.Y+_stringBuilderTime.Length*14+1)));
			textTime2.Text=(_stringBuilderTime.Length*PrefC.GetInt(PrefName.AppointmentTimeIncrement)).ToString();
		}

		private void FillFees(){
			_listFees=Fees.GetFeesForCodeNoOverrides(_procedureCode.CodeNum);
			gridFees.BeginUpdate();
			gridFees.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableProcFee","Sched"),120);
			gridFees.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcFee","Amount"),60,HorizontalAlignment.Right);
			gridFees.Columns.Add(col); 
			gridFees.ListGridRows.Clear();
			GridRow row;
			Fee fee;
			for(int i=0;i<_listFeeScheds.Count;i++){
				fee=Fees.GetFee(_procedureCode.CodeNum,_listFeeScheds[i].FeeSchedNum,0,0,_listFees);
				row=new GridRow();
				row.Cells.Add(_listFeeScheds[i].Description);
				if(fee==null){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(fee.Amount.ToString("n"));
				}
				gridFees.ListGridRows.Add(row);
			}
			gridFees.EndUpdate();
		}

		private void gridFees_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			Fee fee=Fees.GetFee(_procedureCode.CodeNum,_listFeeScheds[e.Row].FeeSchedNum,0,0,_listFees);
			//tbFees.SelectedRow=e.Row;
			//tbFees.ColorRow(e.Row,Color.LightGray);
			using FormFeeEdit FormFeeEdit=new FormFeeEdit();
			if(fee==null) {
				fee=new Fee();
				fee.FeeSched=_listFeeScheds[e.Row].FeeSchedNum;
				fee.CodeNum=_procedureCode.CodeNum;
				Fees.Insert(fee);
				//SecurityLog is updated in FormFeeEdit.
				FormFeeEdit.IsNew=true;
			}
			FormFeeEdit.FeeCur=fee;
			FormFeeEdit.ShowDialog();
			FillFees();
		}

		///<summary>Fills both the Default Completed Notes Grid and the Default TP Notes Grid simultaneously.</summary>
		private void FillNotes(){
			_listProcCodeNotes=ProcCodeNotes.GetList(_procedureCode.CodeNum);
			gridNotes.BeginUpdate();
			gridTpNotes.BeginUpdate();
			gridNotes.Columns.Clear();
			gridTpNotes.Columns.Clear();
			#region gridNotes
			GridColumn col=new GridColumn(Lan.g("TableProcedureNotes","Prov"),80);
			gridNotes.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcedureNotes","Time"),150);
			gridNotes.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcedureNotes","Note"),400);
			gridNotes.Columns.Add(col);
			#endregion
			#region gridTpNotes
			GridColumn column=new GridColumn(Lan.g("TableTpProcedureNotes","Prov"),80);
			gridTpNotes.Columns.Add(column);
			column=new GridColumn(Lan.g("TableTpProcedureNotes","Time"),150);
			gridTpNotes.Columns.Add(column);
			column=new GridColumn(Lan.g("TableTpProcedureNotes","Note"),400);
			gridTpNotes.Columns.Add(column);
			#endregion
			gridNotes.ListGridRows.Clear();
			gridTpNotes.ListGridRows.Clear();
			GridRow row;
			foreach(ProcCodeNote procNoteCur in _listProcCodeNotes) {
				row=new GridRow();
				row.Cells.Add(Providers.GetAbbr(procNoteCur.ProvNum));
				row.Cells.Add(procNoteCur.ProcTime);
				row.Cells.Add(procNoteCur.Note);
				if(procNoteCur.ProcStatus==ProcStat.TP) {
					gridTpNotes.ListGridRows.Add(row);
				}
				else {
					gridNotes.ListGridRows.Add(row);
				}
			}
			gridNotes.EndUpdate();
			gridTpNotes.EndUpdate();
		}

		private void AddNote(bool isTp) {
			using FormProcCodeNoteEdit formProcCodeNoteEdit=new FormProcCodeNoteEdit(isTp);
			formProcCodeNoteEdit.IsNew=true;
			formProcCodeNoteEdit.ProcCodeNoteCur=new ProcCodeNote();
			formProcCodeNoteEdit.ProcCodeNoteCur.CodeNum=_procedureCode.CodeNum;
			if(isTp) {
				formProcCodeNoteEdit.ProcCodeNoteCur.Note=textTpNote.Text;
			}
			else {
				formProcCodeNoteEdit.ProcCodeNoteCur.Note=textNote.Text;
			}
			formProcCodeNoteEdit.ProcCodeNoteCur.ProcTime=_stringBuilderTime.ToString();
			formProcCodeNoteEdit.ShowDialog();
			FillNotes();
		}

		private void AddAutoNote(ODtextBox textBox) {
			//1. Keep track of where the cursor is. (returns position just beyond last char if textbox isn't selected)
			int cursorIdx=textBox.SelectionStart;
			//2. Open up FormAutoNotes in selection mode
			using FormAutoNotes formAutoNotes=new FormAutoNotes();
			formAutoNotes.IsSelectionMode=true;
			if(formAutoNotes.ShowDialog()==DialogResult.OK) {
				//3. Put the selected AutoNote title into the text at the cursor's index (FormAN.AutoNoteCur.AutoNoteName) surrounded by brackets.
				textBox.Text=textBox.Text.Insert(cursorIdx,"[["+formAutoNotes.AutoNoteCur.AutoNoteName+"]]");
			}
		}

		private void tbTime_CellClicked(object sender, CellEventArgs e){
			if(e.Row<_stringBuilderTime.Length){
				if(_stringBuilderTime[e.Row]=='/'){
					_stringBuilderTime.Replace('/','X',e.Row,1);
				}
				else{
					_stringBuilderTime.Replace(_stringBuilderTime[e.Row],'/',e.Row,1);
				}
			}
			FillTime();
		}

		private void butSlider_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			_isMouseDown=true;
			_pointMouseOrigin=new Point(e.X+butSlider.Location.X
				,e.Y+butSlider.Location.Y);
			_pointSliderOrigin=butSlider.Location;
			
		}

		private void butSlider_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(!_isMouseDown)return;
			//tempPoint represents the new location of button of smooth dragging.
			Point point=new Point(_pointSliderOrigin.X
				,_pointSliderOrigin.Y+(e.Y+butSlider.Location.Y)-_pointMouseOrigin.Y);
			int step=(int)(Math.Round((Decimal)(point.Y-tbTime.Location.Y)/14));
			if(step==_stringBuilderTime.Length)return;
			if(step<1)return;
			if(step>tbTime.MaxRows-1) return;
			if(step>_stringBuilderTime.Length){
				_stringBuilderTime.Append('/');
			}
			if(step<_stringBuilderTime.Length){
				_stringBuilderTime.Remove(step,1);
			}
			FillTime();
		}

		private void butSlider_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			_isMouseDown=false;
		}

		private void butNoBillInsByInsPlan_Click(object sender,EventArgs e) {
			using FormInsPlansOverrides formInsPlanOverrides=new FormInsPlansOverrides(_procedureCodeOld.CodeNum);
			formInsPlanOverrides.ShowDialog();//No UI in this proc code edit window needs to be refreshed based on the results of this window.
		}

		private void butColor_Click(object sender,EventArgs e) {
			using ColorDialog colorDialog=new ColorDialog();
			colorDialog.Color=butColor.BackColor;
			colorDialog.ShowDialog();
			butColor.BackColor=colorDialog.Color;
		}

		private void butColorClear_Click(object sender,EventArgs e) {
			butColor.BackColor=Color.FromArgb(0);
		}

		private void butAddTpNote_Click(object sender,EventArgs e) {
			AddNote(true);
		}

		private void butAddNote_Click(object sender,EventArgs e) {
			AddNote(false);
		}

		private void butAutoTPNote_Click(object sender,EventArgs e) {
			AddAutoNote(textTpNote);
		}

		private void butAutoNote_Click(object sender,EventArgs e) {
			AddAutoNote(textNote);
		}

		private void gridNotes_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormProcCodeNoteEdit formProcCodeNoteEdit=new FormProcCodeNoteEdit(false);
			formProcCodeNoteEdit.ProcCodeNoteCur=_listProcCodeNotes[e.Row].Copy();
			formProcCodeNoteEdit.ShowDialog();
			FillNotes();
		}

		private void gridTpNotes_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormProcCodeNoteEdit formProcCodeNoteEdit=new FormProcCodeNoteEdit(true);
			formProcCodeNoteEdit.ProcCodeNoteCur=_listProcCodeNotes[e.Row].Copy();
			formProcCodeNoteEdit.ShowDialog();
			FillNotes();
		}

		private void butMore_Click(object sender,EventArgs e) {
			using FormProcCodeEditMore formProcCodeEditMore=new FormProcCodeEditMore(_procedureCode);
			formProcCodeEditMore.ShowDialog();
			FillFees();//Refresh our list of fees cause the user may have changed something.
		}

		private void butAuditTrail_Click(object sender,EventArgs e) {
			List<Permissions> listPermissions=new List<Permissions>();
			listPermissions.Add(Permissions.ProcFeeEdit);
			using FormAuditOneType formAuditOneType=new FormAuditOneType(0,listPermissions,Lan.g(this,"All changes for")+" "+_procedureCode.AbbrDesc+" - "+_procedureCode.ProcCode,_procedureCode.CodeNum);
			formAuditOneType.ShowDialog();
		}

		private void listPaintType_SelectedIndexChanged(object sender, EventArgs e){
			if(listPaintType.GetSelected<ToothPaintingType>()==ToothPaintingType.Text){
				labelPaintText.Visible=true;
				textPaintText.Visible=true;
			}
			else{
				labelPaintText.Visible=false;
				textPaintText.Visible=false;
			}
		}

		private void listTreatArea_SelectedIndexChanged(object sender,EventArgs e) {
			TreatmentArea treatmentArea=listTreatArea.GetSelected<TreatmentArea>();
			if(treatmentArea.In(TreatmentArea.Arch,TreatmentArea.Quad)){
				checkToothRange.Enabled=true;
			}
			else{
				checkToothRange.Enabled=false;
				checkToothRange.Checked=false;
			}
		}

		///<summary>Returns a line that can be used in a security log entry if the entries are changed.</summary>
		private string SecurityLogEntryHelper(string oldVal, string newVal,string textInLog) {			
			if(oldVal!=newVal) {
				return "Code "+_procedureCodeOld.ProcCode+" "+textInLog+" changed from '"+oldVal+"' to  '"+newVal+"'\r\n";
			}
			return "";
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textMedicalCode.Text!="" && !ProcedureCodes.GetContainsKey(textMedicalCode.Text)){
				MsgBox.Show(this,"Invalid medical code.  It must refer to an existing procedure code entered separately");
				return;
			}
			if(textSubstitutionCode.Text!="" && !ProcedureCodes.GetContainsKey(textSubstitutionCode.Text)) {
				MsgBox.Show(this,"Invalid substitution code.  It must refer to an existing procedure code entered separately");
				return;
			}
			/*bool DoSynchRecall=false;
			if(IsNew && checkSetRecall.Checked){
				DoSynchRecall=true;
			}
			else if(ProcCode.SetRecall!=checkSetRecall.Checked){//set recall changed
				DoSynchRecall=true;
			}
			if(DoSynchRecall){
				if(!MsgBox.Show(this,true,"Because you have changed the recall setting for this procedure code, all your patient recalls will be resynchronized, which can take a minute or two.  Do you want to continue?")){
					return;
				}
			}*/
			if(!textBaseUnits.IsValid() || (CultureInfo.CurrentCulture.Name.EndsWith("CA") && !textTimeUnits.IsValid())) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			_procedureCode.AlternateCode1=textAlternateCode1.Text;
			_procedureCode.MedicalCode=textMedicalCode.Text;
			_procedureCode.SubstitutionCode=textSubstitutionCode.Text;
			_procedureCode.SubstOnlyIf=(SubstitutionCondition)comboSubstOnlyIf.SelectedIndex;
			_procedureCode.Descript=textDescription.Text;
			_procedureCode.AbbrDesc=textAbbrev.Text;
			_procedureCode.LaymanTerm=textLaymanTerm.Text;
			_procedureCode.ProcTime=_stringBuilderTime.ToString();
			_procedureCode.GraphicColor=butColor.BackColor;
			_procedureCode.IsMultiVisit=checkMultiVisit.Checked;
			_procedureCode.NoBillIns=checkNoBillIns.Checked;
			_procedureCode.IsProsth=checkIsProsth.Checked;
			_procedureCode.IsHygiene=checkIsHygiene.Checked;
			_procedureCode.IsRadiology=checkIsRadiology.Checked;
			_procedureCode.IsCanadianLab=checkIsCanadianLab.Checked;
			_procedureCode.IsTaxed=checkIsAutoTaxed.Checked;
			_procedureCode.DefaultNote=textNote.Text;
			_procedureCode.DefaultTPNote=textTpNote.Text;
			_procedureCode.DefaultClaimNote=textDefaultClaimNote.Text;
			_procedureCode.PaintType=listPaintType.GetSelected<ToothPaintingType>();
			_procedureCode.PaintText=textPaintText.Text;
			_procedureCode.TreatArea=(TreatmentArea)listTreatArea.SelectedIndex;
			_procedureCode.BaseUnits=PIn.Int(textBaseUnits.Text.ToString());
			_procedureCode.DrugNDC=textDrugNDC.Text;
			_procedureCode.RevenueCodeDefault=textRevenueCode.Text;
			if(listCategory.SelectedIndex!=-1) {
				_procedureCode.ProcCat=_listDefsProcCodeCat[listCategory.SelectedIndex].DefNum;
			}
			_procedureCode.ProvNumDefault=comboProvNumDefault.GetSelectedProvNum();			
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA, for CanadaTimeUnits
				_procedureCode.CanadaTimeUnits=PIn.Double(textTimeUnits.Text);
			}
			if(checkBypassLockDate.Checked) {
				_procedureCode.BypassGlobalLock=BypassLockStatus.BypassIfZero;
			}
			else {
				_procedureCode.BypassGlobalLock=BypassLockStatus.NeverBypass;
			}
			_procedureCode.AreaAlsoToothRange=checkToothRange.Checked;
			if(ProcedureCodes.Update(_procedureCode,_procedureCodeOld)) {//whether new or not.
				string secLog="";
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.AlternateCode1,_procedureCode.AlternateCode1,"alt code");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.MedicalCode,_procedureCode.MedicalCode,"medical code");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.SubstitutionCode,_procedureCode.SubstitutionCode,"insurance substitution code");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.SubstOnlyIf.GetDescription(),_procedureCode.SubstOnlyIf.GetDescription(),"only if box");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.Descript,_procedureCode.Descript,"description");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.AbbrDesc,_procedureCode.AbbrDesc,"abbreviation");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.LaymanTerm,_procedureCode.LaymanTerm,"layman's terms");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.ProcTime,_procedureCode.ProcTime,"procedure time");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.IsMultiVisit.ToString(),_procedureCode.IsMultiVisit.ToString(),"'MultiVisit' box");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.NoBillIns.ToString(),_procedureCode.NoBillIns.ToString(),"'Do not usually bill to Ins' box");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.IsProsth.ToString(),_procedureCode.IsProsth.ToString(),"prosthesis box");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.IsHygiene.ToString(),_procedureCode.IsHygiene.ToString(),"hygiene procedure box");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.IsRadiology.ToString(),_procedureCode.IsRadiology.ToString(),"radiology box");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.IsTaxed.ToString(),_procedureCode.IsTaxed.ToString(),"'Auto Tax' box");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.DefaultNote,_procedureCode.DefaultNote,"default note");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.DefaultTPNote,_procedureCode.DefaultTPNote,"TP'd note");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.DefaultClaimNote,_procedureCode.DefaultClaimNote,"default claim note");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.TreatArea.GetDescription(),_procedureCode.TreatArea.GetDescription(),"treatment area");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.BaseUnits.ToString(),_procedureCode.BaseUnits.ToString(),"base units");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.DrugNDC,_procedureCode.DrugNDC,"drug NDC");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.RevenueCodeDefault,_procedureCode.RevenueCodeDefault,"default revenue");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.ProvNumDefault.ToString(),_procedureCode.ProvNumDefault.ToString(),"provider number");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.BypassGlobalLock.ToString(),_procedureCode.BypassGlobalLock.ToString(),"bypass global lock box");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.ProcCatDescript,_procedureCode.ProcCatDescript,"category");
				secLog+=SecurityLogEntryHelper(_procedureCodeOld.AreaAlsoToothRange.ToString(),_procedureCode.AreaAlsoToothRange.ToString(),"AreaAlsoToothRange box");
				SecurityLogs.MakeLogEntry(Permissions.ProcCodeEdit,0,secLog,_procedureCode.CodeNum,_procedureCodeOld.DateTStamp);
				DataValid.SetInvalid(InvalidType.ProcCodes);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
