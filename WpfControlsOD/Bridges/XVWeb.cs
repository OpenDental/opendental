using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using CodeBase;

namespace OpenDental.Bridges {
	///<summary>Bridge to Apteryx's XVWeb</summary>
	public class XVWeb {
		private static Token _xvwebToken;
		private static object _lock=new object();

		private struct Token {
			public string accessToken;
			public string tokenType;
			public DateTime issued;
			public DateTime expires;
		}

		///<summary></summary>
		public XVWeb() {

		}

		///<summary></summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			GetAuthorizationToken();
			string urlPath=ProgramProperties.GetPropVal(ProgramCur.ProgramNum,ProgramProps.UrlPath);
			string progPath=Programs.GetProgramPath(ProgramCur);
			if(pat==null) {
				if(!_xvwebToken.accessToken.IsNullOrEmpty()) {
					urlPath+=$"?token={_xvwebToken.accessToken}";
				}
				//Launch program without any patient.
				try {
					if(string.IsNullOrWhiteSpace(progPath)) {
						//E.g. "https://demo2.apteryxweb.com/" if not using RemoteExecuter
						ODFileUtils.ProcessStart(urlPath);//should start XVWeb without bringing up a pt.
					}
					else {
						//E.g. "C:\Program Files\RemoteExecuter\RemoteExecuter.exe https://demo2.apteryxweb.com/" if using RemoteExecuter
						ODFileUtils.ProcessStart(progPath,urlPath);
					}
				}
				catch {
					MessageBox.Show(Lang.g("XVWeb","Could not find")+urlPath+"\r\n"
						+Lang.g("XVWeb","Please set up a default web browser")+".");
				}
				return;
			}
			string urlcomm="?patientid=";
			if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0") {
				urlcomm+=pat.PatNum.ToString();
			}
			else {
				urlcomm+=Tidy(pat.ChartNumber);
			}
			//Nearly always tidy the names in one way or another
			urlcomm+="&lastname="+Tidy(pat.LName);
			urlcomm+="&firstname="+Tidy(pat.FName);
			//This patterns shows a way to handle gender unknown when gender is optional.
			if(pat.Gender==PatientGender.Female) {
				urlcomm+="&gender=F";//fixed to match XVWeb Documentation 16.3.28+
			}
			else if(pat.Gender==PatientGender.Male) {
				urlcomm+="&gender=M";//fixed to match XVWeb Documentation 16.3.28+
			}
			else if(pat.Gender==PatientGender.Unknown){
				urlcomm+="&gender=O";//fixed to match XVWeb Documentation 16.3.28+
			}
			if(pat.Birthdate.Year>1880) {
				List<ProgramProperty> listProgramProperties=ProgramProperties.GetForProgram(ProgramCur.ProgramNum);
				string bDayFormat=ProgramProperties.GetCur(listProgramProperties,"Birthdate format (default MM/dd/yyyy)").PropertyValue;
				urlcomm+="&birthdate="+pat.Birthdate.ToString(bDayFormat);
			}
			if(!_xvwebToken.accessToken.IsNullOrEmpty()) {
				urlcomm+=$"&token={_xvwebToken.accessToken}";
			}
			string url=(urlPath+urlcomm).Replace(" ","+");
			try {
				if(string.IsNullOrWhiteSpace(progPath)) {
					//E.g. "https://demo2.apteryxweb.com/?patientid=1&lastname=Smith&firstname=John&gender=Male&birthdate=02/05/1976"
					ODFileUtils.ProcessStart(url);
				}
				else {
					//E.g. "C:\RemoteExecuter\RemoteExecuter.exe https://demo2.apteryxweb.com/?patientid=1&lastname=Smith&firstname=John&gender=Male&birthdate=02/05/1976"
					ODFileUtils.ProcessStart(progPath,url);
				}
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		///<summary>Removes ampersands and number symbols.</summary>
		private static string Tidy(string input) {
			string retVal=input.Replace("&","");//get rid of any ampersands.
			retVal=retVal.Replace("#","");//get rid of any number signs.
			return retVal;
		}

		/// <summary>Returns if the original POST token has expired or not to know if another one needs to be made.</summary>
		private static bool IsTokenExpired() {
			if(_xvwebToken.expires <= DateTime.Now.AddSeconds(-10) || _xvwebToken.accessToken==null) {
				return true;
			}
			return false;
		}

		/// <summary>Makes a post request to opendental.xvweb and returns a token string that allows access for get requests. Times out after 5 mins then
		/// string is no longer valid and another post will need to be made to get another access token. 
		/// </summary>
		/// <returns>Returns the authorization header type and the actual token.</returns>
		private static string GetAuthorizationToken(bool getFreshToken=false) {
			lock(_lock) {
				if(!getFreshToken && !IsTokenExpired()) { //reuse the old token if there is a valid one
					return _xvwebToken.tokenType+" "+_xvwebToken.accessToken;
				}
			}
			//decrypt hashed password from database before sending
			string decryptedPassword;
			CDT.Class1.Decrypt(ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.XVWeb),ProgramProps.Password),out decryptedPassword);
			UriBuilder uriBuilder=GetApiUri();
			uriBuilder.Path+="token";
			HttpWebRequest request=(HttpWebRequest)WebRequest.Create(uriBuilder.ToString());
			request.Method="POST";
			var postData=new {
				username=ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.XVWeb),ProgramProps.Username),
				password=decryptedPassword,
				grant_type="password"
			};
			string postString=JsonConvert.SerializeObject(postData);
			request.ContentType="application/json";
			request.Accept="application/json";
			byte[] byteArray=Encoding.UTF8.GetBytes(postString);
			Stream dataStream=request.GetRequestStream();
			dataStream.Write(byteArray,0,byteArray.Length);
			dataStream.Close();
			//Check to for bad user credentials or server problems. 
			HttpWebResponse response=(HttpWebResponse)request.GetResponse();
			if(response.StatusCode==HttpStatusCode.NoContent) {
				throw new ApplicationException(Lang.g("XVWeb","Invalid XVWeb credentials. Please check your username and password in the XVWeb bridge setup."));
			}
			if(response.StatusCode!=HttpStatusCode.OK) {
				throw new ApplicationException(Lang.g("XVWeb","Unable to connect to XVWeb. Response from XVWeb:")+" "+response.StatusDescription);
			}
			dataStream=response.GetResponseStream();
			StreamReader reader=new StreamReader(dataStream);
			string serverResp=reader.ReadToEnd();
			//parse recieved data to get the token
			object tokenResponse=JsonConvert.DeserializeObject(serverResp);
			JToken parseResp=JToken.Parse(tokenResponse.ToString());
			Token authToken=new Token();
			authToken.tokenType=parseResp["token_type"].ToString();
			authToken.accessToken=parseResp["access_token"].ToString();
			authToken.issued=parseResp["issued"].ToObject<DateTime>();
			authToken.expires=parseResp["expires"].ToObject<DateTime>().ToLocalTime();
			string token=authToken.tokenType+" "+authToken.accessToken;
			lock(_lock) {
				_xvwebToken=authToken;
			}
			reader.Close();
			dataStream.Close();
			response.Close();
			return token;
		}

		private static UriBuilder GetApiUri() {
			string baseURL=ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.XVWeb),ProgramProps.UrlPath);
			if(!baseURL.EndsWith("/")) {
				baseURL+="/";
			}
			baseURL+="api/v2/";
			UriBuilder uriBuilder=new UriBuilder(baseURL);
			return uriBuilder;
		}

		public class ProgramProps {
			public const string Username="Username";
			public const string Password="Password";
			public const string UrlPath="Enter desired URL address for XVWeb";
		}

	}

}







