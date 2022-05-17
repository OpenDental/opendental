using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>Only used for editing smoking documentation.</summary>
	public partial class FormEhrMeasureEventEdit:FormODBase {
		private EhrMeasureEvent _measureEventCur;
		public string MeasureDescript;

		public FormEhrMeasureEventEdit(EhrMeasureEvent measureEventCur) {
			InitializeComponent();
			InitializeLayoutManager();
			_measureEventCur=measureEventCur;
		}

		private void FormEhrMeasureEventEdit_Load(object sender,EventArgs e) {
			textDateTime.Text=_measureEventCur.DateTEvent.ToString();
			Patient patCur=Patients.GetPat(_measureEventCur.PatNum);
			if(patCur!=null) {
				textPatient.Text=patCur.GetNameFL();
			}
			if(!String.IsNullOrWhiteSpace(MeasureDescript)) {
				labelMoreInfo.Text=MeasureDescript;
			}
			if(_measureEventCur.EventType==EhrMeasureEventType.TobaccoUseAssessed) {
				Loinc lCur=Loincs.GetByCode(_measureEventCur.CodeValueEvent);//TobaccoUseAssessed events can be one of three types, all LOINC codes
				if(lCur!=null) {
					textType.Text=lCur.NameLongCommon;//Example: History of tobacco use Narrative
				}
				Snomed sCur=Snomeds.GetByCode(_measureEventCur.CodeValueResult);//TobaccoUseAssessed results can be any SNOMEDCT code, we recommend one of 8 codes, but the CQM measure allows 54 codes and we let the user select any SNOMEDCT they want
				if(sCur!=null) {
					textResult.Text=sCur.Description;//Examples: Non-smoker (finding) or Smoker (finding)
				}
				//only visible if event is a tobacco use assessment
				textTobaccoDesireToQuit.Visible=true;
				textTobaccoDuration.Visible=true;
				textTobaccoStartDate.Visible=true;
				labelTobaccoDesireToQuit.Visible=true;
				labelTobaccoDesireScale.Visible=true;
				labelTobaccoStartDate.Visible=true;
				textTobaccoDesireToQuit.Text=_measureEventCur.TobaccoCessationDesire.ToString();
				if(_measureEventCur.DateStartTobacco.Year>=1880) {
					textTobaccoStartDate.Text=_measureEventCur.DateStartTobacco.ToShortDateString();
				}
				CalcTobaccoDuration();
			}
			else {
				//Currently, the TobaccoUseAssessed events are the only ones that can be deleted.
				butDelete.Enabled=false;
			}
			if(textType.Text==""){//if not set by LOINC name above, then either not a TobaccoUseAssessed event or the code was not in the LOINC table, fill with EventType
				textType.Text=_measureEventCur.EventType.ToString();
			}
			textMoreInfo.Text=_measureEventCur.MoreInfo;
		}

		public void CalcTobaccoDuration() {
			textTobaccoDuration.Text="";
			if(!textTobaccoStartDate.IsValid()) {
				return;
			}
			DateTime startDate=PIn.Date(textTobaccoStartDate.Text);
			if(startDate>DateTime.Today || startDate.Year<1880) {
				return;
			}
			int years=DateTime.Now.Year-startDate.Year;
			int months=0;
			if(startDate.Month<DateTime.Now.Month) {//startdate anniversary was in a previous
				months=DateTime.Now.Month-startDate.Month;
				if(DateTime.Now.Day>startDate.Day) {//start month is before current month, and start day is before current day, don't count current month
					months--;
				}
			}
			else if(startDate.Month==DateTime.Now.Month) {//startdate anniversary this month
				if(startDate.Day>DateTime.Now.Day) {//start date anniversary hasn't happened this month, subtract a year and months=11
					years--;
					months=11;
				}
			}
			else {//startdate anniversary later in the year
				years--;
				months=12-(startDate.Month-DateTime.Now.Month);
				if(startDate.Day>DateTime.Now.Day) {
					months--;//haven't reached the day of the month of the startdate, don't count the current month yet
				}
			}
			textTobaccoDuration.Text=years.ToString()+"y "+months.ToString()+"m";
		}

		private void textTobaccoStartDate_Validated(object sender,EventArgs e) {
			CalcTobaccoDuration();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
				return;
			}
			string logEntry=Lan.g(this,"Ehr Measure Event was deleted.")+"  "
				+Lan.g(this,"Date")+": "+PIn.DateT(textDateTime.Text)+"  "
				+Lan.g(this,"Type")+": "+_measureEventCur.EventType.ToString()+"  "
				+Lan.g(this,"Patient")+": "+textPatient.Text;
			SecurityLogs.MakeLogEntry(Permissions.EhrMeasureEventEdit,_measureEventCur.PatNum,logEntry);
			EhrMeasureEvents.Delete(_measureEventCur.EhrMeasureEventNum);
			DialogResult=DialogResult.Cancel;
		}

		private void butOK_Click(object sender,EventArgs e) {
			//inserts never happen here.  Only updates.
			DateTime dateTEvent=PIn.DateT(textDateTime.Text);
			if(dateTEvent==DateTime.MinValue) {
				MsgBox.Show(this,"Please enter a valid date time.");//because this must always be valid
				return;
			}
			if(textTobaccoStartDate.Visible //only visible for tobacco use assessments
				&& !textTobaccoStartDate.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textTobaccoDesireToQuit.Visible //only visible for tobacco use assessments
				&& !textTobaccoDesireToQuit.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			List<string> listLogEdits=new List<string>();
			if(_measureEventCur.MoreInfo!=textMoreInfo.Text) {
				listLogEdits.Add(Lan.g(this,"More Info was changed."));
				_measureEventCur.MoreInfo=textMoreInfo.Text;
			}
			if(_measureEventCur.DateTEvent!=dateTEvent) {
				listLogEdits.Add(Lan.g(this,"Date was changed from")+": "+_measureEventCur.DateTEvent.ToString()+" "+Lan.g(this,"to")+": "+dateTEvent.ToString()+".");
				_measureEventCur.DateTEvent=dateTEvent;
			}
			if(textTobaccoStartDate.Visible && textTobaccoDesireToQuit.Visible) {
				_measureEventCur.DateStartTobacco=PIn.Date(textTobaccoStartDate.Text);
				_measureEventCur.TobaccoCessationDesire=PIn.Byte(textTobaccoDesireToQuit.Text);
			}
			if(listLogEdits.Count>0) {
				listLogEdits.Insert(0,Lan.g(this,"EHR Measure Event was edited."));
				SecurityLogs.MakeLogEntry(Permissions.EhrMeasureEventEdit,_measureEventCur.PatNum,string.Join("  ",listLogEdits));
			}
			if(_measureEventCur.IsNew) {//should never happen, only updates happen here
				EhrMeasureEvents.Insert(_measureEventCur);
			}
			else {
				EhrMeasureEvents.Update(_measureEventCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
