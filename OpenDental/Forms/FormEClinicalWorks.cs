using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary> </summary>
	public partial class FormEClinicalWorks:FormODBase {
		/// <summary>This Program link is new.</summary>
		public bool IsNew;
		public Program ProgramCur;
		private List<ProgramProperty> PropertyList;
		//private static Thread thread;
		private List<UserGroup> _listUserGroups;

		///<summary></summary>
		public FormEClinicalWorks() {
			components=new System.ComponentModel.Container();
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEClinicalWorks_Load(object sender, System.EventArgs e) {
			FillForm();
			if(HL7Defs.IsExistingHL7Enabled()) {
				//Instead of using these, we will use the ones that are part of the HL7Def
				//These will be filled with those values.
				textHL7Server.ReadOnly=true;
				textHL7ServiceName.ReadOnly=true;
				textHL7FolderIn.ReadOnly=true;
				textHL7FolderOut.ReadOnly=true;
				labelDefEnabledWarning.Visible=true;
				checkQuadAsToothNum.Enabled=false;
			}
		}

		private void FillForm(){
			ProgramProperties.RefreshCache();
			PropertyList=ProgramProperties.GetForProgram(ProgramCur.ProgramNum);
			textProgName.Text=ProgramCur.ProgName;
			textProgDesc.Text=ProgramCur.ProgDesc;
			checkEnabled.Checked=ProgramCur.Enabled;
			if(GetProp("HideChartRxButtons")=="1") {
				checkHideButChartRx.Checked=true;
			}
			else {
				checkHideButChartRx.Checked=false;
			}
			if(GetProp("ProcRequireSignature")=="1") {
				checkProcRequireSignature.Checked=true;
			}
			else {
				checkProcRequireSignature.Checked=false;
			}
			if(GetProp("ProcNotesNoIncomplete")=="1") {
				checkProcNotesNoIncomplete.Checked=true;
			}
			else {
				checkProcNotesNoIncomplete.Checked=false;
			}
			SetModeRadioButtons(GetProp("eClinicalWorksMode"));
			SetModeVisibilities();
			textECWServer.Text=GetProp("eCWServer");//this property will not exist if using Oracle, eCW will never use Oracle
			if(HL7Defs.IsExistingHL7Enabled()) {
				HL7Def def=HL7Defs.GetOneDeepEnabled();
				textHL7Server.Text=def.HL7Server;
				textHL7ServiceName.Text=def.HL7ServiceName;
				textHL7FolderIn.Text=def.OutgoingFolder;//because these are the opposite of the way they are in the HL7Def
				textHL7FolderOut.Text=def.IncomingFolder;
				checkQuadAsToothNum.Checked=def.IsQuadAsToothNum;
			}
			else {
				textHL7Server.Text=GetProp("HL7Server");//this property will not exist if using Oracle, eCW will never use Oracle
				textHL7ServiceName.Text=GetProp("HL7ServiceName");//this property will not exist if using Oracle, eCW will never use Oracle
				textHL7FolderIn.Text=PrefC.GetString(PrefName.HL7FolderIn);
				textHL7FolderOut.Text=PrefC.GetString(PrefName.HL7FolderOut);
				//if a def is enabled, the value associated with the def will override this setting
				checkQuadAsToothNum.Checked=GetProp("IsQuadAsToothNum")=="1";//this property will not exist if using Oracle, eCW will never use Oracle
			}
			textODServer.Text=MiscData.GetODServer();
			comboDefaultUserGroup.Items.Clear();
			_listUserGroups=UserGroups.GetList();
			for(int i=0;i<_listUserGroups.Count;i++) {
				comboDefaultUserGroup.Items.Add(_listUserGroups[i].Description);
				if(GetProp("DefaultUserGroup")==_listUserGroups[i].UserGroupNum.ToString()) {
					comboDefaultUserGroup.SelectedIndex=i;
				}
			}
			checkShowImages.Checked=GetProp("ShowImagesModule")=="1";
			checkFeeSchedules.Checked=GetProp("FeeSchedulesSetManually")=="1";
			textMedPanelURL.Text=GetProp("MedicalPanelUrl");//this property will not exist if using Oracle, eCW will never use Oracle
			checkLBSessionId.Checked=GetProp("IsLBSessionIdExcluded")=="1";
		}

		private string GetProp(string desc){
			for(int i=0;i<PropertyList.Count;i++){
				if(PropertyList[i].PropertyDesc==desc){
					return PropertyList[i].PropertyValue;
				}
			}
			throw new ApplicationException("Property not found: "+desc);
		}

		private void checkEnabled_Click(object sender,EventArgs e) {
			bool isHL7Enabled=HL7Defs.IsExistingHL7Enabled();
			if(isHL7Enabled && checkEnabled.Checked) {
				MsgBox.Show(this,"An HL7Def is enabled.  The enabled HL7 definition will control the HL7 messages not this program link.");
				textHL7Server.ReadOnly=true;
				textHL7ServiceName.ReadOnly=true;
				textHL7FolderIn.ReadOnly=true;
				textHL7FolderOut.ReadOnly=true;
				labelDefEnabledWarning.Visible=true;
				checkQuadAsToothNum.Enabled=false;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Warning!  Read the manual carefully before turning this bridge on or off.  Make sure you understand the difference between the bridging modes and how it will affect patient accounts.  Continue anyway?")) {
				checkEnabled.Checked=!checkEnabled.Checked;
				return;
			}
			MsgBox.Show(this,"You will need to restart Open Dental to see the effects.");
		}

		private void radioModeTight_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Warning!  Read the manual carefully before changing this option.  Make sure you understand the difference between the bridging modes and how it will affect patient accounts.  Continue anyway?")) {
				//set radio buttons according to what they already are in the db
				SetModeRadioButtons(GetProp("eClinicalWorksMode"));
				SetModeVisibilities();
				return;
			}
			SetModeVisibilities();
		}

		private void radioModeStandalone_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Warning!  Read the manual carefully before changing this option.  Make sure you understand the difference between the bridging modes and how it will affect patient accounts.  Continue anyway?")) {
				//set radio buttons according to what they already are in the db
				SetModeRadioButtons(GetProp("eClinicalWorksMode"));
				SetModeVisibilities();
				return;
			}
			SetModeVisibilities();
		}

		private void radioModeFull_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Warning!  Read the manual carefully before changing this option.  Make sure you understand the difference between the bridging modes and how it will affect patient accounts.  Continue anyway?")) {
				//set radio buttons according to what they already are in the db
				SetModeRadioButtons(GetProp("eClinicalWorksMode"));
				SetModeVisibilities();
				return;
			}
			SetModeVisibilities();
		}

		private void SetModeVisibilities() {
			if(radioModeTight.Checked || radioModeFull.Checked) {
				labelHL7FolderIn.Visible=true;
				textHL7FolderIn.Visible=true;
				labelDefaultUserGroup.Visible=true;
				comboDefaultUserGroup.Visible=true;
				checkShowImages.Visible=true;
				checkFeeSchedules.Visible=true;
				labelHL7Warning.Visible=true;
			}
			else if(radioModeStandalone.Checked) {
				labelHL7FolderIn.Visible=false;
				textHL7FolderIn.Visible=false;
				labelDefaultUserGroup.Visible=false;
				comboDefaultUserGroup.Visible=false;
				checkShowImages.Visible=false;
				checkFeeSchedules.Visible=false;
				labelHL7Warning.Visible=false;
			}
		}

		///<summary>Pass in the desired eCW mode.  0=Tight,1=Standalone,2=Full.  Defaults to Tight.</summary>
		private void SetModeRadioButtons(string eClinicalWorksMode) {
			switch(eClinicalWorksMode) {
				case "0":
					radioModeTight.Checked=true;
					break;
				case "1":
					radioModeStandalone.Checked=true;
					break;
				case "2":
					radioModeFull.Checked=true;
					break;
				default:
					radioModeTight.Checked=true;
					break;
			}
		}

		private void checkShowImages_Click(object sender,EventArgs e) {
			MsgBox.Show(this,"You will need to restart Open Dental to see the effects.");
		}

		private bool SaveToDb() {
			if((radioModeTight.Checked || radioModeFull.Checked) && comboDefaultUserGroup.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a default user group first.");
				return false;
			}
			if(checkEnabled.Checked) {
				if(textProgDesc.Text=="") {
					MsgBox.Show(this,"Description may not be blank.");
					return false;
				}
				if(!HL7Defs.IsExistingHL7Enabled()) {
					if((radioModeTight.Checked || radioModeFull.Checked) && textHL7FolderIn.Text=="") {
						MsgBox.Show(this,"HL7 in folder may not be blank.");
						return false;
					}
					if(textHL7FolderOut.Text=="") {
						MsgBox.Show(this,"HL7 out folder may not be blank.");
						return false;
					}
					if(textHL7Server.Text=="") {
						MsgBox.Show(this,"HL7 Server may not be blank.");
						return false;
					}
					if(textHL7ServiceName.Text=="") {
						MsgBox.Show(this,"HL7 Service Name may not be blank.");
						return false;
					}
				}
			}
			ProgramCur.ProgDesc=textProgDesc.Text;
			ProgramCur.Enabled=checkEnabled.Checked;
			Programs.Update(ProgramCur);
			Prefs.UpdateString(PrefName.HL7FolderOut,textHL7FolderOut.Text);
			ProgramProperties.SetProperty(ProgramCur.ProgramNum,"HL7Server",textHL7Server.Text);//this property will not exist if using Oracle, eCW will never use Oracle
			ProgramProperties.SetProperty(ProgramCur.ProgramNum,"HL7ServiceName",textHL7ServiceName.Text);//this property will not exist if using Oracle, eCW will never use Oracle
			ProgramProperties.SetProperty(ProgramCur.ProgramNum,"MedicalPanelUrl",textMedPanelURL.Text);//this property will not exist if using Oracle, eCW will never use Oracle
			if(checkLBSessionId.Checked) {
				ProgramProperties.SetProperty(ProgramCur.ProgramNum,"IsLBSessionIdExcluded","1");
			}
			else {
				ProgramProperties.SetProperty(ProgramCur.ProgramNum,"IsLBSessionIdExcluded","0");
			}
			if(checkQuadAsToothNum.Checked) {
				ProgramProperties.SetProperty(ProgramCur.ProgramNum,"IsQuadAsToothNum","1");//this property will not exist if using Oracle, eCW will never use Oracle
			}
			else {
				ProgramProperties.SetProperty(ProgramCur.ProgramNum,"IsQuadAsToothNum","0");//this property will not exist if using Oracle, eCW will never use Oracle
			}
			if(checkHideButChartRx.Checked) {
				ProgramProperties.SetProperty(ProgramCur.ProgramNum,"HideChartRxButtons","1");//this property will not exist if using Oracle, eCW will never use Oracle
			}
			else {
				ProgramProperties.SetProperty(ProgramCur.ProgramNum,"HideChartRxButtons","0");//this property will not exist if using Oracle, eCW will never use Oracle
			}
			if(checkProcRequireSignature.Checked) {
				ProgramProperties.SetProperty(ProgramCur.ProgramNum,"ProcRequireSignature","1");
			}
			else {
				ProgramProperties.SetProperty(ProgramCur.ProgramNum,"ProcRequireSignature","0");
			}
			if(checkProcNotesNoIncomplete.Checked) {
				ProgramProperties.SetProperty(ProgramCur.ProgramNum,"ProcNotesNoIncomplete","1");
			}
			else {
				ProgramProperties.SetProperty(ProgramCur.ProgramNum,"ProcNotesNoIncomplete","0");
			}
			if(radioModeTight.Checked || radioModeFull.Checked) {
				if(radioModeTight.Checked) {
					ProgramProperties.SetProperty(ProgramCur.ProgramNum,"eClinicalWorksMode","0");//Tight
				}
				else {
					ProgramProperties.SetProperty(ProgramCur.ProgramNum,"eClinicalWorksMode","2");//Full
				}
				ProgramProperties.SetProperty(ProgramCur.ProgramNum,"eCWServer",textECWServer.Text);//this property will not exist if using Oracle, eCW will never use Oracle
				Prefs.UpdateString(PrefName.HL7FolderIn,textHL7FolderIn.Text);
				ProgramProperties.SetProperty(ProgramCur.ProgramNum,"DefaultUserGroup",
					_listUserGroups[comboDefaultUserGroup.SelectedIndex].UserGroupNum.ToString());
				if(checkShowImages.Checked) {
					ProgramProperties.SetProperty(ProgramCur.ProgramNum,"ShowImagesModule","1");
				}
				else {
					ProgramProperties.SetProperty(ProgramCur.ProgramNum,"ShowImagesModule","0");
				}
				if(this.checkFeeSchedules.Checked) {
					ProgramProperties.SetProperty(ProgramCur.ProgramNum,"FeeSchedulesSetManually","1");
				}
				else {
					ProgramProperties.SetProperty(ProgramCur.ProgramNum,"FeeSchedulesSetManually","0");
				}
			}
			else if(radioModeStandalone.Checked) {
				ProgramProperties.SetProperty(ProgramCur.ProgramNum,"eClinicalWorksMode","1");
				Prefs.UpdateString(PrefName.HL7FolderIn,"");
				ProgramProperties.SetProperty(ProgramCur.ProgramNum,"DefaultUserGroup","0");
				ProgramProperties.SetProperty(ProgramCur.ProgramNum,"ShowImagesModule","1");
				ProgramProperties.SetProperty(ProgramCur.ProgramNum,"FeeSchedulesSetManually","0");
			}
			DataValid.SetInvalid(InvalidType.Programs,InvalidType.Prefs);
			return true;
		}

		private void butDiagnostic_Click(object sender,EventArgs e) {
			//no need to validate all the other fields on the page.
			ProgramProperties.SetProperty(ProgramCur.ProgramNum,"eCWServer",textECWServer.Text);//this property will not exist if using Oracle, eCW will never use Oracle
			DataValid.SetInvalid(InvalidType.Programs);
			using FormEcwDiag FormECWD=new FormEcwDiag();
			FormECWD.ShowDialog();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!SaveToDb()){
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormProgramLinkEdit_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			
		}

		

	

	

		

		

	

		

		

		
		


	}
}





















