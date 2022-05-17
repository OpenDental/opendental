using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class RepeatChargeT {

		public static RepeatCharge GetRepeatChargeCustomers(eServiceCode eService,long patNum)
		{
			return DataAction.GetCustomers(() => {
				var all=RepeatCharges.Refresh(patNum).ToList();
				var link=EServiceCodeLink.GetAll().FirstOrDefault(x => x.EService==eService)??new EServiceCodeLink();
				return all.FirstOrDefault(x => x.ProcCode==link.ProcCode);
			});
			
		}

		public static RepeatCharge CreateRepeatCharge(long patNum,string procCode,int chargeAmt,DateTime dateStart=default) {
			RepeatCharge repeatCharge=new RepeatCharge {
				ChargeAmt=chargeAmt,
				ChargeAmtAlt=-1,
				DateStart=dateStart==default ? DateTime.Today : dateStart,
				IsEnabled=true,
				PatNum=patNum,
				ProcCode=procCode,
			};
			Patient oldPat=Patients.GetPat(repeatCharge.PatNum);
			repeatCharge.RepeatChargeNum=DataAction.GetCustomers(() => RepeatCharges.Insert(repeatCharge));
			RepeatCharges.InsertRepeatChargeChangeSecurityLogEntry(repeatCharge,Permissions.RepeatChargeCreate,oldPat,isAutomated:true);
			return repeatCharge;
		}
	}
}
