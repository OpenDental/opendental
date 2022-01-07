using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormMountEdit : FormODBase {
		public Mount MountCur=null;
		List<Def> _listDefNumsImageCats;

		///<summary></summary>
		public FormMountEdit(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMountEdit_Load(object sender,EventArgs e) {
			_listDefNumsImageCats=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			for(int i=0;i<_listDefNumsImageCats.Count;i++){
				listCategory.Items.Add(_listDefNumsImageCats[i].ItemName);
				if(_listDefNumsImageCats[i].DefNum==MountCur.DocCategory){
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
			colorDialog.ShowDialog();
			butColorTextBack.BackColor=colorDialog.Color;
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
			DateTime time;
			if(!DateTime.TryParse(textTime.Text,out time)) {
				MsgBox.Show(this,"Please enter a valid time.");
				return;
			}
			MountCur.DocCategory=_listDefNumsImageCats[listCategory.SelectedIndex].DefNum;
			DateTime dateTimeEntered=PIn.DateT(textDate.Text+" "+textTime.Text);
			MountCur.DateCreated=dateTimeEntered;	
			MountCur.ProvNum=comboProv.GetSelectedProvNum();
			MountCur.Description=textDescription.Text;
			MountCur.Note=textNote.Text;
			MountCur.ColorBack=butColorBack.BackColor;
			MountCur.ColorFore=butColorFore.BackColor;
			MountCur.ColorTextBack=butColorTextBack.BackColor;
			Mounts.Update(MountCur);
			//new mounts are never added here, so it's never an insert
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}





















