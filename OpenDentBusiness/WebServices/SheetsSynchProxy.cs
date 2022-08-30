using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase;
using OpenDentBusiness;
using WebServiceSerializer;

namespace OpenDentBusiness {
	public class SheetsSynchProxy {
	
		///<summary></summary>
		private static int _timeoutOverride=100000;//100 seconds
		///<summary></summary>
		private static string _urlOverride="";

		///<summary>Used when we would like to override the service timeout.
		///Will reset itself back to the default value after service instance is finished.</summary>
		public static int TimeoutOverride {
			get {
				return _timeoutOverride;
			}
			set {
				_timeoutOverride=value;
			}
		}
		
		///<summary>Used when we would like to override the service URL.
		///Will reset itself back to the default value after service instance is finished.</summary>
		public static string UrlOverride {
			get {
				return _urlOverride;
			}
			set {
				_urlOverride=value;
			}
		}

		public static ISheetsSynch MockSheetSynchService {
			private get;//Use GetWebServiceInstance()
			set;
		}

		/// <summary></summary>
		public static ISheetsSynch GetWebServiceInstance() {
			if(MockSheetSynchService!=null) {
				return MockSheetSynchService;
			}
			SheetsSynchReal service=new SheetsSynchReal();
			service.Timeout=100000;
			if(TimeoutOverride!=service.Timeout) {
				service.Timeout=TimeoutOverride;
				TimeoutOverride=100000;
			}
			if(string.IsNullOrEmpty(UrlOverride)) {
				service.Url=PrefC.GetString(PrefName.WebHostSynchServerURL);
			}
			else { 
				service.Url=UrlOverride;
				UrlOverride="";
			}
			if(ODBuild.IsDebug()) {
				//service.Url="http://localhost:2923/SheetsSynch.asmx";
			}
			return service;
		}

	}
}
