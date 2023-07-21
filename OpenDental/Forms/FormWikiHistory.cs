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
		public string PageTitle;
		///<summary>True if the page can only be edited by WikiAdmins.</summary>
		public bool IsLocked=false;
		private int _idxRowFirstSelected=0;

		public FormWikiHistory() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWikiHistory_Load(object sender,EventArgs e) {
			ResizeControls();
			FillGrid();
			_idxRowFirstSelected=gridMain.GetSelectedIndex();
			LoadWikiPage(gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag as WikiPageHist);//should never be null.
			Text=Lan.g(this,"Wiki History")+" - "+PageTitle;
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
			int xDivider=(ClientSize.Width-gridMain.Width)/2;
			//textContent
			LayoutManager.MoveLocation(textContent,new Point(gridMain.Width+6,textContent.Location.Y));
			LayoutManager.MoveSize(textContent,new Size(xDivider,gridMain.Height));
			if(textContent.Width<=60) {
				LayoutManager.MoveSize(textContent,new Size(61,gridMain.Height));
			} 
			if(textContent.Height<=60) {
				LayoutManager.MoveSize(textContent,new Size(xDivider,61));
			}
			//webBrowserWiki
			LayoutManager.MoveLocation(webBrowserWiki,new Point(textContent.Location.X+xDivider,webBrowserWiki.Location.Y));
			LayoutManager.MoveSize(webBrowserWiki,new Size(xDivider-15,gridMain.Height));
		}

		private void LoadWikiPage(WikiPageHist wikiPageHist) {
			if(string.IsNullOrEmpty(wikiPageHist.PageContent)) {
				//if this is the first time the user has clicked on this revision, get page content from db (the row's tag will have this as well)
				wikiPageHist.PageContent=WikiPageHists.GetPageContent(wikiPageHist.WikiPageNum);
			}
			WikiPageHist wikiPageHistSelectedFirst=(WikiPageHist)gridMain.ListGridRows[_idxRowFirstSelected].Tag;
			if(wikiPageHistSelectedFirst!=wikiPageHist) {
				return;
			}
			textContent.Text=WikiPages.GetWikiPageContentWithWikiPageTitles(wikiPageHist.PageContent);
			try {
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
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"User"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Del"),25);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Saved"),80);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			List<WikiPageHist> listWikiPageHists=WikiPageHists.GetByTitleNoPageContent(PageTitle);
			WikiPage wikiPage=WikiPages.GetByTitle(PageTitle);
			if(wikiPage!=null) {
				listWikiPageHists.Add(WikiPages.PageToHist(wikiPage));
			}
			for(int i=0;i<listWikiPageHists.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(Userods.GetName(listWikiPageHists[i].UserNum));
				row.Cells.Add((listWikiPageHists[i].IsDeleted?"X":""));
				row.Cells.Add(listWikiPageHists[i].DateTimeSaved.ToString());
				row.Tag=listWikiPageHists[i];
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
			int[] intArrayIndices=gridMain.SelectedIndices;
			if(intArrayIndices.Length==1) {//True only when first revision is selected
				_idxRowFirstSelected=intArrayIndices[0];//Display first seleced revision when multiple revisions are selected
			}
			for(int i=0;i<intArrayIndices.Length;i++) {
				int index=intArrayIndices[i];
				LoadWikiPage(gridMain.ListGridRows[index].Tag as WikiPageHist);
			}
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

		private void butCompare_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Count()==0) {
				MessageBox.Show("Select 2 revisions to compare.  You can select mulitple revisions while holding down (Ctrl).");
				return;
			}
			else if(gridMain.SelectedIndices.Count()==1) {
				MessageBox.Show("Select 1 more revision to compare.  You can select another revision while holding down (Ctrl).");
				return;
			}
			else if(gridMain.SelectedIndices.Count()>2) {
				MessageBox.Show("Only select 2 revisions to compare.  You can unselect mulitple revisions while holding down (Ctrl).");
				return;
			}
			WikiPageHist wikiPageHistLeft=(WikiPageHist)gridMain.SelectedGridRows[0].Tag;
			WikiPageHist wikiPageHistRight=(WikiPageHist)gridMain.SelectedGridRows[1].Tag;
			//gridMain.ListGridRows[gridMain.GetSelectedIndex(gridMain.SelectedIndices)
			FormWikiCompare formWikiCompare=new FormWikiCompare();
			formWikiCompare.InitializeRevisionsSelected(wikiPageHistLeft,wikiPageHistRight);
			formWikiCompare.ShowDialog();
		}

		private void FormWikiHistory_ResizeChanged(object sender,EventArgs e) {
			ResizeControls();
		}
	}
}