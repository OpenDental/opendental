using Newtonsoft.Json;

namespace OpenDentBusiness.OpenAi {
	///<summary>An OpenAi conversation session between an Assistant and a user. Threads store Messages and automatically handle truncation to fit content into a model’s context.</summary>
	public class OAIThread {
		[JsonProperty("id")]
		public string Id;
		[JsonProperty("object")]
		public string Object;
		[JsonProperty("created_at")]
		public long CreatedAt;
		[JsonProperty("metadata")]
		public OAIMetadata Metadata;
	}
}
