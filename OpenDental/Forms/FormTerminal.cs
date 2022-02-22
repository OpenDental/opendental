using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>Fills the primary monitor with the kiosk screen.</summary>
	public partial class FormTerminal:FormODBase {
		///<summary>This is the list of sheets that the patient will be filling out.</summary>
		private List<Sheet> _listSheets;
		///<summary>This gets set externally when IsSimpleMode, and it also gets set when NOT IsSimpleMode in ProcessSignals.</summary>
		public long PatNum;
		///<summary>In simple mode, the terminal is launched from the local machine, and is not tracked in the database in any way.</summary>
		public bool IsSimpleMode;
		///<summary>Form used to display sheets to fill for patient.  This is class-wide so that it can be accessed when the timer ticks in order to force
		///the form to close itself.</summary>
		private FormSheetFillEdit _formSheetFillEdit;

		///<summary></summary>
		public FormTerminal() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormTerminal_Load(object sender,EventArgs e) {
			Size=System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
			Location=new Point(0,0);
			//The layout manager has trouble keeping controls centered sometimes so manually move locations of controls to avoid confusion for further changes.
			//All x values are relative to the form's size, so no scaling is needed.
			//The y value of the top most control is based on a percentage of the form's height.
			//The y position of each subsequent control is based on the height and location of the previous control.
			LayoutManager.MoveLocation(labelWelcome,new Point((Size.Width/2)-(labelWelcome.Width/2),Convert.ToInt32(Size.Height*0.22f)));
			LayoutManager
				.MoveLocation(labelForms,new Point((Size.Width/2)-(labelForms.Width/2),labelWelcome.Location.Y+labelWelcome.Height+LayoutManager.Scale(23)));
			LayoutManager.MoveLocation(listForms,new Point((Size.Width/2)-(listForms.Width/2),labelForms.Location.Y+labelForms.Height+LayoutManager.Scale(3)));
			LayoutManager.MoveLocation(labelThankYou,new Point((Size.Width/2)-(labelThankYou.Width/2),listForms.Location.Y+listForms.Height));
			LayoutManager.MoveLocation(butDone,new Point((Size.Width/2)-(butDone.Width/2),labelThankYou.Location.Y+labelThankYou.Height));
			LayoutManager.MoveLocation(labelConnection,new Point((Size.Width/2)-(labelConnection.Width/2),labelConnection.Location.Y));
			labelConnection.Visible=false;
			_listSheets=new List<Sheet>();
			if(IsSimpleMode) {
				//PatNum set externally (in FormPatientForms)
				return;
			}
			//NOT SimpleMode from here down
			Process processCur=Process.GetCurrentProcess();
			//Delete all terminalactives for this computer, except new one, based on CompName and SessionID
			TerminalActives.DeleteForCmptrSessionAndId(ODEnvironment.MachineName,processCur.SessionId,excludeId:processCur.Id);
			string clientName=null;
			string userName=null;
			try {
				clientName=Environment.GetEnvironmentVariable("ClientName");
				userName=Environment.GetEnvironmentVariable("UserName");
			}
			catch(Exception) {
				//user may not have permission to access environment variables or another error could happen
			}
			if(string.IsNullOrWhiteSpace(clientName)) {
				//ClientName only set for remote sessions, try to find suitable replacement.
				clientName=userName;
				if(processCur.SessionId<2  || string.IsNullOrWhiteSpace(userName)) {
					//if sessionId is 0 or 1, this is not a remote session, use MachineName
					clientName=ODEnvironment.MachineName;
				}
			}
			if(string.IsNullOrWhiteSpace(clientName) || TerminalActives.IsCompClientNameInUse(ODEnvironment.MachineName,clientName)) {
				using InputBox iBox=new InputBox("Please enter a unique name to identify this kiosk.");
				iBox.setTitle(Lan.g(this,"Kiosk Session Name"));
				iBox.ShowDialog();
				while(iBox.DialogResult==DialogResult.OK && TerminalActives.IsCompClientNameInUse(ODEnvironment.MachineName,iBox.textResult.Text)) {
					MsgBox.Show(this,"The name entered is invalid or already in use.");
					iBox.ShowDialog();
				}
				if(iBox.DialogResult!=DialogResult.OK) {
					DialogResult=DialogResult.Cancel;
					return;//not allowed to enter kiosk mode unless a unique human-readable name is entered for this computer session
				}
				clientName=iBox.textResult.Text;
			}
			//if we get here, we have a SessionId (which could be 0 if not in a remote session) and a unique client name for this kiosk
			TerminalActive terminal=new TerminalActive();
			terminal.ComputerName=ODEnvironment.MachineName;
			terminal.SessionId=processCur.SessionId;
			terminal.SessionName=clientName;
			terminal.ProcessId=processCur.Id;
			TerminalActives.Insert(terminal);//tell the database that a terminal is newly active on this computer
			Signalods.SetInvalid(InvalidType.Kiosk,KeyType.ProcessId,processCur.Id);//signal FormTerminalManager to re-fill grids
			timer1.Enabled=true;
		}

		private void FormTerminal_Shown(object sender,EventArgs e) {
			//force immediate loading/unloading of patient without waiting for a signal to be sent and processed
			LoadPatient(false);//this needs to happen in the Shown() method because otherwise the background of the current screen will not be greyed out.
		}

		///<summary>Only subscribed to signal processing if NOT SimpleMode.  Only processes signals of type InvalidType.Kiosk.</summary>
		public override void ProcessSignalODs(List<Signalod> listSignals) {
			if(IsSimpleMode) { //Process signals ONLY if not simple mode.
				return;
			}
			int processIdCur=Process.GetCurrentProcess().Id;
			//load patient if any signals are Kiosk type either without a FKey or with a ProcessId different than this process
			if(listSignals.Any(x => x.IType==InvalidType.Kiosk && (x.FKeyType!=KeyType.ProcessId || x.FKey!=processIdCur))) {
				LoadPatient(false);//will load the current patient if PatNum>0, otherwise will force clearing/unloading the patient
			}
		}

		///<summary>Only in nonSimpleMode.  Occurs every 4 seconds. Checks the database to verify that this kiosk should still be running and that the
		///correct patient's forms are loaded.  If there shouldn't be forms loaded, clears the forms.  If this kiosk (terminalactive row) has been deleted
		///then this form is closed.  If FormSheetFillEdit is visible, signals that form to force it closed (user will lose any unsaved data).</summary>
		private void timer1_Tick(object sender,EventArgs e) {
			TerminalActive terminal=null;
			try{
				Process processCur=Process.GetCurrentProcess();
				terminal=TerminalActives.GetForCmptrSessionAndId(ODEnvironment.MachineName,processCur.SessionId,processCur.Id);
				labelConnection.Visible=false;
			}
			catch(Exception) {//SocketException if db connection gets lost.
				labelConnection.Visible=true;
				return;
			}
			if(terminal==null) {
				//this terminal shouldn't be running, receptionist must've deleted this kiosk, close the form (causes application exit if NOT IsSimpleMode)
				if(_formSheetFillEdit!=null && !_formSheetFillEdit.IsDisposed) {
					_formSheetFillEdit.ForceClose();
				}
				Close();
				return;
			}
			if(_formSheetFillEdit==null || _formSheetFillEdit.IsDisposed) {
				return;
			}
			List<Sheet> listSheets=Sheets.GetForTerminal(terminal.PatNum);
			if(terminal.PatNum==0 || terminal.PatNum!=PatNum || listSheets.Count==0 || listSheets.All(x => x.SheetNum!=_formSheetFillEdit.SheetCur.SheetNum)) {
				//patient has been changed or cleared, or there are no forms to fill for the selected patient, force FormSheetFillEdit closed if open
				_formSheetFillEdit.ForceClose();
			}
		}

		///<summary>Used in both modes.  Loads the list of sheets into the listbox.  Then launches the first sheet and goes through the sequence of sheets.
		///If user clicks cancel, the seqeunce will exit.  If NOT IsSimpleMode, then the TerminalManager can also send a signal to immediately terminate
		///the sequence.  If PatNum is 0, this will unload the patient by making the form list not visible and the welcome message visible.</summary>
		private void LoadPatient(bool isRefreshOnly) {
			TerminalActive terminal=null;
			_listSheets=new List<Sheet>();
			Process processCur=Process.GetCurrentProcess();
			if(IsSimpleMode) {
				if(PatNum>0) {
					_listSheets=Sheets.GetForTerminal(PatNum);
				}
			}
			else {//NOT IsSimpleMode
				try{
					terminal=TerminalActives.GetForCmptrSessionAndId(ODEnvironment.MachineName,processCur.SessionId,processCur.Id);
					labelConnection.Visible=false;
				}
				catch(Exception) {//SocketException if db connection gets lost.
					labelConnection.Visible=true;
					return;
				}
				if(terminal==null) {
					//this terminal shouldn't be running, receptionist must've deleted this kiosk, close the form (causes application exit if NOT IsSimpleMode)
					Close();//signal sent in form closing
					return;
				}
				if(terminal.PatNum>0) {
					_listSheets=Sheets.GetForTerminal(terminal.PatNum);
				}
				if(_listSheets.Count==0) {//either terminal.PatNum is 0 or no sheets for pat
					labelWelcome.Visible=true;
					labelForms.Visible=false;
					listForms.Visible=false;
					butDone.Visible=false;
					if(terminal.PatNum>0) {//pat loaded but no sheets to show, unload pat, update db, send signal
						TerminalActives.SetPatNum(terminal.TerminalActiveNum,0);
						Signalods.SetInvalid(InvalidType.Kiosk,KeyType.ProcessId,processCur.Id);
					}
					PatNum=0;
					if(_formSheetFillEdit!=null && !_formSheetFillEdit.IsDisposed) {
						_formSheetFillEdit.ForceClose();
					}
					return;
				}
			}
			//we have a patient loaded who has some sheets to show in the terminal
			labelWelcome.Visible=false;
			labelForms.Visible=true;
			listForms.Visible=true;
			butDone.Visible=true;
			listForms.Items.Clear();
			listForms.Items.AddList(_listSheets,x => x.Description);
			if(!IsSimpleMode) {
				if(PatNum==terminal.PatNum) {
					return;//if the pat was not cleared or replaced just return, if sheets are currently being displayed (loop below), continue displaying them
				}
				//PatNum is changed, set it to the db terminalactive and signal others, then begin displaying sheets (loop below)
				PatNum=terminal.PatNum;
				if(_formSheetFillEdit!=null && !_formSheetFillEdit.IsDisposed) {
					_formSheetFillEdit.ForceClose();
				}
				Signalods.SetInvalid(InvalidType.Kiosk,KeyType.ProcessId,processCur.Id);
			}
			if(!isRefreshOnly) {
				AutoShowSheets();
			}
		}

		private void AutoShowSheets() {
			//only autoshowsheets if IsSimpleMode or the patient is set/changed
			List<long> listSheetNumsFilled=new List<long>();
			long patNumCur=PatNum;
			long sheetNumCur=_listSheets?.FirstOrDefault()?.SheetNum??0;//defaults to 0, if 0 no sheet will display
			while(patNumCur==PatNum && sheetNumCur>0) {//if patient is changed or no more sheets to display, break out of loop
				Sheet sheetCur=Sheets.GetSheet(sheetNumCur);//we want the very freshest copy of the sheet, so we go straight to the database for it
				_formSheetFillEdit=new FormSheetFillEdit(sheetCur);
				_formSheetFillEdit.IsInTerminal=true;
				_formSheetFillEdit.ShowDialog();
				if(_formSheetFillEdit.DialogResult!=DialogResult.OK) {
					_formSheetFillEdit?.Dispose();
					break;//either patient clicked cancel, or NOT IsSimpleMode and a close signal was received, break out of loop
				}
				_formSheetFillEdit?.Dispose();
				listSheetNumsFilled.Add(sheetNumCur);
				sheetNumCur=_listSheets?.FirstOrDefault(x => !listSheetNumsFilled.Contains(x.SheetNum))?.SheetNum??0;//default to 0
			}
		}

		private void listForms_DoubleClick(object sender,EventArgs e) {
			//this might be used after the patient has completed all the forms and wishes to review or alter one of them
			if(listForms.SelectedIndex==-1) {
				return;
			}
			Sheet sheet=Sheets.GetSheet(_listSheets[listForms.SelectedIndex].SheetNum);
			_formSheetFillEdit=new FormSheetFillEdit(sheet);
			_formSheetFillEdit.IsInTerminal=true;
			_formSheetFillEdit.ShowDialog();
			_formSheetFillEdit?.Dispose();
			LoadPatient(true);//this will verify that this patient is still loaded in this kiosk and refresh the list of forms for the patient
		}

		private void butDone_Click(object sender,EventArgs e) {
			Sheets.ClearFromTerminal(PatNum);
			labelForms.Visible=false;
			listForms.Visible=false;
			butDone.Visible=false;
			if(IsSimpleMode) {//not subscribed to signals if IsSimpleMode
				PatNum=0;
				_listSheets=new List<Sheet>();
				labelThankYou.Visible=true;
				return;
			}
			//NOT IsSimpleMode from here down
			TerminalActive terminal;
			Process processCur=Process.GetCurrentProcess();
			try{
				terminal=TerminalActives.GetForCmptrSessionAndId(ODEnvironment.MachineName,processCur.SessionId,processCur.Id);
				labelConnection.Visible=false;
			}
			catch(Exception) {//SocketException if db connection gets lost.
				labelConnection.Visible=true;
				return;
			}
			//this terminal shouldn't be running, receptionist must've deleted this kiosk, close the form (causes application exit if NOT IsSimpleMode).
			if(terminal==null) {
				Close();//signal sent in form closing
				return;
			}
			if(terminal.PatNum!=0) {
				labelWelcome.Visible=true;
				TerminalActives.SetPatNum(terminal.TerminalActiveNum,0);
				PatNum=0;
				_listSheets=new List<Sheet>();
				Signalods.SetInvalid(InvalidType.Kiosk,KeyType.ProcessId,processCur.Id);//signal the terminal manager to refresh its grid
			}
		}

		private void panelClose_Click(object sender,EventArgs e) {
			//It's fairly safe to not have a password, because the program will exit in remote mode, and in simple mode, the patient is usually supervised.
			if(PrefC.GetString(PrefName.TerminalClosePassword)!="") {
				string resultText="";
				int count=0;
				while(resultText!=PrefC.GetString(PrefName.TerminalClosePassword)) {
					if(count>0) {//we already passed through once, so the entered password must have been invalid. 
						MsgBox.Show(this,"Invalid Password");
					}
					using InputBox iBox=new InputBox("Enter password to exit kiosk.");
					iBox.textResult.PasswordChar='*';
					iBox.setTitle(Lan.g(this,"Kiosk Password"));
					iBox.ShowDialog();
					resultText=iBox.textResult.Text;
					count++;
					if(iBox.DialogResult==DialogResult.Cancel) {
						return;
					}
				}
			}
			//if we get here, either no password required or they entered the correct one
			Close();//signal sent in form closing
		}

		private void FormTerminal_FormClosing(object sender,FormClosingEventArgs e) {
			if(IsSimpleMode) {
				return;
			}
			Process processCur=Process.GetCurrentProcess();
			try {
				Sheets.ClearFromTerminal(PatNum);
				TerminalActives.DeleteForCmptrSessionAndId(ODEnvironment.MachineName,processCur.SessionId,processId:processCur.Id);
				//Just in case, close remaining forms that are open
				_formSheetFillEdit?.ForceClose();
			}
			catch(Exception) {//SocketException if db connection gets lost.
				//if either fail, do nothing, the terminalactives will be cleaned up the next time the kiosk mode is enabled for this computer
			}
			finally {
				Signalods.SetInvalid(InvalidType.Kiosk,KeyType.ProcessId,processCur.Id);
			}
		}



	}
}

