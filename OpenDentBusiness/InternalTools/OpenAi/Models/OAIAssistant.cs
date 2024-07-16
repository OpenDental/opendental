using Newtonsoft.Json;

namespace OpenDentBusiness.OpenAi {
	///<summary>An OpenAi purpose-built AI that uses OpenAI’s models and calls tools.</summary>
	public class OAIAssistant {
		public string Id;
		[JsonProperty("object")]
		public string ObjectType;
		[JsonProperty("created_at")]
		public long CreatedAt;
		public string Name;
		public string Description;
		public string Model;
		public string Instructions;
		public object[] Tools;
		[JsonProperty("tool_resources")]
		public object ToolResources;
	}
}
