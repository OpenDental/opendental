using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class ProcButtonItemT {
		public static void ClearProcButtonItemTable() {
			string command="DELETE FROM procbuttonitem";
			DataCore.NonQ(command);
			ProcButtonItems.RefreshCache();
		}

		public static ProcButtonItem CreateProcButtonItem(long codeNum,long procButtonNum,int itemOrder=0,bool isAutoCode=false) {
			ProcButtonItem procButtonItem=new ProcButtonItem {
				CodeNum=isAutoCode?0:codeNum,
				AutoCodeNum=isAutoCode?codeNum:0,
				ItemOrder=itemOrder,
				ProcButtonNum=procButtonNum,
			};
			long procButtonItemNum=ProcButtonItems.Insert(procButtonItem);
			procButtonItem.ProcButtonItemNum=procButtonItemNum;
			ProcButtonItems.Update(procButtonItem);
			ProcButtonItems.RefreshCache();
			return procButtonItem;
		}
	}
}
