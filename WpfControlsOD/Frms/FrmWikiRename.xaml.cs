using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	///<summary></summary>
	public partial class FrmWikiRename:FrmODBase {

		public string PageTitle;

		public FrmWikiRename() {
			InitializeComponent();
			//Lan.F(this);
			Load+=FrmWikiRename_Load;
		}

		private void FrmWikiRename_Load(object sender,EventArgs e) {
			if(PageTitle!="" && PageTitle!=null) {
				textPageTitle.Text=PageTitle;
				Text="Page Title - "+PageTitle;
				textPageTitle.Text=PageTitle;
			}
			if(textPageTitle.Text=="Home") {//TODO later:replace this with a dynamic "Home" pagename lookup like: PrefC.GetString(PrefName.WikiHomePage);
				MsgBox.Show(this,"Cannot rename the default homepage.");
				//butOK.Enabled=false;
				//textPageTitle.Enabled=false;
				IsDialogOK=false;//user doesn't need to see this form
			}
			textPageTitle.SelectAll(); //textPageTitle needs to be TabIndexOD 0 to have starting focus in WPF
		}

		private bool ValidateTitle() {
			if(textPageTitle.Text=="") {
				MsgBox.Show(this,"Page title cannot be empty.");
				return false;
			}
			if(textPageTitle.Text==PageTitle) {
				//"rename" to the same thing.
				IsDialogOK=false;
				return false;
			}
			string errorMsg="";
			if(!WikiPages.IsWikiPageTitleValid(textPageTitle.Text,out errorMsg)) {
				MsgBox.Show(errorMsg);//errorMsg was already translated.
				return false;
			}
			if(PageTitle!=null && textPageTitle.Text.ToLower()==PageTitle.ToLower()) {
				//the user is just trying to change the capitalization, which is allowed
				return true;
			}
			WikiPage wikiPage=WikiPages.GetByTitle(textPageTitle.Text);//this is case insensitive, so it won't let you name it the same as another page even if capitalization is different.
			if(wikiPage!=null){
				MsgBox.Show(this,"Page title already exists.");
				return false;
			}
			return true;
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(!ValidateTitle()) {
				return;
			}
			PageTitle=textPageTitle.Text;
			//do not save here. Save is handled where this form is called from.
			IsDialogOK=true;
		}


	}
}