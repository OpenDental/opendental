using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using OpenDental.Thinfinity;
using System.Threading;

namespace OpenDental {
	public partial class FormTextPayLink:FormODBase {
		#region Fields - Private
		///<summary>Helper to manager prefs relating to textPaymentLink and getting them to/from the db.</summary>
		private ClinicPrefHelper _clinicPrefHelper=new ClinicPrefHelper(
			PrefName.TextPaymentLinkAppointmentBalance,
			PrefName.TextPaymentLinkAccountBalance);
		//Patient on the appointment
		private Patient _patient;
		private Appointment _appointment;
		///<summary>Family account data. Set with AccountModules.GetAccount.</summary>
		private DataSet _dataSetAccount;
		private List<DataRow> _listDataRowsForAccount;
		private List<Procedure> _listProcedures;
		private List<Adjustment> _listAdjustments;
		private List<Payment> _listPayments;
		private List<ClaimPayment> _listClaimPayments;
		private List<PaySplit> _listPaySplits;
		private List<ClaimProc> _listClaimProcs;
		///<summary>Clinic for _patient.</summary>
		private Clinic _clinic;
		#endregion Fields - Private

		#region Fields - Public
		public string Message="";
		public Statement StatementCur;
		#endregion

		public FormTextPayLink(Appointment appt,Patient pat) {
			InitializeComponent();
			InitializeLayoutManager();
			_patient=pat;//We need to set this before calling fill data, then refresh the patient after.
			_appointment=appt;
			Lan.F(this);
		}

		private void FormTextPayLink_Load(object sender,EventArgs e) {
			comboBoxClinicPicker.IncludeUnassigned=true;
			comboBoxClinicPicker.HqDescription="Default";
			FillData();
			FillPreview();
			LayoutToolBar();
		}

		#region Methods - Fillers
		private void LayoutToolBar() {
			toolBarMain.Buttons.Clear();
			toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Setup Templates"),0,"","Setup Templates"));
		}

		private void FillPreview() {
			Statement statement=StatementCur;
			PrefName prefName=GetSelectedTemplate();
			textPreview.Text=_clinicPrefHelper.GetStringVal(prefName,GetClinicNumSelected());
		}

		private void FillData() {
			Statement statement=new Statement();
			statement.DateRangeFrom=DateTime.MinValue;
			statement.DateRangeTo=DateTime.Today;
			statement.IsInvoice=false;
			_dataSetAccount=AccountModules.GetAccount(_patient.PatNum,statement);
			_clinic=Clinics.GetClinic(_appointment.ClinicNum);
			//Get Proc, Adj, Payment, and ClaimPayment nums for account. (Includes family)
			List<long> listProcNums=new List<long>();
			List<long> listAdjNums=new List<long>();
			List<long> listPayNums=new List<long>();
			List<long> listClaimNums=new List<long>();
			List<long> listClaimPayNums=new List<long>();
			List<DataTable> listDataTablesAccounts=_dataSetAccount.Tables.OfType<DataTable>().Where(x => x.TableName.StartsWith("account")).ToList();
			_listDataRowsForAccount=new List<DataRow>();
			//Gets all the data rows from each account table that are Procedures, Adjustments, Claims, ClaimPayments, or regular payments
			for(int i = 0;i<listDataTablesAccounts.Count;i++) {
				DataTable table=listDataTablesAccounts[i];
				for(int j = 0;j<table.Rows.Count;j++) {
					if(table.Rows[j]["ProcNum"].ToString()!="0") { 
						listProcNums.Add(PIn.Long(table.Rows[j]["ProcNum"].ToString()));
						_listDataRowsForAccount.Add(table.Rows[j]);
					}
					else if(table.Rows[j]["AdjNum"].ToString()!="0") { 
						listAdjNums.Add(PIn.Long(table.Rows[j]["AdjNum"].ToString()));
						_listDataRowsForAccount.Add(table.Rows[j]);
					}
					else if(table.Rows[j]["PayNum"].ToString()!="0") { 
						listPayNums.Add(PIn.Long(table.Rows[j]["PayNum"].ToString()));
						_listDataRowsForAccount.Add(table.Rows[j]);
					}
					else if(table.Rows[j]["ClaimNum"].ToString()!="0") {
						listClaimNums.Add(PIn.Long(table.Rows[j]["ClaimNum"].ToString()));
						_listDataRowsForAccount.Add(table.Rows[j]);
					}
					else if(table.Rows[j]["ClaimPaymentNum"].ToString()!="0") {
						listClaimPayNums.Add(PIn.Long(table.Rows[j]["ClaimPaymentNum"].ToString()));
						_listDataRowsForAccount.Add(table.Rows[j]);
					}
				}
			}
			//Gets the objects associated to the account.
			_listProcedures=Procedures.GetManyProc(listProcNums,false);
			_listAdjustments=Adjustments.GetMany(listAdjNums);
			_listPayments=Payments.GetPayments(listPayNums);
			_listClaimPayments=ClaimPayments.GetByClaimPaymentNums(listClaimPayNums);
			_listPaySplits=PaySplits.GetForPayments(listPayNums);
			_listClaimProcs=ClaimProcs.GetForProcs(listProcNums);
			StatementCur=GetStatement();
		}
		#endregion
		#region Methods - Get Data

		///<summary>Gets the statement for the currently selected UI.</summary>
		private Statement GetStatement(bool saveToDb=false) {
			#region Data For Filtering
			//Get all production objects for the family
			//These lists will be used to house the appropriate production nums when building the appointment statement. Only used in 'PrefName.TextPaymentLinkAppointmentBalance' case.
			List<long> listProcNumsForAppt=null;
			List<long> listAdjNumsForAppt=null;
			List<long> listClaimPayNumsForAppt=null;
			List<long> listPaymentNumsForAppt=null;
			#endregion
			List<PaySplit> listPaySplitsForType=null;
			List<ClaimProc> listClaimProcsForType=null;
			List<DataRow> listDataRowsForBillingType=new List<DataRow>();
			PrefName textPaymentLinkPrefName=GetSelectedTemplate();
			switch(textPaymentLinkPrefName) {
				case PrefName.TextPaymentLinkAppointmentBalance:
					listProcNumsForAppt=_listProcedures.FindAll(x=>x.AptNum==_appointment.AptNum).Select(x=>x.ProcNum).ToList();
					listAdjNumsForAppt=_listAdjustments.FindAll(x=>listProcNumsForAppt.Contains(x.ProcNum)).Select(x=>x.AdjNum).ToList();
					//Finds payments associated to the given procedures, by first getting the paysplits associated to the procs, then their payments.
					listClaimPayNumsForAppt=_listClaimPayments
						.Where(x=>_listClaimProcs.Select(y=>y.ClaimPaymentNum.ToString()).ToList().Contains(x.ClaimPaymentNum.ToString()))
						.Select(x=>x.ClaimPaymentNum).ToList();
					listPaymentNumsForAppt=_listPayments
						.Where(x=>_listPaySplits.Select(y=>y.PayNum.ToString()).ToList().Contains(x.PayNum.ToString()))
						.Select(x=>x.PayNum).ToList();
					listClaimProcsForType=_listClaimProcs.FindAll(x=>listProcNumsForAppt.Contains(x.ProcNum));
					listPaySplitsForType=_listPaySplits.FindAll(x=>listProcNumsForAppt.Contains(x.ProcNum));
					//Get the data rows associated to the appointment.
					listDataRowsForBillingType=_listDataRowsForAccount.FindAll(x=>listProcNumsForAppt.Contains(PIn.Long(x["ProcNum"].ToString()))
					|| listAdjNumsForAppt.Contains(PIn.Long(x["AdjNum"].ToString()))
					|| listClaimPayNumsForAppt.Contains(PIn.Long(x["ClaimPaymentNum"].ToString()))
					|| listClaimProcsForType.Select(x=>x.ClaimNum.ToString()).ToList().Contains(x["ClaimNum"]));
					break;
				case PrefName.TextPaymentLinkAccountBalance:
					listClaimProcsForType=_listClaimProcs;
					listDataRowsForBillingType=_listDataRowsForAccount;
					//If we are not using the family balance, only use data rows for the patient.
					if(!checkBalForFam.Checked) {
						listClaimProcsForType=_listClaimProcs.FindAll(x=>x.PatNum==_patient.PatNum);
						listDataRowsForBillingType=_listDataRowsForAccount.FindAll(x=>x["PatNum"].ToString()==_patient.PatNum.ToString());
					}
					break;
			}
			double balance=GetBalance(listDataRowsForBillingType,listClaimProcsForType,listPaySplitsForType);
			double insuranceEst=GetInsuranceEst(listClaimProcsForType);
			Statement statement=GenerateStatementToSend(
				textPaymentLinkPrefName,
				balance,
				insuranceEst,
				listProcNumsForAppt,
				listClaimPayNumsForAppt,
				listAdjNumsForAppt,
				listPaymentNumsForAppt,
				saveToDb);
			return statement;
		}

		///<summary>Gets the Balance based on the dataRows passed in. If listPaySplits and listClaimProcs are not null, then insurance payments and payments in the data rows will be ignored,
		///and the PaySplits and ClaimProcs will be used instead.</summary>
		private static double GetBalance(List<DataRow> listDataRows,List<ClaimProc> listClaimProcs,List<PaySplit> listPaySplits) {
			//This code was taken from SheetFiller.totInsBalValsHelper()
			double statementTotal=listDataRows.Sum(x => PIn.Double(x["chargesDouble"].ToString())-PIn.Double(x["creditsDouble"].ToString()));//add charges-credits
			if(listPaySplits!=null && listClaimProcs!=null) {
				statementTotal=listDataRows
					.FindAll(x=>x["PayNum"].ToString()=="0" && x["ClaimPaymentNum"].ToString()=="0")
					.Sum(x => PIn.Double(x["chargesDouble"].ToString())-PIn.Double(x["creditsDouble"].ToString()));
				statementTotal-=listPaySplits.Sum(x=>x.SplitAmt)
					+ listClaimProcs.FindAll(x=>x.Status==ClaimProcStatus.Received 
					|| x.Status==ClaimProcStatus.Supplemental 
					|| x.Status==ClaimProcStatus.CapComplete)
					.Sum(x=>x.InsPayAmt+x.WriteOff);
			}
			return statementTotal;
		} 

		///<summary>Gets the insurance estimates for the passed in claimprocs. Includes writeoffs. We don't have to worry about discount plans since their adjustments get created on completion of a procedure.</summary>
		private static double GetInsuranceEst(List<ClaimProc> listClaimProcs) {
			return listClaimProcs.FindAll(x=>x.Status==ClaimProcStatus.NotReceived).Sum(x=>x.InsPayEst+x.WriteOff);
		}

		private PrefName GetSelectedTemplate() {
			if(radioAccount.Checked==true) {
				return PrefName.TextPaymentLinkAccountBalance;
			}
			else {
				return PrefName.TextPaymentLinkAppointmentBalance;
			}
		}
		
		private long GetClinicNumSelected() {
			Clinic clinic=comboBoxClinicPicker.GetSelectedClinic();
			if(clinic is null){
				return 0;
			}
			return clinic.ClinicNum;
		}

		#endregion
		#region Methods - Build Statements

		///<summary>Creates a statement for the passed in prefName. URL related fields are filled with spoof data, and need to be set and updated using BindGuidToStatement.
		///If the prefName is 'PrefName.TextPaymentLinkAppointmentBalance' a limited statement will be created using the passed in optional parameters.</summary>
		private Statement GenerateStatementToSend(PrefName prefName,double balance,double insEst,List<long> listProcNumsForAppt=null,List<long> listClaimPayNumsForAppt=null,
			List<long> listAdjNumsForAppt=null,List<long> listPaymentNumsForAppt=null,bool saveToDB=false) {
			Statement statement=new Statement();
			//TODO - Created limited statements more gooder
			if(prefName==PrefName.TextPaymentLinkAppointmentBalance && saveToDB) {
				statement=Statements.CreateLimitedStatement(
					new List<long>{ _patient.PatNum },
					_patient.PatNum,
					listClaimPayNumsForAppt,
					listAdjNumsForAppt,
					listPaymentNumsForAppt,
					listProcNumsForAppt,
					listPayPlanChargeNums:new List<long>());//Only working with pay plan charges in Account module by selection.
			}
			//-- GUID data gets set on click of 'Prepare to Send', from BindGuidToStatement().
			//-- Finance (Shouldn't actually need to do this, as sheet filler will do it anyway)
			statement.BalTotal=balance;
			statement.InsEst=insEst;
			//-- MetaData
			statement.Mode_=StatementMode.Electronic;
			statement.SmsSendStatus=AutoCommStatus.SendNotAttempted;
			if(prefName==PrefName.TextPaymentLinkAppointmentBalance && saveToDB) {
				return statement;
			}
			statement.IsSent=false;
			statement.IsNew=true;
			statement.HidePayment=false;
			statement.DocNum=0;
			statement.StatementType=StmtType.NotSet;
			//-- Date info
			statement.DateRangeFrom=DateTime.Today.AddDays(-PrefC.GetLong(PrefName.BillingDefaultsLastDays));//Taken from FormBillingOptions.SetDefaults()
			statement.DateRangeTo=DateTime.Today;
			statement.DateSent=DateTime.Today;
			//-- Pat info
			statement.PatNum=_patient.PatNum;
			statement.SinglePatient=!checkBalForFam.Checked;
			//-- Notes
			statement.Note="";
			statement.NoteBold="";
			if(prefName==PrefName.TextPaymentLinkAppointmentBalance) {
				//--Linked production
				statement.ListProcNums=listProcNumsForAppt;
				statement.ListAdjNums=listAdjNumsForAppt;
				statement.ListInsPayClaimNums=listClaimPayNumsForAppt;
				statement.ListPaySplitNums=listPaymentNumsForAppt;
			}
			if(saveToDB) {
				statement.StatementNum=Statements.Insert(statement);
			}
			return statement;
		}

		///<summary>Reserves a ShortGUID and updates the statement ShortGUID, StatementShortURL, and StatementURL fields. Stores changes in DB.</summary>
		private Statement BindGuidToStatement(Statement statement) {
			List<WebServiceMainHQProxy.ShortGuidResult> listShortGuidResults=new List<WebServiceMainHQProxy.ShortGuidResult>();
			try {
				listShortGuidResults=WebServiceMainHQProxy.GetShortGUIDs(1,1,_appointment.ClinicNum,eServiceCode.PatientPortalViewStatement);
			}
			catch(Exception ex) {
				FriendlyException.Show(Lans.g(this,"Unable to create a unique URL for each statement. The Patient Portal URL will be used instead."),ex);
				WebServiceMainHQProxy.ShortGuidResult shortGuidResult = new WebServiceMainHQProxy.ShortGuidResult();
				shortGuidResult.MediumURL=PrefC.GetString(PrefName.PatientPortalURL);
				shortGuidResult.ShortURL=PrefC.GetString(PrefName.PatientPortalURL);
				shortGuidResult.ShortGuid="";
				listShortGuidResults.Add(shortGuidResult);
			}
			statement.ShortGUID=listShortGuidResults.First().ShortGuid;
			statement.StatementShortURL=listShortGuidResults.First().ShortURL;
			statement.StatementURL=listShortGuidResults.First().MediumURL;
			Statements.Update(statement);
			return statement;
		}

		///<summary>Builds a PDF of the current statement object. If isPreview is true, the PDF will be saved to the temp/OpenDental path for viewing, and then deleted.
		///Otherwise, the PDF will be saved to the AtoZ folder (or the cloud for web) and the document number will be set on the statement.</summary>
		private Statement BindPDFToStatement(Statement statement,bool isPreview=false) {
			DataSet dataSetStatement=AccountModules.GetAccount(statement.PatNum,statement,isComputeAging:false,doIncludePatLName:true);
			SheetDef sheetDef=SheetUtil.GetStatementSheetDef(statement);
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,statement.PatNum,StatementCur.HidePayment);
			SheetFiller.FillFields(sheet,dataSetStatement,statement);
			SheetUtil.CalculateHeights(sheet,dataSetStatement,statement,true);
			sheet.Parameters.Add(new SheetParameter(true,"Statement") { ParamValue=statement });
			string filePath=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),statement.PatNum.ToString()+".pdf");
			SheetPrinting.CreatePdf(sheet,filePath,statement,dataSetStatement,null);
			if(isPreview) {
				try {
					if(ODBuild.IsWeb()) {
						ThinfinityUtils.HandleFile(filePath);
					}
					else {
						Process.Start(filePath);
					}		
				}
				catch(Exception ex) {
					FriendlyException.Show(Lan.g(this,"Unable to open the file."),ex);
				}
				return statement;
			}
			List<Def> listDefsImageCat=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			long category=0;
			for(int i=0;i<listDefsImageCat.Count;i++) {
				if(Regex.IsMatch(listDefsImageCat[i].ItemValue,@"S")) {
					category=listDefsImageCat[i].DefNum;
					break;
				}
			}
			if(category==0) {
				category=listDefsImageCat[0].DefNum;//put it in the first category.
			}
			Document document=null;
			try {
				//This will store the document in the patient folder, we'll delete the temp folder later.
				document=ImageStore.Import(filePath,category,_patient);
			}
			catch {
				MsgBox.Show(this,"Error saving document.");
				return statement;
			}
			document.ImgType=ImageType.Document;
			document.Description=Lan.g(this,"Statement");
			document.DateCreated=DateTime.Today;
			statement.DocNum=document.DocNum;
			Documents.Update(document);
			return statement;
		}
		#endregion
		#region Event Handlers - UI

		private void butPreview_Click(object sender,EventArgs e) {
			Statement statement=GetStatement(true);
			BindPDFToStatement(statement,true);
			try {
				Statements.DeleteStatements(new List<Statement>{ statement },true);
			}
			catch(Exception ex) {
				//This shouldn't happen
			}
		}

		private void butSetupTemplates() {
			using FormTextPaySetup formTextPaySetup=new FormTextPaySetup();
			formTextPaySetup.ShowDialog();
			if(formTextPaySetup.DialogResult==DialogResult.OK) {
				_clinicPrefHelper=new ClinicPrefHelper(
					PrefName.TextPaymentLinkAppointmentBalance,
					PrefName.TextPaymentLinkAccountBalance);
				FillPreview();
			}	
		}

		private void checkBalForFam_Click(object sender,EventArgs e) {
			FillPreview();
		}

		private void checkIncludeInsurance_Click(object sender,EventArgs e) {
			FillPreview();
		}

		private void comboBoxClinicPicker_SelectionChangeCommitted(object sender,EventArgs e) {
			FillPreview();
		}

		private void radioAppointment_CheckedChanged(object sender,EventArgs e) {
			FillPreview();
		}

		private void radioAccount_CheckedChanged(object sender,EventArgs e) {
			FillPreview();
		}

		private void toolBarMain_ButtonClick(object sender,OpenDental.UI.ODToolBarButtonClickEventArgs e) {
			switch(e.Button.Tag.ToString()) {
				case "Setup Templates":
					butSetupTemplates();
					break;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			//Set the statement URLs to actual links
			Statement statement=GetStatement(true);
			statement=BindGuidToStatement(statement);
			statement=BindPDFToStatement(statement);
			if(statement.DocNum==0) {
				MsgBox.Show("There was an error creating the document to send");
				DialogResult=DialogResult.Cancel;
				return;
			}
			Statements.Update(statement);
			Message=Statements.ReplaceVarsForSms(_clinicPrefHelper.GetStringVal(GetSelectedTemplate(),GetClinicNumSelected()),_patient,statement,_clinic,checkIncludeInsurance.Checked);
			StatementCur=statement;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
		#endregion
	}
}