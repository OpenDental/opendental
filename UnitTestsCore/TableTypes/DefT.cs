using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;
using System.Drawing;

namespace UnitTestsCore {
	public class DefT {
		private static List<Def> _listDefs=new List<Def>();

		///<summary></summary>
		public static Def CreateDefinition(DefCat category,string itemName,string itemValue="",Color itemColor=new Color(),bool isHidden=false) {
			Def def=new Def();
			def.Category=category;
			def.ItemColor=itemColor;
			def.ItemName=itemName;
			def.ItemValue=itemValue;
			def.IsHidden=isHidden;
			Defs.Insert(def);
			Defs.RefreshCache();
			return def;
		}

		///<summary>Call ResetDefs to restore all defs once testing is complete.</summary>
		public static void DeleteAllForCategory(DefCat defCat) {
			_listDefs.AddRange(Defs.GetCatList((int)defCat));
			string command=$"DELETE FROM definition WHERE Category={POut.Int((int)defCat)}";
			DataCore.NonQ(command);
			Defs.RefreshCache();
		}

		public static void ResetDefs() {
			foreach(Def def in _listDefs) {
				Defs.Insert(def);
			}
			_listDefs=new List<Def>();
		}

		///<summary>Deleting all for a Category can cause other Unit Tests to fail that rely on the first in a Category 
		///to be returned. This allows us to clean up after the singular generation of a new Def when testing.</summary>
		public static void DeleteOne(long defNum) {
			string command=$"DELETE FROM definition WHERE DefNum={POut.Long((long)defNum)}";
			DataCore.NonQ(command);
			Defs.RefreshCache();
		}
	}
}
