using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;

namespace TestCanada {
	public class ProviderTC {
		public static string SetInitialProviders() {
			//Dr. A---------------------------------
			Provider priProv=CreateProvider("A.","Dentist","530123401","1234","DocA");
			//Dr. B---------------------------------
			CreateProvider("B.","Dentist","035678900","1234","DocB");
			//The billing provider for both is Dr. A.
			Prefs.UpdateLong(PrefName.PracticeDefaultProv,priProv.ProvNum);
			Prefs.UpdateLong(PrefName.InsBillingProv,priProv.ProvNum);
			//We create a fake test address for the practice, so that forms looks correct when printed, even though no such test address is provided by CDANet.
			Prefs.UpdateString(PrefName.PracticeAddress,"123 Test Ave");
			Prefs.UpdateString(PrefName.PracticeAddress2,"Suite 100");
			Prefs.UpdateString(PrefName.PracticeCity,"East Westchester");
			Prefs.UpdateString(PrefName.PracticeST,"ON");
			Prefs.UpdateString(PrefName.PracticeZip,"M7F2J9");
			Prefs.UpdateString(PrefName.PracticePhone,"123-456-7890");
			Prefs.RefreshCache();
			return "Dentist objects set.\r\n";
		}

		private static Provider CreateProvider(string fName,string lName,string npi,string officeNum,string abbr) {
			Provider prov=null;
			int maxItemOrder=0;
			for(int i=0;i<ProviderC.ListLong.Count;i++) {
				if(ProviderC.ListLong[i].NationalProvID=="" || ProviderC.ListLong[i].NationalProvID==npi) {
					prov=ProviderC.ListLong[i];
				}
				if(ProviderC.ListLong[i].ItemOrder>maxItemOrder) {
					maxItemOrder=ProviderC.ListLong[i].ItemOrder;
				}
			}
			bool updateProv=(prov!=null);
			if(prov==null) {
				prov=new Provider();
			}
			prov.IsHidden=false;
			prov.IsCDAnet=true;
			prov.FName=fName;
			prov.LName=lName;
			prov.NationalProvID=npi;
			prov.CanadianOfficeNum=officeNum;
			prov.Abbr=abbr;
			prov.FeeSched=53;
			prov.ItemOrder=maxItemOrder+1;
			if(updateProv) {
				Providers.Update(prov);
			}
			else {
				Providers.Insert(prov);
			}
			Providers.RefreshCache();
			return prov;
		}


	}
}
