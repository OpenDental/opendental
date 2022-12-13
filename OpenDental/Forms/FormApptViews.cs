using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormApptViews : FormODBase {
		private bool _changed;
		private List<ApptView> _listApptViews;

		///<summary></summary>
		public FormApptViews()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormApptViews_Load(object sender, System.EventArgs e) {
			comboClinic.SelectedClinicNum=Clinics.ClinicNum;
			FillViewList();
			if(PrefC.GetInt(PrefName.AppointmentTimeIncrement)==5){
				radioFive.Checked=true;
			}
			else if(PrefC.GetInt(PrefName.AppointmentTimeIncrement)==10) {
				radioTen.Checked=true;
			}
			else{
				radioFifteen.Checked=true;
			}
		}

		private void FillViewList(){
			Cache.Refresh(InvalidType.Views);
			listViews.Items.Clear();
			_listApptViews=new List<ApptView>();
			List<ApptView> listApptViews=ApptViews.GetDeepCopy();
			string F;
			for(int i=0;i<listApptViews.Count;i++){
				if(PrefC.HasClinicsEnabled && comboClinic.SelectedClinicNum!=listApptViews[i].ClinicNum) {
					continue;//only add views assigned to the clinic selected
				}
				if(listViews.Items.Count<12)
					F="F"+(listViews.Items.Count+1).ToString()+"-";
				else
					F="";
				listViews.Items.Add(F+listApptViews[i].Description);
				_listApptViews.Add(listApptViews[i]);
			}
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			FillViewList();
		}
		
		private void butAdd_Click(object sender, System.EventArgs e) {
			ApptView apptView=new ApptView();
			if(_listApptViews.Count==0) {
				apptView.ItemOrder=0;
			}
			else {
				apptView.ItemOrder=_listApptViews[_listApptViews.Count-1].ItemOrder+1;
			}
			apptView.ApptTimeScrollStart=DateTime.Parse("08:00:00").TimeOfDay;//default to 8 AM
			apptView.RowsPerIncr=1;//Should never be zero because it will cause OD to hang on an infinite loop.
			ApptViews.Insert(apptView);//this also gets the primary key
			using FormApptViewEdit formApptViewEdit=new FormApptViewEdit();
			formApptViewEdit.ApptViewCur=apptView;
			formApptViewEdit.IsNew=true;
			formApptViewEdit.ClinicNumInitial=comboClinic.SelectedClinicNum;
			formApptViewEdit.ShowDialog();
			if(formApptViewEdit.DialogResult!=DialogResult.OK){
				return;
			}
			_changed=true;
			FillViewList();
			listViews.SelectedIndex=listViews.Items.Count-1;//this works even if no items
		}

		private void listViews_DoubleClick(object sender, System.EventArgs e) {
			if(listViews.SelectedIndex==-1){
				return;
			}
			int selected=listViews.SelectedIndex;
			ApptView apptView=_listApptViews[listViews.SelectedIndex];
			using FormApptViewEdit formApptViewEdit=new FormApptViewEdit();
			formApptViewEdit.ApptViewCur=apptView;
			formApptViewEdit.ClinicNumInitial=comboClinic.SelectedClinicNum;
			formApptViewEdit.ShowDialog();
			if(formApptViewEdit.DialogResult!=DialogResult.OK){
				return;
			}
			_changed=true;
			FillViewList();
			if(selected<listViews.Items.Count) {
				listViews.SelectedIndex=selected;
			}
			else {
				listViews.SelectedIndex=-1;
			}
		}

		private void butUp_Click(object sender, System.EventArgs e) {
			if(listViews.SelectedIndex==-1){
				MessageBox.Show(Lan.g(this,"Please select a category first."));
				return;
			}
			if(listViews.SelectedIndex==0){
				return;//can't go up any more
			}
			//it will flip flop with the one above it
			ApptView apptView=_listApptViews[listViews.SelectedIndex-1];
			apptView.ItemOrder=listViews.SelectedIndex;
			ApptViews.Update(apptView);
			//now the other
			apptView=_listApptViews[listViews.SelectedIndex];
			apptView.ItemOrder=listViews.SelectedIndex-1;
			ApptViews.Update(apptView);
			_changed=true;
			FillViewList();
			listViews.SelectedIndex=_listApptViews.FindIndex(x => x.ApptViewNum==apptView.ApptViewNum);
		}

		private void butDown_Click(object sender, System.EventArgs e) {
			if(listViews.SelectedIndex==-1){
				MessageBox.Show(Lan.g(this,"Please select a category first."));
				return;
			}
			if(listViews.SelectedIndex==listViews.Items.Count-1){
				return;//can't go down any more
			}
			//it will flip flop with the one below it
			ApptView apptView=_listApptViews[listViews.SelectedIndex+1];
			apptView.ItemOrder=listViews.SelectedIndex;
			ApptViews.Update(apptView);
			//now the other
			apptView=_listApptViews[listViews.SelectedIndex];
			apptView.ItemOrder=listViews.SelectedIndex+1;
			ApptViews.Update(apptView);
			_changed=true;
			FillViewList();
			listViews.SelectedIndex=_listApptViews.FindIndex(x => x.ApptViewNum==apptView.ApptViewNum);
		}

		private void butProcColors_Click(object sender,EventArgs e) {
			using FormProcApptColors formProcApptColors=new FormProcApptColors();
			formProcApptColors.ShowDialog();
			DialogResult=DialogResult.None;//This is required to prevent FormApptViews from closing.
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormApptViews_FormClosing(object sender,FormClosingEventArgs e) {
			int newIncrement=15;
			if(radioFive.Checked) {
				newIncrement=5;
			}
			if(radioTen.Checked) {
				newIncrement=10;
			}
			if(Prefs.UpdateInt(PrefName.AppointmentTimeIncrement,newIncrement)){
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			if(_changed){
				DataValid.SetInvalid(InvalidType.Views);
			}
		}

		


	

		


	}
}





















