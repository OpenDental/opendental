using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	///<summary></summary>
	public partial class FormScheduleEdit : FormODBase	{
		public Schedule ScheduleCur;
		///<summary>Filters the list of operatories available to the clinic passed in.  Set to 0 to show all operatories.  Also the clinic selected by
		///default for holidays and provider notes.</summary>
		public long ClinicNum;
		///<summary>All ops if clinics not enabled, otherwise all ops for ClinicNum.</summary>
		private List<Operatory> _listOperatories;
		///<summary>List of schedules for the day set from FormScheduleDayEdit filled with the filtered list of schedules for the day.
		///Used to ensure there is only one holiday schedule item per day/clinic, since this list has not been synced to the db yet.</summary>
		public List<Schedule> ListSchedules;
		private bool _isHolidayOrNote;
		public List<long> ListProvNums;

		///<summary></summary>
		public FormScheduleEdit(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormScheduleEdit_Load(object sender, System.EventArgs e) {
			_isHolidayOrNote=(ScheduleCur.StartTime==TimeSpan.Zero && ScheduleCur.StopTime==TimeSpan.Zero);
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
				if(_isHolidayOrNote && ScheduleCur.SchedType==ScheduleType.Practice) {
					comboClinic.Visible=true;//only visible for holidays and practice notes and only if clinics are enabled
					comboClinic.ClinicNumSelected=Clinics.ClinicNum;
				}
				else {
					comboClinic.Visible=false;
				}
			}
			textNote.Text=ScheduleCur.Note;
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
			DateTime dateTime;
			for(int i=0;i<24;i++) {
				dateTime=DateTime.Today+TimeSpan.FromHours(7)+TimeSpan.FromMinutes(30*i);
				comboStart.Items.Add(dateTime.ToShortTimeString());
				comboStop.Items.Add(dateTime.ToShortTimeString());
			}
			comboStart.Text=ScheduleCur.StartTime.ToShortTimeString();
			comboStop.Text=ScheduleCur.StopTime.ToShortTimeString();
			listBoxOps.Items.Add(Lan.g(this,"not specified"));
			//filter list if using clinics and if a clinic filter was passed in to only ops assigned to the specified clinic, otherwise all non-hidden ops
			_listOperatories=Operatories.GetDeepCopy(true);
			if(PrefC.HasClinicsEnabled && ClinicNum>0) {
				_listOperatories.RemoveAll(x => x.ClinicNum!=ClinicNum);
			}
			for(int i=0;i<_listOperatories.Count;i++) {
				listBoxOps.Items.Add(_listOperatories[i].OpName);
				if(ScheduleCur.Ops.Contains(_listOperatories[i].OperatoryNum)) {
					listBoxOps.SetSelected(listBoxOps.Items.Count-1);
				}
			}
			listBoxOps.SetSelected(0,listBoxOps.SelectedIndices.Count==0);//select 'not specified' if no ops were selected in the loop
			comboStart.Select();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			#region Validation
			DateTime dateTStart=DateTime.MinValue;
			DateTime dateTStop=DateTime.MinValue;
			if(!_isHolidayOrNote) {
				if(listBoxOps.SelectedIndices.Count==0){
					MsgBox.Show(this,"Please select ops first.");
					return;
				}
				if(listBoxOps.SelectedIndices.Count>1 && listBoxOps.SelectedIndices.Contains(0)){
					MsgBox.Show(this,"Invalid selection of ops.");
					return;
				}
				dateTStart=PIn.DateT(comboStart.Text);
				dateTStop=PIn.DateT(comboStop.Text);
				if(dateTStart==DateTime.MinValue || dateTStop==DateTime.MinValue) {
					MsgBox.Show(this,"Incorrect time format");
					return;
				}
				if(dateTStart>dateTStop) {
					MsgBox.Show(this,"Stop time must be later than start time.");
					return;
				}
				List<Schedule> listSchedulesProvOnly=ListSchedules.FindAll(x=>x.SchedType==ScheduleType.Provider);
				List<long> listOperatoryNumsSelected=new List<long>();
				if(listBoxOps.SelectedIndices.Contains(0)) {
					//not specified, so empty list
				}
				else {
					for(int i=0;i<listBoxOps.SelectedIndices.Count;i++) {
						listOperatoryNumsSelected.Add(_listOperatories[listBoxOps.SelectedIndices[i]-1].OperatoryNum);
					}
				}
				ScheduleCur.Ops=listOperatoryNumsSelected.ToList();//deep copy of list. (because it is value type.)
				ScheduleCur.StartTime=dateTStart.TimeOfDay;
				ScheduleCur.StopTime=dateTStop.TimeOfDay;
				List<long> listProvNumsOverlap;
				//====================Pre-Emptive Overlaps====================
				//Because this window is explicitly designed for one start and one stop time we know that there will be overlapping if multiple providers are 
				//selected with at least one specific operatory selected.
				if(ListProvNums.Count > 1 && listOperatoryNumsSelected.Count > 0) {
					listProvNumsOverlap=ListProvNums.Distinct().ToList();
				}
				else {//Go see if there is going to be overlapping issues with this new schedule.
					listProvNumsOverlap=Schedules.GetOverlappingSchedProvNums(ListProvNums,ScheduleCur,listSchedulesProvOnly,listOperatoryNumsSelected);
				}
				if(listProvNumsOverlap.Count>0 && MessageBox.Show(Lan.g(this,"Overlapping provider schedules detected, would you like to continue anyway?")+"\r\n"+Lan.g(this,"Providers affected")+":\r\n  "
					+string.Join("\r\n  ",listProvNumsOverlap.Select(x=>Providers.GetLongDesc(x))),"",MessageBoxButtons.YesNo)!=DialogResult.Yes) 
				{ 
					return;
				}
			}
			else if(ScheduleCur.Status!=SchedStatus.Holiday && textNote.Text=="") {//don't allow blank schedule notes
				MsgBox.Show(this,"Please enter a note first.");
				return;
			}
			long clinicNum=0;
			if(_isHolidayOrNote && ScheduleCur.SchedType==ScheduleType.Practice && PrefC.HasClinicsEnabled) {//prov notes do not have a clinic
				clinicNum=comboClinic.ClinicNumSelected;
				if(ScheduleCur.Status==SchedStatus.Holiday) {//duplicate holiday check
					List<Schedule> listSchedules=ListSchedules.FindAll(x => x.SchedType==ScheduleType.Practice && x.Status==SchedStatus.Holiday);//scheds in local list
					listSchedules.AddRange(Schedules.GetAllForDateAndType(ScheduleCur.SchedDate,ScheduleType.Practice)
						.FindAll(x => x.ScheduleNum!=ScheduleCur.ScheduleNum
							&& x.Status==SchedStatus.Holiday
							&& listSchedules.All(y => y.ScheduleNum!=x.ScheduleNum)));//add any in db that aren't in local list
					if(listSchedules.Any(x => x.ClinicNum==0 || x.ClinicNum==clinicNum)//already a holiday for HQ in db or duplicate holiday for a clinic
						|| (clinicNum==0 && listSchedules.Count>0))//OR trying to create a HQ holiday when a clinic already has one for this day
					{
						MsgBox.Show(this,"There is already a Holiday for the practice or clinic on this date.");
						return;
					}
				}
			}
			#endregion Validation
			#region Set Schedule Fields
      ScheduleCur.Note=textNote.Text;
			ScheduleCur.Ops=new List<long>();
			if(listBoxOps.SelectedIndices.Count>0 && !listBoxOps.SelectedIndices.Contains(0)) {
				for(int i=0;i<listBoxOps.SelectedIndices.Count;i++) {
					ScheduleCur.Ops.Add(_listOperatories[listBoxOps.SelectedIndices[i]-1].OperatoryNum);
				}
			}
			ScheduleCur.ClinicNum=clinicNum;//0 if HQ selected or clinics not enabled or not a holiday or practice note
			#endregion Set Schedule Fields
			DialogResult=DialogResult.OK;		  
    }

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}






