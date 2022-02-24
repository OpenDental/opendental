using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	/// <summary>Only used for medication reconcile to pick the scanned image of the list of medications that the patient brought in.</summary>
	public partial class FormImageSelect : FormODBase{
		///<summary>Don't show pdfs.</summary>
		public bool OnlyShowImages;
		public long PatNum;
		private List<Document> _listDocuments;
		///<summary>If DialogResult==OK, then this will contain the new DocNum of the image we want.</summary>
		public long SelectedDocNum;

		///<summary></summary>
		public FormImageSelect()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormImageSelect_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Date"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Category"),120);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),300);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			_listDocuments=Documents.GetAllWithPat(PatNum).ToList();
			if(OnlyShowImages){
				List<Document> listDocuments=new List<Document>();
				for(int i=0;i<_listDocuments.Count;i++) {
					if(ImageStore.HasImageExtension(_listDocuments[i].FileName)) {
						listDocuments.Add(_listDocuments[i]);
					}
				}
				_listDocuments=listDocuments;
			}
			for(int i=0;i<_listDocuments.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listDocuments[i].DateCreated.ToString());
				row.Cells.Add(Defs.GetName(DefCat.ImageCats,_listDocuments[i].DocCategory));
			  row.Cells.Add(_listDocuments[i].Description);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			SelectedDocNum=_listDocuments[e.Row].DocNum;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select an image first.");
				return;
			}
			SelectedDocNum=_listDocuments[gridMain.GetSelectedIndex()].DocNum;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		


	}
}





















