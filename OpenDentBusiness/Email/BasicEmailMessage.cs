using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace OpenDentBusiness.Email {

	///<summary>Represents an email message at its rawest form. Stays away from associating with Open Dental.</summary>
	public class BasicEmailMessage {

		///<summary>BBC address for this message.</summary>
		public string BccAddress;
		///<summary>Should only be used with non-HTML emails. Should be scrubbed/tidyed before hand.</summary>
		public string BodyText;
		///<summary>CC address for this message.</summary>
		public string CcAddress;
		///<summary>The From address for this message.</summary>
		public string FromAddress;
		///<summary>Indicates if the body of this email is HTML.</summary>
		public bool IsHtml;
		///<summary>The subject of the email. Should be scrubbed/tidyed before hand.</summary>
		public string Subject;
		///<summary>The to address of the email.</summary>
		public string ToAddress;
		///<summary>The list of attachments for this email. The first string is the full file path. The second string is the name of the file 
		///(the one displayed to users). The file should already be downloaded and accessible.</summary>
		public List<BasicEmailAttachment> ListAttachments;
		///<summary>A list of paths to images that are included in the HTML body. The file should already be downloaded and accessible.</summary>
		public List<string> ListHtmlImages;
		///<summary>The body in the case that email is HTML. Should be scrubbed/tidyed before hand. This includes replacing image tags.</summary>
		public string HtmlBody;

	}

}
