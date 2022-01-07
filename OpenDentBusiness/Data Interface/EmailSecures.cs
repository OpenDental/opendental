using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using CodeBase;
using Health.Direct.Common.Extensions;
using OpenDentBusiness.Email;
using OpenDentBusiness.FileIO;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EmailSecures {
		private static char[] _arrEmailAddressDelimiters=new char[] { ';',',' };
		///<summary>Gets all emailsecure rows by EmailFK.</summary>
		public static List<EmailSecure> GetByEmailFK(List<long> listEmailFKs) {
			if(listEmailFKs.IsNullOrEmpty()) {
				return new List<EmailSecure>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<List<EmailSecure>>(MethodBase.GetCurrentMethod(),listEmailFKs);
			}
			string command=$"SELECT * FROM emailsecure WHERE emailsecure.EmailFK IN ({string.Join(",",listEmailFKs.Select(x => POut.Long(x)))}) ";
			return Crud.EmailSecureCrud.SelectMany(command);
		}

		///<summary>Gets an emailsecure row by EmailMessageNum.</summary>
		public static EmailSecure GetByEmailMessageNum(long emailMessageNum) {
			if(emailMessageNum<=0) {
				return null;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<EmailSecure>(MethodBase.GetCurrentMethod(),emailMessageNum);
			}
			string command=$"SELECT * FROM emailsecure WHERE emailsecure.EmailMessageNum = {POut.Long(emailMessageNum)} ";
			return Crud.EmailSecureCrud.SelectOne(command);
		}

		///<summary>Gets all emailsecure rows by primary key.</summary>
		public static List<EmailSecure> GetMany(List<long> listEmailSecureNums,bool doIncludePending=false) {
			if(listEmailSecureNums.IsNullOrEmpty()) {
				return new List<EmailSecure>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<List<EmailSecure>>(MethodBase.GetCurrentMethod(),listEmailSecureNums,doIncludePending);
			}
			string command=$"SELECT * FROM emailsecure WHERE emailsecure.EmailSecureNum IN ({string.Join(",",listEmailSecureNums.Select(x => POut.Long(x)))}) ";
			if(!doIncludePending) {
				command+="AND emailsecure.EmailMessageNum > 0";
			}
			return Crud.EmailSecureCrud.SelectMany(command);
		}

		///<summary>Gets all emailsecure rows that have not yet downloaded the email from the EmailHosting API.  Pass empty list to get for all clinics.</summary>
		public static List<EmailSecure> GetOutstanding(List<long> listClinicNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<List<EmailSecure>>(MethodBase.GetCurrentMethod(),listClinicNums);
			}
			string command="SELECT * FROM emailsecure WHERE emailsecure.EmailMessageNum=0 ";
			if(!listClinicNums.IsNullOrEmpty()) {
				command+="AND emailsecure.ClinicNum IN ("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))+")";
			}
			return Crud.EmailSecureCrud.SelectMany(command);
		}

		public static SerializableDictionary<long,List<EmailSecure>> GetEmailChains(List<long> listEmailChainFKs) {
			if(listEmailChainFKs.Count==0) {
				return new SerializableDictionary<long, List<EmailSecure>>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetSerializableDictionary<long,List<EmailSecure>>(MethodBase.GetCurrentMethod(),listEmailChainFKs);
			}
			string command="SELECT * FROM emailsecure WHERE emailsecure.EmailChainFK IN ("
				+string.Join(",",listEmailChainFKs.Distinct().Select(x => POut.Long(x)))+")";
			return Crud.EmailSecureCrud.SelectMany(command).GroupBy(x => x.EmailChainFK).ToSerializableDictionary(x => x.Key,x => x.ToList());
		}

		///<summary></summary>
		public static long Insert(EmailSecure emailSecure){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				emailSecure.EmailSecureNum=Meth.GetLong(MethodBase.GetCurrentMethod(),emailSecure);
				return emailSecure.EmailSecureNum;
			}
			return Crud.EmailSecureCrud.Insert(emailSecure);
		}

		///<summary></summary>
		public static void InsertMany(List<EmailSecure> listEmailSecures) {
			if(listEmailSecures.Count==0) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listEmailSecures);
				return;
			}
			Crud.EmailSecureCrud.InsertMany(listEmailSecures);
		}

		///<summary></summary>
		public static void Update(EmailSecure emailSecure){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),emailSecure);
				return;
			}
			Crud.EmailSecureCrud.Update(emailSecure);
		}		

		///<summary>Sends a Secure Email.  Determines if the email is a reply, a new email, or if recipients have been added.  Creates copies of the email
		///if recipients have changed.  Updates EmailMessage rows appropriately.  Inserts EmailSecure rows as needed.</summary>
		public static void SendSecureEmail(EmailMessage messageDb,EmailAddress senderAddress,string toAddress,string ccAddress,string bccAddress
			,long clinicNum,EmailMessage messageReplyingTo=null) 
		{			
			IAccountApi api=EmailHostingTemplates.GetAccountApi(clinicNum);
			messageDb.MsgType=EmailMessageSource.Hosting;
			EmailResource email=ToEmailResource(messageDb,senderAddress);
			//Addresses from the message we are replying to, if applicable.
			List<EmailAddressResource> listReplyingToOriginal=new List<EmailAddressResource>();
			if(messageReplyingTo!=null) {
				EmailMessage reply=new EmailMessage();
				EmailMessages.FillEmailAddressesForReply(reply,messageReplyingTo,senderAddress,isReplyAll:true);//Secure Email is ALWAYS 'Reply All'
				//These are the email addresses included on the email message we are replying to.
				listReplyingToOriginal.AddRange(ToEmailAddressResources(reply.FromAddress));
				listReplyingToOriginal.AddRange(ToEmailAddressResources(reply.CcAddress));
			}
			//Addresses parsed from the UI.  The user may have added addresses that were not in the original email.
			List<EmailAddressResource> listToAddresses=ToEmailAddressResources(toAddress);
			List<EmailAddressResource> listCcAddresses=ToEmailAddressResources(ccAddress);
			List<EmailAddressResource> listBccAddresses=ToEmailAddressResources(bccAddress);
			#region Upload attachments
			email.ListAttachments=UploadSecureAttachments(api,messageDb.Attachments);
			#endregion
			List<EmailSecure> listEmailSecures=new List<EmailSecure>();
			List<Action> listSendEmailActions=new List<Action>();
			//Determine if this is a reply email
			EmailSecure replyingToEmailSecure=GetByEmailMessageNum(messageReplyingTo?.EmailMessageNum??0);//Returns null if not found.
			long emailChainFk=replyingToEmailSecure?.EmailChainFK??0;
			//We know this SecureEmail is a reply to a previous SecureEmail if the previous email is linked to an EmailChain.
			bool isReply=emailChainFk!=0;
			#region Send Reply
			//Send a reply to all of the original recipients.  Allows original email chain to be preserved.
			if(isReply) {
				//Ensure this EmailMessage is addressed to only the email addresses that are being replied to.
				messageDb.ToAddress=string.Join(";",listReplyingToOriginal.Select(x => x.Address));
				listSendEmailActions.Add(new Action(() => listEmailSecures.Add(SendReplySecureEmail(api,email,emailChainFk,messageDb,clinicNum))));
			}
			#endregion
			#region Send New Emails
			//Send a new email to all of the To and CC recipients if user added anyone new.  This is a new email chain.
			List<EmailAddressResource> listVisibleEmailAddresses=listToAddresses.Concat(listCcAddresses).ToList();
			//This is a brand new email.
			if(!isReply) {
				//Ensure this EmailMessage is addressed to the correct email addresses in the database.
				messageDb.ToAddress=string.Join(";",listToAddresses.Select(x => x.Address));
				messageDb.CcAddress=string.Join(";",listCcAddresses.Select(x => x.Address));
				//Send a single new Secure Email to the To and CC addresses
				listSendEmailActions.Add(() => listEmailSecures.Add(SendNewSecureEmail(api,email,listVisibleEmailAddresses,messageDb,clinicNum)));
			}
			//This was a reply, but the user added to or removed from the original list of recipients on the email chain.  We must also start a new email chain.
			else if(
				//The user added a new recipient
				listVisibleEmailAddresses.Select(x => x.Address).Except(listReplyingToOriginal.Select(y => y.Address)).Count()!=0 ||
				//The user removed an original recipient
				listReplyingToOriginal.Select(x => x.Address).Except(listVisibleEmailAddresses.Select(y => y.Address)).Count()!=0) 
			{
				EmailMessage messageNewDb=messageDb.Copy();
				messageNewDb.ToAddress=string.Join(";",listToAddresses.Select(x => x.Address));
				messageNewDb.CcAddress=string.Join(";",listCcAddresses.Select(x => x.Address));
				EmailMessages.Insert(messageNewDb);
				//Send a single new Secure Email to the To and CC addresses
				listSendEmailActions.Add(() => listEmailSecures.Add(SendNewSecureEmail(api,email,listVisibleEmailAddresses,messageNewDb,clinicNum)));
			}
			//Send a single new Secure Email to each of the Bcc addresses.  This is a new email chain.
			foreach(EmailAddressResource bcc in listBccAddresses) {
				EmailMessage messageNewDb=messageDb.Copy();
				messageNewDb.ToAddress=bcc.Address;
				messageNewDb.CcAddress="";
				EmailMessages.Insert(messageNewDb);
				listSendEmailActions.Add(() => listEmailSecures.Add(SendNewSecureEmail(api,email,new List<EmailAddressResource> { bcc },messageNewDb,clinicNum)));
			}
			#endregion
			//Send all the emails.
			RunWebCalls(listSendEmailActions);
			InsertMany(listEmailSecures);
		}

		
		private static List<EmailAddressResource> ToEmailAddressResources(string emailAddressStr) {
			return (emailAddressStr??"").Split(_arrEmailAddressDelimiters,StringSplitOptions.RemoveEmptyEntries)
				.Select(x => {
					string emailAddressDecoded=EmailMessages.ProcessInlineEncodedText(x.Trim().ToLower());
					if(!EmailAddresses.IsValidEmail(emailAddressDecoded,out MailAddress mailAddress)) {
						throw new ArgumentException(Lans.g("EmailSecure","Invalid email address: ")+x);
					}
					return new EmailAddressResource {
						Address=mailAddress.Address,
						Alias=mailAddress.DisplayName,
					};})
				.ToList();
		}

		///<summary>Creates an EmailResource from an EmailMessage and EmailAddress.</summary>
		private static EmailResource ToEmailResource(EmailMessage message,EmailAddress senderAddress) {			
			EmailResource email=new EmailResource {
				FromAddress=new EmailAddressResource {
					Address=senderAddress.GetFrom(),
				},
				Subject=message.Subject,
				DateTimeEmail=DateTime.Now,
				ExternalTag=new ExternalTag(),
				ListAttachments=new List<AttachmentResource>(),
			};
			if(new EmailType[] { EmailType.Html, EmailType.RawHtml }.Contains(message.HtmlType)) {
				email.BodyHtml=EmailMessages.EmbedImages(message.HtmlText,message.AreImagesDownloaded);
			}
			else {
				email.BodyHtml=message.BodyText;
			}
			return email;
		}

		///<summary>Sends a single new Secure Email, updates the corresponding EmailMessage in the database, and returns an EmailSecure which has not
		///been inserted into the database.</summary>
		private static EmailSecure SendNewSecureEmail(IAccountApi api,EmailResource email,List<EmailAddressResource> listEmailAddresses,EmailMessage messageDb
			,long clinicNum)
		{
			return SendViaApi(messageDb,() => {
				SendNewEmailResponse response=api.SendNewEmail(new SendNewEmailRequest {
					EmailToSend=email,
					ListEmailAddresses=listEmailAddresses,
					DoSendNotificationsAsOwner=!ClinicPrefs.GetBool(PrefName.EmailHostingUseNoReply,clinicNum)
				});
				return ToEmailSecureSent(messageDb,clinicNum,response.EmailChainNum,response.EmailNum);
			});
		}

		///<summary>Sends a single Secure Email Reply, updates the corresponding EmailMessage in the database, and returns an EmailSecure which has not
		///been inserted into the database.</summary>
		private static EmailSecure SendReplySecureEmail(IAccountApi api,EmailResource email,long emailChainFk,EmailMessage messageDb,long clinicNum) {
			return SendViaApi(messageDb,() => {
				//Replying to an existing EmailChain
				SendReplyResponse response=api.SendReply(new SendReplyRequest {
					EmailToSend=email,
					EmailChainNum=emailChainFk,
					DoSendNotificationsAsOwner=!ClinicPrefs.GetBool(PrefName.EmailHostingUseNoReply,clinicNum)
				});
				return ToEmailSecureSent(messageDb,clinicNum,emailChainFk,response.EmailNum);
			});
		}

		///<summary>Wraps the API call to send the secure email in logic to ensure the EmailMessage is updated correctly in the database.</summary>
		private static EmailSecure SendViaApi(EmailMessage emailMessage,Func<EmailSecure> send) {
			try {
				emailMessage.SentOrReceived=EmailSentOrReceived.SecureEmailSent;//Set before send, so EmailMessage.FailReason is more informative on failure.
				EmailSecure emailSecure=send();
				return emailSecure;
			}
			catch(Exception ex) {
				EmailMessages.SetFailed(emailMessage,ex);
				throw;
			}
			finally {
				EmailMessages.Update(emailMessage);
			}
		}

		///<summary>Marks the EmailMessage as sent, updates the database, and returns an EmailSecure that has not be inserted into the database.</summary>
		private static EmailSecure ToEmailSecureSent(EmailMessage messageDb,long clinicNum,long emailChainNum,long emailNum) {
			return new EmailSecure {
				PatNum=messageDb.PatNum,
				ClinicNum=clinicNum,
				EmailChainFK=emailChainNum,
				EmailFK=emailNum,
				EmailMessageNum=messageDb.EmailMessageNum,
			};
		}

		///<summary>Uploads email attachments to EmailHosting server and returns a list of AttachmentResource to be used to send a Secure Email.</summary>
		private static List<AttachmentResource> UploadSecureAttachments(IAccountApi api,List<EmailAttach> listAttaches) {
			List<AttachmentResource> listAttachmentResources=new List<AttachmentResource>();
			List<AttachmentFile> listFiles=new List<AttachmentFile>();
			#region Load data into memory
			//Load all attachment data into memory in parallel.
			List<Action> listReadFileAction=listAttaches.Select(x => new Action(() => {
				//Get file info for the attachment.  May download from Cloud (i.e. Dropbox, etc).
				BasicEmailAttachment attachment=EmailMessages.GetListAttachmentsAndDownload(x).First();				
				string displayFileName=Path.GetFileNameWithoutExtension(attachment.DisplayedFilename);
				string extension=Path.GetExtension(attachment.DisplayedFilename);
				byte[] bytes=File.ReadAllBytes(attachment.FullPath);
				string bytesBase64=Convert.ToBase64String(bytes);
				listFiles.Add(new AttachmentFile {
					DisplayFileName=displayFileName,
					Extension=extension,
					BytesBase64=bytesBase64,
				});
			})).ToList();
			//Read all files
			RunWebCalls(listReadFileAction);
			#endregion
			#region Upload data to EmailHosting
			//Upload attachment data in parallel.
			List<Action> listUploads=listFiles.Select(x => new Action(() => {				
				UploadS3ObjectResponse response=api.UploadS3Object(new UploadS3ObjectRequest {
					FileName=x.DisplayFileName,
					Extension=x.Extension,
					ObjectPurpose=S3ObjectPurpose.SecureEmailAttachment,
					ObjectType=S3ObjectType.File,
					ObjectBytesBase64=x.BytesBase64,
				});
				listAttachmentResources.Add(new AttachmentResource {
					DisplayedFileName=x.DisplayFileName,
					Extension=x.Extension,
					GUID=response.S3ObjectGuid,
				});
			})).ToList();
			RunWebCalls(listUploads);
			#endregion
			return listAttachmentResources;
		}

		private static void RunWebCalls(List<Action> listActions) {
			//Run in parallel, because we are making web calls, and there is no reason to not do this in parallel
			//ODThread.RunParallel(listActions);
			//This is tremendously inefficient to run these web calls in serial, resulting in a degraded user experience.
			for(int i = 0;i<listActions.Count;i++) {
				listActions[i].Invoke();
			}
		}
		
		///<summary>Helper class to organize attachment data/metadata.</summary>
		private class AttachmentFile {
			public string DisplayFileName;
			public string Extension;
			public string BytesBase64;
		}
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Get Methods
		///<summary></summary>
		public static List<EmailSecure> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EmailSecure>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM emailsecure WHERE PatNum = "+POut.Long(patNum);
			return Crud.EmailSecureCrud.SelectMany(command);
		}
		
		///<summary>Gets one EmailSecure from the db.</summary>
		public static EmailSecure GetOne(long emailSecureNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<EmailSecure>(MethodBase.GetCurrentMethod(),emailSecureNum);
			}
			return Crud.EmailSecureCrud.SelectOne(emailSecureNum);
		}
		#endregion Get Methods
		#region Modification Methods
		///<summary></summary>
		public static void Delete(long emailSecureNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),emailSecureNum);
				return;
			}
			Crud.EmailSecureCrud.Delete(emailSecureNum);
		}
		#endregion Modification Methods
		#region Misc Methods
		

		
		#endregion Misc Methods
		*/



	}
}