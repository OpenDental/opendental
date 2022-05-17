using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CodeBase {
	public interface IODProgress { 
		///<summary>Used within translations as the sender object</summary>
		string LanThis { get; set; }

		///<summary>Updates the status textbox with a new message.</summary>
		void UpdateProgress(string message);
		///<summary>Updates the progress bar with these details.</summary>
		void UpdateProgressDetailed(string labelValue,string percentVal="",string tagString="",int barVal=0,int barMax=100,int marqSpeed=0
			,string labelTop="",bool isLeftHidden=false,bool isTopHidden=false,bool isPercentHidden=false
			,ProgBarStyle progStyle=ProgBarStyle.Blocks,ProgBarEventType progEvent=ProgBarEventType.ProgressBar);
	}

	///<summary>Progress bar where all methods do nothing. Can be used when there can be no progress bar due to a lack of UI. 
	///This is an implementation of the Null Object Pattern.</summary>
	public class ODProgressDoNothing:IODProgress {
		///<summary>A singleton instance of this class that can be used repeatedly so that each consumer doesn't have to create a new object.</summary>
		private static ODProgressDoNothing _instance=new ODProgressDoNothing();
		[XmlIgnore]
		public static ODProgressDoNothing Instance {
			get {
				return _instance;
			}
		}

		public string LanThis {
			get {
				return "ODProgressDoNothing";
			}
			set { }
		}

		public void UpdateProgress(string message) {	}

		public void UpdateProgressDetailed(string labelValue,string percentVal="",string tagString="",int barVal=0,int barMax=100,int marqSpeed=0,
			string labelTop="",bool isLeftHidden=false,bool isTopHidden=false,bool isPercentHidden=false,ProgBarStyle progStyle=ProgBarStyle.Blocks,
			ProgBarEventType progEvent=ProgBarEventType.ProgressBar) { }

		public void Close() { }
	}

	public enum ProgBarStyle {
		NoneSpecified,
		///<summary>Usually used for percentage based progress.</summary>
		Blocks,
		///<summary>Usually used to indicate ongoing processing (no percentage).</summary>
		Marquee,
		///<summary>This is an older option not supported by Window when visual styles are not enabled. Do not use.</summary>
		Continuous
	}

	///<summary>When a progress bar event fires, use these event types to indicate to FormProgressExtended what needs to happen.
	///These events can cause progress bars to change or cause the form itself to behave in specific ways.
	///E.g. Use BringToFront to cause FormProgressExtended to come to the front of all other windows.
	///The most common event will be ProgressBar in order to update the progress visually to the user.  It needs to be the first in the enum.</summary>
	public enum ProgBarEventType {
		///<summary>The default event type.  Adds a new progress bar or updates a current progress bar with the corresponding settings.</summary>
		ProgressBar,
		///<summary>Brings FormProgressExtended to the front.</summary>
		BringToFront,
		///<summary>Sets the Text property of FormProgressExtended to LabelValue.</summary>
		Header,
		///<summary>Sets the Text property of the label just above the text box to LabelValue in FormProgressExtended.</summary>
		ProgressLog,
		///<summary>Appends LabelValue to the text box in FormProgressExtended.</summary>
		TextMsg,
		///<summary>Hides the warning label and enables the Pause button in FormProgressExtended and then sets the Text property of said button to "Resume".</summary>
		WarningOff,
		///<summary>Does the same thing as the WarningOff event.</summary>
		AllowResume,
		///<summary>Sets the Text property of the Close button in FormProgressExtended to "Close" and disables the pause button.</summary>
		Done,
		///<summary>Sets pause and cancel buttons on the bottom of the form to invisible. </summary>
		HideButtons
	}
}
