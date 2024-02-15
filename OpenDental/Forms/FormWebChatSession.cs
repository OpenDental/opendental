using System;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using CodeBase;
using System.Drawing;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormWebChatSession:FormODBase {
		private WebChatSession _webChatSession=null;
		private FormWebChatSessionNoteEdit _formWebChatSessionNoteEdit=null;
		private Action _actionClose=null;
		private Patient _patientSuggested=null;
		private int _widthPrevious;
		///<summary>The msg count as of the last time the user looked at it.</summary>
		private int _countMsgsViewed;
		///<summary>whether or not viewed by user.</summary>
		private int _countMsgsActual;

		///<summary>This form is only displayed when accessed from the triage list at ODHQ.</summary>
		public FormWebChatSession(WebChatSession webChatSession,Action actionClose) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_webChatSession=webChatSession;
			_actionClose=actionClose;
		}

		private void FormWebChatSession_Load(object sender,EventArgs e) {
			if(_webChatSession.DateTend.Year > 1880) {//Session has ended?
				DisableAllExcept(textOwner,textWebChatSessionNum,textName,textEmail,textPractice,textPhone,webChatThread,butAttachSuggestion,butSearchAndAttach,textCustomer
					,tabControlMain);
				//tabControlMain needs to be enabled to view notes, but these options still need to be disabled.
				butSend.Enabled=false;
				textChatMessage.Enabled=false;
			}
			//auto-suggest patient based on supplied phone number if a patient is not yet associated with the session
			if(_webChatSession.PhoneNumber!="" && _webChatSession.PatNum==0) {
				List<Patient> listPatients=new List<Patient>();
				listPatients.AddRange(Patients.GetPatientsByPhone(_webChatSession.PhoneNumber,""));
				if(listPatients.Count==1) { //only one patient matches the supplied phone number
					_patientSuggested=listPatients.FirstOrDefault(); //If patient is null revert to original form behavior if finding a customer fails
					butAttachSuggestion.Enabled=true;
				}
			}
			butAddNote.Visible=false;
			FillSession();
			FillGridODNotes();
			FillMessageThread();
			_countMsgsViewed=_countMsgsActual;
			SetTabColor();
			_widthPrevious=((Control)sender).Size.Width;
		}

		private void FillGridODNotes() {
			if(gridODNotes.Columns.Count==0) {
				GridColumn col=new GridColumn("Date",65);
				gridODNotes.Columns.Add(col);
				col=new GridColumn("Tech",85);
				gridODNotes.Columns.Add(col);
				col=new GridColumn("Note",90);
				gridODNotes.Columns.Add(col);
			}
			List<WebChatNote> listWebChatNotes=WebChatNotes.GetAllForSessions(new List<long>() {_webChatSession.WebChatSessionNum });
			GridRow row;
			GridCell cell;
			gridODNotes.BeginUpdate();
			gridODNotes.ListGridRows.Clear();
			for(int i=0;i<listWebChatNotes.Count;i++) {
				row=new GridRow();
				cell=new GridCell(listWebChatNotes[i].DateTimeNote.ToShortDateString());
				row.Cells.Add(cell);
				cell=new GridCell(listWebChatNotes[i].TechName);
				row.Cells.Add(cell);
				cell=new GridCell(listWebChatNotes[i].Note);
				row.Cells.Add(cell);
				row.Tag=listWebChatNotes[i];
				gridODNotes.ListGridRows.Add(row);
			}
			gridODNotes.EndUpdate();
		}

		public override void ProcessSignalODs(List<Signalod> listSignalods) {
			if(!listSignalods.Exists(x => x.IType==InvalidType.WebChatSessions && x.FKey==_webChatSession.WebChatSessionNum)) {
				return;
			}
			_webChatSession=WebChatSessions.GetOne(_webChatSession.WebChatSessionNum);//Refresh the session in case the owner changed.
			FillSession();
			FillGridODNotes();
			FillMessageThread();
			SetTabColor();
		}

		private void FillSession() {
			if(_patientSuggested!=null) { //using suggested patient information if present
				textCustomer.Text=_patientSuggested.GetNameFL();
			}
			else {
				textCustomer.Text=(_webChatSession.PatNum==0) ? "" : Patients.GetNameLF(_webChatSession.PatNum);
			}
			textOwner.Text=_webChatSession.TechName;
			textName.Text=_webChatSession.UserName;
			textPractice.Text=_webChatSession.PracticeName;
			textWebChatSessionNum.Text=POut.Long(_webChatSession.WebChatSessionNum);
			textEmail.Text=_webChatSession.EmailAddress;
			textPhone.Text=_webChatSession.PhoneNumber;
			checkIsCustomer.Checked=_webChatSession.IsCustomer;
		}

		private void FillMessageThread() {
			List<WebChatMessage> listWebChatMessages=WebChatMessages.GetAllForSessions(_webChatSession.WebChatSessionNum);
			List<SmsThreadMessage> listSmsThreadMessages=new List<SmsThreadMessage>();
			for(int i=0;i<listWebChatMessages.Count;i++) {
				string strMessageText=listWebChatMessages[i].MessageText;
				if(listWebChatMessages[i].MessageType==WebChatMessageType.EndSession) {
					strMessageText=MarkupEdit.ConvertToPlainText(strMessageText);
					butEndSession.Enabled=false;
					butSend.Enabled=false;
					textChatMessage.Text="";
					textChatMessage.Enabled=false;
				}
				SmsThreadMessage smsThreadMessage=new SmsThreadMessage(listWebChatMessages[i].WebChatMessageNum.ToString(),
					listWebChatMessages[i].DateT,
					strMessageText,
					isAlignedLeft:(listWebChatMessages[i].MessageType==WebChatMessageType.Customer),
					isImportant:false,
					isHighlighted:false,
					SmsDeliveryStatus.None,
					listWebChatMessages[i].UserName
				);
				listSmsThreadMessages.Add(smsThreadMessage);
			}
			webChatThread.ListSmsThreadMessages=listSmsThreadMessages;
			_countMsgsActual=listWebChatMessages.Count;
		}

		private void textChatMessage_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.Enter && !e.Shift) {
				e.Handled=true;
				SendMessage();
			}
		}

		private void SendMessage() {
			WebChatSessions.SendTechMessage(_webChatSession.WebChatSessionNum,textChatMessage.Text);
			textChatMessage.Text="";
			FillMessageThread();
			_countMsgsViewed=_countMsgsActual;
			SetTabColor();
		}

		private void SetTabColor(){
			if(_countMsgsViewed==_countMsgsActual){
				tabPageMessages.ColorTab=Color.Empty;
				return;
			}
			tabPageMessages.ColorTab=Color.LightCoral;
		}

		private void butSend_Click(object sender,EventArgs e) {
			SendMessage();
		}

		private void butSearchAndAttach_Click(object sender,EventArgs e) {
			//prefill FormPatientSelect with the customer-provided phone number
			Patient patient=new Patient() {HmPhone=textPhone.Text};
			using FormPatientSelect formPatientSelect=new FormPatientSelect(patient);
			if(formPatientSelect.ShowDialog()!=DialogResult.OK) {
				return;
			}
			patient=Patients.GetPat(formPatientSelect.PatNumSelected);
			GlobalFormOpenDental.PatientSelected(patient,true);
			textCustomer.Text=patient.GetNameLF();
			WebChatSession webChatSessionOld=_webChatSession.Clone();
			_webChatSession.PatNum=patient.PatNum;
			WebChatSessions.Update(_webChatSession,webChatSessionOld);//update here so we can associate pats after chat has ended, or before ownership. Updated immediately for concurrency. 
			butAttachSuggestion.Enabled=false; //Attached searched-for patient instead of suggested one.
			_patientSuggested=null;
		}

		///<summary> Attaches a customer to the session if one has been suggested. </summary>
		private void butAttachSuggestion_Click(object sender,EventArgs e) {
			WebChatSession webChatSessionOld=_webChatSession.Clone();
			_webChatSession.PatNum=_patientSuggested.PatNum;
			WebChatSessions.Update(_webChatSession,webChatSessionOld);//Updated immediately for concurrency. 
			butAttachSuggestion.Enabled=false;
			GlobalFormOpenDental.PatientSelected(_patientSuggested,true);
		}

		private void butEndSession_Click(object sender,EventArgs e) {
			//Refresh the session in case the session ended in less than the last signal interval.
			_webChatSession=WebChatSessions.GetOne(_webChatSession.WebChatSessionNum);
			if(_webChatSession.DateTend.Year>1880) { //session has already ended. Will not send second End Session message
				FillMessageThread();
				SetTabColor();
				butEndSession.Enabled=false;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Permanently end the session for everyone?")) {
				return;
			}
			WebChatSessions.EndSession(_webChatSession.WebChatSessionNum);
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butTakeOwnership_Click(object sender,EventArgs e) {
			TakeOwnership();
		}

		private void TakeOwnership() {
			//Refresh the session in case the owner changed in less than the last signal interval.
			_webChatSession=WebChatSessions.GetOne(_webChatSession.WebChatSessionNum);
			bool isOkToTake=true;
			if(_webChatSession.TechName==Security.CurUser.UserName) {
				//Ownership is already claimed.  Do not run a pointless update command.
				isOkToTake=false;
			}
			else if(!String.IsNullOrEmpty(_webChatSession.TechName)) {
				if(MessageBox.Show(Lan.g(this,"This session is owned by another technician.  Take this session from user")+" "+_webChatSession.TechName+"?",
					"",MessageBoxButtons.YesNo)!=DialogResult.Yes)
				{
					isOkToTake=false;
				}
			}
			if(isOkToTake) {
				WebChatSession webChatSessionOld=_webChatSession.Clone();
				_webChatSession.TechName=Security.CurUser.UserName;
				WebChatSessions.Update(_webChatSession,webChatSessionOld);
			}
			textOwner.Text=_webChatSession.TechName;
			if(isOkToTake) {//show after previous text has changed
				MessageBox.Show(Lan.g(this,"Session is now owned by")+" "+_webChatSession.TechName);
			}
		}

		private bool HasAnotherNoteOpen() {
			if(_formWebChatSessionNoteEdit!=null && !_formWebChatSessionNoteEdit.IsDisposed) {
				MsgBox.Show(this,"One or more web chat session note edit windows are open and must be closed.");
				return true;
			}
			return false;
		}

		private void butAddNote_Click(object sender,EventArgs e) {
			if(HasAnotherNoteOpen()) {
				return;
			}
			WebChatNote webChatNote=new WebChatNote();
			webChatNote.WebChatSessionNum=_webChatSession.WebChatSessionNum;
			webChatNote.TechName=_webChatSession.TechName;
			webChatNote.DateTimeNote=DateTime.Now;
			webChatNote.IsNew=true;
			string strNoteOld= webChatNote.Note;
			_formWebChatSessionNoteEdit=new FormWebChatSessionNoteEdit(webChatNote);
			_formWebChatSessionNoteEdit.ShowDialog(this);
			_formWebChatSessionNoteEdit.Dispose();
			string strNoteNew=webChatNote.Note;
			if(strNoteOld!=strNoteNew) {
				string msg="Web Chat Session ("+webChatNote.WebChatSessionNum.ToString()+") Note has been created";
				SecurityLogs.MakeLogEntry(EnumPermType.WebChatEdit,_webChatSession.PatNum,msg);
			}
			FillGridODNotes();
		}

		private void gridODNotes_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			if(HasAnotherNoteOpen() || gridODNotes.SelectedTag<WebChatNote>()==null) {
				return;
			}
			long webChatNoteNum=gridODNotes.SelectedTag<WebChatNote>().WebChatNoteNum;//(WebChatSession)gridWebChatSurveys.ListGridRows[e.Row].Tag
			WebChatNote webChatNote=WebChatNotes.GetOne(webChatNoteNum);
			if(webChatNote==null) {
				MsgBox.Show(this,"This web chat session note was deleted from elsewhere. Unable to display.");
				return;
			}
			string strNoteOld= webChatNote.Note;
			_formWebChatSessionNoteEdit=new FormWebChatSessionNoteEdit(webChatNote);
			_formWebChatSessionNoteEdit.ShowDialog(this);
			_formWebChatSessionNoteEdit.Dispose();
			string strNoteNew=webChatNote.Note;
			if(strNoteOld!=strNoteNew || _formWebChatSessionNoteEdit.IsDeleted) {
				string msg="Web Chat Session ("+webChatNote.WebChatSessionNum.ToString()+") Note has been edited";
				SecurityLogs.MakeLogEntry(EnumPermType.WebChatEdit,_webChatSession.PatNum,msg);
			}
			FillGridODNotes();
		}

		private void tabControlMain_SelectedIndexChanged(object sender,EventArgs e) {
			if(tabControlMain.SelectedTab==tabPageNotes) {
				butAddNote.Visible=true;
				FillGridODNotes();
				return;
			}
			//clicked messaging tab
			butAddNote.Visible=false;//No need to add notes from the messaging screen.
			_countMsgsViewed=_countMsgsActual;
			SetTabColor(); 
		}

		private void FormWebChatSession_FormClosing(object sender,FormClosingEventArgs e) {
			WebChatSession webChatSessionOld=_webChatSession.Clone();
			WebChatSessions.Update(_webChatSession,webChatSessionOld);
			_actionClose();
		}

		///<summary> When resizing the form, redraw all messages if the width changed.
		/// A redraw is necessary to update the width and height of each message.
		/// If the width didn't change, no need to redraw. </summary>
		private void FormWebChatSession_ResizeEnd(object sender,EventArgs e) {
			int widthPrevious=((Control)sender).Size.Width;
			if(widthPrevious==_widthPrevious) {
				return;
			}
			FillMessageThread();
			SetTabColor();
			FillGridODNotes();
			_widthPrevious=widthPrevious;
		}
	}
}