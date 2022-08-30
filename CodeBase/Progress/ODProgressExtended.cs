using System;
using System.Threading;
using System.Windows.Forms;

namespace CodeBase {
	///<summary>A wrapper for FormProgressExtended.</summary>
	public class ODProgressExtended : IODProgressExtended {
		private Action _actionCloser;
		private IODEvent _event;
		private ProgBarStyle _progBarStyle;
		private ODEventType _odEventType;

		public bool IsPaused { get; private set; }

		public bool IsCanceled { get; private set; }
		
		public string LanThis {	get; set; }

		///<param name="currentForm">The form to activate once the progress is done. If you cannot possibly pass in a form, it is okay to pass in null.
		///</param>
		public ODProgressExtended(ODEventType odEventType,IODEvent odEvent,Form currentForm,object tag=null,ProgBarStyle progBarStyle=ProgBarStyle.Blocks,
			string lanThis="ProgressExtended",string cancelButtonText=null)
		{
			_actionCloser=ODProgress.ShowExtended(odEventType,odEvent.GetType(),currentForm,tag,
				new ProgressCanceledHandler((object sender,EventArgs e) => {
					IsCanceled=true;
				}),
				new ProgressPausedHandler((object sender,ProgressPausedArgs e) => {
					IsPaused=e.IsPaused;
				}),cancelButtonText);
			_event=odEvent;
			_progBarStyle=progBarStyle;
			_odEventType=odEventType;
			LanThis=lanThis;
		}

		public void AllowResume() {//resume progress
			_event.FireEvent(new ODEventArgs(_odEventType,new ProgressBarHelper((""),progressBarEventType:ProgBarEventType.AllowResume)));
		}

		public void HideButtons() {//hide pause and cancel buttons
			_event.FireEvent(new ODEventArgs(_odEventType,new ProgressBarHelper((""),progressBarEventType:ProgBarEventType.HideButtons)));
		}

		public void OnProgressDone() {//shows "Close" if not visible already
			_event.FireEvent(new ODEventArgs(_odEventType,new ProgressBarHelper((""),progressBarEventType:ProgBarEventType.Done)));
		}

		public void Close() {
			_actionCloser?.Invoke();
		}

		public void Fire(ODEventType odEventType,object tag) {
			Fire(new ODEventArgs(odEventType,tag));
		}

		public void Fire(ODEventArgs e) {
			if(e.Tag is ProgressBarHelper) {
				ProgressBarHelper progBarHelper=(ProgressBarHelper)e.Tag;
				if(progBarHelper.ProgressStyle==ProgBarStyle.NoneSpecified) {
					progBarHelper.ProgressStyle=_progBarStyle;
				}
			}
			_event.FireEvent(e);
		}

		/// <summary>Initializes a new bar or updates the progress bar with more specific information</summary>
		public void UpdateProgressDetailed(string labelValue,string tagString,string percentVal="",int barVal=0, int barMax=100,int marqSpeed=0
			,string labelTop="",bool isLeftHidden=false,bool isTopHidden=false,bool isPercentHidden=false
			,ProgBarStyle progStyle=ProgBarStyle.Blocks,ProgBarEventType progEvent=ProgBarEventType.ProgressBar) 
		{
			_event.FireEvent(new ODEventArgs(_odEventType,new ProgressBarHelper(labelValue,percentVal,barVal,barMax,progStyle
				,tagString,marqSpeed,labelTop,isLeftHidden,isTopHidden,isPercentHidden,progressBarEventType:progEvent)));
		}

		public void UpdateProgress(string message) {
			_event.FireEvent(new ODEventArgs(_odEventType,new ProgressBarHelper(message,progressBarEventType:ProgBarEventType.TextMsg)));
		}

		/// <summary>
		/// Updates progress bar with a top label and a percent value. Tagstring needs to be associated to the bar that is reciving the change. 
		/// </summary>
		public void UpdateProgress(string labelTop,string tagstring,string percentVal="",int barVal=0,int barMax=100
			,bool isTopHidden=false,string labelValue="") {
			_event.FireEvent(new ODEventArgs(_odEventType,new ProgressBarHelper(labelValue,percentVal,barVal,barMax,tagString:tagstring,labelTop:labelTop
				,isTopHidden:isTopHidden)));
		}

		/// <summary>Checks if progress is paused and waits if it is. If progress gets canceled (while paused or not) returns true</summary>
		public bool IsPauseOrCancel() {
			while(IsPaused) {
				AllowResume();
				Thread.Sleep(10);
				if(IsCanceled) {
					break;
				}
			}
			if(IsCanceled) {
				return true; //if progress was canceled notifiy instantiator of a cancellation. Do not continue.
			}
			else {
				return false; //progress was paused but is no longer and no cancellation occurred, OK to continue
			}
		}
	}
}
