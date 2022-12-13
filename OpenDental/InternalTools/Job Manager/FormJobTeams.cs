using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormJobTeams:FormODBase {
		#region Private variables
		///<summary>Old copy of teams to sync against.</summary>
		private List<JobTeam> _listJobTeamsOld;
		///<summary>New copy of teams to make changes and sync to DB when form closes.</summary>
		private List<JobTeam> _listJobTeams;
		///<summary>Old copy of team members to sync against.</summary>
		private List<JobTeamUser> _listJobTeamUsersOld;
		///<summary>New copy of team members to make changes and sync to DB when form closes.</summary>
		private List<JobTeamUser> _listJobTeamUsers;
		///<summary>List of all the users in Open Dental that are engineers.</summary>
		private List<Userod> _listUsers;
		///<summary>List of all the users who are associated to the currently selected JobTeam (not hidden).</summary>
		private List<JobTeamUser> _listJobTeamUsersCur;
		#endregion

		public FormJobTeams() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormJobTeams_Load(object sender,EventArgs e) {
			_listJobTeamsOld=JobTeams.GetDeepCopy();
			_listJobTeams=JobTeams.GetDeepCopy();
			_listJobTeamUsersOld=JobTeamUsers.GetDeepCopy();
			_listJobTeamUsers=JobTeamUsers.GetDeepCopy();
			_listUsers=JobHelper.ListEngineerUsers;
			FillAllTeamsUI();
		}

		private void FillAllTeamsUI(int selectedRow=0) {
			FillGridTeams(selectedRow);
			FillListTeamUsers();
			if(listBoxTeamMembers.SelectedItem==null) {
				butChangeTeamLead.Enabled=false;
				butDeleteTeamMember.Enabled=false;
			}
			JobTeam jobTeamSelected=gridTeams.SelectedTag<JobTeam>();
			butAddTeamMember.Enabled=jobTeamSelected==null ? false : true;
		}

		///<summary>Fills the grid pertaining to teams.<summary>
		private void FillGridTeams(int selectedRow=0) {
			gridTeams.BeginUpdate();
			gridTeams.Columns.Clear();
			GridColumn col=new GridColumn("Team Name",130);
			gridTeams.Columns.Add(col);
			col=new GridColumn("Team Lead",130);
			gridTeams.Columns.Add(col);
			col=new GridColumn("Team Focus",0);
			gridTeams.Columns.Add(col);
			gridTeams.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i < _listJobTeams.Count;i++) {
				row=new GridRow() { Tag=_listJobTeams[i] };
				row.Cells.Add(_listJobTeams[i].TeamName);
				JobTeamUser teamLead=_listJobTeamUsers.FirstOrDefault(x => x.JobTeamNum==_listJobTeams[i].JobTeamNum && x.IsTeamLead);
				if(teamLead==null) {//No team lead assigned to team yet.
					row.Cells.Add("");
				}
				else {
					Userod user=_listUsers.FirstOrDefault(x => x.UserNum==teamLead.UserNumEngineer);
					if(user==null) {//Somehow, the user does not exist. Maybe the userod got deleted/hidden, but we didn't delete/hide the JobTeamUser.
						row.Cells.Add("");
					}
					else {
						row.Cells.Add(user.UserName);
					}
				}
				row.Cells.Add(_listJobTeams[i].TeamFocus.ToString());
				gridTeams.ListGridRows.Add(row);
			}
			gridTeams.EndUpdate();
			if(_listJobTeams.Count > 0) {
				if(selectedRow < 0) {//Negative because we deleted the very first row.
					selectedRow=0;
				}
				gridTeams.SetSelected(selectedRow,true);
			}
			return;
		}

		///<summary>Fills the list of team members for the currently selected team.<summary>
		private void FillListTeamUsers() {
			listBoxTeamMembers.Items.Clear();
			if(gridTeams.SelectedTag<JobTeam>()==null) {//Only happens if there are no teams. No need to try and fill team members then.
				return;
			}
			_listJobTeamUsersCur=_listJobTeamUsers.FindAll(x => x.JobTeamNum==gridTeams.SelectedTag<JobTeam>().JobTeamNum);
			if(_listJobTeamUsersCur==null) {//No team members assigned to team yet.
				return;
			}
			List<Userod> listUserodsCur=_listUsers.FindAll(x => _listJobTeamUsersCur.Select(x => x.UserNumEngineer).ToList().Contains(x.UserNum));
			listBoxTeamMembers.Items.AddList(listUserodsCur,x => x.UserName);
		}

		private void gridTeams_TitleAddClick(object sender,EventArgs e) {
			using FormJobTeamEdit formJobTeamEdit=new FormJobTeamEdit();
			formJobTeamEdit.ListJobTeams=_listJobTeams;
			if(formJobTeamEdit.ShowDialog()!=DialogResult.OK) {
				return;
			}
			FillAllTeamsUI(selectedRow:_listJobTeams.Count-1);//The selected team is the new team added.
		}

		///<summary></summary>
		private void butDeleteTeamSelected_Click(object sender,EventArgs e) {
			JobTeam jobTeamSelected=gridTeams.SelectedTag<JobTeam>();
			DeleteTeam(jobTeamSelected);
			FillAllTeamsUI(selectedRow:gridTeams.GetSelectedIndex()-1);//Deleted a row, so select the previous row. FillGridTeams() will handle edge case where selectedRow==-1.
		}

		///<summary>Delete the passed in team and its members. Assumes a non-null JobTeam.</summary>
		private void DeleteTeam(JobTeam jobTeam) {
			_listJobTeamUsers.RemoveAll(x => x.JobTeamNum==jobTeam.JobTeamNum);
			_listJobTeams.Remove(jobTeam);
		}

		///<summary>Changes the team members displayed depending on the manually selected row.</summary>
		private void gridTeams_SelectionCommitted(object sender,EventArgs e) {
			FillListTeamUsers();
			butChangeTeamLead.Enabled=false;
			butDeleteTeamMember.Enabled=false;
		}

		private void gridTeams_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			JobTeam jobTeamSelected=gridTeams.SelectedTag<JobTeam>();
			using FormJobTeamEdit formJobTeamEdit=new FormJobTeamEdit(jobTeamSelected);
			formJobTeamEdit.ListJobTeams=_listJobTeams;
			if(formJobTeamEdit.ShowDialog()!=DialogResult.OK) {
				return;
			}
			FillGridTeams(selectedRow:gridTeams.GetSelectedIndex());
		}

		///<summary>Changes the team lead to the selected one and sets the old one (if applicable) to not be.</summary>
		private void butChangeTeamLead_Click(object sender,EventArgs e) {
			long jobTeamNumSelected=gridTeams.SelectedTag<JobTeam>().JobTeamNum;
			long userNum=listBoxTeamMembers.GetSelected<Userod>().UserNum;
			_listJobTeamUsersCur.ForEach(x => x.IsTeamLead=false);
			JobTeamUser newTeamLead=_listJobTeamUsersCur.FirstOrDefault(x => x.UserNumEngineer==userNum);
			if(newTeamLead!=null) {//Should never happen.
				newTeamLead.IsTeamLead=true;
			}
			FillGridTeams(selectedRow:gridTeams.GetSelectedIndex());
		}

		///<summary>Associate a JobTeamUser to the selected Userod and JobTeam.</summary>
		private void butAddTeamMember_Click(object sender,EventArgs e) {
			List<Userod> listUsersNotInTeam=_listUsers.FindAll(x => !_listJobTeamUsersCur.Select(x => x.UserNumEngineer).Contains(x.UserNum));
			if(listUsersNotInTeam.Count==0) {
				MsgBox.Show("All Engineers have been assigned to this team");
				return;
			}
			bool isTeamLeadOpen=_listJobTeamUsersCur.All(x => !x.IsTeamLead);
			using FormJobTeamUserSelect formJobTeamUserSelect=new FormJobTeamUserSelect(listUsersNotInTeam,_listJobTeamUsersCur,isTeamLeadOpen);
			if(formJobTeamUserSelect.ShowDialog()!=DialogResult.OK) {
				return;
			}
			long jobTeamNumSelected=gridTeams.SelectedTag<JobTeam>().JobTeamNum;
			long userNumSelected=formJobTeamUserSelect.UserNumSelected;
			bool isTeamLead=formJobTeamUserSelect.IsTeamLead;
			JobTeamUser jobTeamUser=new JobTeamUser() { 
				JobTeamNum=jobTeamNumSelected,
				UserNumEngineer=userNumSelected,
				IsTeamLead=isTeamLead,
			};
			_listJobTeamUsers.Add(jobTeamUser);
			FillAllTeamsUI(selectedRow:gridTeams.GetSelectedIndex());
		}

		private void butDeleteTeamMember_Click(object sender,EventArgs e) {
			Userod userSelected=listBoxTeamMembers.GetSelected<Userod>();
			JobTeamUser jobTeamUser=_listJobTeamUsersCur.FirstOrDefault(x => x.UserNumEngineer==userSelected.UserNum);
			_listJobTeamUsers.Remove(jobTeamUser);
			FillAllTeamsUI(selectedRow:gridTeams.GetSelectedIndex());
		}

		private void listBoxTeamMembers_SelectionChangeCommitted(object sender,EventArgs e) {
			butChangeTeamLead.Enabled=true;
			butDeleteTeamMember.Enabled=true;
		}

		private void FormJobTeams_FormClosing(object sender,FormClosingEventArgs e) {
			JobTeams.Sync(_listJobTeams,_listJobTeamsOld);
			JobTeamUsers.Sync(_listJobTeamUsers,_listJobTeamUsersOld);
			DataValid.SetInvalid(InvalidType.JobTeams);
		}
	}
}
