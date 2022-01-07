using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormTrojanCollectSetup : FormODBase {
		private List<Def> _listBillingTypeDefs;
		private Program _progCur;

		///<summary></summary>
		public FormTrojanCollectSetup()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormTrojanCollectSetup_Load(object sender,EventArgs e) {
			if(ODBuild.IsWeb()) {
				MsgBox.Show(this,"This program is not available in web mode.");
				Close();
				return;
			}
			_progCur=Programs.GetCur(ProgramName.TrojanExpressCollect);
			textExportFolder.Text=ProgramProperties.GetPropVal(_progCur.ProgramNum,"FolderPath");
			long billtype=PIn.Long(ProgramProperties.GetPropVal(_progCur.ProgramNum,"BillingType"));
			_listBillingTypeDefs=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			comboBillType.Items.AddRange(_listBillingTypeDefs.Select(x => x.ItemName).ToArray());
			comboBillType.SelectedIndex=Math.Max(_listBillingTypeDefs.FindIndex(x => x.DefNum==billtype),0);
			textPassword.Text=CDT.Class1.TryDecrypt(ProgramProperties.GetPropVal(_progCur.ProgramNum,"Password"));
			checkEnabled.Checked=_progCur.Enabled;
		}

		private void ButBrowse_Click(object sender,EventArgs e) {
			string selectedPath=textExportFolder.Text;
			if(string.IsNullOrEmpty(selectedPath) || !Directory.Exists(selectedPath)) {
				selectedPath="C:\\";
			}
			using(FolderBrowserDialog fb=new FolderBrowserDialog() { SelectedPath=selectedPath,Description=Lan.g(this,"Select Export Folder Location") }) {
				try {
					if(fb.ShowDialog()==DialogResult.OK) {
						textExportFolder.Text=fb.SelectedPath;
					}
				}
				catch(Exception ex) {
					ex.DoNothing();
					MessageBox.Show(Lan.g(this,"There was an error showing the Browse window.")+"\r\n"
						+Lan.g(this,"Try running as an Administrator or manually typing in a path."));
					return;
				}
			}
		}

		private bool Validation() {
			if(!checkEnabled.Checked) {
				return true;//no need to check for valid fields if program link is disabled
			}
			else if(!Programs.IsEnabledByHq(ProgramName.TrojanExpressCollect,out string err)) {
				MessageBox.Show(err);
				return false;
			}
			if(!Directory.Exists(textExportFolder.Text)) {
				MsgBox.Show(this,"Export folder does not exist.");
				return false;
			}
			if(comboBillType.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a billing type.");
				return false;
			}
			if(!Regex.IsMatch(textPassword.Text,@"^[A-Z]{2}\d{4}$")) {
				MsgBox.Show(this,"Password is not in correct format. Must be like this: AB1234");
				return false;
			}
			return true;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!Validation()) {
				return;
			}
			long billtype=-1;
			if(comboBillType.SelectedIndex>-1) {
				billtype=_listBillingTypeDefs[comboBillType.SelectedIndex].DefNum;
			}
			bool hasChanges=false;
			if(_progCur.Enabled!=checkEnabled.Checked) {
				_progCur.Enabled=checkEnabled.Checked;
				Programs.Update(_progCur);
				hasChanges=true;
			}
			string password=CDT.Class1.TryEncrypt(textPassword.Text);
			if(hasChanges
				| ProgramProperties.SetProperty(_progCur.ProgramNum,"FolderPath",textExportFolder.Text)>0
				| ProgramProperties.SetProperty(_progCur.ProgramNum,"BillingType",billtype.ToString())>0
				| ProgramProperties.SetProperty(_progCur.ProgramNum,"Password",password)>0)
			{
				DataValid.SetInvalid(InvalidType.Programs);
			}
			DialogResult=DialogResult.OK;
		}

	}
}





















