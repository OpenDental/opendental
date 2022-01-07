#if !DISABLE_WINDOWS_BRIDGES
using System.Runtime.InteropServices;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental.Bridges{
	///<summary>Provides bridging functionality to Schick CDR.
	///The algorithm provided below was inspired by the Schick example project named CSAutoApp_2008.</summary>
	public class Schick{

		private static CDRDicom.Application _cdrApp=null;
		private static CDRDicom.ExamDocument _exam=null;

		public Schick() {
		}

		///<summary>Launches the main Patient Document window of Schick.</summary>
		public static void SendData(Program ProgramCur,Patient pat){
			if(pat==null){
				return;
			}
			List<ProgramProperty> ForProgram=ProgramProperties.GetForProgram(ProgramCur.ProgramNum);
			ProgramProperty PPCur=ProgramProperties.GetCur(ForProgram,"Enter 0 to use PatientNum, or 1 to use ChartNum");
			string patID="";
			if(PPCur.PropertyValue=="0") {
				patID=pat.PatNum.ToString();
			}
			else {
				patID=pat.ChartNumber;
			}
			try {
				string version4or5=ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Schick Version 4 or 5");
				if(version4or5=="5") {
					ShowExam(pat,patID);
				}
				else {
					VBbridges.Schick3.Launch(patID,pat.LName,pat.FName);
				}
			}
			catch {
				MessageBox.Show("Error launching Schick CDR Dicom.");
			}
		}

		private static void ShowExam(Patient pat,string patID) {
			//If the CDR application is not running yet, then getting a new instance will start the CDR application.
			//If the CDR application is already running, then getting a new instance will simply reference the existing CDR application.
			//Thus there can never be more than one CDR application running.
			_cdrApp=new CDRDicom.Application();
			LoadExamWindow();
			if(!_exam.LoadPatient(pat.LName,pat.FName,patID)) {//Attempt to locate an existing patient in the Schick database.				
				//If the user clicks accross with a new patient when an existing patient is already loaded into Schick, then there are only two options:
				//1) Reuse the same CDRDicom.Application() object and pass "true" into exam.NewExam() below.  The problem with this option is that
				//		the user must set the create patient checkbox, otherwise the exam is created for the previous patient, which is terrifying.
				//2) Quit the application, then create a new exam with the patient create checkbox hidden.  In this specific scenario, Schick will create
				//	the patient even though the create patient checkbox is neither showing nor allowed, because there is no previous patient currently loaded.
				//We chose option #2, because it more closely follows our typical bridge patterns.
				//Quit() causes OnExamClosed() to fire, which sets cdrApp to null so it will be reinitialized in LoadCdrApp().  Var cdrApp is still valid.
				_cdrApp.Quit();
				LoadExamWindow();
				CreateExam(pat,patID);
			}
		}

		///<summary>Initializes Schick and loads an empty exam.</summary>
		private static void LoadExamWindow() {
			if(_exam==null) {
				_exam=(CDRDicom.ExamDocument)_cdrApp.CreateExamDocument();
				_exam.OnClose+=new CDRDicom.IExamDocumentEvents_OnCloseEventHandler(OnExamClosed);
			}
			_exam.Visible=true;
			SetForegroundWindow(_exam.hWnd);
		}

		///<summary>When the user clicks OK from the exam window, the patient will be created.  The user can also cancel.</summary>
		private static void CreateExam(Patient pat,string patID) {
			CDRDATALib.ICDRPatient patient=(CDRDATALib.ICDRPatient)_exam.Patient;
			if(patient!=null) {
				//Copy patient info to the new exam
				patient.LastName=pat.LName;
				patient.FirstName=pat.FName;
				patient.IDNumber=patID;
				//Then display the new exam window.
				_exam.NewExam(false);
			}
		}

		private static void OnExamClosed() {
			_exam=null;
		}

		[DllImport("User32.dll")]
		public static extern bool SetForegroundWindow(int hndRef);

	}
}
#endif