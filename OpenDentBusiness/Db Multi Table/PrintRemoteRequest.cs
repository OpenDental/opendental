using System;
using System.Collections.Generic;

namespace OpenDentBusiness {
	///<summary>Non-db class used for serializing parameters for print requests.</summary>
	[Serializable]
	public class PrintRemoteRequest {
		///<summary>The type of document we're printing.</summary>
		public EnumPrintRequestType PrintRequestType;
		///<summary>The type of source for this print request.</summary>
		public EnumPrintRequestFKeyType PrintRequestFKeyType;
		///<summary>the FKey to the source for this print request.</summary>
		public long FKey;
		///<summary>The PrinterNum override. Overrides the printer table higherarchy when set.</summary>
		public long PrinterNumOverride;
		///<summary>The user requesting the print. This is used to fill the userPref cache.</summary>
		public long RequestUserNum;
		///<summary>The list of serialized parameters to fulfill the print request.</summary>
		public List<string> ListParameters;
	}

	/// <summary>The type of printing being requested.</summary>
	public enum EnumPrintRequestType {
		Rx,
		PayPlan,
		TreatPlan,
		Sheet,
	}

	public enum EnumPrintRequestFKeyType {
		None,
		MobileAppDevice
	}
}