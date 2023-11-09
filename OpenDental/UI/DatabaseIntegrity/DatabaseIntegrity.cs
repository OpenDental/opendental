using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDental {
	///<summary>This is a database table in the HQ bugs database. There is no generated crud code. In the db, all the columns except PK are strings.</summary>
	public class DatabaseIntegrity {
		public long DatabaseIntegrityNum;
		///<summary></summary>
		public EnumWarningIntegrityType WarningIntegrityType;
		///<summary>Only used when WarningIntegrityType is PluginOverride.</summary>
		public string PluginName;
		public EnumIntegrityBehavior Behavior;
		///<summary>This message is what comes up when user clicks the triangle and also what comes up if it's a popup. Typically contains [Class] or [Plugin] that we swap out before display.</summary>
		public string Message;
		///<summary>Mostly used server side to keep track of which plugins belong with which companies.</summary>
		public string Note;
	}

	///<summary>We use the string versions of each of these to make server calls, so don't change spelling or capitalization. Order can be alphabetical since the underlying number does not matter. For now, the strings will also show to the users, exactly as they are, without additional spaces for example.</summary>
	public enum EnumWarningIntegrityType{
		None,
		///<summary>This is the default message and behavior for the classes for WarningIntegrity triangles.</summary>
		DefaultClass,
		///<summary>This is the default message and behavior for plug-ins.</summary>
		DefaultPlugin,
		///<summary>If this is used, specify a PluginName in that column.</summary>
		PluginOverride,
		///<summary>Used to show popups when selecting any module.</summary>
		Module,
		//from here down, alphabetical-------------------------------------------------
		Appointment,
		Patient,
		Payment,
		PayPlan
	}

	///<summary>Behaviors exhibited when an invalid SecurityHash is encountered. String matched and ordered by increasing annoyance. The last three items are completely different and only define plugin loading behavior. </summary>
	public enum EnumIntegrityBehavior{
		None,
		///<summary>Orange static triangle.</summary>
		Triangle,
		///<summary>Red static triangle.</summary>
		TriangleRed,
		///<summary>Orange triangle with increasing and decreasing transparency.</summary>
		TrianglePulse,
		///<summary>Orange triangle toggling visibility slowly.</summary>
		TriangleBlinkSlow,
		///<summary>Orange triangle toggling visibility quickly.</summary>
		TriangleBlinkFast,
		///<summary>The message comes up without clicking on the triangle.</summary>
		Popup,
		///<summary>The message appears when the first offending module was selected, but no other module for that patient, and for no other patient within the session.</summary>
		PopupSession,
		///<summary>The message appears when the first offending module was selected, but no other module for that patient, but would appear for each patient within the session.</summary>
		PopupPatient,
		///<summary>The message appears once for each offending module was selected for that patient. This would apply to each patient within the session.</summary>
		PopupModule,
		///<summary>The message appears every time an offending module was selected. This would apply to each patient within the session.</summary>
		PopupModuleAlways,
		///<summary>This silently allows the plugin.</summary>
		PluginAllow,
		PluginBlock,
		PluginWarning
	}

	///<summary>Associates a selected patient and modules visited.</summary>
	public class PatientModule {
		public long PatNum;
		public List<EnumModuleType> ListModuleTypes=new List<EnumModuleType>();
	}

}

/*
If it's a plug-in, we will need:
Default
PluginName
Behavior: Block or Warning, with a message in both cases 


*/