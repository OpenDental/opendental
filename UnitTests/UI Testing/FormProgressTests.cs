using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OpenDental;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;

namespace UnitTests
{
	public partial class FormProgressTests : FormODBase
	{
		public FormProgressTests()		{
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void butOld_Click(object sender, EventArgs e){
			ODProgress.ShowAction(
				DoThings,
				startingMessage:"Doing things...",
				typeEvent:typeof(ODEvent),
				odEventType:ODEventType.ProgressBar,
				actionException:ex =>  {
					FriendlyException.Show("Error doing things.",ex);
				}
			);
		}

		private void butNew_Click(object sender, EventArgs e){
			ProgressOD progressOD=new ProgressOD();
			progressOD.StartingMessage="Preparing";
			progressOD.ActionMain=DoThings;
			//progressOD.TestSleep=true;
			try{
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex){
				MsgBox.Show(ex.Message);
			}
			if(progressOD.IsCancelled){
				MsgBox.Show("Cancelled");
			}
			if(progressOD.IsSuccess){
				MsgBox.Show("Success");
			}
			else{
				MsgBox.Show("Failure");
			}
		}

		private void butHistory_Click(object sender,EventArgs e) {
			ProgressOD progressOD=new ProgressOD();
			//progressOD.ProgStyle=ProgressBarStyle.Blocks;
			progressOD.StartingMessage="Preparing";
			progressOD.ActionMain=DoThings;
			progressOD.HasHistory=true;
			try{
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex){
				FriendlyException.Show("Error doing things.",ex);
			}
		}

		private void butBlocks_Click(object sender,EventArgs e) {
			ProgressOD progressOD=new ProgressOD();
			progressOD.IsBlocks=true;
			progressOD.StartingMessage="Preparing";
			progressOD.ActionMain=DoThings;
			//progressOD.TestSleep=true;
			progressOD.ShowDialogProgress();
		}

		private void butChain_Click(object sender,EventArgs e) {
			ProgressOD progressOD=new ProgressOD();
			progressOD.HasHistory=true;
			progressOD.ForceCloseHistory=true;
			progressOD.ActionMain=() => { 
				ODEvent.Fire(ODEventType.ProgressBar,"Doing first thing");
				Thread.Sleep(1000);
				};
			progressOD.ShowDialogProgress();
			string progressMsg=progressOD.HistoryMsg;
			//some intermittent code here
			Thread.Sleep(1000);
			progressOD=new ProgressOD();
			progressOD.HasHistory=true;
			progressOD.HistoryMsg=progressMsg;
			progressOD.ActionMain=() => { 
				ODEvent.Fire(ODEventType.ProgressBar,"Doing second thing");
				Thread.Sleep(1000);
				};
			//dlg will not close here because HistoryClose is not set true
			progressOD.ShowDialogProgress();			
		}

		private void DoThings(){
			for(int i=0;i<10;i++){
				if(i==3){
					//throw new Exception("Exception msg");
					string str="Doing step "+i.ToString()+" plus extra text";
					ProgressBarHelper progressBarHelper = new ProgressBarHelper(str,
						blockValue: i+1,blockMax: 9);
					//ODEvent.UpdateProgressMsg(progressBarHelper);
					//This next line is an alternative to the above to demonstrate that the old style of ODEvents is still supported.
					ODEvent.Fire(ODEventType.Cache,progressBarHelper);
					//old:
					//ProgressBarEvent.Fire(ODEventType.ProgressBar,progressBarHelper);
				}
				else{
					string str="Doing step "+i.ToString();
					ProgressBarHelper progressBarHelper = new ProgressBarHelper(str,
						blockValue: i+1,blockMax: 9);
					//ODEvent.UpdateProgressMsg(progressBarHelper);
					//This next line is an alternative to the above to demonstrate that the old style of ODEvents is still supported.
					//ODEvent.Fire(ODEventType.Cache,str);
					//the line below was yet another way to do it, but it was never working:
					//ProgressBarEvent.Fire(ODEventType.ProgressBar,str);
					//The line below is a way to fix the line above:
					ODEvent.Fire(ODEventType.ProgressBar,progressBarHelper);
					//old:
					//ProgressBarEvent.Fire(ODEventType.ProgressBar,progressBarHelper);
				}
				Thread.Sleep(1000);
			}
		}

		private void butException_Click(object sender, EventArgs e){
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=()=> throw new Exception("error") ;
			progressOD.TestSleep=true;
			//try{
				progressOD.ShowDialogProgress();
			//}
			//catch{

			//}
			//result: thread and dialog are closed and exception is thrown.
		}

		private void butInnerException_Click(object sender,EventArgs e) {
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=()=> {
				//Type typeFormProgressTests=typeof(FormProgressTests);
				MethodInfo methodInfo=typeof(FormProgressTests).GetMethod("MethodForInnerException");
				methodInfo.Invoke(null,null);
			};
			//progressOD.TestSleep=true;
			progressOD.ShowDialogProgress();
		}

		public static void MethodForInnerException(){
			//do stuff
			throw new Exception("This should show inner.");
		}

		private void butHideCancel_Click(object sender,EventArgs e) {
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => {
				Thread.Sleep(8000);
			};
			progressOD.ShowCancelButton=false;
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
		}

		
	}
}
