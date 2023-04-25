using System;

namespace OpenDentBusiness {
	///<summary>Indicates if HostedURL can be overridden by resellers, for this service.</summary>
	public class HostedUrlAttribute : Attribute {
		public bool CanResellerOverride;
	}

}