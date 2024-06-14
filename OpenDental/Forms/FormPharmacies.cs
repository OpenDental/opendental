using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDental.Bridges;
using System.Collections.Generic;
using System.Linq;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormPharmacies:FormODBase {
		private bool _isChanged;
		public bool IsSelectionMode;
		///<summary>Only used if IsSelectionMode.  On OK, contains selected pharmacyNum.  Can be 0.  Can also be set ahead of time externally.</summary>
		public long PharmacyNumSelected;
		private List<Pharmacy> _listPharmacies;

		///<summary></summary>
		public FormPharmacies() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPharmacies_Load(object sender, System.EventArgs e) {
			if(!IsSelectionMode){
				butOK.Visible=false;
				butNone.Visible=false;
			}
			FillGrid();
			if(PharmacyNumSelected==0){
				return;
			}
			for(int i=0;i<_listPharmacies.Count;i++){
				if(_listPharmacies[i].PharmacyNum!=PharmacyNumSelected){
					continue;
				}
				gridMain.SetSelected(i,true);
				break;
			}
		}

		private void FillGrid(){
			Pharmacies.RefreshCache();
			_listPharmacies=Pharmacies.GetDeepCopy();
			List<PharmClinic> listPharmClinics=new List<PharmClinic>();
			if(PrefC.HasClinicsEnabled) {
				listPharmClinics=PharmClinics.GetPharmClinicsForPharmacies(_listPharmacies.Select(x => x.PharmacyNum).ToList());
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TablePharmacies","Store Name"),130);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePharmacies","Phone"),90);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePharmacies","Fax"),90);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePharmacies","Address"),120);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePharmacies","City"),90);
			gridMain.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g("TablePharmacies","Clinics"),115);
				gridMain.Columns.Add(col);
			}
			col=new GridColumn(Lan.g("TablePharmacies","Note"),100);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			string txt;
			for(int i=0;i<_listPharmacies.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listPharmacies[i].StoreName);
				row.Cells.Add(_listPharmacies[i].Phone);
				if(Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
					row.Cells[row.Cells.Count-1].ColorText=Color.Blue;
					row.Cells[row.Cells.Count-1].Underline=YN.Yes;
				}
				row.Cells.Add(_listPharmacies[i].Fax);
				txt=_listPharmacies[i].Address;
				if(_listPharmacies[i].Address2!="") {
					txt+="\r\n"+_listPharmacies[i].Address2;
				}
				row.Cells.Add(txt);
				row.Cells.Add(_listPharmacies[i].City);
				if(PrefC.HasClinicsEnabled) {
					List<long> listClinicNums=listPharmClinics.FindAll(x => x.PharmacyNum==_listPharmacies[i].PharmacyNum).Select(y => y.ClinicNum).ToList();
					List<Clinic> listClinics=Clinics.GetClinics(listClinicNums);
					row.Cells.Add(string.Join(",",listClinics.Select(x => x.Abbr)));
				}
				row.Cells.Add(_listPharmacies[i].Note);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormPharmacyEdit formPharmacyEdit=new FormPharmacyEdit();
			formPharmacyEdit.PharmacyCur=new Pharmacy();
			formPharmacyEdit.PharmacyCur.IsNew=true;
			formPharmacyEdit.ShowDialog();
			FillGrid();
			_isChanged=true;
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(IsSelectionMode){
				PharmacyNumSelected=_listPharmacies[e.Row].PharmacyNum;
				DialogResult=DialogResult.OK;
				return;
			}
			using FormPharmacyEdit formPharmacyEdit=new FormPharmacyEdit();
			formPharmacyEdit.PharmacyCur=_listPharmacies[e.Row];
			formPharmacyEdit.ShowDialog();
			FillGrid();
			_isChanged=true;
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			GridCell gridCell=gridMain.ListGridRows[e.Row].Cells[e.Col];
			//Only grid cells with phone numbers are blue and underlined.
			if(gridCell.ColorText==System.Drawing.Color.Blue && gridCell.Underline==YN.Yes && Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
				DentalTek.PlaceCall(gridCell.Text);
			}
		}

		private void butNone_Click(object sender,EventArgs e) {
			//not even visible unless is selection mode
			PharmacyNumSelected=0;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			//not even visible unless is selection mode
			if(gridMain.GetSelectedIndex()==-1){
			//	MsgBox.Show(this,"Please select an item first.");
			//	return;
				PharmacyNumSelected=0;
			}
			else{
				PharmacyNumSelected=_listPharmacies[gridMain.GetSelectedIndex()].PharmacyNum;
			}
			DialogResult=DialogResult.OK;
		}

		private void FormPharmacies_FormClosing(object sender,FormClosingEventArgs e) {
			if(_isChanged){
				DataValid.SetInvalid(InvalidType.Pharmacies);
			}
		}

	}
}