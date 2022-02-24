using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormElectIDEdit:FormODBase {

		///<summary>Must be set before calling Show() or ShowDialog().</summary>
		public ElectID electIDCur;

		public FormElectIDEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormElectIDEdit_Load(object sender,EventArgs e) {
			textPayerID.Text=electIDCur.PayorID;
			textCarrierName.Text=electIDCur.CarrierName;
			textComments.Text=electIDCur.Comments;
			checkIsMedicaid.Checked=electIDCur.IsMedicaid;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textPayerID.Text=="") {
				MsgBox.Show(this,"Payer ID cannot be blank.");
				return;
			}
			if(textCarrierName.Text=="") {
				MsgBox.Show(this,"Carrier name cannot be blank.");
				return;
			}
			electIDCur.PayorID=textPayerID.Text;
			electIDCur.CarrierName=textCarrierName.Text;
			electIDCur.Comments=textComments.Text;
			electIDCur.IsMedicaid=checkIsMedicaid.Checked;
			if(electIDCur.ElectIDNum==0) {
				ElectIDs.Insert(electIDCur);
			}
			else {
				ElectIDs.Update(electIDCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}