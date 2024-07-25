using System;

namespace OpenDentBusiness {

	[Serializable]
	[CrudTable(IsMissingInGeneral=true,CrudExcludePrefC=true)]
	public class WebChatPref:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long WebChatPrefNum;
		///<summary>Preference name, must match a value from WebChatPrefName.</summary>
		public string PrefName;
		///<summary>The raw value as a string.</summary>
		public string ValueString;
	}

	///<summary>Listed in alphabetical order, since the enum option numbers do not matter, only the option names matter.</summary>
	public enum WebChatPrefName {
		///<summary>A string that contains additional instructions for OpenAi chat instances, this is added to the base assistant prompt/context that is setup in OpenAi.</summary>
		OpenAiAdditionalInstructions,
		///<summary>Api key for OpenAi endpoints.</summary>
		OpenAiApiKey,
		///<summary>Assistant ids for OpenAi chat assistants, csv.</summary>
		OpenAiAssistantIds,
		///<summary>Assistant id for the FAQ chat assistant on OpenAi.</summary>
		OpenAiFaqAssistantId,
		///<summary>When messages are sent automatically by the system, they will display in the chat log using this name.
		///Usually a company name.  For example, "Open Dental"</summary>
		SystemName,
		///<summary>The first system message posted to the chat thread when it is created.</summary>
		SystemWelcomeMessage,
		///<summary>The last system message posted to the chat thread when it is ended.
		///After this message is added to the session, the chat thread becomes frozen forever.</summary>
		SystemSessionEndMessage,
	}

}