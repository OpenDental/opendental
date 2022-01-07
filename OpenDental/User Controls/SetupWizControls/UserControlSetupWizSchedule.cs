using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental.User_Controls.SetupWizard {
	public partial class UserControlSetupWizSchedule:SetupWizControl {
		private int _provIdx;
		private List<Schedule> _listSchedules;
		private List<ScheduleProv> _listSchedProvs;

		public UserControlSetupWizSchedule() {
			InitializeComponent();
			this.OnControlDone += ControlDone;
			this.OnControlValidated+=ControlValidated;
		}

		private void UserControlSetupWizClinic_Load(object sender,EventArgs e) {
			List<Provider> listProvs = Providers.GetDeepCopy(true);
			_listSchedProvs=new List<ScheduleProv>();
			listProvs.ForEach(x => {
				_listSchedProvs.Add(new ScheduleProv() {
					Prov = x
				});
			});
			_provIdx=0;
			LoadProv(_provIdx);
			_listSchedules = Schedules.GetTwoYearPeriod(DateTime.Today.AddYears(-1));
		}

		private void LoadProv(int index) {
			butNextProv.Enabled = _provIdx < _listSchedProvs.Count - 1;
			butPrevProv.Enabled = _provIdx > 0;
			ScheduleProv schedProvCur = _listSchedProvs[_provIdx];
			if(_listSchedules != null && _listSchedules.Where(x => x.ProvNum == schedProvCur.Prov.ProvNum).Count() > 0) {
				checkMonday.Enabled=false;
				checkTuesday.Enabled=false;
				checkWednesday.Enabled=false;
				checkThursday.Enabled=false;
				checkFriday.Enabled=false;
				checkSaturday.Enabled=false;
				checkSunday.Enabled=false;
				labelProv.Text=schedProvCur.Prov.Abbr + " already has schedules setup. Please clicked 'Advanced Setup' to make changes to their schedule.";
				return;
			}
			labelProv.Text = "Let's set up schedules for " +schedProvCur.Prov.Abbr +".";
			if(schedProvCur.ListSchedMon.Count > 0) {
				checkMonday.Checked=true;
				gridMonday.Visible=true;
				butAddMonday.Visible=true;
				FillGrid(gridMonday,schedProvCur.ListSchedMon);
			}
			if(schedProvCur.ListSchedTue.Count > 0) {
				checkTuesday.Checked=true;
				gridTuesday.Visible=true;
				butAddTuesday.Visible=true;
				FillGrid(gridTuesday,schedProvCur.ListSchedTue);
			}
			if(schedProvCur.ListSchedWed.Count > 0) {
				checkWednesday.Checked=true;
				gridWednesday.Visible=true;
				butAddWednesday.Visible=true;
				FillGrid(gridWednesday,schedProvCur.ListSchedWed);
			}
			if(schedProvCur.ListSchedThu.Count > 0) {
				checkThursday.Checked=true;
				gridThursday.Visible=true;
				butAddThursday.Visible=true;
				FillGrid(gridThursday,schedProvCur.ListSchedThu);
			}
			if(schedProvCur.ListSchedFri.Count > 0) {
				checkFriday.Checked=true;
				gridFriday.Visible=true;
				butAddFriday.Visible=true;
				FillGrid(gridFriday,schedProvCur.ListSchedFri);
			}
			if(schedProvCur.ListSchedSat.Count > 0) {
				checkSaturday.Checked=true;
				gridSaturday.Visible=true;
				butAddSaturday.Visible=true;
				FillGrid(gridSaturday,schedProvCur.ListSchedSat);
			}
			if(schedProvCur.ListSchedSun.Count > 0) {
				checkSunday.Checked=true;
				gridSunday.Visible=true;
				butAddSunday.Visible=true;
				FillGrid(gridSunday,schedProvCur.ListSchedSun);
			}
		}

		private void FillGrid(GridOD grid, List<Schedule> listSched) {
			grid.BeginUpdate();
			grid.ListGridColumns.Clear();
			grid.ListGridColumns.Add(new GridColumn("StartTime",105));
			grid.ListGridColumns.Add(new GridColumn("EndTime",105));
			grid.ListGridRows.Clear();
			listSched.ForEach(x => {
				GridRow row = new GridRow();
				row.Cells.Add(x.StartTime.ToShortTimeString());
				row.Cells.Add(x.StopTime.ToShortTimeString());
				grid.ListGridRows.Add(row);
			});
			grid.EndUpdate();
		}

		private void AddSched(GridOD grid,List<Schedule> addList) {
			Schedule sched = new Schedule();
			using FormTimePick FormTP = new FormTimePick(false);
			FormTP.SelectedDTime=new DateTime(1,1,1,8,0,0);
			MsgBox.Show("FormSetupWizard","Enter a Start Time.");
			FormTP.ShowDialog();
			if(FormTP.DialogResult == DialogResult.OK) {
				sched.StartTime = FormTP.SelectedDTime.TimeOfDay;
				MsgBox.Show("FormSetupWizard","Enter an End Time.");
				using FormTimePick FormTP2 = new FormTimePick(false);
				FormTP2.SelectedDTime=new DateTime(1,1,1,17,0,0);
				FormTP2.ShowDialog();
				if(FormTP2.DialogResult == DialogResult.OK) {
					sched.StopTime = FormTP2.SelectedDTime.TimeOfDay;
				}
			}
			addList.Add(sched);
			FillGrid(grid,addList);
		}

		private new bool ControlValidated(object sender,EventArgs e) {
			StrIncomplete="In order for this section to be considered complete, you must set up at least one schedule for every provider.";
			List<Schedule> listSchedule = Schedules.GetTwoYearPeriod(DateTime.Today.AddYears(-1));
			if(listSchedule.Count == 0) {
				IsDone=false;
			}
			else {
				List<Provider> listProviders = Providers.GetWhere(x => x.IsNotPerson == false,true).ToList();
				foreach(Provider prov in listProviders) {
					if(!listSchedule.Select(x => x.ProvNum).Contains(prov.ProvNum)) {
						IsDone=false;
						break;
					}
				IsDone=true;
				}
			}
			return true;
		}

		private void ControlDone(object sender,EventArgs e) {
		}

		private void checkMonday_CheckedChanged(object sender,EventArgs e) {
			gridMonday.ListGridRows.Clear();
			groupMon.Visible=checkMonday.Checked;
		}

		private void checkTuesday_CheckedChanged(object sender,EventArgs e) {
			gridTuesday.ListGridRows.Clear();
			groupTue.Visible=checkTuesday.Checked;
		}

		private void checkWednesday_CheckedChanged(object sender,EventArgs e) {
			gridWednesday.ListGridRows.Clear();
			groupWed.Visible=checkWednesday.Checked;
		}

		private void checkThursday_CheckedChanged(object sender,EventArgs e) {
			gridThursday.ListGridRows.Clear();
			groupThu.Visible=checkThursday.Checked;
		}

		private void checkFriday_CheckedChanged(object sender,EventArgs e) {
			gridFriday.ListGridRows.Clear();
			groupFri.Visible=checkFriday.Checked;
		}

		private void checkSaturday_CheckedChanged(object sender,EventArgs e) {
			gridSaturday.ListGridRows.Clear();
			groupSat.Visible=checkSaturday.Checked;
		}

		private void checkSunday_CheckedChanged(object sender,EventArgs e) {
			gridSunday.ListGridRows.Clear();
			groupSun.Visible=checkSunday.Checked;
		}


		private void butAddMonday_Click(object sender,EventArgs e) {
			AddSched(gridMonday,_listSchedProvs[_provIdx].ListSchedMon);
		}

		private void butAddTuesday_Click(object sender,EventArgs e) {
			AddSched(gridTuesday,_listSchedProvs[_provIdx].ListSchedTue);
		}

		private void butAddWednesday_Click(object sender,EventArgs e) {
			AddSched(gridWednesday,_listSchedProvs[_provIdx].ListSchedWed);
		}

		private void butAddThursday_Click(object sender,EventArgs e) {
			AddSched(gridThursday,_listSchedProvs[_provIdx].ListSchedThu);
		}

		private void butAddFriday_Click(object sender,EventArgs e) {
			AddSched(gridFriday,_listSchedProvs[_provIdx].ListSchedFri);
		}

		private void butAddSaturday_Click(object sender,EventArgs e) {
			AddSched(gridSaturday,_listSchedProvs[_provIdx].ListSchedSat);
		}

		private void butAddSunday_Click(object sender,EventArgs e) {
			AddSched(gridSunday,_listSchedProvs[_provIdx].ListSchedSun);
		}

		private void butNextProv_Click(object sender,EventArgs e) {
			LoadProv(++_provIdx);
		}

		private void butPrevProv_Click(object sender,EventArgs e) {
			LoadProv(--_provIdx);
		}
	}



	class ScheduleProv {
		public Provider Prov;
		public List<Schedule> ListSchedMon= new List<Schedule>();
		public List<Schedule> ListSchedTue= new List<Schedule>();
		public List<Schedule> ListSchedWed= new List<Schedule>();
		public List<Schedule> ListSchedThu= new List<Schedule>();
		public List<Schedule> ListSchedFri= new List<Schedule>();
		public List<Schedule> ListSchedSat= new List<Schedule>();
		public List<Schedule> ListSchedSun = new List<Schedule>();
	}

}
