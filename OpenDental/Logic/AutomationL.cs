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
		
		public static bool Trigger(AutomationTrigger trigger,List<string> procCodes,long patNum,long aptNum=0) {
			return Trigger<object>(trigger,procCodes,patNum,aptNum);
		}

		public static bool Trigger<T>(AutomationTrigger trigger,List<string> procCodes,long patNum,long aptNum=0,T triggerObj=default(T)) {
			Action<string> onShowMsg=(msg) => {//msg is pre-translated
				MsgBox.Show(msg);
			};
			Func<string,string,bool> onYesNoMsgPrompt=(msg,caption) => {//msg is pre-translated
				return MessageBox.Show(msg,caption,MessageBoxButtons.YesNo) == DialogResult.Yes;
			};
			Action<Commlog> onShowCommlog=(commLog) => {
				using(FormCommItem commItemView = new FormCommItem(commLog)) {
					commItemView.ShowDialog();
				}
			};
			Action<Sheet> onShowSheetFillEdit=(sheet) => {
				using(FormSheetFillEdit FormSF=new FormSheetFillEdit(sheet)) {
					FormSF.ShowDialog();
				}
			};
			Func<List<Procedure>,Image> funcCreateToothChartImage=(listProcs) => {
				return SheetPrinting.GetToothChartHelper(patNum,false,listProceduresFilteredOverride:listProcs);
			};
			return Automations.Trigger(
				trigger,procCodes,patNum,
				FormOpenDental.DicBlockedAutomations,onShowMsg,onYesNoMsgPrompt,
				onShowCommlog,onShowSheetFillEdit,funcCreateToothChartImage,aptNum,triggerObj
			);
		}

	}
}
