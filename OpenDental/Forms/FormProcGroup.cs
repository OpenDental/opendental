/*=============================================================================================================
Open Dental GPL license Copyright (C) 2003  Jordan Sparks, DMD.  http://www.open-dent.com,  www.docsparks.com
See header in FormOpenDental.cs for complete text.  Redistributions must retain this text.
===============================================================================================================*/
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;
using System.Data;
using Microsoft.Win32;
using OpenDentBusiness;
using CodeBase;
using SparksToothChart;
using OpenDental.UI;
using System.Text.RegularExpressions;

namespace OpenDental{
///<summary></summary>
	public partial class FormProcGroup:FormODBase {
		private ErrorProvider errorProvider2=new ErrorProvider();
		public List<ClaimProcHist> HistList;
		public Procedure GroupCur;
		private Procedure GroupOld;
		public List<ProcGroupItem> GroupItemList;
		public List<Procedure> ProcList;
		private List<Procedure> ProcListOld;
		private List<OrionProc> OrionProcList;
		///<summary>This keeps the noteChanged event from erasing the signature when first loading.</summary>
		private bool IsStartingUp;
		private bool SigChanged;
		private PatField[] PatFieldList;
		private Patient PatCur;
		private Family FamCur;
		///<summary>Used when making an Rx.  Only used when the Rx button is pushed when Orion is enabled.</summary>
		public static bool IsOpen;
		///<summary>Used when making an Rx.  Only used when the Rx button is pushed when Orion is enabled.</summary>
		public static long RxNum;
		private DataTable TablePlanned;
		///<summary>Users can temporarily log in on this form.  Defaults to Security.CurUser.</summary>
		private Userod _curUser=Security.CurUser;
		///<summary>True if the user clicked the Change User button.</summary>
		private bool _hasUserChanged;
		private List<PatFieldDef> _listPatFieldDefs;
		///<summary>True if group note is attached to at least one completed proc.  Used for determining which permission to use.</summary>
		private bool _attachedToCompletedProc;
		private const string _autoNotePromptRegex=@"\[Prompt:""[a-zA-Z_0-9 ]+""\]";

		public FormProcGroup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		///<summary>Inserts are no longer done within this dialog, but must be done ahead of time from outside.You must specify a procedure to edit, and only the changes that are made in this dialog get saved.  Only used when double click in Account, Chart, TP, and in ContrChart.AddProcedure().  The procedure may be deleted if new, and user hits Cancel.</summary>

		//Constructor from ProcEdit. Lots of this will need to be copied into the new Load function.
		/*public FormProcGroup(long groupNum) {
			GroupCur=Procedures.GetOneProc(groupNum,true);
			ProcGroupItem=ProcGroupItems.Refresh(groupNum);
			//Proc
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}*/

		private void FormProcGroup_Load(object sender, System.EventArgs e){
			signatureBoxWrapper.SetAllowDigitalSig(true);
			IsOpen=true;
			IsStartingUp=true;
			//ProcList gets set in ContrChart where this form is created.
			PatCur=Patients.GetPat(GroupCur.PatNum);
			FamCur=Patients.GetFamily(GroupCur.PatNum);
			GroupOld=GroupCur.Copy();
			ProcListOld=new List<Procedure>();
			for(int i=0;i<ProcList.Count;i++){
				ProcListOld.Add(ProcList[i].Copy());
			}
			ModifyForOrionMode();
			textProcDate.Text=GroupCur.ProcDate.ToShortDateString();
			textDateEntry.Text=GroupCur.DateEntryC.ToShortDateString();
			textUser.Text=Userods.GetName(GroupCur.UserNum);//might be blank. Will change automatically if user changes note or alters sig.
			textNotes.Text=GroupCur.Note;
			if(GroupCur.ProcStatus==ProcStat.EC && PrefC.GetBool(PrefName.ProcLockingIsAllowed) && !GroupCur.IsLocked) {
				butLock.Visible=true;
			}
			else {
				butLock.Visible=false;
			}
			Permissions perm=Permissions.ProcCompleteEdit;
			if(ListTools.In(GroupCur.ProcStatus,ProcStat.EO,ProcStat.EC)) {
				perm=Permissions.ProcExistingEdit;
			}
			if(Security.IsGlobalDateLock(perm,GroupCur.ProcDate)) {
				butLock.Enabled=false;
			}
			if(GroupCur.IsLocked) {//Whether locking is currently allowed, this proc group may have been locked previously.
				butOK.Enabled=false;
				butDelete.Enabled=false;
				labelLocked.Visible=true;
				butAppend.Visible=true;
				textNotes.ReadOnly=true;//just for visual cue.  No way to save changes, anyway.
				textNotes.BackColor=SystemColors.Control;
				butInvalidate.Visible=true;
				butInvalidate.Location=butLock.Location;
			}
			else {
				butInvalidate.Visible=false;
				perm=Permissions.ProcDelete;
				DateTime dateForPerm=GroupCur.DateEntryC;
				//because islocked overrides security:
				_attachedToCompletedProc=(ProcGroupItems.GetCountCompletedProcsForGroup(GroupCur.ProcNum)!=0);
				if(_attachedToCompletedProc) {
					perm=Permissions.ProcCompleteNote;
					dateForPerm=GroupCur.ProcDate;
				}
				//This is mainly to make sure that the global security lock date is considered.
				if(!Security.IsAuthorized(perm,dateForPerm)) {
					butOK.Enabled=false;
					butDelete.Enabled=false;
					textNotes.ReadOnly=true;
					textNotes.BackColor=SystemColors.Control;
					butAppend.Enabled=false;
					butChangeUser.Enabled=false;
					signatureBoxWrapper.Enabled=false;
					buttonUseAutoNote.Enabled=false;
					butLock.Enabled=false;
					butInvalidate.Enabled=false;
				}
			}
			if(GroupCur.ProcStatus==ProcStat.D) {//an invalidated proc
				labelInvalid.Visible=true;
				butInvalidate.Visible=false;
				labelLocked.Visible=false;
				butAppend.Visible=false;
				butOK.Enabled=false;
				butDelete.Enabled=false;
			}
			FillProcedures();
			textNotes.Select();
			string keyData=GetSignatureKey();
			signatureBoxWrapper.FillSignature(GroupCur.SigIsTopaz,keyData,GroupCur.Signature);
			signatureBoxWrapper.BringToFront();
			if(!(Security.IsAuthorized(Permissions.GroupNoteEditSigned,true) || signatureBoxWrapper.SigIsBlank || GroupCur.UserNum==Security.CurUser.UserNum)) {
				//User does not have permission and this note was signed by someone else.
				textNotes.ReadOnly=true;
				signatureBoxWrapper.Enabled=false;
				labelPermAlert.Visible=true;
				butAppend.Enabled=false;
				buttonUseAutoNote.Enabled=false;
				butChangeUser.Enabled=false;
			}
			else if(!Userods.CanUserSignNote()) {
				signatureBoxWrapper.Enabled=false;
				labelPermAlert.Visible=true;
				labelPermAlert.Text=Lans.g(this,"Notes can only be signed by providers.");
			}
			_listPatFieldDefs=PatFieldDefs.GetDeepCopy(true);
			FillPatientData();
			FillPlanned();
			textNotes.Select(textNotes.Text.Length,0);
			IsStartingUp=false;
			butEditAutoNote.Visible=HasAutoNotePrompt();
			//string retVal=GroupCur.Note+GroupCur.UserNum.ToString();
			//using MsgBoxCopyPaste msgb=new MsgBoxCopyPaste(retVal);
			//msgb.ShowDialog();
		}

		private void FillPatientData(){
			if(PatCur==null){
				gridPat.BeginUpdate();
				gridPat.ListGridRows.Clear();
				gridPat.ListGridColumns.Clear();
				gridPat.EndUpdate();
				return;
			}
			gridPat.BeginUpdate();
			gridPat.ListGridColumns.Clear();
			GridColumn col=new GridColumn("",150);
			gridPat.ListGridColumns.Add(col);
			col=new GridColumn("",250);
			gridPat.ListGridColumns.Add(col);
			gridPat.ListGridRows.Clear();
			GridRow row;
			PatFieldList=PatFields.Refresh(PatCur.PatNum);
			List<DisplayField> fields=DisplayFields.GetForCategory(DisplayFieldCategory.PatientInformation);
			for(int f=0;f<fields.Count;f++) {
				row=new GridRow();
				if(fields[f].Description==""){
					//...
				}
				else{
					if(fields[f].InternalName=="PatFields") {
						//don't add a cell
					}
					else {
						row.Cells.Add(fields[f].Description);
					}
				}
				switch(fields[f].InternalName){
					//...
					case "PatFields":
						PatField field;
						List<FieldDefLink> listFieldDefLinks=FieldDefLinks.GetForLocation(FieldLocations.GroupNote);
						for(int i=0;i<_listPatFieldDefs.Count;i++) {
							if(listFieldDefLinks.Exists(x => x.FieldDefNum==_listPatFieldDefs[i].PatFieldDefNum)) {
								continue;
							}
							if(i>0){
								row=new GridRow();
							}
							row.Cells.Add(_listPatFieldDefs[i].FieldName);
							field=PatFields.GetByName(_listPatFieldDefs[i].FieldName,PatFieldList);
							if(field==null){
								row.Cells.Add("");
							}
							else{
								if(_listPatFieldDefs[i].FieldType==PatFieldType.Checkbox) {
									row.Cells.Add("X");
								}
								else {
									row.Cells.Add(field.FieldValue);
								}
							}
							row.Tag="PatField"+i.ToString();
							gridPat.ListGridRows.Add(row);
						}
						break;
				}
				if(fields[f].InternalName=="PatFields"){
					//don't add the row here
				}
				else{
					gridPat.ListGridRows.Add(row);
				}
			}
			gridPat.EndUpdate();
		}

		private void FillProcedures(){
			gridProc.BeginUpdate();
			gridProc.ListGridColumns.Clear();
			GridColumn col;
			DisplayFields.RefreshCache();//probably needs to be removed
			List<DisplayField> fields=DisplayFields.GetForCategory(DisplayFieldCategory.ProcedureGroupNote);
			for(int i=0;i<fields.Count;i++) {
				if(fields[i].Description=="") {
					col=new GridColumn(fields[i].InternalName,fields[i].ColumnWidth);
				}
				else {
					col=new GridColumn(fields[i].Description,fields[i].ColumnWidth);
				}
				if(fields[i].InternalName=="Amount") {
					col.TextAlign=HorizontalAlignment.Right;
				}
				if(fields[i].InternalName=="Proc Code") {
					col.TextAlign=HorizontalAlignment.Center;
				}
				gridProc.ListGridColumns.Add(col);
			}
			gridProc.ListGridRows.Clear();
			for(int i=0;i<ProcList.Count;i++) {
				GridRow row=new GridRow();
				for(int f=0;f<fields.Count;f++) {
					switch(fields[f].InternalName) {
						case "Date":
							row.Cells.Add(ProcList[i].ProcDate.ToShortDateString());
							break;
						case "Th":
							row.Cells.Add(Tooth.GetToothLabel(ProcList[i].ToothNum));
							break;
						case "Surf":
							row.Cells.Add(ProcList[i].Surf);
							break;
						case "Description":
							row.Cells.Add(ProcedureCodes.GetLaymanTerm(ProcList[i].CodeNum));
							break;
						case "Stat":
							if(ProcMultiVisits.IsProcInProcess(ProcList[i].ProcNum)) {
								row.Cells.Add(Lan.g("enumProcStat",ProcStatExt.InProcess));
							}
							else {
								row.Cells.Add(Lan.g("enumProcStat",ProcList[i].ProcStatus.ToString()));
							}
							break;
						case "Prov":
							row.Cells.Add(Providers.GetAbbr(ProcList[i].ProvNum));
							break;
						case "Amount":
							row.Cells.Add(ProcList[i].ProcFee.ToString("F"));
							break;
						case "Proc Code":
							row.Cells.Add(ProcedureCodes.GetStringProcCode(ProcList[i].CodeNum));
							break;
						case "Stat 2":
							row.Cells.Add(((OrionStatus)OrionProcList[i].Status2).ToString());
							break;
						case "On Call":
							if(OrionProcList[i].IsOnCall) {
								row.Cells.Add("Y");
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Effective Comm":
							if(OrionProcList[i].IsEffectiveComm) {
								row.Cells.Add("Y");
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Repair":
							if(OrionProcList[i].IsRepair) {
								row.Cells.Add("Y");
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "DPCpost":
							row.Cells.Add(((OrionDPC)OrionProcList[i].DPCpost).ToString());
							break;
					}
				}
				gridProc.ListGridRows.Add(row);
			}
			gridProc.EndUpdate();
		}

		private void ModifyForOrionMode(){
			if(Programs.UsingOrion){
				OrionProcList=new List<OrionProc>();
				for(int i=0;i<ProcList.Count;i++){
					OrionProcList.Add(OrionProcs.GetOneByProcNum(ProcList[i].ProcNum));
				}
				labelOnCall.Visible=true;
				butOnCallY.Visible=true;
				butOnCallN.Visible=true;
				labelEffectiveComm.Visible=true;
				butEffectiveCommY.Visible=true;
				butEffectiveCommN.Visible=true;
				for(int i=0;i<ProcList.Count;i++){
					if(ProcedureCodes.GetProcCodeFromDb(ProcList[i].CodeNum).IsProsth){
						labelRepair.Visible=true;
						butRepairY.Visible=true;
						butRepairN.Visible=true;
					}
				}
				butRx.Visible=true;
				butExamSheets.Visible=true;
				panelPlanned.Visible=true;
				gridPat.Visible=true;
				textProcDate.ReadOnly=false;
				labelDPCpost.Visible=true;
				comboDPCpost.Visible=true;
				comboDPCpost.Items.Clear();
				comboDPCpost.Items.Add("Not Specified");
				comboDPCpost.Items.Add("None");
				comboDPCpost.Items.Add("1A-within 1 day");
				comboDPCpost.Items.Add("1B-within 30 days");
				comboDPCpost.Items.Add("1C-within 60 days");
				comboDPCpost.Items.Add("2-within 120 days");
				comboDPCpost.Items.Add("3-within 1 year");
				comboDPCpost.Items.Add("4-no further treatment/appt");
				comboDPCpost.Items.Add("5-no appointment needed");
			}
			else{
				this.ClientSize=new Size(LayoutManager.Scale(556),LayoutManager.Scale(645));
			}
		}

		private void RefreshGrids(){
			FillPatientData();
			FillProcedures();
			FillPlanned();
		}
		
		#region Planned
		private void FillPlanned(){
			if(PatCur==null){
				butNew.Enabled=false;
				butClear.Enabled=false;
				butUp.Enabled=false;
				butDown.Enabled=false;
				gridPlanned.Enabled=false;
				return;
			}
			else{
				butNew.Enabled=true;
				butClear.Enabled=true;
				butUp.Enabled=true;
				butDown.Enabled=true;
				gridPlanned.Enabled=true;
			}
			//Fill grid
			gridPlanned.BeginUpdate();
			gridPlanned.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TablePlannedAppts","#"),15,HorizontalAlignment.Center);
			gridPlanned.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TablePlannedAppts","Min"),25);
			gridPlanned.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TablePlannedAppts","Procedures"),160);
			gridPlanned.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TablePlannedAppts","Note"),115);
			gridPlanned.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TablePlannedAppts","SchedBy"),50);
			gridPlanned.ListGridColumns.Add(col);
			gridPlanned.ListGridRows.Clear();
			GridRow row;
			TablePlanned=ChartModules.GetPlannedApt(PatCur.PatNum);
			//This gets done in the business layer:
			/*
			bool iochanged=false;
			for(int i=0;i<table.Rows.Count;i++) {
				if(table.Rows[i]["ItemOrder"].ToString()!=i.ToString()) {
					PlannedAppt planned=PlannedAppts.CreateObject(PIn.PLong(table.Rows[i]["PlannedApptNum"].ToString()));
					planned.ItemOrder=i;
					PlannedAppts.InsertOrUpdate(planned);
					iochanged=true;
				}
			}
			if(iochanged) {
				DataSetMain=ChartModules.GetAll(PatCur.PatNum,checkAudit.Checked);
				table=DataSetMain.Tables["Planned"];
			}*/
			for(int i=0;i<TablePlanned.Rows.Count;i++){
				row=new GridRow();
				row.Cells.Add(TablePlanned.Rows[i]["ItemOrder"].ToString());
				row.Cells.Add(TablePlanned.Rows[i]["minutes"].ToString());
				row.Cells.Add(TablePlanned.Rows[i]["ProcDescript"].ToString());
				row.Cells.Add(TablePlanned.Rows[i]["Note"].ToString());
				string text;
				List<Procedure> procsList=Procedures.Refresh(PatCur.PatNum);
				DateTime newDateSched=new DateTime();
				for(int p=0;p<procsList.Count;p++) {
					if(procsList[p].PlannedAptNum==PIn.Long(TablePlanned.Rows[i]["AptNum"].ToString())) {
						OrionProc op=OrionProcs.GetOneByProcNum(procsList[p].ProcNum);
						if(op!=null && op.DateScheduleBy.Year>1880) {
							if(newDateSched.Year<1880) {
								newDateSched=op.DateScheduleBy;
							}
							else {
								if(op.DateScheduleBy<newDateSched) {
									newDateSched=op.DateScheduleBy;
								}
							}
						}
					}
				}
				if(newDateSched.Year>1880) {
					text=newDateSched.ToShortDateString();
				}
				else {
					text="None";
				}
				row.Cells.Add(text);
				row.ColorText=Color.FromArgb(PIn.Int(TablePlanned.Rows[i]["colorText"].ToString()));
				row.ColorBackG=Color.FromArgb(PIn.Int(TablePlanned.Rows[i]["colorBackG"].ToString()));
				gridPlanned.ListGridRows.Add(row);
			}
			gridPlanned.EndUpdate();
		}

		private void butNew_Click(object sender,EventArgs e) {
			/*if(ApptPlanned.Visible){
				if(MessageBox.Show(Lan.g(this,"Replace existing planned appointment?")
					,"",MessageBoxButtons.OKCancel)!=DialogResult.OK)
					return;
				//Procedures.UnattachProcsInPlannedAppt(ApptPlanned.Info.MyApt.AptNum);
				AppointmentL.Delete(PIn.PInt(ApptPlanned.DataRoww["AptNum"].ToString()));
			}*/
			if(!Security.IsAuthorized(Permissions.AppointmentCreate)) {
				return;
			}
			if(PatRestrictionL.IsRestricted(PatCur.PatNum,PatRestrict.ApptSchedule)) {
				return;
			}
			Appointment AptCur=new Appointment();
			AptCur.PatNum=PatCur.PatNum;
			AptCur.ProvNum=PatCur.PriProv;
			AptCur.ClinicNum=PatCur.ClinicNum;
			AptCur.AptStatus=ApptStatus.Planned;
			AptCur.AptDateTime=DateTime.Today;
			AptCur.Pattern="/X/";
			AptCur.TimeLocked=PrefC.GetBool(PrefName.AppointmentTimeIsLocked);
			Appointments.Insert(AptCur);
			PlannedAppt plannedAppt=new PlannedAppt();
			plannedAppt.AptNum=AptCur.AptNum;
			plannedAppt.PatNum=PatCur.PatNum;
			plannedAppt.ItemOrder=TablePlanned.Rows.Count+1;
			PlannedAppts.Insert(plannedAppt);
			using FormApptEdit FormApptEdit2=new FormApptEdit(AptCur.AptNum);
			FormApptEdit2.IsNew=true;
			FormApptEdit2.ShowDialog();
			if(FormApptEdit2.DialogResult!=DialogResult.OK){
				//delete new appt, delete plannedappt, and unattach procs already handled in dialog
				RefreshGrids();
				return;
			}
			List<Procedure> myProcList=Procedures.Refresh(PatCur.PatNum);
			bool allProcsHyg=true;
			for(int i=0;i<myProcList.Count;i++){
				if(myProcList[i].PlannedAptNum!=AptCur.AptNum)
					continue;//only concerned with procs on this plannedAppt
				if(!ProcedureCodes.GetProcCode(myProcList[i].CodeNum).IsHygiene){
					allProcsHyg=false;
					break;
				}
			}
			if(allProcsHyg && PatCur.SecProv!=0){
				Appointment aptOld=AptCur.Copy();
				AptCur.ProvNum=PatCur.SecProv;
				Appointments.Update(AptCur,aptOld);
			}
			Patient patOld=PatCur.Copy();
			//PatCur.NextAptNum=AptCur.AptNum;
			PatCur.PlannedIsDone=false;
			Patients.Update(PatCur,patOld);
			RefreshGrids();//if procs were added in appt, then this will display them
		}

		private void butClear_Click(object sender,EventArgs e) {
			if(gridPlanned.SelectedIndices.Length==0){
				MsgBox.Show(this,"Please select an item first");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete planned appointment(s)?")){
				return;
			}
			for(int i=0;i<gridPlanned.SelectedIndices.Length;i++){
				Appointments.Delete(PIn.Long(TablePlanned.Rows[gridPlanned.SelectedIndices[i]]["AptNum"].ToString()),true);
			}
			RefreshGrids();
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(gridPlanned.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			if(gridPlanned.SelectedIndices.Length>1) {
				MsgBox.Show(this,"Please only select one item first.");
				return;
			}
			int idx=gridPlanned.SelectedIndices[0];
			if(idx==0) {
				return;
			}
			PlannedAppt planned;
			planned=PlannedAppts.GetOne(PIn.Long(TablePlanned.Rows[idx]["PlannedApptNum"].ToString()));
			planned.ItemOrder=idx-1;
			PlannedAppts.Update(planned);
			planned=PlannedAppts.GetOne(PIn.Long(TablePlanned.Rows[idx-1]["PlannedApptNum"].ToString()));
			planned.ItemOrder=idx;
			PlannedAppts.Update(planned);
			TablePlanned=ChartModules.GetPlannedApt(PatCur.PatNum);
			RefreshGrids();
			gridPlanned.SetSelected(idx-1,true);
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(gridPlanned.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			if(gridPlanned.SelectedIndices.Length>1) {
				MsgBox.Show(this,"Please only select one item first.");
				return;
			}
			int idx=gridPlanned.SelectedIndices[0];
			if(idx==TablePlanned.Rows.Count-1) {
				return;
			}
			PlannedAppt planned;
			planned=PlannedAppts.GetOne(PIn.Long(TablePlanned.Rows[idx]["PlannedApptNum"].ToString()));
			planned.ItemOrder=idx+1;
			PlannedAppts.Update(planned);
			planned=PlannedAppts.GetOne(PIn.Long(TablePlanned.Rows[idx+1]["PlannedApptNum"].ToString()));
			planned.ItemOrder=idx;
			PlannedAppts.Update(planned);
			TablePlanned=ChartModules.GetPlannedApt(PatCur.PatNum);
			RefreshGrids();
			gridPlanned.SetSelected(idx+1,true);
		}

		private void gridPlanned_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			long aptnum=PIn.Long(TablePlanned.Rows[e.Row]["AptNum"].ToString());
			using FormApptEdit FormAE=new FormApptEdit(aptnum);
			FormAE.ShowDialog();
			if(FormAE.DialogResult==DialogResult.OK) {
				RefreshGrids();
			}
			for(int i=0;i<TablePlanned.Rows.Count;i++){
				if(TablePlanned.Rows[i]["AptNum"].ToString()==aptnum.ToString()){
					gridPlanned.SetSelected(i,true);
				}
			}
		}
		#endregion Planned

		private void butRx_Click(object sender,EventArgs e) {
			//only visible in Orion mode
			if(!Security.IsAuthorized(Permissions.RxCreate)) {
				return;
			}
			using FormRxSelect FormRS=new FormRxSelect(PatCur);
			FormRS.ShowDialog();
			if(FormRS.DialogResult!=DialogResult.OK) {
				return;
			}
			SecurityLogs.MakeLogEntry(Permissions.RxCreate,PatCur.PatNum,PatCur.GetNameLF());
			RxPat Rx=RxPats.GetRx(RxNum);
			if(textNotes.Text!=""){
				textNotes.Text+="\r\n";
			}
			textNotes.Text+="Rx - "+Rx.Drug+" - #"+Rx.Disp;
			string rxNote=Pharmacies.GetDescription(RxNum);
			if(rxNote!=""){
				textNotes.Text+="\r\n"+rxNote;
			}
		}

		private void buttonUseAutoNote_Click(object sender,EventArgs e) {
			using FormAutoNoteCompose FormA=new FormAutoNoteCompose();
			FormA.ShowDialog();
			if(FormA.DialogResult==DialogResult.OK) {
				textNotes.AppendText(FormA.CompletedNote);
				butEditAutoNote.Visible=HasAutoNotePrompt();
			}
		}

		private void ButEditAutoNote_Click(object sender,EventArgs e) {
			if(HasAutoNotePrompt()) {
				using FormAutoNoteCompose FormA=new FormAutoNoteCompose();
				FormA.MainTextNote=textNotes.Text;
				FormA.ShowDialog();
				if(FormA.DialogResult==DialogResult.OK) {
					textNotes.Text=FormA.CompletedNote;
					butEditAutoNote.Visible=HasAutoNotePrompt();
				}
			}
			else {
				MessageBox.Show(Lan.g(this,"No Auto Note available to edit."));
			}
		}

		private bool HasAutoNotePrompt() {
			return Regex.IsMatch(textNotes.Text,_autoNotePromptRegex);
		}

		private void butExamSheets_Click(object sender,EventArgs e) {
			using FormExamSheets fes=new FormExamSheets();
			fes.PatNum=GroupCur.PatNum;
			fes.ShowDialog();
			//TODO: Print a note about Exam Sheet added.
		}

		private void textNotes_TextChanged(object sender,EventArgs e) {
			if(!IsStartingUp//so this happens only if user changes the note
				&& !SigChanged)//and the original signature is still showing.
			{
				//SigChanged=true;//happens automatically through the event.
				signatureBoxWrapper.ClearSignature();
			}
		}

		private string GetSignatureKey(){
			string keyData=GroupCur.ProcDate.ToShortDateString();
			keyData+=GroupCur.DateEntryC.ToShortDateString();
			keyData+=GroupCur.UserNum.ToString();//Security.CurUser.UserName;
			keyData+=GroupCur.Note;
			GroupItemList=ProcGroupItems.GetForGroup(GroupCur.ProcNum);//Orders the list to ensure same key in all cases.
			for(int i=0;i<GroupItemList.Count;i++){
				keyData+=GroupItemList[i].ProcGroupItemNum.ToString();
			}
			keyData=keyData.Replace("\r\n","\n");//We need all newlines to be the same, a mix of \r\n and \n can invalidate the procedure signature.
			return keyData;
		}

		private void butChangeUser_Click(object sender,EventArgs e) {
			using FormLogOn FormChangeUser=new FormLogOn(isSimpleSwitch:true);
			FormChangeUser.ShowDialog();
			if(FormChangeUser.DialogResult==DialogResult.OK) {
				_curUser=FormChangeUser.CurUserSimpleSwitch; //assign temp user
				bool canUserSignNote=Userods.CanUserSignNote(_curUser);//only show if user can sign
				signatureBoxWrapper.Enabled=canUserSignNote;
				if(!labelPermAlert.Visible && !canUserSignNote) {
					labelPermAlert.Text=Lans.g(this,"Notes can only be signed by providers.");
					labelPermAlert.Visible=true;
				}
				signatureBoxWrapper.ClearSignature(); //clear sig
				signatureBoxWrapper.UserSig=_curUser;
				textUser.Text=_curUser.UserName; //update user textbox.
				SigChanged=true;
				_hasUserChanged=true;
			}
		}

		private void SaveSignature(){
			if(SigChanged){
				string keyData=GetSignatureKey();
				GroupCur.Signature=signatureBoxWrapper.GetSignature(keyData);
				GroupCur.SigIsTopaz=signatureBoxWrapper.GetSigIsTopaz();
			}
		}

		private void signatureBoxWrapper_SignatureChanged(object sender,EventArgs e) {
			GroupCur.UserNum=_curUser.UserNum;
			textUser.Text=_curUser.UserName;
			SigChanged=true;
		}

		private void butOnCallY_Click(object sender,EventArgs e) {			
			for(int i=0;i<OrionProcList.Count;i++){
				OrionProcList[i].IsOnCall=true;
			}
			FillProcedures();
		}

		private void butOnCallN_Click(object sender,EventArgs e) {
			for(int i=0;i<OrionProcList.Count;i++){
				OrionProcList[i].IsOnCall=false;
			}
			FillProcedures();
		}

		private void butEffectiveCommY_Click(object sender,EventArgs e) {
			for(int i=0;i<OrionProcList.Count;i++){
				OrionProcList[i].IsEffectiveComm=true;
			}
			FillProcedures();
		}

		private void butEffectiveCommN_Click(object sender,EventArgs e) {
			for(int i=0;i<OrionProcList.Count;i++){
				OrionProcList[i].IsEffectiveComm=false;
			}
			FillProcedures();
		}

		private void butRepairY_Click(object sender,EventArgs e) {
			for(int i=0;i<OrionProcList.Count;i++){
				if(ProcedureCodes.GetProcCodeFromDb(ProcList[i].CodeNum).IsProsth){//OrionProcList[i] corresponds to ProcList[i]
					OrionProcList[i].IsRepair=true;
				}
			}
			FillProcedures();
		}

		private void butRepairN_Click(object sender,EventArgs e) {
			for(int i=0;i<OrionProcList.Count;i++){
				if(ProcedureCodes.GetProcCodeFromDb(ProcList[i].CodeNum).IsProsth){//OrionProcList[i] corresponds to ProcList[i]
					OrionProcList[i].IsRepair=false;
				}
			}
			FillProcedures();
		}

		private void comboDPCpost_SelectionChangeCommitted(object sender,EventArgs e) {
			for(int i=0;i<OrionProcList.Count;i++) {
				OrionProcList[i].DPCpost=(OrionDPC)comboDPCpost.SelectedIndex;
			}
			FillProcedures();
		}

		///<summary>This button is only visible if 1. Pref ProcLockingIsAllowed is true, 2. Proc isn't already locked, 3. Proc status is C.</summary>
		private void butLock_Click(object sender,EventArgs e) {
			if(!EntriesAreValid()) {
				return;
			}
			GroupCur.IsLocked=true;
			SaveAndClose();//saves all the other various changes that the user made
		}

		///<summary>This button is only visible when proc IsLocked.</summary>
		private void butInvalidate_Click(object sender,EventArgs e) {
			//What this will really do is "delete" the procedure.
			if(!Security.IsAuthorized(Permissions.ProcDelete,GroupCur.DateEntryC)) {
				return;
			}
			if(!Security.IsAuthorized(Permissions.GroupNoteEditSigned)) {
				return;
			}
			try {
				Procedures.Delete(GroupCur.ProcNum);//also deletes any claimprocs (other than ins payments of course).
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			//Log entry does not show procstatus because group notes don't technically have a status, always EC.
			SecurityLogs.MakeLogEntry(Permissions.ProcDelete,PatCur.PatNum,Lan.g(this,"Invalidated: ")+
				ProcedureCodes.GetStringProcCode(GroupCur.CodeNum).ToString()+", "+GroupCur.ProcDate.ToShortDateString());
			DialogResult=DialogResult.OK;
		}

		///<summary>This button is only visible when proc IsLocked.</summary>
		private void butAppend_Click(object sender,EventArgs e) {
			using FormProcNoteAppend formPNA=new FormProcNoteAppend();
			formPNA.ProcCur=GroupCur;
			formPNA.ShowDialog();
			if(formPNA.DialogResult!=DialogResult.OK) {
				return;
			}
			DialogResult=DialogResult.OK;//exit out of this window.  Change already saved, and OK button is disabled in this window, anyway.
		}

		private void gridPat_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			string tag=gridPat.ListGridRows[e.Row].Tag.ToString();
			tag=tag.Substring(8);//strips off all but the number: PatField1
			int index=PIn.Int(tag);
			PatField field=PatFields.GetByName(_listPatFieldDefs[index].FieldName,PatFieldList);
			if(field==null) {
				field=new PatField();
				field.PatNum=PatCur.PatNum;
				field.FieldName=_listPatFieldDefs[index].FieldName;
				field.FieldValue=string.Empty;
				if(_listPatFieldDefs[index].FieldType==PatFieldType.Text) {
					using FormPatFieldEdit FormPF=new FormPatFieldEdit(field);
					FormPF.IsNew=true;
					FormPF.ShowDialog();
				}
				if(_listPatFieldDefs[index].FieldType==PatFieldType.PickList) {
					using FormPatFieldPickEdit FormPF=new FormPatFieldPickEdit(field);
					FormPF.IsNew=true;
					FormPF.ShowDialog();
				}
				if(_listPatFieldDefs[index].FieldType==PatFieldType.Date) {
					using FormPatFieldDateEdit FormPF=new FormPatFieldDateEdit(field);
					FormPF.IsNew=true;
					FormPF.ShowDialog();
				}
				if(_listPatFieldDefs[index].FieldType==PatFieldType.Checkbox) {
					using FormPatFieldCheckEdit FormPF=new FormPatFieldCheckEdit(field);
					FormPF.IsNew=true;
					FormPF.ShowDialog();
				}
			}
			else {
				if(_listPatFieldDefs[index].FieldType==PatFieldType.Text) {
					using FormPatFieldEdit FormPF=new FormPatFieldEdit(field);
					FormPF.ShowDialog();
				}
				if(_listPatFieldDefs[index].FieldType==PatFieldType.PickList) {
					using FormPatFieldPickEdit FormPF=new FormPatFieldPickEdit(field);
					FormPF.ShowDialog();
				}
				if(_listPatFieldDefs[index].FieldType==PatFieldType.Date) {
					using FormPatFieldDateEdit FormPF=new FormPatFieldDateEdit(field);
					FormPF.ShowDialog();
				}
				if(_listPatFieldDefs[index].FieldType==PatFieldType.Checkbox) {
					using FormPatFieldCheckEdit FormPF=new FormPatFieldCheckEdit(field);
					FormPF.ShowDialog();
				}
			}
			FillPatientData();
		}

		private bool EntriesAreValid() {
			if(!textProcDate.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return false;
			}
			if(!signatureBoxWrapper.IsValid) {
				MsgBox.Show(this,"Your signature is invalid. Please sign and click OK again.");
				return false;
			}
			return true;
		}

		private void SaveAndClose() {
			GroupCur.Note=textNotes.Text;
			if(Programs.UsingOrion) {
				GroupCur.ProcDate=PIn.Date(this.textProcDate.Text);
			}
			try {
				SaveSignature();
			}
			catch(Exception ex){
				MessageBox.Show(Lan.g(this,"Error saving signature.")+"\r\n"+ex.Message);
			}
			Procedures.Update(GroupCur,GroupOld);
			if(Programs.UsingOrion) {
				for(int i = 0;i<ProcList.Count;i++) {
					ProcList[i].ProcDate=GroupCur.ProcDate;
					Procedures.Update(ProcList[i],ProcListOld[i]);
				}
				for(int i=0;i<OrionProcList.Count;i++){
					OrionProcs.Update(OrionProcList[i]);
				}
			}
			DialogResult=DialogResult.OK;
			IsOpen=false;
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.GroupNoteEditSigned)) {
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Delete this group note?")){
				return;
			}
			try { 
				Procedures.Delete(GroupCur.ProcNum);
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message+"\r\n"+Lan.g(this,"Please call support."));//GroupNotes should never fail deletion.
				return;
			}
			for(int i=0;i<GroupItemList.Count;i++){
				ProcGroupItems.Delete(GroupItemList[i].ProcGroupItemNum);
			}
			//This log entry is similar to the log entry made when right-clicking in the Chart and using the delete option,
			//except there is an extra : in the description for this log entry, so we programmers can know for sure where the entry was made from.
			if(_attachedToCompletedProc) {
				SecurityLogs.MakeLogEntry(Permissions.ProcCompleteStatusEdit,PatCur.PatNum,
					":"+ProcedureCodes.GetStringProcCode(GroupCur.CodeNum).ToString()+" ("+GroupCur.ProcStatus+"), "+GroupCur.ProcDate.ToShortDateString());
			}
			else {
				SecurityLogs.MakeLogEntry(Permissions.ProcDelete,PatCur.PatNum,
					":"+ProcedureCodes.GetStringProcCode(GroupCur.CodeNum).ToString()+" ("+GroupCur.ProcStatus+"), "+GroupCur.ProcDate.ToShortDateString());
			}
			DialogResult=DialogResult.OK;
			IsOpen=false;
		}		

		private void butOK_Click(object sender,System.EventArgs e) {
			if(!EntriesAreValid()){
				return;
			}
			if(_hasUserChanged && signatureBoxWrapper.SigIsBlank 
				&& !MsgBox.Show(this,MsgBoxButtons.OKCancel,
					"The signature box has not been re-signed.  Continuing will remove the previous signature from this procedure.  Exit anyway?")) 
			{
				return;
			}
			SaveAndClose();
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
			IsOpen=false;
		}

		private void FormProcGroup_FormClosing(object sender,FormClosingEventArgs e) {
			signatureBoxWrapper?.SetTabletState(0);
			if(DialogResult==DialogResult.OK) {
				return;
			}
			if(GroupOld.Note.Replace("\r","").Trim()!=textNotes.Text.Replace("\r","").Trim()) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Note has been changed.  Unsaved changes will be lost.  Continue?")) {
					e.Cancel=true;//Prevent the form from closing.
					IsOpen=true;
					return;
				}
			}
			if(GroupCur.IsNew) {
				Procedures.Delete(GroupCur.ProcNum);
				for(int i=0;i<GroupItemList.Count;i++) {
					ProcGroupItems.Delete(GroupItemList[i].ProcGroupItemNum);
				}
			}
			DialogResult=DialogResult.Cancel;
			IsOpen=false;
		}
	}
}
