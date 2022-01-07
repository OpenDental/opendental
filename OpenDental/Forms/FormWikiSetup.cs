using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormWikiSetup:FormODBase {

		public FormWikiSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWikiSetup_Load(object sender,EventArgs e) {
			textMaster.Text=WikiPages.MasterPage.PageContent;
			checkDetectLinks.Checked=PrefC.GetBool(PrefName.WikiDetectLinks);
			checkCreatePageFromLinks.Checked=PrefC.GetBool(PrefName.WikiCreatePageFromLink);
		}

		private void butOK_Click(object sender,EventArgs e) {
			//Prefs
			if(Prefs.UpdateBool(PrefName.WikiDetectLinks,checkDetectLinks.Checked)
				| Prefs.UpdateBool(PrefName.WikiCreatePageFromLink,checkCreatePageFromLinks.Checked)) 
			{
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			//Master Page
			WikiPage masterPage=WikiPages.MasterPage;
			masterPage.PageContent=textMaster.Text;
			masterPage.UserNum=Security.CurUser.UserNum;
			WikiPages.InsertAndArchive(masterPage);
			DataValid.SetInvalid(InvalidType.Wiki);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}