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
		private bool changed;
		public bool IsSelectionMode;
		///<summary>Only used if IsSelectionMode.  On OK, contains selected pharmacyNum.  Can be 0.  Can also be set ahead of time externally.</summary>
		public long SelectedPharmacyNum;
		private List<Pharmacy> _listPharmacies;

		///<summary></summary>
		public FormPharmacies()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPharmacies_Load(object sender, System.EventArgs e) {
			if(IsSelectionMode){
				butClose.Text=Lan.g(this,"Cancel");
			}
			else{
				butOK.Visible=false;
				butNone.Visible=false;
			}
			FillGrid();
			if(SelectedPharmacyNum!=0){
				for(int i=0;i<_listPharmacies.Count;i++){
					if(_listPharmacies[i].PharmacyNum==SelectedPharmacyNum){
						gridMain.SetSelected(i,true);
						break;
					}
				}
			}
		}

		private void FillGrid(){
			Pharmacies.RefreshCache();
			_listPharmacies=Pharmacies.GetDeepCopy();
			//Key=>PharmacyNum & Value=>List of clinics
			SerializableDictionary<long,List<Clinic>> dictPharmClinics=null;
			if(PrefC.HasClinicsEnabled) {
				dictPharmClinics=Clinics.GetDictClinicsForPharmacy(_listPharmacies.Select(x => x.PharmacyNum).ToArray());
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TablePharmacies","Store Name"),130);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TablePharmacies","Phone"),90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TablePharmacies","Fax"),90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TablePharmacies","Address"),120);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TablePharmacies","City"),90);
			gridMain.ListGridColumns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g("TablePharmacies","Clinics"),115);
				gridMain.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g("TablePharmacies","Note"),100);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			string txt;
			foreach(Pharmacy pharm in _listPharmacies) {
				row=new GridRow();
				row.Cells.Add(pharm.StoreName);
				row.Cells.Add(pharm.Phone);
				if(Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
					row.Cells[row.Cells.Count-1].ColorText=Color.Blue;
					row.Cells[row.Cells.Count-1].Underline=YN.Yes;
				}
				row.Cells.Add(pharm.Fax);
				txt=pharm.Address;
				if(pharm.Address2!="") {
					txt+="\r\n"+pharm.Address2;
				}
				row.Cells.Add(txt);
				row.Cells.Add(pharm.City);
				if(PrefC.HasClinicsEnabled) {
					List<Clinic> listClinics;
					if(!dictPharmClinics.TryGetValue(pharm.PharmacyNum,out listClinics)) {
						listClinics=new List<Clinic>();
					}
					row.Cells.Add(string.Join(",",listClinics.Select(x => x.Abbr)));
				}
				row.Cells.Add(pharm.Note);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormPharmacyEdit FormPE=new FormPharmacyEdit();
			FormPE.PharmCur=new Pharmacy();
			FormPE.PharmCur.IsNew=true;
			FormPE.ShowDialog();
			FillGrid();
			changed=true;
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(IsSelectionMode){
				SelectedPharmacyNum=_listPharmacies[e.Row].PharmacyNum;
				DialogResult=DialogResult.OK;
				return;
			}
			else{
				using FormPharmacyEdit FormP=new FormPharmacyEdit();
				FormP.PharmCur=_listPharmacies[e.Row];
				FormP.ShowDialog();
				FillGrid();
				changed=true;
			}
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			GridCell gridCellCur=gridMain.ListGridRows[e.Row].Cells[e.Col];
			//Only grid cells with phone numbers are blue and underlined.
			if(gridCellCur.ColorText==System.Drawing.Color.Blue && gridCellCur.Underline==YN.Yes && Programs.GetCur(ProgramName.DentalTekSmartOfficePhone).Enabled) {
				DentalTek.PlaceCall(gridCellCur.Text);
			}
		}

		private void butNone_Click(object sender,EventArgs e) {
			//not even visible unless is selection mode
			SelectedPharmacyNum=0;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			//not even visible unless is selection mode
			if(gridMain.GetSelectedIndex()==-1){
			//	MsgBox.Show(this,"Please select an item first.");
			//	return;
				SelectedPharmacyNum=0;
			}
			else{
				SelectedPharmacyNum=_listPharmacies[gridMain.GetSelectedIndex()].PharmacyNum;
			}
			DialogResult=DialogResult.OK;
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormPharmacies_FormClosing(object sender,FormClosingEventArgs e) {
			if(changed){
				DataValid.SetInvalid(InvalidType.Pharmacies);
			}
		}

	

		

		

		



		
	}
}





















