using System;
using System.Collections.Generic;
using System.Text;
using CodeBase;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class Icd10T {
		///<summary>Deletes every entry in the in 'icd10' table.</summary>
		public static void ClearIcd10Table() {
			string command="DELETE from icd10";
			DataCore.NonQ(command);
		}

		///<summary>Inserts the new Icd10 code and returns it.</summary>
		public static Icd10 CreateIcd10(string icd10code = "",string description = "") {
			Icd10 icd10=new Icd10();
			if(icd10code=="") {
				icd10.Icd10Code="Z.100.100";
			}
			icd10.Icd10Code=icd10code;
			icd10.Description=description;
			icd10.Icd10Num=Icd10s.Insert(icd10);
			return icd10;
		}

	}
}
