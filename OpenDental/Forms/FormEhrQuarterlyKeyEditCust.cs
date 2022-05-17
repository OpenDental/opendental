using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEhrQuarterlyKeyEditCust:FormODBase {
		public EhrQuarterlyKey KeyCur;

		public FormEhrQuarterlyKeyEditCust() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEhrQuarterlyKeyEditCust_Load(object sender,EventArgs e) {
			textYear.Text=KeyCur.YearValue.ToString();
			textQuarter.Text=KeyCur.QuarterValue.ToString();
			textPracticeTitle.Text=KeyCur.PracticeName;
			textEhrKey.Text=KeyCur.KeyValue;
			textNotes.Text=KeyCur.Notes;
		}

		private void butGenerate_Click(object sender,EventArgs e) {
			if(textYear.Text==""){
				MessageBox.Show("Please enter a year.");
				return;
			}
			if(textQuarter.Text==""){
				MessageBox.Show("Please enter a quarter.");
				return;
			}
			if(textPracticeTitle.Text=="") {
				MessageBox.Show("Please enter a practice title.");
				return;
			}
			if(!textYear.IsValid() || !textQuarter.IsValid()) {
				MessageBox.Show("Please fix errors first.");
				return;
			}
			//Path for testing:
			//@"E:\My Documents\Shared Projects Subversion\EhrProvKeyGenerator\EhrProvKeyGenerator\bin\Debug\EhrProvKeyGenerator.exe"
			string progPath=PrefC.GetString(PrefName.EhrProvKeyGeneratorPath);
			ProcessStartInfo startInfo=new ProcessStartInfo(progPath);
			startInfo.Arguments="Q \""+textYear.Text+"\" \""+textQuarter.Text+"\" \""+textPracticeTitle.Text+"\"";
			startInfo.UseShellExecute=false;
			startInfo.RedirectStandardOutput=true;
			Process process=Process.Start(startInfo);
			string result=process.StandardOutput.ReadToEnd();
			result=result.Trim();//remove \r\n from the end
			textEhrKey.Text=result;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textYear.Text==""){
				MessageBox.Show("Please enter a year.");
				return;
			}
			if(textQuarter.Text==""){
				MessageBox.Show("Please enter a quarter.");
				return;
			}
			if(textPracticeTitle.Text=="") {
				MessageBox.Show("Please enter a practice title.");
				return;
			}
			if(!textYear.IsValid() || !textQuarter.IsValid()) {
				MessageBox.Show("Please fix errors first.");
				return;
			}
			int quarterValue=PIn.Int(textQuarter.Text);
			int yearValue=PIn.Int(textYear.Text);
			int monthOfQuarter=1;
			if(quarterValue==2){
				monthOfQuarter=4;
			}
			if(quarterValue==3){
				monthOfQuarter=7;
			}
			if(quarterValue==4){
				monthOfQuarter=10;
			}
			DateTime firstDayOfQuarter=new DateTime(2000+yearValue,monthOfQuarter,1);
			DateTime earliestReleaseDate=firstDayOfQuarter.AddMonths(-1);
			if(DateTime.Today<earliestReleaseDate){
				MessageBox.Show("Quarterly keys cannot be released more than one month in advance.");
				return;
			}
			if(!FormEHR.QuarterlyKeyIsValid(textYear.Text,textQuarter.Text,textPracticeTitle.Text,textEhrKey.Text)) {
				MsgBox.Show(this,"Invalid quarterly key");
				return;
			}
			KeyCur.YearValue=PIn.Int(textYear.Text);
			KeyCur.QuarterValue=PIn.Int(textQuarter.Text);
			KeyCur.PracticeName=textPracticeTitle.Text;
			KeyCur.KeyValue=textEhrKey.Text;
			KeyCur.Notes=textNotes.Text;
			if(KeyCur.IsNew) {
				EhrQuarterlyKeys.Insert(KeyCur);
			}
			else {
				EhrQuarterlyKeys.Update(KeyCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(KeyCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")){
				return;
			}
			EhrQuarterlyKeys.Delete(KeyCur.EhrQuarterlyKeyNum);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		




	}
}