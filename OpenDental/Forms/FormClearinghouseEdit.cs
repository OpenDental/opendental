using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using CodeBase;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormClearinghouseEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		private Clearinghouse ClearinghouseClin;
		///<summary>This must be set externally before opening the form.  The HQ version of the clearinghouse.  
		///This may not be null.  Assign a new clearinghouse object to this if creating a new clearinghouse.</summary>
		public Clearinghouse ClearinghouseHq;
		///<summary>This is never edited from within this form.  Set externally for reference to use in the Sync() method.</summary>		
		///This may not be null.  Assign a new clearinghouse object to this if creating a new clearinghouse.</summary>
		public Clearinghouse ClearinghouseHqOld;
		///<summary>This must be set externally before opening the form.  This is the clearinghouse used to display, with properly overridden fields.
		///This may not be null.  Assign a new clearinghouse object to this if creating a new clearinghouse.</summary>
		public Clearinghouse ClearinghouseCur;
		///<summary>Set this outside of the form.  0 for HQ.</summary>
		public long ClinicNum;
		/// <summary>List of all available clinics.  Cannot be null.</summary>
		public List<Clinic> ListClinics;
		///<summary>List of all clinic-level clearinghouses for the current clearinghousenum.  
		///May not be null.  Assign a new list of clearinghouse objects to this if creating a new clearinghouse.</summary>
		public List<Clearinghouse> ListClearinghousesClinCur;
		///<summary>This is never edited from within this form.  Set externally for reference to use in the Sync() method.
		///May not be null.  Assign a new list of clearinghouse objects to this if creating a new clearinghouse.</summary>
		public List<Clearinghouse> ListClearinghousesClinOld;
		private long _clinicNumLastSelected=-1;

		///<summary></summary>
		public FormClearinghouseEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			Lan.C(this, new System.Windows.Forms.Control[]
			{
				this.textBox2
			});
		}

		private void FormClearinghouseEdit_Load(object sender,System.EventArgs e) {
			comboFormat.Items.AddEnums<ElectronicClaimFormat>();
			for(int i=0;i<Enum.GetNames(typeof(EclaimsCommBridge)).Length;i++) {
				string translatedCommBridgeName=Lan.g("enumEclaimsCommBridge",Enum.GetNames(typeof(EclaimsCommBridge))[i]);
				comboCommBridge.Items.Add(translatedCommBridgeName,(EclaimsCommBridge)i);
			}
			comboClinic.SelectedClinicNum=ClinicNum;
			FillFields();
		}

		private void FillFields() {
			ClearinghouseClin=null;//Default to null so that if no clinic is found below we will use the defualt HQ options.
			for(int i=0;i<ListClearinghousesClinCur.Count;i++) {
				if(ListClearinghousesClinCur[i].ClinicNum==ClinicNum)	{
					ClearinghouseClin=ListClearinghousesClinCur[i];
				}
			}
			ClearinghouseCur=Clearinghouses.OverrideFields(ClearinghouseHq,ClearinghouseClin);
			textDescription.Text=ClearinghouseCur.Description;
			textISA02.Text=ClearinghouseCur.ISA02;
			textISA04.Text=ClearinghouseCur.ISA04;
			textISA05.Text=ClearinghouseCur.ISA05;
			textISA07.Text=ClearinghouseCur.ISA07;
			textISA08.Text=ClearinghouseCur.ISA08;
			textISA15.Text=ClearinghouseCur.ISA15;
			textGS03.Text=ClearinghouseCur.GS03;
			textSeparatorData.Text=ClearinghouseCur.SeparatorData;
			textISA16.Text=ClearinghouseCur.ISA16;
			textSeparatorSegment.Text=ClearinghouseCur.SeparatorSegment;
			textLoginID.Text=ClearinghouseCur.LoginID;
			textPassword.Text=ClearinghouseCur.Password;
			textExportPath.Text=ClearinghouseCur.ExportPath;
			textResponsePath.Text=ClearinghouseCur.ResponsePath;
			ODException.SwallowAnyException(() => { comboFormat.SelectedIndex=(int)ClearinghouseCur.Eformat; });
			ODException.SwallowAnyException(() => { comboCommBridge.SelectedIndex=(int)ClearinghouseCur.CommBridge; });
			if(ClearinghouseCur.SenderTIN==""){
				radioSenderOD.Checked=true;
				radioSenderBelow.Checked=false;
				textSenderTIN.Text="";
				textSenderName.Text="";
				textSenderTelephone.Text="";
			}
			else{
				radioSenderOD.Checked=false;
				radioSenderBelow.Checked=true;
				textSenderTIN.Text=ClearinghouseCur.SenderTIN;
				textSenderName.Text=ClearinghouseCur.SenderName;
				textSenderTelephone.Text=ClearinghouseCur.SenderTelephone;
			}
			textModemPort.Text=ClearinghouseCur.ModemPort.ToString();
			textClientProgram.Text=ClearinghouseCur.ClientProgram;
			//checkIsDefault.Checked=ClearinghouseCur.IsDefault;
			textPayors.Text=ClearinghouseCur.Payors;
			if(PrefC.HasClinicsEnabled) {
				labelInfo.Visible=true;
				labelTIN.Font=new System.Drawing.Font(labelTIN.Font,FontStyle.Bold);
				labelSenderName.Font=new System.Drawing.Font(labelSenderName.Font,FontStyle.Bold);
				labelSenderTelephone.Font=new System.Drawing.Font(labelSenderTelephone.Font,FontStyle.Bold);
				labelLoginID.Font=new System.Drawing.Font(labelLoginID.Font,FontStyle.Bold);
				labelPassword.Font=new System.Drawing.Font(labelPassword.Font,FontStyle.Bold);
				labelExportPath.Font=new System.Drawing.Font(labelExportPath.Font,FontStyle.Bold);
				labelReportPath.Font=new System.Drawing.Font(labelReportPath.Font,FontStyle.Bold);
				labelClientProgram.Font=new System.Drawing.Font(labelClientProgram.Font,FontStyle.Bold);
			}
			if(ClinicNum!=0) {
				textDescription.ReadOnly=true;
				textISA02.ReadOnly=true;
				textISA04.ReadOnly=true;
				textISA05.ReadOnly=true;
				textSeparatorData.ReadOnly=true;
				textISA16.ReadOnly=true; ;
				textSeparatorSegment.ReadOnly=true;
				textISA07.ReadOnly=true;
				textISA08.ReadOnly=true;
				textGS03.ReadOnly=true;
				textISA15.ReadOnly=true;
				comboFormat.Enabled=false;
				comboCommBridge.Enabled=false;
				textModemPort.ReadOnly=true;
				textPayors.ReadOnly=true;
				butDelete.Enabled=false;
			}
			else {
				textDescription.ReadOnly=false;
				textISA02.ReadOnly=false;
				textISA04.ReadOnly=false;
				textISA05.ReadOnly=false;
				textSeparatorData.ReadOnly=false;
				textISA16.ReadOnly=false;
				textSeparatorSegment.ReadOnly=false;
				textISA07.ReadOnly=false;
				textISA08.ReadOnly=false;
				textGS03.ReadOnly=false;
				textISA15.ReadOnly=false;
				comboFormat.Enabled=true;
				comboCommBridge.Enabled=true;
				textModemPort.ReadOnly=false;
				textPayors.ReadOnly=false;
				butDelete.Enabled=true;
			}
			FillListBoxEraBehavior();
			listBoxEraBehavior.Enabled=true;
			if(ListTools.In(ClearinghouseCur.CommBridge,EclaimsCommBridge.ClaimConnect,EclaimsCommBridge.EDS,EclaimsCommBridge.Claimstream,EclaimsCommBridge.ITRANS,
				EclaimsCommBridge.EmdeonMedical,EclaimsCommBridge.EdsMedical))
			{
				listBoxEraBehavior.Enabled=true;
				checkIsClaimExportAllowed.Enabled=true;
				checkIsClaimExportAllowed.Checked=ClearinghouseCur.IsClaimExportAllowed;
			}
			//Uncheck and disable if not ClaimConnect as this checkbox only applies to ClaimConnect.
			checkSaveDXC.Checked=PrefC.GetBool(PrefName.SaveDXCAttachments);
			checkSaveDXCSoap.Checked=PrefC.GetBool(PrefName.SaveDXCSOAPAsXML);
			if(ClearinghouseCur.CommBridge!=EclaimsCommBridge.ClaimConnect) {
				checkAllowAttachSend.Enabled=false;
				checkAllowAttachSend.Checked=false;
				checkSaveDXC.Visible=false;
				checkSaveDXCSoap.Visible=false;
			}
			else {
				checkAllowAttachSend.Enabled=true;
				checkAllowAttachSend.Checked=ClearinghouseCur.IsAttachmentSendAllowed;
				checkSaveDXCSoap.Visible=true;
			}
			//Invisible by default, so only need to check if we should make visible on load, rest of checks are handled by CheckChanged event.
			if(checkAllowAttachSend.Checked) {
				checkSaveDXC.Visible=true;
			}
			UpdateClientProgramLabel();
			Plugins.HookAddCode(this,"FormClearingHouseEdit.FillFields_end",ClearinghouseCur);
		}

		private void UpdateClientProgramLabel() {
			if((EclaimsCommBridge)comboCommBridge.SelectedIndex==EclaimsCommBridge.DentiCal) {
				labelClientProgram.Text="SFTP Host Name";
			}
			else {
				labelClientProgram.Text="Launch Client Program";
			}
		}

		private void FillListBoxEraBehavior() {
			bool isCanadaEraClearinghouse=ListTools.In(comboCommBridge.GetSelected<EclaimsCommBridge>(),EclaimsCommBridge.Claimstream,EclaimsCommBridge.ITRANS);
			if(!isCanadaEraClearinghouse) {//If US
				LayoutManager.MoveHeight(listBoxEraBehavior,30);
			}
			else {
				LayoutManager.MoveHeight(listBoxEraBehavior,43);
			}
			listBoxEraBehavior.Items.Clear();
			List<EraBehaviors> listEraBehaviors=new List<EraBehaviors>((EraBehaviors[])Enum.GetValues(typeof(EraBehaviors)));
			if(!isCanadaEraClearinghouse) {//If US
				listEraBehaviors.RemoveAt((int)EraBehaviors.DownloadDoNotReceive);//Do not include DownloadDoNotReceive in non-Canada.
			}
			for(int i=0;i<listEraBehaviors.Count;i++) {
				EraBehaviors behavior=listEraBehaviors[i];
				string description=behavior.GetDescription();
				//use comboCommBridge selection to determine if ERA->EOB "translation" necessary
				if(isCanadaEraClearinghouse) {
					description=Regex.Replace(description,"ERA","EOB");
				}
				else if(behavior==EraBehaviors.DownloadAndReceive) {//This is the only download option available in US.
					description="Download ERAs";
				}
				description=Lan.g("enumEraBehavior",description); //make sure the ERA->EOB replace gets run through translation
				//ODBoxItem<EraBehaviors> listBoxItem=new ODBoxItem<EraBehaviors>(description,behavior);
				listBoxEraBehavior.Items.Add(description,behavior);
				if(behavior==ClearinghouseCur.IsEraDownloadAllowed) {
					listBoxEraBehavior.SelectedIndex=i;
				}
			}
			if(listBoxEraBehavior.GetListSelected<EraBehaviors>().Count==0//No selection made above
				&& !isCanadaEraClearinghouse //If US
				&& ClearinghouseCur.IsEraDownloadAllowed!=EraBehaviors.None) //May have been incorrectly set to DownloadDoNotReceive for US in past.
			{
				listBoxEraBehavior.SelectedIndex=listEraBehaviors.IndexOf(EraBehaviors.DownloadAndReceive);//Only valid option besides None.
			}
		}

		///<summary>Pass in a clearinghouse with an unconcealed password. Will do nothing if the password is blank.</summary>
		private void ConcealClearinghousePass(Clearinghouse clearinghouse) {
			string concealedPassword = "";
			if(string.IsNullOrEmpty(clearinghouse.Password)) {
				return;
			}
			if(CDT.Class1.ConcealClearinghouse(clearinghouse.Password,out concealedPassword)) {
				clearinghouse.Password=concealedPassword;
			}
		}

		///<summary>Pass in a clearinghouse with a concealed password. Will do nothing if the password is blank.</summary>
		private void RevealClearinghousePass(Clearinghouse clearinghouse) {
			string revealedClearinghousePass = Clearinghouses.GetRevealPassword(clearinghouse.Password);
			if(revealedClearinghousePass!="") {
				clearinghouse.Password=revealedClearinghousePass;
			}
		}

		///<summary>All cached clearinghouses' passwords are NOT hashed. 
		///Hashing in this form only happens when transferring data between the database and the program.</summary>
		private bool SaveToCache() {
			if(!ValidateFields()) {
				return false;
			}
			if(ClinicNum==0) {
				#region HQ Clearinghouse
				ClearinghouseHq.Description=textDescription.Text;
				ClearinghouseHq.ISA02=textISA02.Text;
				ClearinghouseHq.ISA04=textISA04.Text;
				ClearinghouseHq.ISA05=textISA05.Text;
				ClearinghouseHq.SenderTIN=textSenderTIN.Text;
				ClearinghouseHq.SenderName=textSenderName.Text;
				ClearinghouseHq.SenderTelephone=textSenderTelephone.Text;
				ClearinghouseHq.ISA07=textISA07.Text;
				ClearinghouseHq.ISA08=textISA08.Text;
				ClearinghouseHq.ISA15=textISA15.Text;
				ClearinghouseHq.GS03=textGS03.Text;
				ClearinghouseHq.SeparatorData=textSeparatorData.Text;
				ClearinghouseHq.ISA16=textISA16.Text;
				ClearinghouseHq.SeparatorSegment=textSeparatorSegment.Text;
				ClearinghouseHq.LoginID=textLoginID.Text;
				ClearinghouseHq.Password=textPassword.Text;
				ClearinghouseHq.ExportPath=textExportPath.Text;
				ClearinghouseHq.ResponsePath=textResponsePath.Text;
				ClearinghouseHq.Eformat=(ElectronicClaimFormat)(comboFormat.SelectedIndex);
				ClearinghouseHq.CommBridge=(EclaimsCommBridge)(comboCommBridge.SelectedIndex);
				ClearinghouseHq.ModemPort=PIn.Byte(textModemPort.Text);
				ClearinghouseHq.ClientProgram=textClientProgram.Text;
				//ClearinghouseHq.IsDefault=checkIsDefault.Checked;
				ClearinghouseHq.Payors=textPayors.Text;
				ClearinghouseHq.IsClaimExportAllowed=checkIsClaimExportAllowed.Checked;
				ClearinghouseHq.IsEraDownloadAllowed=listBoxEraBehavior.GetSelected<EraBehaviors>();
				ClearinghouseHq.IsAttachmentSendAllowed=checkAllowAttachSend.Checked;
				#endregion
			}
			else { 
				#region Clinic Specific Clearinghouse
				if(ClearinghouseClin==null) {
					ClearinghouseClin=new Clearinghouse();
				}
				//Save Clin Values to ClearinghouseClin, then update ListClinicClearinghousesCur.
				ClearinghouseClin.HqClearinghouseNum=ClearinghouseHq.ClearinghouseNum;
				ClearinghouseClin.ClinicNum=ClinicNum;
				ClearinghouseClin.IsClaimExportAllowed=checkIsClaimExportAllowed.Checked;
				ClearinghouseClin.IsEraDownloadAllowed=listBoxEraBehavior.GetSelected<EraBehaviors>();
				ClearinghouseClin.IsAttachmentSendAllowed=checkAllowAttachSend.Checked;
				if(textExportPath.Text==ClearinghouseHq.ExportPath) {
					ClearinghouseClin.ExportPath="";//The value is the same as the default.  Save blank so that default can be updated dynamically.
				}
				else {
					ClearinghouseClin.ExportPath=textExportPath.Text;
				}
				if(textSenderTIN.Text==ClearinghouseHq.SenderTIN) {
					ClearinghouseClin.SenderTIN="";//The value is the same as the default.  Save blank so that default can be updated dynamically.
				}
				else {
					ClearinghouseClin.SenderTIN=textSenderTIN.Text;
				}
				if(textPassword.Text==ClearinghouseHq.Password) {
					ClearinghouseClin.Password="";//The value is the same as the default.  Save blank so that default can be updated dynamically.
				}
				else {
					ClearinghouseClin.Password=textPassword.Text;
				}
				if(textResponsePath.Text==ClearinghouseHq.ResponsePath) {
					ClearinghouseClin.ResponsePath="";//The value is the same as the default.  Save blank so that default can be updated dynamically.
				}
				else {
					ClearinghouseClin.ResponsePath=textResponsePath.Text;
				}
				if(textClientProgram.Text==ClearinghouseHq.ClientProgram) {
					ClearinghouseClin.ClientProgram="";//The value is the same as the default.  Save blank so that default can be updated dynamically.
				}
				else {
					ClearinghouseClin.ClientProgram=textClientProgram.Text;
				}
				if(textLoginID.Text==ClearinghouseHq.LoginID) {
					ClearinghouseClin.LoginID="";//The value is the same as the default.  Save blank so that default can be updated dynamically.
				}
				else {
					ClearinghouseClin.LoginID=textLoginID.Text;
				}
				if(textSenderName.Text==ClearinghouseHq.SenderName) {
					ClearinghouseClin.SenderName="";//The value is the same as the default.  Save blank so that default can be updated dynamically.
				}
				else {
					ClearinghouseClin.SenderName=textSenderName.Text;
				}
				if(textSenderTelephone.Text==ClearinghouseHq.SenderTelephone) {
					ClearinghouseClin.SenderTelephone="";//The value is the same as the default.  Save blank so that default can be updated dynamically.
				}
				else {
					ClearinghouseClin.SenderTelephone=textSenderTelephone.Text;
				}
				if(ListClearinghousesClinCur.Exists(x => x.ClinicNum==ClinicNum)) {
					//Update the corresponding clinic entry within the current list of clearinghouses.
					//The clearinghouse overrides feature operates under the assumption that there exists only 1
					//clearinghouse override per clinic. The method UpdateOverridesForClinic() was introduced to simply update all duplicate rows
					//so that if a customer does have 1,000,000 override rows for one clinic, they all will be updated so regardless of which one
					//MySQL hands to the program, it will always be correct. See jobnum 11387 for more details.
					Clearinghouses.SyncOverridesForClinic(ref ListClearinghousesClinCur,ClearinghouseClin);
				}
				else {//Add the clinic specific clearinghouse if not found in current list of clearinghouses.
					ListClearinghousesClinCur.Add(ClearinghouseClin);
				}
				#endregion
			}
			if(ClearinghouseCur.CommBridge==EclaimsCommBridge.ClaimConnect && (ClearinghouseCur.LoginID!=textLoginID.Text || ClearinghouseCur.Password!=textPassword.Text)){
				Program progPayConnect=Programs.GetCur(ProgramName.PayConnect);
				int billingUseDentalExchangeIdx=PrefC.GetInt(PrefName.BillingUseElectronic);//idx of 1= DentalXChange.
				if(progPayConnect.Enabled || billingUseDentalExchangeIdx==1) {
					MsgBox.Show(this,"ClaimConnect, PayConnect, and Electronic Billing credentials are usually all changed at the same time when using DentalXChange.");
				}
			}
			return true;
		}

		private bool ValidateFields() {
			if(comboFormat.SelectedIndex==-1) {
				MsgBox.Show(this,"Invalid Format.");
				return false;
			}
			if(comboCommBridge.SelectedIndex==-1) {
				MsgBox.Show(this,"Invalid Comm Bridge.");
				return false;
			}
			if(ODBuild.IsWeb() 
				&& Clearinghouses.IsDisabledForWeb(comboFormat.GetSelected<ElectronicClaimFormat>(),comboCommBridge.GetSelected<EclaimsCommBridge>())) 
			{
				MsgBox.Show(this,"This clearinghouse is not available while viewing through the web.");
				return false;
			}
			if(ClinicNum==0) {//HQ
				if(textDescription.Text=="") {
					MsgBox.Show(this,"Description cannot be blank.");//HQ
					return false;
				}
				if(comboFormat.SelectedIndex==(int)ElectronicClaimFormat.x837D_4010 //HQ
				|| comboFormat.SelectedIndex==(int)ElectronicClaimFormat.x837D_5010_dental
				|| comboFormat.SelectedIndex==(int)ElectronicClaimFormat.x837_5010_med_inst) {
					if(textISA02.Text.Length>10) {
						MsgBox.Show(this,"ISA02 must be 10 characters or less.");
						return false;
					}
					if(textISA04.Text.Length>10) {
						MsgBox.Show(this,"ISA04 must be 10 characters or less.");
						return false;
					}
					if(textISA05.Text=="") {
						MsgBox.Show(this,"ISA05 is required.");
						return false;
					}
					if(textISA05.Text!="01" && textISA05.Text!="14" && textISA05.Text!="20" && textISA05.Text!="27" && textISA05.Text!="28"//HQ
					&& textISA05.Text!="29" && textISA05.Text!="30" && textISA05.Text!="33" && textISA05.Text!="ZZ") {
						MsgBox.Show(this,"ISA05 is not valid.");
						return false;
					}
					if(textISA07.Text=="") {//HQ
						MsgBox.Show(this,"ISA07 is required.");
						return false;
					}
					if(textISA07.Text!="01" && textISA07.Text!="14" && textISA07.Text!="20" && textISA07.Text!="27" && textISA07.Text!="28"//HQ
					&& textISA07.Text!="29" && textISA07.Text!="30" && textISA07.Text!="33" && textISA07.Text!="ZZ") {
						MsgBox.Show(this,"ISA07 not valid.");
						return false;
					}
					if(textISA08.Text.Length<2) {//HQ
						MsgBox.Show(this,"ISA08 not valid.");
						return false;
					}
					if(textISA15.Text!="T" && textISA15.Text!="P") {//HQ
						MsgBox.Show(this,"ISA15 not valid.");
						return false;
					}
					if(textGS03.Text.Length<2) {//HQ
						MsgBox.Show(this,"GS03 is required.");
						return false;
					}
					if(textSeparatorData.Text!="" && !Regex.IsMatch(textSeparatorData.Text,"^[0-9A-F]{2}$",RegexOptions.IgnoreCase)) {//HQ
						MsgBox.Show(this,"Data element separator must be a valid 2 digit hexadecimal number or blank.");
						return false;
					}
					if(textISA16.Text!="" && !Regex.IsMatch(textISA16.Text,"^[0-9A-F]{2}$",RegexOptions.IgnoreCase)) {//HQ
						MsgBox.Show(this,"Component element separator must be a valid 2 digit hexadecimal number or blank.");
						return false;
					}
					if(textSeparatorSegment.Text!="" && !Regex.IsMatch(textSeparatorSegment.Text,"^[0-9A-F]{2}$",RegexOptions.IgnoreCase)) {//HQ
						MsgBox.Show(this,"Segment terminator must be a valid 2 digit hexadecimal number or blank.");
						return false;
					}
					if(comboFormat.SelectedIndex==0) {//HQ
						if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Format not selected. Claims will not send. Continue anyway?")) {
							return false;
						}
					}
				}
			}//end HQ
			if(textISA08.Text=="0135WCH00" && !radioSenderOD.Checked) {//Clinic
				MsgBox.Show(this,"When using Emdeon, this software must be the sender.");
				return false;
			}
			if(comboFormat.SelectedIndex==(int)ElectronicClaimFormat.x837D_4010
				|| comboFormat.SelectedIndex==(int)ElectronicClaimFormat.x837D_5010_dental
				|| comboFormat.SelectedIndex==(int)ElectronicClaimFormat.x837_5010_med_inst)
			{//Clinic
				if(radioSenderBelow.Checked) {
					if(textSenderTIN.Text.Length<2) {
						MsgBox.Show(this,"Sender TIN is required.");
						return false;
					}
					if(textSenderName.Text=="") {
						MsgBox.Show(this,"Sender Name is required.");
						return false;
					}
					if(!Regex.IsMatch(textSenderTelephone.Text,@"^\d{10}$")) {
						MsgBox.Show(this,"Sender telephone must be 10 digits with no punctuation.");
						return false;
					}
				}	
			}
			//todo: Check all parts of program to allow either trailing slash or not
			if(checkIsClaimExportAllowed.Checked && textExportPath.Text!="" && !Directory.Exists(textExportPath.Text)) {
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Export path does not exist. Attempt to create?")) {
					try{
						Directory.CreateDirectory(textExportPath.Text);
						MsgBox.Show(this,"Folder created.");
					}
					catch{
						if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Not able to create folder. Continue anyway?")){
							return false;
						}
					}
				}
			}
			if(textResponsePath.Text!="" && !Directory.Exists(textResponsePath.Text)) {//Clinic
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Report path does not exist. Attempt to create?")) {
					try {
						Directory.CreateDirectory(textResponsePath.Text);
						MsgBox.Show(this,"Folder created.");
					}
					catch {
						if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Not able to create folder. Continue anyway?")) {
							return false;
						}
					}
				}
			}
			/*if(comboFormat.SelectedIndex==(int)ElectronicClaimFormat.X12){
				if(textISA08.Text!="BCBSGA"
					&& textISA08.Text!="100000"//Medicaid of GA
					&& textISA08.Text!="0135WCH00"//WebMD
					&& textISA08.Text!="330989922"//WebClaim
					&& textISA08.Text!="RECS"
					&& textISA08.Text!="AOS"
					&& textISA08.Text!="PostnTrack"
					)
				{
					if(!MsgBox.Show(this,true,"Clearinghouse ID not recognized. Continue anyway?")){
						return;
					}
				}
			}*/
			return true;
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			if(_clinicNumLastSelected==comboClinic.SelectedClinicNum) {
				return;//Selection did not change.
			}
			if(!SaveToCache()) {//Validation failed.
				comboClinic.SelectedClinicNum=_clinicNumLastSelected;//Revert selection.
				return;
			}
			_clinicNumLastSelected=comboClinic.SelectedClinicNum;
			ClinicNum=_clinicNumLastSelected;
			FillFields();
		}
		
		private void comboCommBridge_SelectionChangeCommitted(object sender,EventArgs e) {
			FillListBoxEraBehavior();//Update ERA/EOB list box, specifically for if we print ERA vs EOB.
			bool hasEclaimsEnabled=ListTools.In(comboCommBridge.GetSelected<EclaimsCommBridge>(),
				EclaimsCommBridge.ClaimConnect,EclaimsCommBridge.EDS,EclaimsCommBridge.Claimstream,EclaimsCommBridge.ITRANS,
				EclaimsCommBridge.EmdeonMedical,EclaimsCommBridge.EdsMedical);
			listBoxEraBehavior.Enabled=hasEclaimsEnabled;
			checkIsClaimExportAllowed.Enabled=hasEclaimsEnabled;
			UpdateClientProgramLabel();
		}

		private void comboFormat_SelectedIndexChanged(object sender,EventArgs e) {
			//Phone number cannot include punctuation when one of these formats is selected.
			if(comboFormat.SelectedIndex==(int)ElectronicClaimFormat.x837D_4010
				|| comboFormat.SelectedIndex==(int)ElectronicClaimFormat.x837D_5010_dental
				|| comboFormat.SelectedIndex==(int)ElectronicClaimFormat.x837_5010_med_inst)
			{
				textSenderTelephone.IsFormattingEnabled=false;
			}
			else {
				textSenderTelephone.IsFormattingEnabled=true;
			}
		}

		private void radio_Click(object sender,EventArgs e) {
			if(radioSenderOD.Checked) {
				textSenderTIN.Text="";
				textSenderName.Text="";
				textSenderTelephone.Text="";
			}
			else {
				textSenderTIN.Text=ClearinghouseCur.SenderTIN;
				textSenderName.Text=ClearinghouseCur.SenderName;
				textSenderTelephone.Text=ClearinghouseCur.SenderTelephone;
			}
		}

		private void checkAllowAttachSend_CheckedChanged(object sender,EventArgs e) {
			checkSaveDXC.Visible=checkAllowAttachSend.Checked;
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This clearinghouse will be deleted for all clinics.  Continue?")){
				return;
			}
			Clearinghouses.Delete(ClearinghouseHq);
			if(PrefC.GetLong(PrefName.ClearinghouseDefaultDent)==ClearinghouseHq.ClearinghouseNum) {
				Prefs.UpdateLong(PrefName.ClearinghouseDefaultDent,0);
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			if(PrefC.GetLong(PrefName.ClearinghouseDefaultMed)==ClearinghouseHq.ClearinghouseNum) {
				Prefs.UpdateLong(PrefName.ClearinghouseDefaultMed,0);
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			ClearinghouseCur=null;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(!SaveToCache()) {//Validation failed.
				return;//Block user from leaving.
			}
			//When saving, hash all passwords.
			ConcealClearinghousePass(ClearinghouseHq);
			ListClearinghousesClinCur.ForEach(x => {
				ConcealClearinghousePass(x);
			});
			ConcealClearinghousePass(ClearinghouseHqOld);
			ListClearinghousesClinOld.ForEach(x => {
				ConcealClearinghousePass(x);
			});
			if(IsNew) {
				long clearinghouseNumNew=Clearinghouses.Insert(ClearinghouseHq);
				for(int i=0;i<ListClearinghousesClinCur.Count;i++) {
					ListClearinghousesClinCur[i].HqClearinghouseNum=clearinghouseNumNew;
				}
			}
			else {
				Clearinghouses.Update(ClearinghouseHq,ClearinghouseHqOld);
			}
			Clearinghouses.Sync(ListClearinghousesClinCur,ListClearinghousesClinOld);
			//After saving, reveal all passwords.
			RevealClearinghousePass(ClearinghouseHq);
			foreach(Clearinghouse c in ListClearinghousesClinCur) {
				RevealClearinghousePass(c);
			}
			//Reveal the "olds" just in case someone uses them outside this form.
			RevealClearinghousePass(ClearinghouseHqOld);
			foreach(Clearinghouse c in ListClearinghousesClinOld) {
				RevealClearinghousePass(c);
			}
			//If the DXC attachment saving preferences changed send a signal to have all clients update their preference cache.
			bool hasSOAPPrefChanged=Prefs.UpdateBool(PrefName.SaveDXCSOAPAsXML,checkSaveDXCSoap.Checked);//Can't do this in the if check or we risk not running the right side of the OR.
			if(Prefs.UpdateBool(PrefName.SaveDXCAttachments,checkSaveDXC.Checked) || hasSOAPPrefChanged) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}





















