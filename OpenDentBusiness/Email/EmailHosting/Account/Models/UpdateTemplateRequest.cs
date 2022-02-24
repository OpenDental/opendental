using System;
using System.Collections.Generic;
using System.IO;

//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	///<summary></summary>
	public class UpdateTemplateRequest {

		///<summary>The template num to update. This must belong to the account that is calling the method.</summary>
		public long TemplateNum { get; set; }

		///<summary>The contents of the template that should be updated.</summary>
		public Template Template { get; set; }

	}
}
