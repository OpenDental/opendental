using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using CodeBase;

namespace OpenDental.Bridges {
	/// <summary></summary>
	public class Owandy {
		/// <summary></summary>
		public Owandy() {

		}

		//AAD External Call declaration for Owandy bridge (Start)
		///<summary></summary>
		[DllImport("user32.dll")]
		public static extern IntPtr FindWindow(string lpClassName,string lpWindowName);
		/// <summary></summary>
		[DllImport("user32.dll")]
		public static extern Boolean IsWindow(IntPtr hWnd);
		/// <summary></summary>
		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr hWnd,int Msg,Int32 wParam,String lParam);

		/// <summary></summary>
		//public IntPtr formHandle;
		/// <summary></summary>
		public static IntPtr hwndLink;
		/// <summary></summary>
		public const int WM_SETTEXT = 0x000C;
		// AAD External Call declaration for Owandy bridge (nd)

		//static extern long SendMessage(long hWnd, long Msg, long wParam, string lParam);
		///<summary>Launches the program using command line, then passes some data using Windows API.</summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			List<ProgramProperty> listProgProperties =ProgramProperties.GetForProgram(ProgramCur.ProgramNum);
			//ProgramProperties.GetForProgram();
			string info;
			if(pat != null) {
				try {
					//formHandle = Parent.Handle;
					System.Diagnostics.Process.Start(path,ProgramCur.CommandLine);//"C /LINK "+ formHandle;
					if(!IsWindow(hwndLink)) {
						hwndLink=FindWindow("MjLinkWndClass",null);
					}
					// info = "/P:1,DEMO,Patient1";
					//Patient id:
					string patId="";
					ProgramProperty propertyCur=ProgramProperties.GetCur(listProgProperties, "Enter 0 to use PatientNum, or 1 to use ChartNum");;
					if(propertyCur.PropertyValue=="0"){
						patId=POut.Long(pat.PatNum);
					}
					else {
						patId=POut.String(pat.ChartNumber);
					}
					info = "/P:" + patId + "," + pat.LName + "," + pat.FName;
					if(IsWindow(hwndLink) == true) {
						IntPtr lResp=SendMessage(hwndLink,WM_SETTEXT,0,info);
					}

				}
				catch {
					MessageBox.Show(path + " is not available, or there is an error in the command line options.");
				}
			}//if patient is loaded
			else {
				try {
					ODFileUtils.ProcessStart(path);//should start Owandy without bringing up a pt.
				}
				catch {
					MessageBox.Show(path + " is not available.");
				}
			}
		}

		
	}

}










