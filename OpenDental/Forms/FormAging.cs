using System;
using System.Drawing;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental{
	///<summary></summary>
	public partial class FormAging : FormODBase {

		///<summary></summary>
		public FormAging(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			Lan.C(this,new Control[]
				{this.textBox1});
		}

		private void FormAging_Load(object sender, System.EventArgs e) {
			DateTime dateLastAging=PrefC.GetDate(PrefName.DateLastAging);
			if(dateLastAging.Year<1880){
				textDateLast.Text="";
			}
			else{
				textDateLast.Text=dateLastAging.ToShortDateString();
			}
			if(PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily)){
				if(dateLastAging < DateTime.Today.AddDays(-15)) {
					textDateCalc.Text=dateLastAging.AddMonths(1).ToShortDateString();
				}
				else {
					textDateCalc.Text=dateLastAging.ToShortDateString();
				}
			}
			else{
				textDateCalc.Text=DateTime.Today.ToShortDateString();
				if(PrefC.GetBool(PrefName.AgingIsEnterprise)) {//enterprise aging requires daily not monthly calc
					textDateCalc.ReadOnly=true;
					textDateCalc.BackColor=SystemColors.Control;
				}
			}
		}

		private bool RunAgingEnterprise(DateTime dateCalc) {
			DateTime dateLastAging=PrefC.GetDate(PrefName.DateLastAging);
			if(dateLastAging.Date==dateCalc.Date) {
				if(MessageBox.Show(this,Lan.g(this,"Aging has already been calculated for")+" "+dateCalc.ToShortDateString()+" "
					+Lan.g(this,"and does not normally need to run more than once per day.\r\n\r\nRun anyway?"),"",MessageBoxButtons.YesNo)!=DialogResult.Yes)
				{
					return false;
				}
			}
			//Refresh prefs because AgingBeginDateTime is very time sensitive
			Prefs.RefreshCache();
			DateTime dateTAgingBeganPref=PrefC.GetDateT(PrefName.AgingBeginDateTime);
			if(dateTAgingBeganPref>DateTime.MinValue) {
				MessageBox.Show(this,Lan.g(this,"You cannot run aging until it has finished the current calculations which began on")+" "
					+dateTAgingBeganPref.ToString()+".\r\n"+Lans.g(this,"If you believe the current aging process has finished, a user with SecurityAdmin permission "
					+"can manually clear the date and time by going to Setup | Miscellaneous and pressing the 'Clear' button."));
				return false;
			}
			SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Starting Aging - Aging Form");
			Prefs.UpdateString(PrefName.AgingBeginDateTime,POut.DateT(MiscData.GetNowDateTime(),false));//get lock on pref to block others
			Signalods.SetInvalid(InvalidType.Prefs);//signal a cache refresh so other computers will have the updated pref as quickly as possible
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => { 
				Ledgers.ComputeAging(0,dateCalc);
				Prefs.UpdateString(PrefName.DateLastAging,POut.Date(dateCalc,false));
			};
			progressOD.StartingMessage=Lan.g(this,"Calculating enterprise aging for all patients as of")+" "+dateCalc.ToShortDateString()+"...";
			progressOD.TestSleep=true;
			try{
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex){
				Ledgers.AgingExceptionHandler(ex,this);
			}
			Prefs.UpdateString(PrefName.AgingBeginDateTime,"");//clear lock on pref whether aging was successful or not
			if(progressOD.IsCancelled) {
				return false;
			}
			SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Aging complete - Aging Form");
			Signalods.SetInvalid(InvalidType.Prefs);
			return progressOD.IsSuccess;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDateCalc.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			DateTime dateCalc=PIn.Date(textDateCalc.Text);
			if(PrefC.GetBool(PrefName.AgingIsEnterprise)) {
				//if this is true, dateCalc has to be DateTime.Today and aging calculated daily not monthly.
				if(!RunAgingEnterprise(dateCalc)) {
					//this will happen if exception or if user clicked Cancel.
					//Errors displayed from RunAgingEnterprise
					return;
				}
			}
			else {
				SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Starting Aging - Aging Form");
				ProgressOD progressOD=new ProgressOD();
				progressOD.ActionMain=() => Ledgers.ComputeAging(0,dateCalc);
				progressOD.StartingMessage=Lan.g(this,"Calculating aging for all patients as of")+" "+dateCalc.ToShortDateString()+"...";
				progressOD.TestSleep=true;
				try{
					progressOD.ShowDialogProgress();
				}
				catch(Exception ex){
					Ledgers.AgingExceptionHandler(ex,this);
				}
				if(progressOD.IsCancelled) {
					return;
				}
				SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Aging complete - Aging Form");
				if(!progressOD.IsSuccess) {
					//error was already shown
					return;//stay in this window.  User can try again or click Cancel.
				}
				if(Prefs.UpdateString(PrefName.DateLastAging,POut.Date(dateCalc,false))) {
					DataValid.SetInvalid(InvalidType.Prefs);
				}
			}
			MsgBox.Show(this,"Aging Complete");
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, EventArgs e){
			DialogResult=DialogResult.Cancel;
		}





	}
}
