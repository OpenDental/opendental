using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;
using System.Drawing;

namespace UnitTestsCore {
	public class DefT {

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

		public static void DeleteAllForCategory(DefCat defCat) {
			string command=$"DELETE FROM definition WHERE Category={POut.Int((int)defCat)}";
			DataCore.NonQ(command);
			Defs.RefreshCache();
		}

	}
}
