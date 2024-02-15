using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental {
	public partial class FormDoseSpotAssignUserId:FormODBase {
		private List<Userod> _listUserods;
		private long _userNumSelected;
		private ProviderErx _providerErx;
		Program _program;

		public FormDoseSpotAssignUserId(long providerErxNum) {
			InitializeComponent();
			InitializeLayoutManager();
			//get providerErx from provErxNum that was passed in
			_providerErx=ProviderErxs.GetFirstOrDefault(x => x.ProviderErxNum==providerErxNum);
			Lan.F(this);
		}

		private void FormDoseSpotAssignUserId_Load(object sender,EventArgs e) {
			_program=Programs.GetCur(ProgramName.eRx);
			_listUserods=GetListDoseSpotUsers(true,_providerErx.NationalProviderID);
			if(_listUserods.Count==0) {//empty list, populate with all users
				_listUserods=GetListDoseSpotUsers(false);
			}
			FillComboBox();
			textUserId.Text=_providerErx.UserId;//UserID passed from Alert
		}

		private List<Userod> GetListDoseSpotUsers(bool includeProv,string provNpi="") {
			List<Userod> listUserods=new List<Userod>();
			List<Provider> listProviders=Providers.GetWhere(x => x.NationalProvID==provNpi,true);
			List<UserOdPref> listUserOdPrefs=UserOdPrefs.GetAllByFkeyAndFkeyType(_program.ProgramNum,UserOdFkeyType.Program);
			listUserOdPrefs=listUserOdPrefs.FindAll(x => string.IsNullOrWhiteSpace(x.ValueString));
			if(includeProv) {
				listUserods=Userods.GetWhere(x => listProviders.Exists(y => y.ProvNum==x.ProvNum) //Find users that have a link to the NPI that has been passed in
						&& !listUserOdPrefs.Exists(y => y.UserNum==x.UserNum) //Also, these users shouldn't already have a DoseSpot User ID.
					,true);//Only consider non-hidden users.
			}
			else {
				listUserods=Userods.GetWhere(
					(x => !listUserOdPrefs.Exists(y => y.UserNum==x.UserNum)) //All users that don't already have a DoseSpot User ID.
					,true);//Only consider non-hidden users.
			}
			return listUserods;
		}

		private void FillComboBox() {
			comboDoseUsers.Items.Clear();
			comboDoseUsers.Items.Add(Lan.g(this,"None"));
			comboDoseUsers.SelectedIndex=0;
			for(int i=0;i<_listUserods.Count;i++){
				comboDoseUsers.Items.Add(_listUserods[i].UserName,_listUserods[i]);
				if(_listUserods[i].UserNum==_userNumSelected) {
					comboDoseUsers.SelectedIndex=comboDoseUsers.Items.Count-1;//Select The item that was just added if it is the selected num.
				}
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(_userNumSelected==0) {
				MsgBox.Show(this,"Please select a user.");
				return;
			}
			UserOdPref userOdPref=UserOdPrefs.GetByCompositeKey(_userNumSelected,_program.ProgramNum,UserOdFkeyType.Program);
			UserOdPref userPrefOld=userOdPref.Clone();
			userOdPref.ValueString=_providerErx.UserId.ToString();
			if(userOdPref.IsNew) {
				userOdPref.Fkey=_program.ProgramNum;
				UserOdPrefs.Insert(userOdPref);
				DataValid.SetInvalid(InvalidType.UserOdPrefs);
			}
			else { 
				if(UserOdPrefs.Update(userOdPref,userPrefOld)) {
					//Only need to signal cache refresh on change.
					DataValid.SetInvalid(InvalidType.UserOdPrefs);
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butUserPick_Click(object sender,EventArgs e) {
			FrmUserPick frmUserPick=new FrmUserPick();
			frmUserPick.IsSelectionMode=true;
			frmUserPick.ListUserodsFiltered=_listUserods;
			frmUserPick.IsPickAllAllowed=false;
			frmUserPick.ShowDialog();
			if(!frmUserPick.IsDialogOK) {
				return;
			}
			_userNumSelected=frmUserPick.UserNumSelected;
			FillComboBox();
		}

		private void comboDoseUsers_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboDoseUsers.SelectedIndex==0) {
				_userNumSelected=0;
				return;
			}
			_userNumSelected=comboDoseUsers.GetSelectedKey<Userod>(x => x.UserNum);
		}

	}
}