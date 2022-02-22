using System;
using System.Collections.Generic;
using System.Text;
using CodeBase;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class Icd10T {

		///<summary>Inserts the new Icd10 code and returns it.</summary>
		public static Icd10 CreateIcd10(string icd10code = "") {
			Icd10 icd10=new Icd10();
			icd10.Icd10Code=icd10code;
			if(icd10code=="") {
				icd10.Icd10Code="Z.100.100";
			}
			icd10.Icd10Code=icd10code;
			Icd10s.Insert(icd10);
			return icd10;
		}

	}
}
