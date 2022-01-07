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
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormQuickPaste : FormODBase {
		private List<QuickPasteNote> _listNotes;
		private List<QuickPasteNote> _listNotesOld;
		private List<QuickPasteCat> _listCats;
		private List<QuickPasteCat> _listCatsOld;
		//<summary>This is the note that gets passed back to the calling function.</summary>
		//public string SelectedNote;
		///<summary>Set this property before calling this form. It will insert the value into this textbox.</summary>
		private bool _hasChanges;
		private bool _isSetupMode;
		public RichTextBox TextToFill;

		///<summary></summary>
		public QuickPasteType QuickType;

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
			if(TextToFill==null) {
				butDate.Visible=false;
			}
			_listCats=QuickPasteCats.GetDeepCopy();
			_listCatsOld=_listCats.Select(x=>x.Copy()).ToList();
			FillCats();
			listCat.SelectedIndex=QuickPasteCats.GetDefaultType(QuickType);
			_listNotes=QuickPasteNotes.GetDeepCopy();
			_listNotesOld=_listNotes.Select(x=>x.Copy()).ToList();
			FillMain();
		}

		private void FillCats() {
			int selected=listCat.SelectedIndex;
			listCat.Items.Clear();
			listCat.Items.AddList(_listCats,x => x.Description);
			if(selected<listCat.Items.Count) {
				listCat.SelectedIndex=selected;
			}
			if(listCat.SelectedIndex==-1) {
				listCat.SelectedIndex=listCat.Items.Count-1;
			}
		}

		private void FillMain() {
			int selectedIdx=gridMain.GetSelectedIndex();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn("Abbr",75));
			gridMain.ListGridColumns.Add(new GridColumn("Note",600));
			gridMain.ListGridRows.Clear();
			if(listCat.SelectedIndex==-1) {
				gridMain.EndUpdate();
				return;
			}
			GridRow row;
			_listNotes=_listNotes.OrderBy(x => x.ItemOrder).ToList();
			foreach(QuickPasteNote note in _listNotes) {
				if(note.QuickPasteCatNum!=_listCats[listCat.SelectedIndex].QuickPasteCatNum) {
					continue;
				}
				row=new GridRow();
				row.Cells.Add(string.IsNullOrWhiteSpace(note.Abbreviation)?"":"?"+note.Abbreviation);
				row.Cells.Add(StringTools.Truncate(note.Note.Replace("\r","").Replace("\n",""),120,true));
				row.Tag=note;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			if(selectedIdx==-1) {//Select the last option.
				gridMain.SetSelected(gridMain.ListGridRows.Count-1,true);
				gridMain.ScrollToEnd();
			}
			else if(selectedIdx<gridMain.ListGridRows.Count) {//Select the previously selected position.
				gridMain.SetSelected(selectedIdx,true);
			}
		}

		private void butAddCat_Click(object sender, System.EventArgs e) {
			QuickPasteCat quickCat=new QuickPasteCat();
      using FormQuickPasteCat FormQ=new FormQuickPasteCat(quickCat);
			FormQ.ShowDialog();
			if(FormQ.DialogResult!=DialogResult.OK) {
				return;
			}
			quickCat=FormQ.QuickCat;
			QuickPasteCats.Insert(quickCat);
			_hasChanges=true;
			//We are doing this so tha when the sync is called in FormQuickPaste_FormClosing(...) we do not re-insert.
			//For now the sync will still detect a change due to the item orders.
			_listCatsOld.Add(quickCat.Copy());
			if(listCat.SelectedIndex!=-1) {
				_listCats.Insert(listCat.SelectedIndex,quickCat);//insert at selectedindex AND selects new category when we refill grid below.
			}
			else {//Will only happen if they do not have any categories.
				_listCats.Add(quickCat);//add to bottom of list, will be selected when we fill grid below.
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
			QuickPasteCat category=_listCats[listCat.SelectedIndex];
			_listCats.Remove(category);
			_listNotes.RemoveAll(x=>x.QuickPasteCatNum==category.QuickPasteCatNum);
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
			_listCats.Reverse(listCat.SelectedIndex-1,2);
			listCat.SelectedIndex--;
			FillCats();
			FillMain();
		}

		private void butDownCat_Click(object sender,System.EventArgs e) {
			if(listCat.SelectedIndex==-1) {
				MessageBox.Show(Lan.g(this,"Please select a category first."));
				return;
			}
			if(listCat.SelectedIndex==_listCats.Count-1) {
				return;//can't go down any more
			}
			_listCats.Reverse(listCat.SelectedIndex,2);
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
			using FormQuickPasteCat FormQ=new FormQuickPasteCat(_listCats[listCat.SelectedIndex]);
			FormQ.ShowDialog();
			if(FormQ.DialogResult!=DialogResult.OK) {
				return;
			}
			_listCats[listCat.SelectedIndex]=FormQ.QuickCat;
			FillCats();
			FillMain();
		}

		private void butAddNote_Click(object sender,System.EventArgs e) {
			if(listCat.SelectedIndex==-1) {
				MessageBox.Show(Lan.g(this,"Please select a category first."));
				return;
			}
			QuickPasteNote quickNote=new QuickPasteNote();
			quickNote.QuickPasteCatNum=_listCats[listCat.SelectedIndex].QuickPasteCatNum;
			using FormQuickPasteNoteEdit FormQ=new FormQuickPasteNoteEdit(quickNote);
			FormQ.ShowDialog();
			if(FormQ.DialogResult!=DialogResult.OK || FormQ.QuickNote==null) {//Deleted
				return;
			}
			if(gridMain.GetSelectedIndex()==-1) {
				_listNotes.Add(FormQ.QuickNote);
			}
			else { 
				int selectedIdx=_listNotes.IndexOf((QuickPasteNote)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag);
				_listNotes.Insert(selectedIdx,FormQ.QuickNote);//Insert the new note at the selected index.
			}			
			FillMain();
		}

		private void butEditNote_Click(object sender,System.EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MessageBox.Show(Lan.g(this,"Please select a note first."));
				return;
			}
			QuickPasteNote quickNote=(QuickPasteNote)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			ShowEditWindow(quickNote);
		}

		private void ShowEditWindow(QuickPasteNote quickNote) {
			using FormQuickPasteNoteEdit FormQ=new FormQuickPasteNoteEdit((QuickPasteNote)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag);
			FormQ.ShowDialog();
			if(FormQ.DialogResult!=DialogResult.OK) {
				return;
			}
			if(FormQ.QuickNote==null) {//deleted
				_listNotes.Remove(quickNote);
			}
			else { 
				_listNotes[_listNotes.IndexOf(quickNote)]=FormQ.QuickNote;
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
			int selectedIdx=gridMain.GetSelectedIndex();
			int destinationIdx=(selectedIdx+(isDown?1:-1));
			if(selectedIdx==-1) {
				MessageBox.Show(Lan.g(this,"Please select a note first."));
				return;
			}
			if(!destinationIdx.Between(0,gridMain.ListGridRows.Count-1)) {
				return;//can't go up or down any more
			}
			QuickPasteNote sourceNote=(QuickPasteNote)gridMain.ListGridRows[selectedIdx].Tag;
			QuickPasteNote destNote=(QuickPasteNote)gridMain.ListGridRows[destinationIdx].Tag;
			sourceNote.ItemOrder=_listNotes.IndexOf(destNote);
			destNote.ItemOrder=_listNotes.IndexOf(sourceNote);
			gridMain.SetAll(false);
			FillMain();
			gridMain.SetSelected(destinationIdx,true);
		}

		private void butAlphabetize_Click(object sender,EventArgs e) {
			//Since this string is hardcoded we can pass them into the MsgBox for translation even with the variable
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Quick Paste Notes will be ordered alphabetically by "
				+(radioSortByAbbrev.Checked?"abbreviation":"note")
				+".  This cannot be undone.  Continue?")) 
			{
				return;
			}
			List<QuickPasteNote> listNotesForCat=_listNotes.FindAll(x => x.QuickPasteCatNum==_listCats[listCat.SelectedIndex].QuickPasteCatNum);
			if(radioSortByAbbrev.Checked) {
				listNotesForCat=listNotesForCat.OrderBy(x => x.Abbreviation).ToList();
			}
			else {
				listNotesForCat=listNotesForCat.OrderBy(x => x.Note).ToList();
			}
			List<int> listIndices=listNotesForCat.Select(x => x.ItemOrder).OrderBy(x => x).ToList();
			for(int i=0;i<listNotesForCat.Count;i++) {
				listNotesForCat[i].ItemOrder=listIndices[i];//lists are 1:1
			}
			_hasChanges=true;
			FillMain();
		}

		private void gridMain_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			if(_isSetupMode) {
				QuickPasteNote quickNote=(QuickPasteNote)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
				ShowEditWindow(quickNote);
			}
			else {
				InsertValue(((QuickPasteNote)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag).Note);
				DialogResult=DialogResult.OK;
			}
		}

		private void butDate_Click(object sender, System.EventArgs e) {
			InsertValue(DateTime.Today.ToShortDateString());
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(TextToFill!=null) {
				if(gridMain.GetSelectedIndex()==-1) {
					MessageBox.Show(Lan.g(this,"Please select a note first."));
					return;
				}
				InsertValue(((QuickPasteNote)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag).Note);
			}
			DialogResult=DialogResult.OK;
		}

		private void InsertValue(string strPaste) {
			if(TextToFill==null) {
				return;
			}
			try {
				//When trying to paste plain text into the Rtf text, an exception will throw.
				TextToFill.SelectedRtf=strPaste;
			}
			catch(Exception) {
				//If we couldn't paste into the Rtf text, try to paste into the plain text section.
				try {
					TextToFill.SelectedText=strPaste;
				}
				catch(Exception) {
					//If pasting into the Rtf AND the plain text fails, notify the user.
					MsgBox.Show(this,"There was a problem pasting clipboard contents.");
				}
			}
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormQuickPaste_FormClosing(object sender,FormClosingEventArgs e) {
			for(int i=0;i<_listNotes.Count;i++) {
				_listNotes[i].ItemOrder=i;//Fix item orders.
			}
			for(int i=0;i<_listCats.Count;i++) {
				_listCats[i].ItemOrder=i;//Fix item orders.
			}
			_hasChanges|=QuickPasteCats.Sync(_listCats,_listCatsOld);
			if(QuickPasteNotes.Sync(_listNotes,_listNotesOld) || _hasChanges) {
				SecurityLogs.MakeLogEntry(Permissions.AutoNoteQuickNoteEdit,0,"Quick Paste Notes/Cats Changed");
				DataValid.SetInvalid(InvalidType.QuickPaste);
			}
		}

	}
}





















