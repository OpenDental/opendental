using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	/// <summary></summary>
	public partial class FormTrojanCollectSetup : FormODBase {
		private List<Def> _listDefsBillingType;
		private Program _program;

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
			_program=Programs.GetCur(ProgramName.TrojanExpressCollect);
			textExportFolder.Text=ProgramProperties.GetPropVal(_program.ProgramNum,"FolderPath");
			long billtype=PIn.Long(ProgramProperties.GetPropVal(_program.ProgramNum,"BillingType"));
			_listDefsBillingType=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			comboBillType.Items.AddList(_listDefsBillingType.Select(x => x.ItemName));
			comboBillType.SelectedIndex=Math.Max(_listDefsBillingType.FindIndex(x => x.DefNum==billtype),0);
			textPassword.Text=CDT.Class1.TryDecrypt(ProgramProperties.GetPropVal(_program.ProgramNum,"Password"));
			checkEnabled.Checked=_program.Enabled;
		}

		private void ButBrowse_Click(object sender,EventArgs e) {
			string selectedPath=textExportFolder.Text;
			if(string.IsNullOrEmpty(selectedPath) || !Directory.Exists(selectedPath)) {
				selectedPath="C:\\";
			}
			using(FolderBrowserDialog folderBrowserDialog=new FolderBrowserDialog() { SelectedPath=selectedPath,Description=Lan.g(this,"Select Export Folder Location") }) {
				try {
					if(folderBrowserDialog.ShowDialog()==DialogResult.OK) {
						textExportFolder.Text=folderBrowserDialog.SelectedPath;
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
			long defNum=-1;
			if(comboBillType.SelectedIndex>-1) {
				defNum=_listDefsBillingType[comboBillType.SelectedIndex].DefNum;
			}
			bool hasChanges=false;
			if(_program.Enabled!=checkEnabled.Checked) {
				_program.Enabled=checkEnabled.Checked;
				Programs.Update(_program);
				hasChanges=true;
			}
			string password=CDT.Class1.TryEncrypt(textPassword.Text);
			hasChanges |= ProgramProperties.SetProperty(_program.ProgramNum,"FolderPath",textExportFolder.Text)>0;
			hasChanges |= ProgramProperties.SetProperty(_program.ProgramNum,"BillingType",defNum.ToString())>0;
			hasChanges |= ProgramProperties.SetProperty(_program.ProgramNum,"Password",password)>0;
			if(hasChanges) {
				DataValid.SetInvalid(InvalidType.Programs);
			}
			DialogResult=DialogResult.OK;
		}
	}
}