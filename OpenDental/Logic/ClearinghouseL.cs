using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	///<summary></summary>
	public class ClearinghouseL {

		///<summary>Returns the clearinghouse specified by the given num.  Will only return an HQ-level clearinghouse.
		///Do not attempt to pass in a clinic-level clearinghouseNum.</summary>
		public static Clearinghouse GetClearinghouseHq(long clearinghouseNumHq) {
			return GetClearinghouseHq(clearinghouseNumHq,false);
		}

		///<summary>Returns the clearinghouse specified by the given num.  Will only return an HQ-level clearinghouse.
		///Do not attempt to pass in a clinic-level clearinghouseNum.  Can return null if no match found.</summary>
		public static Clearinghouse GetClearinghouseHq(long clearinghouseNumHq,bool suppressError) {
			Clearinghouse clearinghouse=Clearinghouses.GetClearinghouse(clearinghouseNumHq);
			if(clearinghouse==null && !suppressError) {
				MsgBox.Show("Clearinghouses","Error. Could not locate Clearinghouse.");
			}
			return clearinghouse;
		}

		///<summary></summary>
		public static string GetDescript(long clearinghouseNum) {
			if(clearinghouseNum==0) {
				return "";
			}
			return GetClearinghouseHq(clearinghouseNum).Description;
		}

	}
}