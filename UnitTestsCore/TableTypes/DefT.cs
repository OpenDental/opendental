using CodeBase;
using OpenDentBusiness;
using OpenDentBusiness.Crud;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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
			DeleteForCategories(ListTools.FromSingle(defCat));
			Defs.RefreshCache();
		}

		public static void ResetDefs() {
			if(_listDefs.IsNullOrEmpty()) {
				return;
			}
			List<DefCat> listDefCats=_listDefs.Select(x => x.Category).ToList();
			DeleteForCategories(listDefCats);
			foreach(Def def in _listDefs) {
				DefCrud.Insert(def,true);
			}
			Defs.RefreshCache();
			_listDefs=new List<Def>();
		}

		private static void DeleteForCategories(List<DefCat> listDefCats) {
			if(listDefCats.IsNullOrEmpty()) {
				return;
			}
			string strCategories=string.Join(",",listDefCats.Distinct().Select(x => POut.Enum(x)));
			string command="DELETE FROM definition WHERE Category IN ("+strCategories+")";
			DataCore.NonQ(command);
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
