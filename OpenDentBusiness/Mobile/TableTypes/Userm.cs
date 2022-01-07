using System;

namespace OpenDentBusiness.Mobile {

	///<summary>One username/password for one customer.</summary>
	[Serializable]
	[CrudTable(IsMobile=true)]
	public class Userm:TableBase {
		///<summary>Primary key 1.</summary>
		[CrudColumn(IsPriKeyMobile1=true)]
		public long CustomerNum;
		///<summary>Primary key 2.  Just here for compatibility with existing crud.  Always set to 1.</summary>
		[CrudColumn(IsPriKeyMobile2=true)]
		public long UsermNum;
		///<summary></summary>
		public string UserName;
		///<summary>Hashed in the same manner as the main program.  UTF-8, md5, base64.  See userods.EncryptPassword().</summary>
		public string Password;

		///<summary></summary>
		public Userm Copy() {
			return (Userm)this.MemberwiseClone();
		}



	}




}