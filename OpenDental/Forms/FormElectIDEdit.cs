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
		public ElectID ElectIDCur;

		public FormElectIDEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormElectIDEdit_Load(object sender,EventArgs e) {
			textPayerID.Text=ElectIDCur.PayorID;
			textCarrierName.Text=ElectIDCur.CarrierName;
			textComments.Text=ElectIDCur.Comments;
			checkIsMedicaid.Checked=ElectIDCur.IsMedicaid;
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
			ElectIDCur.PayorID=textPayerID.Text;
			ElectIDCur.CarrierName=textCarrierName.Text;
			ElectIDCur.Comments=textComments.Text;
			ElectIDCur.IsMedicaid=checkIsMedicaid.Checked;
			if(ElectIDCur.ElectIDNum==0) {
				ElectIDs.Insert(ElectIDCur);
			}
			else {
				ElectIDs.Update(ElectIDCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}