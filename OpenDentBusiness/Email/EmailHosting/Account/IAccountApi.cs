//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	public interface IAccountApi {

		///<summary></summary>
		CreateAccessCodeResponse CreateAccessCode(CreateAccessCodeRequest request);

		///<summary></summary>
		ValidateAccessCodeResponse ValidateAccessCode(ValidateAccessCodeRequest request);

		///<summary>Returns all the emails associated to the given chain id.</summary>
		GetChainEmailsResponse GetChainEmails(GetChainEmailsRequest request);

		///<summary>Returns the totcal count of emails ever associated to the given chain id.The primary key of the chain.</summary>
		GetChainEmailsTotalEmailsResponse GetChainEmailsTotalEmails(GetChainEmailsTotalEmailsRequest request);

		///<summary>Returns a secure email for the given primary key.The primary key of the secure email to get.</summary>
		GetEmailResponse GetEmail(GetEmailRequest request);

		///<summary>Queues all of the given template destinations for the given template to be sent.</summary>
		SendMassEmailResponse SendMassEmail(SendMassEmailRequest request);

		///<summary>Creates a new email and email chain for the given request. Sends appropriate account transmissions.</summary>
		SendNewEmailResponse SendNewEmail(SendNewEmailRequest request);

		///<summary>Sends a new email, attaches it to the current chain, and sends appropriate account transmissions.</summary>
		SendReplyResponse SendReply(SendReplyRequest request);

		///<summary>Sends a new email, attaches it to the current chain, and sends appropriate account transmissions.</summary>
		SendReplyWebClientResponse SendReplyWebClient(SendReplyWebClientRequest request);

		///<summary>Creates a new domain identity for the given domain. If the domain already exists as an identity, will return a BadRequest.</summary>
		CreateDomainIdentityResponse CreateDomainIdentity(CreateDomainIdentityRequest request);

		///<summary>Creates a new domain identity for the given domain. If the domain already exists as an identity, will return a BadRequest.</summary>
		CreateEmailAddressIdentityResponse CreateEmailAddressIdentity(CreateEmailAddressIdentityRequest request);

		///<summary>Removes the identity at the given primary key.The email address or domain of the identity to delete.</summary>
		DeleteIdentityResponse DeleteIdentity(DeleteIdentityRequest request);

		///<summary>For the given identity num, returns the domain tokens needed for the CNAME records of the DNS.The primary key of the identity.</summary>
		GetDomainDKIMTokensResponse GetDomainDKIMTokens(GetDomainDKIMTokensRequest request);

		///<summary>Gets all email identities (emails/domains that can be sent from) for the authorized account.</summary>
		GetIdentitiesResponse GetIdentities(GetIdentitiesRequest request);

		///<summary>Gets an email identity (emails/domains that can be sent from) with the given ID.The primary key of the identity.</summary>
		GetIdentityResponse GetIdentity(GetIdentityRequest request);

		///<summary>Returns signed download links for the S3 objects that are given.</summary>
		CreateSignedS3LinksResponse CreateSignedS3Links(CreateSignedS3LinksRequest request);

		///<summary>Uploads an object to the API which will in turn host the object on s3.</summary>
		UploadS3ObjectResponse UploadS3Object(UploadS3ObjectRequest request);

		///<summary>Creates a new template with the given information.</summary>
		CreateTemplateResponse CreateTemplate(CreateTemplateRequest request);

		///<summary>Removes a template for the given primary key. The template must belong to the account calling this method. If the template has already been removed, this will still return no content.The primary key of the template to delete.</summary>
		DeleteTemplateResponse DeleteTemplate(DeleteTemplateRequest request);

		///<summary>Returns all templates for the given account.</summary>
		GetAllTemplatesByAccountResponse GetAllTemplatesByAccount(GetAllTemplatesByAccountRequest request);

		///<summary>Returns all templates for the given account.</summary>
		GetAllTemplatesByGuarantorResponse GetAllTemplatesByGuarantor(GetAllTemplatesByGuarantorRequest request);

		///<summary>Gets a template with the given ID.The primary key of the template.</summary>
		GetTemplateResponse GetTemplate(GetTemplateRequest request);

		///<summary>Updates the Account.SignatureHtml and Account.SignaturePlanText.</summary>
		UpdateSignatureResponse UpdateSignature(UpdateSignatureRequest request);

		///<summary>Updates a template with the given primary key to the given information. All fields are required.</summary>
		UpdateTemplateResponse UpdateTemplate(UpdateTemplateRequest request);
	}
}
