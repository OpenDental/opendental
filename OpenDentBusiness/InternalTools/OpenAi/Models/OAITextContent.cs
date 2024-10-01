using Newtonsoft.Json;
using System.Collections.Generic;

namespace OpenDentBusiness.OpenAi {
	public class OAITextContent {
		[JsonProperty("value")]
		public string Value;
		[JsonProperty("annotations")]
		public List<object> Annotations=new List<object>();
	}
}
