using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormCommItemUserPrefs:FormODBase {
		///<summary>Helper variable that gets set to Security.CurUser.UserNum on load.</summary>
		private long _userNumCur;
		private UserOdPref _userOdPrefClearNote;
		private UserOdPref _userOdPrefEndDate;
		private UserOdPref _userOdPrefUpdateDateTimeNewPat;

		public FormCommItemUserPrefs() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormCommItemUserPrefs_Load(object sender,EventArgs e) {
			if(Security.CurUser==null || Security.CurUser.UserNum < 1) {
				MsgBox.Show(this,"Invalid user currently logged in.  No user preferences can be saved.");
				DialogResult=DialogResult.Abort;
				return;
			}
			_userNumCur=Security.CurUser.UserNum;
			//Add the user name of the user currently logged in to the title of this window much like we do for FormOpenDental.
			this.Text+=" {"+Security.CurUser.UserName+"}";
			_userOdPrefClearNote=UserOdPrefs.GetByUserAndFkeyType(_userNumCur,UserOdFkeyType.CommlogPersistClearNote).FirstOrDefault();
			_userOdPrefEndDate=UserOdPrefs.GetByUserAndFkeyType(_userNumCur,UserOdFkeyType.CommlogPersistClearEndDate).FirstOrDefault();
			_userOdPrefUpdateDateTimeNewPat=UserOdPrefs.GetByUserAndFkeyType(_userNumCur,UserOdFkeyType.CommlogPersistUpdateDateTimeWithNewPatient).FirstOrDefault();
			checkCommlogPersistClearNote.Checked=(_userOdPrefClearNote==null) ? true : PIn.Bool(_userOdPrefClearNote.ValueString);
			checkCommlogPersistClearEndDate.Checked=(_userOdPrefEndDate==null) ? true : PIn.Bool(_userOdPrefEndDate.ValueString);
			checkCommlogPersistUpdateDateTimeWithNewPatient.Checked=(_userOdPrefUpdateDateTimeNewPat==null) ? true : PIn.Bool(_userOdPrefUpdateDateTimeNewPat.ValueString);
		}

		///<summary>Helper method to update or insert the passed in UserOdPref utilizing the specified valueString and keyType.
		///If the user pref passed in it null then a new user pref will be inserted.  Otherwise the user pref is updated.</summary>
		private void UpsertUserOdPref(UserOdPref userOdPref,UserOdFkeyType keyType,string valueString) {
			if(userOdPref==null) {
				UserOdPref userOdPrefTemp=new UserOdPref();
				userOdPrefTemp.Fkey=0;
				userOdPrefTemp.FkeyType=keyType;
				userOdPrefTemp.UserNum=_userNumCur;
				userOdPrefTemp.ValueString=valueString;
				UserOdPrefs.Insert(userOdPrefTemp);
			}
			else {
				userOdPref.FkeyType=keyType;
				userOdPref.ValueString=valueString;
				UserOdPrefs.Update(userOdPref);
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			UpsertUserOdPref(_userOdPrefClearNote
				,UserOdFkeyType.CommlogPersistClearNote
				,POut.Bool(checkCommlogPersistClearNote.Checked));
			UpsertUserOdPref(_userOdPrefEndDate
				,UserOdFkeyType.CommlogPersistClearEndDate
				,POut.Bool(checkCommlogPersistClearEndDate.Checked));
			UpsertUserOdPref(_userOdPrefUpdateDateTimeNewPat
				,UserOdFkeyType.CommlogPersistUpdateDateTimeWithNewPatient
				,POut.Bool(checkCommlogPersistUpdateDateTimeWithNewPatient.Checked));
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}