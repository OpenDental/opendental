using System;
using System.Collections.Generic;
using System.IO;

//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	///<summary></summary>
	public class TemplateDestination {

		///<summary>The email address of the destination for this template.</summary>
		public string Destination { get; set; }

		///<summary>A unique identifier for the caller. This will be returned with EmailHosting's unique identifier after queueing all the emails for sending.</summary>
		public string UniqueID { get; set; }

		///<summary>The key value pairs of replacements/tags for the subject of the given template. Must exactly match the template.</summary>
		public Dictionary<string,string> SubjectReplacements { get; set; }

		///<summary>The key value pairs of replacements/tags for the body of the given template. Must exactly match the template.</summary>
		public Dictionary<string,string> BodyReplacements { get; set; }

	}
}
