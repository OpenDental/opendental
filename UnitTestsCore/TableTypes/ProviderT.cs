using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeBase;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class ProviderT {

		///<summary>Inserts the new provider, refreshes the cache and then returns ProvNum</summary>
		public static long CreateProvider(string abbr,string fName="",string lName="",long feeSchedNum=0,bool isSecondary=false,bool isHidden=false,
			string ssn="",bool isUsingTIN=false,string nationalProvID="")
		{
			Provider prov=new Provider();
			prov.Abbr=abbr;
			prov.FName=fName;
			prov.LName=lName;
			prov.FeeSched=feeSchedNum;
			prov.IsSecondary=isSecondary;
			prov.IsHidden=isHidden;
			prov.SSN=ssn;
			prov.UsingTIN=isUsingTIN;
			prov.NationalProvID=nationalProvID;
			Providers.Insert(prov);
			Providers.RefreshCache();
			return prov.ProvNum;
		}

		///<summary>Returns the first provnum in provider table. Creates a new provider if no providers are present.</summary>
		public static long GetFirstProvNum() {
			List<Provider> listProvs=Providers.GetAll();
			if(listProvs.IsNullOrEmpty()) {
				return CreateProvider(MethodBase.GetCurrentMethod().Name);
			}
			return listProvs.First().ProvNum;
		}

		///<summary>Updates the provider passed in to the database and then refreshes the cache.</summary>
		public static void Update(Provider provider) {
			Providers.Update(provider);
			Providers.RefreshCache();
		}

		/// <summary>Creates a list of three providers.</summary>
		public static List<Provider> CreateProviderList() {
			List<Provider> listProviders=new List<Provider>(); 
			Provider provOne=new Provider();
			provOne.Abbr="La Flame";
			provOne.FName="Travis";
			provOne.LName="Scott";
			provOne.FeeSched=0;
			provOne.IsSecondary=false;
			provOne.IsHidden=false;
			provOne.Suffix="DMD";
			provOne.SSN="54434343223";
			provOne.UsingTIN=false;
			provOne.NationalProvID="17777";
			Provider provTwo=new Provider();
			provTwo.Abbr="Billy Corgan";
			provTwo.FName="William";
			provTwo.LName="Corgan";
			provTwo.FeeSched=0;
			provTwo.IsSecondary=false;
			provTwo.IsHidden=false;
			provTwo.Suffix="DMD";
			provTwo.SSN="5499002000";
			provTwo.UsingTIN=false;
			provTwo.NationalProvID="189900000";
			Provider provThree=new Provider();
			provThree.Abbr="T-Pain";
			provThree.FName="Faheem";
			provThree.LName="Rajm";
			provThree.FeeSched=0;
			provThree.IsSecondary=false;
			provThree.IsHidden=false;
			provThree.Suffix="DMD";
			provThree.SSN="544780108000";
			provThree.UsingTIN=false;
			provThree.NationalProvID="788999000";
			listProviders.Add(provOne);
			listProviders.Add(provTwo);
			listProviders.Add(provThree);
			return listProviders;
		}

		/// <summary>Creates a list of three providers with some empty fields.</summary>
		public static List<Provider> CreateProviderListEmpties() {
			List<Provider> listProviders=new List<Provider>(); 
			Provider provOne=new Provider();
			provOne.FName="Travis";
			provOne.LName="Scott";
			provOne.FeeSched=0;
			provOne.IsSecondary=false;
			provOne.IsHidden=false;
			provOne.Suffix="DMD";
			provOne.SSN="54434343223";
			provOne.UsingTIN=false;
			provOne.NationalProvID="17777";
			Provider provTwo=new Provider();
			provTwo.FName="William";
			provTwo.LName="Corgan";
			provTwo.FeeSched=0;
			provTwo.IsSecondary=false;
			provTwo.IsHidden=false;
			provTwo.Suffix="DMD";
			provTwo.SSN="5499002000";
			provTwo.UsingTIN=false;
			provTwo.NationalProvID="189900000";
			Provider provThree=new Provider();
			provThree.FName="Faheem";
			provThree.LName="Rajm";
			provThree.FeeSched=0;
			provThree.IsSecondary=false;
			provThree.IsHidden=false;
			provThree.Suffix="DMD";
			provThree.SSN="544780108000";
			provThree.UsingTIN=false;
			provThree.NationalProvID="788999000";
			listProviders.Add(provOne);
			listProviders.Add(provTwo);
			listProviders.Add(provThree);
			return listProviders;
		}

		///<summary>Deletes (almost) everything from the provider table.</summary>
		public static void ClearProviderTable() {
			string command="DELETE FROM provider WHERE ProvNum > 2";
			DataCore.NonQ(command);
		}

	}
}
