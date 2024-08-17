#if !DISABLE_WINDOWS_BRIDGES
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using OpenDentBusiness;
using CodeBase;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace OpenDental.Bridges{
	/// <summary></summary>
	public class DrCeph{
		
		/// <summary></summary>
		public DrCeph(){
			
		}

		///<summary>Uses a VB dll to launch.</summary>
		public static void SendData(Program ProgramCur, Patient pat){
			string path=Programs.GetProgramPath(ProgramCur);
			if(pat==null){
				MessageBox.Show("Please select a patient first.");
				return;
			}
			//Make sure the program is running
			if(ODBuild.IsWeb()) {
				try {
					ODFileUtils.ProcessStart(path,tryLaunch:true);
				}
				catch(Exception ex) {
					FriendlyException.Show(Lans.g("DrCeph","An error occurred when checking for the Dr.Ceph bridge"),ex);
					return;
				}
			}
			else if(Process.GetProcessesByName("DrCeph").Length==0) {
				try{
					ODFileUtils.ProcessStart(path);
				}
				catch{
					MsgBox.Show("DrCeph","Program path not set properly.");
					return;
				}
				Thread.Sleep(TimeSpan.FromSeconds(4));
			}
			List<ProgramProperty> listProgProps=ProgramProperties.GetForProgram(ProgramCur.ProgramNum);;
			try{
				using FormDrCeph form=new FormDrCeph(pat,listProgProps);
				form.ShowDialog();
				if(form.DialogResult==DialogResult.Cancel) {
					return;
				}
				DrCephArgs cephArgs=form.Args;
				if(ODBuild.IsWeb()) {
					string patArgs=JsonConvert.SerializeObject(cephArgs);
					ODCloudClient.SendToDrCeph(patArgs);
				}
				else {
					DrCephUtils.Launch(cephArgs);
				}
			}
			catch(Exception ex) {
				MessageBox.Show("An error occurred. "+ex.Message);
			}
		}

		

	}
}
#endif