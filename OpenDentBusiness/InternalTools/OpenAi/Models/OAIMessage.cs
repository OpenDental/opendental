using Newtonsoft.Json;
using System.Collections.Generic;

namespace OpenDentBusiness.OpenAi {
	///<summary>An OpenAi message created by an Assistant or a user. Messages can include text, images, and other files.</summary>
	public class OAIMessage {
		[JsonProperty("id")]
		public string Id;
		[JsonProperty("object")]
		public string Object;
		[JsonProperty("created_at")]
		public long CreatedAt;
		[JsonProperty("thread_id")]
		public string ThreadId;
		[JsonProperty("role")]
		public string Role="user";
		[JsonProperty("content")]
		public List<OAIContent> Content=new List<OAIContent>();
		[JsonProperty("file_ids")]
		public List<string> FileIds=new List<string>();
		[JsonProperty("assistant_id")]
		public string AssistantId;
		[JsonProperty("run_id")]
		public string RunId;
		[JsonProperty("metadata")]
		public Dictionary<string, string> Metadata=new Dictionary<string, string>();
	}
}
