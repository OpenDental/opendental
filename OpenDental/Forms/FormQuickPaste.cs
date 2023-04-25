using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using OpenDental.UI;
using System.Linq;
using CodeBase;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormQuickPaste : FormODBase {
		private List<QuickPasteNote> _listQuickPasteNotes;
		private List<QuickPasteNote> _listQuickPasteNotesOld;
		private List<QuickPasteCat> _listQuickPasteCats;
		private List<QuickPasteCat> _listQuickPasteCatsOld;
		//<summary>This is the note that gets passed back to the calling function.</summary>
		//public string SelectedNote;
		///<summary>Set this property before calling this form. It will insert the value into this textbox.</summary>
		private bool _hasChanges;
		private bool _isSetupMode;
		public RichTextBox RichTextBox_;
		///<summary></summary>
		public QuickPasteType QuickPasteType_;

		///<summary></summary>
		public FormQuickPaste(bool isSetupMode){
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			_isSetupMode=isSetupMode;
			Lan.F(this);
		}

		private void FormQuickPaste_Load(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.AutoNoteQuickNoteEdit,true)) {
				butAddCat.Enabled=false;
				butDeleteCat.Enabled=false;
				butAddNote.Enabled=false;
				butUpCat.Enabled=false;
				butUpNote.Enabled=false;
				butDownCat.Enabled=false;
				butDownNote.Enabled=false;
				butAlphabetize.Enabled=false;
				butEditNote.Text="View";
			}
			if(RichTextBox_==null) {
				butDate.Visible=false;
			}
			_listQuickPasteCats=QuickPasteCats.GetDeepCopy();
			_listQuickPasteCatsOld=_listQuickPasteCats.Select(x=>x.Copy()).ToList();
			FillCats();
			listCat.SelectedIndex=QuickPasteCats.GetDefaultType(QuickPasteType_);
			_listQuickPasteNotes=QuickPasteNotes.GetDeepCopy();
			_listQuickPasteNotesOld=_listQuickPasteNotes.Select(x=>x.Copy()).ToList();
			FillMain();
		}

		private void FillCats() {
			int selectedIndex=listCat.SelectedIndex;
			listCat.Items.Clear();
			listCat.Items.AddList(_listQuickPasteCats,x => x.Description);
			if(selectedIndex<listCat.Items.Count) {
				listCat.SelectedIndex=selectedIndex;
			}
			if(listCat.SelectedIndex==-1) {
				listCat.SelectedIndex=listCat.Items.Count-1;
			}
		}

		private void FillMain() {
			int selectedIndex=gridMain.GetSelectedIndex();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new GridColumn("Abbr",75));
			gridMain.Columns.Add(new GridColumn("Note",600));
			gridMain.ListGridRows.Clear();
			if(listCat.SelectedIndex==-1) {
				gridMain.EndUpdate();
				return;
			}
			GridRow row;
			_listQuickPasteNotes=_listQuickPasteNotes.OrderBy(x => x.ItemOrder).ToList();
			for(int i=0;i<_listQuickPasteNotes.Count;i++) {
				if(_listQuickPasteNotes[i].QuickPasteCatNum!=_listQuickPasteCats[listCat.SelectedIndex].QuickPasteCatNum) {
					continue;
				}
				row=new GridRow();
				if(string.IsNullOrWhiteSpace(_listQuickPasteNotes[i].Abbreviation)) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add("?"+_listQuickPasteNotes[i].Abbreviation);
				}
				row.Cells.Add(StringTools.Truncate(_listQuickPasteNotes[i].Note.Replace("\r","").Replace("\n",""),120,true));
				row.Tag=_listQuickPasteNotes[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			if(selectedIndex==-1) {//Select the last option.
				gridMain.SetSelected(gridMain.ListGridRows.Count-1,true);
				gridMain.ScrollToEnd();
				return;
			}
			if(selectedIndex>=gridMain.ListGridRows.Count) {
				return;
			}
			//Select the previously selected position.
			gridMain.SetSelected(selectedIndex,true);
			
		}

		private void butAddCat_Click(object sender, System.EventArgs e) {
			QuickPasteCat quickPasteCat=new QuickPasteCat();
			using FormQuickPasteCat formQuickPasteCat=new FormQuickPasteCat(quickPasteCat);
			formQuickPasteCat.ShowDialog();
			if(formQuickPasteCat.DialogResult!=DialogResult.OK) {
				return;
			}
			quickPasteCat=formQuickPasteCat.QuickPasteCatCur;
			QuickPasteCats.Insert(quickPasteCat);
			_hasChanges=true;
			//We are doing this so tha when the sync is called in FormQuickPaste_FormClosing(...) we do not re-insert.
			//For now the sync will still detect a change due to the item orders.
			_listQuickPasteCatsOld.Add(quickPasteCat.Copy());
			if(listCat.SelectedIndex!=-1) {
				_listQuickPasteCats.Insert(listCat.SelectedIndex,quickPasteCat);//insert at selectedindex AND selects new category when we refill grid below.
			}
			else {//Will only happen if they do not have any categories.
				_listQuickPasteCats.Add(quickPasteCat);//add to bottom of list, will be selected when we fill grid below.
			}
			FillCats();
			FillMain();
		}

		private void butDeleteCat_Click(object sender,System.EventArgs e) {
			if(listCat.SelectedIndex==-1) {
				MessageBox.Show(Lan.g(this,"Please select a category first."));
				return;
			}
			if(MessageBox.Show(Lan.g(this,"Are you sure you want to delete the entire category and all notes in it?"),"",MessageBoxButtons.OKCancel)!=DialogResult.OK){
				return;
			}
			QuickPasteCat quickPasteCat=_listQuickPasteCats[listCat.SelectedIndex];
			_listQuickPasteCats.Remove(quickPasteCat);
			_listQuickPasteNotes.RemoveAll(x=>x.QuickPasteCatNum==quickPasteCat.QuickPasteCatNum);
			FillCats();
			FillMain();
		}

		private void butUpCat_Click(object sender,System.EventArgs e) {
			if(listCat.SelectedIndex==-1) {
				MessageBox.Show(Lan.g(this,"Please select a category first."));
				return;
			}
			if(listCat.SelectedIndex==0) {
				return;//can't go up any more
			}
			_listQuickPasteCats.Reverse(listCat.SelectedIndex-1,2);
			listCat.SelectedIndex--;
			FillCats();
			FillMain();
		}

		private void butDownCat_Click(object sender,System.EventArgs e) {
			if(listCat.SelectedIndex==-1) {
				MessageBox.Show(Lan.g(this,"Please select a category first."));
				return;
			}
			if(listCat.SelectedIndex==_listQuickPasteCats.Count-1) {
				return;//can't go down any more
			}
			_listQuickPasteCats.Reverse(listCat.SelectedIndex,2);
			listCat.SelectedIndex++;
			FillCats();
			FillMain();
		}

		private void listCat_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			FillMain();
		}

		private void listCat_DoubleClick(object sender,System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.AutoNoteQuickNoteEdit)) {
				return;
			}
			if(listCat.SelectedIndex==-1) {
				return;
			}
			using FormQuickPasteCat formQuickPasteCat=new FormQuickPasteCat(_listQuickPasteCats[listCat.SelectedIndex]);
			formQuickPasteCat.ShowDialog();
			if(formQuickPasteCat.DialogResult!=DialogResult.OK) {
				return;
			}
			_listQuickPasteCats[listCat.SelectedIndex]=formQuickPasteCat.QuickPasteCatCur;
			FillCats();
			FillMain();
		}

		private void butAddNote_Click(object sender,System.EventArgs e) {
			if(listCat.SelectedIndex==-1) {
				MessageBox.Show(Lan.g(this,"Please select a category first."));
				return;
			}
			QuickPasteNote quickPasteNote=new QuickPasteNote();
			quickPasteNote.QuickPasteCatNum=_listQuickPasteCats[listCat.SelectedIndex].QuickPasteCatNum;
			using FormQuickPasteNoteEdit formQuickPasteNoteEdit=new FormQuickPasteNoteEdit(quickPasteNote);
			formQuickPasteNoteEdit.ShowDialog();
			if(formQuickPasteNoteEdit.DialogResult!=DialogResult.OK || formQuickPasteNoteEdit.QuickPasteNoteCur==null) {//Deleted
				return;
			}
			if(gridMain.GetSelectedIndex()==-1) {
				_listQuickPasteNotes.Add(formQuickPasteNoteEdit.QuickPasteNoteCur);
				FillMain();
				return;
			}
			int selectedIndex=_listQuickPasteNotes.IndexOf((QuickPasteNote)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag);
			_listQuickPasteNotes.Insert(selectedIndex,formQuickPasteNoteEdit.QuickPasteNoteCur);//Insert the new note at the selected index.
			FillMain();
		}

		private void butEditNote_Click(object sender,System.EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MessageBox.Show(Lan.g(this,"Please select a note first."));
				return;
			}
			QuickPasteNote quickPasteNote=(QuickPasteNote)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			ShowEditWindow(quickPasteNote);
		}

		private void ShowEditWindow(QuickPasteNote quickPasteNote) {
			using FormQuickPasteNoteEdit formQuickPasteNoteEdit=new FormQuickPasteNoteEdit((QuickPasteNote)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag);
			formQuickPasteNoteEdit.ShowDialog();
			if(formQuickPasteNoteEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			if(formQuickPasteNoteEdit.QuickPasteNoteCur==null) {//deleted
				_listQuickPasteNotes.Remove(quickPasteNote);
			}
			else {
				_listQuickPasteNotes[_listQuickPasteNotes.IndexOf(quickPasteNote)]=formQuickPasteNoteEdit.QuickPasteNoteCur;
			}
			FillMain();
		}

		private void butUpNote_Click(object sender, System.EventArgs e) {
			MoveNote(false);
		}

		private void butDownNote_Click(object sender, System.EventArgs e) {
			MoveNote(true);
		}

		/// <summary>Moves the selected note either down or up in the grid and sets the itemOrder for each.</summary>
		private void MoveNote(bool isDown) {
			int selectedIndex=gridMain.GetSelectedIndex();
			int destinationIndex=(selectedIndex+(isDown?1:-1));
			if(selectedIndex==-1) {
				MessageBox.Show(Lan.g(this,"Please select a note first."));
				return;
			}
			if(!destinationIndex.Between(0,gridMain.ListGridRows.Count-1)) {
				return;//can't go up or down any more
			}
			QuickPasteNote quickPasteNoteSource=(QuickPasteNote)gridMain.ListGridRows[selectedIndex].Tag;
			QuickPasteNote quickPasteNoteDestination=(QuickPasteNote)gridMain.ListGridRows[destinationIndex].Tag;
			quickPasteNoteSource.ItemOrder=_listQuickPasteNotes.IndexOf(quickPasteNoteDestination);
			quickPasteNoteDestination.ItemOrder=_listQuickPasteNotes.IndexOf(quickPasteNoteSource);
			gridMain.SetAll(false);
			FillMain();
			gridMain.SetSelected(destinationIndex,true);
		}

		private void butAlphabetize_Click(object sender,EventArgs e) {
			//Since this string is hardcoded we can pass them into the MsgBox for translation even with the variable
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Quick Paste Notes will be ordered alphabetically by "
				+(radioSortByAbbrev.Checked?"abbreviation":"note")
				+".  This cannot be undone.  Continue?")) 
			{
				return;
			}
			List<QuickPasteNote> listQuickPasteNotes=_listQuickPasteNotes.FindAll(x => x.QuickPasteCatNum==_listQuickPasteCats[listCat.SelectedIndex].QuickPasteCatNum);
			if(radioSortByAbbrev.Checked) {
				listQuickPasteNotes=listQuickPasteNotes.OrderBy(x => x.Abbreviation).ToList();
			}
			else {
				listQuickPasteNotes=listQuickPasteNotes.OrderBy(x => x.Note).ToList();
			}
			List<int> listIndices=listQuickPasteNotes.Select(x => x.ItemOrder).OrderBy(x => x).ToList();
			for(int i=0;i<listQuickPasteNotes.Count;i++) {
				listQuickPasteNotes[i].ItemOrder=listIndices[i];//lists are 1:1
			}
			_hasChanges=true;
			FillMain();
		}

		private void gridMain_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			if(_isSetupMode) {
				QuickPasteNote quickPasteNote=(QuickPasteNote)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
				ShowEditWindow(quickPasteNote);
				return;
			}
			InsertValue(((QuickPasteNote)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag).Note);
			DialogResult=DialogResult.OK;
			
		}

		private void butDate_Click(object sender, System.EventArgs e) {
			InsertValue(DateTime.Today.ToShortDateString());
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(RichTextBox_==null){
				DialogResult=DialogResult.OK;
				return;
			}
			if(gridMain.GetSelectedIndex()==-1) {
				MessageBox.Show(Lan.g(this,"Please select a note first."));
				return;
			}
			InsertValue(((QuickPasteNote)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag).Note);
			DialogResult=DialogResult.OK;
		}

		private void InsertValue(string strPaste) {
			if(RichTextBox_==null) {
				return;
			}
			try {
				//When trying to paste plain text into the Rtf text, an exception will throw.
				RichTextBox_.SelectedRtf=strPaste;
				return;
			}
			catch{
				//If we couldn't paste into the Rtf text, try to paste into the plain text section.
			}
			try {
				RichTextBox_.SelectedText=strPaste;
			}
			catch{
				//If pasting into the Rtf AND the plain text fails, notify the user.
				MsgBox.Show(this,"There was a problem pasting clipboard contents.");
			}
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormQuickPaste_FormClosing(object sender,FormClosingEventArgs e) {
			for(int i=0;i<_listQuickPasteNotes.Count;i++) {
				_listQuickPasteNotes[i].ItemOrder=i;//Fix item orders.
			}
			for(int i=0;i<_listQuickPasteCats.Count;i++) {
				_listQuickPasteCats[i].ItemOrder=i;//Fix item orders.
			}
			_hasChanges|=QuickPasteCats.Sync(_listQuickPasteCats,_listQuickPasteCatsOld);
			if(QuickPasteNotes.Sync(_listQuickPasteNotes,_listQuickPasteNotesOld) || _hasChanges) {
				SecurityLogs.MakeLogEntry(Permissions.AutoNoteQuickNoteEdit,0,"Quick Paste Notes/Cats Changed");
				DataValid.SetInvalid(InvalidType.QuickPaste);
			}
		}

	}
}





















