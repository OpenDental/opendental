using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;

namespace OpenDental.UI.Voice {
	public class VoiceController:IDisposable {
		///<summary>True if the controller is listening for audio input.</summary>
		public bool IsListening { get; private set; }
		///<summary>The mimimum level of confidence the recognizer must have to act upon audio input.</summary>
		public double ConfidenceThreshold=0.9;
		///<summary>Event is fired when the speech is recognized.</summary>
		public event ODSpeechRecognizedEventHandler SpeechRecognized=null;
		public delegate void ODSpeechRecognizedEventHandler(object sender,ODSpeechRecognizedEventArgs e);
		private SpeechRecognitionEngine _recEngine=new SpeechRecognitionEngine();
		private SpeechSynthesizer _synth=new SpeechSynthesizer();
		///<summary>The list of commands that this controller is listening for.</summary>
		private readonly List<VoiceCommand> _listCommands;
		///<summary>True if the controller will give verbal feedback.</summary>
		private bool _isGivingFeedback;

		///<summary>Throws an exception of unable to find a suitable microphone.</summary>
		public VoiceController(VoiceCommandArea area,bool doIncludeGlobal=true,bool isGivingFeedback=true) {
			_isGivingFeedback=isGivingFeedback;
			List<VoiceCommandArea> listAreas=new List<VoiceCommandArea> { area };
			if(doIncludeGlobal) {
				listAreas.Add(VoiceCommandArea.Global);
			}
			_listCommands=VoiceCommandList.GetCommands(listAreas);
			Choices commandChoices=new Choices();
			commandChoices.Add(_listCommands.SelectMany(x => x.Commands).ToArray());
			// Create a GrammarBuilder object and append the Choices object.
			GrammarBuilder gb=new GrammarBuilder();
			gb.Append(commandChoices);
			// Create the Grammar instance and load it into the speech recognition engine.
			Grammar g=new Grammar(gb);
			_recEngine=new SpeechRecognitionEngine();
			_recEngine.LoadGrammarAsync(g);
			_recEngine.SetInputToDefaultAudioDevice();
			_recEngine.RecognizeAsync(RecognizeMode.Multiple);//Recognize speech until we tell it to stop
			_recEngine.SpeechRecognized+=RecEngine_SpeechRecognized;
			_synth.SetOutputToDefaultAudioDevice();
			_synth.SelectVoiceByHints(VoiceGender.Female);
			IsListening=true;
		}

		///<summary>This methods sets the controller to accept audio input.</summary>
		public void StartListening() {
			IsListening=true;
		}

		///<summary>This methods sets the controller to not accept audio input.</summary>
		public void StopListening() {
			IsListening=false;
		}

		protected void RecEngine_SpeechRecognized(object sender,SpeechRecognizedEventArgs e) {
			VoiceCommand voiceCommand=_listCommands.FirstOrDefault(x => x.Commands.Contains(e.Result.Text))?.Copy();
			if(voiceCommand==null) {
				return;
			}
			voiceCommand.DoSayResponse=false;
			if(e.Result.Confidence<ConfidenceThreshold) {
				voiceCommand.ActionToPerform=VoiceCommandAction.DidntGetThat;
				voiceCommand.Response="I didn't get that";
			}
			switch(voiceCommand.ActionToPerform) {
				case VoiceCommandAction.GiveFeedback:
					if(IsListening) {
						_isGivingFeedback=true;
						SayResponseAsync(voiceCommand.Response);
					}
					break;
				case VoiceCommandAction.StopGivingFeedback:
					if(IsListening) {
						SayResponseAsync(voiceCommand.Response);
						_isGivingFeedback=false;
					}
					break;
				case VoiceCommandAction.DidntGetThat:
					if(IsListening) {
						SayResponseAsync(voiceCommand.Response);
					}
					break;
				case VoiceCommandAction.StartListening:
					IsListening=true;
					SayResponseAsync(voiceCommand.Response);
					break;
				case VoiceCommandAction.StopListening:
					IsListening=false;
					SayResponseAsync(voiceCommand.Response);
					break;
				default:
					voiceCommand.DoSayResponse=true;
					break;
			}
			if(!IsListening && voiceCommand.ActionToPerform!=VoiceCommandAction.StopListening) {
				return;
			}
			SpeechRecognized?.Invoke(this,new ODSpeechRecognizedEventArgs(voiceCommand));
		}
		
		public void SayResponseAsync(string response) {
			if(_isGivingFeedback && !string.IsNullOrEmpty(response)) {
				_synth.SpeakAsync(response);
			}
		}

		public void Dispose() {
			_recEngine?.Dispose();
			_synth?.Dispose();
		}

	}

	public class ODSpeechRecognizedEventArgs {
		public VoiceCommand Command;

		public ODSpeechRecognizedEventArgs(VoiceCommand command) {
			Command=command;
		}
	}
}
