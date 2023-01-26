using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
using OpenDental.UI;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormPerio : FormODBase {
		#region Fields
		private bool _isLocalDefsChanged;
		private bool _isTenDown;
		//private int pagesPrinted;
		///<summary>Gets a list of missing teeth as strings on load. Includes "1"-"32", and "A"-"Z".</summary>
		private List<string> _listMissingTeeth;
		private Patient _patient;
		///<summary>This is not a list of valid procedures.  The only values to be trusted in this list are the ToothNum and CodeNum.  Never used.</summary>
		private List<Procedure> _listProcedures;
		private PerioExam _perioExam;
		private UserOdPref _userOdPrefCurrentOnly;
		private UserOdPref _userOdPrefCustomAdvance;
		private VoiceController _voiceController;
		private PerioCell _perioCellPrevLocation;
		private PerioCell _perioCellCurLocation;
		private List<Def> _listDefsMiscColors;
		private bool _isExamInUse;
		#endregion Fields

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

		private void FormPerio_Load(object sender, System.EventArgs e) {
			_listDefsMiscColors=Defs.GetDefsForCategory(DefCat.MiscColors,true);
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
			//Procedure[] procList=Procedures.Refresh(PatCur.PatNum);
			List<ToothInitial> listToothInitials=ToothInitials.GetPatientData(_patient.PatNum);
			_listMissingTeeth=ToothInitials.GetMissingOrHiddenTeeth(listToothInitials);
			RefreshListExams();
			if(Programs.UsingOrion) {
				labelPlaqueHistory.Visible=true;
				listPlaqueHistory.Visible=true;
				RefreshListPlaque();
			}
			gridODExam.SetSelected(PerioExams.ListExams.Count-1,true);//this works even if no items.
			_userOdPrefCurrentOnly=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.PerioCurrentExamOnly).FirstOrDefault();
			if(_userOdPrefCurrentOnly != null && PIn.Bool(_userOdPrefCurrentOnly.ValueString)) {
				checkShowCurrent.Checked=true;
			}
			_userOdPrefCustomAdvance=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.PerioAutoAdvanceCustom).FirstOrDefault();
			if(_userOdPrefCustomAdvance!=null && PIn.Bool(_userOdPrefCustomAdvance.ValueString)) {
				radioFacialsFirst.Checked=true;
				contrPerio.Direction=AutoAdvanceDirection.Custom;
			}
			FillGrid();
			CheckMobileActivity();
			Plugins.HookAddCode(this,"FormPerio.Load_end");
		}

		private void LayoutMenu(){
			menuMain.BeginUpdate();
			//Setup----------------------------------------------------------------------------------------------------------
			menuMain.Add(new MenuItemOD("Setup",menuItemSetup_Click));
			menuMain.EndUpdate();
		}

		private void menuItemSetup_Click(Object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormPerioSetup formPerioSetup=new FormPerioSetup();
			formPerioSetup.ShowDialog();
		}

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
				FillGrid(true);
				//Since FillGrid can re-enable the GridPerio editing.
				SetEClipBoardEditing(_isExamInUse);
			}
		}

		private void timerEClipCheck_Tick(object sender,EventArgs e) {
			CheckMobileActivity();
		}

		///<summary>Goes to the first cell in the grid for this tooth.</summary>
		///<param name="isFacial">When false, equivalent to lingual.</param>
		private void GoToTooth(int toothNum,bool isFacial) {
			Point pointForTooth=GetPointForTooth(toothNum,isFacial);
			contrPerio.SetNewCell(pointForTooth.X,pointForTooth.Y);
		}

		private void GoToToothSurface(int toothNum,bool isFacial,PerioSurface perioSurface) {
			Point pointForTooth=GetSurfacePointForTooth(toothNum,isFacial,perioSurface);
			contrPerio.SetNewCell(pointForTooth.X,pointForTooth.Y);
		}

		///<summary>Sets the cursor to the first available open perio slot. The path used in this method follows the autoadvance i.e. 1-16 F,16-1 L,32-17 F, and 17-32 L</summary>
		private void ResumePath() {
			List<int> listSkippedTeeth=new List<int>();//int 1-32
			if(PerioExams.ListExams.Count > 0) {
				//set skipped teeth based on the last exam in the list: 
				listSkippedTeeth=PerioMeasures.GetSkipped(PerioExams.ListExams[PerioExams.ListExams.Count-1].PerioExamNum);
			}
			List<AutoAdvanceCustom> listAutoAdvanceCustoms=AutoAdvanceCustom.GetDefaultPath();
			if(radioFacialsFirst.Checked) {
				listAutoAdvanceCustoms=AutoAdvanceCustom.GetCustomPaths();
			}
			Point pointNextAvailable;
			for(int i=0;i<listAutoAdvanceCustoms.Count;i++) {
				if(listAutoAdvanceCustoms[i]==null || listSkippedTeeth.Contains(listAutoAdvanceCustoms[i].ToothNum)) {
					continue;
				}
				if(radioFacialsFirst.Checked) {//Auto advance custom.
					pointNextAvailable=GetNextAvailablePointForToothCustom(listAutoAdvanceCustoms[i],ContrPerio.GetNext(listAutoAdvanceCustoms[i],listAutoAdvanceCustoms),
						ContrPerio.GetPrevious(listAutoAdvanceCustoms[i],listAutoAdvanceCustoms));
					if(pointNextAvailable.X==-1 || pointNextAvailable.Y==-1) {
						continue;
					}
				}
				else {//Auto advance NOT custom.
					pointNextAvailable=TryGetNextAvailablePointForTooth(listAutoAdvanceCustoms[i].ToothNum,listAutoAdvanceCustoms[i].Surface==PerioSurface.Facial);
					if(pointNextAvailable.X==-1 || pointNextAvailable.Y==-1) {
						continue;
					}
					if(listAutoAdvanceCustoms[i].Surface==PerioSurface.Facial) {
						contrPerio.Direction=AutoAdvanceDirection.Left;
						radioLeft.Checked=true;
					}
					else {//lingual
						contrPerio.Direction=AutoAdvanceDirection.Right;
						radioRight.Checked=true;
					}
				}
				contrPerio.SetNewCell(pointNextAvailable.X,pointNextAvailable.Y);
				return;
			}
		}

		///<summary>Returns the next available Point for tooth custom if the current tooth passed in is empty. Checks the tooth's 3 positions to find out if the tooth is empty. Otherwise returns a Point with value (-1,-1) to indicate 'none'. This method should only used if Auto Advance Custom is True.</summary>
		private Point GetNextAvailablePointForToothCustom(AutoAdvanceCustom autoAdvanceCustom,AutoAdvanceCustom autoAdvanceCustomNext,AutoAdvanceCustom autoAdvanceCustomPrev) {
			Point pointNextAvailable;
			if(autoAdvanceCustom==null) {//This shouldn't happen.
				return new Point(-1,-1);
			}
			//Get the default point for the current tooth.
			Point pointForTooth=GetPointForTooth(autoAdvanceCustom.ToothNum,autoAdvanceCustom.Surface==PerioSurface.Facial);
			PerioSequenceType perioSequenceType=contrPerio.GetSequenceForPoint(pointForTooth);
			//Get the first position(mesial or distal) for the tooth passed in. We need this because the first position for a custom path will not be the 
			//same as the default(not custom).
			pointForTooth=contrPerio.GetAutoAdvanceCustomPoint(autoAdvanceCustom.ToothNum,autoAdvanceCustom.Surface,perioSequenceType,isReverse:false);
			//Check all three positions for the tooth. It will return either an empty position or the last position of the tooth.
			pointNextAvailable=new Point(FirstEmptyPositionX(pointForTooth),pointForTooth.Y);
			//Check the position that was returned above for measurements. This will be the last position of the current tooth.
			if(string.IsNullOrEmpty(contrPerio.GetCellText(pointNextAvailable.X,pointNextAvailable.Y))) {
				return pointNextAvailable;//we found a point
			}
			return new Point(-1,-1);//couldn't find point
		}

		///<summary>Sets bleeding flag on the specified surface for the current tooth. If the cursor is on a blank space and the previous tooth has 
		///probing entries, it will put the bleeding flag on the previous tooth.</summary>
		private void SetBleedingFlagForSurface(PerioSurface perioSurface,BleedingFlags bleedingFlags) {
			int toothNum=_perioCellCurLocation.ToothNum;
			bool isFacial=_perioCellCurLocation.Surface==PerioSurface.Facial;
			Point pointForTooth=GetPointForTooth(toothNum,isFacial);
			if(contrPerio.IsCellTextEmpty(pointForTooth)) {
				//Get the previous tooth. Skipped teeth will get considered. 
				pointForTooth=GetPreviousToothPoint(toothNum,isFacial,false);
				isFacial=IsFacialFromPoint(pointForTooth);
				toothNum=GetToothNumFromPoint(pointForTooth);
			}
			if(perioSurface==PerioSurface.Facial) {
				//User specified the facial point of the tooth.
				isFacial=true;
			}
			else if(perioSurface==PerioSurface.Lingual) {
				//User specified the lingual point of the tooth.
				isFacial=false;
			}
			//Get the point for the surface passed in. 
			pointForTooth=GetSurfacePointForTooth(toothNum,isFacial,perioSurface);
			contrPerio.SetBleedingFlagForPoint(pointForTooth,bleedingFlags);
		}

		///<summary>Sets the approprate Auto Advance radio button.</summary>
		private void SetAutoAdvance() {
			if(contrPerio.Direction==AutoAdvanceDirection.Custom) {
				return;
			}
			if(_perioCellPrevLocation.Surface==PerioSurface.Facial && _perioCellCurLocation.Surface==PerioSurface.Lingual) {
				contrPerio.Direction=AutoAdvanceDirection.Right;
				radioRight.Checked=true;
			}
			if(_perioCellPrevLocation.Surface==PerioSurface.Lingual && _perioCellCurLocation.Surface==PerioSurface.Facial) {
				contrPerio.Direction=AutoAdvanceDirection.Left;
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
			Point pointTooth=contrPerio.CurCell;
			bool isRadioRightChecked=radioRight.Checked;
			contrPerio.SaveCurExam(_perioExam);
			int selectedExam=contrPerio.SelectedExam;
			List<int> listSkippedTeeth=new List<int>();//int 1-32
			if(PerioExams.ListExams.Count > 0) {
				//set skipped teeth based on the last exam in the list: 
				listSkippedTeeth=PerioMeasures.GetSkipped(PerioExams.ListExams[PerioExams.ListExams.Count-1].PerioExamNum);
			}
			if(!listSkippedTeeth.Contains(toothNum)) {
				listSkippedTeeth.Add(toothNum);
			}
			PerioMeasures.SetSkipped(_perioExam.PerioExamNum,listSkippedTeeth);
			RefreshListExams();
			gridODExam.SetSelected(selectedExam,true);
			FillGrid();
			if(_perioCellCurLocation.ToothNum==toothNum) {//Go to the next tooth if the _perioCellCurLocation is on the tooth being skipped. 
				Point pointNextTooth=GetNextToothPoint(toothNum,_perioCellCurLocation.Surface==PerioSurface.Facial);
				contrPerio.SetNewCell(pointNextTooth.X,pointNextTooth.Y);
			}
			else {
				contrPerio.SetNewCell(pointTooth.X,pointTooth.Y);
			}
			if(contrPerio.Direction!=AutoAdvanceDirection.Custom) {
				radioRight.Checked=isRadioRightChecked;
				contrPerio.Direction=AutoAdvanceDirection.Left;
				if(isRadioRightChecked) {
					contrPerio.Direction=AutoAdvanceDirection.Right;
				}
			}
			return true;
		}

		///<summary>Moves the cursor to the probing row for the current tooth.</summary>
		private void GoToProbing() {
			int yVal;
			if(_perioCellCurLocation.Surface==PerioSurface.Facial) {
				if(_perioCellCurLocation.ToothNum<=16) {
					if(checkShowCurrent.Checked) {
						yVal=5;
					}
					else {
						yVal=6-Math.Min(6,contrPerio.SelectedExam+1);
					}
				}
				else {//ToothNum >= 17
					if(checkShowCurrent.Checked) {
						yVal=37;
					}
					else {
						yVal=36+Math.Min(6,contrPerio.SelectedExam+1);
					}
				}
			}
			else {//Lingual
				if(_perioCellCurLocation.ToothNum<=16) {
					if(checkShowCurrent.Checked) {
						yVal=15;
					}
					else {
						yVal=14+Math.Min(6,contrPerio.SelectedExam+1);
					}
				}
				else {//ToothNum >= 17
					if(checkShowCurrent.Checked) {
						yVal=26;
					}
					else {
						yVal=27-Math.Min(6,contrPerio.SelectedExam+1);
					}
				}
			}
			contrPerio.SetNewCell(FirstEmptyPositionX(_perioCellCurLocation.ToothNum,yVal),yVal);
		}

		///<summary>Moves the cursor to the mobility row for the current tooth.</summary>
		private void GoToMobility() {
			PerioCell perioCell=contrPerio.GetCurrentCell();
			int toothNum=perioCell.ToothNum;
			bool isFacial=_perioCellCurLocation.Surface==PerioSurface.Facial;
			if(contrPerio.IsCellTextEmpty(GetPointForTooth(toothNum,isFacial))) {
				toothNum=GetToothNumFromPoint(GetPreviousToothPoint(toothNum,isFacial));
			}
			Point pointForMobility=GetPointForMobility(toothNum);
			if(!contrPerio.IsCellTextEmpty(pointForMobility)) {
				pointForMobility=GetPointForMobility(perioCell.ToothNum);
			}
			contrPerio.SetNewCell(pointForMobility.X,pointForMobility.Y);
		}

		private Point GetPointForMobility(int toothNum) {
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
			return new Point(xVal,yVal);
		}

		///<summary>Moves the cursor to the gingival margin row for the current tooth.</summary>
		private void GoToGingivalMargin() {
			GoToSequenceType(PerioSequenceType.GingMargin);
		}

		///<summary>Moves the cursor to the furcation row for the current tooth.</summary>
		private void GoToFurcation() {
			GoToSequenceType(PerioSequenceType.Furcation);
		}

		///<summary>Moves the cursor to the muco gingival junction row for the current tooth.</summary>
		private void GoToMGJ() {
			GoToSequenceType(PerioSequenceType.MGJ);
		}

		///<summary>Moves the cursor to the passed in SequenceType for the current tooth.</summary>
		private void GoToSequenceType(PerioSequenceType perioSequenceType) {
			int toothNum=_perioCellCurLocation.ToothNum;
			AutoAdvanceDirection autoAdvanceDirectionOld=contrPerio.Direction;
			bool isFacial=_perioCellCurLocation.Surface==PerioSurface.Facial;
			Point pointForTooth=GetPointForTooth(toothNum,isFacial);
			if(contrPerio.IsCellTextEmpty(pointForTooth)) {
				pointForTooth=GetPreviousToothPoint(toothNum,isFacial);
				isFacial=IsFacialFromPoint(pointForTooth);
				toothNum=GetToothNumFromPoint(pointForTooth);
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
			//create a new point with the updated Y pos for the passed in sequence type. 
			Point pointForSequenceType=new Point(pointForTooth.X,yPos);
			//Teeth 1-32 F isReverse=false, Teeth 1-32 L isReverse=true;
			//Find the next available point for the sequence type.
			Point pointNextAvailable=TryGetNextEmptyPoint(pointForSequenceType,!isFacial);
			if(toothNum!=GetToothNumFromPoint(pointNextAvailable)) 
			{
				//No positions are empty or the next available point is on a new tooth,. Use the xPos from the toothOld.
				xPos=GetPointForTooth(_perioCellCurLocation.ToothNum,_perioCellCurLocation.Surface==PerioSurface.Facial).X;
				contrPerio.Direction=autoAdvanceDirectionOld;//change this value back just in case we changed it above.
			}
			else {
				//The new point for the sequence was changed. Set the new xPos.
				xPos=FirstEmptyPositionX(toothNum,yPos);
			}
			//Set the cursor with the new points for the sequence.
			contrPerio.SetNewCell(xPos,yPos);
		}

		private bool IsFacialFromPoint(Point point) {
			return contrPerio.GetPerioCellFromPoint(point).Surface==PerioSurface.Facial;
		}

		///<summary>>Gets the previous tooth point. Takes into account skipped teeth.</summary>
		private Point GetPreviousToothPoint(int toothOld,bool isFacial,bool doSetDirection=true) {
			contrPerio.TryFindNextCell(GetPointForTooth(toothOld,isFacial),true,out Point pointPrev,doSetDirection);
			return pointPrev;
		}

		///<summary>Gets the next tooth point.  Takes into account skipped teeth.</summary>
		private Point GetNextToothPoint(int toothOld,bool isFacial) {
			contrPerio.TryFindNextCell(GetPointForTooth(toothOld,isFacial),false,out Point pointNext);
			return pointNext;
		}

		///<summary>Returns a point for the tooth's surface passed in.</summary>
		private Point GetSurfacePointForTooth(int toothNum,bool isFacial,PerioSurface perioSurface) {
			Point pointForTooth=GetPointForTooth(toothNum,isFacial);
			//Get set to either the right(Lingual) or left(Facial) position.
			int xPos=pointForTooth.X;
			//Midline=Looking at the Perio Chart(FormPerio.cs designer), this is the vertial line down the middle.
			//Facial and Lingual= will always be the middle cell of each of the cells in the perio chart.
			//Mesial=This is the position closest to the Midline. Teeth 1-8 and 25-32 right cell, teeth 9-24 left cell.
			//Distal=This is the postion furthest away from the midline. Teeth 1-8 and 25-32 left cell, teeth 9-24 right cell.
			switch(perioSurface) {
				case PerioSurface.Distal:
					if(toothNum.Between(9,24)) {
						//Get the right position.
						xPos=xPos + (isFacial ? 2 : 0);
					}
					else {
						//Teeth 1-8 and 25-32. Get the left position.
						xPos=xPos + (isFacial ? 0 : -2);
					}
					break;
				case PerioSurface.Facial:
				case PerioSurface.Lingual:
					xPos=xPos + (isFacial ? 1 : -1);//middle position
					break;
				case PerioSurface.Mesial:
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
			return new Point(xPos,pointForTooth.Y);
		}

		///<summary>Returns a point for the tooth's passed in.</summary>
		private Point GetPointForTooth(int toothNum,bool isFacial) {
			int x=-1;
			int y=-1;
			bool isLingual=!isFacial;
			if(isFacial && toothNum.Between(1,16)) {
				x=(toothNum)*3-2;
				if (checkShowCurrent.Checked) {
					y=5;
				}
				else {
					y=6-Math.Min(6,contrPerio.SelectedExam+1);
				}
			}
			else if(isFacial && toothNum.Between(17,32)) {
				x=(32-toothNum)*3+1;
				if (checkShowCurrent.Checked) {
					y=37;
				}
				else {
					y=36+Math.Min(6,contrPerio.SelectedExam+1);
				}
			}
			else if(isLingual && toothNum.Between(1,16)) {
				x=(toothNum)*3;
				if (checkShowCurrent.Checked) {
					y=15;
				}
				else {
					y=14+Math.Min(6,contrPerio.SelectedExam+1);
				}
			}
			else {//if(isLingual && toothNum.Between(17,32)) {
				x=(32-toothNum)*3+3;
				if (checkShowCurrent.Checked) {
					y=26;
				}
				else {
					y=27-Math.Min(6,contrPerio.SelectedExam+1);
				}
			}
			return new Point(x,y);
		}

		private int GetToothNumFromPoint(Point point) {
			PerioCell perioCell=contrPerio.GetPerioCellFromPoint(point);
			return perioCell.ToothNum;
		}

		///<summary>Gets the next available tooth(Point) if there is an empty cell. Returns a Point with value (-1,-1) if all cells are filled.</summary>
		private Point TryGetNextAvailablePointForTooth(int toothNum,bool isFacial) {
			//Get the first point for the tooth passed in.
			Point pointForTooth=GetPointForTooth(toothNum,isFacial);
			//Check all three positions for the tooth. It will return either an empty position or the last position of the tooth.
			Point pointNextAvailable=new Point(FirstEmptyPositionX(pointForTooth),pointForTooth.Y);
			//Check the position that was returned above for measurements.
			if(!string.IsNullOrEmpty(contrPerio.GetCellText(pointNextAvailable.X,pointNextAvailable.Y))) {
				return new Point(-1,-1);//Tooth is not empty.
			}
			return pointNextAvailable;
		}

		///<summary>Gets the next available point.</summary>
		private Point TryGetNextEmptyPoint(Point pointForTooth,bool isReverse) {
			if(string.IsNullOrEmpty(contrPerio.GetCellText(pointForTooth.X,pointForTooth.Y))) {
				return pointForTooth;//Tooth is not empty.
			}
			return NextEmptyPointHelper(pointForTooth,isReverse);
		}

		///<summary>Uses the AdvanceCell method to figure out the.</summary>
		private Point NextEmptyPointHelper(Point pointForTooth,bool isReverse) {
			contrPerio.TryFindNextCell(pointForTooth,isReverse,out Point pointNextAvailable);
			if(string.IsNullOrEmpty(contrPerio.GetCellText(pointNextAvailable.X,pointNextAvailable.Y))) {
				return pointNextAvailable;//Tooth is not empty.
			}
			int toothNum=GetToothNumFromPoint(pointNextAvailable);
			if(toothNum<1 || toothNum>32) {//I think we need <=1 and >=32
				return pointForTooth;
			}
			if(pointForTooth==pointNextAvailable) {
				return pointForTooth;//We are probably trying to advance into a skipped tooth that's the last tooth.
			}
			return NextEmptyPointHelper(pointNextAvailable,isReverse);
		}

		///<summary>Returns the first non-empty cell for the tooth. If all cells in the tooth are filled, returns the last cell of the tooth.</summary>
		private int FirstEmptyPositionX(int toothNum,int yVal) {
			int xVal;
			if(_perioCellCurLocation.Surface==PerioSurface.Facial) {
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

		///<summary>Returns the first non-empty cell for the Point passed in.</summary>
		private int FirstEmptyPositionX(Point pointForTooth) {
			int xVal=pointForTooth.X;
			int yVal=pointForTooth.Y;
			if(string.IsNullOrEmpty(contrPerio.GetCellText(xVal,yVal))) {
				return xVal;
			}
			//First position is not empty. Get the 2nd position of the point passed in.
			Point pointNextAvailable;
			if(!contrPerio.TryFindNextCell(pointForTooth,false,out pointNextAvailable,doSetDirection: false)) {
				return xVal;
			}
			xVal=pointNextAvailable.X;
			if(string.IsNullOrEmpty(contrPerio.GetCellText(xVal,yVal))) {
				return xVal;
			}
			//The section position is not empty. Get the last position of the point passed in.
			contrPerio.TryFindNextCell(pointNextAvailable,false,out pointNextAvailable,doSetDirection: false);
			return pointNextAvailable.X;//this should be the last position of the point passed in.
		}

		/// <summary>Used to force focus to the hidden textbox when showing this form.</summary>
		private void FormPerio_Shown(object sender,EventArgs e) {
			textInputBox.Focus();//This cannot go into load because focus must come after window has been shown.
		}

		///<summary>After this method runs, the selected index is usually set.</summary>
		private void RefreshListExams(bool doSkipRefreshMeasures=false) {
			//most recent date at the bottom
			PerioExams.Refresh(_patient.PatNum);
			if(!doSkipRefreshMeasures) {
				PerioMeasures.Refresh(_patient.PatNum,PerioExams.ListExams);
			}
			gridODExam.BeginUpdate();
			gridODExam.Columns.Clear();
			gridODExam.ListGridRows.Clear();
			OpenDental.UI.GridColumn col;
			col=new OpenDental.UI.GridColumn(Lan.g("TablePerioExam","Exam Date"),75,HorizontalAlignment.Center);
			gridODExam.Columns.Add(col);
			col=new OpenDental.UI.GridColumn(Lan.g("TablePerioExam","Provider"),75,HorizontalAlignment.Center);
			gridODExam.Columns.Add(col);
			col=new OpenDental.UI.GridColumn(Lan.g("TablePerioExam","Exam Notes"),150,HorizontalAlignment.Center);
			gridODExam.Columns.Add(col);
			for(int i=0;i<PerioExams.ListExams.Count;i++) {
				OpenDental.UI.GridRow row=new OpenDental.UI.GridRow();
				row.Cells.Add(PerioExams.ListExams[i].ExamDate.ToShortDateString());
				row.Cells.Add(Providers.GetAbbr(PerioExams.ListExams[i].ProvNum));
				int index5thNewLine=GetIndexCount(PerioExams.ListExams[i].Note,"\n",5);//Our goal is to get no more than 5 lines into the text box.
				int noteEnd=90;//90 characters is about what it takes to get most strings to fit the 5 lines.
				if(PerioExams.ListExams[i].Note.Length > noteEnd || index5thNewLine > -1) {
					if(index5thNewLine > -1) {
						noteEnd=Math.Min(noteEnd,index5thNewLine);
					}
					row.Cells.Add(PerioExams.ListExams[i].Note.Substring(0,noteEnd)+"(...)");
				} 
				else {
					row.Cells.Add(PerioExams.ListExams[i].Note);
				}
				gridODExam.ListGridRows.Add(row);
			}
			gridODExam.EndUpdate();
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

		///<summary>Orion only.</summary>
		private void RefreshListPlaque() {
			PerioExams.Refresh(_patient.PatNum);
			PerioMeasures.Refresh(_patient.PatNum,PerioExams.ListExams);
			listPlaqueHistory.Items.Clear();
			for(int i=0;i<PerioExams.ListExams.Count;i++) {
				string ph="";
				ph=PerioExams.ListExams[i].ExamDate.ToShortDateString()+"\t";
				contrPerio.SelectedExam=i;
				contrPerio.LoadData();
				ph+=contrPerio.ComputeOrionPlaqueIndex();
				listPlaqueHistory.Items.Add(ph);
			}
			//Not sure if necessary but set it back to what it was
			contrPerio.SelectedExam=gridODExam.GetSelectedIndex();
		}

		///<summary>Checks if perio is active on a MAD row with the current patient. If it is, locks editing.</summary>
		private void CheckMobileActivity() {
			if(!LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.ODTouch,Clinics.ClinicNum)) {
				groupEClip.Enabled=false;
				groupEClip.Visible=false;
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

		///<summary>Unique to when eClipboard is viewing / editing this patients perio exams. Starts the eClipboard timer check.</summary>
		private void SetEClipBoardEditing(bool isEditingOnEClipBoard) {
			contrPerio.perioEdit=!isEditingOnEClipBoard;
			gridODExam.Enabled=!isEditingOnEClipBoard;
			butAdd.Enabled=!isEditingOnEClipBoard;
			butCopyNote.Enabled=!isEditingOnEClipBoard;
			butCopyPrevious.Enabled=!isEditingOnEClipBoard;
			butDelete.Enabled=!isEditingOnEClipBoard;
			butEClipboard.Enabled=!isEditingOnEClipBoard;
			butAllExamsEclip.Enabled=!isEditingOnEClipBoard;
			butListen.Enabled=!isEditingOnEClipBoard;
			butDefault.Enabled=!isEditingOnEClipBoard;
			butUnlockEClip.Enabled=isEditingOnEClipBoard;
			labelIsMobileActive.Visible=isEditingOnEClipBoard;
		}

		///<summary>Usually set the selected index first</summary>
		private void FillGrid(bool doSelectCell=true) {
			if(gridODExam.GetSelectedIndex()!=-1){
				contrPerio.perioEdit=true;
				if(!Security.IsAuthorized(Permissions.PerioEdit,PerioExams.ListExams[gridODExam.GetSelectedIndex()].ExamDate,true)) {
					contrPerio.perioEdit=false;
				}
				_perioExam=PerioExams.ListExams[gridODExam.GetSelectedIndex()];
			}
			contrPerio.SelectedExam=gridODExam.GetSelectedIndex();
			contrPerio.DoShowCurrentExamOnly=checkShowCurrent.Checked;
			contrPerio.LoadData(doSelectCell);
			FillIndexes();
			FillCounts();
			contrPerio.Invalidate();
			contrPerio.Focus();//this still doesn't seem to work to enable first arrow click to move
			if(_perioExam!=null) {
				textExamNotes.Text=_perioExam.Note;
			}
		}
		
		private void gridODExam_CellClick(object sender,UI.ODGridClickEventArgs e) {
			if(gridODExam.GetSelectedIndex()==contrPerio.SelectedExam) { 
				return;
			}
			//Only continues if clicked on other than current exam
			contrPerio.SaveCurExam(_perioExam);
			//no need to RefreshListExams because it has not changed
			PerioExams.Refresh(_patient.PatNum);//refresh instead
			PerioMeasures.Refresh(_patient.PatNum,PerioExams.ListExams);
			FillGrid();
		}

		private void gridODExam_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			//remember that the first click may not have triggered the mouse down routine
			//and the second click will never trigger it.
			if(gridODExam.GetSelectedIndex()==-1) {
				return;
			}
			if(!Security.IsAuthorized(Permissions.PerioEdit,PerioExams.ListExams[gridODExam.GetSelectedIndex()].ExamDate)) {
				return;
			}
			//a PerioExam.Cur will always have been set through mousedown(or similar),then FillGrid
			contrPerio.SaveCurExam(_perioExam);
			PerioExams.Refresh(_patient.PatNum);//list will not change
			PerioMeasures.Refresh(_patient.PatNum,PerioExams.ListExams);
			using FormPerioEdit formPerioEdit=new FormPerioEdit();
			formPerioEdit.PerioExamCur=_perioExam;
			formPerioEdit.ShowDialog();
			int index=gridODExam.GetSelectedIndex();
			RefreshListExams();
			gridODExam.SetSelected(index,true);
			FillGrid();
		}

		private void butDefault_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.PerioEdit,MiscData.GetNowDateTime())){
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
			SecurityLogs.MakeLogEntry(Permissions.PerioEdit,_patient.PatNum,"Default perio exam created");
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

		private void butAdd_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.PerioEdit,MiscData.GetNowDateTime())){
				return;
			}
			if(gridODExam.GetSelectedIndex()!=-1){
				contrPerio.SaveCurExam(_perioExam);
			}
			CreateNewPerioChart();
			RefreshListExams();
			gridODExam.SetSelected(gridODExam.ListGridRows.Count-1,true);
			FillGrid();
			SecurityLogs.MakeLogEntry(Permissions.PerioEdit,_patient.PatNum,"Perio exam created");
		}

		private void butCopyNote_Click(object sender,EventArgs e) {
			if(gridODExam.GetSelectedIndex()!=-1) {
				ODClipboard.SetClipboard(textExamNotes.Text);
			}
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

		///<summary>Returns a list of skipped teeth.</summary>
		private List<int> GetSkippedTeeth() {
			List<int> listSkippedTeeth=new List<int>();
			if(PerioExams.ListExams.Count > 0) {
				//set skipped teeth based on the last exam in the list: 
				listSkippedTeeth=PerioMeasures.GetSkipped(PerioExams.ListExams[PerioExams.ListExams.Count-1].PerioExamNum);
			}
			//For patient's first perio chart, any teeth marked missing are automatically marked skipped.
			if(PerioExams.ListExams.Count != 0 && !PrefC.GetBool(PrefName.PerioSkipMissingTeeth)) {
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

		private void butCopyPrevious_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.PerioEdit,MiscData.GetNowDateTime())) {
				return;
			}
			if(PerioExams.ListExams.Count==0){
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
			List<PerioMeasure> listPerioMeasures=PerioMeasures.GetAllForExam(PerioExams.ListExams[PerioExams.ListExams.Count-1].PerioExamNum);
			for(int i=0;i<listPerioMeasures.Count;i++) { //add all of the previous exam's measures to this perio exam.
				listPerioMeasures[i].PerioExamNum=_perioExam.PerioExamNum;
				PerioMeasures.Insert(listPerioMeasures[i]);
			}
			RefreshListExams();
			gridODExam.SetSelected(PerioExams.ListExams.Count-1,true); //select the exam that was just inserted.
			FillGrid();
			SecurityLogs.MakeLogEntry(Permissions.PerioEdit,_patient.PatNum,"Perio exam copied.");
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(gridODExam.GetSelectedIndex()==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			if(!Security.IsAuthorized(Permissions.PerioEdit,PerioExams.ListExams[gridODExam.GetSelectedIndex()].ExamDate)) {
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
				gridODExam.SetSelected(PerioExams.ListExams.Count-1,true);
			}	
			FillGrid();
			if(gridODExam.ListGridRows.Count==0) {
				textExamNotes.Text="";
			}
			SecurityLogs.MakeLogEntry(Permissions.PerioEdit,_patient.PatNum,"Perio exam deleted");
		}

		private void butListen_Click(object sender,EventArgs e) {
			if(ODBuild.IsWeb()) {
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

		private void checkShowCurrent_Click(object sender,EventArgs e) {
			if(gridODExam.GetSelectedIndex()==-1) {
				return;
			}
			contrPerio.SaveCurExam(_perioExam);
			PerioExams.Refresh(_patient.PatNum);
			PerioMeasures.Refresh(_patient.PatNum,PerioExams.ListExams);
			FillGrid();
		}

		private void radioRight_Click(object sender, System.EventArgs e) {
			contrPerio.Direction=AutoAdvanceDirection.Right;
			contrPerio.Focus();
		}

		private void radioLeft_Click(object sender, System.EventArgs e) {
			contrPerio.Direction=AutoAdvanceDirection.Left;
			contrPerio.Focus();
		}

		private void RadioFacialsFirst_Click(object sender,EventArgs e) {
			contrPerio.Direction=AutoAdvanceDirection.Custom;
			contrPerio.Focus();
		}

		private void gridP_DirectionChangedRight(object sender, System.EventArgs e) {
			radioRight.Checked=true;
		}

		private void gridP_DirectionChangedLeft(object sender, System.EventArgs e) {
			radioLeft.Checked=true;
		}

		private void checkThree_Click(object sender, System.EventArgs e) {
			contrPerio.ThreeAtATime=checkThree.Checked;
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

		///<summary>The only valid numbers are 0 through 9</summary>
		private void NumberClicked(int number){
			if(contrPerio.SelectedExam==-1) {
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
			contrPerio.Focus();
		}

		private void but10_Click(object sender, System.EventArgs e) {
			_isTenDown=true;
		}

		private void butCalcIndex_Click(object sender, System.EventArgs e) {
			FillIndexes();
			if(!listPlaqueHistory.Visible) {
				return;
			}
			contrPerio.SaveCurExam(_perioExam);
			RefreshListPlaque();
			FillGrid();
		}

		private void FillIndexes(){
			textIndexPlaque.Text=contrPerio.ComputeIndex(BleedingFlags.Plaque);
			textIndexCalculus.Text=contrPerio.ComputeIndex(BleedingFlags.Calculus);
			textIndexBleeding.Text=contrPerio.ComputeIndex(BleedingFlags.Blood);
			textIndexSupp.Text=contrPerio.ComputeIndex(BleedingFlags.Suppuration);
		}

		private void butBleed_Click(object sender, System.EventArgs e) {
			_isTenDown=false;
			contrPerio.ButtonPressed("b");
			contrPerio.Focus();
		}

		private void butPus_Click(object sender, System.EventArgs e) {
			_isTenDown=false;
			contrPerio.ButtonPressed("s");
			contrPerio.Focus();
		}

		private void butPlaque_Click(object sender, System.EventArgs e) {
			_isTenDown=false;
			contrPerio.ButtonPressed("p");
			contrPerio.Focus();
		}

		private void butCalculus_Click(object sender, System.EventArgs e) {
			_isTenDown=false;
			contrPerio.ButtonPressed("c");
			contrPerio.Focus();
		}

		private void butColorBleed_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.DefEdit)) {
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

		private void butColorPus_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.DefEdit)) {
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

		private void butColorPlaque_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.DefEdit)) {
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

		private void butColorCalculus_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.DefEdit)) {
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

		private void butSkip_Click(object sender, System.EventArgs e) {
			if(gridODExam.GetSelectedIndex()<0){//PerioExamCur could still be set to a deleted exam and would not be null even if there is no exam.
				MessageBox.Show(Lan.g(this,"Please select an exam first."));
				return;
			}
			contrPerio.ToggleSkip(_perioExam.PerioExamNum);
		}

		private void updownRed_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			//this is necessary because Microsoft's updown control is too buggy to be useful
			Cursor=Cursors.WaitCursor;
			PrefName prefname=PrefName.PerioRedProb;
			if(sender==updownProb){
				prefname=PrefName.PerioRedProb;
			}
			else if(sender==updownMGJ) {
				prefname=PrefName.PerioRedMGJ;
			}
			else if(sender==updownGing) {
				prefname=PrefName.PerioRedGing;
			}
			else if(sender==updownCAL) {
				prefname=PrefName.PerioRedCAL;
			}
			else if(sender==updownFurc) {
				prefname=PrefName.PerioRedFurc;
			}
			else if(sender==updownMob) {
				prefname=PrefName.PerioRedMob;
			}
			int val=PrefC.GetInt(prefname);
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
			Prefs.UpdateLong(prefname,val);
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

		private void butCount_Click(object sender, System.EventArgs e) {
			FillCounts();
			contrPerio.Focus();
		}

		private void FillCounts(){
			textCountProb.Text=contrPerio.CountTeeth(PerioSequenceType.Probing).Count.ToString();
			textCountMGJ.Text=contrPerio.CountTeeth(PerioSequenceType.MGJ).Count.ToString();
			textCountGing.Text=contrPerio.CountTeeth(PerioSequenceType.GingMargin).Count.ToString();
			textCountCAL.Text=contrPerio.CountTeeth(PerioSequenceType.CAL).Count.ToString();
			textCountFurc.Text=contrPerio.CountTeeth(PerioSequenceType.Furcation).Count.ToString();
			textCountMob.Text=contrPerio.CountTeeth(PerioSequenceType.Mobility).Count.ToString();
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

		private void butSave_Click(object sender,EventArgs e) {
			if(this.gridODExam.GetSelectedIndex()<0){
				MessageBox.Show(Lan.g(this,"Please select an exam first."));
				return;
			}
			contrPerio.SaveCurExam(_perioExam);
			PerioExams.Refresh(_patient.PatNum);
			PerioMeasures.Refresh(_patient.PatNum,PerioExams.ListExams);
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
			using Bitmap bitmapGrid=new Bitmap(contrPerio.RectBoundsShowing.Width,contrPerio.RectBoundsShowing.Height);
			contrPerio.DrawToBitmap(bitmapGrid,contrPerio.RectBoundsShowing);
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

		private void pd2_PrintPage(object sender, PrintPageEventArgs ev){//raised for each page to be printed.
			Graphics g=ev.Graphics;
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
			ev.HasMorePages=false;
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

		private void butAllExamsEclip_Click(object sender,EventArgs e) {
			if(_patient==null) {
				return;
			}
			contrPerio.SaveCurExam(_perioExam);
			MobileDataByte funcInsertDataForUnlockCode(string unlockCode) {
				if(MobileDataBytes.TryInsertPatientPerio(_patient,unlockCode,0,out string errorMsg,out MobileDataByte mobileDataByte)) {
					return mobileDataByte;
				}
				MsgBox.Show(errorMsg);
				return null;
			};
			using FormMobileCode formMobileCode=new FormMobileCode(funcInsertDataForUnlockCode);
			formMobileCode.ShowDialog();
			if(formMobileCode.HasMobileCodeBeenReceived) {
				SetEClipBoardEditing(true);
			}
		}

		private void butEClipboard_Click(object sender,EventArgs e) {
			if(_perioExam==null || gridODExam.GetSelectedIndex()==-1) {
				MsgBox.Show("No Perio exam selected, please select an exam first.");
				return;
			}
			contrPerio.SaveCurExam(_perioExam);
			MobileDataByte funcInsertDataForUnlockCode(string unlockCode) {
				if(MobileDataBytes.TryInsertPatientPerio(_patient,unlockCode,_perioExam.PerioExamNum,out string errorMsg,out MobileDataByte mobileDataByte)) {
					return mobileDataByte;
				}
				MsgBox.Show(errorMsg);
				return null;
			};
			using FormMobileCode formMobileCode=new FormMobileCode(funcInsertDataForUnlockCode);
			formMobileCode.ShowDialog();
			if(formMobileCode.HasMobileCodeBeenReceived) {
				SetEClipBoardEditing(true);
			}
		}

		private void butUnlockEClip_Click(object sender,EventArgs e) {
			bool isExamInUse=MobileAppDevices.IsInUse(_patient.PatNum,MADPage.PerioExamEditPage,MADPage.PerioExamListPage);
			bool isAllowingEdit=false;
			if(isExamInUse) {
				isAllowingEdit=MsgBox.Show(MsgBoxButtons.YesNo,"This patients exam(s) are currently being edited in eClipboard. Would you like to edit this exam and close the eClipboard session?");
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
				OpenDentBusiness.WebTypes.PushNotificationUtils.CI_GoToCheckin(mobileAppDevice.MobileAppDeviceNum);
				mobileAppDevice.DevicePage=MADPage.CheckinPage;
				MobileAppDevices.Update(mobileAppDevice);
				SetEClipBoardEditing(false);
			}
			return;
		}

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
			PerioExams.Refresh(_patient.PatNum);//refresh instead
			PerioMeasures.Refresh(_patient.PatNum,PerioExams.ListExams);
			FillGrid();
			using FormPerioGraphical formPerioGraphical=new FormPerioGraphical(PerioExams.ListExams[gridODExam.GetSelectedIndex()],_patient);
			formPerioGraphical.ShowDialog();
		}

		private void checkGingMarg_CheckedChanged(object sender,EventArgs e) {
			contrPerio.GingMargPlus=checkGingMarg.Checked;
			contrPerio.Focus();
		}

		///<summary>This ensures that the textbox will always have focus when using FormPerio.</summary>
		private void textInputBox_Leave(object sender,EventArgs e) {
			if(!textExamNotes.Focused) {
				textInputBox.Focus();
			}
		}

		private void textExamNotes_Leave(object sender,EventArgs e) {
			if(_perioExam!=null) { 
				PerioExam perioExamOld=_perioExam.Copy();
				_perioExam.Note=textExamNotes.Text;
				if(PerioExams.Update(_perioExam,perioExamOld)) {
					int selectionOld=gridODExam.GetSelectedIndex();
					RefreshListExams(doSkipRefreshMeasures:true);
					gridODExam.SetSelected(selectionOld);
				}
			}
			textInputBox.Focus();
		}

		private void gridP_Click(object sender,EventArgs e) {
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
					|| charArrayInputs[i]=='p') 
				{
					contrPerio.ButtonPressed(charArrayInputs[i].ToString());
				}
				else if(charArrayInputs[i]==' ') {
					contrPerio.ButtonPressed("b");
				}
			}
			textInputBox.Clear();
		}

		private void butClose_Click(object sender,System.EventArgs e) {
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
			if(_userOdPrefCustomAdvance==null) {
				UserOdPrefs.Insert(new UserOdPref() {
					UserNum=Security.CurUser.UserNum,
					FkeyType=UserOdFkeyType.PerioAutoAdvanceCustom,
					ValueString=POut.Bool(radioFacialsFirst.Checked)
				});
				DataValid.SetInvalid(InvalidType.UserOdPrefs);
			}
			else {
				UserOdPref userOdPrefOld=_userOdPrefCustomAdvance.Clone();
				_userOdPrefCustomAdvance.ValueString=POut.Bool(radioFacialsFirst.Checked);
				if(UserOdPrefs.Update(_userOdPrefCustomAdvance,userOdPrefOld)) {
					//Only need to signal cache refresh on change.
					DataValid.SetInvalid(InvalidType.UserOdPrefs);
				}
			}
			Close();
		}

		private void FormPerio_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(_perioExam!=null) { //When no Exam is selected or ExamList is empty we don't want to save ExamNotes.
				PerioExam perioExamOld=_perioExam.Copy();
				_perioExam.Note=textExamNotes.Text;
				PerioExams.Update(_perioExam,perioExamOld);
			}
			if(_isLocalDefsChanged){
				DataValid.SetInvalid(InvalidType.Defs, InvalidType.Prefs);
			}
			if(gridODExam.GetSelectedIndex()!=-1){
				contrPerio.SaveCurExam(_perioExam);
			}
			_voiceController?.Dispose();
			Plugins.HookAddCode(this,"FormPerio.Closing_end");
		}
	}
}





















