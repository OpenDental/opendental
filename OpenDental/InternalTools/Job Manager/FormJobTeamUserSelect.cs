using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormJobTeamUserSelect:FormODBase {
		#region Private Variables
		private List<Userod> _listUsers;
		private List<JobTeamUser> _listJobTeamUsers;
		#endregion
		#region Public Variables
		public long UserNumSelected;
		public bool IsTeamLead;
		#endregion


		public FormJobTeamUserSelect(List<Userod> listUsers,List<JobTeamUser> listJobTeamUsers,bool isTeamLeadOpen) {
			InitializeComponent();
			InitializeLayoutManager();
			_listUsers=listUsers;
			_listJobTeamUsers=listJobTeamUsers;
			checkIsTeamLead.Visible=isTeamLeadOpen;
		}

		private void FormJobTeamUserSelect_Load(object sender,EventArgs e) {
			FillComboUser();
		}

		private void FillComboUser() {
			comboUser.Items.Clear();
			comboUser.Items.AddRange(_listUsers.ToArray());
		}
		
		private void butOK_Click(object sender,EventArgs e) {
			Userod user=(Userod)comboUser.SelectedItem;
			if(user==null) {
				MsgBox.Show("No user selected.");
				return;
			}
			bool isSelectedUserInTeam=_listJobTeamUsers.Any(x => x.UserNumEngineer==user.UserNum);
			if(isSelectedUserInTeam) {//should never happen
				MsgBox.Show("User is already in the current team.");
				return;
			}
			UserNumSelected=user.UserNum;
			IsTeamLead=checkIsTeamLead.Checked;
			DialogResult=DialogResult.OK;
			//Close();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			//Close();
		}
	}
}