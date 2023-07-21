using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsWeb {
	public class PatFieldDefT {
		public static PatFieldDef CreateTextPatFieldDef(int itemOrder=0) {
			PatFieldDef patFieldDef=new PatFieldDef {
				FieldName="Text",
				FieldType=PatFieldType.Text,
				IsHidden=false,
				ItemOrder=itemOrder,
				PickList="",
			};
			long patFieldDefNum=PatFieldDefs.Insert(patFieldDef);
			patFieldDef.PatFieldDefNum=patFieldDefNum;
			PatFieldDefs.Update(patFieldDef,"Text");
			return patFieldDef;
		}
	}
}
