/*=============================================================================================================
Open Dental GPL license Copyright (C) 2003  Jordan Sparks, DMD.  http://www.open-dent.com,  www.docsparks.com
See header in FormOpenDental.cs for complete text.  Redistributions must retain this text.
===============================================================================================================*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
///<summary></summary>
	public partial class FormConfirmList : FormODBase {
		///<summary>Will be set to true when form closes if user click Send to Pinboard.</summary>
		public bool PinClicked=false;
		private int pagesPrinted;
		private DataTable AddrTable;
		private int patientsPrinted;
		///<summary>This list of appointments displayed</summary>
		private DataTable _table;
		private bool headingPrinted;
		private int headingPrintH;
		private List<EmailAddress> _listEmailAddresses;
		private List<Provider> _listProviders;
		private List<Def> _listApptConfirmedDefs;

		///<summary></summary>
		public FormConfirmList(){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormConfirmList_Load(object sender, System.EventArgs e) {
			Cursor=Cursors.WaitCursor;
			comboShowRecall.SelectedIndex=0;//Default to show all.
			textDateFrom.Text=AddWorkDays(1,DateTime.Today).ToShortDateString();
			textDateTo.Text=AddWorkDays(2,DateTime.Today).ToShortDateString();
			comboProv.Items.Add(Lan.g(this,"All"));
			comboProv.SelectedIndex=0;
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++) {
				comboProv.Items.Add(_listProviders[i].GetLongDesc());
			}
			if(PrefC.GetBool(PrefName.EnterpriseApptList)){
				comboClinic.IncludeAll=false;
			}
			//textPostcardMessage.Text=PrefC.GetString(PrefName.ConfirmPostcardMessage");
			comboStatus.Items.Clear();
			comboViewStatus.Items.Clear();
			comboViewStatus.Items.Add(Lan.g(this,"None"),new Def());
			comboViewStatus.SelectedIndex=0;
			_listApptConfirmedDefs=Defs.GetDefsForCategory(DefCat.ApptConfirmed,true);
			for(int i=0;i<_listApptConfirmedDefs.Count;i++){
				comboStatus.Items.Add(_listApptConfirmedDefs[i].ItemName);
				comboViewStatus.Items.Add(_listApptConfirmedDefs[i].ItemName,_listApptConfirmedDefs[i]);
			}
			if(!Security.IsAuthorized(Permissions.ApptConfirmStatusEdit,true)) {//Suppress message because it would be very annoying to users.
				comboStatus.Enabled=false;
			}
			if(!Programs.IsEnabled(ProgramName.CallFire) && !SmsPhones.IsIntegratedTextingEnabled()) {
				butText.Enabled=false;
			}
			FillComboEmail();
			checkGroupFamilies.Checked=PrefC.GetBool(PrefName.ConfirmGroupByFamily);
			Cursor=Cursors.Default;
			Plugins.HookAddCode(this,"FormConfirmList.Load_End",butText);
		}

		///<summary>There is a bug in ODProgress.cs that forces windows that use a progress bar on load to go behind other applications. 
		///This is a temporary workaround until we decide how to address the issue.</summary>
		private void FormConfirmList_Shown(object sender,EventArgs e) {
			FillMain();
		}

		private void FillComboEmail() {
			_listEmailAddresses=EmailAddresses.GetDeepCopy();//Does not include user specific email addresses.
			List<Clinic> listClinicsAll=Clinics.GetDeepCopy();
			for(int i=0;i<listClinicsAll.Count;i++) {//Exclude any email addresses that are associated to a clinic.
				_listEmailAddresses.RemoveAll(x => x.EmailAddressNum==listClinicsAll[i].EmailAddressNum);
			}
			//Exclude default practice email address.
			_listEmailAddresses.RemoveAll(x => x.EmailAddressNum==PrefC.GetLong(PrefName.EmailDefaultAddressNum));
			//Exclude web mail notification email address.
			_listEmailAddresses.RemoveAll(x => x.EmailAddressNum==PrefC.GetLong(PrefName.EmailNotifyAddressNum));
			comboEmailFrom.Items.Add(Lan.g(this,"Practice/Clinic"));//default
			comboEmailFrom.SelectedIndex=0;
			//Add all email addresses which are not associated to a user, a clinic, or either of the default email addresses.
			for(int i=0;i<_listEmailAddresses.Count;i++) {
				comboEmailFrom.Items.Add(_listEmailAddresses[i].EmailUsername);
			}
			//Add user specific email address if present.
			EmailAddress emailAddressMe=EmailAddresses.GetForUser(Security.CurUser.UserNum);//can be null
			if(emailAddressMe!=null) {
				_listEmailAddresses.Insert(0,emailAddressMe);
				comboEmailFrom.Items.Insert(1,Lan.g(this,"Me")+" <"+emailAddressMe.EmailUsername+">");//Just below Practice/Clinic
			}
		}

		private void menuRight_click(object sender,System.EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an appointment first.");
				return;
			}
			switch(menuRightClick.Items.IndexOf((ToolStripMenuItem)sender)) {
				case 0:
					SelectPatient_Click();
					break;
				case 1:
					SeeChart_Click();
					break;
				case 2:
					SendPinboard_Click();
					break;
			}
		}

		private void SelectPatient_Click() {
			//If multiple selected, just take the last one to remain consistent with SendPinboard_Click.
			long patNum=PIn.Long(_table.Rows[gridMain.SelectedIndices[gridMain.SelectedIndices.Length-1]]["PatNum"].ToString());
			Patient pat=Patients.GetPat(patNum);
			FormOpenDental.S_Contr_PatientSelected(pat,true);
		}

		private void gridMain_MouseUp(object sender,MouseEventArgs e) {
			if(e.Button==MouseButtons.Right && gridMain.SelectedIndices.Length>0) {
				//To maintain legacy behavior we will use the last selected index if multiple are selected.
				Patient pat=Patients.GetLim(PIn.Long(_table.Rows[gridMain.SelectedIndices[gridMain.SelectedIndices.Length-1]]["PatNum"].ToString()));
				toolStripMenuItemSelectPatient.Text=Lan.g(gridMain.TranslationName,"Select Patient")+" ("+pat.GetNameFL()+")";
			}
		}

		///<summary>If multiple patients are selected in UnchedList, will select the last patient to remain consistent with sending to pinboard behavior.</summary>
		private void SeeChart_Click() {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an appointment first.");
				return;
			}
			//If multiple selected, just take the last one to remain consistent with SendPinboard_Click.
			long patNum=PIn.Long(_table.Rows[gridMain.SelectedIndices[gridMain.SelectedIndices.Length-1]]["PatNum"].ToString());
			Patient pat=Patients.GetPat(patNum);
			FormOpenDental.S_Contr_PatientSelected(pat,false);
			GotoModule.GotoChart(pat.PatNum);
		}

		private void SendPinboard_Click() {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an appointment first.");
				return;
			}
			List<long> listAptSelected=new List<long>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				listAptSelected.Add(PIn.Long(_table.Rows[gridMain.SelectedIndices[i]]["AptNum"].ToString()));
			}
			//This will send all appointments in listAptSelected to the pinboard, and will select the patient attached to the last appointment.
			GotoModule.PinToAppt(listAptSelected,0);
		}

		///<summary>Adds the specified number of work days, skipping saturday and sunday.</summary>
		private DateTime AddWorkDays(int days,DateTime date){
			DateTime retVal=date;
			for(int i=0;i<days;i++){
				retVal=retVal.AddDays(1);
				//then, this part jumps to monday if on a sat or sun
				while(retVal.DayOfWeek==DayOfWeek.Saturday || retVal.DayOfWeek==DayOfWeek.Sunday){
					retVal=retVal.AddDays(1);
				}
			}
			return retVal;
		}

		private void FillMain(){
			DateTime dateFrom=PIn.Date(textDateFrom.Text);
			DateTime dateTo=PIn.Date(textDateTo.Text);
			long provNum=0;
			if(comboProv.SelectedIndex!=0) {
				provNum=_listProviders[comboProv.SelectedIndex-1].ProvNum;
			}
			bool showRecalls=false;
			bool showNonRecalls=false;
			bool showHygienePrescheduled=false;
			if(comboShowRecall.SelectedIndex==0 || comboShowRecall.SelectedIndex==1) {//All or Recall Only
				showRecalls=true;
			}
			if(comboShowRecall.SelectedIndex==0 || comboShowRecall.SelectedIndex==2) {//All or Exclude Recalls
				showNonRecalls=true;
			}
			if(comboShowRecall.SelectedIndex==0 || comboShowRecall.SelectedIndex==3) {//All or Hygiene Prescheduled
				showHygienePrescheduled=true;
			}
			Def apptConfirmedType=comboViewStatus.GetSelected<Def>();
			long clinicNum=PrefC.HasClinicsEnabled ? comboClinic.SelectedClinicNum : -1;
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => _table=Appointments.GetConfirmList(dateFrom,dateTo,provNum,clinicNum,showRecalls,showNonRecalls,
				showHygienePrescheduled,apptConfirmedType.DefNum,checkGroupFamilies.Checked);
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
			int scrollVal=gridMain.ScrollValue;
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableConfirmList","Date Time"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableConfirmList","DateSched"),80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableConfirmList","Patient"),80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableConfirmList","Age"),30);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableConfirmList","Contact"),150);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableConfirmList","Addr/Ph Note"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableConfirmList","Status"),80);//confirmed
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableConfirmList","Procs"),110);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableConfirmList","Medical"),80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableConfirmList","Appt Note"),124);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			GridCell cell;
			foreach(DataRow dataRow in _table.Rows) {
				Patient pat=new Patient {
					FName=PIn.String(dataRow["FName"].ToString()),
					Preferred=PIn.String(dataRow["Preferred"].ToString()),
					LName=PIn.String(dataRow["LName"].ToString()),
				};
				row=new GridRow();
				row.Cells.Add(PIn.DateT(dataRow["AptDateTime"].ToString()).ToString());
				row.Cells.Add(dataRow["dateSched"].ToString());
				row.Cells.Add(pat.GetNameLF());
				row.Cells.Add(dataRow["age"].ToString());
				row.Cells.Add(dataRow["contactMethod"].ToString());
				row.Cells.Add(dataRow["AddrNote"].ToString());
				row.Cells.Add(dataRow["confirmed"].ToString());
				row.Cells.Add(dataRow["ProcDescript"].ToString());
				cell=new GridCell(dataRow["medNotes"].ToString());
				cell.ColorText=Color.Red;
				row.Cells.Add(cell);
				row.Cells.Add(dataRow["Note"].ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.ScrollValue=scrollVal;
		}

		private void grid_CellClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			//row selected before this event triggered
			SetFamilyColors();
			comboStatus.SelectedIndex=-1;
		}

		private void SetFamilyColors(){
			if(gridMain.SelectedIndices.Length!=1){
				for(int i=0;i<gridMain.ListGridRows.Count;i++){
					gridMain.ListGridRows[i].ColorText=Color.Black;
				}
				gridMain.Invalidate();
				return;
			}
			long guar=PIn.Long(_table.Rows[gridMain.SelectedIndices[0]]["Guarantor"].ToString());
			int famCount=0;
			for(int i=0;i<gridMain.ListGridRows.Count;i++){
				if(PIn.Long(_table.Rows[i]["Guarantor"].ToString())==guar){
					famCount++;
					gridMain.ListGridRows[i].ColorText=Color.Red;
				}
				else{
					gridMain.ListGridRows[i].ColorText=Color.Black;
				}
			}
			if(famCount==1){//only the highlighted patient is red at this point
				gridMain.ListGridRows[gridMain.SelectedIndices[0]].ColorText=Color.Black;
			}
			gridMain.Invalidate();
		}

		private void grid_Click(object sender,EventArgs e) {
			
		}

		private void grid_CellDoubleClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			Cursor=Cursors.WaitCursor;
			long selectedApt=PIn.Long(_table.Rows[e.Row]["AptNum"].ToString());
			Patient pat=Patients.GetPat(PIn.Long(_table.Rows[e.Row]["PatNum"].ToString()));
			FormOpenDental.S_Contr_PatientSelected(pat,true);
			using FormApptEdit FormA=new FormApptEdit(selectedApt);
			FormA.PinIsVisible=true;
			FormA.ShowDialog();
			if(FormA.PinClicked) {//set from inside form.
				SendPinboard_Click();//Whatever they double clicked on will still be selected, just fire the event.
				DialogResult=DialogResult.OK;
			}
			else {
				FillMain();
			}
			for(int i=0;i<_table.Rows.Count;i++){
				if(PIn.Long(_table.Rows[i]["AptNum"].ToString())==selectedApt){
					gridMain.SetSelected(i,true);
				}
			}
			SetFamilyColors();
			Cursor=Cursors.Default;
		}

		private void comboStatus_SelectionChangeCommitted(object sender, System.EventArgs e) {
			if(gridMain.SelectedIndices.Length==0){
				return;//because user could never initiate this action.
			}
			Appointment apt;
			Cursor=Cursors.WaitCursor;
			long[] selectedApts=new long[gridMain.SelectedIndices.Length];
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){
				selectedApts[i]=PIn.Long(_table.Rows[gridMain.SelectedIndices[i]]["AptNum"].ToString());
			}
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){
				apt=Appointments.GetOneApt(PIn.Long(_table.Rows[gridMain.SelectedIndices[i]]["AptNum"].ToString()));
				Appointment aptOld=apt.Copy();
				int selectedI=comboStatus.SelectedIndex;
				apt.Confirmed=_listApptConfirmedDefs[selectedI].DefNum;
				try{
					Appointments.Update(apt,aptOld);
				}
				catch(ApplicationException ex){
					Cursor=Cursors.Default;
					MessageBox.Show(ex.Message);
					return;
				}
				if(apt.Confirmed!=aptOld.Confirmed) {
					//Log confirmation status changes.
					SecurityLogs.MakeLogEntry(Permissions.ApptConfirmStatusEdit,apt.PatNum,Lans.g(this,"Appointment confirmation status changed from")+" "
						+Defs.GetName(DefCat.ApptConfirmed,aptOld.Confirmed)+" "+Lans.g(this,"to")+" "+Defs.GetName(DefCat.ApptConfirmed,apt.Confirmed)
						+" "+Lans.g(this,"from the confirmation list")+".",apt.AptNum,aptOld.DateTStamp);
				}
			}
			FillMain();
			//reselect all the apts
			for(int i=0;i<_table.Rows.Count;i++){
				for(int j=0;j<selectedApts.Length;j++){
					if(PIn.Long(_table.Rows[i]["AptNum"].ToString())==selectedApts[j]){
						gridMain.SetSelected(i,true);
					}
				}
			}
			SetFamilyColors();
			comboStatus.SelectedIndex=-1;
			Cursor=Cursors.Default;
		}

		private void comboStatus_SelectedIndexChanged(object sender, System.EventArgs e) {
			//?
		}

		private void butReport_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.UserQuery)) {
				return;
			}
		  if(_table.Rows.Count==0){
        MessageBox.Show(Lan.g(this,"There are no appointments in the list.  Must have at least one to run report."));    
        return;
      }
			long[] aptNums;
      if(gridMain.SelectedIndices.Length==0){
				aptNums=new long[_table.Rows.Count];
        for(int i=0;i<aptNums.Length;i++){
          aptNums[i]=PIn.Long(_table.Rows[i]["AptNum"].ToString());
        }
      }
      else{
				aptNums=new long[gridMain.SelectedIndices.Length];
        for(int i=0;i<aptNums.Length;i++){
          aptNums[i]=PIn.Long(_table.Rows[gridMain.SelectedIndices[i]]["AptNum"].ToString());
        }
      }
      using FormRpConfirm FormC=new FormRpConfirm(aptNums);
      FormC.ShowDialog(); 
		}

		private void butLabels_Click(object sender, System.EventArgs e) {
			if(_table.Rows.Count==0){
        MessageBox.Show(Lan.g(this,"There are no appointments in the list.  Must have at least one to print."));    
        return;
      }
			if(gridMain.SelectedIndices.Length==0){
				for(int i=0;i<_table.Rows.Count;i++){
					gridMain.SetSelected(i,true);
				}
			}
			List<long> aptNums=new List<long>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
        aptNums.Add(PIn.Long(_table.Rows[gridMain.SelectedIndices[i]]["AptNum"].ToString()));
      }
			AddrTable=Appointments.GetAddrTable(aptNums,checkGroupFamilies.Checked);
			pagesPrinted=0;
			patientsPrinted=0;
			PrinterL.TryPreview(pdLabels_PrintPage,
				Lan.g(this,"Confirmation list labels printed"),
				PrintSituation.LabelSheet,
				new Margins(0,0,0,0),
				PrintoutOrigin.AtMargin,
				totalPages:(int)Math.Ceiling((double)AddrTable.Rows.Count/30)
			);
		}

		///<summary>Changes made to printing confirmation postcards need to be made in FormRecallList.butPostcards_Click() as well.</summary>
		private void butPostcards_Click(object sender,System.EventArgs e) {
			if(_table.Rows.Count==0) {
				MessageBox.Show(Lan.g(this,"There are no appointments in the list.  Must have at least one to print."));
				return;
			}
			if(gridMain.SelectedIndices.Length==0) {
				ContactMethod cmeth;
				for(int i=0;i<_table.Rows.Count;i++) {
					cmeth=(ContactMethod)PIn.Long(_table.Rows[i]["PreferConfirmMethod"].ToString());
					if(cmeth!=ContactMethod.Mail && cmeth!=ContactMethod.None) {
						continue;
					}
					gridMain.SetSelected(i,true);
				}
			}
			List<long> aptNums=new List<long>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				aptNums.Add(PIn.Long(_table.Rows[gridMain.SelectedIndices[i]]["AptNum"].ToString()));
			}
			if(aptNums.Count==0) {
				MsgBox.Show(this,"No postcards necessary because contact method is not set to Mail for anyone in the list.");
				return;
			}
			AddrTable=Appointments.GetAddrTable(aptNums, checkGroupFamilies.Checked);
			pagesPrinted=0;
			patientsPrinted=0;
			PaperSize paperSize;
			PrintoutOrientation orient=PrintoutOrientation.Default;
			if(PrefC.GetLong(PrefName.RecallPostcardsPerSheet)==1) {
				paperSize=new PaperSize("Postcard",500,700);
				orient=PrintoutOrientation.Landscape;
			}
			else if(PrefC.GetLong(PrefName.RecallPostcardsPerSheet)==3) {
				paperSize=new PaperSize("Postcard",850,1100);
			}
			else {//4
				paperSize=new PaperSize("Postcard",850,1100);
				orient=PrintoutOrientation.Landscape;
			}
			int totalPages=((int)Math.Ceiling((double)AddrTable.Rows.Count/(double)PrefC.GetLong(PrefName.RecallPostcardsPerSheet)));
			bool isDialogOk=PrinterL.TryPreview(pdCards_PrintPage,
				Lan.g(this,"Confirmation list postcards printed"),
				PrintSituation.Postcard,
				new Margins(0,0,0,0),
				PrintoutOrigin.AtMargin,
				paperSize,
				orient,
				totalPages
			);
			if(isDialogOk) { //dialog result was OK means that the postcards were sent to the printer.
				for(int i=0;i<AddrTable.Rows.Count;i++) { //loop through the address table and create commlog entries for all selected.
					Commlog postcardCommlog=new Commlog();
						postcardCommlog.CommDateTime=DateTime.Now;
						postcardCommlog.Mode_=CommItemMode.Mail;
						postcardCommlog.Note="Confirmation postcard printed for "+AddrTable.Rows[i]["LName"].ToString()
								+", "+AddrTable.Rows[i]["FName"].ToString()+"\r\n"+AddrTable.Rows[i]["Address"].ToString()+"\r\n";
						if(AddrTable.Rows[i]["Address2"].ToString()!="") {
							postcardCommlog.Note+=AddrTable.Rows[i]["Address2"].ToString()+"\r\n";
						}
						postcardCommlog.Note+=AddrTable.Rows[i]["City"].ToString()+", "
						+AddrTable.Rows[i]["State"].ToString()+"   "
						+AddrTable.Rows[i]["Zip"].ToString()+"\r\n";
						postcardCommlog.PatNum=PIn.Long(AddrTable.Rows[i]["PatNum"].ToString());
						postcardCommlog.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.MISC);
						postcardCommlog.SentOrReceived=CommSentOrReceived.Sent;
						postcardCommlog.UserNum=Security.CurUser.UserNum;
					Commlogs.Insert(postcardCommlog);
				}
			}
		}

		///<summary>raised for each page to be printed.</summary>
		private void pdLabels_PrintPage(object sender, PrintPageEventArgs ev){
			int totalPages=(int)Math.Ceiling((double)AddrTable.Rows.Count/30);
			Graphics g=ev.Graphics;
			float yPos=75;
			float xPos=50;
			string text="";
			while(yPos<1000 && patientsPrinted<AddrTable.Rows.Count){
				if(checkGroupFamilies.Checked && AddrTable.Rows[patientsPrinted]["famList"].ToString()!="") {
					text=AddrTable.Rows[patientsPrinted]["guarLName"].ToString()+" "+Lan.g(this,"Household")+"\r\n";
				}
				else {
					text=AddrTable.Rows[patientsPrinted]["FName"].ToString()+" "
						+AddrTable.Rows[patientsPrinted]["MiddleI"].ToString()+" "
						+AddrTable.Rows[patientsPrinted]["LName"].ToString()+"\r\n";
				}
				text+=AddrTable.Rows[patientsPrinted]["Address"].ToString()+"\r\n";
				if(AddrTable.Rows[patientsPrinted]["Address2"].ToString()!=""){
					text+=AddrTable.Rows[patientsPrinted]["Address2"].ToString()+"\r\n";
				}
				text+=AddrTable.Rows[patientsPrinted]["City"].ToString()+", "
					+AddrTable.Rows[patientsPrinted]["State"].ToString()+"   "
					+AddrTable.Rows[patientsPrinted]["Zip"].ToString()+"\r\n";
				Rectangle rect=new Rectangle((int)xPos,(int)yPos,275,100);
				MapAreaRoomControl.FitText(text,new Font(FontFamily.GenericSansSerif,11),Brushes.Black,rect,new StringFormat(),g);
				//reposition for next label
				xPos+=275;
				if(xPos>850){//drop a line
					xPos=50;
					yPos+=100;
				}
				patientsPrinted++;
			}
			pagesPrinted++;
			if(pagesPrinted>=totalPages){
				ev.HasMorePages=false;
				pagesPrinted=0;//because it has to print again from the print preview
				patientsPrinted=0;
			}
			else{
				ev.HasMorePages=true;
			}
			g.Dispose();
		}

		///<summary>raised for each page to be printed.</summary>
		private void pdCards_PrintPage(object sender, PrintPageEventArgs ev){
			int totalPages=(int)Math.Ceiling((double)AddrTable.Rows.Count/(double)PrefC.GetLong(PrefName.RecallPostcardsPerSheet));
			Graphics g=ev.Graphics;
			int yAdj=(int)(PrefC.GetDouble(PrefName.RecallAdjustDown)*100);
			int xAdj=(int)(PrefC.GetDouble(PrefName.RecallAdjustRight)*100);
			float yPos=0+yAdj;//these refer to the upper left origin of each postcard
			float xPos=0+xAdj;
			const int bottomPageMargin=100;
			string str;
			while(yPos<ev.PageBounds.Height-bottomPageMargin && patientsPrinted<AddrTable.Rows.Count){
				//Return Address--------------------------------------------------------------------------
				if(PrefC.GetBool(PrefName.RecallCardsShowReturnAdd)){
					if(!PrefC.HasClinicsEnabled || PIn.Long(AddrTable.Rows[patientsPrinted]["ClinicNum"].ToString())==0) {//No clinics or no clinic selected for this appt
						str=PrefC.GetString(PrefName.PracticeTitle)+"\r\n";
						g.DrawString(str,new Font(FontFamily.GenericSansSerif,9,FontStyle.Bold),Brushes.Black,xPos+45,yPos+60);
						str=PrefC.GetString(PrefName.PracticeAddress)+"\r\n";
						if(PrefC.GetString(PrefName.PracticeAddress2)!="") {
							str+=PrefC.GetString(PrefName.PracticeAddress2)+"\r\n";
						}
						str+=PrefC.GetString(PrefName.PracticeCity)+",  "+PrefC.GetString(PrefName.PracticeST)+"  "+PrefC.GetString(PrefName.PracticeZip)+"\r\n";
						string phone=PrefC.GetString(PrefName.PracticePhone);
						if(CultureInfo.CurrentCulture.Name=="en-US"&& phone.Length==10) {
							str+="("+phone.Substring(0,3)+")"+phone.Substring(3,3)+"-"+phone.Substring(6);
						}
						else {//any other phone format
							str+=phone;
						}
					}
					else {//Clinics enabled and clinic selected
						Clinic clinic=Clinics.GetClinic(PIn.Long(AddrTable.Rows[patientsPrinted]["ClinicNum"].ToString()));
						str=clinic.Description+"\r\n";
						g.DrawString(str,new Font(FontFamily.GenericSansSerif,9,FontStyle.Bold),Brushes.Black,xPos+45,yPos+60);
						str=clinic.Address+"\r\n";
						if(clinic.Address2!="") {
							str+=clinic.Address2+"\r\n";
						}
						str+=clinic.City+",  "+clinic.State+"  "+clinic.Zip+"\r\n";
						string phone=clinic.Phone;
						if(CultureInfo.CurrentCulture.Name=="en-US"&& phone.Length==10) {
							str+="("+phone.Substring(0,3)+")"+phone.Substring(3,3)+"-"+phone.Substring(6);
						}
						else {//any other phone format
							str+=phone;
						}
					}
					g.DrawString(str,new Font(FontFamily.GenericSansSerif,8),Brushes.Black,xPos+45,yPos+75);
				}
				//Body text-------------------------------------------------------------------------------
				string famList=AddrTable.Rows[patientsPrinted]["famList"].ToString();
				if(checkGroupFamilies.Checked	&& famList!=""){
					str=PrefC.GetString(PrefName.ConfirmPostcardFamMessage);
					str=str.Replace("[FamilyApptList]",famList);
				}
				//Body text, single card-------------------------------------------------------------------
				else{
					DateTime dateTimeAskedToArrive=PIn.Date(AddrTable.Rows[patientsPrinted]["DateTimeAskedToArrive"].ToString());
					DateTime aptDateTime=PIn.Date(AddrTable.Rows[patientsPrinted]["AptDateTime"].ToString());
					Patient pat=new Patient {
						FName=PIn.String(AddrTable.Rows[patientsPrinted]["FName"].ToString()),
						Preferred=PIn.String(AddrTable.Rows[patientsPrinted]["Preferred"].ToString()),
					};
					str=PatComm.BuildConfirmMessage(ContactMethod.Mail,pat,dateTimeAskedToArrive,aptDateTime);
				}
				g.DrawString(str,new Font(FontFamily.GenericSansSerif,10),Brushes.Black,new RectangleF(xPos+45,yPos+180,250,190));
				//Patient's Address-----------------------------------------------------------------------
				if(checkGroupFamilies.Checked	&& AddrTable.Rows[patientsPrinted]["famList"].ToString()!="")//print family card
				{
					str=AddrTable.Rows[patientsPrinted]["guarLName"].ToString()+" "+Lan.g(this,"Household")+"\r\n";
				}
				else{//print single card
					str=AddrTable.Rows[patientsPrinted]["FName"].ToString()+" "
						+AddrTable.Rows[patientsPrinted]["MiddleI"].ToString()+" "
						+AddrTable.Rows[patientsPrinted]["LName"].ToString()+"\r\n";
				}
				str+=AddrTable.Rows[patientsPrinted]["Address"].ToString()+"\r\n";
					if(AddrTable.Rows[patientsPrinted]["Address2"].ToString()!=""){
						str+=AddrTable.Rows[patientsPrinted]["Address2"].ToString()+"\r\n";
					}
					str+=AddrTable.Rows[patientsPrinted]["City"].ToString()+", "
						+AddrTable.Rows[patientsPrinted]["State"].ToString()+"   "
						+AddrTable.Rows[patientsPrinted]["Zip"].ToString()+"\r\n";
				g.DrawString(str,new Font(FontFamily.GenericSansSerif,11),Brushes.Black,xPos+320,yPos+240);
				if(PrefC.GetLong(PrefName.RecallPostcardsPerSheet)==1){
					//Setting it to this value will cause it to break out of the while loop.
					yPos=ev.PageBounds.Height-bottomPageMargin;
				}
				else if(PrefC.GetLong(PrefName.RecallPostcardsPerSheet)==3){
					yPos+=366;
				}
				else{//4
					xPos+=550;
					if(xPos>1000){
						xPos=0+xAdj;
						yPos+=425;
					}
				}
				patientsPrinted++;
			}//while
			pagesPrinted++;
			if(pagesPrinted==totalPages){
				ev.HasMorePages=false;
				pagesPrinted=0;
				patientsPrinted=0;
			}
			else{
				ev.HasMorePages=true;
			}
		}

		private void butRefresh_Click(object sender, System.EventArgs e) {
			FillMain();
		}

		private void butSetStatus_Click(object sender, System.EventArgs e) {
			/*if(comboStatus.SelectedIndex==-1){
				return;
			}
			int[] originalRecalls=new int[tbMain.SelectedIndices.Length];
			for(int i=0;i<tbMain.SelectedIndices.Length;i++){
				originalRecalls[i]=((RecallItem)MainAL[tbMain.SelectedIndices[i]]).RecallNum;
				Recalls.UpdateStatus(
					((RecallItem)MainAL[tbMain.SelectedIndices[i]]).RecallNum,
					Defs.Short[(int)DefCat.RecallUnschedStatus][comboStatus.SelectedIndex].DefNum);
				//((RecallItem)MainAL[tbMain.SelectedIndices[i]]).up
			}
			FillMain();
			for(int i=0;i<tbMain.MaxRows;i++){
				for(int j=0;j<originalRecalls.Length;j++){
					if(originalRecalls[j]==((RecallItem)MainAL[i]).RecallNum){
						tbMain.SetSelected(i,true);
					}
				}
			}*/
		}

		private void butEmail_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.EmailSend)){
				return;
			}
			if(gridMain.ListGridRows.Count==0) {
				MsgBox.Show(this,"There are no Patients in the table.  Must have at least one.");
				return;
			}
			if(!EmailAddresses.ExistsValidEmail()) {
				MsgBox.Show(this,"You need to enter an SMTP server name in e-mail setup before you can send e-mail.");
				return;
			}
			if(PrefC.GetLong(PrefName.ConfirmStatusEmailed)==0) {
				MsgBox.Show(this,"No 'Status for e-mailed confirmation' set in the Setup Confirmation window.");
				return;
			}
			if(gridMain.SelectedIndices.Length==0) {
				ContactMethod cmeth;
				for(int i=0;i<_table.Rows.Count;i++) {
					cmeth=(ContactMethod)PIn.Int(_table.Rows[i][checkGroupFamilies.Checked?"guarPreferConfirmMethod":"PreferConfirmMethod"].ToString());
					if(cmeth!=ContactMethod.Email) {
						continue;
					}
					if(_table.Rows[i]["confirmed"].ToString()==Defs.GetName(DefCat.ApptConfirmed,PrefC.GetLong(PrefName.ConfirmStatusEmailed))) {//Already confirmed by email
						continue;
					}
					if(_table.Rows[i][checkGroupFamilies.Checked?"guarEmail":"email"].ToString()=="") {
						continue;
					}
					gridMain.SetSelected(i,true);
				}
				if(gridMain.SelectedIndices.Length==0) {
					MsgBox.Show(this,"Confirmations have been sent to all patients of email type who also have an email address entered.");
					return;
				}
			}
			else {//deselect the ones that do not have email addresses specified
				int skipped=0;
				for(int i=gridMain.SelectedIndices.Length-1;i>=0;i--) {
					if(_table.Rows[gridMain.SelectedIndices[i]][checkGroupFamilies.Checked?"guarEmail":"email"].ToString()=="") {
						skipped++;
						gridMain.SetSelected(gridMain.SelectedIndices[i],false);
					}
				}
				if(gridMain.SelectedIndices.Length==0) {
					MsgBox.Show(this,"None of the selected patients had email addresses entered.");
					return;
				}
				if(skipped>0) {
					MessageBox.Show(Lan.g(this,"Selected patients skipped due to missing email addresses: ")+skipped.ToString());
				}
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Send email to all of the selected patients?")) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			EmailMessage message;
			string str="";
			List<long> listPatNumsSelected=new List<long>();
			List<long> listPatNumsFailed=new List<long>();
			EmailAddress emailAddress;
			string errors="";
			string familyApptList="";
			List<long> listApptNums=new List<long>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){				
				Patient pat=new Patient {
					FName=PIn.String(_table.Rows[gridMain.SelectedIndices[i]]["FName"].ToString()),
					Preferred=PIn.String(_table.Rows[gridMain.SelectedIndices[i]]["Preferred"].ToString()),
				};
				DateTime dateTimeAskedToArrive=PIn.DateT(_table.Rows[gridMain.SelectedIndices[i]]["DateTimeAskedToArrive"].ToString());
				DateTime apptDateTime=PIn.DateT(_table.Rows[gridMain.SelectedIndices[i]]["AptDateTime"].ToString());
				if(checkGroupFamilies.Checked) {//build the list of appointments a family has scheduled
					if(i==gridMain.SelectedIndices.Length-1 || _table.Rows[gridMain.SelectedIndices[i]]["Guarantor"].ToString()!=_table.Rows[gridMain.SelectedIndices[i+1]]["Guarantor"].ToString()) {
						if(familyApptList!=""){
							familyApptList+="\r\n"+PatComm.BuildAppointmentMessage(pat,dateTimeAskedToArrive,apptDateTime);
							//continue with sending an email, because there are no more family members
						}
					}
					else if(_table.Rows[gridMain.SelectedIndices[i]]["Guarantor"].ToString()==_table.Rows[gridMain.SelectedIndices[i+1]]["Guarantor"].ToString()) {
						listApptNums.Add(PIn.Long(_table.Rows[gridMain.SelectedIndices[i]]["AptNum"].ToString()));
						familyApptList+=(familyApptList!=""?"\r\n":"")+PatComm.BuildAppointmentMessage(pat,dateTimeAskedToArrive,apptDateTime);
						continue;//skip sending emails to anyone that isn't the guarantor or isn't a single patient
					}
				}
				listApptNums.Add(PIn.Long(_table.Rows[gridMain.SelectedIndices[i]]["AptNum"].ToString()));
				message=new EmailMessage();
				long clinicNum=Clinics.ClinicNum;
				message.PatNum=PIn.Long(_table.Rows[gridMain.SelectedIndices[i]][checkGroupFamilies.Checked?"Guarantor":"PatNum"].ToString());
				message.ToAddress=_table.Rows[gridMain.SelectedIndices[i]][checkGroupFamilies.Checked?"guarEmail":"email"].ToString();//Could be guarantor email.
				if(comboEmailFrom.SelectedIndex==0) { //clinic/practice default
					clinicNum=PIn.Long(_table.Rows[gridMain.SelectedIndices[i]][checkGroupFamilies.Checked?"guarClinicNum":"ClinicNum"].ToString());
					emailAddress=EmailAddresses.GetByClinic(clinicNum);
				}
				else { //me or static email address, email address for 'me' is the first one in _listEmailAddresses
					emailAddress=_listEmailAddresses[comboEmailFrom.SelectedIndex-1];//-1 to account for predefined "Clinic/Practice" item in combobox
				}
				message.FromAddress=emailAddress.GetFrom();				
				message.Subject=PrefC.GetString(PrefName.ConfirmEmailSubject);
				listPatNumsSelected.Add(message.PatNum);
				if(checkGroupFamilies.Checked && familyApptList!="") {
					str=PrefC.GetString(PrefName.ConfirmPostcardFamMessage);
					str=str.Replace("[FamilyApptList]",familyApptList);
					familyApptList="";
				}
				else {
					str=PatComm.BuildConfirmMessage(ContactMethod.Email,pat,dateTimeAskedToArrive,apptDateTime);
				}
				message.BodyText=EmailMessages.FindAndReplacePostalAddressTag(str,clinicNum);
				message.MsgDateTime=DateTime.Now;
				message.SentOrReceived=EmailSentOrReceived.Sent;
				message.MsgType=EmailMessageSource.Confirmation;
				try {
					EmailMessages.SendEmail(message,emailAddress);
				}
				catch (Exception ex){
					listPatNumsFailed.Add(message.PatNum);
					if(!errors.Contains("Message send fail for Patnum:"+message.PatNum+":  "+ex.Message)) {//unique messages only.
						errors+=("Message send fail for Patnum:"+message.PatNum+":  "+ex.Message+"\r\n");
					}
					continue;
				}
				for(int j=0; j<listApptNums.Count; j++) {
					Appointment appt=Appointments.GetOneApt(listApptNums[j]);
					Appointments.SetConfirmed(appt,PrefC.GetLong(PrefName.ConfirmStatusEmailed));
				}
				listApptNums.Clear();
			}
			Cursor=Cursors.Default;
			if(listPatNumsFailed.Count==gridMain.SelectedIndices.Length){ //all failed
				//no need to refresh
				if(DialogResult.Yes != MessageBox.Show(Lan.g(this,"All emails failed. Possibly due to invalid email addresses, a loss of connectivity, or a firewall blocking communication.  Would you like to see additional details?"),"",MessageBoxButtons.YesNo)){
					return;
				}
				using MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(errors);
				msgbox.ShowDialog();
				return;
			}
			else if(listPatNumsFailed.Count>0){//if some failed
				FillMain();
				//reselect only the failed ones
				for(int i=0;i<_table.Rows.Count;i++) { //table.Rows.Count=grid.Rows.Count
					long patNum=PIn.Long(_table.Rows[i]["PatNum"].ToString());
					if(listPatNumsFailed.Contains(patNum)) {
						gridMain.SetSelected(i,true);
					}
				}
				if(DialogResult.Yes != MessageBox.Show(Lan.g(this,"Some emails failed to send.  All failed email confirmations have been selected in the confirmation list.  Would you like to see additional details?"),"",MessageBoxButtons.YesNo)) {
					return;
				}
				using MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(errors);
				msgbox.ShowDialog();
				SecurityLogs.MakeLogEntry(Permissions.EmailSend,0,"Confirmation Emails Sent: "+(listPatNumsSelected.Count-listPatNumsFailed.Count));
				return;
			}
			//none failed
			SecurityLogs.MakeLogEntry(Permissions.EmailSend,0,"Confirmation Emails Sent: "+listPatNumsSelected.Count);
			FillMain();
			//reselect the original list 
			for(int i=0;i<_table.Rows.Count;i++) {
				long patNum=PIn.Long(_table.Rows[i]["PatNum"].ToString());
				if(listPatNumsSelected.Contains(patNum)) {
					gridMain.SetSelected(i,true);
				}
			}
		}

		private void butText_Click(object sender,EventArgs e) {
			long patNum;
			string wirelessPhone;
			YN txtMsgOk;
			if(gridMain.ListGridRows.Count==0) {
				MsgBox.Show(this,"There are no Patients in the table.  Must have at least one.");
				return;
			}
			if(PrefC.GetLong(PrefName.ConfirmStatusTextMessaged)==0) {
				MsgBox.Show(this,"You need to set a status for text message confirmations in the Confirmation Setup window.");
				return;
			}
			if(gridMain.SelectedIndices.Length==0) {//None selected. Select all of type text that are not yet confirmed by text message.
				ContactMethod cmeth;
				for(int i=0;i<_table.Rows.Count;i++) {
					cmeth=(ContactMethod)PIn.Int(_table.Rows[i][checkGroupFamilies.Checked?"guarPreferConfirmMethod":"PreferConfirmMethod"].ToString());
					if(cmeth!=ContactMethod.TextMessage) {
						continue;
					}
					if(_table.Rows[i]["confirmed"].ToString()==Defs.GetName(DefCat.ApptConfirmed,PrefC.GetLong(PrefName.ConfirmStatusTextMessaged))) {//Already confirmed by text
						continue;
					}
					if(!_table.Rows[i]["contactMethod"].ToString().StartsWith("Text:")) {//Check contact method
						continue;
					}
					gridMain.SetSelected(i,true);
				}
				if(gridMain.SelectedIndices.Length==0) {
					MsgBox.Show(this,"All patients of text message type have been sent confirmations.");
					return;
				}
			}
			//deselect the ones that do not have text messages specified or are not OK to send texts to or have already been texted
			int skipped=0;
			for(int i=gridMain.SelectedIndices.Length-1;i>=0;i--) {
				wirelessPhone=_table.Rows[gridMain.SelectedIndices[i]][checkGroupFamilies.Checked?"guarWirelessPhone":"WirelessPhone"].ToString();
				if(wirelessPhone=="") {//Check for wireless number
					skipped++;
					gridMain.SetSelected(gridMain.SelectedIndices[i],false);
					continue;
				}
				txtMsgOk=(YN)PIn.Int(_table.Rows[gridMain.SelectedIndices[i]][checkGroupFamilies.Checked?"guarTxtMsgOK":"TxtMsgOk"].ToString());
				if(txtMsgOk==YN.Unknown	&& PrefC.GetBool(PrefName.TextMsgOkStatusTreatAsNo)) {//Check if OK to text
					skipped++;
					gridMain.SetSelected(gridMain.SelectedIndices[i],false);
					continue;
				}
				if(txtMsgOk==YN.No){//Check if OK to text
					skipped++;
					gridMain.SetSelected(gridMain.SelectedIndices[i],false);
					continue;
				}
				if(PrefC.HasClinicsEnabled && SmsPhones.IsIntegratedTextingEnabled()){//using clinics with Integrated texting must have a non-zero clinic num.
					patNum=PIn.Long(_table.Rows[gridMain.SelectedIndices[i]][checkGroupFamilies.Checked?"Guarantor":"PatNum"].ToString());
					long clinicNum=SmsPhones.GetClinicNumForTexting(patNum);
					if(clinicNum==0 || Clinics.GetClinic(clinicNum).SmsContractDate.Year<1880) {//no clinic or assigned clinic is not enabled.
						skipped++;
						gridMain.SetSelected(gridMain.SelectedIndices[i],false);
						continue;
					}
				}
			}
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"None of the selected patients have wireless phone numbers and are OK to text.");
				return;
			}
			if(skipped>0) {
				MessageBox.Show(Lan.g(this,"Selected patients skipped: ")+skipped.ToString());
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Send text message to all of the selected patients?")) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			FormTxtMsgEdit FormTME=new FormTxtMsgEdit();
			string message="";
			string familyApptList="";
			List<long> listApptNums=new List<long>();
			//Appointment apt;
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){
				Patient pat=new Patient {
					FName=PIn.String(_table.Rows[gridMain.SelectedIndices[i]]["FName"].ToString()),
					Preferred=PIn.String(_table.Rows[gridMain.SelectedIndices[i]]["Preferred"].ToString()),
				};
				DateTime dateTimeAskedToArrive=PIn.DateT(_table.Rows[gridMain.SelectedIndices[i]]["DateTimeAskedToArrive"].ToString());
				DateTime apptDateTime=PIn.DateT(_table.Rows[gridMain.SelectedIndices[i]]["AptDateTime"].ToString());
				if(checkGroupFamilies.Checked) {//build the list of appointments a family has scheduled
					if(i==gridMain.SelectedIndices.Length-1 || _table.Rows[gridMain.SelectedIndices[i]]["Guarantor"].ToString()!=_table.Rows[gridMain.SelectedIndices[i+1]]["Guarantor"].ToString()) {
						if(familyApptList!=""){
							familyApptList+="\r\n"+PatComm.BuildAppointmentMessage(pat,dateTimeAskedToArrive,apptDateTime);
							//continue with sending an email, because there are no more family members
						}
					}
					else if(_table.Rows[gridMain.SelectedIndices[i]]["Guarantor"].ToString()==_table.Rows[gridMain.SelectedIndices[i+1]]["Guarantor"].ToString()) {
						listApptNums.Add(PIn.Long(_table.Rows[gridMain.SelectedIndices[i]]["AptNum"].ToString()));
						familyApptList+=(familyApptList!=""?"\r\n":"")+PatComm.BuildAppointmentMessage(pat,dateTimeAskedToArrive,apptDateTime);
						continue;//skip sending emails to anyone that isn't the guarantor or isn't a single patient
					}
				}
				listApptNums.Add(PIn.Long(_table.Rows[gridMain.SelectedIndices[i]]["AptNum"].ToString()));
				patNum=PIn.Long(_table.Rows[gridMain.SelectedIndices[i]][checkGroupFamilies.Checked?"Guarantor":"PatNum"].ToString());
				long clinicNum=SmsPhones.GetClinicNumForTexting(patNum);
				wirelessPhone=PIn.String(_table.Rows[gridMain.SelectedIndices[i]][checkGroupFamilies.Checked?"guarWirelessPhone":"WirelessPhone"].ToString());
				txtMsgOk=((YN)PIn.Int(_table.Rows[gridMain.SelectedIndices[i]][checkGroupFamilies.Checked?"guarTxtMsgOK":"TxtMsgOk"].ToString()));
				if(checkGroupFamilies.Checked && familyApptList!="") {
					message=PrefC.GetString(PrefName.ConfirmTextFamMessage);
					message=message.Replace("[FamilyApptList]",familyApptList);
					familyApptList="";
				}
				else {
					message=PatComm.BuildConfirmMessage(ContactMethod.TextMessage,pat,dateTimeAskedToArrive,apptDateTime);
				}
				if(FormTME.SendText(patNum,wirelessPhone,message,txtMsgOk,clinicNum,SmsMessageSource.Confirmation)) {
					for(int j=0; j<listApptNums.Count; j++) {
						long aptNum=listApptNums[j];
						long newStatus=PrefC.GetLong(PrefName.ConfirmStatusTextMessaged);
						Appointment aptOld = Appointments.GetOneApt(aptNum);
						long oldStatus=aptOld.Confirmed;
						Appointments.SetConfirmed(aptOld,newStatus);
						if(newStatus!=oldStatus) {
							//Log confirmation status changes.
							SecurityLogs.MakeLogEntry(Permissions.ApptConfirmStatusEdit,patNum,
								Lans.g(this,"Appointment confirmation status automatically changed from")+" "
								+Defs.GetName(DefCat.ApptConfirmed,oldStatus)+" "+Lans.g(this,"to")+" "+Defs.GetName(DefCat.ApptConfirmed,newStatus)
								+" "+Lans.g(this,"from the confirmation list")+".",aptNum,aptOld.DateTStamp);
						}
					}
				}
				else {//There was an exception thrown in FormTME.SendText() meaning something went wrong.  Give the user an option to stop sending messages.
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"There was an error sending, do you want to continue sending messages?")) {
						break;
					}
				}
			}
			FillMain();
			Cursor=Cursors.Default;
		}

		private void CheckGroupFamilies_Click(object sender,EventArgs e) {
			FillMain();
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			/*if(  textDaysPast.errorProvider1.GetError(textDaysPast)!=""
				|| textDaysFuture.errorProvider1.GetError(textDaysFuture)!="")
			{
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			Prefs.Cur.PrefName="RecallDaysPast";
			Prefs.Cur.ValueString=textDaysPast.Text;
			Prefs.UpdateCur();
			Prefs.Cur.PrefName="RecallDaysFuture";
			Prefs.Cur.ValueString=textDaysFuture.Text;
			Prefs.UpdateCur();
			DataValid.SetInvalid(InvalidTypes.Prefs);*/
		}

		private void butPrint_Click(object sender,EventArgs e) {
			pagesPrinted=0;
			headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Confirmation list printed"),PrintoutOrientation.Landscape);
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			if(!headingPrinted) {
				text=Lan.g(this,"Confirmation List");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				text=textDateFrom.Text+" "+Lan.g(this,"to")+" "+textDateTo.Text;
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=20;
				headingPrinted=true;
				headingPrintH=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,pagesPrinted,bounds,headingPrintH);
			pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		

	

		

	

		

		

		

		

		
	}

}
