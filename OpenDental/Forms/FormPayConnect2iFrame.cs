using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Nodes;
using System.Windows.Forms;
using CodeBase;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using OpenDentBusiness;
using static OpenDentBusiness.PayConnect2;

namespace OpenDental {
	///<summary>Use this form when there is no token on file for the credit card being used.</summary>
	public partial class FormPayConnect2iFrame:FormODBase {
		/// <summary>Controls if we request a tokenizer or payment type iFrame from PayConnect</summary>
		private bool _isAddingCard;
		private PayConnect2Response _response=new PayConnect2Response();
		private long _clinicNum;
		private int _amountInCents;

		public FormPayConnect2iFrame(long clinicNum,int amountInCents=0,bool isAddingCard=false) {
			_isAddingCard=isAddingCard;
			_clinicNum=clinicNum;
			_amountInCents=amountInCents;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private async void FormPayConnect2iFrame_Load(object sender,EventArgs e) {
			if(ODBuild.IsWeb()) {
				//Unable to support PayConnect 2 on OD Cloud for the following reasons: OD Cloud uses Thinfinity, which does not allow for using WebView2 controls, meaning cloud would need to use the
				//old WebBrowser control. This issue with this is we currently do not know of a way to retrieve the iFrame response from a WebBrowser control. Maybe when Payment Portal is finished
				//we could try using a modified version of that to send the transaction data to the office's eConnector. We could also try making a "dummy" html page that contains the iFrame that is
				//capable of storing the iFrame response and then parse the DOM afterthe user is finished.
				MsgBox.Show(this,"Open Dental Cloud does not currently support PayConnect version 2.");
				DialogResult=DialogResult.Cancel;
				Close(); 
				return;
			}
			string url="";
			try {
				url=GetiFrameUrl();
			}
			catch(ODException ex) {
				FriendlyException.Show("Error loading window.",ex);
				DialogResult=DialogResult.Cancel;
				Close();
				return;
			}
			//Cloud requires using the old web browser control due to constraints from thinfinity.
			if(ODBuild.IsWeb()) {
				webViewMain.Visible=false;
				webBrowserMain.Visible=true;
				//webBrowserMain.Navigate();
				webBrowserMain.DocumentCompleted+= webBrowserMain_DocumentCompleted;
				webBrowserMain.Navigate(url);
			}
			else {
				webViewMain.Visible=true;
				webBrowserMain.Visible=false;
				try {
					await webViewMain.Init();
					webViewMain.CoreWebView2.WebMessageReceived+=GetTransactionResult;
					await webViewMain.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.addEventListener(\'message\', e => { window.chrome.webview.postMessage(e.data); })");
				}
				catch(Exception ex){
					FriendlyException.Show("Error initializing window.",ex);
					DialogResult=DialogResult.Cancel;
					Close();
					return;
				}
				webViewMain.CoreWebView2.Navigate(url);
			}
			
		}

		///<summary>Throws exceptions.</summary>
		private string GetiFrameUrl() {
			EmbedSessionRequest embedSessionRequest=new EmbedSessionRequest();
			if(_isAddingCard) { 
				embedSessionRequest.Type=PayConnect2.IframeType.Tokenizer;
			}
			else {
				embedSessionRequest.Type=PayConnect2.IframeType.Payment;
				if(_amountInCents>0) {
					//Amount is optional, setting it just prevents it from being changed in the PayConnect iFrame.
					embedSessionRequest.Amount=_amountInCents;
				}
			}
			PayConnect2Response payConnect2Response=PayConnect2.PostEmbedSession(embedSessionRequest,_clinicNum);
			if(payConnect2Response.ResponseType==PayConnect2.ResponseType.EmbedSession) {
				return payConnect2Response.EmbedSessionResponse.Url;
			}
			throw new ODException("Error occurred retrieving payment form URL from PayConnect.");
		}

		///<summary>Throws exceptions.</summary>
		private void GetTransactionResult(object sender,CoreWebView2WebMessageReceivedEventArgs args) {
			iFrameResponse response=null;
			try {
				response=JsonConvert.DeserializeObject<iFrameResponse>(args.WebMessageAsJson);
			}
			catch(JsonException jEx) {
				//failed to deserialize, we probably did not recieve a success response from the iFrame.
				jEx.DoNothing();
			}
			catch (Exception ex) {
				throw new ODException("Error retrieving response from PayConnect.",ex);
			}
			//Immediately call the GetStatus endpoint  for easier processing.
			if(response!=null && response.IFrameStatus.ToLower()=="success") {
				//When adding a card the transaction status and reference ID fields will return null from PayConnect, therefore we cannot run GetStatus.
				if(_isAddingCard) {
					_response.iFrameResponse=response;
					_response.ResponseType=ResponseType.IFrame;
				}
				else {
					_response=GetTransactionStatus(_clinicNum,response.Response.ReferenceId);
				}
			}
		}

		public PayConnect2Response GetResponse() {
			return _response;
		}

		private void GetTransactionResultCloud(object sender, EventArgs e) {
			string result="";
		}

		private void webBrowserMain_DocumentCompleted(object sender,WebBrowserDocumentCompletedEventArgs e) {
			webBrowserMain.Document.AttachEventHandler("message",GetTransactionResultCloud);
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}


	}
}