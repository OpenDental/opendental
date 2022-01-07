using System;
using System.Drawing;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;
using System.Collections.Generic;

namespace OpenDental {
	/// <summary></summary>
	public partial class FormHL7DefEdit:FormODBase {
		public HL7Def HL7DefCur;
		private List<Def> _listImageCatDefs;

		///<summary></summary>
		public FormHL7DefEdit() {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormHL7DefEdit_Load(object sender,EventArgs e) {
			FillGrid();
			textDescription.Text=HL7DefCur.Description;
			checkInternal.Checked=HL7DefCur.IsInternal;
			checkEnabled.Checked=HL7DefCur.IsEnabled;
			textInternalType.Text=HL7DefCur.InternalType.ToString();
			textInternalTypeVersion.Text=HL7DefCur.InternalTypeVersion;
			textFieldSep.Text=HL7DefCur.FieldSeparator;
			textRepSep.Text=HL7DefCur.RepetitionSeparator;
			textCompSep.Text=HL7DefCur.ComponentSeparator;
			textSubcompSep.Text=HL7DefCur.SubcomponentSeparator;
			textEscChar.Text=HL7DefCur.EscapeCharacter;
			textNote.Text=HL7DefCur.Note;
			textHL7Server.Text=HL7DefCur.HL7Server;
			textHL7ServiceName.Text=HL7DefCur.HL7ServiceName;
			SetShowRadioButtons();
			checkShowAccount.Checked=HL7DefCur.ShowAccount;
			checkShowAppts.Checked=HL7DefCur.ShowAppts;
			checkQuadAsToothNum.Checked=HL7DefCur.IsQuadAsToothNum;
			checkLongDCodes.Checked=HL7DefCur.HasLongDCodes;
			checkProcsAppt.Checked=HL7DefCur.IsProcApptEnforced;
			textSftpUsername.Text=HL7DefCur.SftpUsername;
			textSftpPassword.Text=CDT.Class1.TryDecrypt(HL7DefCur.SftpPassword);
			for(int i=0;i<Enum.GetNames(typeof(ModeTxHL7)).Length;i++) {
				comboModeTx.Items.Add(Lan.g("enumModeTxHL7",Enum.GetName(typeof(ModeTxHL7),i).ToString()));
				if((int)HL7DefCur.ModeTx==i){
					comboModeTx.SelectedIndex=i;
				}
			}
			if(HL7DefCur.InternalType==HL7InternalType.MedLabv2_3) {
				comboModeTx.SelectedIndex=(int)ModeTxHL7.Sftp;//just in case, for MedLabv2_3 types this should always be Sftp and isn't editable for now
			}
			_listImageCatDefs=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			for(int i=0;i<_listImageCatDefs.Count;i++) {
				comboLabImageCat.Items.Add(_listImageCatDefs[i].ItemName);
				if(_listImageCatDefs[i].DefNum==HL7DefCur.LabResultImageCat) {
					comboLabImageCat.SelectedIndex=i;
				}
			}
			//no need to call SetControls since the comboModeTx_SelectedIndexChanged and/or checkEnabled_CheckedChanged triggered it already
		}

		///<summary>Sets control visibility and label/textbox text based on ModeTx and internal type.  Also sets control read only property based on enabled status.</summary>
		private void SetControls() {
			#region Enabled/Disabled Affected Controls
			butBrowseIn.Enabled=false;
			butBrowseOut.Enabled=false;
			checkLongDCodes.Enabled=false;
			checkQuadAsToothNum.Enabled=false;
			checkShowAccount.Enabled=false;
			checkShowAppts.Enabled=false;
			checkProcsAppt.Enabled=false;
			comboLabImageCat.Enabled=false;
			//comboModeTx.Enabled=false;//also part of the internal type controls, must be type other than MedLabv2_3 and enabled for comboModeTx to be enabled
			groupDelimeters.Enabled=false;
			groupShowDemographics.Enabled=false;
			textDescription.ReadOnly=false;
			textHL7Server.ReadOnly=false;
			textHL7ServiceName.ReadOnly=false;
			textInPathOrSocket.ReadOnly=false;
			textOutPathSocketOrDir.ReadOnly=false;
			textSftpPassword.ReadOnly=false;
			textSftpUsername.ReadOnly=false;
			#endregion Enabled/Disabled Affected Controls
			#region Internal Type Affected Controls
			butAdd.Enabled=false;
			checkLongDCodes.Visible=false;
			checkQuadAsToothNum.Visible=false;
			checkShowAccount.Visible=false;
			checkShowAppts.Visible=false;
			checkProcsAppt.Visible=false;
			comboModeTx.Enabled=false;
			groupShowDemographics.Visible=false;
			#endregion Internal Type Affected Controls
			#region IsInternal Affected Controls
			//butAdd.Enabled=false;//also part of internal type controls, must be type other than MedLabv2_3 and not internal for butAdd to be visible
			butDelete.Enabled=false;
			labelDelete.Visible=false;
			#endregion IsInternal Affected Controls
			#region Tx Mode Affected Controls
			butBrowseIn.Visible=false;
			butBrowseOut.Visible=false;
			comboLabImageCat.Visible=false;
			labelInSocketEx.Visible=false;
			labelLabImageCat.Visible=false;
			labelOutSocketOrDirEx.Visible=false;
			labelSftpPassword.Visible=false;
			labelSftpUsername.Visible=false;
			textSftpPassword.Visible=false;
			textSftpUsername.Visible=false;
			#endregion Tx Mode Affected Controls
			#region Set Enabled/Disabled Controls
			if(checkEnabled.Checked) {
				butBrowseIn.Enabled=true;
				butBrowseOut.Enabled=true;
				checkLongDCodes.Enabled=true;//set visible down further.
				checkQuadAsToothNum.Enabled=true;
				checkShowAccount.Enabled=true;
				checkShowAppts.Enabled=true;
				checkProcsAppt.Enabled=true;
				comboLabImageCat.Enabled=true;
				groupShowDemographics.Enabled=true;
				groupDelimeters.Enabled=true;
			}
			else {
				textDescription.ReadOnly=true;
				textHL7Server.ReadOnly=true;
				textHL7ServiceName.ReadOnly=true;
				textInPathOrSocket.ReadOnly=true;
				textOutPathSocketOrDir.ReadOnly=true;
				textSftpPassword.ReadOnly=true;
				textSftpUsername.ReadOnly=true;
			}
			#endregion Set Enabled/Disabled Controls
			#region Set Internal Type Controls
			if(HL7DefCur.InternalType!=HL7InternalType.MedLabv2_3) {
				if(!HL7DefCur.IsInternal) {
					butAdd.Enabled=true;
				}
				if(checkEnabled.Checked) {
					comboModeTx.Enabled=true;
				}
				checkQuadAsToothNum.Visible=true;
				checkShowAccount.Visible=true;
				checkShowAppts.Visible=true;
				checkProcsAppt.Visible=true;
				groupShowDemographics.Visible=true;
			}
			if(HL7DefCur.InternalType==HL7InternalType.eCWFull
				|| HL7DefCur.InternalType==HL7InternalType.eCWTight
				|| HL7DefCur.InternalType==HL7InternalType.eCWStandalone)
			{
				checkLongDCodes.Visible=true;
			}
			#endregion Set Internal Type Controls
			#region Set IsInternal Controls
			if(HL7DefCur.IsInternal) {
				labelDelete.Visible=true;
			}
			else {
				butDelete.Enabled=true;
			}
			#endregion Set IsInternal Controls
			#region Set Tx Mode Controls
			switch(comboModeTx.SelectedIndex) {
				case (int)ModeTxHL7.File:
					butBrowseIn.Visible=true;
					butBrowseOut.Visible=true;
					labelInPathOrSocket.Text="Inbound Folder";
					labelOutPathSocketOrDir.Text="Outbound Folder";
					textInPathOrSocket.Text=HL7DefCur.IncomingFolder;
					textOutPathSocketOrDir.Text=HL7DefCur.OutgoingFolder;
					break;
				case (int)ModeTxHL7.TcpIp:
					labelInPathOrSocket.Text="Inbound Port";
					labelInSocketEx.Text="Ex: 5845";
					labelInSocketEx.Visible=true;
					labelOutPathSocketOrDir.Text="Outbound IP:Port";
					labelOutSocketOrDirEx.Text="Ex: 192.168.0.23:5846";
					labelOutSocketOrDirEx.Visible=true;
					textInPathOrSocket.Text=HL7DefCur.IncomingPort;
					textOutPathSocketOrDir.Text=HL7DefCur.OutgoingIpPort;
					break;
				case (int)ModeTxHL7.Sftp:
					labelInPathOrSocket.Text="Sftp Server Address:Port";
					labelInSocketEx.Text="Ex: server.address.com:12345";
					labelInSocketEx.Visible=true;
					labelOutPathSocketOrDir.Text="Sftp Server Results Directory";
					labelOutSocketOrDirEx.Text="Ex: ./results";
					labelOutSocketOrDirEx.Visible=true;
					labelLabImageCat.Visible=true;
					labelSftpPassword.Visible=true;
					labelSftpUsername.Visible=true;
					textInPathOrSocket.Text=HL7DefCur.SftpInSocket;
					textOutPathSocketOrDir.Text=HL7DefCur.IncomingFolder;
					textSftpPassword.Visible=true;
					textSftpUsername.Visible=true;
					comboLabImageCat.Visible=true;
					break;
				default:
					break;
			}
			#endregion Set Tx Mode Controls
		}

		private void FillGrid() {
			//Our strategy in this window and all sub windows is to get all data directly from the database.
			if(!HL7DefCur.IsInternal && !HL7DefCur.IsNew) {
				HL7DefCur.hl7DefMessages=HL7DefMessages.GetDeepFromDb(HL7DefCur.HL7DefNum);
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Message"),150);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Seg"),35);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Note"),100);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			if(HL7DefCur!=null && HL7DefCur.hl7DefMessages!=null) {
				for(int i=0;i<HL7DefCur.hl7DefMessages.Count;i++) {
					GridRow row=new GridRow();
					row.Cells.Add(HL7DefCur.hl7DefMessages[i].MessageType.ToString()+", "+HL7DefCur.hl7DefMessages[i].MessageStructure.ToString()+", "+HL7DefCur.hl7DefMessages[i].InOrOut.ToString());
					row.Cells.Add("");
					row.Cells.Add(HL7DefCur.hl7DefMessages[i].Note);
					row.Tag=HL7DefCur.hl7DefMessages[i];
					gridMain.ListGridRows.Add(row);
					if(HL7DefCur.hl7DefMessages[i].hl7DefSegments!=null) {
						for(int j=0;j<HL7DefCur.hl7DefMessages[i].hl7DefSegments.Count;j++) {
							row=new GridRow();
							row.Cells.Add("");
							row.Cells.Add(HL7DefCur.hl7DefMessages[i].hl7DefSegments[j].SegmentName.ToString());
							row.Cells.Add(HL7DefCur.hl7DefMessages[i].hl7DefSegments[j].Note);
							row.Tag=HL7DefCur.hl7DefMessages[i];
							gridMain.ListGridRows.Add(row);
						}
					}
				}
			}
			gridMain.EndUpdate();
		}

		///<summary>Sets radio button for the current def's ShowDemographics setting.</summary>
		private void SetShowRadioButtons() {
			switch(HL7DefCur.ShowDemographics) {
				case HL7ShowDemographics.Hide:
					radioHide.Checked=true;
					break;
				case HL7ShowDemographics.Show:
					radioShow.Checked=true;
					break;
				case HL7ShowDemographics.Change:
					radioChange.Checked=true;
					break;
				case HL7ShowDemographics.ChangeAndAdd:
					radioChangeAndAdd.Checked=true;
					break;
			}
		}

		private void butBrowseIn_Click(object sender,EventArgs e) {
			FolderBrowserDialog dlg=new FolderBrowserDialog();
			dlg.SelectedPath=textInPathOrSocket.Text;
			if(dlg.ShowDialog()==DialogResult.OK) {
				textInPathOrSocket.Text=dlg.SelectedPath;
			}
		}

		private void butBrowseOut_Click(object sender,EventArgs e) {
			FolderBrowserDialog dlg=new FolderBrowserDialog();
			dlg.SelectedPath=textOutPathSocketOrDir.Text;
			if(dlg.ShowDialog()==DialogResult.OK) {
				textOutPathSocketOrDir.Text=dlg.SelectedPath;
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormHL7DefMessageEdit FormS=new FormHL7DefMessageEdit();
			FormS.HL7DefMesCur=(HL7DefMessage)gridMain.ListGridRows[e.Row].Tag;
			FormS.IsHL7DefInternal=HL7DefCur.IsInternal;
			FormS.InternalType=HL7DefCur.InternalType;
			FormS.ShowDialog();
			FillGrid();
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				if(gridMain.ListGridRows[i].Tag==gridMain.ListGridRows[e.Row].Tag) {
					gridMain.ListGridRows[i].ColorText=Color.Red;
				}
				else {
					gridMain.ListGridRows[i].ColorText=Color.Black;
				}
			}
			gridMain.Invalidate();
		}

		private void checkEnabled_CheckedChanged(object sender,EventArgs e) {
			SetControls();
		}

		private void checkEnabled_Click(object sender,EventArgs e) {
			if(checkEnabled.Checked) {
				bool isHL7Enabled=HL7Defs.IsExistingHL7Enabled(HL7DefCur.HL7DefNum,HL7DefCur.InternalType==HL7InternalType.MedLabv2_3);
				if(isHL7Enabled) {
					checkEnabled.Checked=false;
					MsgBox.Show(this,"Only one HL7 process can be enabled.  Another HL7 definition is enabled.");
					return;
				}
				if(Programs.IsEnabled(ProgramName.eClinicalWorks)) {
					MsgBox.Show(this,"The eClinicalWorks program link is enabled.  This definition will now control the HL7 messages.");
				}
			}
			else {
				//
			}
		}

		private void comboModeTx_SelectedIndexChanged(object sender,System.EventArgs e) {
			SetControls();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			//This button is only enabled if this is a custom def.
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete entire HL7Def?")) {
				return;
			}
			for(int m=0;m<HL7DefCur.hl7DefMessages.Count;m++) {
				for(int s=0;s<HL7DefCur.hl7DefMessages[m].hl7DefSegments.Count;s++) {
					for(int f=0;f<HL7DefCur.hl7DefMessages[m].hl7DefSegments[s].hl7DefFields.Count;f++) {
						HL7DefFields.Delete(HL7DefCur.hl7DefMessages[m].hl7DefSegments[s].hl7DefFields[f].HL7DefFieldNum);
					}
					HL7DefSegments.Delete(HL7DefCur.hl7DefMessages[m].hl7DefSegments[s].HL7DefSegmentNum);
				}
				HL7DefMessages.Delete(HL7DefCur.hl7DefMessages[m].HL7DefMessageNum);
			}
			HL7Defs.Delete(HL7DefCur.HL7DefNum);
			DataValid.SetInvalid(InvalidType.HL7Defs);
			DialogResult=DialogResult.OK;			
		}

		private void butAdd_Click(object sender,EventArgs e) {
			//This button is only enabled if this is a custom def.
			using FormHL7DefMessageEdit FormS=new FormHL7DefMessageEdit();
			FormS.HL7DefMesCur=new HL7DefMessage();
			FormS.HL7DefMesCur.HL7DefNum=HL7DefCur.HL7DefNum;
			FormS.HL7DefMesCur.IsNew=true;
			FormS.IsHL7DefInternal=false;
			FormS.InternalType=HL7DefCur.InternalType;
			FormS.ShowDialog();
			FillGrid();
		}

		///<summary>Make user enter password before allowing them to add patients through OD since this could be dangerous</summary>
		private void RadioAddPts_Click(object sender,EventArgs e) {
			using InputBox pwd=new InputBox("In our online manual, on the HL7 page, look for the password and enter it below.");
			if(pwd.ShowDialog()!=DialogResult.OK) {
				SetShowRadioButtons();
				return;
			}
			if(pwd.textResult.Text!="hl7") {
				MessageBox.Show("Wrong password.");
				SetShowRadioButtons();
				return;
			}
		}

		///<summary>If all of the entered data is valid, or if the def is not enabled, returns true.  Otherwise false.</summary>
		private bool ValidateData() {
			if(!checkEnabled.Checked) {
				return true;
			}
			if(ODBuild.IsWeb()) {
				MsgBox.Show(this,"HL7 is not supported in web mode.");
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
			switch(comboModeTx.SelectedIndex) {
				case (int)ModeTxHL7.File:
					if(textInPathOrSocket.Text=="") {
						MsgBox.Show(this,"The path for Inbound Folder is empty.");
						return false;
					}
					if(textOutPathSocketOrDir.Text=="") {
						MsgBox.Show(this,"The path for Outbound Folder is empty.");
						return false;
					}
					//paths are checked when service starts, not when closing form, since paths are local paths but only exist on the ODHL7 server
					break;
				case (int)ModeTxHL7.TcpIp:
					if(textInPathOrSocket.Text=="") {
						MsgBox.Show(this,"The Inbound Port is empty.");
						return false;
					}
					if(textOutPathSocketOrDir.Text=="") {
						MsgBox.Show(this,"The Outbound IP:Port is empty.");
						return false;
					}
					string[] strIpPort=textOutPathSocketOrDir.Text.Split(':');
					if(strIpPort.Length!=2) {//there isn't a ':' in the IpPort field
						MsgBox.Show(this,"The Outbound IP:Port field requires an IP address, followed by a colon, followed by a port number.");
						return false;
					}
					try {
						System.Net.IPAddress.Parse(strIpPort[0]);
					}
					catch {
						MsgBox.Show(this,"The Outbound IP address is invalid.");
						return false;
					}
					try {
						int.Parse(strIpPort[1]);
					}
					catch {
						MsgBox.Show(this,"The Outbound Port must be a valid integer.");
						return false;
					}
					try {
						int.Parse(textInPathOrSocket.Text.ToString());
					}
					catch {
						MsgBox.Show(this,"The Inbound Port must be a valid integer.");
						return false;
					}
					break;
				case (int)ModeTxHL7.Sftp:
					if(textInPathOrSocket.Text=="") {
						MsgBox.Show(this,"The Sftp Server Address:Port field is empty.");
						return false;
					}
					if(textSftpUsername.Text=="") {
						MsgBox.Show(this,"The Sftp Username field is empty.");
						return false;
					}
					if(textSftpPassword.Text=="") {
						MsgBox.Show(this,"The Sftp Password field is empty.");
						return false;
					}
					//NOTE: May not always require a port, so this test may not be necessary
					string[] strAddressPort=textInPathOrSocket.Text.Split(':');
					if(strAddressPort.Length>=2) {
						try {
							int.Parse(strAddressPort[1]);
						}
						catch(Exception ex) {
							ex.DoNothing();
							MsgBox.Show(this,"The Sftp Server Port must be a valid integer.");
							return false;
						}
					}
					break;
				default:
					break;
			}
			if(textFieldSep.Text.Length!=1) {
				MsgBox.Show(this,"The field separator must be a single character.");
				return false;
			}
			if(textRepSep.Text.Length!=1) {
				MsgBox.Show(this,"The repetition separator must be a single character.");
				return false;
			}
			if(textCompSep.Text.Length!=1) {
				MsgBox.Show(this,"The component separator must be a single character.");
				return false;
			}
			if(textSubcompSep.Text.Length!=1) {
				MsgBox.Show(this,"The subcomponent separator must be a single character.");
				return false;
			}
			if(textEscChar.Text.Length!=1) {
				MsgBox.Show(this,"The escape character must be a single character.");
				return false;
			}
			for(int i=0;i<HL7DefCur.hl7DefMessages.Count;i++) {
				if(HL7DefCur.InternalType==HL7InternalType.MedLabv2_3 || !checkShowAppts.Checked) {
					break;//The ShowAppts flag is hidden for MedLab, so we ignore MebLab defs here.
				}
				//if there is an inbound SIU defined, appts will be created from these messages and there is no overlap check
				//since OD has no control over the scheduling of these appts and inserts them all with OpNum=0, the appts module should be hidden
				if(HL7DefCur.hl7DefMessages[i].MessageType==MessageTypeHL7.SIU && HL7DefCur.hl7DefMessages[i].InOrOut==InOutHL7.Incoming) {
					MsgBox.Show(this,"The Appts module should be hidden if there is an inbound SIU message defined.\r\n"
						+"Either uncheck the Show Appts Module check box or delete the inbound SIU message definition.");
					return false;
				}
			}
			return true;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!ValidateData()) {
				return;
			}
			#region Set Values
			HL7DefCur.HL7Server=textHL7Server.Text;
			HL7DefCur.HL7ServiceName=textHL7ServiceName.Text;
			HL7DefCur.IsInternal=checkInternal.Checked;
			//this is read-only and cannot be changed, so no need to resave it.
			//HL7DefCur.InternalType=(HL7InternalType)Enum.Parse(typeof(HL7InternalType),textInternalType.Text);
			HL7DefCur.InternalTypeVersion=textInternalTypeVersion.Text;
			HL7DefCur.Description=textDescription.Text;
			HL7DefCur.FieldSeparator=textFieldSep.Text;
			HL7DefCur.RepetitionSeparator=textRepSep.Text;
			HL7DefCur.ComponentSeparator=textCompSep.Text;
			HL7DefCur.SubcomponentSeparator=textSubcompSep.Text;
			HL7DefCur.EscapeCharacter=textEscChar.Text;
			HL7DefCur.Note=textNote.Text;
			HL7DefCur.ModeTx=(ModeTxHL7)comboModeTx.SelectedIndex;
			if(radioHide.Checked) {
				HL7DefCur.ShowDemographics=HL7ShowDemographics.Hide;
			}
			else if(radioShow.Checked) {
				HL7DefCur.ShowDemographics=HL7ShowDemographics.Show;
			}
			else if(radioChange.Checked) {
				HL7DefCur.ShowDemographics=HL7ShowDemographics.Change;
			}
			else {//must be ChangeAndAdd
				HL7DefCur.ShowDemographics=HL7ShowDemographics.ChangeAndAdd;
			}
			HL7DefCur.ShowAccount=checkShowAccount.Checked;
			HL7DefCur.ShowAppts=checkShowAppts.Checked;
			HL7DefCur.IsQuadAsToothNum=checkQuadAsToothNum.Checked;
			HL7DefCur.HasLongDCodes=checkLongDCodes.Checked;
			HL7DefCur.IsProcApptEnforced=checkProcsAppt.Checked;
			//clear all fields in order to save the relevant data in the proper fields and clear out data that may not be relevant for the Tx mode
			HL7DefCur.IncomingFolder="";
			HL7DefCur.OutgoingFolder="";
			HL7DefCur.IncomingPort="";
			HL7DefCur.OutgoingIpPort="";
			HL7DefCur.SftpInSocket="";
			HL7DefCur.SftpUsername="";
			HL7DefCur.SftpPassword="";
			if(comboModeTx.SelectedIndex==(int)ModeTxHL7.File) {
				HL7DefCur.IncomingFolder=textInPathOrSocket.Text.Trim();
				HL7DefCur.OutgoingFolder=textOutPathSocketOrDir.Text.Trim();
			}
			else if(comboModeTx.SelectedIndex==(int)ModeTxHL7.TcpIp) {
				HL7DefCur.IncomingPort=textInPathOrSocket.Text.Trim();
				HL7DefCur.OutgoingIpPort=textOutPathSocketOrDir.Text.Trim();
			}
			else if(comboModeTx.SelectedIndex==(int)ModeTxHL7.Sftp) {
				HL7DefCur.SftpInSocket=textInPathOrSocket.Text.Trim();
				HL7DefCur.IncomingFolder=textOutPathSocketOrDir.Text.Trim();
				HL7DefCur.SftpUsername=textSftpUsername.Text.Trim();
				HL7DefCur.SftpPassword=CDT.Class1.TryEncrypt(textSftpPassword.Text.Trim());
			}
			if(comboLabImageCat.SelectedIndex>=0) {
				HL7DefCur.LabResultImageCat=_listImageCatDefs[comboLabImageCat.SelectedIndex].DefNum;
			}
			#endregion Set Values
			#region Save
			if(checkEnabled.Checked) {
				HL7DefCur.IsEnabled=true;
				if(checkInternal.Checked){
					if(HL7Defs.GetInternalFromDb(HL7DefCur.InternalType)==null){ //it's not in the database.
						HL7Defs.Insert(HL7DefCur);//The user wants to enable this, so we will need to save this def to the db.
					}
					else {
						HL7Defs.Update(HL7DefCur);
					}
				}
				else {//all custom defs are already in the db.
					HL7Defs.Update(HL7DefCur);
				}
			}
			else {//not enabled
				if(HL7DefCur.IsInternal) {
					if(HL7DefCur.IsEnabled) {//If def was enabled but user wants to disable
						if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"Disable HL7Def?  Changes made will be lost.  Continue?")) {
							HL7Defs.Delete(HL7DefCur.HL7DefNum);
						}
						else {//user selected Cancel
							return;
						}
					}
					else {//was disabled and is still disabled
						if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Changes made will be lost.  Continue?")) {
							return;
						}
						//do nothing.  Changes will be lost.
					}
				}
				else {//custom
					//Disable the custom def
					HL7DefCur.IsEnabled=false;
					HL7Defs.Update(HL7DefCur);
				}
			}
			#endregion Save
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
