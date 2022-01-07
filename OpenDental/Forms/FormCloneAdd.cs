using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class FormCloneAdd:FormODBase {
		///<summary>Shallow copy of the "master" patient that was passed into the constructor that the clone will be created from.</summary>
		private Patient _patientMaster;
		private Family _familyCur;
		private List<InsPlan> _listInsPlans;
		private List<InsSub> _listInsSubs;
		private List<Benefit> _listBenefits;
		private long _provNumSelected;
		private List<Provider> _listProviders;
		///<summary>A dictionary of specialties (key: DefNum) and the clinics that are set to that specialty (value: List&lt;Clinic&gt;)
		///If no specialties are present then this dictionary will have a single entry with a key of 0 with a list of all clinics available to the user.
		///Only filled on load if clinics are enabled.</summary>
		private Dictionary<long,List<Clinic>> _dictSpecialtyClinics;
		///<summary>Will be set to the PatNum of the new clone that was created if the user actually created one.</summary>
		public long PatNumClone;

		///<summary>Patient must be the original or master patient that will have a clone made from them.</summary>
		public FormCloneAdd(Patient patientMaster,Family familyCur=null,List<InsPlan> listInsPlans=null,List<InsSub> listInsSubs=null
			,List<Benefit> listBenefits=null) 
		{
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patientMaster=patientMaster;
			_familyCur=familyCur;
			_listInsSubs=listInsSubs;
			_listInsPlans=listInsPlans;
			_listBenefits=listBenefits;
		}

		private void FormCloneAdd_Load(object sender,EventArgs e) {
			//Make sure that this patient is not already a clone of another patient.  We don't allow clones of clones.
			//PatientLink.PatNumFrom is the master and PatientLink.PatNumTo is the actual clone.  We care if this patient has ever been a "PatNumTo".
			if(PatientLinks.IsPatientAClone(_patientMaster.PatNum)) {
				MsgBox.Show(this,"Cannot create a clone of a clone.  Please select the original patient in order to create another clone.");
				return;
			}
			textLName.Text=_patientMaster.LName;
			textFName.Text=_patientMaster.FName;
			textPreferred.Text=_patientMaster.Preferred;
			textMiddleI.Text=_patientMaster.MiddleI;
			textBirthDate.Text=(_patientMaster.Birthdate.Year < 1880) ? "" : _patientMaster.Birthdate.ToShortDateString();
			textAge.Text=PatientLogic.DateToAgeString(_patientMaster.Birthdate,_patientMaster.DateTimeDeceased);
			//We intentionally don't synch the patient's provider since the clone feature is so the clone can be assigned to a different provider for tracking production.
			_provNumSelected=PrefC.GetLong(PrefName.PracticeDefaultProv);
			_listProviders=Providers.GetDeepCopy(true);
			comboPriProv.Items.Clear();
			for(int i = 0;i<_listProviders.Count;i++) {
				comboPriProv.Items.Add(_listProviders[i].GetLongDesc());
				if(_listProviders[i].ProvNum==_provNumSelected) {
					comboPriProv.SelectedIndex=i;
				}
			}
			if(_provNumSelected==0) {
				comboPriProv.SelectedIndex=0;
				_provNumSelected=_listProviders[0].ProvNum;
			}
			if(comboPriProv.SelectedIndex==-1) {
				comboPriProv.Text=Providers.GetLongDesc(_provNumSelected);
			}
			labelSpecialty.Visible=true;
			comboSpecialty.Visible=true;
			if(PrefC.HasClinicsEnabled) {
				labelClinic.Visible=true;
				comboClinic.Visible=true;
				FillClinicComboBoxes();
			}
			else{//Without clinics enabled the specialty box is filled differently.
				FillComboSpecialtyNoClinics();
			}
		}

		///<summary>Fills both the Specialty and Clinic combo boxes according to the available clinics to the user and the unused specialties for the patient.
		///Only fills the combo box of clinics with clinics that are associated to specialties that have not been used for this patient yet.
		///E.g. Even if the user has access to Clinic X, if there is already a clone of this patient for Clinic X, it will no longer show.
		///Throws exceptions that should be shown to the user which should then be followed by closing the window.</summary>
		private void FillClinicComboBoxes() {
			_dictSpecialtyClinics=new Dictionary<long,List<Clinic>>();
			//Fill the list of clinics for this user.
			List<Clinic> listClinicsForUser=Clinics.GetForUserod(Security.CurUser);
			//Make a deep copy of the list of clinics so that we can filter down to the clinics that have no specialty specified if all are hidden.
			List<Clinic> listClinicsNoSpecialty=listClinicsForUser.Select(x => x.Copy()).ToList();
			//Fill the list of defLinks used by clones of this patient.
			List<long> listClonePatNum=PatientLinks.GetPatNumsLinkedFrom(_patientMaster.PatNum,PatientLinkType.Clone);
			List<DefLink> listPatCurDefLinks=DefLinks.GetListByFKeys(listClonePatNum,DefLinkType.Patient);
			//Fill the list of clinics defLink
			List<DefLink> listClinicDefLinks=DefLinks.GetDefLinksByType(DefLinkType.Clinic);
			//Filter out any specialties that are currently in use by clones of this patient.
			if(listPatCurDefLinks.Count>0) {
				listClinicDefLinks.RemoveAll(x => ListTools.In(x.DefNum,listPatCurDefLinks.Select(y => y.DefNum).ToList()));
			}
			//Get all non-hidden specialties
			List<Def> listSpecialtyDefs=Defs.GetDefsForCategory(DefCat.ClinicSpecialty,true);
			//If there are specialties present, we need to know which clinics have no specialty set so that the user can always make clones for that specialty.
			if(listSpecialtyDefs.Count > 0) {
				listClinicsNoSpecialty.RemoveAll(x => ListTools.In(x.ClinicNum,listClinicDefLinks.Select(y => y.FKey).ToList()));
			}
			//Remove all clinics that do not have any specialties from the original list of clinics for the user.
			listClinicsForUser.RemoveAll(x => !ListTools.In(x.ClinicNum,listClinicDefLinks.Select(y => y.FKey).ToList()));
			//Filter out any specialties that are not associated to any available clinics for this user.
			listSpecialtyDefs.RemoveAll(x => !ListTools.In(x.DefNum,listClinicDefLinks.Select(y => y.DefNum).ToList()));
			//Lump all of the left over specialties into a dictionary and slap the associated clinics to them.
			comboSpecialty.Items.Clear();
			//Create a dummy specialty of 0 if there are any clinics that do not have a specialty.
			if(listClinicsNoSpecialty!=null && listClinicsNoSpecialty.Count > 0) {
				comboSpecialty.Items.Add(Lan.g(this,"Unspecified"),new Def() { DefNum=0 });
				_dictSpecialtyClinics[0]=listClinicsNoSpecialty;
			}
			foreach(Def specialty in listSpecialtyDefs) {
				comboSpecialty.Items.Add(specialty.ItemName,specialty);
				//Get a list of all deflinks for the def
				List<DefLink> listLinkForDef=listClinicDefLinks.FindAll(x => x.DefNum==specialty.DefNum).ToList();
				_dictSpecialtyClinics[specialty.DefNum]=listClinicsForUser.FindAll(x => ListTools.In(x.ClinicNum,listLinkForDef.Select(y => y.FKey).ToList()));
			}
			//If there are no specialties to show, we need to let the user know that they need to associate at least one clinic to a specialty.
			if(_dictSpecialtyClinics.Count < 1) {
				MsgBox.Show(this,"This patient already has a clone for every Clinic Specialty available.\r\n"
					+"In the main menu, click Setup, Definitions, Clinic Specialties category to add new specialties.\r\n"
					+"In the main menu, click Lists, Clinics, and double click a clinic to set a Specialty.");
				DialogResult=DialogResult.Abort;
				return;
			}
			comboSpecialty.SelectedIndex=0;
			FillComboClinic();
		}

		private void FillComboClinic() {
			if(!PrefC.HasClinicsEnabled) {
				return;
			}
			comboClinic.Items.Clear();
			if(comboSpecialty.GetSelected<Def>()==null || comboSpecialty.GetSelected<Def>().GetType()!=typeof(Def)) {
				return;//Somehow the specialty box changed to an invalid item.  Nothing else to do.
			}
			//Only allow the Unassigned clinic for the Unspecified specialty.
			if(comboSpecialty.SelectedIndex==0 
				&& comboSpecialty.GetSelected<Def>()!=null
				&& comboSpecialty.GetSelected<Def>().DefNum==0)
			{
				comboClinic.Items.Add(Lan.g(this,"Unassigned"),new Clinic());
			}
			foreach(Clinic clinic in _dictSpecialtyClinics[comboSpecialty.GetSelected<Def>().DefNum]) {
				comboClinic.Items.Add(clinic.Abbr,clinic);
			}
		}

		///<summary>Used in the case when clinics are disabled. Requires special logic that doesn't use clinics.</summary>
		private void FillComboSpecialtyNoClinics() {
			//Get all non-hidden specialties
			List<Def> listSpecialtyDefs=Defs.GetDefsForCategory(DefCat.ClinicSpecialty,true);
			//Fill the list of defLinks used by clones of this patient.
			List<long> listClonePatNums=PatientLinks.GetPatNumsLinkedFrom(_patientMaster.PatNum,PatientLinkType.Clone);
			List<DefLink> listPatCurDefLinks=DefLinks.GetListByFKeys(listClonePatNums,DefLinkType.Patient);
			//Filter out any specialties that are currently in use by clones of this patient.
			if(listPatCurDefLinks.Count>0) {
				listSpecialtyDefs.RemoveAll(x => ListTools.In(x.DefNum,listPatCurDefLinks.Select(y => y.DefNum).ToList()));
			}
			comboSpecialty.Items.Clear();
			//Create a dummy specialty of 0.  Always allow the user to make Unspecified clones.
			comboSpecialty.Items.Add(Lan.g(this,"Unspecified"),new Def() { DefNum=0 });
			foreach(Def specialty in listSpecialtyDefs) {
				comboSpecialty.Items.Add(specialty.ItemName,specialty);
			}
			comboSpecialty.SelectedIndex=0;
		}

		private void butPickPrimary_Click(object sender,EventArgs e) {
			using FormProviderPick FormPP=new FormProviderPick(_listProviders);
			if(comboPriProv.SelectedIndex > -1) {//Initial FormP selection if selected prov is not hidden.
				FormPP.SelectedProvNum=_provNumSelected;
			}
			FormPP.ShowDialog();
			if(FormPP.DialogResult!=DialogResult.OK) {
				return;
			}
			comboPriProv.SelectedIndex=_listProviders.FindIndex(x => x.ProvNum==FormPP.SelectedProvNum);
			_provNumSelected=FormPP.SelectedProvNum;
		}

		private void comboPriProv_SelectionChangeCommitted(object sender,EventArgs e) {
			_provNumSelected=_listProviders[comboPriProv.SelectedIndex].ProvNum;
		}

		///<summary>The clinic combo box needs to get refilled every time the specialty changes.</summary>
		private void comboSpecialty_SelectionChangeCommitted(object sender,EventArgs e) {
			FillComboClinic();
		}

		///<summary>Validates the form and will show a message to the user if something is invalid and then will return false, otherwise true.</summary>
		private bool IsValid() {
			if(_provNumSelected < 1) {
				MsgBox.Show(this,"Invalid Primary Provider selected.");
				return false;
			}
			if(PrefC.HasClinicsEnabled) {
				#region Clinic Specific Validation
				if(comboSpecialty.GetSelected<Def>()!=null && comboSpecialty.GetSelected<Def>().GetType()!=typeof(Def)) {
					MsgBox.Show(this,"Invalid Specialty selected.");
					return false;
				}
				if(comboSpecialty.SelectedIndex < 0) {
					MsgBox.Show(this,"A Specialty is required in order to create a clone.");
					return false;
				}
				if(comboClinic.SelectedIndex < 0) {
					MsgBox.Show(this,"A Clinic is required in order to create a clone.");
					return false;
				}
				#endregion
			}
			return true;
		}

		private void butClone_Click(object sender,EventArgs e) {
			if(!IsValid()) {
				return;//A message should have already shown to the user.
			}
			long clinicNum=0;
			long defNum=0;
			if(PrefC.HasClinicsEnabled) {
				clinicNum=comboClinic.GetSelected<Clinic>().ClinicNum;
			}
			defNum=comboSpecialty.GetSelected<Def>().DefNum;
			Patient clone=Patients.CreateCloneAndSynch(_patientMaster,_familyCur,_listInsPlans,_listInsSubs,_listBenefits,_provNumSelected,clinicNum);
			if(clone!=null) {
				PatNumClone=clone.PatNum;
				if(defNum!=0) {
					DefLinks.Insert(new DefLink() {
						DefNum=defNum,
						FKey=PatNumClone,
						LinkType=DefLinkType.Patient,
					});
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}