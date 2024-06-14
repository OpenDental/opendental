using Newtonsoft.Json;

namespace OpenDentBusiness.OpenAi {
	public class OAIContent {
		[JsonProperty("type")]
		public string Type;
		[JsonProperty("text")]
		public OAITextContent Text;
	}
}
