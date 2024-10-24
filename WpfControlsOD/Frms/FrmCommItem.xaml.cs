using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CodeBase;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	public partial class FrmCommItem : FrmODBase {
		///<summary>Set to true to not load default selections for Type, Mode_, and SentOrReceived controls.</summary>
		public bool DoOmitDefaults=false;
		///<summary>Set to true if opening from the FormCommItemHistory window. Indicates that the passed in Commlog is actually derived from a CommlogHist and should not be editable.</summary>
		public bool IsHistoric=false;
		///<summary>Set to true if this commlog window should always stay open. Changes lots of functionality throughout the entire window.</summary>
		public bool IsPersistent=false;
		private Commlog _commlog;
		private Commlog _commlogOld;
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
		private List<Def> _listDefsCommlogTypes;
		private bool _isUserClosing=true;
		private DispatcherTimer _dispatcherTimerAutoSave;

		public FrmCommItem(Commlog commlog) {
			InitializeComponent();
			Lang.F(this);
			_commlog=commlog;
			_commlogOld=commlog.Copy();
			Load+=FrmCommItem_Load;
			textNote.TextChanged+=ClearSignature_Handler;
			listSentOrReceived.SelectedIndexChanged+=ClearSignature_Handler;
			listMode.SelectedIndexChanged+=ClearSignature_Handler;
			textDateTime.TextChanged+=ClearSignature_Handler;
			listType.SelectedIndexChanged+=ClearSignature_Handler;
			signatureBoxWrapper.SignatureChanged+=signatureBoxWrapper_SignatureChanged;
			this.FormClosing+=FrmCommItem_FormClosing;
			PreviewKeyDown+=FrmCommItem_PreviewKeyDown;
		}

		private void FrmCommItem_Load(object sender,EventArgs e) {
			_isStartingUp=true;
			labelSavedManually.Visible=false;
			_listDefsCommlogTypes=Defs.GetDefsForCategory(DefCat.CommLogTypes,true);
			if(!PrefC.IsODHQ) {
				_listDefsCommlogTypes.RemoveAll(x => x.ItemValue==CommItemTypeAuto.ODHQ.ToString());
			}
			//there will usually be a commtype set before this dialog is opened
			for(int i=0;i<_listDefsCommlogTypes.Count;i++){
				listType.Items.Add(_listDefsCommlogTypes[i].ItemName);
				if(!DoOmitDefaults && _listDefsCommlogTypes[i].DefNum==_commlog.CommType) {
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
			bool isNewOrPersistent=_commlogOld.IsNew || IsPersistent;
			//Check if the commlog is new and user has permission to create
			if(isNewOrPersistent && !Security.IsAuthorized(EnumPermType.CommlogCreate)) {
				IsDialogOK=false;
				Close();
				return;
			}
			//Check if the user has permission to edit old commlogs
			if(!isNewOrPersistent && !Security.IsAuthorized(EnumPermType.CommlogEdit,_commlog.CommDateTime)) {
				butDelete.IsEnabled=false;
				butSave.IsEnabled=false;
				butEditAutoNote.IsEnabled=false;
			}
			string keyData=GetSignatureKey();
			signatureBoxWrapper.FillSignature(_commlog.SigIsTopaz,keyData,_commlog.Signature);
			if(IsPersistent) {
				RefreshUserOdPrefs();
				labelCommlogNum.Visible=false;
				textCommlogNum.Visible=false;
				butSave.Text=Lans.g(this,"Create");
				butDelete.Visible=false;
			}
			else{
				butUserPrefs.Visible=false;
			}
			_dispatcherTimerAutoSave=new DispatcherTimer();
			_dispatcherTimerAutoSave.Interval=TimeSpan.FromSeconds(10);
			_dispatcherTimerAutoSave.Tick+=timerAutoSave_Tick;
			if(_commlogOld.IsNew && PrefC.GetBool(PrefName.CommLogAutoSave)) {
				_dispatcherTimerAutoSave.Start();
			}
			textPatientName.Text=Patients.GetLim(_commlog.PatNum).GetNameFL();
			textUser.Text=Userods.GetName(_commlog.UserNum);//might be blank.
			if(_commlog.DateTEntry.Year>1880) {
				textDateTimeCreated.Text=_commlog.DateTEntry.ToShortDateString()+"  "+_commlog.DateTEntry.ToShortTimeString(); ; //blank if 01-01-01
			}
			textDateTime.Text=_commlog.CommDateTime.ToShortDateString()+"  "+_commlog.CommDateTime.ToShortTimeString();
			if(_commlog.DateTimeEnd.Year>1880) {
				textDateTimeEnd.Text=_commlog.DateTimeEnd.ToShortDateString()+"  "+_commlog.DateTimeEnd.ToShortTimeString();
			}
			if(!DoOmitDefaults) {
				listMode.SetSelected((int)_commlog.Mode_);
				listSentOrReceived.SetSelected((int)_commlog.SentOrReceived);
			}
			textNote.Text=_commlog.Note;//ok because no formatting characters in the commlog note.
			textNote.SelectionStart=textNote.Text.Length;
			textNote.Focus();
			butEditAutoNote.Visible=GetHasAutoNotePrompt();
			if(!ODBuild.IsDebug()) {
				labelCommlogNum.Visible=false;
				textCommlogNum.Visible=false;
			}
			textCommlogNum.Text=_commlog.CommlogNum.ToString();
			if(IsPersistent) {
				ODEvent.Fired+=PatientChangedEvent_Fired;
			}
			ODEvent.Fired+=CommItemSaveEvent_Fired;
			_isStartingUp=false;
			//Turn off any UI that could alter this Commlog.
			if(PrefC.IsODHQ) {
				if(IsHistoric) {
					DisableAllExcept(textNote);
					textNote.ReadOnly=true;//so scrolling is still enabled.
				}
				if(_commlog.IsNew || IsHistoric) {//Commlog must exist in the db in order to have history.
					butCommlogHist.Visible=false;
				}
			}
			else{//Always hide the button since it's not HQ
				butCommlogHist.Visible=false;
			}
			float scale=(float)VisualTreeHelper.GetDpi(this).DpiScaleX;
			signatureBoxWrapper.SetScaleAndZoom(scale,GetZoom());
		}

		private bool SyncCommlogWithUI(bool showMsg) {
			if(!IsValid(showMsg)) {
				return false;
			}
			_commlog.DateTimeEnd=PIn.DateT(textDateTimeEnd.Text);
			_commlog.CommDateTime=PIn.DateT(textDateTime.Text);
			//there may not be a commtype selected.
			if(listType.SelectedIndex==-1) {
				_commlog.CommType=0;
			}
			else {
				_commlog.CommType=_listDefsCommlogTypes[listType.SelectedIndex].DefNum;
			}
			_commlog.Mode_=listMode.GetSelected<CommItemMode>();
			_commlog.SentOrReceived=listSentOrReceived.GetSelected<CommSentOrReceived>();
			_commlog.Note=textNote.Text;
			if(_sigChanged) {
				string keyData=GetSignatureKey();
				try {
					_commlog.Signature=signatureBoxWrapper.GetSignature(keyData);
				}
				catch(Exception) {
					//Currently the only way for this method to fail is when saving the signature.
					if(showMsg) {
						MsgBox.Show(this,"Error saving signature.");
					}
					return false;
				}
				_commlog.SigIsTopaz=signatureBoxWrapper.GetSigIsTopaz();
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
			textNote.Text="";
		}

		private void ClearDateTimeEnd() {
			textDateTimeEnd.Text="";
		}

		private bool GetHasAutoNotePrompt() {
			return Regex.IsMatch(textNote.Text,_autoNotePromptRegex);
		}

		private void signatureBoxWrapper_SignatureChanged(object sender,EventArgs e) {
			_commlog.UserNum=Security.CurUser.UserNum;
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
			string keyData=_commlog.UserNum.ToString();
			keyData+=_commlog.CommDateTime.ToString();
			keyData+=_commlog.Mode_.ToString();
			keyData+=_commlog.SentOrReceived.ToString();
			if(_commlog.Note!=null) {
				keyData+=_commlog.Note.ToString();
			}
			return keyData;
		}

		///<summary>Validates the UI and returns a boolean indicating if the UI is in a valid state.
		///Set showMsg true in order to display a warning message regarding invalid UI prior to returning false. Otherwise; silently fails.</summary>
		private bool IsValid(bool showMsg) {
			string errorText="";
			if(String.IsNullOrEmpty(textDateTime.Text)) {
				errorText+=Lans.g(this,"Date / Time is required")+"\r\n";
			}
			else {
				try {
					DateTime.Parse(textDateTime.Text);
				}
				catch {
					errorText+=Lans.g(this,"Date / Time is invalid")+"\r\n";
				}
			}
			if(!String.IsNullOrEmpty(textDateTimeEnd.Text)) {
				try {
					DateTime.Parse(textDateTimeEnd.Text);
				}
				catch {
					errorText+=Lans.g(this,"End date is invalid")+"\r\n";
				}
			}
			if(DoOmitDefaults) { //Only validate if in enterprise mode
				if(listType.SelectedIndex==-1) {
					errorText+=Lans.g(this,"Type is required.")+"\r\n";
				}
				if(listMode.SelectedIndex==-1) {
					errorText+=Lans.g(this,"Mode is required.")+"\r\n";
				}
				if(listSentOrReceived.SelectedIndex==-1) {
					errorText+=Lans.g(this,"SentOrReceived is required.")+"\r\n";
				}
			}
			if(String.IsNullOrEmpty(errorText)) {//no errors
				return true;
			}
			if(showMsg) {
				MessageBox.Show(Lans.g(this,"Please fix the following error(s)")+":\r\n\r\n"+errorText);
			}
			return false;
		}

		///<summary>Returns true if the commlog was able to save to the database. Otherwise returns false.
		///Set showMsg to true to show a meaningful message when the commlog cannot be saved.</summary>
		private bool SaveCommItem(bool showMsg) {
			if(PrefC.IsODHQ && IsHistoric) {
				return false;// Don't actually save because it is historic.
			}
			if(!SyncCommlogWithUI(showMsg)) {
				return false;
			}
			if(IsPersistent && string.IsNullOrWhiteSpace(_commlog.Note)) { //in persistent mode, we don't want to save empty commlogs
				return false;
			}
			if(_commlogOld.IsNew || IsPersistent) {
				Commlogs.Insert(_commlog);
				_commlog.IsNew=false;
				_commlogOld=_commlog.Copy();
				textCommlogNum.Text=_commlog.CommlogNum.ToString();
				SecurityLogs.MakeLogEntry(EnumPermType.CommlogCreate,_commlog.PatNum,"");
				//Post insert persistent user preferences.
				if(!IsPersistent) {
					return true;
				}
				if(_userOdPrefClearNote==null || PIn.Bool(_userOdPrefClearNote.ValueString)) {
					ClearNote();
				}
				if(_userOdPrefEndDate==null || PIn.Bool(_userOdPrefEndDate.ValueString)) {
					ClearDateTimeEnd();
				}
				ODException.SwallowAnyException(() => {
				GlobalFormOpenDental.RefreshCurrentModule(isClinicRefresh:true);
				});
				return true;
			}
			Commlogs.Update(_commlog);
			SecurityLogs.MakeLogEntry(EnumPermType.CommlogEdit,_commlog.PatNum,"");
			return true;
		}

		private void AutoSaveCommItem() {
			if(PrefC.IsODHQ && IsHistoric) {
				return;
			}
			if(_commlogOld.IsNew) {
				//Insert
				Commlogs.Insert(_commlog);
				textCommlogNum.Text=this._commlog.CommlogNum.ToString();
				SecurityLogs.MakeLogEntry(EnumPermType.CommlogCreate,_commlog.PatNum,"Autosave");
				_commlog.IsNew=false;
			}
			else {
				//Update
				Commlogs.Update(_commlog);
			}
			_commlogOld=_commlog.Copy();
		}

		public void SaveCommlogHist(string customerNumber,CommlogHistSource commlogHistSource) {
			if(_commlog.IsNew || _commlog.CommlogNum==0) {
				return;//Don't create a hist entry if the commlog is still new. Commlogs autosave every 10 seconds so this could only happen if the commlog was created very recently.
			}
			//Synchronize _commlog with the UI before making a commloghist entry out of it.
			if(!SyncCommlogWithUI(false)) {
				return;//The UI is not in a valid state to save to the database. Do not make a commlog hist entry out of it (would also be invalid).
			}
			CommlogHist commlogHist=CommlogHists.CreateFromCommlog(_commlog);
			commlogHist.CustomerNumberRaw=customerNumber;
			commlogHist.HistSource=commlogHistSource;
			CommlogHists.Insert(commlogHist);
		}

		private void timerAutoSave_Tick(object sender,EventArgs e) {
			if(IsPersistent) {
				//Just in case the auto save timer got turned on in persistent mode.
				_dispatcherTimerAutoSave.Stop();
				return;
			}
			if(!SyncCommlogWithUI(false)) {
				return;
			}
			if(_commlogOld.Compare(_commlog)) {//They're equal, don't bother saving
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
				textCommlogNum.Text=this._commlog.CommlogNum.ToString();
			}
			this.Text=Lans.g(this,"Communication Item - Saved:")+" "+DateTime.Now;
		}

		private void butUserPrefs_Click(object sender,EventArgs e) {
			FrmCommItemUserPrefs frmCommItemUserPrefs=new FrmCommItemUserPrefs();
			frmCommItemUserPrefs.ShowDialog();
			if(frmCommItemUserPrefs.IsDialogOK) {
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
			FrmAutoNoteCompose frmAutoNoteCompose=new FrmAutoNoteCompose();
			frmAutoNoteCompose.ShowDialog();
			if(frmAutoNoteCompose.IsDialogOK) {
				textNote.Text+=frmAutoNoteCompose.StrCompletedNote;
				butEditAutoNote.Visible=GetHasAutoNotePrompt();
			}
		}

		private void butEditAutoNote_Click(object sender,EventArgs e) {
			if(!GetHasAutoNotePrompt()) {
				MessageBox.Show(Lans.g(this,"No Auto Note available to edit."));
				return;
			}
			FrmAutoNoteCompose frmAutoNoteCompose=new FrmAutoNoteCompose();
			frmAutoNoteCompose.StrMainTextNote=textNote.Text;
			frmAutoNoteCompose.ShowDialog();
			if(frmAutoNoteCompose.IsDialogOK) {
				textNote.Text=frmAutoNoteCompose.StrCompletedNote;
				butEditAutoNote.Visible=GetHasAutoNotePrompt();
			}
		}

		private void butClearNote_Click(object sender,EventArgs e) {
			textNote.Text="";
		}

		private void butCommlogHist_Click(object sender,EventArgs e) {
			FrmCommItemHistory frmCommItemHistory=new FrmCommItemHistory(_commlog.CommlogNum);
			frmCommItemHistory.ShowDialog();
		}

		private void PatientChangedEvent_Fired(ODEventArgs e) {
			if(e.EventType!=ODEventType.Patient || e.Tag.GetType()!=typeof(long)) {
				return;
			}
			//The tag for this event is the newly selected PatNum
			long patNum=(long)e.Tag;
			_commlog.PatNum=patNum;
			textPatientName.Text=Patients.GetLim(patNum).GetNameFL();
			if(IsPersistent && (_userOdPrefUpdateDateTimeNewPat==null || PIn.Bool(_userOdPrefUpdateDateTimeNewPat.ValueString))) {
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
			DispatcherTimer dispatcherTimer=new DispatcherTimer();
			dispatcherTimer.Interval=TimeSpan.FromSeconds(1.5);
			dispatcherTimer.Tick += (o,s) => {
				labelSavedManually.Visible=false;
				dispatcherTimer.Stop();
			};
			dispatcherTimer.Start();
		}

		///<summary>Tries to delete the commlog passed in. Throws exceptions if anything goes wrong.</summary>
		private void DeleteCommlog(Commlog commlog,string logText="Delete") {
			Commlogs.Delete(commlog);
			if (PrefC.IsODHQ){
				CommlogHists.DeleteForCommlog(commlog.CommlogNum);
			}
			SecurityLogs.MakeLogEntry(EnumPermType.CommlogEdit,commlog.PatNum,logText);
		}

		private void FrmCommItem_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
			if(butDelete.IsAltKey(Key.D,e)) {
				butDelete_Click(this,new EventArgs());
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_commlogOld.IsNew) {
				IsDialogOK=false;
				return;
			}
			//button not enabled if no permission and is invisible for persistent mode.
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
				return;
			}
			try {
				DeleteCommlog(_commlog);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);//Tell the user what went wrong with deleting the commlog.
			}
			IsDialogOK=true;
		}

		private void butSave_Click(object sender,EventArgs e) {
			//button not enabled if no permission
			if(!SaveCommItem(true)) {
				return;
			}
			if(IsPersistent) {
				//Show the user an indicator that the commlog has been saved but do not close the window.
				ShowManualSaveLabel();
				return;
			}
			IsDialogOK=true;
		}

		private void FrmCommItem_FormClosing(object sender,CancelEventArgs e) {
			signatureBoxWrapper?.SetTabletState(0);
			ODEvent.Fired-=CommItemSaveEvent_Fired;
			if(IsPersistent) {
				ODEvent.Fired-=PatientChangedEvent_Fired;
			}
			if(IsPersistent) {
				return;
			}
			//Only delete the commlog from the database when the user manually closes the form.
			if(!IsDialogOK && _dispatcherTimerAutoSave.IsEnabled && !_commlogOld.IsNew && _isUserClosing) {
				try {
					DeleteCommlog(_commlog,"Autosaved Commlog Deleted");
				}
				catch(Exception ex) {
					MsgBox.Show(this,ex.Message);
				}
			}
			_dispatcherTimerAutoSave.Stop();
			_dispatcherTimerAutoSave.IsEnabled=false;
		}

	}
}