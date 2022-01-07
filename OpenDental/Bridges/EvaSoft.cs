using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental {
	class EvaSoft {

		public EvaSoft() {
		}

		public static void SendData(Program ProgramCur,Patient pat) {
			if(pat==null) {
				MsgBox.Show("EvaSoft","You must select a patient first.");
				return;
			}
			Process[] evaSoftInstances=Process.GetProcessesByName("EvaSoft");
			if(evaSoftInstances.Length==0) {
				MsgBox.Show("EvaSoft","EvaSoft is not running. EvaSoft must be running before the bridge will work.");
				return;
			}
			List<ProgramProperty> ForProgram =ProgramProperties.GetForProgram(ProgramCur.ProgramNum);
			UdpClient udpClient=new UdpClient();
			string udpMessage="REQUEST01123581321~~~0.1b~~~pmaddpatient~~~";
			//Patient id can be any string format
			ProgramProperty PPCur=ProgramProperties.GetCur(ForProgram,"Enter 0 to use PatientNum, or 1 to use ChartNum");
			if(PPCur.PropertyValue=="0") {
				udpMessage+=pat.PatNum.ToString();
			}
			else {
				udpMessage+=pat.ChartNumber.Replace(",","").Trim();//Remove any commas. Not likely to exist, but just to be safe.
			}
			udpMessage+=","+pat.FName.Replace(",","").Trim();//Remove commas from data, because they are the separator.
			udpMessage+=","+pat.LName.Replace(",","").Trim();//Remove commas from data, because they are the separator.
			udpMessage+=","+pat.Birthdate.ToString("MM/dd/yyyy");
			udpMessage+=","+((pat.Gender==PatientGender.Female)?"female":"male");
			udpMessage+=","+(pat.Address+" "+pat.Address2).Replace(",","").Trim();//Remove commas from data, because they are the separator.
			udpMessage+=","+pat.City.Replace(",","").Trim();//Remove commas from data, because they are the separator.
			udpMessage+=","+pat.State.Replace(",","").Trim();//Remove commas from data, because they are the separator.
			udpMessage+=","+pat.Zip.Replace(",","").Trim();//Remove commas from data, because they are the separator.
			byte[] udpMessageBytes=Encoding.ASCII.GetBytes(udpMessage);
			udpClient.Send(udpMessageBytes,udpMessageBytes.Length,"localhost",35678);
		}

	}
}
