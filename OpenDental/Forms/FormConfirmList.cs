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
using System.Linq;

namespace OpenDental{
///<summary></summary>
	public partial class FormConfirmList : FormODBase {
		///<summary>Will be set to true when form closes if user click Send to Pinboard.</summary>
		public bool IsPinClicked=false;
		private int _pagesPrinted;
		private DataTable _tableAddresses;
		private int _patientsPrinted;
		///<summary>This list of appointments displayed</summary>
		private DataTable _tableAppointments;
		private bool _isHeadingPrinted;
		private int _heightHeadingPrint;
		private List<Provider> _listProviders;
		private List<Def> _listDefsApptConfirmed;

		///<summary></summary>
		public FormConfirmList(){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormConfirmList_Load(object sender, System.EventArgs e) {
			List<string> listRecallOptions=new List<string>() {
				"All",
				"Recall Only",
				"Exclude Recall",
				"Hygiene Prescheduled"
			};
			comboShowRecall.Items.AddList(listRecallOptions);
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
			_listDefsApptConfirmed=Defs.GetDefsForCategory(DefCat.ApptConfirmed,true);
			for(int i=0;i<_listDefsApptConfirmed.Count;i++){
				comboStatus.Items.Add(_listDefsApptConfirmed[i].ItemName);
				comboViewStatus.Items.Add(_listDefsApptConfirmed[i].ItemName,_listDefsApptConfirmed[i]);
			}
			if(!Security.IsAuthorized(Permissions.ApptConfirmStatusEdit,suppressMessage:true)) {//Suppress message because it would be very annoying to users.
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
			long curUserNum=Security.CurUser.UserNum;
			List<EmailAddress> emailAddresses=EmailAddresses.GetEmailAddressesForComboBoxes(curUserNum);
			comboEmailFrom.Items.AddList(emailAddresses,(x)=>EmailAddresses.GetDisplayStringForComboBox(x,curUserNum));
			comboEmailFrom.SelectedIndex=0;
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
			long patNum=PIn.Long(_tableAppointments.Rows[gridMain.SelectedIndices[gridMain.SelectedIndices.Length-1]]["PatNum"].ToString());
			Patient patient=Patients.GetPat(patNum);
			FormOpenDental.S_Contr_PatientSelected(patient,isRefreshCurModule:true);
		}

		private void gridMain_MouseUp(object sender,MouseEventArgs e) {
			if(e.Button==MouseButtons.Right && gridMain.SelectedIndices.Length>0) {
				//To maintain legacy behavior we will use the last selected index if multiple are selected.
				Patient patient=Patients.GetLim(PIn.Long(_tableAppointments.Rows[gridMain.SelectedIndices[gridMain.SelectedIndices.Length-1]]["PatNum"].ToString()));
				toolStripMenuItemSelectPatient.Text=Lan.g(gridMain.TranslationName,"Select Patient")+" ("+patient.GetNameFL()+")";
			}
		}

		///<summary>If multiple patients are selected in UnchedList, will select the last patient to remain consistent with sending to pinboard behavior.</summary>
		private void SeeChart_Click() {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an appointment first.");
				return;
			}
			//If multiple selected, just take the last one to remain consistent with SendPinboard_Click.
			long patNum=PIn.Long(_tableAppointments.Rows[gridMain.SelectedIndices[gridMain.SelectedIndices.Length-1]]["PatNum"].ToString());
			Patient patient=Patients.GetPat(patNum);
			FormOpenDental.S_Contr_PatientSelected(patient,isRefreshCurModule:false);
			GotoModule.GotoChart(patient.PatNum);
		}

		private void SendPinboard_Click() {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an appointment first.");
				return;
			}
			List<long> listAptNumsSelected=new List<long>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				listAptNumsSelected.Add(PIn.Long(_tableAppointments.Rows[gridMain.SelectedIndices[i]]["AptNum"].ToString()));
			}
			//This will send all appointments in listAptNumsSelected to the pinboard, and will select the patient attached to the last appointment.
			GotoModule.PinToAppt(listAptNumsSelected,0);
		}

		///<summary>Adds the specified number of work days, skipping saturday and sunday.</summary>
		private DateTime AddWorkDays(int days,DateTime date){
			DateTime dateTime=date;
			for(int i=0;i<days;i++){
				dateTime=dateTime.AddDays(1);
				//then, this part jumps to monday if on a sat or sun
				while(dateTime.DayOfWeek==DayOfWeek.Saturday || dateTime.DayOfWeek==DayOfWeek.Sunday){
					dateTime=dateTime.AddDays(1);
				}
			}
			return dateTime;
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
			Def def=comboViewStatus.GetSelected<Def>();
			long clinicNum=-1;
			if(PrefC.HasClinicsEnabled) {
				clinicNum=comboClinic.SelectedClinicNum;
			}
			ProgressOD progress=new ProgressOD();
			progress.ActionMain=() => _tableAppointments=Appointments.GetConfirmList(dateFrom,dateTo,provNum,clinicNum,showRecalls,showNonRecalls,
				showHygienePrescheduled,def.DefNum,checkGroupFamilies.Checked);
			progress.ShowDialogProgress();
			if(progress.IsCancelled){
				return;
			}
			int scrollVal=gridMain.ScrollValue;
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableConfirmList","Date Time"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableConfirmList","DateSched"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableConfirmList","Patient"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableConfirmList","Age"),30);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableConfirmList","Contact"),150);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableConfirmList","Addr/Ph Note"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableConfirmList","Status"),80);//confirmed
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableConfirmList","Procs"),110);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableConfirmList","Medical"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableConfirmList","Appt Note"),124);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			GridCell cell;
			for(int i=0;i<_tableAppointments.Rows.Count;i++) {
				Patient patient=new Patient {
					FName=PIn.String(_tableAppointments.Rows[i]["FName"].ToString()),
					Preferred=PIn.String(_tableAppointments.Rows[i]["Preferred"].ToString()),
					LName=PIn.String(_tableAppointments.Rows[i]["LName"].ToString()),
				};
				row=new GridRow();
				row.Cells.Add(PIn.DateT(_tableAppointments.Rows[i]["AptDateTime"].ToString()).ToString());
				row.Cells.Add(_tableAppointments.Rows[i]["dateSched"].ToString());
				row.Cells.Add(patient.GetNameLF());
				row.Cells.Add(_tableAppointments.Rows[i]["age"].ToString());
				row.Cells.Add(_tableAppointments.Rows[i]["contactMethod"].ToString());
				row.Cells.Add(_tableAppointments.Rows[i]["AddrNote"].ToString());
				row.Cells.Add(_tableAppointments.Rows[i]["confirmed"].ToString());
				row.Cells.Add(_tableAppointments.Rows[i]["ProcDescript"].ToString());
				cell=new GridCell(_tableAppointments.Rows[i]["medNotes"].ToString());
				cell.ColorText=Color.Red;
				row.Cells.Add(cell);
				row.Cells.Add(_tableAppointments.Rows[i]["Note"].ToString());
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
			long guarantor=PIn.Long(_tableAppointments.Rows[gridMain.SelectedIndices[0]]["Guarantor"].ToString());
			int famCount=0;
			for(int i=0;i<gridMain.ListGridRows.Count;i++){
				if(PIn.Long(_tableAppointments.Rows[i]["Guarantor"].ToString())==guarantor){
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
			long aptNum=PIn.Long(_tableAppointments.Rows[e.Row]["AptNum"].ToString());
			Patient patient=Patients.GetPat(PIn.Long(_tableAppointments.Rows[e.Row]["PatNum"].ToString()));
			FormOpenDental.S_Contr_PatientSelected(patient,isRefreshCurModule:true);
			using FormApptEdit formApptEdit=new FormApptEdit(aptNum);
			formApptEdit.PinIsVisible=true;
			formApptEdit.ShowDialog();
			if(formApptEdit.PinClicked) {//set from inside form.
				SendPinboard_Click();//Whatever they double clicked on will still be selected, just fire the event.
				DialogResult=DialogResult.OK;
			}
			else {
				FillMain();
			}
			for(int i=0;i<_tableAppointments.Rows.Count;i++){
				if(PIn.Long(_tableAppointments.Rows[i]["AptNum"].ToString())==aptNum){
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
			if(comboStatus.SelectedIndex==-1) {
				return;//User selected the comboBox but deslected after not selecting a value.
			}
			Appointment appointment;
			Cursor=Cursors.WaitCursor;
			long[] longArrayAptNumsSelected=new long[gridMain.SelectedIndices.Length];
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){
				longArrayAptNumsSelected[i]=PIn.Long(_tableAppointments.Rows[gridMain.SelectedIndices[i]]["AptNum"].ToString());
			}
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){
				appointment=Appointments.GetOneApt(PIn.Long(_tableAppointments.Rows[gridMain.SelectedIndices[i]]["AptNum"].ToString()));
				Appointment appointmentOld=appointment.Copy();
				int idxSelected=comboStatus.SelectedIndex;
				appointment.Confirmed=_listDefsApptConfirmed[idxSelected].DefNum;
				try{
					Appointments.Update(appointment,appointmentOld);
				}
				catch(ApplicationException ex){
					Cursor=Cursors.Default;
					MessageBox.Show(ex.Message);
					return;
				}
				if(appointment.Confirmed!=appointmentOld.Confirmed) {
					//Log confirmation status changes.
					SecurityLogs.MakeLogEntry(Permissions.ApptConfirmStatusEdit,appointment.PatNum,Lans.g(this,"Appointment confirmation status changed from")+" "
						+Defs.GetName(DefCat.ApptConfirmed,appointmentOld.Confirmed)+" "+Lans.g(this,"to")+" "+Defs.GetName(DefCat.ApptConfirmed,appointment.Confirmed)
						+" "+Lans.g(this,"from the confirmation list")+".",appointment.AptNum,appointmentOld.DateTStamp);
				}
			}
			FillMain();
			//reselect all the apts
			for(int i=0;i<_tableAppointments.Rows.Count;i++){
				for(int j=0;j<longArrayAptNumsSelected.Length;j++){
					if(PIn.Long(_tableAppointments.Rows[i]["AptNum"].ToString())==longArrayAptNumsSelected[j]){
						gridMain.SetSelected(i,true);
					}
				}
			}
			SetFamilyColors();
			comboStatus.SelectedIndex=-1;
			Cursor=Cursors.Default;
		}

		private void butReport_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.UserQuery)) {
				return;
			}
			if(_tableAppointments.Rows.Count==0) {
				MessageBox.Show(Lan.g(this,"There are no appointments in the list.  Must have at least one to run report."));
				return;
			}
			long[] longArrayAptNums;
			if(gridMain.SelectedIndices.Length==0) {
				longArrayAptNums=new long[_tableAppointments.Rows.Count];
				for(int i = 0;i<longArrayAptNums.Length;i++) {
					longArrayAptNums[i]=PIn.Long(_tableAppointments.Rows[i]["AptNum"].ToString());
				}
			}
			else {
				longArrayAptNums=new long[gridMain.SelectedIndices.Length];
				for(int i = 0;i<longArrayAptNums.Length;i++) {
					longArrayAptNums[i]=PIn.Long(_tableAppointments.Rows[gridMain.SelectedIndices[i]]["AptNum"].ToString());
				}
			}
			using FormRpConfirm formRpConfirm=new FormRpConfirm(longArrayAptNums);
			formRpConfirm.ShowDialog();
		}

		private void butLabels_Click(object sender,System.EventArgs e) {
			if(_tableAppointments.Rows.Count==0) {
				MessageBox.Show(Lan.g(this,"There are no appointments in the list.  Must have at least one to print."));
				return;
			}
			if(gridMain.SelectedIndices.Length==0) {
				for(int i = 0;i<_tableAppointments.Rows.Count;i++) {
					gridMain.SetSelected(i,true);
				}
			}
			List<long> listAptNums=new List<long>();
			for(int i = 0;i<gridMain.SelectedIndices.Length;i++) {
				listAptNums.Add(PIn.Long(_tableAppointments.Rows[gridMain.SelectedIndices[i]]["AptNum"].ToString()));
			}
			_tableAddresses=Appointments.GetAddrTable(listAptNums,checkGroupFamilies.Checked);
			_pagesPrinted=0;
			_patientsPrinted=0;
			Margins margins = new Margins(0,0,0,0);
			PrinterL.TryPreview(pdLabels_PrintPage,
				Lan.g(this,"Confirmation list labels printed"),
				PrintSituation.LabelSheet,
				margins,
				PrintoutOrigin.AtMargin,
				totalPages:(int)Math.Ceiling((double)_tableAddresses.Rows.Count/30)
			);
		}

		///<summary>Changes made to printing confirmation postcards need to be made in FormRecallList.butPostcards_Click() as well.</summary>
		private void butPostcards_Click(object sender,System.EventArgs e) {
			if(_tableAppointments.Rows.Count==0) {
				MessageBox.Show(Lan.g(this,"There are no appointments in the list.  Must have at least one to print."));
				return;
			}
			if(gridMain.SelectedIndices.Length==0) {
				ContactMethod contactMethod;
				for(int i=0;i<_tableAppointments.Rows.Count;i++) {
					contactMethod=(ContactMethod)PIn.Long(_tableAppointments.Rows[i]["PreferConfirmMethod"].ToString());
					if(contactMethod!=ContactMethod.Mail && contactMethod!=ContactMethod.None) {
						continue;
					}
					gridMain.SetSelected(i,true);
				}
			}
			List<long> listAptNums=new List<long>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				listAptNums.Add(PIn.Long(_tableAppointments.Rows[gridMain.SelectedIndices[i]]["AptNum"].ToString()));
			}
			if(listAptNums.Count==0) {
				MsgBox.Show(this,"No postcards necessary because contact method is not set to Mail for anyone in the list.");
				return;
			}
			_tableAddresses=Appointments.GetAddrTable(listAptNums, checkGroupFamilies.Checked);
			_pagesPrinted=0;
			_patientsPrinted=0;
			PaperSize paperSize;
			PrintoutOrientation printoutOrientation=PrintoutOrientation.Default;
			if(PrefC.GetLong(PrefName.RecallPostcardsPerSheet)==1) {
				paperSize=new PaperSize("Postcard",500,700);
				printoutOrientation=PrintoutOrientation.Landscape;
			}
			else if(PrefC.GetLong(PrefName.RecallPostcardsPerSheet)==3) {
				paperSize=new PaperSize("Postcard",850,1100);
			}
			else {//4
				paperSize=new PaperSize("Postcard",850,1100);
				printoutOrientation=PrintoutOrientation.Landscape;
			}
			int totalPages=((int)Math.Ceiling((double)_tableAddresses.Rows.Count/(double)PrefC.GetLong(PrefName.RecallPostcardsPerSheet)));
			Margins margins = new Margins(0,0,0,0);
			bool isDialogOk=PrinterL.TryPreview(pdCards_PrintPage,
				Lan.g(this,"Confirmation list postcards printed"),
				PrintSituation.Postcard,
				margins,
				PrintoutOrigin.AtMargin,
				paperSize,
				printoutOrientation,
				totalPages
			);
			if(!isDialogOk) { //dialog result not OK, postcards weren't sent to the printer.
				return;
			}
			for(int i=0;i<_tableAddresses.Rows.Count;i++) { //loop through the address table and create commlog entries for all selected.
				Commlog commlog=new Commlog();
				commlog.CommDateTime=DateTime.Now;
				commlog.Mode_=CommItemMode.Mail;
				commlog.Note="Confirmation postcard printed for "+_tableAddresses.Rows[i]["LName"].ToString()
						+", "+_tableAddresses.Rows[i]["FName"].ToString()+"\r\n"+_tableAddresses.Rows[i]["Address"].ToString()+"\r\n";
				if(_tableAddresses.Rows[i]["Address2"].ToString()!="") {
					commlog.Note+=_tableAddresses.Rows[i]["Address2"].ToString()+"\r\n";
				}
				commlog.Note+=_tableAddresses.Rows[i]["City"].ToString()+", "
				+_tableAddresses.Rows[i]["State"].ToString()+"   "
				+_tableAddresses.Rows[i]["Zip"].ToString()+"\r\n";
				commlog.PatNum=PIn.Long(_tableAddresses.Rows[i]["PatNum"].ToString());
				commlog.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.MISC);
				commlog.SentOrReceived=CommSentOrReceived.Sent;
				commlog.UserNum=Security.CurUser.UserNum;
				Commlogs.Insert(commlog);
			}
		}

		///<summary>raised for each page to be printed.</summary>
		private void pdLabels_PrintPage(object sender, PrintPageEventArgs ev){
			int totalPages=(int)Math.Ceiling((double)_tableAddresses.Rows.Count/30);
			Graphics g=ev.Graphics;
			float yPos=75;
			float xPos=50;
			string text="";
			while(yPos<1000 && _patientsPrinted<_tableAddresses.Rows.Count){
				if(checkGroupFamilies.Checked && _tableAddresses.Rows[_patientsPrinted]["famList"].ToString()!="") {
					text=_tableAddresses.Rows[_patientsPrinted]["guarLName"].ToString()+" "+Lan.g(this,"Household")+"\r\n";
				}
				else {
					text=_tableAddresses.Rows[_patientsPrinted]["FName"].ToString()+" "
						+_tableAddresses.Rows[_patientsPrinted]["MiddleI"].ToString()+" "
						+_tableAddresses.Rows[_patientsPrinted]["LName"].ToString()+"\r\n";
				}
				text+=_tableAddresses.Rows[_patientsPrinted]["Address"].ToString()+"\r\n";
				if(_tableAddresses.Rows[_patientsPrinted]["Address2"].ToString()!=""){
					text+=_tableAddresses.Rows[_patientsPrinted]["Address2"].ToString()+"\r\n";
				}
				text+=_tableAddresses.Rows[_patientsPrinted]["City"].ToString()+", "
					+_tableAddresses.Rows[_patientsPrinted]["State"].ToString()+"   "
					+_tableAddresses.Rows[_patientsPrinted]["Zip"].ToString()+"\r\n";
				Rectangle rectangle=new Rectangle((int)xPos,(int)yPos,275,100);
				using Font font=new Font(FontFamily.GenericSansSerif,11);
				MapAreaRoomControl.FitText(text,font,Brushes.Black,rectangle,new StringFormat(),g);
				//reposition for next label
				xPos+=275;
				if(xPos>850){//drop a line
					xPos=50;
					yPos+=100;
				}
				_patientsPrinted++;
			}
			_pagesPrinted++;
			if(_pagesPrinted>=totalPages){
				ev.HasMorePages=false;
				_pagesPrinted=0;//because it has to print again from the print preview
				_patientsPrinted=0;
			}
			else{
				ev.HasMorePages=true;
			}
			g.Dispose();
		}

		///<summary>raised for each page to be printed.</summary>
		private void pdCards_PrintPage(object sender, PrintPageEventArgs ev){
			int totalPages=(int)Math.Ceiling((double)_tableAddresses.Rows.Count/(double)PrefC.GetLong(PrefName.RecallPostcardsPerSheet));
			Graphics g=ev.Graphics;
			int yAdjust=(int)(PrefC.GetDouble(PrefName.RecallAdjustDown)*100);
			int xAdjust=(int)(PrefC.GetDouble(PrefName.RecallAdjustRight)*100);
			float yPos=0+yAdjust;//these refer to the upper left origin of each postcard
			float xPos=0+xAdjust;
			const int bottomPageMargin=100;
			string postcardMessage;
			while(yPos<ev.PageBounds.Height-bottomPageMargin && _patientsPrinted<_tableAddresses.Rows.Count){
				//Return Address--------------------------------------------------------------------------
				if(PrefC.GetBool(PrefName.RecallCardsShowReturnAdd)){
					if(!PrefC.HasClinicsEnabled || PIn.Long(_tableAddresses.Rows[_patientsPrinted]["ClinicNum"].ToString())==0) {//No clinics or no clinic selected for this appt
						postcardMessage=PrefC.GetString(PrefName.PracticeTitle)+"\r\n";
						using Font font=new Font(FontFamily.GenericSansSerif,9,FontStyle.Bold);
						g.DrawString(postcardMessage,font,Brushes.Black,xPos+45,yPos+60);
						postcardMessage=PrefC.GetString(PrefName.PracticeAddress)+"\r\n";
						if(PrefC.GetString(PrefName.PracticeAddress2)!="") {
							postcardMessage+=PrefC.GetString(PrefName.PracticeAddress2)+"\r\n";
						}
						postcardMessage+=PrefC.GetString(PrefName.PracticeCity)+",  "+PrefC.GetString(PrefName.PracticeST)+"  "+PrefC.GetString(PrefName.PracticeZip)+"\r\n";
						string phone=PrefC.GetString(PrefName.PracticePhone);
						if(phone.Length==10) {
							postcardMessage+=TelephoneNumbers.ReFormat(phone);
						}
						else {//any other phone format
							postcardMessage+=phone;
						}
					}
					else {//Clinics enabled and clinic selected
						Clinic clinic=Clinics.GetClinic(PIn.Long(_tableAddresses.Rows[_patientsPrinted]["ClinicNum"].ToString()));
						postcardMessage=clinic.Description+"\r\n";
						using Font font=new Font(FontFamily.GenericSansSerif,9,FontStyle.Bold);
						g.DrawString(postcardMessage,font,Brushes.Black,xPos+45,yPos+60);
						postcardMessage=clinic.Address+"\r\n";
						if(clinic.Address2!="") {
							postcardMessage+=clinic.Address2+"\r\n";
						}
						postcardMessage+=clinic.City+",  "+clinic.State+"  "+clinic.Zip+"\r\n";
						string phone=clinic.Phone;
						if(phone.Length==10) {
							postcardMessage+=TelephoneNumbers.ReFormat(phone);
						}
						else {//any other phone format
							postcardMessage+=phone;
						}
					}
					using Font fontReturn=new Font(FontFamily.GenericSansSerif,8);
					g.DrawString(postcardMessage,fontReturn,Brushes.Black,xPos+45,yPos+75);
				}
				//Body text-------------------------------------------------------------------------------
				string famList=_tableAddresses.Rows[_patientsPrinted]["famList"].ToString();
				if(checkGroupFamilies.Checked	&& famList!=""){
					postcardMessage=PrefC.GetString(PrefName.ConfirmPostcardFamMessage);
					postcardMessage=postcardMessage.Replace("[FamilyApptList]",famList);
				}
				//Body text, single card-------------------------------------------------------------------
				else{
					DateTime dateTimeAskedToArrive=PIn.Date(_tableAddresses.Rows[_patientsPrinted]["DateTimeAskedToArrive"].ToString());
					DateTime dateTimeApt=PIn.Date(_tableAddresses.Rows[_patientsPrinted]["AptDateTime"].ToString());
					Patient patient=new Patient();
					patient.FName=PIn.String(_tableAddresses.Rows[_patientsPrinted]["FName"].ToString());
					patient.Preferred=PIn.String(_tableAddresses.Rows[_patientsPrinted]["Preferred"].ToString());
					postcardMessage=PatComm.BuildConfirmMessage(ContactMethod.Mail,patient,dateTimeAskedToArrive,dateTimeApt);
				}
				using Font fontBody=new Font(FontFamily.GenericSansSerif,10);
				RectangleF rectangleF=new RectangleF(xPos+45,yPos+180,250,190);
				g.DrawString(postcardMessage,fontBody,Brushes.Black,rectangleF);
				//Patient's Address-----------------------------------------------------------------------
				if(checkGroupFamilies.Checked	&& _tableAddresses.Rows[_patientsPrinted]["famList"].ToString()!="")//print family card
				{
					postcardMessage=_tableAddresses.Rows[_patientsPrinted]["guarLName"].ToString()+" "+Lan.g(this,"Household")+"\r\n";
				}
				else{//print single card
					postcardMessage=_tableAddresses.Rows[_patientsPrinted]["FName"].ToString()+" "
						+_tableAddresses.Rows[_patientsPrinted]["MiddleI"].ToString()+" "
						+_tableAddresses.Rows[_patientsPrinted]["LName"].ToString()+"\r\n";
				}
				postcardMessage+=_tableAddresses.Rows[_patientsPrinted]["Address"].ToString()+"\r\n";
					if(_tableAddresses.Rows[_patientsPrinted]["Address2"].ToString()!=""){
						postcardMessage+=_tableAddresses.Rows[_patientsPrinted]["Address2"].ToString()+"\r\n";
					}
					postcardMessage+=_tableAddresses.Rows[_patientsPrinted]["City"].ToString()+", "
						+_tableAddresses.Rows[_patientsPrinted]["State"].ToString()+"   "
						+_tableAddresses.Rows[_patientsPrinted]["Zip"].ToString()+"\r\n";
				using Font fontAddress=new Font(FontFamily.GenericSansSerif,11);
				g.DrawString(postcardMessage,fontAddress,Brushes.Black,xPos+320,yPos+240);
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
						xPos=0+xAdjust;
						yPos+=425;
					}
				}
				_patientsPrinted++;
			}//while
			_pagesPrinted++;
			if(_pagesPrinted==totalPages){
				ev.HasMorePages=false;
				_pagesPrinted=0;
				_patientsPrinted=0;
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
			if(comboEmailFrom.GetSelected<EmailAddress>() is null) {
				MsgBox.Show(this,"No valid email address is selected.");
				return;
			}
			if(gridMain.SelectedIndices.Length==0) {
				ContactMethod contactMethod;
				for(int i=0;i<_tableAppointments.Rows.Count;i++) {
					contactMethod=(ContactMethod)PIn.Int(_tableAppointments.Rows[i][checkGroupFamilies.Checked?"guarPreferConfirmMethod":"PreferConfirmMethod"].ToString());
					if(contactMethod!=ContactMethod.Email) {
						continue;
					}
					if(_tableAppointments.Rows[i]["confirmed"].ToString()==Defs.GetName(DefCat.ApptConfirmed,PrefC.GetLong(PrefName.ConfirmStatusEmailed))) {//Already confirmed by email
						continue;
					}
					if(_tableAppointments.Rows[i][checkGroupFamilies.Checked?"guarEmail":"email"].ToString()=="") {
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
					if(_tableAppointments.Rows[gridMain.SelectedIndices[i]][checkGroupFamilies.Checked?"guarEmail":"email"].ToString()=="") {
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
			EmailMessage emailMessage;
			string confirmationMessage="";
			List<long> listPatNumsSelected=new List<long>();
			List<long> listPatNumsFailed=new List<long>();
			EmailAddress emailAddress=comboEmailFrom.GetSelected<EmailAddress>();
			string errors="";
			string familyApptList="";
			List<long> listAptNums=new List<long>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){				
				Patient patient=new Patient();
				patient.FName=PIn.String(_tableAppointments.Rows[gridMain.SelectedIndices[i]]["FName"].ToString());
				patient.Preferred=PIn.String(_tableAppointments.Rows[gridMain.SelectedIndices[i]]["Preferred"].ToString());
				DateTime dateTimeAskedToArrive=PIn.DateT(_tableAppointments.Rows[gridMain.SelectedIndices[i]]["DateTimeAskedToArrive"].ToString());
				DateTime dateTimeApt=PIn.DateT(_tableAppointments.Rows[gridMain.SelectedIndices[i]]["AptDateTime"].ToString());
				if(checkGroupFamilies.Checked) {//build the list of appointments a family has scheduled
					if(i==gridMain.SelectedIndices.Length-1 
						|| _tableAppointments.Rows[gridMain.SelectedIndices[i]]["Guarantor"].ToString()!=_tableAppointments.Rows[gridMain.SelectedIndices[i+1]]["Guarantor"].ToString()) 
					{
						if(familyApptList!=""){
							familyApptList+="\r\n"+PatComm.BuildAppointmentMessage(patient,dateTimeAskedToArrive,dateTimeApt);
							//continue with sending an email, because there are no more family members
						}
					}
					else if(_tableAppointments.Rows[gridMain.SelectedIndices[i]]["Guarantor"].ToString()==_tableAppointments.Rows[gridMain.SelectedIndices[i+1]]["Guarantor"].ToString()) {
						listAptNums.Add(PIn.Long(_tableAppointments.Rows[gridMain.SelectedIndices[i]]["AptNum"].ToString()));
						familyApptList+=(familyApptList!=""?"\r\n":"")+PatComm.BuildAppointmentMessage(patient,dateTimeAskedToArrive,dateTimeApt);
						continue;//skip sending emails to anyone that isn't the guarantor or isn't a single patient
					}
				}
				listAptNums.Add(PIn.Long(_tableAppointments.Rows[gridMain.SelectedIndices[i]]["AptNum"].ToString()));
				emailMessage=new EmailMessage();
				long clinicNum=Clinics.ClinicNum;
				emailMessage.PatNum=PIn.Long(_tableAppointments.Rows[gridMain.SelectedIndices[i]][checkGroupFamilies.Checked?"Guarantor":"PatNum"].ToString());
				emailMessage.ToAddress=_tableAppointments.Rows[gridMain.SelectedIndices[i]][checkGroupFamilies.Checked?"guarEmail":"email"].ToString();//Could be guarantor email.
				if(emailAddress.EmailAddressNum==0) { //clinic/practice default
					clinicNum=PIn.Long(_tableAppointments.Rows[gridMain.SelectedIndices[i]][checkGroupFamilies.Checked?"guarClinicNum":"ClinicNum"].ToString());
					emailAddress=EmailAddresses.GetByClinic(clinicNum);
				}
				emailMessage.FromAddress=emailAddress.GetFrom();				
				emailMessage.Subject=PrefC.GetString(PrefName.ConfirmEmailSubject);
				listPatNumsSelected.Add(emailMessage.PatNum);
				if(checkGroupFamilies.Checked && familyApptList!="") {
					confirmationMessage=PrefC.GetString(PrefName.ConfirmPostcardFamMessage);
					confirmationMessage=confirmationMessage.Replace("[FamilyApptList]",familyApptList);
					familyApptList="";
				}
				else {
					confirmationMessage=PatComm.BuildConfirmMessage(ContactMethod.Email,patient,dateTimeAskedToArrive,dateTimeApt);
				}
				emailMessage.BodyText=EmailMessages.FindAndReplacePostalAddressTag(confirmationMessage,clinicNum);
				emailMessage.MsgDateTime=DateTime.Now;
				emailMessage.SentOrReceived=EmailSentOrReceived.Sent;
				emailMessage.MsgType=EmailMessageSource.Confirmation;
				try {
					EmailMessages.SendEmail(emailMessage,emailAddress);
				}
				catch (Exception ex){
					listPatNumsFailed.Add(emailMessage.PatNum);
					if(!errors.Contains("Message send fail for Patnum:"+emailMessage.PatNum+":  "+ex.Message)) {//unique messages only.
						errors+=("Message send fail for Patnum:"+emailMessage.PatNum+":  "+ex.Message+"\r\n");
					}
					continue;
				}
				for(int j=0; j<listAptNums.Count; j++) {
					Appointment appt=Appointments.GetOneApt(listAptNums[j]);
					Appointments.SetConfirmed(appt,PrefC.GetLong(PrefName.ConfirmStatusEmailed));
				}
				listAptNums.Clear();
			}
			Cursor=Cursors.Default;
			if(listPatNumsFailed.Count==gridMain.SelectedIndices.Length){ //all failed
				//no need to refresh
				if(DialogResult.Yes != MessageBox.Show(Lan.g(this,"All emails failed. Possibly due to invalid email addresses, a loss of connectivity, or a firewall blocking communication.  Would you like to see additional details?"),"",MessageBoxButtons.YesNo)){
					return;
				}
				using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(errors);
				msgBoxCopyPaste.ShowDialog();
				return;
			}
			else if(listPatNumsFailed.Count>0){//if some failed
				FillMain();
				//reselect only the failed ones
				for(int i=0;i<_tableAppointments.Rows.Count;i++) { //table.Rows.Count=grid.Rows.Count
					long patNum=PIn.Long(_tableAppointments.Rows[i]["PatNum"].ToString());
					if(listPatNumsFailed.Contains(patNum)) {
						gridMain.SetSelected(i,true);
					}
				}
				if(DialogResult.Yes != MessageBox.Show(Lan.g(this,"Some emails failed to send.  All failed email confirmations have been selected in the confirmation list.  Would you like to see additional details?"),"",MessageBoxButtons.YesNo)) {
					return;
				}
				using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(errors);
				msgBoxCopyPaste.ShowDialog();
				SecurityLogs.MakeLogEntry(Permissions.EmailSend,0,"Confirmation Emails Sent: "+(listPatNumsSelected.Count-listPatNumsFailed.Count));
				return;
			}
			//none failed
			SecurityLogs.MakeLogEntry(Permissions.EmailSend,0,"Confirmation Emails Sent: "+listPatNumsSelected.Count);
			FillMain();
			//reselect the original list 
			for(int i=0;i<_tableAppointments.Rows.Count;i++) {
				long patNum=PIn.Long(_tableAppointments.Rows[i]["PatNum"].ToString());
				if(listPatNumsSelected.Contains(patNum)) {
					gridMain.SetSelected(i,true);
				}
			}
		}

		private void butText_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.TextMessageSend)) {
				return;
			}
			long patNum;
			string wirelessPhone;
			YN YNtxtMsgOk;
			if(gridMain.ListGridRows.Count==0) {
				MsgBox.Show(this,"There are no Patients in the table.  Must have at least one.");
				return;
			}
			if(PrefC.GetLong(PrefName.ConfirmStatusTextMessaged)==0) {
				MsgBox.Show(this,"You need to set a status for text message confirmations in the Confirmation Setup window.");
				return;
			}
			if(gridMain.SelectedIndices.Length==0) {//None selected. Select all of type text that are not yet confirmed by text message.
				ContactMethod contactMethod;
				for(int i=0;i<_tableAppointments.Rows.Count;i++) {
					contactMethod=(ContactMethod)PIn.Int(_tableAppointments.Rows[i][checkGroupFamilies.Checked?"guarPreferConfirmMethod":"PreferConfirmMethod"].ToString());
					if(contactMethod!=ContactMethod.TextMessage) {
						continue;
					}
					if(_tableAppointments.Rows[i]["confirmed"].ToString()==Defs.GetName(DefCat.ApptConfirmed,PrefC.GetLong(PrefName.ConfirmStatusTextMessaged))) {//Already confirmed by text
						continue;
					}
					if(!_tableAppointments.Rows[i]["contactMethod"].ToString().StartsWith("Text:")) {//Check contact method
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
				wirelessPhone=_tableAppointments.Rows[gridMain.SelectedIndices[i]][checkGroupFamilies.Checked?"guarWirelessPhone":"WirelessPhone"].ToString();
				if(wirelessPhone=="") {//Check for wireless number
					skipped++;
					gridMain.SetSelected(gridMain.SelectedIndices[i],false);
					continue;
				}
				YNtxtMsgOk=(YN)PIn.Int(_tableAppointments.Rows[gridMain.SelectedIndices[i]][checkGroupFamilies.Checked?"guarTxtMsgOK":"TxtMsgOk"].ToString());
				if(YNtxtMsgOk==YN.Unknown	&& PrefC.GetBool(PrefName.TextMsgOkStatusTreatAsNo)) {//Check if OK to text
					skipped++;
					gridMain.SetSelected(gridMain.SelectedIndices[i],false);
					continue;
				}
				if(YNtxtMsgOk==YN.No){//Check if OK to text
					skipped++;
					gridMain.SetSelected(gridMain.SelectedIndices[i],false);
					continue;
				}
				if(PrefC.HasClinicsEnabled && SmsPhones.IsIntegratedTextingEnabled()){//using clinics with Integrated texting must have a non-zero clinic num.
					patNum=PIn.Long(_tableAppointments.Rows[gridMain.SelectedIndices[i]][checkGroupFamilies.Checked?"Guarantor":"PatNum"].ToString());
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
			FormTxtMsgEdit formTxtMsgEdit=new FormTxtMsgEdit();
			string confirmationMessage="";
			string familyApptList="";
			List<long> listAptNums=new List<long>();
			//Appointment apt;
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){
				Patient patient=new Patient();
				patient.FName=PIn.String(_tableAppointments.Rows[gridMain.SelectedIndices[i]]["FName"].ToString());
				patient.Preferred=PIn.String(_tableAppointments.Rows[gridMain.SelectedIndices[i]]["Preferred"].ToString());
				DateTime dateTimeAskedToArrive=PIn.DateT(_tableAppointments.Rows[gridMain.SelectedIndices[i]]["DateTimeAskedToArrive"].ToString());
				DateTime dateTimeApt=PIn.DateT(_tableAppointments.Rows[gridMain.SelectedIndices[i]]["AptDateTime"].ToString());
				if(checkGroupFamilies.Checked) {//build the list of appointments a family has scheduled
					if(i==gridMain.SelectedIndices.Length-1 
						|| _tableAppointments.Rows[gridMain.SelectedIndices[i]]["Guarantor"].ToString()!=_tableAppointments.Rows[gridMain.SelectedIndices[i+1]]["Guarantor"].ToString()) 
					{
						if(familyApptList!=""){
							familyApptList+="\r\n"+PatComm.BuildAppointmentMessage(patient,dateTimeAskedToArrive,dateTimeApt);
							//continue with sending an email, because there are no more family members
						}
					}
					else if(_tableAppointments.Rows[gridMain.SelectedIndices[i]]["Guarantor"].ToString()==_tableAppointments.Rows[gridMain.SelectedIndices[i+1]]["Guarantor"].ToString()) {
						listAptNums.Add(PIn.Long(_tableAppointments.Rows[gridMain.SelectedIndices[i]]["AptNum"].ToString()));
						familyApptList+=(familyApptList!=""?"\r\n":"")+PatComm.BuildAppointmentMessage(patient,dateTimeAskedToArrive,dateTimeApt);
						continue;//skip sending emails to anyone that isn't the guarantor or isn't a single patient
					}
				}
				listAptNums.Add(PIn.Long(_tableAppointments.Rows[gridMain.SelectedIndices[i]]["AptNum"].ToString()));
				patNum=PIn.Long(_tableAppointments.Rows[gridMain.SelectedIndices[i]][checkGroupFamilies.Checked?"Guarantor":"PatNum"].ToString());
				long clinicNum=SmsPhones.GetClinicNumForTexting(patNum);
				wirelessPhone=PIn.String(_tableAppointments.Rows[gridMain.SelectedIndices[i]][checkGroupFamilies.Checked?"guarWirelessPhone":"WirelessPhone"].ToString());
				YNtxtMsgOk=((YN)PIn.Int(_tableAppointments.Rows[gridMain.SelectedIndices[i]][checkGroupFamilies.Checked?"guarTxtMsgOK":"TxtMsgOk"].ToString()));
				if(checkGroupFamilies.Checked && familyApptList!="") {
					confirmationMessage=PrefC.GetString(PrefName.ConfirmTextFamMessage);
					confirmationMessage=confirmationMessage.Replace("[FamilyApptList]",familyApptList);
					familyApptList="";
				}
				else {
					confirmationMessage=PatComm.BuildConfirmMessage(ContactMethod.TextMessage,patient,dateTimeAskedToArrive,dateTimeApt);
				}
				if(formTxtMsgEdit.SendText(patNum,wirelessPhone,confirmationMessage,YNtxtMsgOk,clinicNum,SmsMessageSource.Confirmation)) {
					for(int j=0; j<listAptNums.Count; j++) {
						long aptNum=listAptNums[j];
						long confirmStatusNew=PrefC.GetLong(PrefName.ConfirmStatusTextMessaged);
						Appointment appointmentOld = Appointments.GetOneApt(aptNum);
						long confirmStatusOld=appointmentOld.Confirmed;
						Appointments.SetConfirmed(appointmentOld,confirmStatusNew);
						if(confirmStatusNew!=confirmStatusOld) {
							//Log confirmation status changes.
							SecurityLogs.MakeLogEntry(Permissions.ApptConfirmStatusEdit,patNum,
								Lans.g(this,"Appointment confirmation status automatically changed from")+" "
								+Defs.GetName(DefCat.ApptConfirmed,confirmStatusOld)+" "+Lans.g(this,"to")+" "+Defs.GetName(DefCat.ApptConfirmed,confirmStatusNew)
								+" "+Lans.g(this,"from the confirmation list")+".",aptNum,appointmentOld.DateTStamp);
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
			_pagesPrinted=0;
			_isHeadingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Confirmation list printed"),PrintoutOrientation.Landscape);
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle rectangle=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string headerText;
			using Font fontHeading=new Font("Arial",13,FontStyle.Bold);
			using Font fontSubHeading=new Font("Arial",10,FontStyle.Bold);
			int yPos=rectangle.Top;
			int center=rectangle.X+rectangle.Width/2;
			#region printHeading
			if(!_isHeadingPrinted) {
				headerText=Lan.g(this,"Confirmation List");
				g.DrawString(headerText,fontHeading,Brushes.Black,center-g.MeasureString(headerText,fontHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(headerText,fontHeading).Height;
				headerText=textDateFrom.Text+" "+Lan.g(this,"to")+" "+textDateTo.Text;
				g.DrawString(headerText,fontSubHeading,Brushes.Black,center-g.MeasureString(headerText,fontSubHeading).Width/2,yPos);
				yPos+=20;
				_isHeadingPrinted=true;
				_heightHeadingPrint=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,_pagesPrinted,rectangle,_heightHeadingPrint);
			_pagesPrinted++;
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
