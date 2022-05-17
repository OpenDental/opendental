using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class ApptFieldDefT {

		///<summary></summary>
		public static ApptFieldDef CreateApptFieldDef(string fieldName,ApptFieldType fieldType,string pickList="") {
			ApptFieldDef apptFieldDef=new ApptFieldDef();
			apptFieldDef.FieldName=fieldName;
			apptFieldDef.FieldType=fieldType;
			apptFieldDef.PickList=pickList;
			apptFieldDef.ItemOrder=ApptFieldDefs.GetCount();
			ApptFieldDefs.Insert(apptFieldDef);
			ApptFieldDefs.RefreshCache();
			return apptFieldDef;
		}

		public static void ClearApptFieldDefTable() {
			string command="DELETE FROM apptfielddef WHERE ApptFieldDefNum > 0";
			DataCore.NonQ(command);
		}
	}
}
