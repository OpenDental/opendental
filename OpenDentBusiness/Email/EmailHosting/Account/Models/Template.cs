using System;
using System.Collections.Generic;
using System.IO;

//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	///<summary></summary>
	public class Template {

		///<summary>The name of the template.</summary>
		public string TemplateName { get; set; }

		///<summary>The subject of the template. Replacements should be encoded with [{[{ KEY }]}]. KEY must be alphanumeric.</summary>
		public string TemplateSubject { get; set; }

		///<summary>The body of the template with HTML. Replacements should be encoded with [{[{ KEY }]}]. KEY must be alphanumeric. Must match plain text keys.</summary>
		public string TemplateBodyHtml { get; set; }

		///<summary>The body of the template, plain text. Replacements should be encoded with [{[{ KEY }]}]. KEY must be alphanumeric. Must match html keys.</summary>
		public string TemplateBodyPlainText { get; set; }

	}
}
