using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class CodeGroupT {

		///<summary>Returns true if a new CodeGroup was added.</summary>
		public static bool AddIfNotPresent(string groupName,string procCodes,EnumCodeGroupFixed codeGroupFixed=EnumCodeGroupFixed.None,bool isHidden=false) {
			if(CodeGroups.GetExists(x => x.GroupName==groupName)) {
				return false;
			}
			CodeGroups.Insert(new CodeGroup() {
				CodeGroupFixed=codeGroupFixed,
				GroupName=groupName,
				ItemOrder=CodeGroups.GetCount(),
				IsHidden=isHidden,
				ProcCodes=procCodes,
			});
			CodeGroups.RefreshCache();
			return true;
		}

		///<summary>Updates or inserts a CodeGroup with the provided group name (and additional information). Returns the new or updated code group from cache.</summary>
		public static CodeGroup Upsert(string groupName,string procCodes,EnumCodeGroupFixed codeGroupFixed=EnumCodeGroupFixed.None,bool isHidden=false) {
			if(codeGroupFixed!=EnumCodeGroupFixed.None) {
				//There can only be one.
				string command="DELETE FROM codegroup WHERE CodeGroupFixed = "+POut.Enum(codeGroupFixed);
				DataCore.NonQ(command);
				CodeGroups.RefreshCache();
			}
			CodeGroup codeGroup=CodeGroups.GetFirstOrDefault(x => x.GroupName==groupName);
			if(codeGroup==null) {
				AddIfNotPresent(groupName,procCodes,codeGroupFixed:codeGroupFixed,isHidden:isHidden);
			}
			else {
				codeGroup.CodeGroupFixed=codeGroupFixed;
				codeGroup.ProcCodes=procCodes;
				codeGroup.IsHidden=isHidden;
				CodeGroups.Update(codeGroup);
				CodeGroups.RefreshCache();
			}
			return CodeGroups.GetFirst(x => x.GroupName==groupName);
		}

		public static void ResetToDefaults() {
			ClearCodeGroupTable();
			CodeGroupT.Upsert("BW Codes","D0272,D0274",EnumCodeGroupFixed.BW);
			CodeGroupT.Upsert("Exam Codes","D0120,D0150",EnumCodeGroupFixed.Exam);
			CodeGroupT.Upsert("PanoFMX Codes","D0210,D0330",EnumCodeGroupFixed.PanoFMX);
			CodeGroupT.Upsert("CancerScreening Codes","D0431");
			CodeGroupT.Upsert("Prophy Codes","D1110,D1120",EnumCodeGroupFixed.Prophy);
			CodeGroupT.Upsert("Fluoride Codes","D1206,D1208",EnumCodeGroupFixed.Fluoride);
			CodeGroupT.Upsert("Sealant Codes","D1351",EnumCodeGroupFixed.Sealant);
			CodeGroupT.Upsert("Crown Codes","D2740,D2750,D2751,D2752,D2780,D2781,D2782,D2783,D2790,D2791,D2792,D2794");
			CodeGroupT.Upsert("SRP Codes","D4341,D4342",EnumCodeGroupFixed.SRP);
			CodeGroupT.Upsert("FMDebride Codes","D4355",EnumCodeGroupFixed.FMDebride);
			CodeGroupT.Upsert("Perio Codes","D4910",EnumCodeGroupFixed.Perio);
			CodeGroupT.Upsert("Dentures Codes","D5110,D5120,D5130,D5140,D5211,D5212,D5213,D5214,D5221,D5222,D5223,D5224,D5225,D5226");
			CodeGroupT.Upsert("Implant Codes","D6010");
		}

		///<summary>Deletes everything from the codegroup table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearCodeGroupTable() {
			string command="DELETE FROM codegroup";
			DataCore.NonQ(command);
		}

	}
}
