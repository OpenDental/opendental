using CodeBase;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SystemTask = System.Threading.Tasks.Task;

namespace OpenDentBusiness.Pearl {
	public interface IPearlApiClient {
		///<summary>Throws Exception.
		///Retrieves and sets the auth header for the Pearl http client.</summary>
		void RefreshAuthTokenIfNeeded(string clientId, string clientSecret);

		///<summary>Attempts to upload an image to Pearl and Amazon for the given bitmap.
		///Returns null if there was an error.</summary>
		string SendOneImageToPearl(long docNum,Bitmap bitmap,Patient patient);

		///<summary>Attempts to get an image result from Pearl for the given requestId.
		///Returns null if there was an error.</summary>
		Result GetOneImageFromPearl(string requestId);

		///<summary>Throws Exception. Attempts to retrieve a token from Pearl..</summary>
		AuthTokenResponse GetAuthToken(string clientId,string clientSecret);

		///<summary>Throws Exception.
		///Attempts to retrieve the pre-signed Amazon URL information from Pearl we will need for uploading an image for processing.
		///Returns null if response is not valid or there was an exception.</summary>
		ImageResponse GetAwsPresignedUrlInfo(ImageRequest image);

		///<summary>Throws Exception.
		///Attempts to upload an image to an Amazon pre-signed URL.
		///Returns null if response is not valid or there was an exception.</summary>
		bool UploadToAwsPresignedUrl(Bitmap bitmap,ImageResponse image);

		///<summary>Throws Exception.
		///Attempts to retrieve image data for the given organizationId and requestId.
		///The resulting ImageRequestIdResponse should be verified via the is_completed and is_rejected flags.
		///If the StatusCode is Accepted, then imageRequestIdResponse.result will be null.</summary>
		ImageRequestIdResponse GetImageByRequestId(string organizationId,string requestId);

		///<summary>Throws Exception. Returns a temporary path for the given bitmap, this file will need to be deleted later.</summary>
		string BitmapToImageTempCopy(Bitmap bitmap,string fileName);
	}
}
