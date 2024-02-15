using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class CovCatT {

		///<summary>Returns covcat. defaultPercent > 100 sets to 100. Refreshes covcat cache.</summary>
		public static CovCat CreateCovCat(string description,int defaultPercent=-1,bool isHidden=false,EbenefitCategory ebenefitCat=EbenefitCategory.None) {
			CovCat covCat=new CovCat();
			covCat.Description=description;
			covCat.DefaultPercent=defaultPercent;
			if(defaultPercent>100) { //Only -1 through 100 are valid defaultPercent.
				covCat.DefaultPercent=100;
			}
			covCat.CovOrder=CovCats.GetDeepCopy().Count;
			covCat.IsHidden=isHidden;
			covCat.EbenefitCat=ebenefitCat;
			CovCats.Insert(covCat);
			CovCats.RefreshCache();
			return covCat;
		}

		///<summary>Deletes everything from the covcat table where PK > 14 and refreshes cache. CovCatNum 1-14 are included in the dump file.</summary>
		public static void ClearCovCatTable() {
			string command="DELETE FROM covcat WHERE CovCatNum > 14";
			DataCore.NonQ(command);
			CovCats.RefreshCache();
		}
	}


}
