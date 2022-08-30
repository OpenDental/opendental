using System;
using System.Windows.Forms;
using OpenDentBusiness;

namespace ODR{
	///<summary></summary>
	public class MakeReadable{
		///<summary></summary>
		public static string PatStatus(string patStatus){
			return Enum.GetName(typeof(PatientStatus),Convert.ToInt32(patStatus));
		}

		//public static string DefNumPaymentType(string defNum){
		//	return DefB.GetName(DefCat.PaymentTypes,PIn.PInt(defNum));
		//}

		public static string Query(string query,string parameters){
			MessageBox.Show("query: "+query);
			MessageBox.Show("parameter info: "+parameters);
			return "SELECT * FROM patient "+DbHelper.LimitWhere(10);
		}


	}

	

}
