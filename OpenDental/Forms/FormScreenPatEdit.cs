using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>Was never finished/implemented.  It's a mess.</summary>
	public partial class FormScreenPatEdit:FormODBase {
		//public ScreenPat ScreenPatCur;
		//private Patient PatCur;
		//public ScreenGroup ScreenGroupCur;
		//private SheetDef ExamSheetDefCur;
		//public bool IsNew;

		public FormScreenPatEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormScreenPatEdit_Load(object sender,EventArgs e) {
			/*
			if(IsNew) {
				ScreenPatCur.SheetNum=PrefC.GetLong(PrefName.PublicHealthScreeningSheet);
			}
			PatCur=Patients.GetPat(ScreenPatCur.PatNum);
			if(PatCur!=null) {
				textPatient.Text=PatCur.GetNameLF();
			}
			if(ScreenGroupCur!=null) {
				textScreenGroup.Text=ScreenGroupCur.Description;
			}
			ExamSheetDefCur=SheetDefs.GetSheetDef(ScreenPatCur.SheetNum);
			if(ExamSheetDefCur!=null) {
				textSheet.Text=ExamSheetDefCur.Description;
			}*/
		}

		/*private void butPatSelect_Click(object sender,EventArgs e) {
			FormPatientSelect FormPS=new FormPatientSelect();
			FormPS.ShowDialog();
			if(FormPS.DialogResult!=DialogResult.OK) {
				return;
			}
			PatCur=Patients.GetPat(FormPS.SelectedPatNum);
			ScreenPatCur.PatNum=PatCur.PatNum;
			textPatient.Text=PatCur.GetNameLF();
		}*/

		private void butOK_Click(object sender,EventArgs e) {
			//if(IsNew) {
			//	ScreenPats.Insert(ScreenPatCur);
			//}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}