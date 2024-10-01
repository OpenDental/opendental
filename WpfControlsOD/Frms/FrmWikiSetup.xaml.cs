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
	public partial class FrmWikiSetup:FrmODBase {

		public FrmWikiSetup() {
			InitializeComponent();
			Load+=FrmWikiSetup_Load;
			PreviewKeyDown+=FrmWikiSetup_PreviewKeyDown;
		}

		private void FrmWikiSetup_Load(object sender,EventArgs e) {
			Lang.F(this);
			textMaster.Text=WikiPages.WikiPageMaster.PageContent;
			checkDetectLinks.Checked=PrefC.GetBool(PrefName.WikiDetectLinks);
			checkCreatePageFromLinks.Checked=PrefC.GetBool(PrefName.WikiCreatePageFromLink);
		}

		private void FrmWikiSetup_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			//Prefs
			if(Prefs.UpdateBool(PrefName.WikiDetectLinks,(bool)checkDetectLinks.Checked)
				| Prefs.UpdateBool(PrefName.WikiCreatePageFromLink,(bool)checkCreatePageFromLinks.Checked)) 
			{
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			//Master Page
			WikiPage wikiPage=WikiPages.WikiPageMaster;
			wikiPage.PageContent=textMaster.Text;
			wikiPage.UserNum=Security.CurUser.UserNum;
			WikiPages.InsertAndArchive(wikiPage);
			DataValid.SetInvalid(InvalidType.Wiki);
			IsDialogOK=true;
		}

	}
}