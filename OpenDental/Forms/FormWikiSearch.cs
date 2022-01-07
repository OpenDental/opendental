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
		private List<string> listWikiPageTitles;
		public string wikiPageTitleSelected;
		public NavToPageDeligate NavToPage;
		private UserOdPref _userIncludeContentPref;

		public FormWikiSearch() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWikiSearch_Load(object sender,EventArgs e) {
			SetFilterControlsAndAction(() => FillGrid(),
				(int)TimeSpan.FromSeconds(0.5).TotalMilliseconds,
				textSearch);
			Rectangle rectWorkingArea=System.Windows.Forms.Screen.GetWorkingArea(this);
			Top=0;
			Left=Math.Max(0,((rectWorkingArea.Width-1200)/2)+rectWorkingArea.Left);
			Width=Math.Min(rectWorkingArea.Width,1200);
			Height=rectWorkingArea.Height;
			FillGrid();
			wikiPageTitleSelected="";
			_userIncludeContentPref=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.WikiSearchIncludeContent).FirstOrDefault();
            if(_userIncludeContentPref==null) {
				checkIgnoreContent.Checked=true;
            }
		}

		private void LoadWikiPage(string WikiPageTitleCur) {
			webBrowserWiki.AllowNavigation=true;
			butRestore.Enabled=false;
			try {
				if(checkArchivedOnly.Checked) {
					webBrowserWiki.DocumentText=MarkupEdit.TranslateToXhtml(WikiPages.GetByTitle(WikiPageTitleCur,isDeleted:true).PageContent,true);
					butRestore.Enabled=true;
				}
				else {
					webBrowserWiki.DocumentText=MarkupEdit.TranslateToXhtml(WikiPages.GetByTitle(WikiPageTitleCur).PageContent,true);
				}
			}
			catch(Exception ex) {
				webBrowserWiki.DocumentText="";
				MessageBox.Show(this,Lan.g(this,"This page is broken and cannot be viewed.  Error message:")+" "+ex.Message);
			}
		}

		/// <summary></summary>
		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Title"),70);
			gridMain.ListGridColumns.Add(col);
			//col=new ODGridColumn(Lan.g(this,"Saved"),42);
			//gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			//This used to search the wikipagehist table, now archived pages are stored in wikipage.  See JobNum 4429.
			listWikiPageTitles=WikiPages.GetForSearch(textSearch.Text,checkIgnoreContent.Checked,checkArchivedOnly.Checked,checkBoxMatchWholeWord.Checked);
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

		private void checkIgnoreContent_CheckedChanged(object sender,EventArgs e) {
			//if preference is not set to ignore content, set preference on uncheck
            if(_userIncludeContentPref==null && !checkIgnoreContent.Checked) {
				UserOdPrefs.Insert(new UserOdPref() {
					UserNum=Security.CurUser.UserNum,
					FkeyType=UserOdFkeyType.WikiSearchIncludeContent
				});
			}//user preference is set and box is checked, remove preference for not ignoring content
			else if(_userIncludeContentPref!=null && checkIgnoreContent.Checked) {
				UserOdPrefs.Delete(_userIncludeContentPref.UserOdPrefNum);
			}
			//update cached pref value
			_userIncludeContentPref=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.WikiSearchIncludeContent).FirstOrDefault();
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
	}
}