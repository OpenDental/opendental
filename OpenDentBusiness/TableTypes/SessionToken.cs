using System;

namespace OpenDentBusiness {
	///<summary>Stores the session token for when a user has logged into something.</summary>
	[Serializable]
  public class SessionToken:TableBase {
    ///<summary>Primary key.</summary>
    [CrudColumn(IsPriKey=true)]
    public long SessionTokenNum;
    ///<summary>The hash of the token. Hashed using SHA3_512 without a salt.</summary>
    public string SessionTokenHash;
    ///<summary>The datetime when this token will expire.</summary>
    [CrudColumn(SpecialType=CrudSpecialColType.DateT)]
    public DateTime Expiration;
    ///<summary>Enum:SessionTokenType The type of token this is.</summary>
    public SessionTokenType TokenType;
    ///<summary>The FKey this token is for. For Patient Portal tokens, this is patient.PatNum. For Mobile Web tokens, this is userod.UserNum.</summary>
    public long FKey;
    ///<summary>The plain text (non-hashed) version of the token.</summary>
    [CrudColumn(IsNotDbColumn=true)]
    public string TokenPlainText;

    ///<summary></summary>
    public SessionToken Copy() {
      return (SessionToken)this.MemberwiseClone();
    }

  }

  public enum SessionTokenType {
    ///<summary>0 - Should not be used in the database.</summary>
    Undefined,
    ///<summary>1 - The patient has logged in with a username and password.</summary>
    PatientPortal,
    ///<summary>2 - The OD user has logged in with a username and password.</summary>
    MobileWeb,
    ///<summary>3 - This token is for an OD HQ service that has authenticated to us.</summary>
    ODHQ,
    ///<summary>4 - The patient verified him or herself with just a name and birthdate.</summary>
    PatientPortalVerifyUser,
  }
}