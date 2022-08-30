using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
#if EHRTEST
using EHR;
#endif

namespace OpenDental {
	public partial class FormEhrProvKeyEditCust:FormODBase {
		public EhrProvKey KeyCur;

		public FormEhrProvKeyEditCust() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEhrProvKeyEditCust_Load(object sender,EventArgs e) {
			textLName.Text=KeyCur.LName;
			textFName.Text=KeyCur.FName;
			textCalYear.Text=POut.Long(KeyCur.YearValue);
			textEhrKey.Text=KeyCur.ProvKey;
			textFullTimeEquiv.Text=KeyCur.FullTimeEquiv.ToString();
			textNotes.Text=KeyCur.Notes;
		}

		private void butGenerate_Click(object sender,EventArgs e) {
			if(textLName.Text=="" || textFName.Text=="") {
				MessageBox.Show("Please enter firstname and lastname.");
				return;
			}
			if(!textCalYear.IsValid()) {
				MessageBox.Show("Invalid year, must be two digits.");
				return;
			}
			//Path for testing:
			//@"E:\My Documents\Shared Projects Subversion\EhrProvKeyGenerator\EhrProvKeyGenerator\bin\Debug\EhrProvKeyGenerator.exe"
			string progPath=PrefC.GetString(PrefName.EhrProvKeyGeneratorPath);
			ProcessStartInfo startInfo=new ProcessStartInfo(progPath);
			string args="P \""+textLName.Text.Replace("\"","")+"\" \""+textFName.Text.Replace("\"","")+"\" "+textCalYear.Text.Replace("\"","").Trim();
			startInfo.Arguments=args;
			startInfo.UseShellExecute=false;
			startInfo.RedirectStandardOutput=true;
			Process process=Process.Start(startInfo);
			string result=process.StandardOutput.ReadToEnd();
			result=result.Trim();//remove \r\n from the end
			//process.WaitForExit();
			textEhrKey.Text=result;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(KeyCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
				return;
			}
			EhrProvKeys.Delete(KeyCur.EhrProvKeyNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			//if(textEhrKey.Text=="") {
			//	MessageBox.Show("Key must not be blank");
			//	return;
			//}
			try{
				float fte=float.Parse(textFullTimeEquiv.Text);
				if(fte<=0) {
					MessageBox.Show("FTE must be greater than 0.");
					return;
				}
				if(fte>1) {
					MessageBox.Show("FTE must be 1 or less.");
					return;
				}
			}
			catch{
				//not allowed to be blank. Usually 1.
				MessageBox.Show("Invalid FTE.");
				return;
			}
			if(textEhrKey.Text!="") {
				if(!FormEHR.ProvKeyIsValid(textLName.Text,textFName.Text,PIn.Int(textCalYear.Text),textEhrKey.Text)) {
					MessageBox.Show("Invalid provider key");
					return;
				}
			}
			KeyCur.LName=textLName.Text;
			KeyCur.FName=textFName.Text;
			KeyCur.YearValue=PIn.Int(textCalYear.Text);
			KeyCur.ProvKey=textEhrKey.Text;
			KeyCur.FullTimeEquiv=PIn.Float(textFullTimeEquiv.Text);
			KeyCur.Notes=textNotes.Text;
			if(KeyCur.IsNew) {
				EhrProvKeys.Insert(KeyCur);
			}
			else {
				EhrProvKeys.Update(KeyCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	

		

		

		
	}
}