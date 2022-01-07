using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormMountDefs : FormODBase {
		private bool changed;
		private List<MountDef> _listMountDefs;

		///<summary></summary>
		public FormMountDefs()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMountDefs_Load(object sender, System.EventArgs e) {
			FillList();
		}

		private void FillList(){
			MountDefs.RefreshCache();
			listBoxMain.Items.Clear();
			_listMountDefs=MountDefs.GetDeepCopy();
			for(int i=0;i<_listMountDefs.Count;i++){
				if(_listMountDefs[i].ItemOrder!=i){
					_listMountDefs[i].ItemOrder=i;
					MountDefs.Update(_listMountDefs[i]);
					changed=true;
				}
				listBoxMain.Items.Add(_listMountDefs[i].Description);
			}
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			MountDef mountDef=new MountDef();
			mountDef.IsNew=true;
			mountDef.Description="Mount";
			mountDef.Width=600;
			mountDef.Height=400;
			if(_listMountDefs.Count>0){
				mountDef.ItemOrder=_listMountDefs.Count;
			}
			MountDefs.Insert(mountDef);//Insert mount here instead of inside edit window so that we have an object to add items to
			using FormMountDefEdit formMountDefEdit=new FormMountDefEdit();
			formMountDefEdit.MountDefCur=mountDef;
			formMountDefEdit.ShowDialog();
			FillList();
			changed=true;
		}

		private void listMain_DoubleClick(object sender, System.EventArgs e) {
			if(listBoxMain.SelectedIndex==-1){
				return;
			}
			using FormMountDefEdit formMountDefEdit=new FormMountDefEdit();
			formMountDefEdit.MountDefCur=_listMountDefs[listBoxMain.SelectedIndex];
			formMountDefEdit.ShowDialog();
			FillList();
			changed=true;
		}

		private void butUp_Click(object sender,EventArgs e) {
			int selectedIdx=listBoxMain.SelectedIndex;
			if(selectedIdx==-1) {
				return;
			}
			if(selectedIdx==0) {//at top
				return;
			}
			MountDef mountDef=_listMountDefs[selectedIdx];
			mountDef.ItemOrder--;
			MountDefs.Update(mountDef);
			MountDef mountDefAbove=_listMountDefs[selectedIdx-1];
			mountDefAbove.ItemOrder++;
			MountDefs.Update(mountDefAbove);
			FillList();
			listBoxMain.SelectedIndex=selectedIdx-1;
			changed=true;
		}

		private void butDown_Click(object sender,EventArgs e) {
			int selectedIdx=listBoxMain.SelectedIndex;
			if(selectedIdx==-1) {
				return;
			}
			if(selectedIdx==_listMountDefs.Count-1) {//at bottom
				return;
			}
			MountDef mountDef=_listMountDefs[selectedIdx];
			mountDef.ItemOrder++;
			MountDefs.Update(mountDef);
			MountDef mountDefBelow=_listMountDefs[selectedIdx+1];
			mountDefBelow.ItemOrder--;
			MountDefs.Update(mountDefBelow);
			FillList();
			listBoxMain.SelectedIndex=selectedIdx+1;
			changed=true;
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormMounts_FormClosing(object sender,FormClosingEventArgs e) {
			if(changed) {
				DataValid.SetInvalid(InvalidType.ToolButsAndMounts);
			}
		}

		

		



		
	}
}





















