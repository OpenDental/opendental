using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;
using System.Linq;
using OpenDental.Bridges;

namespace OpenDental{
	/// <summary> </summary>
	public partial class FormProgramLinkEdit : FormODBase {
		/// <summary>This Program link is new.</summary>
		public bool IsNew;
		public Program ProgramCur;
		private string pathOverrideOld;
		private bool _isLoading = false;

		///<summary>Set to false if we do not want to allow assigning program link to toolbars.</summary>
		public bool AllowToolbarChanges=true;

		///<summary></summary>
		public FormProgramLinkEdit(){
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormProgramLinkEdit_Load(object sender, System.EventArgs e) {
			_isLoading=true;
			if(ProgramCur.ProgName!=""){
				//user not allowed to delete program links that we include, only their own.
				butDelete.Enabled=false;
			}
			pathOverrideOld=ProgramProperties.GetLocalPathOverrideForProgram(ProgramCur.ProgramNum);
			textOverride.Text=pathOverrideOld;
			FillForm();
			DisableUIElementsBasedOnClinicRestriction();//Disable the UI Elements if needed.
			HideClinicControls(PrefC.HasClinicsEnabled);//Hide the "Hide Button for Clinics" button based upon the user's clinics being on or off.
			ShowPLButHiddenLabel();//Display warning label for "Hide Button for Clinics" if needed.
			SetAdvertising();
			if(!CanEnableProgram()) {
				labelCloudMessage.Visible=true;
			}
			_isLoading=false;
		}

		///<summary>Handles both visibility and checking of checkHideButtons.</summary>
		private void SetAdvertising() {
			checkHideButtons.Visible=true;
			ProgramProperty prop = ProgramProperties.GetForProgram(ProgramCur.ProgramNum).FirstOrDefault(x => x.PropertyDesc=="Disable Advertising");
			if(checkEnabled.Checked || prop==null) {
				checkHideButtons.Visible=false;
			}
			if(prop!=null) {
				checkHideButtons.Checked=(prop.PropertyValue=="1");
			}
		}

		private void checkEnabled_CheckedChanged(object sender,EventArgs e) {
			SetAdvertising();
			if(checkEnabled.Checked && !CanEnableProgram() && !_isLoading) {
				checkEnabled.Checked=false;
				MsgBox.Show(this,"Web users cannot currently enable this bridge");
			}
		}

		private bool CanEnableProgram() {
			if(!ODBuild.IsWeb()) {
				return true;
			}
			if(ListTools.In(ProgramCur.ProgName,Programs.GetListDisabledForWeb().Select(x => x.ToString()))) {
				return false;//these programs are not currently allowed for web users
			}
			return true;//it was not one of the programs listed
		}

		private void FillForm(){
			//this is not refined enough to be called more than once on the form because it will not
			//remember the toolbars that were selected.
			ToolButItems.RefreshCache();
			ProgramProperties.RefreshCache();
			textProgName.Text=ProgramCur.ProgName;
			textProgDesc.Text=ProgramCur.ProgDesc;
			checkEnabled.Checked=ProgramCur.Enabled;
			textPath.Text=ProgramCur.Path;
			textCommandLine.Text=ProgramCur.CommandLine;
			textPluginDllName.Text=ProgramCur.PluginDllName;
			textNote.Text=ProgramCur.Note;
			pictureBox.Image=PIn.Bitmap(ProgramCur.ButtonImage);
			List<ToolButItem> itemsForProgram=ToolButItems.GetForProgram(ProgramCur.ProgramNum);
			listToolBars.Items.Clear();
			listToolBars.Items.AddEnums<ToolBarsAvail>();
			for(int i=0;i<itemsForProgram.Count;i++) {
				listToolBars.SetSelectedEnum(itemsForProgram[i].ToolBar);
			}
			if(!AllowToolbarChanges) {//As we add more static bridges, we will need to enhance this to show/hide controls as needed.
				listToolBars.ClearSelected();
				listToolBars.Enabled=false;
			}
			if(itemsForProgram.Count>0){//the text on all buttons will be the same for now
				textButtonText.Text=itemsForProgram[0].ButtonText;
			}
			FillGrid();
		}

		private void FillGrid(){
			List<ProgramProperty> ProgramPropertiesForProgram=ProgramProperties.GetForProgram(ProgramCur.ProgramNum);
			ProgramPropertiesForProgram=ProgramProperties.FilterProperties(ProgramCur,ProgramPropertiesForProgram);
			Plugins.HookAddCode(this,"FormProgramLinkEdit.FillGrid_GetProgramProperties",ProgramPropertiesForProgram,ProgramCur);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Property"),260);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Value"),130);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			foreach(ProgramProperty property in ProgramPropertiesForProgram) { 
				if(ListTools.In(property.PropertyDesc,"Disable Advertising","Disable Advertising HQ",ProgramProperties.PropertyDescs.ClinicHideButton)) {//Don't display in grid
					continue;
				}
				row=new GridRow();
				row.Cells.Add(property.PropertyDesc);
				if(property.IsMasked) {
					string password;
					bool isXVWebPassword=ProgramCur.ProgName==ProgramName.XVWeb.ToString() && property.PropertyDesc==XVWeb.ProgramProps.Password;
					if(isXVWebPassword) {
						CDT.Class1.Decrypt(property.PropertyValue,out password);
					}
					else {
						password=property.PropertyValue;
					}
					row.Cells.Add(new string('*',password.Length));
				}
				else if(ProgramCur.ProgName==ProgramName.XVWeb.ToString() && property.PropertyDesc==XVWeb.ProgramProps.ImageCategory) {
					Def imageCat=Defs.GetDefsForCategory(DefCat.ImageCats).FirstOrDefault(x => x.DefNum==PIn.Long(property.PropertyValue));
					if(imageCat==null) {
						row.Cells.Add("");
					}
					else if(imageCat.IsHidden) {
						row.Cells.Add(imageCat.ItemName+" "+Lans.g(this,"(hidden)"));
					}
					else {
						row.Cells.Add(imageCat.ItemName);
					}
				}
				else {
					row.Cells.Add(property.PropertyValue);
				}
				row.Tag=property;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		///<summary>This method hides (Visible=false) controls when the Clinics are turned off.</summary>
		private void HideClinicControls(bool hasClinicsEnabled) {
			if(!hasClinicsEnabled) {
				butClinicLink.Visible=false;
				labelClinicStateWarning.Visible=false;
			}
		}

		///<summary>If Clinics are enabled, and the Program Link button is hidden for at least one clinic, display the warning label 
		///labelClinicStateWarning.</summary>
		private void ShowPLButHiddenLabel() {
			List<ProgramProperty> listProps=ProgramProperties.GetForProgram(ProgramCur.ProgramNum)
				.Where(x => x.PropertyDesc==ProgramProperties.PropertyDescs.ClinicHideButton).ToList();
			if(PrefC.HasClinicsEnabled && !listProps.IsNullOrEmpty()) {//If anything is in list, they have a hidden clinic.
				labelClinicStateWarning.Visible=true;
			}
			else {
				labelClinicStateWarning.Visible=false;
			}
		}

		///<summary>If Clinics are enabled, and the user is clinic restricted, disable certain UI elements and turn on the warning that the user is 
		///restricted.  Any ProgramLink settings which would affect clinics to which the user does not have access are disabled.</summary>
		private void DisableUIElementsBasedOnClinicRestriction() {
			if(PrefC.HasClinicsEnabled && Security.CurUser.ClinicIsRestricted) {//Clinics are Enabled and the user is restricted.
				//TODO: change this logic to be explicit instead of implicit (i.e get a list of all controls we want to explicitly disable.)
				List<Control> listEnabled=new List<Control>() { 
					label1,textProgName,label2,textProgDesc,checkEnabled,checkHideButtons,label3,textPath,label4,textCommandLine,label9,label7,textButtonText,
					label5,textPluginDllName,gridMain,label8,textNote,butOutputFile,label6,listToolBars,label10,pictureBox,butClear,butImport,butDelete
					};
				foreach(Control ctl in UIHelper.GetAllControls(this).Where(x => ListTools.In(x,listEnabled))) {
					ctl.Enabled=false;//Turn off all but the specified controls above in ProgramLinkEdit window.
				}
				labelDisableForClinic.Visible=true;//Turn on the warning in the ProgramLinkEdit window that some controls are disabled for this user.
			}
			else {
				labelDisableForClinic.Visible=false;
			}
		}

		/// <summary>Chooses which type of form to open based on current program and selected property.</summary>
		private void gridMain_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			ProgramProperty programProperty=(ProgramProperty)gridMain.ListGridRows[e.Row].Tag;
			switch(ProgramCur.ProgName) {
				case nameof(ProgramName.XVWeb):
					switch(programProperty.PropertyDesc) {
						case XVWeb.ProgramProps.ImageCategory:
							List<string> listDefNums=Defs.GetDefsForCategory(DefCat.ImageCats,true).Select(x => POut.Long(x.DefNum)).ToList();
							List<string> listItemNames=Defs.GetDefsForCategory(DefCat.ImageCats,true).Select(x => x.ItemName).ToList();
							ShowComboBoxForProgramProperty(programProperty,listDefNums,listItemNames,Lans.g(this,"Choose an Image Category"));
							return;
						case XVWeb.ProgramProps.ImageQuality:
							List<string> listOptions=Enum.GetValues(typeof(XVWebImageQuality)).Cast<XVWebImageQuality>().Select(x => x.ToString()).ToList();
							List<string> listDisplay=listOptions.Select(x => Lans.g(this,x)).ToList();
							ShowComboBoxForProgramProperty(programProperty,listOptions,listDisplay,Lans.g(this,"Choose an Image Quality"));
							return;
					}
					break;
				case nameof(ProgramName.PDMP):
				case nameof(ProgramName.Appriss):
					switch(programProperty.PropertyDesc) {
						case PdmpProperty.PdmpProvLicenseField:
							List<string> listLicenseOptions=new List<string> { nameof(ProviderClinic.StateLicense), nameof(ProviderClinic.StateRxID)};
							List<string> listLicenseDisplay=listLicenseOptions.Select(x=>Lans.g(this,x)).ToList();
							ShowComboBoxForProgramProperty(programProperty,listLicenseOptions,listLicenseDisplay,Lans.g(this,"Choose License Type for PDMP Program"));
							return;
						}	
					break;
				}
			ShowFormProgramProperty(programProperty);
		}

		private void butOutputFile_Click(object sender,EventArgs e) {
			using FormProgramLinkOutputFile formPLOF=new FormProgramLinkOutputFile(ProgramCur);
			formPLOF.ShowDialog();
		}

		private void butImport_Click(object sender,EventArgs e) {
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

		private void butClear_Click(object sender,EventArgs e) {
			pictureBox.Image=null;
		}

		private void checkHideButtons_CheckedChanged(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			ProgramProperty property = ProgramProperties.GetForProgram(ProgramCur.ProgramNum).FirstOrDefault(x => x.PropertyDesc=="Disable Advertising");
			if(property==null) {
				return;//should never happen.
			}
			if(checkHideButtons.Checked) {
				property.PropertyValue="1";
			}
			else {
				property.PropertyValue="0";
			}
			ProgramProperties.Update(property);
		}

		///<summary>Opens a form where the user can type in their selection for a program poperty.</summary>
		private void ShowFormProgramProperty(ProgramProperty programProperty) {
			bool propIsPassword=programProperty.PropertyDesc.Contains("Password");//We encrypt all passwords now, so just check for "Password" keyword.
			using FormProgramProperty formProgramProperty=new FormProgramProperty(propIsPassword);
			formProgramProperty.ProgramPropertyCur=programProperty;
			formProgramProperty.ShowDialog();
			if(formProgramProperty.DialogResult!=DialogResult.OK) {
				return;
			}
			ProgramProperties.RefreshCache();
			FillGrid();
		}

		///<summary>Opens a form where the user can select an option from a combo box for a program poperty.</summary>
		///<param name="listValuesForDb">The value that should be stored in the db for the corresponding display item that is selected. This list should
		///have the same number of items as listForDisplay.</param>
		///<param name="listForDisplay">The value that will be displayed to the user in the combo box. This list should have the same number of items 
		///as listValuesForDb.</param>
		private void ShowComboBoxForProgramProperty(ProgramProperty programProperty,List<string> listValuesForDb,List<string> listForDisplay
			,string prompt) 
		{
			ProgramProperty programPropertyOld=programProperty.Copy();
			using InputBox inputBox=new InputBox(prompt,listForDisplay,listValuesForDb.FindIndex(x => x==programProperty.PropertyValue));
			inputBox.ShowDialog();
			if(inputBox.DialogResult!=DialogResult.OK || inputBox.SelectedIndex==-1 || 
				listValuesForDb[inputBox.SelectedIndex]==programPropertyOld.PropertyValue) 
			{
				return;
			}
			programProperty.PropertyValue=listValuesForDb[inputBox.SelectedIndex];
			ProgramProperties.Update(programProperty,programPropertyOld);
			ProgramProperties.RefreshCache();
			FillGrid();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(ProgramCur.ProgName!=""){//prevent users from deleting program links that we included.
				MsgBox.Show(this,"Not allowed to delete a program link with an internal name.");
				return;
			}
			if(MessageBox.Show(Lan.g(this,"Delete this program link?"),"",MessageBoxButtons.OKCancel)
				!=DialogResult.OK){
				return;
			}
			if(!IsNew){
				Programs.Delete(ProgramCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butClinicLink_Click(object sender,EventArgs e) {
			//Get the users total list of unrestricted clinics, then acquire their list of ProgramProperties so we can tell which PL buttons 
			//should be hidden based upon ProgramProperty.PropertyDesc/ClinicNum. 
			List<Clinic> listClinics=Clinics.GetForUserod(Security.CurUser,doIncludeHQ:true,hqClinicName:Lan.g(this,"HQ"));//Include HQ if user not restricted.
			//Filter the list of all Hidden button ProgramProperties down to the clinics the user has access to.  This will be passed to FormProgramLinkHideClinics.
			List<ProgramProperty> listPropsForUser=ProgramProperties.GetForProgram(ProgramCur.ProgramNum)
				.Where(x =>  x.PropertyDesc==ProgramProperties.PropertyDescs.ClinicHideButton 
					&& ListTools.In(x.ClinicNum,listClinics.Select(y => y.ClinicNum)))
				.ToList();
			using FormProgramLinkHideClinics formProgramLinkHideClinics=new FormProgramLinkHideClinics(ProgramCur,listPropsForUser,listClinics);
			if(formProgramLinkHideClinics.ShowDialog()==DialogResult.OK) {
				//Ensure other WS update their "hidden by clinic" properties.
				DataValid.SetInvalid(InvalidType.Programs,InvalidType.ToolButsAndMounts);
			}
			ShowPLButHiddenLabel();//Set the "Hide Button for Clinics" button based on the updated list.
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			//If a program has been disabled by HQ when a customer attempts to enable it, we should block them from doing so, so as not to waste their time trying to launch it
			if(checkEnabled.Checked && !Programs.IsEnabledByHq(ProgramCur,out string err)) {
				MsgBox.Show(err);
				return;
			}
			if(checkEnabled.Checked && textPluginDllName.Text!="") {
				if(ODBuild.IsWeb()) {
					MessageBox.Show(Lan.g(this,"Plugins are not allowed in Cloud mode."));
					return;
				}
				string dllPath=ODFileUtils.CombinePaths(Application.StartupPath,textPluginDllName.Text);
				if(dllPath.Contains("[VersionMajMin]")) {
					Version vers = new Version(Application.ProductVersion);
					dllPath = dllPath.Replace("[VersionMajMin]","");//now stripped clean
				}
				if(!File.Exists(dllPath)) {
					MessageBox.Show(Lan.g(this,"Dll file not found:")+" "+dllPath);
					return;
				}
			}
			if(textPluginDllName.Text!="" && textPath.Text!="") {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"If both a path and a plug-in are specified, the path will be ignored.  Continue anyway?")) {
					return;
				}
			}
			bool enabledChanged=false;//just for XVWeb and Suni
			if(ProgramCur.Enabled!=checkEnabled.Checked){
				enabledChanged=true;
			}
			if(enabledChanged 
				&& (ProgramCur.ProgName==ProgramName.XVWeb.ToString()
				|| textProgDesc.Text=="Suni"))//not a built-in bridge.  Instructions in manual to manually add this bridge.
			{
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"The entire program will now need to close to reset imaging.")){
					return;
				}
			}
			ProgramCur.ProgName=textProgName.Text;
			ProgramCur.ProgDesc=textProgDesc.Text;
			ProgramCur.Enabled=checkEnabled.Checked;
			ProgramCur.Path=textPath.Text;
			if(pathOverrideOld!=textOverride.Text) {
				ProgramProperties.InsertOrUpdateLocalOverridePath(ProgramCur.ProgramNum,textOverride.Text);
				ProgramProperties.RefreshCache();
			}
			ProgramCur.CommandLine=textCommandLine.Text;
			ProgramCur.PluginDllName=textPluginDllName.Text;
			ProgramCur.Note=textNote.Text;
			ProgramCur.ButtonImage=POut.Bitmap((Bitmap)pictureBox.Image,System.Drawing.Imaging.ImageFormat.Png);
			if(IsNew){
				Programs.Insert(ProgramCur);
			}
			else{
				Programs.Update(ProgramCur);
			}
			ToolButItems.DeleteAllForProgram(ProgramCur.ProgramNum);
			//then add one toolButItem for each highlighted row in listbox
			ToolButItem ToolButItemCur;
			List<ToolBarsAvail> listToolBarsAvail=listToolBars.GetListSelected<ToolBarsAvail>();
			for(int i=0;i<listToolBarsAvail.Count;i++){
				ToolButItemCur=new ToolButItem();
				ToolButItemCur.ProgramNum=ProgramCur.ProgramNum;
				ToolButItemCur.ButtonText=textButtonText.Text;
				ToolButItemCur.ToolBar=listToolBarsAvail[i];
				ToolButItems.Insert(ToolButItemCur);
			}
			DialogResult=DialogResult.OK;
			if(enabledChanged
				&& (ProgramCur.ProgName==ProgramName.XVWeb.ToString()
				|| ProgramCur.ProgDesc=="Suni"))
			{
				//if using XVWeb or Suni, then also use the old images module
				Prefs.UpdateBool(PrefName.ImagesModuleUsesOld2020,checkEnabled.Checked);
				Cursor=Cursors.WaitCursor;
				FormOpenDental.S_ProcessKillCommand();//just no possible way to elegantly keep it from crashing
			}
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormProgramLinkEdit_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(DialogResult==DialogResult.OK)
				return;
			if(IsNew){
				Programs.Delete(ProgramCur);
			}
		}
	}
}





















