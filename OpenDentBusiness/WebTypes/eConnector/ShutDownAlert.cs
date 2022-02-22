using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using Newtonsoft.Json;

namespace OpenDentBusiness.WebTypes {
	///<summary>This class is used to hold information regarding a an alert sent to an eConnector instructing it to shutdown.</summary>
	public class ShutDownEConnectorCommand : WebBase {
		public AlertType Type;
		public ActionType Actions;
		public string Description;
		public SeverityType Severity;
		public string ItemValue;

		public ShutDownEConnectorCommand() {

		}

		/// <summary>Converts a ShutDownAlert into an AlertItem, for forward compatibility.</summary>
		public AlertItem ConvertToAlertItem() {
			return new AlertItem() {
				Type=this.Type,
				Actions=this.Actions,
				Description=this.Description,
				Severity=this.Severity,
				ItemValue=this.ItemValue,
			};
		}
	}
}
