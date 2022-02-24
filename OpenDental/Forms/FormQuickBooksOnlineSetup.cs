using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using Bridges;
using System.Diagnostics;
using System.Linq;
using System.Net;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormQuickBooksOnlineSetup:FormODBase {
		///<summary>Only show Enabled checkbox if we are opening this form from the Program Links window.</summary>
		private bool _isFromFormProgramLinks;
		///<summary>Holds the current state of the QuickBooks Online program.</summary>
		private Program _progCur;
		///<summary>Holds the state of the QuickBooks Online program stored in the DB.</summary>
		private Program _progOld;
		///<summary>Holds the access token in the DB.</summary>
		private ProgramProperty _programPropAccessToken;
		///<summary>Holds the refresh token in the DB.</summary>
		private ProgramProperty _programPropRefreshToken;
		///<summary>Holds the current refresh and access tokens. Also holds exceptions that ocurred during a token request or token refresh request.</summary>
		private QuickBooksOnlineToken _tokens;
		///<summary>Holds the string of deposit accounts stored in the DB.</summary>
		private ProgramProperty _programPropDepositAccounts;
		///<summary>List of deposit accounts selected for use in Open Dental.</summary>
		private List<QuickBooksOnlineAccount> _listDepositAccountsInOD;
		///<summary>List of deposit accounts available from QuickBooks Online.</summary>
		List<QuickBooksOnlineAccount> _listDepositAccountsAvailable=new List<QuickBooksOnlineAccount>();
		///<summary>Holds the string of income accounts stored in the DB.</summary>
		private ProgramProperty _programPropIncomeAccounts;
		///<summary>List of income accounts selected for use in Open Dental.</summary>
		private List<QuickBooksOnlineAccount> _listIncomeAccountsInOD;
		///<summary>List of income accounts available from QuickBooks Online.</summary>
		List<QuickBooksOnlineAccount> _listIncomeAccountsAvailable=new List<QuickBooksOnlineAccount>();
		///<summary>Holds the string of class refs stored in the DB.</summary>
		private ProgramProperty _programPropClassRefs;
		///<summary>List of class refs selected for use in Open Dental.</summary>
		private List<QuickBooksOnlineClassRef> _listClassRefsInOD;
		///<summary>List of class refs available from QuickBooks Online.</summary>
		List<QuickBooksOnlineClassRef> _listClassRefsAvailable=new List<QuickBooksOnlineClassRef>();
		///<summary>The realm ID stored in the DB.</summary>
		private ProgramProperty _programPropRealmId;
		///<summary>The current realm ID.</summary>
		private string _realmIdCur;
		///<summary>True when an error response is received while attempting to get accounts and classes from QBO.</summary>
		private bool _hasQboError;

		public FormQuickBooksOnlineSetup(bool IsFromFormProgramLinks=false) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_isFromFormProgramLinks=IsFromFormProgramLinks;
		}

		private void FormQuickBooksOnlineSetup_Load(object sender,EventArgs e) {
			_progCur=Programs.GetCur(ProgramName.QuickBooksOnline);
			_progOld=_progCur.Copy();
			_programPropAccessToken=ProgramProperties.GetPropForProgByDesc(_progCur.ProgramNum,"Access Token");
			_programPropRefreshToken=ProgramProperties.GetPropForProgByDesc(_progCur.ProgramNum,"Refresh Token");
			_tokens=new QuickBooksOnlineToken(_programPropAccessToken.PropertyValue,_programPropRefreshToken.PropertyValue);
			_programPropDepositAccounts=ProgramProperties.GetPropForProgByDesc(_progCur.ProgramNum,"Deposit Accounts");
			_listDepositAccountsInOD=QuickBooksOnlineAccount.StringAccountsToList(_programPropDepositAccounts.PropertyValue);
			_programPropIncomeAccounts=ProgramProperties.GetPropForProgByDesc(_progCur.ProgramNum,"Income Accounts");
			_listIncomeAccountsInOD=QuickBooksOnlineAccount.StringAccountsToList(_programPropIncomeAccounts.PropertyValue);
			_programPropClassRefs=ProgramProperties.GetPropForProgByDesc(_progCur.ProgramNum,"Class Refs");
			_listClassRefsInOD=QuickBooksOnlineClassRef.StringClassRefsToList(_programPropClassRefs.PropertyValue);
			_programPropRealmId=ProgramProperties.GetPropForProgByDesc(_progCur.ProgramNum,"Realm ID");
			_realmIdCur=_programPropRealmId.PropertyValue;
			checkEnabled.Visible=_isFromFormProgramLinks;
			checkEnabled.Checked=_progCur.Enabled;
			FillTokenFields();
			EnableOrDisableControls();
			if(checkEnabled.Checked) {
				GetAccountsAndClassRefsFromQuickBooksOnline();
			}
		}

		private void FillTokenFields() {
			textAccessToken.Text=_programPropAccessToken.PropertyValue;
			textRefreshToken.Text=_programPropRefreshToken.PropertyValue;
		}

		private void EnableOrDisableControls() {
			bool isEnabled=checkEnabled.Checked;
			butAuthenticate.Enabled=isEnabled;
			butAddDepositAccount.Enabled=isEnabled;
			butRemoveDepositAccount.Enabled=isEnabled;
			butAddIncomeAccount.Enabled=isEnabled;
			butRemoveIncomeAccount.Enabled=isEnabled;
			butAddClass.Enabled=isEnabled;
			butRemoveClass.Enabled=isEnabled;
			listBoxDepositAccountsInOD.Enabled=isEnabled;
			listBoxDepositAccountsAvailable.Enabled=isEnabled;
			listBoxIncomeAccountsInOD.Enabled=isEnabled;
			listBoxIncomeAccountsAvailable.Enabled=isEnabled;
			listBoxClassRefsInOD.Enabled=isEnabled;
			listBoxClassRefsAvailable.Enabled=isEnabled;
		}

		private void GetAccountsAndClassRefsFromQuickBooksOnline() {
			if(_tokens.AccessToken.IsNullOrEmpty()) {
				MsgBox.Show(
					this,"Please click the Authenticate button to log in to your QuickBooks Online account and obtain an Authorization Code and Realm ID.");
				return;
			}
			try {
				GetDepositAccountsFromQBO();
				GetIncomeAccountsFromQBO();
				GetClassRefsFromQBO();
				_hasQboError=false;
			}
			catch(Exception ex) {
				FriendlyException.Show(ex.Message,ex);
				_hasQboError=true;
			}
			FillListBoxesWithAccounts(_listDepositAccountsInOD,_listDepositAccountsAvailable,listBoxDepositAccountsInOD,listBoxDepositAccountsAvailable);
			FillListBoxesWithAccounts(_listIncomeAccountsInOD,_listIncomeAccountsAvailable,listBoxIncomeAccountsInOD,listBoxIncomeAccountsAvailable);
			FillListBoxesWithClassRefs();
		}

		///<summary>Throws exceptions</summary>
		private void GetDepositAccountsFromQBO() {
			_listDepositAccountsAvailable=QuickBooksOnline.GetDepositAccounts(_realmIdCur,_tokens);
			if(!_listDepositAccountsAvailable.IsNullOrEmpty() && _listDepositAccountsAvailable[0].RequestException!=null) {
				if(QuickBooksOnline.RefreshTokensIfNeeded(_listDepositAccountsAvailable[0].RequestException,_tokens)) {
					//Save new tokens and try again.
					SaveTokensAndRealmId();
					_listDepositAccountsAvailable=QuickBooksOnline.GetDepositAccounts(_realmIdCur,_tokens);
				}
				//Check again as a different error may have occured.
				if(!_listDepositAccountsAvailable.IsNullOrEmpty() && _listDepositAccountsAvailable[0].RequestException!=null) {
					throw _listDepositAccountsAvailable[0].RequestException;
				}
			}
		}

		///<summary>Throws exceptions</summary>
		private void GetIncomeAccountsFromQBO() {
			_listIncomeAccountsAvailable=QuickBooksOnline.GetIncomeAccounts(_realmIdCur,_tokens);
			if(!_listIncomeAccountsAvailable.IsNullOrEmpty() && _listIncomeAccountsAvailable[0].RequestException!=null) {
				if(QuickBooksOnline.RefreshTokensIfNeeded(_listIncomeAccountsAvailable[0].RequestException,_tokens)) {
					//Save new tokens and try again.
					SaveTokensAndRealmId();
					_listIncomeAccountsAvailable=QuickBooksOnline.GetIncomeAccounts(_realmIdCur,_tokens);
				}
				//Check again as a different error may have occured.
				if(!_listIncomeAccountsAvailable.IsNullOrEmpty() && _listIncomeAccountsAvailable[0].RequestException!=null) {
					throw _listIncomeAccountsAvailable[0].RequestException;
				}
			}
		}

		///<summary>Throws exceptions</summary>
		private void GetClassRefsFromQBO() {
			_listClassRefsAvailable=QuickBooksOnline.GetClassRefs(_realmIdCur,_tokens);
			if(!_listClassRefsAvailable.IsNullOrEmpty() && _listClassRefsAvailable[0].RequestException!=null) {
				if(QuickBooksOnline.RefreshTokensIfNeeded(_listClassRefsAvailable[0].RequestException,_tokens)) {
					//Save new tokens and try again.
					SaveTokensAndRealmId();
					_listClassRefsAvailable=QuickBooksOnline.GetClassRefs(_realmIdCur,_tokens);
				}
				//Check again as a different error may have occured.
				if(!_listClassRefsAvailable.IsNullOrEmpty() && _listClassRefsAvailable[0].RequestException!=null) {
					throw _listClassRefsAvailable[0].RequestException;
				}
			}
		}

		private void FillListBoxesWithAccounts(List<QuickBooksOnlineAccount> listAccountsInOD,List<QuickBooksOnlineAccount> listAccountsAvailable
			,ListBoxOD listBoxInOD,ListBoxOD listBoxAvailable)
		{
			listBoxInOD.Items.Clear();
			listBoxAvailable.Items.Clear();
			listAccountsInOD=listAccountsInOD.OrderBy(x => x.Name).ToList();
			listAccountsAvailable=listAccountsAvailable.OrderBy(x => x.Name).ToList();
			List<QuickBooksOnlineAccount> listAccountsInBoth=new List<QuickBooksOnlineAccount>();
			for(int i=0;i<listAccountsInOD.Count;i++) {
				QuickBooksOnlineAccount accountAvailable=listAccountsAvailable.FirstOrDefault(x => x.Id==listAccountsInOD[i].Id);
				string displayText=listAccountsInOD[i].Name;
				if(accountAvailable==null) {
					if(!_hasQboError) {
						displayText+=" (No longer in QBO)";//We successfully got accounts from QBO and this account no longer exists.
					}
				}
				else {
					listAccountsInBoth.Add(accountAvailable);
				}
				listBoxInOD.Items.Add(displayText,listAccountsInOD[i]);
			}
			for(int i=0;i<listAccountsAvailable.Count;i++) {
				//If account already displays as selected for use in OD, then don't display it in available accounts.
				if(listAccountsInBoth.Contains(listAccountsAvailable[i])) {
					continue;
				}
				listBoxAvailable.Items.Add(listAccountsAvailable[i].Name,listAccountsAvailable[i]);
			}
		}

		private void FillListBoxesWithClassRefs() {
			listBoxClassRefsInOD.Items.Clear();
			listBoxClassRefsAvailable.Items.Clear();
			_listClassRefsInOD=_listClassRefsInOD.OrderBy(x => x.Name).ToList();
			_listClassRefsAvailable=_listClassRefsAvailable.OrderBy(x => x.Name).ToList();
			List<QuickBooksOnlineClassRef> listClassRefsInBoth=new List<QuickBooksOnlineClassRef>();
			for(int i=0;i<_listClassRefsInOD.Count;i++) {
				QuickBooksOnlineClassRef classRefAvailable=_listClassRefsAvailable.FirstOrDefault(x => x.Id==_listClassRefsInOD[i].Id);
				string displayText=_listClassRefsInOD[i].Name;
				if(classRefAvailable==null) {
					if(!_hasQboError) {
						displayText+=" (No longer in QBO)";//We successfully got classRefs from QBO and this classRef no longer exists.
					}
				}
				else {
					listClassRefsInBoth.Add(classRefAvailable);
				}
				listBoxClassRefsInOD.Items.Add(displayText,_listClassRefsInOD[i]);
			}
			for(int i=0;i<_listClassRefsAvailable.Count;i++) {
				//If class ref already displays as selected for use in OD, then don't display it in available class refs.
				if(listClassRefsInBoth.Contains(_listClassRefsAvailable[i])) {
					continue;
				}
				listBoxClassRefsAvailable.Items.Add(_listClassRefsAvailable[i].Name,_listClassRefsAvailable[i]);
			}
		}

		private void SaveTokensAndRealmId() {
			bool changed=false;
			if(!_tokens.AccessToken.IsNullOrEmpty()) {
				changed|=ProgramProperties.UpdateProgramPropertyWithValue(_programPropAccessToken,_tokens.AccessToken);
				_programPropAccessToken.PropertyValue=_tokens.AccessToken;
			}
			if(!_tokens.RefreshToken.IsNullOrEmpty()) {
				changed|=ProgramProperties.UpdateProgramPropertyWithValue(_programPropRefreshToken,_tokens.RefreshToken);
				_programPropRefreshToken.PropertyValue=_tokens.RefreshToken;
			}
			changed|=ProgramProperties.UpdateProgramPropertyWithValue(_programPropRealmId,_realmIdCur);
			if(changed) {
				DataValid.SetInvalid(InvalidType.Programs);
			}
			FillTokenFields();
		}

		private void checkEnabled_Click(object sender,EventArgs e) {
			//Can't disable QuickBooks Online if it is the currently selected deposit software.
			if(PrefC.GetEnum<AccountingSoftware>(PrefName.AccountingSoftware)==AccountingSoftware.QuickBooksOnline && _progOld.Enabled) {
				checkEnabled.Checked=true;
				MsgBox.Show(this,"You cannot disable QuickBooks Online while it is the selected Deposit Software in Manage Module Preferences.");
				return;
			}
			EnableOrDisableControls();
			if(checkEnabled.Checked) {
				GetAccountsAndClassRefsFromQuickBooksOnline();
			}
		}

		private void butAuthenticate_Click(object sender,EventArgs e) {
			try {
				string authorizationUrl=QuickBooksOnline.GetQuickBooksOnlineAuthorizationUrl();
				Process.Start(authorizationUrl);
				using FormQuickBooksOnlineAuthorization formQuickBooksOnlineAuth=new FormQuickBooksOnlineAuthorization();
				formQuickBooksOnlineAuth.ShowDialog();
				if(formQuickBooksOnlineAuth.DialogResult!=DialogResult.OK) {
					return;
				}
				if(formQuickBooksOnlineAuth.AuthCode.IsNullOrEmpty() || formQuickBooksOnlineAuth.RealmId.IsNullOrEmpty()) {
					MsgBox.Show(this,"The Authorization Code or Realm ID was missing from your entry.");
					return;
				}
				string authCode=formQuickBooksOnlineAuth.AuthCode;
				QuickBooksOnlineToken tokensNew=QuickBooksOnline.MakeAccessTokenRequest(authCode);
				if(tokensNew.RequestException!=null) {
					throw tokensNew.RequestException;
				}
				_tokens.AccessToken=tokensNew.AccessToken;
				_tokens.RefreshToken=tokensNew.RefreshToken;
				_realmIdCur=formQuickBooksOnlineAuth.RealmId;
				SaveTokensAndRealmId();
				GetAccountsAndClassRefsFromQuickBooksOnline();
			}
			catch(Exception ex) {
				FriendlyException.Show(ex.Message,ex);
			}
		}

		private void butAddDepositAccount_Click(object sender,EventArgs e) {
			QuickBooksOnlineAccount account=listBoxDepositAccountsAvailable.GetSelected<QuickBooksOnlineAccount>();
			if(account==null) {
				return;//No available deposit account selected.
			}
			if(account.Name.Contains(",") || account.Name.Contains("|")) {
				MsgBox.Show(this,"Accounts from QuickBooks Online with names that contain commas or vertical bars cannot be used in Open Dental.");
				return;
			}
			_listDepositAccountsInOD.Add(account);
			FillListBoxesWithAccounts(_listDepositAccountsInOD,_listDepositAccountsAvailable,listBoxDepositAccountsInOD,listBoxDepositAccountsAvailable);
		}

		private void butRemoveDepositAccount_Click(object sender,EventArgs e) {
			QuickBooksOnlineAccount account=listBoxDepositAccountsInOD.GetSelected<QuickBooksOnlineAccount>();
			if(account==null) {
				return;//No deposit account from OD selected.
			}
			_listDepositAccountsInOD.Remove(account);
			FillListBoxesWithAccounts(_listDepositAccountsInOD,_listDepositAccountsAvailable,listBoxDepositAccountsInOD,listBoxDepositAccountsAvailable);
		}

		private void butAddIncomeAccount_Click(object sender,EventArgs e) {
			QuickBooksOnlineAccount account=listBoxIncomeAccountsAvailable.GetSelected<QuickBooksOnlineAccount>();
			if(account==null) {
				return;//No available income account selected.
			}
			if(account.Name.Contains(",") || account.Name.Contains("|")) {
				MsgBox.Show(this,"Accounts from QuickBooks Online with names that contain commas or vertical bars cannot be used in Open Dental.");
				return;
			}
			_listIncomeAccountsInOD.Add(account);
			FillListBoxesWithAccounts(_listIncomeAccountsInOD,_listIncomeAccountsAvailable,listBoxIncomeAccountsInOD,listBoxIncomeAccountsAvailable);
		}

		private void butRemoveIncomeAccount_Click(object sender,EventArgs e) {
			QuickBooksOnlineAccount account=listBoxIncomeAccountsInOD.GetSelected<QuickBooksOnlineAccount>();
			if(account==null) {
				return;//No income account from OD selected.
			}
			_listIncomeAccountsInOD.Remove(account);
			FillListBoxesWithAccounts(_listIncomeAccountsInOD,_listIncomeAccountsAvailable,listBoxIncomeAccountsInOD,listBoxIncomeAccountsAvailable);
		}

		private void butAddClass_Click(object sender,EventArgs e) {
			QuickBooksOnlineClassRef classRef=listBoxClassRefsAvailable.GetSelected<QuickBooksOnlineClassRef>();
			if(classRef==null) {
				return;//No available class ref selected.
			}
			if(classRef.Name.Contains(",") || classRef.Name.Contains("|")) {
				MsgBox.Show(this,"Classes from QuickBooks Online with names that contain commas or vertical bars cannot be used in Open Dental.");
				return;
			}
			_listClassRefsInOD.Add(classRef);
			FillListBoxesWithClassRefs();
		}

		private void butRemoveClass_Click(object sender,EventArgs e) {
			QuickBooksOnlineClassRef classRef=listBoxClassRefsInOD.GetSelected<QuickBooksOnlineClassRef>();
			if(classRef==null) {
				return;//No classRef from OD selected.
			}
			_listClassRefsInOD.Remove(classRef);
			FillListBoxesWithClassRefs();
		}

		private void butOK_Click(object sender,EventArgs e) {
			_progCur.Enabled=checkEnabled.Checked;
			bool changed=false;
			changed|=Programs.Update(_progCur,_progOld);
			changed|=ProgramProperties.
				UpdateProgramPropertyWithValue(_programPropDepositAccounts,QuickBooksOnlineAccount.ListAccountsToString(_listDepositAccountsInOD));
			changed|=ProgramProperties.
				UpdateProgramPropertyWithValue(_programPropIncomeAccounts,QuickBooksOnlineAccount.ListAccountsToString(_listIncomeAccountsInOD));
			changed|=ProgramProperties.
				UpdateProgramPropertyWithValue(_programPropClassRefs,QuickBooksOnlineClassRef.ListClassRefsToString(_listClassRefsInOD));
			if(changed) {
				DataValid.SetInvalid(InvalidType.Programs);
			}
			if(!_progOld.Enabled && _progCur.Enabled
				&& MsgBox.Show(MsgBoxButtons.YesNo,"You have enabled QuickBooks Online. Would you like to set it as your Deposit Software?"))
			{
				Prefs.UpdateInt(PrefName.AccountingSoftware,(int)AccountingSoftware.QuickBooksOnline);
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}