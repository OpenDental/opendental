using System.Xml;
using OpenDentBusiness;

namespace OpenDental {
	public class WebServiceRequest {

		///<summary>Returns error message if the passed in xml document has an Error or KeyDisabled node.</summary>
		public static string CheckForErrors(XmlDocument doc) {
			XmlNode node=doc.SelectSingleNode("//Error");
			if(node!=null) {
				return node.InnerText;
			}
			node=doc.SelectSingleNode("//KeyDisabled");
			if(node!=null) {
				//Disabled message. Update the preference and return the error.
				if(Prefs.UpdateBool(PrefName.RegistrationKeyIsDisabled,true)) {
					DataValid.SetInvalid(InvalidType.Prefs);
				}
				return node.InnerText;
			}
			//no error, and no disabled message
			if(Prefs.UpdateBool(PrefName.RegistrationKeyIsDisabled,false)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			return "";
		}
	}
}
