using System;
using System.Windows.Forms;
using CodeBase;

namespace OpenDental.UI.Voice {
	public partial class FormMsgBox:FormODBase {
		private string _messageText;
		private MsgBoxButtons _buttons;
		private VoiceController _voiceController;
		private bool _doJustShowOk;

		private FormMsgBox() {
			InitializeComponent();
			InitializeLayoutManager();
			try {
				_voiceController=new VoiceController(VoiceCommandArea.VoiceMsgBox);
				_voiceController.SpeechRecognized+=_voiceController_SpeechRecognized;
				_voiceController.StartListening();
			}
			catch(Exception ex) {
				ex.DoNothing();//The user can just click on the dialog box instead.
			}
		}

		public FormMsgBox(string text) : this() {
			_messageText=text;
			_doJustShowOk=true;
		}

		public FormMsgBox(string text,MsgBoxButtons buttons) : this() {
			_messageText=text;
			_buttons=buttons;
		}

		private void _voiceController_SpeechRecognized(object sender,ODSpeechRecognizedEventArgs e) {
			switch(e.Command.ActionToPerform) {
				case VoiceCommandAction.Ok:
				case VoiceCommandAction.Yes:
					DialogResult=DialogResult.OK;
					break;
				case VoiceCommandAction.Cancel:
				case VoiceCommandAction.No:
					DialogResult=DialogResult.Cancel;
					break;
			}
		}

		private void FormMsgBox_Load(object sender,EventArgs e) {
			labelText.Text=_messageText;
			//add resizing later
			if (_buttons==MsgBoxButtons.YesNo) {
				butOK.Text="&Yes";
				butCancel.Text="&No";
			}
			if(_doJustShowOk) {
				butCancel.Visible=false;
				LayoutManager.MoveLocation(butOK,butCancel.Location);
			}
			_voiceController?.SayResponseAsync(_messageText);
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormMsgBox_FormClosing(object sender,FormClosingEventArgs e) {
			_voiceController?.Dispose();
		}
	}
}
