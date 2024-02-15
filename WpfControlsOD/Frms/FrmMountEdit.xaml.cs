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
	/// <summary></summary>
	public partial class FrmMountEdit : FrmODBase {
		public Mount MountCur=null;
		List<Def> _listDefsImageCats;

		///<summary></summary>
		public FrmMountEdit(){
			InitializeComponent();
			Load+=FrmMountEdit_Load;
			PreviewKeyDown+=FrmMountEdit_PreviewKeyDown;
		}

		private void FrmMountEdit_Load(object sender,EventArgs e) {
			Lang.F(this);
			_listDefsImageCats=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			for(int i=0;i<_listDefsImageCats.Count;i++){
				listCategory.Items.Add(_listDefsImageCats[i].ItemName);
				if(_listDefsImageCats[i].DefNum==MountCur.DocCategory){
					listCategory.SelectedIndex=i;
				}
			}
			textDate.Text=MountCur.DateCreated.ToShortDateString();
			textTime.Text=MountCur.DateCreated.ToShortTimeString();
			comboProv.Items.AddProvNone();
			comboProv.Items.AddProvsAbbr(Providers.GetDeepCopy());
			comboProv.SetSelectedProvNum(MountCur.ProvNum);
			textDescription.Text=MountCur.Description;
			textNote.Text=MountCur.Note;
			butColorBack.ColorBack=ColorOD.ToWpf(MountCur.ColorBack);
			butColorFore.ColorBack=ColorOD.ToWpf(MountCur.ColorFore);
			butColorTextBack.ColorBack=ColorOD.ToWpf(MountCur.ColorTextBack);
			if(MountCur.ColorTextBack.A==0){//Transparent
				checkTransparent.Checked=true;
				butColorTextBack.ColorBack=ColorOD.ToWpf(MountCur.ColorBack);
			}
			checkFlipOnAcquire.Checked=MountCur.FlipOnAcquire;
			checkAdjModeAfterSeries.Checked=MountCur.AdjModeAfterSeries;
		}

		private void butLayout_Click(object sender,EventArgs e) {
			FrmMountLayoutEdit frmMountLayoutEdit=new FrmMountLayoutEdit();
			frmMountLayoutEdit.MountCur=MountCur;
			frmMountLayoutEdit.ShowDialog();
		}

		private void butColorBack_Click(object sender, System.EventArgs e) {
			FrmColorDialog frmColorDialog=new FrmColorDialog();
			frmColorDialog.Color=butColorBack.ColorBack;
			frmColorDialog.ShowDialog();
			butColorBack.ColorBack=frmColorDialog.Color;
		}

		private void butColorFore_Click(object sender, System.EventArgs e) {
			FrmColorDialog frmColorDialog=new FrmColorDialog();
			frmColorDialog.Color=butColorFore.ColorBack;
			frmColorDialog.ShowDialog();
			butColorFore.ColorBack=frmColorDialog.Color;
		}

		private void butColorTextBack_Click(object sender, System.EventArgs e) {
			FrmColorDialog frmColorDialog=new FrmColorDialog();
			frmColorDialog.Color=butColorTextBack.ColorBack;
			frmColorDialog.ShowDialog();
			if(frmColorDialog.IsDialogCancel){
				//if Transparent was checked, it can stay checked.
				return;
			}
			checkTransparent.Checked=false;
			butColorTextBack.ColorBack=frmColorDialog.Color;
		}

		private void checkTransparent_Click(object sender,EventArgs e) {
			if(checkTransparent.Checked==true){
				butColorTextBack.ColorBack=butColorBack.ColorBack;//interpreted by user as transparent
			}
			else{
				//they can also do the same thing by editing the color, and the box will automatically uncheck.
				butColorTextBack.ColorBack=butColorBack.ColorBack;
			}
		}

		private void FrmMountEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(listCategory.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a category.");
				return;
			}
			if(!textDate.IsValid()) {
				MessageBox.Show(Lang.g(this,"Please fix data entry errors first."));
				return;
			}
			if(textDate.Text=="") {
				MsgBox.Show(this,"Please enter a date.");
				return;
			}
			if(textTime.Text=="") {
				MsgBox.Show(this,"Please enter a time.");
				return;
			}
			try {
				DateTime dateTime=DateTime.Parse(textTime.Text);
			}
			catch {
				MsgBox.Show(this,"Please enter a valid time.");
				return;
			}
			MountCur.DocCategory=_listDefsImageCats[listCategory.SelectedIndex].DefNum;
			DateTime dateTimeEntered=PIn.DateT(textDate.Text+" "+textTime.Text);
			MountCur.DateCreated=dateTimeEntered;	
			MountCur.ProvNum=comboProv.GetSelectedProvNum();
			MountCur.Description=textDescription.Text;
			MountCur.Note=textNote.Text;
			MountCur.ColorBack=ColorOD.FromWpf(butColorBack.ColorBack);
			MountCur.ColorFore=ColorOD.FromWpf(butColorFore.ColorBack);
			if(checkTransparent.Checked==true){
				MountCur.ColorTextBack=System.Drawing.Color.Transparent;
			}
			else{
				MountCur.ColorTextBack=ColorOD.FromWpf(butColorTextBack.ColorBack);
			}
			MountCur.FlipOnAcquire=checkFlipOnAcquire.Checked==true;
			MountCur.AdjModeAfterSeries=checkAdjModeAfterSeries.Checked==true;
			Mounts.Update(MountCur);
			Documents.UpdateDocCategoryForMountItems(MountCur.MountNum,MountCur.DocCategory);
			//new mounts are never added here, so it's never an insert
			IsDialogOK=true;
		}

	}
}