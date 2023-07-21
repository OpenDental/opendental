using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class AutoCodeItemT {
		public static AutoCodeItem CreateAutoCodeItem(long autoCodeNum,long procCodeNum,List<AutoCodeCond> listAutoCodeConds=null) {
			if(listAutoCodeConds==null) {
				listAutoCodeConds=new List<AutoCodeCond>();
			}
			AutoCodeItem autoCodeItem=new AutoCodeItem {
				AutoCodeNum=autoCodeNum,
				CodeNum=procCodeNum,
				ListConditions=listAutoCodeConds,
			};
			autoCodeItem.AutoCodeItemNum=AutoCodeItems.Insert(autoCodeItem);
			AutoCodeItems.RefreshCache();
			return autoCodeItem;
		}
	}
}
