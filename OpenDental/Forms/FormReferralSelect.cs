using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	///<summary></summary>
	public partial class FormReferralSelect:FormODBase {
		///<summary></summary>
		public bool IsSelectionMode;
		///<summary>Used when coming from FormEHR if a Transition of Care is needed for a reconcile.</summary>
		public bool IsDoctorSelectionMode;
		private List<Referral> _listReferrals;
		///<summary>This will contain the referral that was selected.</summary>
		public Referral ReferralSelected;
		///<summary>True by default.  Set to false if the results should exclude patient referral sources.
		///The show patient check box is set based on the value of this bool.</summary>
		public bool IsShowPat=true;
		///<summary>True by default.  Set to false if the results should exclude doctor referral sources.
		///The show doctor check box is set based on the value of this bool.</summary>
		public bool IsShowDoc=true;
		///<summary>True by default.  Set to false if the results should exclude non-patient non-doctor referral sources.
		///The show other check box is set based on the value of this bool.</summary>
		public bool IsShowOther=true;

		///<summary></summary>
		public FormReferralSelect() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormReferralSelect_Load(object sender,System.EventArgs e) {
			checkShowPat.Checked=IsShowPat;
			checkShowDoctor.Checked=IsShowDoc;
			checkShowOther.Checked=IsShowOther;
			checkPreferred.Checked=PrefC.GetBool(PrefName.ShowPreferedReferrals);
			if(Clinics.ClinicNum==0) {
				comboClinicPicker.IsAllSelected=true;//All
			}
			else {
				comboClinicPicker.ClinicNumSelected=Clinics.ClinicNum;
			}
			FillTable();
			//labelResultCount.Text="";
		}

		private void FillTable() {
			Referrals.RefreshCache();
			_listReferrals=Referrals.GetDeepCopy();
			if(!checkHidden.Checked) {
				_listReferrals.RemoveAll(x => x.IsHidden);
			}
			if(!checkShowPat.Checked) {
				_listReferrals.RemoveAll(x => x.PatNum>0);
			}
			if(!checkShowDoctor.Checked) {
				_listReferrals.RemoveAll(x => x.IsDoctor);
			}
			if(!checkShowOther.Checked) {
				_listReferrals.RemoveAll(x => x.PatNum==0 && !x.IsDoctor);
			}
			if(checkPreferred.Checked) {
				_listReferrals.RemoveAll(x => !x.IsPreferred);
			}
			if(!comboClinicPicker.IsAllSelected) {		
				if(comboClinicPicker.IsUnassignedSelected) {//If unassigned is selected, filter out any referrals attached to a clinic.
					List<long> listReferralNums=ReferralClinicLinks.GetReferralNumsWithLinks();
					_listReferrals.RemoveAll(x => listReferralNums.Contains(x.ReferralNum));
				}
				else {//If on a selected clinic, filter out any referrals not attached to the selected clinic number.
					List<ReferralClinicLink> listReferralClinicLink=ReferralClinicLinks.GetAllForClinic(comboClinicPicker.ClinicNumSelected);
					_listReferrals.RemoveAll(x => !listReferralClinicLink.Select(y => y.ReferralNum).Contains(x.ReferralNum));
				}
;			}
			if(!string.IsNullOrWhiteSpace(textSearch.Text)) {
				string[] searchTokens=textSearch.Text.ToLower().Split(new[] { ' ' },StringSplitOptions.RemoveEmptyEntries);
				_listReferrals.RemoveAll(x => searchTokens.Any(y => !x.FName.ToLower().Contains(y) 
					&& !x.LName.ToLower().Contains(y) 
					&& !x.BusinessName.ToLower().Contains(y)));
			}
			int scrollValue=gridMain.ScrollValue;
			long selectedRefNum=-1;
			if(gridMain.GetSelectedIndex()>-1) {
				selectedRefNum=((Referral)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag).ReferralNum;
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new GridColumn(Lan.g("TableSelectRefferal","LastName"),150));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableSelectRefferal","FirstName"),80));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableSelectRefferal","MI"),30));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableSelectRefferal","Title"),70));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableSelectRefferal","Specialty"),60));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableSelectRefferal","Patient"),45));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableSelectRefferal","Business Name"),150));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableSelectRefferal","Note"),0));
			gridMain.ListGridRows.Clear();
			GridRow row;
			int indexSelectedRef=-1;
			for(int i=0;i<_listReferrals.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listReferrals[i].LName);
				row.Cells.Add(_listReferrals[i].FName);
				row.Cells.Add(StringTools.Truncate(_listReferrals[i].MName,1).ToUpper());//Truncate will return empty string if MName is null or empty string, so ToUpper is null safe
				row.Cells.Add(_listReferrals[i].Title);
				row.Cells.Add(_listReferrals[i].IsDoctor?Lan.g("enumDentalSpecialty",Defs.GetName(DefCat.ProviderSpecialties,_listReferrals[i].Specialty)):"");
				row.Cells.Add(_listReferrals[i].PatNum>0?"X":"");
				row.Cells.Add(_listReferrals[i].BusinessName);
				row.Cells.Add(_listReferrals[i].Note);
				if(_listReferrals[i].IsHidden) {
					row.ColorText=Color.Gray;
				}
				row.Tag=_listReferrals[i];
				gridMain.ListGridRows.Add(row);
				if(_listReferrals[i].ReferralNum==selectedRefNum) {
					indexSelectedRef=gridMain.ListGridRows.Count-1;
				}
			}
			gridMain.EndUpdate();
			if(indexSelectedRef>-1) {
				gridMain.SetSelected(indexSelectedRef,true);
			}
			gridMain.ScrollValue=scrollValue;
			labelResultCount.Text=gridMain.ListGridRows.Count.ToString()+Lan.g(this," results found");
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//This does not automatically select a referral when in selection mode; it just lets user edit.
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a referral first");
				return;
			}
			using FormReferralEdit formReferralEdit = new FormReferralEdit(_listReferrals[e.Row]);
			formReferralEdit.ShowDialog();
			if(formReferralEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			//int selectedIndex=gridMain.GetSelectedIndex();
			FillTable();
			//gridMain.SetSelected(selectedIndex,true);
		}

		private void butAdd_Click(object sender,System.EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.ReferralAdd)) {
				return;
			}	
			Referral referralSelected=new Referral();
			bool isReferralNew=true;
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Is the referral source an existing patient?"))	{
				using FormPatientSelect formPatientSelect=new FormPatientSelect();
				formPatientSelect.IsSelectionModeOnly=true;
				formPatientSelect.ShowDialog();
				if(formPatientSelect.DialogResult!=DialogResult.OK) {
					return;
				}
				referralSelected.PatNum=formPatientSelect.PatNumSelected;
				Referral referral=Referrals.GetFirstOrDefault(x => x.PatNum==formPatientSelect.PatNumSelected);
				if(referral!=null) {
					referralSelected=referral;
					isReferralNew=false;
				}
			}
			using FormReferralEdit formReferralEdit=new FormReferralEdit(referralSelected);//the ReferralNum must be added here
			formReferralEdit.IsNew=isReferralNew;
			formReferralEdit.ShowDialog();
			if(formReferralEdit.DialogResult==DialogResult.Cancel) {
				return;
			}
			if(IsSelectionMode) {
				if(IsDoctorSelectionMode && !formReferralEdit.ReferralCur.IsDoctor) {
					MsgBox.Show(this,"Please select a doctor referral.");
					gridMain.SetAll(false);//Remove selection to prevent caching issue on OK click.  This line is an attempted fix.
					FillTable();
					return;
				}
				ReferralSelected=formReferralEdit.ReferralCur;
				DialogResult=DialogResult.OK;
				return;
			}
			else {
				FillTable();
				for(int i=0;i<_listReferrals.Count;i++) {
					if(_listReferrals[i].ReferralNum==formReferralEdit.ReferralCur.ReferralNum) {
						gridMain.SetSelected(i,true);
					}
				}
			}
		}

		private void checkHidden_Click(object sender,System.EventArgs e) {
			FillTable();
		}

		private void textSearch_TextChanged(object sender,EventArgs e) {
			FillTable();
		}

		private void checkShowPat_Click(object sender,EventArgs e) {
			FillTable();
		}

		private void checkShowDoctor_Click(object sender,EventArgs e) {
			FillTable();
		}

		private void checkShowOther_Click(object sender,EventArgs e) {
			FillTable();
		}

		private void checkPreferred_Click(object sender,EventArgs e) {
			FillTable();
		}

		private void comboClinicPicker_SelectionChangeCommitted(object sender,EventArgs e) {
			FillTable();
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(!IsSelectionMode) {
				DialogResult=DialogResult.OK;
				return;
			}
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a referral first");
				return;
			}
			if(IsDoctorSelectionMode && _listReferrals[gridMain.GetSelectedIndex()].IsDoctor==false) {
				MsgBox.Show(this,"Please select a doctor referral.");
				return;
			}
			ReferralSelected=(Referral)_listReferrals[gridMain.GetSelectedIndex()];
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}



	}
}
