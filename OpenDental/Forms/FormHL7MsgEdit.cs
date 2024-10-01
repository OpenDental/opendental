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
		public HL7Msg HL7MsgCur;

		///<summary></summary>
		public FormHL7MsgEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormHL7DefSegmentEdit_Load(object sender,EventArgs e) {
			textHL7MsgNum.Text=HL7MsgCur.HL7MsgNum.ToString();
			textDateTStamp.Text=HL7MsgCur.DateTStamp.ToString();
			if(HL7MsgCur.PatNum>0) {
				textPatient.Text=Patients.GetLim(HL7MsgCur.PatNum).GetNameLF();
			}
			if(HL7MsgCur.AptNum>0) {
				textAptNum.Text=HL7MsgCur.AptNum.ToString();
			}
			textHL7Status.Text=HL7MsgCur.HL7Status.ToString();
			textMsgTxt.Text=HL7MsgCur.MsgText;
			textNote.Text=HL7MsgCur.Note;
		}

		private void butSave_Click(object sender,EventArgs e) {
			HL7MsgCur.Note=textNote.Text;
			HL7Msgs.Update(HL7MsgCur);
			DialogResult=DialogResult.OK;
		}

	}
}