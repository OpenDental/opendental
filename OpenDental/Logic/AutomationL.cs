using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental {
	public class AutomationL {
		///<summary>ProcCodes will be null unless trigger is CompleteProcedure or ScheduledProcedure.
		///This routine will generally fail silently.  Will return true if a trigger happened.</summary>
		
		public static bool Trigger(EnumAutomationTrigger automationTrigger,List<string> listProcCodes,long patNum,long aptNum=0) {
			return Trigger<object>(automationTrigger,listProcCodes,patNum,aptNum);
		}

		public static bool Trigger<T>(EnumAutomationTrigger automationTrigger,List<string> listProcCodes,long patNum,long aptNum=0,T triggerObj=default(T)) {
			Action<string> actionShowMsg=(msg) => {//msg is pre-translated
				MsgBox.Show(msg);
			};
			Func<string,string,bool> funcYesNoMsgPrompt=(msg,caption) => {//msg is pre-translated
				return MessageBox.Show(msg,caption,MessageBoxButtons.YesNo) == DialogResult.Yes;
			};
			Action<Commlog> actionShowCommlog=(commLog) => {
				using(FormCommItem formCommItem = new FormCommItem(commLog)) {
					formCommItem.ShowDialog();
				}
			};
			Action<Sheet> actionShowSheetFillEdit=(sheet) => {
				using(FormSheetFillEdit formSheetFillEdit=new FormSheetFillEdit(sheet)) {
					formSheetFillEdit.ShowDialog();
				}
			};
			Func<List<Procedure>,Image> funcCreateToothChartImage=(listProcs) => {
				return SheetPrinting.GetToothChartHelper(patNum,false,listProceduresFilteredOverride:listProcs);
			};
			return Automations.Trigger(
				automationTrigger,listProcCodes,patNum,
				FormOpenDental.DicBlockedAutomations,actionShowMsg,funcYesNoMsgPrompt,
				actionShowCommlog,actionShowSheetFillEdit,funcCreateToothChartImage,aptNum,triggerObj
			);
		}

	}
}
