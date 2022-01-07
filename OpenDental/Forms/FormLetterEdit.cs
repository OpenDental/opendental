using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormLetterEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		public Letter LetterCur;

		///<summary></summary>
		public FormLetterEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormLetterEdit_Load(object sender, System.EventArgs e) {
			textDescription.Text=LetterCur.Description;
			textBody.Text=LetterCur.BodyText;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			LetterCur.Description=textDescription.Text;
			LetterCur.BodyText=textBody.Text;
			if(IsNew){
				Letters.Insert(LetterCur);
			}
			else{
				Letters.Update(LetterCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		


	}
}





















