using OpenDentBusiness;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace OpenDental {
	public partial class FormCommItem : FormODBase {
		private Commlog _commlogCur;
		private Commlog _commlogOld;
		///<summary>Set to true if this commlog window should always stay open.  Changes lots of functionality throughout the entire window.</summary>
		private bool _isPersistent;
		///<summary>The user pref that indicates if this user wants the Note text box to clear after a commlog is saved in persistent mode.
		///Can be null and will be treated as turned on (true) if null.</summary>
		private UserOdPref _userOdPrefClearNote;
		///<summary>The user pref that indicates if this user wants the End text box to clear after a commlog is saved in persistent mode.
		///Can be null and will be treated as turned on (true) if null</summary>
		private UserOdPref _userOdPrefEndDate;
		///<summary>The user pref that indicates if this user wants the Date / Time text box to clear after a commlog is saved in persistent mode.
		///Can be null and will be treated as turned on (true) if null</summary>
		private UserOdPref _userOdPrefUpdateDateTimeNewPat;
		private const string _autoNotePromptRegex=@"\[Prompt:""[a-zA-Z_0-9 ]+""\]";
		private bool _sigChanged;
		private bool _isStartingUp;
		private List<Def> _listCommlogTypeDefs;
		private bool _isUserClosing=true;

		public FormCommItem(Commlog commlog,bool isPersistent=false) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_isPersistent=isPersistent;
			_commlogCur=commlog;
			_commlogOld=commlog.Copy();
		}

		private void FormCommItem_Load(object sender,EventArgs e) {
			_isStartingUp=true;
			_listCommlogTypeDefs=Defs.GetDefsForCategory(DefCat.CommLogTypes,true);
			if(!PrefC.IsODHQ) {
				_listCommlogTypeDefs.RemoveAll(x => x.ItemValue==CommItemTypeAuto.ODHQ.ToString());
			}
			//there will usually be a commtype set before this dialog is opened
			for(int i=0;i<_listCommlogTypeDefs.Count;i++){
				listType.Items.Add(_listCommlogTypeDefs[i].ItemName);
				if(_listCommlogTypeDefs[i].DefNum==_commlogCur.CommType) {
					listType.SetSelected(i);
				}
			}
			listMode.Items.AddEnums<CommItemMode>();
			listSentOrReceived.Items.AddEnums<CommSentOrReceived>();
			if(!PrefC.IsODHQ) {
				labelDateTimeEnd.Visible=false;
				textDateTimeEnd.Visible=false;
				butNow.Visible=false;
				butNowEnd.Visible=false;
			}
			bool isNewOrPersistent=_commlogOld.IsNew || _isPersistent;
			//Check if the commlog is new and user has permission to create
			if(isNewOrPersistent && !Security.IsAuthorized(Permissions.CommlogCreate)) {
				DialogResult=DialogResult.Cancel;
				Close();
				return;
			}
			//Check if the user has permission to edit old commlogs
			if(!isNewOrPersistent && !Security.IsAuthorized(Permissions.CommlogEdit,_commlogCur.CommDateTime)) {
				butDelete.Enabled=false;
				butOK.Enabled=false;
				butEditAutoNote.Enabled=false;
			}
			textNote.Select();
			string keyData=GetSignatureKey();
			signatureBoxWrapper.FillSignature(_commlogCur.SigIsTopaz,keyData,_commlogCur.Signature);
			signatureBoxWrapper.BringToFront();
			if(_isPersistent) {
				RefreshUserOdPrefs();
				labelCommlogNum.Visible=false;
				textCommlogNum.Visible=false;
				butUserPrefs.Visible=true;
				butOK.Text=Lan.g(this,"Create");
				butCancel.Text=Lan.g(this,"Close");
				butDelete.Visible=false;
			}
			if(_commlogOld.IsNew && PrefC.GetBool(PrefName.CommLogAutoSave)) {
				timerAutoSave.Start();
			}
			textPatientName.Text=Patients.GetLim(_commlogCur.PatNum).GetNameFL();
			textUser.Text=Userods.GetName(_commlogCur.UserNum);//might be blank.
			textDateTime.Text=_commlogCur.CommDateTime.ToShortDateString()+"  "+_commlogCur.CommDateTime.ToShortTimeString();
			if(_commlogCur.DateTimeEnd.Year>1880) {
				textDateTimeEnd.Text=_commlogCur.DateTimeEnd.ToShortDateString()+"  "+_commlogCur.DateTimeEnd.ToShortTimeString();
			}
			listMode.SetSelected((int)_commlogCur.Mode_);
			listSentOrReceived.SetSelected((int)_commlogCur.SentOrReceived);
			textNote.Text=_commlogCur.Note;
			textNote.SelectionStart=textNote.Text.Length;
			butEditAutoNote.Visible=GetHasAutoNotePrompt();
			if(!ODBuild.IsDebug()) {
				labelCommlogNum.Visible=false;
				textCommlogNum.Visible=false;
			}
			textCommlogNum.Text=_commlogCur.CommlogNum.ToString();
			if(_isPersistent) {
				PatientChangedEvent.Fired+=PatientChangedEvent_Fired;
			}
			CommItemSaveEvent.Fired+=CommItemSaveEvent_Fired;
			_isStartingUp=false;
		}

		private bool SyncCommlogWithUI(bool showMsg) {
			if(!IsValid(showMsg)) {
				return false;
			}
			_commlogCur.DateTimeEnd=PIn.DateT(textDateTimeEnd.Text);
			_commlogCur.CommDateTime=PIn.DateT(textDateTime.Text);
			//there may not be a commtype selected.
			if(listType.SelectedIndex==-1) {
				_commlogCur.CommType=0;
			}
			else {
				_commlogCur.CommType=_listCommlogTypeDefs[listType.SelectedIndex].DefNum;
			}
			_commlogCur.Mode_=listMode.GetSelected<CommItemMode>();
			_commlogCur.SentOrReceived=listSentOrReceived.GetSelected<CommSentOrReceived>();
			_commlogCur.Note=textNote.Text;
			try {
				if(_sigChanged) {
					string keyData=GetSignatureKey();
					_commlogCur.Signature=signatureBoxWrapper.GetSignature(keyData);
					_commlogCur.SigIsTopaz=signatureBoxWrapper.GetSigIsTopaz();
				}
			}
			catch(Exception) {
				//Currently the only way for this method to fail is when saving the signature.
				MsgBox.Show(this,"Error saving signature.");
				return false;
			}
			return true;
		}

		private void RefreshUserOdPrefs() {
			if(Security.CurUser==null || Security.CurUser.UserNum < 1) {
				return;
			}
			_userOdPrefClearNote=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.CommlogPersistClearNote).FirstOrDefault();
			_userOdPrefEndDate=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.CommlogPersistClearEndDate).FirstOrDefault();
			_userOdPrefUpdateDateTimeNewPat=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.CommlogPersistUpdateDateTimeWithNewPatient).FirstOrDefault();
		}

		///<summary>Helper method to update textDateTime with DateTime.Now</summary>
		private void UpdateButNow() {
			textDateTime.Text=DateTime.Now.ToShortDateString()+"  "+DateTime.Now.ToShortTimeString();
		}

		private void ClearNote() {
			textNote.Clear();
		}

		private void ClearDateTimeEnd() {
			textDateTimeEnd.Clear();
		}

		private bool GetHasAutoNotePrompt() {
			return Regex.IsMatch(textNote.Text,_autoNotePromptRegex);
		}

		private void signatureBoxWrapper_SignatureChanged(object sender,EventArgs e) {
			_commlogCur.UserNum=Security.CurUser.UserNum;
			textUser.Text=Userods.GetName(Security.CurUser.UserNum);//might be blank.
			_sigChanged=true;
		}
		
		private void ClearSignature() {
			if(!_isStartingUp//so this happens only if user changes the note
				&& !_sigChanged)//and the original signature is still showing.
			{
				//SigChanged=true;//happens automatically through the event.
				signatureBoxWrapper.ClearSignature();
			}
		}

		private void ClearSignature_Handler(object sender,EventArgs e) {
			ClearSignature();
		}

		private string GetSignatureKey() {
			string keyData=_commlogCur.UserNum.ToString();
			keyData+=_commlogCur.CommDateTime.ToString();
			keyData+=_commlogCur.Mode_.ToString();
			keyData+=_commlogCur.SentOrReceived.ToString();
			if(_commlogCur.Note!=null) {
				keyData+=_commlogCur.Note.ToString();
			}
			return keyData;
		}

		///<summary>Validates the UI and returns a boolean indicating if the UI is in a valid state.
		///Set showMsg true in order to display a warning message regarding invalid UI prior to returning false.  Otherwise; silently fails.</summary>
		private bool IsValid(bool showMsg) {
			if(textDateTime.Text=="") {
				if(showMsg) {
					MsgBox.Show(this,"Please enter a date first.");
				}
				return false;
			}
			try {
				DateTime.Parse(textDateTime.Text);
			}
			catch {
				if(showMsg) {
					MsgBox.Show(this,"Date / Time invalid.");
				}
				return false;
			}
			if(textDateTimeEnd.Text!="") {
				try {
					DateTime.Parse(textDateTimeEnd.Text);
				}
				catch {
					if(showMsg) {
						MsgBox.Show(this,"End date and time invalid.");
					}
					return false;
				}
			}
			return true;
		}

		///<summary>Returns true if the commlog was able to save to the database.  Otherwise returns false.
		///Set showMsg to true to show a meaningful message when the commlog cannot be saved.</summary>
		private bool SaveCommItem(bool showMsg) {
			if(!SyncCommlogWithUI(showMsg)) {
				return false;
			}
			if(_isPersistent && string.IsNullOrWhiteSpace(_commlogCur.Note)) { //in persistent mode, we don't want to save empty commlogs
				return false;
			}
			if(_commlogOld.IsNew || _isPersistent) {
				Commlogs.Insert(_commlogCur);
				_commlogCur.IsNew=false;
				_commlogOld=_commlogCur.Copy();
				textCommlogNum.Text=_commlogCur.CommlogNum.ToString();
				SecurityLogs.MakeLogEntry(Permissions.CommlogEdit,_commlogCur.PatNum,"Insert");
				//Post insert persistent user preferences.
				if(_isPersistent) {
					if(_userOdPrefClearNote==null || PIn.Bool(_userOdPrefClearNote.ValueString)) {
						ClearNote();
					}
					if(_userOdPrefEndDate==null || PIn.Bool(_userOdPrefEndDate.ValueString)) {
						ClearDateTimeEnd();
					}
					ODException.SwallowAnyException(() => {
						FormOpenDental.S_RefreshCurrentModule();
					});
				}
			}
			else {
				Commlogs.Update(_commlogCur);
				SecurityLogs.MakeLogEntry(Permissions.CommlogEdit,_commlogCur.PatNum,"");
			}
			return true;
		}

		private void AutoSaveCommItem() {
			if(_commlogOld.IsNew) {
				//Insert
				Commlogs.Insert(_commlogCur);
				textCommlogNum.Text=this._commlogCur.CommlogNum.ToString();
				SecurityLogs.MakeLogEntry(Permissions.CommlogEdit,_commlogCur.PatNum,"Autosave Insert");
				_commlogCur.IsNew=false;
			}
			else {
				//Update
				Commlogs.Update(_commlogCur);
			}
			_commlogOld=_commlogCur.Copy();
		}

		private void timerAutoSave_Tick(object sender,EventArgs e) {
			if(_isPersistent) {
				//Just in case the auto save timer got turned on in persistent mode.
				timerAutoSave.Stop();
				return;
			}
			if(!SyncCommlogWithUI(false)) {
				return;
			}
			if(_commlogOld.Compare(_commlogCur)) {//They're equal, don't bother saving
				return;
			}
			bool doUpdateCommlogNum=false;
			if(_commlogOld.IsNew) {
				//Flag the UI to be updated after saving the commlog.
				doUpdateCommlogNum=true;
			}
			AutoSaveCommItem();
			//Getting this far means that the commlog was successfully updated so we need to update the UI to reflect that event.
			if(doUpdateCommlogNum) {
				textCommlogNum.Text=this._commlogCur.CommlogNum.ToString();
				butCancel.Enabled=false;
			}
			this.Text=Lan.g(this,"Communication Item - Saved:")+" "+DateTime.Now;
		}

		private void butUserPrefs_Click(object sender,EventArgs e) {
			using FormCommItemUserPrefs FormCIUP=new FormCommItemUserPrefs();
			FormCIUP.ShowDialog();
			if(FormCIUP.DialogResult==DialogResult.OK) {
				RefreshUserOdPrefs();
			}
		}

		private void butNow_Click(object sender,EventArgs e) {
			UpdateButNow();
		}

		private void butNowEnd_Click(object sender,EventArgs e) {
			textDateTimeEnd.Text=DateTime.Now.ToShortDateString()+"  "+DateTime.Now.ToShortTimeString();
		}

		private void butAutoNote_Click(object sender,EventArgs e) {
			using FormAutoNoteCompose FormA=new FormAutoNoteCompose();
			FormA.ShowDialog();
			if(FormA.DialogResult==DialogResult.OK) {
				textNote.AppendText(FormA.CompletedNote);
				butEditAutoNote.Visible=GetHasAutoNotePrompt();
			}
		}

		private void butEditAutoNote_Click(object sender,EventArgs e) {
			if(GetHasAutoNotePrompt()) {
				using FormAutoNoteCompose FormA=new FormAutoNoteCompose();
				FormA.MainTextNote=textNote.Text;
				FormA.ShowDialog();
				if(FormA.DialogResult==DialogResult.OK) {
					textNote.Text=FormA.CompletedNote;
					butEditAutoNote.Visible=GetHasAutoNotePrompt();
				}
			}
			else {
				MessageBox.Show(Lan.g(this,"No Auto Note available to edit."));
			}
		}

		private void butClearNote_Click(object sender,EventArgs e) {
			textNote.Clear();
		}

		private void PatientChangedEvent_Fired(ODEventArgs e) {
			if(e.EventType!=ODEventType.Patient || e.Tag.GetType()!=typeof(long)) {
				return;
			}
			//The tag for this event is the newly selected PatNum
			long patNum=(long)e.Tag;
			_commlogCur.PatNum=patNum;
			textPatientName.Text=Patients.GetLim(patNum).GetNameFL();
			if(_isPersistent && (_userOdPrefUpdateDateTimeNewPat==null || PIn.Bool(_userOdPrefUpdateDateTimeNewPat.ValueString))) {
				UpdateButNow();
			}
		}

		private void CommItemSaveEvent_Fired(CodeBase.ODEventArgs e) {
			if(e.EventType!=ODEventType.CommItemSave) {
				return;
			}
			if(e.Tag!=null && e.Tag is string && e.Tag.ToString()=="ShutdownAllWorkstations") {
				_isUserClosing=false;
			}
			//save comm item
			SaveCommItem(false);
		}

		///<summary>Shows the saved manually label for 1.5 seconds.</summary>
		private void ShowManualSaveLabel() {
			labelSavedManually.Visible=true;
			ODThread odThread=new ODThread((o) => {
				Thread.Sleep((int)TimeSpan.FromSeconds(1.5).TotalMilliseconds);
				this.InvokeIfNotDisposed(() => {
					labelSavedManually.Visible=false;
				});
			});
			odThread.AddExceptionHandler((e) => e.DoNothing());
			odThread.Start();
		}

		///<summary>Tries to delete the commlog passed in.  Throws exceptions if anything goes wrong.</summary>
		private void DeleteCommlog(Commlog commlog,string logText="Delete") {
			Commlogs.Delete(commlog);
			SecurityLogs.MakeLogEntry(Permissions.CommlogEdit,commlog.PatNum,logText);
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_commlogOld.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			//button not enabled if no permission and is invisible for persistent mode.
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
				return;
			}
			try {
				DeleteCommlog(_commlogCur);
				DialogResult=DialogResult.OK;
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);//Tell the user what went wrong with deleting the commlog.
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			//button not enabled if no permission
			if(!SaveCommItem(true)) {
				return;
			}
			if(_isPersistent) {
				//Show the user an indicator that the commlog has been saved but do not close the window.
				ShowManualSaveLabel();
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}

		private void FormCommItem_FormClosing(object sender,FormClosingEventArgs e) {
			signatureBoxWrapper?.SetTabletState(0);
			CommItemSaveEvent.Fired-=CommItemSaveEvent_Fired;
			if(_isPersistent) {
				PatientChangedEvent.Fired-=PatientChangedEvent_Fired;
			}
			if(_isPersistent) {
				return;
			}
			//Only delete the commlog from the database when the user manually closes the form.
			if(DialogResult==DialogResult.Cancel && timerAutoSave.Enabled && !_commlogOld.IsNew && _isUserClosing) {
				try {
					DeleteCommlog(_commlogCur,"Autosaved Commlog Deleted");
				}
				catch(Exception ex) {
					MessageBox.Show(this,ex.Message);
				}
			}
			timerAutoSave.Stop();
			timerAutoSave.Enabled=false;
		}
	}

}