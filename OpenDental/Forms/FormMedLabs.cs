using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMedLabs:FormODBase {
		public Patient PatCur;
		///<summary>Used to show the labs for a specific patient.  May be the same as PatCur or a different selected patient or null for all patients.</summary>
		private Patient _selectedPat;
		///<summary>List of clinics the current user has permission to access.  Exactly matches the contents of the clinic drop-down combobox, including a
		///Headquarters clinic with ClinicNum 0 if the user is unrestricted.</summary>
		private List<Clinic> _listUserClinics;
		///<summary>Dictionary of lab account num key to clinic description value used to fill the grid.  Dictionary is filled on load with the
		///lab account nums and clinic descriptions for all clinics for which the current user has permission to access.</summary>
		private Dictionary<string,string> _dictLabAcctClinic;

		public FormMedLabs() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMedLabs_Load(object sender,EventArgs e) {
			_selectedPat=PatCur;
			if(_selectedPat==null) {
				checkIncludeNoPat.Checked=true;
				checkOnlyNoPat.Checked=true;
			}
			textDateStart.Text=DateTime.Today.AddMonths(-3).ToShortDateString();//default list to start with showing the last three months
			//One time reconcile may need to be run to create embedded PDFs for MedLabs that are not attached to a patient.
			if(!PrefC.GetBool(PrefName.MedLabReconcileDone) && PrefC.AtoZfolderUsed!=DataStorageType.InDatabase) {
				int countMedLabs=MedLabs.GetCountForPatient(0);
				if(MessageBox.Show(this,Lan.g(this,"There are MedLabs in the database that have not been associated with a patient.\r\nA one time "
					+"reconciliation must be performed that will reprocess the HL7 messages for these MedLabs.  This can take some time.\r\nDo you want to "
					+"continue?\r\nNumber of MedLabs not associated with a patient")+": "+countMedLabs+".","",MessageBoxButtons.YesNo)==DialogResult.No)
				{
					Close();
					return;
				}
				Cursor=Cursors.WaitCursor;
				int reconcileFailedCount=MedLabs.Reconcile();
				Cursor=Cursors.Default;
				if(reconcileFailedCount>0) {
					MessageBox.Show(this,Lan.g(this,"Some of the MedLab objects in the database could not be reconciled.\r\nThis may be due to an issue "
						+"processing the original HL7 message text file.\r\nNumber failed")+": "+reconcileFailedCount);
				}
				Prefs.UpdateBool(PrefName.MedLabReconcileDone,true);
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			_dictLabAcctClinic=new Dictionary<string,string>();
			_listUserClinics=new List<Clinic>();
			if(PrefC.HasClinicsEnabled) {
				_listUserClinics.Add(new Clinic() { ClinicNum=-1,Description=Lan.g(this,"All") });//ClinicNum will be -1 at index 0, "All" means all the user has access to
				if(!Security.CurUser.ClinicIsRestricted) {
					//ClinicNum 0 at index==1, "Headquarters" means any where MedLab.PatAccountNum does not match any clinic.MedLabAccountNum
					_listUserClinics.Add(new Clinic() { ClinicNum=0,Description=Lan.g(this,"Unassigned") });
				}
				_listUserClinics.AddRange(Clinics.GetForUserod(Security.CurUser));
				_listUserClinics.ForEach(x => comboClinic.Items.Add(x.Description));
				_dictLabAcctClinic=_listUserClinics.Where(x => !string.IsNullOrEmpty(x.MedLabAccountNum))
					.ToDictionary(x => x.MedLabAccountNum,x => x.Description);
				if(!Security.CurUser.ClinicIsRestricted && Clinics.ClinicNum==0) {//if unrestricted and the currently selected clinic is HQ
					comboClinic.SelectedIndex=1;//all users will have the "All" clinic, unrestricted users will also have the "Unassigned" clinic, so index==1
				}
				else {
					comboClinic.SelectedIndex=_listUserClinics.FindIndex(x => x.ClinicNum==Clinics.ClinicNum);
				}
				if(comboClinic.SelectedIndex<0) {
					comboClinic.SelectedIndex=0;
				}
			}
			else {
				comboClinic.Visible=false;
				labelClinic.Visible=false;
				FillGrid();//if clinics are enabled, comboClinic.SelectedIndexChanged event handler will fill the grid, no need to call FillGrid
			}
		}

		private void FillGrid() {
			if(IsDisposed) {//This can happen if an auto logoff happens with FormMedLabEdit open
				return;
			}
			if(!textDateStart.IsValid() || !textDateEnd.IsValid()) {
				return;
			}
			textPatient.Text="";
			if(_selectedPat!=null) {
				textPatient.Text=_selectedPat.GetNameLF();
				checkOnlyNoPat.Checked=false;
			}
			Application.DoEvents();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn("Date & Time Reported",135,GridSortingStrategy.DateParse));//most recent date and time a result came in
			gridMain.ListGridColumns.Add(new GridColumn("Date & Time Entered",135,GridSortingStrategy.DateParse));
			gridMain.ListGridColumns.Add(new GridColumn("Status",75));
			gridMain.ListGridColumns.Add(new GridColumn("Patient",180));
			gridMain.ListGridColumns.Add(new GridColumn("Provider",70));
			gridMain.ListGridColumns.Add(new GridColumn("Specimen ID",100));//should be the ID sent on the specimen container to lab
			gridMain.ListGridColumns.Add(new GridColumn("Test(s) Description",235));//description of the test ordered
			if(PrefC.HasClinicsEnabled) {
				gridMain.ListGridColumns.Add(new GridColumn("Clinic",150));
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			DateTime dateEnd=PIn.Date(textDateEnd.Text);
			if(dateEnd==DateTime.MinValue) {
				dateEnd=DateTime.MaxValue;
			}
			Cursor=Cursors.WaitCursor;
			Clinic clinCur=new Clinic();
			if(PrefC.HasClinicsEnabled) {
				clinCur=_listUserClinics[comboClinic.SelectedIndex];
			}
			List<Clinic> listClinicsSelected=new List<Clinic>();
			if(clinCur.ClinicNum==-1) {//"All" clinic
				listClinicsSelected=_listUserClinics.FindAll(x => x.ClinicNum>-1);//will include ClinicNum 0 ("Unassigned" clinic) if user is unrestricted
			}
			else {//a single clinic was selected, either the "Unassigned" clinic or a regular clinic
				listClinicsSelected.Add(clinCur);
			}
			List<MedLab> listMedLabs=MedLabs.GetOrdersForPatient(_selectedPat,checkIncludeNoPat.Checked,checkOnlyNoPat.Checked,PIn.Date(textDateStart.Text),
				dateEnd,listClinicsSelected);
			Dictionary<long,Patient> dictPats=Patients.GetLimForPats(listMedLabs.Select(x => x.PatNum).Where(x => x>0).Distinct().ToList())
				.ToDictionary(x => x.PatNum);
			foreach(MedLab medLabCur in listMedLabs) {
				row=new GridRow();
				row.Cells.Add(medLabCur.DateTimeReported.ToString("MM/dd/yyyy hh:mm tt"));
				row.Cells.Add(medLabCur.DateTimeEntered.ToString("MM/dd/yyyy hh:mm tt"));
				if(medLabCur.IsPreliminaryResult) {//check whether the test or any of the most recent results for the test is marked as preliminary
					row.Cells.Add(MedLabs.GetStatusDescript(ResultStatus.P));
				}
				else {
					row.Cells.Add(MedLabs.GetStatusDescript(medLabCur.ResultStatus));
				}
				string nameFL="";
				if(dictPats.ContainsKey(medLabCur.PatNum)) {
					nameFL=dictPats[medLabCur.PatNum].GetNameFLnoPref();
				}
				row.Cells.Add(nameFL);
				row.Cells.Add(Providers.GetAbbr(medLabCur.ProvNum));//will be blank if ProvNum=0
				row.Cells.Add(medLabCur.SpecimenID);
				row.Cells.Add(medLabCur.ObsTestDescript);
				if(PrefC.HasClinicsEnabled) {
					string clinicDesc="";
					if(_dictLabAcctClinic.ContainsKey(medLabCur.PatAccountNum)) {
						clinicDesc=_dictLabAcctClinic[medLabCur.PatAccountNum];
					}
					row.Cells.Add(clinicDesc);
				}
				row.Tag=medLabCur.PatNum.ToString()+","+medLabCur.SpecimenID+","+medLabCur.SpecimenIDFiller;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			Cursor=Cursors.Default;
		}

		private void comboClinic_SelectedIndexChanged(object sender,EventArgs e) {
			FillGrid();
		}
		
		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormMedLabEdit FormLE=new FormMedLabEdit();
			long patNum=0;
			string[] patSpecimenIds=gridMain.ListGridRows[e.Row].Tag.ToString().Split(',');
			if(patSpecimenIds.Length>0) {
				patNum=PIn.Long(patSpecimenIds[0]);//if PatNum portion of the tag is an empty string, patNum will remain 0
			}
			FormLE.PatCur=Patients.GetPat(patNum);//could be null if PatNum=0
			string specimenId="";
			string specimenIdFiller="";
			if(patSpecimenIds.Length>1) {
				specimenId=patSpecimenIds[1];
			}
			if(patSpecimenIds.Length>2) {
				specimenIdFiller=patSpecimenIds[2];
			}
			FormLE.ListMedLabs=MedLabs.GetForPatAndSpecimen(patNum,specimenId,specimenIdFiller);//patNum could be 0 if this MedLab is not attached to a pat
			FormLE.ShowDialog();
			FillGrid();
		}

		private void checkIncludeNoPat_Click(object sender,EventArgs e) {
			if(!checkIncludeNoPat.Checked) {
				checkOnlyNoPat.Checked=false;
			}
			FillGrid();
		}

		private void checkOnlyNoPat_Click(object sender,EventArgs e) {
			if(checkOnlyNoPat.Checked) {
				checkIncludeNoPat.Checked=true;
				_selectedPat=null;
			}
			FillGrid();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butCurrent_Click(object sender,EventArgs e) {
			_selectedPat=PatCur;
			FillGrid();
		}

		private void butFind_Click(object sender,EventArgs e) {
			using FormPatientSelect FormPS=new FormPatientSelect();
			FormPS.ShowDialog();
			if(FormPS.DialogResult!=DialogResult.OK) {
				return;
			}
			_selectedPat=Patients.GetPat(FormPS.SelectedPatNum);
			FillGrid();
		}

		private void butAll_Click(object sender,EventArgs e) {
			_selectedPat=null;
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			this.Close();
		}
		
	}
}
