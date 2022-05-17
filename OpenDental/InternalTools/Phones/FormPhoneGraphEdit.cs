using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormPhoneGraphEdit:FormODBase {
		public PhoneGraph PhoneGraphCur;
		///<summary>For this employee today, ordered by time.</summary>
		public List<Schedule> ListSchedulesEmp;
		///<summary>For this provider today, ordered by time. Null if no prov.</summary>
		public List<Schedule> ListSchedulesProv;
		private PhoneEmpDefault _phoneEmpDefault;
		///<summary>If there is a matching provider for this emp, based on FName match.  Otherwise, 0.</summary>
		public long ProvNum;

		public FormPhoneGraphEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}
		
		private void FormPhoneGraphCreate_Load(object sender,System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Schedules,true)){
				checkIsGraphed.Enabled=false;
				checkPrescheduledOff.Enabled=false;
				checkAbsent.Enabled=false;
				groupOverrides.Enabled=false;
				groupProv.Enabled=false;
				textNote.Enabled=false;
				butCopy.Enabled=false;
				butCopyEmp.Enabled=false;
				butCopyOverride.Enabled=false;
				butClear.Enabled=false;
				butOK.Enabled=false;
				butDelete.Enabled=false;
			}
			textDateEntry.Text=PhoneGraphCur.DateEntry.ToShortDateString();
			_phoneEmpDefault=PhoneEmpDefaults.GetOne(PhoneGraphCur.EmployeeNum);//Because of where we open this from, it's guaranteed to be valid
			checkGraphDefault.Checked=_phoneEmpDefault.IsGraphed;
			checkIsGraphed.Checked=PhoneGraphCur.IsGraphed;
			checkPrescheduledOff.Checked=PhoneGraphCur.PreSchedOff;
			checkAbsent.Checked=PhoneGraphCur.Absent;
			textEmployee.Text=_phoneEmpDefault.EmpName;
			if(ListSchedulesEmp.Count>0){
				textSchedStart1.Text=ListSchedulesEmp[0].StartTime.ToShortTimeString();
				textSchedStop1.Text=ListSchedulesEmp[0].StopTime.ToShortTimeString();
			}
			if(ListSchedulesEmp.Count>1){
				textSchedStart2.Text=ListSchedulesEmp[1].StartTime.ToShortTimeString();
				textSchedStop2.Text=ListSchedulesEmp[1].StopTime.ToShortTimeString();
			}
			switch(PhoneGraphCur.PreSchedTimes){
				case EnumPresched.Presched:
					radioPrescheduled.Checked=true;
					break;
				case EnumPresched.ShortNotice:
					radioShortNotice.Checked=true;
					break;
				case EnumPresched.NotTracked:
					radioNotTracked.Checked=true;
					break;
			}
			if(PhoneGraphCur.DateTimeStart1.Year>1880){
				textStart1.Text=PhoneGraphCur.DateTimeStart1.ToShortTimeString();
			}
			if(PhoneGraphCur.DateTimeStop1.Year>1880){
				textStop1.Text=PhoneGraphCur.DateTimeStop1.ToShortTimeString();
			}
			if(PhoneGraphCur.DateTimeStart2.Year>1880){
				textStart2.Text=PhoneGraphCur.DateTimeStart2.ToShortTimeString();
			}
			if(PhoneGraphCur.DateTimeStop2.Year>1880){
				textStop2.Text=PhoneGraphCur.DateTimeStop2.ToShortTimeString();
			}
			if(ProvNum==0){
				textProvider.Text="none";
				groupProv.Enabled=false;
				butCopyEmp.Enabled=false;
				butCopyOverride.Enabled=false;
			}
			else{
				textProvider.Text=_phoneEmpDefault.EmpName;//because it's the same
				if(ListSchedulesProv.Count>0){
					textProvStart1.Text=ListSchedulesProv[0].StartTime.ToShortTimeString();
					textProvStop1.Text=ListSchedulesProv[0].StopTime.ToShortTimeString();
				}
				if(ListSchedulesProv.Count>1){
					textProvStart2.Text=ListSchedulesProv[1].StartTime.ToShortTimeString();
					textProvStop2.Text=ListSchedulesProv[1].StopTime.ToShortTimeString();
				}
			}
			textNote.Text=PhoneGraphCur.Note;
		}

		private void butCopy_Click(object sender, EventArgs e){
			textStart1.Text=textSchedStart1.Text;
			textStop1.Text=textSchedStop1.Text;
			textStart2.Text=textSchedStart2.Text;
			textStop2.Text=textSchedStop2.Text;
		}

		private void butClear_Click(object sender, EventArgs e){
			textStart1.Text="";
			textStop1.Text="";
			textStart2.Text="";
			textStop2.Text="";
		}

		private void butCopyEmp_Click(object sender, EventArgs e){
			textProvStart1.Text=textSchedStart1.Text;
			textProvStop1.Text=textSchedStop1.Text;
			textProvStart2.Text=textSchedStart2.Text;
			textProvStop2.Text=textSchedStop2.Text;
		}

		private void butCopyOverride_Click(object sender, EventArgs e){
			textProvStart1.Text=textStart1.Text;
			textProvStop1.Text=textStop1.Text;
			textProvStart2.Text=textStart2.Text;
			textProvStop2.Text=textStop2.Text;
		}

		private void checkPrescheduledOff_MouseDown(object sender, MouseEventArgs e){
			//autocheck is off
			if(checkPrescheduledOff.Checked){
				checkPrescheduledOff.Checked=false;
				return;
			}
			//trying to check the box
			checkPrescheduledOff.Checked=true;
			checkIsGraphed.Checked=false;
			textProvStart1.Text="";
			textProvStop1.Text="";
			textProvStart2.Text="";
			textProvStop2.Text="";
		}

		private void checkAbsent_MouseDown(object sender, MouseEventArgs e){
			//autocheck is off
			if(checkAbsent.Checked){
				checkAbsent.Checked=false;
				return;
			}
			//trying to check the box
			checkAbsent.Checked=true;
			checkIsGraphed.Checked=false;
			textProvStart1.Text="";
			textProvStop1.Text="";
			textProvStart2.Text="";
			textProvStop2.Text="";
			textStart1.Text="";
			textStop1.Text="";
			textStart2.Text="";
			textStop2.Text="";
		}

		private void butSign_Click(object sender, EventArgs e){
			if(textNote.Text!=""){
				textNote.Text+=" ";
			}
			textNote.Text+=Security.CurUser.UserName+" "+DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString();
		}

		private void butDelete_Click(object sender,System.EventArgs e) {
			if(PhoneGraphCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			PhoneGraphs.Delete(PhoneGraphCur.PhoneGraphNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(textStop1.Text!="" || textStart2.Text!="" || textStop2.Text!=""){
				if(textStart1.Text==""){
					MsgBox.Show(this,"If overriding any times, the first time must not be blank.");
					return;
				}
			}
			if(textProvStop1.Text!="" || textProvStart2.Text!="" || textProvStop2.Text!=""){
				if(textProvStart1.Text==""){
					MsgBox.Show(this,"If entering provider times, the first time must not be blank.");
					return;
				}
			}
			if((textStart1.Text!="" && textStop1.Text=="")
				|| (textStart2.Text!="" && textStop2.Text=="")
				|| (textStop2.Text!="" && textStart2.Text=="")
				|| (textProvStart1.Text!="" && textProvStop1.Text=="")
				|| (textProvStart2.Text!="" && textProvStop2.Text=="")
				|| (textProvStop2.Text!="" && textProvStart2.Text==""))
			{
				MsgBox.Show(this,"Times must be in pairs");
				return;
			}
			DateTime dateTimeStart1=DateTime.MinValue;
			DateTime dateTimeStop1=DateTime.MinValue;
			DateTime dateTimeStart2=DateTime.MinValue;
			DateTime dateTimeStop2=DateTime.MinValue;
			DateTime dateTimeProvStart1=DateTime.MinValue;
			DateTime dateTimeProvStop1=DateTime.MinValue;
			DateTime dateTimeProvStart2=DateTime.MinValue;
			DateTime dateTimeProvStop2=DateTime.MinValue;
			try{
				if(textStart1.Text!=""){
					dateTimeStart1=PhoneGraphCur.DateEntry+DateTime.Parse(textStart1.Text).TimeOfDay;
				}
				if(textStop1.Text!=""){
					dateTimeStop1=PhoneGraphCur.DateEntry+DateTime.Parse(textStop1.Text).TimeOfDay;
				}
				if(textStart2.Text!=""){
					dateTimeStart2=PhoneGraphCur.DateEntry+DateTime.Parse(textStart2.Text).TimeOfDay;
				}
				if(textStop2.Text!=""){
					dateTimeStop2=PhoneGraphCur.DateEntry+DateTime.Parse(textStop2.Text).TimeOfDay;
				}
				if(textProvStart1.Text!=""){
					dateTimeProvStart1=PhoneGraphCur.DateEntry+DateTime.Parse(textProvStart1.Text).TimeOfDay;
				}
				if(textProvStop1.Text!=""){
					dateTimeProvStop1=PhoneGraphCur.DateEntry+DateTime.Parse(textProvStop1.Text).TimeOfDay;
				}
				if(textProvStart2.Text!=""){
					dateTimeProvStart2=PhoneGraphCur.DateEntry+DateTime.Parse(textProvStart2.Text).TimeOfDay;
				}
				if(textProvStop2.Text!=""){
					dateTimeProvStop2=PhoneGraphCur.DateEntry+DateTime.Parse(textProvStop2.Text).TimeOfDay;
				}
			}
			catch{
				MsgBox.Show(this,"Please fix time formats.");
				return;
			}
			if((dateTimeStart1.Year>1880 && dateTimeStart1>dateTimeStop1)
				|| (dateTimeStart2.Year>1880 && dateTimeStop1>dateTimeStart2)
				|| (dateTimeStart2.Year>1880 && dateTimeStart2>dateTimeStop2)
				|| (dateTimeProvStart1.Year>1880 && dateTimeProvStart1>dateTimeProvStop1)
				|| (dateTimeProvStart2.Year>1880 && dateTimeProvStop1>dateTimeProvStart2)
				|| (dateTimeProvStart2.Year>1880 && dateTimeProvStart2>dateTimeProvStop2))
			{
				MsgBox.Show(this,"Times must be sequential.");
				return;
			}
			//end of validation
			//Date can't be changed
			PhoneGraphCur.IsGraphed=checkIsGraphed.Checked;
			PhoneGraphCur.PreSchedOff=checkPrescheduledOff.Checked;
			PhoneGraphCur.Absent=checkAbsent.Checked;
			if(radioPrescheduled.Checked){
				PhoneGraphCur.PreSchedTimes=EnumPresched.Presched;
			}
			if(radioShortNotice.Checked){
				PhoneGraphCur.PreSchedTimes=EnumPresched.ShortNotice;
			}
			if(radioNotTracked.Checked){
				PhoneGraphCur.PreSchedTimes=EnumPresched.NotTracked;
			}
			PhoneGraphCur.DateTimeStart1=dateTimeStart1;
			PhoneGraphCur.DateTimeStop1=dateTimeStop1;
			PhoneGraphCur.DateTimeStart2=dateTimeStart2;
			PhoneGraphCur.DateTimeStop2=dateTimeStop2;
			PhoneGraphCur.Note=textNote.Text;
			PhoneGraphs.InsertOrUpdate(PhoneGraphCur);
			if(ProvNum==0){
				DialogResult=DialogResult.OK;
				return;
			}
			//must test for changes, because we don't want to trigger an office-wide snych
			bool provChanged=false;
			if(ListSchedulesProv.Count==0 && textProvStart1.Text!=""){
				provChanged=true;
			}
			if(ListSchedulesProv.Count>0 && textProvStart1.Text==""){
				provChanged=true;//they will all get deleted
			}
			if(ListSchedulesProv.Count<2 && textProvStart2.Text!=""){
				provChanged=true;
			}
			if(ListSchedulesProv.Count>2){
				provChanged=true;//the extra ones not showing will get deleted
			}
			for(int i=0;i<ListSchedulesProv.Count;i++){
				if(i==0){
					if(ListSchedulesProv[i].StartTime != dateTimeProvStart1.TimeOfDay){
						provChanged=true;
					}
					if(ListSchedulesProv[i].StopTime != dateTimeProvStop1.TimeOfDay){
						provChanged=true;
					}
				}
				if(i==2){
					if(ListSchedulesProv[i].StartTime != dateTimeProvStart2.TimeOfDay){
						provChanged=true;
					}
					if(ListSchedulesProv[i].StopTime != dateTimeProvStop2.TimeOfDay){
						provChanged=true;
					}
				}
			}
			if(!provChanged){
				DialogResult=DialogResult.OK;
				return;
			}
			for(int i=0;i<ListSchedulesProv.Count;i++){
				Schedules.Delete(ListSchedulesProv[i]);//no signal
			}
			Schedule schedule;
			if(textProvStart1.Text!=""){
				schedule=new Schedule();
				schedule.ProvNum=ProvNum;
				schedule.SchedType=ScheduleType.Provider;
				schedule.SchedDate=PhoneGraphCur.DateEntry;
				schedule.Status=SchedStatus.Open;
				schedule.StartTime=dateTimeProvStart1.TimeOfDay;
				schedule.StopTime=dateTimeProvStop1.TimeOfDay;
				Schedules.Insert(schedule,validate:false,hasSignal:false);
			}
			if(textProvStart2.Text!=""){
				schedule=new Schedule();
				schedule.ProvNum=ProvNum;
				schedule.SchedType=ScheduleType.Provider;
				schedule.SchedDate=PhoneGraphCur.DateEntry;
				schedule.Status=SchedStatus.Open;
				schedule.StartTime=dateTimeProvStart2.TimeOfDay;
				schedule.StopTime=dateTimeProvStop2.TimeOfDay;
				Schedules.Insert(schedule,validate:false,hasSignal:false);
			}
			DataValid.SetInvalid(InvalidType.Employees);
			SecurityLogs.MakeLogEntry(Permissions.Schedules,0,"");
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			this.DialogResult=DialogResult.Cancel;
		}

		
	}
}
