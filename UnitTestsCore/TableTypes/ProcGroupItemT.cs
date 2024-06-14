using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class ProcGroupItemT {
		public static ProcGroupItem CreateProcGroupItem(long procNum,long groupProcNum) {
			ProcGroupItem procGroupItem=new ProcGroupItem {
				ProcNum=procNum,
				GroupNum=groupProcNum,
			};
			ProcGroupItems.Insert(procGroupItem);
			return procGroupItem;
		}
	}
}
