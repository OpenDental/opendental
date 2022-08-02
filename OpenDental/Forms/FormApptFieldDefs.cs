using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Collections.Generic;
using System.Linq;

namespace OpenDental{
	/// <summary>
	/// </summary>
	public partial class FormApptFieldDefs:FormODBase {
		private List<ApptFieldDef> _listApptFieldDefs;
		///<summary></summary>
		public FormApptFieldDefs()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormApptFieldDefs_Load(object sender, System.EventArgs e) {
			LayoutMenu();
			ApptFieldDefs.RefreshCache();
			_listApptFieldDefs=ApptFieldDefs.GetDeepCopy();
			FillListMain();
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",menuItemSetup_Click));
			menuMain.EndUpdate();
		}

		private void FillListMain(){
			listMain.Items.Clear();
			_listApptFieldDefs.Sort(CompareItemOrder);
			bool needsUpdate=false;
			for(int i=0;i<_listApptFieldDefs.Count;i++) {
				if(FieldDefLinks.GetExists(x => x.FieldDefType==FieldDefTypes.Appointment && x.FieldDefNum==_listApptFieldDefs[i].ApptFieldDefNum)) {
					listMain.Items.Add(_listApptFieldDefs[i].FieldName+" (Hidden)");
				}
				else {
					listMain.Items.Add(_listApptFieldDefs[i].FieldName);
				}
				if(_listApptFieldDefs[i].ItemOrder!=i) {
					_listApptFieldDefs[i].ItemOrder=i;
					needsUpdate=true;
				}
			}
			if(needsUpdate) {
				ApptFieldDefs.Sync(_listApptFieldDefs.ToList());//Some of the ItemOrders were incorrect. Creating shallow copy of list because Sync() reorders list on primary key.
			}
		}

		private void menuItemSetup_Click(object sender,EventArgs e) {
			ApptFieldDefs.Sync(_listApptFieldDefs);//Must sync and refresh in case new fields were added
			ApptFieldDefs.RefreshCache();
			using FormFieldDefLink formFieldDefLink=new FormFieldDefLink(FieldLocations.AppointmentEdit);
			formFieldDefLink.ShowDialog();
			FillListMain();
		}

		private void listMain_DoubleClick(object sender, System.EventArgs e) {
			if(listMain.SelectedIndex==-1){
				return;
			}
			using FormApptFieldDefEdit formApptFieldDefEdit=new FormApptFieldDefEdit();
			formApptFieldDefEdit.ApptFieldDef=_listApptFieldDefs[listMain.SelectedIndex];
			formApptFieldDefEdit.ShowDialog();
			if(formApptFieldDefEdit.DialogResult==DialogResult.OK) {
				if(formApptFieldDefEdit.ApptFieldDef==null) {
					_listApptFieldDefs.Remove(_listApptFieldDefs[listMain.SelectedIndex]);
				}
				else {
					_listApptFieldDefs[listMain.SelectedIndex]=formApptFieldDefEdit.ApptFieldDef;
				}
				FillListMain();
			}
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			ApptFieldDef apptFieldDef=new ApptFieldDef();
			apptFieldDef.ItemOrder=_listApptFieldDefs.Count;
			using FormApptFieldDefEdit formApptFieldDefEdit=new FormApptFieldDefEdit();
			formApptFieldDefEdit.ApptFieldDef=apptFieldDef;
			formApptFieldDefEdit.IsNew=true;
			formApptFieldDefEdit.ShowDialog();
			if(formApptFieldDefEdit.DialogResult==DialogResult.OK) {
				_listApptFieldDefs.Add(formApptFieldDefEdit.ApptFieldDef);
				FillListMain();
			}
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void butDown_Click(object sender, System.EventArgs e) {
			int idx=listMain.SelectedIndex;
			if(idx==-1){
				MsgBox.Show(this,"Please select an Appointment Field Definition first.");
				return;
			}
			if(idx==listMain.Items.Count-1) {
				return;
			}
			ApptFieldDef apptFieldDefTemp=_listApptFieldDefs[idx];
			_listApptFieldDefs[idx]=_listApptFieldDefs[idx+1];
			_listApptFieldDefs[idx].ItemOrder--;
			_listApptFieldDefs[idx+1]=apptFieldDefTemp;
			_listApptFieldDefs[idx+1].ItemOrder++;
			FillListMain();
			listMain.SetSelected(idx+1);
		}

		private void butUp_Click(object sender, System.EventArgs e) {
			int idx=listMain.SelectedIndex;
			if(idx==-1){
				MsgBox.Show(this,"Please select an Appointment Field Definition first.");
				return;
			}
			if(idx==0) {
				return;
			}
			ApptFieldDef apptFieldDefTemp=_listApptFieldDefs[idx];
			_listApptFieldDefs[idx]=_listApptFieldDefs[idx-1];
			_listApptFieldDefs[idx].ItemOrder++;
			_listApptFieldDefs[idx-1]=apptFieldDefTemp;
			_listApptFieldDefs[idx-1].ItemOrder--;
			FillListMain();
			listMain.SetSelected(idx-1);
		}

		private void FormApptFieldDefs_FormClosing_1(object sender,FormClosingEventArgs e) {
			ApptFieldDefs.Sync(_listApptFieldDefs);
			DataValid.SetInvalid(InvalidType.PatFields);
			DataValid.SetInvalid(InvalidType.Views);
		}

		///<summary>This sorts apptFieldDefs by their item order.</summary>
		private static int CompareItemOrder(ApptFieldDef apptFieldDef1,ApptFieldDef apptFieldDef2) {
			return apptFieldDef1.ItemOrder.CompareTo(apptFieldDef2.ItemOrder);
		}
	}
}



























