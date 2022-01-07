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
		private bool mouseIsDown;
		private Point	mouseOrigin;
		private Point sliderOrigin;
		private StringBuilder strBTime;
		private ProcedureCode ProcCode;
		private ProcedureCode _procCodeOld;
		private List<ProcCodeNote> NoteList;
		private List<FeeSched> _listFeeScheds;
		private List<Def> _listProcCodeCatDefs;
		public bool ShowHiddenCategories;
		private List<Fee> _listFees;

		///<summary>The procedure code must have already been insterted into the database.</summary>
		public FormProcCodeEdit(ProcedureCode procCode){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			tbTime.CellClicked += new OpenDental.ContrTable.CellEventHandler(tbTime_CellClicked);
			Lan.F(this);
			ProcCode=procCode;
			_procCodeOld=procCode.Copy();
		}

		private void FormProcCodeEdit_Load(object sender, System.EventArgs e) {
			List<ProcedureCode> listCodes=CDT.Class1.GetADAcodes();
			if(listCodes.Count>0 && ProcCode.ProcCode.Length==5 && ProcCode.ProcCode.Substring(0,1)=="D") {
				for(int i=0;i<listCodes.Count;i++) {
					if(listCodes[i].ProcCode==ProcCode.ProcCode) {
						textDescription.ReadOnly=true;
					}
				}
			}
			textProcCode.Text=ProcCode.ProcCode;
			textAlternateCode1.Text=ProcCode.AlternateCode1;
			textMedicalCode.Text=ProcCode.MedicalCode;
			textSubstitutionCode.Text=ProcCode.SubstitutionCode;
			for(int i=0;i<Enum.GetNames(typeof(SubstitutionCondition)).Length;i++) {
				comboSubstOnlyIf.Items.Add(Lan.g("enumSubstitutionCondition",Enum.GetNames(typeof(SubstitutionCondition))[i]));
			}
			comboSubstOnlyIf.SelectedIndex=(int)ProcCode.SubstOnlyIf;
			textDescription.Text=ProcCode.Descript;
			textAbbrev.Text=ProcCode.AbbrDesc;
			textLaymanTerm.Text=ProcCode.LaymanTerm;
			strBTime=new StringBuilder(ProcCode.ProcTime);
			butColor.BackColor=ProcCode.GraphicColor;
			checkMultiVisit.Checked=ProcCode.IsMultiVisit;
			checkMultiVisit.Visible=Programs.UsingOrion;
			checkNoBillIns.Checked=ProcCode.NoBillIns;
			checkIsHygiene.Checked=ProcCode.IsHygiene;
			checkIsProsth.Checked=ProcCode.IsProsth;
			textBaseUnits.Text=ProcCode.BaseUnits.ToString();
			textDrugNDC.Text=ProcCode.DrugNDC;
			textRevenueCode.Text=ProcCode.RevenueCodeDefault;
			checkIsRadiology.Checked=ProcCode.IsRadiology;
			checkIsAutoTaxed.Checked=ProcCode.IsTaxed;
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Not Canadian. en-CA or fr-CA
				checkIsCanadianLab.Visible=false;
			}
			//else {//always enabled
				//checkIsCanadianLab.Enabled=IsNew || !Procedures.IsUsingCode(ProcCode.CodeNum);
			//}
			checkIsCanadianLab.Checked=ProcCode.IsCanadianLab;
			textNote.Text=ProcCode.DefaultNote;
			textTpNote.Text=ProcCode.DefaultTPNote;
			textDefaultClaimNote.Text=ProcCode.DefaultClaimNote;
			listTreatArea.Items.Clear();
			listTreatArea.Items.AddEnums<TreatmentArea>();
			listTreatArea.SetSelectedEnum(ProcCode.TreatArea);	
			checkToothRange.Checked=ProcCode.AreaAlsoToothRange;
			listPaintType.Items.AddEnums<ToothPaintingType>();
			listPaintType.SetSelectedEnum(ProcCode.PaintType);
			textPaintText.Text=ProcCode.PaintText;
			_listProcCodeCatDefs=Defs.GetDefsForCategory(DefCat.ProcCodeCats,!ShowHiddenCategories);
			for(int i=0;i<_listProcCodeCatDefs.Count;i++){
				string isHidden=(_listProcCodeCatDefs[i].IsHidden) ? " (hidden)" : "";
				listCategory.Items.Add(_listProcCodeCatDefs[i].ItemName+isHidden);
				if(_listProcCodeCatDefs[i].DefNum==ProcCode.ProcCat) {
					listCategory.SelectedIndex=i;
				}
			}
			if(listCategory.SelectedIndex==-1) {
				listCategory.SelectedIndex=0;
			}
			comboProvNumDefault.Items.AddProvNone();
			comboProvNumDefault.Items.AddProvsAbbr(Providers.GetDeepCopy(true));
			comboProvNumDefault.SetSelectedProvNum(ProcCode.ProvNumDefault);
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				labelTreatArea.Visible=false;
				listTreatArea.Visible=false;
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				//Since Time Units are currently only helpful in Canada,
				//we have decided not to show this textbox in other countries for now.
				labelTimeUnits.Visible=true;
				textTimeUnits.Visible=true;
				textTimeUnits.Text=ProcCode.CanadaTimeUnits.ToString();
			}
			checkBypassLockDate.Checked=(ProcCode.BypassGlobalLock==BypassLockStatus.BypassIfZero);
			_listFeeScheds=FeeScheds.GetDeepCopy(true);
			FillTime();
			FillFees();
			FillNotes();
		}

		private void FillTime(){
			for (int i=0;i<strBTime.Length;i++){
				tbTime.Cell[0,i]=strBTime.ToString(i,1);
				tbTime.BackGColor[0,i]=Color.White;
			}
			for (int i=strBTime.Length;i<tbTime.MaxRows;i++){
				tbTime.Cell[0,i]="";
				tbTime.BackGColor[0,i]=Color.FromName("Control");
			}
			tbTime.Refresh();
			LayoutManager.MoveLocation(butSlider,new Point(tbTime.Location.X+2
				,(tbTime.Location.Y+strBTime.Length*14+1)));
			textTime2.Text=(strBTime.Length*PrefC.GetInt(PrefName.AppointmentTimeIncrement)).ToString();
		}

		private void FillFees(){
			_listFees=Fees.GetFeesForCodeNoOverrides(ProcCode.CodeNum);
			gridFees.BeginUpdate();
			gridFees.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableProcFee","Sched"),120);
			gridFees.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcFee","Amount"),60,HorizontalAlignment.Right);
			gridFees.ListGridColumns.Add(col); 
			gridFees.ListGridRows.Clear();
			GridRow row;
			Fee fee;
			for(int i=0;i<_listFeeScheds.Count;i++){
				fee=Fees.GetFee(ProcCode.CodeNum,_listFeeScheds[i].FeeSchedNum,0,0,_listFees);
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
			Fee FeeCur=Fees.GetFee(ProcCode.CodeNum,_listFeeScheds[e.Row].FeeSchedNum,0,0,_listFees);
			//tbFees.SelectedRow=e.Row;
			//tbFees.ColorRow(e.Row,Color.LightGray);
			using FormFeeEdit FormFE=new FormFeeEdit();
			if(FeeCur==null) {
				FeeCur=new Fee();
				FeeCur.FeeSched=_listFeeScheds[e.Row].FeeSchedNum;
				FeeCur.CodeNum=ProcCode.CodeNum;
				Fees.Insert(FeeCur);
				//SecurityLog is updated in FormFeeEdit.
				FormFE.IsNew=true;
			}
			FormFE.FeeCur=FeeCur;
			FormFE.ShowDialog();
			FillFees();
		}

		///<summary>Fills both the Default Completed Notes Grid and the Default TP Notes Grid simultaneously.</summary>
		private void FillNotes(){
			NoteList=ProcCodeNotes.GetList(ProcCode.CodeNum);
			gridNotes.BeginUpdate();
			gridTpNotes.BeginUpdate();
			gridNotes.ListGridColumns.Clear();
			gridTpNotes.ListGridColumns.Clear();
			#region gridNotes
			GridColumn col=new GridColumn(Lan.g("TableProcedureNotes","Prov"),80);
			gridNotes.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcedureNotes","Time"),150);
			gridNotes.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcedureNotes","Note"),400);
			gridNotes.ListGridColumns.Add(col);
			#endregion
			#region gridTpNotes
			GridColumn column=new GridColumn(Lan.g("TableTpProcedureNotes","Prov"),80);
			gridTpNotes.ListGridColumns.Add(column);
			column=new GridColumn(Lan.g("TableTpProcedureNotes","Time"),150);
			gridTpNotes.ListGridColumns.Add(column);
			column=new GridColumn(Lan.g("TableTpProcedureNotes","Note"),400);
			gridTpNotes.ListGridColumns.Add(column);
			#endregion
			gridNotes.ListGridRows.Clear();
			gridTpNotes.ListGridRows.Clear();
			GridRow row;
			foreach(ProcCodeNote procNoteCur in NoteList) {
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
			using FormProcCodeNoteEdit FormP=new FormProcCodeNoteEdit(isTp);
			FormP.IsNew=true;
			FormP.NoteCur=new ProcCodeNote();
			FormP.NoteCur.CodeNum=ProcCode.CodeNum;
			if(isTp) {
				FormP.NoteCur.Note=textTpNote.Text;
			}
			else {
				FormP.NoteCur.Note=textNote.Text;
			}
			FormP.NoteCur.ProcTime=strBTime.ToString();
			FormP.ShowDialog();
			FillNotes();
		}

		private void AddAutoNote(ODtextBox textBox) {
			//1. Keep track of where the cursor is. (returns position just beyond last char if textbox isn't selected)
			int cursorIdx=textBox.SelectionStart;
			//2. Open up FormAutoNotes in selection mode
			using FormAutoNotes FormAN=new FormAutoNotes();
			FormAN.IsSelectionMode=true;
			if(FormAN.ShowDialog()==DialogResult.OK) {
				//3. Put the selected AutoNote title into the text at the cursor's index (FormAN.AutoNoteCur.AutoNoteName) surrounded by brackets.
				textBox.Text=textBox.Text.Insert(cursorIdx,"[["+FormAN.AutoNoteCur.AutoNoteName+"]]");
			}
		}

		private void tbTime_CellClicked(object sender, CellEventArgs e){
			if(e.Row<strBTime.Length){
				if(strBTime[e.Row]=='/'){
					strBTime.Replace('/','X',e.Row,1);
				}
				else{
					strBTime.Replace(strBTime[e.Row],'/',e.Row,1);
				}
			}
			FillTime();
		}

		private void butSlider_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			mouseIsDown=true;
			mouseOrigin=new Point(e.X+butSlider.Location.X
				,e.Y+butSlider.Location.Y);
			sliderOrigin=butSlider.Location;
			
		}

		private void butSlider_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(!mouseIsDown)return;
			//tempPoint represents the new location of button of smooth dragging.
			Point tempPoint=new Point(sliderOrigin.X
				,sliderOrigin.Y+(e.Y+butSlider.Location.Y)-mouseOrigin.Y);
			int step=(int)(Math.Round((Decimal)(tempPoint.Y-tbTime.Location.Y)/14));
			if(step==strBTime.Length)return;
			if(step<1)return;
			if(step>tbTime.MaxRows-1) return;
			if(step>strBTime.Length){
				strBTime.Append('/');
			}
			if(step<strBTime.Length){
				strBTime.Remove(step,1);
			}
			FillTime();
		}

		private void butSlider_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			mouseIsDown=false;
		}

		private void butColor_Click(object sender,EventArgs e) {
			using ColorDialog colorDialog1=new ColorDialog();
			colorDialog1.Color=butColor.BackColor;
			colorDialog1.ShowDialog();
			butColor.BackColor=colorDialog1.Color;
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
			using FormProcCodeNoteEdit FormP=new FormProcCodeNoteEdit(false);
			FormP.NoteCur=NoteList[e.Row].Copy();
			FormP.ShowDialog();
			FillNotes();
		}

		private void gridTpNotes_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormProcCodeNoteEdit FormP=new FormProcCodeNoteEdit(true);
			FormP.NoteCur=NoteList[e.Row].Copy();
			FormP.ShowDialog();
			FillNotes();
		}

		private void butMore_Click(object sender,EventArgs e) {
			using FormProcCodeEditMore FormPCEM=new FormProcCodeEditMore(ProcCode);
			FormPCEM.ShowDialog();
			FillFees();//Refresh our list of fees cause the user may have changed something.
		}

		private void butAuditTrail_Click(object sender,EventArgs e) {
			List<Permissions> perms=new List<Permissions>();
			perms.Add(Permissions.ProcFeeEdit);
			using FormAuditOneType FormA=new FormAuditOneType(0,perms,Lan.g(this,"All changes for")+" "+ProcCode.AbbrDesc+" - "+ProcCode.ProcCode,ProcCode.CodeNum);
			FormA.ShowDialog();
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
			if(ListTools.In(treatmentArea,TreatmentArea.Arch,TreatmentArea.Quad)){
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
				return "Code "+_procCodeOld.ProcCode+" "+textInLog+" changed from '"+oldVal+"' to  '"+newVal+"'\r\n";
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
			ProcCode.AlternateCode1=textAlternateCode1.Text;
			ProcCode.MedicalCode=textMedicalCode.Text;
			ProcCode.SubstitutionCode=textSubstitutionCode.Text;
			ProcCode.SubstOnlyIf=(SubstitutionCondition)comboSubstOnlyIf.SelectedIndex;
			ProcCode.Descript=textDescription.Text;
			ProcCode.AbbrDesc=textAbbrev.Text;
			ProcCode.LaymanTerm=textLaymanTerm.Text;
			ProcCode.ProcTime=strBTime.ToString();
			ProcCode.GraphicColor=butColor.BackColor;
			ProcCode.IsMultiVisit=checkMultiVisit.Checked;
			ProcCode.NoBillIns=checkNoBillIns.Checked;
			ProcCode.IsProsth=checkIsProsth.Checked;
			ProcCode.IsHygiene=checkIsHygiene.Checked;
			ProcCode.IsRadiology=checkIsRadiology.Checked;
			ProcCode.IsCanadianLab=checkIsCanadianLab.Checked;
			ProcCode.IsTaxed=checkIsAutoTaxed.Checked;
			ProcCode.DefaultNote=textNote.Text;
			ProcCode.DefaultTPNote=textTpNote.Text;
			ProcCode.DefaultClaimNote=textDefaultClaimNote.Text;
			ProcCode.PaintType=listPaintType.GetSelected<ToothPaintingType>();
			ProcCode.PaintText=textPaintText.Text;
			ProcCode.TreatArea=(TreatmentArea)listTreatArea.SelectedIndex;
			ProcCode.BaseUnits=PIn.Int(textBaseUnits.Text.ToString());
			ProcCode.DrugNDC=textDrugNDC.Text;
			ProcCode.RevenueCodeDefault=textRevenueCode.Text;
			if(listCategory.SelectedIndex!=-1) {
				ProcCode.ProcCat=_listProcCodeCatDefs[listCategory.SelectedIndex].DefNum;
			}
			ProcCode.ProvNumDefault=comboProvNumDefault.GetSelectedProvNum();			
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA, for CanadaTimeUnits
				ProcCode.CanadaTimeUnits=PIn.Double(textTimeUnits.Text);
			}
			if(checkBypassLockDate.Checked) {
				ProcCode.BypassGlobalLock=BypassLockStatus.BypassIfZero;
			}
			else {
				ProcCode.BypassGlobalLock=BypassLockStatus.NeverBypass;
			}
			ProcCode.AreaAlsoToothRange=checkToothRange.Checked;
			if(ProcedureCodes.Update(ProcCode,_procCodeOld)) {//whether new or not.
				string secLog="";
				secLog+=SecurityLogEntryHelper(_procCodeOld.AlternateCode1,ProcCode.AlternateCode1,"alt code");
				secLog+=SecurityLogEntryHelper(_procCodeOld.MedicalCode,ProcCode.MedicalCode,"medical code");
				secLog+=SecurityLogEntryHelper(_procCodeOld.SubstitutionCode,ProcCode.SubstitutionCode,"insurance substitution code");
				secLog+=SecurityLogEntryHelper(_procCodeOld.SubstOnlyIf.GetDescription(),ProcCode.SubstOnlyIf.GetDescription(),"only if box");
				secLog+=SecurityLogEntryHelper(_procCodeOld.Descript,ProcCode.Descript,"description");
				secLog+=SecurityLogEntryHelper(_procCodeOld.AbbrDesc,ProcCode.AbbrDesc,"abbreviation");
				secLog+=SecurityLogEntryHelper(_procCodeOld.LaymanTerm,ProcCode.LaymanTerm,"layman's terms");
				secLog+=SecurityLogEntryHelper(_procCodeOld.ProcTime,ProcCode.ProcTime,"procedure time");
				secLog+=SecurityLogEntryHelper(_procCodeOld.IsMultiVisit.ToString(),ProcCode.IsMultiVisit.ToString(),"'MultiVisit' box");
				secLog+=SecurityLogEntryHelper(_procCodeOld.NoBillIns.ToString(),ProcCode.NoBillIns.ToString(),"'Do not usually bill to Ins' box");
				secLog+=SecurityLogEntryHelper(_procCodeOld.IsProsth.ToString(),ProcCode.IsProsth.ToString(),"prosthesis box");
				secLog+=SecurityLogEntryHelper(_procCodeOld.IsHygiene.ToString(),ProcCode.IsHygiene.ToString(),"hygiene procedure box");
				secLog+=SecurityLogEntryHelper(_procCodeOld.IsRadiology.ToString(),ProcCode.IsRadiology.ToString(),"radiology box");
				secLog+=SecurityLogEntryHelper(_procCodeOld.IsTaxed.ToString(),ProcCode.IsTaxed.ToString(),"'Auto Tax' box");
				secLog+=SecurityLogEntryHelper(_procCodeOld.DefaultNote,ProcCode.DefaultNote,"default note");
				secLog+=SecurityLogEntryHelper(_procCodeOld.DefaultTPNote,ProcCode.DefaultTPNote,"TP'd note");
				secLog+=SecurityLogEntryHelper(_procCodeOld.DefaultClaimNote,ProcCode.DefaultClaimNote,"default claim note");
				secLog+=SecurityLogEntryHelper(_procCodeOld.TreatArea.GetDescription(),ProcCode.TreatArea.GetDescription(),"treatment area");
				secLog+=SecurityLogEntryHelper(_procCodeOld.BaseUnits.ToString(),ProcCode.BaseUnits.ToString(),"base units");
				secLog+=SecurityLogEntryHelper(_procCodeOld.DrugNDC,ProcCode.DrugNDC,"drug NDC");
				secLog+=SecurityLogEntryHelper(_procCodeOld.RevenueCodeDefault,ProcCode.RevenueCodeDefault,"default revenue");
				secLog+=SecurityLogEntryHelper(_procCodeOld.ProvNumDefault.ToString(),ProcCode.ProvNumDefault.ToString(),"provider number");
				secLog+=SecurityLogEntryHelper(_procCodeOld.BypassGlobalLock.ToString(),ProcCode.BypassGlobalLock.ToString(),"bypass global lock box");
				secLog+=SecurityLogEntryHelper(_procCodeOld.ProcCatDescript,ProcCode.ProcCatDescript,"category");
				secLog+=SecurityLogEntryHelper(_procCodeOld.AreaAlsoToothRange.ToString(),ProcCode.AreaAlsoToothRange.ToString(),"AreaAlsoToothRange box");
				SecurityLogs.MakeLogEntry(Permissions.ProcCodeEdit,0,secLog,ProcCode.CodeNum,_procCodeOld.DateTStamp);
				DataValid.SetInvalid(InvalidType.ProcCodes);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}
