using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormJobTeamEdit:FormODBase {
		#region Private Variables
		private JobTeam _jobTeam;
		#endregion
		#region Public Variables
		public List<JobTeam> ListJobTeams;
		#endregion


		public FormJobTeamEdit(JobTeam jobTeam=null) {
			InitializeComponent();
			InitializeLayoutManager();
			_jobTeam=jobTeam;
		}

		private void FormJobTeamEdit_Load(object sender,EventArgs e) {
			comboTeamFocus.Items.AddEnums<JobTeamFocus>();
			if(_jobTeam==null) {
				return;
			}
			textTeamName.Text=PIn.String(_jobTeam.TeamName);
			textTeamDescription.Text=PIn.String(_jobTeam.TeamDescription);
			comboTeamFocus.SetSelectedEnum(_jobTeam.TeamFocus);
		}

		///<summary>To be considered valid, the team info must have all non-empty or non-None enum.</summary>
		private bool IsTeamInfoValid() {
			return !textTeamName.Text.IsNullOrEmpty() 
				&& !textTeamDescription.Text.IsNullOrEmpty() 
				&& comboTeamFocus.GetSelected<JobTeamFocus>()!=JobTeamFocus.None;
		}
	
		private void butOK_Click(object sender,EventArgs e) {
			if(!IsTeamInfoValid()) {
				MsgBox.Show("Fill all missing fields before saving team information.");
				return;
			}
			if(_jobTeam==null) {//new team
				_jobTeam=new JobTeam() { 
					TeamName=POut.String(textTeamName.Text),
					TeamDescription=POut.String(textTeamDescription.Text),
					TeamFocus=comboTeamFocus.GetSelected<JobTeamFocus>()
				};
				ListJobTeams.Add(_jobTeam);
				DialogResult=DialogResult.OK;
				return;
			}
			_jobTeam.TeamName=POut.String(textTeamName.Text);
			_jobTeam.TeamDescription=POut.String(textTeamDescription.Text);
			_jobTeam.TeamFocus=comboTeamFocus.GetSelected<JobTeamFocus>();
			for(int i=0;i < ListJobTeams.Count;i++) {
				if(ListJobTeams[i].JobTeamNum==_jobTeam.JobTeamNum) {
					ListJobTeams[i]=_jobTeam;
					break;
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}
	}
}