using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental.Bridges {
	/// <summary>CADI uses their own OLE format for passing data to CADI. 
	/// See the bridging documents in \\opendental.od\serverfiles\Storage\OPEN DENTAL\Programmers Documents\Bridge Info\CADI for details.</summary>
	public static class CADI {
		///<summary>Keep as a static object so it doesn't get garbage collected.</summary>
		private static CADINativeWindow CADIWindow; 

		public static void SendData(Program ProgramCur,Patient pat) {
			try {
				if(CADIWindow==null) { //Only release this reference once the CADIWindow instance has closed.
					CADIWindow=new CADINativeWindow(new Action(() => CADIWindow=null ));
				}
				CADIWindow.SendData(ProgramCur,pat);
			}
			catch (Exception e) {
				MessageBox.Show(e.Message);
				SecurityLogs.MakeLogEntry(Permissions.ChartModule,ODMethodsT.Coalesce(pat).PatNum,e.Message);
			}
		}
	}

	///<summary>Contains the logic for opening the CADI application, sending OLE commands to it, and listening to messages</summary>
	internal class CADINativeWindow:NativeWindow {
		///<summary>Per bridging documents, this is the programID used to identify CADI</summary>
		private const string PROGRAM_ID="Mediadent.Application";
		///<summary>Per bridging documents, this is the value of the sysuint we expect back when the CADI window is closed by the user</summary>
		private const int OLECLIENTNOTIFY_APPLICATIONEXIT=1;
		///<summary>Per bridging documents, birthdate is specified using COM TDATE, which is calculated as the number of days that have elapsed since 12/30/1899</summary>
		private static readonly DateTime TDATE_START=new DateTime(1899,12,30);
		//Define the windows close message to send back to this window from the CADI application when it closes
		//See https://msdn.microsoft.com/en-us/library/windows/desktop/ms632617(v=vs.85).aspx
		private const uint WM_CLOSE=0x0010;
		///<summary>The comObject used to communicate to CADI</summary>
		private dynamic _comObject;
		///<summary>A handle to this "window" object. Register this handle with CADI to receive window messages (e.g. Window closing etc.);</summary>
		private int _windowHandle;
		///<summary>Event will be raised anytime this instance closes. Alerts owner to release reference to this instance.</summary>
		Action _onClose=null;

		public CADINativeWindow(Action onClose) {
			_onClose=onClose;
			//Set up handle to get back closing events from CADI window
			CreateParams cp=new CreateParams();
			this.CreateHandle(cp);			
		}

		///<summary>Launches the program as a comObject, and passes CADI OLE commands to the program.
		///See https://stackoverflow.com/questions/29787825/how-to-interact-with-another-programs-with-ole-automation-in-c </summary>
		public void SendData(Program ProgramCur,Patient pat) {
			try {
				if(_comObject==null) {
					Type progType=Type.GetTypeFromProgID(PROGRAM_ID);
					_comObject=Activator.CreateInstance(progType);
					//register the handle of this window with CADI to listen for application closing events
					int result=_comObject.OleClientWindowHandle(_windowHandle,OLECLIENTNOTIFY_APPLICATIONEXIT,WM_CLOSE,0,0);
					if(result!=0) { //0=success; 14=Invalid argument; -1=Unspecified error.
						throw new Exception("Unable to communicate with CADI. Verify it is installed and try again.");
					}			
				}
				if(pat==null) {
					return;
				}
				//Set patient info
				_comObject.OleBringToFrontApp();
				_comObject.OleBeginTransaction();
				string id="";
				if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,ProgramProperties.PropertyDescs.PatOrChartNum)=="0") {
					id=pat.PatNum.ToString();
				}
				else {
					id=pat.ChartNumber;
				}
				_comObject.OleSetPatientName(pat.LName+", "+pat.FName);
				_comObject.OleSetPatientDateOfBirth(pat.Birthdate.Subtract(TDATE_START).TotalDays);
				_comObject.OleSetPatientBiologicalAge(pat.Age);
				_comObject.OleSetPatientSex(pat.Gender==PatientGender.Female ? "F" : "M");
				//Send patient images file path
				string imagePath=ProgramProperties.GetPropVal(ProgramCur.ProgramNum,ProgramProperties.PropertyDescs.ImageFolder);
				_comObject.OleLoadPicLib(CodeBase.ODFileUtils.CombinePaths(imagePath,id));
				_comObject.OleEndTransaction();				
			}
			catch(COMException) { //_comObject cannot be referenced except to set to null in this context. Anything else would throw an exception.
				_comObject=null;
				CloseWindow();
				throw new Exception("Unable to access CADI. Verify it is installed and try again.");
			}
			catch (Exception e) { //Not a COMException so it is ok to reference the _comObject.
				bool throwFriendly=_comObject==null;
				CloseWindow();
				if(throwFriendly) {
					throw new Exception("Unable to open CADI. Verify it is installed and try again.");
				}				
				throw e;
			}
		}

		///<summary>Release _comObject and inform the owner window that is can release it's reference to this class instance.</summary>
		private void CloseWindow() {
			ODException.SwallowAnyException(() => { this.ReleaseHandle(); });
			_onClose();
			if(_comObject==null) {
				return;
			}
			ODException.SwallowAnyException(() => { Marshal.ReleaseComObject(_comObject); });
			_comObject=null;
		}

		///<summary>Listen to when the handle changes to keep the variable in sync
		///See https://msdn.microsoft.com/en-us/library/system.windows.forms.nativewindow.handle(v=vs.110).aspx </summary>
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    protected override void OnHandleChange()
    {
        _windowHandle = (int)this.Handle;
    }

		///<summary>Listen for messages being sent back to this handle by CADI. We need to include this to allow the CADI window to close properly.
		///See https://msdn.microsoft.com/en-us/library/system.windows.forms.nativewindow.handle(v=vs.110).aspx </summary>
		[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    protected override void WndProc(ref Message m)
    {
			switch ((uint)m.Msg)
			{
        case WM_CLOSE: //Note, if in demo mode and user clicks 'Bye', this event won't be raised. This will fix itself if they open the bridge again.
					CloseWindow();
				break;
			}
			base.WndProc(ref m);
    }
	}
}