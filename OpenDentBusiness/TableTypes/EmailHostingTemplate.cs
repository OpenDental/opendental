using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	[Serializable]
	public class EmailHostingTemplate:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EmailHostingTemplateNum;
		///<summary>Name of the template.</summary>
		public string TemplateName;
		///<summary>Default subject line.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Subject;
		///<summary>Body of the email</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string BodyPlainText;
		///<summary>Body of the email. When email is regular html this will only contain the body text. Will contain full html when email type is RawHtml</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string BodyHTML;
		///<summary>The email hosting template's identifier </summary>
		public long TemplateId;
		///<summary>FK to clinic.ClinicNum </summary>
		public long ClinicNum;
		///<summary>Enum:EmailType The type of email template this is (Regular HTML or Full HTML)</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public EmailType EmailTemplateType;
		///<summary>Enum:PromotionType the type of mass email this template is for</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public PromotionType TemplateType;

		///<summary>Returns a copy of this EmailTemplate.</summary>
		public EmailHostingTemplate Copy(){
			return (EmailHostingTemplate)this.MemberwiseClone();
		}
	}
}
