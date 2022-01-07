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
		private List<RepeatCharge> _listErxRepeatCharges=null;
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
			string[] arrayCsvLines=csvData.Split(new string[] { "\r\n" },StringSplitOptions.None);			
			for(int i=2;i<arrayCsvLines.Length;i++) {//Skip the first row (the column names), skip the second row (the totals).
				string[] arrayLineValues=arrayCsvLines[i].Split('\t');
				if(arrayLineValues.Length < 25) {
					continue;//This often happens on the last line of the file, due to a trailing newline.  Skip this line or any other malformed line.
				}
				for(int j=0;j<arrayLineValues.Length;j++) {
					arrayLineValues[j]=arrayLineValues[j].Trim('\"');//Remove leading and trailing double quotes from each cell of the csv table.
				}
				NewCropCharge charge=new NewCropCharge();
				charge.ParentAccount=arrayLineValues[0];
				charge.AccountName=arrayLineValues[1];
				charge.AccountId=arrayLineValues[2];
				charge.SiteId=arrayLineValues[3];
				charge.LastName=arrayLineValues[4];
				charge.FirstName=arrayLineValues[5];
				charge.Dea=arrayLineValues[6];
				charge.NPI=arrayLineValues[7];
				charge.DateAdded=arrayLineValues[8];
				charge.DoctorID=arrayLineValues[9];
				charge.DoctorType=arrayLineValues[10];
				charge.FTEPercent=arrayLineValues[11];
				charge.CompFTEPercent=arrayLineValues[12];
				charge.BasicFTEPercent=arrayLineValues[13];
				charge.DrugChecks=arrayLineValues[14];
				charge.ForumaryChecks=arrayLineValues[15];
				charge.EPCS=arrayLineValues[16];
				charge.IDP=arrayLineValues[17];
				charge.Interop=arrayLineValues[18];
				charge.PatientPortal=arrayLineValues[19];
				charge.Registries=arrayLineValues[20];
				charge.Labs=arrayLineValues[21];
				charge.Genomics=arrayLineValues[22];
				charge.Direct=arrayLineValues[23];
				charge.DoctorDirect=arrayLineValues[24];				
				int patNumLength=charge.AccountId.IndexOf("-");
				string patNumStr=PIn.String(charge.AccountId.Substring(0,patNumLength));
				charge.PatNumForRegKey=PIn.Long(patNumStr);//PatNum of registration key used to create the account id.
				if(charge.PatNumForRegKey==6566) {
					//Account 6566 corresponds to our software key in the training database.  These accounts are test accounts.
					continue;//Do not show OD test accounts.
				}				
				_listNewCropCharges.Add(charge);
			}
			#endregion Parse CSV
			_listErxRepeatCharges=RepeatCharges.GetForErx();
			foreach(NewCropCharge charge in _listNewCropCharges) {
				charge.repeatCharge=_listErxRepeatCharges.FirstOrDefault(x => x.ErxAccountId==charge.AccountId && x.Npi==charge.NPI);
				if(charge.repeatCharge is null && isLoading) {
					_listNewCropChargesToAdd.Add(charge);
				}
			}
		}

		private void RefreshGridColumns() {
			gridBillingList.BeginUpdate();
			gridBillingList.ListGridColumns.Clear();
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
			gridBillingList.ListGridColumns.Add(new GridColumn("New",isNewWidth,HorizontalAlignment.Center));//0
			gridBillingList.ListGridColumns.Add(new GridColumn("ErxAccountId",erxAccountIdWidth,HorizontalAlignment.Center));//1
			gridBillingList.ListGridColumns.Add(new GridColumn("PatNum",patNumWidth,HorizontalAlignment.Center));//2
			gridBillingList.ListGridColumns.Add(new GridColumn("NPI",npiWidth,HorizontalAlignment.Center));//3
			gridBillingList.ListGridColumns.Add(new GridColumn("FirstLastName",firstLastNameWidth,HorizontalAlignment.Left));//4
			gridBillingList.ListGridColumns.Add(new GridColumn("DEA",deaWidth,HorizontalAlignment.Center));//5
			gridBillingList.ListGridColumns.Add(new GridColumn("DateAdded",dateAddedWidth,HorizontalAlignment.Center));//6
			gridBillingList.ListGridColumns.Add(new GridColumn("cFTE",compFteWidth,HorizontalAlignment.Center));//7
			gridBillingList.ListGridColumns.Add(new GridColumn("bFTE",basicFteWidth,HorizontalAlignment.Center));//8
			gridBillingList.ListGridColumns.Add(new GridColumn("Drug",drugWidth,HorizontalAlignment.Center));//9
			gridBillingList.ListGridColumns.Add(new GridColumn("Form",formWidth,HorizontalAlignment.Center));//10
			gridBillingList.ListGridColumns.Add(new GridColumn("EPCS",epcsWidth,HorizontalAlignment.Center));//11
			gridBillingList.ListGridColumns.Add(new GridColumn("IDP",idpWidth,HorizontalAlignment.Center));//12
			gridBillingList.ListGridColumns.Add(new GridColumn("PracticeTitle",practiceTitleWidth,HorizontalAlignment.Left));//13
			gridBillingList.EndUpdate();
		}

		private void FillGrid(bool isLoading=false) {
			try {
				CreateChargeList(isLoading:isLoading);
				RefreshGridColumns();
				gridBillingList.BeginUpdate();
				gridBillingList.ListGridRows.Clear();
				labelDuplicateProviders.Visible=false;
				foreach(NewCropCharge charge in _listNewCropCharges) {
					GridRow gr=new GridRow();
					if(charge.repeatCharge==null) {
						gr.ColorBackG=System.Drawing.Color.Orange;//highlight new charges
					}
					if(_listNewCropCharges.FindAll(x => x.AccountId==charge.AccountId && x.NPI==charge.NPI).Count > 1) {
						gr.ColorText=System.Drawing.Color.Red;
						gr.Bold=true;
						labelDuplicateProviders.Visible=true;
					}
					//0 New
					gr.Cells.Add(new GridCell((charge.repeatCharge==null)?"X":""));
					//1 ErxAccountId					
					gr.Cells.Add(new GridCell(charge.AccountId));
					//2 PatNum
					if(charge.repeatCharge==null) {
						gr.Cells.Add(new GridCell(POut.Long(charge.PatNumForRegKey)));
					}
					else {
						gr.Cells.Add(new GridCell(POut.Long(charge.repeatCharge.PatNum)));//Allows techs to manually move repeating charge to another account.
					}
					//3 NPI
					gr.Cells.Add(new GridCell(charge.NPI));
					//4 FirstLastName
					gr.Cells.Add(new GridCell(charge.FirstName+" "+charge.LastName));
					//5 DEA
					gr.Cells.Add(new GridCell(charge.Dea));
					//6 DateAdded
					gr.Cells.Add(new GridCell(charge.DateAdded));
					//7 CompFTE
					gr.Cells.Add(new GridCell(charge.CompFTEPercent));
					//8 BasicFTE
					gr.Cells.Add(new GridCell(charge.BasicFTEPercent));
					//9 Drug
					gr.Cells.Add(new GridCell((charge.DrugChecks=="Active")?"Y":"N"));
					//10 Drug
					gr.Cells.Add(new GridCell((charge.ForumaryChecks=="Active")?"Y":"N"));
					//11 EPCS
					gr.Cells.Add(new GridCell(charge.EPCS));
					//12 IDP
					gr.Cells.Add(new GridCell(charge.IDP));
					//13 PracticeTitle
					gr.Cells.Add(new GridCell(charge.AccountName));
					gridBillingList.ListGridRows.Add(gr);
				}
				gridBillingList.EndUpdate();
			}
			catch(Exception ex) {
				MessageBox.Show("There is something wrong with the input file. Try again. If issue persists, then contact a programmer: "+ex.Message);
			}
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
				foreach(RepeatCharge repeatCharge in _listErxRepeatCharges) {
					if(repeatCharge.PatNum!=patNumForRegKey) {
						continue;
					}
					if(repeatCharge.ProcCode!=procCode) {
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
				ProcedureCode code=new ProcedureCode();
				code.ProcCode=procCode;
				code.Descript="Electronic Rx";
				code.AbbrDesc="eRx";
				code.ProcTime="/X/";
				code.ProcCat=162;//Software
				code.TreatArea=TreatmentArea.Mouth;
				ProcedureCodes.Insert(code);
				ProcedureCodes.RefreshCache();
			}
			return procCode;
		}

		private int GetChargeDayOfMonth(long patNum) {
			//Match the day of the month for the eRx repeating charge to their existing monthly support charge (even if the monthly support is disabled).
			int day=15;//Day 15 will be used if they do not have any existing repeating charges.
			RepeatCharge[] chargesForPat=RepeatCharges.Refresh(patNum);
			bool hasMaintCharge=false;
			for(int j=0;j<chargesForPat.Length;j++) {
				if(chargesForPat[j].ProcCode=="001") {//Monthly maintenance repeating charge
					hasMaintCharge=true;
					day=chargesForPat[j].DateStart.Day;
					break;
				}
			}
			//The customer is not on monthly support, so use any other existing repeating charge day (example EHR Monthly and Mobile).
			if(!hasMaintCharge && chargesForPat.Length>0) {
				day=chargesForPat[0].DateStart.Day;
			}
			return day;
		}
		///<summary>Appends the Name, AccountId, and NPI of each charge in the list of charges passed in to the StringBuilder object also passed in</summary>
		private void StringBuilderAddRepeatCharges(StringBuilder sbMsg,List<NewCropCharge> listNewCropCharges) { 
			const int colNameWidth=62;
			const int colErxAccountIdWidth=18;
			const int colNpiWidth=10;
			sbMsg.Append("  ");
			sbMsg.Append("NAME".PadRight(colNameWidth,' '));
			sbMsg.Append("ERXACCOUNTID".PadRight(colErxAccountIdWidth,' '));
			sbMsg.AppendLine("NPI".PadRight(colNpiWidth,' '));
			foreach(NewCropCharge charge in listNewCropCharges) {
				string firstLastName=charge.FirstName+" "+charge.LastName;
				if(firstLastName.Length > colNameWidth) {
					firstLastName=firstLastName.Substring(0,colNameWidth);
				}
				sbMsg.Append("  ");
				sbMsg.Append(firstLastName.PadRight(colNameWidth,' '));
				sbMsg.Append(charge.AccountId.PadRight(colErxAccountIdWidth,' '));
				sbMsg.AppendLine(charge.NPI.PadRight(colNpiWidth,' '));
			}
		}

		private void butProcess_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will add a new repeating charge for each provider in the list above"
				+" who is new (does not already have a repeating charge), based on PatNum and NPI.  Continue?")) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			List<NewCropCharge> listAddedCharges=new List<NewCropCharge>();
			List<NewCropCharge> listNewCropChargesNoProcedure=new List<NewCropCharge>();
			int numSkipped=0;
			StringBuilder strBldArchivedPats=new StringBuilder();
			foreach(NewCropCharge charge in _listNewCropCharges) {
				if(charge.repeatCharge==null) {//No such repeating charge exists yet for the given npi.
					//We consider the provider a new provider and create a new repeating charge.
					int dayOtherCharges=GetChargeDayOfMonth(charge.PatNumForRegKey);//The day of the month that the customer already has other repeating charges. Keeps their billing simple (one bill per month for all charges).
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
					charge.repeatCharge=new RepeatCharge();
					charge.repeatCharge.IsNew=true;
					charge.repeatCharge.PatNum=charge.PatNumForRegKey;
					charge.repeatCharge.ProcCode=GetProcCodeForNewCharge(charge.PatNumForRegKey);
					charge.repeatCharge.ChargeAmt=22;//$22/month is the cost for the basic plan. The user has to manually change the fee if it's supposed to be comprehensive.
					charge.repeatCharge.DateStart=dateErxCharge;
					charge.repeatCharge.Npi=charge.NPI;
					charge.repeatCharge.ErxAccountId=charge.AccountId;
					charge.repeatCharge.ProviderName=charge.FirstName+" "+charge.LastName;
					charge.repeatCharge.IsEnabled=true;
					charge.repeatCharge.CopyNoteToProc=true;//Copy the billing note to the procedure note by default so that the customer can see the NPI the charge corresponds to. Can be unchecked by user if a private note is added later (rare).
					Patient patOld=null;
					Patient patNew=null;
					if(!RepeatCharges.ActiveRepeatChargeExists(charge.repeatCharge.PatNum)) { 
						//Set the patient's billing day to the start day on the repeat charge
						patOld=Patients.GetPat(charge.repeatCharge.PatNum);
						patNew=patOld.Copy();
						//Check the patients status and move them to Archived if they are currently deleted.
						if(patOld.PatStatus==PatientStatus.Deleted) {
							patNew.PatStatus=PatientStatus.Archived;
							string logEntry=Lan.g(this,"Patient's status changed from ")+patOld.PatStatus.GetDescription()
								+Lan.g(this," to ")+patNew.PatStatus.GetDescription()+Lan.g(this," from NewCrop Billing window.");
							SecurityLogs.MakeLogEntry(Permissions.PatientEdit,patNew.PatNum,logEntry);
						}
						//Notify the user about any deleted or archived patients that were just given a new repeating charge.
						if(patOld.PatStatus==PatientStatus.Archived || patOld.PatStatus==PatientStatus.Deleted) {
							strBldArchivedPats.AppendLine("#"+patOld.PatNum+" - "+patOld.GetNameLF());
						}
						patNew.BillingCycleDay=charge.repeatCharge.DateStart.Day;
						Patients.Update(patNew,patOld);
					}
					charge.repeatCharge.RepeatChargeNum=RepeatCharges.Insert(charge.repeatCharge);
					RepeatCharges.InsertRepeatChargeChangeSecurityLogEntry(charge.repeatCharge,Permissions.RepeatChargeCreate,isAutomated:true,oldPat:patOld,newPat:patNew);
					DateTime now=MiscData.GetNowDateTime();
					Procedure procedure=RepeatCharges.AddProcForRepeatCharge(charge.repeatCharge,now,now,new OrthoCaseProcLinkingData(charge.repeatCharge.PatNum),isNewCropInitial:true);
					if(procedure.ProcNum==0) {
						listNewCropChargesNoProcedure.Add(charge);
					}
					_listErxRepeatCharges.Add(charge.repeatCharge);
					listAddedCharges.Add(charge);
				}
				else { //The repeating charge for eRx billing already exists for the given npi.
					RepeatCharge oldCharge=charge.repeatCharge.Copy();
					DateTime dateEndLastMonth=(new DateTime(DateTime.Today.Year,DateTime.Today.Month,1)).AddDays(-1);
					if(charge.repeatCharge.DateStop.Year>2010) {//eRx support for this provider was disabled at one point, but has been used since.
						if(charge.repeatCharge.DateStop<dateEndLastMonth) {//If the stop date is in the future or already at the end of the month, then we cannot presume that there will be a charge next month.
							charge.repeatCharge.DateStop=dateEndLastMonth;//Make sure the recent use is reflected in the end date.
							Patient oldPat=Patients.GetPat(oldCharge.PatNum);
							RepeatCharges.Update(charge.repeatCharge);
							RepeatCharges.InsertRepeatChargeChangeSecurityLogEntry(oldCharge,Permissions.RepeatChargeUpdate,oldPat,newCharge:charge.repeatCharge,isAutomated:true,source:LogSources.eRx);
						}
					}
				}
			}
			FillGrid();
			Cursor=Cursors.Default;
			StringBuilder sbMsg=new StringBuilder();
			sbMsg.AppendLine("Done.");
			if(numSkipped>0) {
				sbMsg.AppendLine("Number skipped due to old DateBilling (over 3 months ago): "+numSkipped);
			}
			if(listAddedCharges.Count > 0) {
				sbMsg.AppendLine("Added the following new repeating charges ("+listAddedCharges.Count+" total):");
				StringBuilderAddRepeatCharges(sbMsg,listAddedCharges);
			}
			if(listNewCropChargesNoProcedure.Count>0) {
				sbMsg.AppendLine("Could not attach a z-code proc to the following repeated charges: ");
				StringBuilderAddRepeatCharges(sbMsg,listNewCropChargesNoProcedure);
			}
			List<NewCropCharge> listNewCropChargesFailed=_listNewCropChargesToAdd.Where(x => listAddedCharges.All(y => y.AccountId != x.AccountId && y.NPI != x.NPI)).ToList();
			if(listNewCropChargesFailed.Count>0) { 
				sbMsg.AppendLine("Failed to add the following repeating charges: ");
				StringBuilderAddRepeatCharges(sbMsg,listNewCropChargesFailed);
			}
			if(strBldArchivedPats.Length > 0) {
				sbMsg.AppendLine("Archived patients that had a repeating charge created:");
				sbMsg.AppendLine(strBldArchivedPats.ToString());
			}
			using MsgBoxCopyPaste msgBoxCP=new MsgBoxCopyPaste(sbMsg.ToString());
			msgBoxCP.ShowDialog();//Must be modal, because non-modal does not display here for some reason.
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
