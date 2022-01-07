using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class EmailHostingTemplateT {
		public static EmailHostingTemplate CreateEmailHostingTemplate(long clinicNum,PromotionType templateType) 
		{
			EmailHostingTemplate template=EmailHostingTemplates.CreateDefaultTemplate(clinicNum,templateType);
			EmailHostingTemplates.Insert(template);
			return template;
		}
	}
}
