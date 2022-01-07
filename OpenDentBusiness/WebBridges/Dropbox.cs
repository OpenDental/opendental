using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	public class Dropbox {

		public class PropertyDescs {
			public static string AtoZPath="Dropbox AtoZ Path";
			public static string AccessToken="Dropbox API Token";
		}

		public static string AtoZPath {
			get { return ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.Dropbox),Dropbox.PropertyDescs.AtoZPath); }
		}

		public static string AccessToken {
			get { return ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.Dropbox),Dropbox.PropertyDescs.AccessToken); }
		}

		///<summary>Called by OAuth web app to display this URL in their browser.</summary>
		public static string GetDropboxAuthorizationUrl(string appkey) {
			return OpenDentalCloud.Dropbox.GetDropboxAuthorizationUrl(appkey);
		}
		
		///<summary>Throws exception.  Called by Open Dental Web Services HQ to get the real access code form the code given by Dropbox.</summary>
		public static string GetDropboxAccessToken(string code,string appkey,string appsecret) {
			return OpenDentalCloud.Dropbox.GetDropboxAccessToken(code,appkey,appsecret);
		}
	}
}
