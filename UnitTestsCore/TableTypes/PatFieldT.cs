using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsWeb {
	public class PatFieldT {
		public static void ClearPatFieldTable() {
			string command="DELETE FROM patfield";
			DataCore.NonQ(command);
		}

		public static PatField CreatePatField(long patNum,PatFieldDef patFieldDef,string fieldValue="Test") {
			PatField patField=new PatField {
				FieldName=patFieldDef.FieldName,
				FieldValue=fieldValue,
				PatNum=patNum,
				SecDateEntry=DateTime.Now,
				SecDateTEdit=DateTime.Now,
				SecUserNumEntry=0,
			};
			long patFieldNum=PatFields.Insert(patField);
			patField.PatFieldNum=patFieldNum;
			PatFields.Update(patField);
			return patField;
		}
	}
}
