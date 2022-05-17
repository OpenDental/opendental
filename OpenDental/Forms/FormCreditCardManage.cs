using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Text.RegularExpressions;
using OpenDental.Bridges;
using CodeBase;
using System.Text;

namespace OpenDental {
	public partial class FormCreditCardManage:FormODBase {
		private Patient _patient;
		private List<CreditCard> _listCreditCards;

		public FormCreditCardManage(Patient patient) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patient=patient;
		}
		
		private void FormCreditCardManage_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new GridColumn("Card Number",140));
			if(PrefC.HasClinicsEnabled) {
				gridMain.Columns.Add(new GridColumn("Clinic",80,HorizontalAlignment.Center));
			}
			gridMain.Columns.Add(new GridColumn("Start Date",80,HorizontalAlignment.Center));
			gridMain.Columns.Add(new GridColumn("Stop Date",80,HorizontalAlignment.Center));
			gridMain.Columns.Add(new GridColumn("Amount",80));
			gridMain.Columns.Add(new GridColumn("Charge Frequency",140));
			gridMain.Columns.Add(new GridColumn("Pay Plan",70,HorizontalAlignment.Center));
			if(Programs.IsEnabled(ProgramName.EdgeExpress)) {
				gridMain.Columns.Add(new GridColumn("EdgeExpress",90,HorizontalAlignment.Center));
			}
			if(Programs.IsEnabled(ProgramName.Xcharge)) {
				gridMain.Columns.Add(new GridColumn("X-Charge",70,HorizontalAlignment.Center));
			}
			if(Programs.IsEnabled(ProgramName.PayConnect)) {
				gridMain.Columns.Add(new GridColumn("PayConnect",85,HorizontalAlignment.Center));
			}
			if(Programs.IsEnabled(ProgramName.PaySimple)) {
				gridMain.Columns.Add(new GridColumn("PaySimple",80,HorizontalAlignment.Center));
				gridMain.Columns.Add(new GridColumn("ACH",40,HorizontalAlignment.Center));
			}
			if(PrefC.HasOnlinePaymentEnabled(out ProgramName programNameForPayments)) {
				if(programNameForPayments.In(ProgramName.Xcharge,ProgramName.EdgeExpress)) {
					gridMain.Columns.Add(new GridColumn("XWeb",45,HorizontalAlignment.Center));
				}
				else {
					gridMain.Columns.Add(new GridColumn("PayConnect\r\nPortal",85,HorizontalAlignment.Center));
				}
			}
			gridMain.Columns.Add(new GridColumn("Recurring\r\nActive",70,HorizontalAlignment.Center));
			gridMain.HScrollVisible=false;
			gridMain.Columns[gridMain.Columns.Count-1].IsWidthDynamic=true;
			if(gridMain.Columns.Sum(x => x.ColWidth) > gridMain.Width) {
				gridMain.Columns[gridMain.Columns.Count-1].IsWidthDynamic=false;
				gridMain.HScrollVisible=true;
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			_listCreditCards=CreditCards.Refresh(_patient.PatNum);
			for(int i=0;i<_listCreditCards.Count;i++) {
				CreditCard creditCard=_listCreditCards[i];
				//_listCreditCards is filled / displayed in descending order. 
				if(creditCard.ItemOrder!=(_listCreditCards.Count-1-i)) {
					creditCard.ItemOrder=(_listCreditCards.Count-1-i);
					CreditCards.Update(creditCard);
				}
				row=new GridRow();
				string strCardNum=creditCard.CCNumberMasked;
				if(Regex.IsMatch(strCardNum,"^\\d{12}(\\d{0,7})")) {	//Credit cards can have a minimum of 12 digits, maximum of 19
					int idxLast4Digits=(strCardNum.Length-4);
					strCardNum=(new string('X',12))+strCardNum.Substring(idxLast4Digits);//replace the first 12 with 12 X's
				}
				row.Cells.Add(strCardNum);//1: Card Number
				if(PrefC.HasClinicsEnabled) {
					string abbrCardClinic=Clinics.GetAbbr(creditCard.ClinicNum);
					row.Cells.Add(abbrCardClinic);//2: Clinic Abbreviation
				}
				//Add Start Date for recurring charge
				if(!CreditCards.IsRecurring(creditCard)) {
					row.Cells.Add("");//3: Start Date
					row.Cells.Add("");//4: Stop Date
					row.Cells.Add("");//5: Charge amount
					row.Cells.Add("");//6: Charge frequency
					row.Cells.Add("");//7: PayPlam
				}
				else {
					row.Cells.Add(creditCard.DateStart.ToShortDateString());//2: Start Date
					if(creditCard.DateStop.Year<1880) {
						row.Cells.Add("");//4: Stop Date
					}
					else {
						row.Cells.Add(creditCard.DateStop.ToShortDateString());//4: Stop Date
					}
					row.Cells.Add(creditCard.ChargeAmt.ToString());//5: Charge amount
					row.Cells.Add(CreditCards.GetHumanReadableFrequency(creditCard.ChargeFrequency));//6: Charge frequency
					row.Cells.Add(creditCard.PayPlanNum!=0 ? "X" : "");//7: PayPlan
				}
				if(Programs.IsEnabled(ProgramName.EdgeExpress)) {
					if(!string.IsNullOrEmpty(creditCard.XChargeToken) 
						&& (creditCard.CCSource==CreditCardSource.EdgeExpressCNP || creditCard.CCSource==CreditCardSource.EdgeExpressRCM)) 
					{
						row.Cells.Add("X");
					}
					else {
						row.Cells.Add("");
					}
				}
				if(Programs.IsEnabled(ProgramName.Xcharge)) {
					if(!string.IsNullOrEmpty(creditCard.XChargeToken) && !creditCard.IsXWeb() && creditCard.CCSource==CreditCardSource.XServer) {
						row.Cells.Add("X");
					}
					else {
						row.Cells.Add("");
					}
				}
				if(Programs.IsEnabled(ProgramName.PayConnect)) {
					row.Cells.Add(!string.IsNullOrEmpty(creditCard.PayConnectToken) && !creditCard.IsPayConnectPortal() ? "X" : "");
				}
				if(Programs.IsEnabled(ProgramName.PaySimple)) {
					row.Cells.Add(!string.IsNullOrEmpty(creditCard.PaySimpleToken) ? "X" : "");
					row.Cells.Add(creditCard.CCSource==CreditCardSource.PaySimpleACH ? "X" : "");
				}
				if(PrefC.HasOnlinePaymentEnabled(out programNameForPayments)) {
					if(programNameForPayments.In(ProgramName.Xcharge,ProgramName.EdgeExpress)) {
						row.Cells.Add(!string.IsNullOrEmpty(creditCard.XChargeToken) && creditCard.IsXWeb() ? "X" : "");
					}
					else {//PayConnectPortal
						row.Cells.Add(!string.IsNullOrEmpty(creditCard.PayConnectToken) && creditCard.IsPayConnectPortal() ? "X" : "");
					}
				}
				row.Cells.Add(creditCard.IsRecurringActive?"X":"");
				row.Tag=creditCard;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormCreditCardEdit formCreditCardEdit=new FormCreditCardEdit(_patient);
			formCreditCardEdit.CreditCardCur=(CreditCard)gridMain.ListGridRows[e.Row].Tag;
			formCreditCardEdit.ShowDialog();
			FillGrid();
			if(gridMain.ListGridRows.Count>0) {//could have deleted the only CC, make sure there's at least one row
				int idxCreditCard=gridMain.ListGridRows.OfType<GridRow>().ToList().FindIndex(x => ((CreditCard)x.Tag).CreditCardNum==formCreditCardEdit.CreditCardCur.CreditCardNum);
				gridMain.SetSelected(Math.Max(0,idxCreditCard),true);
			}
		}

		private void butReuseCard_Click(object sender,EventArgs e) {
			int selectedIndex=gridMain.GetSelectedIndex();
			if(selectedIndex==-1) {//If None Selected call butAddCard
				MsgBox.Show("Please select a credit card to reuse.");
				return;
			}
			CreditCard creditCard=(CreditCard)gridMain.ListGridRows[selectedIndex].Tag;
			bool isRecurring=CreditCards.IsRecurring(creditCard);
			FormCreditCardEdit formCreditCardEdit;
			if(isRecurring) {//If selected HAS recurring charge, assign to FormCreditCardEdit.CreditCardCur, and create new card
				formCreditCardEdit=new FormCreditCardEdit(_patient,true);
				//Clear out authorized charge values
				CreditCard creditCardTemp=creditCardTemp=creditCard.Copy();
				creditCardTemp.PayPlanNum=0;
				creditCardTemp.ChargeAmt=0;
				creditCardTemp.DateStart=DateTime.MinValue;
				creditCardTemp.DateStop=DateTime.MinValue;
				creditCardTemp.ChargeFrequency="";
				creditCardTemp.ItemOrder=_listCreditCards.Max(x => x.ItemOrder)+1;
				formCreditCardEdit.CreditCardCur=creditCardTemp;
			}
			else {//If selected DOES NOT have recurring charge, act like the cell was double clicked
				//If the card is not a recurring charge, the card will be updated to have a recurring charge. FormCreditCardEdit will be called with isDuplicate set to false.
				formCreditCardEdit=new FormCreditCardEdit(_patient);
				formCreditCardEdit.CreditCardCur=creditCard;
			}
			formCreditCardEdit.ShowDialog();
			FillGrid();
			if(gridMain.ListGridRows.Count>0) {//could have deleted the only CC, make sure there's at least one row
				int idxCreditCard=gridMain.ListGridRows.OfType<GridRow>().ToList().FindIndex(x => ((CreditCard)x.Tag).CreditCardNum==formCreditCardEdit.CreditCardCur.CreditCardNum);
				gridMain.SetSelected(Math.Max(0,idxCreditCard),true);
			}
			formCreditCardEdit.Dispose();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(PrefC.GetBool(PrefName.StoreCCnumbers)) {
				return;
			}
			bool hasEdgeExpress=false;
			bool hasXCharge=false;
			bool hasPayConnect=false;
			bool hasPaySimple=false;
			Dictionary<string,int> dictionaryEnabledProcessors=new Dictionary<string,int>();
			int idx=0;
			StringBuilder stringBuilder=new StringBuilder();
			#region check if pay prog is enabled
			bool isEnabled(ProgramName programName,string preventPropName) {
				bool hasPreventCreditCardAdd=PIn.Bool(ProgramProperties.GetPropVal(Programs.GetCur(programName).ProgramNum,preventPropName,Clinics.ClinicNum));
				string errMsg="";
				bool isPayProgEnabled=Programs.IsEnabled(programName) && !hasPreventCreditCardAdd && Programs.IsEnabledByHq(programName,out errMsg);
				if(!isPayProgEnabled && !string.IsNullOrWhiteSpace(errMsg)) {
					//Disabled by HQ.
					stringBuilder.AppendLine(errMsg);
				}
				return isPayProgEnabled;
			}
			#endregion
			if(isEnabled(ProgramName.EdgeExpress,ProgramProperties.PropertyDescs.EdgeExpress.PreventSavingNewCC)) {
				dictionaryEnabledProcessors["EdgeExpress"]=idx++;
				hasEdgeExpress=true;
			}
			if(isEnabled(ProgramName.Xcharge,ProgramProperties.PropertyDescs.XCharge.XChargePreventSavingNewCC)) {
				dictionaryEnabledProcessors["X-Charge"]=idx++;
				hasXCharge=true;
			}
			if(isEnabled(ProgramName.PayConnect,PayConnect.ProgramProperties.PayConnectPreventSavingNewCC)) {
				dictionaryEnabledProcessors["PayConnect"]=idx++;
				hasPayConnect=true;
			}
			if(isEnabled(ProgramName.PaySimple,PaySimple.PropertyDescs.PaySimplePreventSavingNewCC)) {
				dictionaryEnabledProcessors["PaySimple"]=idx++;
				hasPaySimple=true;
			}
			if(dictionaryEnabledProcessors.Count>1) {
				List<string> listCreditCardProcessors=dictionaryEnabledProcessors.Select(x => x.Key).ToList();
				using InputBox inputBox=
					new InputBox(Lan.g(this,"For which credit card processing company would you like to add this card?"),listCreditCardProcessors,true);
				if(inputBox.ShowDialog()==DialogResult.Cancel) {
					return;
				}
				hasEdgeExpress=dictionaryEnabledProcessors.ContainsKey("EdgeExpress") && inputBox.SelectedIndices.Contains(dictionaryEnabledProcessors["EdgeExpress"]);
				hasXCharge=dictionaryEnabledProcessors.ContainsKey("X-Charge") && inputBox.SelectedIndices.Contains(dictionaryEnabledProcessors["X-Charge"]);
				hasPayConnect=dictionaryEnabledProcessors.ContainsKey("PayConnect") && inputBox.SelectedIndices.Contains(dictionaryEnabledProcessors["PayConnect"]);
				hasPaySimple=dictionaryEnabledProcessors.ContainsKey("PaySimple") && inputBox.SelectedIndices.Contains(dictionaryEnabledProcessors["PaySimple"]);
			}
			else if(dictionaryEnabledProcessors.Count==0){//not storing CC numbers and both PayConnect and X-Charge are disabled
				string errMsg=stringBuilder.ToString();
				if(string.IsNullOrWhiteSpace(errMsg)) {
					//When an enabled bridge was disabled by HQ, there will be a custom error message, otherwise, use the old generic error message which means that
					//all bridges were disabled by the dental office (not HQ).
					errMsg=Lan.g(this,"Not allowed to store credit cards.");
				}
				MessageBox.Show(this,errMsg);
				return;
			}
			CreditCard creditCard=null;
			if(hasEdgeExpress) {
				long programNum=Programs.GetProgramNum(ProgramName.EdgeExpress);
				//In FormEdgeExpressSetup, if one of these program properties is filled, the form will require the other two to be filled also.
				bool hasCredentials=ProgramProperties.GetPropVal(programNum,ProgramProperties.PropertyDescs.EdgeExpress.XWebID,Clinics.ClinicNum)!=""
					&& ProgramProperties.GetPropVal(programNum,ProgramProperties.PropertyDescs.EdgeExpress.TerminalID,Clinics.ClinicNum)!=""
					&& ProgramProperties.GetPropVal(programNum,ProgramProperties.PropertyDescs.EdgeExpress.AuthKey,Clinics.ClinicNum)!="";
				if(!hasCredentials) {
					MsgBox.Show(this,"EdgeExpress is not set up properly for the current clinic. Fix this and try again.");
					return;
				}
				bool doUseRCM=MsgBox.Show("EdgeExpress",MsgBoxButtons.YesNo,"Would you like to add this card via the terminal?\r\n"
					+"No will require the card information be typed manually.");
				Cursor=Cursors.WaitCursor;
				try {
					if(doUseRCM) {
						EdgeExpress.RcmResponse rcmResponse=EdgeExpress.RCM.CreateAlias(_patient,Clinics.ClinicNum,isWebPayment:false);
						Cursor=Cursors.Default;
						if(rcmResponse.ALIAS.IsNullOrEmpty()) {
							throw new ODException("Alias from EdgeExpress is empty.");
						}
						creditCard=CreditCards.CreateNewOpenEdgeCard(_patient.PatNum,Clinics.ClinicNum,rcmResponse.ALIAS,rcmResponse.EXPMONTH,rcmResponse.EXPYEAR,
							rcmResponse.ACCOUNT,CreditCardSource.EdgeExpressRCM);
					}
					//Card Not Present API
					else {
						XWebResponse xWebResponse=EdgeExpress.CNP.GetUrlForCreditCardAlias(_patient.PatNum,CreditCardSource.EdgeExpressCNP,isWebPayment:false,doUseCurrentClinicNum:true);
						Cursor=Cursors.Default;
						FormWebBrowser formWebBrowser=new FormWebBrowser(xWebResponse.HpfUrl);
						formWebBrowser.ShowDialog();
						//Process without creating a new payment.
						XWebResponse xWebResponseProcessed=EdgeExpress.CNP.ProcessTransaction(xWebResponse,null,doCreatePayment:false);
						if(!xWebResponseProcessed.TransactionStatus.In(
								XWebTransactionStatus.EdgeExpressCompleteAliasCreated,
								XWebTransactionStatus.EdgeExpressCompletePaymentApproved))
						{
							throw new ODException(xWebResponseProcessed.DebugError);
						}
					}
				}
				catch(Exception ex) {
					Cursor=Cursors.Default;
					FriendlyException.Show(Lans.g(this,"There was a problem adding the credit card.  Please try again."),ex);
				}
			}
			if(hasXCharge) {
				if(ODBuild.IsWeb()) {
					MsgBox.Show(this,"XCharge is not available while viewing through the web.");
					return;
				}
				Program program=Programs.GetCur(ProgramName.Xcharge);
				string path=Programs.GetProgramPath(program);
				string xUsername=ProgramProperties.GetPropVal(program.ProgramNum,"Username",Clinics.ClinicNum).Trim();
				string xPassword=ProgramProperties.GetPropVal(program.ProgramNum,"Password",Clinics.ClinicNum).Trim();
				//Force user to retry entering information until it's correct or they press cancel
				while(!File.Exists(path) || string.IsNullOrEmpty(xPassword) || string.IsNullOrEmpty(xUsername)) {
					MsgBox.Show(this,"The Path, Username, and/or Password for X-Charge have not been set or are invalid.");
					if(!Security.IsAuthorized(Permissions.Setup)) {
						return;
					}
					using FormXchargeSetup formXchargeSetup=new FormXchargeSetup();//refreshes program and program property caches on OK click
					formXchargeSetup.ShowDialog();
					if(formXchargeSetup.DialogResult!=DialogResult.OK) {//if user presses cancel, return
						return;
					}
					program=Programs.GetCur(ProgramName.Xcharge);//refresh local variable prog to reflect any changes made in setup window
					path=Programs.GetProgramPath(program);
					xUsername=ProgramProperties.GetPropVal(program.ProgramNum,"Username",Clinics.ClinicNum).Trim();
					xPassword=ProgramProperties.GetPropVal(program.ProgramNum,"Password",Clinics.ClinicNum).Trim();
				}
				xPassword=CodeBase.MiscUtils.Decrypt(xPassword);
				ProcessStartInfo processStartInfo=new ProcessStartInfo(path);
				string resultFile=PrefC.GetRandomTempFile("txt");
				try {
						File.Delete(resultFile);//delete the old result file.					
				}
				catch {
					MsgBox.Show(this,"Could not delete XResult.txt file.  It may be in use by another program, flagged as read-only, or you might not have sufficient permissions.");
					return;
				}
				processStartInfo.Arguments="";
				processStartInfo.Arguments+="/TRANSACTIONTYPE:ArchiveVaultAdd /LOCKTRANTYPE ";
				processStartInfo.Arguments+="/RESULTFILE:\""+resultFile+"\" ";
				processStartInfo.Arguments+="/USERID:"+xUsername+" ";
				processStartInfo.Arguments+="/PASSWORD:"+xPassword+" ";
				processStartInfo.Arguments+="/VALIDATEARCHIVEVAULTACCOUNT ";
				processStartInfo.Arguments+="/STAYONTOP ";
				processStartInfo.Arguments+="/SMARTAUTOPROCESS ";
				processStartInfo.Arguments+="/AUTOCLOSE ";
				processStartInfo.Arguments+="/HIDEMAINWINDOW ";
				processStartInfo.Arguments+="/SMALLWINDOW ";
				processStartInfo.Arguments+="/NORESULTDIALOG ";
				processStartInfo.Arguments+="/TOOLBAREXITBUTTON ";
				Cursor=Cursors.WaitCursor;
				Process process=new Process();
				process.StartInfo=processStartInfo;
				process.EnableRaisingEvents=true;
				process.Start();
				process.WaitForExit();
				Thread.Sleep(200);//Wait 2/10 second to give time for file to be created.
				Cursor=Cursors.Default;
				string resultMsg="";
				string line="";
				string xChargeToken="";
				string accountMasked="";
				string exp="";;
				bool insertCard=false;
				using TextReader textReader=new StreamReader(resultFile);
				try {
					line=textReader.ReadLine();
					while(line!=null) {
						if(resultMsg!="") {
							resultMsg+="\r\n";
						}
						resultMsg+=line;
						if(line.StartsWith("RESULT=")) {
							if(line!="RESULT=SUCCESS") {
								throw new Exception("X-Charge result was not success.");
							}
							insertCard=true;
						}
						if(line.StartsWith("XCACCOUNTID=")) {
							xChargeToken=PIn.String(line.Substring(12));
						}
						if(line.StartsWith("ACCOUNT=")) {
							accountMasked=PIn.String(line.Substring(8));
						}
						if(line.StartsWith("EXPIRATION=")) {
							exp=PIn.String(line.Substring(11));
						}
						line=textReader.ReadLine();
					}
					if(insertCard && xChargeToken!="") {//Might not be necessary but we've had successful charges with no tokens returned before.
						creditCard=CreditCards.CreateNewOpenEdgeCard(_patient.PatNum,Clinics.ClinicNum,xChargeToken,exp.Substring(0,2),exp.Substring(2,2),
							accountMasked,CreditCardSource.XServer);
					}
				}
				catch(Exception ex) {
					string errMsg=Lans.g(this,"There was a problem adding the credit card.  Please try again.");
					if(ex.Message=="X-Charge result was not success.") {
						MessageBox.Show(errMsg);
					}
					else {
						FriendlyException.Show(errMsg,ex);
					}
				}
			}
			if(hasPayConnect) {
				using FormPayConnect formPayConnect=new FormPayConnect(Clinics.ClinicNum,_patient,(decimal)0.01,creditCard,isAddingCard:true);
				formPayConnect.ShowDialog();
			}
			if(hasPaySimple) {
				using FormPaySimple formPaySimple=new FormPaySimple(Clinics.ClinicNum,_patient,(decimal)0.01,creditCard,isAddingCard:true);
				formPaySimple.ShowDialog();
			}
			FillGrid();
			if(gridMain.ListGridRows.Count>0 && creditCard!=null) {
				gridMain.SetSelected(gridMain.ListGridRows.Count-1,true);
			}
			return;
		}

		private void butMoveTo_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()<0) {
				MsgBox.Show(this,"Please select a card first.");
				return;
			}
			// block users from moving cards with recurring charges
			if(CreditCards.IsRecurring(_listCreditCards[gridMain.GetSelectedIndex()])) {
				MsgBox.Show(Lan.g(this, "Cannot move a credit card with active recurring charges. Please clear recurring charges before moving the card to another patient."));
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Move this credit card information to a different patient account?")) {
				return;
			}
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			if(formPatientSelect.ShowDialog()!=DialogResult.OK) {
				return;
			}
			int selectedIndex=gridMain.GetSelectedIndex();
			CreditCard creditCard=_listCreditCards[selectedIndex];
			long patNum=creditCard.PatNum;
			creditCard.PatNum=formPatientSelect.SelectedPatNum;
			creditCard.ItemOrder=CreditCards.GetMaxItemOrderForPat(formPatientSelect.SelectedPatNum)+1;
			CreditCards.Update(creditCard);
			FillGrid();
			MsgBox.Show(this,"Credit card moved successfully");
			SecurityLogs.MakeLogEntry(Permissions.CreditCardMove,patNum,$"Credit card moved to PatNum: {formPatientSelect.SelectedPatNum}");
			SecurityLogs.MakeLogEntry(Permissions.CreditCardMove,formPatientSelect.SelectedPatNum,$"Credit card moved from PatNum: {patNum}");
		}

		private void butUp_Click(object sender,EventArgs e) {
			int selectedIndex=gridMain.GetSelectedIndex();
			if(selectedIndex==-1) {
				MsgBox.Show(this,"Please select a card first.");
				return;
			}
			if(selectedIndex==0) {
				return;//can't move up any more
			}
			int idxOld;
			int idxNew;
			CreditCard creditCardOld;
			CreditCard creditCardNew;
			idxOld=_listCreditCards[selectedIndex].ItemOrder;
			idxNew=idxOld+1; 
			for(int i=0;i<_listCreditCards.Count;i++) {
				if(_listCreditCards[i].ItemOrder==idxOld) {
					creditCardOld=_listCreditCards[i];
					creditCardNew=_listCreditCards[i-1];
					creditCardOld.ItemOrder=creditCardNew.ItemOrder;
					creditCardNew.ItemOrder-=1;
					CreditCards.Update(creditCardOld);
					CreditCards.Update(creditCardNew);
				}
			}
			FillGrid();
			gridMain.SetSelected(selectedIndex-1,true);
		}

		private void butDown_Click(object sender,EventArgs e) {
			int selectedIndex=gridMain.GetSelectedIndex();
			if(selectedIndex==-1) {
				MsgBox.Show(this,"Please select a card first.");
				return;
			}
			if(selectedIndex==_listCreditCards.Count-1) {
				return;//can't move down any more
			}
			int idxOld;
			int idxNew;
			CreditCard creditCardOld;
			CreditCard creditCardNew;
			idxOld=_listCreditCards[selectedIndex].ItemOrder;
			idxNew=idxOld-1;
			for(int i=0;i<_listCreditCards.Count;i++) {
				if(_listCreditCards[i].ItemOrder==idxNew) {
					creditCardNew=_listCreditCards[i];
					creditCardOld=_listCreditCards[i-1];
					creditCardNew.ItemOrder=creditCardOld.ItemOrder;
					creditCardOld.ItemOrder-=1;
					CreditCards.Update(creditCardOld);
					CreditCards.Update(creditCardNew);
				}
			}
			FillGrid();
			gridMain.SetSelected(selectedIndex+1,true);
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}