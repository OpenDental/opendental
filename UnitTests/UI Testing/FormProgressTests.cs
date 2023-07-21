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
				typeEvent:typeof(ProgressBarEvent),
				odEventType:ODEventType.ProgressBar,
				actionException:ex =>  {
					FriendlyException.Show("Error doing things.",ex);
				}
			);
		}

		private void butNew_Click(object sender, EventArgs e){
			ProgressOD progressOD=new ProgressOD();
			//progressOD.ProgStyle=ProgressBarStyle.Blocks;
			progressOD.StartingMessage="Preparing";
			progressOD.ActionMain=DoThings;
			progressOD.TestSleep=true;
			progressOD.ShowDialogProgress();
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

		private void butChain_Click(object sender,EventArgs e) {
			ProgressOD progressOD=new ProgressOD();
			progressOD.HasHistory=true;
			progressOD.HistoryClose=true;
			progressOD.ActionMain=() => { 
				ProgressBarEvent.Fire(ODEventType.ProgressBar,"Doing first thing");
				//do something
				};
			progressOD.ShowDialogProgress();
			string progressMsg=progressOD.HistoryMsg;
			//some intermittent code here
			progressOD=new ProgressOD();
			progressOD.HasHistory=true;
			progressOD.HistoryMsg=progressMsg;
			progressOD.ActionMain=() => { 
				ProgressBarEvent.Fire(ODEventType.ProgressBar,"Doing second thing");
				//do something
				};
			//dlg will not close here because HistoryClose is not set true
			progressOD.ShowDialogProgress();			
		}

		private void DoThings(){
			for(int i=0;i<10;i++){
				if(i==3){
					ProgressBarHelper progressBarHelper = new ProgressBarHelper("Doing step "+i.ToString()+" plus extra text",
						blockValue: i+1,blockMax: 9);
					ProgressBarEvent.Fire(ODEventType.ProgressBar,progressBarHelper);
				}
				else{
					ProgressBarHelper progressBarHelper = new ProgressBarHelper("Doing step "+i.ToString(),
						blockValue: i+1,blockMax: 9);
					ProgressBarEvent.Fire(ODEventType.ProgressBar,progressBarHelper);
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
