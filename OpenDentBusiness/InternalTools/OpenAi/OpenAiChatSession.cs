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
		public EventHandler<SendInfo> OnSendStatusChanged=null;
		#endregion
		#region Private Properties
		///<summary>Values come from preference.</summary>
		private readonly List<string> _listAssistantIds;
		///<summary>Values come from preference.</summary>
		private readonly string _additionalInstructions;
		private readonly string _faqAssistantId;
		///<summary>Chat thread, not a processing thread.</summary>
		private OAIThread _thread;
		private bool _isOpenAiInitilized => (_thread!=null && !_listAssistantIds.IsNullOrEmpty() && _additionalInstructions!=null && _faqAssistantId!=null);
		private HttpClient _odSearchClient=new HttpClient() {
			BaseAddress=new Uri("https://search.opendental.com/")
		};
		///<summary>A list of ordered responses from AI. This can be useful when creating an in-memory chat session that is not stored in the DB.</summary>
		public List<string> ListAiResponses= new List<string>();
		#endregion

		///<summary>Constructor. Set additionalInstructionsOverride when you do not want to use DB WebChatPrefName.OpenAiAdditionalInstructions value.
		///Currently this is only done for the FAQ assistant prompts.</summary>
		public OpenAiChatSession(string additionalInstructionsOverride=null) {
			_listAssistantIds=WebChatPrefs.GetString(WebChatPrefName.OpenAiAssistantIds).Split(',').ToList();
			_additionalInstructions=additionalInstructionsOverride ?? WebChatPrefs.GetString(WebChatPrefName.OpenAiAdditionalInstructions);
			_faqAssistantId=WebChatPrefs.GetString(WebChatPrefName.OpenAiFaqAssistantId);
		}

		///<summary>Translates the description before calling the OnSendStatusChanged event.</summary>
		private void ReportSendStatus(SendStatus status,string description) {
			SendInfo sendInfo=new SendInfo();
			sendInfo.Status=status;
			sendInfo.Description=Lans.g(this,description);
			OnSendStatusChanged?.Invoke(this,sendInfo);
		}

		#region Public Methods
		///<summary>Returns an error string if there was an issue, otherwise null.</summary>
		public async Task<string> SendMessagesAsync(string assistantId,string msg,bool isSavedToDb=true) {
			bool didCreateThread=false;
			try {
				if(isSavedToDb && ChatSession==null) {
					ReportSendStatus(SendStatus.InitSession,"Sending question to AI");
					InitializeWebChatSession(msg);//The first message is stored in WebChatSession.QuestionText
				}
				if(isSavedToDb) {
					InsertWebChatSessionMsg(msg,WebChatMessageType.Technician);
					ReportSendStatus(SendStatus.QuestionSaved,"Saved question");
				}
				if(_thread==null) {
					ReportSendStatus(SendStatus.InitThread,"Initializing AI chat thread");
					await InitializeOpenAiThread(msg);
					didCreateThread=true;
				}
				if(!_isOpenAiInitilized) {
					return Lans.g(this,"Failed to initilize OpenAI chat session.");
				}
				if(!didCreateThread) {//When the thread is created, the initial message is already provided and does not need to be sent.
					ReportSendStatus(SendStatus.SendingToAI,"Sending question to AI");
					await OpenAiClient.Inst.CreateMessage(_thread.Id,msg);
				}
				ReportSendStatus(SendStatus.AwaitingResponse,"Waiting for response from AI");
				if(await PollForRunCompletion(await OpenAiClient.Inst.CreateRun(_thread.Id,assistantId,_additionalInstructions))) {
					var response=await OpenAiClient.Inst.GetListMessages(_thread.Id);
					string aiResponse=response.Data.Last().Content.Last().Text.Value;
					if(isSavedToDb) {
						InsertWebChatSessionMsg(aiResponse, WebChatMessageType.AI);
					}
					ListAiResponses.Add(aiResponse);
					return null;//No errors
				}
			}
			catch(Exception ex) {
				return Lans.g(this,$"An error occured: {ex.Message}");
			}
			finally {
				ReportSendStatus(SendStatus.Finished,"");
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
		///<summary>Throws Exception. Returns true if OpenAi's thread is created, otherwise false.</summary>
		private async Task<bool> InitializeOpenAiThread(string msg) {
			if(_thread==null) {
				_thread=await OpenAiClient.Inst.CreateThread(msg);
			}
			return _thread!=null;
		}

		///<summary>Creates a ChatSession if one has not already been created.</summary>
		private void InitializeWebChatSession(string msg) {
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
							ReportSendStatus(SendStatus.Error, $"An unexpected status has occured: {run.Status}");
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
			List<OAIToolOutput> results=new List<OAIToolOutput>();
			foreach(OAIToolCall toolCall in run.RequiredAction.SubmitToolOutputs.ToolCalls) {
				switch(toolCall.Function.Name) {
					case OpenAiFunctionName.get_manual_url:
						ReportSendStatus(SendStatus.RetrievingManualPages,"Retrieving manual pages from Open Dental");
						OAIManualUrlFunctionArgs argManual=JsonConvert.DeserializeObject<OAIManualUrlFunctionArgs>(toolCall.Function.Arguments);
						string version=argManual.Version?.ToString().Replace(".", "")??"";
						string searchUrl=$"manual{version}/index?searchTerm={HttpUtility.UrlEncode(argManual.Keywords)}";
						List<DocumentationItem> result=await APIRequest.Inst.PostRequestAsync<List<DocumentationItem>>(searchUrl,String.Empty,clientOverride:_odSearchClient);
						if(!result.IsNullOrEmpty()) {
							ReportSendStatus(SendStatus.InitThread,"Providing manual pages to AI");
							List<string> listPages=result.Where(x => x.IsManualPage && x.IsInSite==EnumSiteOrManual.Manual)
								.Take(3).Select(page => 
									$"https://www.opendental.com/manual{version}/{page.FileName}.html"
								).ToList();
							ReportSendStatus(SendStatus.RetrievingManualPages,"Providing manual pages to AI");
							results.Add(new OAIToolOutput() { 
								ToolCallId=toolCall.Id,
								Output=string.Join(",",listPages)
							});
						}
						else {
							ReportSendStatus(SendStatus.InitThread, "No manual pages were found");
						}
						break;
					case OpenAiFunctionName.get_faq_response:
						ReportSendStatus(SendStatus.RetrievingManualPages,"Checking Open Dental FAQs");
						OAIFaqResponseFunctionArgs argsFaq=JsonConvert.DeserializeObject<OAIFaqResponseFunctionArgs>(toolCall.Function.Arguments);
						string additionalInstructions=null;
						if(argsFaq.Version.HasValue) {
							additionalInstructions=$"I am using version {argsFaq.Version}, do not reference FAQs questions or answers that have a 'Minimum Version' that is greater than version {argsFaq.Version}.";
						}
						var session=new OpenAiChatSession(additionalInstructions);
						string error=await session.SendMessagesAsync(_faqAssistantId,argsFaq.Prompt,false);
						if(error.IsNullOrEmpty()) {
							ReportSendStatus(SendStatus.InitThread,"Providing FAQ data to AI");
							results.Add(new OAIToolOutput() { 
								ToolCallId=toolCall.Id,
								Output=session.ListAiResponses.Last()
							});
						}
						else { 
							ReportSendStatus(SendStatus.InitThread,"No information was found in the FAQ");
						}
						break;
				}
			}
			await OpenAiClient.Inst.SubmitToolResults(_thread.Id,run.Id,results);
			return results.Count()==run.RequiredAction.SubmitToolOutputs.ToolCalls.Count();
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

	public class SendInfo {
		public SendStatus Status;
		///<summary>The human-readable description of the Status.</summary>
		public string Description;
	}

	public enum SendStatus {
		InitSession,
		QuestionSaved,
		InitThread,
		SendingToAI,
		AwaitingResponse,
		RetrievingManualPages,
		Error,
		Finished
	}

}