using Newtonsoft.Json;

namespace OpenDentBusiness.OpenAi {
	///<summary>An OpenAi class that contains an array of local ToolCalls/methods that need to be called locally in order for the OpenAi run to complete.</summary>
	public class OAISubmitToolOutput {
		[JsonProperty("tool_calls")]
		public OAIToolCall[] ToolCalls;
	}
}
