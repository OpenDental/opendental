using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	/// <summary></summary>
	public partial class FormMountEdit : FormODBase {
		public Mount MountCur=null;
		List<Def> _listDefsImageCats;

		///<summary></summary>
		public FormMountEdit(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMountEdit_Load(object sender,EventArgs e) {
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
			butColorBack.BackColor=MountCur.ColorBack;
			butColorFore.BackColor=MountCur.ColorFore;
			butColorTextBack.BackColor=MountCur.ColorTextBack;
			if(MountCur.ColorTextBack.ToArgb()==Color.Transparent.ToArgb()){
				checkTransparent.Checked=true;
				butColorTextBack.BackColor=MountCur.ColorBack;
			}
			checkFlipOnAcquire.Checked=MountCur.FlipOnAcquire;
			checkAdjModeAfterSeries.Checked=MountCur.AdjModeAfterSeries;
		}

		private void butLayout_Click(object sender,EventArgs e) {
			using FormMountLayoutEdit formMountLayoutEdit=new FormMountLayoutEdit();
			formMountLayoutEdit.MountCur=MountCur;
			formMountLayoutEdit.ShowDialog();
		}

		private void butColorBack_Click(object sender, System.EventArgs e) {
			using ColorDialog colorDialog=new ColorDialog();
			colorDialog.FullOpen=true;
			colorDialog.Color=butColorBack.BackColor;
			colorDialog.ShowDialog();
			butColorBack.BackColor=colorDialog.Color;
		}

		private void butColorFore_Click(object sender, System.EventArgs e) {
			using ColorDialog colorDialog=new ColorDialog();
			colorDialog.FullOpen=true;
			colorDialog.Color=butColorFore.BackColor;
			colorDialog.ShowDialog();
			butColorFore.BackColor=colorDialog.Color;
		}

		private void butColorTextBack_Click(object sender, System.EventArgs e) {
			using ColorDialog colorDialog=new ColorDialog();
			colorDialog.FullOpen=true;
			colorDialog.Color=butColorTextBack.BackColor;
			DialogResult dialogResult=colorDialog.ShowDialog();
			if(dialogResult!=DialogResult.OK){
				//if Transparent was checked, it can stay checked.
				return;
			}
			checkTransparent.Checked=false;
			butColorTextBack.BackColor=colorDialog.Color;
		}

		private void checkTransparent_Click(object sender,EventArgs e) {
			if(checkTransparent.Checked){
				butColorTextBack.BackColor=butColorBack.BackColor;//interpreted by user as transparent
			}
			else{
				//they can also do the same thing by editing the color, and the box will automatically uncheck.
				butColorTextBack.BackColor=butColorBack.BackColor;
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(listCategory.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a category.");
				return;
			}
			if(!textDate.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
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
			MountCur.ColorBack=butColorBack.BackColor;
			MountCur.ColorFore=butColorFore.BackColor;
			if(checkTransparent.Checked){
				MountCur.ColorTextBack=Color.Transparent;
			}
			else{
				MountCur.ColorTextBack=butColorTextBack.BackColor;
			}
			MountCur.FlipOnAcquire=checkFlipOnAcquire.Checked;
			MountCur.AdjModeAfterSeries=checkAdjModeAfterSeries.Checked;
			Mounts.Update(MountCur);
			Documents.UpdateDocCategoryForMountItems(MountCur.MountNum,MountCur.DocCategory);
			//new mounts are never added here, so it's never an insert
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}





















