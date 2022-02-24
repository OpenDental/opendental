using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEhrPatientSmoking:FormODBase {
		public Patient PatCur;
		///<summary>A copy of the original patient object, as it was when this form was first opened.</summary>
		private Patient _patOld;
		///<summary>List of tobacco use screening type codes.  Currently only 3 allowed SNOMED codes as of 2014.</summary>
		private List<EhrCode> _listAssessmentCodes;
		///<summary>All EhrCodes in the tobacco cessation counseling value set (2.16.840.1.113883.3.526.3.509).
		///When comboInterventionType has selected index 0, load the counseling intervention codes into comboInterventionCode.</summary>
		private List<EhrCode> _listCounselInterventionCodes;
		///<summary>All EhrCodes in the tobacco cessation medication value set (2.16.840.1.113883.3.526.3.1190).
		///When comboInterventionType has selected index 1, load the medication intervention codes into comboInterventionCode.</summary>
		private List<EhrCode> _listMedInterventionCodes;
		private List<EhrCode> _listRecentIntvCodes;
		///<summary>This list contains all of the intervention codes in the comboInterventionCode, counsel, medication, both.</summary>
		private List<EhrCode> _listInterventionCodes;
		///<summary>All EhrCodes in the tobacco user value set (2.16.840.1.113883.3.526.3.1170).
		///When radioAll or radioUser is selected, comboTobaccoStatuses will be filled with this list.</summary>
		private List<EhrCode> _listUserCodes;
		///<summary>All EhrCodes in the tobacco non-user value set (2.16.840.1.113883.3.526.3.1189).
		///When radioAll or radioNonUser is selected, comboTobaccoStatuses will be filled with this list.</summary>
		private List<EhrCode> _listNonUserCodes;
		///<summary>List of tobacco statuses selected from the SNOMED list for this pat that aren't in the list of EHR user and non-user codes</summary>
		private List<EhrCode> _listCustomTobaccoCodes;
		///<summary>List of recently used tobacco statuses, ordered by a date used weighted sum of number of times used.  Codes used the most will be
		///first in the list, with more recent EhrMeasureEvents having a heavier weight.</summary>
		private List<EhrCode> _listRecentTobaccoCodes;
		///<summary>This list contains all of the tobacco statuses in the comboTobaccoStatus, user, non-user, or both.  This list may also contain
		///statuses that the user has selected from the SNOMED list that are not a user or non-user code.</summary>
		private List<EhrCode> _listTobaccoStatuses;
		private ToolTip _comboToolTip;

		public FormEhrPatientSmoking() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormPatientSmoking_Load(object sender,EventArgs e) {
			_patOld=PatCur.Copy();
			textDateAssessed.Text=DateTime.Now.ToString();
			textDateIntervention.Text=DateTime.Now.ToString();
			#region ComboSmokeStatus
			comboSmokeStatus.Items.Add("None");//First and default index
			//Smoking statuses add in the same order as they appear in the SmokingSnoMed enum (Starting at comboSmokeStatus index 1).
			//Changes to the enum order will change the order added so they will always match
			for(int i=0;i<Enum.GetNames(typeof(SmokingSnoMed)).Length;i++) {
				//if snomed code exists in the snomed table, use the snomed description for the combo box, otherwise use the original abbreviated description
				Snomed smokeCur=Snomeds.GetByCode(((SmokingSnoMed)i).ToString().Substring(1));
				if(smokeCur!=null) {
					comboSmokeStatus.Items.Add(smokeCur.Description);
				}
				else {
					switch((SmokingSnoMed)i) {
						case SmokingSnoMed._266927001:
							comboSmokeStatus.Items.Add("UnknownIfEver");
							break;
						case SmokingSnoMed._77176002:
							comboSmokeStatus.Items.Add("SmokerUnknownCurrent");
							break;
						case SmokingSnoMed._266919005:
							comboSmokeStatus.Items.Add("NeverSmoked");
							break;
						case SmokingSnoMed._8517006:
							comboSmokeStatus.Items.Add("FormerSmoker");
							break;
						case SmokingSnoMed._428041000124106:
							comboSmokeStatus.Items.Add("CurrentSomeDay");
							break;
						case SmokingSnoMed._449868002:
							comboSmokeStatus.Items.Add("CurrentEveryDay");
							break;
						case SmokingSnoMed._428061000124105:
							comboSmokeStatus.Items.Add("LightSmoker");
							break;
						case SmokingSnoMed._428071000124103:
							comboSmokeStatus.Items.Add("HeavySmoker");
							break;
					}
				}
			}
			comboSmokeStatus.SelectedIndex=0;//None
			try {
				comboSmokeStatus.SelectedIndex=(int)Enum.Parse(typeof(SmokingSnoMed),"_"+PatCur.SmokingSnoMed,true)+1;
			}
			catch {
				//if not one of the statuses in the enum, get the Snomed object from the patient's current smoking snomed code
				Snomed smokeCur=Snomeds.GetByCode(PatCur.SmokingSnoMed);
				if(smokeCur!=null) {//valid snomed code, set the combo box text to this snomed description
					comboSmokeStatus.SelectedIndex=-1;
					comboSmokeStatus.Text=smokeCur.Description;
				}
			}
			#endregion
			//This takes a while the first time the window loads due to Code Systems.
			Cursor=Cursors.WaitCursor;
			FillGridAssessments();
			FillGridInterventions();
			Cursor=Cursors.Default;
			#region ComboAssessmentType
			_listAssessmentCodes=EhrCodes.GetForValueSetOIDs(new List<string> { "2.16.840.1.113883.3.526.3.1278" },true);//'Tobacco Use Screening' value set
			if(_listAssessmentCodes.Count==0) {//This should only happen if the EHR.dll does not exist or if the codes in the ehrcode list do not exist in the corresponding table
				MsgBox.Show(this,"The codes used for Tobacco Use Screening assessments do not exist in the LOINC table in your database.  You must run the Code System Importer tool in Setup | Chart | EHR to import this code set.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			_listAssessmentCodes.ForEach(x => comboAssessmentType.Items.Add(x.Description));
			string mostRecentAssessmentCode="";
			if(gridAssessments.ListGridRows.Count>1) {
				//gridAssessments.Rows are tagged with all TobaccoUseAssessed events for the patient ordered by DateTEvent, last is most recent
				mostRecentAssessmentCode=((EhrMeasureEvent)gridAssessments.ListGridRows[gridAssessments.ListGridRows.Count-1].Tag).CodeValueResult;
			}
			//use Math.Max so that if _listAssessmentCodes doesn't contain the mostRecentAssessment code the combobox will default to the first in the list
			comboAssessmentType.SelectedIndex=Math.Max(0,_listAssessmentCodes.FindIndex(x => x.CodeValue==mostRecentAssessmentCode));
			#endregion ComboAssessmentType
			#region ComboTobaccoStatus
			//list is filled with the EhrCodes for all tobacco user statuses using the CQM value set
			_listUserCodes=EhrCodes.GetForValueSetOIDs(new List<string> { "2.16.840.1.113883.3.526.3.1170" },true).OrderBy(x => x.Description).ToList();
			//list is filled with the EhrCodes for all tobacco non-user statuses using the CQM value set
			_listNonUserCodes=EhrCodes.GetForValueSetOIDs(new List<string> { "2.16.840.1.113883.3.526.3.1189" },true).OrderBy(x => x.Description).ToList();
			_listRecentTobaccoCodes=EhrCodes.GetForEventTypeByUse(EhrMeasureEventType.TobaccoUseAssessed);
			//list is filled with any SNOMEDCT codes that are attached to EhrMeasureEvents for the patient that are not in the User and NonUser lists
			_listCustomTobaccoCodes=new List<EhrCode>();
			//codeValues is an array of all user and non-user tobacco codes
			string[] codeValues=_listUserCodes.Concat(_listNonUserCodes).Concat(_listRecentTobaccoCodes).Select(x => x.CodeValue).ToArray();
			//listEventCodes will contain all unique tobacco codes that are not in the user and non-user lists
			List<string> listEventCodes=new List<string>();
			foreach(GridRow row in gridAssessments.ListGridRows) {
				string eventCodeCur=((EhrMeasureEvent)row.Tag).CodeValueResult;
				if(codeValues.Contains(eventCodeCur) || listEventCodes.Contains(eventCodeCur)) {
					continue;
				}
				listEventCodes.Add(eventCodeCur);
			}
			Snomed sCur;
			foreach(string eventCode in listEventCodes.OrderBy(x => x)) {
				sCur=Snomeds.GetByCode(eventCode);
				if(sCur==null) {//don't add invalid SNOMEDCT codes
					continue;
				}
				_listCustomTobaccoCodes.Add(new EhrCode { CodeValue=sCur.SnomedCode,Description=sCur.Description });
			}
			_listCustomTobaccoCodes=_listCustomTobaccoCodes.OrderBy(x => x.Description).ToList();
			//list will contain all of the tobacco status EhrCodes currently in comboTobaccoStatus
			_listTobaccoStatuses=new List<EhrCode>();
			//default to all tobacco statuses (custom, user, and non-user) in the status dropdown box
			radioRecentStatuses.Checked=true;//causes combo box and _listTobaccoStatuses to be filled with all statuses
			#endregion ComboTobaccoStatus
			#region ComboInterventionType and ComboInterventionCode
			//list is filled with EhrCodes for counseling interventions using the CQM value set
			_listCounselInterventionCodes=EhrCodes.GetForValueSetOIDs(new List<string> { "2.16.840.1.113883.3.526.3.509" },true).OrderBy(x => x.Description).ToList();
			//list is filled with EhrCodes for medication interventions using the CQM value set
			_listMedInterventionCodes=EhrCodes.GetForValueSetOIDs(new List<string> { "2.16.840.1.113883.3.526.3.1190" },true).OrderBy(x => x.Description).ToList();
			_listRecentIntvCodes=EhrCodes.GetForIntervAndMedByUse(InterventionCodeSet.TobaccoCessation,new List<string> { "2.16.840.1.113883.3.526.3.1190" });
			_listInterventionCodes=new List<EhrCode>();
			//default to all interventions (couseling and medication) in the intervention dropdown box
			radioRecentInterventions.Checked=true;//causes combo box and _listInterventionCodes to be filled with all intervention codes
			#endregion ComboInterventionType and ComboInterventionCode
			_comboToolTip=new ToolTip() { InitialDelay=1000,ReshowDelay=1000,ShowAlways=true };
		}

		private void FillGridAssessments() {
			gridAssessments.BeginUpdate();
			gridAssessments.ListGridColumns.Clear();
			gridAssessments.ListGridColumns.Add(new GridColumn("Date",70));
			gridAssessments.ListGridColumns.Add(new GridColumn("Type",170));
			gridAssessments.ListGridColumns.Add(new GridColumn("Description",170));
			gridAssessments.ListGridColumns.Add(new GridColumn("Documentation",170));
			gridAssessments.ListGridRows.Clear();
			GridRow row;
			List<EhrMeasureEvent> listEvents=EhrMeasureEvents.RefreshByType(PatCur.PatNum,EhrMeasureEventType.TobaccoUseAssessed);
			foreach(EhrMeasureEvent eventCur in listEvents) {
				row=new GridRow();
				row.Cells.Add(eventCur.DateTEvent.ToShortDateString());
				Loinc lCur=Loincs.GetByCode(eventCur.CodeValueEvent);//TobaccoUseAssessed events can be one of three types, all LOINC codes
				row.Cells.Add(lCur!=null?lCur.NameLongCommon:eventCur.EventType.ToString());
				Snomed sCur=Snomeds.GetByCode(eventCur.CodeValueResult);
				row.Cells.Add(sCur!=null?sCur.Description:"");
				row.Cells.Add(eventCur.MoreInfo);
				row.Tag=eventCur;
				gridAssessments.ListGridRows.Add(row);
			}
			gridAssessments.EndUpdate();
		}

		private void FillGridInterventions() {
			gridInterventions.BeginUpdate();
			gridInterventions.ListGridColumns.Clear();
			gridInterventions.ListGridColumns.Add(new GridColumn("Date",70));
			gridInterventions.ListGridColumns.Add(new GridColumn("Type",150));
			gridInterventions.ListGridColumns.Add(new GridColumn("Description",160));
			gridInterventions.ListGridColumns.Add(new GridColumn("Declined",60) { TextAlign=HorizontalAlignment.Center });
			gridInterventions.ListGridColumns.Add(new GridColumn("Documentation",140));
			gridInterventions.ListGridRows.Clear();
			//build list of rows of CessationInterventions and CessationMedications so we can order the list by date and type before filling the grid
			List<GridRow> listRows=new List<GridRow>();
			GridRow row;
			#region CessationInterventions
			List<Intervention> listInterventions=Interventions.Refresh(PatCur.PatNum,InterventionCodeSet.TobaccoCessation);
			foreach(Intervention iCur in listInterventions) {
				row=new GridRow();
				row.Cells.Add(iCur.DateEntry.ToShortDateString());
				string type=InterventionCodeSet.TobaccoCessation.ToString()+" Counseling";
				string descript="";
				switch(iCur.CodeSystem) {
					case "CPT":
						Cpt cptCur=Cpts.GetByCode(iCur.CodeValue);
						descript=cptCur!=null?cptCur.Description:"";
						break;
					case "SNOMEDCT":
						Snomed sCur=Snomeds.GetByCode(iCur.CodeValue);
						descript=sCur!=null?sCur.Description:"";
						break;
					case "RXNORM":
						//if the user checks the "Patient Declined" checkbox, we enter the tobacco cessation medication as an intervention that was declined
						type=InterventionCodeSet.TobaccoCessation.ToString()+" Medication";
						RxNorm rCur=RxNorms.GetByRxCUI(iCur.CodeValue);
						descript=rCur!=null?rCur.Description:"";
						break;
				}
				row.Cells.Add(type);
				row.Cells.Add(descript);
				row.Cells.Add(iCur.IsPatDeclined?"X":"");
				row.Cells.Add(iCur.Note);
				row.Tag=iCur;
				listRows.Add(row);
			}
			#endregion
			#region CessationMedications
			//Tobacco Use Cessation Pharmacotherapy Value Set
			string[] arrayRxCuiStrings=EhrCodes.GetForValueSetOIDs(new List<string> { "2.16.840.1.113883.3.526.3.1190" },true)
				.Select(x => x.CodeValue).ToArray();
			//arrayRxCuiStrings will contain 41 RxCui strings for tobacco cessation medications if those exist in the rxnorm table
			List<MedicationPat> listMedPats=MedicationPats.Refresh(PatCur.PatNum,true).FindAll(x => arrayRxCuiStrings.Contains(x.RxCui.ToString()));
			foreach(MedicationPat medPatCur in listMedPats) {
				row=new GridRow();
				List<string> listMedDates=new List<string>();
				if(medPatCur.DateStart.Year>1880) {
					listMedDates.Add(medPatCur.DateStart.ToShortDateString());
				}
				if(medPatCur.DateStop.Year>1880) {
					listMedDates.Add(medPatCur.DateStop.ToShortDateString());
				}
				if(listMedDates.Count==0) {
					listMedDates.Add(medPatCur.DateTStamp.ToShortDateString());
				}
				row.Cells.Add(string.Join(" - ",listMedDates));
				row.Cells.Add(InterventionCodeSet.TobaccoCessation.ToString()+" Medication");
				row.Cells.Add(RxNorms.GetDescByRxCui(medPatCur.RxCui.ToString()));
				row.Cells.Add(medPatCur.PatNote);
				row.Tag=medPatCur;
				listRows.Add(row);
			}
			#endregion
			listRows.OrderBy(x => PIn.Date(x.Cells[0].Text))//rows ordered by date, oldest first
				.ThenBy(x => x.Cells[3].Text!="")
				//interventions at the top, declined med interventions below normal interventions
				.ThenBy(x => x.Tag.GetType().Name!="Intervention" || ((Intervention)x.Tag).CodeSystem=="RXNORM").ToList()
				.ForEach(x => gridInterventions.ListGridRows.Add(x));//then add rows to gridInterventions
			gridInterventions.EndUpdate();
		}

		private void gridAssessments_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//we will allow them to change the DateTEvent, but not the status or more info box
			using FormEhrMeasureEventEdit FormM=new FormEhrMeasureEventEdit((EhrMeasureEvent)gridAssessments.ListGridRows[e.Row].Tag);
			FormM.ShowDialog();
			FillGridAssessments();
		}

		private void gridInterventions_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Object objCur=gridInterventions.ListGridRows[e.Row].Tag;
			//the intervention grid will be filled with Interventions and MedicationPats, load form accordingly
			if(objCur is Intervention) {
				using FormInterventionEdit FormI=new FormInterventionEdit();
				FormI.InterventionCur=(Intervention)objCur;
				FormI.IsAllTypes=false;
				FormI.IsSelectionMode=false;
				FormI.InterventionCur.IsNew=false;
				FormI.ShowDialog();
			}
			else if(objCur is MedicationPat) {
				using FormMedPat FormMP=new FormMedPat();
				FormMP.MedicationPatCur=(MedicationPat)objCur;
				FormMP.IsNew=false;
				FormMP.ShowDialog();
			}
			FillGridInterventions();
		}

		private void comboSmokeStatus_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboSmokeStatus.SelectedIndex<1) {//If None or text set to other selected Snomed code so -1, do not create an event
				return;
			}
			//Insert measure event if one does not already exist for this date
			DateTime dateTEntered=PIn.DateT(textDateAssessed.Text);//will be set to DateTime.Now when form loads
			EhrMeasureEvent eventCur;
			foreach(GridRow row in gridAssessments.ListGridRows) {
				eventCur=(EhrMeasureEvent)row.Tag;
				if(eventCur.DateTEvent.Date==dateTEntered.Date) {//one already exists for this date, don't auto insert event
					return;
				}
			}
			//no entry for the date entered, so insert one
			eventCur=new EhrMeasureEvent();
			eventCur.DateTEvent=dateTEntered;
			eventCur.EventType=EhrMeasureEventType.TobaccoUseAssessed;
			eventCur.PatNum=PatCur.PatNum;
			eventCur.CodeValueEvent=_listAssessmentCodes[comboAssessmentType.SelectedIndex].CodeValue;
			eventCur.CodeSystemEvent=_listAssessmentCodes[comboAssessmentType.SelectedIndex].CodeSystem;
			//SelectedIndex guaranteed to be greater than 0
			eventCur.CodeValueResult=((SmokingSnoMed)comboSmokeStatus.SelectedIndex-1).ToString().Substring(1);
			eventCur.CodeSystemResult="SNOMEDCT";//only allow SNOMEDCT codes for now.
			eventCur.MoreInfo="";
			EhrMeasureEvents.Insert(eventCur);
			FillGridAssessments();
		}

		private void comboTobaccoStatus_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboTobaccoStatus.SelectedIndex<_listTobaccoStatuses.Count) {//user selected a code in the list, just return
				return;
			}
			if(comboTobaccoStatus.SelectedIndex==_listTobaccoStatuses.Count
				&& !MsgBox.Show(this,MsgBoxButtons.OKCancel,"Selecting a code that is not in the recommended list of codes may make "
					+"it more difficult to meet CQM's."))
			{
				comboTobaccoStatus.SelectedIndex=-1;
				return;
			}
			//user wants to select a custom status from the SNOMED list
			using FormSnomeds FormS=new FormSnomeds();
			FormS.IsSelectionMode=true;
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK) {
				comboTobaccoStatus.SelectedIndex=-1;
				return;
			}
			if(!_listTobaccoStatuses.Any(x => x.CodeValue==FormS.SelectedSnomed.SnomedCode)) {
				_listCustomTobaccoCodes.Add(new EhrCode() { CodeValue=FormS.SelectedSnomed.SnomedCode,Description=FormS.SelectedSnomed.Description });
				_listCustomTobaccoCodes=_listCustomTobaccoCodes.OrderBy(x => x.Description).ToList();
				radioTobaccoStatuses_CheckedChanged(GetSelectedStatusFilter(),new EventArgs());//refills drop down with newly added custom code
			}
			//selected code guaranteed to exist in the drop down at this point
			comboTobaccoStatus.Items.Clear();
			comboTobaccoStatus.Items.AddRange(_listTobaccoStatuses.Select(x => x.Description).ToArray());
			comboTobaccoStatus.Items.Add(Lan.g(this,"Choose from all SNOMED CT codes")+"...");
			comboTobaccoStatus.SelectedIndex=_listTobaccoStatuses.FindIndex(x => x.CodeValue==FormS.SelectedSnomed.SnomedCode);//add 1 for ...choose from
		}

		///<summary>Fill comboTobaccoStatus with user and non-user tobacco status codes using _listUserCodes and/or _listNonUserCodes
		///depending on which radio button is selected.</summary>
		private void radioTobaccoStatuses_CheckedChanged(object sender,EventArgs e) {
			RadioButton radButCur=(RadioButton)sender;
			if(!radButCur.Checked) {
				return;
			}
			_listTobaccoStatuses.Clear();
			if(_listCustomTobaccoCodes.Count>0) {
				_listTobaccoStatuses.AddRange(_listCustomTobaccoCodes);
			}
			if(radButCur.Name==radioRecentStatuses.Name) {
				_listTobaccoStatuses.AddRange(_listRecentTobaccoCodes);
			}
			else {
				if(new[] { radioAllStatuses.Name,radioUserStatuses.Name }.Contains(radButCur.Name)) {
					_listTobaccoStatuses.AddRange(_listUserCodes);
				}
				if(new[] { radioAllStatuses.Name,radioNonUserStatuses.Name }.Contains(radButCur.Name)) {
					_listTobaccoStatuses.AddRange(_listNonUserCodes);
				}
			}
			_listTobaccoStatuses=_listTobaccoStatuses.OrderBy(x => x.Description).ToList();
			comboTobaccoStatus.Items.Clear();
			comboTobaccoStatus.Items.AddRange(_listTobaccoStatuses.Select(x => x.Description).ToArray());
			comboTobaccoStatus.Items.Add(Lan.g(this,"Choose from all SNOMED CT codes")+"...");
		}

		///<summary>Fill comboInterventionCode with counseling and medication intervention codes using _listCounselInterventionCodes
		///and/or _listMedInterventionCodes depending on which radio button is selected.</summary>
		private void radioInterventions_CheckedChanged(object sender,EventArgs e) {
			RadioButton radButCur=(RadioButton)sender;
			if(!radButCur.Checked) {//if not checked, do nothing, caused by another radio button being checked
				return;
			}
			_listInterventionCodes.Clear();
			if(radButCur.Name==radioRecentInterventions.Name) {
				_listInterventionCodes.AddRange(_listRecentIntvCodes);
			}
			if(new[] { radioAllInterventions.Name,radioCounselInterventions.Name }.Contains(radButCur.Name)) {
				_listInterventionCodes.AddRange(_listCounselInterventionCodes);
			}
			if(new[] { radioAllInterventions.Name,radioMedInterventions.Name }.Contains(radButCur.Name)) {
				_listInterventionCodes.AddRange(_listMedInterventionCodes);
			}
			_listInterventionCodes=_listInterventionCodes.OrderBy(x => x.Description).ToList();
			comboInterventionCode.Items.Clear();
			//this is the max width of the description, minus the width of "..." and, if > 30 items in the list, the width of the vertical scroll bar
			int maxItemWidth=comboInterventionCode.DropDownWidth-(_listInterventionCodes.Count>30?25:8);//8 for just "...", 25 for scroll bar plus "..."
			foreach(EhrCode code in _listInterventionCodes) {
				if(TextRenderer.MeasureText(code.Description,comboInterventionCode.Font).Width<comboInterventionCode.DropDownWidth-15
					|| code.Description.Length<3)
				{
					comboInterventionCode.Items.Add(code.Description);
					continue;
				}
				StringBuilder abbrDesc=new StringBuilder();
				foreach(char c in code.Description) {
					if(TextRenderer.MeasureText(abbrDesc.ToString()+c,comboInterventionCode.Font).Width<maxItemWidth) {
						abbrDesc.Append(c);
						continue;
					}
					comboInterventionCode.Items.Add(abbrDesc.ToString()+"...");
					break;
				}
			}
		}

		private void comboInterventionCode_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboInterventionCode.SelectedIndex>=0) {
				_comboToolTip.SetToolTip(comboInterventionCode,_listInterventionCodes[comboInterventionCode.SelectedIndex].Description);
			}
		}

		///<summary>Returns the radio button for tobacco status filter that is checked.</summary>
		private RadioButton GetSelectedStatusFilter() {
			if(radioUserStatuses.Checked) {
				return radioUserStatuses;
			}
			else if(radioNonUserStatuses.Checked) {
				return radioNonUserStatuses;
			}
			return radioAllStatuses;
		}

		private void butAssessed_Click(object sender,EventArgs e) {
			if(comboTobaccoStatus.SelectedIndex<0 || comboTobaccoStatus.SelectedIndex>=_listTobaccoStatuses.Count) {
				MsgBox.Show(this,"You must select a tobacco status.");
				return;
			}
			DateTime dateTEntered=PIn.DateT(textDateAssessed.Text);
			EhrMeasureEvent meas=new EhrMeasureEvent();
			meas.DateTEvent=dateTEntered;
			meas.EventType=EhrMeasureEventType.TobaccoUseAssessed;
			meas.PatNum=PatCur.PatNum;
			meas.CodeValueEvent=_listAssessmentCodes[comboAssessmentType.SelectedIndex].CodeValue;
			meas.CodeSystemEvent=_listAssessmentCodes[comboAssessmentType.SelectedIndex].CodeSystem;
			meas.CodeValueResult=_listTobaccoStatuses[comboTobaccoStatus.SelectedIndex].CodeValue;
			meas.CodeSystemResult="SNOMEDCT";//only allow SNOMEDCT codes for now.
			meas.MoreInfo="";
			EhrMeasureEvents.Insert(meas);
			comboTobaccoStatus.SelectedIndex=-1;
			FillGridAssessments();
		}

		private void butIntervention_Click(object sender,EventArgs e) {
			if(comboInterventionCode.SelectedIndex<0) {
				MsgBox.Show(this,"You must select an intervention code.");
				return;
			}
			EhrCode iCodeCur=_listInterventionCodes[comboInterventionCode.SelectedIndex];
			DateTime dateCur=PIn.Date(textDateIntervention.Text);
			if(iCodeCur.CodeSystem=="RXNORM" && !checkPatientDeclined.Checked) {//if patient declines the medication, enter as a declined intervention
				//codeVal will be RxCui of medication, see if it already exists in Medication table
				Medication medCur=Medications.GetMedicationFromDbByRxCui(PIn.Long(iCodeCur.CodeValue));
				if(medCur==null) {//no med with this RxCui, create one
					medCur=new Medication();
					Medications.Insert(medCur);//so that we will have the primary key
					medCur.GenericNum=medCur.MedicationNum;
					medCur.RxCui=PIn.Long(iCodeCur.CodeValue);
					medCur.MedName=RxNorms.GetDescByRxCui(iCodeCur.CodeValue);
					Medications.Update(medCur);
					Medications.RefreshCache();//refresh cache to include new medication
				}
				MedicationPat medPatCur=new MedicationPat();
				medPatCur.PatNum=PatCur.PatNum;
				medPatCur.ProvNum=PatCur.PriProv;
				medPatCur.MedicationNum=medCur.MedicationNum;
				medPatCur.RxCui=medCur.RxCui;
				medPatCur.DateStart=dateCur;
				using FormMedPat FormMP=new FormMedPat();
				FormMP.MedicationPatCur=medPatCur;
				FormMP.IsNew=true;
				FormMP.ShowDialog();
				if(FormMP.DialogResult!=DialogResult.OK) {
					return;
				}
				if(FormMP.MedicationPatCur.DateStart.Date<dateCur.AddMonths(-6).Date || FormMP.MedicationPatCur.DateStart.Date>dateCur.Date) {
					MsgBox.Show(this,"The medication order just entered is not within the 6 months prior to the date of this intervention.  You can modify the "
						+"date of the medication order in the patient's medical history section.");
				}
			}
			else {
				Intervention iCur=new Intervention();
				iCur.PatNum=PatCur.PatNum;
				iCur.ProvNum=PatCur.PriProv;
				iCur.DateEntry=dateCur;
				iCur.CodeValue=iCodeCur.CodeValue;
				iCur.CodeSystem=iCodeCur.CodeSystem;
				iCur.CodeSet=InterventionCodeSet.TobaccoCessation;
				iCur.IsPatDeclined=checkPatientDeclined.Checked;
				Interventions.Insert(iCur);
			}
			comboInterventionCode.SelectedIndex=-1;
			FillGridInterventions();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(comboSmokeStatus.SelectedIndex==0) {//None
				PatCur.SmokingSnoMed="";
			}
			else {
				PatCur.SmokingSnoMed=((SmokingSnoMed)comboSmokeStatus.SelectedIndex-1).ToString().Substring(1);
			}
			Patients.Update(PatCur,_patOld);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
