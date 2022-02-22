using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class AllergyDefT {

		///<summary></summary>
		public static AllergyDef CreateAllergyDef(string description,long medicationNum=0,bool isHidden=false,SnomedAllergy snomedType=SnomedAllergy.None) {
			AllergyDef allergyDef=new AllergyDef();
			allergyDef.DateTStamp=DateTime.Now;
			allergyDef.Description=description;
			allergyDef.IsHidden=isHidden;
			allergyDef.MedicationNum=medicationNum;
			allergyDef.SnomedType=snomedType;
			allergyDef.UniiCode="";
			AllergyDefs.Insert(allergyDef);
			return allergyDef;
		}

		public static void ClearAllergyDefTable() {
			string command=$"DELETE FROM allergydef WHERE AllergyDefNum > 0";
			DataCore.NonQ(command);
		}
	}
}
