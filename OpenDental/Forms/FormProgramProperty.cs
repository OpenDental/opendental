using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

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
			if(_isPassword && ProgramPropertyCur.PropertyValue!="") {
				string decryptedText;
				CDT.Class1.Decrypt(ProgramPropertyCur.PropertyValue,out decryptedText);
				textValue.Text=decryptedText;
			}
			else {
				textValue.Text=ProgramPropertyCur.PropertyValue;
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			ProgramPropertyCur.PropertyValue=textValue.Text;
			if(_isPassword) {
				string encryptedText;
				CDT.Class1.Encrypt(ProgramPropertyCur.PropertyValue,out encryptedText);
				ProgramPropertyCur.PropertyValue=encryptedText;
			}
			ProgramProperties.Update(ProgramPropertyCur);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		


	}
}





















