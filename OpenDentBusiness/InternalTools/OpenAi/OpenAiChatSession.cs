using CodeBase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace OpenDentBusiness.OpenAi {
	public class OpenAiChatSession {
		#region Public Properties
		public WebChatSession ChatSession;
		#endregion
		#region Private Properties
		///<summary>Values come from preference.</summary>
		private readonly List<string> _listAssistantIds;
		///<summary>Values come from preference.</summary>
		private readonly string _additionalInstructions;
		///<summary>Chat thread, not a processing thread.</summary>
		private OAIThread _thread;
		private bool _isInitilized => (_thread!=null && ChatSession!=null && !_listAssistantIds.IsNullOrEmpty() && _additionalInstructions!=null);
		private HttpClient _odSearchClient = new HttpClient() {
			BaseAddress = new Uri("https://search.opendental.com/")
		};
		#endregion

		public OpenAiChatSession() {
			_listAssistantIds=WebChatPrefs.GetString(WebChatPrefName.OpenAiAssistantIds).Split(',').ToList();
			_additionalInstructions=WebChatPrefs.GetString(WebChatPrefName.OpenAiAdditionalInstructions);
		}

		#region Public Methods
		///<summary>Returns an error string if there was an issue, otherwise null.</summary>
		public async Task<string> SendMessagesAsync(string assistantId,params string[] arrayMessages) {
			try {
				for(int i=0;i<arrayMessages.Length;i++) {
					string msg=arrayMessages[i].Trim();
					if(_isInitilized) {
						await OpenAiClient.Inst.CreateMessage(_thread.Id,msg);
					}
					else if(i>0) {//Not initilized and we are past the first message, initilization must have failed.
						return Lans.g(this,"Failed to initilize OpenAI chat session.");
					}
					else {
						await Initialize(msg);
					}
					InsertWebChatSessionMsg(msg,WebChatMessageType.Technician);
				}
				if(await PollForRunCompletion(await OpenAiClient.Inst.CreateRun(_thread.Id,assistantId,_additionalInstructions))) {
					var response=await OpenAiClient.Inst.GetListMessages(_thread.Id);
					string aiResponse=response.Data.Last().Content.Last().Text.Value;
					InsertWebChatSessionMsg(aiResponse,WebChatMessageType.Ai);
					return null;//No errors
				}
			}
			catch(Exception ex) {
				return Lans.g(this,$"An error occured: {ex.Message}");
			}
			return Lans.g(this,$"The session was initilized, but the response could not be retrieved.");
		}

		///<summary>Throws Exception. 
		///Should only be called when resuming a previous AI chat that was closed then reopened.
		///Does not insert any webchat related table rows into the DB.</summary>
		public async System.Threading.Tasks.Task ResumeChatSession(WebChatSession session,List<WebChatMessage> listMessages) {
			ChatSession=session;
			_thread=await OpenAiClient.Inst.CreateThread(listMessages);
		}

		///<summary>Throws Exception. Returns a dictionary such that the key is the Assistant.Id and the value is the assistant.</summary>
		public async Task<List<OAIAssistant>> GetListAssistants() {
			var tasks=_listAssistantIds.Select(x => OpenAiClient.Inst.GetAssistant(x));
			return(await System.Threading.Tasks.Task.WhenAll(tasks)).ToList();
		}
		#endregion

		#region Private Methods
		///<summary>Throws Exception. Returns true if all mission-critical properties are initialized, otherwise false.</summary>
		private async Task<bool> Initialize(string msg) {
			if(_thread==null) {
				_thread=await OpenAiClient.Inst.CreateThread(msg);
			}
			if(ChatSession==null) {
				ChatSession=new WebChatSession() {
					TechName=Security.CurUser.UserName,
					DateTcreated=DateTime.Now,
					IsCustomer=false,
					PatNum=0,
					EmailAddress="",
					QuestionText=msg,
					PhoneNumber="",
					PracticeName="",
					IpAddress=ODEnvironment.GetLocalIPAddress(),
					UserName="",
					SessionType=WebChatSessionType.OpenAi
				};
				ChatSession.WebChatSessionNum=WebChatSessions.Insert(ChatSession);
			}
			return _isInitilized;
		}

		///<summary>Throws Exception. Polls OpenAI for job status changes. Returns true when run.Status is completed, otherwise false.</summary>
		private async Task<bool> PollForRunCompletion(OAIRun run) {
			try {
				int pollCount=0;
				while(pollCount<25) {
					run=await OpenAiClient.Inst.GetRun(_thread.Id,run.Id);
					switch(run.Status) {
						case OpenAiRunStatus.requires_action:
							await HandleRequiredActions(run);
							break;
						case OpenAiRunStatus.in_progress:
						case OpenAiRunStatus.queued:
							await System.Threading.Tasks.Task.Delay(3000);
							pollCount++;
							break;
						case OpenAiRunStatus.completed:
							return true;
						default:
						case OpenAiRunStatus.cancelling:
						case OpenAiRunStatus.cancelled:
						case OpenAiRunStatus.failed:
						case OpenAiRunStatus.expired:
							return false;
					}
				}
				if(pollCount >= 25) {
					throw new ApplicationException($"OpenAi poll count exceeded the limit. AssistantId: {run.AssistantId} ThreadId: {_thread.Id}");
				}
			}
			catch(Exception ex) {
				throw ex;
			}
			return false;
		}

		///<summary>Throws Exception. Returns true if local logic was ran and the result was sent back to OpenAi, otherwise false.</summary>
		private async Task<bool> HandleRequiredActions(OAIRun run) {
			foreach(OAIToolCall toolCall in run.RequiredAction.SubmitToolOutputs.ToolCalls) {
				switch(toolCall.Function.Name) {
					case OpenAiFunctionName.get_manual_url:
						OAIManualUrlFunctionArgs args=JsonConvert.DeserializeObject<OAIManualUrlFunctionArgs>(toolCall.Function.Arguments);
						string version=args.Version?.ToString().Replace(".", "")??"";
						string searchUrl=$"manual{version}/index?searchTerm={HttpUtility.UrlEncode(args.Keywords)}";
						List<DocumentationItem> result=await APIRequest.Inst.PostRequestAsync<List<DocumentationItem>>(searchUrl,String.Empty,clientOverride:_odSearchClient);
						if(!result.IsNullOrEmpty()) {
							List<string> listPages=result.Where(x => x.IsManualPage && x.IsInSite==EnumSiteOrManual.Manual)
								.Take(3).Select(page => 
									$"https://www.opendental.com/manual{version}/{page.FileName}.html"
								).ToList();
							await OpenAiClient.Inst.SubmitToolResult(_thread.Id, run.Id, toolCall.Id, string.Join(",", listPages));
							return true;
						}
						break;
				}
			}
			return false;
		}

		///<summary>Inserts a webChatMessage into HQ database.
		///Automatically sets the message.UserName to Security.CurUser.UserName if msgType is Technician, otherwise blank for AI.</summary>
		private WebChatMessage InsertWebChatSessionMsg(string msg,WebChatMessageType msgType) {
			WebChatMessage webChatMessage=new WebChatMessage() {
				WebChatSessionNum=ChatSession.WebChatSessionNum,
				DateT=DateTime.Now,
				IpAddress="",
				MessageText=msg,
				MessageType=msgType,
				UserName=(msgType==WebChatMessageType.Technician)?Security.CurUser.UserName:"",
			};
			webChatMessage.WebChatMessageNum=WebChatMessages.Insert(webChatMessage);
			return webChatMessage;
		}
		#endregion
	}

	///<summary>JSON object from the result of searching OD manual.</summary>
	public class DocumentationItem {
		public int VersionNumber;
		public string PageName;
		public string DisplayText;
		public string FileName;
		public bool IsManualPage;
		public EnumSiteOrManual IsInSite;
	}

	public enum EnumSiteOrManual {
		///<summary>Output to versioned manual folders.</summary>
		Manual,
		///<summary>Output to site folder and has site style.</summary>
		Site,
		///<summary>Output to site folder, but has TOC tree on the left and uses manual style.</summary>
		SiteWithTree
	}
}