using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	/// <summary></summary>
	public partial class FormHL7MsgEdit:FormODBase {
		public HL7Msg MsgCur;

		///<summary></summary>
		public FormHL7MsgEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormHL7DefSegmentEdit_Load(object sender,EventArgs e) {
			textHL7MsgNum.Text=MsgCur.HL7MsgNum.ToString();
			textDateTStamp.Text=MsgCur.DateTStamp.ToString();
			if(MsgCur.PatNum>0) {
				textPatient.Text=Patients.GetLim(MsgCur.PatNum).GetNameLF();
			}
			if(MsgCur.AptNum>0) {
				textAptNum.Text=MsgCur.AptNum.ToString();
			}
			textHL7Status.Text=MsgCur.HL7Status.ToString();
			textMsgTxt.Text=MsgCur.MsgText;
			textNote.Text=MsgCur.Note;
		}

		private void butOK_Click(object sender,EventArgs e) {
			MsgCur.Note=textNote.Text;
			HL7Msgs.Update(MsgCur);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}