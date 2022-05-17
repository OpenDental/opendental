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
		private Schedule _schedCur;
		private long _clinicNum;
		private List<Operatory> _listOps;
		private List<Def> _listBlockoutCatDefs;

		///<summary>Setting clinicNum to 0 will show all operatories, otherwise only operatories for the clinic passed in will show.  
		///If a list of defs is passed in it will fill the blockout type select box.  If no list is passed in, it will show all defs.</summary>
		public FormScheduleBlockEdit(Schedule schedCur, long clinicNum, List<Def> listDefsToShow=null) {
			InitializeComponent();
			InitializeLayoutManager();
			_schedCur=schedCur;
			_clinicNum=clinicNum;
			_listBlockoutCatDefs=listDefsToShow;
			Lan.F(this);
		}

		private void FormScheduleBlockEdit_Load(object sender,System.EventArgs e) {
			listType.Items.Clear();
			//This list will be null if there isn't a passed in list.  We pass in lists if we want to show a special modified list.
			if(_listBlockoutCatDefs==null) {
				_listBlockoutCatDefs=Defs.GetDefsForCategory(DefCat.BlockoutTypes,true);
			}
			for(int i=0;i<_listBlockoutCatDefs.Count;i++){
				listType.Items.Add(_listBlockoutCatDefs[i].ItemName);
				if(_schedCur.BlockoutType==_listBlockoutCatDefs[i].DefNum){
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
			List<Operatory> listOpsShort=Operatories.GetDeepCopy(true);
			_listOps=new List<Operatory>();
			//Get all op nums for this schedule, as _schedCur.Ops may not actually contain all of them.
			List<long> listSchedOpNums=ScheduleOps.GetForSched(_schedCur.ScheduleNum).Select(x=>x.OperatoryNum).ToList();
			for(int i=0;i<listOpsShort.Count;i++) {
				if(PrefC.HasClinicsEnabled && _clinicNum!=0) {//Using clinics and a clinic filter was passed in.
					if(listOpsShort[i].ClinicNum!=_clinicNum) {
						continue;
					}
				}
				listOp.Items.Add(listOpsShort[i].OpName);
				_listOps.Add(listOpsShort[i]);
				if(_schedCur.Ops.Contains(listOpsShort[i].OperatoryNum) || listSchedOpNums.Contains(listOpsShort[i].OperatoryNum)) {
					listOp.SetSelected(listOp.Items.Count-1);
				}
			}
			DateTime time;
			for(int i=0;i<24;i++){
				time=DateTime.Today+TimeSpan.FromHours(7)+TimeSpan.FromMinutes(30*i);
				comboStart.Items.Add(time.ToShortTimeString());
				comboStop.Items.Add(time.ToShortTimeString());
			}
			comboStart.Text=_schedCur.StartTime.ToShortTimeString();
			comboStop.Text=_schedCur.StopTime.ToShortTimeString();
			textNote.Text=_schedCur.Note;
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
        Schedules.Delete(_schedCur,true);
				Schedules.BlockoutLogHelper(BlockoutAction.Delete,_schedCur);
			}
      DialogResult=DialogResult.Cancel;
		}

    private void butOK_Click(object sender, System.EventArgs e) { 
			if(listOp.SelectedIndices.Count==0){
				MsgBox.Show(this,"Please select at least one operatory first.");
				return;
			}
		  try{
				_schedCur.StartTime=DateTime.Parse(comboStart.Text).TimeOfDay;
				_schedCur.StopTime=DateTime.Parse(comboStop.Text).TimeOfDay;
			}
			catch{
				MsgBox.Show(this,"Incorrect time format");
				return;
			}
      _schedCur.Note=textNote.Text;
			_schedCur.BlockoutType=_listBlockoutCatDefs[listType.SelectedIndex].DefNum;
			_schedCur.Ops=new List<long>();
			for(int i=0;i<listOp.SelectedIndices.Count;i++){
				_schedCur.Ops.Add(_listOps[listOp.SelectedIndices[i]].OperatoryNum);
			}
			List<Schedule> listOverlapSchedules;
			if(Schedules.Overlaps(_schedCur,out listOverlapSchedules)) {
				if(!PrefC.GetBool(PrefName.ReplaceExistingBlockout) || !Schedules.IsAppointmentBlocking(_schedCur.BlockoutType)) {
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
			try{
				if(IsNew) {
					Schedules.Insert(_schedCur,true);
					Schedules.BlockoutLogHelper(BlockoutAction.Create,_schedCur);
				}
				else {
					Schedules.Update(_schedCur);
					Schedules.BlockoutLogHelper(BlockoutAction.Edit,_schedCur);
				}
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






