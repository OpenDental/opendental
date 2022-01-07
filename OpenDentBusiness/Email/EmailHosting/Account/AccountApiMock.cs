using CodeBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	public class AccountApiMock : IAccountApi {
		protected static Dictionary<long,AccountApiMock> _dictApis=new Dictionary<long, AccountApiMock>();
		protected static object _lock=new object();

		protected Dictionary<long,Template> _dictTemplates=new Dictionary<long, Template>();		
		protected static List<S3Object> _listS3Objects=new List<S3Object> {
			new S3Object {
				GUID=Guid.NewGuid().ToString(),
				Data="some data",
				DisplayName="smiley",
				Extension=".jpg",
				Url="https://dreamix.eu/blog/wp-content/uploads/2017/05/20150224test644-1508x706_c.jpg",
			},
		};
		protected List<EmailChain> _listEmailChains=new List<EmailChain> {
			new EmailChain {
				EmailChainNum=1,
				ListEmails=new List<Email> {
					new Email {
						EmailNum=1,
						EmailResource=new EmailResource { 
							FromAddress=new EmailAddressResource { Address="patient@emailaddress.com" },
							Subject="Test Email",
							BodyHtml="Test Email Body",
							ListAttachments=new List<AttachmentResource> { 
								new AttachmentResource {
									GUID=_listS3Objects.First().GUID,
									DisplayedFileName=_listS3Objects.First().DisplayName,
									Extension=_listS3Objects.First().Extension,
								}
						},
					},
				},
			},
			},
		};

		protected bool _hasCredentials;
		protected string _emailClinic;
		protected List<IdentityResource> _listIdentities=new List<IdentityResource>();
		public long ClinicNum;

		private AccountApiMock(string guid,string secret,long clinicNum,string emailClinic) {
			_hasCredentials=!string.IsNullOrWhiteSpace(guid) && !string.IsNullOrWhiteSpace(secret);
			_emailClinic=emailClinic;
			ClinicNum=clinicNum;
		}

		private void SimulateApiCall() {
			if(!_hasCredentials) {
				throw new ApplicationException($"HttpStatusCode returned: 403 - Forbiden\nInformation: NULL RESULT");
			}
		}

		public static IAccountApi Get(long clinicNum,string guid,string secret) {
			if(!_dictApis.TryGetValue(clinicNum,out AccountApiMock api)) {
				string emailClinic=EmailAddresses.GetByClinic(clinicNum).EmailUsername;
				api=new AccountApiMock(guid,secret,clinicNum,emailClinic);
				lock(_lock) {
					_dictApis[clinicNum]=api;
				}
			}
			return api;
		}

		public CreateDomainIdentityResponse CreateDomainIdentity(CreateDomainIdentityRequest request) {
			SimulateApiCall();
			//Not used yet.
			throw new NotImplementedException();
		}

		public CreateTemplateResponse CreateTemplate(CreateTemplateRequest request) {
			SimulateApiCall();
			long templateNum=(_dictTemplates.Count>0 ? _dictTemplates.Keys.Max() : 0)+1;
			_dictTemplates[templateNum]=request.Template;
			return new CreateTemplateResponse { 
				TemplateNum=templateNum,
			};
		}

		public DeleteIdentityResponse DeleteIdentity(DeleteIdentityRequest request) {
			SimulateApiCall();
			_listIdentities.RemoveAll(x => x.Identity==request.Address);
			return new DeleteIdentityResponse();
		}

		public DeleteTemplateResponse DeleteTemplate(DeleteTemplateRequest request) {
			SimulateApiCall();
			_dictTemplates.Remove(request.TemplateNum);
			return new DeleteTemplateResponse();
		}

		public GetAllTemplatesByAccountResponse GetAllTemplatesByAccount(GetAllTemplatesByAccountRequest request) {
			SimulateApiCall();
			return new GetAllTemplatesByAccountResponse {
				DictionaryTemplates=_dictTemplates, 
			};
		}

		public GetAllTemplatesByGuarantorResponse GetAllTemplatesByGuarantor(GetAllTemplatesByGuarantorRequest request) {
			SimulateApiCall();
			//Not used yet.
			throw new NotImplementedException();
		}

		public GetChainEmailsResponse GetChainEmails(GetChainEmailsRequest request) {
			SimulateApiCall();
			//Not used yet.
			throw new NotImplementedException();
		}

		public GetDomainDKIMTokensResponse GetDomainDKIMTokens(GetDomainDKIMTokensRequest request) {
			SimulateApiCall();
			//Not used yet.
			throw new NotImplementedException();
		}

		public GetEmailResponse GetEmail(GetEmailRequest request) {
			SimulateApiCall();
			EmailChain emailChain=GetEmailChainFromEmailNum(request.EmailNum);
			Email email=GetEmail(request.EmailNum,emailChain);
			if(email is null) {				
				throw new ApplicationException($"HttpStatusCode returned: 404.");
			}
			return new GetEmailResponse {
				Email=email.EmailResource,
				ChainOwner=new EmailAddressResource {
					Address=emailChain.ChainOwnerEmailAddress,
				},
				ListEmailAddresses=emailChain.ListEmailAddresses.Select(x => new EmailAddressResource {
					Address=x,
				}).ToList(),
			};
		}

		public GetIdentityResponse GetIdentity(GetIdentityRequest request) {
			SimulateApiCall();
			//Not used yet.
			throw new NotImplementedException();
		}

		public GetTemplateResponse GetTemplate(GetTemplateRequest request) {
			SimulateApiCall();
			//Not used yet.
			throw new NotImplementedException();
		}

		public SendMassEmailResponse SendMassEmail(SendMassEmailRequest request) {
			SimulateApiCall();
			return new SendMassEmailResponse {
				DictionaryUniqueIDToHostingID=request.ListDestinations.ToDictionary(x => x.UniqueID,x => (long)0),
			};
		}

		public SendNewEmailResponse SendNewEmail(SendNewEmailRequest request) {
			SimulateApiCall();
			EmailChain chain=new EmailChain {
				EmailChainNum=GetNewEmailChainNum(),
				ChainOwnerEmailAddress=request.EmailToSend.FromAddress.Address,
				ListEmails=new List<Email> { 
					new Email {
						EmailNum=GetNewEmailNum(),
						EmailResource=request.EmailToSend,//Also sets EmailAttachment.EmailFK where appropriate (if attachments have already been uploaded).
					}
				},
				ListEmailAddresses=request.ListEmailAddresses.Select(x => x.Address).ToList(),
				DoSendNotificationsAsOwner=request.DoSendNotificationsAsOwner,
			};
			_listEmailChains.Add(chain);
			return new SendNewEmailResponse {
				EmailChainNum=chain.EmailChainNum,
				EmailNum=chain.ListEmails.First().EmailNum,
			};
		}

		public SendReplyResponse SendReply(SendReplyRequest request) {
			SimulateApiCall();
			EmailChain chain=_listEmailChains.FirstOrDefault(x => x.EmailChainNum==request.EmailChainNum);
			Email email=new Email {
				EmailNum=GetNewEmailNum(),
				EmailResource=request.EmailToSend,
			};
			chain.ListEmails.Add(email);
			return new SendReplyResponse {
				EmailNum=email.EmailNum,
			};
		}

		public UpdateTemplateResponse UpdateTemplate(UpdateTemplateRequest request) {
			SimulateApiCall();
			if(_dictTemplates.TryGetValue(request.TemplateNum,out Template template)) {
				template=request.Template;
			}
			return new UpdateTemplateResponse();
		}

		public UploadS3ObjectResponse UploadS3Object(UploadS3ObjectRequest request) {
			string actualFileName=Path.ChangeExtension(request.FileName,request.Extension);
			EmailAttach attachment=EmailAttaches.CreateAttach(request.FileName,actualFileName,Convert.FromBase64String(request.ObjectBytesBase64),true);
			S3Object file=new S3Object {
				GUID=Guid.NewGuid().ToString(),
				Data=request.ObjectBytesBase64,
				DisplayName=request.FileName,
				Extension=request.Extension,
				Url=ODFileUtils.CombinePaths(EmailAttaches.GetAttachPath(),attachment.ActualFileName),
			};
			_listS3Objects.Add(file);
			return new UploadS3ObjectResponse {
				S3ObjectGuid=file.GUID,
				Url=file.Url,
			};
		}

		protected Email GetEmail(long emailNum,EmailChain emailChain=null) {
			return (emailChain??GetEmailChainFromEmailNum(emailNum))?.ListEmails?.FirstOrDefault(x => x.EmailNum==emailNum);
		}

		protected EmailChain GetEmailChainFromEmailNum(long emailNum) {
			return _listEmailChains.FirstOrDefault(x => x?.ListEmails?.Any(y => y.EmailNum==emailNum)??false);
		}

		protected long GetNewEmailNum() {
			//Can get away with Max here due to seeding _listEmailChains with default data
			return _listEmailChains.SelectMany(x => x.ListEmails).Max(x => x.EmailNum)+1;
		}

		protected long GetNewEmailChainNum() {
			//Can get away with Max here due to seeding _listEmailChains with default data
			return _listEmailChains.Max(x => x.EmailChainNum)+1;
		}

		public CreateAccessCodeResponse CreateAccessCode(CreateAccessCodeRequest request) {
			throw new NotImplementedException();
		}

		public ValidateAccessCodeResponse ValidateAccessCode(ValidateAccessCodeRequest request) {
			throw new NotImplementedException();
		}

		public CreateSignedS3LinksResponse CreateSignedS3Links(CreateSignedS3LinksRequest request) {
			SimulateApiCall();
			return new CreateSignedS3LinksResponse {
				DictGuidToLink=_listS3Objects.Where(x => ListTools.In(x.GUID,request.ListS3Guids)).ToDictionary(x => x.GUID,x => x.Url)
			};
		}

		public UpdateSignatureResponse UpdateSignature(UpdateSignatureRequest request) {
			SimulateApiCall();
			return new UpdateSignatureResponse();
		}

		public CreateEmailAddressIdentityResponse CreateEmailAddressIdentity(CreateEmailAddressIdentityRequest request) {
			_listIdentities.Add(new IdentityResource {
				Identity=request.EmailAddress.Address,
				VerificationStatus=IdentityVerificationStatus.Success,
				DKIMEnabled=false,
				DKIMVerificationStatus=IdentityVerificationStatus.Pending,
				VerificationToken=null,
			});
			return new CreateEmailAddressIdentityResponse {
				EmailIdentityNum=0,
			};
		}

		public GetIdentitiesResponse GetIdentities(GetIdentitiesRequest request) {
			SimulateApiCall();
			return new GetIdentitiesResponse {
				ListIdentityResources=_listIdentities,
			};
		}

		public GetChainEmailsTotalEmailsResponse GetChainEmailsTotalEmails(GetChainEmailsTotalEmailsRequest request) {
			SimulateApiCall();
			EmailChain chain=_listEmailChains.FirstOrDefault(x => x.EmailChainNum==request.EmailChainNum);
			return new GetChainEmailsTotalEmailsResponse {
				TotalEmails=chain?.ListEmails?.Count()??0,
			};
		}

		public SendReplyWebClientResponse SendReplyWebClient(SendReplyWebClientRequest request) {
			throw new NotImplementedException();//Only called from the web app.
		}

		protected class EmailChain {
			public long EmailChainNum;
			public string ChainOwnerEmailAddress;
			public List<Email> ListEmails;
			public List<string> ListEmailAddresses;
			public bool DoSendNotificationsAsOwner;
		}

		protected class Email {
			public long EmailNum;
			public EmailResource EmailResource { get; set; }
		}

		protected class S3Object {
			public string GUID;
			public string Url;
			public string Data;
			public string DisplayName;
			public string Extension;
		}
	}
}
