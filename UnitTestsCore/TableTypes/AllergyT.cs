using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class AllergyT {
		public static void ClearAllergyTable() {
			string command=$"DELETE FROM allergy WHERE AllergyNum > 0";
			DataCore.NonQ(command);
		}

		public static Allergy CreateAllergy(long patNum,long allergyDefNum,string reaction="",string snomedReaction="",bool statusIsActive=true) {
			Allergy allergy=new Allergy {
				AllergyDefNum=allergyDefNum,
				DateAdverseReaction=DateTime.MinValue,
				DateTStamp=DateTime.Now,
				PatNum=patNum,
				Reaction=reaction,
				SnomedReaction=snomedReaction,
				StatusIsActive=statusIsActive,
			};
			allergy.AllergyNum=Allergies.Insert(allergy);
			Allergies.Update(allergy);
			return allergy;
		}
	}
}
