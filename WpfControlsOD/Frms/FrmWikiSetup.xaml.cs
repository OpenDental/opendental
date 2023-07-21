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
			//Lan.F(this);
		}

		private void FrmWikiSetup_Loaded(object sender,RoutedEventArgs e) {
			textMaster.Text=WikiPages.WikiPageMaster.PageContent;
			checkDetectLinks.Checked=PrefC.GetBool(PrefName.WikiDetectLinks);
			checkCreatePageFromLinks.Checked=PrefC.GetBool(PrefName.WikiCreatePageFromLink);
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