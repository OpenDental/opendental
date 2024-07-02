using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDental.UI.Voice;
using OpenDentBusiness;
using SparksToothChart;
using WpfControls.UI;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormPerio : FormODBase {
		#region Fields - private
		private bool _isExamInUse;
		private bool _isLocalDefsChanged;
		private bool _isTenDown;
		private List<Def> _listDefsMiscColors;
		///<summary>Gets a list of missing teeth as strings on load. Includes "1"-"32", and "A"-"Z".</summary>
		private List<string> _listMissingTeeth;
		///<summary>This is not a list of valid procedures.  The only values to be trusted in this list are the ToothNum and CodeNum.  Never used.</summary>
		private List<Procedure> _listProcedures;
		private Patient _patient;
		//private int pagesPrinted;
		private PerioCell _perioCellCurLocation;
		private PerioCell _perioCellPrevLocation;
		private PerioExam _perioExam;
		private UserOdPref _userOdPrefCurrentOnly;
		private VoiceController _voiceController;
		#endregion Fields - private

		#region Constructor
		///<summary></summary>
		public FormPerio(Patient patient,List<Procedure> listProcedures)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			LayoutMenu();
			_patient=patient;
			_listProcedures=listProcedures;
			Lan.F(this);
		}
		#endregion Constructor

		#region Methods - private
		///<summary>Checks if perio is active on a MAD row with the current patient. If it is, locks editing.</summary>
		private void CheckMobileActivity() {
			if(!ClinicPrefs.IsODTouchAllowed(Clinics.ClinicNum)) {
				labelIsMobileActive.Enabled=false;
				labelIsMobileActive.Visible=false;
				butUnlockEClip.Enabled=false;
				butUnlockEClip.Visible=false;
				return;
			}
			//If someone is currently using this patients exams in eClipboard, ask if this user would like to kick off the other user.
			bool isExamInUse=MobileAppDevices.IsInUse(_patient.PatNum,MADPage.PerioExamEditPage,MADPage.PerioExamListPage,MADPage.PerioExamOverviewPage,MADPage.FileViewerPage,MADPage.Undefined);
			bool hasUIChanged=!(_isExamInUse && isExamInUse); //This checks if the state has changed, we don't want to keep refreshing UI if it hasn't.
			if(!hasUIChanged) {
				return;
			}
			_isExamInUse=isExamInUse;
			if(_isExamInUse) { //Answered No, they would not like to force eClip to checkin, or this method got called by the timer.
				timerEClipCheck.Start();
				SetEClipBoardEditing(true);
				return;
			}
			else {
				timerEClipCheck.Stop();
				SetEClipBoardEditing(false);
			}
		}

		private string ConvertListToString(List<string> listStringsTeeth){
			if(listStringsTeeth.Count==0){
				return "none";
			}
			string retVal="";
			for(int i=0;i<listStringsTeeth.Count;i++){
				if(i>0)
					retVal+=",";
				retVal+=listStringsTeeth[i];
			}
			return retVal;
		}

		///<summary>Creates a new perio chart and marks any teeth missing as necessary.</summary>
		private void CreateNewPerioChart() {
			_perioExam=new PerioExam();
			_perioExam.PatNum=_patient.PatNum;
			_perioExam.ExamDate=DateTime.Today;
			_perioExam.ProvNum=_patient.PriProv;
			_perioExam.DateTMeasureEdit=MiscData.GetNowDateTime();
			PerioExams.Insert(_perioExam);
			PerioMeasures.SetSkipped(_perioExam.PerioExamNum,GetSkippedTeeth());
		}

		private void FillCounts(){
			textCountProb.Text=contrPerio.CountTeeth(PerioSequenceType.Probing).Count.ToString();
			textCountMGJ.Text=contrPerio.CountTeeth(PerioSequenceType.MGJ).Count.ToString();
			textCountGing.Text=contrPerio.CountTeeth(PerioSequenceType.GingMargin).Count.ToString();
			textCountCAL.Text=contrPerio.CountTeeth(PerioSequenceType.CAL).Count.ToString();
			textCountFurc.Text=contrPerio.CountTeeth(PerioSequenceType.Furcation).Count.ToString();
			textCountMob.Text=contrPerio.CountTeeth(PerioSequenceType.Mobility).Count.ToString();
		}

		///<summary>Usually set the selected index first</summary>
		private void FillGrid(bool selectCell=true) {
			if(gridODExam.GetSelectedIndex()!=-1){
				contrPerio.AllowPerioEdit=true;
				if(!Security.IsAuthorized(EnumPermType.PerioEdit,contrPerio.ListPerioExams[gridODExam.GetSelectedIndex()].ExamDate,suppressMessage:true)) {
					contrPerio.AllowPerioEdit=false;
				}
				_perioExam=contrPerio.ListPerioExams[gridODExam.GetSelectedIndex()];
			}
			contrPerio.IdxExamSelected=gridODExam.GetSelectedIndex();
			contrPerio.SetShowCurrentExamOnly(checkShowCurrent.Checked);
			contrPerio.LoadData(selectCell);
			FillIndexes();
			FillCounts();
			contrPerio.Invalidate();
			contrPerio.Focus();//this still doesn't seem to work to enable first arrow click to move
			if(_perioExam!=null) {
				textExamNotes.Text=_perioExam.Note;
			}
			FormPerio_ChangeTitle();
		}

		private void FillIndexes(){
			textIndexPlaque.Text=contrPerio.ComputeIndex(BleedingFlags.Plaque);
			textIndexCalculus.Text=contrPerio.ComputeIndex(BleedingFlags.Calculus);
			textIndexBleeding.Text=contrPerio.ComputeIndex(BleedingFlags.Blood);
			textIndexSupp.Text=contrPerio.ComputeIndex(BleedingFlags.Suppuration);
		}

		///<summary>Returns the first non-empty cell for the tooth. If all cells in the tooth are filled, returns the last cell of the tooth.</summary>
		private int FirstEmptyPositionX(int toothNum,int yVal) {
			int xVal;
			if(_perioCellCurLocation.IsFacial) {
				if(toothNum<=16) {
					xVal=(toothNum-1)*3+1;
				}
				else {//ToothNum >= 17
					xVal=46-(toothNum-17)*3;
				}
				if(string.IsNullOrEmpty(contrPerio.GetCellText(xVal,yVal))) {
					return xVal;
				}
				xVal++;
				if(string.IsNullOrEmpty(contrPerio.GetCellText(xVal,yVal))) {
					return xVal;
				}
				return ++xVal;
			}
			else {//Lingual
				if(toothNum<=16) {
					xVal=(toothNum-1)*3+3;
				}
				else {//ToothNum >= 17
					xVal=48-(toothNum-17)*3;
				}
				if(string.IsNullOrEmpty(contrPerio.GetCellText(xVal,yVal))) {
					return xVal;
				}
				xVal--;
				if(string.IsNullOrEmpty(contrPerio.GetCellText(xVal,yVal))) {
					return xVal;
				}
				return --xVal;
			}
		}

		///<summary>Returns the first non-empty cell for the ColRow passed in.</summary>
		private int FirstEmptyPositionX(ColRow colRow) {
			int xVal=colRow.Col;
			int yVal=colRow.Row;
			if(string.IsNullOrEmpty(contrPerio.GetCellText(xVal,yVal))) {
				return xVal;
			}
			//First position is not empty. Get the 2nd position of the colrow passed in.
			ColRow colRowNextAvailable=contrPerio.TryFindNextCell(colRow,isReverse:false,setDirection:false);
			if(colRowNextAvailable==null) {
				return xVal;
			}
			xVal=colRowNextAvailable.Col;
			if(string.IsNullOrEmpty(contrPerio.GetCellText(xVal,yVal))) {
				return xVal;
			}
			//The section position is not empty. Get the last position of the colrow passed in.
			colRowNextAvailable=contrPerio.TryFindNextCell(colRowNextAvailable,isReverse:false,setDirection:false);
			if(colRowNextAvailable==null){
				return xVal;
			}
			return colRowNextAvailable.Col;//this should be the last position of the colrow passed in.
		}

		private ColRow GetColRowForMobility(int toothNum) {
			int yVal;
			int xVal;
			if(toothNum <= 16) {
				xVal=(toothNum-1)*3+2;//Middle cell
				yVal=10;
			}
			else {//ToothNum >= 17
				xVal=47-(toothNum-17)*3;
				yVal=32;
			}
			return new ColRow(xVal,yVal);
		}

		///<summary>Returns a colrow for the tooth's passed in.</summary>
		private ColRow GetColRowForTooth(int toothNum,bool isFacial) {
			int x=-1;
			int y=-1;
			bool isLingual=!isFacial;
			if(isFacial && toothNum.Between(1,16)) {
				x=(toothNum)*3-2;
				if (checkShowCurrent.Checked) {
					y=5;
				}
				else {
					y=6-Math.Min(6,contrPerio.IdxExamSelected+1);
				}
			}
			else if(isFacial && toothNum.Between(17,32)) {
				x=(32-toothNum)*3+1;
				if (checkShowCurrent.Checked) {
					y=37;
				}
				else {
					y=36+Math.Min(6,contrPerio.IdxExamSelected+1);
				}
			}
			else if(isLingual && toothNum.Between(1,16)) {
				x=(toothNum)*3;
				if (checkShowCurrent.Checked) {
					y=15;
				}
				else {
					y=14+Math.Min(6,contrPerio.IdxExamSelected+1);
				}
			}
			else {//if(isLingual && toothNum.Between(17,32)) {
				x=(32-toothNum)*3+3;
				if (checkShowCurrent.Checked) {
					y=26;
				}
				else {
					y=27-Math.Min(6,contrPerio.IdxExamSelected+1);
				}
			}
			return new ColRow(x,y);
		}

		///<summary>Gets the index of the given target string, within the value string, where the count is the number of matched iterations. Returns -1 if there are no matches or less than count matches.</summary>
		private int GetIndexCount(string value,string target,int count) {
			int countOccurrences=0;
			for(int i=0;i<value.Length-target.Length+1;i++) {
				if(value.Substring(i,target.Length)==target) {
					countOccurrences++;
					if(countOccurrences==count) {
						return i;
					}
				}
			}
			return -1;
		}

		///<summary>Gets the next tooth colrow.  Takes into account skipped teeth.</summary>
		private ColRow GetNextToothColRow(int toothOld,bool isFacial) {
			ColRow colRow=GetColRowForTooth(toothOld,isFacial);
			ColRow colRowNext=contrPerio.TryFindNextCell(colRow,isReverse:false);
			if(colRowNext==null){
				return colRow;
			}
			return colRowNext;
		}

		///<summary>Gets the previous tooth colrow. Takes into account skipped teeth.</summary>
		private ColRow GetPreviousToothColRow(int toothOld,bool isFacial,bool doSetDirection=true) {
			ColRow colRow=GetColRowForTooth(toothOld,isFacial);
			ColRow colRowPrev=contrPerio.TryFindNextCell(colRow,isReverse:true,doSetDirection);
			if(colRowPrev==null){
				return colRow;
			}
			return colRowPrev;
		}

		///<summary>Returns a list of skipped teeth.</summary>
		private List<int> GetSkippedTeeth() {
			List<int> listSkippedTeeth=new List<int>();
			if(contrPerio.ListPerioExams.Count > 0) {
				//set skipped teeth based on the last exam in the list: 
				listSkippedTeeth=PerioMeasures.GetSkipped(contrPerio.ListPerioExams[contrPerio.ListPerioExams.Count-1].PerioExamNum);
			}
			//For patient's first perio chart, any teeth marked missing are automatically marked skipped.
			if(contrPerio.ListPerioExams.Count != 0 && !PrefC.GetBool(PrefName.PerioSkipMissingTeeth)) {
				return listSkippedTeeth.Distinct().ToList();
			}
			for(int i=0;i<_listMissingTeeth.Count;i++) {
				if(_listMissingTeeth[i].CompareTo("A") >= 0 && _listMissingTeeth[i].CompareTo("Z") <= 0) {//if is a letter (not a number)
					continue;//Skipped teeth are only recorded by tooth number within the perio exam.
				}
				int toothNum=PIn.Int(_listMissingTeeth[i]);
				//Check if this tooth has had an implant done AND the office has the preference to SHOW implants
				if(PrefC.GetBool(PrefName.PerioTreatImplantsAsNotMissing) && ContrPerio.IsImplant(toothNum)) {
					listSkippedTeeth.RemoveAll(x => x==toothNum);//Remove the tooth from the list of skipped teeth if it exists.
					continue;//We do note want to add it back to the list below.
				}
				//This tooth is missing and we know it is not an implant OR the office has the preference to ignore implants.
				//Simply add it to our list of skipped teeth.
				listSkippedTeeth.Add(toothNum);
			}
			return listSkippedTeeth.Distinct().ToList();
		}

		///<summary>Returns a colrow for the tooth's surface passed in.</summary>
		private ColRow GetSurfaceColRowForTooth(int toothNum,bool isFacial,EnumPerioMMidD perioSurface) {
			ColRow colRowForTooth=GetColRowForTooth(toothNum,isFacial);
			//Get set to either the right(Lingual) or left(Facial) position.
			int xPos=colRowForTooth.Col;
			//Midline=Looking at the Perio Chart(FormPerio.cs designer), this is the vertial line down the middle.
			//Middle= will always be the middle cell of each triplet of cells in the perio chart.
			//Mesial=This is the position closest to the Midline. Teeth 1-8 and 25-32 right cell, teeth 9-24 left cell.
			//Distal=This is the postion furthest away from the midline. Teeth 1-8 and 25-32 left cell, teeth 9-24 right cell.
			switch(perioSurface) {
				case EnumPerioMMidD.Distal:
					if(toothNum.Between(9,24)) {
						//Get the right position.
						xPos=xPos + (isFacial ? 2 : 0);
					}
					else {
						//Teeth 1-8 and 25-32. Get the left position.
						xPos=xPos + (isFacial ? 0 : -2);
					}
					break;
				case EnumPerioMMidD.Middle:
					xPos=xPos + (isFacial ? 1 : -1);//middle position
					break;
				case EnumPerioMMidD.Mesial:
					if(toothNum.Between(9,24)) {
						//Get the left position.
						xPos=xPos + (isFacial ? 0 : -2);
					}
					else {
						//Teeth 1-8 and 25-32 right position
						xPos=xPos + (isFacial ? 2 : 0);
					}
					break;
			}
			return new ColRow(xPos,colRowForTooth.Row);
		}

		private int GetToothNumFromColRow(ColRow colRow) {
			PerioCell perioCell=contrPerio.GetPerioCellFromColRow(colRow);
			return perioCell.ToothNum;
		}

		private void LayoutMenu(){
			menuMain.BeginUpdate();
			//Setup----------------------------------------------------------------------------------------------------------
			menuMain.Add(new MenuItemOD("Setup",menuItemSetup_Click));
			menuMain.EndUpdate();
		}

		///<summary>Uses the AdvanceCell method to figure out the.</summary>
		private ColRow NextEmptyColRowHelper(ColRow colRowForTooth,bool isReverse) {
			ColRow colRowNextAvailable=contrPerio.TryFindNextCell(colRowForTooth,isReverse);
			if(colRowNextAvailable==null){
				colRowNextAvailable=colRowForTooth;
			}
			if(string.IsNullOrEmpty(contrPerio.GetCellText(colRowNextAvailable.Col,colRowNextAvailable.Row))) {
				return colRowNextAvailable;//Tooth is not empty.
			}
			int toothNum=GetToothNumFromColRow(colRowNextAvailable);
			if(toothNum<1 || toothNum>32) {//I think we need <=1 and >=32
				return colRowForTooth;
			}
			if(colRowForTooth.IsEquivalentTo(colRowNextAvailable)) {
				return colRowForTooth;//We are probably trying to advance into a skipped tooth that's the last tooth.
			}
			return NextEmptyColRowHelper(colRowNextAvailable,isReverse);
		}

		///<summary>The only valid numbers are 0 through 9</summary>
		private void NumberClicked(int number){
			if(contrPerio.IdxExamSelected==-1) {
				MessageBox.Show(Lan.g(this,"Please add or select an exam first in the list to the left."));
				return;
			}
			if(_isTenDown) {
				contrPerio.ButtonPressed(10+number);
			}
			else {
				contrPerio.ButtonPressed(number);
			}
			_isTenDown=false;
			SetLabelFwdReverse();
			contrPerio.Focus();
			FormPerio_ChangeTitle();
		}

		///<summary>After this method runs, the selected index is usually set.</summary>
		private void RefreshListExams(bool skipRefreshMeasures=false) {
			//most recent date at the bottom
			contrPerio.ListPerioExams=PerioExams.Refresh(_patient.PatNum);
			if(!skipRefreshMeasures) {
				contrPerio.ListPerioMeasures=PerioMeasures.GetForPatient(_patient.PatNum);
			}
			gridODExam.BeginUpdate();
			gridODExam.Columns.Clear();
			gridODExam.ListGridRows.Clear();
			OpenDental.UI.GridColumn gridColumn;
			gridColumn=new OpenDental.UI.GridColumn(Lan.g("TablePerioExam","Exam Date"),75,HorizontalAlignment.Center);
			gridODExam.Columns.Add(gridColumn);
			gridColumn=new OpenDental.UI.GridColumn(Lan.g("TablePerioExam","Provider"),75,HorizontalAlignment.Center);
			gridODExam.Columns.Add(gridColumn);
			gridColumn=new OpenDental.UI.GridColumn(Lan.g("TablePerioExam","Exam Notes"),150,HorizontalAlignment.Center);
			gridODExam.Columns.Add(gridColumn);
			for(int i=0;i<contrPerio.ListPerioExams.Count;i++) {
				OpenDental.UI.GridRow gridRow=new OpenDental.UI.GridRow();
				gridRow.Cells.Add(contrPerio.ListPerioExams[i].ExamDate.ToShortDateString());
				gridRow.Cells.Add(Providers.GetAbbr(contrPerio.ListPerioExams[i].ProvNum));
				int index5thNewLine=GetIndexCount(contrPerio.ListPerioExams[i].Note,"\n",5);//Our goal is to get no more than 5 lines into the text box.
				int noteEnd=90;//90 characters is about what it takes to get most strings to fit the 5 lines.
				if(contrPerio.ListPerioExams[i].Note.Length > noteEnd || index5thNewLine > -1) {
					if(index5thNewLine > -1) {
						noteEnd=Math.Min(noteEnd,index5thNewLine);
					}
					gridRow.Cells.Add(contrPerio.ListPerioExams[i].Note.Substring(0,noteEnd)+"(...)");
				} 
				else {
					gridRow.Cells.Add(contrPerio.ListPerioExams[i].Note);
				}
				gridODExam.ListGridRows.Add(gridRow);
			}
			gridODExam.EndUpdate();
		}

		///<summary>Inserts a perio measure for each tooth using the PerioDefaultProbeDepths preference. Does not insert values for skipped teeth</summary>
		private void SetDefault(string rawPerioMeasures) {
			if(rawPerioMeasures.Length!=192) {
				return;
			}
			List<int> listSkippedTeeth=GetSkippedTeeth();
			string rowF1=rawPerioMeasures.Substring(0,48);
			string rowL1=rawPerioMeasures.Substring(48,48);
			string rowL32=rawPerioMeasures.Substring(96,48);
			string rowF32=rawPerioMeasures.Substring(144);
			List<PerioMeasure> listPerioMeasures=new List<PerioMeasure>();
			for(int i=0;i<16;i++) {
				if(listSkippedTeeth.Contains(i+1)) {
					continue;
				}
				PerioMeasure perioMeasureUpper=new PerioMeasure();//1-16.
				perioMeasureUpper.PerioExamNum=_perioExam.PerioExamNum;
				perioMeasureUpper.IntTooth=i+1;
				perioMeasureUpper.SequenceType=PerioSequenceType.Probing;
				if(i<8) {//Right side.
					//Upper tooth default values.
					perioMeasureUpper.DBvalue=PIn.Int(rowF1[i*3].ToString());
					perioMeasureUpper.Bvalue= PIn.Int(rowF1[i*3+1].ToString());
					perioMeasureUpper.MBvalue=PIn.Int(rowF1[i*3+2].ToString());
					perioMeasureUpper.DLvalue=PIn.Int(rowL1[i*3].ToString());
					perioMeasureUpper.Lvalue= PIn.Int(rowL1[i*3+1].ToString());
					perioMeasureUpper.MLvalue=PIn.Int(rowL1[i*3+2].ToString());
				}
				else {//Left side.
					//Upper tooth default values.
					perioMeasureUpper.MBvalue=PIn.Int(rowF1[i*3].ToString());
					perioMeasureUpper.Bvalue= PIn.Int(rowF1[i*3+1].ToString());
					perioMeasureUpper.DBvalue=PIn.Int(rowF1[i*3+2].ToString());
					perioMeasureUpper.MLvalue=PIn.Int(rowL1[i*3].ToString());
					perioMeasureUpper.Lvalue= PIn.Int(rowL1[i*3+1].ToString());
					perioMeasureUpper.DLvalue=PIn.Int(rowL1[i*3+2].ToString());
				}
				listPerioMeasures.Add(perioMeasureUpper);
			}
			for(int i=0;i<16;i++) {
				if(listSkippedTeeth.Contains(32-i)) {
					continue;
				}
				PerioMeasure perioMeasureLower=new PerioMeasure();//32-17.
				perioMeasureLower.PerioExamNum=_perioExam.PerioExamNum;
				perioMeasureLower.IntTooth=32-i;
				perioMeasureLower.SequenceType=PerioSequenceType.Probing;
				if(i<8) {
					//Lower tooth default values.
					perioMeasureLower.DBvalue=PIn.Int(rowF32[i*3].ToString());
					perioMeasureLower.Bvalue= PIn.Int(rowF32[i*3+1].ToString());
					perioMeasureLower.MBvalue=PIn.Int(rowF32[i*3+2].ToString());
					perioMeasureLower.DLvalue=PIn.Int(rowL32[i*3].ToString());
					perioMeasureLower.Lvalue= PIn.Int(rowL32[i*3+1].ToString());
					perioMeasureLower.MLvalue=PIn.Int(rowL32[i*3+2].ToString());
				}
				else {
					perioMeasureLower.MBvalue=PIn.Int(rowF32[i*3].ToString());
					perioMeasureLower.Bvalue= PIn.Int(rowF32[i*3+1].ToString());
					perioMeasureLower.DBvalue=PIn.Int(rowF32[i*3+2].ToString());
					perioMeasureLower.MLvalue=PIn.Int(rowL32[i*3].ToString());
					perioMeasureLower.Lvalue= PIn.Int(rowL32[i*3+1].ToString());
					perioMeasureLower.DLvalue=PIn.Int(rowL32[i*3+2].ToString());
				}
				listPerioMeasures.Add(perioMeasureLower);
			}
			PerioMeasures.InsertMany(listPerioMeasures);
		}

		///<summary>Unique to when eClipboard is viewing / editing this patients perio exams. Starts the eClipboard timer check.</summary>
		private void SetEClipBoardEditing(bool isEditingOnEClipBoard) {
			contrPerio.AllowPerioEdit=!isEditingOnEClipBoard;
			gridODExam.Enabled=!isEditingOnEClipBoard;
			butAdd.Enabled=!isEditingOnEClipBoard;
			butCopyNote.Enabled=!isEditingOnEClipBoard;
			butCopyPrevious.Enabled=!isEditingOnEClipBoard;
			butDelete.Enabled=!isEditingOnEClipBoard;
			butListen.Enabled=!isEditingOnEClipBoard;
			butDefault.Enabled=!isEditingOnEClipBoard;
			butUnlockEClip.Enabled=isEditingOnEClipBoard;
			labelIsMobileActive.Visible=isEditingOnEClipBoard;
		}

		///<summary>Sets Forward/Reverse label in Current Direction groupbox based on expected direction from the path and the current direction</summary>
		private void SetLabelFwdReverse(){
			PerioCell perioCell=contrPerio.GetCurrentCell() ;
			List<PerioAdvancePos> listPerioAdvancePoss=PerioAdvancePos.GetPath(contrPerio.EnumAdvanceSequence_);
			PerioAdvancePos perioAdvancePos=listPerioAdvancePoss.Find(x => x.ToothNum==perioCell.ToothNum && x.IsFacial==perioCell.IsFacial);
			if(perioAdvancePos is null){
				return;
			}
			labelFwdReverse.Text="Forward";
			if(perioAdvancePos.EnumCurrentDirection2!=contrPerio.EnumCurrentDirection_){//Direction expected by the path and current direction do not match
				labelFwdReverse.Text="Reverse";
			}
		}

		///<summary>Gets the next available tooth(ColRow) if there is an empty cell. Returns a ColRow with value (-1,-1) if all cells are filled.</summary>
		private ColRow TryGetNextAvailableColRowForTooth(int toothNum,bool isFacial) {
			//Get the first colrow for the tooth passed in.
			ColRow colRowForTooth=GetColRowForTooth(toothNum,isFacial);
			//Check all three positions for the tooth. It will return either an empty position or the last position of the tooth.
			ColRow colRowNextAvailable=new ColRow(FirstEmptyPositionX(colRowForTooth),colRowForTooth.Row);
			//Check the position that was returned above for measurements.
			if(!string.IsNullOrEmpty(contrPerio.GetCellText(colRowNextAvailable.Col,colRowNextAvailable.Row))) {
				return new ColRow(-1,-1);//Tooth is not empty.
			}
			return colRowNextAvailable;
		}

		///<summary>Gets the next available colrow.</summary>
		private ColRow TryGetNextEmptyColRow(ColRow colRowForTooth,bool isReverse) {
			if(string.IsNullOrEmpty(contrPerio.GetCellText(colRowForTooth.Col,colRowForTooth.Row))) {
				return colRowForTooth;//Tooth is not empty.
			}
			return NextEmptyColRowHelper(colRowForTooth,isReverse);
		}
		#endregion Methods - private

		#region Methods - Event Handlers public
		public override void ProcessSignalODs(List<Signalod> listSignals) {
			if(listSignals.Any(x => x.IType==InvalidType.EClipboard)) {
				CheckMobileActivity();
			}
			//This signal comes in for every action taken by a user on eClipboard - Perio.
			if(_perioExam!=null && listSignals.Any(x=>x.FKeyType==KeyType.PatNum && x.FKey==_perioExam.PatNum && x.IType==InvalidType.PerioExams)) {
				RefreshListExams(false);
				if(gridODExam.ListGridRows.Count>0) {
					gridODExam.SetSelected(gridODExam.ListGridRows.Count-1);
				}
				FillGrid(selectCell:true);
				//Since FillGrid can re-enable the GridPerio editing.
				SetEClipBoardEditing(_isExamInUse);
			}
		}
		#endregion Methods - Event Handlers public

		#region Methods - Event Handlers private
		private void FormPerio_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			//There is no OK vs Cancel. It just always saves.
			if(_perioExam!=null) { //When no Exam is selected or ExamList is empty we don't want to save ExamNotes.
				PerioExam perioExamOld=_perioExam.Copy();
				_perioExam.Note=textExamNotes.Text;
				PerioExams.Update(_perioExam,perioExamOld);
			}
			if(gridODExam.GetSelectedIndex()!=-1){
				contrPerio.SaveCurExam(_perioExam);
			}
			_voiceController?.Dispose();
			if(_isLocalDefsChanged){
				DataValid.SetInvalid(InvalidType.Defs, InvalidType.Prefs);
			}
			if(_userOdPrefCurrentOnly==null) {
				UserOdPrefs.Insert(new UserOdPref() {
					UserNum=Security.CurUser.UserNum,
					FkeyType=UserOdFkeyType.PerioCurrentExamOnly,
					ValueString=POut.Bool(checkShowCurrent.Checked)
				});
				DataValid.SetInvalid(InvalidType.UserOdPrefs);
			}
			else {
				UserOdPref userOdPrefOld=_userOdPrefCurrentOnly.Clone();
				_userOdPrefCurrentOnly.ValueString=POut.Bool(checkShowCurrent.Checked);
				if(UserOdPrefs.Update(_userOdPrefCurrentOnly,userOdPrefOld)) {
					//Only need to signal cache refresh on change.
					DataValid.SetInvalid(InvalidType.UserOdPrefs);
				}
			}
			UserOdPref userOdPrefAdvance=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.PerioAutoAdvanceFacialsFirst).FirstOrDefault();
			if(userOdPrefAdvance==null) {
				UserOdPrefs.Insert(new UserOdPref() {
					UserNum=Security.CurUser.UserNum,
					FkeyType=UserOdFkeyType.PerioAutoAdvanceFacialsFirst,
					ValueString=POut.Bool(radioFacialsFirst.Checked)
				});
				DataValid.SetInvalid(InvalidType.UserOdPrefs);
			}
			else {
				UserOdPref userOdPrefOld=userOdPrefAdvance.Clone();
				userOdPrefAdvance.ValueString=POut.Bool(radioFacialsFirst.Checked);//0=MaxFirst, 1=FacialsFirst
				if(UserOdPrefs.Update(userOdPrefAdvance,userOdPrefOld)) {
					//Only need to signal cache refresh on change.
					DataValid.SetInvalid(InvalidType.UserOdPrefs);
				}
			}
			Plugins.HookAddCode(this,"FormPerio.Closing_end");
		}

		private void FormPerio_Load(object sender, System.EventArgs e) {
			labelListening.Visible=false;
			_listDefsMiscColors=Defs.GetDefsForCategory(DefCat.MiscColors,isShort:true);
			butColorBleed.BackColor=_listDefsMiscColors[(int)DefCatMiscColors.PerioBleeding].ItemColor;
			butColorPus.BackColor=_listDefsMiscColors[(int)DefCatMiscColors.PerioSuppuration].ItemColor;
			butColorPlaque.BackColor=_listDefsMiscColors[(int)DefCatMiscColors.PerioPlaque].ItemColor;
			butColorCalculus.BackColor=_listDefsMiscColors[(int)DefCatMiscColors.PerioCalculus].ItemColor;
			textRedProb.Text=PrefC.GetString(PrefName.PerioRedProb);
			textRedMGJ.Text =PrefC.GetString(PrefName.PerioRedMGJ);
			textRedGing.Text=PrefC.GetString(PrefName.PerioRedGing);
			textRedCAL.Text =PrefC.GetString(PrefName.PerioRedCAL);
			textRedFurc.Text=PrefC.GetString(PrefName.PerioRedFurc);
			textRedMob.Text =PrefC.GetString(PrefName.PerioRedMob);
			if(Programs.IsEnabled(ProgramName.BolaAI)) {
				butListen.Visible=false;
				butBolaGet.Visible=false;
			}
			else {
				if(ProgramProperties.IsAdvertisingDisabled(ProgramName.BolaAI)) {
					butBolaGet.Visible=false;
				}
				butBolaLaunch.Visible=false;
			}
			//Procedure[] procList=Procedures.Refresh(PatCur.PatNum);
			List<ToothInitial> listToothInitials=ToothInitials.GetPatientData(_patient.PatNum);
			_listMissingTeeth=ToothInitials.GetMissingOrHiddenTeeth(listToothInitials);
			RefreshListExams();
			gridODExam.SetSelected(contrPerio.ListPerioExams.Count-1,true);//this works even if no items.
			_userOdPrefCurrentOnly=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.PerioCurrentExamOnly).FirstOrDefault();
			if(_userOdPrefCurrentOnly != null && PIn.Bool(_userOdPrefCurrentOnly.ValueString)) {
				checkShowCurrent.Checked=true;
			}
			UserOdPref userOdPrefAdvance=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.PerioAutoAdvanceFacialsFirst).FirstOrDefault();
			if(userOdPrefAdvance!=null && PIn.Bool(userOdPrefAdvance.ValueString)) {
				radioFacialsFirst.Checked=true;
				contrPerio.EnumAdvanceSequence_=EnumAdvanceSequence.FacialsFirst;
			}
			FillGrid();
			CheckMobileActivity();
			Plugins.HookAddCode(this,"FormPerio.Load_end");
		}

		/// <summary>Used to force focus to the hidden textbox when showing this form.</summary>
		private void FormPerio_Shown(object sender,EventArgs e) {
			textInputBox.Focus();//This cannot go into load because focus must come after window has been shown.
		}

		private void FormPerio_ChangeTitle() {
			if(!Programs.IsEnabled(ProgramName.BolaAI)) {
				return;
			}
			ColRow colRow=contrPerio.ColRowSelected;
			if(colRow.Col==-1 || colRow.Row==-1) {
				return;
			}
			PerioCell perioCell=contrPerio.GetPerioCellFromColRow(colRow);
			PerioSequenceType perioSequenceType=contrPerio.GetSequenceForColRow(colRow);
			string perioSequenceTypeAbbr="";
			//Any changes here should also be changed in ContrPerio.EnterValue and ContrPerio.OnKeyDown
			if(perioSequenceType==PerioSequenceType.MGJ) {
				perioSequenceTypeAbbr="j";
			}
			else if(perioSequenceType==PerioSequenceType.GingMargin) {
				perioSequenceTypeAbbr="g";
			}
			else if(perioSequenceType==PerioSequenceType.Furcation) {
				perioSequenceTypeAbbr="f";
			}
			else if(perioSequenceType==PerioSequenceType.Mobility) {
				perioSequenceTypeAbbr="m";
			}
			else if(perioSequenceType==PerioSequenceType.Probing) {
				perioSequenceTypeAbbr=".";
			}
			string facialOrLingual="L";
			if(perioCell.IsFacial) {
				facialOrLingual="F";
			}
			string abbrMCD="D";//Distal
			if(colRow.Col%3==2) {//Center is always the same place in perioCell
				abbrMCD="C";
			}
			else if(colRow.Col%3==1 && perioCell.ToothNum.Between(9,24)) {//Mesial, if on left side of perioCell for teeth 9-24
				abbrMCD="M";
			}
			else if(colRow.Col%3==0 && !perioCell.ToothNum.Between(9,24)) {//Mesial, if on right side of perioCell for teeth 1-8 and 25-32
				abbrMCD="M";
			}
			this.Text="Perio Chart ("+colRow.Col+","+colRow.Row+","+perioCell.ProbingPosition+")("+perioCell.ToothNum+","+perioSequenceTypeAbbr+","+facialOrLingual+","+abbrMCD+")";
			//(14,.,L,M) or (3,g,F,C)
			//tooth number
			//type of row (shortcut abbreviations, lowercase)
			//L or F
			//which of the 3: M, C, or D
		}

		private void menuItemSetup_Click(Object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.Setup)) {
				return;
			}
			using FormPerioSetup formPerioSetup=new FormPerioSetup();
			formPerioSetup.ShowDialog();
		}

		private void pd2_PrintPage(object sender, PrintPageEventArgs printPageEventArgs){//raised for each page to be printed.
			Graphics g=printPageEventArgs.Graphics;
			//MessageBox.Show(grfx.
			float yPos=67+25+20+20+20+6;
			float xPos=100;
			g.TranslateTransform(xPos,yPos);
			contrPerio.DrawChart(g);//have to print graphics first, or they cover up title.
			g.DrawString("F",new Font("Arial",15),Brushes.Black,new Point(-26,92));
			g.DrawString("L",new Font("Arial",15),Brushes.Black,new Point(-26,212));
			g.DrawString("L",new Font("Arial",15),Brushes.Black,new Point(-26,416));
			g.DrawString("F",new Font("Arial",15),Brushes.Black,new Point(-26,552));
			g.TranslateTransform(-xPos,-yPos);
			yPos=67;
			xPos=100;
			string clinicName="";
			//This clinic name could be more accurate here in the future if we make perio exams clinic specific.
			//Perhaps if there were a perioexam.ClinicNum column.
			if(_patient.ClinicNum!=0) {
				Clinic clinic=Clinics.GetClinic(_patient.ClinicNum);
				clinicName=clinic.Description;
			} 
			else {
				clinicName=PrefC.GetString(PrefName.PracticeTitle);
			}
			StringFormat stringFormat=new StringFormat();
			stringFormat.Alignment=StringAlignment.Center;
			g.DrawString(Lan.g(this,"Periodontal Charting"),new Font("Arial",15),Brushes.Black,
				new RectangleF(xPos,yPos,650,25),stringFormat);			
			yPos+=25;
			g.DrawString(clinicName,new Font("Arial",11),Brushes.Black
				,new RectangleF(xPos,yPos,650,25),stringFormat);
			yPos+=20;
			g.DrawString(_patient.GetNameFL(),new Font("Arial",11),Brushes.Black
				,new RectangleF(xPos,yPos,650,25),stringFormat);
			yPos+=20;
			string time=MiscData.GetNowDateTime().ToShortDateString();
			g.DrawString(time,new Font("Arial",11),Brushes.Black,
				new RectangleF(xPos,yPos,650,25),stringFormat);
			yPos+=20;//Offset for printed text
			yPos+=688;//Offset for grid that we already drew.
			using Font font=new Font("Arial",9);
			g.FillEllipse(new SolidBrush(butColorPlaque.BackColor),xPos,yPos+3,8,8);
			g.DrawString(Lan.g(this,"Plaque Index:")+" "+contrPerio.ComputeIndex(BleedingFlags.Plaque)+" %"
				,font,Brushes.Black,xPos+12,yPos);
			yPos+=20;
			g.FillEllipse(new SolidBrush(butColorCalculus.BackColor),xPos,yPos+3,8,8);
			g.DrawString(Lan.g(this,"Calculus Index:")+" "+contrPerio.ComputeIndex(BleedingFlags.Calculus)+" %"
				,font,Brushes.Black,xPos+12,yPos);
			yPos+=20;
			g.FillEllipse(new SolidBrush(butColorBleed.BackColor),xPos,yPos+3,8,8);
			g.DrawString(Lan.g(this,"Bleeding Index:")+" "+contrPerio.ComputeIndex(BleedingFlags.Blood)+" %"
				,font,Brushes.Black,xPos+12,yPos);
			yPos+=20;
			g.FillEllipse(new SolidBrush(butColorPus.BackColor),xPos,yPos+3,8,8);
			g.DrawString(Lan.g(this,"Suppuration Index:")+" "+contrPerio.ComputeIndex(BleedingFlags.Suppuration)+" %"
				,font,Brushes.Black,xPos+12,yPos);
			yPos+=20;
			g.DrawString(Lan.g(this,"Teeth with Probing greater than or equal to")+" "+textRedProb.Text+" mm: "
				+ConvertListToString(contrPerio.CountTeeth(PerioSequenceType.Probing))
				,font,Brushes.Black,xPos,yPos);
			yPos+=20;
			g.DrawString(Lan.g(this,"Teeth with MGJ less than or equal to")+" "+textRedMGJ.Text+" mm: "
				+ConvertListToString(contrPerio.CountTeeth(PerioSequenceType.MGJ))
				,font,Brushes.Black,xPos,yPos);
			yPos+=20;
			g.DrawString(Lan.g(this,"Teeth with Gingival Margin greater than or equal to")+" "+textRedGing.Text+" mm: "
				+ConvertListToString(contrPerio.CountTeeth(PerioSequenceType.GingMargin))
				,font,Brushes.Black,xPos,yPos);
			yPos+=20;
			g.DrawString(Lan.g(this,"Teeth with CAL greater than or equal to")+" "+textRedCAL.Text+" mm: "
				+ConvertListToString(contrPerio.CountTeeth(PerioSequenceType.CAL))
				,font,Brushes.Black,xPos,yPos);
			yPos+=20;
			g.DrawString(Lan.g(this,"Teeth with Furcations greater than or equal to class")+" "+textRedFurc.Text+": "
				+ConvertListToString(contrPerio.CountTeeth(PerioSequenceType.Furcation))
				,font,Brushes.Black,xPos,yPos);
			yPos+=20;
			g.DrawString(Lan.g(this,"Teeth with Mobility greater than or equal to")+" "+textRedMob.Text+": "
				+ConvertListToString(contrPerio.CountTeeth(PerioSequenceType.Mobility))
				,font,Brushes.Black,xPos,yPos);
			//pagesPrinted++;
			printPageEventArgs.HasMorePages=false;
		}

		private void timerEClipCheck_Tick(object sender,EventArgs e) {
			CheckMobileActivity();
		}

		private void updownRed_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.Setup)) {
				return;
			}
			//this is necessary because Microsoft's updown control is too buggy to be useful
			Cursor=Cursors.WaitCursor;
			PrefName prefName=PrefName.PerioRedProb;
			if(sender==updownProb){
				prefName=PrefName.PerioRedProb;
			}
			else if(sender==updownMGJ) {
				prefName=PrefName.PerioRedMGJ;
			}
			else if(sender==updownGing) {
				prefName=PrefName.PerioRedGing;
			}
			else if(sender==updownCAL) {
				prefName=PrefName.PerioRedCAL;
			}
			else if(sender==updownFurc) {
				prefName=PrefName.PerioRedFurc;
			}
			else if(sender==updownMob) {
				prefName=PrefName.PerioRedMob;
			}
			int val=PrefC.GetInt(prefName);
			if(e.Y<8){//up
				val++;
			}
			else{//down
				if(val==0){
					Cursor=Cursors.Default;
					return;
				}
				val--;
			}
			Prefs.UpdateLong(prefName,val);
			//pref.ValueString=currentValue.ToString();
			//Prefs.Update(pref);
			_isLocalDefsChanged=true;
			Cache.Refresh(InvalidType.Prefs);
			if(sender==updownProb){
				textRedProb.Text=val.ToString();
				textCountProb.Text=contrPerio.CountTeeth(PerioSequenceType.Probing).Count.ToString();
			}
			else if(sender==updownMGJ){
				textRedMGJ.Text=val.ToString();
				textCountMGJ.Text=contrPerio.CountTeeth(PerioSequenceType.MGJ).Count.ToString();
			}
			else if(sender==updownGing){
				textRedGing.Text=val.ToString();
				textCountGing.Text=contrPerio.CountTeeth(PerioSequenceType.GingMargin).Count.ToString();
			}
			else if(sender==updownCAL){
				textRedCAL.Text=val.ToString();
				textCountCAL.Text=contrPerio.CountTeeth(PerioSequenceType.CAL).Count.ToString();
			}
			else if(sender==updownFurc){
				textRedFurc.Text=val.ToString();
				textCountFurc.Text=contrPerio.CountTeeth(PerioSequenceType.Furcation).Count.ToString();
			}
			else if(sender==updownMob){
				textRedMob.Text=val.ToString();
				textCountMob.Text=contrPerio.CountTeeth(PerioSequenceType.Mobility).Count.ToString();
			}
			contrPerio.Invalidate();
			Cursor=Cursors.Default;
			contrPerio.Focus();
		}
		#endregion Methods - Event Handlers private

		#region Methods - Events Handlers button clicks
		private void butAdd_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.PerioEdit,MiscData.GetNowDateTime())){
				return;
			}
			if(gridODExam.GetSelectedIndex()!=-1){
				contrPerio.SaveCurExam(_perioExam);
			}
			CreateNewPerioChart();
			RefreshListExams();
			gridODExam.SetSelected(gridODExam.ListGridRows.Count-1,true);
			FillGrid();
			SecurityLogs.MakeLogEntry(EnumPermType.PerioEdit,_patient.PatNum,"Perio exam created");
		}

		private void butBleed_Click(object sender, System.EventArgs e) {
			_isTenDown=false;
			contrPerio.ButtonPressed("b");
			contrPerio.Focus();
		}

		private void butCalcIndex_Click(object sender, System.EventArgs e) {
			FillIndexes();
		}

		private void butCalculus_Click(object sender, System.EventArgs e) {
			_isTenDown=false;
			contrPerio.ButtonPressed("c");
			contrPerio.Focus();
		}

		private void butColorBleed_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.DefEdit)) {
				return;
			}
			colorDialog1.Color=butColorBleed.BackColor;
			if(colorDialog1.ShowDialog()!=DialogResult.OK){
				colorDialog1?.Dispose();
				return;
			}
			butColorBleed.BackColor=colorDialog1.Color;
			Def defMiscColors=_listDefsMiscColors[(int)DefCatMiscColors.PerioBleeding].Copy();
			defMiscColors.ItemColor=colorDialog1.Color;
			DefL.Update(defMiscColors);
			Cache.Refresh(InvalidType.Defs);
			_isLocalDefsChanged=true;
			contrPerio.SetColors();
			contrPerio.Invalidate();
			contrPerio.Focus();
			colorDialog1?.Dispose();
		}

		private void butColorCalculus_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.DefEdit)) {
				return;
			}
			colorDialog1.Color=butColorCalculus.BackColor;
			if(colorDialog1.ShowDialog()!=DialogResult.OK){
				colorDialog1?.Dispose();
				return;
			}
			butColorCalculus.BackColor=colorDialog1.Color;
			Def defMiscColors=_listDefsMiscColors[(int)DefCatMiscColors.PerioCalculus].Copy();
			defMiscColors.ItemColor=colorDialog1.Color;
			DefL.Update(defMiscColors);
			Cache.Refresh(InvalidType.Defs);
			_isLocalDefsChanged=true;
			contrPerio.SetColors();
			contrPerio.Invalidate();
			contrPerio.Focus();
			colorDialog1?.Dispose();
		}

		private void butColorPlaque_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.DefEdit)) {
				return;
			}
			colorDialog1.Color=butColorPlaque.BackColor;
			if(colorDialog1.ShowDialog()!=DialogResult.OK){
				colorDialog1?.Dispose();
				return;
			}
			butColorPlaque.BackColor=colorDialog1.Color;
			Def defMiscColors=_listDefsMiscColors[(int)DefCatMiscColors.PerioPlaque].Copy();
			defMiscColors.ItemColor=colorDialog1.Color;
			DefL.Update(defMiscColors);
			Cache.Refresh(InvalidType.Defs);
			_isLocalDefsChanged=true;
			contrPerio.SetColors();
			contrPerio.Invalidate();
			contrPerio.Focus();
			colorDialog1?.Dispose();
		}

		private void butColorPus_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.DefEdit)) {
				return;
			}
			colorDialog1.Color=butColorPus.BackColor;
			if(colorDialog1.ShowDialog()!=DialogResult.OK){
				colorDialog1?.Dispose();
				return;
			}
			butColorPus.BackColor=colorDialog1.Color;
			Def defMiscColors=_listDefsMiscColors[(int)DefCatMiscColors.PerioSuppuration].Copy();
			defMiscColors.ItemColor=colorDialog1.Color;
			DefL.Update(defMiscColors);
			Cache.Refresh(InvalidType.Defs);
			_isLocalDefsChanged=true;
			contrPerio.SetColors();
			contrPerio.Invalidate();
			contrPerio.Focus();
			colorDialog1?.Dispose();
		}

		private void butCopyNote_Click(object sender,EventArgs e) {
			if(gridODExam.GetSelectedIndex()!=-1) {
				ODClipboard.SetClipboard(textExamNotes.Text);
			}
		}

		private void butCopyPrevious_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.PerioEdit,MiscData.GetNowDateTime())) {
				return;
			}
			if(contrPerio.ListPerioExams.Count==0){
				MsgBox.Show(this,"Please use the Add button to create an initial exam.");
				return;
			}
			if(gridODExam.GetSelectedIndex()==-1) {
				//with the current code, this will never actually get hit because we don't allow deselecting.
				MsgBox.Show(this,"Please select an existing exam first.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"A copy will be made of the previous exam.  Continue?")){
				return;
			}
			contrPerio.SaveCurExam(_perioExam);
			CreateNewPerioChart();
			//get meaures from last exam
			List<PerioMeasure> listPerioMeasures=PerioMeasures.GetAllForExam(contrPerio.ListPerioExams[contrPerio.ListPerioExams.Count-1].PerioExamNum);
			for(int i=0;i<listPerioMeasures.Count;i++) { //add all of the previous exam's measures to this perio exam.
				listPerioMeasures[i].PerioExamNum=_perioExam.PerioExamNum;
				PerioMeasures.Insert(listPerioMeasures[i]);
			}
			RefreshListExams();
			gridODExam.SetSelected(contrPerio.ListPerioExams.Count-1,true); //select the exam that was just inserted.
			FillGrid();
			SecurityLogs.MakeLogEntry(EnumPermType.PerioEdit,_patient.PatNum,"Perio exam copied.");
		}

		private void butCount_Click(object sender, System.EventArgs e) {
			FillCounts();
			contrPerio.Focus();
		}

		private void butDefault_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.PerioEdit,MiscData.GetNowDateTime())){
				return;
			}
			string prefProbeDepths=PrefC.GetString(PrefName.PerioDefaultProbeDepths);
			if(prefProbeDepths.Length!=192) {
				MsgBox.Show(this,"You have not set up a default exam. Please click Setup to set up a default exam.");
				return;
			}
			if(gridODExam.GetSelectedIndex()!=-1){
				contrPerio.SaveCurExam(_perioExam);
			}
			CreateNewPerioChart();
			SetDefault(prefProbeDepths);
			RefreshListExams();
			gridODExam.SetSelected(gridODExam.ListGridRows.Count-1,true);
			FillGrid();
			FormPerio_ChangeTitle();
			SecurityLogs.MakeLogEntry(EnumPermType.PerioEdit,_patient.PatNum,"Default perio exam created");
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(gridODExam.GetSelectedIndex()==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			if(!Security.IsAuthorized(EnumPermType.PerioEdit,contrPerio.ListPerioExams[gridODExam.GetSelectedIndex()].ExamDate)) {
				return;
			}
			if(MessageBox.Show(Lan.g(this,"Delete Exam?"),"",MessageBoxButtons.OKCancel)!=DialogResult.OK){
				return;
			}
			int selectedIndex=gridODExam.GetSelectedIndex();
			PerioExams.Delete(_perioExam);
			RefreshListExams();
			if(selectedIndex < gridODExam.ListGridRows.Count) {
				gridODExam.SetSelected(selectedIndex,true);
			}
			else {
				gridODExam.SetSelected(contrPerio.ListPerioExams.Count-1,true);
			}	
			FillGrid();
			if(gridODExam.ListGridRows.Count==0) {
				textExamNotes.Text="";
			}
			SecurityLogs.MakeLogEntry(EnumPermType.PerioEdit,_patient.PatNum,"Perio exam deleted");
		}

		//		private void butEditNotes_Click(object sender,EventArgs e) {
//			int examIndex=gridODExam.GetSelectedIndex();
//			if(examIndex==-1) {
//				return;
//			}
//			using InputBox inputBox = new InputBox("Edit Notes",true,PerioExamCur.Note,new Size(351,215));
//			if(inputBox.ShowDialog()!=DialogResult.OK){
//				return;
//			}
//			if(PerioExamCur.Note!=inputBox.textResult.Text) {
//				PerioExamCur.Note=inputBox.textResult.Text;
//				PerioExams.Update(PerioExamCur);
//			}
//			gridP.SaveCurExam(PerioExamCur);
//			FillGrid();
//			RefreshListExams();
//			gridODExam.SetSelected(examIndex,true);
//		}

		private void butGraphical_Click(object sender,EventArgs e) {
			if(!ToothChartRelay.IsSparks3DPresent){
				if(ComputerPrefs.LocalComputer.GraphicsSimple!=DrawingMode.DirectX) {
					MsgBox.Show(this,"In the Graphics setup window, you must first select DirectX.");
					return;
				}
			}
			if(gridODExam.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Exam must be selected first.");
				return;
			}
			if(_isLocalDefsChanged) {
				DataValid.SetInvalid(InvalidType.Defs,InvalidType.Prefs);
			}
			//if(listExams.SelectedIndex!=-1) {
			contrPerio.SaveCurExam(_perioExam);
			contrPerio.ListPerioExams=PerioExams.Refresh(_patient.PatNum);//refresh instead
			contrPerio.ListPerioMeasures=PerioMeasures.GetForPatient(_patient.PatNum);
			FillGrid();
			using FormPerioGraphical formPerioGraphical=new FormPerioGraphical(contrPerio.ListPerioExams[gridODExam.GetSelectedIndex()],_patient);
			formPerioGraphical.ShowDialog();
		}

		private void butListen_Click(object sender,EventArgs e) {
			if(ODBuild.IsThinfinity()) {
				MsgBox.Show(this,"Voice Perio is not available for the Web version at this time.");
				return;
			}
			if(_voiceController!=null && _voiceController.IsListening) {
				_voiceController.StopListening();
				labelListening.Visible=false;
				return;
			}
			if(_voiceController==null) {
				try{
					_voiceController=new VoiceController(VoiceCommandArea.PerioChart);
				}
				catch(PlatformNotSupportedException) {
					MsgBox.Show(this,"The Voice Command feature does not work over server RDP sessions.");
					return;
				}
				catch(Exception ex) {
					FriendlyException.Show(Lan.g(this,"Unable to initialize audio input. Try plugging a different microphone into the computer."),ex);
					return;
				}
				_voiceController.SpeechRecognized+=VoiceController_SpeechRecognized;
			}
			_voiceController.StartListening();
			labelListening.Visible=true;
		}

		private void butPrint_Click(object sender, System.EventArgs e) {
			if(this.gridODExam.GetSelectedIndex()<0) {
				MsgBox.Show(this,"Please select an exam first.");
				return;
			}
			//Store Font and Size information for the control so that we can adjust temporarily for printing.
			Font fontOriginal=contrPerio.Font;
			Size sizeContrOriginal=contrPerio.Size;
			contrPerio.LayoutManager.Is96dpi=true;//Tells the layoutManager not to scale the size and font again when the ContrPerio.OnResize event is called.
			contrPerio.Font=new Font("Microsoft Sans Serif", 8.25f);
			LayoutManager.MoveSize(contrPerio,new Size(602, 642));//Revert to default drawn grid size for printing, be sure to adjust this if the default size is changed on ContrPerio.cs, this will then trigger the ContrPerio.OnResize Event adjusting all of the formatting necessary for printing.
			PrinterL.TryPrint(pd2_PrintPage,
				Lan.g(this,"Perio chart from")+" "+_perioExam.ExamDate+" "+Lan.g(this,"printed"),
				_patient.PatNum,
				PrintSituation.TPPerio,
				new Margins(0,0,0,0),
				PrintoutOrigin.AtMargin
			);
			contrPerio.LayoutManager.Is96dpi=false;
			contrPerio.Font=fontOriginal;
			LayoutManager.MoveSize(contrPerio,sizeContrOriginal);
			contrPerio.Focus();
		}

		private void butPlaque_Click(object sender, System.EventArgs e) {
			_isTenDown=false;
			contrPerio.ButtonPressed("p");
			contrPerio.Focus();
		}

		private void butPus_Click(object sender, System.EventArgs e) {
			_isTenDown=false;
			contrPerio.ButtonPressed("s");
			contrPerio.Focus();
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(this.gridODExam.GetSelectedIndex()<0){
				MessageBox.Show(Lan.g(this,"Please select an exam first."));
				return;
			}
			contrPerio.SaveCurExam(_perioExam);
			contrPerio.ListPerioExams=PerioExams.Refresh(_patient.PatNum);
			contrPerio.ListPerioMeasures=PerioMeasures.GetForPatient(_patient.PatNum);
			FillGrid(!_isExamInUse);
			//Document doc=new Document();
			//try {
			using Bitmap bitmapPerioPrintImage=new Bitmap(LayoutManager.Scale(750),LayoutManager.Scale(1000));
			bitmapPerioPrintImage.SetResolution(96,96);
			using Graphics g=Graphics.FromImage(bitmapPerioPrintImage);
			g.Clear(Color.White);
			g.CompositingQuality=System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			g.InterpolationMode=System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			g.SmoothingMode=System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			g.TextRenderingHint=System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			string clinicName="";
			//This clinic name could be more accurate here in the future if we make perio exams clinic specific.
			//Perhaps if there were a perioexam.ClinicNum column.
			if(_patient.ClinicNum!=0) {
				Clinic clinic=Clinics.GetClinic(_patient.ClinicNum);
				clinicName=clinic.Description;
			} 
			else {
				clinicName=PrefC.GetString(PrefName.PracticeTitle);
			}
			float y=50f;
			SizeF sizeF;
			//Title
			using Font fontTitle=new Font("Arial",LayoutManager.Scale(15));
			string title=Lan.g(this,"Periodontal Charting");
			sizeF=g.MeasureString(title,fontTitle);
			g.DrawString(title,fontTitle,Brushes.Black,new PointF(bitmapPerioPrintImage.Width/2f-sizeF.Width/2f,y));
			y+=sizeF.Height;
			//Clinic Name
			using Font font=new Font("Arial",LayoutManager.Scale(11));
			sizeF=g.MeasureString(clinicName,font);
			g.DrawString(clinicName,font,Brushes.Black,
				new PointF(bitmapPerioPrintImage.Width/2f-sizeF.Width/2f,y));
			y+=sizeF.Height;
			//Patient name
			string patName=_patient.GetNameFLFormal();
			sizeF=g.MeasureString(patName,font);
			g.DrawString(patName,font,Brushes.Black,new PointF(bitmapPerioPrintImage.Width/2f-sizeF.Width/2f,y));
			y+=sizeF.Height;
			//Date
			//We put the current datetime here because the specific exam dates are listed in the form.
			string time=MiscData.GetNowDateTime().ToShortDateString();
			sizeF=g.MeasureString(time,font);
			g.DrawString(time,font,Brushes.Black,new PointF(bitmapPerioPrintImage.Width/2f-sizeF.Width/2f,y));
			y+=sizeF.Height;
			//Now draw the grid
			Rectangle rectangle=contrPerio.GetRectangleBoundsShowing();
			using Bitmap bitmapGrid=new Bitmap(rectangle.Width,rectangle.Height);
			contrPerio.DrawToBitmap(bitmapGrid,rectangle);
			g.DrawImageUnscaled(bitmapGrid,(int)((bitmapPerioPrintImage.Width-bitmapGrid.Width)/2f),(int)y);
			long defNumCategory=Defs.GetImageCat(ImageCategorySpecial.T);
			if(defNumCategory==0) {
				MsgBox.Show(this,"No image category set for tooth charts in definitions.");
				return;
			}
			try {
				ImageStore.Import(bitmapPerioPrintImage,defNumCategory,ImageType.Photo,_patient);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Unable to save file. ") + ex.Message);
				return;
			}
			MsgBox.Show(this,"Saved.");
			/*
			string patImagePath=ImageStore.GetPatientFolder(PatCur);
			string filePath="";
			do {
				doc.DateCreated=MiscData.GetNowDateTime();
				doc.FileName="perioexam_"+doc.DateCreated.ToString("yyyy_MM_dd_hh_mm_ss")+".png";
				filePath=ODFileUtils.CombinePaths(patImagePath,doc.FileName);
			} while(File.Exists(filePath));//if a file with this name already exists, then it will stay in this loop
			doc.PatNum=PatCur.PatNum;
			doc.ImgType=ImageType.Photo;
			doc.DocCategory=Defs.GetByExactName(DefCat.ImageCats,"Tooth Charts");
			doc.Description="Perio Exam";
			Documents.Insert(doc,PatCur);
			docCreated=true;
			bitmapPerioPrintImage.Save(filePath,System.Drawing.Imaging.ImageFormat.Png);
			MessageBox.Show(Lan.g(this,"Image saved."));*/
			/*} 
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Image failed to save: "+Environment.NewLine+ex.ToString()));
				if(docCreated) {
					Documents.Delete(doc);
				}
			}
			*/
		}

		private void butSkip_Click(object sender, System.EventArgs e) {
			if(gridODExam.GetSelectedIndex()<0){//PerioExamCur could still be set to a deleted exam and would not be null even if there is no exam.
				MessageBox.Show(Lan.g(this,"Please select an exam first."));
				return;
			}
			contrPerio.ToggleSkip(_perioExam.PerioExamNum);
		}

		private void butUnlockEClip_Click(object sender,EventArgs e) {
			bool isExamInUse=MobileAppDevices.IsInUse(_patient.PatNum,MADPage.PerioExamEditPage,MADPage.PerioExamListPage);
			bool isAllowingEdit=false;
			if(isExamInUse) {
				isAllowingEdit=MsgBox.Show(MsgBoxButtons.YesNo,Lans.g(this, "This patient has a perio exam currently being edited in ODTouch. Would you like to edit anyway? (Not recommended)."));
			}
			else {
				SetEClipBoardEditing(false);
				return;
			}
			if(isAllowingEdit) {
				MobileAppDevice mobileAppDevice=MobileAppDevices.GetForPat(_patient.PatNum);
				if(mobileAppDevice==null) {
					return;
				}
				MobileNotifications.CI_GoToCheckin(mobileAppDevice.MobileAppDeviceNum);
				mobileAppDevice.DevicePage=MADPage.CheckinPage;
				MobileAppDevices.Update(mobileAppDevice);
				SetEClipBoardEditing(false);
			}
			return;
		}

		private void but0_Click(object sender, System.EventArgs e) {
			NumberClicked(0);
		}

		private void but1_Click(object sender, System.EventArgs e) {
			NumberClicked(1);
		}

		private void but2_Click(object sender, System.EventArgs e) {
			NumberClicked(2);
		}

		private void but3_Click(object sender, System.EventArgs e) {
			NumberClicked(3);
		}

		private void but4_Click(object sender, System.EventArgs e) {
			NumberClicked(4);
		}

		private void but5_Click(object sender, System.EventArgs e) {
			NumberClicked(5);
		}

		private void but6_Click(object sender, System.EventArgs e) {
			NumberClicked(6);
		}

		private void but7_Click(object sender, System.EventArgs e) {
			NumberClicked(7);
		}

		private void but8_Click(object sender, System.EventArgs e) {
			NumberClicked(8);
		}

		private void but9_Click(object sender, System.EventArgs e) {
			NumberClicked(9);
		}

		private void but10_Click(object sender, System.EventArgs e) {
			_isTenDown=true;
		}

		///<summary>When BolaAi is not enabled, this button shows under the listen button. When clicked it launches their website.</summary>
		private void butBolaGet_Click(object sender, EventArgs e) {
			try {
				ODFileUtils.ProcessStart("https://bola.ai/");
			}
			catch(Exception ex) {
				BugSubmissions.SubmitException(ex);
				MsgBox.Show(this,"There was an error launching the web browser.");
			}
		}

		///<summary>When BolaAi is enabled, this button shows instead of the standard listen button. When clicked it launches their exe.</summary>
		private void butBolaLaunch_Click(object sender, EventArgs e) {
			string path=Programs.GetProgramPath(ProgramName.BolaAI);
			string pathExpanded=Environment.ExpandEnvironmentVariables(path);//This will handle %USERPROFILE% in the path
			if(File.Exists(pathExpanded)) {
				try {
					ODFileUtils.ProcessStart(pathExpanded);
				}
				catch(Exception ex) {
					MsgBox.Show(this, "There was an error launching Bola AI.");
				}
			}
			else {
				MsgBox.Show(this, "Invalid path for Bola executable.");
			}
		}
		#endregion Methods - Events Handlers button clicks

		#region Methods - Event Handlers checkboxes
		private void checkGingMarg_CheckedChanged(object sender,EventArgs e) {
			contrPerio.GingMargPlus=checkGingMarg.Checked;
			contrPerio.Focus();
		}

		private void checkShowCurrent_Click(object sender,EventArgs e) {
			if(gridODExam.GetSelectedIndex()==-1) {
				return;
			}
			contrPerio.SaveCurExam(_perioExam);
			contrPerio.ListPerioExams=PerioExams.Refresh(_patient.PatNum);
			contrPerio.ListPerioMeasures=PerioMeasures.GetForPatient(_patient.PatNum);
			FillGrid();
		}

		private void checkThree_Click(object sender, System.EventArgs e) {
			contrPerio.ThreeAtATime=checkThree.Checked;
		}
		#endregion Methods - Event Handlers checkboxes

		#region Methods - Event Handlers grids
		private void gridP_Click(object sender,EventArgs e) {
			textInputBox.Focus();
			SetLabelFwdReverse();
			FormPerio_ChangeTitle();
		}

		private void gridP_DirectionChangedLeft(object sender, System.EventArgs e) {
			radioLeft.Checked=true;
		}

		private void gridP_DirectionChangedRight(object sender, System.EventArgs e) {
			radioRight.Checked=true;
		}

		private void gridODExam_CellClick(object sender,UI.ODGridClickEventArgs e) {
			if(gridODExam.GetSelectedIndex()==contrPerio.IdxExamSelected) { 
				return;
			}
			//Only continues if clicked on other than current exam
			contrPerio.SaveCurExam(_perioExam);
			//no need to RefreshListExams because it has not changed
			contrPerio.ListPerioExams=PerioExams.Refresh(_patient.PatNum);//refresh instead
			contrPerio.ListPerioMeasures=PerioMeasures.GetForPatient(_patient.PatNum);
			FillGrid();
		}

		private void gridODExam_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			//remember that the first click may not have triggered the mouse down routine
			//and the second click will never trigger it.
			if(gridODExam.GetSelectedIndex()==-1) {
				return;
			}
			if(!Security.IsAuthorized(EnumPermType.PerioEdit,contrPerio.ListPerioExams[gridODExam.GetSelectedIndex()].ExamDate)) {
				return;
			}
			//a PerioExam.Cur will always have been set through mousedown(or similar),then FillGrid
			contrPerio.SaveCurExam(_perioExam);
			contrPerio.ListPerioExams=PerioExams.Refresh(_patient.PatNum);//list will not change
			contrPerio.ListPerioMeasures=PerioMeasures.GetForPatient(_patient.PatNum);
			using FormPerioEdit formPerioEdit=new FormPerioEdit();
			formPerioEdit.PerioExamCur=_perioExam;
			formPerioEdit.ShowDialog();
			int index=gridODExam.GetSelectedIndex();
			RefreshListExams();
			gridODExam.SetSelected(index,true);
			FillGrid();
		}
		#endregion Methods - Event Handlers grids

		#region Methods - Event Handlers radio buttons
		private void radioRight_Click(object sender, System.EventArgs e) {
			contrPerio.EnumCurrentDirection_=EnumCurrentDirection.Right;
			SetLabelFwdReverse();
			contrPerio.Focus();
		}

		private void radioLeft_Click(object sender, System.EventArgs e) {
			contrPerio.EnumCurrentDirection_=EnumCurrentDirection.Left;
			SetLabelFwdReverse();
			contrPerio.Focus();
		}

		private void radioFacialsFirst_Click(object sender,EventArgs e) {
			contrPerio.EnumAdvanceSequence_=EnumAdvanceSequence.FacialsFirst;
			SetLabelFwdReverse();
			contrPerio.Focus();
		}

		private void radioMaxFirst_Click(object sender,EventArgs e) {
			contrPerio.EnumAdvanceSequence_=EnumAdvanceSequence.MaxFirst;
			SetLabelFwdReverse();
			contrPerio.Focus();
		}
		#endregion Methods - Event Handlers radio buttons

		#region Methods - Event Handlers text
		private void textExamNotes_Leave(object sender,EventArgs e) {
			if(_perioExam!=null) { 
				PerioExam perioExamOld=_perioExam.Copy();
				_perioExam.Note=textExamNotes.Text;
				if(PerioExams.Update(_perioExam,perioExamOld)) {
					int selectionOld=gridODExam.GetSelectedIndex();
					RefreshListExams(skipRefreshMeasures:true);
					gridODExam.SetSelected(selectionOld);
				}
			}
			textInputBox.Focus();
		}

		///<summary>Catches any non-alphanumeric keypresses so that they can be passed into the grid separately.</summary>
		private void textInputBox_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.Left
				|| e.KeyCode==Keys.Right
				|| e.KeyCode==Keys.Up
				|| e.KeyCode==Keys.Down
				|| e.KeyCode==Keys.Delete
				|| e.KeyCode==Keys.Back
				|| e.Modifiers==Keys.Control) 
			{
				contrPerio.KeyPressed(e);
				FormPerio_ChangeTitle();
			}
		}

		///<summary>This ensures that the textbox will always have focus when using FormPerio.</summary>
		private void textInputBox_Leave(object sender,EventArgs e) {
			if(!textExamNotes.Focused) {
				textInputBox.Focus();
			}
		}

		///<summary>Used to force buttons to be pressed as characters are typed/dictated/pasted into the textbox.</summary>
		private void textInputBox_TextChanged(object sender,EventArgs e) {
			Char[] charArrayInputs=textInputBox.Text.ToLower().ToCharArray();
			for(int i=0;i<charArrayInputs.Length;i++) {
				if(charArrayInputs[i]>=48 && charArrayInputs[i]<=57) {
					NumberClicked(charArrayInputs[i]-48);
				}
				else if(charArrayInputs[i]=='b'
					|| charArrayInputs[i]=='c'
					|| charArrayInputs[i]=='s'
					|| charArrayInputs[i]=='p'
					|| charArrayInputs[i]=='j'
					|| charArrayInputs[i]=='g'
					|| charArrayInputs[i]=='f'
					|| charArrayInputs[i]=='m'
					|| charArrayInputs[i]=='.') 
				{
					contrPerio.ButtonPressed(charArrayInputs[i].ToString());
				}
				else if(charArrayInputs[i]==' ') {
					contrPerio.ButtonPressed("b");
				}
			}
			FormPerio_ChangeTitle();
			textInputBox.Clear();
		}
		#endregion Methods - Event Handlers text

		#region Methods - Voice Perio
		///<summary>Used for Voice Perio. Returns the next available ColRow for tooth if the current tooth passed in is empty. Checks the tooth's 3 positions to find out if the tooth is empty. Otherwise returns a ColRow with value (-1,-1) to indicate 'none'. This method should only be used if Auto Advance Sequence is Facials First.</summary>
		private ColRow GetNextAvailableColRowForTooth(PerioAdvancePos perioAdvancePos,PerioAdvancePos perioAdvancePosNext,PerioAdvancePos perioAdvancePosPrev) {
			ColRow colRowNextAvailable;
			if(perioAdvancePos==null) {//This shouldn't happen.
				return new ColRow(-1,-1);
			}
			//Get the default colrow for the current tooth.
			ColRow colRowForTooth=GetColRowForTooth(perioAdvancePos.ToothNum,perioAdvancePos.IsFacial);
			PerioSequenceType perioSequenceType=contrPerio.GetSequenceForColRow(colRowForTooth);
			//Get the first position(mesial or distal) for the tooth passed in. We need this because the first position for a Facials First path will not be the same as the default.
			colRowForTooth=contrPerio.GetAutoAdvanceColRow(perioAdvancePos.ToothNum,perioAdvancePos.IsFacial,perioSequenceType,isReverse:false);
			//Check all three positions for the tooth. It will return either an empty position or the last position of the tooth.
			colRowNextAvailable=new ColRow(FirstEmptyPositionX(colRowForTooth),colRowForTooth.Row);
			//Check the position that was returned above for measurements. This will be the last position of the current tooth.
			if(string.IsNullOrEmpty(contrPerio.GetCellText(colRowNextAvailable.Col,colRowNextAvailable.Row))) {
				return colRowNextAvailable;//we found a colrow
			}
			return new ColRow(-1,-1);//couldn't find colrow
		}

		///<summary>Moves the cursor to the furcation row for the current tooth.</summary>
		private void GoToFurcation() {
			GoToSequenceType(PerioSequenceType.Furcation);
		}

		///<summary>Moves the cursor to the gingival margin row for the current tooth.</summary>
		private void GoToGingivalMargin() {
			GoToSequenceType(PerioSequenceType.GingMargin);
		}

		///<summary>Moves the cursor to the muco gingival junction row for the current tooth.</summary>
		private void GoToMGJ() {
			GoToSequenceType(PerioSequenceType.MGJ);
		}

		///<summary>Moves the cursor to the mobility row for the current tooth.</summary>
		private void GoToMobility() {
			PerioCell perioCell=contrPerio.GetCurrentCell();
			int toothNum=perioCell.ToothNum;
			bool isFacial=_perioCellCurLocation.IsFacial;
			if(contrPerio.IsCellTextEmpty(GetColRowForTooth(toothNum,isFacial))) {
				toothNum=GetToothNumFromColRow(GetPreviousToothColRow(toothNum,isFacial));
			}
			ColRow colRowForMobility=GetColRowForMobility(toothNum);
			if(!contrPerio.IsCellTextEmpty(colRowForMobility)) {
				colRowForMobility=GetColRowForMobility(perioCell.ToothNum);
			}
			contrPerio.SetNewCell(colRowForMobility.Col,colRowForMobility.Row);
			FormPerio_ChangeTitle();
		}

		///<summary>Moves the cursor to the probing row for the current tooth.</summary>
		private void GoToProbing() {
			int yVal;
			if(_perioCellCurLocation.IsFacial) {
				if(_perioCellCurLocation.ToothNum<=16) {
					if(checkShowCurrent.Checked) {
						yVal=5;
					}
					else {
						yVal=6-Math.Min(6,contrPerio.IdxExamSelected+1);
					}
				}
				else {//ToothNum >= 17
					if(checkShowCurrent.Checked) {
						yVal=37;
					}
					else {
						yVal=36+Math.Min(6,contrPerio.IdxExamSelected+1);
					}
				}
			}
			else {//Lingual
				if(_perioCellCurLocation.ToothNum<=16) {
					if(checkShowCurrent.Checked) {
						yVal=15;
					}
					else {
						yVal=14+Math.Min(6,contrPerio.IdxExamSelected+1);
					}
				}
				else {//ToothNum >= 17
					if(checkShowCurrent.Checked) {
						yVal=26;
					}
					else {
						yVal=27-Math.Min(6,contrPerio.IdxExamSelected+1);
					}
				}
			}
			contrPerio.SetNewCell(FirstEmptyPositionX(_perioCellCurLocation.ToothNum,yVal),yVal);
			FormPerio_ChangeTitle();
		}

		///<summary>Moves the cursor to the passed in SequenceType for the current tooth.</summary>
		private void GoToSequenceType(PerioSequenceType perioSequenceType) {
			int toothNum=_perioCellCurLocation.ToothNum;
			EnumCurrentDirection enumCurrentDirectionOld=contrPerio.EnumCurrentDirection_;
			bool isFacial=_perioCellCurLocation.IsFacial;
			ColRow colRowForTooth=GetColRowForTooth(toothNum,isFacial);
			if(contrPerio.IsCellTextEmpty(colRowForTooth)) {
				colRowForTooth=GetPreviousToothColRow(toothNum,isFacial);
				isFacial=IsFacialFromColRow(colRowForTooth);
				toothNum=GetToothNumFromColRow(colRowForTooth);
			}
			int yPos=-1;
			switch(perioSequenceType) {
				case PerioSequenceType.Furcation:
					if(toothNum<=16) {
						yPos=(isFacial ? 9 : 12);
					}
					else {//ToothNum >= 17
						yPos=(isFacial ? 33 : 30);
					}
					break;
				case PerioSequenceType.GingMargin:
					if(toothNum<=16) {
						yPos=(isFacial ? 7 : 14);
					}
					else {//ToothNum >= 17
						yPos=(isFacial ? 35 : 28);
					}
					break;
				case PerioSequenceType.MGJ:
					if(toothNum<=16) {
						yPos=6;
					}
					else {//ToothNum >= 17
						yPos=(isFacial ? 36 : 27);
					}
					break;
			}
			int xPos;
			//create a new colrow with the updated Y pos for the passed in sequence type. 
			ColRow colRowForSequenceType=new ColRow(colRowForTooth.Col,yPos);
			//Teeth 1-32 F isReverse=false, Teeth 1-32 L isReverse=true;
			//Find the next available colrow for the sequence type.
			ColRow colRowNextAvailable=TryGetNextEmptyColRow(colRowForSequenceType,!isFacial);
			if(toothNum==GetToothNumFromColRow(colRowNextAvailable)){
				//The new colrow for the sequence was changed. Set the new xPos.
				xPos=FirstEmptyPositionX(toothNum,yPos);				
			}
			else{
				//No positions are empty or the next available colrow is on a new tooth,. Use the xPos from the toothOld.
				xPos=GetColRowForTooth(_perioCellCurLocation.ToothNum,_perioCellCurLocation.IsFacial).Col;
				contrPerio.EnumCurrentDirection_=enumCurrentDirectionOld;//change this value back just in case we changed it above.
			}
			//Set the cursor with the new colrows for the sequence.
			contrPerio.SetNewCell(xPos,yPos);
			FormPerio_ChangeTitle();
		}

		///<summary>Used for Voice Perio. Goes to the first cell in the grid for this tooth.</summary>
		///<param name="isFacial">When false, equivalent to lingual.</param>
		private void GoToTooth(int toothNum,bool isFacial) {
			ColRow colRowForTooth=GetColRowForTooth(toothNum,isFacial);
			contrPerio.SetNewCell(colRowForTooth.Col,colRowForTooth.Row);
			FormPerio_ChangeTitle();
		}

		/// <summary>Used for Voice Perio. Goes to the cell in the grid for the specified tooth and surface.</summary>
		private void GoToToothSurface(int toothNum,bool isFacial,EnumPerioMMidD perioSurface) {
			ColRow colRowForTooth=GetSurfaceColRowForTooth(toothNum,isFacial,perioSurface);
			contrPerio.SetNewCell(colRowForTooth.Col,colRowForTooth.Row);
			FormPerio_ChangeTitle();
		}
		
		private bool IsFacialFromColRow(ColRow colRow) {
			return contrPerio.GetPerioCellFromColRow(colRow).IsFacial;
		}

		///<summary>Used for Voice Perio. Sets the cursor to the first available open perio slot. The path used in this method follows the autoadvance i.e. 1-16 F,16-1 L,32-17 F, and 17-32 L</summary>
		private void ResumePath() {
			List<int> listSkippedTeeth=new List<int>();//int 1-32
			if(contrPerio.ListPerioExams.Count > 0) {
				//set skipped teeth based on the last exam in the list: 
				listSkippedTeeth=PerioMeasures.GetSkipped(contrPerio.ListPerioExams[contrPerio.ListPerioExams.Count-1].PerioExamNum);
			}
			List<PerioAdvancePos> listPerioAdvancePoss=PerioAdvancePos.GetPath(contrPerio.EnumAdvanceSequence_);
			ColRow colRowNextAvailable;
			for(int i=0;i<listPerioAdvancePoss.Count;i++) {
				if(listPerioAdvancePoss[i]==null || listSkippedTeeth.Contains(listPerioAdvancePoss[i].ToothNum)) {
					continue;
				}
				if(radioFacialsFirst.Checked) {//Auto advance Facials First.
					colRowNextAvailable=GetNextAvailableColRowForTooth(listPerioAdvancePoss[i],ContrPerio.GetNext(listPerioAdvancePoss[i],listPerioAdvancePoss),
						ContrPerio.GetPrevious(listPerioAdvancePoss[i],listPerioAdvancePoss));
					if(colRowNextAvailable.Col==-1 || colRowNextAvailable.Row==-1) {
						continue;
					}
				}
				else {//Auto advance NOT Facials First.
					colRowNextAvailable=TryGetNextAvailableColRowForTooth(listPerioAdvancePoss[i].ToothNum,listPerioAdvancePoss[i].IsFacial);
					if(colRowNextAvailable.Col==-1 || colRowNextAvailable.Row==-1) {
						continue;
					}
					if(listPerioAdvancePoss[i].IsFacial) {
						contrPerio.EnumCurrentDirection_=EnumCurrentDirection.Left;
						radioLeft.Checked=true;
					}
					else {//lingual
						contrPerio.EnumCurrentDirection_=EnumCurrentDirection.Right;
						radioRight.Checked=true;
					}
				}
				contrPerio.SetNewCell(colRowNextAvailable.Col,colRowNextAvailable.Row);
				FormPerio_ChangeTitle();
				return;
			}
		}

		///<summary>Used for Voice Perio. Sets bleeding flag on the specified surface for the current tooth. If the cursor is on a blank space and the previous tooth has probing entries, it will put the bleeding flag on the previous tooth.</summary>
		private void SetBleedingFlagForSurface(EnumPerioMMidD perioSurface,BleedingFlags bleedingFlags,bool isFacial) {
			int toothNum=_perioCellCurLocation.ToothNum;
			ColRow colRowForTooth=GetColRowForTooth(toothNum,isFacial);
			if(contrPerio.IsCellTextEmpty(colRowForTooth)) {
				//Get the previous tooth. Skipped teeth will get considered. 
				colRowForTooth=GetPreviousToothColRow(toothNum,isFacial,doSetDirection:false);
				isFacial=IsFacialFromColRow(colRowForTooth);
				toothNum=GetToothNumFromColRow(colRowForTooth);
			}
			//Get the colrow for the surface passed in. 
			colRowForTooth=GetSurfaceColRowForTooth(toothNum,isFacial,perioSurface);
			contrPerio.SetBleedingFlagForColRow(colRowForTooth,bleedingFlags);
		}

		///<summary>Used for Voice Perio. Sets the appropriate Current Direction radio button.</summary>
		private void SetCurrentDirection() {
			if(contrPerio.EnumAdvanceSequence_==EnumAdvanceSequence.FacialsFirst) {
				contrPerio.EnumCurrentDirection_=EnumCurrentDirection.Left;
				radioLeft.Checked=true;
				if(_perioCellCurLocation.IsFacial && _perioCellCurLocation.ToothNum>=17) {
					contrPerio.EnumCurrentDirection_=EnumCurrentDirection.Right;
					radioRight.Checked=true;
				}
				else if(!_perioCellCurLocation.IsFacial && _perioCellCurLocation.ToothNum<=16) {
					contrPerio.EnumCurrentDirection_=EnumCurrentDirection.Right;
					radioRight.Checked=true;
				}
				return;
			}
			if(_perioCellPrevLocation.IsFacial && !_perioCellCurLocation.IsFacial) {
				contrPerio.EnumCurrentDirection_=EnumCurrentDirection.Right;
				radioRight.Checked=true;
			}
			if(!_perioCellPrevLocation.IsFacial && _perioCellCurLocation.IsFacial) {
				contrPerio.EnumCurrentDirection_=EnumCurrentDirection.Left;
				radioLeft.Checked=true;
			}
		}
		
		///<summary>Marks the tooth as skipped.</summary>
		///<param name="doVerifyByVoice">If true, will verbally ask the user if they want to skip the tooth.</param>
		///<returns>True if the user does choose to skip the tooth.</returns>
		private bool SkipTooth(int toothNum,bool doVerifyByVoice=true) {
			if(doVerifyByVoice) {
				_voiceController?.StopListening();
				if(!VoiceMsgBox.Show("Mark tooth "+toothNum+" as skipped?",MsgBoxButtons.YesNo)) {
					this.BeginInvoke(() => _voiceController?.StartListening());//Invoking because recognition events will run when the main thread is free.
					return false;
				}
				this.BeginInvoke(() => _voiceController?.StartListening());//Invoking because recognition events will run when the main thread is free.
			}
			ColRow colRowTooth=contrPerio.ColRowSelected;
			bool isRadioRightChecked=radioRight.Checked;
			contrPerio.SaveCurExam(_perioExam);
			int selectedExam=contrPerio.IdxExamSelected;
			List<int> listSkippedTeeth=new List<int>();//int 1-32
			if(contrPerio.ListPerioExams.Count > 0) {
				//set skipped teeth based on the last exam in the list: 
				listSkippedTeeth=PerioMeasures.GetSkipped(contrPerio.ListPerioExams[contrPerio.ListPerioExams.Count-1].PerioExamNum);
			}
			if(!listSkippedTeeth.Contains(toothNum)) {
				listSkippedTeeth.Add(toothNum);
			}
			PerioMeasures.SetSkipped(_perioExam.PerioExamNum,listSkippedTeeth);
			RefreshListExams();
			gridODExam.SetSelected(selectedExam,true);
			FillGrid();
			if(_perioCellCurLocation.ToothNum==toothNum) {//Go to the next tooth if the _perioCellCurLocation is on the tooth being skipped. 
				ColRow colRowNextTooth=GetNextToothColRow(toothNum,_perioCellCurLocation.IsFacial);
				contrPerio.SetNewCell(colRowNextTooth.Col,colRowNextTooth.Row);
			}
			else {
				contrPerio.SetNewCell(colRowTooth.Col,colRowTooth.Row);
			}
			FormPerio_ChangeTitle();
			contrPerio.EnumCurrentDirection_=EnumCurrentDirection.Left;
			if(isRadioRightChecked) {
				contrPerio.EnumCurrentDirection_=EnumCurrentDirection.Right;
			}
			return true;
		}
		#endregion Methods - Voice Perio

	}
}