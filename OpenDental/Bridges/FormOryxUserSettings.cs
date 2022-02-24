using System;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	/// <summary>Used for user-specific settings that are unique to the Oryx bridge.</summary>
	public partial class FormOryxUserSettings:FormODBase {
		///<summary>User pref holding the user's Oryx username.</summary>
		private UserOdPref _userNamePref;
		///<summary>User pref holding the user's Oryx password.</summary>
		private UserOdPref _passwordPref;
		///<summary>Oryx program bridge.</summary>
		private Program _progOryx;

		public FormOryxUserSettings() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormUserSetting_Load(object sender,EventArgs e) {
			_progOryx=Programs.GetCur(ProgramName.Oryx);
			_userNamePref=UserOdPrefs.GetByUserFkeyAndFkeyType(Security.CurUser.UserNum,_progOryx.ProgramNum,UserOdFkeyType.ProgramUserName)
				.FirstOrDefault();
			_passwordPref=UserOdPrefs.GetByUserFkeyAndFkeyType(Security.CurUser.UserNum,_progOryx.ProgramNum,UserOdFkeyType.ProgramPassword)
				.FirstOrDefault();
			if(_userNamePref!=null) {
				textUsername.Text=_userNamePref.ValueString;
			}
			if(_passwordPref!=null) {
				string passwordPlain;
				CDT.Class1.Decrypt(_passwordPref.ValueString,out passwordPlain);
				textPassword.Text=passwordPlain;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			_userNamePref=_userNamePref??new UserOdPref {
				Fkey=_progOryx.ProgramNum,
				FkeyType=UserOdFkeyType.ProgramUserName,
				UserNum=Security.CurUser.UserNum,
			};
			_passwordPref=_passwordPref??new UserOdPref {
				Fkey=_progOryx.ProgramNum,
				FkeyType=UserOdFkeyType.ProgramPassword,
				UserNum=Security.CurUser.UserNum,
			};
			_userNamePref.ValueString=textUsername.Text;
			CDT.Class1.Encrypt(textPassword.Text,out _passwordPref.ValueString);
			UserOdPrefs.Upsert(_userNamePref);
			UserOdPrefs.Upsert(_passwordPref);
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}

	}
}