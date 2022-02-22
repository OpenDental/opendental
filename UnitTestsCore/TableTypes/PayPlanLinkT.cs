using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class PayPlanLinkT {

		///<summary>For use with Dynamic Payment Plans. Production (proceudres and adjustments) is attached via PayPlanLinks.</summary>
		public static PayPlanLink CreatePayPlanLink(PayPlan payplan,long fKey,PayPlanLinkType linkType,double amountOverride=0) {
			return CreatePayPlanLink(payplan.PayPlanNum,fKey,linkType,amountOverride);
		}

		///<summary>For use with Dynamic Payment Plans. Production (proceudres and adjustments) is attached via PayPlanLinks.</summary>
		public static PayPlanLink CreatePayPlanLink(long payPlanNum,long fKey,PayPlanLinkType linkType,double amountOverride=0) {
			PayPlanLink link=new PayPlanLink() {
				AmountOverride=amountOverride,
				FKey=fKey,
				LinkType=linkType,
				PayPlanNum=payPlanNum,
			};
			PayPlanLinks.Insert(link);
			return link;
		}

		public static void UpdatePayPlanLinkSecurityDate(long payPlanLinkNum,DateTime secDateTEntry) {
			DataCore.NonQ($"UPDATE payplanlink SET SecDateTEntry={POut.DateT(secDateTEntry)} WHERE PayPlanLinkNum={POut.Long(payPlanLinkNum)}");
		}

	}
}
