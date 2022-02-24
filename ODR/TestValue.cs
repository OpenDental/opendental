using System;
using System.Windows.Forms;
using OpenDentBusiness;

namespace ODR{
	///<summary></summary>
	public class TestValue{
		///<summary>Used to test the sign on debits and credits for the five different account types.  Pass in a number in string format.  Like "2", for example.</summary>
		public static bool AccountDebitIsPos(string accountType) {
			switch(accountType) {
				case "0"://asset
				case "4"://expense
					return true;
				case "1"://liability
				case "2"://equity //because liabilities and equity are treated the same
				case "3"://revenue
					return false;
			}
			return true;//will never happen
		}

	}

	

}
