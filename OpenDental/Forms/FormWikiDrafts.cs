using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	public partial class FormWikiDrafts:FormODBase {
		private List<WikiPage> _listWikiPage;
		public FormWiki OwnerForm;

		public FormWikiDrafts() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWikiDrafts_Load(object sender,EventArgs e) {
			ResizeControls();
			FillGrid();
			gridMain.SetSelected(gridMain.ListGridRows.Count-1,true);//select most recent draft
			LoadWikiPage(_listWikiPage[gridMain.SelectedIndices[0]]);
			Text=Lan.g(this,"Wiki Drafts")+" - "+OwnerForm.WikiPageCur.PageTitle;
		}

		///<summary>Resize text boxes to each occupy ~1/2 of screen from top to bottom.</summary>
		private void ResizeControls() {
			//assuming gridMain, textNumbers do not change width or location.
			Rectangle actualWorkingArea=new Rectangle(294,12,ClientSize.Width-397,ClientSize.Height-24);
			//textNumbers resize
			LayoutManager.MoveHeight(textNumbers,actualWorkingArea.Height);
			//text resize
			LayoutManager.MoveLocation(textContent,new Point(textContent.Location.X,actualWorkingArea.Top));
			LayoutManager.MoveHeight(textContent,actualWorkingArea.Height);
			LayoutManager.MoveLocation(textContent,new Point(actualWorkingArea.Left,textContent.Location.Y));
			LayoutManager.MoveWidth(textContent,actualWorkingArea.Width/2-2);
			//Browser resize
			LayoutManager.MoveLocation(webBrowserWiki,new Point(webBrowserWiki.Location.X,actualWorkingArea.Top));
			LayoutManager.MoveHeight(webBrowserWiki,actualWorkingArea.Height);
			LayoutManager.MoveLocation(webBrowserWiki,new Point(actualWorkingArea.Left+actualWorkingArea.Width/2+2,webBrowserWiki.Location.Y));
			LayoutManager.MoveWidth(webBrowserWiki,actualWorkingArea.Width/2-2);
		}

		private void LoadWikiPage(WikiPage WikiPageCur) {
			try {
				textContent.Text=WikiPages.GetWikiPageContentWithWikiPageTitles(WikiPageCur.PageContent);
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
			col=new GridColumn(Lan.g(this,"Last Saved"),80);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			_listWikiPage=WikiPages.GetDraftsByTitle(OwnerForm.WikiPageCur.PageTitle);
			for(int i=0;i<_listWikiPage.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(Userods.GetName(_listWikiPage[i].UserNum));
				row.Cells.Add(_listWikiPage[i].DateTimeSaved.ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length<1) {
				return;
			}
			webBrowserWiki.AllowNavigation=true;
			LoadWikiPage(_listWikiPage[gridMain.SelectedIndices[0]]);
			gridMain.Focus();
		}

		private void gridMain_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			EditWikiDraft();
		}

		private void webBrowserWiki_Navigated(object sender,WebBrowserNavigatedEventArgs e) {
			webBrowserWiki.AllowNavigation=false;//to disable links in pages.
		}

		private void butEdit_Click(object sender,EventArgs e) {
			EditWikiDraft();
		}

		private void EditWikiDraft() {
			int selectedIndex=gridMain.GetSelectedIndex();
			using FormWikiEdit FormWE=new FormWikiEdit();
			FormWE.WikiPageCur=_listWikiPage[selectedIndex];
			FormWE.OwnerForm=this.OwnerForm;
			FormWE.ShowDialog();
			if(FormWE.HasSaved) {
				Close();
				return;
			}
			FillGrid();
			gridMain.SetSelected(selectedIndex,true);
			webBrowserWiki.AllowNavigation=true;
			LoadWikiPage(_listWikiPage[selectedIndex]);
			gridMain.Focus();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()<0) {
				return;
			}
			int selectedIndex=gridMain.GetSelectedIndex();
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Delete this draft?")) {
				return;
			}
			try {
				WikiPages.DeleteDraft(_listWikiPage[selectedIndex]);
			}
			catch (Exception ex){
				//should never happen because we are only ever editing drafts here.
				MessageBox.Show(ex.Message);
				return;
			}
			//deleting Edge cases.
			FillGrid();
			if(selectedIndex>=_listWikiPage.Count) {
				selectedIndex--;
			}
			if(_listWikiPage.Count<1) {
				//Nothing else a user could possibly do when there are no drafts. Exit.
				Close();
				return;
			}
			gridMain.SetSelected(selectedIndex,true);
			webBrowserWiki.AllowNavigation=true;
			LoadWikiPage(_listWikiPage[selectedIndex]);
			gridMain.Focus();
		}

	}
}