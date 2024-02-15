using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CodeBase;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	/// <summary></summary>
	public partial class FrmQuickPaste : FrmODBase {
		private List<QuickPasteNote> _listQuickPasteNotes;
		private List<QuickPasteNote> _listQuickPasteNotesOld;
		private List<QuickPasteCat> _listQuickPasteCats;
		private List<QuickPasteCat> _listQuickPasteCatsOld;
		//<summary>This is the note that gets passed back to the calling function.</summary>
		//public string SelectedNote;
		///<summary>Set this property before calling this form. It will insert the value into this textbox.</summary>
		private bool _hasChanges;
		public bool IsSelectionMode;
		///<summary></summary>
		public EnumQuickPasteType QuickPasteType_;
		///<summary>The reason this is an action is because there are two different event handlers, depending on whether we are calling from ODTextBox (WinForms) or TextRich (WPF).</summary>
		public Action<string> ActionInsertVal;

		///<summary></summary>
		public FrmQuickPaste(){
			InitializeComponent();
			Load+=FormQuickPaste_Load;
			gridMain.CellDoubleClick+=gridMain_CellDoubleClick;
			listCat.MouseDown+=listCat_MouseDown;
			listCat.MouseDoubleClick+=listCat_DoubleClick;
			FormClosing+=FrmQuickPaste_FormClosing;
			PreviewKeyDown+=FrmQuickPaste_PreviewKeyDown;
		}

		private void FormQuickPaste_Load(object sender, EventArgs e) {
			Lang.F(this);
			if(!Security.IsAuthorized(EnumPermType.AutoNoteQuickNoteEdit,true) || IsSelectionMode) {
				butAddCat.IsEnabled=false;
				butDeleteCat.IsEnabled=false;
				butAddNote.IsEnabled=false;
				butUpCat.IsEnabled=false;
				butUpNote.IsEnabled=false;
				butDownCat.IsEnabled=false;
				butDownNote.IsEnabled=false;
				butAlphabetize.IsEnabled=false;
				butEditNote.Text="View";
			}
			if(IsSelectionMode) {
				butSave.Text="OK";
			}
			else{
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
			FrmQuickPasteCat frmQuickPasteCat=new FrmQuickPasteCat(quickPasteCat);
			frmQuickPasteCat.ShowDialog();
			if(frmQuickPasteCat.IsDialogOK==false) {
				return;
			}
			quickPasteCat=frmQuickPasteCat.QuickPasteCatCur;
			QuickPasteCats.Insert(quickPasteCat);
			_hasChanges=true;
			//We are doing this so that when the sync is called in FormQuickPaste_FormClosing(...) we do not re-insert.
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
				MessageBox.Show(Lans.g(this,"Please select a category first."));
				return;
			}
			if(MessageBox.Show(Lans.g(this,"Are you sure you want to delete the entire category and all notes in it?"),"",MessageBoxButton.OKCancel)!=MessageBoxResult.OK){
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
				MessageBox.Show(Lans.g(this,"Please select a category first."));
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
				MessageBox.Show(Lans.g(this,"Please select a category first."));
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

		private void listCat_MouseDown(object sender,MouseEventArgs e) {
			FillMain();
		}

		private void listCat_DoubleClick(object sender,System.EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.AutoNoteQuickNoteEdit)) {
				return;
			}
			if(listCat.SelectedIndex==-1) {
				return;
			}
			if(IsSelectionMode){
				return;
			}
			FrmQuickPasteCat frmQuickPasteCat=new FrmQuickPasteCat(_listQuickPasteCats[listCat.SelectedIndex]);
			frmQuickPasteCat.ShowDialog();
			if(frmQuickPasteCat.IsDialogOK==false) {
				return;
			}
			_listQuickPasteCats[listCat.SelectedIndex]=frmQuickPasteCat.QuickPasteCatCur;
			FillCats();
			FillMain();
		}

		private void butAddNote_Click(object sender,System.EventArgs e) {
			if(listCat.SelectedIndex==-1) {
				MessageBox.Show(Lans.g(this,"Please select a category first."));
				return;
			}
			QuickPasteNote quickPasteNote=new QuickPasteNote();
			quickPasteNote.QuickPasteCatNum=_listQuickPasteCats[listCat.SelectedIndex].QuickPasteCatNum;
			FrmQuickPasteNoteEdit frmQuickPasteNoteEdit=new FrmQuickPasteNoteEdit(quickPasteNote);
			frmQuickPasteNoteEdit.IsReadOnly=IsSelectionMode;
			frmQuickPasteNoteEdit.ShowDialog();
			if(frmQuickPasteNoteEdit.IsDialogOK==false || frmQuickPasteNoteEdit.QuickPasteNoteCur==null) {//Deleted
				return;
			}
			if(gridMain.GetSelectedIndex()==-1) {
				_listQuickPasteNotes.Add(frmQuickPasteNoteEdit.QuickPasteNoteCur);
				FillMain();
				return;
			}
			int selectedIndex=_listQuickPasteNotes.IndexOf((QuickPasteNote)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag);
			_listQuickPasteNotes.Insert(selectedIndex,frmQuickPasteNoteEdit.QuickPasteNoteCur);//Insert the new note at the selected index.
			FillMain();
		}

		private void butEditNote_Click(object sender,System.EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MessageBox.Show(Lans.g(this,"Please select a note first."));
				return;
			}
			QuickPasteNote quickPasteNote=(QuickPasteNote)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			ShowEditWindow(quickPasteNote);
		}

		private void ShowEditWindow(QuickPasteNote quickPasteNote) {
			FrmQuickPasteNoteEdit frmQuickPasteNoteEdit=new FrmQuickPasteNoteEdit((QuickPasteNote)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag);
			frmQuickPasteNoteEdit.IsReadOnly=IsSelectionMode;
			frmQuickPasteNoteEdit.ShowDialog();
			if(frmQuickPasteNoteEdit.IsDialogOK==false) {
				return;
			}
			if(frmQuickPasteNoteEdit.QuickPasteNoteCur==null) {//deleted
				_listQuickPasteNotes.Remove(quickPasteNote);
			}
			else {
				_listQuickPasteNotes[_listQuickPasteNotes.IndexOf(quickPasteNote)]=frmQuickPasteNoteEdit.QuickPasteNoteCur;
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
				MessageBox.Show(Lans.g(this,"Please select a note first."));
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

		private void gridMain_CellDoubleClick(object sender,GridClickEventArgs e) {
			if(IsSelectionMode){
				ActionInsertVal(((QuickPasteNote)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag).Note);
				IsDialogOK=true;
				return;
			}
			QuickPasteNote quickPasteNote=(QuickPasteNote)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			ShowEditWindow(quickPasteNote);
		}

		private void butDate_Click(object sender, System.EventArgs e) {
			ActionInsertVal(DateTime.Today.ToShortDateString());
			IsDialogOK=true;
		}

		private void FrmQuickPaste_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(IsSelectionMode){
				//this button says "OK"
				if(gridMain.GetSelectedIndex()==-1) {
					MessageBox.Show(Lans.g(this,"Please select a note first."));
					return;
				}
				ActionInsertVal(((QuickPasteNote)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag).Note);
				IsDialogOK=true;
				return;
			}
			//setup
			_hasChanges|=QuickPasteCats.Sync(_listQuickPasteCats,_listQuickPasteCatsOld);
			_hasChanges|=QuickPasteNotes.Sync(_listQuickPasteNotes,_listQuickPasteNotesOld);
			//orders will still be fixed separately in Closing
			IsDialogOK=true;
		}

		private void FrmQuickPaste_FormClosing(object sender,CancelEventArgs e) {
			//the only saving that gets done here is fixing item orders.
			//The old lists here are just for the itemorders.
			bool fixedItemOrder=false;
			_listQuickPasteNotesOld=_listQuickPasteNotes.Select(x=>x.Copy()).ToList();
			_listQuickPasteCatsOld=_listQuickPasteCats.Select(x=>x.Copy()).ToList();
			for(int i=0;i<_listQuickPasteNotes.Count;i++) {
				if(_listQuickPasteNotes[i].ItemOrder==i) {
					continue;
				}
				fixedItemOrder=true;
				_listQuickPasteNotes[i].ItemOrder=i;
				
			}
			for(int i=0;i<_listQuickPasteCats.Count;i++) {
				if(_listQuickPasteCats[i].ItemOrder==i) {
					continue;
				}
				fixedItemOrder=true;
				_listQuickPasteCats[i].ItemOrder=i;
			}
			if(fixedItemOrder) {
				_hasChanges|=QuickPasteCats.Sync(_listQuickPasteCats,_listQuickPasteCatsOld);
				_hasChanges|=QuickPasteNotes.Sync(_listQuickPasteNotes,_listQuickPasteNotesOld);
			}
			if(_hasChanges) {
				SecurityLogs.MakeLogEntry(EnumPermType.AutoNoteQuickNoteEdit,0,"Quick Paste Notes/Cats Changed");
				DataValid.SetInvalid(InvalidType.QuickPaste);
			}
		}

	}
}