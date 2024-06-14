using Newtonsoft.Json;

namespace OpenDentBusiness.OpenAi {
	///<summary>An OpenAi class that is used when the Run requires feedback in order to execute the rest of the run/instructions. See OpenAi assistant functions.</summary>
	public class OAIRequiredAction {
		public string Type;
		[JsonProperty("submit_tool_outputs")]
		public OAISubmitToolOutput SubmitToolOutputs;
	}
}
