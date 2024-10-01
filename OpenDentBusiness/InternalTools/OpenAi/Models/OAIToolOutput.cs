using Newtonsoft.Json;

namespace OpenDentBusiness.OpenAi {
	public class OAIToolOutput {
		[JsonProperty("tool_call_id")]
		public string ToolCallId;
		[JsonProperty("output")]
		public string Output;
	}
}