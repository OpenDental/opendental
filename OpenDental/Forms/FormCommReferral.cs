using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormCommReferral:FormODBase {
		public Commlog CommlogCur;
		private Commlog _commlogOld;

		public FormCommReferral() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormCommReferral_Load(object sender,EventArgs e) {
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

		private void butSave_Click(object sender,EventArgs e) {
			if(String.IsNullOrEmpty(textNote.Text)) {//Don't bother saving if there's nothing in the note field.
				DialogResult=DialogResult.Cancel;
				return;
			}
			//Decide CommReferralBehavior:
			//If user selects Anchored, set it.
			//If user selects Anchored and Hidden, hide instead. If it's Hidden, we don't care if it's Anchored.
			CommlogCur.CommReferralBehavior=EnumCommReferralBehavior.None;//It's not Anchored and it's not Hidden.
			if(checkAnchored.Checked) {
				CommlogCur.CommReferralBehavior=EnumCommReferralBehavior.TopAnchored;
			}
			if(checkHidden.Checked) {
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
			DialogResult=DialogResult.OK;
		}

	}
}