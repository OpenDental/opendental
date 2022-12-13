using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormProgramProperty : FormODBase {
		///<summary>If true, the program property value will be decrypted when displayed to the user and encrypted when saved to the database.///</summary>
		private bool _isPassword;
		public ProgramProperty ProgramPropertyCur;

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
			textProperty.Text=ProgramPropertyCur.PropertyDesc;
			if(_isPassword) {
				if(ProgramPropertyCur.PropertyValue!="") {
					string decryptedText;
					CDT.Class1.Decrypt(ProgramPropertyCur.PropertyValue,out decryptedText);
					textValue.Text=decryptedText;
				}
				checkIsHighSecurity.Visible=true;
				checkIsHighSecurity.Checked=ProgramPropertyCur.IsHighSecurity;
			}
			else {
				textValue.Text=ProgramPropertyCur.PropertyValue;
			}
			ProgramPropertyCur.TagOD=textValue.Text;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			string progName=Programs.GetProgram(ProgramPropertyCur.ProgramNum).ProgName;
			ProgramPropertyCur.PropertyValue=textValue.Text;
			if(_isPassword) {
				string encryptedText;
				CDT.Class1.Encrypt(ProgramPropertyCur.PropertyValue,out encryptedText);
				ProgramPropertyCur.PropertyValue=encryptedText;
			}
			if(ProgramPropertyCur.IsHighSecurity!=checkIsHighSecurity.Checked) {
				SecurityLogs.MakeLogEntry(Permissions.ManageHighSecurityProgProperties,0,$"Security level for {progName}'s {ProgramPropertyCur.PropertyDesc} was altered.",ProgramPropertyCur.ProgramNum,DateTime.Now);
			}
			if(checkIsHighSecurity.Checked && ProgramPropertyCur.TagOD.ToString()!=textValue.Text) {
				SecurityLogs.MakeLogEntry(Permissions.ManageHighSecurityProgProperties,0,$"Value for {progName}'s {ProgramPropertyCur.PropertyDesc} was altered.",ProgramPropertyCur.ProgramNum,DateTime.Now);
			}
			ProgramPropertyCur.IsHighSecurity=checkIsHighSecurity.Checked;
			ProgramProperties.Update(ProgramPropertyCur);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}





















