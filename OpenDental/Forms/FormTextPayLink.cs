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
		private Patient _patCur;
		private Appointment _apptCur;
		///<summary>Family account data. Set with AccountModules.GetAccount.</summary>
		private DataSet _accountData;
		private List<DataRow> _listDataRowsForAccount;
		private List<Procedure> _listProcs;
		private List<Adjustment> _listAdjustments;
		private List<Payment> _listPayments;
		private List<ClaimPayment> _listClaimPayments;
		private List<PaySplit> _listPaySplits;
		private List<ClaimProc> _listClaimProcs;
		///<summary>Clinic for _patCur.</summary>
		Clinic _clinic;

		private long _selectedClinicNum {
			get {
				return comboBoxClinicPicker.GetSelectedClinic()==null ? 0 : comboBoxClinicPicker.GetSelectedClinic().ClinicNum;
			}
		}	
		#endregion Fields - Private
		#region Fields - Public
		public string Message="";
		public Statement StmtCur;
		#endregion
		public FormTextPayLink(Appointment appt,Patient pat) {
			InitializeComponent();
			InitializeLayoutManager();
			_patCur=pat;//We need to set this before calling fill data, then refresh the patient after.
			_apptCur=appt;
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
			Statement stmt=StmtCur;
			PrefName prefName=GetSelectedTemplate();
			textPreview.Text=_clinicPrefHelper.GetStringVal(prefName,_selectedClinicNum);
		}

		private void FillData() {
			Statement stmt=new Statement();
			stmt.DateRangeFrom=DateTime.MinValue;
			stmt.DateRangeTo=DateTime.Today;
			stmt.IsInvoice=false;
			_accountData=AccountModules.GetAccount(_patCur.PatNum,stmt);
			_clinic=Clinics.GetClinic(_apptCur.ClinicNum);
			//Get Proc, Adj, Payment, and ClaimPayment nums for account. (Includes family)
			List<long> listProcNums=new List<long>();
			List<long> listAdjNums=new List<long>();
			List<long> listPayNums=new List<long>();
			List<long> listClaimNums=new List<long>();
			List<long> listClaimPayNums=new List<long>();
			List<DataTable> accountTables=_accountData.Tables.OfType<DataTable>().Where(x => x.TableName.StartsWith("account")).ToList();
			_listDataRowsForAccount=new List<DataRow>();
			//Gets all the data rows from each account table that are Procedures, Adjustments, Claims, ClaimPayments, or regular payments
			for(int i = 0;i<accountTables.Count;i++) {
				DataTable temp=accountTables[i];
				for(int j = 0;j<temp.Rows.Count;j++) {
					if(temp.Rows[j]["ProcNum"].ToString()!="0") { 
						listProcNums.Add(PIn.Long(temp.Rows[j]["ProcNum"].ToString()));
						_listDataRowsForAccount.Add(temp.Rows[j]);
					}
					else if(temp.Rows[j]["AdjNum"].ToString()!="0") { 
						listAdjNums.Add(PIn.Long(temp.Rows[j]["AdjNum"].ToString()));
						_listDataRowsForAccount.Add(temp.Rows[j]);
					}
					else if(temp.Rows[j]["PayNum"].ToString()!="0") { 
						listPayNums.Add(PIn.Long(temp.Rows[j]["PayNum"].ToString()));
						_listDataRowsForAccount.Add(temp.Rows[j]);
					}
					else if(temp.Rows[j]["ClaimNum"].ToString()!="0") {
						listClaimNums.Add(PIn.Long(temp.Rows[j]["ClaimNum"].ToString()));
						_listDataRowsForAccount.Add(temp.Rows[j]);
					}
					else if(temp.Rows[j]["ClaimPaymentNum"].ToString()!="0") {
						listClaimPayNums.Add(PIn.Long(temp.Rows[j]["ClaimPaymentNum"].ToString()));
						_listDataRowsForAccount.Add(temp.Rows[j]);
					}
				}
			}
			//Gets the objects associated to the account.
			_listProcs=Procedures.GetManyProc(listProcNums,false);
			_listAdjustments=Adjustments.GetMany(listAdjNums);
			_listPayments=Payments.GetPayments(listPayNums);
			_listClaimPayments=ClaimPayments.GetByClaimPaymentNums(listClaimPayNums);
			_listPaySplits=PaySplits.GetForPayments(listPayNums);
			_listClaimProcs=ClaimProcs.GetForProcs(listProcNums);
			StmtCur=GetStatement();
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
					GetListProductionNumsForAppt(
						out listAdjNumsForAppt,
						out listClaimPayNumsForAppt,
						out listPaymentNumsForAppt,
						out listProcNumsForAppt);
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
						listClaimProcsForType=_listClaimProcs.FindAll(x=>x.PatNum==_patCur.PatNum);
						listDataRowsForBillingType=_listDataRowsForAccount.FindAll(x=>x["PatNum"].ToString()==_patCur.PatNum.ToString());
					}
					break;
			}
			double balance=GetBalance(listDataRowsForBillingType,listClaimProcsForType,listPaySplitsForType);
			double insuranceEst=GetInsuranceEst(listClaimProcsForType);
			Statement stmt=GenerateStatementToSend(
				textPaymentLinkPrefName,
				balance,
				insuranceEst,
				listProcNumsForAppt,
				listClaimPayNumsForAppt,
				listAdjNumsForAppt,
				listPaymentNumsForAppt,
				saveToDb);
			return stmt;
		}

		///<summary>Sets AdjNums, ClaimPayNums, and PaymentNums linked to the classwide Procedures, PaySplits, and ClaimProcs. 
		///The List of Payments, Adjustments, and ClaimPayments are used for reference (since they house ClaimProcs,PaySplits,etc).</summary>
		private void GetListProductionNumsForAppt(out List<long> listAdjNumsForAppt,out List<long> listClaimPayNumsForAppt,out List<long> listPaymentNumsForAppt,
			out List<long> listProcNumsForAppt) 
		{
			listProcNumsForAppt=_listProcs.FindAll(x=>x.AptNum==_apptCur.AptNum).Select(x=>x.ProcNum).ToList();
			List<long> procNums=listProcNumsForAppt.Select(x=>x).ToList();
			//Get the Adj, Claimproc, ClaimPay, and Payment nums associated to those procs
			listAdjNumsForAppt=_listAdjustments.Where(x=>procNums.Contains(x.ProcNum)).Select(x=>x.AdjNum).ToList();
			//Finds payments associated to the given procedures, by first getting the paysplits associated to the procs, then their payments.
			listClaimPayNumsForAppt=_listClaimPayments
				.Where(x=>_listClaimProcs.Select(y=>y.ClaimPaymentNum.ToString()).ToList().Contains(x.ClaimPaymentNum.ToString()))
				.Select(x=>x.ClaimPaymentNum).ToList();
			listPaymentNumsForAppt=_listPayments
				.Where(x=>_listPaySplits.Select(y=>y.PayNum.ToString()).ToList().Contains(x.PayNum.ToString()))
				.Select(x=>x.PayNum).ToList();
		}

		///<summary>Gets the Balance based on the dataRows passed in. If listPaySplits and listClaimProcs are not null, then insurance payments and payments in the data rows will be ignored,
		///and the PaySplits and ClaimProcs will be used instead.</summary>
		private static double GetBalance(List<DataRow> dataRows,List<ClaimProc> listClaimProcs,List<PaySplit> listPaySplits) {
			//This code was taken from SheetFiller.totInsBalValsHelper()
			double statementTotal=dataRows.Sum(x => PIn.Double(x["chargesDouble"].ToString())-PIn.Double(x["creditsDouble"].ToString()));//add charges-credits
			if(listPaySplits!=null && listClaimProcs!=null) {
				statementTotal=dataRows
					.Where(x=>x["PayNum"].ToString()=="0" && x["ClaimPaymentNum"].ToString()=="0")
					.Sum(x => PIn.Double(x["chargesDouble"].ToString())-PIn.Double(x["creditsDouble"].ToString()));
				statementTotal-=listPaySplits.Sum(x=>x.SplitAmt)
					+ listClaimProcs.Where(x=>x.Status==ClaimProcStatus.Received 
					|| x.Status==ClaimProcStatus.Supplemental 
					|| x.Status==ClaimProcStatus.CapComplete)
					.Sum(x=>x.InsPayAmt+x.WriteOff);
			}
			return statementTotal;
		} 

		///<summary>Gets the insurance estimates for the passed in claimprocs. Includes writeoffs. We don't have to worry about discount plans since their adjustments get created on completion of a procedure.</summary>
		private static double GetInsuranceEst(List<ClaimProc> listClaimProcs) {
			return listClaimProcs.Where(x=>x.Status==ClaimProcStatus.NotReceived).Sum(x=>x.InsPayEst+x.WriteOff);
		}

		private PrefName GetSelectedTemplate() {
			if(radioAccount.Checked==true) {
				return PrefName.TextPaymentLinkAccountBalance;
			}
			else {
				return PrefName.TextPaymentLinkAppointmentBalance;
			}
		}
		#endregion
		#region Methods - Build Statements

		///<summary>Creates a statement for the passed in prefName. URL related fields are filled with spoof data, and need to be set and updated using BindGuidToStatement.
		///If the prefName is 'PrefName.TextPaymentLinkAppointmentBalance' a limited statement will be created using the passed in optional parameters.</summary>
		private Statement GenerateStatementToSend(PrefName prefName,double balance,double insEst,List<long> listProcNumsForAppt=null,List<long> listClaimPayNumsForAppt=null,
			List<long> listAdjNumsForAppt=null,List<long> listPaymentNumsForAppt=null,bool saveToDB=false) {
			Statement stmt=new Statement();
			//TODO - Created limited statements more gooder
			if(prefName==PrefName.TextPaymentLinkAppointmentBalance && saveToDB) {
				stmt=Statements.CreateLimitedStatement(
					new List<long>{ _patCur.PatNum },
					_patCur.PatNum,
					listClaimPayNumsForAppt,
					listAdjNumsForAppt,
					listPaymentNumsForAppt,
					listProcNumsForAppt);
			}
			//-- GUID data gets set on click of 'Prepare to Send', from BindGuidToStatement().
			//-- Finance (Shouldn't actually need to do this, as sheet filler will do it anyway)
			stmt.BalTotal=balance;
			stmt.InsEst=insEst;
			//-- MetaData
			stmt.Mode_=StatementMode.Electronic;
			stmt.SmsSendStatus=AutoCommStatus.SendNotAttempted;
			if(prefName==PrefName.TextPaymentLinkAppointmentBalance && saveToDB) {
				return stmt;
			}
			stmt.IsSent=false;
			stmt.IsNew=true;
			stmt.HidePayment=false;
			stmt.DocNum=0;
			stmt.StatementType=StmtType.NotSet;
			//-- Date info
			stmt.DateRangeFrom=DateTime.Today.AddDays(-PrefC.GetLong(PrefName.BillingDefaultsLastDays));//Taken from FormBillingOptions.SetDefaults()
			stmt.DateRangeTo=DateTime.Today;
			stmt.DateSent=DateTime.Today;
			//-- Pat info
			stmt.PatNum=_patCur.PatNum;
			stmt.SinglePatient=!checkBalForFam.Checked;
			//-- Notes
			stmt.Note="";
			stmt.NoteBold="";
			if(prefName==PrefName.TextPaymentLinkAppointmentBalance) {
				//--Linked production
				stmt.ListProcNums=listProcNumsForAppt;
				stmt.ListAdjNums=listAdjNumsForAppt;
				stmt.ListInsPayClaimNums=listClaimPayNumsForAppt;
				stmt.ListPaySplitNums=listPaymentNumsForAppt;
			}
			if(saveToDB) {
				stmt.StatementNum=Statements.Insert(stmt);
			}
			return stmt;
		}

		///<summary>Reserves a ShortGUID and updates the statement ShortGUID, StatementShortURL, and StatementURL fields. Stores changes in DB.</summary>
		private Statement BindGuidToStatement(Statement stmt) {
			List<WebServiceMainHQProxy.ShortGuidResult> guid=new List<WebServiceMainHQProxy.ShortGuidResult>();
			try {
				guid=WebServiceMainHQProxy.GetShortGUIDs(1,1,_apptCur.ClinicNum,eServiceCode.PatientPortalViewStatement);
			}
			catch(Exception ex) {
				FriendlyException.Show(Lans.g(this,"Unable to create a unique URL for each statement. The Patient Portal URL will be used instead."),ex);
				guid=new List<WebServiceMainHQProxy.ShortGuidResult>{ new WebServiceMainHQProxy.ShortGuidResult {
					MediumURL=PrefC.GetString(PrefName.PatientPortalURL),
					ShortURL=PrefC.GetString(PrefName.PatientPortalURL),
					ShortGuid=""
				}};
			}
			stmt.ShortGUID=guid.First().ShortGuid;
			stmt.StatementShortURL=guid.First().ShortURL;
			stmt.StatementURL=guid.First().MediumURL;
			Statements.Update(stmt);
			return stmt;
		}

		///<summary>Builds a PDF of the current statement object. If isPreview is true, the PDF will be saved to the temp/OpenDental path for viewing, and then deleted.
		///Otherwise, the PDF will be saved to the AtoZ folder (or the cloud for web) and the document number will be set on the statement.</summary>
		private Statement BindPDFToStatement(Statement stmt,bool isPreview=false) {
			DataSet stmtData=AccountModules.GetAccount(stmt.PatNum,stmt,isComputeAging:false,doIncludePatLName:true);
			SheetDef sheetDef=SheetUtil.GetStatementSheetDef(stmt);
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,stmt.PatNum,StmtCur.HidePayment);
			SheetFiller.FillFields(sheet,stmtData,stmt);
			SheetUtil.CalculateHeights(sheet,stmtData,stmt,true);
			sheet.Parameters.Add(new SheetParameter(true,"Statement") { ParamValue=stmt });
			string filePath=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),stmt.PatNum.ToString()+".pdf");
			SheetPrinting.CreatePdf(sheet,filePath,stmt,stmtData,null);
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
				return stmt;
			}
			List<Def> listImageCatDefs=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			long category=0;
			for(int i=0;i<listImageCatDefs.Count;i++) {
				if(Regex.IsMatch(listImageCatDefs[i].ItemValue,@"S")) {
					category=listImageCatDefs[i].DefNum;
					break;
				}
			}
			if(category==0) {
				category=listImageCatDefs[0].DefNum;//put it in the first category.
			}
			Document doc=null;
			try {
				//This will store the document in the patient folder, we'll delete the temp folder later.
				doc=ImageStore.Import(filePath,category,_patCur);
			}
			catch {
				MsgBox.Show(this,"Error saving document.");
				return stmt;
			}
			doc.ImgType=ImageType.Document;
			doc.Description=Lan.g(this,"Statement");
			doc.DateCreated=DateTime.Today;
		  stmt.DocNum=doc.DocNum;
			Documents.Update(doc);
			return stmt;
		}
		#endregion
		#region Event Handlers - UI

		private void butPreview_Click(object sender,EventArgs e) {
			Statement stmt=GetStatement(true);
			BindPDFToStatement(stmt,true);
			try {
				Statements.DeleteStatements(new List<Statement>{ stmt },true);
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
			Statement stmt=GetStatement(true);
			stmt=BindGuidToStatement(stmt);
			stmt=BindPDFToStatement(stmt);
			if(stmt.DocNum==0) {
				MsgBox.Show("There was an error creating the document to send");
				DialogResult=DialogResult.Cancel;
				return;
			}
			Statements.Update(stmt);
			Message=Statements.ReplaceVarsForSms(_clinicPrefHelper.GetStringVal(GetSelectedTemplate(),_selectedClinicNum),_patCur,stmt,_clinic,checkIncludeInsurance.Checked);
			StmtCur=stmt;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
		#endregion
	}
}