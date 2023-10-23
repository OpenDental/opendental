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

		private Program _program;
		private ErxOption _erxOption;
		private List<ProgramProperty> _listProgramProperties=new List<ProgramProperty>();

		private ProgramProperty ErxOptionPP {
			get {
				ProgramProperty programProperty=_listProgramProperties.FirstOrDefault(x => x.PropertyDesc==Erx.PropertyDescs.ErxOption);
				if(programProperty==null) {
					throw new Exception("The database is missing an eRx option program property.");
				}
				return programProperty;
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
				_program=Programs.GetCur(ProgramName.eRx);
				if(_program==null) {
					throw new Exception("The eRx bridge is missing from the database.");
				}
				_listProgramProperties=ProgramProperties.GetForProgram(_program.ProgramNum);
				checkEnabled.Checked=_program.Enabled;
				_erxOption=PIn.Enum<ErxOption>(ErxOptionPP.PropertyValue);
				if(_erxOption==ErxOption.NewCrop) {
					radioNewCrop.Checked=true;
				}
				else if(_erxOption==ErxOption.DoseSpot) {
					radioDoseSpot.Checked=true;
					//HideLegacy();
				}
				else if(_erxOption==ErxOption.DoseSpotWithNewCrop) {
					radioDoseSpotLegacy.Checked=true;
					//HideLegacy();
				}
				textNewCropAccountID.Text=PrefC.GetString(PrefName.NewCropAccountId);
				List<ProgramProperty> listProgramPropertiesClinicIDs=_listProgramProperties.FindAll(x => x.PropertyDesc==Erx.PropertyDescs.ClinicID);
				List<ProgramProperty> listProgramPropertiesClinicKeys=_listProgramProperties.FindAll(x => x.PropertyDesc==Erx.PropertyDescs.ClinicKey);
				//Always make sure clinicnum 0 (HQ) exists, regardless of if clinics are enabled
				if(!listProgramPropertiesClinicIDs.Exists(x => x.ClinicNum==0)) {
					ProgramProperty programProperty=new ProgramProperty();
					programProperty.ProgramNum=_program.ProgramNum;
					programProperty.ClinicNum=0;
					programProperty.PropertyDesc=Erx.PropertyDescs.ClinicID;
					programProperty.PropertyValue="";
					_listProgramProperties.Add(programProperty);
				}
				if(!listProgramPropertiesClinicKeys.Exists(x => x.ClinicNum==0)) {
					ProgramProperty programProperty=new ProgramProperty();
					programProperty.ProgramNum=_program.ProgramNum;
					programProperty.ClinicNum=0;
					programProperty.PropertyDesc=Erx.PropertyDescs.ClinicKey;
					programProperty.PropertyValue="";
					_listProgramProperties.Add(programProperty);
				}
				if(PrefC.HasClinicsEnabled) {
					foreach(Clinic clinicCur in Clinics.GetAllForUserod(Security.CurUser)) {
						if(!listProgramPropertiesClinicIDs.Exists(x => x.ClinicNum==clinicCur.ClinicNum)) {//Only add a program property if it doesn't already exist.
							ProgramProperty programProperty=new ProgramProperty();
							programProperty.ProgramNum=_program.ProgramNum;
							programProperty.ClinicNum=clinicCur.ClinicNum;
							programProperty.PropertyDesc=Erx.PropertyDescs.ClinicID;
							programProperty.PropertyValue="";
							_listProgramProperties.Add(programProperty);
						}
						if(!listProgramPropertiesClinicKeys.Exists(x => x.ClinicNum==clinicCur.ClinicNum)) {//Only add a program property if it doesn't already exist.
							ProgramProperty programProperty=new ProgramProperty();
							programProperty.ProgramNum=_program.ProgramNum;
							programProperty.ClinicNum=clinicCur.ClinicNum;
							programProperty.PropertyDesc=Erx.PropertyDescs.ClinicKey;
							programProperty.PropertyValue="";
							_listProgramProperties.Add(programProperty);
						}
					}
				}
				else {
					checkShowHiddenClinics.Visible=false;
				}
				FillGridDoseSpot();
				SetRadioButtonChecked(_erxOption);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error loading the eRx program: ")+ex.Message);
				DialogResult=DialogResult.Cancel;
				return;
			}
		}
		
		private void FillGridDoseSpot() {
			gridProperties.BeginUpdate();
			gridProperties.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Clinic"),120);
			gridProperties.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Clinic ID"),160);
			gridProperties.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Clinic Key"),160);
			gridProperties.Columns.Add(col);
			gridProperties.ListGridRows.Clear();
			List<ProgramProperty> listProgramPropertiesHQClinicIDs=GetPropertyForClinic(0,Erx.PropertyDescs.ClinicID);
			List<ProgramProperty> listProgramPropertiesHQClinicKeys=GetPropertyForClinic(0,Erx.PropertyDescs.ClinicKey);
			//Should only be one, but if there are more we need to display them so they can be deleted.
			for(int i = 0;i<listProgramPropertiesHQClinicIDs.Count();i++) {
				DoseSpotGridRowModel doseSpotGridRowModel=new DoseSpotGridRowModel();
				doseSpotGridRowModel.ClinicCur=new Clinic();
				doseSpotGridRowModel.ClinicCur.ClinicNum=0;
				doseSpotGridRowModel.ClinicCur.Abbr=Lan.g(this,"Headquarters");
				try {
					doseSpotGridRowModel.ProgramPropertyClinicID=listProgramPropertiesHQClinicIDs[i];
					doseSpotGridRowModel.ProgramPropertyClinicKey=listProgramPropertiesHQClinicKeys[i];
					gridProperties.ListGridRows.Add(CreateDoseSpotGridRow(doseSpotGridRowModel));
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
					List<ProgramProperty> listProgramPropertiesClinicIDs=GetPropertyForClinic(clinicCur.ClinicNum,Erx.PropertyDescs.ClinicID);
					List<ProgramProperty> listProgramPropertiesClinicKeys=GetPropertyForClinic(clinicCur.ClinicNum,Erx.PropertyDescs.ClinicKey);
					//Each clinic should only have one set of properties but display all of them in case there was an error during setup.
					for(int i = 0;i<listProgramPropertiesClinicIDs.Count();i++) {
						DoseSpotGridRowModel doseSpotGridRowModel=new DoseSpotGridRowModel();
						try {
							doseSpotGridRowModel.ClinicCur=clinicCur.Copy();
							doseSpotGridRowModel.ProgramPropertyClinicID=listProgramPropertiesClinicIDs[i];
							doseSpotGridRowModel.ProgramPropertyClinicKey=listProgramPropertiesClinicKeys[i];
							gridProperties.ListGridRows.Add(CreateDoseSpotGridRow(doseSpotGridRowModel));
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

		private GridRow CreateDoseSpotGridRow(DoseSpotGridRowModel doseSpotGridRowModel) {
			GridRow row=new GridRow();
			row.Cells.Add(doseSpotGridRowModel.ClinicCur.Abbr);
			row.Cells.Add(doseSpotGridRowModel.ProgramPropertyClinicID==null ? "" : doseSpotGridRowModel.ProgramPropertyClinicID.PropertyValue);
			row.Cells.Add(doseSpotGridRowModel.ProgramPropertyClinicKey==null ? "" : doseSpotGridRowModel.ProgramPropertyClinicKey.PropertyValue);
			row.Tag=doseSpotGridRowModel;
			return row;
		}

		///<summary>This method should only ever return one item but in the event that an error has occurred setting up the DoseSpot account we need to know if there are multiple sets of credentials in the db.</summary>
		private List<ProgramProperty> GetPropertyForClinic(long clinicNum,string propDesc) {
			return _listProgramProperties.FindAll(x => x.ClinicNum==clinicNum && x.PropertyDesc==propDesc);
		}

		///<summary>All references removed in I12045.</summary>
		private void HideNewCrop() {
			radioNewCrop.Visible=false;
			radioDoseSpotLegacy.Location=radioDoseSpot.Location;
			radioDoseSpot.Location=radioNewCrop.Location;
			groupErxOptions.Size=new Size(groupErxOptions.Size.Width,radioDoseSpotLegacy.Location.Y+radioDoseSpotLegacy.Height+5);
		}

		private void SetRadioButtonChecked(ErxOption erxOption) {
			_erxOption=erxOption;
			if(erxOption==ErxOption.NewCrop) {
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
			SetRadioButtonChecked(ErxOption.NewCrop);
		}

		private void radioDoseSpot_Click(object sender,EventArgs e) {
			MsgBox.Show(this,"This enables the DoseSpot program link only.  You must contact support to cancel current eRx NewCrop charges and sign up for DoseSpot.");
			SetRadioButtonChecked(ErxOption.DoseSpot);
		}

		private void radioDoseSpotLegacy_Click(object sender,EventArgs e) {
			MsgBox.Show(this,"This enables the program links only. You must contact support to sign up for eRx.");
			SetRadioButtonChecked(ErxOption.DoseSpotWithNewCrop);
		}

		private void butClearAccountId_Click(object sender,EventArgs e) {
			using InputBox inputBox=new InputBox("Please enter password");
			inputBox.setTitle("Clear Account ID");
			inputBox.textResult.PasswordChar='*';
			inputBox.ShowDialog();
			if(inputBox.DialogResult!=DialogResult.OK) {
				return;
			}
			if(inputBox.textResult.Text!="eRxTeamPW") {
				MsgBox.Show(this,"Wrong password");
				return;
			}
			Pref pref=Prefs.GetPref(PrefName.NewCropAccountId.ToString());
			pref.ValueString="";
			Prefs.Update(pref);
			textNewCropAccountID.Text="";
			DataValid.SetInvalid(InvalidType.Prefs);
		}

		private void gridProperties_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			DoseSpotGridRowModel doseSpotGridRowModel=(DoseSpotGridRowModel)gridProperties.ListGridRows[e.Row].Tag;
			using FormDoseSpotPropertyEdit formDoseSpotPropertyEdit=new FormDoseSpotPropertyEdit(doseSpotGridRowModel.ClinicCur,doseSpotGridRowModel.ProgramPropertyClinicID.PropertyValue,doseSpotGridRowModel.ProgramPropertyClinicKey.PropertyValue,_listProgramProperties);
			formDoseSpotPropertyEdit.ShowDialog();
			if(formDoseSpotPropertyEdit.DialogResult==DialogResult.OK) {
				int clinicIdPos=_listProgramProperties.FindIndex(x => x.ClinicNum==doseSpotGridRowModel.ProgramPropertyClinicID.ClinicNum && x.PropertyDesc==doseSpotGridRowModel.ProgramPropertyClinicID.PropertyDesc);
				_listProgramProperties[clinicIdPos].PropertyValue=formDoseSpotPropertyEdit.ClinicIdVal;
				int clinicKeyPos=_listProgramProperties.FindIndex(x => x.ClinicNum==doseSpotGridRowModel.ProgramPropertyClinicKey.ClinicNum && x.PropertyDesc==doseSpotGridRowModel.ProgramPropertyClinicKey.PropertyDesc);
				_listProgramProperties[clinicKeyPos].PropertyValue=formDoseSpotPropertyEdit.ClinicKeyVal;
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
			DoseSpotGridRowModel doseSpotGridRowModel=gridProperties.SelectedTag<DoseSpotGridRowModel>();
			//_listProgramProperties is synced against the cache on OK click, removing these from the list will cause them to be deleted.
			_listProgramProperties.RemoveAll(x => x.ProgramPropertyNum==doseSpotGridRowModel.ProgramPropertyClinicID.ProgramPropertyNum);
			_listProgramProperties.RemoveAll(x => x.ProgramPropertyNum==doseSpotGridRowModel.ProgramPropertyClinicKey.ProgramPropertyNum);
			FillGridDoseSpot();
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(checkEnabled.Checked && !Programs.IsEnabledByHq(ProgramName.eRx,out string err)) {
				MsgBox.Show(err);
				return;
			}
			ErxOptionPP.PropertyValue=POut.Int((int)_erxOption);
			_program.Enabled=checkEnabled.Checked;
			Programs.Update(_program);
			ProgramProperties.Sync(_listProgramProperties,_program.ProgramNum);
			DataValid.SetInvalid(InvalidType.Programs);
			DialogResult=DialogResult.OK;
		}

		private class DoseSpotGridRowModel {
			public Clinic ClinicCur;
			public ProgramProperty ProgramPropertyClinicID;
			public ProgramProperty ProgramPropertyClinicKey;

			public DoseSpotGridRowModel() {
			}
		}

	}
}