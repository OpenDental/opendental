using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormNotePick:FormODBase {
		///<summary></summary>
		private string[] Notes;
		///<summary>Upon closing with OK, this will be the final note to save.</summary>
		public string SelectedNote;
		public bool UseTrojanImportDescription;

		///<summary></summary>
		public FormNotePick(string[] notes)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			Notes=new string[notes.Length];
			notes.CopyTo(Notes,0);
			if(UseTrojanImportDescription) {
				label1.Text=Lan.g(this,"Multiple versions of the note exist.  Please pick or edit one version to retain. You can also pick multiple rows to combine notes.");
				label2.Text=Lan.g(this,"This is the final note that will be used.");
			}
		}

		private void FormNotePick_Load(object sender,EventArgs e) {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("",630);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<Notes.Length;i++) {
				row=new GridRow();
				row.Cells.Add(Notes[i]);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			if(Notes.Length>0){
				gridMain.SetSelected(0,true);
				textNote.Text=Notes[0];
			}
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			textNote.Text="";
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){
				textNote.Text+=Notes[gridMain.SelectedIndices[i]];
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			SelectedNote=Notes[e.Row];
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			SelectedNote=textNote.Text;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	

		

	


	}
}





















