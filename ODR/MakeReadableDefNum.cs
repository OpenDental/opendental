using System;
using System.Collections;
using System.Data;
using OpenDentBusiness;

namespace ODR{
	///<summary></summary>
	public class MakeReadableDefNum{
		//private Hashtable hash;


		///<summary>Constructor</summary>
		public MakeReadableDefNum(){
			//hash=new Hashtable();
			string command="SELECT * FROM definition ORDER BY Category,ItemOrder";
			DataConnection dcon=new DataConnection();
			DataTable table=dcon.GetTable(command);
			Defs.FillCacheFromTable(table);
		}

		public string GetPaymentType(string defNum){
			return "any";
			//return DefB.GetName(DefCat.PaymentTypes,PIn.PInt(defNum));
		}
		
	}

}
