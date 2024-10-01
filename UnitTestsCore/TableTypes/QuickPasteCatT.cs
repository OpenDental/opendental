using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;
using System.Linq;

namespace UnitTestsCore {
	public class QuickPasteCatT {

		///<summary>Creates and inserts a quickpastecat.</summary>
		///<param name="defaultForTypes">Comma-separated values of type Enum.QuickPasteType</param>
		public static QuickPasteCat CreateQuickPasteCat(string description,int itemOrder,string defaultForTypes) {
			QuickPasteCat quickPasteCat=new QuickPasteCat {
				Description=description,
				ItemOrder=itemOrder,
				DefaultForTypes=defaultForTypes
			};
			QuickPasteCats.Insert(quickPasteCat);
			return quickPasteCat;
		}

		///<summary>Clears the quickpastecat table. Does not truncate as to not let the PKs repeat.</summary>
		public static void ClearQuickPasteCatTable() {
			string command="DELETE FROM quickpastecat";
			DataCore.NonQ(command);
			QuickPasteCats.RefreshCache();
		}

	}
}
