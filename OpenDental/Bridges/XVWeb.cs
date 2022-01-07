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
			string urlPath=ProgramProperties.GetPropVal(ProgramCur.ProgramNum,ProgramProps.UrlPath);
			string progPath=Programs.GetProgramPath(ProgramCur);
			if(pat==null) {
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
					MessageBox.Show(Lan.g("XVWeb","Could not find")+urlPath+"\r\n"
						+Lan.g("XVWeb","Please set up a default web browser")+".");
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
			UriBuilder uriBuilder=GetApiUri(true);
			uriBuilder.Path+="token";
			HttpWebRequest request=(HttpWebRequest)WebRequest.Create(uriBuilder.ToString());
			request.Method="POST";
			var postData=new {
				username=ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.XVWeb),ProgramProps.Username),
				password=decryptedPassword 
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
				throw new ApplicationException(Lan.g("XVWeb","Invalid XVWeb credentials. Please check your username and password in the XVWeb bridge setup."));
			}
			if(response.StatusCode!=HttpStatusCode.OK) {
				throw new ApplicationException(Lan.g("XVWeb","Unable to connect to XVWeb. Response from XVWeb:")+" "+response.StatusDescription);
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

		/// <summary>A simple get request that saves XVWeb Id's for the object it is getting</summary> 
		private static List<long> GetRequestIds(string token,UriBuilder url,int nextRecord=0) {
			UriBuilder newUriBuilder=new UriBuilder(url.ToString());
			if(nextRecord > 0) {
				newUriBuilder.Query+=(newUriBuilder.Query=="" ? "" : "&")+"NextRecord="+nextRecord;
			}
			string serverResp=GetRequestHelper(token,newUriBuilder);
			//parse recieved data to get the token
			object rsp=JsonConvert.DeserializeObject(serverResp);
			JToken parseResp=JToken.Parse(rsp.ToString());
			JToken records=parseResp["Records"];
			List<long> listIds=records.Select(x => PIn.Long(x["Id"].ToString())).ToList();
			int newNextRecord=PIn.Int(parseResp["NextRecord"].ToString());
			if(newNextRecord>0 && newNextRecord<=PIn.Long(parseResp["TotalRecords"].ToString())){ 
				//we know there is another page of records
				listIds.AddRange(GetRequestIds(token,url,newNextRecord));//Recursively call ourselves
			}
			return listIds;
		}

		/// <summary>A GET request that saves XVWeb Image information so we can get an image later on.</summary>
		private static List<ApteryxImage> GetImages(string token,UriBuilder url,int nextRecord=0) {
			UriBuilder newUriBuilder=new UriBuilder(url.ToString());
			if(nextRecord > 0) {
				newUriBuilder.Query+=(newUriBuilder.Query=="" ? "" : "&")+"NextRecord="+nextRecord;
			}
			string serverResp=GetRequestHelper(token,newUriBuilder);
			//parse recieved data to get the token
			var rsp=JsonConvert.DeserializeObject(serverResp);
			var parseResp=JToken.Parse(rsp.ToString());
			var records=parseResp["Records"];
			int newNextRecord=PIn.Int(parseResp["NextRecord"].ToString());
			List<ApteryxImage> listImages=new List<ApteryxImage>();
			if(records.Count()!=0) {//if we are at images and and at least one image exists in this series
				JsonSerializerSettings settings=new JsonSerializerSettings();
				try {
					Rootobject<ApteryxImage> rootImage=JsonConvert.DeserializeObject<Rootobject<ApteryxImage>>(serverResp,settings);//get limited image info
					listImages.AddRange(rootImage.Records);
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
			}
			if(newNextRecord>0 && newNextRecord<PIn.Long(parseResp["TotalRecords"].ToString())){ 
				//we know there is another page of images
				listImages.AddRange(GetImages(token,url,newNextRecord));//Recursively call ourselves
			}
			return listImages;
		}

		/// <summary>A GET request that returns a bitmap image for XVWeb.</summary>
		public static Bitmap GetBitmap(ApteryxImage img,IProgressHandler progressWindow) {
			string token=GetAuthorizationToken();//reuse old token or get a new one. 
			UriBuilder uriBuilder=GetApiUri();
			uriBuilder.Path+="bitmap/"+img.Id;
			Stream responseStream=GetRequestHelperStream(token,uriBuilder,accept:GetMimeTypeForImageQuality());
			int bytesRead;
			Bitmap image;
			long totalBytesRead=0;
			byte[] buffer=new byte[10 * 1024];
			using(MemoryStream ms=new MemoryStream()) { 
				try {
					while((bytesRead=responseStream.Read(buffer,0,buffer.Length)) > 0) {
						totalBytesRead+=bytesRead;
						if(totalBytesRead!=img.FileSize) {
							progressWindow.UpdateBytesRead(totalBytesRead);
						}
						ms.Write(buffer,0,bytesRead);
					}
					progressWindow.CloseProgress();
				}
				catch(Exception ex) {
					progressWindow.DisplayError(ex.Message);
				}
				image=new Bitmap(ms);
			}
			responseStream.Close();
			return image;
		}

		///<summary>Saves the image to A to Z in a new document if the program property to save images is set. Returns null if images are not set to be 
		///saved or if the image already exists.</summary>
		public static Document SaveApteryxImageToDoc(ApteryxImage img,Bitmap saveImage,Patient patCur) {
			if(ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.XVWeb),ProgramProps.SaveImages).Trim().ToLower()!="yes"
				|| Documents.DocExternalExists(img.Id.ToString(),ExternalSourceType.XVWeb)) //if they want to save and it doesn't already exist in DB
			{
				return null;
			}
			//store the image in the database
			string imageCat=ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.XVWeb),ProgramProps.ImageCategory);
			Document doc=ImageStore.Import(saveImage,Defs.GetDef(DefCat.ImageCats,PIn.Long(imageCat)).DefNum,ImageType.Photo,
				patCur,GetMimeTypeForImageQuality());
			doc.ToothNumbers=img.FormattedTeeth;
			doc.DateCreated=img.AcquisitionDate;
			doc.Description=doc.ToothNumbers;
			doc.ExternalGUID=img.Id.ToString();
			doc.ExternalSource=ExternalSourceType.XVWeb;
			Documents.Update(doc);
			return doc;
		}

		/// <summary>A GET request that returns a bitmap image for XVWeb.</summary>
		public static Bitmap GetThumbnail(ApteryxImage img) {
			string token=GetAuthorizationToken();
			UriBuilder uriBuilder=GetApiUri();
			uriBuilder.Path+="bitmap/thumbnail/"+img.Id;
			using(Stream responseStream=GetRequestHelperStream(token,uriBuilder,accept:GetMimeTypeForImageQuality())) {
				return new Bitmap(responseStream);
			}
		}

		///<summary>Get a list of thumbnails for this patient.</summary>
		public static IEnumerable<ApteryxThumbnail> GetListThumbnails(Patient PatCur,List<string> listIdsToExclude) {
			List<ApteryxThumbnail> listApteryxThumbnails=new List<ApteryxThumbnail>();
			List<ApteryxImage> listApteryxImage=GetImagesList(PatCur);
			listApteryxImage.RemoveAll(x => ListTools.In(x.Id.ToString(),listIdsToExclude));
			foreach(ApteryxImage image in listApteryxImage) {
				ApteryxThumbnail thumbnail=new ApteryxThumbnail();
				thumbnail.Thumbnail=GetThumbnail(image);
				thumbnail.Image=image;
				thumbnail.PatNum=PatCur.PatNum;
				listApteryxThumbnails.Add(thumbnail);
				yield return thumbnail;
			}
		}

		/// <summary>A general GET request helper that accepts JSON and saves it to a string.</summary>
		private static string GetRequestHelper(string token,UriBuilder url,bool doRetryIfUnauthorized=true,string contentType="application/json") {
			Stream responseStream=GetRequestHelperStream(token,url,doRetryIfUnauthorized,contentType);
			string serverResp=new StreamReader(responseStream).ReadToEnd();
			responseStream.Close();
			return serverResp;
		}

		private static Stream GetRequestHelperStream(string token,UriBuilder url,bool doRetryIfUnauthorized=true,string contentType="application/json"
			,string accept="application/json") 
		{
			HttpWebRequest request=(HttpWebRequest)WebRequest.Create(url.ToString());
			request.Method="GET";
			request.ContentType=contentType;
			request.Accept=accept;
			request.Headers.Add(HttpRequestHeader.Authorization,token);
			HttpWebResponse response;
			try {
				response=(HttpWebResponse)request.GetResponse();
			}
			catch(WebException ex) {
				if(doRetryIfUnauthorized &&
					((HttpWebResponse)ex.Response).StatusCode==HttpStatusCode.Unauthorized) //In case the token has expired, get a new token and try again.
				{ 
					return GetRequestHelperStream(GetAuthorizationToken(true),url,false);
				}
				throw;
			}
			return response.GetResponseStream();
		}

		/// <summary>Runs all the necessary POST and GET requests for XVWeb and saves the results to a list containing the image information.</summary>
		public static List<ApteryxImage> GetImagesList(Patient patCur) {
			UriBuilder uriBuilder=GetApiUri();
			List<ApteryxImage> listImages=new List<ApteryxImage>();
			string token=GetAuthorizationToken();//perform initial post request to get an authorization token
			UriBuilder uriBuilderPatient=new UriBuilder(uriBuilder.ToString());
			uriBuilderPatient.Path+="patient";
			uriBuilderPatient.Query="PrimaryId="+patCur.PatNum+"&lastname="+patCur.LName+"&firstname="+patCur.FName;
			List<long> listPatientIds=GetRequestIds(token,uriBuilderPatient);
			if(listPatientIds.Count<1) {
				return listImages; 
			}
			object locker=new object();
			List<Action> listActions=new List<Action>();
			List<long> listStudyIds=new List<long>();
			foreach(long id in listPatientIds) { //get studies for all patients. (Patients can have one ODpatnum but several different patientIDs)
				listActions.Add(new Action(() => {
					UriBuilder uriBuilderStudy=new UriBuilder(uriBuilder.ToString());
					uriBuilderStudy.Path+="study";
					uriBuilderStudy.Query="patient="+id;
					List<long> listIds=GetRequestIds(token,uriBuilderStudy);
					lock(locker) {
						listStudyIds.AddRange(listIds);
					}
				}));
			}
			ODThread.RunParallel(listActions,TimeSpan.FromMinutes(1));
			if(listStudyIds.Count<1) {
				return listImages; 
			}
			listActions=new List<Action>();
			List<long> listSeriesIds=new List<long>();
			foreach(long id in listStudyIds) { //get series for all studies for all patients
				listActions.Add(new Action(() => {
					UriBuilder uriBuilderSeries=new UriBuilder(uriBuilder.ToString());
					uriBuilderSeries.Path+="series";
					uriBuilderSeries.Query="study="+id;
					List<long> listIds=GetRequestIds(token,uriBuilderSeries);
					lock(locker) {
						listSeriesIds.AddRange(listIds);
					}
				}));
			}
			ODThread.RunParallel(listActions,TimeSpan.FromMinutes(1));
			if(listSeriesIds.Count<1) {
				return listImages; 
			}
			listActions=new List<Action>();
			foreach(long id in listSeriesIds) { //get images for all series for all studies for all patients
				listActions.Add(new Action(() => {
					UriBuilder uriBuilderImage=new UriBuilder(uriBuilder.ToString());
					uriBuilderImage.Path+="Image";
					uriBuilderImage.Query="series="+id;
					List<ApteryxImage> listImageDownloads=GetImages(token,uriBuilderImage);
					lock(locker) {
						listImages.AddRange(listImageDownloads);
					}
				}));
			}
			ODThread.RunParallel(listActions,TimeSpan.FromMinutes(1));
			return listImages;
		}

		private static UriBuilder GetApiUri(bool isForToken=false) {
			string baseURL=ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.XVWeb),ProgramProps.UrlPath);
			if(!baseURL.EndsWith("/")) {
				baseURL+="/";
			}
			UriBuilder uriBuilder=new UriBuilder(baseURL);
			if(!isForToken) {
				uriBuilder.Path+="api/";
			}
			return uriBuilder;
		}

		public static bool IsDisplayingImagesInProgram {
			get	{
				return Programs.IsEnabled(ProgramName.XVWeb)
					&& ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.XVWeb),ProgramProps.Username)!=""
					&& ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.XVWeb),ProgramProps.Password)!=""
					&& ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.XVWeb),ProgramProps.ImageCategory)!="";
			}
		}

		private static string GetMimeTypeForImageQuality() {
			if(ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.XVWeb),ProgramProps.ImageQuality)==XVWebImageQuality.Highest.ToString()) {
				return "image/png";
			}
			return "image/jpeg";
		}

		public class ProgramProps {
			public const string Username="Username";
			public const string Password="Password";
			public const string ImageCategory="ImageCategory";
			public const string UrlPath="Enter desired URL address for XVWeb";
			public const string SaveImages="Save Images (yes or no)";
			public const string ImageQuality="Image Quality";
		}

		private class Rootobject<T> {
			public int TotalRecords { get; set; }
			public int NextRecord { get; set; }
			public T[] Records { get; set; }
		}

	}

	///<summary>Options for XVWeb image quality.</summary>
	public enum XVWebImageQuality {
		///<summary>Will use image/png.</summary>
		Highest,
		///<summary>Will use image/jpeg.</summary>
		Moderate,
	}

}







