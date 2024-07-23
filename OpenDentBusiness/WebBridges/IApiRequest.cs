using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenDentBusiness;

namespace OpenDentBusiness.WebBridges {
	public interface IApiRequest {
		T SendRequest<T>(string urlEndpoint,HttpMethod method,AuthenticationHeaderValue authHeaderVal,string body,HttpContentType contentType=HttpContentType.Json,HttpClient clientOverride=null,List<string> queryParameters=null,JsonSerializerSettings deserializeSettings=null);
		Task<T> GetRequestAsync<T>(string urlEndpoint,HttpClient clientOverride=null);
		Task<T> PostRequestAsync<T>(string urlEndpoint,string body,HttpContentType contentType=HttpContentType.Json,HttpClient clientOverride=null);
		Task<T> SendRequestAsync<T>(string urlEndpoint,HttpMethod method,AuthenticationHeaderValue authHeaderVal,string body,HttpContentType contentType=HttpContentType.Json,HttpClient clientOverride=null,List<string> queryParameters=null,JsonSerializerSettings deserializeSettings=null);
	}
}
