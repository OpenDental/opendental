using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormNewCropBilling:FormODBase {

		///<summary>Holds a cached list of all eRx repeating charges in the entire database.
		///Even 10s of thousands of records would only take about 1MB of memory.
		///At the time this caching was added (04/21/2016), there were only ~500 records.</summary>
		private List<RepeatCharge> _listRepeatChargesErx=null;
		///<summary>Holds all relevant charges reported from NewCrop XML.  Corresponds 1-1 with what shows in the grid.</summary>
		private List<NewCropCharge> _listNewCropCharges=null;
		///<summary>Holds all NewCrop repeating charges that must be added</summary>
		private List<NewCropCharge> _listNewCropChargesToAdd=new List<NewCropCharge>();
		private DateTime _dateBillingMonthYear=DateTime.MinValue;

		public FormNewCropBilling() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormBillingList_Resize(object sender,EventArgs e) {
			RefreshGridColumns();
		}

		private void butBrowse_Click(object sender,EventArgs e) {
			if(openFileDialog1.ShowDialog()==DialogResult.OK) {
				textBillingFilePath.Text=openFileDialog1.FileName;
			}
			openFileDialog1.Dispose();
		}

		private void textBillingFilePath_TextChanged(object sender,EventArgs e) {
			string fileName=Path.GetFileName(textBillingFilePath.Text);
			if(Regex.IsMatch(fileName,"^[0-9]{4}_[0-9]{2}.*")) {
				textBillingYearMonth.Text=fileName.Substring(0,7).Replace("_","");
			}
			if(Regex.IsMatch(fileName,"^[0-9]{4}[0-9]{2}.*")) {
				textBillingYearMonth.Text=fileName.Substring(0,6);
			}
		}

		private void butLoad_Click(object sender,EventArgs e) {
			if(!textBillingFilePath.Text.ToLower().EndsWith(".csv")) {
				MessageBox.Show("Billing file must be a comma separated values (csv) file.");
				return;
			}
			if(!File.Exists(textBillingFilePath.Text)) {
				MessageBox.Show("Billing file does not exist or could not be accessed. Make sure the file is not open in another program and try again.");
				return;
			}
			if(!Regex.IsMatch(textBillingYearMonth.Text,"^[0-9]{6}.*")) {
				MessageBox.Show("Invalid billing year or month.");
				return;
			}
			try {
				int year=PIn.Int(textBillingYearMonth.Text.Substring(0,4));
				int month=PIn.Int(textBillingYearMonth.Text.Substring(4,2));
				_dateBillingMonthYear=new DateTime(year,month,1);
			}
			catch {
				MessageBox.Show("Invalid billing year or month.");
				return;
			}
			_listNewCropChargesToAdd=new List<NewCropCharge>();
			FillGrid(isLoading:true);
		}

		private void CreateChargeList(bool isLoading) {
			#region Parse CSV
			_listNewCropCharges=new List<NewCropCharge>();
			string csvData=File.ReadAllText(textBillingFilePath.Text);
			string[] stringArrayCsvLines=csvData.Split(new string[] { "\r\n" },StringSplitOptions.None);			
			for(int i=2;i<stringArrayCsvLines.Length;i++) {//Skip the first row (the column names), skip the second row (the totals).
				string[] stringArrayLineValues=stringArrayCsvLines[i].Split('\t');
				if(stringArrayLineValues.Length < 25) {
					continue;//This often happens on the last line of the file, due to a trailing newline.  Skip this line or any other malformed line.
				}
				for(int j=0;j<stringArrayLineValues.Length;j++) {
					stringArrayLineValues[j]=stringArrayLineValues[j].Trim('\"');//Remove leading and trailing double quotes from each cell of the csv table.
				}
				NewCropCharge newCropCharge=new NewCropCharge();
				newCropCharge.ParentAccount=stringArrayLineValues[0];
				newCropCharge.AccountName=stringArrayLineValues[1];
				newCropCharge.AccountId=stringArrayLineValues[2];
				newCropCharge.SiteId=stringArrayLineValues[3];
				newCropCharge.LastName=stringArrayLineValues[4];
				newCropCharge.FirstName=stringArrayLineValues[5];
				newCropCharge.Dea=stringArrayLineValues[6];
				newCropCharge.NPI=stringArrayLineValues[7];
				newCropCharge.DateAdded=stringArrayLineValues[8];
				newCropCharge.DoctorID=stringArrayLineValues[9];
				newCropCharge.DoctorType=stringArrayLineValues[10];
				newCropCharge.FTEPercent=stringArrayLineValues[11];
				newCropCharge.CompFTEPercent=stringArrayLineValues[12];
				newCropCharge.BasicFTEPercent=stringArrayLineValues[13];
				newCropCharge.DrugChecks=stringArrayLineValues[14];
				newCropCharge.ForumaryChecks=stringArrayLineValues[15];
				newCropCharge.EPCS=stringArrayLineValues[16];
				newCropCharge.IDP=stringArrayLineValues[17];
				newCropCharge.Interop=stringArrayLineValues[18];
				newCropCharge.PatientPortal=stringArrayLineValues[19];
				newCropCharge.Registries=stringArrayLineValues[20];
				newCropCharge.Labs=stringArrayLineValues[21];
				newCropCharge.Genomics=stringArrayLineValues[22];
				newCropCharge.Direct=stringArrayLineValues[23];
				newCropCharge.DoctorDirect=stringArrayLineValues[24];				
				int patNumLength=newCropCharge.AccountId.IndexOf("-");
				string patNumStr=PIn.String(newCropCharge.AccountId.Substring(0,patNumLength));
				newCropCharge.PatNumForRegKey=PIn.Long(patNumStr);//PatNum of registration key used to create the account id.
				if(newCropCharge.PatNumForRegKey==6566) {
					//Account 6566 corresponds to our software key in the training database.  These accounts are test accounts.
					continue;//Do not show OD test accounts.
				}				
				_listNewCropCharges.Add(newCropCharge);
			}
			#endregion Parse CSV
			_listRepeatChargesErx=RepeatCharges.GetForErx();
			for(int i = 0;i<_listNewCropCharges.Count;i++) {
				_listNewCropCharges[i].repeatCharge=_listRepeatChargesErx.FirstOrDefault(x => x.ErxAccountId==_listNewCropCharges[i].AccountId 
					&& x.Npi==_listNewCropCharges[i].NPI
				);
				if(_listNewCropCharges[i].repeatCharge is null && isLoading) {
					_listNewCropChargesToAdd.Add(_listNewCropCharges[i]);
				}
			}
			
				
			
		}

		private void RefreshGridColumns() {
			gridBillingList.BeginUpdate();
			gridBillingList.Columns.Clear();
			int gridWidth=this.Width-50;
			int erxAccountIdWidth=80;//fixed width
			int patNumWidth=50;//fixed width
			int npiWidth=74;//fixed width
			int isNewWidth=28;//fixed width
			int firstLastNameWidth=160;//fixed width
			int deaWidth=72;//fixed width
			int dateAddedWidth=68;//fixed width
			int compFteWidth=40;//fixed width
			int basicFteWidth=40;//fixed width
			int drugWidth=36;//fixed width
			int formWidth=36;//fixed width
			int epcsWidth=40;//fixed width
			int idpWidth=68;//fixed width
			int variableWidth=gridWidth-isNewWidth-erxAccountIdWidth-patNumWidth-npiWidth-firstLastNameWidth-deaWidth-dateAddedWidth-compFteWidth
				-basicFteWidth-drugWidth-formWidth-epcsWidth-idpWidth;
			int practiceTitleWidth=variableWidth;//variable width
			gridBillingList.Columns.Add(new GridColumn("New",isNewWidth,HorizontalAlignment.Center));//0
			gridBillingList.Columns.Add(new GridColumn("ErxAccountId",erxAccountIdWidth,HorizontalAlignment.Center));//1
			gridBillingList.Columns.Add(new GridColumn("PatNum",patNumWidth,HorizontalAlignment.Center));//2
			gridBillingList.Columns.Add(new GridColumn("NPI",npiWidth,HorizontalAlignment.Center));//3
			gridBillingList.Columns.Add(new GridColumn("FirstLastName",firstLastNameWidth,HorizontalAlignment.Left));//4
			gridBillingList.Columns.Add(new GridColumn("DEA",deaWidth,HorizontalAlignment.Center));//5
			gridBillingList.Columns.Add(new GridColumn("DateAdded",dateAddedWidth,HorizontalAlignment.Center));//6
			gridBillingList.Columns.Add(new GridColumn("cFTE",compFteWidth,HorizontalAlignment.Center));//7
			gridBillingList.Columns.Add(new GridColumn("bFTE",basicFteWidth,HorizontalAlignment.Center));//8
			gridBillingList.Columns.Add(new GridColumn("Drug",drugWidth,HorizontalAlignment.Center));//9
			gridBillingList.Columns.Add(new GridColumn("Form",formWidth,HorizontalAlignment.Center));//10
			gridBillingList.Columns.Add(new GridColumn("EPCS",epcsWidth,HorizontalAlignment.Center));//11
			gridBillingList.Columns.Add(new GridColumn("IDP",idpWidth,HorizontalAlignment.Center));//12
			gridBillingList.Columns.Add(new GridColumn("PracticeTitle",practiceTitleWidth,HorizontalAlignment.Left));//13
			gridBillingList.EndUpdate();
		}

		private void FillGrid(bool isLoading=false) {
			try {
				CreateChargeList(isLoading);
			}
			catch(Exception ex) {
				MessageBox.Show("There is something wrong with the input file. Try again. If issue persists, then contact a programmer: "+ex.Message);
				return;
			}
			RefreshGridColumns();
			gridBillingList.BeginUpdate();
			gridBillingList.ListGridRows.Clear();
			labelDuplicateProviders.Visible=false;
			for(int i=0;i<_listNewCropCharges.Count;i++) {
					GridRow row=new GridRow();
					if(_listNewCropCharges[i].repeatCharge==null) {
						row.ColorBackG=System.Drawing.Color.Orange;//highlight new charges
					}
					if(_listNewCropCharges.FindAll(x => x.AccountId==_listNewCropCharges[i].AccountId && x.NPI==_listNewCropCharges[i].NPI).Count > 1) {
						row.ColorText=System.Drawing.Color.Red;
						row.Bold=true;
						labelDuplicateProviders.Visible=true;
					}
					//0 New
					row.Cells.Add(new GridCell((_listNewCropCharges[i].repeatCharge==null)?"X":""));
					//1 ErxAccountId					
					row.Cells.Add(new GridCell(_listNewCropCharges[i].AccountId));
					//2 PatNum
					if(_listNewCropCharges[i].repeatCharge==null) {
						row.Cells.Add(new GridCell(POut.Long(_listNewCropCharges[i].PatNumForRegKey)));
					}
					else {
						row.Cells.Add(new GridCell(POut.Long(_listNewCropCharges[i].repeatCharge.PatNum)));//Allows techs to manually move repeating charge to another account.
					}
					//3 NPI
					row.Cells.Add(new GridCell(_listNewCropCharges[i].NPI));
					//4 FirstLastName
					row.Cells.Add(new GridCell(_listNewCropCharges[i].FirstName+" "+_listNewCropCharges[i].LastName));
					//5 DEA
					row.Cells.Add(new GridCell(_listNewCropCharges[i].Dea));
					//6 DateAdded
					row.Cells.Add(new GridCell(_listNewCropCharges[i].DateAdded));
					//7 CompFTE
					row.Cells.Add(new GridCell(_listNewCropCharges[i].CompFTEPercent));
					//8 BasicFTE
					row.Cells.Add(new GridCell(_listNewCropCharges[i].BasicFTEPercent));
					//9 Drug
					row.Cells.Add(new GridCell((_listNewCropCharges[i].DrugChecks=="Active")?"Y":"N"));
					//10 Drug
					row.Cells.Add(new GridCell((_listNewCropCharges[i].ForumaryChecks=="Active")?"Y":"N"));
					//11 EPCS
					row.Cells.Add(new GridCell(_listNewCropCharges[i].EPCS));
					//12 IDP
					row.Cells.Add(new GridCell(_listNewCropCharges[i].IDP));
					//13 PracticeTitle
					row.Cells.Add(new GridCell(_listNewCropCharges[i].AccountName));
					gridBillingList.ListGridRows.Add(row);
				}
				gridBillingList.EndUpdate();
		}

		///<summary>Returns a code in format Z###, depending on which codes are already in use for the current patnum.
		///The returned code is guaranteed to exist in the database, because codes are created if they do not exist.</summary>
		private string GetProcCodeForNewCharge(long patNumForRegKey) {
			//Locate a proc code for eRx which is not already in use.
			string procCode="Z000";
			int attempts=0;
			bool procCodeInUse;
			do {
				procCodeInUse=false;
				for(int i = 0;i<_listRepeatChargesErx.Count;i++) {
					if(_listRepeatChargesErx[i].PatNum!=patNumForRegKey) {
						continue;
					}
					if(_listRepeatChargesErx[i].ProcCode!=procCode) {
						continue;						
					}
					procCodeInUse=true;
					break;
				}
				if(procCodeInUse) {
					attempts++;//Should start at 2. The Codes will be "Z001", "Z002", "Z003", etc...
					if(attempts>999) {
						throw new Exception("Cannot add more than 999 Z-codes yet.  Ask programmer to increase.");
					}
					procCode="Z"+(attempts.ToString().PadLeft(3,'0'));
				}
			} while(procCodeInUse);
			//If the selected code is not in the database already, then add it automatically.
			long codeNum=ProcedureCodes.GetCodeNum(procCode);
			if(codeNum==0) {//The selected code does not exist, so we must add it.
				ProcedureCode procedureCode=new ProcedureCode();
				procedureCode.ProcCode=procCode;
				procedureCode.Descript="Electronic Rx";
				procedureCode.AbbrDesc="eRx";
				procedureCode.ProcTime="/X/";
				procedureCode.ProcCat=162;//Software
				procedureCode.TreatArea=TreatmentArea.Mouth;
				ProcedureCodes.Insert(procedureCode);
				ProcedureCodes.RefreshCache();
			}
			return procCode;
		}

		private int GetChargeDayOfMonth(long patNum) {
			//Match the day of the month for the eRx repeating charge to their existing monthly support charge (even if the monthly support is disabled).
			int day=15;//Day 15 will be used if they do not have any existing repeating charges.
			RepeatCharge[] repeatChargeArrayForPats=RepeatCharges.Refresh(patNum);
			bool hasMaintCharge=false;
			for(int j=0;j<repeatChargeArrayForPats.Length;j++) {
				if(repeatChargeArrayForPats[j].ProcCode=="001") {//Monthly maintenance repeating charge
					hasMaintCharge=true;
					day=repeatChargeArrayForPats[j].DateStart.Day;
					break;
				}
			}
			//The customer is not on monthly support, so use any other existing repeating charge day (example EHR Monthly and Mobile).
			if(!hasMaintCharge && repeatChargeArrayForPats.Length>0) {
				day=repeatChargeArrayForPats[0].DateStart.Day;
			}
			return day;
		}
		///<summary>Appends the Name, AccountId, and NPI of each charge in the list of charges passed in to the StringBuilder object also passed in</summary>
		private void StringBuilderAddRepeatCharges(StringBuilder stringBuilderMsg,List<NewCropCharge> listNewCropCharges) { 
			const int colNameWidth=62;
			const int colErxAccountIdWidth=18;
			const int colNpiWidth=10;
			stringBuilderMsg.Append("  ");
			stringBuilderMsg.Append("NAME".PadRight(colNameWidth,' '));
			stringBuilderMsg.Append("ERXACCOUNTID".PadRight(colErxAccountIdWidth,' '));
			stringBuilderMsg.AppendLine("NPI".PadRight(colNpiWidth,' '));
			for(int i=0;i<listNewCropCharges.Count;i++) {
				string firstLastName=listNewCropCharges[i].FirstName+" "+listNewCropCharges[i].LastName;
				if(firstLastName.Length > colNameWidth) {
					firstLastName=firstLastName.Substring(0,colNameWidth);
				}
				stringBuilderMsg.Append("  ");
				stringBuilderMsg.Append(firstLastName.PadRight(colNameWidth,' '));
				stringBuilderMsg.Append(listNewCropCharges[i].AccountId.PadRight(colErxAccountIdWidth,' '));
				stringBuilderMsg.AppendLine(listNewCropCharges[i].NPI.PadRight(colNpiWidth,' '));
			}
		}

		private void butProcess_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will add a new repeating charge for each provider in the list above"
				+" who is new (does not already have a repeating charge), based on PatNum and NPI.  Continue?")) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			List<NewCropCharge> listNewCropChargesAdded=new List<NewCropCharge>();
			List<NewCropCharge> listNewCropChargesNoProcedure=new List<NewCropCharge>();
			int numSkipped=0;
			StringBuilder stringBuilderArchivedPats=new StringBuilder();
			for(int i=0;i<_listNewCropCharges.Count;i++) {
				if(_listNewCropCharges[i].repeatCharge==null) {//No such repeating charge exists yet for the given npi.
					//We consider the provider a new provider and create a new repeating charge.
					int dayOtherCharges=GetChargeDayOfMonth(_listNewCropCharges[i].PatNumForRegKey);//The day of the month that the customer already has other repeating charges. Keeps their billing simple (one bill per month for all charges).
					int daysInMonth=DateTime.DaysInMonth(DateTime.Today.Year,DateTime.Today.Month);
					if(dayOtherCharges>daysInMonth) {
						//The day that the user used eRx (signed up) was in a month that does not have the day of the other monthly charges in it.
						//E.g.  dayOtherCharges = 31 and the user started a new eRx account in a month without 31 days.
						//Therefore, we have to use the last day of the month that they started.
						//This can introduce multiple statements being sent out which can potentially delay us (HQ) from getting paid in a timely fashion.
						//A workaround for this would be to train our techs to never run billing after the 28th of every month that way incomplete statements are not sent.
						dayOtherCharges=daysInMonth;
					}
					DateTime dateErxCharge=new DateTime(DateTime.Today.Year,DateTime.Today.Month,dayOtherCharges);
					//This if statement is no longer relevant after jobnum 6917 
					if(dateErxCharge<DateTime.Today.AddMonths(-3)) {//Just in case the user runs an older report.
						numSkipped++;
						continue;
					}
					bool isFutureDated=false;//Used to see if we need to future date the repeating charge in the case that the billing date has passed.  If the customer's charge date of the month has passed, we need to future date the repeating charge procedure so that this makes more sense to our customers from a billing standpoint.  We will instead include a proc for the previous month, a proc for the current month, and a future dated repeating charge.
					if(dateErxCharge.Day<=DateTime.Today.Day) {
						dateErxCharge=dateErxCharge.AddMonths(1);
						isFutureDated=true;
					}
					_listNewCropCharges[i].repeatCharge=new RepeatCharge();
					_listNewCropCharges[i].repeatCharge.IsNew=true;
					_listNewCropCharges[i].repeatCharge.PatNum=_listNewCropCharges[i].PatNumForRegKey;
					_listNewCropCharges[i].repeatCharge.ProcCode=GetProcCodeForNewCharge(_listNewCropCharges[i].PatNumForRegKey);
					_listNewCropCharges[i].repeatCharge.ChargeAmt=29;//$29/month is the cost for the basic plan. The user has to manually change the fee if it's supposed to be comprehensive.
					_listNewCropCharges[i].repeatCharge.DateStart=dateErxCharge;
					_listNewCropCharges[i].repeatCharge.Npi=_listNewCropCharges[i].NPI;
					_listNewCropCharges[i].repeatCharge.ErxAccountId=_listNewCropCharges[i].AccountId;
					_listNewCropCharges[i].repeatCharge.ProviderName=_listNewCropCharges[i].FirstName+" "+_listNewCropCharges[i].LastName;
					_listNewCropCharges[i].repeatCharge.IsEnabled=true;
					_listNewCropCharges[i].repeatCharge.CopyNoteToProc=true;//Copy the billing note to the procedure note by default so that the customer can see the NPI the charge corresponds to. Can be unchecked by user if a private note is added later (rare).
					Patient patientOld=null;
					Patient patientNew=null;
					if(!RepeatCharges.ActiveRepeatChargeExists(_listNewCropCharges[i].repeatCharge.PatNum)) { 
						//Set the patient's billing day to the start day on the repeat charge
						patientOld=Patients.GetPat(_listNewCropCharges[i].repeatCharge.PatNum);
						patientNew=patientOld.Copy();
						//Check the patients status and move them to Archived if they are currently deleted.
						if(patientOld.PatStatus==PatientStatus.Deleted) {
							patientNew.PatStatus=PatientStatus.Archived;
							string logEntry=Lan.g(this,"Patient's status changed from ")+patientOld.PatStatus.GetDescription()
								+Lan.g(this," to ")+patientNew.PatStatus.GetDescription()+Lan.g(this," from NewCrop Billing window.");
							SecurityLogs.MakeLogEntry(Permissions.PatientEdit,patientNew.PatNum,logEntry);
						}
						//Notify the user about any deleted or archived patients that were just given a new repeating charge.
						if(patientOld.PatStatus==PatientStatus.Archived || patientOld.PatStatus==PatientStatus.Deleted) {
							stringBuilderArchivedPats.AppendLine("#"+patientOld.PatNum+" - "+patientOld.GetNameLF());
						}
						patientNew.BillingCycleDay=_listNewCropCharges[i].repeatCharge.DateStart.Day;
						Patients.Update(patientNew,patientOld);
					}
					_listNewCropCharges[i].repeatCharge.RepeatChargeNum=RepeatCharges.Insert(_listNewCropCharges[i].repeatCharge);
					RepeatCharges.InsertRepeatChargeChangeSecurityLogEntry(_listNewCropCharges[i].repeatCharge,Permissions.RepeatChargeCreate,isAutomated:true,oldPat:patientOld,newPat:patientNew);
					DateTime dateNow=MiscData.GetNowDateTime();
					Procedure procedurePrevMonth=new Procedure();
					Procedure procedureCurrentMonth=new Procedure();
					try {
						procedurePrevMonth=RepeatCharges.AddProcForRepeatCharge(_listNewCropCharges[i].repeatCharge,dateNow,dateNow,new OrthoCaseProcLinkingData(_listNewCropCharges[i].repeatCharge.PatNum),isNewCropInitial:true);
						if(isFutureDated) {//Since we are future dating the repeating charge we need a second proc for the current month
							procedureCurrentMonth=RepeatCharges.AddProcForRepeatCharge(_listNewCropCharges[i].repeatCharge,dateNow,dateNow,new OrthoCaseProcLinkingData(_listNewCropCharges[i].repeatCharge.PatNum),isNewCropInitial:false,isNewCropFutureDated:true);
						}
					}
					catch(ODException ex) {
						ex.DoNothing();//Shouldn't happen because we ignore throwing for Z-codes, but something could've went wrong. Next conditional will add it to listNewCropChargesNoProcedure.
					}
					if(procedurePrevMonth.ProcNum==0 || (isFutureDated && procedureCurrentMonth.ProcNum==0)) {
						listNewCropChargesNoProcedure.Add(_listNewCropCharges[i]);
					}
					_listRepeatChargesErx.Add(_listNewCropCharges[i].repeatCharge);
					listNewCropChargesAdded.Add(_listNewCropCharges[i]);
				}
				else { //The repeating charge for eRx billing already exists for the given npi.
					RepeatCharge repeatChargeOld=_listNewCropCharges[i].repeatCharge.Copy();
					DateTime dateEndLastMonth=(new DateTime(DateTime.Today.Year,DateTime.Today.Month,1)).AddDays(-1);
					if(_listNewCropCharges[i].repeatCharge.DateStop.Year>2010) {//eRx support for this provider was disabled at one point, but has been used since.
						if(_listNewCropCharges[i].repeatCharge.DateStop<dateEndLastMonth) {//If the stop date is in the future or already at the end of the month, then we cannot presume that there will be a charge next month.
							_listNewCropCharges[i].repeatCharge.DateStop=dateEndLastMonth;//Make sure the recent use is reflected in the end date.
							Patient patientOld=Patients.GetPat(repeatChargeOld.PatNum);
							RepeatCharges.Update(_listNewCropCharges[i].repeatCharge);
							RepeatCharges.InsertRepeatChargeChangeSecurityLogEntry(repeatChargeOld,Permissions.RepeatChargeUpdate,patientOld,newCharge:_listNewCropCharges[i].repeatCharge,isAutomated:true,source:LogSources.eRx);
						}
					}
				}
			}
			FillGrid();
			Cursor=Cursors.Default;
			StringBuilder stringBuilderMsg=new StringBuilder();
			stringBuilderMsg.AppendLine("Done.");
			if(numSkipped>0) {
				stringBuilderMsg.AppendLine("Number skipped due to old DateBilling (over 3 months ago): "+numSkipped);
			}
			if(listNewCropChargesAdded.Count > 0) {
				stringBuilderMsg.AppendLine("Added the following new repeating charges ("+listNewCropChargesAdded.Count+" total):");
				StringBuilderAddRepeatCharges(stringBuilderMsg,listNewCropChargesAdded);
			}
			if(listNewCropChargesNoProcedure.Count>0) {
				stringBuilderMsg.AppendLine("Could not attach one of the z-code procs to the following repeated charges: ");
				StringBuilderAddRepeatCharges(stringBuilderMsg,listNewCropChargesNoProcedure);
			}
			List<NewCropCharge> listNewCropChargesFailed=_listNewCropChargesToAdd.Where(x => listNewCropChargesAdded.All(y => y.AccountId != x.AccountId && y.NPI != x.NPI)).ToList();
			if(listNewCropChargesFailed.Count>0) { 
				stringBuilderMsg.AppendLine("Failed to add the following repeating charges: ");
				StringBuilderAddRepeatCharges(stringBuilderMsg,listNewCropChargesFailed);
			}
			if(stringBuilderArchivedPats.Length > 0) {
				stringBuilderMsg.AppendLine("Archived patients that had a repeating charge created:");
				stringBuilderMsg.AppendLine(stringBuilderArchivedPats.ToString());
			}
			using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(stringBuilderMsg.ToString());
			msgBoxCopyPaste.ShowDialog();//Must be modal, because non-modal does not display here for some reason.
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}

	internal class NewCropCharge {
		#region CSV fields - All fields in this region are named exactly the same as the column names of the CSV file.
		public string ParentAccount;
		public string AccountName;
		public string AccountId;
		public string SiteId;
		public string LastName;
		public string FirstName;
		public string Dea;
		public string NPI;
		public string DateAdded;
		public string DoctorID;
		public string DoctorType;
		public string FTEPercent;
		public string CompFTEPercent;
		public string BasicFTEPercent;
		public string DrugChecks;
		public string ForumaryChecks;
		public string EPCS;
		public string IDP;
		public string Interop;
		public string PatientPortal;
		public string Registries;
		public string Labs;
		public string Genomics;
		public string Direct;
		public string DoctorDirect;
		#endregion CSV fields
		///<summary>The existing OD repeating charge which corresponds to this NewCrop charge.  Will be null for new charges.</summary>
		public RepeatCharge repeatCharge=null;
		///<summary>PatNum of registration key used to create the account id.  The PatNum as taken from the ErxAccountId.
		///Not equal to the PatNum of the repeating charge necessarily.</summary>
		public long PatNumForRegKey;
	}

}
