using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	///<summary>Email autograph edit.</summary>
	public partial class FormEmailAutographEdit:FormODBase {
		///<summary></summary>
		public bool IsNew;
		///<summary></summary>
		private EmailAutograph _emailAutograph;

		public EmailAutograph EmailAutographCur {
			get { return _emailAutograph; }
		}

		///<summary></summary>
		public FormEmailAutographEdit(EmailAutograph emailAutograph,bool isNew=false)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_emailAutograph=emailAutograph;
			IsNew=isNew;
		}

		private void FormEmailTemplateEdit_Load(object sender, System.EventArgs e) {
			textDescription.Text=_emailAutograph.Description;
			textEmailAddress.Text=_emailAutograph.EmailAddress;
			textAutographText.Text=_emailAutograph.AutographText;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textAutographText.Text=="") {
				MsgBox.Show(this,"Autograph Text cannot be blank.");
				return;
			}
			if(textDescription.Text==""){
				MsgBox.Show(this,"Description cannot be blank.");
				return;
			}
			_emailAutograph.Description=textDescription.Text;
			_emailAutograph.EmailAddress=textEmailAddress.Text;
			_emailAutograph.AutographText=textAutographText.Text;
			if(IsNew){
				EmailAutographs.Insert(_emailAutograph);
			}
			else{
				EmailAutographs.Update(_emailAutograph);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}





















