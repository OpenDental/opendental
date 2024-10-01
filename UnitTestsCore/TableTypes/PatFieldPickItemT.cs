using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace UnitTestsWeb {
	public class PatFieldPickItemT {

		///<summary>Creates and inserts a PatFieldPickItem for a specific PatFieldDef.</summary>
		public static PatFieldPickItem CreatePatFieldPickItem(long patFieldDefNum,string name,string abbreviation="",bool isHidden=false) {
			PatFieldPickItem patFieldPickItem=new PatFieldPickItem {
				PatFieldDefNum=patFieldDefNum,
				Name=name,
				Abbreviation=abbreviation,
				ItemOrder=PatFieldPickItems.GetCount(),
				IsHidden=isHidden,
			};
			PatFieldPickItems.Insert(patFieldPickItem);
			PatFieldPickItems.RefreshCache();
			return patFieldPickItem;
		}

		///<summary>Deletes everything from the patfieldpickitem table.</summary>
		public static void ClearPatFieldPickItemTable() {
			string command="DELETE FROM patfieldpickitem";
			DataCore.NonQ(command);
		}
	}
}
