using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormWikiIncomingLinks:FormODBase {
		public string PageTitleCur;
		public WikiPage JumpToPage;
		private List<WikiPage> ListWikiPages;

		public FormWikiIncomingLinks() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWiki_Load(object sender,EventArgs e) {
			Text="Incoming links to "+PageTitleCur;
			FillGrid();
			if(ListWikiPages.Count==0) {
				MsgBox.Show(this,"This page has no incoming links.");
				Close();
			}
		}

		private void LoadWikiPage(WikiPage wikiPage) {
			try {
				webBrowserWiki.DocumentText=MarkupEdit.TranslateToXhtml(wikiPage.PageContent,false);
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
			GridColumn col=new GridColumn(Lan.g(this,"Page Title"),70);
			gridMain.ListGridColumns.Add(col);
			//col=new ODGridColumn(Lan.g(this,"Saved"),42);
			//gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			ListWikiPages=WikiPages.GetIncomingLinks(PageTitleCur);
			for(int i=0;i<ListWikiPages.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(ListWikiPages[i].PageTitle);
				//row.Cells.Add(page.DateTimeSaved.ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length<1) {
				return;
			}
			webBrowserWiki.AllowNavigation=true;
			LoadWikiPage(ListWikiPages[gridMain.SelectedIndices[0]]);
			gridMain.Focus();
		}

		private void gridMain_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			JumpToPage=ListWikiPages[e.Row];
			DialogResult=DialogResult.OK;
		}

		private void webBrowserWiki_Navigated(object sender,WebBrowserNavigatedEventArgs e) {
			webBrowserWiki.AllowNavigation=false;//to disable links in pages.
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}