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
		public Reactivation ReactivationCur;

		///<summary></summary>
		public FormReactivationEdit(long patNum) {
			Reactivation reactivation = new Reactivation();
			reactivation.PatNum=patNum;
			reactivation.IsNew=true;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			ReactivationCur=reactivation;
		}

		///<summary></summary>
		public FormReactivationEdit(Reactivation reactivation) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			ReactivationCur=reactivation;
		}

		private void FormReactivationEdit_Load(object sender, System.EventArgs e) {
			textPatName.Text=Patients.GetNameFL(ReactivationCur.PatNum);
			DateTime dateTimeLastContacted=Reactivations.GetDateLastContacted(ReactivationCur.PatNum);
			textDateLastContacted.Text=dateTimeLastContacted==DateTime.MinValue?"":dateTimeLastContacted.ToString();
			comboStatus.Items.AddDefNone();
			comboStatus.Items.AddDefs(Defs.GetDefsForCategory(DefCat.RecallUnschedStatus,isShort:true));
			comboStatus.SetSelectedDefNum(ReactivationCur.ReactivationStatus);
			checkBoxDNC.Checked=ReactivationCur.DoNotContact;
			textNote.Text=ReactivationCur.ReactivationNote;
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Delete this Reactivation?")) {
				return;
			}
			if(!ReactivationCur.IsNew) {
				Reactivations.Delete(ReactivationCur.ReactivationNum);
			}
			ReactivationCur=null;
			DialogResult=DialogResult.Abort;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			Def defSelectedStatus=comboStatus.GetSelected<Def>();//Null when 'None' is selected.
			bool didStatusChange=(ReactivationCur.ReactivationStatus!=(defSelectedStatus?.DefNum??0));
			ReactivationCur.ReactivationStatus=defSelectedStatus==null?0:defSelectedStatus.DefNum;
			ReactivationCur.ReactivationNote=textNote.Text;
			ReactivationCur.DoNotContact=checkBoxDNC.Checked;
			if(ReactivationCur.IsNew) {
				Reactivations.Insert(ReactivationCur);
			}
			else {
				Reactivations.Update(ReactivationCur);
			}
			if(didStatusChange){
				//If the reactivation status is changing, we need to create a reactivation commlog. Rather than repeating
				//this logic, return a different DialogResult and use that as an indicator to the parent window (currently only
				//FormRecallList) that it should create the commlog and do any other required logic.
				DialogResult=DialogResult.Yes;
				return;
			}
			//Status didn't change, we don't need to do anything special
			DialogResult=DialogResult.OK;
		
		}

	}
}