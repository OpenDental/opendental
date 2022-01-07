using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	///<summary></summary>
	public partial class FormScheduleEdit : FormODBase	{
		public Schedule SchedCur;
		///<summary>Filters the list of operatories available to the clinic passed in.  Set to 0 to show all operatories.  Also the clinic selected by
		///default for holidays and provider notes.</summary>
		public long ClinicNum;
		///<summary>All ops if clinics not enabled, otherwise all ops for ClinicNum.</summary>
		private List<Operatory> _listOps;
		///<summary>List of schedules for the day set from FormScheduleDayEdit filled with the filtered list of schedules for the day.
		///Used to ensure there is only one holiday schedule item per day/clinic, since this list has not been synced to the db yet.</summary>
		public List<Schedule> ListScheds;
		private bool _isHolidayOrNote;
		public List<long> ListProvNums;

		///<summary></summary>
		public FormScheduleEdit(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormScheduleEdit_Load(object sender, System.EventArgs e) {
			_isHolidayOrNote=(SchedCur.StartTime==TimeSpan.Zero && SchedCur.StopTime==TimeSpan.Zero);
			if(PrefC.HasClinicsEnabled) {
				if(ClinicNum==0) {
					Text+=" - "+Lan.g(this,"Headquarters");
				}
				else {
					string abbr=Clinics.GetAbbr(ClinicNum);
					if(!string.IsNullOrWhiteSpace(abbr)) {
						Text+=" - "+abbr;
					}
				}
				//if clinics are enabled and this is a holiday or practice note, set visible and fill the clinic combobox and private list of clinics
				if(_isHolidayOrNote && SchedCur.SchedType==ScheduleType.Practice) {
					comboClinic.Visible=true;//only visible for holidays and practice notes and only if clinics are enabled
					comboClinic.SelectedClinicNum=Clinics.ClinicNum;
				}
				else {
					comboClinic.Visible=false;
				}
			}
			textNote.Text=SchedCur.Note;
			if(_isHolidayOrNote) {
				comboStart.Visible=false;
				labelStart.Visible=false;
				comboStop.Visible=false;
				labelStop.Visible=false;
				listBoxOps.Visible=false;
				labelOps.Visible=false;
				textNote.Select();
				return;
			}
			//from here on, NOT a practice note or holiday
			DateTime time;
			for(int i=0;i<24;i++) {
				time=DateTime.Today+TimeSpan.FromHours(7)+TimeSpan.FromMinutes(30*i);
				comboStart.Items.Add(time.ToShortTimeString());
				comboStop.Items.Add(time.ToShortTimeString());
			}
			comboStart.Text=SchedCur.StartTime.ToShortTimeString();
			comboStop.Text=SchedCur.StopTime.ToShortTimeString();
			listBoxOps.Items.Add(Lan.g(this,"not specified"));
			//filter list if using clinics and if a clinic filter was passed in to only ops assigned to the specified clinic, otherwise all non-hidden ops
			_listOps=Operatories.GetDeepCopy(true);
			if(PrefC.HasClinicsEnabled && ClinicNum>0) {
				_listOps.RemoveAll(x => x.ClinicNum!=ClinicNum);
			}
			for(int i=0;i<_listOps.Count;i++) {
				listBoxOps.Items.Add(_listOps[i].OpName);
				if(SchedCur.Ops.Contains(_listOps[i].OperatoryNum)) {
					listBoxOps.SetSelected(listBoxOps.Items.Count-1);
				}
			}
			listBoxOps.SetSelected(0,listBoxOps.SelectedIndices.Count==0);//select 'not specified' if no ops were selected in the loop
			comboStart.Select();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			#region Validation
			DateTime startDateT=DateTime.MinValue;
			DateTime stopDateT=DateTime.MinValue;
			if(!_isHolidayOrNote) {
				if(listBoxOps.SelectedIndices.Count==0){
					MsgBox.Show(this,"Please select ops first.");
					return;
				}
				if(listBoxOps.SelectedIndices.Count>1 && listBoxOps.SelectedIndices.Contains(0)){
					MsgBox.Show(this,"Invalid selection of ops.");
					return;
				}
				startDateT=PIn.DateT(comboStart.Text);
				stopDateT=PIn.DateT(comboStop.Text);
				if(startDateT==DateTime.MinValue || stopDateT==DateTime.MinValue) {
					MsgBox.Show(this,"Incorrect time format");
					return;
				}
				if(startDateT>stopDateT) {
					MsgBox.Show(this,"Stop time must be later than start time.");
					return;
				}
				List<Schedule> listProvSchedsOnly=ListScheds.FindAll(x=>x.SchedType==ScheduleType.Provider);
				List<long> listSelectedOps=new List<long>();
				if(listBoxOps.SelectedIndices.Contains(0)) {
					//not specified, so empty list
				}
				else {
					for(int i=0;i<listBoxOps.SelectedIndices.Count;i++) {
						listSelectedOps.Add(_listOps[listBoxOps.SelectedIndices[i]-1].OperatoryNum);
					}
				}
				SchedCur.Ops=listSelectedOps.ToList();//deep copy of list. (because it is value type.)
				SchedCur.StartTime=startDateT.TimeOfDay;
				SchedCur.StopTime=stopDateT.TimeOfDay;
				List<long> listProvsOverlap;
				//====================Pre-Emptive Overlaps====================
				//Because this window is explicitly designed for one start and one stop time we know that there will be overlapping if multiple providers are 
				//selected with at least one specific operatory selected.
				if(ListProvNums.Count > 1 && listSelectedOps.Count > 0) {
					listProvsOverlap=ListProvNums.Distinct().ToList();
				}
				else {//Go see if there is going to be overlapping issues with this new schedule.
					listProvsOverlap=Schedules.GetOverlappingSchedProvNums(ListProvNums,SchedCur,listProvSchedsOnly,listSelectedOps);
				}
				if(listProvsOverlap.Count>0 && MessageBox.Show(Lan.g(this,"Overlapping provider schedules detected, would you like to continue anyway?")+"\r\n"+Lan.g(this,"Providers affected")+":\r\n  "
					+string.Join("\r\n  ",listProvsOverlap.Select(x=>Providers.GetLongDesc(x))),"",MessageBoxButtons.YesNo)!=DialogResult.Yes) 
				{ 
					return;
				}
			}
			else if(SchedCur.Status!=SchedStatus.Holiday && textNote.Text=="") {//don't allow blank schedule notes
				MsgBox.Show(this,"Please enter a note first.");
				return;
			}
			long clinicNum=0;
			if(_isHolidayOrNote && SchedCur.SchedType==ScheduleType.Practice && PrefC.HasClinicsEnabled) {//prov notes do not have a clinic
				clinicNum=comboClinic.SelectedClinicNum;
				if(SchedCur.Status==SchedStatus.Holiday) {//duplicate holiday check
					List<Schedule> listScheds=ListScheds.FindAll(x => x.SchedType==ScheduleType.Practice && x.Status==SchedStatus.Holiday);//scheds in local list
					listScheds.AddRange(Schedules.GetAllForDateAndType(SchedCur.SchedDate,ScheduleType.Practice)
						.FindAll(x => x.ScheduleNum!=SchedCur.ScheduleNum
							&& x.Status==SchedStatus.Holiday
							&& listScheds.All(y => y.ScheduleNum!=x.ScheduleNum)));//add any in db that aren't in local list
					if(listScheds.Any(x => x.ClinicNum==0 || x.ClinicNum==clinicNum)//already a holiday for HQ in db or duplicate holiday for a clinic
						|| (clinicNum==0 && listScheds.Count>0))//OR trying to create a HQ holiday when a clinic already has one for this day
					{
						MsgBox.Show(this,"There is already a Holiday for the practice or clinic on this date.");
						return;
					}
				}
			}
			#endregion Validation
			#region Set Schedule Fields
      SchedCur.Note=textNote.Text;
			SchedCur.Ops=new List<long>();
			if(listBoxOps.SelectedIndices.Count>0 && !listBoxOps.SelectedIndices.Contains(0)) {
				for(int i=0;i<listBoxOps.SelectedIndices.Count;i++) {
					SchedCur.Ops.Add(_listOps[listBoxOps.SelectedIndices[i]-1].OperatoryNum);
				}
			}
			SchedCur.ClinicNum=clinicNum;//0 if HQ selected or clinics not enabled or not a holiday or practice note
			#endregion Set Schedule Fields
			DialogResult=DialogResult.OK;		  
    }

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}






