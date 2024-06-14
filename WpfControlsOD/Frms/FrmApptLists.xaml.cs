using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	/// <summary></summary>
	public partial class FrmApptLists:FrmODBase {
	///<summary>After this window closes, if dialog result is OK, this will contain which list was selected.</summary>
	public ApptListSelection ApptListSelectionResult;

		///<summary></summary>
		public FrmApptLists()
		{
			InitializeComponent();
			Load+=FrmApptLists_Load;
		}

		private void FrmApptLists_Load(object sender, EventArgs e) {
			Lang.F(this);
			Plugins.HookAddCode(this,"FormApptLists.Load_start");
		}

		private void butRecall_Click(object sender, System.EventArgs e) {
			ApptListSelectionResult=ApptListSelection.Recall;
			IsDialogOK=true;
		}

		private void butConfirm_Click(object sender, System.EventArgs e) {
			ApptListSelectionResult=ApptListSelection.Confirm;
			IsDialogOK=true;
		}

		private void butPlanned_Click(object sender, System.EventArgs e) {
			ApptListSelectionResult=ApptListSelection.Planned;
			IsDialogOK=true;
		}

		private void butUnsched_Click(object sender, System.EventArgs e) {
			ApptListSelectionResult=ApptListSelection.Unsched;
			IsDialogOK=true;
		}

		private void butASAP_Click(object sender,EventArgs e) {
			ApptListSelectionResult=ApptListSelection.ASAP;
			IsDialogOK=true;
		}

		private void butRadOrders_Click(object sender,EventArgs e) {
			ApptListSelectionResult=ApptListSelection.Radiology;
			IsDialogOK=true;
		}

		private void butInsVerify_Click(object sender,EventArgs e) {
			ApptListSelectionResult=ApptListSelection.InsVerify;
			IsDialogOK=true;
		}

	}

	///<summary>Used in FormApptLists as the selection result.</summary>
	public enum ApptListSelection{
		///<summary></summary>
		Recall,
		///<summary></summary>
		Confirm,
		///<summary></summary>
		Planned,
		///<summary></summary>
		Unsched,
		///<summary></summary>
		ASAP,
		///<summary></summary>
		Radiology,
		///<summary></summary>
		InsVerify
	}

}