using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web;
using CDO;
using CodeBase;
using MimeKit;
using MailKit.Security;
using GmailApi=Google.Apis.Gmail.v1;
using Google;
using System.Text.RegularExpressions;
using System.Linq;

namespace OpenDentBusiness.Email {

	public static class SendEmail {

		public const int EMAIL_SEND_TIMEOUT_MS=180000;//3 minutes

		///<summary>Throws exceptions. Attempts to physically send the message over the network wire. This is used from wherever email needs to be 
		///sent throughout the program. If a message must be encrypted, then encrypt it before calling this function. nameValueCollectionHeaders can 
		///be null.</summary>
		public static void WireEmailUnsecure(BasicEmailAddress address,BasicEmailMessage emailMessage,NameValueCollection nameValueCollectionHeaders
			,params AlternateView[] arrayAlternateViews) 
		{
			WireEmailUnsecure(address,emailMessage,nameValueCollectionHeaders,EMAIL_SEND_TIMEOUT_MS,arrayAlternateViews);
		}

		///<summary>Throws exceptions. Attempts to physically send the message over the network wire. This is used from wherever email needs to be 
		///sent throughout the program. If a message must be encrypted, then encrypt it before calling this function. nameValueCollectionHeaders can 
		///be null.</summary>
		public static void WireEmailUnsecure(BasicEmailAddress address,BasicEmailMessage emailMessage,NameValueCollection nameValueCollectionHeaders,
			int emailSendTimeoutMs=EMAIL_SEND_TIMEOUT_MS,params AlternateView[] arrayAlternateViews) {
			//For now we only support OAuth for Gmail but this may change in the future.
			if(!address.AccessToken.IsNullOrEmpty() && address.SMTPserver=="smtp.gmail.com") {
				SendEmailOAuth(address,emailMessage);
			}
			else {
				bool isImplicitSsl=(address.ServerPort==465);
				if(isImplicitSsl) {//Microsoft CDO supports implicit SSL, System.Net.Mail.SmtpClient only supports explicit SSL.
					var cdo=new Message();
					var cfg=cdo.Configuration;
					cfg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"].Value=address.SMTPserver;
					cfg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"].Value=address.ServerPort;
					cfg.Fields["http://schemas.microsoft.com/cdo/configuration/sendusing"].Value="2";//sendusing: 1=pickup, 2=port, 3=using microsoft exchange
					cfg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"].Value="1";//0=anonymous,1=clear text auth,2=context (NTLM)				
					cfg.Fields["http://schemas.microsoft.com/cdo/configuration/sendusername"].Value=address.EmailUsername.Trim();
					cfg.Fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"].Value=address.EmailPassword;
					cfg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"].Value=true;//false was also tested and does not work
					if(nameValueCollectionHeaders!=null) {
						//How to add headers which do not have formal fields - https://msdn.microsoft.com/en-us/library/ms526317(v=exchg.10).aspx
						string[] arrayHeaderKeys=nameValueCollectionHeaders.AllKeys;
						for(int i = 0;i<arrayHeaderKeys.Length;i++) {//Needed for Direct Acks to work.
							string headerName=arrayHeaderKeys[i];
							string headerValue=nameValueCollectionHeaders[headerName];
							cfg.Fields["urn:schemas:mailheader:"+headerName].Value=headerValue;
						}
					}
					cfg.Fields.Update();
					cdo.From=emailMessage.FromAddress.Trim();
					if(!string.IsNullOrWhiteSpace(emailMessage.ToAddress)) {
						cdo.To=emailMessage.ToAddress.Trim();
					}
					if(!string.IsNullOrWhiteSpace(emailMessage.CcAddress)) {
						cdo.CC=emailMessage.CcAddress.Trim();
					}
					if(!string.IsNullOrWhiteSpace(emailMessage.BccAddress)) {
						cdo.BCC=emailMessage.BccAddress.Trim();
					}
					cdo.Subject=emailMessage.Subject;
					if(emailMessage.IsHtml) {
						cdo.HTMLBody=emailMessage.HtmlBody;
						if(!emailMessage.ListHtmlImages.IsNullOrEmpty()) {
							foreach(string imagePath in emailMessage.ListHtmlImages) {
								var imgAttach=cdo.AddAttachment(imagePath);
								imgAttach.Fields["urn:schemas:mailheader:content-id"].Value=HttpUtility.UrlEncode(Path.GetFileName(imagePath));
								imgAttach.Fields.Update();
							}
						}
					}
					else {
						cdo.TextBody=emailMessage.BodyText;
					}
					if(!emailMessage.ListAttachments.IsNullOrEmpty()) {
						foreach(BasicEmailAttachment attachmentPath in emailMessage.ListAttachments) {
							var cdoatt=cdo.AddAttachment(attachmentPath.FullPath);
							//Use actual file name for this field.
							cdoatt.Fields["urn:schemas:mailheader:content-id"].Value=Path.GetFileName(attachmentPath.FullPath);
							cdoatt.Fields.Update();
						}
					}
					cdo.Send();
				}
				else {//No SSL or explicit SSL on port 587  
					SmtpClient client=null;
					MailMessage message=null;
					try {
						client=new SmtpClient(address.SMTPserver,address.ServerPort);
						//The default credentials are not used by default, according to: 
						//http://msdn2.microsoft.com/en-us/library/system.net.mail.smtpclient.usedefaultcredentials.aspx
						client.Credentials=new NetworkCredential(address.EmailUsername.Trim(),address.EmailPassword);
						client.DeliveryMethod=SmtpDeliveryMethod.Network;
						client.EnableSsl=address.UseSSL;
						client.Timeout=emailSendTimeoutMs;
						message=BasicEmailMessageToMailMessage(emailMessage,nameValueCollectionHeaders,arrayAlternateViews);
						client.Send(message);
					}
					finally {
						//Dispose of the client and messages here. For large customers, sending thousands of emails will start to fail until they restart the
						//app. Freeing memory here can prevent OutOfMemoryExceptions.
						client?.Dispose();
						if(message!=null) {
							if(message.Attachments!=null) {
								message.Attachments.ForEach(x => x.Dispose());
							}
							message.Dispose();
						}
					}
				}
			}
		}

		public static MailMessage BasicEmailMessageToMailMessage(BasicEmailMessage basicMessage,NameValueCollection nameValueCollectionHeaders,
			params AlternateView[] arrayAlternateViews) {
			MailMessage message=new MailMessage();
			message.From=new MailAddress(basicMessage.FromAddress.Trim());
			if(!string.IsNullOrWhiteSpace(basicMessage.ToAddress)) {
				message.To.Add(basicMessage.ToAddress.Trim());
			}
			if(!string.IsNullOrWhiteSpace(basicMessage.CcAddress)) {
				message.CC.Add(basicMessage.CcAddress.Trim());
			}
			if(!string.IsNullOrWhiteSpace(basicMessage.BccAddress)) {
				message.Bcc.Add(basicMessage.BccAddress.Trim());
			}
			message.Subject=basicMessage.Subject;
			if(basicMessage.IsHtml) {
				//create alternate view in case browser cannot render html
				message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(basicMessage.HtmlBody
					,new System.Net.Mime.ContentType("text/html")));
				message.IsBodyHtml=true;
				message.Body=basicMessage.HtmlBody;
				if(!basicMessage.ListHtmlImages.IsNullOrEmpty()) {
					foreach(string imagePath in basicMessage.ListHtmlImages) {
						Attachment imgAttach=new Attachment(imagePath);
						imgAttach.ContentId=HttpUtility.UrlEncode(Path.GetFileName(imagePath));
						imgAttach.ContentDisposition.Inline=true;
						message.Attachments.Add(imgAttach);
					}
				}
			}
			else {
				message.IsBodyHtml=false;
				message.Body=basicMessage.BodyText;
			}
			if(nameValueCollectionHeaders!=null) {
				message.Headers.Add(nameValueCollectionHeaders);//Needed for Direct Acks to work.
			}
			for(int i=0;i<arrayAlternateViews.Length;i++) {//Needed for Direct messages to be interpreted encrypted on the receiver's end.
				message.AlternateViews.Add(arrayAlternateViews[i]);
			}
			if(!basicMessage.ListAttachments.IsNullOrEmpty()) {
				foreach(BasicEmailAttachment attachment in basicMessage.ListAttachments) {
					//@"C:\OpenDentalData\EmailAttachments\1");
					Attachment attach=new Attachment(attachment.FullPath);
					//"canadian.gif";
					attach.Name=attachment.DisplayedFilename;
					message.Attachments.Add(attach);
				}
			}
			return message;
		}

		///<summary>Throws exceptions if failing to send emails or authenticate with Google.</summary>
		private static void SendEmailOAuth(BasicEmailAddress address,BasicEmailMessage message) {
			using GmailApi.GmailService gService=GoogleApiConnector.CreateGmailService(address);
			try {
				GmailApi.Data.Message gmailMessage=CreateGmailMsg(address,message);
				gService.Users.Messages.Send(gmailMessage,address.EmailUsername).Execute();
			}
			catch(GoogleApiException gae) {
				//This will bubble up to the UI level and be caught in a copypaste box.
				throw new GoogleApiException(gae.ServiceName,"Unable to authenticate with Google: "+gae.Message,gae);
			}
			catch(Exception ex) {
				//This will bubble up to the UI level and be caught in a copypaste box.
				throw new Exception($"Error sending email with OAuth authorization: {ex.Message}");
			}
		}

		///<summary>Helper method that creates a new MIME message based on the parameters (to, cc, bcc, from, and body)</summary>
		private static MimeMessage CreateMIMEMsg(BasicEmailAddress emailAddress,BasicEmailMessage emailMessage) {
			MimeMessage mimeMsg=new MimeMessage();
			if(!emailMessage.Subject.IsNullOrEmpty()) {
				mimeMsg.Subject=emailMessage.Subject;
			}
			//Create MailAddress objects for all addresses.
			//MailMessage will automatically parse out "DisplayName" <email@address.com> and every variation.
			//We are using MailMessage as an in-between because its Add() function can automatically parse a single string containing multiple email addresses into a collection which is needed for MimeMessage.
			MailMessage msg=new MailMessage();
			msg.From=new MailAddress(emailMessage.FromAddress);
			mimeMsg.From.Add(new MailboxAddress(msg.From.DisplayName,msg.From.Address));
			msg.To.Add(emailMessage.ToAddress.Trim());
			mimeMsg.To.AddRange(msg.To.Select(x => new MailboxAddress(x.DisplayName,x.Address)));
			if(!emailMessage.CcAddress.IsNullOrEmpty()) {
				msg.CC.Add(emailMessage.CcAddress.Trim());
				mimeMsg.Cc.AddRange(msg.CC.Select(x => new MailboxAddress(x.DisplayName,x.Address)));
			}
			if(!emailMessage.BccAddress.IsNullOrEmpty()) {
				msg.Bcc.Add(emailMessage.BccAddress.Trim());
				mimeMsg.Bcc.AddRange(msg.Bcc.Select(x => new MailboxAddress(x.DisplayName,x.Address)));
			}
			BodyBuilder body=new BodyBuilder();
			if(!emailMessage.ListHtmlImages.IsNullOrEmpty()) {
				foreach(string imagePath in emailMessage.ListHtmlImages) {
					MimeEntity imgAttach=body.LinkedResources.Add(imagePath);
					imgAttach.ContentId=HttpUtility.UrlEncode(Path.GetFileName(imagePath));
					imgAttach.ContentDisposition=new ContentDisposition(ContentDisposition.Inline);
				}
			}
			if(emailMessage.ListAttachments!=null) {
				foreach(BasicEmailAttachment attachmentPath in emailMessage.ListAttachments) {
					body.Attachments.Add(attachmentPath.FullPath);
				}
			}
			if(emailMessage.IsHtml) {
				body.HtmlBody=emailMessage.HtmlBody;
			}
			else {
				body.TextBody=emailMessage.BodyText;
			}
			foreach(MimePart attachment in body.Attachments) {
				//Can break PDFs otherwise, since the file's 0A (/r) bytes are treated as line endings when getting encoded
				if(attachment.ContentTransferEncoding==ContentEncoding.QuotedPrintable) {
					attachment.ContentTransferEncoding=ContentEncoding.Base64;
				}
			}
			mimeMsg.Body=body.ToMessageBody();
			return mimeMsg;
		}

		///<summary>Helper method that returns a ready-to-send Gmail Message</summary>
		private static GmailApi.Data.Message CreateGmailMsg(BasicEmailAddress emailAddress,BasicEmailMessage emailMessage) {
			MimeKit.MimeMessage mimeMsg=CreateMIMEMsg(emailAddress,emailMessage);
			using Stream stream=new MemoryStream();
			mimeMsg.WriteTo(stream);
			stream.Position=0;
			using StreamReader sr=new StreamReader(stream);
			GmailApi.Data.Message gMsg=new GmailApi.Data.Message();
			string rawString=sr.ReadToEnd();
			byte[] raw=System.Text.Encoding.UTF8.GetBytes(rawString);
			gMsg.Raw=System.Convert.ToBase64String(raw);
			//What we send to Gmail must be a Base64 File/URL safe string.  We must convert our base64 to be URL safe. (replace - and _ with + and / respectively)
			gMsg.Raw=gMsg.Raw.Replace("+","-");
			gMsg.Raw=gMsg.Raw.Replace("/","_");
			return gMsg;
		}

	}

}
