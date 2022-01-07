using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormProcTools : FormODBase {
		public bool Changed;
		///<summary>The actual list of ADA codes as published by the ADA.  Only available on our compiled releases.  There is no other way to get this info.  For Canada, list will get filled on Run click by downloading code list from our website.</summary>
		private List<ProcedureCode> _codeList;

		///<summary></summary>
		public FormProcTools()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		protected override string GetHelpOverride() {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return "FormProcToolsCanada";
			}
			return "FormProcTools";
		}

		private void FormProcTools_Load(object sender,EventArgs e) {
			if(ODBuild.IsTrial()) {
				checkTcodes.Checked=false;
				checkNcodes.Checked=false;
				checkDcodes.Checked=false;
				checkAutocodes.Checked=false;
				checkProcButtons.Checked=false;
				checkApptProcsQuickAdd.Checked=false;
				checkTcodes.Enabled=false;
				//checkNcodes.Enabled=false;
				checkDcodes.Enabled=false;
				checkAutocodes.Enabled=false;
				checkProcButtons.Enabled=false;
				if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {
					checkRecallTypes.Enabled=false;
					checkApptProcsQuickAdd.Enabled=false;
				}
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				//Tcodes remain enabled
				//Ncodes remain enabled
				checkDcodes.Text="CDA codes - Add any missing 2019 CDA codes.  This option does not work in the trial version.";
				checkRecallTypes.Text="Recall Types - Resets the recall types and triggers to default.  Replaces any T codes with CDA codes.";
				_codeList=null;//Is only filled when the code tool runs because the user might not need to download the codes.
			}
			else { //USA
				_codeList=CDT.Class1.GetADAcodes();
				//If this is not the full USA release version, then disable the D-code import because the CDT codes will not be available.
				if(_codeList==null || _codeList.Count==0) {
					checkDcodes.Checked=false;
					checkDcodes.Enabled=false;
				}
			}
		}

		private void butUncheck_Click(object sender,EventArgs e) {
			checkTcodes.Checked=false;
			checkNcodes.Checked=false;
			checkDcodes.Checked=false;
			checkAutocodes.Checked=false;
			checkProcButtons.Checked=false;
			checkApptProcsQuickAdd.Checked=false;
			checkRecallTypes.Checked=false;
		}

		///<summary>Downloads Canadian procedure codes from our website and updates _codeList accordingly.</summary>
		private void CanadaDownloadProcedureCodes() {
			Cursor=Cursors.WaitCursor;
			_codeList=new List<ProcedureCode>();
			string url=@"http://www.opendental.com/feescanada/procedurecodes.txt";
			string tempFile=PrefC.GetRandomTempFile(".tmp");
			WebClient myWebClient=new WebClient();
			try {
				myWebClient.DownloadFile(url,tempFile);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Failed to download procedure codes")+":\r\n"+ex.Message);
				Cursor=Cursors.Default;
				return;
			}
			string codeData=File.ReadAllText(tempFile);
			File.Delete(tempFile);
			string[] codeLines=codeData.Split('\n');
			for(int i=0;i<codeLines.Length;i++) {
				string[] fields=codeLines[i].Split('\t');
				if(fields.Length<1) {//Skip blank lines if they exist.
					continue;
				}
				ProcedureCode procCode=new ProcedureCode();
				procCode.ProcCode=PIn.String(fields[0]);//0 ProcCode
				procCode.Descript=PIn.String(fields[1]);//1 Description
				procCode.TreatArea=(TreatmentArea)PIn.Int(fields[2]);//2 TreatArea
				procCode.NoBillIns=PIn.Bool(fields[3]);//3 NoBillIns
				procCode.IsProsth=PIn.Bool(fields[4]);//4 IsProsth
				procCode.IsHygiene=PIn.Bool(fields[5]);//5 IsHygiene
				procCode.PaintType=(ToothPaintingType)PIn.Int(fields[6]);//6 PaintType
				procCode.ProcCatDescript=PIn.String(fields[7]);//7 ProcCatDescript
				procCode.ProcTime=PIn.String(fields[8]);//8 ProcTime
				procCode.AbbrDesc=PIn.String(fields[9]);//9 AbbrDesc
				procCode.CanadaTimeUnits=PIn.Double(fields[10]);//10 CanadaTimeUnits
				_codeList.Add(procCode);
			}
			Cursor=Cursors.Default;
		}

		private void butRun_Click(object sender,EventArgs e) {
			//The updating of CDT codes takes place towards the end of the year, while we typically do it in December, we have 
			//done it as early as Novemeber before. This warning will inform users that using the new codes will cause rejection
			//on their claims if they try to use them before the first of the new year.
			DateTime datePromptStart=new DateTime(2021,10,28);
			DateTime datePromptEnd=new DateTime(datePromptStart.Year,12,31);
			if(DateTime.Now.Between(datePromptStart,datePromptEnd) && checkDcodes.Checked) {//Only validate if attempting to update D Codes
				if(MessageBox.Show(//Still between datePromptStart and the first of the next year, prompt that these codes may cause problems.
						Lan.g(this,"Updating D Codes at this time could result in acquiring codes which are not valid until ")
						+datePromptEnd.AddDays(1).ToShortDateString()+Lan.g(this,". Using these codes could cause claims to be rejected, continue?")
						,Lan.g(this,"D Codes"),MessageBoxButtons.YesNo)==DialogResult.No) 
				{
					return;//Early return if the user is between datePromptStart and the first of the next year and they've said no to updating D Codes.
				}
			}
			if(!checkTcodes.Checked && !checkNcodes.Checked && !checkDcodes.Checked && !checkAutocodes.Checked 
				&& !checkProcButtons.Checked && !checkApptProcsQuickAdd.Checked && !checkRecallTypes.Checked)
			{
				MsgBox.Show(this,"Please select at least one tool first.");
				return;
			}
			Changed=false;
			int rowsInserted=0;
			#region N Codes
			if(checkNcodes.Checked) {
				try {
					rowsInserted+=FormProcCodes.ImportProcCodes("",null,Properties.Resources.NoFeeProcCodes);
				}
				catch(ApplicationException ex) {
					MessageBox.Show(ex.Message);
				}
				Changed=true;
				DataValid.SetInvalid(InvalidType.Defs, InvalidType.ProcCodes);
			}
			#endregion
			#region D Codes
			if(checkDcodes.Checked) {
				try {
					if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
						if(_codeList==null) {
							CanadaDownloadProcedureCodes();
						}
					}
					rowsInserted+=FormProcCodes.ImportProcCodes("",_codeList,"");
					Changed=true;
					int procCodesFixed=ProcedureCodes.ResetADAdescriptionsAndAbbrs();
					MessageBox.Show(Lan.g(this,"Procedures codes with descriptions or abbreviations updated:")+" "+procCodesFixed.ToString());
				}
				catch(ApplicationException ex) {
					MessageBox.Show(ex.Message);
				}
				DataValid.SetInvalid(InvalidType.Defs, InvalidType.ProcCodes);
			}
			#endregion
			if(checkNcodes.Checked || checkDcodes.Checked){
				MessageBox.Show("Procedure codes inserted: "+rowsInserted);
			}
			#region Auto Codes
			if(checkAutocodes.Checked) {
				//checking for any AutoCodes and prompting the user if they exist
				if(AutoCodes.GetCount() > 0) {
					string msgText=Lan.g(this,"This tool will delete all current autocodes and then add in the default autocodes.")+"\r\n";
					//If the proc tool isn't going to put the procedure buttons back to default, warn them that they will need to reassociate them.
					if(!checkProcButtons.Checked) {
						msgText+=Lan.g(this,"Any procedure buttons associated with the current autocodes will be dissociated and will need to be reassociated manually.")+"\r\n";
					}
					msgText+=Lan.g(this,"Continue?");
					if(MsgBox.Show(this,MsgBoxButtons.YesNo,msgText)) {
						AutoCodes.SetToDefault();
						Changed=true;
						DataValid.SetInvalid(InvalidType.AutoCodes);
					}
					else {
						checkAutocodes.Checked=false; //if the user hits no on the popup, uncheck and continue 
					}
				}
				//If there are no autocodes then add the defaults
				else {
					AutoCodes.SetToDefault();
					Changed=true;
					DataValid.SetInvalid(InvalidType.AutoCodes);
				}
			}
			#endregion
			#region Proc Buttons
			if(checkProcButtons.Checked) {
					if(MsgBox.Show(this,MsgBoxButtons.YesNo,"This tool will delete all current ProcButtons and Quick Buttons from the Chart Module and add in the defaults. Continue?")) {
						ProcButtons.SetToDefault();
						ProcButtonQuicks.SetToDefault();
						Changed=true;
						DataValid.SetInvalid(InvalidType.ProcButtons,InvalidType.Defs);
					}
					else {
						checkProcButtons.Checked=false;//continue and uncheck if user hits no on the popup
					}
			}
			#endregion
			#region Appt Procs Quick Add
			if(checkApptProcsQuickAdd.Checked) {
				//checking for any ApptProcsQuickAdd and prompting the user if they exist
				if(Defs.GetDefsForCategory(DefCat.ApptProcsQuickAdd).Count>0) {
					if(MsgBox.Show(this,MsgBoxButtons.YesNo,"This tool will reset the list of procedures in the appointment edit window to the defaults. Continue?")) {
						if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
							ProcedureCodes.ResetApptProcsQuickAddCA();
						}
						else {//USA
							ProcedureCodes.ResetApptProcsQuickAdd();
						}
						Changed=true;
						DataValid.SetInvalid(InvalidType.Defs);
					}
					else {
						checkApptProcsQuickAdd.Checked=false;//uncheck and continue if no is selected on the popup
					}
				}
				//run normally if no customizations are found
				else {
					if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
						ProcedureCodes.ResetApptProcsQuickAddCA();
					}
					else {//USA
						ProcedureCodes.ResetApptProcsQuickAdd();
					}
					Changed=true;
					DataValid.SetInvalid(InvalidType.Defs);
				}
			}
			#endregion
			#region Recall Types
			if(checkRecallTypes.Checked) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This will delete all patient recalls for recall types which were manually added, and resets recall types that were modified. Continue?")) 
				{  
					checkRecallTypes.Checked=false;
				}
				else {
					if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
						RecallTypes.SetToDefaultCA();
					}
					else {//USA
						RecallTypes.SetToDefault();
					}
					Changed=true;
					DataValid.SetInvalid(InvalidType.RecallTypes,InvalidType.Prefs);				
					SecurityLogs.MakeLogEntry(Permissions.RecallEdit,0,"Recall types set to default.");
				}
			}
			#endregion
			#region T Codes
			if(checkTcodes.Checked){//Even though this is first in the interface, we need to run it last, since other regions need the T codes above.
				ProcedureCodes.TcodesClear();
				Changed=true;
				//yes, this really does refresh before moving on.
				DataValid.SetInvalid(InvalidType.Defs, InvalidType.ProcCodes);
				SecurityLogs.MakeLogEntry(Permissions.ProcCodeEdit,0,"T-Codes deleted.");
			}
			#endregion
			if(Changed) {
				MessageBox.Show(Lan.g(this,"Done."));
				SecurityLogs.MakeLogEntry(Permissions.Setup,0,"New Customer Procedure codes tool was run.");
			}
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}
	}
}





















