using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class AlertCategoryT {
		///<summary>Creates an AlertCategory and AlertCategoryLinks between the new AlertCategory and all AlertTypes from the passed in list.
		///Will create an AlertCategory for All AlerTypes if null is passed or no list is provided. Refreshes AlertCategory and AlertCategoryLink caches.</summary>
		public static AlertCategory CreateAlertCategory(List<AlertType> listAlertTypes=null,string description="TestAlertCategoryDescription",bool isHQCategory=false,string internalName="TestAlertCategoryInternalName") {
			AlertCategory alertCategory=new AlertCategory() {
				IsHQCategory=isHQCategory,
				InternalName=internalName,
				Description=description
			};
			long alertCategoryNum=AlertCategories.Insert(alertCategory);
			if(listAlertTypes==null) {
				listAlertTypes=Enum.GetValues(typeof(AlertType)).Cast<AlertType>().ToList();
			}
			for(int i=0;i<listAlertTypes.Count;i++) {
				AlertCategoryLinks.Insert(new AlertCategoryLink(alertCategoryNum,listAlertTypes[i]));
			}
			RefreshCache();
			return AlertCategories.GetOne(alertCategoryNum);
		}

		///<summary>Clears AlertCategory and AlertCategoryLink tables.</summary>

		public static void ClearAlertCategoryTable() {
			string command="DELETE FROM alertcategory WHERE AlertCategoryNum > 0";
			DataCore.NonQ(command);
			command="DELETE FROM alertcategorylink WHERE AlertCategoryLinkNum > 0";
			DataCore.NonQ(command);
			RefreshCache();
		}

		///<summary>Refreshes the caches for AlertCategory and AlertCategoryLink tables.</summary>

		public static void RefreshCache() {
			AlertCategories.GetTableFromCache(true);
			AlertCategoryLinks.GetTableFromCache(true);
		}
	}
}
