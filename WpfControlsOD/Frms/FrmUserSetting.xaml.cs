using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	///<summary>This form (per Nathan) should be used for any future features that could be categorized as a user setting. The intent of this class was to
	/// create a place for specific user settings.</summary>
	public partial class FrmUserSetting:FrmODBase {

		private UserOdPref _userOdPrefSuppressLogOffMessage;
		private UserOdPref _userOdPrefDiffChartColorForCurProv=null;

		public FrmUserSetting() {
			InitializeComponent();
			//Lan.F(this);
		}

		private void FrmUserSetting_Loaded(object sender,RoutedEventArgs e) {
			//Logoff After Minutes
			UserOdPref userOdPrefLogOffAfterMinutes=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.LogOffTimerOverride).FirstOrDefault();
			textLogOffAfterMinutes.Text=(userOdPrefLogOffAfterMinutes==null) ? "" : userOdPrefLogOffAfterMinutes.ValueString;
			//Suppress Logoff Message
			_userOdPrefSuppressLogOffMessage=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.SuppressLogOffMessage).FirstOrDefault();
			if(_userOdPrefSuppressLogOffMessage!=null) {//Does exist in the database
				checkSuppressMessage.Checked=true;
			}
			_userOdPrefDiffChartColorForCurProv=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.ToothChartUsesDiffColorByProv).FirstOrDefault();
			if(_userOdPrefDiffChartColorForCurProv!=null) {
				checkToothChartUsesDiffColorByProv.Checked=true;
			}	
		}

		private void SavePreferences() {
			bool doSetInvalid=false;
			#region Suppress Logoff Message
			if(checkSuppressMessage.Checked==true && _userOdPrefSuppressLogOffMessage==null) {
				UserOdPrefs.Insert(new UserOdPref() {
					UserNum=Security.CurUser.UserNum,
					FkeyType=UserOdFkeyType.SuppressLogOffMessage
				});
				doSetInvalid=true;
			}
			else if(checkSuppressMessage.Checked==false && _userOdPrefSuppressLogOffMessage!=null) {
				UserOdPrefs.Delete(_userOdPrefSuppressLogOffMessage.UserOdPrefNum);
				doSetInvalid=true;
			}
			#endregion
			#region Chart Color
			if(checkToothChartUsesDiffColorByProv.Checked==true && _userOdPrefDiffChartColorForCurProv==null) {
				UserOdPrefs.Insert(new UserOdPref() {
					UserNum=Security.CurUser.UserNum,
					FkeyType=UserOdFkeyType.ToothChartUsesDiffColorByProv
				});
				doSetInvalid=true;
				ToothChartRelay.RefreshToothColorsPrefs();
			}
			if(checkToothChartUsesDiffColorByProv.Checked==false && _userOdPrefDiffChartColorForCurProv!=null) {
				UserOdPrefs.Delete(_userOdPrefDiffChartColorForCurProv.UserOdPrefNum);
				doSetInvalid=true;
				ToothChartRelay.RefreshToothColorsPrefs();
			}
			if(doSetInvalid) {
				DataValid.SetInvalid(InvalidType.UserOdPrefs);
			}
			#endregion
		}

		private void butSave_Click(object sender,EventArgs e) {
			SavePreferences();
			IsDialogOK=true;
		}


	}
}