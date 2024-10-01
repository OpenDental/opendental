using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
    public partial class FormWikiAllPages:FormODBase {
		///<summary>Need a reference to the form where this was launched from so that we can tell it to refresh later.</summary>
		public FormWikiEdit FormWikiEditOwner;

		public FormWikiAllPages() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWikiAllPages_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void textSearch_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void LoadWikiPage(string wikiPageTitle) {
			bool includeArchived=checkIncludeArchived.Checked;
			WikiPage wikiPage=WikiPages.GetByTitle(wikiPageTitle,isDeleted:includeArchived);
			try {
				webBrowserWiki.DocumentText=MarkupEdit.TranslateToXhtml(wikiPage.PageContent,isPreviewOnly:true);;
			}
			catch(Exception ex) {
				webBrowserWiki.DocumentText="";
				MessageBox.Show(this,Lan.g(this,"This page is broken and cannot be viewed.  Error message:")+" "+ex.Message);
			}
		}

		/// <summary></summary>
		private void FillGrid() {
			bool includeArchived=checkIncludeArchived.Checked;
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Title"),70);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			List<string> listWikiPageTitles=WikiPages.GetForSearch(textSearch.Text,true,isDeleted:includeArchived);
			for(int i=0;i<listWikiPageTitles.Count;i++) {
				GridRow row=new GridRow();
				string wikiPageTitle=PIn.String(listWikiPageTitles[i]);
				row.Tag=wikiPageTitle;
				row.Cells.Add(wikiPageTitle);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			webBrowserWiki.AllowNavigation=true;
			LoadWikiPage(gridMain.SelectedTag<string>());
			gridMain.Focus();
		}

		private void gridMain_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			if(FormWikiEditOwner!=null && !FormWikiEditOwner.IsDisposed) {
				WikiPage wikiPageSelected=WikiPages.GetByTitle(gridMain.SelectedTag<string>());
				FormWikiEditOwner.RefreshPage(wikiPageSelected);
			}
			Close();
		}

		private void webBrowserWiki_Navigated(object sender,WebBrowserNavigatedEventArgs e) {
			webBrowserWiki.AllowNavigation=false;//to disable links in pages.
		}

		///<summary>Adds a new wikipage.</summary>
		private void butAdd_Click(object sender,EventArgs e) {
			FrmWikiRename frmWikiRename=new FrmWikiRename();
			frmWikiRename.ShowDialog();
			if(!frmWikiRename.IsDialogOK) {
				return;
			}
			Action<string> actionWikiSaved=new Action<string>((pageTitleNew) => {
				//return the new wikipage added to FormWikiEdit
				WikiPage wikiPage=WikiPages.GetByTitle(pageTitleNew);
				if(wikiPage!=null && FormWikiEditOwner!=null && !FormWikiEditOwner.IsDisposed) {
					FormWikiEditOwner.RefreshPage(wikiPage);
				}
			});
			FormWikiEdit formWikiEdit=new FormWikiEdit(actionWikiSaved);
			formWikiEdit.WikiPageCur=new WikiPage();
			formWikiEdit.WikiPageCur.IsNew=true;
			formWikiEdit.WikiPageCur.PageTitle=frmWikiRename.PageTitle;
			formWikiEdit.WikiPageCur.PageContent="[["+FormWikiEditOwner.WikiPageCur.PageTitle+"]]\r\n"//link back
				+"<h1>"+frmWikiRename.PageTitle+"</h1>\r\n";//page title
			formWikiEdit.Show();
			Close();
		}

		/// <summary></summary>
		private void butBrackets_Click(object sender,EventArgs e) {
			if(FormWikiEditOwner!=null && !FormWikiEditOwner.IsDisposed) {
				FormWikiEditOwner.RefreshPage(null);
			}
			Close();
		}

		private void checkIncludeArchived_CheckedChanged(object sender,EventArgs e) {
			FillGrid();
		}

		/// <summary></summary>
		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a page first.");
				return;
			}
			if(FormWikiEditOwner!=null && !FormWikiEditOwner.IsDisposed) {
				WikiPage wikiPageSelected=WikiPages.GetByTitle(gridMain.SelectedTag<string>());
				FormWikiEditOwner.RefreshPage(wikiPageSelected);
			}
			Close();
		}

	
	}
}