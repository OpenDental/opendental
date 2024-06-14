using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.OpenAi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormAiChatSession:FormODBase {
		private FormWebChatSessionNoteEdit _formWebChatSessionNoteEdit=null;
		///<summary>This is used for redraw logic, see references.</summary>
		private int _widthPrevious;
		///<summary>The msg count as of the last time the user looked at it.</summary>
		private int _countMsgsViewed;
		///<summary>whether or not viewed by user.</summary>
		private int _countMsgsActual;
		///<summary>There is one OpenAiChatSession per instance of FormAiChatSession.cs.
		///This object contains all of the chat/instance specific properties and data needed for OpenAi chat assistants. </summary>
		private OpenAiChatSession _openAiChatSession=new OpenAiChatSession();
		private bool _isPreviousSession=false;
		private List<WebChatMessage> _listWebChatMessages;
		///<summary>The initial message that can be set when this window opens, defaults to null.
		///When this form loads with an initial message via the constructor, the message is sent automatically while form loads., </summary>
		private string _initialMessage;
		private Action _actionClose=null;

		public FormAiChatSession(WebChatSession session,Action actionClose) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_openAiChatSession.ChatSession=session;
			_isPreviousSession=true;
			_actionClose=actionClose;
		}

		public FormAiChatSession(string message=null,Action actionClose=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_initialMessage=message;
			_actionClose=actionClose;
		}

		private async void FormWebChatSession_Load(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.AiChatSession)) {
				Close();
				return;
			}
			try {
				if(_openAiChatSession.ChatSession!=null) {
					if(_openAiChatSession.ChatSession.DateTend.Year > 1880) {//Session has ended?
						DisableAllExcept(textOwner,webChatThread,tabControlMain);
						//tabControlMain needs to be enabled to view notes, but these options still need to be disabled.
						butSend.Enabled=false;
						textChatMessage.Enabled=false;
					}
				}
				butAddNote.Visible=false;
				comboVersions.Items.AddList(await _openAiChatSession.GetListAssistants(),(x) => x?.Name??"ERROR");
				comboVersions.SetSelected(0);
				if(!_initialMessage.IsNullOrEmpty()) {
					await SendMessage(_initialMessage);
				}
				FillSession();
				FillGridODNotes();
				FillMessageThread();
				_countMsgsViewed=_countMsgsActual;
				SetTabColor();
				_widthPrevious=((Control)sender).Size.Width;
				if(_isPreviousSession) {//Loading a previously created AI chat session, we need to re-load the message context.
					await _openAiChatSession.ResumeChatSession(_openAiChatSession.ChatSession,_listWebChatMessages);
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
				//Either an error from _openAiChatSession.GetAssistantDict() or _openAiChatSession.ResumeChatSession(...)
				MessageBox.Show(this,$"There was an error loading the window.");
				Close();
			}
		}

		private void FillGridODNotes() {
			if(_openAiChatSession.ChatSession==null) {
				return;
			}
			if(gridODNotes.Columns.Count==0) {
				GridColumn col=new GridColumn("Date",65);
				gridODNotes.Columns.Add(col);
				col=new GridColumn("Tech",85);
				gridODNotes.Columns.Add(col);
				col=new GridColumn("Note",90);
				gridODNotes.Columns.Add(col);
			}
			List<WebChatNote> listWebChatNotes=WebChatNotes.GetAllForSessions(new List<long>() {_openAiChatSession.ChatSession.WebChatSessionNum });
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
			if(_openAiChatSession.ChatSession==null) {
				return;
			}
			if(!listSignalods.Exists(x => x.IType==InvalidType.WebChatSessions && x.FKey==_openAiChatSession.ChatSession.WebChatSessionNum)) {
				return;
			}
			_openAiChatSession.ChatSession=WebChatSessions.GetOne(_openAiChatSession.ChatSession.WebChatSessionNum);//Refresh the session in case the owner changed.
			FillSession();
			FillGridODNotes();
			FillMessageThread();
			SetTabColor();
		}

		private void FillSession() {
			if(_openAiChatSession.ChatSession==null) {
				return;
			}
			textOwner.Text=_openAiChatSession.ChatSession.TechName;
		}

		private void FillMessageThread() {
			if(_openAiChatSession.ChatSession==null) {
				return;
			}
			_listWebChatMessages=WebChatMessages.GetAllForSessions(_openAiChatSession.ChatSession.WebChatSessionNum);
			List<SmsThreadMessage> listSmsThreadMessages=new List<SmsThreadMessage>();
			for(int i=0;i<_listWebChatMessages.Count;i++) {
				WebChatMessage message=_listWebChatMessages[i];
				string strMessageText=message.MessageText;
				if(message.MessageType==WebChatMessageType.EndSession) {
					strMessageText=MarkupEdit.ConvertToPlainText(strMessageText);
					butEndSession.Enabled=false;
					butSend.Enabled=false;
					textChatMessage.Text="";
					textChatMessage.Enabled=false;
				}
				listSmsThreadMessages.Add(
					new SmsThreadMessage(message.WebChatMessageNum.ToString(),
						message.DateT,
						strMessageText,
						isAlignedLeft:(message.MessageType==WebChatMessageType.Ai),
						isImportant:false,
						isHighlighted:false,
						SmsDeliveryStatus.None,
						message.UserName,
						isAiMessage:(message.MessageType==WebChatMessageType.Ai),
						message
					)
				);
			}
			webChatThread.ListSmsThreadMessages=listSmsThreadMessages;
			_countMsgsActual=_listWebChatMessages.Count;
		}

		private async void textChatMessage_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.Enter && !e.Shift) {
				e.Handled=true;
				await SendMessage(textChatMessage.Text);
			}
		}

		private async Task<bool> SendMessage(params string[] msgs) {
			if(!(comboVersions.SelectedItem is OAIAssistant selectedAssistant)) {
				return false;
			}
			UseWaitCursor=true;
			butSend.Enabled=false;
			textChatMessage.Enabled=false;
			butSend.Text="Loading...";
			string error=await _openAiChatSession.SendMessagesAsync(selectedAssistant.Id,msgs);
			if(!error.IsNullOrEmpty()) {
				MessageBox.Show(error);
			}
			FillSession();
			FillGridODNotes();
			textChatMessage.Text="";
			FillMessageThread();
			_countMsgsViewed=_countMsgsActual;
			SetTabColor();
			butSend.Text="Send";
			butSend.Enabled=true;
			textChatMessage.Enabled=true;
			UseWaitCursor=false;
			return error.IsNullOrEmpty();
		}

		private void SetTabColor(){
			if(_countMsgsViewed==_countMsgsActual){
				tabPageMessages.ColorTab=Color.Empty;
				return;
			}
			tabPageMessages.ColorTab=Color.LightCoral;
		}

		private async void butSend_Click(object sender,EventArgs e) {
			await SendMessage(textChatMessage.Text);
		}

		private void butEndSession_Click(object sender,EventArgs e) {
			//Refresh the session in case the session ended in less than the last signal interval.
			_openAiChatSession.ChatSession=WebChatSessions.GetOne(_openAiChatSession.ChatSession.WebChatSessionNum);
			if(_openAiChatSession.ChatSession.DateTend.Year>1880) { //session has already ended. Will not send second End Session message
				FillMessageThread();
				SetTabColor();
				butEndSession.Enabled=false;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Permanently end the session for everyone?")) {
				return;
			}
			WebChatSessions.EndSession(_openAiChatSession.ChatSession.WebChatSessionNum);
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butTakeOwnership_Click(object sender,EventArgs e) {
			TakeOwnership();
		}

		private void TakeOwnership() {
			//Refresh the session in case the owner changed in less than the last signal interval.
			_openAiChatSession.ChatSession=WebChatSessions.GetOne(_openAiChatSession.ChatSession.WebChatSessionNum);
			bool isOkToTake=true;
			if(_openAiChatSession.ChatSession.TechName==Security.CurUser.UserName) {
				//Ownership is already claimed.  Do not run a pointless update command.
				isOkToTake=false;
			}
			else if(!String.IsNullOrEmpty(_openAiChatSession.ChatSession.TechName)) {
				if(MessageBox.Show(Lan.g(this,"This session is owned by another technician.  Take this session from user")+" "+_openAiChatSession.ChatSession.TechName+"?",
					"",MessageBoxButtons.YesNo)!=DialogResult.Yes)
				{
					isOkToTake=false;
				}
			}
			if(isOkToTake) {
				WebChatSession webChatSessionOld=_openAiChatSession.ChatSession.Clone();
				_openAiChatSession.ChatSession.TechName=Security.CurUser.UserName;
				WebChatSessions.Update(_openAiChatSession.ChatSession,webChatSessionOld);
			}
			textOwner.Text=_openAiChatSession.ChatSession.TechName;
			if(isOkToTake) {//show after previous text has changed
				MessageBox.Show(Lan.g(this,"Session is now owned by")+" "+_openAiChatSession.ChatSession.TechName);
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
			webChatNote.WebChatSessionNum=_openAiChatSession.ChatSession.WebChatSessionNum;
			webChatNote.TechName=_openAiChatSession.ChatSession.TechName;
			webChatNote.DateTimeNote=DateTime.Now;
			webChatNote.IsNew=true;
			string strNoteOld=webChatNote.Note;
			_formWebChatSessionNoteEdit=new FormWebChatSessionNoteEdit(webChatNote);
			_formWebChatSessionNoteEdit.ShowDialog(this);
			_formWebChatSessionNoteEdit.Dispose();
			string strNoteNew=webChatNote.Note;
			if(strNoteOld!=strNoteNew) {
				string msg="Web Chat Session ("+webChatNote.WebChatSessionNum.ToString()+") Note has been created";
				SecurityLogs.MakeLogEntry(EnumPermType.WebChatEdit,_openAiChatSession.ChatSession.PatNum,msg);
			}
			FillGridODNotes();
		}

		private void gridODNotes_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			if(HasAnotherNoteOpen() || gridODNotes.SelectedTag<WebChatNote>()==null) {
				return;
			}
			long webChatNoteNum=gridODNotes.SelectedTag<WebChatNote>().WebChatNoteNum;
			WebChatNote webChatNote=WebChatNotes.GetOne(webChatNoteNum);
			if(webChatNote==null) {
				MsgBox.Show(this,"This web chat session note was deleted from elsewhere. Unable to display.");
				return;
			}
			string strNoteOld=webChatNote.Note;
			_formWebChatSessionNoteEdit=new FormWebChatSessionNoteEdit(webChatNote);
			_formWebChatSessionNoteEdit.ShowDialog(this);
			_formWebChatSessionNoteEdit.Dispose();
			string strNoteNew=webChatNote.Note;
			if(strNoteOld!=strNoteNew || _formWebChatSessionNoteEdit.IsDeleted) {
				string msg="Web Chat Session ("+webChatNote.WebChatSessionNum.ToString()+") Note has been edited";
				SecurityLogs.MakeLogEntry(EnumPermType.WebChatEdit,_openAiChatSession.ChatSession.PatNum,msg);
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
			if(_openAiChatSession.ChatSession!=null) { 
				WebChatSession webChatSessionOld=_openAiChatSession.ChatSession.Clone();
				WebChatSessions.Update(_openAiChatSession.ChatSession,webChatSessionOld);
			}
			_actionClose?.Invoke();
		}

		///<summary> When resizing the form, redraw all messages if the width changed.
		///A redraw is necessary to update the width and height of each message.
		///If the width didn't change, no need to redraw. </summary>
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