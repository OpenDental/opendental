using System;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	/// <summary>
	/// This form (per Nathan) should be used for any future features that could be categorized as a user setting. The intent of this class was to
	/// create a place for specific user settings.
	/// </summary>
	public partial class FormUserSetting:FormODBase {
		private UserOdPref _suppressLogOffMessage;
		private UserOdPref _userODdiffChartColorForCurProv=null;

		public FormUserSetting() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}
		private void FormUserSetting_Load(object sender,EventArgs e) {
			//Logoff After Minutes
			UserOdPref logOffAfterMinutes=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.LogOffTimerOverride).FirstOrDefault();
			textLogOffAfterMinutes.Text=(logOffAfterMinutes==null) ? "" : logOffAfterMinutes.ValueString;
			//Suppress Logoff Message
			_suppressLogOffMessage=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.SuppressLogOffMessage).FirstOrDefault();
			if(_suppressLogOffMessage!=null) {//Does exist in the database
				checkSuppressMessage.Checked=true;
			}
			_userODdiffChartColorForCurProv=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.ToothChartUsesDiffColorByProv).FirstOrDefault();
			if(_userODdiffChartColorForCurProv!=null) {
				checkToothChartUsesDiffColorByProv.Checked=true;
			}	
		}

		private void SavePreferences() {
			#region Suppress Logoff Message
			if(checkSuppressMessage.Checked && _suppressLogOffMessage==null) {
				UserOdPrefs.Insert(new UserOdPref() {
					UserNum=Security.CurUser.UserNum,
					FkeyType=UserOdFkeyType.SuppressLogOffMessage
				});
			}
			else if(!checkSuppressMessage.Checked && _suppressLogOffMessage!=null) {
				UserOdPrefs.Delete(_suppressLogOffMessage.UserOdPrefNum);
			}
			#endregion
			#region Chart Color
			if(checkToothChartUsesDiffColorByProv.Checked && _userODdiffChartColorForCurProv==null) {
				UserOdPrefs.Insert(new UserOdPref() {
					UserNum=Security.CurUser.UserNum,
					FkeyType=UserOdFkeyType.ToothChartUsesDiffColorByProv
				});
				ToothChartRelay.RefreshToothColorsPrefs();
			}
			if(!checkToothChartUsesDiffColorByProv.Checked && _userODdiffChartColorForCurProv!=null) {
				UserOdPrefs.Delete(_userODdiffChartColorForCurProv.UserOdPrefNum);
				ToothChartRelay.RefreshToothColorsPrefs();
			}
			#endregion
		}

		private void butOK_Click(object sender,EventArgs e) {
			SavePreferences();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}