using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDentBusiness.HL7;
using OpenDentBusiness.UI;
using CodeBase;

namespace OpenDental{
	public class PatRestrictionL {
		///<summary>Checks for an existing patrestriction for the specified patient and PatRestrictType. If one exists returns true.
		///boolean to suppress or show message. If suppress message is false, will display msgbox.</summary>
		public static bool IsRestricted(long patNum,PatRestrict patRestrictType,bool suppressMessage = false) {
			if(PatRestrictions.IsRestricted(patNum,patRestrictType)) {
				if(!suppressMessage) {
					MessageBox.Show(Lans.g("PatRestrictions","Not allowed due to patient restriction")+"\r\n"+PatRestrictions.GetPatRestrictDesc(patRestrictType));
				}
				return true;
			}
			else {
				return false;
			}
		}
	}
}
