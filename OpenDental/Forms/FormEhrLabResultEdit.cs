using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Drawing.Printing;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormEhrLabResultEdit:FormODBase {
		public bool IsNew;
		public LabResult LabCur;
		
		public FormEhrLabResultEdit() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormLabResultEdit_Load(object sender,EventArgs e) {
			if(LabCur.DateTimeTest.Year>1800) {
				textDateTimeTest.Text=LabCur.DateTimeTest.ToString();
			}
			textTestID.Text=LabCur.TestID;
			textTestName.Text=LabCur.TestName;
			textObsValue.Text=LabCur.ObsValue;
			textObsUnits.Text=LabCur.ObsUnits;
			textObsRange.Text=LabCur.ObsRange;
			for(int i=0;i<Enum.GetNames(typeof(LabAbnormalFlag)).Length;i++) {
				comboAbnormalFlag.Items.Add(Enum.GetNames(typeof(LabAbnormalFlag))[i]);
			}
			comboAbnormalFlag.SelectedIndex=(int)LabCur.AbnormalFlag;

		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(MessageBox.Show("Delete?","Delete?",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
				return;
			}
			LabResults.Delete(LabCur.LabResultNum);
			DialogResult=DialogResult.OK;
		}

		private void butOk_Click(object sender,EventArgs e) {
			if(textDateTimeTest.Text=="") {
				MessageBox.Show("Please input a valid date.");
				return;
			}
			try {
				LabCur.DateTimeTest=DateTime.Parse(textDateTimeTest.Text);
			}
			catch {
				MessageBox.Show("Please input a valid date.");
				return;
			}
			LabCur.TestID=textTestID.Text;
			LabCur.TestName=textTestName.Text;
			LabCur.ObsValue=textObsValue.Text;
			LabCur.ObsUnits=textObsUnits.Text;
			LabCur.ObsRange=textObsRange.Text;
			LabCur.AbnormalFlag=(LabAbnormalFlag)comboAbnormalFlag.SelectedIndex;
			if(IsNew) {
				LabResults.Insert(LabCur);
			}
			else{
				LabResults.Update(LabCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void comboAbnormalFlag_SelectedIndexChanged(object sender,EventArgs e) {

		}

		
	}
}
