using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormWikiRename:FormODBase {
		public string PageTitle;

		public FormWikiRename() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWikiRename_Load(object sender,EventArgs e) {
			if(PageTitle!="" && PageTitle!=null) {
				textPageTitle.Text=PageTitle;
				Text="Page Title - "+PageTitle;
				textPageTitle.Text=PageTitle;
			}
			if(textPageTitle.Text=="Home") {//TODO later:replace this with a dynamic "Home" pagename lookup like: PrefC.GetString(PrefName.WikiHomePage);
				MsgBox.Show(this,"Cannot rename the default homepage.");
				//butOK.Enabled=false;
				//textPageTitle.Enabled=false;
				DialogResult=DialogResult.Cancel;//user doesn't need to see this form
			}
		}

		private bool ValidateTitle() {
			if(textPageTitle.Text=="") {
				MsgBox.Show(this,"Page title cannot be empty.");
				return false;
			}
			if(textPageTitle.Text==PageTitle) {
				//"rename" to the same thing.
				DialogResult=DialogResult.Cancel;
				return false;
			}
			string errorMsg="";
			if(!WikiPages.IsWikiPageTitleValid(textPageTitle.Text,out errorMsg)) {
				MessageBox.Show(errorMsg);//errorMsg was already translated.
				return false;
			}
			if(PageTitle!=null && textPageTitle.Text.ToLower()==PageTitle.ToLower()) {
				//the user is just trying to change the capitalization, which is allowed
				return true;
			}
			WikiPage wp=WikiPages.GetByTitle(textPageTitle.Text);//this is case insensitive, so it won't let you name it the same as another page even if capitalization is different.
			if(wp!=null){
				MsgBox.Show(this,"Page title already exists.");
				return false;
			}
			return true;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!ValidateTitle()) {
				return;
			}
			PageTitle=textPageTitle.Text;
			//do not save here. Save is handled where this form is called from.
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}