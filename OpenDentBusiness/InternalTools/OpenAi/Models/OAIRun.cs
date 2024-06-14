using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace OpenDentBusiness.OpenAi {
	///<summary>An OpenAi invocation of an Assistant on a Thread. The Assistant uses its configuration and the Thread’s Messages to perform tasks by calling models and tools. As part of a Run, the Assistant appends Messages to the Thread.</summary>
	public class OAIRun {
		public string Id;
		public string Object="thread.run";
		public long CreatedAt;
		public string ThreadId;
		public string AssistantId;
		[JsonConverter(typeof(OpenAiRunStatusConverter))]
		public OpenAiRunStatus Status;
		[JsonProperty("required_action")]
		public OAIRequiredAction RequiredAction;
		public object LastError; // Consider a more specific type
		public long ExpiresAt;
		public long? StartedAt;
		public long? CancelledAt;
		public long? FailedAt;
		public long? CompletedAt;
		public string Model;
		public string Instructions;
		public List<object> Tools=new List<object>(); // Consider a more specific type
		public List<string> FileIds=new List<string>();
		public Dictionary<string, string> Metadata=new Dictionary<string, string>();
		public OAIUsage Usage;
	}

	public enum OpenAiRunStatus {
		queued,
		in_progress,
		requires_action,
		cancelling,
		cancelled,
		failed,
		completed,
		expired
	}

	public class OpenAiRunStatusConverter : JsonConverter {
		public override bool CanConvert(Type objectType) {
			return (objectType == typeof(OpenAiRunStatus));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			if(reader.TokenType == JsonToken.String) {
				return reader.Value.ToString().ToLower() switch {
					"queued" => OpenAiRunStatus.queued,
					"in_progress" => OpenAiRunStatus.in_progress,
					"requires_action" => OpenAiRunStatus.requires_action,
					"cancelling" => OpenAiRunStatus.cancelling,
					"cancelled" => OpenAiRunStatus.cancelled,
					"failed" => OpenAiRunStatus.failed,
					"completed" => OpenAiRunStatus.completed,
					"expired" => (object)OpenAiRunStatus.expired,
					_ => throw new JsonSerializationException("Invalid value for OpenAiRunStatus"),
				};
			}
			throw new JsonSerializationException("Unexpected token type for OpenAiRunStatus");
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			throw new NotImplementedException();
		}
	}
}
