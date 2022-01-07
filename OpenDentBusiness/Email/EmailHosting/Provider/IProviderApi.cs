//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	public interface IProviderApi {

		///<summary>Creates a new account. ExternalID must be unique for this account guarantor.</summary>
		CreateAccountResponse CreateAccount(CreateAccountRequest request);

		///<summary>Gets an AccountGuarantor for the given primary key.</summary>
		GetAccountResponse GetAccount(GetAccountRequest request);

		///<summary>Updates the status for the given account.</summary>
		UpdateAccountStatusResponse UpdateAccountStatus(UpdateAccountStatusRequest request);

		///<summary>Creates a new account guarantor. ExternalID must be unique for this provider.</summary>
		CreateAccountGuarantorResponse CreateAccountGuarantor(CreateAccountGuarantorRequest request);

		///<summary>Gets an AccountGuarantor for the given primary key.</summary>
		GetAccountGuarantorResponse GetAccountGuarantor(GetAccountGuarantorRequest request);

		///<summary>Returns all of the usage statistics for the given external IDs. If an external ID is not in the list, it will not be returned in the result.</summary>
		GetGuarantorUsageResponse GetGuarantorUsage(GetGuarantorUsageRequest request);

		///<summary>Updates the status of all account's associated to this AccountGuarantor.</summary>
		UpdateAccountGuarantorStatusResponse UpdateAccountGuarantorStatus(UpdateAccountGuarantorStatusRequest request);

		///<summary>Returns health status for the various Secure Email services.</summary>
		GetHealthResponse GetHealth(GetHealthRequest request);
	}
}
