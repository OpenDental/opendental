using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using OpenDentBusiness.DataAccess;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Text.RegularExpressions;

namespace OpenDental
{
	public partial class FormAnesthMedDelDose : Form{

		public AnestheticMedsGiven Med;
		public int anestheticRecordNum;
		public int anestheticMedNum;

		public FormAnesthMedDelDose(){
			InitializeComponent();
			Lan.F(this);
		}

		private void FormAnesthMedDelDose_Load(object sender, EventArgs e){

			textAnesthMedName.Text = Med.AnesthMedName;
			textDose.Text = Med.QtyGiven; 
			textDoseTimeStamp.Text = Med.DoseTimeStamp;
			textQtyWasted.Text = Med.QtyWasted;
			anestheticMedNum = Convert.ToInt32(Med.AnestheticMedNum);
			anestheticRecordNum = Convert.ToInt32(Med.AnestheticRecordNum);
			
		}

		private void textDate_TextChanged(object sender, EventArgs e){

		}

		private void textAnesthMed_TextChanged(object sender, EventArgs e){

		}

		private void butCancel_Click(object sender, EventArgs e){

			DialogResult = DialogResult.Cancel;
		}

		private void butClose_Click(object sender, EventArgs e){
			//Regular Expressions to validate the format of the entered value in the textAnesthDose and textQtyWasted
			Regex regexDose = new Regex("^\\d{1,4}(\\.\\d{1,2})?$");
			Regex regexQtyWasted = new Regex("^\\d{1,4}(\\.\\d{1,2})?$");
			if (!(regexDose.IsMatch(textDose.Text)) && textDose.Text != "")
			{
				MessageBox.Show(Lan.g(this,"Dose should be xxx.xx format"));
				textQtyWasted.Focus();
			}
			
			else if (!(regexQtyWasted.IsMatch(textQtyWasted.Text)) && textQtyWasted.Text != "")
			{
				MessageBox.Show(Lan.g(this,"Amount wasted should be xxx.xx format"));
				textQtyWasted.Focus();
			}
			else
			{
				double dose = Convert.ToDouble(textDose.Text);
				double amtWasted = Convert.ToDouble(textQtyWasted.Text);
				AnesthMeds.UpdateAMedDose(textAnesthMedName.Text, Convert.ToDouble(textDose.Text), Convert.ToDouble(amtWasted), textDoseTimeStamp.Text, anestheticMedNum, anestheticRecordNum);
				Close();
			}

		}

		private void textDose_TextChanged(object sender, EventArgs e){

		}

		private void butDelAnesthMeds_Click(object sender, EventArgs e){
			//Regular Expressions to validate the format of the entered value in the textAnesthDose and textQtyWasted
			Regex regexDose = new Regex("^\\d{1,4}(\\.\\d{1,2})?$");
			Regex regexQtyWasted = new Regex("^\\d{1,4}(\\.\\d{1,2})?$");
			if (!(regexDose.IsMatch(textDose.Text)) && textDose.Text != "")
			{
				MessageBox.Show(Lan.g(this,"Dose should be xxx.xx format"));
				textQtyWasted.Focus();
			}

			else if (!(regexQtyWasted.IsMatch(textQtyWasted.Text)) && textQtyWasted.Text != "")
			{
				MessageBox.Show(Lan.g(this,"Amount wasted should be xxx.xx format"));
				textQtyWasted.Focus();
			}
			else
			{
				AnesthMeds.DeleteAMedDose(textAnesthMedName.Text, Convert.ToDouble(textDose.Text), Convert.ToDouble(textQtyWasted.Text), textDoseTimeStamp.Text, anestheticRecordNum);

				DialogResult = DialogResult.OK;
			}
		}

		private void groupBoxAnesthMedDelete_Enter(object sender, EventArgs e){

		}




	}
}
