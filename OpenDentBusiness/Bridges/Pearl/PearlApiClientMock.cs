using OpenDentBusiness.Pearl;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness.Pearl {
	public class PearlApiClientMock:IPearlApiClient {
		public bool SucceedGetAuthToken=true;
		public bool SucceedGetAwsPresignedUrlInfo=true;
		public bool SucceedUploadToAwsPresignedURL=true;
		///<summary>Defaults to a happy path is_complete return value. Set to null if you want API to mock throwing an exception.</summary>
		public Func<string,string,ImageRequestIdResponse> FuncGetImageByRequestIdOutput=(string organizationId,string requestId) => {
			ImageRequestIdResponse response=new ImageRequestIdResponse();
			response.result=new Result();
			response.result.request_id=requestId;
			response.result.organization_id=organizationId;
			response.result.is_completed=true;
			return response;
		};


		public string BitmapToImageTempCopy(Bitmap bitmap,string fileName) {
			return "";
		}

		public AuthTokenResponse GetAuthToken(string clientId,string clientSecret) {
			if(SucceedGetAuthToken) {
				AuthTokenResponse response=new AuthTokenResponse();
				return response;
			}
			throw new Exception("GetAuthToken threw an exception.");
		}

		public ImageResponse GetAwsPresignedUrlInfo(ImageRequest image) {
			if(SucceedGetAwsPresignedUrlInfo) {
				return new ImageResponse();
			}
			throw new Exception("GetAwsPresignedUrlInfo threw an exception.");
		}

		public ImageRequestIdResponse GetImageByRequestId(string organizationId,string requestId) {
			if(FuncGetImageByRequestIdOutput==null) {
				throw new Exception("GetImageByRequestId threw an exception.");
			}
			return FuncGetImageByRequestIdOutput(organizationId,requestId);			
		}

		public Result GetOneImageFromPearl(string requestId) {
			ImageRequestIdResponse imageRequestIdResponse=GetImageByRequestId(organizationId:"",requestId);
			return imageRequestIdResponse.result;
		}

		public void RefreshAuthTokenIfNeeded(string clientId,string clientSecret) {
			GetAuthToken(clientId,clientSecret);
		}

		public string SendOneImageToPearl(long docNum,Bitmap bitmap,Patient patient) {
			string requestId=Guid.NewGuid().ToString();
			RefreshAuthTokenIfNeeded(clientId:"",clientSecret:"");
			PearlRequest pearlRequest=new PearlRequest();
			pearlRequest.DocNum=docNum;
			pearlRequest.RequestId=requestId;
			pearlRequest.RequestStatus=EnumPearlStatus.Polling;
			pearlRequest.DateTSent=DateTime.Now;
			ImageRequest request=new ImageRequest();
			request.request_id=requestId;
			ImageResponse imageResponse=GetAwsPresignedUrlInfo(request);
			bool isSuccess=UploadToAwsPresignedUrl(bitmap,imageResponse);
			PearlRequests.Insert(pearlRequest);
			return requestId;
		}

		public bool UploadToAwsPresignedUrl(Bitmap bitmap,ImageResponse image) {
			if(SucceedUploadToAwsPresignedURL) {
				return true;
			}
			throw new Exception("UploadToAwsPresignedUrl threw an exception.");
		}
	}
}
