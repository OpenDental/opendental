namespace OpenDentBusiness.OpenAi {
	///<summary>OpenAi args for the get_manual_url function.</summary>
	public class OAIManualUrlFunctionArgs {
		public string Keywords;
		///<summary>The version number for the request, null when the request is for stable version manual pages.</summary>
		public double? Version;
	}
}
