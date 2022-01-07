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
		private Patient PatCur;
		private List<CreditCard> _listCreditCards;

		public FormCreditCardManage(Patient pat) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			PatCur=pat;
		}
		
		private void FormCreditCardManage_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn("Card Number",140));
			if(PrefC.HasClinicsEnabled) {
				gridMain.ListGridColumns.Add(new GridColumn("Clinic",80,HorizontalAlignment.Center));
			}
			gridMain.ListGridColumns.Add(new GridColumn("Start Date",80,HorizontalAlignment.Center));
			gridMain.ListGridColumns.Add(new GridColumn("Stop Date",80,HorizontalAlignment.Center));
			gridMain.ListGridColumns.Add(new GridColumn("Amount",80));
			gridMain.ListGridColumns.Add(new GridColumn("Charge Frequency",140));
			gridMain.ListGridColumns.Add(new GridColumn("Pay Plan",70,HorizontalAlignment.Center));
			if(Programs.IsEnabled(ProgramName.EdgeExpress)) {
				gridMain.ListGridColumns.Add(new GridColumn("EdgeExpress",90,HorizontalAlignment.Center));
			}
			else if(Programs.IsEnabled(ProgramName.Xcharge)) {
				gridMain.ListGridColumns.Add(new GridColumn("X-Charge",70,HorizontalAlignment.Center));
			}
			if(Programs.IsEnabled(ProgramName.PayConnect)) {
				gridMain.ListGridColumns.Add(new GridColumn("PayConnect",85,HorizontalAlignment.Center));
			}
			if(Programs.IsEnabled(ProgramName.PaySimple)) {
				gridMain.ListGridColumns.Add(new GridColumn("PaySimple",80,HorizontalAlignment.Center));
				gridMain.ListGridColumns.Add(new GridColumn("ACH",40,HorizontalAlignment.Center));
			}
			if(PrefC.HasOnlinePaymentEnabled(out ProgramName progNameForPayments)) {
				if(ListTools.In(progNameForPayments,ProgramName.Xcharge,ProgramName.EdgeExpress)) {
					gridMain.ListGridColumns.Add(new GridColumn("XWeb",45,HorizontalAlignment.Center));
				}
				else {
					gridMain.ListGridColumns.Add(new GridColumn("PayConnect\r\nPortal",85,HorizontalAlignment.Center));
				}
			}
			gridMain.ListGridColumns.Add(new GridColumn("Recurring\r\nActive",70,HorizontalAlignment.Center));
			gridMain.HScrollVisible=false;
			gridMain.ListGridColumns[gridMain.ListGridColumns.Count-1].IsWidthDynamic=true;
			if(gridMain.ListGridColumns.Sum(x => x.ColWidth) > gridMain.Width) {
				gridMain.ListGridColumns[gridMain.ListGridColumns.Count-1].IsWidthDynamic=false;
				gridMain.HScrollVisible=true;
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			_listCreditCards=CreditCards.Refresh(PatCur.PatNum);
			for(int i=0;i<_listCreditCards.Count;i++) {
				CreditCard cc=_listCreditCards[i];
				//_listCreditCards is filled / displayed in descending order. 
				if(cc.ItemOrder!=(_listCreditCards.Count-1-i)) {
					cc.ItemOrder=(_listCreditCards.Count-1-i);
					CreditCards.Update(cc);
				}
				row=new GridRow();
				string ccNum=cc.CCNumberMasked;
				if(Regex.IsMatch(ccNum,"^\\d{12}(\\d{0,7})")) {	//Credit cards can have a minimum of 12 digits, maximum of 19
					int idxLast4Digits=(ccNum.Length-4);
					ccNum=(new string('X',12))+ccNum.Substring(idxLast4Digits);//replace the first 12 with 12 X's
				}
				row.Cells.Add(ccNum);//1: Card Number
				if(PrefC.HasClinicsEnabled) {
					string cardClinicAbbr=Clinics.GetAbbr(cc.ClinicNum);
					row.Cells.Add(cardClinicAbbr);//2: Clinic Abbreviation
				}
				//Add Start Date for recurring charge
				if(!CreditCards.IsRecurring(cc)) {
					row.Cells.Add("");//3: Start Date
					row.Cells.Add("");//4: Stop Date
					row.Cells.Add("");//5: Charge amount
					row.Cells.Add("");//6: Charge frequency
					row.Cells.Add("");//7: PayPlam
				}
				else {
					row.Cells.Add(cc.DateStart.ToShortDateString());//2: Start Date
          if(cc.DateStop.Year<1880) {
						row.Cells.Add("");//4: Stop Date
					}
					else {
						row.Cells.Add(cc.DateStop.ToShortDateString());//4: Stop Date
					}
					row.Cells.Add(cc.ChargeAmt.ToString());//5: Charge amount
					row.Cells.Add(CreditCards.GetHumanReadableFrequency(cc.ChargeFrequency));//6: Charge frequency
					row.Cells.Add(cc.PayPlanNum!=0 ? "X" : "");//7: PayPlan
				}
				if(Programs.IsEnabled(ProgramName.EdgeExpress) || Programs.IsEnabled(ProgramName.Xcharge)) {
					row.Cells.Add(!string.IsNullOrEmpty(cc.XChargeToken) && !cc.IsXWeb()?"X":"");
				}
				if(Programs.IsEnabled(ProgramName.PayConnect)) {
					row.Cells.Add(!string.IsNullOrEmpty(cc.PayConnectToken) && !cc.IsPayConnectPortal() ? "X" : "");
				}
				if(Programs.IsEnabled(ProgramName.PaySimple)) {
					row.Cells.Add(!string.IsNullOrEmpty(cc.PaySimpleToken) ? "X" : "");
					row.Cells.Add(cc.CCSource==CreditCardSource.PaySimpleACH ? "X" : "");
				}
				if(PrefC.HasOnlinePaymentEnabled(out progNameForPayments)) {
					if(ListTools.In(progNameForPayments,ProgramName.Xcharge,ProgramName.EdgeExpress)) {
						row.Cells.Add(!string.IsNullOrEmpty(cc.XChargeToken) && cc.IsXWeb() ? "X" : "");
					}
					else {//PayConnectPortal
						row.Cells.Add(!string.IsNullOrEmpty(cc.PayConnectToken) && cc.IsPayConnectPortal() ? "X" : "");
					}
				}
				row.Cells.Add(cc.IsRecurringActive?"X":"");
				row.Tag=cc;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormCreditCardEdit FormCCE=new FormCreditCardEdit(PatCur);
			FormCCE.CreditCardCur=(CreditCard)gridMain.ListGridRows[e.Row].Tag;
			FormCCE.ShowDialog();
			FillGrid();
			if(gridMain.ListGridRows.Count>0) {//could have deleted the only CC, make sure there's at least one row
				int indexCC=gridMain.ListGridRows.OfType<GridRow>().ToList().FindIndex(x => ((CreditCard)x.Tag).CreditCardNum==FormCCE.CreditCardCur.CreditCardNum);
				gridMain.SetSelected(Math.Max(0,indexCC),true);
			}
		}

		private void butReuseCard_Click(object sender,EventArgs e) {
			int selectedIndex=gridMain.GetSelectedIndex();
			if(selectedIndex==-1) {//If None Selected call butAddCard
				MsgBox.Show("Please select a credit card to reuse.");
				return;
			}
			CreditCard selectedCard=(CreditCard)gridMain.ListGridRows[selectedIndex].Tag;
			bool isRecurring=CreditCards.IsRecurring(selectedCard);
			FormCreditCardEdit formCreditCardEdit;
			if(isRecurring) {//If selected HAS recurring charge, assign to FormCreditCardEdit.CreditCardCur, and create new card
				formCreditCardEdit=new FormCreditCardEdit(PatCur,true);
				//Clear out authorized charge values
				CreditCard tempCC=tempCC=selectedCard.Copy();
				tempCC.PayPlanNum=0;
				tempCC.ChargeAmt=0;
				tempCC.DateStart=DateTime.MinValue;
				tempCC.DateStop=DateTime.MinValue;
				tempCC.ChargeFrequency="";
				tempCC.ItemOrder=_listCreditCards.Max(x => x.ItemOrder)+1;
				formCreditCardEdit.CreditCardCur=tempCC;
			}
			else {//If selected DOES NOT have recurring charge, act like the cell was double clicked
				//If the card is not a recurring charge, the card will be updated to have a recurring charge. FormCreditCardEdit will be called with isDuplicate set to false.
				formCreditCardEdit=new FormCreditCardEdit(PatCur);
				formCreditCardEdit.CreditCardCur=selectedCard;
			}
			formCreditCardEdit.ShowDialog();
			FillGrid();
			if(gridMain.ListGridRows.Count>0) {//could have deleted the only CC, make sure there's at least one row
				int indexCC=gridMain.ListGridRows.OfType<GridRow>().ToList().FindIndex(x => ((CreditCard)x.Tag).CreditCardNum==formCreditCardEdit.CreditCardCur.CreditCardNum);
				gridMain.SetSelected(Math.Max(0,indexCC),true);
			}
			formCreditCardEdit.Dispose();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(!PrefC.GetBool(PrefName.StoreCCnumbers)) {
				bool hasEdgeExpress=false;
				bool hasXCharge=false;
				bool hasPayConnect=false;
				bool hasPaySimple=false;
				Dictionary<string,int> dictEnabledProcessors=new Dictionary<string,int>();
				int idx=0;
				StringBuilder err=new StringBuilder();
				#region check if pay prog is enabled
				bool isEnabled(ProgramName progName,string preventPropName) {
					bool hasPreventCcAdd=PIn.Bool(ProgramProperties.GetPropVal(Programs.GetCur(progName).ProgramNum,preventPropName,Clinics.ClinicNum));
					string error="";
					bool retVal=Programs.IsEnabled(progName) && !hasPreventCcAdd && Programs.IsEnabledByHq(progName,out error);
					if(!retVal && !string.IsNullOrWhiteSpace(error)) {
						//Disabled by HQ.
						err.AppendLine(error);
					}
					return retVal;
				}
				#endregion
				if(isEnabled(ProgramName.EdgeExpress,ProgramProperties.PropertyDescs.EdgeExpress.PreventSavingNewCC)) {
					dictEnabledProcessors["EdgeExpress"]=idx++;
					hasEdgeExpress=true;
				}
				if(isEnabled(ProgramName.Xcharge,ProgramProperties.PropertyDescs.XCharge.XChargePreventSavingNewCC)) {
					dictEnabledProcessors["X-Charge"]=idx++;
					hasXCharge=true;
				}
				if(isEnabled(ProgramName.PayConnect,PayConnect.ProgramProperties.PayConnectPreventSavingNewCC)) {
					dictEnabledProcessors["PayConnect"]=idx++;
					hasPayConnect=true;
				}
				if(isEnabled(ProgramName.PaySimple,PaySimple.PropertyDescs.PaySimplePreventSavingNewCC)) {
					dictEnabledProcessors["PaySimple"]=idx++;
					hasPaySimple=true;
				}
				if(dictEnabledProcessors.Count>1) {
					List<string> listCCProcessors=dictEnabledProcessors.Select(x => x.Key).ToList();
					using InputBox chooseProcessor=
						new InputBox(Lan.g(this,"For which credit card processing company would you like to add this card?"),listCCProcessors,true);
					if(chooseProcessor.ShowDialog()==DialogResult.Cancel) {
						return;
					}
					hasEdgeExpress=dictEnabledProcessors.ContainsKey("EdgeExpress") && chooseProcessor.SelectedIndices.Contains(dictEnabledProcessors["EdgeExpress"]);
					hasXCharge=dictEnabledProcessors.ContainsKey("X-Charge") && chooseProcessor.SelectedIndices.Contains(dictEnabledProcessors["X-Charge"]);
					hasPayConnect=dictEnabledProcessors.ContainsKey("PayConnect") && chooseProcessor.SelectedIndices.Contains(dictEnabledProcessors["PayConnect"]);
					hasPaySimple=dictEnabledProcessors.ContainsKey("PaySimple") && chooseProcessor.SelectedIndices.Contains(dictEnabledProcessors["PaySimple"]);
				}
				else if(dictEnabledProcessors.Count==0){//not storing CC numbers and both PayConnect and X-Charge are disabled
					string errMsg=err.ToString();
					if(string.IsNullOrWhiteSpace(errMsg)) {
						//When an enabled bridge was disabled by HQ, there will be a custom error message, otherwise, use the old generic error message which means that
						//all bridges were disabled by the dental office (not HQ).
						errMsg=Lan.g(this,"Not allowed to store credit cards.");
					}
					MessageBox.Show(this,errMsg);
					return;
				}
				CreditCard creditCardCur=null;
				if(hasEdgeExpress) {
					long programNum=Programs.GetProgramNum(ProgramName.EdgeExpress);
					//In FormEdgeExpressSetup, if one of these program properties is filled, the form will require the other two to be filled also.
					bool hasCredentials=ProgramProperties.GetPropVal(programNum,ProgramProperties.PropertyDescs.EdgeExpress.XWebID,Clinics.ClinicNum)!="" &&
						ProgramProperties.GetPropVal(programNum,ProgramProperties.PropertyDescs.EdgeExpress.TerminalID,Clinics.ClinicNum)!="" &&
						ProgramProperties.GetPropVal(programNum,ProgramProperties.PropertyDescs.EdgeExpress.AuthKey,Clinics.ClinicNum)!="";
					if(!hasCredentials) {
						MsgBox.Show(this,"EdgeExpress is not set up properly for the current clinic. Fix this and try again.");
						return;
					}
					bool doUseRCM=MsgBox.Show("EdgeExpress",MsgBoxButtons.YesNo,"Would you like to add this card via the terminal?\r\n"
						+"No will require the card information be typed manually.");
					Cursor=Cursors.WaitCursor;
					try {
						if(doUseRCM) {
							EdgeExpress.RcmResponse response=EdgeExpress.RCM.CreateAlias(PatCur.PatNum,Clinics.ClinicNum,false);
							Cursor=Cursors.Default;
							if(response.ALIAS.IsNullOrEmpty()) {
								throw new ODException("Alias from EdgeExpress is empty.");
							}
							creditCardCur=CreditCards.CreateNewOpenEdgeCard(PatCur.PatNum,Clinics.ClinicNum,response.ALIAS,response.EXPMONTH,response.EXPYEAR,
								response.ACCOUNT,CreditCardSource.EdgeExpressRCM);
						}
						//Card Not Present API
						else {
							XWebResponse responseUrl=EdgeExpress.CNP.GetUrlForCreditCardAlias(PatCur.PatNum,CreditCardSource.EdgeExpressCNP,isWebPayment:false,doUseCurrentClinicNum:true);
							Cursor=Cursors.Default;
							FormWebBrowser formWB=new FormWebBrowser(responseUrl.HpfUrl);
							formWB.ShowDialog();
							//Process without creating a new payment.
							XWebResponse xWebResponseProcessed=EdgeExpress.CNP.ProcessTransaction(responseUrl,null,false);
							if(!ListTools.In(xWebResponseProcessed.TransactionStatus,
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
					Program prog=Programs.GetCur(ProgramName.Xcharge);
					string path=Programs.GetProgramPath(prog);
					string xUsername=ProgramProperties.GetPropVal(prog.ProgramNum,"Username",Clinics.ClinicNum).Trim();
					string xPassword=ProgramProperties.GetPropVal(prog.ProgramNum,"Password",Clinics.ClinicNum).Trim();
					//Force user to retry entering information until it's correct or they press cancel
					while(!File.Exists(path) || string.IsNullOrEmpty(xPassword) || string.IsNullOrEmpty(xUsername)) {
						MsgBox.Show(this,"The Path, Username, and/or Password for X-Charge have not been set or are invalid.");
						if(!Security.IsAuthorized(Permissions.Setup)) {
							return;
						}
						using FormXchargeSetup FormX=new FormXchargeSetup();//refreshes program and program property caches on OK click
						FormX.ShowDialog();
						if(FormX.DialogResult!=DialogResult.OK) {//if user presses cancel, return
							return;
						}
						prog=Programs.GetCur(ProgramName.Xcharge);//refresh local variable prog to reflect any changes made in setup window
						path=Programs.GetProgramPath(prog);
						xUsername=ProgramProperties.GetPropVal(prog.ProgramNum,"Username",Clinics.ClinicNum).Trim();
						xPassword=ProgramProperties.GetPropVal(prog.ProgramNum,"Password",Clinics.ClinicNum).Trim();
					}
					xPassword=CodeBase.MiscUtils.Decrypt(xPassword);
					ProcessStartInfo info=new ProcessStartInfo(path);
					string resultfile=PrefC.GetRandomTempFile("txt");
					try {
						File.Delete(resultfile);//delete the old result file.
					}
					catch {
						MsgBox.Show(this,"Could not delete XResult.txt file.  It may be in use by another program, flagged as read-only, or you might not have sufficient permissions.");
						return;
					}
					info.Arguments="";
					info.Arguments+="/TRANSACTIONTYPE:ArchiveVaultAdd /LOCKTRANTYPE ";
					info.Arguments+="/RESULTFILE:\""+resultfile+"\" ";
					info.Arguments+="/USERID:"+xUsername+" ";
					info.Arguments+="/PASSWORD:"+xPassword+" ";
					info.Arguments+="/VALIDATEARCHIVEVAULTACCOUNT ";
					info.Arguments+="/STAYONTOP ";
					info.Arguments+="/SMARTAUTOPROCESS ";
					info.Arguments+="/AUTOCLOSE ";
					info.Arguments+="/HIDEMAINWINDOW ";
					info.Arguments+="/SMALLWINDOW ";
					info.Arguments+="/NORESULTDIALOG ";
					info.Arguments+="/TOOLBAREXITBUTTON ";
					Cursor=Cursors.WaitCursor;
					Process process=new Process();
					process.StartInfo=info;
					process.EnableRaisingEvents=true;
					process.Start();
					process.WaitForExit();
					Thread.Sleep(200);//Wait 2/10 second to give time for file to be created.
					Cursor=Cursors.Default;
					string resulttext="";
					string line="";
					string xChargeToken="";
					string accountMasked="";
					string exp="";;
					bool insertCard=false;
					try {
						using(TextReader reader=new StreamReader(resultfile)) {
							line=reader.ReadLine();
							while(line!=null) {
								if(resulttext!="") {
									resulttext+="\r\n";
								}
								resulttext+=line;
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
								line=reader.ReadLine();
							}
							if(insertCard && xChargeToken!="") {//Might not be necessary but we've had successful charges with no tokens returned before.
								creditCardCur=CreditCards.CreateNewOpenEdgeCard(PatCur.PatNum,Clinics.ClinicNum,xChargeToken,exp.Substring(0,2),exp.Substring(2,2),
									accountMasked,CreditCardSource.XServer);
							}
						}
					}
					catch(Exception ex) {
						string msg=Lans.g(this,"There was a problem adding the credit card.  Please try again.");
						if(ex.Message=="X-Charge result was not success.") {
							MessageBox.Show(msg);
						}
						else {
							FriendlyException.Show(msg,ex);
						}
					}
				}
				if(hasPayConnect) {
					using FormPayConnect FormPC=new FormPayConnect(Clinics.ClinicNum,PatCur,(decimal)0.01,creditCardCur,true);
					FormPC.ShowDialog();
				}
				if(hasPaySimple) {
					using FormPaySimple formPS=new FormPaySimple(Clinics.ClinicNum,PatCur,(decimal)0.01,creditCardCur,true);
					formPS.ShowDialog();
				}
				FillGrid();
				if(gridMain.ListGridRows.Count>0 && creditCardCur!=null) {
					gridMain.SetSelected(gridMain.ListGridRows.Count-1,true);
				}
				return;
			}
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
			using FormPatientSelect form=new FormPatientSelect();
			if(form.ShowDialog()!=DialogResult.OK) {
				return;
			}
			int selected=gridMain.GetSelectedIndex();
			CreditCard creditCard=_listCreditCards[selected];
			long patNumOrig=creditCard.PatNum;
			creditCard.PatNum=form.SelectedPatNum;
			creditCard.ItemOrder=CreditCards.GetMaxItemOrderForPat(form.SelectedPatNum)+1;
			CreditCards.Update(creditCard);
			FillGrid();
			MsgBox.Show(this,"Credit card moved successfully");
			SecurityLogs.MakeLogEntry(Permissions.CreditCardMove,patNumOrig,$"Credit card moved to PatNum: {form.SelectedPatNum}");
			SecurityLogs.MakeLogEntry(Permissions.CreditCardMove,form.SelectedPatNum,$"Credit card moved from PatNum: {patNumOrig}");
		}

		private void butUp_Click(object sender,EventArgs e) {
			int placement=gridMain.GetSelectedIndex();
			if(placement==-1) {
				MsgBox.Show(this,"Please select a card first.");
				return;
			}
			if(placement==0) {
				return;//can't move up any more
			}
			int oldIdx;
			int newIdx;
			CreditCard oldItem;
			CreditCard newItem;
			oldIdx=_listCreditCards[placement].ItemOrder;
			newIdx=oldIdx+1; 
			for(int i=0;i<_listCreditCards.Count;i++) {
				if(_listCreditCards[i].ItemOrder==oldIdx) {
					oldItem=_listCreditCards[i];
					newItem=_listCreditCards[i-1];
					oldItem.ItemOrder=newItem.ItemOrder;
					newItem.ItemOrder-=1;
					CreditCards.Update(oldItem);
					CreditCards.Update(newItem);
				}
			}
			FillGrid();
			gridMain.SetSelected(placement-1,true);
		}

		private void butDown_Click(object sender,EventArgs e) {
			int placement=gridMain.GetSelectedIndex();
			if(placement==-1) {
				MsgBox.Show(this,"Please select a card first.");
				return;
			}
			if(placement==_listCreditCards.Count-1) {
				return;//can't move down any more
			}
			int oldIdx;
			int newIdx;
			CreditCard oldItem;
			CreditCard newItem;
			oldIdx=_listCreditCards[placement].ItemOrder;
			newIdx=oldIdx-1;
			for(int i=0;i<_listCreditCards.Count;i++) {
				if(_listCreditCards[i].ItemOrder==newIdx) {
					newItem=_listCreditCards[i];
					oldItem=_listCreditCards[i-1];
					newItem.ItemOrder=oldItem.ItemOrder;
					oldItem.ItemOrder-=1;
					CreditCards.Update(oldItem);
					CreditCards.Update(newItem);
				}
			}
			FillGrid();
			gridMain.SetSelected(placement+1,true);
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}