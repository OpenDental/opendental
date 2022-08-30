using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEhrVitalsignEdit:FormODBase {
		public Vitalsign VitalsignCur;
		private Patient patCur;

		public FormEhrVitalsignEdit() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormVitalsignEdit_Load(object sender,EventArgs e) {
			patCur=Patients.GetPat(VitalsignCur.PatNum);
			if(patCur.Age>0 && patCur.Age<17) {//child
				groupBox1.Text="";
				checkFollowup.Visible=false;
				checkIneligible.Text="BMI measurement was not taken because patient is pregnant.";
			}
			else {
				checkNutrition.Visible=false;
				checkActivity.Visible=false;
			}
			textDateTaken.Text=VitalsignCur.DateTaken.ToShortDateString();
			textDocumentation.Text=VitalsignCur.Documentation;
			textHeight.Text=VitalsignCur.Height.ToString();
			textWeight.Text=VitalsignCur.Weight.ToString();
			CalcBMI();
			checkFollowup.Checked=VitalsignCur.HasFollowupPlan;
			checkNutrition.Checked=VitalsignCur.ChildGotNutrition;
			checkActivity.Checked=VitalsignCur.ChildGotPhysCouns;
			checkIneligible.Checked=VitalsignCur.IsIneligible;
			textBPd.Text=VitalsignCur.BpDiastolic.ToString();
			textBPs.Text=VitalsignCur.BpSystolic.ToString();
		}

		private void textWeight_TextChanged(object sender,EventArgs e) {
			CalcBMI();
		}

		private void textHeight_TextChanged(object sender,EventArgs e) {
			CalcBMI();
		}

		private void CalcBMI() {
			//BMI = (lbs*703)/(in^2)
			float height;
			float weight;
			try{
				height = float.Parse(textHeight.Text);
				weight = float.Parse(textWeight.Text);
			}
			catch{
				return;
			}
			if(height==0) {
				return;
			}
			if(weight==0) {
				return;
			}
			float bmi=Vitalsigns.CalcBMI(weight, height);// ((float)(weight*703)/(height*height));
			textBMI.Text=bmi.ToString("n1");
			return;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(VitalsignCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(MessageBox.Show("Delete?","",MessageBoxButtons.OKCancel)!=DialogResult.OK){
				return;
			}
			Vitalsigns.Delete(VitalsignCur.VitalsignNum);
			DialogResult=DialogResult.Cancel;
		}

		private void butOK_Click(object sender,EventArgs e) {
			//validate
			DateTime date;
			if(textDateTaken.Text=="") {
				MessageBox.Show("Please enter a date.");
				return;
			}
			try {
				date=DateTime.Parse(textDateTaken.Text);
			}
			catch {
				MessageBox.Show("Please fix date first.");
				return;
			}
			//validate height
			float height=0;
			try {
				if(textHeight.Text!="") {
					height = float.Parse(textHeight.Text);
				}
			}
			catch {
				MessageBox.Show("Please fix height first.");
				return;
			}
			//validate weight
			float weight=0;
			try {
				if(textWeight.Text!="") {
					weight = float.Parse(textWeight.Text);
				}
			}
			catch {
				MessageBox.Show("Please fix weight first.");
				return;
			}
			//validate bp
			int BPsys=0;
			int BPdia=0;
			try {
				if(textBPs.Text!="") {
					BPsys = int.Parse(textBPs.Text);
				}
				if(textBPd.Text!="") {
					BPdia = int.Parse(textBPd.Text);
				}
			}
			catch {
				MessageBox.Show("Please fix BP first.");
				return;
			}
			if(checkFollowup.Checked || checkIneligible.Checked) {
				if(textDocumentation.Text=="") {
					MessageBox.Show("Documentation must be entered.");
					return;
				}
			}
			//save------------------------------------------------------------------
			VitalsignCur.DateTaken=date;
			VitalsignCur.Height=height;
			VitalsignCur.Weight=weight;
			VitalsignCur.BpDiastolic=BPdia;
			VitalsignCur.BpSystolic=BPsys;
			VitalsignCur.HasFollowupPlan=checkFollowup.Checked;
			VitalsignCur.ChildGotNutrition=checkNutrition.Checked;
			VitalsignCur.ChildGotPhysCouns=checkActivity.Checked;
			VitalsignCur.IsIneligible=checkIneligible.Checked;
			VitalsignCur.Documentation=textDocumentation.Text;
			if(VitalsignCur.IsNew) {
			  Vitalsigns.Insert(VitalsignCur);
			}
			else {
				Vitalsigns.Update(VitalsignCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

	

	


	}
}
