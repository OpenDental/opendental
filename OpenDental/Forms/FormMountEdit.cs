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
		private Mount _mountCur=null;
		List<Def> _listDefNumsImageCats;

		///<summary></summary>
		public FormMountEdit(Mount mount)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_mountCur=mount;
		}

		private void FormMountEdit_Load(object sender,EventArgs e) {
			_listDefNumsImageCats=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			for(int i=0;i<_listDefNumsImageCats.Count;i++){
				listCategory.Items.Add(_listDefNumsImageCats[i].ItemName);
				if(_listDefNumsImageCats[i].DefNum==_mountCur.DocCategory){
					listCategory.SelectedIndex=i;
				}
			}
			textDate.Text=_mountCur.DateCreated.ToShortDateString();
			textTime.Text=_mountCur.DateCreated.ToShortTimeString();
			comboProv.Items.AddProvNone();
			comboProv.Items.AddProvsAbbr(Providers.GetDeepCopy());
			comboProv.SetSelectedProvNum(_mountCur.ProvNum);
			textDescription.Text=_mountCur.Description;
			textNote.Text=_mountCur.Note;
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
			_mountCur.DocCategory=_listDefNumsImageCats[listCategory.SelectedIndex].DefNum;
			DateTime dateTimeEntered=PIn.DateT(textDate.Text+" "+textTime.Text);
			_mountCur.DateCreated=dateTimeEntered;	
			_mountCur.ProvNum=comboProv.GetSelectedProvNum();
			_mountCur.Description=textDescription.Text;
			_mountCur.Note=textNote.Text;
			Mounts.Update(_mountCur);
			//new mounts are never added here, so it's never an insert
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


	}
}





















