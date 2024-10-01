using CodeBase;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace OpenDentBusiness.Pearl {
	public class PearlApiClient : IPearlApiClient {
		public static IPearlApiClient Inst=new PearlApiClient();
		private static object _lockAuthToken=new object();
		private static DateTime _dateTimeLastAuthTokenRefresh;
		///<summary>HttpClient used for communicating with Pearl API.</summary>
		private static HttpClient _httpPearlClient=new HttpClient(){ 
			BaseAddress=new Uri("https://native-integration-prod.hellopearl.com")
		};
		///<summary>HttpClient used exclusively for uploading images to the Pearl provided Amazon S3 bucket via a pre-signed URL. </summary>
		private static HttpClient _httpAwsClient=new HttpClient();

		///<summary>Retrieves and sets the auth header for the Pearl http client if it is empty or has expired. Throws exceptions.</summary>
		public void RefreshAuthTokenIfNeeded(string clientId,string clientSecret) {
			if(DateTime_.Now<_dateTimeLastAuthTokenRefresh) {
				return;
			}
			lock(_lockAuthToken) {
				if(DateTime_.Now<_dateTimeLastAuthTokenRefresh) {
					return;
				}
				HttpClient httpClientTemp=new HttpClient();
				httpClientTemp.BaseAddress=new Uri("https://native-integration-prod.hellopearl.com");
				AuthTokenResponse result=GetAuthToken(clientId,clientSecret);
				httpClientTemp.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue(result.token_type,result.access_token);
				_httpPearlClient=httpClientTemp;
				//TODO: Replace _httpPearlClient and _httpAwsClient with HttpClientWrappers so they can be properly disposed after B55906 is implemented.
				//Tokens expire in 7 days. Set ours to refresh an hour before that.
				_dateTimeLastAuthTokenRefresh=DateTime_.Now.AddSeconds(result.expires_in).AddHours(-1);
			}
		}

		///<summary>Attempts to upload an image to Pearl and Amazon for the given bitmap. Throws exceptions.</summary>
		public string SendOneImageToPearl(long docNum,Bitmap bitmap,Patient patient) {
			if(bitmap==null) {
				return null;
			}
			string clientId=ProgramProperties.GetPropVal(ProgramName.Pearl,Bridges.Pearl.PEARL_CLIENT_ID_PROPERTY);
			string clientSecret=ProgramProperties.GetPropVal(ProgramName.Pearl,Bridges.Pearl.PEARL_CLIENT_SECRET_PROPERTY);
			string officeId=ProgramProperties.GetPropVal(ProgramName.Pearl,Bridges.Pearl.PEARL_OFFICE_ID_PROPERTY);
			string organizationId=ProgramProperties.GetPropVal(ProgramName.Pearl,Bridges.Pearl.PEARL_ORGANIZATION_ID_PROPERTY);
			string requestId=Guid.NewGuid().ToString();
			PearlRequest pearlRequest=new PearlRequest();
			pearlRequest.RequestId=requestId;
			pearlRequest.DocNum=docNum;
			RefreshAuthTokenIfNeeded(clientId,clientSecret);
			ImageResponse imageResponse=GetAwsPresignedUrlInfo(new ImageRequest() {
				request_id=requestId,
				extension=".jpg",//ImageFormat.Jpeg below for temp image path
				patient_id=patient.PatNum.ToString(),
				study_date=DateTime.Now.ToString("yyyy-MM-dd"),
				patient_name=patient.GetNameLF(),
				patient_dob=patient.Birthdate.ToString("yyyy-MM-dd"),
				office_id=officeId,
				organization_id=organizationId
			});
			if(imageResponse==null || !(imageResponse is ImageResponse)) {
				throw new ApplicationException("A presigned url was not valid.");
			}
			bool wasImageUploaded=UploadToAwsPresignedUrl(bitmap,imageResponse);
			if(!wasImageUploaded) {
				throw new ApplicationException("Could not upload image successfully.");
			}
			pearlRequest.RequestStatus=EnumPearlStatus.Polling;
			pearlRequest.DateTSent=DateTime.Now;
			PearlRequests.Insert(pearlRequest);
			return requestId;
		}

		///<summary>Attempts to get an image result from Pearl for the given requestId. Throws exceptions.</summary>
		public Result GetOneImageFromPearl(string requestId) {
			if(string.IsNullOrWhiteSpace(requestId)) {
				return null;
			}
			string organizationId=ProgramProperties.GetPropVal(ProgramName.Pearl,Bridges.Pearl.PEARL_ORGANIZATION_ID_PROPERTY);
			Result result=GetImageByRequestId(organizationId,requestId).result;
			return result;
		}

		///<summary>Attempts to retrieve a token from Pearl. Throws exceptions.</summary>
		public AuthTokenResponse GetAuthToken(string clientId,string clientSecret) {
			AuthTokenResponse retVal=APIRequest.Inst.SendRequest<AuthTokenResponse>(
				urlEndpoint:"/api/v1/auth/token",
				method:HttpMethod.Post,
				authHeaderVal:null,
				body:JsonConvert.SerializeObject(new {
					client_id = clientId,
					client_secret = clientSecret
				}),
				clientOverride:_httpPearlClient
			);
			return retVal;
		}

		///<summary>Attempts to retrieve the pre-signed Amazon URL information from Pearl we will need for uploading an image for processing. 
		///Throws exceptions.</summary>
		public ImageResponse GetAwsPresignedUrlInfo(ImageRequest image) {
			ImageResponse retVal=APIRequest.Inst.SendRequest<ImageResponse>(
				urlEndpoint:"/api/v1/image",
				method:HttpMethod.Post,
				authHeaderVal:null,
				body:JsonConvert.SerializeObject(image),
				clientOverride:_httpPearlClient
			);
			return retVal;
		}

		///<summary>Attempts to upload an image to an Amazon pre-signed URL. Throws exceptions.</summary>
		public bool UploadToAwsPresignedUrl(Bitmap bitmap,ImageResponse image) {
			string fileName=image.image_url.Split('/').Last(); 
			string tempPath=Path.GetFullPath(BitmapToImageTempCopy(bitmap,fileName));
			using(FileStream tempFileStream = new FileStream(tempPath,FileMode.Open)) {
				MultipartFormDataContent formData=new MultipartFormDataContent();
				for(int i=0;i<image.presigned_url.fields.Length;i++) {
					string value=image.presigned_url.fields[i].value;
					formData.Add(new StringContent(value),image.presigned_url.fields[i].header);
				}
				//Per the AWS documentation, the file must be the last element in formData.
				formData.Add(new StreamContent(tempFileStream),"file",fileName);
				HttpResponseMessage response=APIRequest.Inst.SendRequest<HttpResponseMessage,MultipartFormDataContent>(
					urlEndpoint:image.presigned_url.url,
					method:HttpMethod.Post,
					authHeaderVal:null,
					body:formData,
					contentType:HttpContentType.Multipart,
					clientOverride:_httpAwsClient
				);
				if(File.Exists(tempPath)) {
					File.Delete(tempPath);
				}
			}
			return true;
		}

		///<summary>Attempts to retrieve image data for the given organizationId and requestId.
		///The resulting ImageRequestIdResponse should be verified via the is_completed and is_rejected flags.
		///If the StatusCode is Accepted, then imageRequestIdResponse.result will be null. Throws exceptions.</summary>
		public ImageRequestIdResponse GetImageByRequestId(string organizationId,string requestId) {
			//Previously the code below would handle a special case, now that we use APIRequest we can't change the response object.
			//If the StatusCode is Accepted, then imageRequestIdResponse.result will be null.
			//if(response.StatusCode==HttpStatusCode.Accepted) {
			//	//Image is still processing, or there is an image_status like 'missing_pearl_patient_id'
			//	//The Pearl 202 response object only contains 'message' and 'image_status' properties, these do not overlap with ImageRequestIdResponse.
			//	//We do not know why Pearl uses 202 responses for both curently processing and invalid images.
			//	return new ImageRequestIdResponse() {
			//		result = new Result {
			//			is_completed = false
			//		}
			//	};
			//}
			ImageRequestIdResponse retVal=APIRequest.Inst.SendRequest<ImageRequestIdResponse>(
				urlEndpoint:$"/api/v1/image/{organizationId}/byRequestId/{requestId}",
				method:HttpMethod.Get,
				authHeaderVal:null,
				body:null,
				clientOverride:_httpPearlClient
			);
			return retVal;
		}

		///<summary>Returns a temporary path for the given bitmap, this file will need to be deleted later. Throws exceptions.</summary>
		public string BitmapToImageTempCopy(Bitmap bitmap,string fileName) {
			string tempPath=Path.Combine(Path.GetTempPath(),"opendental",fileName);
			bitmap.Save(tempPath,ImageFormat.Jpeg);
			return tempPath;
		}
	}
}
