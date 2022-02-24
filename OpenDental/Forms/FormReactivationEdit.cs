using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	///<summary>Summary description for FormReactivationEdit.
	///Note that the form returns different dialog results for different "states". DialogResult.Yes indicates that the reactivation status changed
	///and was saved, so the parent form knows to do any additional associated work. DialogResult.Ok indicates that the reactivation was saved, but the
	///status was not changed. DialogResult.Abort indicates the user clicked "Delete" to explicitly remove the reactivation. DialogResult.Cancel
	///indicates the user clicked Cancel or closed normally.</summary>
	public partial class FormReactivationEdit : FormODBase {
		private Reactivation _reactivationCur;

		///<summary></summary>
		public FormReactivationEdit(long patNum) : this(new Reactivation() { PatNum=patNum,IsNew=true }) {
		}

		///<summary></summary>
		public FormReactivationEdit(Reactivation reactivationCur) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_reactivationCur=reactivationCur;
		}

		private void FormReactivationEdit_Load(object sender, System.EventArgs e) {
			textPatName.Text=Patients.GetNameFL(_reactivationCur.PatNum);
			DateTime lastContacted=Reactivations.GetDateLastContacted(_reactivationCur.PatNum);
			textDateLastContacted.Text=lastContacted==DateTime.MinValue?"":lastContacted.ToString();
			comboStatus.Items.AddDefNone();
			comboStatus.Items.AddDefs(Defs.GetDefsForCategory(DefCat.RecallUnschedStatus));
			comboStatus.SetSelectedDefNum(_reactivationCur.ReactivationStatus);
			checkBoxDNC.Checked=_reactivationCur.DoNotContact;
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Delete this Reactivation?")) {
				return;
			}
			if(!_reactivationCur.IsNew) {
				Reactivations.Delete(_reactivationCur.ReactivationNum);
			}
			_reactivationCur=null;
			DialogResult=DialogResult.Abort;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			Def selectedStatus=comboStatus.GetSelected<Def>();//Null when 'None' is selected.
			bool didStatusChange=(_reactivationCur.ReactivationStatus!=(selectedStatus?.DefNum??0));
			_reactivationCur.ReactivationStatus=selectedStatus==null?0:selectedStatus.DefNum;
			_reactivationCur.ReactivationNote=textNote.Text;
			_reactivationCur.DoNotContact=checkBoxDNC.Checked;
			if(_reactivationCur.IsNew) {
				Reactivations.Insert(_reactivationCur);
			}
			else {
				Reactivations.Update(_reactivationCur);
			}
			if(didStatusChange) {
				//If the reactivation status is changing, we need to create a reactivation commlog. Rather than repeating
				//this logic, return a different DialogResult and use that as an indicator to the parent window (currently only
				//FormRecallList) that it should create the commlog and do any other required logic.
				DialogResult=DialogResult.Yes;
			}
			else {
				//Status didn't change, we don't need to do anything special
				DialogResult=DialogResult.OK;
			}
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}





















