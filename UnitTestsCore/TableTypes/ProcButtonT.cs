using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class ProcButtonT { 
		public static void ClearProcButtonTable() {
			string command="DELETE FROM procbutton";
			DataCore.NonQ(command);
			ProcButtons.RefreshCache();
		}

		public static ProcButton CreateProcButton(long defNum,string desc,int itemOrder=0,bool createProcButtonItem=true,long codeNum=0,bool isAutoCode=false) {
			ProcButton procButton=new ProcButton {
				Category=defNum,
				Description=desc,
				ItemOrder=itemOrder,
			};
			long procButtonNum=ProcButtons.Insert(procButton);
			procButton.ProcButtonNum=procButtonNum;
			ProcButtons.Update(procButton);
			if(createProcButtonItem && codeNum!=0) {
				ProcButtonItemT.CreateProcButtonItem(codeNum,procButton.ProcButtonNum,itemOrder,isAutoCode);
			}
			ProcButtons.RefreshCache();
			return procButton;
		}
	}
}
