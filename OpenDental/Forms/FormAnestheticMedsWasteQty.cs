using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental
{
	public partial class FormAnestheticMedsWasteQty : Form{

		public FormAnestheticMedsWasteQty()
		{
			InitializeComponent();
			Lan.F(this);
		}

		private void FormAnestheticMedsWasteQty_Load(object sender, EventArgs e)
		{

		}

		private void textDate_TextChanged(object sender, EventArgs e)
		{

		}

		private void textAnesthMed_TextChanged(object sender, EventArgs e)
		{

		}

		private void textDate_TextChanged_1(object sender, EventArgs e)
		{

		}

		private void butCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void butClose_Click(object sender, EventArgs e)
		{

			DialogResult = DialogResult.OK;
				
		}

		private void groupBox1_Enter(object sender, EventArgs e)
		{

		}


	}
}
