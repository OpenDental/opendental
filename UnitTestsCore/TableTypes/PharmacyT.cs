using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	///<summary>Has methods for Pharmacy.</summary>
	public class PharmacyT {
		///<summary>Deletes everything from Pharmacy.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearPharmacies() {
			string command="DELETE FROM pharmacy WHERE PharmacyNum > 0";
			DataCore.NonQ(command);
		}

		public static void CreatePharmacy(string name,string addr,string city,string state,string zip,string phone) {
			Pharmacies.Insert(new Pharmacy() {
				StoreName=name,
				Address=addr,
				City=city,
				State=state,
				Zip=zip,
				Phone=phone,
			});
		}

		public static void CreatePharmacies() {
			CreatePharmacy("Walgreens Commercial","4380 Commercial St SE","Salem","OR","97302","(503) 399-8148");
			CreatePharmacy("Walgreens Lancaster","124 Lancaster Dr SE","Salem","OR","97317","(503) 428-5004");
			CreatePharmacy("Rite Aid Lancaster","681 Lancaster Dr NE","Salem","OR","97301","(503) 585-7616");
			CreatePharmacy("Rite Aid Liberty","435 Liberty St NE","Salem","OR","97301","(503) 362-3654");
			CreatePharmacy("Costco","1010 Hawthorne Ave SE","Salem","OR","97301","(503) 371-8739");
			CreatePharmacy("Safeway Commercial","5660 Commercial St SE","Salem","OR","97306","(503) 364-1520");
			CreatePharmacy("Walmart Turner Rd","1940 Turnder Rd SE","Salem","OR","97302","(503) 391-0586");
		}
	}
}
