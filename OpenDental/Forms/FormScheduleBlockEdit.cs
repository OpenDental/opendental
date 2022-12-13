using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public partial class FormScheduleBlockEdit : FormODBase	{
		///<summary></summary>
		public bool IsNew;
		private Schedule _schedule;
		private long _clinicNum;
		private List<Operatory> _listOperatories;
		private List<Def> _listDefsBlockoutCategories;

		///<summary>Setting clinicNum to 0 will show all operatories, otherwise only operatories for the clinic passed in will show.  
		///If a list of defs is passed in it will fill the blockout type select box.  If no list is passed in, it will show all defs.</summary>
		public FormScheduleBlockEdit(Schedule schedule, long clinicNum, List<Def> listDefsToShow=null) {
			InitializeComponent();
			InitializeLayoutManager();
			_schedule=schedule;
			_clinicNum=clinicNum;
			_listDefsBlockoutCategories=listDefsToShow;
			Lan.F(this);
		}

		private void FormScheduleBlockEdit_Load(object sender,System.EventArgs e) {
			listType.Items.Clear();
			//This list will be null if there isn't a passed in list.  We pass in lists if we want to show a special modified list.
			if(_listDefsBlockoutCategories==null) {
				_listDefsBlockoutCategories=Defs.GetDefsForCategory(DefCat.BlockoutTypes,true);
			}
			for(int i=0;i<_listDefsBlockoutCategories.Count;i++){
				listType.Items.Add(_listDefsBlockoutCategories[i].ItemName);
				if(_schedule.BlockoutType==_listDefsBlockoutCategories[i].DefNum){
					listType.SelectedIndex=i;
				}
			}
			if(listType.Items.Count==0){
				MsgBox.Show(this,"You must setup blockout types first in Setup-Definitions.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(listType.SelectedIndex==-1){
				listType.SelectedIndex=0;
			}
			listOp.Items.Clear();
			//Filter clinics by the clinic passed in.
			List<Operatory> listOperatories=Operatories.GetDeepCopy(true);
			_listOperatories=new List<Operatory>();
			//Get all op nums for this schedule, as _schedCur.Ops may not actually contain all of them.
			List<long> listSchedOpNums=ScheduleOps.GetForSched(_schedule.ScheduleNum).Select(x=>x.OperatoryNum).ToList();
			for(int i=0;i<listOperatories.Count;i++) {
				if(PrefC.HasClinicsEnabled && _clinicNum!=0) {//Using clinics and a clinic filter was passed in.
					if(listOperatories[i].ClinicNum!=_clinicNum) {
						continue;
					}
				}
				listOp.Items.Add(listOperatories[i].OpName);
				_listOperatories.Add(listOperatories[i]);
				if(_schedule.Ops.Contains(listOperatories[i].OperatoryNum) || listSchedOpNums.Contains(listOperatories[i].OperatoryNum)) {
					listOp.SetSelected(listOp.Items.Count-1);
				}
			}
			DateTime time;
			for(int i=0;i<24;i++){
				time=DateTime.Today+TimeSpan.FromHours(7)+TimeSpan.FromMinutes(30*i);
				comboStart.Items.Add(time.ToShortTimeString());
				comboStop.Items.Add(time.ToShortTimeString());
			}
			comboStart.Text=_schedule.StartTime.ToShortTimeString();
			comboStop.Text=_schedule.StopTime.ToShortTimeString();
			textNote.Text=_schedule.Note;
			comboStart.Select();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(MessageBox.Show(Lan.g(this,"Delete Blockout?"),"",MessageBoxButtons.OKCancel)!=DialogResult.OK){
			  return;   
			}
			if(IsNew){
				DialogResult=DialogResult.Cancel; 
			}
			else{ 
				Schedules.Delete(_schedule,true);
				Schedules.BlockoutLogHelper(BlockoutAction.Delete,_schedule);
			}
			DialogResult=DialogResult.Cancel;
		}

		private void butOK_Click(object sender, System.EventArgs e) { 
			if(listOp.SelectedIndices.Count==0){
				MsgBox.Show(this,"Please select at least one operatory first.");
				return;
			}
			try{
				_schedule.StartTime=DateTime.Parse(comboStart.Text).TimeOfDay;
				_schedule.StopTime=DateTime.Parse(comboStop.Text).TimeOfDay;
			}
			catch{
				MsgBox.Show(this,"Incorrect time format");
				return;
			}
			_schedule.Note=textNote.Text;
			_schedule.BlockoutType=_listDefsBlockoutCategories[listType.SelectedIndex].DefNum;
			_schedule.Ops=new List<long>();
			for(int i=0;i<listOp.SelectedIndices.Count;i++){
				_schedule.Ops.Add(_listOperatories[listOp.SelectedIndices[i]].OperatoryNum);
			}
			List<Schedule> listOverlapSchedules;
			if(Schedules.Overlaps(_schedule,out listOverlapSchedules)) {
				if(!PrefC.GetBool(PrefName.ReplaceExistingBlockout) || !Schedules.IsAppointmentBlocking(_schedule.BlockoutType)) {
					MsgBox.Show(this,"Blockouts not allowed to overlap.");
					return;
				}
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Creating this blockout will cause blockouts to overlap. Continuing will delete the existing "
					+"blockout(s). Continue?")) 
				{
					return;
				}
				Schedules.DeleteMany(listOverlapSchedules.Select(x => x.ScheduleNum).ToList());
			}
			if(IsNew) {
				try {
					Schedules.Insert(_schedule,true);
					Schedules.BlockoutLogHelper(BlockoutAction.Create,_schedule);
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					return;
				}
				DialogResult=DialogResult.OK;	
				return;
			}
			try {
				Schedules.Update(_schedule);
				Schedules.BlockoutLogHelper(BlockoutAction.Edit,_schedule);
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;		  
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}   

	}
}






