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
		public EmailAutograph EmailAutographCur;

		///<summary></summary>
		public FormEmailAutographEdit(EmailAutograph emailAutograph,bool isNew=false)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			EmailAutographCur=emailAutograph;
			IsNew=isNew;
		}

		private void FormEmailTemplateEdit_Load(object sender, System.EventArgs e) {
			textDescription.Text=EmailAutographCur.Description;
			textEmailAddress.Text=EmailAutographCur.EmailAddress;
			textAutographText.Text=EmailAutographCur.AutographText;
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
			EmailAutographCur.Description=textDescription.Text;
			EmailAutographCur.EmailAddress=textEmailAddress.Text;
			EmailAutographCur.AutographText=textAutographText.Text;
			if(IsNew){
				EmailAutographs.Insert(EmailAutographCur);
			}
			else{
				EmailAutographs.Update(EmailAutographCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}