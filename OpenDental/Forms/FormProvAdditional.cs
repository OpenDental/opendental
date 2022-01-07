using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using CodeBase;
using Bridges;

namespace OpenDental {
	public partial class FormProvAdditional:FormODBase {
		/// <summary>This is a list of providerclinic rows that were given to this form, containing any modifications that were made while in FormProvAdditional.</summary>
		public List<ProviderClinic> ListProviderClinicOut=new List<ProviderClinic>();
		private Provider _provCur;
		private List<ProviderClinic> _listProvClinic;
		ProviderClinic _provClinicDefault;

		public FormProvAdditional(List<ProviderClinic> listProvClinic,Provider provCur) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listProvClinic=listProvClinic.Select(x => x.Copy()).ToList();
			_provCur=provCur;
		}

		private void FormProvAdditional_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			Cursor=Cursors.WaitCursor;
			gridProvProperties.BeginUpdate();
			gridProvProperties.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableProviderProperties","Clinic"),120);
			gridProvProperties.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProviderProperties","DEA Num"),120,true);
			gridProvProperties.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProviderProperties","State License Num"),120,true);
			gridProvProperties.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProviderProperties","State Rx ID"),120,true);
			gridProvProperties.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProviderProperties","State Where Licensed"),120,true);
			gridProvProperties.ListGridColumns.Add(col);
			gridProvProperties.ListGridColumns.Add(new GridColumn(Lan.g("TableProviderProperties","CareCredit Merch Num"),130,true));
			gridProvProperties.ListGridRows.Clear();
			GridRow row;
			_provClinicDefault=_listProvClinic.Find(x => x.ClinicNum==0);
			//Didn't have an HQ row
			if(_provClinicDefault==null) {//Doesn't exist in list
				_provClinicDefault=ProviderClinics.GetOne(_provCur.ProvNum,0);
				if(_provClinicDefault==null) {//Doesn't exist in database
					_provClinicDefault=new ProviderClinic {
						ProvNum=_provCur.ProvNum,
						ClinicNum=0,
						DEANum=_provCur.DEANum,
						StateLicense=_provCur.StateLicense,
						StateRxID=_provCur.StateRxID,
						StateWhereLicensed=_provCur.StateWhereLicensed,
						CareCreditMerchantId="",
					};
				}
				_listProvClinic.Add(_provClinicDefault);//If not in list, add to list.
			}
			row=new GridRow();
			row.Cells.Add("Default");
			row.Cells.Add(_provClinicDefault.DEANum);
			row.Cells.Add(_provClinicDefault.StateLicense);
			row.Cells.Add(_provClinicDefault.StateRxID);
			row.Cells.Add(_provClinicDefault.StateWhereLicensed);
			row.Cells.Add(_provClinicDefault.CareCreditMerchantId);
			row.Tag=_provClinicDefault;
			gridProvProperties.ListGridRows.Add(row);
			if(PrefC.HasClinicsEnabled) {
				foreach(Clinic clinicCur in Clinics.GetForUserod(Security.CurUser)) {
					row=new GridRow();
					ProviderClinic provClinic=_listProvClinic.Find(x => x.ClinicNum == clinicCur.ClinicNum);
					//Doesn't exist in Db, create a new one
					if(provClinic==null) {
						provClinic=new ProviderClinic() {
							ProvNum=_provCur.ProvNum,
							ClinicNum=clinicCur.ClinicNum,
							CareCreditMerchantId="",
						};
						_listProvClinic.Add(provClinic);
					}
					row.Cells.Add(clinicCur.Abbr);
					row.Cells.Add(provClinic.DEANum);
					row.Cells.Add(provClinic.StateLicense);
					row.Cells.Add(provClinic.StateRxID);
					row.Cells.Add(provClinic.StateWhereLicensed);
					row.Cells.Add(provClinic.CareCreditMerchantId);
					row.Tag=provClinic;
					gridProvProperties.ListGridRows.Add(row);
				}
			}
			gridProvProperties.EndUpdate();
			Cursor=Cursors.Default;
		}

		private void gridProvProperties_CellLeave(object sender,ODGridClickEventArgs e) {
			GridRow selectedRow=gridProvProperties.SelectedGridRows.First();
			if(selectedRow==null) {
				return;
			}
			ProviderClinic provClin=(ProviderClinic)selectedRow.Tag;
			string strNewValue=PIn.String(selectedRow.Cells[e.Col].Text);
			if(e.Col==1) {
				provClin.DEANum=strNewValue;
			}
			else if(e.Col==2) {
				provClin.StateLicense=strNewValue;
			}
			else if(e.Col==3) {
				provClin.StateRxID=strNewValue;
			}
			else if(e.Col==4) {
				provClin.StateWhereLicensed=strNewValue;
			}
			else if(e.Col==5) {
				provClin.CareCreditMerchantId=strNewValue;
			}
		}

		///<summary>Returns true if provClinic is empty. ProvClinic is considered empty if the DEANum,StateLicense,StateRxID, StateWereLicensed,
		///CareCreditMerchantId are empty.</summary>
		private bool IsEmpty(ProviderClinic provClinic) {
			return (provClinic!=null && string.IsNullOrEmpty(provClinic.DEANum)
					&& string.IsNullOrEmpty(provClinic.StateLicense)
					&& string.IsNullOrEmpty(provClinic.StateRxID)
					&& string.IsNullOrEmpty(provClinic.StateWhereLicensed)
					&& string.IsNullOrEmpty(provClinic.CareCreditMerchantId));
		}

		///<summary>Returns true if the provClinic was modified. Otherwise, returns false.</summary>
		private bool IsProvClinicModified(ProviderClinic provClinic) {
			if(provClinic==null || (provClinic!=_provClinicDefault && IsEmpty(provClinic))) {
				return false;//No overrided added/edited or override were removed
			}
			return true;
		}

		private bool AreMerchantNumbersValid(List<ProviderClinic> listProvClinics) {
			List<ProviderClinic> listInvalidMerchantNums=listProvClinics.FindAll(x => IsProvClinicModified(x)
				&& !string.IsNullOrWhiteSpace(x.CareCreditMerchantId)
				&& !CareCredit.IsMerchantNumValid(x.CareCreditMerchantId));
			if(listInvalidMerchantNums.IsNullOrEmpty()) {
				return true;//all are valid
			}
			List<Clinic> listClinics=new List<Clinic>();
			List<long> listClinicNums=listInvalidMerchantNums.Select(x => x.ClinicNum).ToList();
			if(listClinicNums.Contains(0)) {
				//Add the default clinic
				listClinics.Add(new Clinic(){ ClinicNum=0,Abbr="Default" });
			}
			listClinics.AddRange(Clinics.GetClinics(listClinicNums));
			string invalidClinics=string.Join(",",listClinics.Select(x => x.Abbr));
			MsgBox.Show(this,$"Invalid Merchant Id for the following clinic(s) '{invalidClinics}'.\r\nMerchant Ids must be a 16-digit number.");
			return false;
		}

		private void butOK_Click(object sender,EventArgs e) {
			List<ProviderClinic> listProvClinics=gridProvProperties.ListGridRows.Select(x => (ProviderClinic)x.Tag).ToList();
			if(!AreMerchantNumbersValid(listProvClinics)) {
				//At least one of the provider clinic was modifed and the merchant id that was entered is invalid.
				return;
			}
			ListProviderClinicOut=new List<ProviderClinic>();
			foreach(ProviderClinic provClinic in listProvClinics) {
				//always add the default _provClinicDefault
				if(!IsProvClinicModified(provClinic)) {
					continue;//No overrided added/edited or override were removed
				}
				ListProviderClinicOut.Add(provClinic);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}