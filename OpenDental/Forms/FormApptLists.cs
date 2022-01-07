using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormApptLists : FormODBase {
    ///<summary>After this window closes, if dialog result is OK, this will contain which list was selected.</summary>
    public ApptListSelection SelectionResult;

		///<summary></summary>
		public FormApptLists()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormApptLists_Load(object sender, System.EventArgs e) {
			Plugins.HookAddCode(this,"FormApptLists.Load_start");
		}

		private void butRecall_Click(object sender, System.EventArgs e) {
			SelectionResult=ApptListSelection.Recall;
			DialogResult=DialogResult.OK;
		}

		private void butConfirm_Click(object sender, System.EventArgs e) {
			SelectionResult=ApptListSelection.Confirm;
			DialogResult=DialogResult.OK;
		}

		private void butPlanned_Click(object sender, System.EventArgs e) {
			SelectionResult=ApptListSelection.Planned;
			DialogResult=DialogResult.OK;
		}

		private void butUnsched_Click(object sender, System.EventArgs e) {
			SelectionResult=ApptListSelection.Unsched;
			DialogResult=DialogResult.OK;
		}

		private void butASAP_Click(object sender,EventArgs e) {
			SelectionResult=ApptListSelection.ASAP;
			DialogResult=DialogResult.OK;
		}

		private void butRadOrders_Click(object sender,EventArgs e) {
			SelectionResult=ApptListSelection.Radiology;
			DialogResult=DialogResult.OK;
		}

		private void butInsVerify_Click(object sender,EventArgs e) {
			SelectionResult=ApptListSelection.InsVerify;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
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





















