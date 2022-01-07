using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using OpenDental.UI;

namespace OpenDental {
	///<summary>This form is used to make changes for the eRx program link.
	///With the integration of DoseSpot, the default program link form is no longer sufficient.</summary>
	public partial class FormErxSetup:FormODBase {

		private Program _progCur;
		private ErxOption _eRxOption;
		private List<ProgramProperty> _listProgramProperties=new List<ProgramProperty>();

		private ProgramProperty ErxOptionPP {
			get {
				ProgramProperty retVal=_listProgramProperties.FirstOrDefault(x => x.PropertyDesc==Erx.PropertyDescs.ErxOption);
				if(retVal==null) {
					throw new Exception("The database is missing an eRx option program property.");
				}
				return retVal;
			}
			set {
				int pos=_listProgramProperties.IndexOf(ErxOptionPP);
				_listProgramProperties[pos]=value;
			}
		}

		public FormErxSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormErxSetup_Load(object sender,EventArgs e) {
			try {
				_progCur=Programs.GetCur(ProgramName.eRx);
				if(_progCur==null) {
					throw new Exception("The eRx bridge is missing from the database.");
				}
				_listProgramProperties=ProgramProperties.GetForProgram(_progCur.ProgramNum);
				checkEnabled.Checked=_progCur.Enabled;
				_eRxOption=PIn.Enum<ErxOption>(ErxOptionPP.PropertyValue);
				if(_eRxOption==ErxOption.Legacy) {
					radioNewCrop.Checked=true;
				}
				else if(_eRxOption==ErxOption.DoseSpot) {
					radioDoseSpot.Checked=true;
					//HideLegacy();
				}
				else if(_eRxOption==ErxOption.DoseSpotWithLegacy) {
					radioDoseSpotLegacy.Checked=true;
					//HideLegacy();
				}
				textNewCropAccountID.Text=PrefC.GetString(PrefName.NewCropAccountId);
				List<ProgramProperty> listClinicIDs=_listProgramProperties.FindAll(x => x.PropertyDesc==Erx.PropertyDescs.ClinicID);
				List<ProgramProperty> listClinicKeys=_listProgramProperties.FindAll(x => x.PropertyDesc==Erx.PropertyDescs.ClinicKey);
				//Always make sure clinicnum 0 (HQ) exists, regardless of if clinics are enabled
				if(!listClinicIDs.Exists(x => x.ClinicNum==0)) {
					ProgramProperty ppClinicID=new ProgramProperty();
					ppClinicID.ProgramNum=_progCur.ProgramNum;
					ppClinicID.ClinicNum=0;
					ppClinicID.PropertyDesc=Erx.PropertyDescs.ClinicID;
					ppClinicID.PropertyValue="";
					_listProgramProperties.Add(ppClinicID);
				}
				if(!listClinicKeys.Exists(x => x.ClinicNum==0)) {
					ProgramProperty ppClinicKey=new ProgramProperty();
					ppClinicKey.ProgramNum=_progCur.ProgramNum;
					ppClinicKey.ClinicNum=0;
					ppClinicKey.PropertyDesc=Erx.PropertyDescs.ClinicKey;
					ppClinicKey.PropertyValue="";
					_listProgramProperties.Add(ppClinicKey);
				}
				if(PrefC.HasClinicsEnabled) {
					foreach(Clinic clinicCur in Clinics.GetAllForUserod(Security.CurUser)) {
						if(!listClinicIDs.Exists(x => x.ClinicNum==clinicCur.ClinicNum)) {//Only add a program property if it doesn't already exist.
							ProgramProperty ppClinicID=new ProgramProperty();
							ppClinicID.ProgramNum=_progCur.ProgramNum;
							ppClinicID.ClinicNum=clinicCur.ClinicNum;
							ppClinicID.PropertyDesc=Erx.PropertyDescs.ClinicID;
							ppClinicID.PropertyValue="";
							_listProgramProperties.Add(ppClinicID);
						}
						if(!listClinicKeys.Exists(x => x.ClinicNum==clinicCur.ClinicNum)) {//Only add a program property if it doesn't already exist.
							ProgramProperty ppClinicKey=new ProgramProperty();
							ppClinicKey.ProgramNum=_progCur.ProgramNum;
							ppClinicKey.ClinicNum=clinicCur.ClinicNum;
							ppClinicKey.PropertyDesc=Erx.PropertyDescs.ClinicKey;
							ppClinicKey.PropertyValue="";
							_listProgramProperties.Add(ppClinicKey);
						}
					}
				}
				else {
					checkShowHiddenClinics.Visible=false;
				}
				FillGridDoseSpot();
				SetRadioButtonChecked(_eRxOption);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error loading the eRx program: ")+ex.Message);
				DialogResult=DialogResult.Cancel;
				return;
			}
		}
		
		private void FillGridDoseSpot() {
			gridProperties.BeginUpdate();
			gridProperties.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Clinic"),120);
			gridProperties.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Clinic ID"),160);
			gridProperties.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Clinic Key"),160);
			gridProperties.ListGridColumns.Add(col);
			gridProperties.ListGridRows.Clear();
			List<ProgramProperty> listHQClinicIDs=GetPropertyForClinic(0,Erx.PropertyDescs.ClinicID);
			List<ProgramProperty> listHQClinicKeys=GetPropertyForClinic(0,Erx.PropertyDescs.ClinicKey);
			//Should only be one, but if there are more we need to display them so they can be deleted.
			for(int i = 0;i<listHQClinicIDs.Count();i++) {
				DoseSpotGridRowModel clinicHqModel=new DoseSpotGridRowModel();
				clinicHqModel.Clinic=new Clinic();
				clinicHqModel.Clinic.ClinicNum=0;
				clinicHqModel.Clinic.Abbr=Lan.g(this,"Headquarters");
				try {
					clinicHqModel.ClinicIDProperty=listHQClinicIDs[i];
					clinicHqModel.ClinicKeyProperty=listHQClinicKeys[i];
					gridProperties.ListGridRows.Add(CreateDoseSpotGridRow(clinicHqModel));
				}
				catch {
					//The ClinicID and ClinicKey program properties should always be made as pairs but try catch just in case there is one without a match.
					continue;
				}
			}
			if(PrefC.HasClinicsEnabled) {
				foreach(Clinic clinicCur in Clinics.GetAllForUserod(Security.CurUser)) {
					if(!checkShowHiddenClinics.Checked && clinicCur.IsHidden) {
						continue;
					}
					List<ProgramProperty> listClinicIDs=GetPropertyForClinic(clinicCur.ClinicNum,Erx.PropertyDescs.ClinicID);
					List<ProgramProperty> listClinicKeys=GetPropertyForClinic(clinicCur.ClinicNum,Erx.PropertyDescs.ClinicKey);
					//Each clinic should only have one set of properties but display all of them in case there was an error during setup.
					for(int i = 0;i<listClinicIDs.Count();i++) {
						DoseSpotGridRowModel model=new DoseSpotGridRowModel();
						try {
							model.Clinic=clinicCur.Copy();
							model.ClinicIDProperty=listClinicIDs[i];
							model.ClinicKeyProperty=listClinicKeys[i];
							gridProperties.ListGridRows.Add(CreateDoseSpotGridRow(model));
						}
						catch {
							//The ClinicID and ClinicKey program properties should always be made as pairs but try catch just in case there is one without a match.
							continue;
						}
					}

				}
			}
			gridProperties.EndUpdate();
		}

		private GridRow CreateDoseSpotGridRow(DoseSpotGridRowModel model) {
			GridRow row=new GridRow();
			row.Cells.Add(model.Clinic.Abbr);
			row.Cells.Add(model.ClinicIDProperty==null ? "" : model.ClinicIDProperty.PropertyValue);
			row.Cells.Add(model.ClinicKeyProperty==null ? "" : model.ClinicKeyProperty.PropertyValue);
			row.Tag=model;
			return row;
		}

		///<summary>This method should only ever return one item but in the event that an error has occured setting up the DoseSpot account we need to know if there are multiple sets of credentials in the db.</summary>
		private List<ProgramProperty> GetPropertyForClinic(long clinicNum,string propDesc) {
			return _listProgramProperties.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc==propDesc);
		}

		///<summary>All references removed in I12045.</summary>
		private void HideLegacy() {
			radioNewCrop.Visible=false;
			radioDoseSpotLegacy.Location=radioDoseSpot.Location;
			radioDoseSpot.Location=radioNewCrop.Location;
			groupErxOptions.Size=new Size(groupErxOptions.Size.Width,radioDoseSpotLegacy.Location.Y+radioDoseSpotLegacy.Height+5);
		}

		private void SetRadioButtonChecked(ErxOption option) {
			_eRxOption=option;
			if(option==ErxOption.Legacy) {
				label7.Visible=true;
				textNewCropAccountID.Visible=true;
				butClearAccountId.Visible=true;
				checkShowHiddenClinics.Visible=false;
				gridProperties.Visible=false;
				butDelete.Visible=false;
			}
			else {
				//This will also display the DoseSpot controls if DoseSpotWithLegacy is checked.
				//This is important because the user is migrating away from Legacy to use DoseSpot.
				//Plus, the user cannot do anything in the NewCrop controls.
				label7.Visible=false;
				textNewCropAccountID.Visible=false;
				butClearAccountId.Visible=false;
				checkShowHiddenClinics.Visible=true;
				gridProperties.Visible=true;
				butDelete.Visible=true;
			}
		}

		private void checkShowHiddenClinics_CheckedChanged(object sender,EventArgs e) {
			FillGridDoseSpot();
		}

		private void radioNewCrop_Click(object sender,EventArgs e) {
			SetRadioButtonChecked(ErxOption.Legacy);
		}

		private void radioDoseSpot_Click(object sender,EventArgs e) {
			MsgBox.Show(this,"This enables the DoseSpot program link only.  You must contact support to cancel current eRx Legacy charges and sign up for DoseSpot.");
			SetRadioButtonChecked(ErxOption.DoseSpot);
		}

		private void radioDoseSpotLegacy_Click(object sender,EventArgs e) {
			MsgBox.Show(this,"This enables the program links only. You must contact support to sign up for eRx.");
			SetRadioButtonChecked(ErxOption.DoseSpotWithLegacy);
		}

		private void butClearAccountId_Click(object sender,EventArgs e) {
			using InputBox inputbox=new InputBox("Please enter password");
			inputbox.setTitle("Clear Account ID");
			inputbox.textResult.PasswordChar='*';
			inputbox.ShowDialog();
			if(inputbox.DialogResult!=DialogResult.OK) {
				return;
			}
			if(inputbox.textResult.Text!="eRxTeamPW") {
				MsgBox.Show(this,"Wrong password");
				return;
			}
			Pref prefNewCropAccount=Prefs.GetPref(PrefName.NewCropAccountId.ToString());
			prefNewCropAccount.ValueString="";
			Prefs.Update(prefNewCropAccount);
			textNewCropAccountID.Text="";
			DataValid.SetInvalid(InvalidType.Prefs);
		}

		private void gridProperties_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			DoseSpotGridRowModel model=(DoseSpotGridRowModel)gridProperties.ListGridRows[e.Row].Tag;
			using FormDoseSpotPropertyEdit FormDPE=new FormDoseSpotPropertyEdit(model.Clinic,model.ClinicIDProperty.PropertyValue,model.ClinicKeyProperty.PropertyValue,_listProgramProperties);
			FormDPE.ShowDialog();
			if(FormDPE.DialogResult==DialogResult.OK) {
				int clinicIdPos=_listProgramProperties.FindIndex(x => x.ClinicNum==model.ClinicIDProperty.ClinicNum && x.PropertyDesc==model.ClinicIDProperty.PropertyDesc);
				_listProgramProperties[clinicIdPos].PropertyValue=FormDPE.ClinicIdVal;
				int clinicKeyPos=_listProgramProperties.FindIndex(x => x.ClinicNum==model.ClinicKeyProperty.ClinicNum && x.PropertyDesc==model.ClinicKeyProperty.PropertyDesc);
				_listProgramProperties[clinicKeyPos].PropertyValue=FormDPE.ClinicKeyVal;
			}
			FillGridDoseSpot();//Always fill grid because clinics could have been edited in FormDoseSpotPropertyEdit.
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(gridProperties.SelectedGridRows.Count()!=1) {
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure you want to delete this row?")) {
				return;
			}
			DoseSpotGridRowModel model=gridProperties.SelectedTag<DoseSpotGridRowModel>();
			//_listProgramProperties is synced against the cache on OK click, removing these from the list will cause them to be deleted.
			_listProgramProperties.RemoveAll(x => x.ProgramPropertyNum==model.ClinicIDProperty.ProgramPropertyNum);
			_listProgramProperties.RemoveAll(x => x.ProgramPropertyNum==model.ClinicKeyProperty.ProgramPropertyNum);
			FillGridDoseSpot();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(checkEnabled.Checked && !Programs.IsEnabledByHq(ProgramName.eRx,out string err)) {
				MsgBox.Show(err);
				return;
			}
			ErxOptionPP.PropertyValue=POut.Int((int)_eRxOption);
			_progCur.Enabled=checkEnabled.Checked;
			Programs.Update(_progCur);
			ProgramProperties.Sync(_listProgramProperties,_progCur.ProgramNum);
			DataValid.SetInvalid(InvalidType.Programs);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private class DoseSpotGridRowModel {
			public Clinic Clinic;
			public ProgramProperty ClinicIDProperty;
			public ProgramProperty ClinicKeyProperty;

			public DoseSpotGridRowModel() {
			}
		}
	}
}