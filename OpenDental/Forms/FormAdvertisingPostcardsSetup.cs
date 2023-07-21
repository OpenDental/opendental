using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Bridges;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormAdvertisingPostcardsSetup:FormODBase {
		private PostcardManiaMetaData _postcardManiaMetaData;
		private const string DEFAULT="Default";

		public FormAdvertisingPostcardsSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMassPostcardAdvertising_Load(object sender,EventArgs e) {
			RefreshData();
			RefreshView();
		}

		///<summary>Calls Services HQ to get Account data for office.</summary>
		private void RefreshData() {
			_postcardManiaMetaData=new PostcardManiaMetaData();
			try {
				UI.ProgressOD progressOD=new UI.ProgressOD();
				progressOD.ActionMain=() => {
					_postcardManiaMetaData=AdvertisingPostcards.GetPostcardManiaMetaData();
				};
				progressOD.StartingMessage=Lan.g(this,"Getting Account Data")+"...";
				progressOD.ShowDialogProgress();
				if(progressOD.IsCancelled){
					return;
				}
			}
			catch(Exception ex) {
				FriendlyException.Show(ex.Message,ex);
			}
			if(string.IsNullOrWhiteSpace(PrefC.GetString(PrefName.AdvertisingPostCardGuid)) && !_postcardManiaMetaData.Accounts.IsNullOrEmpty()) {
				Prefs.UpdateString(PrefName.AdvertisingPostCardGuid,_postcardManiaMetaData.Accounts.First().Guid);
				Signalods.SetInvalid(InvalidType.Prefs);
			}
			if(PrefC.HasClinicsEnabled) { //This is to include a default in the Clinic grid's cmboboxes.
				_postcardManiaMetaData.Accounts.Insert(0,new PostcardManiaAccountData(DEFAULT,DEFAULT,DEFAULT));
			}
		}

		///<summary>Fills grids and populates UI.</summary>
		private void RefreshView() {
			if(!_postcardManiaMetaData.Accounts.IsNullOrEmpty()) {
				FillClinicGrid();
				FillAccountsGrid();
			}
			FillUI();	
		}

		private void FillClinicGrid() {
			if(!PrefC.HasClinicsEnabled) {
				return;
			}
			List<Clinic> listClinics=Clinics.GetForUserod(Security.CurUser);//Intentionally ignores the 0 clinic, thats handled by accounts grid.
			gridClinics.BeginUpdate();
			gridClinics.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Clinic"),150);
			gridClinics.Columns.Add(col);
			col=new GridColumn("Postcards Account",165) {
				ListDisplayStrings=_postcardManiaMetaData.Accounts.Select(x => x.AccountTitle).ToList(),
				Tag=_postcardManiaMetaData,
			};
			gridClinics.Columns.Add(col);
			gridClinics.ListGridRows.Clear();
			foreach(Clinic clinic in listClinics) {
				ClinicPref clinicPref=ClinicPrefs.GetPref(PrefName.AdvertisingPostCardGuid,clinic.ClinicNum);
				GridRow row=new GridRow();
				row.Cells.Add(Clinics.GetAbbr(clinic.ClinicNum,listClinics));
				int comboSelectedIndex=0;
				if(clinicPref!=null) 
				{
					comboSelectedIndex=_postcardManiaMetaData.Accounts.FindIndex(x=>x.Guid==clinicPref.ValueString);
				}
				if(comboSelectedIndex<0) { //Shouldn't happen. Something is out of synch.
					comboSelectedIndex=0;
				}
				row.Cells.Add(new GridCell($"{_postcardManiaMetaData.Accounts[comboSelectedIndex].AccountTitle}") {
					ComboSelectedIndex=comboSelectedIndex,
				});
				row.Tag=clinic;
				gridClinics.ListGridRows.Add(row);
			}
			gridClinics.EndUpdate();
		}

		private void FillAccountsGrid() {
			string defaultGuid=PrefC.GetString(PrefName.AdvertisingPostCardGuid);
			gridAccounts.BeginUpdate();
			gridAccounts.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,DEFAULT),60,HorizontalAlignment.Center);
			gridAccounts.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Account"),60,HorizontalAlignment.Center);
			gridAccounts.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Email"),190);
			gridAccounts.Columns.Add(col);
			gridAccounts.ListGridRows.Clear();
			foreach(PostcardManiaAccountData postcardManiaMetaData in _postcardManiaMetaData.Accounts) {
				if(postcardManiaMetaData.Email==DEFAULT) {
					continue;
				}
				GridRow row=new GridRow();
				if(postcardManiaMetaData.Guid==defaultGuid) {
					row.Cells.Add(new GridCell("X"));
				}
				else {
					row.Cells.Add(new GridCell(""));
				}
				row.Cells.Add(new GridCell(postcardManiaMetaData.AccountTitle));
				row.Cells.Add(new GridCell(postcardManiaMetaData.Email));
				row.Tag=postcardManiaMetaData;
				gridAccounts.ListGridRows.Add(row);
			}
			gridAccounts.EndUpdate();
		}

		private void FillUI() {
			webBrowser.Navigate(_postcardManiaMetaData.displayURL);
			gridClinics.Visible=PrefC.HasClinicsEnabled;
			labelClinicsGridHint.Visible=PrefC.HasClinicsEnabled;
			labelNotEnabled.Visible=_postcardManiaMetaData.Accounts.Where(x=>x.Email!=DEFAULT).ToList().IsNullOrEmpty();
		}

		///<summary>If accountData is set the setup window will populate with the existing data. This allows the user to edit the account title.</summary>
		private void ManageMassPostcardAccount(PostcardManiaAccountData accountData=null) {
			using FormAdvertisingPostcardsAccountSetup formMassPostcardSetup=new FormAdvertisingPostcardsAccountSetup();
			if(accountData!=null) {
				formMassPostcardSetup.AccountDataEditing=accountData;
			}
			formMassPostcardSetup.ListAccountData=_postcardManiaMetaData.Accounts;
			formMassPostcardSetup.ShowDialog();
			if(formMassPostcardSetup.DialogResult==DialogResult.OK) {
				RefreshData();
				if(string.IsNullOrWhiteSpace(PrefC.GetString(PrefName.AdvertisingPostCardGuid)) && !_postcardManiaMetaData.Accounts.IsNullOrEmpty()) {
					Prefs.UpdateString(PrefName.AdvertisingPostCardGuid,_postcardManiaMetaData.Accounts.FirstOrDefault().Guid);
					Signalods.SetInvalid(InvalidType.Prefs);
				}
				RefreshView();
			}
		}

		private void butManageAccount_Click(object sender,EventArgs e) {
			ManageMassPostcardAccount();
		}

		private void gridAccounts_CellClick(object sender,ODGridClickEventArgs e) {
			//Toggling the 'X' in the defualt column.
			int selectedIndex=gridAccounts.GetSelectedIndex();
			if(gridAccounts.SelectedCell.X!=0 || selectedIndex==-1) {
				return;
			}
			PostcardManiaAccountData accountData=gridAccounts.SelectedTag<PostcardManiaAccountData>();
			if(accountData==null) {
				return;
			}
			gridAccounts.BeginUpdate();
			for(int i=0;i<gridAccounts.ListGridRows.Count;i++) {
				gridAccounts.ListGridRows[i].Cells[0].Text="";
			}
			gridAccounts.EndUpdate();
			Prefs.UpdateString(PrefName.AdvertisingPostCardGuid,accountData.Guid);
			Signalods.SetInvalid(InvalidType.Prefs);
			RefreshView();
		}

		private void gridAccounts_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			int selectedIndex=gridAccounts.GetSelectedIndex();
			if(gridAccounts.SelectedCell.X<=0 || selectedIndex==-1) {
				return;
			}
			PostcardManiaAccountData accountData=gridAccounts.SelectedTag<PostcardManiaAccountData>();
			if(accountData==null) {
				return;
			}
			ManageMassPostcardAccount(accountData);
		}

		private void gridClinics_CellSelectionCommitted(object sender,ODGridClickEventArgs e) {
			GridRow row=gridClinics.ListGridRows[e.Row];
			int accountIndex=row.Cells[e.Col].ComboSelectedIndex;
			string guid=_postcardManiaMetaData.Accounts[accountIndex].Guid;
			Clinic clinic=(Clinic)row.Tag;
			bool doInvalidate=false;
			if(accountIndex>0) {
				doInvalidate=ClinicPrefs.Upsert(PrefName.AdvertisingPostCardGuid,clinic.ClinicNum,guid);
			}
			else {
				doInvalidate=ClinicPrefs.DeletePrefs(clinic.ClinicNum,ListTools.FromSingle(PrefName.AdvertisingPostCardGuid))!=0;
			}
			if(doInvalidate) {
				DataValid.SetInvalid(InvalidType.ClinicPrefs);
			}
			row.Cells[1].Text=_postcardManiaMetaData.Accounts[accountIndex].AccountTitle;
		}
	}
}