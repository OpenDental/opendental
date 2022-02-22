using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using OpenDentBusiness;

namespace UnitTests {
	public class WebServiceT {

		public static bool ConnectToMiddleTier(string serverURI,string userName,string password,string productVersion,bool isOracle) {
			if(isOracle) {
				return false;
			}
			RemotingClient.ServerURI=serverURI;
			try{
				Userod user=Security.LogInWeb(userName,password,"",productVersion,false);
				Security.CurUser=user;
				Security.PasswordTyped=password;
				RemotingClient.RemotingRole=RemotingRole.ClientWeb;
			}
			catch(Exception) {
				return false;
			}
			return true;
		}

	}
}
