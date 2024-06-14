namespace OpenDentBusiness.OpenAi {
	///<summary>An OpenAi class that contains the ToolCall information for what local functions need to be ran and returned back to OpenAi for the current run to complete.</summary>
	public class OAIToolCall {
		public string Id;
		public string Type;
		public OAIFunction Function;
	}
}
