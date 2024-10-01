using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	/// <summary> </summary>
	public partial class FrmQuickPasteNoteEdit : FrmODBase {
		public QuickPasteNote QuickPasteNoteCur;
		public bool IsReadOnly;

		///<summary></summary>
		public FrmQuickPasteNoteEdit(QuickPasteNote quickPasteNote){
			QuickPasteNoteCur=quickPasteNote.Copy();
			InitializeComponent();
			Load+=FrmQuickPasteNoteEdit_Load;
			textNote.TextChanged+=textNote_TextChanged;
			PreviewKeyDown+=FrmQuickPasteNoteEdit_PreviewKeyDown;
		}

		private void FrmQuickPasteNoteEdit_Load(object sender, EventArgs e) {
			Lang.F(this);
			if(!Security.IsAuthorized(EnumPermType.AutoNoteQuickNoteEdit,true) || IsReadOnly) {
				textAbbreviation.ReadOnly=true;
				textNote.ReadOnly=true;
				butDelete.IsEnabled=false;
				butSave.IsEnabled=false;
			}
			textAbbreviation.Text=QuickPasteNoteCur.Abbreviation;
			textNote.Text=QuickPasteNoteCur.Note;
			textNote.SelectAll();
			textAbbreviation.SelectAll();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(MessageBox.Show(Lans.g(this,"Delete note?"),"",MessageBoxButton.OKCancel)!=MessageBoxResult.OK){
				return;
			}
			QuickPasteNoteCur=null;//triggers an action in the calling form
			IsDialogOK=true;
		}

		private void FrmQuickPasteNoteEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(textAbbreviation.Text.Contains("?")) {
				MsgBox.Show(this,"Question mark not allowed in abbreviation.  Use the question mark later when trying to insert a quick note.");
				return;
			}
			if(textAbbreviation.Text.Contains("*")) {
				MsgBox.Show(this,"Asterisk character not allowed in abbreviation.  Use the asterisk later when trying to insert a quick note.");
				return;
			}
			QuickPasteNoteCur.Abbreviation=textAbbreviation.Text;
			if(QuickPasteNoteCur.Abbreviation!=""){
				string msgText=QuickPasteNotes.AbbrAlreadyInUse(QuickPasteNoteCur);
				if(!String.IsNullOrEmpty(msgText) && MessageBox.Show(msgText,Lans.g(this,"Warning"),MessageBoxButton.YesNo)==MessageBoxResult.No) {
					return;
				}
			}
			QuickPasteNoteCur.Note=textNote.Text;
			IsDialogOK=true;
		}

		private void textNote_TextChanged(object sender,EventArgs e) {
			//We do not normally use TextChanged but we need to perform maintenance of the data when it loads, is saved, or is pasted to.
			textNote.Text=textNote.Text.Replace("\r\n","\n")//convert windows to \n
				.Replace("\r","\n")									//replace linux or other \n
																			//Mac is already \n
				.Replace("\n","\r\n");								//reset to Windows newline character		
		}

	}
}