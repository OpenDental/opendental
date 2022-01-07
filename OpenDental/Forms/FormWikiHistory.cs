using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormWikiHistory:FormODBase {
		public string PageTitleCur;
		///<summary>True if the page can only be edited by WikiAdmins.</summary>
		public bool IsLocked=false;

		public FormWikiHistory() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWikiHistory_Load(object sender,EventArgs e) {
			ResizeControls();
			//textContent.ReadOnly=true;
			FillGrid();
			LoadWikiPage(gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag as WikiPageHist);//should never be null.
			Text=Lan.g(this,"Wiki History")+" - "+PageTitleCur;
			//Page is locked and user doesn't have permission
			if(IsLocked && !Security.IsAuthorized(Permissions.WikiAdmin,true)) {
				butRevert.Enabled=false;
			}
			else {
				labelNotAuthorized.Visible=false;
			}
		}

		private void ResizeControls() {
			//assuming gridMain, textNumbers do not change width or location.
			Rectangle actualWorkingArea=new Rectangle(294,12,ClientSize.Width-397,ClientSize.Height-24);
			//text resize
			textContent.Top=actualWorkingArea.Top;
			textContent.Height=actualWorkingArea.Height;
			textContent.Left=actualWorkingArea.Left;
			textContent.Width=actualWorkingArea.Width/2-2;
			//Browser resize
			webBrowserWiki.Top=actualWorkingArea.Top;
			webBrowserWiki.Height=actualWorkingArea.Height;
			webBrowserWiki.Left=actualWorkingArea.Left+actualWorkingArea.Width/2+2;
			webBrowserWiki.Width=actualWorkingArea.Width/2-2;
			//Button move
			//butRefresh.Left=ClientSize.Width/2+2;
		}

		private void LoadWikiPage(WikiPageHist wikiPageCur) {
			try {
				if(string.IsNullOrEmpty(wikiPageCur.PageContent)) {
					//if this is the first time the user has clicked on this revision, get page content from db (the row's tag will have this as well)
					wikiPageCur.PageContent=WikiPageHists.GetPageContent(wikiPageCur.WikiPageNum);
				}
				textContent.Text=WikiPages.GetWikiPageContentWithWikiPageTitles(wikiPageCur.PageContent);
				webBrowserWiki.DocumentText=MarkupEdit.TranslateToXhtml(textContent.Text,false,hasWikiPageTitles: true);
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
			GridColumn col=new GridColumn(Lan.g(this,"User"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Del"),25);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Saved"),80);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			List<WikiPageHist> listWikiPageHists=WikiPageHists.GetByTitleNoPageContent(PageTitleCur);
			WikiPage wp=WikiPages.GetByTitle(PageTitleCur);
			if(wp!=null) {
				listWikiPageHists.Add(WikiPages.PageToHist(wp));
			}
			Dictionary<long,string> dictUsers=Userods.GetUsers(listWikiPageHists.Select(x => x.UserNum).Distinct().ToList())//gets from cache, very fast
				.ToDictionary(x => x.UserNum,x => x.UserName);//create dictionary of key=UserNum, value=UserName for fast lookup
			foreach(WikiPageHist wPage in listWikiPageHists) {
				GridRow row=new GridRow();
				string userName;
				if(!dictUsers.TryGetValue(wPage.UserNum,out userName)) {
					userName="";
				}
				row.Cells.Add(userName);
				row.Cells.Add((wPage.IsDeleted?"X":""));
				row.Cells.Add(wPage.DateTimeSaved.ToString());
				row.Tag=wPage;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.SetSelected(gridMain.ListGridRows.Count-1,true);//There will always be at least one page in the history (the current revision of the page)
			gridMain.ScrollToEnd();//in case there are LOTS of revisions
		}

		private void gridMain_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				return;
			}
			webBrowserWiki.AllowNavigation=true;
			LoadWikiPage(gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag as WikiPageHist);
			gridMain.Focus();
		}

		private void gridMain_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			//using MsgBoxCopyPaste mbox = new MsgBoxCopyPaste(ListWikiPageHists[e.Row].PageContent);
			//mbox.ShowDialog();
			//using FormWikiEdit FormWE = new FormWikiEdit();
			//FormWE.WikiPageCur=listWikiPages[gridMain.SelectedIndices[0]];
			//FormWE.ShowDialog();
			//if(FormWE.DialogResult!=DialogResult.OK) {
			//  return;
			//}
			//FillGrid();
			//LoadWikiPage(listWikiPages[0]);
		}

		private void webBrowserWiki_Navigated(object sender,WebBrowserNavigatedEventArgs e) {
			webBrowserWiki.AllowNavigation=false;//to disable links in pages.
		}

		private void butRevert_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				return;
			}
			if(gridMain.GetSelectedIndex()==gridMain.ListGridRows.Count-1) {//current revision of page
				//DialogResult=DialogResult.OK;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Revert page to currently selected revision?")) {
				return;
			}
			WikiPage wikiPageNew = WikiPageHists.RevertFrom(gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag as WikiPageHist);
			wikiPageNew.UserNum=Security.CurUser.UserNum;
			WikiPages.InsertAndArchive(wikiPageNew);
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}