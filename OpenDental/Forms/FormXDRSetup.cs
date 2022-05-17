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
		private Program _progCur;
		private ProgramProperty _patNumOrChartNum;
		private ProgramProperty _infoFilePath;
		private Dictionary<long,ProgramProperty> _dictLocationIDs=new Dictionary<long, ProgramProperty>();
		///<summary>Local cache of all of the clinic nums the current user has permission to access at the time the form loads.
		///Filled at the same time as comboClinic and is used to set programproperty.ClinicNum when saving.</summary>
		private List<long> _listUserClinicNums;
		private List<ProgramProperty> _listProgramProperties;
		///<summary>Can be 0 for "Headquarters" or non clinic users.</summary>
		private long _clinicNumCur;
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
				_listUserClinicNums=new List<long>();
				comboClinic.Items.Clear();
				comboClinic.Items.Add(Lan.g(this,"Headquarters"));
				//This way both lists have the same number of items in it and if 'Headquarters' is selected the programproperty.ClinicNum will be set to 0
				_listUserClinicNums.Add(0);
				comboClinic.SelectedIndex=0;
				_clinicNumCur=0;
				List<Clinic> listClinics=Clinics.GetForUserod(Security.CurUser);
				for(int i=0;i<listClinics.Count;i++) {
					comboClinic.Items.Add(listClinics[i].Abbr);
					_listUserClinicNums.Add(listClinics[i].ClinicNum);
					if(Clinics.ClinicNum==listClinics[i].ClinicNum) {
						comboClinic.SelectedIndex=i;
						if(!Security.CurUser.ClinicIsRestricted) {
							comboClinic.SelectedIndex++;//increment the SelectedIndex to account for 'Headquarters' in the list at position 0 if the user is not restricted.
						}
						_clinicNumCur=_listUserClinicNums[comboClinic.SelectedIndex];
					}
				}
			}
			else {//clinics are not enabled, use ClinicNum 0 to indicate 'Headquarters' or practice level program properties
				comboClinic.Visible=false;
				labelClinic.Visible=false;
				_listUserClinicNums=new List<long>() { 0 };//if clinics are disabled, programproperty.ClinicNum will be set to 0
				_clinicNumCur=0;
			}
			_progCur=Programs.GetCur(ProgramName.XDR);
			if(_progCur==null) {
				MsgBox.Show(this,"The XDR bridge is missing from the database.");//should never happen
				DialogResult=DialogResult.Cancel;
				return;
			}
			try {
				long clinicNum=0;
				if(comboClinic.SelectedIndex>0) {//0 is always "All" so only check for greater than 0.
					clinicNum=_listUserClinicNums[comboClinic.SelectedIndex];
				}
				_listProgramProperties=ProgramProperties.GetListForProgramAndClinicWithDefault(_progCur.ProgramNum,clinicNum);
				_patNumOrChartNum=_listProgramProperties.FirstOrDefault(x => x.PropertyDesc==XDR.PropertyDescs.PatNumOrChartNum);
				_infoFilePath=_listProgramProperties.FirstOrDefault(x => x.PropertyDesc==XDR.PropertyDescs.InfoFilePath);
				List<ProgramProperty> listLocationIDs=ProgramProperties.GetForProgram(_progCur.ProgramNum).FindAll(x => x.PropertyDesc==XDR.PropertyDescs.LocationID);
				_dictLocationIDs.Clear();
				//If clinics is off, this will only grab the program property with a 0 clinicNum (_listUserClinicNums will only have 0).
				foreach(ProgramProperty ppCur in listLocationIDs) {
					if(_dictLocationIDs.ContainsKey(ppCur.ClinicNum) || !_listUserClinicNums.Contains(ppCur.ClinicNum)) {
						continue;
					}
					_dictLocationIDs.Add(ppCur.ClinicNum,ppCur);
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
				MsgBox.Show(this,"You are missing a program property for XDR.  Please contact support to resolve this issue.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			FillForm();
		}

		private void FillForm() {
			//ComboClinic is filled in the load method
			if(PIn.Int(_patNumOrChartNum.PropertyValue)==1) {
				radioChart.Checked=true;
			}
			else {
				radioPatient.Checked=true;
			}
			List<ToolButItem> listToolButItems=ToolButItems.GetForProgram(_progCur.ProgramNum);
			listToolBars.Items.Clear();
			listToolBars.Items.AddEnums<ToolBarsAvail>();
			for(int i=0;i<listToolButItems.Count;i++) {
				listToolBars.SetSelectedEnum(listToolButItems[i].ToolBar);
			}
			checkEnabled.Checked=_progCur.Enabled;
			textPath.Text=_progCur.Path;
			textButtonText.Text=listToolButItems[0].ButtonText;
			pictureBox.Image=PIn.Bitmap(_progCur.ButtonImage);
			try {
				textInfoFile.Text=_infoFilePath.PropertyValue;
				_pathOverrideOld=ProgramProperties.GetLocalPathOverrideForProgram(_progCur.ProgramNum);
				textOverride.Text=_pathOverrideOld;
				if(_dictLocationIDs.ContainsKey(_clinicNumCur)) {
					textLocationID.Text=_dictLocationIDs[_clinicNumCur].PropertyValue;
				}
			}
			catch(Exception) {
				MsgBox.Show(this,"You are missing a program property from the database.  Please call support to resolve this issue.");
				DialogResult=DialogResult.Cancel;
				return;
			}
		}

		///<summary>Updates the in memory dictionary with any changes made to the current locationID for each clinic before showing the next one.</summary>
		private void SaveClinicCurProgramPropertiesToDict() {
			//First check if Headquarters (default) is selected.
			if(_clinicNumCur==0) {
				//Headquarters is selected so only update the location ID (might have changed) on all other location ID properties that match the "old" location ID of HQ.
				if(_dictLocationIDs.ContainsKey(_clinicNumCur)) {
					//Get the location ID so that we correctly update all program properties with a matching location ID.
					string locationIdOld=_dictLocationIDs[_clinicNumCur].PropertyValue;
					foreach(KeyValuePair<long,ProgramProperty> item in _dictLocationIDs) {
						ProgramProperty ppCur=item.Value;
						if(ppCur.PropertyValue==locationIdOld) {
							ppCur.PropertyValue=textLocationID.Text;
						}
					}
				}
				return;//No other clinic specific changes could have been made, we need to return.
			}
			//Update or Insert clinic specific properties into memory
			ProgramProperty ppLocationID=new ProgramProperty();
			if(_dictLocationIDs.ContainsKey(_clinicNumCur)) {
				ppLocationID=_dictLocationIDs[_clinicNumCur];//Override the database's property with what is in memory.
			}
			else {//Get default programproperty from db.
				ppLocationID=ProgramProperties.GetListForProgramAndClinicWithDefault(_progCur.ProgramNum,_clinicNumCur)
					.FirstOrDefault(x => x.PropertyDesc==XDR.PropertyDescs.LocationID);
			}
			if(ppLocationID.ClinicNum==0) {//No program property for current clinic, since _clinicNumCur!=0
				ProgramProperty ppLocationIDNew=ppLocationID.Copy();
				ppLocationIDNew.ProgramPropertyNum=0;
				ppLocationIDNew.ClinicNum=_clinicNumCur;
				ppLocationIDNew.PropertyValue=textLocationID.Text;
				if(!_dictLocationIDs.ContainsKey(_clinicNumCur)) {//Should always happen
					_dictLocationIDs.Add(_clinicNumCur,ppLocationIDNew);
				}
				return;
			}
			//At this point we know that the clinicnum isn't 0 and the database has a property for that clinicnum.
			if(_dictLocationIDs.ContainsKey(_clinicNumCur)) {//Should always happen
				ppLocationID.PropertyValue=textLocationID.Text;
				_dictLocationIDs[_clinicNumCur]=ppLocationID;
			}
			else {
				_dictLocationIDs.Add(_clinicNumCur,ppLocationID);//Should never happen.
			}
		}

		private void SaveProgram() {
			SaveClinicCurProgramPropertiesToDict();
			_progCur.Enabled=checkEnabled.Checked;
			_progCur.Path=textPath.Text;
			_progCur.ButtonImage=POut.Bitmap((Bitmap)pictureBox.Image,System.Drawing.Imaging.ImageFormat.Png);
			ToolButItems.DeleteAllForProgram(_progCur.ProgramNum);
			//Then add one toolButItem for each highlighted row in listbox
			ToolButItem toolButItemCur;
			for(int i=0;i<listToolBars.SelectedIndices.Count;i++) {
				toolButItemCur=new ToolButItem() {
					ProgramNum=_progCur.ProgramNum,
					ButtonText=textButtonText.Text,
					ToolBar=(ToolBarsAvail)listToolBars.SelectedIndices[i]
				};
				ToolButItems.Insert(toolButItemCur);
			}
			if(_pathOverrideOld!=textOverride.Text) {//If there was no previous override _pathOverrideOld will be empty string.
				_hasProgramPropertyChanged=true;
				ProgramProperties.InsertOrUpdateLocalOverridePath(_progCur.ProgramNum,textOverride.Text);
			}
			UpdateProgramProperty(_patNumOrChartNum,POut.Bool(radioChart.Checked));//Will need to be enhanced if another radio button ever gets added.
			UpdateProgramProperty(_infoFilePath,textInfoFile.Text);
			UpsertProgramPropertiesForClinics();
			Programs.Update(_progCur);
		}

		private void UpdateProgramProperty(ProgramProperty ppFromDb,string newpropertyValue) {
			if(ppFromDb.PropertyValue==newpropertyValue) {
				return;
			}
			ppFromDb.PropertyValue=newpropertyValue;
			ProgramProperties.Update(ppFromDb);
			_hasProgramPropertyChanged=true;
		}

		private void UpsertProgramPropertiesForClinics() {
			List<ProgramProperty> listLocationIDsFromDb=ProgramProperties.GetForProgram(_progCur.ProgramNum).FindAll(x => x.PropertyDesc==XDR.PropertyDescs.LocationID);
			List<ProgramProperty> listLocationIDsCur=_dictLocationIDs.Values.ToList();
			foreach(ProgramProperty ppCur in listLocationIDsCur) {
				if(listLocationIDsFromDb.Exists(x => x.ProgramPropertyNum == ppCur.ProgramPropertyNum)) {
					UpdateProgramProperty(listLocationIDsFromDb[listLocationIDsFromDb.FindIndex(x => x.ProgramPropertyNum == ppCur.ProgramPropertyNum)],ppCur.PropertyValue);//ppCur.PropertyValue will match textLocationID.Text
				}
				else {
					ProgramProperties.Insert(ppCur);//Program property for that clinicnum didn't exist, so insert it into the db.
					_hasProgramPropertyChanged=true;
				}
			}
		}

		private void ComboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			SaveClinicCurProgramPropertiesToDict();
			_clinicNumCur=_listUserClinicNums[comboClinic.SelectedIndex];
			//This will either display the HQ value, or the clinic specific value.
			if(_dictLocationIDs.ContainsKey(_clinicNumCur)) {
				textLocationID.Text=_dictLocationIDs[_clinicNumCur].PropertyValue;
			}
			else {
				textLocationID.Text=_dictLocationIDs[0].PropertyValue;//Default to showing the HQ value when filling info for a clinic with no program property.
			}
		}

		private void ButImport_Click(object sender,EventArgs e) {
			using OpenFileDialog dlg=new OpenFileDialog();
			if(dlg.ShowDialog()!=DialogResult.OK) {
				return;
			}
			try {
				Image importedImg=Image.FromFile(dlg.FileName);
				if(importedImg.Size!=new Size(22,22)) {
					MessageBox.Show(Lan.g(this,"Required image dimensions are 22x22.")
						+"\r\n"+Lan.g(this,"Selected image dimensions are")+": "+importedImg.Size.Width+"x"+importedImg.Size.Height);
					return;
				}
				pictureBox.Image=importedImg;
			}
			catch {
				MsgBox.Show(this,"Error loading file.");
			}
		}

		private void ButClear_Click(object sender,EventArgs e) {
			pictureBox.Image=null;
		}

		private void butOK_Click(object sender,EventArgs e) {
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

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}