using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormInsBenefitNotes : FormODBase {
		///<summary></summary>
		public string BenefitNotes;

		///<summary></summary>
		public FormInsBenefitNotes()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormInsBenefitNotes_Load(object sender, System.EventArgs e) {
			textBenefitNotes.Text=BenefitNotes;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			BenefitNotes=textBenefitNotes.Text;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		


	}
}





















