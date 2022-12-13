using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	///<summary></summary>
	public delegate void NavToPageDeligate(string pageTitle);

	public partial class FormWikiSearch:FormODBase {
		private const bool DO_INCLUDE_CONTENT_DEFAULT=false;
		private List<string> listWikiPageTitles;
		public string wikiPageTitleSelected;
		public NavToPageDeligate NavToPage;

		public FormWikiSearch() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWikiSearch_Load(object sender,EventArgs e) {
			Rectangle rectWorkingArea=System.Windows.Forms.Screen.GetWorkingArea(this);
			Top=0;
			Left=Math.Max(0,((rectWorkingArea.Width-LayoutManager.Scale(1200))/2)+rectWorkingArea.Left);
			Width=Math.Min(rectWorkingArea.Width,LayoutManager.Scale(1200));
			Height=rectWorkingArea.Height;
			FillGrid();
			wikiPageTitleSelected="";
			UserOdPref userIncludeContentPref=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.WikiSearchIncludeContent).FirstOrDefault();
			bool doIncludeContent=DO_INCLUDE_CONTENT_DEFAULT;
			if(userIncludeContentPref!=null) {
				doIncludeContent=PIn.Bool(userIncludeContentPref.ValueString);
			}
			checkIgnoreContent.Checked=!doIncludeContent;
			if(string.IsNullOrWhiteSpace(PrefC.GetString(PrefName.ReportingServerDbName))
				|| string.IsNullOrWhiteSpace(PrefC.GetString(PrefName.ReportingServerCompName))) 
			{
				checkReportServer.Visible=false;
			}
			else {//default to report server when one is set up.
				checkReportServer.Visible=true;
				checkReportServer.Checked=true;
			}
			SetFilterControlsAndAction(
				() => { 
					//This is so we're not running queries every time this event fires unless we want it to
					if(checkIgnoreContent.Checked) { 
						FillGrid(); 
					} 
				},
				(int)TimeSpan.FromSeconds(0.5).TotalMilliseconds,
				textSearch);
		}

		private void LoadWikiPage(string wikiPageTitle) {
			webBrowserWiki.AllowNavigation=true;
			butRestore.Enabled=false;
			try {
				if(checkArchivedOnly.Checked) {
					webBrowserWiki.DocumentText=MarkupEdit.TranslateToXhtml(WikiPages.GetByTitle(wikiPageTitle,isDeleted:true).PageContent,true);
					butRestore.Enabled=true;
				}
				else {
					webBrowserWiki.DocumentText=MarkupEdit.TranslateToXhtml(WikiPages.GetByTitle(wikiPageTitle).PageContent,true);
				}
			}
			catch(Exception ex) {
				webBrowserWiki.DocumentText="";
				if(MessageBox.Show(this,Lan.g(this,"This page is broken and cannot be viewed.  Error message:")+" "+ex.Message
					+"\r\n\r\n"+Lan.g(this,"Would you like to edit this wiki page?"),
					Lan.g(this,"Wiki page content error"),MessageBoxButtons.YesNo)==DialogResult.No)
				{
					return;
				}
				//Similar to how FormWiki.Edit_Click() loads up FormWikiEdit.
				using FormWikiEdit formWikiEdit=new FormWikiEdit();
				formWikiEdit.WikiPageCur=WikiPages.GetByTitle(wikiPageTitle);
				//Maximized non-modal forms pop up on the wrong screen, as documented in FormODBase.OnLoad, so:
				Rectangle rectangleScreen=System.Windows.Forms.Screen.FromControl(this).WorkingArea;
				//Make it half size for when user un-maximizes.
				formWikiEdit.Bounds=new Rectangle(rectangleScreen.Left+rectangleScreen.Width/4,rectangleScreen.Top+rectangleScreen.Height/4,
					rectangleScreen.Width/2,rectangleScreen.Height/2);
				formWikiEdit.WindowState=FormWindowState.Maximized;
				if(formWikiEdit.ShowDialog()==DialogResult.OK) {
					LoadWikiPage(wikiPageTitle);//Try translating the page content again since the user most likely fixed the issue when editing.
				}
			}
		}

		/// <summary></summary>
		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Title"),70);
			gridMain.Columns.Add(col);
			//col=new ODGridColumn(Lan.g(this,"Saved"),42);
			//gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			//This used to search the wikipagehist table, now archived pages are stored in wikipage.  See JobNum 4429.
			listWikiPageTitles=ReportsComplex.RunFuncOnReportServer(() => WikiPages.GetForSearch(textSearch.Text,checkIgnoreContent.Checked,checkArchivedOnly.Checked,
				checkBoxMatchWholeWord.Checked,checkBoxShowMainPages.Checked),checkReportServer.Checked);
			for(int i=0;i<listWikiPageTitles.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(listWikiPageTitles[i]);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			webBrowserWiki.DocumentText="";
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			LoadWikiPage(listWikiPageTitles[e.Row]);
			gridMain.Focus();
		}

		private void gridMain_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {			
			//SelectedWikiPage=listWikiPages[e.Row];
			if(checkArchivedOnly.Checked) {
				return;
			}
			wikiPageTitleSelected=listWikiPageTitles[e.Row];
			NavToPage(wikiPageTitleSelected);
			Close();
		}

		private void textSearch_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.Enter) {
				FillGrid();
			}
		}

		private void butSearch_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void checkBoxShowMainPages_CheckedChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void checkIgnoreContent_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void checkArchivedOnly_CheckedChanged(object sender,EventArgs e) {
			butOK.Enabled=!checkArchivedOnly.Checked;
			FillGrid();
		}

		private void checkBoxMatchWholeWord_CheckedChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void webBrowserWiki_Navigated(object sender,WebBrowserNavigatedEventArgs e) {
			webBrowserWiki.AllowNavigation=false;//to disable links in pages.
		}

		private void butRestore_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				return;//should never happen.
			}
			wikiPageTitleSelected=listWikiPageTitles[gridMain.SelectedIndices[0]];
			if(WikiPages.GetByTitle(wikiPageTitleSelected)!=null) {
				MsgBox.Show(this,"Selected page has already been restored.");//should never happen.
				return;
			}
			WikiPage wikiPageRestored=WikiPages.GetByTitle(listWikiPageTitles[gridMain.SelectedIndices[0]],isDeleted:true);
			if(wikiPageRestored==null) {
				MsgBox.Show(this,"Selected page has already been restored.");//should never happen.
				return;
			}
			WikiPages.WikiPageRestore(wikiPageRestored,Security.CurUser.UserNum);
			Close();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length>0) {
				wikiPageTitleSelected=listWikiPageTitles[gridMain.SelectedIndices[0]];
			}
			NavToPage(wikiPageTitleSelected);
			Close();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			Close();
		}

		private void FormWikiSearch_FormClosing(object sender,FormClosingEventArgs e) {
			bool doIncludeContent=!checkIgnoreContent.Checked;
			UserOdPref userIncludeContentPref=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.WikiSearchIncludeContent).FirstOrDefault();
			if(userIncludeContentPref==null) {
				if(doIncludeContent!=DO_INCLUDE_CONTENT_DEFAULT) {
					//Only insert the new UserOdPref if not the default value.
					UserOdPrefs.Insert(new UserOdPref {
						UserNum=Security.CurUser.UserNum,
						FkeyType=UserOdFkeyType.WikiSearchIncludeContent,
						ValueString=POut.Bool(doIncludeContent),
					});
					DataValid.SetInvalid(InvalidType.UserOdPrefs);
				}
				return;
			}
			UserOdPref userOdPrefOld=userIncludeContentPref.Clone();
			userIncludeContentPref.ValueString=POut.Bool(doIncludeContent);
			if(UserOdPrefs.Update(userIncludeContentPref,userOdPrefOld)) {
				//Only need to signal cache refresh on change.
				DataValid.SetInvalid(InvalidType.UserOdPrefs);
			}
		}
	}
}