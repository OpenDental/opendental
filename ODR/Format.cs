using System;
using System.Windows.Forms;
using OpenDentBusiness;

namespace ODR{
	///<summary></summary>
	public class Format{
		///<summary></summary>
		public static string NumberHideZero(object original){//string original){
			if(original==null){
				return "";
			}
			double num=(double)original;//PIn.PDouble(original);
			if(num==0){
				return "";
			}
			return num.ToString("N");
			//}
		}



	}

	

}
