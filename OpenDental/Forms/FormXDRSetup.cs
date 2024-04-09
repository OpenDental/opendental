using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.Bridges;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormXDRSetup:FormODBase {
		private Program _program;
		private ProgramProperty _programPropertyPatNumOrChartNum;
		private ProgramProperty _programPropertyInfoFilePath;
		///<summary>Local cache of all of the clinic nums the current user has permission to access at the time the form loads.
		///Filled at the same time as comboClinic and is used to set programproperty.ClinicNum when saving.</summary>
		private List<long> _listClinicNumsUser;
		private List<ProgramProperty> _listProgramProperties;
		///<summary>Can be 0 for "Headquarters" or non clinic users.</summary>
		private long _clinicNum;
		private bool _hasProgramPropertyChanged;
		//The local path override for this computer when opening the form.  Used to check if programproperty needs to be updated/inserted.
		private string _pathOverrideOld;

		public FormXDRSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormXDRSetup_Load(object sender,EventArgs e) {
			if(PrefC.HasClinicsEnabled) {//Using clinics
				_listClinicNumsUser=new List<long>();
				comboClinic.Items.Clear();
				comboClinic.Items.Add(Lan.g(this,"Headquarters"));
				//This way both lists have the same number of items in it and if 'Headquarters' is selected the programproperty.ClinicNum will be set to 0
				_listClinicNumsUser.Add(0);
				comboClinic.SelectedIndex=0;
				_clinicNum=0;
				List<Clinic> listClinics=Clinics.GetForUserod(Security.CurUser);
				for(int i=0;i<listClinics.Count;i++) {
					comboClinic.Items.Add(listClinics[i].Abbr);
					_listClinicNumsUser.Add(listClinics[i].ClinicNum);
					if(Clinics.ClinicNum==listClinics[i].ClinicNum) {
						comboClinic.SelectedIndex=i;
						if(!Security.CurUser.ClinicIsRestricted) {
							comboClinic.SelectedIndex++;//increment the SelectedIndex to account for 'Headquarters' in the list at position 0 if the user is not restricted.
						}
						_clinicNum=_listClinicNumsUser[comboClinic.SelectedIndex];
					}
				}
			}
			else {//clinics are not enabled, use ClinicNum 0 to indicate 'Headquarters' or practice level program properties
				comboClinic.Visible=false;
				labelClinic.Visible=false;
				_listClinicNumsUser=new List<long>() { 0 };//if clinics are disabled, programproperty.ClinicNum will be set to 0
				_clinicNum=0;
			}
			_program=Programs.GetCur(ProgramName.XDR);
			if(_program==null) {
				MsgBox.Show(this,"The XDR bridge is missing from the database.");//should never happen
				DialogResult=DialogResult.Cancel;
				return;
			}
			long clinicNum=0;
			if(comboClinic.SelectedIndex>0) {//0 is always "All" so only check for greater than 0.
				clinicNum=_listClinicNumsUser[comboClinic.SelectedIndex];
			}
			_listProgramProperties=ProgramProperties.GetForProgram(_program.ProgramNum);
			_programPropertyPatNumOrChartNum=_listProgramProperties.Find(x => x.PropertyDesc==XDR.PropertyDescs.PatNumOrChartNum);
			_programPropertyInfoFilePath=_listProgramProperties.Find(x => x.PropertyDesc==XDR.PropertyDescs.InfoFilePath);
			if(_programPropertyPatNumOrChartNum is null || _programPropertyInfoFilePath is null) { 
				MsgBox.Show(this,"You are missing a program property for XDR.  Please contact support to resolve this issue.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			FillForm();
		}

		private void FillForm() {
			//ComboClinic is filled in the load method
			if(PIn.Int(_programPropertyPatNumOrChartNum.PropertyValue)==1) {
				radioChart.Checked=true;
			}
			else {
				radioPatient.Checked=true;
			}
			List<ToolButItem> listToolButItems=ToolButItems.GetForProgram(_program.ProgramNum);
			listToolBars.Items.Clear();
			listToolBars.Items.AddEnums<EnumToolBar>();
			for(int i=0;i<listToolButItems.Count;i++) {
				listToolBars.SetSelectedEnum(listToolButItems[i].ToolBar);
			}
			checkEnabled.Checked=_program.Enabled;
			textPath.Text=_program.Path;
			textButtonText.Text=listToolButItems[0].ButtonText;
			pictureBox.Image=PIn.Bitmap(_program.ButtonImage);
			ProgramProperty programProperty=_listProgramProperties.Find(x=>x.ClinicNum==_clinicNum && x.PropertyDesc==XDR.PropertyDescs.LocationID);
			if(programProperty!=null) {
				textLocationID.Text=programProperty.PropertyValue;
			}
			textInfoFile.Text=_programPropertyInfoFilePath.PropertyValue;
			_pathOverrideOld=ProgramProperties.GetLocalPathOverrideForProgram(_program.ProgramNum);
			textOverride.Text=_pathOverrideOld;
		}

		///<summary>Updates the in memory list with any changes made to the current locationID for each clinic before showing the next one.</summary>
		private void SaveLocationIdToList() {
			//First check if Headquarters (default) is selected.
			if(_clinicNum==0) {
				//Headquarters is selected so update the location ID (might have changed) on all other location ID properties that match the "old" location ID of HQ.
				ProgramProperty programProperty=_listProgramProperties.Find(x=>x.ClinicNum==0 && x.PropertyDesc==XDR.PropertyDescs.LocationID);
				if(programProperty==null) {
					return;//shouldn't happen
				}
				//Get the location ID so that we correctly update all program properties with a matching location ID.
				string locationIdOld=programProperty.PropertyValue;
				for(int i=0;i<_listProgramProperties.Count;i++) {
					//don't skip hq because we need to set it for hq and other matches
					if(_listProgramProperties[i].PropertyDesc!=XDR.PropertyDescs.LocationID) { 
						continue;
					}
					if(_listProgramProperties[i].PropertyValue!=locationIdOld) {
						continue;
					}
					_listProgramProperties[i].PropertyValue=textLocationID.Text;
				}
				return;
			}
			ProgramProperty programPropertyLocationID=_listProgramProperties.Find(x=>x.ClinicNum==_clinicNum && x.PropertyDesc==XDR.PropertyDescs.LocationID);
			if(programPropertyLocationID==null) {//this clinic does not yet have a pp for LocationID.
				ProgramProperty programPropertyLocationIDNew=new ProgramProperty();
				programPropertyLocationIDNew.ProgramNum=_program.ProgramNum;
				programPropertyLocationIDNew.PropertyDesc=XDR.PropertyDescs.LocationID;
				programPropertyLocationIDNew.ClinicNum=_clinicNum;
				programPropertyLocationIDNew.PropertyValue=textLocationID.Text;
				_listProgramProperties.Add(programPropertyLocationIDNew);
				return;
			}
			//update existing pp
			programPropertyLocationID.PropertyValue=textLocationID.Text;
		}

		private void SaveProgram() {
			SaveLocationIdToList();
			_program.Enabled=checkEnabled.Checked;
			_program.Path=textPath.Text;
			_program.ButtonImage=POut.Bitmap((Bitmap)pictureBox.Image,System.Drawing.Imaging.ImageFormat.Png);
			ToolButItems.DeleteAllForProgram(_program.ProgramNum);
			//Then add one toolButItem for each highlighted row in listbox
			ToolButItem toolButItem;
			for(int i=0;i<listToolBars.SelectedIndices.Count;i++) {
				toolButItem=new ToolButItem() {
					ProgramNum=_program.ProgramNum,
					ButtonText=textButtonText.Text,
					ToolBar=(EnumToolBar)listToolBars.SelectedIndices[i]
				};
				ToolButItems.Insert(toolButItem);
			}
			if(_pathOverrideOld!=textOverride.Text) {//If there was no previous override _pathOverrideOld will be empty string.
				_hasProgramPropertyChanged=true;
				ProgramProperties.InsertOrUpdateLocalOverridePath(_program.ProgramNum,textOverride.Text);
			}
			UpdateProgramProperty(_programPropertyPatNumOrChartNum,POut.Bool(radioChart.Checked));//Will need to be enhanced if another radio button ever gets added.
			UpdateProgramProperty(_programPropertyInfoFilePath,textInfoFile.Text);
			UpsertLocationIdsForClinics();
			Programs.Update(_program);
		}

		private void UpdateProgramProperty(ProgramProperty programPropertyFromDb,string newpropertyValue) {
			if(programPropertyFromDb.PropertyValue==newpropertyValue) {
				return;
			}
			programPropertyFromDb.PropertyValue=newpropertyValue;
			ProgramProperties.Update(programPropertyFromDb);
			_hasProgramPropertyChanged=true;
		}

		private void UpsertLocationIdsForClinics() {
			List<ProgramProperty> listProgramPropertiesDb=ProgramProperties.GetForProgram(_program.ProgramNum).FindAll(x => x.PropertyDesc==XDR.PropertyDescs.LocationID);
			for(int i=0;i<_listProgramProperties.Count;i++) { 
				ProgramProperty programProperty=listProgramPropertiesDb.Find(x => x.ProgramPropertyNum == _listProgramProperties[i].ProgramPropertyNum);
				if(programProperty is null) {//Program property for that clinicnum didn't exist, so insert it into the db.
					ProgramProperties.Insert(_listProgramProperties[i]);//Program property for that clinicnum didn't exist, so insert it into the db.
					_hasProgramPropertyChanged=true;
				}
				else {//update existing
					UpdateProgramProperty(programProperty,_listProgramProperties[i].PropertyValue);
				}
			}
		}

		private void ComboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			SaveLocationIdToList();
			_clinicNum=_listClinicNumsUser[comboClinic.SelectedIndex];
			//This will either display the HQ value, or the clinic specific value.
			textLocationID.Text=_listProgramProperties.Find(x=>x.ClinicNum==_clinicNum && x.PropertyDesc==XDR.PropertyDescs.LocationID).PropertyValue;
			if(textLocationID.Text==null) {
				textLocationID.Text=_listProgramProperties.Find(x=>x.ClinicNum==0).PropertyValue;//Default to showing the HQ value when filling info for a clinic with no program property.
				return;
			}
		}

		private void ButImport_Click(object sender,EventArgs e) {
			string importFilePath;
			if(!ODBuild.IsWeb() && ODCloudClient.IsAppStream) {
				importFilePath=ODCloudClient.ImportFileForCloud();
				if(importFilePath.IsNullOrEmpty()) {
					return; //User cancelled out of OpenFileDialog
				}
			}
			else {
				using OpenFileDialog openFileDialog=new OpenFileDialog();
				if(openFileDialog.ShowDialog()!=DialogResult.OK) {
					return;
				}
				importFilePath=openFileDialog.FileName;
			}
			Image imageImported;
			try {
				imageImported=Image.FromFile(importFilePath);
			}
			catch {
				MsgBox.Show(this,"Error loading file.");
				return;
			}
			if(imageImported.Size!=new Size(22,22)) {
					MessageBox.Show(Lan.g(this,"Required image dimensions are 22x22.")
						+"\r\n"+Lan.g(this,"Selected image dimensions are")+": "+imageImported.Size.Width+"x"+imageImported.Size.Height);
					return;
				}
			pictureBox.Image=imageImported;
		}

		private void ButClear_Click(object sender,EventArgs e) {
			pictureBox.Image=null;
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(checkEnabled.Checked && !Programs.IsEnabledByHq(ProgramName.XDR,out string err)) {
				MessageBox.Show(err);
				return;
			}
			SaveProgram();
			if(_hasProgramPropertyChanged) {
				DataValid.SetInvalid(InvalidType.Programs,InvalidType.ToolButsAndMounts);
			}
			DialogResult=DialogResult.OK;
		}
	}
}