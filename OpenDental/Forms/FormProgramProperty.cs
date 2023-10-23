using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary>/// </summary>
	public partial class FormProgramProperty : FormODBase {
		///<summary>If true, the program property value will be decrypted when displayed to the user and encrypted when saved to the database.///</summary>
		private bool _isPassword;
		public ProgramProperty ProgramPopertyCur;

		///<summary></summary>
		public FormProgramProperty(bool isPassword=false)
		{
			//
			// Required for Windows Form Designer support
			//
			_isPassword=isPassword;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormProgramProperty_Load(object sender, System.EventArgs e) {
			textValue.Select();
			textProperty.Text=ProgramPopertyCur.PropertyDesc;
			if(_isPassword) {
				if(ProgramPopertyCur.PropertyValue!="") {
					string decryptedText;
					CDT.Class1.Decrypt(ProgramPopertyCur.PropertyValue,out decryptedText);
					textValue.Text=decryptedText;
				}
				checkIsHighSecurity.Visible=true;
				checkIsHighSecurity.Checked=ProgramPopertyCur.IsHighSecurity;
			}
			else {
				textValue.Text=ProgramPopertyCur.PropertyValue;
			}
			ProgramPopertyCur.TagOD=textValue.Text;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			string progName=Programs.GetProgram(ProgramPopertyCur.ProgramNum).ProgName;
			ProgramPopertyCur.PropertyValue=textValue.Text;
			if(_isPassword) {
				string encryptedText;
				CDT.Class1.Encrypt(ProgramPopertyCur.PropertyValue,out encryptedText);
				ProgramPopertyCur.PropertyValue=encryptedText;
			}
			if(ProgramPopertyCur.IsHighSecurity!=checkIsHighSecurity.Checked) {
				SecurityLogs.MakeLogEntry(EnumPermType.ManageHighSecurityProgProperties,0,$"Security level for {progName}'s {ProgramPopertyCur.PropertyDesc} was altered.",ProgramPopertyCur.ProgramNum,DateTime.Now);
			}
			if(checkIsHighSecurity.Checked && ProgramPopertyCur.TagOD.ToString()!=textValue.Text) {
				SecurityLogs.MakeLogEntry(EnumPermType.ManageHighSecurityProgProperties,0,$"Value for {progName}'s {ProgramPopertyCur.PropertyDesc} was altered.",ProgramPopertyCur.ProgramNum,DateTime.Now);
			}
			ProgramPopertyCur.IsHighSecurity=checkIsHighSecurity.Checked;
			ProgramProperties.Update(ProgramPopertyCur);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}





















