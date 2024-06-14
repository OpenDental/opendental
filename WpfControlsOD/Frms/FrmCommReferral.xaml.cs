using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	public partial class FrmCommReferral:FrmODBase {
		public Commlog CommlogCur;
		private Commlog _commlogOld;

		public FrmCommReferral() {
			InitializeComponent();
			Load+=FrmCommReferral_Load;
			PreviewKeyDown+=FrmCommReferral_PreviewKeyDown;
		}

		private void FrmCommReferral_Load(object sender,EventArgs e) {
			Lang.F(this);
			_commlogOld=CommlogCur.Copy();
			textUser.Text=Userods.GetName(CommlogCur.UserNum);//Might be blank.
			if(CommlogCur.DateTEntry.Year>1880) {
				textDateTimeCreated.Text=CommlogCur.DateTEntry.ToShortDateString()+"  "+CommlogCur.DateTEntry.ToShortTimeString();
			}
			if(CommlogCur.CommReferralBehavior==EnumCommReferralBehavior.TopAnchored) {
				checkAnchored.Checked=true;
			}
			if(CommlogCur.CommReferralBehavior==EnumCommReferralBehavior.Hidden) {
				checkHidden.Checked=true;
			}
			listMode.Items.AddEnums<CommItemMode>();
			listSentOrReceived.Items.AddEnums<CommSentOrReceived>();
			listMode.SetSelected((int)CommlogCur.Mode_);
			listSentOrReceived.SetSelected((int)CommlogCur.SentOrReceived);
			textNote.Text=CommlogCur.Note;
			textNote.SelectionStart=textNote.Text.Length;
		}

		private void FrmCommReferral_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender,System.EventArgs e) {
			if(String.IsNullOrEmpty(textNote.Text)) {//Don't bother saving if there's nothing in the note field.
				IsDialogOK=false;
				return;
			}
			//Decide CommReferralBehavior:
			//If user selects Anchored, set it.
			//If user selects Anchored and Hidden, hide instead. If it's Hidden, we don't care if it's Anchored.
			CommlogCur.CommReferralBehavior=EnumCommReferralBehavior.None;//It's not Anchored and it's not Hidden.
			if(checkAnchored.Checked==true) {
				CommlogCur.CommReferralBehavior=EnumCommReferralBehavior.TopAnchored;
			}
			if(checkHidden.Checked==true) {
				CommlogCur.CommReferralBehavior=EnumCommReferralBehavior.Hidden;
			}
			CommlogCur.Mode_=listMode.GetSelected<CommItemMode>();
			CommlogCur.SentOrReceived=listSentOrReceived.GetSelected<CommSentOrReceived>();
			CommlogCur.Note=textNote.Text;
			if(CommlogCur.IsNew) {
				Commlogs.Insert(CommlogCur);
			}
			else {
				Commlogs.Update(CommlogCur,_commlogOld);
			}
			IsDialogOK=true;
		}

	}
}