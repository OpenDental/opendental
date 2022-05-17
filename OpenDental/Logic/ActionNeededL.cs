using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDental {
	public class ActionNeededL {


	}

	///<summary></summary>
	public class ActionNeededEventArgs {
		private ActionNeededTypes _actionType;

		///<summary></summary>
		public ActionNeededEventArgs(ActionNeededTypes actionType) {
			_actionType=actionType;
		}

		///<summary></summary>
		public ActionNeededTypes ActionType {
			get {
				return _actionType;
			}
		}
	}

	public enum ActionNeededTypes {
		RadiologyProcedures,
	}

	///<summary></summary>
	public delegate void ActionNeededEventHandler(object sender,ActionNeededEventArgs e);

}