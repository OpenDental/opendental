using Newtonsoft.Json;
using System;

namespace OpenDentBusiness.OpenAi {
	///<summary>An OpenAi class that continas the RequiredActions ToolCall Function information. This class represents a call-back request from OpenAi.</summary>
	public class OAIFunction {
		[JsonConverter(typeof(OpenAiFunctionNameConverter))]
		///<summary>The name of the ToolCall function that is being requested by the OpenAi run, this function is ran locally and the result is given back to OpenAi.</summary>
		public OpenAiFunctionName Name;
		///<summary>JSON class that needs is converted into specific args class depending on the Name, see ManualUrlFunctionArgs.</summary>
		public string Arguments;
	}

	public enum OpenAiFunctionName {
		get_manual_url
	}

	public class OpenAiFunctionNameConverter : JsonConverter {
		public override bool CanConvert(Type objectType) {
			return (objectType == typeof(OpenAiFunctionName));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			if(reader.TokenType == JsonToken.String) {
				return reader.Value.ToString().ToLower() switch {
					"get_manual_url" => OpenAiFunctionName.get_manual_url,
					_ => throw new JsonSerializationException($"Invalid value for OpenAiFunctionName: {reader.Value.ToString()}"),
				};
			}
			throw new JsonSerializationException("Unexpected token type for OpenAiFunctionName");
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			throw new NotImplementedException();
		}
	}
}
