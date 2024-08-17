﻿using System;

namespace OpenDentBusiness.WebTypes.WebForms {
	[Serializable]
	[CrudTable(CrudLocationOverride=@"..\..\..\OpenDentBusiness\WebTypes\WebForms\Crud",NamespaceOverride="OpenDentBusiness.WebTypes.WebForms.Crud",CrudExcludePrefC=true)]
	public class WebForms_Log:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long LogNum;
		///<summary></summary>
		public long DentalOfficeID;
		///<summary></summary>
		public string WebSheetDefIDs;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string LogMessage;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		///<summary>FK to customers.registrationkey.RegistrationKeyNum</summary>
		public long RegistrationKeyNum;
		
    public WebForms_Log(){

		}
		
    public WebForms_Log Copy(){
			return (WebForms_Log)this.MemberwiseClone();
		}
	}
}