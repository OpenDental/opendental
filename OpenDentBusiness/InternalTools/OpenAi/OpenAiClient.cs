using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OpenDentBusiness.OpenAi {
	//https://platform.openai.com/docs/assistants/how-it-works/agents
	public class OpenAiClient {
		public static OpenAiClient Inst=new OpenAiClient();
		private readonly HttpClient _httpClient;
		private const string _apiVersion="v1";

		public OpenAiClient() {
			_httpClient=new HttpClient {
				BaseAddress=new Uri($"https://api.openai.com/{_apiVersion}/")
			};
			_httpClient.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",WebChatPrefs.GetString(WebChatPrefName.OpenAiApiKey));
			_httpClient.DefaultRequestHeaders.Add("OpenAI-Beta",$"assistants={_apiVersion}");
		}

		#region Threads: An OpenAi conversation session between an Assistant and a user.
		///<summary>Throws Exception. Creates a thread on OpenAi, returns the Thread if successful. Otherwise null.</summary>
		public async Task<OAIThread> CreateThread(List<WebChatMessage> messages) {
			string content=JsonConvert.SerializeObject(new {
				messages=messages.Select(message => new {
					role=(message.MessageType==WebChatMessageType.Ai)?"assistant":"user",
					content=message.MessageText
				}).ToArray()
			});
			return await APIRequest.Inst.PostRequestAsync<OAIThread>("threads",content,clientOverride:_httpClient);
		}

		///<summary>Throws Exception. Creates a thread on OpenAi, returns the Thread if successful. Otherwise null.</summary>
		public async Task<OAIThread> CreateThread(string message) {
			string content=JsonConvert.SerializeObject(new {
				messages=new[] {
						new {
							role="user",
							content=message
						}
					}
			});
			return await APIRequest.Inst.PostRequestAsync<OAIThread>("threads",content,clientOverride:_httpClient);
		}

		///<summary>Throws Exception. Returns a thread from OpenAi if successful. Otherwise null.</summary>
		public async Task<OAIThread> GetThread(string id) {
			return await APIRequest.Inst.GetRequestAsync<OAIThread>($"threads/{id}",_httpClient);
		}
		#endregion

		#region Messages: The messages that are used in a specific OpenAi Thread.
		///<summary>Throws Exception. Creates a message on OpenAi, returns the message if successful. Otherwise null.</summary>
		public async Task<OAIMessage> CreateMessage(string threadId,string msgContent,params string[] fileIds) {
			string content=JsonConvert.SerializeObject(new {
				role="user",
				content=msgContent,
				file_ids=fileIds??new string[0],
			});
			return await APIRequest.Inst.PostRequestAsync<OAIMessage>($"threads/{threadId}/messages",content,clientOverride:_httpClient);
		}

		///<summary>Throws Exception. Returns a message from OpenAi for the given threadId and messageId if successful. Otherwise null.</summary>
		public async Task<OAIMessage> GetMessage(string threadId,string messageId) {
			return await APIRequest.Inst.GetRequestAsync<OAIMessage>($"threads/{threadId}/messages/{messageId}",_httpClient);
		}

		///<summary>Throws Exception. Returns a list messages from OpenAi for the given threadId if successful. Otherwise null.</summary>
		public async Task<OAIAssistantsListResponse<OAIMessage>> GetListMessages(string threadId) {
			return await APIRequest.Inst.GetRequestAsync<OAIAssistantsListResponse<OAIMessage>>($"threads/{threadId}/messages?limit=20&order=asc",_httpClient);
		}
		#endregion

		#region Runs: An OpenAi invocation of an Assistant on a Thread.
		///<summary>Throws Exception. Creates a run on OpenAi, returns the run if successful. Otherwise null.</summary>
		public async Task<OAIRun> CreateRun(string threadId,string assistantId,string additionalInstructions=null) {
			string content=JsonConvert.SerializeObject(new {
				assistant_id=assistantId,
				additional_instructions=additionalInstructions,
			});
			return await APIRequest.Inst.PostRequestAsync<OAIRun>($"threads/{threadId}/runs",content,clientOverride:_httpClient);
		}

		///<summary>Throws Exception. Returns a run from OpenAi for the given threadId and runId if successful. Otherwise null.</summary>
		public async Task<OAIRun> GetRun(string threadId,string runId) {
			return await APIRequest.Inst.GetRequestAsync<OAIRun>($"threads/{threadId}/runs/{runId}",_httpClient);
		}

		///<summary>Throws Exception. Returns the Run if succesful, otherwise null.</summary>
		public async Task<OAIRun> SubmitToolResult(string threadId,string runId,string toolCallId,object result) {
			string content=JsonConvert.SerializeObject(new {
				tool_outputs=new[] {
					new {
						tool_call_id=toolCallId,
						output=result.ToString()
					}
				}
			});
			return await APIRequest.Inst.PostRequestAsync<OAIRun>($"threads/{threadId}/runs/{runId}/submit_tool_outputs",content,clientOverride:_httpClient);
		}
		#endregion

		///<summary>Throws Exception. Returns an Assistant from OpenAi for the given id if successful. Otherwise null.</summary>
		public async Task<OAIAssistant> GetAssistant(string id) {
			return await APIRequest.Inst.GetRequestAsync<OAIAssistant>($"assistants/{id}",_httpClient);
		}
	}

}
